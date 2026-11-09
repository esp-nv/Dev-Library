[⬅️ 01-02. Жизнен цикъл и Middleware](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-04. Dependency Injection](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)

---

# 01-03. Model Binding и Валидация на данни в ASP.NET Core

**Цел:** Изучаване на механизма на автоматично преобразуване на HTTP входни данни към C# обекти (Model Binding), работа с източници на данни (`[FromBody]`, `[FromRoute]`) и налагане на сървърна защита чрез Data Annotations и `ModelState`.

## Съдържание на темата
* [1. Какво е Model Binding (Преводачът на входящи параметри)](#1-какво-е-model-binding-преводачът-на-входящи-параметри)
* [2. Източници на данни (Binding Sources)](#2-източници-на-данни-binding-sources)
* [3. Сървърна валидация чрез Data Annotations и ModelState](#3-сървърна-валидация-чрез-data-annotations-и-modelstate)
* [4. Системни капани: Сривове при невалиден вход и Сигурност](#4-системни-капани-сривове-при-невалиден-вход-и-сигурност)

---

## 1. КАКВО Е MODEL BINDING (ПРЕВОДАЧЪТ НА ВХОДЯЩИ ПАРАМЕТРИ)

Когато клиентът изпрати HTTP заявка, съдържаща данни (текст в URL адреса, параметри в Query String или JSON обект в тялото), уеб сървърът получава суров текст. **Model Binding** е вграденият механизъм в ASP.NET Core, който автоматично сканира този входящ текст, разчита ключовете и ги преобразува директно в съответните C# типове данни или сложни обекти в параметрите на твоя метод.

### Посока на превода:
```text
  HTTP Request (Суров текст / JSON) ──> [ Model Binder Engine ] ──> C# Параметър / Обект в метода
```
*Двигателят автоматично извършва парсване (напр. превръща текста `"25"` в чист `int age`) и спестява стотици редове ръчен код.*

---

## 2. ИЗТОЧНИЦИ НА ДАННИ (BINDING SOURCES)

За да помогнеш на софтуера да работи по-бързо и сигурно, можеш изрично да укажеш чрез атрибути от кой точно сектор на HTTP пакета трябва да се извлече стойността:

*   **`[FromRoute]`**: Търси стойността директно в самия уеб път на URL адреса (напр. `/api/users/{id}`).
*   **`[FromQuery]`**: Търси параметри в края на адреса след въпросителния знак (Query String - напр. `?search=Ivan&page=2`).
*   **`[FromBody]`**: Сканира суровото тяло (`Body`) на заявката. Задължителен при приемане на JSON обекти през `POST` или `PUT` методи.
*   **`[FromForm]`**: Използва се при традиционни HTML форми, изпращащи данни по стандартен кодиран модел (`application/x-www-form-urlencoded`).

---

## 3. СЪРВЪРНА ВАЛИДАЦИЯ ЧРЕЗ DATA ANNOTATIONS И MODELSTATE

Никога не трябва да се доверяваме на данните, идващи от клиента. Всеки входящ модел трябва да бъде строго валидиран на сървърно ниво, преди да бъде изпратен към бизнес логиката или записан в базата данни.

Дефинираме модел за регистрация на Студент (`StudentRegisterModel`) с ограничения в свойствата:

### 📄 1. Софтуерен код на Модела и Контролера:
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class StudentRegisterModel
{
    [Required(ErrorMessage = "Името е абсолютно задължително за попълване!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Името трябва да бъде между 3 и 20 символа!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Имейл адресът е задължителен!")]
    [EmailAddress(ErrorMessage = "Невалиден формат на имейл адреса!")]
    public string Email { get; set; } = null!;

    [Range(18, 100, ErrorMessage = "Студентът трябва да бъде пълнолетен (между 18 и 100 години)!")]
    public int Age { get; set; }
}

public class AccountController : Controller
{
    [HttpPost("/api/account/register-student")]
    public IActionResult RegisterStudent([FromBody] StudentRegisterModel model)
    {
        // Проверяваме дали входящите данни съответстват на Data Annotations правилата
        if (!ModelState.IsValid)
        {
            // Автоматично връщаме статус 400 Bad Request заедно със списък от правописните грешки
            return BadRequest(ModelState);
        }

        // Бизнес логика при успешен запис...
        return Ok(new { Message = $"Студентът {model.Name} е регистриран успешно!" });
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman) при НЕВАЛИДЕН вход:
Ако изпратиш `POST` заявка с празно име и невалидна възраст (`"age": 12`), ASP.NET Core хваща грешката и връща статус **`400 Bad Request`** със следната JSON структура:
```json
{
  "Name": [
    "Името трябва да бъде между 3 и 20 символа!"
  ],
  "Age": [
    "Студентът трябва да бъде пълнолетен (между 18 и 100 години)!"
  ]
}
```

---

## 4. СИСТЕМНИ КАПАНИ: СРИВОВЕ ПРИ НЕВАЛИДЕН ВХОД И СИГУРНОСТ

При обработката на входящи модели съществуват два изключително тежки системни капана:

### 🚨 Капан №1: Срив при несъответствие на типовете (Тип сблъсък)
*   **Какво правиш:** Имаш свойство `public int Age { get; set; }`. Клиентът изпраща в JSON тялото невалиден текстов формат: `"age": "twenty-five"`.
*   **Точен софтуерен срив (Exception Message) зад кулисите:**
    ```text
    System.Text.Json.JsonException: The JSON value could not be converted to System.Int32.
    ```
*   **Защо се случва:** Вграденият JSON десериализатор се опитва да прочете стойността, но буквите `"twenty-five"` не могат да се парснат към целочислен `int`. Понеже сривът настъпва още по време на четене на пакета, кодът в контролера ти изобщо не се достига, а Kestrel прекратява хода на заявката.
*   **Как да го избегнеш:** Промени типа в C# на Nullable (`int?`), за да може Model Binder-ът да запише `null` при грешен вход, а ти след това да го уловиш чисто чрез атрибута `[Required]` в `ModelState.IsValid`, вместо приложението да crash-не.

### 🚨 Капан №2: Уязвимост от масово присвояване (Over-Posting / Mass Assignment)
*   **Какво правиш:** За да спестиш време, в параметрите на уеб метода приемаш директно твоето физическо Entity от базата данни: `public IActionResult UpdateProfile(User user)`.
*   **Финансов/Бизнес капан:** Твоето `User` Entity съдържа свойство `public bool IsAdmin { get; set; }`. Злонамерен потребител отваря Postman, изгражда ръчен `POST` пакет и насила дописва в JSON тялото: `"isAdmin": true`. Понеже Model Binder-ът вижда такова свойство в класа, той безкритично го мапва и записва в базата данни, давайки права на администратор на обикновен хакер.
*   **Как да го избегнеш:** **Никога не приемай Entity модели в уеб параметрите!** Задължително създавай леки преносни класове (Input Models / DTOs), които съдържат само и единствено полетата, които потребителят има право да попълни (напр. само `FirstName` и `Phone`).

---
[⬅️ 01-02. Жизнен цикъл и Middleware](01-02-ASP-NET-Core-Lifecycle-And-Middleware.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-04. Dependency Injection](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md)

© 2026 Dev-Library / ASP.NET Core: Model Binding и Валидация
