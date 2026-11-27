[⬅️ 05-01. Internet, HTTP & Architecture](05-01-00-Architecture-And-HTTP.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-02-01. Syntax From Scratch](05-02-01-Syntax-From-Scratch.md)

---

# 05-02-00. URLs, Routing and Function-Based Views (Теория, Request/Response & Капани)

**Модул 05:** Django Fundamentals | **Лекционна тема 02:** Система за маршрутизиране (`urls.py`), динамични URL параметри и конвертори (`<int:pk>`, `<slug:slug>`), именувани маршрути, `HttpRequest` срещу `HttpResponse` и Function-Based Views (FBV).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на маршрутизиране (URL Dispatcher) и Блок-схема](#2-етапи-на-маршрутизиране-url-dispatcher-и-блок-схема)
* [3. Архитектурно сравнение: Django срещу C# ASP.NET Core и Express.js](#3-архитектурно-сравнение-django-срещу-c-aspnet-core-и-expressjs)
* [4. Плюсове и минуси на Function-Based Views (FBV)](#4-плюсове-и-минуси-на-function-based-views-fbv)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Пощенска сортировъчна станция (`urls.py`):** Представи си главния `config/urls.py` като централната поща за страната, която гледа само първата част от адреса (напр. `books/`), отрязва я и препраща писмото към локалния клон `books/urls.py`, който гледа остатъка (напр. `details/5/`).
*   **Договорът на всяко View (`request` влиза ➔ `HttpResponse` излиза):** Всяка функция в `views.py` е като сервитьор: **задължително** приема поръчката (`request: HttpRequest` като първи параметър) и **задължително** връща поднос на клиента (`HttpResponse`, `render(...)`, `redirect(...)` или `JsonResponse(...)`). Ако функцията върне `None`, Django гърми!

---

## 2. ЕТАПИ НА МАРШРУТИЗИРАНЕ (URL DISPATCHER) И БЛОК-СХЕМА

Django проверява списъка `urlpatterns` **строго отгоре надолу** и спира на **първото съвпадение**.

```text
Заявка: GET /books/42/

┌──────────────────────────┐     1. Премахва 'books/'   ┌─────────────────────────┐
│ config/urls.py           │ ─────────────────────────► │ books/urls.py           │
│ path("books/", include())│                            │ path("<int:pk>/", view) │
└──────────────────────────┘                            └────────────┬────────────┘
                                                                     │ 2. Парсва pk=42
                                                                     ▼
┌──────────────────────────┐     3. Извиква функцията   ┌─────────────────────────┐
│ Връща HttpResponse       │ ◄───────────────────────── │ book_details(request,   │
│ (200 OK + HTML/JSON)     │                            │              pk=42)     │
└──────────────────────────┘                            └─────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO СРЕЩУ C# ASP.NET CORE И EXPRESS.JS

| Концепция | Django 5.x (Python) | ASP.NET Core MVC (C#) | Express.js (Node.js) |
| :--- | :--- | :--- | :--- |
| **Дефиниране на маршрут** | `path("books/<int:id>/", book_detail)` | `[HttpGet("books/{id:int}")]` | `router.get("/books/:id", handler)` |
| **Разделяне по модули** | `include("books.urls")` | `[Route("api/[controller]")]` | `app.use("/books", bookRouter)` |
| **Query параметри (`?q=hi`)** | `request.GET.get("q")` | `[FromQuery] string q` | `req.query.q` |
| **Връщане на HTML шаблон** | `return render(request, "a.html", ctx)` | `return View(model);` | `res.render("a", ctx)` |
| **Връщане на JSON** | `return JsonResponse({"ok": True})` | `return Ok(new { ok = true });` | `res.json({ ok: true })` |
| **Пренасочване (Redirect)** | `return redirect("book-list")` | `return RedirectToAction("Index");` | `res.redirect("/books")` |
| **404 ако липсва запис** | `get_object_or_404(Book, pk=pk)` | `if (book == null) return NotFound();` | `if (!book) return res.sendStatus(404)` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА FUNCTION-BASED VIEWS (FBV)

### 🟢 Предимства (Pros):
*   **Кристална четимост и линейност:** Кодът се чете отгоре надолу като обикновена Python функция без скрита магия в базови класове.
*   **Лесни декоратори:** Поставянето на `@login_required` или `@require_POST` над функцията е напълно естествено и ясно.

### 🔴 Недостатъци (Cons):
*   **Повторение на код (Boilerplate):** При 10 различни модела ще пишеш едни и същи `if request.method == "POST":` и `get_object_or_404()` проверки отново и отново (тук помагат Class-Based Views).
*   **Няма лесно наследяване:** Не можеш да наследиш едно FBV и да промениш само една малка част от него, без да дублираш функцията.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Грешен ред на маршрутите, твърдо кодирани URL адреси и `DoesNotExist` срив):
```python
# --- Във файла books/urls.py ---
urlpatterns = [
    # 🚨 ГРЕШКА 1: Текстовият маршрут <str:slug> стои ПРЕДИ конкретния маршрут 'create/'!
    # Когато потребителят отвори /books/create/, Django ще помисли, че 'create' е име на книга!
    path("<slug:slug>/", views.book_detail),
    path("create/", views.book_create),  # Никога няма да бъде достигнато!
]

# --- Във файла books/views.py ---
def book_detail(request, pk):
    # 🚨 ГРЕШКА 2: Използване на .get() без try/except!
    # Ако някой напише /books/99999/, сървърът гърми с 500 Internal Server Error вместо 404 Not Found!
    book = Book.objects.get(pk=pk)
    return render(request, "books/detail.html", {"book": book})
```
*   **Как да се избегне:** Винаги слагай **конкретните маршрути (`create/`, `archive/`) най-отгоре**, а динамичните (`<slug:slug>/`) най-отдолу. За търсене на обект в View **винаги** използвай `get_object_or_404()`.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт за URLs и FBV):
```python
# --- Във файла books/urls.py ---
from django.urls import path
from . import views

app_name = "books"  # Namespace за избягване на сблъсъци между приложения!

urlpatterns = [
    path("", views.book_list, name="list"),
    path("create/", views.book_create, name="create"),          # Конкретният път е ПЪРВИ
    path("<int:pk>/", views.book_detail, name="detail"),        # Динамичният път е СЛЕД него
]

# --- Във файла books/views.py ---
from django.shortcuts import render, get_object_or_404, redirect
from django.views.decorators.http import require_http_methods
from .models import Book

@require_http_methods(["GET"])
def book_detail(request, pk: int):
    # Връща чист 404 Not Found статус, ако книгата липсва в базата
    book = get_object_or_404(Book, pk=pk)
    context = {"book": book}
    return render(request, "books/detail.html", context)
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Как работи настройката `APPEND_SLASH = True` в Django и защо изпращането на `POST` заявка към URL без наклонена черта накрая (напр. `/api/orders` вместо `/api/orders/`) води до грешка или загуба на данни?“*
*   **Уловката:** Кандидатите често пишат маршрути без `/` накрая и се чудят защо при `POST` заявка от Postman или JavaScript данните им изчезват!
*   **Точен Senior отговор:** По подразбиране в Django `APPEND_SLASH = True`. Когато клиент изпрати `GET /books` (без `/` накрая), Django не намира съвпадение, но вижда, че съществува `/books/` и връща **`301 Moved Permanently`** към `/books/`. Браузърът автоматично прави втора `GET` заявка и всичко работи. Но при **`POST` заявка**, HTTP стандартът забранява автоматичното препращане на `POST` тялото при `301` пренасочване (браузърът го превръща в празна `GET` заявка без данните!), затова Django хвърля **`RuntimeError: You called this URL via POST, but the URL doesn't end in a slash`**. **Правило:** В Django всеки URL път в `urls.py` **винаги** завършва с `/`!

---
[⬅️ 05-01. Internet, HTTP & Architecture](05-01-00-Architecture-And-HTTP.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-02-01. Syntax From Scratch](05-02-01-Syntax-From-Scratch.md)
