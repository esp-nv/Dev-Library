⬅️ [04-05. C# Data Access (ADO.NET и Dapper)](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [00-03. Архитектура, Бекъпи и Инструменти](00-03-DBMS-Architecture-Backups-And-Tools.md)

---

# 04-06. OLTP срещу OLAP и Data Warehouses (Складове за данни)

**Цел:** Каква е разликата между оперативната база на приложението (**OLTP**) и аналитичната база за бизнес отчети и AI (**OLAP / Data Warehouse**), как се моделира **Star Schema** (Fact и Dimension таблици) и как данните се прехвърлят чрез **ETL / ELT**.

## Съдържание на темата
* [1. OLTP срещу OLAP — Сравнение по предназначение и архитектура](#1-oltp-срещу-olap--сравнение-по-предназначение-и-архитектура)
* [2. Data Warehouse и Star Schema (Fact срещу Dimension таблици)](#2-data-warehouse-и-star-schema-fact-срещу-dimension-таблици)
* [3. Колонно съхранение (Columnstore) срещу Редово (Rowstore)](#3-колонно-съхранение-columnstore-срещу-редово-rowstore)
* [4. ETL срещу ELT процеси (Как данните стигат до склада)](#4-etl-срещу-elt-процеси-как-данните-стигат-до-склада)
* [5. Чести грешки, Капани и Как да ги избегнем](#5-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. OLTP СРЕЩУ OLAP — СРАВНЕНИЕ ПО ПРЕДНАЗНАЧЕНИЕ И АРХИТЕКТУРА

В една сериозна компания има **два коренно различни вида бази данни**, които работят паралелно:

| Критерий | OLTP (Online Transaction Processing) | OLAP (Online Analytical Processing / Data Warehouse) |
| :--- | :--- | :--- |
| **Основна цел** | Обслужва **ежедневната работа на приложението** (покупки, регистрации, плащания). | Обслужва **бизнес анализи, отчети (Power BI) и AI модели**. |
| **Вид операции** | Много на брой, кратки `INSERT`, `UPDATE`, `DELETE` и точкови `SELECT` по ID. | Малко на брой, но гигантски `SELECT` заявки с `SUM`, `AVG`, `GROUP BY` върху милиарди редове. |
| **Модел на данните** | **Нормализиран (`3NF`)** — без дублиране, много свързани таблици. | **Денормализиран (`Star Schema` / `Snowflake Schema`)** — за максимално бързо четене. |
| **Обхват на данните** | Текущи и актуални данни (последните месеци/години). | **Исторически данни** за 5–10+ години от много различни системи (Сайт + SAP + CRM). |
| **Популярни технологии** | MS SQL Server, PostgreSQL, MySQL, Oracle. | **Snowflake**, **Google BigQuery**, **Azure Synapse**, **Amazon Redshift**, ClickHouse. |

---

## 2. DATA WAREHOUSE И STAR SCHEMA (FACT СРЕЩУ DIMENSION ТАБЛИЦИ)

В **Data Warehouse** не проектираме таблиците по 3NF, а използваме **Star Schema (Звездна схема)**, която има една централна таблица с факти и няколко заобикалящи таблици с измерения:

```text
                  [ DimCustomers ]
                         |
[ DimDates ] --- [ FactSales (Милиарди редове) ] --- [ DimProducts ]
                         |
                   [ DimStores ]
```

* **Fact Table (Таблица с факти — `FactSales`):** Съдържа числовите измервания на събитието (`Quantity`, `TotalAmount`, `Discount`) и външни ключове към измеренията. Расте непрекъснато.
* **Dimension Tables (Таблици с измерения — `DimProducts`, `DimCustomers`, `DimDates`):** Описват контекста (*Кой? Какво? Къде? Кога?*). Съдържат текстови атрибути (име на продукт, категория, град, ден от седмицата, празник ли е).

```sql
-- ============================================================
-- ПРИМЕР ЗА STAR SCHEMA В DATA WAREHOUSE
-- ============================================================
CREATE TABLE DimDates (
    DateKey INT PRIMARY KEY,                -- Формат: 20261108
    FullDate DATE NOT NULL,
    Year INT NOT NULL,
    Quarter INT NOT NULL,
    MonthName NVARCHAR(20) NOT NULL,
    IsWeekend BIT NOT NULL
);

CREATE TABLE DimProducts (
    ProductKey INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,     -- Денормализирано (няма отделна таблица Categories!)
    BrandName NVARCHAR(50) NOT NULL
);

CREATE TABLE FactSales (
    SaleId BIGINT IDENTITY(1,1) PRIMARY KEY,
    DateKey INT NOT NULL REFERENCES DimDates(DateKey),
    ProductKey INT NOT NULL REFERENCES DimProducts(ProductKey),
    QuantitySold INT NOT NULL,
    TotalRevenue DECIMAL(18, 2) NOT NULL
);

-- Аналитична заявка: Общи приходи по категория за уикендите на 2026 г.:
SELECT p.CategoryName, SUM(f.TotalRevenue) AS WeekendRevenue
FROM FactSales AS f
INNER JOIN DimProducts AS p ON f.ProductKey = p.ProductKey
INNER JOIN DimDates AS d ON f.DateKey = d.DateKey
WHERE d.Year = 2026 AND d.IsWeekend = 1
GROUP BY p.CategoryName
ORDER BY WeekendRevenue DESC;
```

---

## 3. КОЛОННО СЪХРАНЕНИЕ (COLUMNSTORE) СРЕЩУ РЕДОВО (ROWSTORE)

* **Rowstore (OLTP — MS SQL, Postgres):** Записва целия ред заедно (`Id, Name, Price, Date...`). Идеално, когато взимаш всички данни за 1 поръчка.
* **Columnstore (OLAP — BigQuery, Snowflake, SQL Server Columnstore Index):** Записва всяка колона отделно на диска (всички `TotalRevenue` стойности една до друга). Когато пуснеш `SUM(TotalRevenue)` върху 1 милиард реда, базата чете **само тази една колона** (и я компресира до 10 пъти!), без да зарежда останалите 30 колони в RAM паметта.

---

## 4. ETL СРЕЩУ ELT ПРОЦЕСИ (КАК ДАННИТЕ СТИГАТ ДО СКЛАДА)

Данните от оперативните системи (уеб сайта, мобилното приложение, SAP) се прехвърлят в Data Warehouse чрез автоматизирани конвейери (Pipelines):

1. **ETL (Extract ➔ Transform ➔ Load):**
   * Първо извличаш данните от OLTP, **преобразуваш ги на отделен сървър** (чистиш грешки, валути, формати) и накрая ги зареждаш в склада. Класически подход.
2. **ELT (Extract ➔ Load ➔ Transform):**
   * Извличаш суровите данни, **зареждаш ги директно** в модерния облачен склад (Snowflake / BigQuery) и използваш неговата огромна изчислителна мощ, за да ги преобразуваш чрез SQL (инструмент като **dbt**). Модерният стандарт.

---

## 5. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Пускане на тежки BI / Power BI отчети директно върху OLTP базата на сайта
* **Какво се случва:** Финансовият отдел отваря Power BI сутрин и пуска отчет `SUM()` върху 50 милиона поръчки за последните 5 години директно върху сървъра на онлайн магазина. Заявката изяжда 100% от CPU и RAM паметта и клиентите не могат да плащат количките си.
* **Как да избегнеш:** Никога не връзвай тежки аналитични отчети към основната OLTP база — насочи ги към **Read Replica** или отделен **Data Warehouse**.

### Грешка 2: Използване на Data Warehouse (BigQuery / Snowflake) като база за уеб сайт (OLTP)
* **Какво се случва:** Опитваш се да правиш единични `UPDATE` или `INSERT` заявки ред по ред в колонна OLAP база. Тъй като колонните бази са оптимизирани за масово четене, единичните промени отнемат секунди и струват скъпо.
* **Как да избегнеш:** Ползвай OLTP база (PostgreSQL / MS SQL) за приложението, а OLAP (Snowflake / BigQuery) — само за анализи и отчети.

---
⬅️ [04-05. C# Data Access (ADO.NET и Dapper)](04-05-CSharp-Data-Access-ADONET-Dapper-Intro.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [00-03. Архитектура, Бекъпи и Инструменти](00-03-DBMS-Architecture-Backups-And-Tools.md)

© 2026 Dev-Library / Бази данни: OLTP срещу OLAP и Data Warehouses
