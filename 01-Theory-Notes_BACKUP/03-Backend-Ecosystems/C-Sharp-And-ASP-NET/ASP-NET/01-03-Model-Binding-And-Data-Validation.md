[⬅️ 01-02. Жизнен цикъл и Middleware](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-04. Dependency Injection](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)

---

# 01-03. Model Binding и Валидация на данни в ASP.NET Core

**Цел:** Изучаване на механизма за автоматично преобразуване на HTTP входящи данни към C# обекти (Model Binding), работа с източници на данни и налагане на напреднала сървърна валидация чрез Data Annotations и интерфейса `IValidatableObject`.

## 🧭 Съдържание на темата
* [1. Какво е Model Binding (Преводачът на входящи параметри)](#1-какво-е-model-binding-преводачът-на-входящи-параметри)
* [2. Източници на данни (Binding Attributes)](#2-източници-на-данни-binding-attributes)
* [3. Сървърна валидация и Бизнес правила (IValidatableObject)](#3-сървърна-валидация-и-бизнес-правила-ivalidatableobject)
* [4. Системни капани: Сривове при тип-несъответствия и Over-Posting](#4-системни-капани-сривове-при-тип-несъответствия-и-over-posting)

---

## 1. КАКВО Е MODEL BINDING (ПРЕВОДАЧЪТ НА ВХОДЯЩИ ПАРАМЕТРИ)

Когато клиентът изпрати HTTP заявка, съдържаща уеб данни (параметри в URL адреса, Query String или текстово JSON тяло), уеб сървърът получава суров текст. **Model Binding** е вграденият механизъм в ASP.NET Core, който автоматично сканира този входящ текст, разчита съответните ключове по имена и ги преобразува директно в съответните C# типове данни или сложни обекти в параметрите на твоя метод.

### Посока на превода:
```text
  HTTP Request (Суров текст / JSON) ──> [ Model Binder Engine ] ──> C# Параметър / Обект в метода
```
*Двигателят автоматично извършва парсване (напр. превръща текста `"25"` в чист `int age`) и спестява стотици редове ръчен код.*

---

## 2. ИЗТОЧНИЦИ НА ДАННИ (BINDING ATTRIBUTES)

За да помогнеш на софтуера да работи по-бързо и сигурно, можеш изрично да укажеш чрез атрибути от кой точно сектор на HTTP пакета трябва да се извлече стойността:

*   **`[FromRoute]`**: Търси стойността директно в самия уеб път на URL адреса (напр. `/api/books/{id}`).
*   **`[FromQuery]`**: Търси параметри в края на адреса след въпросителния знак (Query String - напр. `?search=Ivan&page=2`).
*   **`[FromBody]`**: Сканира суровото тяло (`Body`) на заявката. Задължителен при приемане на JSON обекти през `POST` или `PUT` методи в Web API архитектури.
*   **`[FromForm]`**: Използва се при традиционни HTML форми, изпращащи данни по стандартен модел.

---

## 3. СЪРВЪРНА ВАЛИДАЦИЯ И БИЗНЕС ПРАВИЛА (IVALIDATABLEOBJECT)

Никога не трябва да се доверяваме на данните, идващи от клиента, тъй като клиентските проверки в браузъра се заобикалят лесно. Поради тази причина **сървърната валидация е абсолютно задължителна за сигурността на софтуера**.

Когато имаме комплексна бизнес логика, която изисква сравнение между две различни свойства едновременно (например: *Дата на напускане на хотел задължително трябва да бъде след датата на пристигане*), вградените атрибути не ни вършат работа. Имплементираме интерфейса **`IValidatableObject`** директно в нашия модел.

### 📄 1. Софтуерен код на Модела и Контролера (BookingModel.cs):
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class BookingModel : IValidatableObject // ➔ Вграден интерфейс за cross-property валидация
{
    [Required(ErrorMessage = "Името на госта е абсолютно задължително!")] // source: 7
    public string GuestName { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; } // source: 7

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; } // source: 7

    // Методът Validate се извиква автоматично от .NET СЛЕД като Data Annotations филтрите преминат успешно
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Бизнес правило №1: Датата на напускане не може да бъде преди датата на пристигане
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Грешка: Датата на напускане (Check-out) задължително трябва да бъде след датата на пристигане!",
                new[] { nameof(CheckOutDate) }); // Обвързваме грешката към конкретното уеб поле
        }

        // Бизнес правило №2: Не позволяваме резервации в миналото
        if (CheckInDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "Грешка: Не можете да правите резервации със задна дата в миналото!",
                new[] { nameof(CheckInDate) }); // source: 7
        }
    }
}

public class BookingController : Controller
{
    [HttpPost("/api/store/book-room")]
    public IActionResult CreateBooking([FromBody] BookingModel model)
    {
        // Проверяваме дали моделът е валиден спрямо Data Annotations и IValidatableObject правилата
        if (!ModelState.IsValid) // source: 7
        {
            // Връщаме статус 400 Bad Request заедно със структуриран списък от грешките
            return BadRequest(ModelState);
        }

        return Ok(new { Message = $"Успешна резервация за {model.GuestName}!" });
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman) при НЕВАЛИДЕН вход:
Ако изпратиш `POST` заявка, при която напускането е конфигурирано преди пристигането, ASP.NET Core автоматично улавя срива през `IValidatableObject` и връща статус **`400 Bad Request`** със следния JSON:
```json
{
  "CheckOutDate": [
    "Грешка: Датата на напускане (Check-out) задължително трябва да бъде след датата на пристигане!"
  ]
}
```

---

## 4. СИСТЕМНИ КАПАНИ: СРИВОВЕ ПРИ ТИП-НЕСЪОТВЕТСТВИЯ И OVER-POSTING

### 🚨 Капан №1: Срив при несъответствие на типовете (JsonException)
*   **Какво правиш:** Имаш числово свойство `public int Age { get; set; }`. Клиентът изпраща в JSON тялото грешен текстов формат: `"age": "twenty-five"`.
*   **Точен софтуерен срив (Exception Message) в конзолата на Kestrel:**
    ```text
    System.Text.Json.JsonException: The JSON value could not be converted to System.Int32.
    ```
*   **Защо се случва:** Вграденият JSON десериализатор се опитва да преведе стойността, но буквите не могат да се парснат към целочислен `int`. Понеже сривът настъпва по време на четене на пакета, контролерът ти изобщо не се достига, а Kestrel прекратява заявката.
*   **Как да го избегнеш:** Промени типа в C# на Nullable (`int?`), за да може Model Binder-ът да запише `null` при грешен вход, а ти след това да го уловиш чисто чрез `ModelState.IsValid`, предпазвайки приложението от краш.

### 🚨 Капан №2: Уязвимост от масово присвояване (Over-Posting / Mass Assignment)
*   **Какво правиш:** В параметрите на уеб метода приемаш директно твоето физическо Entity от базата данни: `public IActionResult UpdateProfile(User user)`. Твоето `User` Entity съдържа чувствително администраторско свойство `public bool IsAdmin { get; set; }`.
*   **Бизнес капан:** Злонамерен потребител отваря Postman, изгражда ръчен `POST` пакет и насила дописва в JSON тялото: `"isAdmin": true`. Понеже Model Binder-ът вижда такова свойство в класа, той безкритично го мапва и записва в базата данни, давайки му администраторски права автоматично.
*   **Как да го избегнеш:** **Никога не приемай Entity модели в уеб параметрите!** Задължително съобразявай изискванията за сигурност и създавай леки преносни класове (Input Models / DTOs / ViewModels), които съдържат само и единствено полетата, които потребителят има право да попълни в конкретния уеб екран (напр. само `FirstName` и `Phone`).

---

[⬅️ 01-02. Жизнен цикъл и Middleware](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-04. Dependency Injection](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)

© 2026 Dev-Library / ASP.NET Core: Model Binding и Валидация
