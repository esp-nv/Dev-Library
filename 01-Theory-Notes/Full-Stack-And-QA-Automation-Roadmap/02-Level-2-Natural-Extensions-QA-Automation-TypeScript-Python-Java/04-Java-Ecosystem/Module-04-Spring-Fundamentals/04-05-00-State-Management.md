[⬅️ 04-04. Spring MVC with Thymeleaf & Validation](04-04-00-Spring-MVC-Thymeleaf.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-05-01. Syntax From Scratch](04-05-01-Syntax-From-Scratch.md)

---

# 04-05-00. State Management (Cookies & HttpSession — Теория, Сигурност & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 05:** Как се запазва състоянието върху **Stateless HTTP** протокола чрез **Cookies (Бисквитки)** и сървърни сесии (**`HttpSession` / `JSESSIONID`**), флагове за сигурност (`HttpOnly`, `Secure`, `SameSite`), мащабиране с **Spring Session + Redis** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на `HttpSession` и `JSESSIONID` Cookie (Блок-схема)](#2-архитектура-на-httpsession-и-jsessionid-cookie-блок-схема)
* [3. Сравнителна карта в 4 езика (Java Spring, C# ASP.NET Core, Python Django, JS Express)](#3-сравнителна-карта-в-4-езика-java-spring-c-aspnet-core-python-django-js-express)
* [4. Плюсове и минуси на Cookies срещу HttpSession](#4-плюсове-и-минуси-на-cookies-срещу-httpsession)
* [5. Кодов одит: Правилен срещу Грешен код (XSS уязвимост без `HttpOnly` и чувствителни данни в Cookie)](#5-кодов-одит-правилен-срещу-грешен-код-xss-уязвимост-без-httponly-и-чувствителни-данни-в-cookie)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot / Servlet) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot--servlet-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`Cookie` = Хартиен билет в джоба на клиента:** Сървърът го издава и браузърът го пази на устройството на потребителя. При всяка следваща заявка към същия домейн браузърът автоматично показва билета чрез заглавието **`Cookie: ...`**. Максималният размер е ~**4 KB**, а потребителят може да го отвори и промени във всяка секунда в DevTools!
*   **`HttpSession` = Гардероб в театъра с номератор:** Всички палта и чанти (потребителското ID, ролята, количката за пазаруване) се пазят **на сигурно място в паметта на сървъра**. Клиентът получава в джоба си само едно малко номерче — бисквитката **`JSESSIONID=8F9A2B...`**.
*   **Трите златни щита на всяка сесийна бисквитка:**
    1.  **`HttpOnly = true`:** Забранява на JavaScript (`document.cookie`) да чете бисквитката — спасява те от **XSS (Cross-Site Scripting)** кражба на сесия!
    2.  **`Secure = true`:** Браузърът изпраща бисквитката **само по криптиран HTTPS канал** (никога по чист HTTP).
    3.  **`SameSite = Lax` или `Strict`:** Защитава срещу **CSRF (Cross-Site Request Forgery)** атаки от чужди сайтове.

---

## 2. АРХИТЕКТУРА НА `HTTPSESSION` И `JSESSIONID` COOKIE (БЛОК-СХЕМА)

```text
[ Браузър ]                                     [ Spring Boot Сървър (RAM / Redis) ]
     │ 1. POST /login (username + pass)                       │
     ├───────────────────────────────────────────────────────►│ Създава сесия #991A
     │ 2. Set-Cookie: JSESSIONID=991A; HttpOnly; Secure       │ Записва: { userId: 42, role: "ADMIN" }
     │◄───────────────────────────────────────────────────────┤
     │                                                        │
     │ 3. GET /dashboard                                      │
     │    Cookie: JSESSIONID=991A                             │
     ├───────────────────────────────────────────────────────►│ Намира сесия #991A -> Връща данните!
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA SPRING, C# ASP.NET CORE, PYTHON DJANGO, JS EXPRESS)

| Операция | ☕ Java 21+ (Spring / Servlet) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (Django) | 🟡 JS / TS (`express-session`) |
| :--- | :--- | :--- | :--- | :--- |
| **Запис на бисквитка** | `ResponseCookie.from("k", "v")...` | `Response.Cookies.Append("k", "v", opts)` | `response.set_cookie("k", "v", httponly=True)` | `res.cookie("k", "v", { httpOnly: true })` |
| **Четене на бисквитка** | `@CookieValue(value = "k", defaultValue = "")` | `Request.Cookies["k"]` | `request.COOKIES.get("k")` | `req.cookies.k` |
| **Запис в сървърна сесия** | `session.setAttribute("userId", 42L)` | `HttpContext.Session.SetInt32("userId", 42)` | `request.session["user_id"] = 42` | `req.session.userId = 42` |
| **Четене от сесия** | `(Long) session.getAttribute("userId")` | `HttpContext.Session.GetInt32("userId")` | `request.session.get("user_id")` | `req.session.userId` |
| **Изтриване на сесия (Logout)** | `session.invalidate()` | `HttpContext.Session.Clear()` | `request.session.flush()` | `req.session.destroy()` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА COOKIES СРЕЩУ HTTPSESSION

### 🟢 `HttpSession` (Сървърно състояние):
*   **Предимства:** Клиентът не вижда и не може да подправи данните в сесията; при Logout с `session.invalidate()` сесията се унищожава мигновено на сървъра.
*   **Недостатъци:** Заема RAM памет на сървъра и затруднява хоризонталното мащабиране (ако имаш 3 сървъра зад Load Balancer, потребителят може да попадне на сървър №2, който няма неговата сесия — решава се с **Spring Session + Redis**).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (XSS УЯЗВИМОСТ БЕЗ `HTTPONLY` И ЧУВСТВИТЕЛНИ ДАННИ В COOKIE)

### ❌ ГРЕШЕН КОД (Записване на роля/ID в обикновена бисквитка без `HttpOnly` и `Secure`):
```java
@GetMapping("/bad-login")
public String badLogin(HttpServletResponse response) {
    // 🚨 ГРЕШКА 1: Никога не пази права или потребителско ID в некриптирана бисквитка!
    // Всеки потребител може да отвори F12 -> Application -> Cookies и да смени "USER" на "ADMIN"!
    Cookie roleCookie = new Cookie("userRole", "USER");

    // 🚨 ГРЕШКА 2: Липсват setHttpOnly(true) и setSecure(true)!
    // Всеки злонамерен JS скрипт на страницата може да открадне бисквитката с document.cookie!
    response.addCookie(roleCookie);
    return "redirect:/";
}
```
*   **Как да се избегне:** Пази чувствителните данни в **`HttpSession`** (или криптиран/подписан JWT), а за бисквитки използвай модерния Spring билдер **`ResponseCookie`**, който поддържа нативно `.httpOnly(true)`, `.secure(true)` и `.sameSite("Lax")`.

### 🟢 ПРАВИЛЕН КОД (Модерен `ResponseCookie` и безопасно управление на `HttpSession`):
```java
import jakarta.servlet.http.HttpSession;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseCookie;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.time.Duration;

@RestController
@RequestMapping("/api/preferences")
public class SafeStateController {

    @PostMapping("/theme")
    public ResponseEntity<Void> setThemeCookie(@RequestParam String theme) {
        ResponseCookie cookie = ResponseCookie.from("ui_theme", theme)
                .httpOnly(true)
                .secure(true)
                .sameSite("Lax")
                .path("/")
                .maxAge(Duration.ofDays(30))
                .build();

        return ResponseEntity.ok()
                .header(HttpHeaders.SET_COOKIE, cookie.toString())
                .build();
    }

    @PostMapping("/logout")
    public ResponseEntity<Void> logout(HttpSession session) {
        session.invalidate(); // Унищожава сесията на сървъра напълно!
        return ResponseEntity.noContent().build();
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво представлява атаката **Session Fixation** и как Spring Security я предотвратява автоматично при вход (Login)?“*
*   **Уловката:** Повечето разработчици знаят за XSS и CSRF, но пропускат Session Fixation!
*   **Точен Senior отговор:** При **Session Fixation** атакуващият предварително отваря сайта, взима валиден `JSESSIONID=ABC123` и подмамва жертвата да влезе в профила си с точно този сесиен идентификатор. Ако сървърът запази същото ID след логин, атакуващият вече има достъп до акаунта на жертвата! **Решението:** Веднага след успешна автентикация сървърът **задължително** трябва да унищожи старата анонимна сесия и да издаде **чисто ново `JSESSIONID`** (`session.changeSessionId()` — в Spring Security това е включено по подразбиране чрез стратегията `changeSessionId`).

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT / SERVLET) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot / Jakarta Servlet) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Име на сесийната бисквитка по подразбиране** | `JSESSIONID` | `.AspNetCore.Session` |
| **Активиране на сесии** | Работи директно от кутията в Tomcat | Изисква `builder.Services.AddSession()` + `app.UseSession()` |
| **Какво може да се записва в сесията** | Всеки Java обект (`Object` — препоръчително `Serializable` за Redis) | Само `byte[]`, `int` и `string` (изисква JSON сериализация за обекти) |
| **Разпределена сесия в клъстер (Redis)** | **Spring Session Data Redis** (`@EnableRedisHttpSession`) | `builder.Services.AddStackExchangeRedisCache(...)` |
| **Защита на бисквитка срещу подправяне** | Ръчно криптиране или JWT | Вграден **ASP.NET Core Data Protection API** (автоматично криптиране) |

---
[⬅️ 04-04. Spring MVC with Thymeleaf & Validation](04-04-00-Spring-MVC-Thymeleaf.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-05-01. Syntax From Scratch](04-05-01-Syntax-From-Scratch.md)
