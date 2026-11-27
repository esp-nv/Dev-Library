[⬅️ Назад към Инженерната Библиотека (01-Theory-Notes)](../README.md) | [📂 Предишен домейн: 03. Backend Ecosystems](../03-Backend-Ecosystems/README.md) | [🏠 Начало](../../README.md) | [📂 Следващ домейн: 05. QA & Test Automation](../05-QA-And-Test-Automation/README.md) | [▶️ Започни от SQL & NoSQL](Databases-SQL-And-NoSQL/README.md)

---

# 🗄️ 04. Бази Данни, ORM Системи и Кеширане (Databases & Caching)

Добре дошли в **Домейн 04** на **Dev-Library**. Този сектор обхваща целия жизнен цикъл на работа с данни в съвременните софтуерни системи — от релационни бази данни (**MS SQL Server, PostgreSQL, MySQL, SQLite**) и **NoSQL** хранилища (**MongoDB, Cassandra, Vector & Graph DBs**), през **Entity Framework Core** за C# / .NET, до високоскоростно кеширане в паметта с **Redis**.

## 🧭 Карта на Подмодулите

* **[01. Релационни и NoSQL Бази Данни (`Databases-SQL-And-NoSQL/`)](Databases-SQL-And-NoSQL/README.md)**
  * *Обхваща:* СУБД архитектура, SQL логика и ред на изпълнение, нормализация, релации, сложни JOIN заявки, Window функции, B-Tree индекси, Execution Plans, ACID трансакции, NoSQL моделиране (MongoDB, Cassandra, Vector DB) и OLTP срещу OLAP.
  1. **[00 & 01. DBMS Архитектура, SQL Основи & NoSQL Преглед](Databases-SQL-And-NoSQL/00-01-DBMS-Architecture-Backups-And-Tools.md):** Типове данни, диалекти (MSSQL/Postgres), MongoDB, Redis, Kafka и Vector Search.
  2. **[02. Моделиране на Данни и Нормализация (1NF–3NF)](Databases-SQL-And-NoSQL/02-00-DATA-MODELING-OVERVIEW.md):** Таблични релации, ключове, ограничения и NoSQL документно моделиране.
  3. **[03. Напреднали SQL Заявки (JOIN, CTE & Window Functions)](Databases-SQL-And-NoSQL/03-00-SQL-QUERIES-OVERVIEW.md):** Подзаявки, рекурсивни CTE, агрегации и аналитични функции.
  4. **[04. Производителност, Индекси и Трансакции (ACID)](Databases-SQL-And-NoSQL/04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md):** Индексиране, оптимизация на заявки, нива на изолация, Stored Procedures и ADO.NET/Dapper.

* **[02. Entity Framework Core & .NET ORM (`Databases-And-EF-Core/`)](Databases-And-EF-Core/README.md)**
  * *Обхваща:* Code-First и Database-First подходи, Mapping стратегии (**Convention vs. Attributes vs. Fluent API**), `ApplicationDbContext`, миграции, Change Tracking, Global Query Filters (Soft Delete), Interceptors, N+1 оптимизация и стъпка-по-стъпка карта за изграждане на проект.
  1. **[01. SQL Server Релации и Трансакции в .NET контекст](Databases-And-EF-Core/01-00-SQL-SERVER-AND-RELATIONS-INTRO.md):** Базов преговор на релациите и типовете преди работа с EF Core.
  2. **[02. EF Core Code-First, Fluent API & LINQ Заявки](Databases-And-EF-Core/02-00-EF-CORE-AND-CODE-FIRST-INTRO.md):** Конфигурация на модели, миграции, мулти-базови настройки и сложни LINQ операции.
  3. **[03. Напреднали DbContext Защити (Soft Delete & Interceptors)](Databases-And-EF-Core/03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md):** Глобални филтри, прихващане на `SaveChanges` и Cascade Delete ограничения.
  4. **[04-95. Project Workflow: Карта Стъпка по Стъпка (Entity ➔ Mapping ➔ LINQ ➔ SQL)](Databases-And-EF-Core/04-95-Project-Workflow-And-Step-By-Step-Map.md):** Практическо ръководство за писане на EF Core слой в реален проект.

* **[03. Кеширане в Паметта с Redis (`Redis-Caching/`)](Redis-Caching/README.md)** *(Премести `03-02-00` и `03-02-01` тук)*
  * *Обхваща:* In-Memory Key-Value архитектура, структури от данни в Redis (Strings, Hashes, Lists, Sets, Sorted Sets), стратегии за кеширане (**Cache-Aside**, Write-Through), TTL управление и предотвратяване на `KEYS *` блокиране в продукция.
  1. **[01-00. Redis Caching Архитектура & Стратегии](Redis-Caching/03-02-00-Redis-Caching-Architecture.md):** RAM срещу Disk, Cache-Aside модел, Eviction политики и капани.
  2. **[01-01. Redis Синтаксис от Нулата & .NET / CLI Cheat Sheet](Redis-Caching/03-02-01-Redis-Syntax-From-Scratch.md):** Команди в `redis-cli`, работа с `IDistributedCache` / `StackExchange.Redis` и справочна таблица.

---

[⬅️ Назад към Инженерната Библиотека (01-Theory-Notes)](../README.md) | [📂 Предишен домейн: 03. Backend Ecosystems](../03-Backend-Ecosystems/README.md) | [🏠 Начало](../../README.md) | [📂 Следващ домейн: 05. QA & Test Automation](../05-QA-And-Test-Automation/README.md) | [▶️ Започни от SQL & NoSQL](Databases-SQL-And-NoSQL/README.md)

© 2026 Dev-Library / 04. Бази Данни, ORM Системи и Кеширане
