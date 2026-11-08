⬅️ [03-01. SQL Joins](03-01-SQL-Joins-Visual-And-Code.md) | 📂 [03-00. Дълбок SQL: Свързване, Подзаявки, Агрегации и Функции](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md)  | ➡️ [03-03. Групиране, Агрегации и Window Functions](03-03-SQL-Grouping-Aggregations-Window-Functions.md)

---

# 03-02. Подзаявки (Subqueries) и CTE (Common Table Expressions)

**Цел:** Как да разделяме сложни SQL проблеми на малки, четими стъпки чрез вложени заявки (`IN`, `EXISTS`, скаларни подзаявки) и модерни `WITH` (CTE) изрази, включително рекурсивни CTE за дървовидни структури.

## Съдържание на темата
* [1. Какво е Subquery и видове подзаявки](#1-какво-е-subquery-и-видове-подзаявки)
* [2. IN срещу EXISTS (Ключова разлика за производителност)](#2-in-срещу-exists-ключова-разлика-за-производителност)
* [3. CTE (Common Table Expressions — WITH клауза)](#3-cte-common-table-expressions--with-клауза)
* [4. Рекурсивно CTE (Обхождане на йерархии и дървета)](#4-рекурсивно-cte-обхождане-на-йерархии-и-дървета)
* [5. Чести грешки, Капани и Как да ги избегнем](#5-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. КАКВО Е SUBQUERY И ВИДОВЕ ПОДЗАЯВКИ

**Subquery (Подзаявка)** е заявка `SELECT`, вложена вътре в друга заявка (в `WHERE`, `FROM` или `SELECT`).

| Вид подзаявка | Какво връща? | Къде се използва? | Пример |
| :--- | :--- | :--- | :--- |
| **Скаларна (Scalar)** | **Една клетка** (1 ред и 1 колона). | В `WHERE` или `SELECT` за сравнение с число/дата. | `WHERE Price > (SELECT AVG(Price) FROM Products)` |
| **Списъчна (Multi-row)** | **Една колона** с много редове. | С операторите `IN`, `NOT IN`, `ANY`, `ALL`. | `WHERE CategoryId IN (SELECT Id FROM Categories WHERE IsActive = 1)` |
| **Таблична (Derived Table)** | **Цяла таблица** (много редове и колони). | В `FROM` клаузата (задължително с `AS Alias`). | `FROM (SELECT UserId, COUNT(*) AS Cnt FROM Orders GROUP BY UserId) AS t` |
| **Корелирана (Correlated)** | Изпълнява се **по веднъж за всеки ред** на външната заявка. | С `EXISTS` / `NOT EXISTS` (реферира колона отвън). | `WHERE EXISTS (SELECT 1 FROM Orders o WHERE o.UserId = u.Id)` |

```sql
-- ============================================================
-- 1. СКАЛАРНА ПОДЗАЯВКА В WHERE
-- ============================================================
-- Задача: Намери продуктите, по-скъпи от средната цена в магазина.
SELECT Name, Price
FROM Products
WHERE Price > (SELECT AVG(Price) FROM Products)
ORDER BY Price DESC;
```

---

## 2. IN СРЕЩУ EXISTS (КЛЮЧОВА РАЗЛИКА ЗА ПРОИЗВОДИТЕЛНОСТ)

И двата оператора проверяват дали даден запис има съвпадение в друга таблица, но работят по коренно различен начин под капака:

* **`IN`:** Първо изпълнява вътрешната заявка и зарежда **целия списък** от ID-та в паметта, след което сравнява. Добър за малки списъци (`IN (1, 2, 5)`).
* **`EXISTS`:** Връща `TRUE` в момента, в който намери **първото съвпадение** (Semi-Join), и веднага спира да търси нататък. Много по-бърз при големи таблици и **безопасен при `NULL` стойности**!

```sql
-- ============================================================
-- 2. EXISTS (ЗЛАТЕН СТАНДАРТ ЗА ПРОВЕРКА НА СЪЩЕСТВУВАНЕ)
-- ============================================================
-- Задача: Намери всички клиенти, които имат поне една платена поръчка.
SELECT c.Id, c.FullName
FROM Customers AS c
WHERE EXISTS (
    SELECT 1 
    FROM Orders AS o 
    WHERE o.CustomerId = c.Id AND o.IsPaid = 1
);
```

---

## 3. CTE (COMMON TABLE EXPRESSIONS — WITH КЛАУЗА)

**CTE** е временна, именувана таблица, която живее **само по време на изпълнението на една заявка**. Превръща грозните вложени подзаявки в чист, модулен код, който се чете отгоре надолу като стъпки в C#.

### Предимства на CTE пред вложените подзаявки:
1. **Четимост:** Вместо 3 нива вложени скоби `(SELECT ... FROM (SELECT ...))`, пишеш `WITH Step1 AS (...), Step2 AS (...)`.
2. **Многократна употреба:** Можеш да реферираш едно и също CTE няколко пъти в главния `SELECT`.
3. **Решаване на проблема с `Window Functions` в `WHERE`:** Тъй като `ROW_NUMBER()` не може да стои в `WHERE`, го изчисляваш в CTE и го филтрираш отвън.

```sql
-- ============================================================
-- 3. МНОЖЕСТВЕНО CTE (СТЪПКА ПО СТЪПКА)
-- ============================================================
-- Важно: Ако преди WITH има друга заявка, тя ЗАДЪЛЖИТЕЛНО завършва с ;

WITH CustomerTotals AS (
    -- Стъпка 1: Смятаме общата сума и броя поръчки за всеки клиент
    SELECT 
        CustomerId,
        COUNT(Id) AS OrdersCount,
        SUM(TotalAmount) AS TotalSpent
    FROM Orders
    WHERE IsPaid = 1
    GROUP BY CustomerId
),
VipCustomers AS (
    -- Стъпка 2: Отделяме само VIP клиентите (над 1000 лв.)
    SELECT CustomerId, OrdersCount, TotalSpent
    FROM CustomerTotals
    WHERE TotalSpent > 1000
)
-- Стъпка 3: Свързваме VIP клиентите с имената им от таблица Customers
SELECT c.FullName, c.Email, v.OrdersCount, v.TotalSpent
FROM VipCustomers AS v
INNER JOIN Customers AS c ON v.CustomerId = c.Id
ORDER BY v.TotalSpent DESC;
```

---

## 4. РЕКУРСИВНО CTE (ОБХОЖДАНЕ НА ЙЕРАРХИИ И ДЪРВЕТА)

Рекурсивното CTE извиква **самото себе си**, за да обходи неограничена дълбочина от нива (напр. категории и подкатегории в онлайн магазин, или организационна структура мениджър ➔ служител).

Състои се от две части, съединени с **`UNION ALL`**:
1. **Anchor Member (Котва):** Началният връх на дървото (напр. `ParentId IS NULL`).
2. **Recursive Member (Рекурсивна стъпка):** Свързва следващото ниво с предишното, докато не останат деца.

```sql
-- ============================================================
-- 4. РЕКУРСИВНО CTE: ЙЕРАРХИЯ НА КАТЕГОРИИ В МАГАЗИН
-- ============================================================
-- Таблица Categories: Id, Name, ParentCategoryId (NULL за главните категории)

WITH CategoryTree AS (
    -- 1. Котва: Взимаме главните категории (Ниво 0)
    SELECT Id, Name, ParentCategoryId, 0 AS Level, CAST(Name AS NVARCHAR(500)) AS FullPath
    FROM Categories
    WHERE ParentCategoryId IS NULL

    UNION ALL

    -- 2. Рекурсия: Добавяме подкатегориите и увеличаваме нивото
    SELECT c.Id, c.Name, c.ParentCategoryId, ct.Level + 1, CAST(ct.FullPath + N' > ' + c.Name AS NVARCHAR(500))
    FROM Categories AS c
    INNER JOIN CategoryTree AS ct ON c.ParentCategoryId = ct.Id
)
SELECT Id, Level, FullPath
FROM CategoryTree
ORDER BY FullPath;
-- Резултат:
-- Електроника (Level 0)
-- Електроника > Компютри (Level 1)
-- Електроника > Компютри > Лаптопи (Level 2)
```

---

## 5. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Смъртоносният капан на `NOT IN` при наличие на `NULL`
* **Какво се случва:** Пишеш `SELECT * FROM Customers WHERE Id NOT IN (SELECT CustomerId FROM Orders)`. Ако в таблица `Orders` има дори **един-единствен ред** с `CustomerId = NULL`, цялата заявка връща **0 реда (празен резултат)**! Причината е тризначната логика в SQL: `Id != NULL` връща `UNKNOWN`, а не `TRUE`.
* **Как да избегнеш:** За отрицателни проверки **никога** не ползвай `NOT IN` върху подзаявка — винаги ползвай **`NOT EXISTS`** или **`LEFT JOIN ... WHERE o.Id IS NULL`**:
  ```sql
  -- БЕЗОПАСНО И БЪРЗО (работи правилно дори да има NULL стойности):
  SELECT c.Id, c.FullName
  FROM Customers AS c
  WHERE NOT EXISTS (SELECT 1 FROM Orders AS o WHERE o.CustomerId = c.Id);
  ```

### Грешка 2: Липсваща точка и запетая (`;`) преди `WITH` в MS SQL Server
* **Какво се случва:** Ако имаш две заявки в един скрипт и втората започва с `WITH MyCte AS (...)`, MS SQL Server хвърля синтактична грешка *Incorrect syntax near the keyword 'with'. If this statement is a common table expression, an xmlnamespaces clause or a change tracking context clause, the previous statement must be terminated with a semicolon.*
* **Как да избегнеш:** Винаги завършвай предходната команда с `;` (много разработчици пишат `;WITH MyCte AS (...)` като защитен навик).

### Грешка 3: Скаларна подзаявка връща повече от 1 ред
* **Какво се случва:** Пишеш `WHERE DepartmentId = (SELECT Id FROM Departments WHERE Location = N'София')`. Ако в София има 2 отдела, базата гърми с грешка *Subquery returned more than 1 value*.
* **Как да избегнеш:** Когато подзаявката може да върне повече от една стойност, използвай `IN (...)` вместо `= (...)`, или сложи `TOP 1` / `LIMIT 1` с `ORDER BY`.

### Грешка 4: Безкраен цикъл при Рекурсивно CTE
* **Какво се случва:** В данните има цикъл (напр. Служител А е мениджър на Служител Б, а Служител Б е мениджър на А). Рекурсивното CTE стига лимита от 100 нива и гърми (*The maximum recursion 100 has been exhausted*).
* **Как да избегнеш:** Валидирай данните при запис, за да няма цикли, и при нужда контролирай дълбочината с `OPTION (MAXRECURSION 50)` в MS SQL.

---
⬅️ [03-01. SQL Joins](03-01-SQL-Joins-Visual-And-Code.md) | 📂 [03-00. Дълбок SQL: Свързване, Подзаявки, Агрегации и Функции](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md)  | ➡️ [03-03. Групиране, Агрегации и Window Functions](03-03-SQL-Grouping-Aggregations-Window-Functions.md)

© 2026 Dev-Library / Бази данни: Subqueries и CTE
