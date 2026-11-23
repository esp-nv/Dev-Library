🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🧭 [Архитектурен Атлас](../Architecture-And-Cross-Stack/) | 💾 [Entity Framework Core](../Databases-And-EF-Core/) | ▶️ [Започни от 00-01](00-01-DBMS-Architecture-Backups-And-Tools.md)

---

# 🗄️ Databases (SQL & NoSQL) – Архитектурна Библиотека

Пълна справочна база по релационни и нерелационни бази данни (MS SQL Server, PostgreSQL, MySQL, SQLite, MongoDB, Redis, Cassandra) и интеграцията им с **C# / .NET**, структурирана модулно с прегледни справочници (`*-OVERVIEW.md` / `01-00...`) и детайлни подмодули с примери и капани.

## 🧭 Навигация и Съдържание

### 🔹 Въведение: Архитектура на СУБД и Администриране
* **00-01. Архитектура на СУБД, Инструменти, Бекъпи и Миграции**
  * 🛠️ [Справочник и Команди](00-01-DBMS-Architecture-Backups-And-Tools.md)
  * *Обхваща:* Системни бази (`master`, `model`, `msdb`, `tempdb`), клиенти (SSMS, pgAdmin, DBeaver), Docker команди, бекъпи (`.bak`, `pg_dump`) и миграции (PMC `Add-Migration`, `.NET CLI` и `Script-Migration`).

---

### 🔹 Модул 1: Основи на SQL, Типове данни и Видове БД
* **01-00. Въведение в БД, Типове данни и CRUD операции**
  * ⚡ [Главен справочник на темата](01-00-SQL-INTRO-AND-CRUD.md)
  * *Обхваща:* DDL (`CREATE`, `ALTER`, `DROP`), DML (`SELECT`, `INSERT`, `UPDATE`, `DELETE`), DCL/TCL и базови правила.
  * **01-01. Логика на мислене и Ред на написване на SELECT**
    * 🪜 [Синтаксис и Логика](01-01-SQL-Logic-And-Select-Order.md)
    * *Обхваща:* Синтактичната „стълба“ срещу логическия ред на изпълнение (`FROM` ➔ `WHERE` ➔ `GROUP BY` ➔ `HAVING` ➔ `SELECT` ➔ `ORDER BY`).
  * **01-02. Типове данни, Заемане на памет и Мостът към C#**
    * 💾 [Памет и Типове](01-02-SQL-Data-Types-Memory-And-CSharp.md)
    * *Обхваща:* Размери в байтове, Data Pages (8 KB), Unicode (`N'...'`), `DECIMAL` срещу `FLOAT`, `Guid` и Nullable типове в C#.
  * **01-03. Релационни диалекти: MS SQL Server, PostgreSQL, MySQL и SQLite**
    * 🗄️ [Сравнение на диалекти](01-03-Relational-Dialects-MSSQL-Postgres-MySQL-SQLite.md)
    * *Обхваща:* Auto-Increment ключове, `ILIKE`, `utf8mb4`, пагинация (`OFFSET-FETCH` срещу `LIMIT`) и избор на база.
  * **01-04. NoSQL Бази данни: MongoDB и Apache Cassandra**
    * 🍃 [Document & Wide-Column](01-04-NoSQL-Documents-And-WideColumn-Mongo-Cassandra.md)
    * *Обхваща:* Превод на термините от SQL, MongoDB CRUD + C# `MongoDB.Driver` и Cassandra CQL.
  * **01-05. Redis Cache, Cloud NoSQL и Корпоративни системи**
    * ⚡ [RAM Кеш и Облак](01-05-NoSQL-InMemory-Cache-Redis.md)
    * *Обхваща:* Redis CLI и TTL, Cache-Aside в C#, Amazon DynamoDB срещу Azure Cosmos DB, Active Directory (Entra ID) и SAP.
  * **01-06. Формати за данни: JSON и XML**
    * 📦 [Формати в SQL и C#](01-06-Data-Formats-JSON-And-XML.md)
    * *Обхваща:* JSON срещу XML, `JSON_VALUE` / `OPENJSON` в MS SQL, `JSONB` в PostgreSQL и `System.Text.Json`.
  * **01-07. Съседи на Базата данни в архитектурата**
    * 🧩 [Опашки, API и Mobile](01-07-Architecture-Neighbors-RabbitMQ-Kafka-GraphQL-Enterprise.md)
    * *Обхваща:* RabbitMQ срещу Apache Kafka, GraphQL срещу REST API, Mobile/Android БД (`SQLite`, `Room`, `Realm`), AD и SAP.
  * **01-08. Специализирани Бази данни: Vector (AI), Search и Graph**
    * 🧠 [AI, Търсачки и Графове](01-08-Specialized-Databases-Vector-Search-Graph.md)
    * *Обхваща:* Vector БД (`pgvector`, Pinecone) за RAG, Elasticsearch, Graph БД (`Neo4j`) и Time-Series (`TimescaleDB`).

---

### 🔹 Модул 2: Проектиране и Моделиране на Данни
* **02-00. Проектиране и Моделиране на Бази Данни (Общ преглед)**
  * 🗺️ [Карта на Модул 02](02-00-DATA-MODELING-OVERVIEW.md)
  * *Обхваща:* Сравнение на подходите за моделиране при SQL, Document, Key-Value, Wide-Column и Graph бази.
  * **02-01. Връзки между таблици, Ключове и Ограничения**
    * 🔗 [PK, FK и Constraints](02-01-SQL-Table-Relations-And-Constraints.md)
    * *Обхваща:* Дефиниране на ключове по диалекти, връзки `1:1`, `1:Many`, `Many:Many` и `ON DELETE CASCADE / RESTRICT / SET NULL`.
  * **02-02. Нормализация на Бази Данни (1NF, 2NF, 3NF) и Денормализация**
    * 📐 [Нормални форми](02-02-SQL-Database-Normalization.md)
    * *Обхваща:* Аномалии при запис/триене, преминаване от `1NF` до `3NF` с примери „преди/след“ и кога умишлено денормализираме.
  * **02-03. Особености при моделиране в Document БД (MongoDB)**
    * 📄 [Embedding vs Referencing](02-03-Document-DB-Modeling-MongoDB.md)
    * *Обхваща:* Вграждане срещу рефериране, Extended Reference Pattern и защита от лимита от 16 MB.
  * **02-04. Моделиране в Key-Value (Redis) и Wide-Column (Cassandra)**
    * 🔑 [RAM Структури и Query-First](02-04-KeyValue-And-WideColumn-Modeling-Redis-Cassandra.md)
    * *Обхваща:* Именуване на ключове (`:`) и избор между `Hash`, `List`, `Set`, `Sorted Set` в Redis + `Partition Key` в Cassandra.
  * **02-05. Моделиране в Graph, Time-Series и Vector (AI) Бази данни**
    * 🕸️ [Nodes, Chunks и Embeddings](02-05-Graph-TimeSeries-Vector-Modeling.md)
    * *Обхваща:* Възли и връзки в Neo4j, Hypertables в TimescaleDB и Chunking + Metadata за сигурност във Vector БД.

---

### 🔹 Модул 3: Дълбок SQL (Заявки, Агрегации и Функции)
* **03-00. Дълбок SQL: Свързване, Подзаявки, Агрегации и Функции**
  * 🗺️ [Карта на Модул 03](03-00-SQL-QUERIES-OVERVIEW.md)
  * *Обхваща:* Матрица за бърз избор на правилния SQL инструмент според бизнес задачата.
  * **03-01. SQL Joins — Визуално и с Код**
    * 🔄 [INNER, LEFT, FULL, CROSS, SELF](03-01-SQL-Joins-Visual-And-Code.md)
    * *Обхваща:* Venn диаграми, алгоритъм за избор в 3 стъпки, Anti-Join (`WHERE o.Id IS NULL`) и капанът с `WHERE` при `LEFT JOIN`.
  * **03-02. Подзаявки (Subqueries) и CTE (Common Table Expressions)**
    * 🧩 [Subqueries & WITH](03-02-SQL-Subqueries-And-CTEs.md)
    * *Обхваща:* Скаларни и корелирани подзаявки, `IN` срещу `EXISTS`, четими `WITH` стъпки и рекурсивни CTE за йерархии.
  * **03-03. Групиране, Агрегации и Window Functions**
    * 📊 [GROUP BY & OVER()](03-03-SQL-Grouping-Aggregations-Window-Functions.md)
    * *Обхваща:* `COUNT`, `SUM`, `AVG`, `STRING_AGG`, `HAVING` и прозоречни функции (`ROW_NUMBER`, `RANK`, `DENSE_RANK`, `LAG`, `LEAD`).
  * **03-04. Вградени функции и Сет оператори**
    * 🧰 [Текст, Дати, CASE и UNION](03-04-SQL-BuiltIn-Functions-And-Set-Operators.md)
    * *Обхваща:* Текстови и датови функции, `CASE WHEN`, `COALESCE`, `NULLIF` и сет оператори (`UNION ALL`, `UNION`, `INTERSECT`, `EXCEPT`).

---

### 🔹 Модул 4: Производителност, Програмируемост и .NET Интеграция
* **04-00. Производителност, Програмируемост и .NET Интеграция**
  * 🗺️ [Карта на Модул 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md)
  * *Обхваща:* Матрица за диагностика на бавни заявки и продукционни правила.
  * **04-01. Индекси (B-Tree) и Четене на Execution Plans**
    * 🚀 [B-Tree & Seek vs Scan](04-01-Indices-And-Execution-Plans.md)
    * *Обхваща:* Clustered срещу Non-Clustered индекс, Composite & Covering (`INCLUDE`) индекси, Filtered индекси и `Key Lookup`.
  * **04-02. Оптимизация на заявки, таблици и архитектура**
    * ⚡ [Query & Architecture Tuning](04-02-SQL-Query-And-Architecture-Optimization.md)
    * *Обхваща:* SARGable заявки, Keyset (Cursor) Pagination, Partitioning, Read Replicas и решаване на `N+1` проблема.
  * **04-03. Транзакции, ACID свойства и Нива на изолация**
    * 🔒 [ACID, Isolation и Deadlocks](04-03-Transactions-ACID-And-Isolation-Levels.md)
    * *Обхваща:* `BEGIN TRAN / COMMIT / ROLLBACK`, `TRY...CATCH`, `READ COMMITTED`, `RCSI` (MVCC) и предотвратяване на `Deadlocks`.
  * **04-04. Views, Functions, Stored Procedures и Triggers**
    * ⚙️ [Програмируемост в БД](04-04-Views-Functions-Procedures-Triggers.md)
    * *Обхваща:* Кога пишем `VIEW`, Inline Table-Valued Functions, Stored Procedures с `OUTPUT` и Set-Based одит тригери.
  * **04-05. Достъп до данни в C#: ADO.NET ➔ Dapper ➔ EF Core**
    * 💻 [ADO.NET и Dapper](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md)
    * *Обхваща:* `SqlConnection`, `SqlCommand`, `SqlDataReader`, защита от SQL Injection, Dapper `QueryAsync` и хибриден CQRS подход.
  * **04-06. OLTP срещу OLAP и Data Warehouses**
    * 📈 [Data Warehouses & Star Schema](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md)
    * *Обхваща:* Оперативни срещу аналитични бази, `Fact` и `Dimension` таблици, `Columnstore` и `ETL / ELT`.
  * **04-07. Релации и Joins в ASP.NET Core (DbContext, ModelBuilder и LINQ)**
    * 🔌 [EF Core Fluent API & Joins](04-07-EFCore-Relations-ModelBuilder-And-Joins.md)
    * *Обхваща:* Конфигуриране на `1:1`, `1:Many`, `Many:Many` и `DeleteBehavior` в `OnModelCreating` + `INNER` и `LEFT JOIN` в LINQ.

---
🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🧭 [Архитектурен Атлас](../Architecture-And-Cross-Stack/) | 💾 [Entity Framework Core](../Databases-And-EF-Core/) | ▶️ [Започни от 00-01](00-01-DBMS-Architecture-Backups-And-Tools.md)

© 2026 Dev-Library / Пълна архитектурна библиотека по Бази данни
