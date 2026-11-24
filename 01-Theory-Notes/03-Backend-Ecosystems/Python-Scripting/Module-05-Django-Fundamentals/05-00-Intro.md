[⬅️ Назад към Module 05: Django Fundamentals](README.md) | [📂 Текуща папка: Python Scripting](../../README.md) | [🏠 Начало](../../../../README.md) | [➡️ 05-01-00. Django Architecture & HTTP Lifecycle](05-01-00-Architecture-And-HTTP.md)

---

# 05-00. Intro to Django Fundamentals & MTV Architecture

**Модул 05:** Django Fundamentals | **Цел:** Въведение в философията на **Django** (*„The web framework for perfectionists with deadlines“*), архитектурния шаблон **MTV (Model – Template – View)** спрямо класическия **MVC (ASP.NET Core / Spring Boot)**, анатомията на един Django проект (`manage.py`, `settings.py`, `urls.py`, `wsgi.py`/`asgi.py`) и разделението между **Project** и **Apps**.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: MTV срещу MVC)](#1-как-да-се-запомни-по-лесно-мнемоника-mtv-срещу-mvc)
* [2. Архитектура на Django проект и Блок-схема на заявката](#2-архитектура-на-django-проект-и-блок-схема-на-заявката)
* [3. Сравнителна матрица: Django (MTV) срещу ASP.NET Core (MVC)](#3-сравнителна-матрица-django-mtv-срещу-aspnet-core-mvc)
* [4. Пътна карта на Модул 05](#4-пътна-карта-на-модул-05)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: MTV СРЕЩУ MVC)

Ако идваш от **C# (ASP.NET Core MVC)** или **Java (Spring MVC)**, терминологията в Django леко се разменя, но логиката е 100% същата:

* **Model (M)** = **Model** в C# (класовете за базата данни в `models.py`).
* **Template (T)** = **View** в C# (HTML шаблоните с Django Template Language, аналог на Razor `.cshtml`).
* **View (V)** = **Controller** в C# (Python функциите или класовете в `views.py`, които приемат HTTP заявката, викат базата данни и връщат HTML/JSON отговор!).
* **Django самият** играе ролята на рутера и диспечера (`urls.py`).

---

## 2. АРХИТЕКТУРА НА DJANGO ПРОЕКТ И БЛОК-СХЕМА НА ЗАЯВКАТА

```text
  HTTP Request ──► [urls.py (URL Dispatcher)] ──► [views.py (Бизнес логика)]
                                                         │         │
                                           (Чете/Пише)   ▼         ▼   (Рендерира)
                                                  [models.py]   [templates/*.html]
                                                 (Django ORM)          │
  HTTP Response ◄──────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА МАТРИЦА: DJANGO (MTV) СРЕЩУ ASP.NET CORE (MVC)

| Компонент / Задача | 🐍 Django 5.x (Python) | 🟣 ASP.NET Core 8+ (C#) |
| :--- | :--- | :--- |
| **Конфигурация на проекта** | `settings.py` (Python модул) | `appsettings.json` + `Program.cs` |
| **Маршрутизация (Routing)** | `urls.py` (`path('books/', views.list)`) | `[Route("books")]` / `app.MapControllerRoute(...)` |
| **Обработка на заявка** | `views.py` (FBV или CBV) | `BooksController.cs` (Action методи) |
| **HTML Шаблони** | `templates/books.html` (`{{ book.title }}`) | `Views/Books/Index.cshtml` (`@Model.Title`) |
| **Административен панел** | **Вграден** (`django.contrib.admin`) | Изисква външен пакет или ръчно писане |
| **Конзолен инструмент** | `python manage.py runserver / migrate` | `dotnet run` / `dotnet ef database update` |

---

## 4. ПЪТНА КАРТА НА МОДУЛ 05

1. **05-01 (`-00` / `-01`):** Django Architecture, Request/Response цикъл и настройка на проект.
2. **05-02 (`-00` / `-01`):** URL Routing и Function-Based Views (FBV).
3. **05-03 (`-00` / `-01`):** Django Templates (DTL), наследяване на шаблони, Static и Media файлове.
4. **05-04 (`-00` / `-01`):** Django Forms, ModelForms и CSRF защита.
5. **05-05 (`-00` / `-01`):** Class-Based Views (CBV — `ListView`, `DetailView`, `CreateView`, `UpdateView`, `DeleteView`).
6. **05-06 (`-00` / `-01`):** Персонализиране на вградения Django Admin панел.

---

[⬅️ Назад към Module 05: Django Fundamentals](README.md) | [📂 Текуща папка: Python Scripting](../../README.md) | [🏠 Начало](../../../../README.md) | [➡️ 05-01-00. Django Architecture & HTTP Lifecycle](05-01-00-Architecture-And-HTTP.md)

© 2026 Dev-Library / 05-00. Intro to Django Fundamentals & MTV Architecture
