[⬅️ 06-02. User Model & Signals](06-02-00-User-Model-And-Signals.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-03-01. Syntax From Scratch](06-03-01-Syntax-From-Scratch.md)

---

# 06-03-00. Django Middlewares and Sessions (Теория, Onion Архитектура & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 03:** Конвейерът за обработка на заявки (Middleware Onion Architecture), ред на изпълнение, разлика между бисквитки (Cookies) и сесии (Sessions), и видове съхранение на сесии (Database, Redis, Signed Cookies).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Луковицата (Onion Architecture) и Блок-схема](#2-архитектура-на-луковицата-onion-architecture-и-блок-схема)
* [3. Архитектурно сравнение: Django Middleware & Sessions срещу C# и Node.js](#3-архитектурно-сравнение-django-middleware--sessions-срещу-c-и-nodejs)
* [4. Плюсове и минуси на различните Session Backends](#4-плюсове-и-минуси-на-различните-session-backends)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Middleware = Слоевете на луковица (Onion Model):** Всяка HTTP заявка преминава през списъка `MIDDLEWARE` в `settings.py` **отгоре надолу** по пътя към твоето View. След като View-то сглоби отговора, той минава обратно през същите слоеве **отдолу нагоре** към браузъра.
*   **Cookie срещу Session (Гардеробен номер срещу Самото яке):**
    *   **Cookie (Бисквитка):** Пази се в **браузъра на клиента**. Съдържа само едно малко картонче с номерче (`sessionid = "abc987xyz"`).
    *   **Session (Сесия):** Пази се на **сървъра** (в PostgreSQL или Redis). Под номерчето `"abc987xyz"` сървърът пази тежкото яке — кой потребител е логнат, какво има в пазарската му количка и какви права има.

---

## 2. АРХИТЕКТУРА НА ЛУКОВИЦАТА (ONION ARCHITECTURE) И БЛОК-СХЕМА

```text
HTTP Request ──┐                                              ┌──► HTTP Response
               ▼                                              │
┌────────────────────────────────────────────────────────────────────────────┐
│ 1. SecurityMiddleware (HTTPS пренасочване, HSTS хедъри)                    │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ 2. SessionMiddleware (Чете 'sessionid' cookie и зарежда req.session) │  │
│  │  ┌────────────────────────────────────────────────────────────────┐  │  │
│  │  │ 3. AuthenticationMiddleware (Закача req.user към заявката)     │  │  │
│  │  │  ┌──────────────────────────────────────────────────────────┐  │  │  │
│  │  │  │               ТВОЯТ ИЗГЛЕД (views.py)                    │  │  │  │
│  │  │  └──────────────────────────────────────────────────────────┘  │  │  │
│  │  └────────────────────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO MIDDLEWARE & SESSIONS СРЕЩУ C# И NODE.JS

| Концепция | Django 5.x (Python) | ASP.NET Core (C#) | Express.js / NestJS |
| :--- | :--- | :--- | :--- |
| **Регистрация на Middleware** | Списък `MIDDLEWARE = [...]` в `settings.py` | `app.UseMiddleware<T>()` в `Program.cs` | `app.use(fn)` / `consumer.apply()` |
| **Ред на изпълнение** | **Отгоре надолу** (Request) ➔ **Отдолу нагоре** (Response) | Точно по реда на `app.Use...()` | Точно по реда на `app.use()` |
| **Достъп до сесията** | `request.session["cart"] = ...` | `HttpContext.Session.SetString(...)` | `req.session.cart = ...` |
| **Четене на бисквитка** | `request.COOKIES.get("theme")` | `Request.Cookies["theme"]` | `req.cookies.theme` |
| **Запис на бисквитка** | `response.set_cookie("theme", "dark")` | `Response.Cookies.Append("theme", "dark")` | `res.cookie("theme", "dark")` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА РАЗЛИЧНИТЕ SESSION BACKENDS

В Django можеш да смениш къде се пазят сесиите само с 1 ред (`SESSION_ENGINE`) в `settings.py`:

### 🟢 1. Database Sessions (По подразбиране — таблица `django_session`):
*   **Предимства:** Работи веднага без допълнителни сървъри; лесно изтриване на сесиите на конкретен потребител от админ панела.
*   **Недостатъци:** При всеки клик на потребителя Django прави SQL заявка към базата данни за четене/запис на сесията, което забавя сайта при висок трафик.

### 🔵 2. Redis Cache Sessions (`django.contrib.sessions.backends.cache` — Препоръчителен за продукция!):
*   **Предимства:** Светкавично четене и писане в RAM паметта (под 1 милисекунда); автоматично изтриване на изтекли сесии (TTL).
*   **Недостатъци:** Изисква работещ Redis сървър.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Грешен ред в `MIDDLEWARE` и промяна на вложен списък в сесията без `modified = True`):
```python
# 🚨 ГРЕШКА 1: AuthenticationMiddleware стои ПРЕДИ SessionMiddleware в settings.py!
# AuthenticationMiddleware чете request.session -> ако тя не е заредена още, сървърът гърми с AttributeError!
MIDDLEWARE = [
    "django.contrib.auth.middleware.AuthenticationMiddleware",
    "django.contrib.sessions.middleware.SessionMiddleware",
]

# 🚨 ГРЕШКА 2: Промяна на вложен списък/речник вътре в request.session!
def add_to_cart_buggy(request):
    # Ако request.session["cart"] е списък, .append() променя списъка в паметта,
    # но Django НЕ засича, че сесията е променена и НЕ я записва в базата!
    request.session["cart"].append("Laptop")
```
*   **Как да се избегне:** В `MIDDLEWARE` винаги дръж `SessionMiddleware` **преди** `AuthenticationMiddleware`. Когато променяш вложен списък или речник в `request.session`, **задължително** добави **`request.session.modified = True`**.

### 🟢 ПРАВИЛЕН КОД (Персонализиран Middleware за измерване на време и безопасна сесия):
```python
import time

# 1. Във файла core/middleware.py (Модерен class-based Middleware в Django)
class RequestTimingMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response  # Изпълнява се веднъж при старт на сървъра

    def __call__(self, request):
        # Код ПРЕДИ изгледа (Request фаза)
        start_time = time.perf_counter()

        response = self.get_response(request)  # Предава заявката навътре по веригата

        # Код СЛЕД изгледа (Response фаза)
        duration_ms = (time.perf_counter() - start_time) * 1000
        response["X-Execution-Time-Ms"] = f"{duration_ms:.2f}"
        return response

# 2. Безопасна работа със сесия във views.py
def add_to_cart_safe(request):
    cart = request.session.get("cart", [])
    cart.append("Laptop")
    request.session["cart"] = cart
    request.session.modified = True  # Гарантира запис в базата / Redis!
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между флаговете `HttpOnly`, `Secure` и `SameSite` върху Session Cookie-то и от какви хакерски атаки предпазва всеки от тях?“*
*   **Уловката:** Кандидатите често знаят, че тези настройки съществуват в `settings.py`, но не могат да обяснят механизма на защита.
*   **Точен Senior отговор:** 
    1.  **`SESSION_COOKIE_HTTPONLY = True`:** Забранява на клиентския JavaScript (`document.cookie`) да чете бисквитката на сесията. Предпазва от **XSS (Cross-Site Scripting)** кражба на акаунти.
    2.  **`SESSION_COOKIE_SECURE = True`:** Браузърът изпраща бисквитката **само по криптирана HTTPS връзка**, никога по обикновен HTTP. Предпазва от подслушване на трафика (Man-in-the-Middle).
    3.  **`SESSION_COOKIE_SAMESITE = "Lax"` (или `"Strict"`):** Забранява изпращането на бисквитката при заявки, идващи от чужди злонамерени сайтове. Предпазва от **CSRF (Cross-Site Request Forgery)** атаки.

---
[⬅️ 06-02. User Model & Signals](06-02-00-User-Model-And-Signals.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-03-01. Syntax From Scratch](06-03-01-Syntax-From-Scratch.md)
