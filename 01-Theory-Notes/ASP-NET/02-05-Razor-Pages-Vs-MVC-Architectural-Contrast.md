 [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) |

---

# 02-05. Архитектурен контраст: Razor Pages срещу ASP.NET Core MVC

**Цел:** Дълбоко софтуерно съпоставяне на двата основни модела за разработка на потребителски интерфейс на сървърно ниво — класическата трислойна MVC структура срещу модерната страница-ориентирана MVVM архитектура (Razor Pages), критерии за избор, реални кодове и управление на грешки.

## 🧭 Съдържание на темата
* [1. Архитектурна философия: MVC срещу Razor Pages (MVVM)](#1-архитектурна-философия-mvc-срещу-razor-pages-mvvm)
* [2. Сравнителен C# код: Реализация на една и съща форма](#2-сравнителен-c-код-реализация-на-една-и-съща-форма)
* [3. Плюсове, минуси и архитектурни компромиси (Trade-offs)](#3-плюсове-минуси-и-архитектурни-компромиси-trade-offs)
* [4. Системни капани: Грешки при Handler съответствия (Routing Сривове)](#4-системни-капани-грешки-при-handler-съответствия-routing-сривове)

---

## 1. АРХИТЕКТУРНА ФИЛОСОФИЯ: MVC СРЕЩУ RAZOR PAGES (MVVM)

Докато класическото **MVC** разделя приложението хоризонтално (всички контролери на едно място, всички изгледи в друга папка), модерната система **Razor Pages** разделя приложението вертикално на базата на функционални функции (Feature-focused). Тя стъпва върху **MVVM (Model-View-ViewModel)** шаблона:

*   **Класическо MVC:** HTTP заявката удря Контролера, той извиква Бизнес средите и накрая избира кое View да зареди. Един контролер често управлява десетки различни уеб действия (Actions).
*   **Razor Pages:** Всеки уеб екран е напълно самостоятелна, изолирана двойка от два файла: `.cshtml` (HTML дизайна / View) и `.cshtml.cs` (C# логиката / PageModel / ViewModel). Вместо екшън методи, тук комуникацията се извършва през специфични улавящи манипулатори, наречени **Handler Methods** (напр. `OnGet()`, `OnPost()`), които реагират на HTTP глаголите автоматично.

---

## 2. СРАВНИТЕЛЕН C# КОД: РЕАЛИЗАЦИЯ НА ЕДНА И СЪЩА ФОРМА

Искаме да изградим уеб екран за бързо записване на имейл бюлетин с валидация на данните. Ето как се реализира софтуерно при двата модела:

### 🔹 Подход А: Реализация чрез Razor Pages (Изолиран PageModel)
В Razor Pages нямаш външен Контролер. Цялата памет и вход се капсулират в двойката файлове на самата страница:

#### 📄 1. Софтуерен C# код (Newsletter.cshtml.cs):
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class NewsletterModel : PageModel
{
    // Атрибутът [BindProperty] обвързва автоматично уеб формата към това C# свойство при POST заявка!
    [BindProperty]
    [Required(ErrorMessage = "Имейлът е задължителен за участие!")]
    [EmailAddress(ErrorMessage = "Невалиден имейл формат!")]
    public string SubscriberEmail { get; set; } = null!;

    // Автоматичен Handler за улавяне на входящия GET трафик
    public void OnGet()
    {
        // Изпълнява се първоначално при отваряне на екрана...
    }

    // Автоматичен Handler за улавяне на изпратената POST форма
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            // Ако данните не съвпадат с валидацията, презареждаме СЪЩАТА страница веднага
            return Page(); 
        }

        // Логика за запис в базата...
        return RedirectToPage("/Index"); // Сигурен заден ход през PRG патерна
    }
}
```

---

### 🔹 Подход Б: Традиционна реализация чрез класическо MVC
При MVC си длъжна да разбиеш тази логика в два напълно отделни сектора на уеб проекта:

#### 📄 1. Софтуерен C# код в Контролера (NewsletterController.cs):
```csharp
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class NewsletterInputModel
{
    [Required(ErrorMessage = "Имейлът е задължителен за участие!")]
    [EmailAddress(ErrorMessage = "Невалиден имейл формат!")]
    public string SubscriberEmail { get; set; } = null!;
}

public class NewsletterController : Controller
{
    [HttpGet("/newsletter/signup")]
    public IActionResult Subscribe() => View(); // Търси файл във \Views\Newsletter\Subscribe.cshtml

    [HttpPost("/newsletter/signup")]
    public IActionResult Subscribe(NewsletterInputModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        // Логика за запис...
        return RedirectToAction("Index", "Home");
    }
}
```

---

## 3. ПЛЮСОВЕ, МИНУСИ И АРХИТЕКТУРНИ КОМПРОМИСИ (TRADE-OFFS)

### 🟢 Razor Pages среди (MVVM модел)
*   **Плюсове:** 
    *   *Висока четимост при форми:* Идеална архитектура за уеб страници, които работят с единични формуляри (регистрации, контактни форми, детайли). Всичко нужно за екрана е събрано на едно място.
    *   *По-малко софтуерен баласт:* Спестява постоянното скачане между три папки (`Controllers` ➔ `Models` ➔ `Views`) във VS Code.
*   **Минуси:** Не е подходяща за изграждане на чисти RESTful Web APIs, тъй като е тясно обвързана с физическото рендериране на Razor страниците на сървъра.

### 🔵 Класическо MVC (Model-View-Controller)
*   **Плюсове:** Златен стандарт за мащабни Enterprise системи със сложна навигация, където едно действие пренасочва динамично към десетки различни изгледи в зависимост от сложни бизнес правила.
*   **Минуси:** Излишно усложняване (Over-engineering) при малки уеб проекти с прости екрани.

---

## 4. СИСТЕМНИ КАПАНИ: ГРЕШКИ ПРИ HANDLER СЪОТВЕТСТВИЯ (ROUTING СРИВОВЕ)

Един от най-тежките капани при Razor Pages е грешното написване на улавящите Handler методи.

### 🚨 Капан №1: Невалидно име на POST манипулатора (Грешка 405 Method Not Allowed)
*   **Какво правиш:** Искаш да обработиш изпратена форма и кръщаваш C# метода си `public IActionResult PostData()`.
*   **Точен върнат статус код от сървъра (Response Status):**
    ```text
    HTTP ERROR 405 - Method Not Allowed (Методът не е разрешен)
    ```
*   **Защо се случва:** Маршрутизаторът на Razor Pages е изключително строг към конвенциите за имена. Той търси методи, започващи задължително с представката **`On`**, последвана точно от HTTP глагола: **`OnGet()`** или **`OnPost()`**. Тъй като твоят метод няма представка `On`, .NET го мисли за обикновена функция, пропуска го, не открива кой да обработи формата и Kestrel спира заявката.
*   **Как да го поправиш:** Винаги спазвай закования Razor синтаксис: `public IActionResult OnPost()` или асинхронния му еквивалент `public async Task<IActionResult> OnPostAsync()`.

---
 [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)| | [🏠 Начало (ASP.NET)](README.md) |

© 2026 Dev-Library / Архитектура: Razor Pages срещу MVC
