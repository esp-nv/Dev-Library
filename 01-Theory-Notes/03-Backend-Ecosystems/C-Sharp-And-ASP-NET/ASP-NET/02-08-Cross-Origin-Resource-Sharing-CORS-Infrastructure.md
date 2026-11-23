[⬅️ 02-07. Глобална Софтуерна Сигурност](02-07-Global-Web-Security-XSS-SQL-Injection-And-CSRF.md) | 
 [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-09. Автоматизирано Селениум Тестване](02-09-UI-Automation-Testing-Selenium-And-Fluent-Assertions.md)

---

# 02-08. Мрежов суверен достъп през CORS и Same-Origin Policy

**Цел:** Изучаване на браузърната сигурност, наложена от Same-Origin Policy (SOP), разясняване на механизма за крос-домейн достъп (CORS) и правилно конфигуриране на споделени корпоративни политики в ASP.NET Core Web APIs.

## 🧭 Съдържание на темата
* [1. Браузърният затвор: Какво е Same-Origin Policy (SOP)?](#1-браузърният-затвор-какво-е-same-origin-policy-sop)
* [2. Спасителният клапан: Как работи CORS и Preflight заявките (OPTIONS)](#2-спасителният-клапан-как-работи-cors-и-preflight-заявките-options)
* [3. Практически C# код за активиране на CORS Политики](#3-практически-c-код-за-активиране-на-cors-политики)
* [4. Системни капани: Блокирани ресурси (CORS Header Missing)](#4-системни-капани-блокирани-ресурси-cors-header-missing)

---

## 1. БРАУЗЪРНИЯТ ЗАТВОР: КАКВО Е SAME-ORIGIN POLICY (SOP)?

Всеки съвременен уеб браузър налага желязна софтуерна рестрикция, наречена **Same-Origin Policy (Политика за еднакъв произход)**. Произходът (`Origin`) се дефинира от три стълба: **Протокол**, **Домейн (Хост)** и **Порт**. Ако те съвпадат, браузърът позволява пълен достъп.

Ако твоят фронтенд работи на `http://localhost:3000` (React), а бекендът ти на .NET Core слуша на `https://localhost:7214`, те имат различен порт и протокол. Браузърът веднага ги класифицира като два коренно различни сайта и по подразбиране **блокира насила** правото на JavaScript кода от React да прочете JSON данните, дошли от C#.

---

## 2. КАК РАБОТИ CORS И PREFLIGHT ЗАЯВКИТЕ (OPTIONS)

За да позволим на нашия декуплиран фронтенд безопасно да чете от бекенда, уеб сървърът Kestrel трябва изрично да изпрати специални разрешаващи заглавни части в HTTP отговора. Този механизъм се нарича **CORS (Cross-Origin Resource Sharing)**.

### Механизъм на Preflight заявката:
Когато JavaScript се опита да направи сложна заявка (напр. `POST` с JSON тяло) към различен домейн, браузърът не я изпраща веднага. Той първо прави скрито фоново сондиране, наречено **Preflight Request**, използвайки HTTP метода **`OPTIONS`**. Сървърът Kestrel улавя това OPTIONS запитване, вижда кои домейни питат и ако са разрешени, връща метаданни `Access-Control-Allow-Origin`. Чак тогава браузърът се успокоява и пуска реалната `POST` заявка с данните.

---

## 3. ПРАКТИЧЕСКИ C# КОД ЗА АКТИВИРАНЕ НА CORS ПОЛИТИКИ

Конфигурирането изисква два ясни етапа в централния файл `Program.cs`:

### 📄 1. Софтуерен код (в Program.cs):
```csharp
var builder = WebApplication.CreateBuilder(args);

// ЕТАП 1: Добавяне на CORS услугите и дефиниране на корпоративна политика
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://myproduction-site.com") // Посочваме точните домейни!
              .WithMethods("GET", "POST", "PUT", "DELETE") // Разрешаваме само тези глаголи
              .WithHeaders("Content-Type", "Authorization"); // Позволяваме инжектиране на JWT токени
    });
});

var app = builder.Build();

app.UseRouting();

// ЕТАП 2: Активиране на CORS Middleware компонента в конвейера!
// ЗАДЪЛЖИТЕЛНО се поставя точно СЛЕД UseRouting и твърдо ПРЕДИ UseAuthentication/UseAuthorization!
app.UseCors("FrontendAngularApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## 4. СИСТЕМНИ КАПАНИ: БЛОКИРАНИ РЕСУРСИ (CORS HEADER MISSING)

### 🚨 Капан №1: Червена браузърна CORS блокада (Срив във фронтенда)
*   **Какво се случва:** Извикваш API-то от фронтенд кода успешно, но на екрана не излизат никакви данни. Влизаш в Chrome DevTools, отваряш таба `Console` и виждаш голяма червена системна грешка.
*   **Точен софтуерен срив (Error Message) в конзолата на браузъра:**
    ```text
    Access to XMLHttpRequest at 'https://localhost:7214/api/v1/data' from origin 'http://localhost:4200' has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource.
    ```
*   **Защо се случва:** Ти си забравила да извикаш метода `app.UseCors("Политика");` в `Program.cs`, или си объркала софтуерната подредба в конвейера, поставяйки CORS компонента най-отдолу. Понеже Kestrel връща HTTP отговор без метаданната `Access-Control-Allow-Origin`, браузърът спазва SOP закона и скрива данните от JavaScript паметта насила.
*   **Как да го поправиш:** Провери дали името на политиката в `AddPolicy` съвпада буква по буква с името в `UseCors()`. Подсигури, че в стринга на домейните няма затваряща наклонена черта накрая (напр. `"http://localhost:4200/"` — това е невалиден Origin низ и .NET ще го отхвърли, пиши само `"http://localhost:4200"`).

---
[⬅️ 02-07. Глобална Софтуерна Сигурност](02-07-Global-Web-Security-XSS-SQL-Injection-And-CSRF.md) | 
 [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-09. Автоматизирано Селениум Тестване](02-09-UI-Automation-Testing-Selenium-And-Fluent-Assertions.md)

© 2026 Dev-Library / Справочник: CORS и Мрежова Инфраструктура
