[⬅️ Назад към Индекса](README.md) | 📂 Папка: Full-Stack-And-QA-Automation-Roadmap | [🏠 Начало (ASP.NET)](../README.md)

---

# 01-01. Ниво 1: Бързите победи (HTML, CSS, Bootstrap, SEO, Redis)

**Модул 01:** Основи на пълния стек | **Цел:** Светкавично синтактично и практическо навлизане в декларативния уеб дизайн, адаптивните интерфейси, техническата оптимизация и високопроизводителното RAM кеширане.

## 🧭 Съдържание на наръчника
* [1. HTML5 и CSS3: Дигиталният скелет на уеб страниците](#1-html5-и-css3-дигиталният-скелет-на-уеб-страниците)
* [2. Bootstrap 5: Адаптивни уеб форми и оформление](#2-bootstrap-5-адаптивни-уеб-форми-и-оформление)
* [3. Technical SEO: Метаданни и уеб индексиране](#3-technical-seo-метаданни-и-уеб-индексиране)
* [4. Суров Redis: Мениджмънт на Ключ-Стойност в паметта](#4-суров-redis-мениджмънт-на-ключ-стойност-в-паметта)
* [5. Системни капани: Сривове при Redis сесии и Корекция](#5-системни-капани-сривове-при-redis-сесии-и-корекция)

---

## 1. HTML5 И CSS3: ДИГИТАЛНИЯТ СКЕЛЕТ НА УЕБ СТРАНИЦИТЕ

*   **HTML5 (HyperText Markup Language):** Маркиращ език, дефиниращ структурата и съдържанието на сайта през елементи (Tags).
*   **CSS3 (Cascading Style Sheets):** Стилизиращ език, отговорен за визуалното оформление (цветове, размери, отстъпи).

### 📄 Синтактичен пример (Чист HTML и локален CSS):
```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <style>
        .custom-card {
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            padding: 15px;
            border-radius: 4px;
        }
    </style>
</head>
<body>
    <div class="custom-card">
        <h3>Заглавие на продукта</h3>
        <p>Кратко описание на бекенд архитектурата.</p>
    </div>
</body>
</html>
```

---

## 2. BOOTSTRAP 5: АДАПТИВНИ УЕБ ФОРМИ И ОФОРМЛЕНИЕ

*   **Философия:** Най-популярната в света CSS уеб библиотека, съдържаща готови, предварително дефинирани класове за бърз дизайн, който автоматично се мащабира спрямо екраните на мобилни телефони и десктоп компютри.

### 📄 Синтактичен пример (Bootstrap 5 Регистрационна форма):
```html
<div class="container mt-4">
    <div class="row justify-content-center">
        <div class="col-md-6 card p-4 shadow-sm">
            <h4 class="text-center mb-3">Вход в системата</h4>
            <form>
                <div class="mb-3">
                    <label class="form-label">Имейл адрес</label>
                    <input type="email" class="form-control" placeholder="user@softuni.bg" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">Парола</label>
                    <input type="password" class="form-control" required>
                </div>
                <button type="submit" class="btn btn-success w-100">Влез</button>
            </form>
        </div>
    </div>
</div>
```

---

## 3. TECHNICAL SEO: МЕТАДАННИ И УЕБ ИНДЕКСИРАНЕ

За един бекенд програмист оптимизацията за търсачки не е писане на статии, а **Технически одит на Kestrel изхода**:
1.  *Сървърно рендериране:* Роботите на Google четат трудно JavaScript SPA среди. Използването на MVC Razor Views гарантира, че чистият HTML е готов веднага.
2.  *Мета тагове:* Инжектиране на уникални SEO описания в `<head>` секцията на всеки уеб модел.

### 📄 Синтактичен пример (SEO Meta Headers):
```html
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Практически уеб справочник и архитектурни пищови за .NET среди.">
    <meta name="keywords" content="C#, ASP.NET Core, Redis, SQL, Software Architecture">
    <meta name="author" content="Dev-Library Team">
</head>
```

---

## 4. СУРОВ REDIS: МЕНИДЖМЪНТ НА КЛЮЧ-СЛОЙНОСТ В ПАМЕТТАТА

*   **Философия:** Изключително бърза NoSQL нерелационна база данни, работеща изцяло в RAM паметта. Тъй като вече владееш `IDistributedCache` в ASP.NET Core, под капака тя пренася чисти низове или байтове.

### 📄 Суров C# Код за директна сесийна Redis проверка:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;

public class CacheItem { public string Data { get; set; } = null!; }

[ApiController]
[Route("api/v1/cache")]
public class CachingController : ControllerBase
{
    private readonly IDistributedCache _redisCache;

    public CachingController(IDistributedCache redisCache) => _redisCache = redisCache;

    [HttpGet("fetch/{key}")]
    public async Task<IActionResult> GetValue(string key)
    {
        // Директно асинхронно извличане от Redis RAM паметта
        string? cacheData = await _redisCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cacheData))
        {
            return NotFound(new { Error = "Кеш мис: Ключът е изтекъл или не съществува в Redis!" });
        }

        var item = JsonSerializer.Deserialize<CacheItem>(cacheData);
        return Ok(item);
    }
}
```

---

## 5. СИСТЕМНИ КАПАНИ: СРИВОВЕ ПРИ REDIS СЕСИИ И КОРЕКЦИЯ

### 🚨 Капан №1: Срив 500 при опит за директно четене на сложен обект в Redis среди
*   **Какво правиш:** Опитваш се да извлечеш сложен обект `List<User>` от Redis, забравяйки че разпределеният кеш не е локална памет и не поддържа директни C# инстанции.
*   **Точен софтуерен краш (Exception Message) в конзолата на Kestrel:**
    ```text
    JsonException: The input does not contain any valid JSON tokens. Expected the input to start with a valid JSON token.
    ```
*   **Защо се случва:** Redis пази данните като суров байтов масив или чист плосък текст. Ако се опиташ да прочетеш несериализиран обект, .NET десериализаторът засича счупени байтове и крашва софтуера веднага.
*   **Как да го поправиш:** Винаги обвивайте операциите по следния златен стандарт:
    1. *Запис:* `JsonSerializer.Serialize(обект)` ➔ `await _cache.SetStringAsync(key, text);`
    2. *Четене:* `string text = await _cache.GetStringAsync(key);` ➔ `JsonSerializer.Deserialize<T>(text);`

---
[⬅️ Назад към Индекса](README.md) | 📂 Папка: Full-Stack-And-QA-Automation-Roadmap | [🏠 Начало (ASP.NET)](../README.md)

© 2026 Dev-Library / Основи на пълния стек: Ниво 1
