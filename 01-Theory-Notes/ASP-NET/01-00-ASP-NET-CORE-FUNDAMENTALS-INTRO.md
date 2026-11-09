[⏪ Глава 00-00. Уводни технологии](00-01-Internet-Protocols-And-HTTP-Fundamentals.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-01. Еволюция Преди и Сега](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md) | [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)

---

# 01-00. ASP.NET Core Fundamentals (Кратък справочник)

**Цел:** Изучаване на архитектурния модел Model-View-Controller (MVC) — как се свързват трите компонента, как уеб сървърът Kestrel рутира адресите и как се изгражда първият работещ уеб контролер с реален изход и диагностика на грешки.

## Съдържание на темата
* [1. Архитектурният триъгълник: Model-View-Controller (MVC)](#1-архитектурният-триъгълник-model-view-controller-mvc)
* [2. Първият уеб контролер в C# (Реален код и Очакван изход)](#2-първият-уеб-контролер-в-c-реален-код-и-очакван-изход)
* [3. Системен капан: Грешки при маршрутизация (Routing) и как да ги поправим](#3-системен-капан-грешки-при-маршрутизация-routing-и-как-да-ги-поправим)

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
[⏪ Глава 00-00. Уводни технологии](00-01-Internet-Protocols-And-HTTP-Fundamentals.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-01. Еволюция Преди и Сега](01-01-ASP-NET-Framework-Vs-Core-Evolution-Deep-Dive.md) | [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)

© 2026 Dev-Library / ASP.NET Core: Fundamentals Overview
