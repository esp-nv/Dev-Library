🏠 [Начало (Библиотека БД)](../Databases-SQL-And-NoSQL/README.md) | ➡️ [01-02. Стек и Купчина](01-02-Stack-Vs-Heap-Memory-Deep-Dive.md)

---

# 01-01. Cross-Stack Карта: SQL ➔ C# ➔ JS / TypeScript ➔ Python

**Цел:** Превод на типове данни, заявки и уеб архитектури между SQL, .NET, JavaScript, TypeScript, Python и Java.

## Съдържание на темата
* [1. Матрица на типовете данни (База данни ➔ Бекенд ➔ Фронтенд)](#1-матрица-на-типовете-данни-база-данни-➔-бекенд-➔-фронтенд)
* [2. Пътят на един модел от край до край (End-to-End Flow)](#2-пътят-на-един-модел-от-край-до-край-end-to-end-flow)
* [3. Превод на заявки: SQL ➔ C# LINQ ➔ JS / TypeScript ➔ Python](#3-превод-на-заявки-sql-➔-c-linq-➔-js--typescript-➔-python)
* [4. ASP.NET Core срещу други уеб платформи (Backend екосистеми)](#4-aspnet-core-срещу-други-уеб-платформи-backend-екосистеми)
* [5. Критични Full-Stack капани при превод между езици](#5-критични-full-stack-капани-при-превод-между-езици)

---



---

## 1. МАТРИЦА НА ТИПОВЕТЕ ДАННИ (БАЗА ДАННИ ➔ БЕКЕНД ➔ ФРОНТЕНД)

Когато един запис пътува от базата данни към C# сървъра и оттам като JSON към клиента, типовете се преобразуват по строго определени правила:

| Концепция | MS SQL Server | PostgreSQL | C# (.NET 8+) | JSON / JS /TypeScript | Python | Java |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Цяло число (32-bit)** | `INT` | `INTEGER` (`INT4`) | `int` | `number` | `int` | `int` / `Integer` |
| **Голямо цяло (64-bit)** | `BIGINT` | `BIGINT` (`INT8`) | `long` | `number` / `bigint` | `int` | `long` / `Long` |
| **Пари / Точност** | `DECIMAL(18,2)` | `NUMERIC(18,2)` | `decimal` | `number` *(или `string`)* | `Decimal` | `BigDecimal` |
| **Дробно (Научно)** | `FLOAT` | `DOUBLE PRECISION` | `double` | `number` | `float` | `double` / `Double` |
| **Логическо** | `BIT` | `BOOLEAN` | `bool` | `boolean` | `bool` | `boolean` / `Boolean` |
| **Текст (Unicode)** | `NVARCHAR(n)` | `VARCHAR(n)` / `TEXT` | `string` | `string` | `str` | `String` |
| **Дата и час (UTC)** | `DATETIME2` | `TIMESTAMPTZ` | `DateTime` / `DateTimeOffset` | `string` *(ISO 8601)* | `datetime` | `Instant` / `LocalDateTime` |
| **Само дата** | `DATE` | `DATE` | `DateOnly` | `string` (`"2026-11-07"`) | `date` | `LocalDate` |
| **Само час** | `TIME` | `TIME` | `TimeOnly` | `string` (`"14:30:00"`) | `time` | `LocalTime` |
| **Уникален ID** | `UNIQUEIDENTIFIER` | `UUID` | `Guid` | `string` | `uuid.UUID` | `UUID` |
| **JSON структура** | `NVARCHAR(MAX)` | `JSONB` | `JsonDocument` / `string` | `object` / `Record<K,V>` | `dict` | `JsonNode` / `Map` |
| **Двоични данни** | `VARBINARY(MAX)` | `BYTEA` | `byte[]` | `Uint8Array` / Base64 | `bytes` | `byte[]` |

---

## 2. ПЪТЯТ НА ЕДИН МОДЕЛ ОТ КРАЙ ДО КРАЙ (END-TO-END FLOW)

Ето как изглежда една и съща таблица `Orders` в целия стек:

```sql
// ==================== 1. БАЗА ДАННИ (POSTGRESQL / MS SQL) ====================
CREATE TABLE orders (
    id UUID PRIMARY KEY,
    total_price NUMERIC(18, 2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    is_paid BOOLEAN NOT NULL
);
```

```csharp
// ==================== 2. C# ENTITY & ASP.NET CORE DTO ====================
public class OrderEntity
{
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsPaid { get; set; }
}

// ASP.NET Core автоматично сериализира C# PascalCase към JSON camelCase!
public record OrderResponseDto(Guid Id, decimal TotalPrice, DateTimeOffset CreatedAt, bool IsPaid);
```

```json
// ==================== 3. JSON ПО МРЕЖАТА (HTTP RESPONSE) ====================
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalPrice": 149.90,
  "createdAt": "2026-11-07T19:50:00Z",
  "isPaid": true
}
```

```typescript
// ==================== 4. TYPESCRIPT ИНТЕРФЕЙС (ФРОНТЕНД / NESTJS) ====================
export interface OrderResponse {
  id: string;
  totalPrice: number;
  createdAt: string; // В JSON датите винаги пристигат като ISO 8601 string!
  isPaid: boolean;
}
```

---

## 3. ПРЕВОД НА ЗАЯВКИ: SQL ➔ C# LINQ ➔ JS / TYPESCRIPT ➔ PYTHON


Една и съща операция (филтриране, сортиране и проекция на данни) в различните езици:

| Операция | SQL (MS SQL / Postgres) | C# LINQ (EF Core) | TypeScript / JS | Python |
| :--- | :--- | :--- | :--- | :--- |
| **Филтриране** | `WHERE Price > 50` | `.Where(p => p.Price > 50)` | `.filter(p => p.price > 50)` | `[p for p in products if p.price > 50]` |
| **Проекция** | `SELECT Name, Price` | `.Select(p => new { p.Name, p.Price })` | `.map(p => ({ name: p.name, price: p.price }))` | `[{"name": p.name, "price": p.price} for p in products]` |
| **Сортиране** | `ORDER BY Price DESC` | `.OrderByDescending(p => p.Price)` | `[...products].sort((a, b) => b.price - a.price)` | `sorted(products, key=lambda p: p.price, reverse=True)` |
| **Първи елемент** | `LIMIT 1` / `TOP 1` | `.FirstOrDefault()` | `.find(...) ?? null` | `next((p for p in products), None)` |
| **Проверка (Any)** | `WHERE EXISTS (...)` | `.Any(p => p.InStock)` | `.some(p => p.inStock)` | `any(p.in_stock for p in products)` |
| **Всички (All)** | `NOT EXISTS (WHERE NOT ...)` | `.All(p => p.InStock)` | `.every(p => p.inStock)` | `all(p.in_stock for p in products)` |
| **Агрегация (Sum)** | `SELECT SUM(Price)` | `.Sum(p => p.Price)` | `.reduce((acc, p) => acc + p.price, 0)` | `sum(p.price for p in products)` |

---

## 4. ASP.NET CORE СРЕЩУ ДРУГИ УЕБ ПЛАТФОРМИ (BACKEND ЕКОСИСТЕМИ)

Когато усвоиш архитектурата на **ASP.NET Core**, преминаването към друг език е само смяна на синтаксиса — слоевете (`Controller ➔ Service ➔ Repository`) са идентични:

| Архитектурен слой | C# / .NET (ASP.NET Core) | TypeScript / Node.js | Python | Java |
| :--- | :--- | :--- | :--- | :--- |
| **Web Framework** | **ASP.NET Core** (Web API / MVC) | **NestJS** *(1:1 с .NET)* / Express | **FastAPI** / Django | **Spring Boot** |
| **ORM / Data Access** | **Entity Framework Core** / Dapper | **Prisma** / TypeORM / Drizzle | **SQLAlchemy** / Django ORM | **Hibernate** / Spring Data JPA |
| **Dependency Injection** | Вграден (`AddScoped<I, T>()`) | Вграден в **NestJS** (`@Injectable()`) | `Depends()` във **FastAPI** | Вграден в **Spring** (`@Autowired`) |
| **Валидация на DTO** | Data Annotations / **FluentValidation** | **Zod** / `class-validator` | **Pydantic** (вграден във FastAPI) | Jakarta Validation (`@NotNull`) |
| **Асинхронност** | `async Task<T>` + `await` | `async Promise<T>` + `await` | `async def` + `await` | `CompletableFuture<T>` / Virtual Threads |
| **Unit Testing** | **NUnit** / xUnit + **Moq** | **Vitest** / Jest | **Pytest** + `unittest.mock` | **JUnit 5** + **Mockito** |
| **Package Manager** | **NuGet** (`.csproj`) | **npm** / pnpm (`package.json`) | **pip** / uv (`pyproject.toml`) | **Maven** / Gradle (`pom.xml`) |

---

## 5. КРИТИЧНИ FULL-STACK КАПАНИ ПРИ ПРЕВОД МЕЖДУ ЕЗИЦИ

### 🛑 Капан 1: Загуба на точност при пари (`DECIMAL` ➔ JavaScript `number`)
* **Проблем:** В C# ползваш `decimal`, в базата `DECIMAL(18,2)`, но JavaScript/TypeScript има само 64-bit `number` (`0.1 + 0.2 === 0.30000000000000004`).
* **Решение:** Всички финансови изчисления се правят **само в бекенда (C#)** или в базата данни. При критични банкови API-та сумите се изпращат в JSON като `string` (`"149.90"`).

### 🛑 Капан 2: Часови зони при дати (`DateTime` срещу `DateTimeOffset` / `TIMESTAMPTZ`)
* **Проблем:** Записваш локално време в базата, сървърът в облака работи в UTC, а браузърът в София добавя още +2/+3 часа.
* **Решение:** В базата ползвай `DATETIME2` (със стойност `DateTime.UtcNow`) или `TIMESTAMPTZ` (в PostgreSQL), а в C# ползвай `DateTimeOffset`. В JSON винаги изпращай UTC формат завършващ на `Z` (`2026-11-07T19:50:00Z`).

### 🛑 Капан 3: Конвенции за именуване (Naming Conventions)
* **C# и MS SQL Server:** `PascalCase` (`FirstName`, `OrderItems`).
* **PostgreSQL и Python:** `snake_case` (`first_name`, `order_items`).
* **JSON, TypeScript и Java:** `camelCase` (`firstName`, `orderItems`).
* *(Съвет: В EF Core с PostgreSQL инсталирай пакета `EFCore.NamingConventions` и добави `.UseSnakeCaseNamingConvention()`, за да не пишеш ръчно `[Column("first_name")]` навсякъде!)*

---
🏠 [Начало (Библиотека БД)](../Databases-SQL-And-NoSQL/README.md) | ➡️ [01-02. Стек и Купчина](01-02-Stack-Vs-Heap-Memory-Deep-Dive.md)

---
© 2026 Dev-Library / Full-Stack & Multi-Language Архитектурна карта
