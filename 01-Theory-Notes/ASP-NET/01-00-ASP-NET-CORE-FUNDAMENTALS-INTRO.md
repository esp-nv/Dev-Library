[⏪ Глава 00-00. Уводни технологии](00-01-Internet-Protocols-And-HTTP-Fundamentals.md) | 📂 Глава 01. ASP.NET Core Fundamentals | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-01. Еволюция Преди и Сега](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md)

---

# 01-00. ASP.NET Core Fundamentals (Кратък справочник)

**Цел:** Изучаване на архитектурния модел Model-View-Controller (MVC) — как се свързват трите компонента, как уеб сървърът Kestrel рутира адресите и как се изгражда първият работещ уеб контролер с реален изход и диагностика на грешки.

## Съдържание на темата
* [1. Архитектурният триъгълник: Model-View-Controller (MVC)](#1-архитектурният-триъгълник-model-view-controller-mvc)
* [2. Първият уеб контролер в C# (Реален код и Очакван изход)](#2-първият-уеб-контролер-в-c-реален-код-и-очакван-изход)
* [3. Системен капан: Грешки при маршрутизация (Routing) и как да ги поправим](#3-системен-капан-грешки-при-маршрутизация-routing-и-как-да-ги-поправим)
* [Детайлни подмодули към Глава 01](#детайлни-подмодули-към-глава-01)

---

## 1. АРХИТЕКТУРНИЯТ ТРИЪГЪЛНИК: MODEL-VIEW-CONTROLLER (MVC)

Шаблонът **MVC** разделя потребителския интерфейс от бизнес данните с цел по-лесна поддръжка, тестване и пълно разделение на отговорностите (Separation of Concerns).

```text
       [ Браузър / Клиент ] ──( HTTP Request )──> [ Controller ]
               ▲                                       │
               │                                (Извиква и пълни)
        (Рендерира и връща)                            ▼
               │                                  [ Model ] (Данни)
         [ View Layer ] <──( Подава напълнен Model ) ──┘
```

*   **Model (Модел):** Чисти C# класове, които държат бизнес данните на приложението (напр. `Product`, `Student`). Те нямат представа как изглежда сайтът визуално.
*   **View (Изглед):** HTML страниците (обикновено `.cshtml` Razor файлове), които съдържат дизайна. Те приемат напълнения Модел и го форматират визуално за екрана.
*   **Controller (Контролер):** Мозъкът на уеб операцията. Приема HTTP заявката от браузъра, извлича нужните данни от базата, пакетира ги в Модел и ги изпраща към Изгледа (View).

---

## 2. ПЪРВИЯТ УЕБ КОНТРОЛЕР В C# (РЕАЛЕН КОД И ОЧАКВАН ИЗХОД)

В папка `/Controllers` създаваме нов клас `ProductsController`, наследяващ базовия `Controller`. Той ще връща списък с лаптопи:

### 📄 1. Софтуерен код (ProductsController.cs):
```csharp
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class ProductsController : Controller
{
    // Атрибут за маршрутизация: казва на какъв URL адрес отговаря този метод
    [HttpGet("/api/products/list")]
    public IActionResult GetAvailableProducts()
    {
        // Сглобяваме временен Модел с данни
        var productsList = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop ASUS ROG" },
            new Product { Id = 2, Name = "Laptop Lenovo Legion" }
        };

        // Връщаме данните автоматично форматирани като JSON пакет
        return Json(productsList);
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman / Браузър):
Ако стартираш приложението и извикаш в Postman адрес `https://localhost:7214/api/products/list`, на екрана ти ще излезе чист, подреден HTTP статус `200 OK` и следният JSON масив:
```json
[
  {
    "id": 1,
    "name": "Laptop ASUS ROG"
  },
  {
    "id": 2,
    "name": "Laptop Lenovo Legion"
  }
]
```

---

## 3. СИСТЕМЕН КАПАН: ГРЕШКИ ПРИ МАРШРУТИЗАЦИЯ (ROUTING) И КАК ДА ГИ ПОПРАВИМ

Един от най-честите сривове при начинаещи програмисти е грешката при извикване на URL адреса на контролера.

### 🚨 А. Капан №1: Пътна правописна грешка (Грешка 404)
*   **Какво правиш:** Опитваш се да достъпиш адреса, но допускаш малка правописна грешка при писане (напр. извикваш `https://localhost:7214/api/product/list` - без буквата "s").
*   **Точно съобщение за грешка на екрана:**
    ```text
    HTTP ERROR 404 - Not Found (Ресурсът не е открит)
    ```
*   **Защо се случва:** Маршрутизаторът (Routing engine) в ASP.NET Core сканира твоето уеб приложение, но не открива нито един контролер, който да е маркиран с атрибут `[HttpGet("/api/product/list")]`.
*   **Как да го поправиш:** Провери буква по буква съответствието между низа в квадратните скоби над C# метода и уеб адреса, който въвеждаш в Postman.

### 🚨 Б. Капан №2: Дублиран уеб маршрут (Грешка при стартиране)
*   **Какво правиш:** Копираш съществуващ метод, за да направиш нов, но забравяш да смениш URL текста в квадратните скоби. Имаш два C# метода, които сочат към абсолютно един и същ адрес: `[HttpGet("/api/products/list")]`.
*   **Точен софтуерен Crash (Exception Message) в конзолата:**
    ```text
    AmbiguousMatchException: The request matched multiple endpoints.
    Matches: ProductsController.GetAvailableProducts and ProductsController.GetNewProducts
    ```
*   **Защо се случва:** Сървърът Kestrel приема успешно заявката, но засича две различни C# софтуерни дестинации за един и същ път и блокира изпълнението на място, за да не изтече грешна информация.
*   **Как да го поправиш:** Промени уеб низа на единия от двата метода, за да бъде напълно уникален (напр. `/api/products/new`).

---
## Детайлни подмодули към Глава 02
*   **[00-00. Въведение в уеб технологиите и Git инженерна хигиена (Кратък справочник)](00-00-WEB-TECHNOLOGIES-AND-GIT-HYGIENE-INTRO.md)** - Нуждата от разбиране на уеб инфраструктурата, безсъстоятелния (Stateless) проблем на HTTP и общия контрол на версиите.
*   **[00-01. Мрежови протоколи и HTTP Анатомия](00-01-Internet-Protocols-And-HTTP-Fundamentals.md)** -     *   *Обхваща:* Пакетна комуникация, IPv4/IPv6, DNS сървъри, transport layers TCP срещу UDP, анатомия на URL и съпоставка на HTTP методите с CRUD.
*   **[00-02. Git и проектна документация: Най-добри практики](00-02-Git-And-Project-Documentation-Best-Practices.md)**
    *   *Обхваща:* Разлики между Git и GitHub, атомарни коммити (Atomic Commits), правилна конфигурация на индустриален `.gitignore` за .NET и 5-те стълба на README.
*   **[00-03. Тестване на уеб заявки и диагностика: Postman и Chrome DevTools](00-03-Postman-And-Chrome-DevTools-Diagnostic-Guide.md)**
    *   *Обхваща:* Сглобяване на заявки в Postman, JSON Payload, справяне с грешка `415 Unsupported Media Type`, заобикаляне на SSL и работа с DevTools десните прозорци (Styles, Computed Box Model, Layout).
*   **[00-04. State Management и Асинхронна обработка (Asynchronous Processing)](00-04-State-Management-And-Asynchronous-Processing.md)**
    *   *Обхваща:* Изграждане на изкуствена памет през HTTP Cookies и Session в C#, мениджмънт на Thread Pool под Kestrel и избягване на фатални Deadlocks.

### 🔹 Глава 01: ASP.NET Core Fundamentals (Основи & MVC Архитектура)
*   **[01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md)** -  Дълбоко навлизане в архитектурния модел Model-View-Controller (MVC), роля на слоевете и маршрутизация (Routing).
*   **[01-01. Еволюция на архитектурата: Класически .NET Framework срещу .NET Core+](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md)** -   Монолитни структури срещу модулни двигатели, изхвърлени компоненти (`System.Web`, `web.config`, `Global.asax`) и защо Kestrel постига рекордна скорост.
*   **[01-02. Жизнен цикъл и Middleware компоненти в ASP.NET Core](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md)** -   Двупосочен поток на заявката през Request Pipeline, късо съединение (Short-circuiting), златния хронологичен ред в `Program.cs` и капсулиране на къстъм `app.Use()`.
*   **[01-03. Model Binding и Валидация на данни in ASP.NET Core](01-03-Model-Binding-And-Data-Validation.md)** -  Автоматичен превод от HTTP вход към C# обекти, Binding източници (`[FromBody]`, `[FromRoute]`), валидации през Data Annotations и уязвимост от Over-Posting.
*   **[01-04. Dependency Injection Дълбоко гмуркане и Жизнени цикли](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)** -  Проблеми със силната обвързаност срещу Inversion of Control, разлики между Transient, Scoped и Singleton и улавяне на Captive Dependencies.
*   **[01-05. Архитектурни слоеве, Сервизи и пренасяне на данни (DTOs)](01-05-Layers-Services-And-Data-Transfer-Objects.md)** -  Разделение на отговорностите (Presentation, Service, Data), защита на Entity моделите от изтичане към браузъра, релационни цикли в паметта и AutoMapper Profiles.
*   **[01-06. Потребителски сесии, бисквитки и сигурност с Identity](01-06-ASP-NET-Core-Identity-Authentication.md)**Имплементиране на ASP.NET Core Identity, еднопосочно осоляване (Salting) на пароли, налагане на авторизационни Restrict филтри и разграничаване на грешки `401 Unauthorized` и `403 Forbidden`.
*   **[01-07. Етапи за изграждане на ASP.NET Core приложение и сравнение на структури](01-07-ASP-NET-Core-Application-Architecture-Steps.md)** -  Индустриални стъпки за изграждане от нулата, съпоставяне на еднопроектен монолитен MVC модел с мулти-проектна структура на чиста архитектура (Solution Layouts).
*   **[01-08. ASP.NET Core Fundamentals (Глобален Инженерен Пищов)](01-08-ASP-NET-Core-Fundamentals-CheatSheet.md)** -  Категоризирани готови шаблони с кодови структури за Middleware, DI матрици, Binding атрибути, AutoMapper и чести системни изключения с точните им решения.
*   **[01-09. Наръчник по конфигурация: AutoMapper и Identity в папка Fundamentals)](01-09-AutoMapper-Identity-Configuration-Guide.md)** -  Пълна синтактична и практическа справка за свързване, инжектиране и управление на AutoMapper Профили и ASP.NET Core Identity среди в `Program.cs` и Контролерите.
*   **[01-10. Reflection и Attributes (Дълбок Архитектурен Справочник))](01-10-Reflection-And-Attributes.md)** -  Пълно разглобяване на концепциите за Метапрограмиране, инспекция на метаданни по време на изпълнение (Reflection API), динамично извикване на конструктори и методи и изграждане на Къстъм Атрибути.
---
[⏪ Глава 00-00. Уводни технологии](00-01-Internet-Protocols-And-HTTP-Fundamentals.md) | 📂 Глава 01. ASP.NET Core Fundamentals | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-01. Еволюция Преди и Сега](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md)

© 2026 Dev-Library / ASP.NET Core: Fundamentals Overview
