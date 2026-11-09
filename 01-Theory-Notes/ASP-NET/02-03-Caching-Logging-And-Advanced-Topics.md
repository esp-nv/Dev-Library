[⬅️ 02-02. Web API и Асинхронност](02-02-Web-API-And-Asynchronous-Processing.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-04. Напреднал Глобален Пищов](02-04-ASP-NET-Core-Advanced-CheatSheet.md)

---

# 02-03. Кеширане, Логване на заявки и Облачен деплой

**Цел:** Изучаване на стратегии за оптимизация на натоварването чрез разпределено кеширане (`IDistributedCache`), изграждане на професионално структурирано логване (Logging) и имплементиране на двупосочна уеб комуникация в реално време чрез SignalR Hubs.

## Съдържание на темата
* [1. Стратегии за Кеширане: In-Memory срещу Distributed Cache (Redis)](#1-стратегии-за-кеширане-in-memory-срещу-distributed-cache-redis)
* [2. Практическа реализация на IDistributedCache и Очакван изход](#2-практическа-реализация-на-idistributedcache-и-очакван-изход)
* [3. Двупосочна комуникация в реално време със SignalR Hubs](#3-двупосочна-комуникация-в-реално-време-със-signalr-hubs)
* [4. Системни капани: Грешки при кеш фрагментация и Сериализация](#4-системни-капани-грешки-при-кеш-фрагментация-и-сериализация)

---

## 1. СТРАТЕГИИ ЗА КЕШИРАНЕ: IN-MEMORY СРЕЩУ DISTRIBUTED CACHE (REDIS)

За да предпазим базата данни (SQL Server) от претоварване при милиони еднакви заявки (напр. зареждане на начална страница на онлайн магазин), използваме **Кеширане (Caching)**. Данните се записват веднъж под формата на ключ-стойност двойка в бърза RAM памет.

*   **In-Memory Caching:** Пази кеш кутиите директно вътре в RAM паметта на текущия работещ Kestrel уеб сървър. Изключително бързо, но ако имаш два паралелни сървъра зад балансьор на натоварването (Load Balancer), те ще виждат различен кеш.
*   **Distributed Caching (Разпределено кеширане):** Кешът се изнася в отделна, независима външна RAM база данни, работеща като индустриален стандарт (най-често **Redis**). Всички паралелни сървъри четат и пишат в този общ център. Ако един уеб сървър крашне и се рестартира, кешът не се изгубва.

---

## 2. ПРАКТИЧЕСКА РЕАЛИЗАЦИЯ НА IDISTRIBUTEDCACHE И ОЧАКВАН ИЗХОД

Чрез интерфейса `IDistributedCache` в ASP.NET Core можем да разпишем абстрактен код, който работи по еднакъв начин, независимо дали отдолу сме вързали локална памет или тежък Redis сървър.

Искаме да кешираме ценови продуктов каталог за 10 минути:

### 📄 1. Софтуерен код на API метода:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/catalog")]
public class CatalogApiController : ControllerBase
{
    private readonly IDistributedCache _cache;

    public CatalogApiController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetCachedCatalog()
    {
        string cacheKey = "Catalog_Products_List";
        
        // 1. Опит за четене от Разпределения кеш
        string? cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Кеш ХИТ (Cache Hit): Данните са намерени бързо в RAM!
            var products = JsonSerializer.Deserialize<object>(cachedData);
            return Ok(new { Source = "Redis_Cache", Data = products });
        }

        // 2. Кеш МИС (Cache Miss): Данните ги няма в RAM. Отиваме до тежката SQL DB
        var freshDbData = new[] { new { Id = 101, Title = "Pro CPU v2" } };

        // Настройваме времето на живот на кеша
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Изтрий автоматично след 10 мин
        };

        // Сериализираме обекта до JSON текст и го записваме в кеша
        string jsonText = JsonSerializer.Serialize(freshDbData);
        await _cache.SetStringAsync(cacheKey, jsonText, cacheOptions);

        return Ok(new { Source = "Microsoft_SQL_Database", Data = freshDbData });
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman) при първо и второ повикване:

*   **Повикване №1 (Cache Miss):** Тъй като паметта е празна, сървърът чете бавно от твърдия диск и връща:
    ```json
    {
      "source": "Microsoft_SQL_Database",
      "data": [{ "id": 101, "title": "Pro CPU v2" }]
    }
    ```
*   **Повикване №2 (Cache Hit):** Извикваш същия адрес 1 милисекунда след това. Кодът извлича данните за частица от секундата директно от RAM паметта и връща:
    ```json
    {
      "source": "Redis_Cache",
      "data": [{ "id": 101, "title": "Pro CPU v2" }]
    }
    ```

---

## 3. ДВУПОСОЧНА КОМУНИКАЦИЯ В РЕАЛНО ВРЕМЕ СЪС SIGNALR HUBS

Традиционният HTTP протокол изисква браузърът изрично да поиска данни, за да получи отговор. При чат системи, финансови борси или спортни резултати обаче се изисква **сървърът сам да бута данни (Server-Push)** към екрана на потребителя веднага щом възникне събитие. За целта използваме библиотеката **SignalR**, която изгражда постоянна, двупосочна мрежова връзка през **WebSockets**.

### 📄 1. Софтуерен код на Сървърния Хъб (NotificationHub.cs):
```csharp
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    // Метод, който уеб клиентите могат да извикват отдалечено през JavaScript
    public async Task BroadcastSystemMessage(string user, string message)
    {
        // Изпраща съобщението моментално до абсолютно всички свързани браузъри по света в същата милисекунда!
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }
}
```

### ⚙️ Регистрация в Program.cs:
```csharp
builder.Services.AddSignalR(); // Добавя услугите за реално време
// ...
app.UseRouting();
app.MapHub<NotificationHub>("/hubs/notifications"); // Начертава софтуерния WebSocket път
```

---

## 4. СИСТЕМНИ КАПАНИ: ГРЕШКИ ПРИ КЕШ ФРАГМЕНТАЦИЯ И СЕРИАЛИЗАЦИЯ

При конфигурирането на разпределени среди в облака съществуват два изключително напреднали архитектурни капана:

### 🚨 Капан №1: Срив при директно кастване на сложни обекти в IDistributedCache
*   **Какво правиш:** За разлика от `IMemoryCache`, който поддържа запис на цели сложни C# обекти в локалната памет, `IDistributedCache` работи само със сурови байтове или стрингове. Ти се опитваш да подадеш директно C# списък `List<Product>` в Redis метода:
    ```csharp
    // ❌ ФАТАЛЕН КРАШ (Кодът изобщо няма да се компилира!):
    await _cache.SetAsync("key", productsList); 
    ```
*   **Точно софтуерно обяснение на срива:** Интерфейсът `IDistributedCache` няма вграден десериализатор за облачни мрежи. Той изисква обектите изрично да се превърнат в плосък JSON текст или суров байтов масив (`byte[]`), преди да напуснат паметта на приложението.
*   **Как да го поправиш:** Винаги използвай `JsonSerializer.Serialize()` за превръщане в текст преди запис и съответно `JsonSerializer.Deserialize<T>()` след извличане от паметта, както е разписано в Секция 2.

### 🚨 Капан №2: Липсваща Redis софтуерна инфраструктура (Срив при деплоймент)
*   **Какво правиш:** В `Program.cs` написваш конфигурацията за Redis провайдър: `builder.Services.AddStackExchangeRedisCache(...)`. Стартираш проекта, но уеб сървърът Kestrel блокира веднага и отказва да зареди.
*   **Точен софтуерен Crash (Exception Message) в конзолата:**
    ```text
    RedisConnectionException: It was not possible to connect to the redis server(s). Connection refused.
    ```
*   **Защо се случва:** Ти си казала на C# кода да пренасочи разпределения кеш към Redis сървър, но на локалната ти машина или в облачния Docker контейнер няма реално инсталиран и стартиран работещ Redis двигател на порт `6379`.
*   **Как да го поправиш:** Подсигури, че Redis сървърът е физически пуснат (напр. през Docker команда: `docker run -d -p 6379:6379 redis`), или по време на локална разработка смени конфигурацията към InMemory симулация: `builder.Services.AddDistributedMemoryCache();`, която ще симулира разпределен кеш изцяло вътре в твоята RAM памет, без да търси външен сървър.

---
[⬅️ 02-02. Web API и Асинхронност](02-02-Web-API-And-Asynchronous-Processing.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-04. Напреднал Глобален Пищов](02-04-ASP-NET-Core-Advanced-CheatSheet.md)

© 2026 Dev-Library / ASP.NET Core Advanced: Оптимизации и Кеш
