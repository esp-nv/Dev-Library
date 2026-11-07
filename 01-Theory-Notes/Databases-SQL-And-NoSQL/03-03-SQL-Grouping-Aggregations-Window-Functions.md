⬅️ [03-02. Subqueries и CTE](03-02-SQL-Subqueries-And-CTEs.md) | 📂 [Глава 03](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [03-04. Вградени функции и Сет оператори](03-04-SQL-BuiltIn-Functions-And-Set-Operators.md)

---

# 03-03. Групиране, Агрегации и Window Functions (Прозоречни функции)

**Цел:** Как да обобщаваме данни по категории чрез `GROUP BY`, `HAVING` и агрегатни функции (`COUNT`, `SUM`, `AVG`, `STRING_AGG`), и как да правим класации, топ-N на група и сравнения с предходен ред чрез `OVER (PARTITION BY ... ORDER BY ...)` без да губим детайлите на отделните редове.

## Съдържание на темата
* [1. Агрегатни функции и работа с NULL стойности](#1-агрегатни-функции-и-работа-с-null-стойности)
* [2. GROUP BY и HAVING — Свиване на редовете в групи](#2-group-by-и-having--свиване-на-редовете-в-групи)
* [3. Window Functions (OVER) — Изчисления БЕЗ смачкване на редовете](#3-window-functions-over--изчисления-без-смачкване-на-редовете)
* [4. Сравнение на функциите за ранг: ROW_NUMBER, RANK и DENSE_RANK](#4-сравнение-на-функциите-за-ранг-row_number-rank-и-dense_rank)
* [5. Чести грешки, Капани и Как да ги избегнем](#5-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. АГРЕГАТНИ ФУНКЦИИ И РАБОТА С NULL СТОЙНОСТИ

Агрегатните функции взимат множество редове и връщат **една-единствена обобщена стойност**.

| Функция | Какво изчислява? | Как третира `NULL`? | Пример |
| :--- | :--- | :--- | :--- |
| **`COUNT(*)`** | Брои **всички редове** в групата (включително редове с `NULL`). | Брои ги. | `SELECT COUNT(*) FROM Users;` |
| **`COUNT(col)`** | Брои само редовете, където `col` **НЕ е `NULL`**. | Игнорира `NULL`. | `SELECT COUNT(PhoneNumber) FROM Users;` |
| **`COUNT(DISTINCT col)`** | Брои само **уникалните** не-`NULL` стойности. | Игнорира `NULL`. | `SELECT COUNT(DISTINCT City) FROM Users;` |
| **`SUM(col)`** | Сумира числовите стойности. | Игнорира `NULL` (ако всички са `NULL`, връща `NULL`). | `SELECT COALESCE(SUM(Total), 0) FROM Orders;` |
| **`AVG(col)`** | Средно аритметично (`SUM / COUNT(col)`). | **Игнорира `NULL`** (не дели на редовете с `NULL`!). | `SELECT AVG(Grade) FROM Exams;` |
| **`MIN(col)` / `MAX(col)`** | Най-малката / най-голямата стойност (числа, дати, текст). | Игнорира `NULL`. | `SELECT MIN(Price), MAX(Price) FROM Products;` |
| **`STRING_AGG(col, sep)`** | Слепва текстове от множество редове в един низ с разделител. | Игнорира `NULL`. | `SELECT STRING_AGG(Name, ', ') FROM Tags;` |

---

## 2. GROUP BY И HAVING — СВИВАНЕ НА РЕДОВЕТЕ В ГРУПИ

Когато използваш `GROUP BY`, базата обединява редовете с еднакви стойности в един обобщен ред.
* **`WHERE`** филтрира индивидуалните редове **преди** групирането.
* **`HAVING`** филтрира самите групи **след** изчисляване на агрегатите.

```sql
-- ============================================================
-- 1. GROUP BY + HAVING + STRING_AGG
-- ============================================================
-- Задача: За всеки отдел изведи броя служители, средната заплата
-- и списък с имената им, но САМО за отдели с над 3 души и бюджет > 10 000 лв.

SELECT 
    Department,
    COUNT(Id) AS EmployeesCount,
    AVG(Salary) AS AverageSalary,
    SUM(Salary) AS TotalDepartmentBudget,
    STRING_AGG(FullName, N', ') WITHIN GROUP (ORDER BY FullName ASC) AS TeamMembers
FROM Employees
WHERE IsActive = 1                          -- 1. Махаме неактивните служители (РЕДОВЕ)
GROUP BY Department                         -- 2. Групираме останалите по отдел
HAVING COUNT(Id) > 3                        -- 3. Оставяме само отдели с > 3 души (ГРУПИ)
   AND SUM(Salary) > 10000
ORDER BY TotalDepartmentBudget DESC;
```

---

## 3. WINDOW FUNCTIONS (OVER) — ИЗЧИСЛЕНИЯ БЕЗ СМАЧКВАНЕ НА РЕДОВЕТЕ

**Window Functions (Прозоречни функции)** са едно от най-силните оръжия в модерния SQL. За разлика от `GROUP BY`, те **НЕ смачкват редовете** — всеки ред остава видим с всичките си детайли, но до него се добавя изчислена колона върху „прозорец“ от сродни редове.

### Анатомия на клаузата `OVER (...)`:
* **`PARTITION BY col`:** Разделя таблицата на логически дялове (прозорци) — например по `Department` или `CustomerId` (като `GROUP BY`, но без да свива редовете). Ако го пропуснеш, прозорецът е цялата таблица.
* **`ORDER BY col`:** Подрежда редовете вътре във всеки прозорец (задължително за `ROW_NUMBER`, `RANK`, `LAG`, `LEAD` и кумулативни суми).

```sql
-- ============================================================
-- 2. WINDOW FUNCTIONS: СРАВНЕНИЕ НА СЛУЖИТЕЛЯ С НЕГОВИЯ ОТДЕЛ
-- ============================================================
SELECT 
    FullName,
    Department,
    Salary,
    -- Средна заплата само за неговия отдел (без да губим реда на служителя!):
    AVG(Salary) OVER (PARTITION BY Department) AS DeptAvgSalary,
    -- Разлика спрямо средната за отдела:
    Salary - AVG(Salary) OVER (PARTITION BY Department) AS DiffFromAvg,
    -- Кумулативна сума (Running Total) на заплатите по ред на наемане:
    SUM(Salary) OVER (PARTITION BY Department ORDER BY HireDate ASC) AS RunningBudget
FROM Employees
WHERE IsActive = 1;
```

### Полезни аналитични прозоречни функции (`LAG` и `LEAD`):
* **`LAG(col, 1)`:** Взима стойността от **предходния ред** в прозореца (идеално за сравнение „тази поръчка спрямо предишната поръчка на същия клиент“).
* **`LEAD(col, 1)`:** Взима стойността от **следващия ред** в прозореца.

```sql
-- ============================================================
-- 3. LAG(): РАЗЛИКА СПРЯМО ПРЕДИШНАТА ПОРЪЧКА НА КЛИЕНТА
-- ============================================================
SELECT 
    CustomerId,
    OrderDate,
    TotalAmount AS CurrentOrder,
    LAG(TotalAmount, 1, 0) OVER (PARTITION BY CustomerId ORDER BY OrderDate) AS PreviousOrder,
    TotalAmount - LAG(TotalAmount, 1, 0) OVER (PARTITION BY CustomerId ORDER BY OrderDate) AS Difference
FROM Orders;
```

---

## 4. СРАВНЕНИЕ НА ФУНКЦИИТЕ ЗА РАНГ: ROW_NUMBER, RANK И DENSE_RANK

Когато правиш класации (напр. *„Най-скъпите 3 продукта във всяка категория“*), разликата между трите функции е при **равни стойности (Ties)**:

Нека имаме продукти с цени: `100 лв.`, `100 лв.`, `80 лв.`, `50 лв.`:

| Цена | `ROW_NUMBER()` (Уникални поредни номера) | `RANK()` (Пропуска номера при равенство) | `DENSE_RANK()` (Не пропуска номера при равенство) |
| :--- | :--- | :--- | :--- |
| `100` | **1** | **1** | **1** |
| `100` | **2** | **1** *(равенство)* | **1** *(равенство)* |
| `80` | **3** | **3** *(пропуска 2!)* | **2** *(продължава с 2)* |
| `50` | **4** | **4** | **3** |

```sql
-- ============================================================
-- 4. КЛАСИЧЕСКА ЗАДАЧА ЗА ИНТЕРВЮ: ТОП 2 НАЙ-СКЪПИ ПРОДУКТА НА КАТЕГОРИЯ
-- ============================================================
WITH RankedProducts AS (
    SELECT 
        CategoryId,
        Name,
        Price,
        DENSE_RANK() OVER (PARTITION BY CategoryId ORDER BY Price DESC) AS PriceRank
    FROM Products
)
SELECT CategoryId, Name, Price, PriceRank
FROM RankedProducts
WHERE PriceRank <= 2; -- Филтрираме прозоречната функция извън CTE!
```

---

## 5. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Използване на `ROW_NUMBER()` директно в `WHERE` клаузата
* **Какво се случва:** Пишеш `SELECT Name, ROW_NUMBER() OVER (ORDER BY Price DESC) AS rn FROM Products WHERE rn <= 5`. Базата гърми с грешка *Windowed functions can only appear in the SELECT or ORDER BY clauses* (или *Invalid column name 'rn'*), защото `WHERE` се изпълнява **преди** `SELECT` и прозоречните функции!
* **Как да избегнеш:** Винаги обгръщай прозоречната функция в **`CTE` (`WITH ...`)** и прави филтрирането `WHERE rn <= 5` във външния `SELECT`.

### Грешка 2: Изпускане на колона в `GROUP BY`
* **Какво се случва:** Пишеш `SELECT Department, FullName, AVG(Salary) FROM Employees GROUP BY Department`. Базата хвърля грешка *Column 'Employees.FullName' is invalid in the select list because it is not contained in either an aggregate function or the GROUP BY clause*.
* **Как да избегнеш:** Всяка колона в `SELECT`, която не е обвита в `SUM / COUNT / AVG / MIN / MAX`, **задължително** трябва да присъства в `GROUP BY`. Ако ти трябва индивидуалното име на служителя до средната заплата, не ползвай `GROUP BY`, а ползвай `AVG(Salary) OVER (PARTITION BY Department)`.

### Грешка 3: Грешно смятане на средна оценка при `NULL` стойности (`AVG`)
* **Какво се случва:** Студент има 3 изпита: `6.00`, `6.00` и `NULL` (не се е явил). `AVG(Grade)` връща **`6.00`** (`12 / 2`), защото игнорира `NULL`! Ако по бизнес правила неявяването се брои за `2.00`, средният му успех трябва да е `4.67`.
* **Как да избегнеш:** Когато `NULL` означава нула или слаба оценка, обвивай колоната преди агрегата: `AVG(COALESCE(Grade, 2.00))`.

### Грешка 4: Липсващ `ORDER BY` вътре в `OVER (...)` при `ROW_NUMBER()`
* **Какво се случва:** В PostgreSQL можеш да напишеш `ROW_NUMBER() OVER ()`, но редът на номериране е напълно случаен и непредвидим; в MS SQL Server това директно хвърля синтактична грешка (*The function 'ROW_NUMBER' must have an OVER clause with ORDER BY*).
* **Как да избегнеш:** Винаги указвай ясен `ORDER BY` критерий вътре в скобите на `OVER (...)` (и добавяй `Id` накрая като tie-breaker, напр. `ORDER BY Price DESC, Id ASC`, за детерминиран резултат).

---
⬅️ [03-02. Subqueries и CTE](03-02-SQL-Subqueries-And-CTEs.md) | 📂 [Глава 03](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [03-04. Вградени функции и Сет оператори](03-04-SQL-BuiltIn-Functions-And-Set-Operators.md)

© 2026 Dev-Library / Бази данни: Групиране, Агрегации и Window Functions
