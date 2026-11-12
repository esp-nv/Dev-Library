[⬅️ 04-03. Spring Core (IoC & Beans)](04-03-00-Spring-Core-IoC.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-04-01. Syntax From Scratch](04-04-01-Syntax-From-Scratch.md)

---

# 04-04-00. Spring MVC with Thymeleaf & Validation (`@Valid` — Теория & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 04:** Архитектура на **Spring MVC (`DispatcherServlet` ➔ `HandlerMapping` ➔ `Controller` ➔ `Model` ➔ `ViewResolver`)**, разлика между `@Controller` и `@RestController`, сървърно рендиране на HTML с **Thymeleaf**, сървърна валидация с **Jakarta Bean Validation (`@Valid` & `BindingResult`)**, архитектурният патерн **PRG (Post-Redirect-Get)** и сравнение с други езици.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Поток на Spring MVC и PRG Патерна (Post-Redirect-Get Блок-схема)](#2-поток-на-spring-mvc-и-prg-патерна-post-redirect-get-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring MVC + Thymeleaf, ASP.NET Razor, Django Templates, Express EJS)](#3-сравнителна-карта-в-4-езика-spring-mvc--thymeleaf-aspnet-razor-django-templates-express-ejs)
* [4. Плюсове и минуси на Server-Side Rendering (Thymeleaf) срещу SPA (React / Angular)](#4-плюсове-и-минуси-на-server-side-rendering-thymeleaf-срещу-spa-react--angular)
* [5. Кодов одит: Правилен срещу Грешен код (Редът на `BindingResult` и двойно изпращане на форма при Refresh)](#5-кодов-одит-правилен-срещу-грешен-код-редът-на-bindingresult-и-двойно-изпращане-на-форма-при-refresh)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring MVC + Thymeleaf) срещу C# 12+ (ASP.NET Core MVC / Razor)](#7-архитектурно-сравнение-java-21-spring-mvc--thymeleaf-срещу-c-12-aspnet-core-mvc--razor)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`@Controller` срещу `@RestController`:**
    *   **`@Controller`** връща име на HTML шаблон или пренасочване (`redirect:`).
    *   **`@RestController`** комбинира `@Controller` и `@ResponseBody`, връщайки директно сурови данни (JSON/XML).
*   **Патернът PRG (Post-Redirect-Get):**
    *   След `POST` заявка никога не се връща HTML директно, за да се избегне повторно изпращане на формата при натискане на F5. Винаги се прави пренасочване (`redirect`) към `GET` адрес.

---

## 2. ПОТОК НА SPRING MVC И PRG ПАТЕРНА (POST-REDIRECT-GET БЛОК-СХЕМА)

При грешки във формата валидационните резултати се записват във Flash атрибути и се прави пренасочване обратно към страницата с формата. При успешен запис се пренасочва към дестинация след `GET` заявка.

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING MVC + THYMELEAF, ASP.NET RAZOR, DJANGO TEMPLATES, EXPRESS EJS)

| Концепция | ☕ Java 21+ (Spring MVC + Thymeleaf) | 🟣 C# 12+ (ASP.NET Core Razor) | 🐍 Python 3.x (Django) | 🟡 JS / TS (Express + EJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Подаване на данни към View** | `model.addAttribute("users", list)` | `return View(model);` | `render(req, "t.html", {"users": l})` | `res.render("t", { users: l })` |
| **Извеждане на текст (HTML-escaped)** | `<span th:text="${user.name}"></span>` | `<span>@Model.Name</span>` | `<span>{{ user.name }}</span>` | `<span><%= user.name %></span>` |
| **Цикъл в шаблона** | `<li th:each="u : ${users}">` | `@foreach (var u in Model.Users)` | `{% for u in users %}` | `<% users.forEach(u => { %>` |
| **Свързване на форма с DTO** | `<form th:object="${dto}">` + `th:field="*{name}"` | `<input asp-for="Name" />` | `{{ form.as_p }}` | `<input name="name" />` |
| **Анотации за валидация** | `@NotBlank`, `@Size`, `@Email`, `@Min` | `[Required]`, `[StringLength]`, `[EmailAddress]` | `forms.CharField(max_length=...)` | `express-validator` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SERVER-SIDE RENDERING (THYMELEAF) СРЕЩУ SPA (REACT / ANGULAR)

*   **Предимства на Thymeleaf:** Естествени HTML5 шаблони (валидни без стартиран сървър) и по-добро SEO с бързо първоначално зареждане.
*   **Недостатъци:** Пълно презареждане на страницата при навигация, поради което за динамични интерфейси SPA или REST API е по-подходящ избор.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (РЕДът НА `BINDINGRESULT` И ДВОЙНО ИЗПРАЩАНЕ НА ФОРМА ПРИ REFRESH)

*   **Критичен капан:** Параметърът `BindingResult` **трябва** да бъде позициониран веднага след `@Valid` обекта. Всякакви други аргументи между тях водят до грешка при свързването и хвърляне на изключение (`MethodArgumentNotValidException`).
*   **Правилен подход:** Използване на `@ModelAttribute` за инициализация на модела и пренасочване чрез `RedirectAttributes` (`addFlashAttribute`) за запазване на грешките и въведените данни след редирект.

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **`@NotNull`:** Валидира само срещу `null` (празни стрингове и интервали са валидни).
*   **`@NotEmpty`:** Проверява за `null` и дължина $> 0$ (подходящо за колекции; интервалите се считат за валидни).
*   **`@NotBlank`:** Приложимо само за `String` — изисква поне един видим символ след почистване на интервалите (`trim()`).

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING MVC + THYMELEAF) СРЕЩУ C# 12+ (ASP.NET CORE MVC / RAZOR)

Двете платформи предлагат сходни концепции за сървърно рендиране (Thymeleaf срещу Razor Views), обвързване на параметри и управление на временни съобщения при пренасочване (`RedirectAttributes` срещу `TempData`).

---
[⬅️ 04-03. Spring Core (IoC & Beans)](04-03-00-Spring-Core-IoC.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-04-01. Syntax From Scratch](04-04-01-Syntax-From-Scratch.md)
