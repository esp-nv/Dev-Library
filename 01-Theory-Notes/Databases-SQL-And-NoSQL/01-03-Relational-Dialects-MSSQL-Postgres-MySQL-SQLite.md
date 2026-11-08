⬅️ [01-02. Типове данни и Памет](01-02-SQL-Data-Types-Memory-And-CSharp.md) | [📂 Глава 01-00-SQL-INTRO-AND-CRUD](01-00-SQL-INTRO-AND-CRUD.md) |  [🏠 Начало (README)](README.md) | ➡️ [01-04. NoSQL: MongoDB](01-04-NoSQL-Documents-MongoDB.md)

---

# 🗄️ 01-03. Релационни диалекти: MS SQL Server, PostgreSQL, MySQL и SQLite

**Цел:** Бързо превключване между 4-те основни релационни бази данни — разлики в синтаксиса, Auto-Increment ключовете, датите, текста, пагинацията и кога се избира всяка от тях.

## 📑 Съдържание на темата
* [1. Голямата матрица на SQL диалектите](#-1-голямата-матрица-на-sql-диалектите)
* [2. Една и съща таблица и заявка в 4-те бази](#-2-една-и-съща-таблица-и-заявка-в-4-те-бази)
* [3. Кога коя база да избереш?](#-3-кога-коя-база-да-избереш)
* [4. Критични капани при преминаване между базите](#-4-критични-капани-при-преминаване-между-базите)

---

## 📊 1. ГОЛЯМАТА МАТРИЦА НА SQL ДИАЛЕКТИТЕ

| Характеристика / Команда | MS SQL Server (T-SQL) | PostgreSQL (PL/pgSQL) | MySQL / MariaDB | SQLite |
| :--- | :--- | :--- | :--- | :--- |
| **Auto-Increment (PK)** | `INT IDENTITY(1,1)` | `INT GENERATED ALWAYS AS IDENTITY` *(или `SERIAL`)* | `INT AUTO_INCREMENT` | `INTEGER PRIMARY KEY AUTOINCREMENT` |
| **Unicode текст (Кирилица)** | `NVARCHAR(100)` + префикс `N'...'` | `VARCHAR(100)` / `TEXT` *(UTF-8 по подразбиране)* | `VARCHAR(100)` *(с `utf8mb4`)* | `TEXT` *(винаги UTF-8)* |
| **Логически тип (Bool)** | `BIT` (`0` или `1`) | `BOOLEAN` (`TRUE` / `FALSE`) | `TINYINT(1)` / `BOOLEAN` | `INTEGER` (`0` или `1` — няма отделен тип) |
| **Текущо UTC време** | `GETUTCDATE()` / `SYSUTCDATETIME()` | `NOW() AT TIME ZONE 'UTC'` | `UTC_TIMESTAMP()` | `datetime('now')` |
| **Слепване на низове** | `CONCAT(a, b)` или `a + b` | `CONCAT(a, b)` или `a \|\| b` | `CONCAT(a, b)` | `a \|\| b` *(няма `CONCAT` в стари версии)* |
| **Защита от `NULL`** | `ISNULL(col, 0)` или `COALESCE(col, 0)` | `COALESCE(col, 0)` | `IFNULL(col, 0)` или `COALESCE(col, 0)` | `IFNULL(col, 0)` или `COALESCE(col, 0)` |
| **Взимане на първите N** | `SELECT TOP 10 * FROM ...` | `SELECT * FROM ... LIMIT 10` | `SELECT * FROM ... LIMIT 10` | `SELECT * FROM ... LIMIT 10` |
| **Странициране (Pagination)** | `OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY` | `LIMIT 5 OFFSET 10` | `LIMIT 5 OFFSET 10` | `LIMIT 5 OFFSET 10` |
| **Екраниране на имена** | `[Order]` *(квадратни скоби)* | `"order"` *(двойни кавички)* | `` `order` `` *(обратни апострофи)* | `"order"` или `[order]` |
| **Главни / Малки букви** | **Case-Insensitive** по подразбиране (`'abc' = 'ABC'`) | **Case-Sensitive** (`'abc' != 'ABC'`; има `ILIKE`) | **Case-Insensitive** по подразбиране | **Case-Insensitive** само за ASCII |

---

## 💻 2. ЕДНА И СЪЩА ТАБЛИЦА И ЗАЯВКА В 4-ТЕ БАЗИ

### 🔹 А. MS SQL Server (T-SQL) — Корпоративен стандарт за .NET
```sql
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    IsPaid BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Търсене без значение от главни/малки букви + странициране
SELECT TOP 5 Id, CustomerName, Total
FROM Orders
WHERE IsPaid = 1 AND CustomerName LIKE N'%иван%'
ORDER BY CreatedAt DESC;
```

### 🔹 Б. PostgreSQL — Най-модерната Open-Source база
```sql
CREATE TABLE orders (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_name VARCHAR(100) NOT NULL,
    total NUMERIC(18, 2) NOT NULL,
    is_paid BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- ILIKE прави Case-Insensitive търсене (стандартният LIKE в Postgres е Case-Sensitive!)
SELECT id, customer_name, total
FROM orders
WHERE is_paid = TRUE AND customer_name ILIKE '%иван%'
ORDER BY created_at DESC
LIMIT 5 OFFSET 0;
```

### 🔹 В. MySQL / MariaDB — Класика за Web и PHP/Node проекти
```sql
CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_name VARCHAR(100) CHARACTER SET utf8mb4 NOT NULL,
    total DECIMAL(18, 2) NOT NULL,
    is_paid BOOLEAN NOT NULL DEFAULT FALSE,
    created_at DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP())
);

SELECT id, customer_name, total
FROM orders
WHERE is_paid = TRUE AND customer_name LIKE '%иван%'
ORDER BY created_at DESC
LIMIT 5 OFFSET 0;
```

### 🔹 Г. SQLite — Локален файл на диска (без сървър)
```sql
CREATE TABLE orders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_name TEXT NOT NULL,
    total REAL NOT NULL, -- Внимание: SQLite няма истински DECIMAL!
    is_paid INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

SELECT id, customer_name, total
FROM orders
WHERE is_paid = 1 AND customer_name LIKE '%иван%'
ORDER BY created_at DESC
LIMIT 5 OFFSET 0;
```

---

## 🎯 3. КОГА КОЯ БАЗА ДА ИЗБЕРЕШ?

| База данни | Кога е най-добрият избор? | Къде НЕ е подходяща? |
| :--- | :--- | :--- |
| **MS SQL Server** | Корпоративни .NET / C# системи, банкови и ERP приложения, тежка бизнес логика със Stored Procedures. | Стартъпи с ограничен бюджет за лицензи (Enterprise версията е скъпа). |
| **PostgreSQL** | Нови уеб/микросървис проекти (с C#, Node.js или Python), геоданни (PostGIS), хибридни данни (`JSONB`), AI вектори (`pgvector`). | Много прости мобилни/десктоп приложения без сървър. |
| **MySQL / MariaDB** | Класически уеб сайтове, CMS системи (WordPress), проекти с много четене и по-прости заявки. | Сложни аналитични заявки и тежки релационни отчети. |
| **SQLite** | Мобилни приложения (iOS/Android), десктоп програми, локално кеширане и бързи **Integration тестове** в C#. | Уеб приложения с много едновременни потребители, които пишат (заключва целия файл при запис). |

---

## 🚨 4. КРИТИЧНИ КАПАНИ ПРИ ПРЕМИНАВАНЕ МЕЖДУ БАЗИТЕ

1. **PostgreSQL обръща имената без кавички в малки букви:** Ако напишеш `CREATE TABLE Users (FirstName ...)`, Postgres го записва като `users` и `firstname`. Ако после напишеш `SELECT "FirstName"`, ще гръмне! Затова в Postgres винаги ползвай `snake_case` (`first_name`).
2. **PostgreSQL `LIKE` е чувствителен към главни/малки букви:** В MS SQL `'Ivan' LIKE 'ivan'` е `TRUE`, но в PostgreSQL е `FALSE`! В Postgres ползвай **`ILIKE`** за търсене без значение от регистъра.
3. **MySQL и капанът `utf8` срещу `utf8mb4`:** В старите версии на MySQL `utf8` поддържа само 3 байта и реже емоджита и редки символи. Винаги задавай **`utf8mb4`**.
4. **SQLite няма строги типове (Dynamic Typing) и `DECIMAL`:** SQLite съхранява числата с десетична запетая като 64-битов `REAL` (`FLOAT`), което не е подходящо за тежки финансови сметки, а `ALTER TABLE` има силно ограничени възможности (не можеш лесно да променяш тип на съществуваща колона).

---
⬅️ [01-02. Типове данни и Памет](01-02-SQL-Data-Types-Memory-And-CSharp.md) | [📂 Глава 01-00-SQL-INTRO-AND-CRUD](01-00-SQL-INTRO-AND-CRUD.md) | [🏠 Начало (README)](README.md)  | ➡️ [01-04. NoSQL: MongoDB](01-04-NoSQL-Documents-MongoDB.md)

© 2026 Dev-Library / Бази данни: Релационни диалекти
