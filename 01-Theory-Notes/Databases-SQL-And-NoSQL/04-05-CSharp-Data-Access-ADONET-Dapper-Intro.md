⬅️ [04-04. Views, Functions, Procedures и Triggers](04-04-Views-Functions-Procedures-Triggers.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-06. OLTP срещу OLAP и Data Warehouses](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md)

---

# 04-05. Достъп до данни в C#: ADO.NET ➔ Dapper ➔ EF Core

**Цел:** Как C# комуникира с базата данни под капака — какво стои в основата (**ADO.NET**: `SqlConnection`, `SqlCommand`, `SqlDataReader`), защо **Dapper** е стандарт за светкавични заявки и как да избираме между **ADO.NET**, **Dapper** и **Entity Framework Core**.

## Съдържание на темата
* [1. Пирамидата на достъпа до данни в .NET (ADO.NET ➔ Dapper ➔ EF Core)](#1-пирамидата-на-достъпа-до-данни-в-net-adonet--dapper--ef-core)
* [2. ADO.NET под капака (SqlConnection, SqlCommand, SqlDataReader)](#2-adonet-под-капака-sqlconnection-sqlcommand-sqldatareader)
* [3. Dapper (Micro-ORM) — Суров SQL със скоростта на ADO.NET](#3-dapper-micro-orm--суров-sql-със-скоростта-на-adonet)
* [4. Хибриден подход в реални C# проекти (EF Core + Dapper)](#4-хибриден-подход-в-реални-c-проекти-ef-core--dapper)
* [5. Чести грешки, Капани и Как да ги избегнем](#5-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. ПИРАМИДАТА НА ДОСТЪПА ДО ДАННИ В .NET

Всички библиотеки за бази данни в C# (включително EF Core и Dapper) стъпват върху **ADO.NET** — нисконивовия слой в .NET за отваряне на мрежова връзка и изпращане на SQL команди.

| Критерий | ADO.NET (`Microsoft.Data.SqlClient`) | Dapper (Micro-ORM) | Entity Framework Core (Full ORM) |
| :--- | :--- | :--- | :--- |
| **Как пишеш заявките?** | 100% ръчен SQL + ръчно четене колона по колона. | 100% ръчен SQL, но Dapper сам пълни C# класовете. | Пишеш **C# LINQ** (EF сам генерира SQL). |
| **Скорост на изпълнение** | **Най-бърз (100% базово ниво)** | **Почти идентична с ADO.NET (~98%)** | По-бавен при сложни заявки (има Change Tracker). |
| **Change Tracking & Migrations** | Няма | Няма | **Да** (следи промени по обектите и генерира миграции). |
| **Къде е предпочитан?** | Системни инструменти, импорт на милиони редове (`SqlBulkCopy`). | **Тежки отчети, сложни `JOIN`-ове, Stored Procedures, високонатоварени API-та.** | Стандартни CRUD операции, бизнес логика, бърза разработка. |

---

## 2. ADO.NET ПОД КАПАКА (SQLCONNECTION, SQLCOMMAND, SQLDATAREADER)

Познаването на **ADO.NET** е задължително за интервюта, защото показва, че разбираш как се управляват връзките и как се предпазваш от **SQL Injection**.

```csharp
// ============================================================
// ЧИСТ ADO.NET ПРИМЕР (ЧЕТЕНЕ С ПАРАМЕТРИЗИРАНА ЗАЯВКА)
// ============================================================
using Microsoft.Data.SqlClient;

public async Task<List<string>> GetCustomerNamesByCityAsync(string connectionString, string city)
{
    var names = new List<string>();

    // 1. using гарантира, че връзката ще се върне в Connection Pool-а дори при грешка!
    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    // 2. Винаги ползваме @City параметър (защита от SQL Injection!)
    const string sql = "SELECT FullName FROM Customers WHERE City = @City AND IsActive = 1;";

    await using var command = new SqlCommand(sql, connection);
    command.Parameters.AddWithValue("@City", city);

    // 3. SqlDataReader чете резултата ред по ред (Forward-Only, Read-Only поток)
    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        names.Add(reader.GetString(0)); // Чете колона 0 (FullName)
    }

    return names;
}
```

---

## 3. DAPPER (MICRO-ORM) — СУРОВ SQL СЪС СКОРОСТТА НА ADO.NET

**Dapper** (създаден от екипа на Stack Overflow) разширява `IDbConnection` с няколко метода (`QueryAsync<T>`, `ExecuteAsync`), които премахват досадния `SqlDataReader` код, но ти дават пълен контрол върху SQL заявката.

```csharp
// ============================================================
// DAPPER ПРИМЕР (NuGet: Dapper)
// ============================================================
using Dapper;
using Microsoft.Data.SqlClient;

public record OrderSummaryDto(int Id, string CustomerName, decimal TotalAmount);

public class OrderReportRepository
{
    private readonly string _connectionString;

    public OrderReportRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 1. Четене на списък с обекти (QueryAsync):
    public async Task<IEnumerable<OrderSummaryDto>> GetLargeOrdersAsync(decimal minAmount)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
            SELECT o.Id, c.FullName AS CustomerName, o.TotalAmount
            FROM Orders AS o
            INNER JOIN Customers AS c ON o.CustomerId = c.Id
            WHERE o.TotalAmount >= @MinAmount
            ORDER BY o.TotalAmount DESC;
            """;

        // Dapper автоматично мапва колоните към свойствата на OrderSummaryDto!
        return await connection.QueryAsync<OrderSummaryDto>(sql, new { MinAmount = minAmount });
    }

    // 2. Изпълнение на INSERT / UPDATE / DELETE (ExecuteAsync):
    public async Task<int> MarkOrderAsPaidAsync(int orderId)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "UPDATE Orders SET IsPaid = 1 WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, new { Id = orderId }); // Връща броя засегнати редове
    }
}
```

---

## 4. ХИБРИДЕН ПОДХОД В РЕАЛНИ C# ПРОЕКТИ (EF CORE + DAPPER)

В големите .NET проекти **не избираме само едното**, а ги комбинираме (често по **CQRS** модела):
* **За писане (`Commands` — Create, Update, Delete):** Използваме **EF Core**, защото пази бизнес правилата, навигационните свойства, валидациите и транзакциите в чист обектно-ориентиран код.
* **За четене на сложни отчети (`Queries` — Dashboards, Exports):** Използваме **Dapper** (или директно EF Core `.AsNoTracking()`), за да пуснем оптимизиран SQL с `CTE` или `Window Functions` без излишно натоварване на паметта.

*(Добра новина: в EF Core вече можеш да изпълниш суров SQL и да го мапнеш към произволен DTO клас директно чрез `_db.Database.SqlQuery<OrderSummaryDto>($"SELECT ...").ToListAsync()`, без дори да инсталираш Dapper!)*

---

## 5. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Слепване на низове в SQL заявка (`SQL Injection` уязвимост!)
* **Какво се случва:** Пишеш `string sql = "SELECT * FROM Users WHERE Email = '" + userEmail + "'";`. Ако злонамерен потребител въведе `' OR 1=1 --`, той влиза в системата без парола или изтрива таблиците ти!
* **Как да избегнеш:** **Никога** не използвай `+` или `$"..."` (интерполация) за поставяне на потребителски стойности в чист SQL! Винаги подавай параметри (`@Email` в ADO.NET/Dapper).

### Грешка 2: Забравен `using` / `await using` върху `SqlConnection` (Connection Pool Exhaustion)
* **Какво се случва:** Отваряш `var conn = new SqlConnection(...)` и забравяш да го затвориш с `await using`. След 100 заявки пулът от връзки се запълва и приложението забива с грешка *Timeout expired. The timeout period elapsed prior to obtaining a connection from the pool*.
* **Как да избегнеш:** Винаги декларирай връзката, командата и четеца с **`await using`**, за да се освобождават автоматично.

### Грешка 3: Извикване на `.Result` или `.Wait()` вместо `await` (Thread Pool Starvation)
* **Какво се случва:** В уеб приложение (ASP.NET Core) пишеш `connection.QueryAsync(...).Result` вместо `await connection.QueryAsync(...)`. Това блокира нишката, докато чака базата данни, и при повече потребители сървърът спира да отговаря.
* **Как да избегнеш:** Винаги използвай `async / await` от контролера чак до базата данни (`OpenAsync`, `ExecuteReaderAsync`, `QueryAsync`).

---
⬅️ [04-04. Views, Functions, Procedures и Triggers](04-04-Views-Functions-Procedures-Triggers.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ➡️ [04-06. OLTP срещу OLAP и Data Warehouses](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md)

© 2026 Dev-Library / Бази данни: C# Data Access (ADO.NET и Dapper)
