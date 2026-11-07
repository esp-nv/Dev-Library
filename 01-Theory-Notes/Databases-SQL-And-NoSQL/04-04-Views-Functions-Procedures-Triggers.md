⬅️ [04-03. Транзакции и ACID](04-03-Transactions-ACID-And-Isolation-Levels.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-05. C# Data Access (ADO.NET и Dapper)](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md)

---

# 04-04. Views, Functions, Stored Procedures и Triggers

**Цел:** Как да капсулираме повтаряща се SQL логика вътре в базата данни — кога и защо пишем **Views**, каква е разликата между **User-Defined Functions** и **Stored Procedures**, и как работят **Triggers** (`AFTER` и `INSTEAD OF`) при автоматичен одит.

## Съдържание на темата
* [1. Сравнителна карта: View срещу Function срещу Stored Procedure срещу Trigger](#1-сравнителна-карта-view-срещу-function-срещу-stored-procedure-срещу-trigger)
* [2. Views (Изгледи) — Кога се пишат и Видове (Standard vs Materialized)](#2-views-изгледи--кога-се-пишат-и-видове-standard-vs-materialized)
* [3. User-Defined Functions (Scalar и Table-Valued Functions)](#3-user-defined-functions-scalar-и-table-valued-functions)
* [4. Stored Procedures (Съхранени процедури) с параметри и TRY...CATCH](#4-stored-procedures-съхранени-процедури-с-параметри-и-trycatch)
* [5. Triggers (Тригери) — Автоматичен одит при INSERT / UPDATE / DELETE](#5-triggers-тригери--автоматичен-одит-при-insert--update--delete)
* [6. Чести грешки, Капани и Как да ги избегнем](#6-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. СРАВНИТЕЛНА КАРТА: VIEW СРЕЩУ FUNCTION СРЕЩУ STORED PROCEDURE СРЕЩУ TRIGGER

| Обект в БД | Може ли да променя данни (`INSERT/UPDATE/DELETE`)? | Приема ли входни параметри? | Може ли да се ползва вътре в `SELECT` / `WHERE`? | Как се извиква? |
| :--- | :--- | :--- | :--- | :--- |
| **`VIEW`** | Не (само чете чрез `SELECT`). | Не (филтрираш го отвън с `WHERE`). | **Да** (като обикновена таблица). | `SELECT * FROM v_ActiveOrders;` |
| **`FUNCTION` (UDF)** | **Не** (строго забранено да променя таблици). | **Да** (`@Price`, `@Date`). | **Да** (`SELECT dbo.CalcVat(Price)`). | Вътре в `SELECT`, `WHERE` или `JOIN`. |
| **`STORED PROCEDURE`** | **Да** (пълни CRUD + транзакции). | **Да** (входни и `OUTPUT` параметри). | **Не** (не може да се вгради в `SELECT`). | `EXEC usp_CreateOrder @UserId = 5;` |
| **`TRIGGER`** | **Да** (обикновено пише в Audit/Log таблици). | Не (работи със специалните таблици `INSERTED` и `DELETED`). | Не. | **Автоматично** при `INSERT`, `UPDATE` или `DELETE`. |

---

## 2. VIEWS (ИЗГЛЕДИ) — КОГА СЕ ПИШАТ И ВИДОВЕ

**`VIEW`** е „виртуална таблица“, съставена от запазена `SELECT` заявка. Обикновеното `VIEW` не пази данни на диска — всеки път, когато го прочетеш, базата изпълнява вътрешния му `SELECT`.

### Кога пишем `VIEW` в реалната практика?
1. **Скриване на сложни `JOIN`-ове:** Вместо в C# или Power BI да пишеш 40-редова заявка с 5 `JOIN`-а всеки път, правиш `v_OrderDetails` и го четеш с чист `SELECT * FROM v_OrderDetails WHERE Year = 2026`.
2. **Сигурност на колоните (Security):** Имаш таблица `Employees` с чувствителни колони (`Salary`, `EGN`, `PasswordHash`). Създаваш `v_EmployeeContacts` само с име, отдел и служебен имейл, и даваш права на отчетите да четат само от изгледа.
3. **Обратна съвместимост (Backward Compatibility):** Разделяш една стара таблица на две нови, но създаваш `VIEW` със старото име, за да не счупиш стари приложения.
4. **Бързи отчети (`Indexed View` в MS SQL / `Materialized View` в PostgreSQL):** Записва резултата **физически на диска** с индекс, за да се четат тежки агрегации за 1 ms.

```sql
-- ============================================================
-- 1. СЪЗДАВАНЕ НА VIEW В MS SQL SERVER И POSTGRESQL
-- ============================================================
CREATE VIEW v_CustomerOrderSummary AS
SELECT 
    c.Id AS CustomerId,
    c.FullName,
    c.Email,
    COUNT(o.Id) AS TotalOrders,
    COALESCE(SUM(o.TotalAmount), 0) AS TotalSpent
FROM Customers AS c
LEFT JOIN Orders AS o ON c.Id = o.CustomerId
WHERE c.IsActive = 1
GROUP BY c.Id, c.FullName, c.Email;

-- Използване (изглежда точно като обикновена таблица):
SELECT FullName, TotalSpent
FROM v_CustomerOrderSummary
WHERE TotalSpent > 1000
ORDER BY TotalSpent DESC;
```

---

## 3. USER-DEFINED FUNCTIONS (SCALAR И TABLE-VALUED FUNCTIONS)

Функциите в SQL се делят на два вида:
* **Scalar Function:** Връща **една единична стойност** (число, текст, дата).
* **Inline Table-Valued Function (iTVF):** Връща **цяла таблица** на база подаден параметър (работи като параметризирано `VIEW` и е много бърза!).

```sql
-- ============================================================
-- 2. INLINE TABLE-VALUED FUNCTION (БЪРЗА ПАРАМЕТРИЗИРАНА ТАБЛИЦА)
-- ============================================================
CREATE FUNCTION fn_GetOrdersByMinAmount (@MinAmount DECIMAL(18, 2))
RETURNS TABLE
AS
RETURN
(
    SELECT Id, CustomerId, TotalAmount, CreatedAt
    FROM Orders
    WHERE TotalAmount >= @MinAmount
);

-- Извикване като таблица в SELECT или JOIN:
SELECT * FROM dbo.fn_GetOrdersByMinAmount(500.00);
```

---

## 4. STORED PROCEDURES (СЪХРАНЕНИ ПРОЦЕДУРИ)

**Stored Procedure** е предварително компилиран пакет от SQL команди, който приема параметри, съдържа бизнес логика (`IF / ELSE`, цикли, транзакции) и връща резултат или код за успех.

### Предимства в корпоративни проекти:
* **Сигурност:** Даваш право на приложението само да изпълнява `EXEC usp_TransferMoney`, без да има директни права за `UPDATE` или `DELETE` върху таблиците.
* **Намален мрежов трафик:** Вместо C# да праща 5 отделни заявки по мрежата, изпраща 1 команда `EXEC` с параметри.

```sql
-- ============================================================
-- 3. STORED PROCEDURE В MS SQL SERVER С OUTPUT ПАРАМЕТЪР
-- ============================================================
CREATE PROCEDURE usp_RegisterCustomer
    @FullName NVARCHAR(100),
    @Email VARCHAR(100),
    @NewCustomerId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON; -- Спира излишните съобщения "(1 row affected)" за по-добра скорост

    IF EXISTS (SELECT 1 FROM Customers WHERE Email = @Email)
    BEGIN
        THROW 50010, N'Потребител с този имейл вече съществува!', 1;
    END

    INSERT INTO Customers (FullName, Email, IsActive, CreatedAt)
    VALUES (@FullName, @Email, 1, GETUTCDATE());

    SET @NewCustomerId = SCOPE_IDENTITY(); -- Връща ID-то на току-що създадения ред
END;

-- Извикване в T-SQL:
DECLARE @CreatedId INT;
EXEC usp_RegisterCustomer @FullName = N'Мария Георгиева', @Email = 'maria@abv.bg', @NewCustomerId = @CreatedId OUTPUT;
SELECT @CreatedId AS CreatedCustomerId;
```

---

## 5. TRIGGERS (ТРИГЕРИ) — АВТОМАТИЧЕН ОДИТ ПРИ ПРОМЯНА

**Trigger** е специална процедура, която се задейства **автоматично** от базата данни при събитие (`INSERT`, `UPDATE` или `DELETE`) върху конкретна таблица.

В MS SQL Server тригерът има достъп до две виртуални таблици в паметта:
* **`INSERTED`:** Съдържа новите редове (при `INSERT` и `UPDATE`).
* **`DELETED`:** Съдържа старите редове преди промяната (при `DELETE` и `UPDATE`).

```sql
-- ============================================================
-- 4. AFTER UPDATE TRIGGER ЗА ОДИТ НА ПРОМЯНА НА ЗАПЛАТИ
-- ============================================================
CREATE TABLE SalaryAuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    OldSalary DECIMAL(18, 2) NOT NULL,
    NewSalary DECIMAL(18, 2) NOT NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TRIGGER tr_Employees_SalaryAudit
ON Employees
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Записваме в одит таблицата само ако колоната Salary реално е променена!
    IF UPDATE(Salary)
    BEGIN
        INSERT INTO SalaryAuditLogs (EmployeeId, OldSalary, NewSalary)
        SELECT d.Id, d.Salary, i.Salary
        FROM DELETED AS d
        INNER JOIN INSERTED AS i ON d.Id = i.Id
        WHERE d.Salary <> i.Salary;
    END
END;
```

---

## 6. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Писане на тригер с предположение, че се променя само 1 ред (`SELECT @Id = Id FROM INSERTED`)
* **Какво се случва:** Най-честият бъг в тригерите! Тригерът се изпълнява **веднъж на заявка**, а не веднъж на ред. Ако някой пусне `UPDATE Employees SET Salary = Salary * 1.10` за 500 души едновременно, `INSERTED` ще съдържа 500 реда, а `SELECT @Id = Id FROM INSERTED` ще вземе само 1 случаен човек и ще пропусне останалите 499!
* **Как да избегнеш:** Винаги пиши тригерите **Set-Based** (чрез `INSERT INTO ... SELECT ... FROM INSERTED JOIN DELETED`), никога със скаларни променливи.

### Грешка 2: Трупане на `VIEW` върху `VIEW` върху `VIEW` (Nested Views)
* **Какво се случва:** Създаваш `v_OrdersBase`, върху него `v_OrdersWithUsers`, върху него `v_MonthlyReport`. Оптимизаторът на заявката губи точна статистика за броя редове и започва да избира бавни *Table Scans* вместо индекси.
* **Как да избегнеш:** Дръж изгледите плоски (само 1 ниво върху реалните таблици) и никога не слагай `ORDER BY` вътре в `VIEW` (освен с `TOP 100 PERCENT`, което не гарантира сортиране при четене отвън).

### Грешка 3: Скрита бизнес логика в тригери, за която C# екипът не знае
* **Какво се случва:** Слагаш сложна логика в тригер, която обновява 4 други таблици при всяко `INSERT`. Когато настъпи грешка или забавяне, програмистите дебъгват C# кода с часове, без да подозират, че тригер в базата променя данните „на тъмно“.
* **Как да избегнеш:** Използвай тригери **единствено за одит логове (`AuditLogs`)** или критична защита на данни; дръж бизнес логиката в C# сървисите или в ясни `Stored Procedures`.

---
⬅️ [04-03. Транзакции и ACID](04-03-Transactions-ACID-And-Isolation-Levels.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-05. C# Data Access (ADO.NET и Dapper)](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md)

---
© 2026 Dev-Library / Бази данни: Views, Functions, Procedures и Triggers
