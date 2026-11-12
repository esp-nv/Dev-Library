[⬅️ 05-02-00. Теория & Капани](05-02-00-URLs-And-FBV.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-03-00. Templates, Static & Media](05-03-00-Templates-Static-Media.md)

---

# 05-02-01. URLs and FBV: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за свързване на `urls.py`, динамични конвертори, работа с `HttpRequest` и връщане на отговори в Django от нулата.

## 🧭 Съдържание на темата
* [1. Свързване на главен `urls.py` с `urls.py` на приложението](#1-свързване-на-главен-urlspy-с-urlspy-на-приложението)
* [2. Видове URL конвертори (Path Converters)](#2-видове-url-конвертори-path-converters)
* [3. Четене на данни от `request` (`GET`, `POST`, `user`, `headers`)](#3-четене-на-данни-от-request-get-post-user-headers)
* [4. Връщане на различни отговори (`render`, `redirect`, `JsonResponse`, `HttpResponse`)](#4-връщане-на-различни-отговори-render-redirect-jsonresponse-httpresponse)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СВЪРЗВАНЕ НА ГЛАВЕН `URLS.PY` С `URLS.PY` НА ПРИЛОЖЕНИЕТО

### А. Главният файл `config/urls.py`
```python
from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    path("admin/", admin.site.urls),
    # Препраща всички адреси, започващи с 'books/', към файла books/urls.py
    path("books/", include("books.urls")),
]
```

### Б. Локалният файл `books/urls.py` (създава се ръчно в папката на приложението)
```python
from django.urls import path
from . import views

app_name = "books"  # Позволява обръщение като 'books:detail'

urlpatterns = [
    path("", views.index, name="index"),                        # Адрес: /books/
    path("<int:pk>/", views.details, name="detail"),            # Адрес: /books/5/
    path("category/<slug:cat_slug>/", views.by_category, name="by-category"),
]
```

---

## 2. ВИДОВЕ URL КОНВЕРТОРИ (PATH CONVERTERS)

Когато сложиш `<тип:име_на_променлива>` в `path()`, Django автоматично проверява формата и подава стойността като аргумент във функцията в `views.py`:

*   **`<int:pk>`** ➔ Приема само цели положителни числа (`0, 1, 42...`) и ги превръща в Python `int`.
*   **`<str:name>`** ➔ Приема всеки непразен текст без наклонена черта `/` (по подразбиране, ако не пишеш тип: `<name>`).
*   **`<slug:slug>`** ➔ Приема букви, цифри, тирета `-` и долни черти `_` (напр. `django-basics-2026`).
*   **`<uuid:id>`** ➔ Приема валиден UUID идентификатор (напр. `075194d3-6885-417e-a8a8-6c931e272f00`).
*   **`<path:subpath>`** ➔ Приема всякакъв текст, **включително** наклонени черти `/`.

---

## 3. ЧЕТЕНЕ НА ДАННИ ОТ `REQUEST` (`GET`, `POST`, `USER`, `HEADERS`)

```python
from django.http import HttpRequest, HttpResponse

def search_view(request: HttpRequest) -> HttpResponse:
    # 1. Проверка на HTTP метода ("GET", "POST", "PUT", "DELETE")
    if request.method == "GET":
        # Четене на Query параметри от URL: /books/search/?q=python&page=2
        query = request.GET.get("q", "")        # Връща "" ако липсва 'q'
        page = request.GET.get("page", 1)

    # 2. Четене на данни от изпратена HTML форма (POST)
    if request.method == "POST":
        title = request.POST.get("title")

    # 3. Проверка кой е текущият потребител и дали е логнат
    if request.user.is_authenticated:
        username = request.user.username

    return HttpResponse(f"Търсене за: {query}")
```

---

## 4. ВРЪЩАНЕ НА РАЗЛИЧНИ ОТГОВОРИ (`RENDER`, `REDIRECT`, `JSONRESPONSE`, `HTTPRESPONSE`)

```python
from django.shortcuts import render, redirect, get_object_or_404
from django.http import HttpResponse, JsonResponse
from .models import Book

# 1. Връщане на HTML страница с данни (Context речник)
def index(request):
    books = Book.objects.all()
    return render(request, "books/index.html", {"books": books})

# 2. Връщане на чист текст или HTML код директно
def ping(request):
    return HttpResponse("<h1>Pong!</h1>", status=200)

# 3. Връщане на JSON отговор (за AJAX / Fetch заявки)
def api_status(request):
    return JsonResponse({"status": "active", "count": 42})

# 4. Пренасочване към друг маршрут по неговото 'name'
def go_home(request):
    return redirect("books:detail", pk=1)  # Автоматично изгражда /books/1/
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис | Какво прави |
| :--- | :--- | :--- |
| **Включване на под-маршрути** | `path("shop/", include("shop.urls"))` | Свързва главния `urls.py` с приложението |
| **Именуван маршрут** | `path("add/", views.add, name="add")` | Позволява препращане по име без хардкодване |
| **Динамично числово ID** | `path("<int:pk>/", views.detail)` | Подава `pk: int` във функцията |
| **Четене на `?key=val`** | `request.GET.get("key", default)` | Взима параметър от URL адреса |
| **Четене на POST форма** | `request.POST.get("field_name")` | Взима данни от изпратена форма |
| **Търсене или 404 грешка** | `obj = get_object_or_404(Model, pk=pk)` | Връща обекта или статус `404 Not Found` |
| **Рендиране на шаблон** | `return render(request, "t.html", ctx)` | Сглобява HTML страницата с данните |
| **Пренасочване (Redirect)** | `return redirect("app:route_name")` | Изпраща браузъра към друг адрес |

---
[⬅️ 05-02-00. Теория & Капани](05-02-00-URLs-And-FBV.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-03-00. Templates, Static & Media](05-03-00-Templates-Static-Media.md)
