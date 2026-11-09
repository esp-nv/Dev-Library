[⬅️ 02-06. Razor Рендериране и Пренос](02-06-Razor-Pages-Vs-MVC-Architectural-Contrast.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-08. Мрежов достъп през CORS](02-08-Cross-Origin-Resource-Sharing-CORS-Infrastructure.md)

---

# 02-07. Глобална уеб сигурност: Защита от XSS, SQL Injection и CSRF

**Цел:** Овладяване на трите най-критични софтуерни атаки срещу уеб приложенията (OWASP Top 10), разбиране на методите за автоматично неутрализиране на уязвимости и налагане на глобална филтрация през `AutoValidateAntiforgeryToken`.

## 🧭 Съдържание на темата
* [1. SQL Injection: Как хакерите пробиват LINQ и суровия SQL](#1-sql-injection-как-хакерите-пробиват-linq-и-суровия-sql)
* [2. XSS (Cross-Site Scripting): Сигурност на текстовия вход](#2-xss-cross-site-scripting-сигурност-на-текстовия-вход)
* [3. CSRF (Cross-Site Request Forgery) и Анти-фалшифициращи бисквитки](#3-csrf-cross-site-request-forgery-и-анти-фалшифициращи-бисквитки)
* [4. Системни капани: Черни крашове (Bad Antiforgery Token)](#4-системни-капани-черни-крашове-bad-antiforgery-token)

---

## 1. SQL INJECTION: КАК ХАКЕРИТЕ ПРОБИВАТ LINQ И СУРОВИЯ SQL

*   **Уязвимост:** Възниква, когато уеб приложението приема входящ текст от потребителя и го конкатенира директно (свързва го с `+`) в команден уеб низ към базата данни, вместо да използва параметризиране.
*   **Защита в .NET:** По подразбиране **Entity Framework Core е 100% защитен от SQL Injection**, тъй като неговият LINQ двигател автоматично превежда заявките от типа `_db.Users.Where(u => u.Name == input)` в параметризирани SQL команди (`@p0`). Опасността идва само тогава, когато програмистът реши ръчно да разпише суров SQL чрез непараметризирани стрингове.

```csharp
// ❌ ФАТАЛЕН АНТИ-ПАТЕРН (Уязвим за SQL Injection):
string query = "SELECT * FROM Users WHERE Username = '" + userInput + "'";
var users = _context.Users.FromSqlRaw(query).ToList();
// Ако userInput е: admin' OR '1'='1 -- базата данни ще отключи абсолютно всички потребители!

// 🟢 ПРАВИЛЕН АРХИТЕКТУРЕН СТАНДАРТ:
var usersSafe = _context.Users
    .FromSqlInterpolated($"SELECT * FROM Users WHERE Username = {userInput}") // Автоматично параметризира
    .ToList();
```

---

## 2. XSS (CROSS-SITE SCRIPTING): СИГУРНОСТ НА ТЕКСТОВИЯ ВХОД

*   **Уязвимост:** Настъпва, когато хакер успее да инжектира зловреден JavaScript скрипт в текстово поле на сайта (напр. в коментар), сървърът го запише без проверка в базата и след това този скрипт се зареди и изпълни в браузърите на останалите нищо неподозиращи потребители, крадейки техните бисквитки.
*   **Защита в .NET:** Razor двигателят автоматично енкодва (`HTML Encoding`) всеки отпечатан низ през директивата `@`. Текстът `<script>` се превръща в безопасни за четене символи `&lt;script&gt;`. Опасността възниква, когато изрично кажем на софтуера да спре защитата си чрез метода `@Html.Raw()`.

---

## 3. CSRF (CROSS-SITE REQUEST FORGERY) И АНТИ-ФАЛШИФИЦИРАЩИ БИСКВИТКИ

*   **Уязвимост:** Хакерски сайт подлъгва потребителя да натисне линк, който скрито изпраща `POST` заявка към твоето банково приложение (напр. `/api/transfer?amount=1000`). Тъй като потребителят е вече логнат в твоя сайт, браузърът му автоматично прикачва неговата валидна `Identity бисквитка` към пакета. Твоят сървър вижда бисквитката, мисли заявката за легитимна и превежда парите.
*   **Защита в .NET:** Използва се защитен криптографски патерн, наречен **Antiforgery Token**. При зареждане на форма, сървърът инжектира уникален скрит ключ във формата ни (`__RequestVerificationToken`). При `POST` ход Kestrel изисква този токен обратно и го съпоставя с крипто бисквитката. Външни хакерски сайтове нямат достъп до този вътрешен токен и биват блокирани.

### ⚙️ Практическо налагане на Глобална защита в Program.cs:
Вместо да пишем атрибута `[ValidateAntiForgeryToken]` ръчно над всеки отделен уеб метод, налагаме глобален автоматичен одит над абсолютно всички `POST`, `PUT` и `DELETE` операции:

```csharp
builder.Services.AddControllersWithViews(options =>
{
    // Глобално налагане на анти-фалшифициращ филтър на системно ниво
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```

---

## 4. СИСТЕМНИ КАПАНИ: ЧЕРНИ КРАШОВЕ (BAD ANTIFORGERY TOKEN)

### 🚨 Капан №1: Срив 400 Bad Request при асинхронни AJAX/Postman заявки
*   **Какво правиш:** Наложила си глобална защита през `AutoValidateAntiforgeryToken`. Опитваш се да изпратиш легитимна `POST` заявка през Postman или JavaScript фронтенд към твоя уеб метод.
*   **Точен върнат статус код от сървъра (Response Status):**
    ```text
    400 Bad Request (Невалидна или липсваща софтуерна защита)
    ```
*   **Точен софтуерен Crash в конзолата на Kestrel:**
    ```text
    [WARN] Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException: The required antiforgery cookie ".AspNetCore.Antiforgery.xxxx" is not present.
    ```
*   **Как да го поправиш:** Тъй като Postman и външните SPA фронтенд приложения (React/Vue) не четат автоматично скритите HTML полета на формите, те нямат откъде да вземат токена. Трябва изрично да кажеш на .NET да изпраща токена под формата на заглавна част (`Header`), която фронтендът да улавя и връща обратно:

```csharp
// Конфигуриране на Antiforgery услугата да търси токен в HTTP Headers на пакета:
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN"; // Индустриален стандарт за име на заглавие
});
```

---
[⬅️ 02-06. Razor Рендериране и Пренос](02-06-Razor-Pages-Vs-MVC-Architectural-Contrast.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-08. Мрежов достъп през CORS](02-08-Cross-Origin-Resource-Sharing-CORS-Infrastructure.md)

© 2026 Dev-Library / Глобална Сигурност и OWASP Защити
