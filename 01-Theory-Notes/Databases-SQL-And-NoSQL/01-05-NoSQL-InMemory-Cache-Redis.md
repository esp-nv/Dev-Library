⬅️ [01-04. NoSQL: MongoDB и Cassandra](01-04-NoSQL-Documents-And-WideColumn-Mongo-Cassandra.md) | 🏠 [01-00. Начало (Основи и CRUD)](01-00-SQL-INTRO-AND-CRUD.md) | ➡️ [01-06. Формати: JSON и XML](01-06-Data-Formats-JSON-And-XML.md)

---

# ⚡ 01-05. Redis Cache, Cloud NoSQL (DynamoDB & Cosmos DB) и Корпоративни системи

**Цел:** Бърза работа с Redis в RAM паметта, сравнение на облачните NoSQL бази в AWS и Azure, и ясно разграничение на Active Directory и SAP.

## 📑 Съдържание на темата
* [1. Redis — Основни команди (RAM памет и TTL)](#-1-redis--основни-команди-ram-памет-и-ttl)
* [2. Redis в C# — Cache-Aside шаблон](#-2-redis-в-c--cache-aside-шаблон)
* [3. Облачни NoSQL бази: Amazon DynamoDB срещу Azure Cosmos DB](#-3-облачни-nosql-бази-amazon-dynamodb-срещу-azure-cosmos-db)
* [4. Корпоративни системи: Къде стоят Active Directory и SAP?](#-4-корпоративни-системи-къде-стоят-active-directory-и-sap)

---

## ⚡ 1. REDIS — ОСНОВНИ КОМАНДИ (RAM ПАМЕТ И TTL)

**Redis** държи данните изцяло в **RAM паметта** като двойки `Ключ ➔ Стойност` (`Key ➔ Value`) и отговаря за под **1 милисекунда**. Използва се за **кеширане (Cache)**, сесии и количка в онлайн магазин.

```bash
# ============================================================
# REDIS CLI — НАЙ-ВАЖНИТЕ КОМАНДИ
# ============================================================

# 1. Запис и четене на обикновен низ (String)
SET user:101:name "Елена Петрова"
GET user:101:name

# 2. Запис със срок на годност (TTL - Time To Live в секунди)
# След 600 секунди (10 мин.) Redis сам изтрива ключа!
SET product:55 "{'name':'Лаптоп','price':1899}" EX 600
TTL product:55          # Показва колко секунди остават преди изтриване

# 3. Броячи (атомарни операции - за лайкове, преглеждания, наличности)
SET page:views 100
INCR page:views         # Става 101
DECR page:views         # Става 100

# 4. Обекти (Hash - като малък речник в C#)
HSET session:abc userId "101" role "Admin"
HGET session:abc role   # Връща "Admin"

# 5. Изтриване на ключ (Cache Invalidation)
DEL product:55
```

---

## 💻 2. REDIS В C# — CACHE-ASIDE ШАБЛОН (`StackExchange.Redis`)

В реалните проекти C# първо пита Redis. Ако ключът липсва (**Cache Miss**), взима данните от SQL базата и ги слага в Redis за следващите потребители.

```csharp
// ============================================================
// CACHE-ASIDE В C# (REDIS + SQL)
// ============================================================
using System.Text.Json;
using StackExchange.Redis;

public class CacheService
{
    private readonly IDatabase _redis;

    public CacheService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<string?> GetOrSetProductAsync(int id, Func<Task<string>> getFromSqlDb)
    {
        string key = \$"product:{id}";

        // 1. Търсим в RAM (Redis)
        var cached = await _redis.StringGetAsync(key);
        if (cached.HasValue)
        {
            return cached.ToString(); // Cache Hit -> връщаме веднага (< 1 ms)!
        }

        // 2. Няма го в Redis -> викаме SQL базата
        string freshData = await getFromSqlDb();

        // 3. Записваме в Redis за 10 минути
        await _redis.StringSetAsync(key, freshData, TimeSpan.FromMinutes(10));

        return freshData;
    }
}
```

---

## ☁️ 3. ОБЛАЧНИ NOSQL БАЗИ: AMAZON DYNAMODB СРЕЩУ AZURE COSMOS DB

И двете са напълно управлявани (**Serverless**) облачни бази — не инсталираш сървъри, а облакът ги скалира автоматично по целия свят.

| Критерий | Amazon DynamoDB (AWS) | Azure Cosmos DB (Microsoft) |
| :--- | :--- | :--- |
| **Облачен доставчик** | **AWS** (Amazon Web Services) | **Microsoft Azure** |
| **Модел на данните** | Key-Value и Document (JSON) | Мулти-модел (поддържа **MongoDB API**, NoSQL JSON, Cassandra, Graph) |
| **Къде е предпочитана?** | AWS проекти с огромен трафик (игри, пазарски колички, IoT, serverless Lambda функции). | **C# / .NET корпоративни проекти в Azure**, глобални приложения с потребители на няколко континента. |
| **Как се заявяват данни?** | Строго по **Partition Key** (и Sort Key) чрез AWS SDK. | Поддържа **SQL-подобен синтаксис** (`SELECT * FROM c WHERE c.price > 100`) или директно C# `MongoDB.Driver` / EF Core. |
| **Основен капан** | Много скъпо и бавно е да правиш `Scan` (търсене без Partition Key). | Може да стане скъпа, ако не настроиш правилно капацитета (RU/s — Request Units). |

---

## 🏢 4. КОРПОРАТИВНИ СИСТЕМИ: КЪДЕ СТОЯТ ACTIVE DIRECTORY И SAP?

За да не ги смесваш с базите данни, запомни точната им роля във фирмената архитектура:

| Система | Какво представлява всъщност? | Как работи с твоя C# / Web код? |
| :--- | :--- | :--- |
| **Active Directory (AD) / Microsoft Entra ID** | **Система за сигурност, вход и права** (Identity Provider). Пази служителите, паролите, групите и ролите във фирмата. | Когато потребител се логне с фирмения си имейл („Sign in with Microsoft“), Entra ID праща **JWT токен** към твоето ASP.NET Core API, за да докаже кой е и какви права има. |
| **SAP (S/4HANA)** | **Готов ERP софтуер** за управление на целия бизнес (склад, фактури, доставки, счетоводство, HR). | Ти не пишеш SAP от нулата. Твоят C# / Node.js бекенд се свързва със SAP чрез **REST API (JSON), SOAP (XML) или опашки**, за да изпраща нови поръчки или да чете наличности. |

---
⬅️ [01-04. NoSQL: MongoDB и Cassandra](01-04-NoSQL-Documents-And-WideColumn-Mongo-Cassandra.md) | 🏠 [01-00. Начало (Основи и CRUD)](01-00-SQL-INTRO-AND-CRUD.md) | ➡️ [01-06. Формати: JSON и XML](01-06-Data-Formats-JSON-And-XML.md)

© 2026 Dev-Library / Бази данни: Redis, Cloud NoSQL и Корпоративни системи
