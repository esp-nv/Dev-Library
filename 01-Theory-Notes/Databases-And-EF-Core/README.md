🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🌐 [Архитектура и Cross-Stack](../Architecture-And-Cross-Stack/) | 🗄️ [Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | ▶️ [Започни от 01-00](01-00-SQL-SERVER-AND-RELATIONS-INTRO.md)

---

# Databases & Entity Framework Core — Пълен Архитектурен Пътеводител

Тази директория съдържа пълна софтуерна и практическа база знания за работа с релационни бази данни през C# и .NET 8+, детайлно Fluent API проектиране на релации, миграционни жизнени цикли и напреднали архитектурни защити вътре в DbContext.

## 🧭 Навигация и Съдържание

### 🔹 Модул 1: Основи на SQL Server, релации и T-SQL операции
* **01-00. Основи на SQL Server, релации и T-SQL операции (Кратък справочник)**
  * ⚡ [Главен справочник на темата](01-00-SQL-SERVER-AND-RELATIONS-INTRO.md)
  * *Обхваща:* Мостът между типовете, синтактичната подредба (стълба) на SELECT и трансакционна сигурност (Commit/Rollback).
* **01-01. Проектиране на релации и физическо заемане на паметта**
  * 📐 [Структура на релациите и паметта](01-01-SQL-Server-Relations-And-Data-Types.md)
  * *Обхваща:* Външни ключове (FK), избор на ON DELETE поведения и физическа анатомия на редовете на диска (Row Anatomy).
* **01-02. Дълбоко гмуркане в SQL Joins и сигурност при промени**
  * 🤝 [SQL Joins, LIKE и Трансакции](01-02-SQL-Server-Joins-And-Transactions-Deep-Dive.md)
  * *Обхваща:* Визуален анализ на INNER срещу LEFT JOIN, сигурни трансакционни пищови в SSMS и търсене с Wildcards (`LIKE`).

---

### 🔹 Модул 2: Основи на Entity Framework Core и Code-First подход
* **02-00. Основи на Entity Framework Core и Code-First подход (Кратък справочник)**
  * ⚡ [Главен справочник на темата](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md)
  * *Обхваща:* Защо ни е нужен ORM преводач, аналогията с китайския ресторант и командния CLI дневник.
* **02-01. Подходи за Mapping в EF Core: От C# Клас до SQL Таблица**
  * 🗺️ [Сравнение на стиловете за Mapping](02-01-EF-Core-Mapping-Approaches.md)
  * *Обхваща:* Сравнителен анализ на Conventions, Data Attributes и Fluent API с кодови примери „преди и след“.
* **02-02. ApplicationDbContext и Fluent API конфигурация**
  * 🧱 [Конфигуриране на контекста и LINQ](02-02-ApplicationDbContext-And-Fluent-API-Configuration.md)
  * *Обхваща:* Четене на `appsettings.json`, инжектиране на връзки, разглобяване на Ламбда изрази и Fluent API релации.
* **02-03. EF Core Fluent API и Миграции (Глобален Инженерен Пищов)**
  * 🛠️ [Глобален пищов за изпити и интервюта](02-03-EF-Core-Fluent-API-And-Migrations-CheatSheet.md)
  * *Обхваща:* Пълна съпоставка между PMC и .NET CLI команди, идемпотентни SQL скриптове и съкратен синтаксис за релации.
* **02-04. Стратегия за мулти-провайдър: Превключване между SQL и NoSQL бази**
  * 🔌 [Превключване на провайдъри](02-04-Multi-Database-Provider-Switch.md)
  * *Обхваща:* Динамична настройка за SqlServer, PostgreSQL, SQLite и Cosmos DB през JSON, и Cross-Provider капани.
* **02-05. ADO.NET срещу EF Core, Change Tracking и Логване на заявки**
  * 🔎 [Connected Model, памет и дебъгване](02-05-AdoNet-Vs-EfCore-And-Change-Tracking.md)
  * *Обхваща:* Сравнение на `SqlDataReader` с ORM, фази на ChangeTracker в паметта и извличане на нативен SQL през `.ToQueryString()`.
* **02-06. Database-First подход и Автоматично генериране на код (Scaffolding)**
  * ⚙️ [Обратно софтуерно инженерство](02-06-Database-First-Model-Scaffolding.md)
  * *Обхваща:* Автоматично изплюване на код чрез `dbcontext scaffold`, флагове `-f` и `-d`, и partial класове.
* **02-07. EF Core с TDD и Тестване в паметта (In-Memory Testing)**
  * 🔴 [Разработка, водена от тестове (TDD)](02-07-EF-Core-TDD-And-InMemory-Testing.md)
  * *Обхваща:* Цикъл Red-Green-Refactor, конфигуриране на `UseInMemoryDatabase` и изолация на тестове с уникални имена.
* **02-08. Атрибутът InverseProperty при множество връзки между две таблици**
  * 🏷️ [Казусът с дублиращи се релации](02-08-InverseProperty-Multiple-Relationships.md)
  * *Обхваща:* Ситуация с две самостоятелни връзки (Natives и Residents) и разрешаване на софтуерни конфликти.
* **02-09. Валидации с Data Annotations и Специфични Атрибути**
  * 🏷️ [Ограничения, Индекси и Скриване](02-09-Data-Annotations-Validation-And-Attributes.md)
  * *Обхваща:* Конфигуриране на `Required`, `MinLength/MaxLength` и `Range`, създаване на Non-Clustered индекси чрез `[Index]` и изключване на полета с `[NotMapped]`.
* **02-10. Напреднал LINQ, Стратегии за зареждане и Конкурентност**
  * 🧠 [LINQ, материализация и Concurrency](02-10-Advanced-LINQ-And-Query-Operations.md)
  * *Обхваща:* Анализ на IEnumerable срещу IQueryable, стратегии за зареждане (Eager, Explicit, Lazy), капанът N+1 и оптимистичен конкурентен контрол чрез `[Timestamp]`.
---

### 🔹 Модул 3: Напреднали архитектурни защити в DbContext
* **03-00. Напреднали защити в DbContext, Глобални филтри и Soft Delete (Кратък справочник)**
  * ⚡ [Главен справочник на темата](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md)
  * *Обхваща:* Концепция за DbContext като команден пункт на сигурността и преглед на трите защитни стълба.
* **03-01. Глобални Query филтри и имплементиране на Soft Delete**
  * 🕵️‍♂️ [Механизъм на Soft Delete](03-01-Global-Query-Filters-And-Soft-Delete.md)
  * *Обхваща:* Налагане на автоматично `WHERE IsDeleted = 0`, релационни филтри през родители и пробиване на защита чрез `.IgnoreQueryFilters()`.
* **03-02. Прихващане на SaveChanges и ChangeTracker сканиране**
  * 🛡️ [Пренаписване на методи и ChangeTracker](03-02-Intercepting-SaveChanges-And-Cascade-Delete-Restrictions.md)
  * *Обхваща:* Защита на системни котви (Тема №1), хвърляне на InvalidOperationException и асинхронни капани.

---
🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🌐 [Архитектура и Cross-Stack](../Architecture-And-Cross-Stack/) | 🗄️ [Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | ▶️ [Започни от 01-00](01-00-SQL-SERVER-AND-RELATIONS-INTRO.md)

© 2026 Dev-Library / Пълна библиотека по Релации, Мапинг и ORM Архитектура
