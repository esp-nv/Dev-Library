````text
-- ============================================================
-- SQL SERVER — CRUD + DATA TYPES → C# + UNICODE
-- ============================================================


-- ============================================================
-- 1. CRUD
-- ============================================================

-- C = CREATE
-- → създавам / добавям данни
--
-- R = READ
-- → чета / взимам данни
--
-- U = UPDATE
-- → променям данни
--
-- D = DELETE
-- → изтривам данни


-- ============================================================
-- CREATE — INSERT
-- ============================================================

INSERT INTO Students (Name, Grade, Age)
VALUES (N'Maria', 5.50, 22);

-- INSERT INTO → таблицата + колоните
-- VALUES      → стойностите


-- ============================================================
-- READ — SELECT
-- ============================================================

SELECT *
FROM Students;


SELECT Name, Grade
FROM Students;


SELECT Name, Grade
FROM Students
WHERE Grade > 5;

-- SELECT → какво?
-- FROM   → откъде?
-- WHERE  → кои редове?


-- ============================================================
-- UPDATE
-- ============================================================

UPDATE Students
SET Grade = 6
WHERE Id = 5;

-- UPDATE → коя таблица?
-- SET    → какво променям?
-- WHERE  → кои редове?


-- !!! ВНИМАНИЕ !!!

UPDATE Students
SET Grade = 6;

-- БЕЗ WHERE:
-- → променя ВСИЧКИ редове.


-- ============================================================
-- DELETE
-- ============================================================

DELETE FROM Students
WHERE Id = 5;

-- DELETE FROM → таблицата
-- WHERE       → кои редове?


-- !!! ВНИМАНИЕ !!!

DELETE FROM Students;

-- БЕЗ WHERE:
-- → изтрива ВСИЧКИ редове.


-- ============================================================
-- CRUD — МИНИ ШПАРГАЛКА
-- ============================================================

-- CREATE → INSERT → добавям
-- READ   → SELECT → чета
-- UPDATE → UPDATE → променям
-- DELETE → DELETE → изтривам


-- ============================================================
-- 2. SQL SERVER DATA TYPES → C#
-- ============================================================


-- ------------------------------------------------------------
-- ЦЕЛИ ЧИСЛА
-- ------------------------------------------------------------

-- SQL Server        → C#

-- INT               → int
-- BIGINT            → long
-- SMALLINT          → short
-- TINYINT           → byte


-- Пример:

Id INT
Age INT

-- C#:

public int Id { get; set; }
public int Age { get; set; }


-- ------------------------------------------------------------
-- ДЕСЕТИЧНИ ЧИСЛА
-- ------------------------------------------------------------

-- SQL Server        → C#

-- DECIMAL           → decimal
-- NUMERIC           → decimal
-- FLOAT             → double
-- REAL              → float


-- Пример:

Price DECIMAL(18,2)

-- C#:

public decimal Price { get; set; }


-- ЗАПОМНИ:
-- пари / точни десетични стойности
-- → DECIMAL
-- → C# decimal


-- ------------------------------------------------------------
-- TRUE / FALSE
-- ------------------------------------------------------------

-- SQL Server        → C#

-- BIT               → bool


-- SQL:

IsActive BIT

-- C#:

public bool IsActive { get; set; }


-- ------------------------------------------------------------
-- ТЕКСТ
-- ------------------------------------------------------------

-- SQL Server        → C#

-- VARCHAR           → string
-- NVARCHAR          → string
-- CHAR              → string
-- NCHAR             → string


-- ------------------------------------------------------------
-- UNICODE
-- ------------------------------------------------------------

-- VARCHAR
-- → НЕ-Unicode текст
--
-- NVARCHAR
-- → Unicode текст
-- → кирилица
-- → китайски
-- → арабски
-- → други Unicode символи


-- ЗАПОМНИ:
--
-- VARCHAR
-- → string
-- → НЕ-Unicode
--
-- NVARCHAR
-- → string
-- → Unicode


-- Пример:

Name NVARCHAR(100)

-- C#:

public string Name { get; set; }


-- ------------------------------------------------------------
-- UNICODE TEXT LITERAL
-- ------------------------------------------------------------

-- В SQL Server:

SELECT *
FROM Students
WHERE Name = N'Мария';


-- N пред текста:
-- → Unicode текстов литерал


-- ЗАПОМНИ:
--
-- N'Мария'
-- → Unicode текст


-- ------------------------------------------------------------
-- ДАТА / ЧАС
-- ------------------------------------------------------------

-- SQL Server        → C#

-- DATE              → DateTime
-- DATETIME          → DateTime
-- DATETIME2         → DateTime
-- DATETIMEOFFSET    → DateTimeOffset
-- TIME              → TimeSpan


-- Пример:

BirthDate DATE

-- C#:

public DateTime BirthDate { get; set; }


CreatedAt DATETIME2

-- C#:

public DateTime CreatedAt { get; set; }


-- ------------------------------------------------------------
-- UNIQUEIDENTIFIER
-- ------------------------------------------------------------

-- SQL Server        → C#

-- UNIQUEIDENTIFIER  → Guid


-- SQL:

Id UNIQUEIDENTIFIER

-- C#:

public Guid Id { get; set; }


-- ------------------------------------------------------------
-- BINARY
-- ------------------------------------------------------------

-- SQL Server        → C#

-- BINARY            → byte[]
-- VARBINARY         → byte[]


-- C#:

public byte[] FileData { get; set; }


-- ============================================================
-- 3. NULLABLE TYPES
-- ============================================================

-- Ако SQL колоната МОЖЕ да бъде NULL,
-- C# value type също трябва да може да бъде null.


-- SQL:

Age INT NULL

-- C#:

public int? Age { get; set; }


-- SQL:

Price DECIMAL(18,2) NULL

-- C#:

public decimal? Price { get; set; }


-- SQL:

IsActive BIT NULL

-- C#:

public bool? IsActive { get; set; }


-- SQL:

CreatedAt DATETIME2 NULL

-- C#:

public DateTime? CreatedAt { get; set; }


-- SQL:

Id UNIQUEIDENTIFIER NULL

-- C#:

public Guid? Id { get; set; }


-- ЛОГИКА:
--
-- SQL NULL
--      ↓
-- C# nullable
--
-- int      → int?
-- decimal  → decimal?
-- bool     → bool?
-- DateTime → DateTime?
-- Guid     → Guid?


-- string е reference type и може да бъде null.


-- ============================================================
-- 4. НАЙ-ВАЖНИТЕ TYPE MAPPINGS
-- ============================================================

-- SQL Server          → C#

-- INT                 → int
-- BIGINT              → long
-- SMALLINT            → short
-- TINYINT             → byte

-- DECIMAL / NUMERIC   → decimal
-- FLOAT               → double
-- REAL                → float

-- BIT                 → bool

-- VARCHAR             → string
-- NVARCHAR            → string
-- CHAR                → string
-- NCHAR               → string

-- DATE                → DateTime
-- DATETIME            → DateTime
-- DATETIME2           → DateTime
-- DATETIMEOFFSET      → DateTimeOffset
-- TIME                → TimeSpan

-- UNIQUEIDENTIFIER    → Guid

-- BINARY              → byte[]
-- VARBINARY           → byte[]


-- ============================================================
-- 5. SQL → C# ENTITY
-- ============================================================

-- SQL таблица:

CREATE TABLE Students
(
    Id INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Age INT NULL,
    Grade DECIMAL(3,2) NOT NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);


-- C# Entity:

public class Student
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? Age { get; set; }

    public decimal Grade { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}


-- МИСЛЕНЕ:
--
-- SQL таблица
--      ↓
-- SQL колони
--      ↓
-- C# properties
--      ↓
-- SQL type → C# type


-- ============================================================
-- 6. CRUD + WHERE
-- ============================================================

-- READ:

SELECT *
FROM Students
WHERE Id = 5;


-- UPDATE:

UPDATE Students
SET Grade = 6
WHERE Id = 5;


-- DELETE:

DELETE FROM Students
WHERE Id = 5;


-- ЗАПОМНИ:
--
-- WHERE е особено важен при UPDATE и DELETE.
--
-- UPDATE без WHERE
-- → променям всички редове
--
-- DELETE без WHERE
-- → изтривам всички редове


-- ============================================================
-- 7. CRUD + TRANSACTION
-- ============================================================

BEGIN TRANSACTION;

UPDATE Accounts
SET Balance = Balance - 100
WHERE Id = 1;

UPDATE Accounts
SET Balance = Balance + 100
WHERE Id = 2;


-- Ако всичко е правилно:

COMMIT;


-- Ако нещо се обърка:

ROLLBACK;


-- ЛОГИКА:
--
-- BEGIN
-- → започвам
--
-- CRUD операции
-- → правя промените
--
-- COMMIT
-- → потвърждавам
--
-- ROLLBACK
-- → отменям


-- ============================================================
-- 8. 🧠 ЗАПОМНИ КАТО ЛОГИКА
-- ============================================================

-- INSERT
-- → ДОБАВЯМ

-- SELECT
-- → ЧЕТА

-- UPDATE
-- → ПРОМЕНЯМ

-- DELETE
-- → ИЗТРИВАМ


-- INT
-- → int

-- BIGINT
-- → long

-- DECIMAL
-- → decimal

-- BIT
-- → bool

-- VARCHAR
-- → string
-- → НЕ-Unicode

-- NVARCHAR
-- → string
-- → Unicode

-- N'текст'
-- → Unicode текстов литерал

-- DATETIME2
-- → DateTime

-- UNIQUEIDENTIFIER
-- → Guid

-- VARBINARY
-- → byte[]


-- ============================================================
-- 9. МИНИ ШПАРГАЛКА
-- ============================================================

-- CRUD:
--
-- C → INSERT → добавям
-- R → SELECT → чета
-- U → UPDATE → променям
-- D → DELETE → изтривам


-- DATA TYPES:
--
-- INT       → int
-- BIGINT    → long
-- DECIMAL   → decimal
-- BIT       → bool
-- VARCHAR   → string → НЕ-Unicode
-- NVARCHAR  → string → Unicode
-- DATETIME2 → DateTime
-- GUID      → Guid
-- VARBINARY → byte[]


-- NULLABLE:
--
-- INT NULL
--     ↓
-- int?
--
-- DECIMAL NULL
--     ↓
-- decimal?
--
-- BIT NULL
--     ↓
-- bool?
--
-- DATETIME2 NULL
--     ↓
-- DateTime?


-- ============================================================
-- 10. МОСТЪТ КЪМ EF CORE
-- ============================================================

-- SQL Server
--      ↓
-- SQL тип
--      ↓
-- C# тип
--      ↓
-- Entity property
--      ↓
-- EF Core mapping
--      ↓
-- Database


-- Пример:
--
-- NVARCHAR(100)
--      ↓
-- string
--      ↓
-- public string Name { get; set; }


-- INT
--      ↓
-- int
--      ↓
-- public int Id { get; set; }


-- DECIMAL(18,2)
--      ↓
-- decimal
--      ↓
-- public decimal Price { get; set; }


-- BIT
--      ↓
-- bool
--      ↓
-- public bool IsActive { get; set; }


-- ============================================================
-- НЕ ЗУБРИМ ВСИЧКИ ТИПОВЕ.
--
-- Първо мислим:
--
-- число?
-- → INT / BIGINT / DECIMAL
--
-- текст?
-- → VARCHAR / NVARCHAR
--
-- Unicode?
-- → NVARCHAR
--
-- true / false?
-- → BIT
--
-- дата?
-- → DATE / DATETIME2
--
-- уникален идентификатор?
-- → UNIQUEIDENTIFIER
--
-- двоични данни?
-- → VARBINARY
--
-- може ли да няма стойност?
-- → NULL → C# nullable type
--
-- После:
-- SQL type → C# type → EF Core
-- ============================================================


Така вече имаш една цяла SQL → C# шпаргалка, която естествено ни води към EF Core утре.
