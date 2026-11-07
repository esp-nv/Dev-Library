# 📘 Дълъг учебник: Въведение в Базите данни, Типове данни и CRUD

**Статус:** Дълбока архитектура и T-SQL / PostgreSQL практика | **Ниво:** Основи до Продукционен код

---

## 🏛️ 1. КАКВО Е РЕЛАЦИОННА БАЗА ДАННИ (RDBMS) И КАК РАБОТИ ПОД КАПАКА?

**RDBMS (Relational Database Management System)** е софтуерна система (MS SQL Server, PostgreSQL, MySQL), която съхранява данните в двумерни таблици (редове и колони), свързани помежду си чрез ключове.

### 🔍 Какво се случва при изпълнение на една SQL заявка?
1. **Parser & Binder:** Проверява синтаксиса (дали си написал правилно `SELECT`) и дали таблиците и колоните съществуват в системния каталог.
2. **Query Optimizer (Мозъкът на базата):** Тъй като SQL е **декларативен език** (казваш *какво* искаш, а не *как* да го намери), оптимизаторът изчислява десетки варианти (Execution Plans) — дали да прочете цялата таблица (`Table Scan`) или да използва индекс (`Index Seek`).
3. **Storage Engine:** Чете или записва страниците (Pages) в RAM буфера (`Buffer Pool`) и ги синхронизира с диска чрез транзакционния лог (`WAL`).

---

## 🏗️ 2. DDL: СЪЗДАВАНЕ И ПРОМЯНА НА СТРУКТУРАТА НА ТАБЛИЦИТЕ

Добрата структура на таблицата пази данните от грешки още преди C# кодът да се е изпълнил. Използваме **Constraints (Ограничения)**: `PRIMARY KEY`, `NOT NULL`, `UNIQUE`, `CHECK`, `DEFAULT`.

```sql
// ==================== MS SQL SERVER: СЪЗДАВАНЕ НА ТАБЛИЦА ====================
CREATE DATABASE ShopDb;
GO

USE ShopDb;
GO

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Автоматично нарастващо ID (1, 2, 3...)
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE, -- Забранява дублиращи се имейли
    Salary DECIMAL(18, 2) NOT NULL CHECK (Salary >= 933.00), -- Бизнес правило на ниво БД
    Department NVARCHAR(50) NOT NULL DEFAULT N'Общ отдел',
    HireDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

// ==================== POSTGRESQL: ЕКВИВАЛЕНТНА ТАБЛИЦА ====================
CREATE TABLE employees (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    salary NUMERIC(18, 2) NOT NULL CHECK (salary >= 933.00),
    department VARCHAR(50) NOT NULL DEFAULT 'Общ отдел',
    hire_date TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);
```

### 🔧 Промяна и изтриване на структура (`ALTER` и `DROP`):

```sql
// ==================== ПРОМЯНА НА СЪЩЕСТВУВАЩА ТАБЛИЦА ====================
-- Добавяне на нова колона
ALTER TABLE Employees
ADD PhoneNumber VARCHAR(20) NULL;

-- Промяна на тип или размер на колона
ALTER TABLE Employees
ALTER COLUMN FirstName NVARCHAR(100) NOT NULL;

-- Изтриване на колона
ALTER TABLE Employees
DROP COLUMN PhoneNumber;

-- Изтриване на цялата таблица (структура + данни)
DROP TABLE IF EXISTS Employees;
```

---

## 🔄 3. DML & DQL: ПЪЛЕН ЦИКЪЛ НА CRUD ОПЕРАЦИИТЕ

### 1️⃣ CREATE (`INSERT`) — Вмъкване на единични и множествени редове

```sql
// ==================== INSERT ПРИМЕРИ ====================
-- Единичен запис (Винаги изброявай колоните изрично!)
INSERT INTO Employees (FirstName, LastName, Email, Salary, Department)
VALUES (N'Георги', N'Димитров', 'g.dimitrov@company.bg', 3200.00, N'Разработка');

-- Множествено вмъкване в една транзакция (много по-бързо от отделни INSERT-и)
INSERT INTO Employees (FirstName, LastName, Email, Salary)
VALUES 
    (N'Мария', N'Иванова', 'm.ivanova@company.bg', 2850.50),
    (N'Петър', N'Стоянов', 'p.stoyanov@company.bg', 4100.00);

-- Връщане на генерираното ID веднага след запис (както прави EF Core!)
-- MS SQL Server:
INSERT INTO Employees (FirstName, LastName, Email, Salary)
OUTPUT INSERTED.Id
VALUES (N'Елена', N'Николова', 'e.nikolova@company.bg', 3500.00);

-- PostgreSQL:
-- INSERT INTO employees (first_name, last_name, email, salary)
-- VALUES ('Елена', 'Николова', 'e.nikolova@company.bg', 3500.00) RETURNING id;
```

### 2️⃣ READ (`SELECT`) — Филтриране, Изчисления, Сортиране и Странициране

```sql
// ==================== SELECT И ФИЛТРИРАНЕ ====================
-- Изчисляване на колони в реално време + Алиаси (AS)
SELECT 
    Id,
    CONCAT(FirstName, N' ', LastName) AS FullName,
    Salary AS MonthlySalary,
    Salary * 12 AS AnnualSalary
FROM Employees
WHERE IsActive = 1 
  AND Salary BETWEEN 2500 AND 4500
  AND Email LIKE '%@company.bg'
ORDER BY Salary DESC, LastName ASC;

-- Работа с NULL стойности (Тризначна логика: TRUE, FALSE, UNKNOWN)
SELECT FirstName, LastName, COALESCE(Department, N'Няма отдел') AS Dept
FROM Employees
WHERE Department IS NOT NULL;
```

### 3️⃣ UPDATE — Безопасно обновяване на данни

```sql
// ==================== UPDATE С ПРОВЕРКА ====================
-- Увеличаване на заплатата с 10% само за конкретен отдел
UPDATE Employees
SET Salary = Salary * 1.10
WHERE Department = N'Разработка' AND IsActive = 1;
```

### 4️⃣ DELETE срещу Soft Delete (Златен стандарт в работата)

В реалните фирмени проекти **рядко трием физически редове** с `DELETE`, защото губим историята и чупим отчетите. Вместо това ползваме **Soft Delete** (колона `IsDeleted` или `DeletedAt`):

```sql
// ==================== HARD DELETE СРЕЩУ SOFT DELETE ====================
-- Физическо изтриване (Hard Delete) - само за временни/грешни данни
DELETE FROM Employees
WHERE Id = 5;

-- Логическо изтриване (Soft Delete) - записът остава в БД, но е скрит за приложението
UPDATE Employees
SET IsActive = 0
WHERE Id = 3;
```

---

## ⚖️ 4. ДЪЛБОКО СРАВНЕНИЕ: `CHAR` / `VARCHAR` СРЕЩУ `NCHAR` / `NVARCHAR`

Разбирането на тези 4 типа пести гигабайти памет и предотвратява счупени текстове:

* **`CHAR(n)` / `NCHAR(n)` (Фиксирана дължина):** Ако дефинираш `CHAR(10)` и запишеш `'BG'`, базата заделя **всичките 10 символа** и допълва останалите 8 с интервали (`'BG        '`). Използва се **само** за кодове с гарантирана постоянна дължина (ISO кодове на валути `EUR`, `BGN`, ЕГН, пощенски кодове).
* **`VARCHAR(n)` / `NVARCHAR(n)` (Променлива дължина):** Ако дефинираш `NVARCHAR(100)` и запишеш `N'Иван'`, базата заема място само за **4 символа** + 2 байта за дължината.
* **Префиксът `N` (National / Unicode):** В MS SQL `VARCHAR` ползва 1 байт на символ (само латиница/ASCII), докато `NVARCHAR` ползва 2 байта (UTF-16) и съхранява безпроблемно кирилица, немски умлаути, китайски йероглифи и емоджита 🚀.

---

## 🚨 5. ПРАКТИЧЕСКИ КАПАНИ И ДОБРИ ПРАКТИКИ

1. **Никога `SELECT *` в продукционен C# код:** Когато пишеш `SELECT *`, теглиш излишни колони по мрежата (напр. големи описания или снимки) и чупиш оптимизацията на индексите. Изброявай само нужните колони.
2. **SQL Injection защита:** Никога не слепвай потребителски вход със `+` в SQL заявка (`"SELECT * FROM Users WHERE Name = '" + userInput + "'"`). Винаги използвай параметризирани заявки (`@Name`) или EF Core / Dapper.
3. **Затваряне на заявките с `;`:** Въпреки че T-SQL позволява изпускане на точка и запетая в някои случаи, стандартът ANSI SQL и функции като `WITH (CTE)` изискват предходната команда да завършва с `;`.

---
© 2026 Dev-Library / Дълъг учебник: Въведение в SQL и CRUD
