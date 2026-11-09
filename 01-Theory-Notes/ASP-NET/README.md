[🏠 Всички бележки (01-Theory-Notes)](../README.md) | [🗄️ Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | [💾 Entity Framework Core](../Databases-And-EF-Core/) | [▶️ Започни от 00-01](00-01-Internet-Protocols-And-HTTP-Fundamentals.md)

---

# ASP.NET Core & Web Платформи — Пълен Архитектурен Атлас

Тази директория съдържа фундаментална уеб и бекенд база знания — от нисконивовата пакетна и мрежова диагностика през HTTP протокола до жизнения цикъл на заявките, Middleware тръбопроводите и Dependency Injection мениджмънта в .NET средите.

## 🧭 Навигация и Съдържание

### 🔹 Глава 00: Въведение в уеб технологиите и Git инженерна хигиена
*   **[00-00. Въведение в уеб технологиите и Git инженерна хигиена (Кратък справочник)](00-00-WEB-TECHNOLOGIES-AND-GIT-HYGIENE-INTRO.md)**
    *   *Обхваща:* Нуждата от разбиране на уеб инфраструктурата, безсъстоятелния (Stateless) проблем на HTTP и общия контрол на версиите.
*   **[00-01. Мрежови протоколи и HTTP Анатомия](00-01-Internet-Protocols-And-HTTP-Fundamentals.md)**
    *   *Обхваща:* Пакетна комуникация, IPv4/IPv6, DNS сървъри, transport layers TCP срещу UDP, анатомия на URL и съпоставка на HTTP методите с CRUD.
*   **[00-02. Git и проектна документация: Най-добри практики](00-02-Git-And-Project-Documentation-Best-Practices.md)**
    *   *Обхваща:* Разлики между Git и GitHub, атомарни коммити (Atomic Commits), правилна конфигурация на индустриален `.gitignore` за .NET и 5-те стълба на README.
*   **[00-03. Тестване на уеб заявки и диагностика: Postman и Chrome DevTools](00-03-Postman-And-Chrome-DevTools-Diagnostic-Guide.md)**
    *   *Обхваща:* Сглобяване на заявки в Postman, JSON Payload, справяне с грешка `415 Unsupported Media Type`, заобикаляне на SSL и работа с DevTools десните прозорци (Styles, Computed Box Model, Layout).
*   **[00-04. State Management и Асинхронна обработка (Asynchronous Processing)](00-04-State-Management-And-Asynchronous-Processing.md)**
    *   *Обхваща:* Изграждане на изкуствена памет през HTTP Cookies и Session в C#, мениджмънт на Thread Pool под Kestrel и избягване на фатални Deadlocks.

### 🔹 Глава 01: ASP.NET Core Fundamentals (Основи & MVC Архитектура)
*   **[01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md)**
    *   *Обхваща:* Дълбоко навлизане в архитектурния модел Model-View-Controller (MVC), роля на слоевете и маршрутизация (Routing).
*   **[01-01. Еволюция на архитектурата: Класически .NET Framework срещу .NET Core+](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md)**
    *   *Обхваща:* Монолитни структури срещу модулни двигатели, изхвърлени компоненти (`System.Web`, `web.config`, `Global.asax`) и защо Kestrel постига рекордна скорост.
*   **[01-02. Жизнен цикъл и Middleware компоненти в ASP.NET Core](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md)**
    *   *Обхваща:* Двупосочен поток на заявката през Request Pipeline, късо съединение (Short-circuiting), златния хронологичен ред в `Program.cs` и капсулиране на къстъм `app.Use()`.
*   **[01-03. Model Binding и Валидация на данни in ASP.NET Core](01-03-Model-Binding-And-Data-Validation.md)**
    *   *Обхваща:* Автоматичен превод от HTTP вход към C# обекти, Binding източници (`[FromBody]`, `[FromRoute]`), валидации през Data Annotations и уязвимост от Over-Posting.
*   **[01-04. Dependency Injection Дълбоко гмуркане и Жизнени цикли](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)**
    *   *Обхваща:* Проблеми със силната обвързаност срещу Inversion of Control, разлики между Transient, Scoped и Singleton и улавяне на Captive Dependencies.
*   **[01-05. Архитектурни слоеве, Сервизи и пренасяне на данни (DTOs)](01-05-Layers-Services-And-Data-Transfer-Objects.md)**
    *   *Обхваща:* Разделение на отговорностите (Presentation, Service, Data), защита на Entity моделите от изтичане към браузъра, релационни цикли в паметта и AutoMapper Profiles.
*   **[01-06. Потребителски сесии, бисквитки и сигурност с Identity](01-06-ASP-NET-Core-Identity-Authentication.md)**
    *   *Обхваща:* Имплементиране на ASP.NET Core Identity, еднопосочно осоляване (Salting) на пароли, налагане на авторизационни Restrict филтри и разграничаване на грешки `401 Unauthorized` и `403 Forbidden`.
*   **[01-07. Етапи за изграждане на ASP.NET Core приложение и сравнение на структури](01-07-ASP-NET-Core-Application-Architecture-Steps.md)**
    *   *Обхваща:* Индустриални стъпки за изграждане от нулата, съпоставяне на еднопроектен монолитен MVC модел с мулти-проектна структура на чиста архитектура (Solution Layouts).
*   **[01-08. ASP.NET Core Fundamentals (Глобален Инженерен Пищов)](01-08-ASP-NET-Core-Fundamentals-CheatSheet.md)**
    *   *Обхваща:* Категоризирани готови шаблони с кодови структури за Middleware, DI матрици, Binding атрибути, AutoMapper и чести системни изключения с точните им решения.

### 🔹 Глава 02: ASP.NET Core Advanced (Напреднала Облачна Архитектура)
*   **[02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)**
    *   *Обхваща:* Корпоративни принципи за софтуерен инженеринг, къстъм филтриращи двигатели, безсъстоятелни REST APIs и хоризонтално мащабиране под високо натоварване.
*   **[02-01. Разширен уеб поток и Custom Middleware конвейер](02-01-Application-Flow-And-Custom-Middleware.md)**
    *   *Обхваща:* Изграждане на къстъм изолирани Middleware класове, Extension методи за конвейера, Action Filters за проследяване на време и централизирано улавяне на грешки.
*   **[02-02. Web API проектиране и Напреднала асинхронна обработка](02-02-Web-API-And-Asynchronous-Processing.md)**
    *   *Обхваща:* Проектиране на чисти REST ендпоинти, проверка на JSON Web Tokens (JWT) криптографски подписи и защита от Thread Starvation крашове при голям облачен трафик.
*   **[02-03. Кеширане, Логване на заявки и Облачен деплой](02-03-Caching-Logging-And-Advanced-Topics.md)**
    *   *Обхваща:* In-Memory кеширане срещу Разпределено кеширане с Redis (`IDistributedCache`), структурирано файлово логване и двупосочна уеб комуникация в реално време през SignalR Hubs.
*   **[02-04. ASP.NET Core Advanced (Глобален Напреднал Пищов)](02-04-ASP-NET-Core-Advanced-CheatSheet.md)**
    *   *Обхваща:* Синтактичен пищов за къстъм Middleware извиквания, JWT параметризация, Redis сериализация на обекти, неблокиращи асинхронни съответствия и напреднали runtime грешки.

---

[🏠 Всички бележки (01-Theory-Notes)](../README.md) | [🗄️ Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | [💾 Entity Framework Core](../Databases-And-EF-Core/) | [▶️ Започни от 00-01](00-01-Internet-Protocols-And-HTTP-Fundamentals.md)

© 2026 Dev-Library / Пълна архитектурна библиотека по Мрежи, Жизнен цикъл и Уеб сигурност
