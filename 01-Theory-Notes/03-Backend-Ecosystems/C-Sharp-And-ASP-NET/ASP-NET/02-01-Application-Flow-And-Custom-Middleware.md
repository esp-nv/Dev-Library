[⬅️ 02-00. Кратък справочник](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-02. Web API и Асинхронност](02-02-Web-API-And-Asynchronous-Processing.md)

---

# 02-01. Разширен уеб поток и Custom Middleware конвейер

**Цел:** Изучаване на жизнения път на заявката в напредналата архитектура, изграждане на силно капсулирани Къстъм Middleware класове, създаване на Action филтри за одит и налагане на Глобално прехващане на грешки (Global Exception Handling).

## Съдържание на темата
* [1. Пътят на филтриране: Middleware срещу Action Filters](#1-пътят-на-филтриране-middleware-срещу-action-filters)
* [2. Изграждане на чист Къстъм Middleware клас за Одит](#2-изграждане-на-чист-къстъм-middleware-клас-за-одит)
* [3. Глобален прехващач на изключения (Global Exception Handler)](#3-глобален-прехващач-на-изключения-global-exception-handler)
* [4. Системни капани: Сривове при развалена DI архитектура във филтрите](#4-системни-капани-сривове-при-развалена-di-архитектура-във-филтрите)

---

## 1. ПЪТЯТ НА ФИЛТРИРАНЕ: MIDDLEWARE СРЕЩУ ACTION FILTERS

В напредналия софтуерен дизайн съществуват две коренно различни нива за прихващане на HTTP потока, в зависимост от това колко дълбоко в .NET архитектурата трябва да проникне заявката:

*   **Middleware (Компоненти на конвейера):** Живеят в най-външния кръг на приложението. Те оперират на ниво суров HTTP контекст (`HttpContext`), преди заявката изобщо да е разпознала кой Контролер или Екшън метод трябва да се изпълни. Идеални за логване на суров трафик, IP филтри и глобално улавяне на крашове.
*   **Action Filters (Екшън Филтри):** Живеят дълбоко вътре в рамката на MVC/API архитектурата. Те се задействат **СЛЕД** като маршрутизацията е избрала конкретен C# метод. Те имат директен достъп до софтуерните параметри на метода, ModelState речниците и контекста на контролера. Идеални за специфичен бизнес одит и автоматична валидация.

---

## 2. ИЗГРАЖДАНЕ НА ЧИСТ КЪСТЪМ MIDDLEWARE КЛАС ЗА ОДИТ

Вместо да пълним `Program.cs` с дълги анонимни блокове, напредналият инженерен стандарт изисква Middleware компонентите да се капсулират в самостоятелни, чисти класове, използващи конструкторно инжектиране.

Искаме да създадем компонент, който изчислява точно колко милисекунди отнема на сървъра да преработи дадена заявка:

### 📄 1. Софтуерен код на Класа и неговото разширение:
```csharp
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

public class PerformanceLogMiddleware
{
    private readonly RequestDelegate _next;

    // Конструкторът приема следващото звено по веригата
    public PerformanceLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Задължителен за архитектурата Invoke или InvokeAsync метод
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Предаваме пакета напред по тръбопровода
        await _next(context);

        // На връщане спираме хронометъра и извеждаме лог
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        context.Response.Headers.Add("X-Execution-Time-Ms", elapsedMs.ToString());
    }
}

// Архитектурен разширяващ метод (Extension Method) за чист синтаксис
public static class PerformanceLogMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceLog(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceLogMiddleware>();
    }
}
```

### ⚙️ Регистрация в Program.cs:
```csharp
// Извикваме нашия изчистен метод в конвейера преди ендпоинтите
app.UsePerformanceLog();
app.MapControllers();
```

### 🎯 2. Очакван изход (Expected Output в Postman Headers):
Когато изпратиш произволна заявка, бекендът Kestrel връща отговора, а в таба **Headers** на Postman автоматично се появява нашата нова изчислена системна метаданна:
```text
Status: 200 OK
Content-Type: application/json
X-Execution-Time-Ms: 14
```

---

## 3. ГЛОБАЛЕН ПРЕХВАЩАЧ НА ИЗКЛЮЧЕНИЯ (GLOBAL EXCEPTION HANDLER)

В продукционни среди е абсолютно забранено крайните потребители да виждат сурови C# системни грешки и стек следи (`Stack Trace`), ако кодът крашне — това излага архитектурата на риск и изглежда непрофесионално. Създаваме един общ спасителен Middleware, който обвива целия уеб конвейер в `try-catch` блок.

### 📄 1. Софтуерен код на Глобалния прехващач:
```csharp
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Превеждаме заявката напред
        }
        catch (Exception ex)
        {
            // Ако някъде напред в контролерите кодът крашне, улавяме грешката ТУК!
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        // Налагаме твърд статус 500 Вътрешна Сървърна Грешка
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Изграждаме чист, капсулиран JSON отговор без системни тайни
        var responseError = new
        {
            StatusCode = context.Response.StatusCode,
            ErrorMessage = "Възникна неочаквана системна грешка. Моля, опитайте по-късно!",
            Detailed = exception.Message // Може да се скрие или запише само в лог файл
        };

        var jsonResult = JsonSerializer.Serialize(responseError);
        return context.Response.WriteAsync(jsonResult);
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman) при ТЕЖЪК Краш:
Ако твой метод в базата данни хвърли неочаквана грешка (напр. делене на нула или липсваща таблица), браузърът няма да умре. Клиентът ще получи цивилизован статус **`500 Internal Server Error`** и следния JSON:
```json
{
  "statusCode": 500,
  "errorMessage": "Възникна неочаквана системна грешка. Моля, опитайте по-късно!",
  "detailed": "Attempted to divide by zero."
}
```

---

## 4. СИСТЕМНИ КАПАНИ: СРИВОВЕ ПРИ РАЗВАЛЕНА DI АРХИТЕКТУРА ВЪВ ФИЛТРИТЕ

При регистрацията и работата с напреднали къстъм филтри съществува един изключително тежък капан, свързан с жизнения цикъл на паметта.

### 🚨 Капан №1: Опит за директно инжектиране на Scoped Услуга в класически Middleware
*   **Какво правиш:** Твоят къстъм Middleware клас е Singleton по рождение (създава се само веднъж при старт на сървъра). Ти обаче се опитваш да инжектираш твоя `ApplicationDbContext` (който е `Scoped` и трябва да умира след всяка заявка) директно през **конструктора** на Middleware класа:
    ```csharp
    // ❌ ФАТАЛЕН АНТИ-ПАТЕРН (Предизвиква Пленена Зависимост в конвейера!):
    public class SecurityMiddleware {
        private readonly SchoolDbContext _db;
        public SecurityMiddleware(RequestDelegate next, SchoolDbContext db) { _db = db; } // Инжектиране в конструктор
    }
    ```
*   **Точен софтуерен срив (Exception Message) при стартиране на сървъра:**
    ```text
    InvalidOperationException: Cannot resolve scoped service 'SchoolDbContext' from root provider.
    ```
*   **Защо се случва:** Тъй като Middleware компонентите се инстанцират в корена на приложението при първоначалния старт на Kestrel, те не могат безопасно да приемат краткотрайни Scoped зависимости в конструкторите си. Това води до тежък краш на Dependency Injection контейнера [01-04].
*   **Как да го поправиш:** Всички Scoped услуги (като DbContext или твоите бизнес Сервизи) трябва да се инжектират изрично като параметри **директно вътре в самия метод `InvokeAsync`**, а не в конструктора! Методът `InvokeAsync` се вика на всяка заявка и DI контейнерът може да извлече правилния Scoped обхват безопасно:

```csharp
// 🟢 ПРАВИЛЕН АРХИТЕКТУРЕН СТАНДАРТ:
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityMiddleware(RequestDelegate next) // Конструкторът приема САМО next!
    {
        _next = next;
    }

    // Инжектираме Scoped услугите като чисти параметри в самия метод!
    public async Task InvokeAsync(HttpContext context, SchoolDbContext db, IStudentService service)
    {
        // Паметта е напълно защитена и обхватите работят в перфектна изолация
        await _next(context);
    }
}
```

---
[⬅️ 02-00. Кратък справочник](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-02. Web API и Асинхронност](02-02-Web-API-And-Asynchronous-Processing.md)

© 2026 Dev-Library / ASP.NET Core Advanced: Къстъм Уеб Поток
