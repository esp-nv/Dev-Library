[⬅️ 05-00. Intro & Roadmap](05-00-Intro.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-01-01. Syntax From Scratch](05-01-01-Syntax-From-Scratch.md)

---

# 05-01-00. Internet, HTTP & Django Architecture (Теория, MVT, PostgreSQL & Капани)

**Модул 05:** Django Fundamentals | **Лекционна тема 01:** Как работи интернет и HTTP протоколът, структура на Django проект и приложения (Apps), MVT архитектура и свързване с PostgreSQL.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. HTTP Жизнен цикъл и MVT Поток (Блок-схема)](#2-http-жизнен-цикъл-и-mvt-поток-блок-схема)
* [3. Архитектурно сравнение: Django срещу C# ASP.NET Core и Node.js](#3-архитектурно-сравнение-django-срещу-c-aspnet-core-и-nodejs)
* [4. Плюсове и минуси на модулната архитектура (Apps)](#4-плюсове-и-минуси-на-модулната-архитектура-apps)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **HTTP Методите като действия в ежедневието:**
    *   **`GET`:** Четене на менюто в ресторант (само гледаш, нищо не променяш на сървъра).
    *   **`POST`:** Изпращане на нова поръчка в кухнята (създава нов запис в базата).
    *   **`PUT` / `PATCH`:** Смяна на цялата поръчка (`PUT`) или добавяне само на гарнитура (`PATCH`).
    *   **`DELETE`:** Отказване и изтриване на поръчката.
*   **HTTP Статус кодовете по стотици:**
    *   **`2xx` (Успех):** `200 OK`, `201 Created`.
    *   **`3xx` (Пренасочване):** `301 Moved Permanently`, `302 Found` (след логин или форма).
    *   **`4xx` (Грешка на клиента):** `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`.
    *   **`5xx` (Грешка на сървъра):** `500 Internal Server Error` (твоят Python код е гръмнал).

---

## 2. HTTP ЖИЗНЕН ЦИКЪЛ И MVT ПОТОК (БЛОК-СХЕМА)

Когато потребителят напише адрес в браузъра, заявката минава през строга верига в Django:

```text
┌──────────────────┐  1. HTTP Request (GET /books/) ┌───────────────────────┐
│ Браузър (Клиент) │ ─────────────────────────────► │ Middleware Слой       │
└────────▲─────────┘                                │ (Сесии, CSRF, Auth)   │
         │                                          └───────────┬───────────┘
         │ 6. HTTP Response (HTML + 200 OK)                     │ 2. Маршрутизиране
         │                                                      ▼
┌────────┴─────────┐  5. Рендира HTML               ┌───────────────────────┐
│ Template (.html) │ ◄───────────────────────────── │ urls.py ➔ views.py    │
└──────────────────┘  4. Подава Context {}          └───────────▲───────────┘
                                                                │ 3. Чете данни
                                                                ▼
                                                    ┌───────────────────────┐
                                                    │ models.py + PostgreSQL│
                                                    └───────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO СРЕЩУ C# ASP.NET CORE И NODE.JS

| Компонент | Django 5.x (Python) | ASP.NET Core MVC (C#) | Express / NestJS (Node.js) |
| :--- | :--- | :--- | :--- |
| **Главен конфигурационен файл** | `settings.py` | `Program.cs` + `appsettings.json` | `app.ts` + `.env` |
| **Маршрутизиране (Routing)** | `urls.py` (`path(...)`) | `[Route]` атрибути / `MapControllerRoute` | `router.get(...)` / `@Get()` |
| **Обработка на заявката** | `views.py` (Function или Class View) | `Controllers/HomeController.cs` | `controllers/home.controller.ts` |
| **Слой за база данни** | `models.py` (Django ORM) | `Models/` + `AppDbContext.cs` (EF Core) | `entities/` (TypeORM / Prisma) |
| **HTML Шаблони** | `templates/*.html` (DTL) | `Views/*.cshtml` (Razor) | `views/*.ejs` / Handlebars |
| **Входна точка на проекта** | `manage.py` (CLI инструмент) | `Program.cs` (`Main` метод) | `index.js` / `main.ts` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА МОДУЛНАТА АРХИТЕКТУРА (APPS)

### 🟢 Предимства (Pros):
*   **Пълна модулност (Pluggable Apps):** Всяка функционалност (`accounts`, `catalog`, `payments`) живее в собствена папка със свои модели, изгледи, форми и тестове.
*   **Преизползваемост между проекти:** Можеш да копираш папка `blog/` в нов Django проект, да я добавиш в `INSTALLED_APPS` и тя тръгва веднага.

### 🔴 Недостатъци (Cons):
*   **Риск от кръгови импорти (Circular Imports):** Ако приложение `users` импортира модел от `orders`, а `orders` импортира модел от `users`, Python ще гръмне с `ImportError`.
*   **Липса на вграден Service Layer:** Django не създава папка `services.py` по подразбиране — програмистите често трупат цялата бизнес логика в `views.py` или `models.py` ("Fat Models / Fat Views").

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Забравен `INSTALLED_APPS` и кръгов импорт между модели):
```python
# 🚨 ГРЕШКА 1: Създаваш приложение 'python manage.py startapp catalog', 
# но забравяш да го добавиш в settings.py -> INSTALLED_APPS!
# Резултат: 'makemigrations' изобщо не вижда моделите ти и казва "No changes detected"!

# 🚨 ГРЕШКА 2: Директен import в models.py, който води до Circular Import:
from orders.models import Order  # Ако orders/models.py също импортира User -> CRASH!

class UserProfile(models.Model):
    last_order = models.ForeignKey(Order, on_delete=models.SET_NULL, null=True)
```
*   **Как да се избегне:** Винаги регистрирай новото приложение в `INSTALLED_APPS`. При релации между различни приложения посочвай модела като **текстов низ `"orders.Order"`**, вместо да правиш директен Python `import`.

### 🟢 ПРАВИЛЕН КОД (Чиста настройка на `INSTALLED_APPS`, PostgreSQL и релация без кръгов импорт):
```python
# 1. Във файла config/settings.py:
INSTALLED_APPS = [
    "django.contrib.admin",
    "django.contrib.auth",
    "django.contrib.contenttypes",
    "django.contrib.sessions",
    "django.contrib.messages",
    "django.contrib.staticfiles",
    # Собствени приложения (Apps):
    "accounts",
    "catalog",
    "orders",
]

# Настройка на PostgreSQL база данни (вместо SQLite)
DATABASES = {
    "default": {
        "ENGINE": "django.db.backends.postgresql",
        "NAME": "softuni_shop_db",
        "USER": "postgres",
        "PASSWORD": "secret_password",
        "HOST": "localhost",
        "PORT": "5432",
    }
}

# 2. Във файла accounts/models.py (Използваме string референция за избягване на Circular Import):
from django.db import models

class UserProfile(models.Model):
    last_order = models.ForeignKey("orders.Order", on_delete=models.SET_NULL, null=True)
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е точната разлика между HTTP статус кодовете `401 Unauthorized` и `403 Forbidden`, и кога Django връща единия спрямо другия?“*
*   **Уловката:** Името `401 Unauthorized` подвежда хората, че става дума за липса на права (Authorization), докато всъщност то означава липса на самоличност (Authentication)!
*   **Точен Senior отговор:** 
    1.  **`401 Unauthorized` (Неавтентикиран / Кой си ти?):** Сървърът изобщо не знае кой си (не си логнат, липсва сесия или JWT токенът е изтекъл). Ако се логнеш, може и да имаш достъп.
    2.  **`403 Forbidden` (Забранен достъп / Знам кой си, но нямаш права!):** Ти си успешно логнат като обикновен потребител (`User`), но се опитваш да отвориш страница само за администратори (`/admin/delete-all/`). Повторното логване няма да помогне, защото нямаш нужната роля.

---
[⬅️ 05-00. Intro & Roadmap](05-00-Intro.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-01-01. Syntax From Scratch](05-01-01-Syntax-From-Scratch.md)
