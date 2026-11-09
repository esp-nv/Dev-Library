[⬅️ 02-03. Кеширане, Логване и Сигнали](02-03-Caching-Logging-And-Advanced-Topics.md) | [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)| [🏠 Начало (ASP.NET)](README.md)d)

---

# 02-04. ASP.NET Core Advanced (Глобален Напреднал Пищов)

**Цел:** Бърза, синтактична и визуална справка за Senior технически интервюта и мащабни облачни конфигурации. Наръчник за капсулиране на Middleware, JWT автентификация, IDistributedCache и неблокиращи асинхронни нишки.

---

## 🏗️ 1. ШАБЛОН ЗА ЧИСТ КЪСТЪМ MIDDLEWARE КЛАС

Индустриален стандарт за изнасяне на тежки логики от `Program.cs` в изолирани компоненти със защита на `Scoped` зависимостите чрез инжектиране в метода `InvokeAsync`.

```csharp
// 1. Капсулираният софтуерен клас (Middlewares/SecurityAuditMiddleware.cs)
public class SecurityAuditMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityAuditMiddleware(RequestDelegate next)
    {
        _next = next; // Конструкторът приема САМО следващото звено (Singleton цикъл)
    }

    // Всички Scoped зависимости (DbContext/Сервизи) се инжектират КАТО ПАРАМЕТРИ ТУК!
    public async Task InvokeAsync(HttpContext context, SchoolDbContext dbContext)
    {
        // Действие ПРЕДИ изпълнението на Контролера
        string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        await _next(context); // Предаваме пакета напред по тръбопровода

        // Действие СЛЕД изпълнението на Контролера (на връщане)
        Console.WriteLine(\$"[AUDIT] IP: {ipAddress} премина през системата.");
    }
}

// 2. Създаване на Extension Method за чист и бърз синтаксис
public static class SecurityAuditMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityAudit(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityAuditMiddleware>();
    }
}

// 3. Чисто активиране в Program.cs (преди app.MapControllers):
app.UseSecurityAudit();
```

---

## 🔒 2. БЕЗСЪСТОЯТЕЛЕН REST API И JWT BEARER КОНФИГУРАТОР

Бърз пищов за конфигурация на валидацията на криптографски токени в `Program.cs` за микроуслуги и декуплирани среди (Без сесии).

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавяне на JWT Bearer Автентификация в IoC Контейнера
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Проверява дали токенът не е изтекъл като време
        ValidateIssuerSigningKey = true, // Задължителна защита на криптографския подпис!
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});
```

---

## 🏎️ 3. СТАНДАРТ ЗА РАЗПРЕДЕЛЕНО КЕШИРАНЕ (IDISTRIBUTEDCACHE)

Универсален шаблон за безопасна облачна сериализация на сложни обекти (Cache Hit / Cache Miss логика) през Redis или InMemory среди.

```csharp
public async Task<List<ProductDto>> GetCatalogAsync()
{
    string cacheKey = "Global_Catalog_Key";
    
    // 1. Опит за извличане от RAM мрежата
    string? cachedJson = await _cache.GetStringAsync(cacheKey);
    
    if (!string.IsNullOrEmpty(cachedJson))
    {
        // 🟢 CACHE HIT: Светкавично десериализиране и изход
        return JsonSerializer.Deserialize<List<ProductDto>>(cachedJson)!;
    }
    
    // 🔴 CACHE MISS: RAM паметта е празна. Изтегляме бавно от базата
    List<ProductDto> freshData = await _dbService.GetAllFromDatabaseAsync();
    
    var cacheOptions = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15), // Твърд живот
        SlidingExpiration = TimeSpan.FromMinutes(5) // Опреснява живота, ако има постоянно четене
    };
    
    // Сериализираме обекта до текст и го бутаме в Redis
    string jsonText = JsonSerializer.Serialize(freshData);
    await _cache.SetStringAsync(cacheKey, jsonText, cacheOptions);
    
    return freshData;
}
```

---

## ⏳ 4. АСИНХРОНЕН ЗЛАТЕН ЗАКОН: ПРЕМАХВАНЕ НА БЛОКИРАНИЯТА

Матрица за съпоставка на софтуерните глаголи — как да избегнем `Thread Starvation` и мъртви хватки (`Deadlocks`) в Thread Pool средите на Kestrel.

| Желано Действие в Кода | ❌ БЛОКИРАЩО поведение (Thread Killer) | 🟢 АСИНХРОННО поведение (Non-blocking) |
| :--- | :--- | :--- |
| **Материализация на база** | `.ToList()` / `.FirstOrDefault()` | `await ...ToListAsync();` / `FirstOrDefaultAsync();` |
| **Изчакване на нишка** | `Thread.Sleep(2000);` | `await Task.Delay(2000);` |
| **Вземане на резултат**| `var data = MyMethodAsync().Result;` | `var data = await MyMethodAsync();` |
| **Затваряне на поток** | `_stream.Close();` | `await _stream.DisposeAsync();` |

---

## ❌ 5. ТАБЛИЦА НА СЕНЬОР СРИВОВЕТЕ (ADVANCED EXCEPTIONS)

| Какво изписва конзолата? (Exception) | Защо софтуерът крашва под капака? | Как се лекува веднага? (Решение) |
| :--- | :--- | :--- |
| `InvalidOperationException: Cannot resolve scoped service from root provider.` | Опитала си се да инжектираш `Scoped` зависимост (напр. DbContext) в конструктора на Middleware, който е `Singleton`. | Премести инжектирането на Scoped услугата като параметър директно в метода `InvokeAsync(context, DbContext db)`. |
| `RedisConnectionException: It was not possible to connect to the redis server.` | Кодът търси активна Redis инфраструктура на порт `6379`, но сървърът не е стартиран. | Стартирай Redis през Docker контейнер или по време на разработка превключи на `builder.Services.AddDistributedMemoryCache();`. |
| `SecurityTokenInvalidSignatureException: Signature validation failed.` | Подписът на JWT токена е подправен или Secret Key в `Program.cs` не съвпада с ключа на генериране. | Подсигури, че симетричният криптографски ключ в секция Jwt на `appsettings.json` е еднакъв навсякъде. |
| `System.Text.Json.JsonException: A possible object cycle was detected.` | Изпратила си суров Entity модел с двойна релация към браузъра (Безкраен релационен цикъл при сериализация). | Наложи използването на плоски `DTO` кутии и AutoMapper Профили, изключвайки навигационните обекти от уеб отговора. |

---
[⬅️ 02-03. Кеширане, Логване и Сигнали](02-03-Caching-Logging-And-Advanced-Topics.md) | [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)| [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-05. Архитектурен контраст: Razor Pages срещу ASP.NET Core MVC](02-05-Razor-Pages-Vs-MVC-Architectural-Contrast.md)

© 2026 Dev-Library / Справочник: Глобален Advanced Пищов
