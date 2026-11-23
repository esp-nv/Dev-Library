# 📘 Дълъг учебник: Бази данни (SQL срещу NoSQL) и Интеграция с C#

**Статус:** Дълбока архитектура и практика | **Стек:** MS SQL Server, PostgreSQL, MySQL, SQLite, MongoDB, Redis & C# (.NET 8+)

---

## 🏛️ 1. АРХИТЕКТУРА ПОД КАПАКА: КАК РАБОТЯТ РАЗЛИЧНИТЕ ДВИГАТЕЛИ (STORAGE ENGINES)

За да избираш правилната база данни на работа, трябва да знаеш как тя записва и търси байтовете на диска или в паметта:

* **Релационни бази (MS SQL, PostgreSQL, MySQL, SQLite):** Организират данните в **страници (Pages — обикновено 8 KB или 16 KB)** на твърдия диск. Използват **B+ Tree индекси**, за да намират редове за $O(\log n)$ време, и **WAL (Write-Ahead Log)** файл, в който първо се записва всяка промяна преди да влезе в основната таблица — това гарантира **Durability** при спиране на тока.
* **Документни бази (MongoDB):** Използват **WiredTiger** двигател и съхраняват данните като **BSON (Binary JSON)** документи. Вместо да пръскат една поръчка в 4 таблици (`Orders`, `OrderItems`, `Addresses`, `Payments`), MongoDB записва целия обект на едно място на диска, което елиминира нуждата от бавни `JOIN` операции при четене.
* **In-Memory Key-Value бази (Redis):** Държат **100% от данните в RAM паметта** като огромна хеш-таблица (`Dictionary<string, RedisValue>`). Тъй като четенето от RAM е ~100 000 пъти по-бързо от SSD диск, Redis отговаря за под **1 милисекунда**.

---

## 💻 2. РЕЛАЦИОННИ БАЗИ В ДЕТАЙЛИ: MS SQL SERVER СРЕЩУ POSTGRESQL

В модерния C# бекенд 90% от релационните проекти използват **MS SQL Server** или **PostgreSQL**. Ето как се сравняват на ниво реален код:

```sql
// ==================== MS SQL SERVER (T-SQL) ====================
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL CHECK (Price >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Пагинация (Странициране) в T-SQL
SELECT Id, Name, Price
FROM Products
WHERE IsActive = 1
ORDER BY Price DESC
OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY;

// ==================== POSTGRESQL (PL/pgSQL) ====================
CREATE TABLE products (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(150) NOT NULL, -- В Postgres VARCHAR е Unicode (UTF-8) по подразбиране!
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0),
    created_at TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    metadata JSONB -- Огромно предимство: вграден индексиран JSON в SQL таблица!
);

-- Пагинация в PostgreSQL (много по-кратък синтаксис)
SELECT id, name, price
FROM products
WHERE is_active = TRUE
ORDER BY price DESC
LIMIT 5 OFFSET 10;
```

### 🔍 Ключови архитектурни разлики между двете:
1. **Unicode текст:** В MS SQL `VARCHAR` поддържа само ASCII/еднобайтови кодировки и за кирилица задължително се пише `NVARCHAR`. В PostgreSQL `VARCHAR` и `TEXT` са **UTF-8 по подразбиране**.
2. **Хибридна сила (JSONB):** PostgreSQL позволява да имаш строги релационни таблици с `Foreign Key` връзки, но в една колона (`metadata JSONB`) да пазиш гъвкави JSON характеристики без да ти трябва отделна MongoDB база.
3. **EF Core превключване:** В C# кодът ти (LINQ заявките) остава **99% идентичен** — сменяш само конфигурацията в `Program.cs`:

```csharp
// ==================== C# СМЯНА НА SQL ДОСТАВЧИК В ASP.NET CORE ====================
// Вариант А: MS SQL Server (NuGet: Microsoft.EntityFrameworkCore.SqlServer)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConn")));

// Вариант Б: PostgreSQL (NuGet: Npgsql.EntityFrameworkCore.PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConn")));

// Вариант В: SQLite за локални тестове (NuGet: Microsoft.EntityFrameworkCore.Sqlite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app_local.db"));
```

---

## 🍃 3. ДОКУМЕНТНИ БАЗИ (MONGODB) И C# (`MongoDB.Driver`)

В MongoDB няма таблици и колони, а **Collections** и **Documents**. Всеки документ е JSON обект със собствена структура (Embedding модел).

### 📦 Пример за вграден документ (Embedded Document):
Вместо таблица `Orders` и таблица `OrderItems`, един документ в колекцията `orders` изглежда така:

```json
{
  "_id": "672b8f1a9c4e12001f8a9b11",
  "customerName": "Елена Петрова",
  "totalAmount": 249.90,
  "status": "Paid",
  "items": [
    { "productName": "Механична клавиатура", "quantity": 1, "unitPrice": 189.90 },
    { "productName": "Подложка за мишка", "quantity": 2, "unitPrice": 30.00 }
  ]
}
```

### ⚙️ Пълна C# интеграция с `MongoDB.Driver`:

```csharp
// ==================== C# МОДЕЛИ И CRUD С MONGODB ====================
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class OrderDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("customerName")]
    public string CustomerName { get; set; } = null!;

    [BsonElement("totalAmount")]
    [BsonRepresentation(BsonType.Decimal128)] // Запазва точността на decimal в BSON!
    public decimal TotalAmount { get; set; }

    [BsonElement("items")]
    public List<OrderItemSubDoc> Items { get; set; } = new();
}

public class OrderItemSubDoc
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class MongoOrderService
{
    private readonly IMongoCollection<OrderDocument> _orders;

    public MongoOrderService(IMongoClient client)
    {
        var database = client.GetDatabase("ShopNoSqlDb");
        _orders = database.GetCollection<OrderDocument>("orders");
    }

    // 1. Вмъкване на документ (Insert)
    public async Task CreateOrderAsync(OrderDocument order)
    {
        await _orders.InsertOneAsync(order);
    }

    // 2. Търсене с LINQ или FilterBuilder
    public async Task<List<OrderDocument>> GetHighValueOrdersAsync(decimal minTotal)
    {
        var filter = Builders<OrderDocument>.Filter.Gte(o => o.TotalAmount, minTotal);
        return await _orders.Find(filter)
                            .SortByDescending(o => o.TotalAmount)
                            .Limit(10)
                            .ToListAsync();
    }
}
```

---

## ⚡ 4. IN-MEMORY КЕШИРАНЕ С REDIS И C# (CACHE-ASIDE PATTERN)

В реалните фирмени проекти **Redis** работи рамо до рамо със SQL базата по шаблона **Cache-Aside**:
1. Приложението първо проверява в **Redis** (RAM).
2. Ако данните са там (**Cache Hit**), ги връща за **0.5 ms** без изобщо да натоварва SQL сървъра.
3. Ако ги няма (**Cache Miss**), ги взима от **MS SQL / PostgreSQL**, записва ги в Redis с време за изтичане (**TTL — Time To Live**, напр. 10 минути) и ги връща на клиента.

```csharp
// ==================== CACHE-ASIDE С REDIS В C# ====================
// NuGet: StackExchange.Redis (или Microsoft.Extensions.Caching.StackExchangeRedis)
using System.Text.Json;
using StackExchange.Redis;

public class ProductCatalogService
{
    private readonly IDatabase _redisDb;
    private readonly AppDbContext _sqlDb;

    public ProductCatalogService(IConnectionMultiplexer redis, AppDbContext sqlDb)
    {
        _redisDb = redis.GetDatabase();
        _sqlDb = sqlDb;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        string cacheKey = $"product:{productId}";

        // Стъпка 1: Търсим първо в RAM паметта (Redis)
        RedisValue cachedJson = await _redisDb.StringGetAsync(cacheKey);
        if (cachedJson.HasValue)
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedJson!);
        }

        // Стъпка 2: Cache Miss -> Отиваме към бавния диск (SQL базата)
        var product = await _sqlDb.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return null;
        }

        // Стъпка 3: Записваме в Redis за следващите заявки със срок на годност 10 минути
        string serialized = JsonSerializer.Serialize(product);
        await _redisDb.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(10));

        return product;
    }

    // При промяна на цената в SQL, задължително трием стария кеш (Cache Invalidation)!
    public async Task UpdatePriceAsync(int productId, decimal newPrice)
    {
        var product = await _sqlDb.Products.FindAsync(productId);
        if (product is not null)
        {
            product.Price = newPrice;
            await _sqlDb.SaveChangesAsync();

            // Изтриваме остарелия запис от Redis
            await _redisDb.KeyDeleteAsync($"product:{productId}");
        }
    }
}

public record ProductDto(int Id, string Name, decimal Price);
```

---

## 🏗️ 5. РЕАЛНА ПОЛИГЛОТ АРХИТЕКТУРА (POLYGLOT PERSISTENCE)

В големите системи на работа един проект често използва **2 или 3 бази едновременно**, като всяка върши това, в което е най-добра:

```text
                       [ ASP.NET Core Web API ]
                         /         |          \
                        /          |           \
                       v           v            v
          [ PostgreSQL / MS SQL ] [ Redis ]  [ MongoDB ]
          • Потребители & Права   • Сесии    • Продуктови ревюта
          • Поръчки & Плащания    • Количка  • Одит логове и събития
          • Счетоводство (ACID)   • Кеш      • Динамични каталози
```

---

## 🚨 6. АРХИТЕКТУРНИ КАПАНИ ПРИ РАБОТА С МНОГО БАЗИ

# 📘 Дълъг учебник: Бази данни (SQL срещу NoSQL) и Интеграция с C#

**Статус:** Дълбока архитектура и практика | **Стек:** MS SQL Server, PostgreSQL, MySQL, SQLite, MongoDB, Redis & C# (.NET 8+)

---

## 🏛️ 1. АРХИТЕКТУРА ПОД КАПАКА: КАК РАБОТЯТ РАЗЛИЧНИТЕ ДВИГАТЕЛИ (STORAGE ENGINES)

За да избираш правилната база данни на работа, трябва да знаеш как тя записва и търси байтовете на диска или в паметта:

* **Релационни бази (MS SQL, PostgreSQL, MySQL, SQLite):** Организират данните в **страници (Pages — обикновено 8 KB или 16 KB)** на твърдия диск. Използват **B+ Tree индекси**, за да намират редове за $O(\log n)$ време, и **WAL (Write-Ahead Log)** файл, в който първо се записва всяка промяна преди да влезе в основната таблица — това гарантира **Durability** при спиране на тока.
* **Документни бази (MongoDB):** Използват **WiredTiger** двигател и съхраняват данните като **BSON (Binary JSON)** документи. Вместо да пръскат една поръчка в 4 таблици (`Orders`, `OrderItems`, `Addresses`, `Payments`), MongoDB записва целия обект на едно място на диска, което елиминира нуждата от бавни `JOIN` операции при четене.
* **In-Memory Key-Value бази (Redis):** Държат **100% от данните в RAM паметта** като огромна хеш-таблица (`Dictionary<string, RedisValue>`). Тъй като четенето от RAM е ~100 000 пъти по-бързо от SSD диск, Redis отговаря за под **1 милисекунда**.

---

## 💻 2. РЕЛАЦИОННИ БАЗИ В ДЕТАЙЛИ: MS SQL SERVER СРЕЩУ POSTGRESQL

В модерния C# бекенд 90% от релационните проекти използват **MS SQL Server** или **PostgreSQL**. Ето как се сравняват на ниво реален код:

```sql
// ==================== MS SQL SERVER (T-SQL) ====================
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL CHECK (Price >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Пагинация (Странициране) в T-SQL
SELECT Id, Name, Price
FROM Products
WHERE IsActive = 1
ORDER BY Price DESC
OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY;

// ==================== POSTGRESQL (PL/pgSQL) ====================
CREATE TABLE products (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(150) NOT NULL, -- В Postgres VARCHAR е Unicode (UTF-8) по подразбиране!
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0),
    created_at TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    metadata JSONB -- Огромно предимство: вграден индексиран JSON в SQL таблица!
);

-- Пагинация в PostgreSQL (много по-кратък синтаксис)
SELECT id, name, price
FROM products
WHERE is_active = TRUE
ORDER BY price DESC
LIMIT 5 OFFSET 10;
```

### 🔍 Ключови архитектурни разлики между двете:
1. **Unicode текст:** В MS SQL `VARCHAR` поддържа само ASCII/еднобайтови кодировки и за кирилица задължително се пише `NVARCHAR`. В PostgreSQL `VARCHAR` и `TEXT` са **UTF-8 по подразбиране**.
2. **Хибридна сила (JSONB):** PostgreSQL позволява да имаш строги релационни таблици с `Foreign Key` връзки, но в една колона (`metadata JSONB`) да пазиш гъвкави JSON характеристики без да ти трябва отделна MongoDB база.
3. **EF Core превключване:** В C# кодът ти (LINQ заявките) остава **99% идентичен** — сменяш само конфигурацията в `Program.cs`:

```csharp
// ==================== C# СМЯНА НА SQL ДОСТАВЧИК В ASP.NET CORE ====================
// Вариант А: MS SQL Server (NuGet: Microsoft.EntityFrameworkCore.SqlServer)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConn")));

// Вариант Б: PostgreSQL (NuGet: Npgsql.EntityFrameworkCore.PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConn")));

// Вариант В: SQLite за локални тестове (NuGet: Microsoft.EntityFrameworkCore.Sqlite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app_local.db"));
```

---

## 🍃 3. ДОКУМЕНТНИ БАЗИ (MONGODB) И C# (`MongoDB.Driver`)

В MongoDB няма таблици и колони, а **Collections** и **Documents**. Всеки документ е JSON обект със собствена структура (Embedding модел).

### 📦 Пример за вграден документ (Embedded Document):
Вместо таблица `Orders` и таблица `OrderItems`, един документ в колекцията `orders` изглежда така:

```json
{
  "_id": "672b8f1a9c4e12001f8a9b11",
  "customerName": "Елена Петрова",
  "totalAmount": 249.90,
  "status": "Paid",
  "items": [
    { "productName": "Механична клавиатура", "quantity": 1, "unitPrice": 189.90 },
    { "productName": "Подложка за мишка", "quantity": 2, "unitPrice": 30.00 }
  ]
}
```

### ⚙️ Пълна C# интеграция с `MongoDB.Driver`:

```csharp
// ==================== C# МОДЕЛИ И CRUD С MONGODB ====================
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class OrderDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("customerName")]
    public string CustomerName { get; set; } = null!;

    [BsonElement("totalAmount")]
    [BsonRepresentation(BsonType.Decimal128)] // Запазва точността на decimal в BSON!
    public decimal TotalAmount { get; set; }

    [BsonElement("items")]
    public List<OrderItemSubDoc> Items { get; set; } = new();
}

public class OrderItemSubDoc
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class MongoOrderService
{
    private readonly IMongoCollection<OrderDocument> _orders;

    public MongoOrderService(IMongoClient client)
    {
        var database = client.GetDatabase("ShopNoSqlDb");
        _orders = database.GetCollection<OrderDocument>("orders");
    }

    // 1. Вмъкване на документ (Insert)
    public async Task CreateOrderAsync(OrderDocument order)
    {
        await _orders.InsertOneAsync(order);
    }

    // 2. Търсене с LINQ или FilterBuilder
    public async Task<List<OrderDocument>> GetHighValueOrdersAsync(decimal minTotal)
    {
        var filter = Builders<OrderDocument>.Filter.Gte(o => o.TotalAmount, minTotal);
        return await _orders.Find(filter)
                            .SortByDescending(o => o.TotalAmount)
                            .Limit(10)
                            .ToListAsync();
    }
}
```

---

## ⚡ 4. IN-MEMORY КЕШИРАНЕ С REDIS И C# (CACHE-ASIDE PATTERN)

В реалните фирмени проекти **Redis** работи рамо до рамо със SQL базата по шаблона **Cache-Aside**:
1. Приложението първо проверява в **Redis** (RAM).
2. Ако данните са там (**Cache Hit**), ги връща за **0.5 ms** без изобщо да натоварва SQL сървъра.
3. Ако ги няма (**Cache Miss**), ги взима от **MS SQL / PostgreSQL**, записва ги в Redis с време за изтичане (**TTL — Time To Live**, напр. 10 минути) и ги връща на клиента.

```csharp
// ==================== CACHE-ASIDE С REDIS В C# ====================
// NuGet: StackExchange.Redis (или Microsoft.Extensions.Caching.StackExchangeRedis)
using System.Text.Json;
using StackExchange.Redis;

public class ProductCatalogService
{
    private readonly IDatabase _redisDb;
    private readonly AppDbContext _sqlDb;

    public ProductCatalogService(IConnectionMultiplexer redis, AppDbContext sqlDb)
    {
        _redisDb = redis.GetDatabase();
        _sqlDb = sqlDb;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        string cacheKey = $"product:{productId}";

        // Стъпка 1: Търсим първо в RAM паметта (Redis)
        RedisValue cachedJson = await _redisDb.StringGetAsync(cacheKey);
        if (cachedJson.HasValue)
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedJson!);
        }

        // Стъпка 2: Cache Miss -> Отиваме към бавния диск (SQL базата)
        var product = await _sqlDb.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return null;
        }

        // Стъпка 3: Записваме в Redis за следващите заявки със срок на годност 10 минути
        string serialized = JsonSerializer.Serialize(product);
        await _redisDb.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(10));

        return product;
    }

    // При промяна на цената в SQL, задължително трием стария кеш (Cache Invalidation)!
    public async Task UpdatePriceAsync(int productId, decimal newPrice)
    {
        var product = await _sqlDb.Products.FindAsync(productId);
        if (product is not null)
        {
            product.Price = newPrice;
            await _sqlDb.SaveChangesAsync();

            // Изтриваме остарелия запис от Redis
            await _redisDb.KeyDeleteAsync($"product:{productId}");
        }
    }
}

public record ProductDto(int Id, string Name, decimal Price);
```

---

## 🏗️ 5. РЕАЛНА ПОЛИГЛОТ АРХИТЕКТУРА (POLYGLOT PERSISTENCE)

В големите системи на работа един проект често използва **2 или 3 бази едновременно**, като всяка върши това, в което е най-добра:

```text
                       [ ASP.NET Core Web API ]
                         /         |          \
                        /          |           \
                       v           v            v
          [ PostgreSQL / MS SQL ] [ Redis ]  [ MongoDB ]
          • Потребители & Права   • Сесии    • Продуктови ревюта
          • Поръчки & Плащания    • Количка  • Одит логове и събития
          • Счетоводство (ACID)   • Кеш      • Динамични каталози
```

---

## 🚨 6. АРХИТЕКТУРНИ КАПАНИ ПРИ РАБОТА С МНОГО БАЗИ

# 📘 Дълъг учебник: Бази данни (SQL срещу NoSQL) и Интеграция с C#

**Статус:** Дълбока архитектура и практика | **Стек:** MS SQL Server, PostgreSQL, MySQL, SQLite, MongoDB, Redis & C# (.NET 8+)

---

## 🏛️ 1. АРХИТЕКТУРА ПОД КАПАКА: КАК РАБОТЯТ РАЗЛИЧНИТЕ ДВИГАТЕЛИ (STORAGE ENGINES)

За да избираш правилната база данни на работа, трябва да знаеш как тя записва и търси байтовете на диска или в паметта:

* **Релационни бази (MS SQL, PostgreSQL, MySQL, SQLite):** Организират данните в **страници (Pages — обикновено 8 KB или 16 KB)** на твърдия диск. Използват **B+ Tree индекси**, за да намират редове за $O(\log n)$ време, и **WAL (Write-Ahead Log)** файл, в който първо се записва всяка промяна преди да влезе в основната таблица — това гарантира **Durability** при спиране на тока.
* **Документни бази (MongoDB):** Използват **WiredTiger** двигател и съхраняват данните като **BSON (Binary JSON)** документи. Вместо да пръскат една поръчка в 4 таблици (`Orders`, `OrderItems`, `Addresses`, `Payments`), MongoDB записва целия обект на едно място на диска, което елиминира нуждата от бавни `JOIN` операции при четене.
* **In-Memory Key-Value бази (Redis):** Държат **100% от данните в RAM паметта** като огромна хеш-таблица (`Dictionary<string, RedisValue>`). Тъй като четенето от RAM е ~100 000 пъти по-бързо от SSD диск, Redis отговаря за под **1 милисекунда**.

---

## 💻 2. РЕЛАЦИОННИ БАЗИ В ДЕТАЙЛИ: MS SQL SERVER СРЕЩУ POSTGRESQL

В модерния C# бекенд 90% от релационните проекти използват **MS SQL Server** или **PostgreSQL**. Ето как се сравняват на ниво реален код:

```sql
// ==================== MS SQL SERVER (T-SQL) ====================
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL CHECK (Price >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Пагинация (Странициране) в T-SQL
SELECT Id, Name, Price
FROM Products
WHERE IsActive = 1
ORDER BY Price DESC
OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY;

// ==================== POSTGRESQL (PL/pgSQL) ====================
CREATE TABLE products (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(150) NOT NULL, -- В Postgres VARCHAR е Unicode (UTF-8) по подразбиране!
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0),
    created_at TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    metadata JSONB -- Огромно предимство: вграден индексиран JSON в SQL таблица!
);

-- Пагинация в PostgreSQL (много по-кратък синтаксис)
SELECT id, name, price
FROM products
WHERE is_active = TRUE
ORDER BY price DESC
LIMIT 5 OFFSET 10;
```

### 🔍 Ключови архитектурни разлики между двете:
1. **Unicode текст:** В MS SQL `VARCHAR` поддържа само ASCII/еднобайтови кодировки и за кирилица задължително се пише `NVARCHAR`. В PostgreSQL `VARCHAR` и `TEXT` са **UTF-8 по подразбиране**.
2. **Хибридна сила (JSONB):** PostgreSQL позволява да имаш строги релационни таблици с `Foreign Key` връзки, но в една колона (`metadata JSONB`) да пазиш гъвкави JSON характеристики без да ти трябва отделна MongoDB база.
3. **EF Core превключване:** В C# кодът ти (LINQ заявките) остава **99% идентичен** — сменяш само конфигурацията в `Program.cs`:

```csharp
// ==================== C# СМЯНА НА SQL ДОСТАВЧИК В ASP.NET CORE ====================
// Вариант А: MS SQL Server (NuGet: Microsoft.EntityFrameworkCore.SqlServer)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConn")));

// Вариант Б: PostgreSQL (NuGet: Npgsql.EntityFrameworkCore.PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConn")));

// Вариант В: SQLite за локални тестове (NuGet: Microsoft.EntityFrameworkCore.Sqlite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app_local.db"));
```

---

## 🍃 3. ДОКУМЕНТНИ БАЗИ (MONGODB) И C# (`MongoDB.Driver`)

В MongoDB няма таблици и колони, а **Collections** и **Documents**. Всеки документ е JSON обект със собствена структура (Embedding модел).

### 📦 Пример за вграден документ (Embedded Document):
Вместо таблица `Orders` и таблица `OrderItems`, един документ в колекцията `orders` изглежда така:

```json
{
  "_id": "672b8f1a9c4e12001f8a9b11",
  "customerName": "Елена Петрова",
  "totalAmount": 249.90,
  "status": "Paid",
  "items": [
    { "productName": "Механична клавиатура", "quantity": 1, "unitPrice": 189.90 },
    { "productName": "Подложка за мишка", "quantity": 2, "unitPrice": 30.00 }
  ]
}
```

### ⚙️ Пълна C# интеграция с `MongoDB.Driver`:

```csharp
// ==================== C# МОДЕЛИ И CRUD С MONGODB ====================
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class OrderDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("customerName")]
    public string CustomerName { get; set; } = null!;

    [BsonElement("totalAmount")]
    [BsonRepresentation(BsonType.Decimal128)] // Запазва точността на decimal в BSON!
    public decimal TotalAmount { get; set; }

    [BsonElement("items")]
    public List<OrderItemSubDoc> Items { get; set; } = new();
}

public class OrderItemSubDoc
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class MongoOrderService
{
    private readonly IMongoCollection<OrderDocument> _orders;

    public MongoOrderService(IMongoClient client)
    {
        var database = client.GetDatabase("ShopNoSqlDb");
        _orders = database.GetCollection<OrderDocument>("orders");
    }

    // 1. Вмъкване на документ (Insert)
    public async Task CreateOrderAsync(OrderDocument order)
    {
        await _orders.InsertOneAsync(order);
    }

    // 2. Търсене с LINQ или FilterBuilder
    public async Task<List<OrderDocument>> GetHighValueOrdersAsync(decimal minTotal)
    {
        var filter = Builders<OrderDocument>.Filter.Gte(o => o.TotalAmount, minTotal);
        return await _orders.Find(filter)
                            .SortByDescending(o => o.TotalAmount)
                            .Limit(10)
                            .ToListAsync();
    }
}
```

---

## ⚡ 4. IN-MEMORY КЕШИРАНЕ С REDIS И C# (CACHE-ASIDE PATTERN)

В реалните фирмени проекти **Redis** работи рамо до рамо със SQL базата по шаблона **Cache-Aside**:
1. Приложението първо проверява в **Redis** (RAM).
2. Ако данните са там (**Cache Hit**), ги връща за **0.5 ms** без изобщо да натоварва SQL сървъра.
3. Ако ги няма (**Cache Miss**), ги взима от **MS SQL / PostgreSQL**, записва ги в Redis с време за изтичане (**TTL — Time To Live**, напр. 10 минути) и ги връща на клиента.

```csharp
// ==================== CACHE-ASIDE С REDIS В C# ====================
// NuGet: StackExchange.Redis (или Microsoft.Extensions.Caching.StackExchangeRedis)
using System.Text.Json;
using StackExchange.Redis;

public class ProductCatalogService
{
    private readonly IDatabase _redisDb;
    private readonly AppDbContext _sqlDb;

    public ProductCatalogService(IConnectionMultiplexer redis, AppDbContext sqlDb)
    {
        _redisDb = redis.GetDatabase();
        _sqlDb = sqlDb;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        string cacheKey = $"product:{productId}";

        // Стъпка 1: Търсим първо в RAM паметта (Redis)
        RedisValue cachedJson = await _redisDb.StringGetAsync(cacheKey);
        if (cachedJson.HasValue)
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedJson!);
        }

        // Стъпка 2: Cache Miss -> Отиваме към бавния диск (SQL базата)
        var product = await _sqlDb.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return null;
        }

        // Стъпка 3: Записваме в Redis за следващите заявки със срок на годност 10 минути
        string serialized = JsonSerializer.Serialize(product);
        await _redisDb.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(10));

        return product;
    }

    // При промяна на цената в SQL, задължително трием стария кеш (Cache Invalidation)!
    public async Task UpdatePriceAsync(int productId, decimal newPrice)
    {
        var product = await _sqlDb.Products.FindAsync(productId);
        if (product is not null)
        {
            product.Price = newPrice;
            await _sqlDb.SaveChangesAsync();

            // Изтриваме остарелия запис от Redis
            await _redisDb.KeyDeleteAsync($"product:{productId}");
        }
    }
}

public record ProductDto(int Id, string Name, decimal Price);
```

---

## 🏗️ 5. РЕАЛНА ПОЛИГЛОТ АРХИТЕКТУРА (POLYGLOT PERSISTENCE)

В големите системи на работа един проект често използва **2 или 3 бази едновременно**, като всяка върши това, в което е най-добра:

```text
                       [ ASP.NET Core Web API ]
                         /         |          \
                        /          |           \
                       v           v            v
          [ PostgreSQL / MS SQL ] [ Redis ]  [ MongoDB ]
          • Потребители & Права   • Сесии    • Продуктови ревюта
          • Поръчки & Плащания    • Количка  • Одит логове и събития
          • Счетоводство (ACID)   • Кеш      • Динамични каталози
```

---

## 🚨 6. АРХИТЕКТУРНИ КАПАНИ ПРИ РАБОТА С МНОГО БАЗИ

1. **Проблемът с „Остарелия кеш“ (Stale Cache)**: Ако обновиш цената на продукт в PostgreSQL/MS SQL, но забравиш да извикаш _redisDb.KeyDeleteAsync(...), потребителите ще виждат старата цена, докато не изтече TTL таймерът.
2. **Сериализация на decimal в MongoDB**: По подразбиране C# драйверът на MongoDB може да запише decimal като string, което чупи математическите филтри ($gte, $lt). Винаги слагай атрибута [BsonRepresentation(BsonType.Decimal128)].
3. **Забравен AsNoTracking() при четене в SQL**: Когато четеш данни от MS SQL или PostgreSQL, за да ги кешираш в Redis или да ги върнеш по API без да ги променяш, винаги добавяй .AsNoTracking() в EF Core — това пести памет и ускорява заявката с до 30–40%.

----
© 2026 Dev-Library / Дълъг учебник: Бази данни (SQL & NoSQL) и C#
