# 🗄️ Databases (SQL & NoSQL) – Архитектурна Библиотека

Пълна справочна база по релационни и нерелационни бази данни (MS SQL Server, PostgreSQL, MySQL, SQLite, MongoDB, Redis), структурирана в **двоен стандарт**:

- ⚡ **Кратък справочник (`00` – `06`):** Бърз преговор, матрици на типове, синтактични сравнения и Junior капани.
- 📖 **Дълъг учебник (`*-db.md` / `*-csharp.md`):** Дълбока архитектура под капака (B-Tree индекси, ACID/BASE, Execution Plans), кодови сепаратори `// ====================` и реални примери.

---

## 🧭 Навигация и Съдържание

### 🔹 Модул 0: Cross-Stack Екосистема и Архитектура
- **00. Карта на типовете и технологиите (SQL ➔ C# / LINQ / ASP.NET ➔ TS / Python / Java)**
  - 🗺️ [Архитектурна карта](00-CROSS-STACK-TYPES-AND-ECOSYSTEM.md)
  - *Обхваща:* Превод на типове данни (`DECIMAL`, `DATETIME2`, `UNIQUEIDENTIFIER`), LINQ към SQL заявки и сравнение между уеб рамки.

### 🔹 Модул 1: Релационни бази данни (SQL Основи, Връзки и Заявки)
- **01. Въведение в БД, Типове данни и CRUD операции**
  - 📄 [Кратък справочник](01-SQL-INTRO-AND-CRUD.md) | 📘 [Дълъг учебник](sql-intro-crud-db.md)
  - *Обхваща:* DDL (`CREATE`, `ALTER`, `DROP`), DML (`SELECT`, `INSERT`, `UPDATE`, `DELETE`), Unicode текстове и финансови типове.
- **02. Връзки между таблици и Нормализация**
  - 📄 [Кратък справочник](02-TABLE-RELATIONS-NORMALIZATION.md) | 📘 [Дълъг учебник](table-relations-normalization-db.md)
  - *Обхваща:* Primary Key, Foreign Key, връзки `1:1`, `1:Many`, `Many:Many` (Mapping таблици), нормални форми (`1NF`–`3NF`) и `CASCADE` правила.
- **03. Joins, Подзаявки и CTE (Common Table Expressions)**
  - 📄 [Кратък справочник](03-JOINS-SUBQUERIES-CTE.md) | 📘 [Дълъг учебник](joins-subqueries-cte-db.md) | 📋 [Joins Cheat Sheet](03-01-SQL-Joins-Cheat-Sheet.md)
  - *Обхваща:* `INNER`, `LEFT`, `RIGHT`, `FULL` и `CROSS JOIN`, вложени заявки (`IN`, `EXISTS`) и четими `WITH` изрази.

### 🔹 Модул 2: Оптимизация, Индекси и Програмируемост
- **04. Вградени функции, Групиране и Индекси**
  - 📄 [Кратък справочник](04-FUNCTIONS-AGGREGATION-INDICES.md) | 📘 [Дълъг учебник](functions-aggregation-indices-db.md)
  - *Обхваща:* `GROUP BY`, `HAVING` срещу `WHERE`, агрегатни функции, Window Functions (`ROW_NUMBER`, `RANK`) и B-Tree индекси (Clustered vs Non-Clustered).
- **05. Функции, Съхранени процедури, Транзакции и Тригери**
  - 📄 [Кратък справочник](05-PROCEDURES-TRANSACTIONS-TRIGGERS.md) | 📘 [Дълъг учебник](programmability-transactions-db.md)
  - *Обхваща:* Потребителски функции, `CREATE PROCEDURE`, управление на транзакции (`COMMIT` / `ROLLBACK`), ACID и `AFTER` / `INSTEAD OF` тригери.

### 🔹 Модул 3: Мулти-базова архитектура (SQL срещу NoSQL)
- **06. Сравнение на бази данни: SQL срещу NoSQL и интеграция с C#**
  - 📄 [Кратък справочник](06-SQL-VS-NOSQL-COMPARISON.md) | 📘 [Дълъг учебник](databases-sql-nosql-csharp.md)
  - *Обхваща:* MS SQL Server, PostgreSQL, MySQL и SQLite срещу MongoDB и Redis, ACID срещу BASE и примери за връзка с C#.

---
© 2026 Dev-Library / Пълна архитектурна библиотека по Бази данни
