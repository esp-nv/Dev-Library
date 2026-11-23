⏪ [Глава 03. Дълбок SQL и Заявки](03-00-SQL-QUERIES-OVERVIEW.md) | 📂 Глава 04 | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-01. Индекси и Execution Plans](04-01-Indices-And-Execution-Plans.md) 

---

# 04-00. Производителност, Програмируемост и .NET Интеграция

**Цел:** Бърз навигационен справочник за оптимизация и продукционна работа с бази данни — B-Tree индекси, ускоряване на заявки, транзакции и ACID, изгледи/процедури/тригери, C# достъп (`ADO.NET` ➔ `Dapper` ➔ `EF Core` релации и `ModelBuilder`) и разликата между OLTP и OLAP.

## Съдържание на темата
* [1. Матрица за диагностика и избор на решение](#1-матрица-за-диагностика-и-избор-на-решение)
* [2. Петте златни правила за продукционни бази данни](#2-петте-златни-правила-за-продукционни-бази-данни)
* [3. Детайлни подмодули към Глава 04](#3-детайлни-подмодули-към-глава-04)

---

## 1. МАТРИЦА ЗА ДИАГНОСТИКА И ИЗБОР НА РЕШЕНИЕ

| Проблем или архитектурна задача | Правилен инструмент / подход | Къде да го намериш? |
| :--- | :--- | :--- |
| Заявката чете цялата таблица (`Table Scan`) вместо да скача директно. | **B-Tree Index** (`Non-Clustered` или `Covering` с `INCLUDE`). | [04-01. Индекси и Execution Plans](04-01-Indices-And-Execution-Plans.md) |
| Заявката бави заради `YEAR(Date)` в `WHERE` или дълбока пагинация `OFFSET`. | **SARGable условия** и **Keyset (Cursor) Pagination**. | [04-02. Оптимизация на заявки](04-02-SQL-Query-And-Architecture-Optimization.md) |
| Банков превод или поръчка с 3 таблици трябва да мине „всичко или нищо“. | **Транзакция (`BEGIN TRAN` + `TRY...CATCH`)** + `RCSI`. | [04-03. Транзакции и ACID](04-03-Transactions-ACID-And-Isolation-Levels.md) |
| Искаш да скриеш сложни `JOIN`-ове, да ограничиш колони или да водиш одит. | **`VIEW`**, **`Stored Procedure`** или **`AFTER` Trigger**. | [04-04. Views, Procedures и Triggers](04-04-Views-Functions-Procedures-Triggers.md) |
| Трябва ти светкавичен отчет в C# без тежестта на Change Tracker-а. | **Dapper (`QueryAsync<T>`)** или EF Core **`.AsNoTracking()`**. | [04-05. C# Data Access (ADO.NET и Dapper)](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md) |
| Искаш да конфигурираш `1:1`, `1:Many`, `Many:Many` и `JOIN` в ASP.NET Core. | **`DbContext`**, **`ModelBuilder` (Fluent API)** и LINQ `.Select()` / `.Include()`. | [04-07. EF Core Релации и Joins](04-07-EFCore-Relations-ModelBuilder-And-Joins.md) |
| Тежки Power BI анализи натоварват базата на онлайн магазина. | Отделяне на **OLTP** (сайта) от **OLAP / Data Warehouse** (`Star Schema`). | [04-06. OLTP срещу OLAP](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md) |

---

## 2. ПЕТТЕ ЗЛАТНИ ПРАВИЛА ЗА ПРОДУКЦИОННИ БАЗИ ДАННИ

1. **Индексирай умно, не сляпо:** Всеки индекс ускорява `SELECT`, но забавя `INSERT/UPDATE/DELETE`. Винаги индексирай `Foreign Keys` и следвай правилото на левия префикс при съставни индекси.
2. **Дръж транзакциите кратки и в еднакъв ред:** Никога не прави HTTP заявки вътре в SQL транзакция и винаги обновявай таблиците в еднакъв ред, за да предотвратиш `Deadlocks`.
3. **Защитавай се от SQL Injection и течове на връзки:** В C# винаги ползвай параметризирани заявки (`@Param`) и `await using` за връзките към базата.
4. **Не прави `SELECT *` в EF Core без нужда:** Използвай `.AsNoTracking()` и `.Select(x => new Dto(...))` за четене, за да генерираш тесни, бързи `JOIN` заявки.
5. **Разделяй оперативната работа от тежките отчети:** Оперативната база (`OLTP`) трябва да обслужва клиентите за милисекунди; тежките анализи отиват в `Read Replica` или `Data Warehouse (OLAP)`.

---

## 3. ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ГЛАВА 04

* **[04-01. Индекси (B-Tree) и Четене на Execution Plans](04-01-Indices-And-Execution-Plans.md)** — B+ Tree структура, Clustered срещу Non-Clustered, Covering индекси (`INCLUDE`) и четене на `Seek` срещу `Scan`.
* **[04-02. Оптимизация на заявки, таблици и архитектура](04-02-SQL-Query-And-Architecture-Optimization.md)** — SARGable заявки, Keyset Pagination, Partitioning, Read Replicas и решаване на `N+1` проблема в C#.
* **[04-03. Транзакции, ACID свойства и Нива на изолация](04-03-Transactions-ACID-And-Isolation-Levels.md)** — `BEGIN TRAN / COMMIT / ROLLBACK`, `TRY...CATCH`, нива на изолация, MVCC (`RCSI`) и предотвратяване на `Deadlocks`.
* **[04-04. Views, Functions, Stored Procedures и Triggers](04-04-Views-Functions-Procedures-Triggers.md)** — Кога пишем `VIEW`, Inline Table-Valued Functions, Stored Procedures с `OUTPUT` и Set-Based одит тригери.
* **[04-05. Достъп до данни в C#: ADO.NET ➔ Dapper ➔ EF Core](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md)** — `SqlConnection`, `SqlCommand`, `SqlDataReader`, Dapper `QueryAsync` / `ExecuteAsync` и хибриден CQRS подход.
* **[04-06. OLTP срещу OLAP и Data Warehouses](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md)** — Оперативни срещу аналитични бази, `Star Schema` (`Fact` и `Dimension` таблици), `Columnstore` и `ETL / ELT`.
* **[04-07. Релации и Joins в ASP.NET Core (DbContext, ModelBuilder и LINQ)](04-07-EFCore-Relations-ModelBuilder-And-Joins.md)** — Конфигуриране на `1:1`, `1:Many`, `Many:Many` и `DeleteBehavior` в `OnModelCreating`, както и превод на `.Include()`, `.Select()` и `DefaultIfEmpty()` към SQL `INNER` и `LEFT JOIN`.

---
⏪ [Глава 03. Дълбок SQL и Заявки](03-00-SQL-QUERIES-OVERVIEW.md) | 📂 Глава 04 | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-01. Индекси и Execution Plans](04-01-Indices-And-Execution-Plans.md) 

© 2026 Dev-Library / Бази данни: Производителност и Програмируемост
