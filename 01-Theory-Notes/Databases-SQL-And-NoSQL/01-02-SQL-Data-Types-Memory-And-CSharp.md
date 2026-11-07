# 💾 01-02. Типове данни, Заемане на памет и Мостът към C#

**Цел:** Точен превод между SQL типове и C#, физическо разпределение на байтовете на диска (Data Pages), Unicode правила (`N'...'`) и Nullable стойности (`int?`).

---

## 📊 1. ПЪЛНА МАТРИЦА: MS SQL ➔ POSTGRESQL ➔ ПАМЕТ ➔ C#

| Категория | MS SQL Server | PostgreSQL | Памет (Bytes) | C# Тип (`NOT NULL`) | C# Тип (`NULL`) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Много малко цяло** | `TINYINT` (0..255) | `SMALLINT` | `1 B` *(MS)* / `2 B` | `byte` | `byte?` |
| **Малко цяло** | `SMALLINT` | `SMALLINT` | `2 B` | `short` | `short?` |
| **Стандартно цяло** | `INT` | `INTEGER` (`INT4`) | `4 B` | `int` | `int?` |
| **Голямо цяло** | `BIGINT` | `BIGINT` (`INT8`) | `8 B` | `long` | `long?` |
| **Пари / Точност** | `DECIMAL(18, 2)` | `NUMERIC(18, 2)` | `5` до `17 B` *(9 B за 18 цифри)* | `decimal` | `decimal?` |
| **Дробно (64-bit)** | `FLOAT` | `DOUBLE PRECISION` | `8 B` | `double` | `double?` |
| **Дробно (32-bit)** | `REAL` | `REAL` | `4 B` | `float` | `float?` |
| **Логическо** | `BIT` | `BOOLEAN` | `1 B` *(до 8 BIT в 1 B)* | `bool` | `bool?` |
| **Фиксиран текст** | `CHAR(n)` / `NCHAR(n)` | `CHAR(n)` | `n B` / `2*n B` | `string` | `string?` |
| **Променлив текст** | `VARCHAR(n)` / `NVARCHAR(n)` | `VARCHAR(n)` / `TEXT` | `L + 2 B` / `2*L + 2 B` | `string` | `string?` |
| **Само дата** | `DATE` | `DATE` | `3 B` *(MS)* / `4 B` | `DateOnly` / `DateTime` | `DateOnly?` |
| **Дата и час (UTC)** | `DATETIME2` | `TIMESTAMP` | `6` до `8 B` | `DateTime` | `DateTime?` |
| **Дата + Часова зона** | `DATETIMEOFFSET` | `TIMESTAMPTZ` | `10 B` *(MS)* / `8 B` | `DateTimeOffset` | `DateTimeOffset?` |
| **Само час** | `TIME` | `TIME` | `3` до `5 B` | `TimeOnly` / `TimeSpan` | `TimeOnly?` |
| **Уникален ID** | `UNIQUEIDENTIFIER` | `UUID` | `16 B` | `Guid` | `Guid?` |
| **Двоични данни** | `VARBINARY(MAX)` | `BYTEA` | `L + 2 B` *(до 2 GB)* | `byte[]` | `byte[]?` |

---

## 🏗️ 2. КАК БАЗАТА ЗАПИСВА ЕДИН РЕД В ПАМЕТТА (DATA PAGES)

Релационните бази съхраняват данните на диска и в RAM буфера (`Buffer Pool`) на блокове от **8 KB (8192 байта)**, наречени страници (Data Pages). Колкото по-малко байтове заема един ред, толкова повече редове влизат в една страница и толкова по-бързо работи `SELECT`.

```text
+---------------+------------------+------------------+------------------------+------------------------+

| Row Header    | Fixed Columns    | Null Bitmap      | Variable Offset Array  | Variable Data          |
| (4 байта)     | (INT=4B, BIT=1B) | (1 бит на колона)| (2 байта на колона)    | (N'Иван' = 8 байта)    |
+---------------+------------------+------------------+------------------------+------------------------+
```

* **Fixed-Length зона:** Колони с постоянен размер (`INT`, `BIGINT`, `BIT`, `DATETIME2`, `CHAR`, `UNIQUEIDENTIFIER`) стоят плътно в началото на реда.
* **Скрита оптимизация за `BIT`:** Ако имаш до 8 колони от тип `BIT` в една таблица (`IsActive`, `IsDeleted`, `IsPaid`...), базата ги пакетира **всичките в 1 единствен байт**.
* **Variable-Length зона:** Текстовете (`VARCHAR`, `NVARCHAR`) стоят в края и пазят 2 байта за реалната си дължина.
## ⚖️ 3. ПРЕДИМСТВА, НЕДОСТАТЪЦИ И КАПАНИ ПРИ ЗАПИС (С ПРИМЕРИ)

### 🔹 А. Текст и Unicode: `CHAR` срещу `VARCHAR` срещу `NVARCHAR`
* **`CHAR(n)` / `NCHAR(n)` (Фиксирана дължина):** Винаги заема точно `n` (или `2*n`) байта, допълвайки с интервали. При `UPDATE` **няма фрагментация (Page Split)**, защото размерът на реда не се променя.
* **`VARCHAR(n)` / `NVARCHAR(n)` (Променлива дължина):** Заема само реално въведените символи + 2 байта. Ако при `UPDATE` удължиш текста (напр. от `''` на 150 символа) и редът не се побере в текущата 8 KB страница, базата прави **Page Split** (разцепва страницата на две), което забавя писането.
* **Unicode префикс `N` в MS SQL Server:** `VARCHAR` пази само 1 байт (ASCII/латиница) и превръща кирилицата в `'????'`. За кирилица, емоджита и международни символи задължително ползвай `NVARCHAR` и префикс `N'...'`. *(В PostgreSQL `VARCHAR` и `TEXT` са UTF-8 по подразбиране).*

```sql
-- ============================================================
-- UNICODE И ДЪЛЖИНА НА ТЕКСТА В MS SQL SERVER
-- ============================================================
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CountryCode CHAR(2) NOT NULL,           -- Винаги 2 байта ('BG', 'US') -> без Page Split
    Email VARCHAR(100) NOT NULL,            -- 1 байт на символ (имейлите са на латиница)
    FullName NVARCHAR(100) NOT NULL,        -- 2 байта на символ (кирилица: N'Елена Петрова')
    Bio NVARCHAR(MAX) NULL                  -- Над 8 KB отива в отделни LOB страници!
);

-- ПРАВИЛНО търсене и запис на кирилица в MS SQL:
INSERT INTO Customers (CountryCode, Email, FullName)
VALUES ('BG', 'elena@example.com', N'Елена Петрова');

SELECT * FROM Customers WHERE FullName = N'Елена Петрова';
```

### 🔹 Б. Пари и числа: `DECIMAL` срещу `FLOAT`
* **`DECIMAL(18, 2)` (9 байта):** Записва десетичните цифри точно. Задължителен за пари, цени и заплати.
* **`FLOAT` (8 байта):** Използва двоична плаваща запетая (IEEE 754) — смята светкавично при графики и сензори, но дава грешки от закръгляне (`0.1 + 0.2 = 0.30000000000000004`).

```sql
-- ============================================================
-- ТОЧНОСТ НА ПАРИТЕ (DECIMAL СРЕЩУ FLOAT)
-- ============================================================
DECLARE @ExactMoney DECIMAL(18, 2) = 0.10 + 0.20; -- Резултат: 0.30 (100% точно)
DECLARE @ApproxFloat FLOAT = 0.10 + 0.20;         -- Резултат: 0.30000000000000004 (ГРЕШКА!)
```

### 🔹 В. Идентификатори: `INT IDENTITY` (4 B) срещу `UNIQUEIDENTIFIER` / `Guid` (16 B)
* **`INT` (4 байта):** Най-бърз за CPU и индексиране.
* **`Guid` (16 байта):** 4 пъти по-голям от `INT` и се копира във **всеки** Non-Clustered индекс. Стандартният произволен `Guid.NewGuid()` вкарва редове по средата на B-Tree индекса и причинява тежки Page Splits (в .NET 9+ ползвай последователния `Guid.CreateVersion7()`).

---

## 💻 4. ПРАКТИЧЕСКИ МОСТ: SQL ТАБЛИЦА ➔ C# ENTITY (И NULLABLE ТИПОВЕ)

Когато една колона в SQL е маркирана с `NULL`, в C# задължително слагаме `?` (`int?`, `decimal?`, `DateTime?`), иначе EF Core / Dapper ще хвърлят грешка при четене на празна стойност.

```sql
-- ============================================================
-- 1. SQL SERVER ТАБЛИЦА
-- ============================================================
CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,       -- 4 B -> int
    FullName NVARCHAR(100) NOT NULL,        -- до 202 B -> string
    Age TINYINT NULL,                       -- 1 B (0..255) -> byte? (Nullable!)
    Grade DECIMAL(3, 2) NOT NULL,           -- 5 B (напр. 5.75) -> decimal
    Scholarship DECIMAL(18, 2) NULL,        -- 9 B -> decimal? (Nullable!)
    IsActive BIT NOT NULL DEFAULT 1,        -- 1 B -> bool
    CreatedAt DATETIME2 NOT NULL            -- 7 B -> DateTime
);
```

```csharp
// ============================================================
// 2. C# ENTITY КЛАС (ЗА EF CORE И DAPPER)
// ============================================================
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public byte? Age { get; set; }              // SQL TINYINT NULL -> C# byte?
    public decimal Grade { get; set; }          // SQL DECIMAL(3,2) -> C# decimal
    public decimal? Scholarship { get; set; }   // SQL DECIMAL NULL -> C# decimal?
    public bool IsActive { get; set; }          // SQL BIT -> C# bool
    public DateTime CreatedAt { get; set; }     // SQL DATETIME2 -> C# DateTime
}
```

---

## 🚨 5. КРИТИЧНИ JUNIOR КАПАНИ

1. **Забравен префикс `N` пред кирилица в MS SQL:** `WHERE Name = 'Мария'` може да не намери записа или да го запише като `'????'`. Винаги пиши `N'Мария'`.
2. **Несъответствие между SQL `NULL` и C# тип:** Ако в базата `Age` е `INT NULL`, а в C# напишеш `public int Age { get; set; }` (без `?`), приложението ще гръмне с `SqlNullValueException` при първия ред без възраст.
3. **`NVARCHAR(MAX)` навсякъде по навик:** Полетата с `(MAX)` се изнасят в отделни LOB страници, забавят `SELECT` и **не могат да бъдат индексирани** като ключови колони. Слагай точен лимит (`NVARCHAR(100)`, `NVARCHAR(500)`).
4. **Използване на `BIGINT` за малки таблици:** Ако таблицата има 500 или 50 000 реда (напр. `Cities`, `Categories`), `INT` (до 2.14 милиарда) е напълно достатъчен и пести памет във всички свързани таблици и индекси.

---
© 2026 Dev-Library / Бази данни: Типове данни, Памет и C#
