⬅️ [03-03. Групиране, Агрегации и Window Functions](03-03-SQL-Grouping-Aggregations-Window-Functions.md) | 📂 [Глава 03](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [Глава 04. Производителност и Програмируемост](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md)

---

# 03-04. Вградени функции (Текст, Дати, CASE WHEN) и Сет оператори (UNION, INTERSECT, EXCEPT)

**Цел:** Как да обработваме текстове и дати директно в SQL, как да пишем условна логика (`CASE WHEN`, `COALESCE`, `NULLIF`) и как да обединяваме или сравняваме резултатите от две заявки вертикално (`UNION`, `UNION ALL`, `INTERSECT`, `EXCEPT`).

## Съдържание на темата
* [1. Текстови функции (String Functions)](#1-текстови-функции-string-functions)
* [2. Функции за дата и час (Date & Time Functions)](#2-функции-за-дата-и-час-date--time-functions)
* [3. Условна логика и NULL защита (CASE WHEN, COALESCE, NULLIF)](#3-условна-логика-и-null-защита-case-when-coalesce-nullif)
* [4. Сет оператори (UNION, UNION ALL, INTERSECT, EXCEPT)](#4-сет-оператори-union-union-all-intersect-except)
* [5. Чести грешки, Капани и Как да ги избегнем](#5-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. ТЕКСТОВИ ФУНКЦИИ (STRING FUNCTIONS)

| Функция | Какво прави? | MS SQL Server | PostgreSQL | Пример и резултат |
| :--- | :--- | :--- | :--- | :--- |
| **`CONCAT(a, b, ...)`** | Слепва низове (третира `NULL` като празен низ `''`). | `CONCAT(First, ' ', Last)` | `CONCAT(first, ' ', last)` | `'Иван Петров'` |
| **`SUBSTRING(...)`** | Изрязва част от текст *(в SQL броенето започва от **1**, не от 0!)*. | `SUBSTRING('Sofia', 1, 3)` | `SUBSTRING('Sofia' FROM 1 FOR 3)` | `'Sof'` |
| **`LEFT` / `RIGHT`** | Взима първите / последните `N` символа. | `LEFT('BG1234', 2)` | `LEFT('BG1234', 2)` | `'BG'` |
| **`TRIM` / `LTRIM` / `RTRIM`** | Премахва излишните интервали в началото и края. | `TRIM('  Ivan  ')` | `TRIM('  Ivan  ')` | `'Ivan'` |
| **`UPPER` / `LOWER`** | Превръща в главни / малки букви. | `LOWER('Admin@BG')` | `LOWER('Admin@BG')` | `'admin@bg'` |
| **`REPLACE(str, old, new)`** | Заменя всички срещания на подниз с нов. | `REPLACE('088-123', '-', '')` | `REPLACE('088-123', '-', '')` | `'088123'` |
| **`LEN` / `LENGTH`** | Връща броя символи в текста. | `LEN(N'София')` | `LENGTH('София')` | `5` |

```sql
-- ============================================================
-- 1. ПРАКТИЧЕСКИ ПРИМЕР С ТЕКСТОВИ ФУНКЦИИ
-- ============================================================
-- Задача: Изчисти интервалите, направи имейла с малки букви и вземи домейна след '@'.
SELECT 
    TRIM(FullName) AS CleanName,
    LOWER(TRIM(Email)) AS NormalizedEmail,
    SUBSTRING(Email, CHARINDEX('@', Email) + 1, LEN(Email)) AS EmailDomain
FROM Customers;
```

---

## 2. ФУНКЦИИ ЗА ДАТА И ЧАС (DATE & TIME FUNCTIONS)

Работата с дати има различен синтаксис в MS SQL Server и PostgreSQL:

| Операция | MS SQL Server (T-SQL) | PostgreSQL | Какво връща? |
| :--- | :--- | :--- | :--- |
| **Текущо UTC време** | `GETUTCDATE()` / `SYSUTCDATETIME()` | `NOW() AT TIME ZONE 'UTC'` | Текущата дата и час в UTC. |
| **Добавяне / Изваждане** | `DATEADD(DAY, -7, GETUTCDATE())` | `NOW() - INTERVAL '7 days'` | Датата преди точно 7 дни. |
| **Разлика между две дати** | `DATEDIFF(DAY, StartDate, EndDate)` | `EXTRACT(DAY FROM (EndDate - StartDate))` | Брой дни между двете дати. |
| **Изваждане на година/месец** | `YEAR(OrderDate)`, `MONTH(OrderDate)`, `DATEPART(WEEKDAY, OrderDate)` | `EXTRACT(YEAR FROM order_date)`, `EXTRACT(MONTH FROM order_date)` | Цяло число (`2026`, `11`). |
| **Форматиране / Отрязване** | `CAST(CreatedAt AS DATE)` | `created_at::DATE` *(или `DATE_TRUNC('month', created_at)`)* | Премахва часа и оставя само датата. |

```sql
-- ============================================================
-- 2. ФИЛТРИРАНЕ ПО ДАТИ (MS SQL SERVER)
-- ============================================================
-- Задача: Вземи всички поръчки от последните 30 дни и сметни преди колко дни са направени.
SELECT 
    Id,
    OrderDate,
    DATEDIFF(DAY, OrderDate, GETUTCDATE()) AS DaysAgo
FROM Orders
WHERE OrderDate >= DATEADD(DAY, -30, GETUTCDATE())
ORDER BY OrderDate DESC;
```

---

## 3. УСЛОВНА ЛОГИКА И NULL ЗАЩИТА (CASE WHEN, COALESCE, NULLIF)

* **`CASE WHEN ... THEN ... ELSE ... END`:** Аналог на `if / else if / else` или `switch` в C#. Може да се ползва в `SELECT`, `ORDER BY` и дори в `GROUP BY`.
* **`COALESCE(val1, val2, ...)`:** Връща **първата стойност, която НЕ е `NULL`**.
* **`NULLIF(a, b)`:** Връща `NULL`, ако `a = b`, иначе връща `a`. Незаменим трик за **защита от деление на нула** (`Sales / NULLIF(ItemsCount, 0)`).

```sql
-- ============================================================
-- 3. CASE WHEN, COALESCE И NULLIF В ДЕЙСТВИЕ
-- ============================================================
SELECT 
    FullName,
    TotalSpent,
    -- 1. Категоризиране на клиенти (if / else if / else):
    CASE 
        WHEN TotalSpent >= 2000 THEN N'VIP Платинен'
        WHEN TotalSpent >= 500  THEN N'Златен клиент'
        ELSE N'Стандартен клиент'
    END AS CustomerTier,
    -- 2. Защита от NULL при телефон:
    COALESCE(PhoneNumber, N'Не е посочен') AS ContactPhone,
    -- 3. Безопасно деление без риск от грешка "Divide by zero":
    TotalSpent / NULLIF(OrdersCount, 0) AS AverageOrderValue
FROM Customers;
```

---

## 4. СЕТ ОПЕРАТОРИ (UNION, UNION ALL, INTERSECT, EXCEPT)

Докато `JOIN` свързва таблиците **хоризонтално** (добавя нови колони вдясно), **Сет операторите** обединяват резултатите от две `SELECT` заявки **вертикално** (един под друг).

### Задължителни правила за всички Сет оператори:
1. Двете `SELECT` заявки трябва да имат **еднакъв брой колони**.
2. Колоните на съответните позиции трябва да имат **съвместими типове данни**.
3. Имената на колоните в крайния резултат се взимат от **първия `SELECT`**, а `ORDER BY` се пише **само веднъж в самия край**.

| Оператор | Какво прави с редовете от Заявка А и Заявка Б? | Премахва ли дубликати? | Скорост |
| :--- | :--- | :--- | :--- |
| **`UNION ALL`** | Взима **всички редове** от А и всички от Б директно. | **Не** (запазва дубликатите). | **Най-бърз** (не сортира за дубликати). |
| **`UNION`** | Обединява редовете от А и Б, но **изчиства повтарящите се**. | **Да** (прави скрит `DISTINCT`). | По-бавен от `UNION ALL`. |
| **`INTERSECT`** | Връща **само редовете, които присъстват И в А, И в Б**. | **Да** | Средна (сравнява множествата). |
| **`EXCEPT`** | Връща редовете, които ги има в **А, но ги НЯМА в Б** (`A - B`). | **Да** | Средна (идеален за сравнение на данни). |

```sql
-- ============================================================
-- 4. СЕТ ОПЕРАТОРИ: ОБЕДИНЯВАНЕ И СРАВНЕНИЕ НА МНОЖЕСТВА
-- ============================================================

-- А. UNION ALL: Общ списък с контакти от две различни таблици (Клиенти + Доставчици)
SELECT FullName AS ContactName, Email, N'Клиент' AS RoleType
FROM Customers
WHERE IsActive = 1

UNION ALL

SELECT CompanyName AS ContactName, ContactEmail AS Email, N'Доставчик' AS RoleType
FROM Suppliers
WHERE IsActive = 1
ORDER BY ContactName ASC; -- Само един ORDER BY най-отдолу!

-- Б. EXCEPT: Кои продукти ги има в каталога, но НИКОГА не са поръчвани?
SELECT Id FROM Products
EXCEPT
SELECT DISTINCT ProductId FROM OrderItems;
```

---

## 5. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Използване на `UNION` вместо `UNION ALL` по навик
* **Какво се случва:** Обединяваш две големи заявки (напр. архивни поръчки от 2025 г. и текущи поръчки от 2026 г.), където по дефиниция няма как да има дубликати, но пишеш `UNION`. Базата прави тежко сортиране върху милиони редове, за да търси несъществуващи дубликати.
* **Как да избегнеш:** Винаги ползвай **`UNION ALL`** по подразбиране. Използвай `UNION` само когато изрично искаш да премахнеш дублиращи се редове.

### Грешка 2: Филтриране на колона с функция в `WHERE` (Убиване на индекса — Non-SARGable)
* **Какво се случва:** Искаш поръчките от 2026 г. и пишеш `WHERE YEAR(OrderDate) = 2026` (или `WHERE LOWER(Email) = 'ivan@abv.bg'`). Когато обвиеш колоната във функция отляво на `=`, базата **не може да използва индекса** върху `OrderDate` и сканира цялата таблица ред по ред!
* **Как да избегнеш:** Остави колоната чиста и премести изчислението отдясно на знака за сравнение:
  ```sql
  -- ГРЕШНО (бавно — чупи индекса):
  WHERE YEAR(OrderDate) = 2026

  -- ПРАВИЛНО (светкавично — ползва индекса върху OrderDate):
  WHERE OrderDate >= '2026-01-01' AND OrderDate < '2027-01-01'
  ```

### Грешка 3: Липсващ `ELSE` в `CASE WHEN`
* **Какво се случва:** Написваш `CASE WHEN Grade >= 5.50 THEN 'Отличен' END` без `ELSE`. За всички студенти с оценка под 5.50 базата връща **`NULL`**, което може да счупи C# мапинга, ако свойството в C# е `string` (non-nullable).
* **Как да избегнеш:** Винаги слагай изрична `ELSE` клауза в края на всеки `CASE WHEN`.

### Грешка 4: Броене на индекси в `SUBSTRING` от 0 (C# навик)
* **Какво се случва:** В C# първият символ на низа е на индекс `0` (`str.Substring(0, 3)`). В SQL (T-SQL и PostgreSQL) първият символ е на позиция **`1`** (`SUBSTRING(str, 1, 3)`). Ако подадеш `0`, в T-SQL ще изгубиш първия символ!
* **Как да избегнеш:** Запомни, че в SQL позициите в низове (`SUBSTRING`, `CHARINDEX`, `POSITION`) винаги започват от **1**.

---
⬅️ [03-03. Групиране, Агрегации и Window Functions](03-03-SQL-Grouping-Aggregations-Window-Functions.md) | 📂 [Глава 03](03-00-SQL-QUERIES-OVERVIEW.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [Глава 04. Производителност и Програмируемост](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md)

© 2026 Dev-Library / Бази данни: Вградени функции и Сет оператори
