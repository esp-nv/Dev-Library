[⬅️ 02-03. Глобален Пищов](02-03-EF-Core-Fluent-API-And-Migrations-CheatSheet.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) 

---

# 02-04. Стратегия за мулти-провайдър: Превключване между SQL и NoSQL бази

**Цел:** Изграждане на гъвкава архитектура в `Program.cs`, позволяваща превключване на двигателя на базата данни (MS SQL Server, PostgreSQL, SQLite, Cosmos DB) само през промяна в `appsettings.json`, и управление на специфичните NuGet пакети.

## Съдържание на темата
* [1. Пълен каталог на Connection Strings и NuGet пакети](#1-пълен-каталог-на-connection-strings-и-nuget-пакети)
* [2. Архитектурен превключвател в Program.cs (Двата подхода)](#2-архитектурен-превключвател-в-programcs-двата-подхода)
* [3. Критични Cross-Provider капани, грешки и решения](#3-критични-cross-provider-капани-грешки-и-решения)

---

## 1. ПЪЛЕН КАТАЛОГ НА CONNECTION STRINGS И NUGET ПАКЕТИ

Всеки доставчик на база данни изисква специфичен софтуерен преводач (Provider), който се инсталира през NuGet, и има уникални параметри в низа за връзка:

### А. MS SQL Server (Класически корпоративен избор)
*   **NuGet пакет:** `Microsoft.EntityFrameworkCore.SqlServer`
*   **Connection String в `appsettings.json`:**
    ```json
    "SqlServerConnection": "Server=Localhost;Database=SchoolProdDb;Trusted_Connection=True;TrustServerCertificate=True;"
    ```
*   **Кога се използва:** Корпоративни Windows екосистеми, ERP системи и проекти, изискващи дълбока интеграция с Microsoft инструменти.

### Б. PostgreSQL (Златен стандарт с отворен код за Linux/Docker)
*   **NuGet пакет:** `Npgsql.EntityFrameworkCore.PostgreSQL`
*   **Connection String в `appsettings.json`:**
    ```json
    "PostgresConnection": "Host=localhost;Port=5432;Database=SchoolProdDb;Username=postgres;Password=YourSecurePassword123;"
    ```
*   **Кога се използва:** Модерни уеб приложения, контейнеризирани в Docker, микроуслуги и облачни деплоймънти в Linux среди (спестява лицензи за Windows).

### В. SQLite (Свръхлек локален файл)
*   **NuGet пакет:** `Microsoft.EntityFrameworkCore.Sqlite`
*   **Connection String в `appsettings.json`:**
    ```json
    "SqliteConnection": "Data Source=school_local.db;"
    ```
*   **Кога се използва:** За бързи локални тестове от разработчиците (без нужда от инсталиране на тежки сървъри), мобилни приложения, настолни софтуери или IoT устройства. Цялата база е просто един файл в папката на проекта.

### Г. Azure Cosmos DB (Облачен NoSQL документен доставчик)
*   **NuGet пакет:** `Microsoft.EntityFrameworkCore.Cosmos`
*   **Connection String в `appsettings.json`:**
    ```json
    "CosmosConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob068A==;Database=SchoolCosmosDb;"
    ```
*   **Кога се използва:** Глобални уеб платформи с милиони потребители, изискващи хоризонтално мащабиране в реално време, ниска латентност по целия свят и гъвкава JSON структура, управлявана директно през EF Core LINQ заявки.

---
## 2. АРХИТЕКТУРЕН ПРЕВКЛЮЧВАТЕЛ В PROGRAM.CS (ДВАТА ПОДХОДА)

За да накараме приложението да сменя своя двигател динамично, можем да приложим два различни подхода в `Program.cs`. Преди това дефинираме един управляващ ключ в `appsettings.json`:

```json
{
  "DatabaseProvider": "PostgreSQL", // Опции: "SqlServer", "PostgreSQL", "SQLite", "CosmosDB"
  "ConnectionStrings": {
    "SqlServer": "Server=Localhost;Database=SchoolDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQL": "Host=localhost;Database=SchoolDb;Username=postgres;Password=YourPassword;",
    "SQLite": "Data Source=school.db;",
    "CosmosDB": "AccountEndpoint=https://localhost:8081/;AccountKey=Key==;Database=SchoolDb;"
  }
}
```

### Подход А: Чрез прости условни проверки (Чист и лесен начин)
Този подход е най-четлив и директен. Проверява текста от конфигурацията и активира съответния NuGet метод:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Взимаме стойността на избрания доставчик и низовете за връзка
var provider = builder.Configuration["DatabaseProvider"];

builder.Services.AddDbContext<SchoolDbContext>(options =>
{
    switch (provider)
    {
        case "SqlServer":
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            break;
            
        case "PostgreSQL":
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
            break;
            
        case "SQLite":
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
            break;
            
        case "CosmosDB":
            options.UseCosmos(
                builder.Configuration.GetConnectionString("CosmosDB"),
                databaseName: "SchoolDb");
            break;
            
        default:
            throw new InvalidOperationException(\$"Доставчикът '{provider}' не се поддържа!");
    }
});
```

### Подход Б: Чрез капсулиране в разширяващ метод (Архитектурен стандарт)
При този подход изнасяме цялата логика в отделен статичен клас, за да поддържаме `Program.cs` максимално чист и прегледен.

```csharp
// 1. Създаваме помощен конфигурационен клас
public static class DependencyInjection
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["DatabaseProvider"];
        
        return services.AddDbContext<SchoolDbContext>(options =>
        {
            _ = provider switch
            {
                "SqlServer" => options.UseSqlServer(configuration.GetConnectionString("SqlServer")),
                "PostgreSQL" => options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")),
                "SQLite" => options.UseSqlite(configuration.GetConnectionString("SQLite")),
                "CosmosDB" => options.UseCosmos(configuration.GetConnectionString("CosmosDB"), "SchoolDb"),
                _ => throw new InvalidOperationException(\$"Невалиден доставчик: {provider}")
            };
        });
    }
}

// 2. В уеб проекта (Program.cs) го извикваме само с един чист ред:
builder.Services.AddDatabaseConfig(builder.Configuration);
```

---

## 3. КРИТИЧНИ CROSS-PROVIDER КАПАНИ, ГРЕШКИ И РЕШЕНИЯ

Когато софтуерът ти поддържа повече от един тип база данни (особено смес от релационни и NoSQL среди), ще се сблъскаш със следните тежки капани:

### Капан 1: Срив при миграциите (Проблемът със специфичния SQL синтаксис)
*   **Проблема:** Ако генерираш миграция, докато превключвателят е на `SqlServer`, EF Core ще запише в миграционния файл специфични команди като `SqlServer:Identity(1, 1)`. Ако утре превключиш конфигурацията на `SQLite` или `PostgreSQL` и пуснеш `Update-Database`, приложението ще гръмне с фатална грешка, защото те не разбират този синтаксис.
*   **Решение:** Различните провайдъри изискват отделни папки за миграции! Генерират се чрез изричен параметър в конзолата:
    ```bash
    Add-Migration InitialSqlServer -Context SchoolDbContext -OutputDir Migrations/SqlServer
    ```

### Капан 2: Тотален срив при LINQ JOIN заявки в NoSQL среди (Cosmos DB)
*   **Проблема:** В C# пишеш LINQ заявка, която прави мощно свързване на три таблици: `context.Students.Include(s => s.Course).ThenInclude(...)`. Когато превключвателят е на MS SQL или Postgres, всичко лети. В момента, в който превключиш на **Cosmos DB (NoSQL)**, заявката се срива с `InvalidOperationException`.
*   **Защо се случва:** NoSQL документите нямат концепция за релационни `JOIN` връзки по мрежата! Те изискват данните да бъдат денормализирани и вградени (Embedded) директно в един общ документ.
*   **Решение:** Ако софтуерът поддържа Cosmos DB доставчик, Fluent API релациите в `OnModelCreating` трябва изрично да се пренапишат чрез метода `.ToJsonProperty()` и обектите да се вградят чрез `.OwnsMany()` или `.OwnsOne()`, вместо да се свързват с външни ключове.

### Капан 3: Разлика в Case Sensitivity при именуването
*   **Проблема:** SQL Server чете и филтрира колоните без значение от малки и главни букви. Когато LINQ заявка се преведе към PostgreSQL, той очаква стриктно малки букви (`snake_case`). Ако в C# свойството е `FirstName`, Postgres ще хвърли грешка, че колоната не съществува.
*   **Решение:** Използвай универсалниFluent API конвенции или инсталирай NuGet пакета `EFCore.NamingConventions` и добави `.UseSnakeCaseNamingConvention()` към конфигурацията на Postgres провайдъра.

---
[⬅️ 02-03. Глобален Пищов](02-03-EF-Core-Fluent-API-And-Migrations-CheatSheet.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) 

© 2026 Dev-Library / Справочник: Мулти-провайдър превключване
