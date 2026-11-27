[⬅️ 05-03-00. Теория & Капани](05-03-00-Templates-Static-Media.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-04-00. Forms and ModelForms](05-04-00-Forms-And-ModelForms.md)

---

# 05-03-01. Django Templates, Static & Media: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за DTL тагове, филтри, наследяване на страници, конфигурация на `STATIC` и `MEDIA` файлове в Django от нулата.

## 🧭 Съдържание на темата
* [1. Наследяване на шаблони (`base.html` и `child.html`)](#1-наследяване-на-шаблони-basehtml-и-childhtml)
* [2. Основни DTL тагове и вградени филтри](#2-основни-dtl-тагове-и-вградени-филтри)
* [3. Настройка на Статични файлове (`STATIC_URL` и `STATICFILES_DIRS`)](#3-настройка-на-статични-файлове-static_url-и-staticfiles_dirs)
* [4. Настройка на Медийни файлове (`MEDIA_URL`, `MEDIA_ROOT` и `ImageField`)](#4-настройка-на-медийни-файлове-media_url-media_root-и-imagefield)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. НАСЛЕДЯВАНЕ НА ШАБЛОНИ (`BASE.HTML` И `CHILD.HTML`)

### А. Главният шаблон `templates/base.html`
```html
{% load static %}
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <title>{% block title %}Моят Сайт{% endblock %}</title>
    <link rel="stylesheet" href="{% static 'css/main.css' %}">
</head>
<body>
    <nav>
        <a href="{% url 'books:index' %}">Начало</a>
    </nav>

    <main>
        <!-- Тук дъщерните страници ще вмъкват своето съдържание -->
        {% block content %}{% endblock %}
    </main>

    <footer>© 2026 SoftUni Project</footer>
</body>
</html>
```

### Б. Дъщерният шаблон `templates/books/list.html`
*Задължително правило: `{% extends 'base.html' %}` винаги трябва да е на **първия ред** във файла!*
```html
{% extends "base.html" %}

{% block title %}Всички книги{% endblock %}

{% block content %}
    <h2>Списък с книги</h2>
    {% include "partials/search_bar.html" %} <!-- Вмъква малък преизползваем фрагмент -->
{% endblock %}
```

---

## 2. ОСНОВНИ DTL ТАГОВЕ И ВГРАДЕНИ ФИЛТРИ

```html
<!-- 1. Условни проверки -->
{% if user.is_authenticated %}
    <p>Добре дошла, {{ user.username|capfirst }}!</p>
{% elif user.is_staff %}
    <p>Служебен достъп</p>
{% else %}
    <a href="{% url 'login' %}">Вход</a>
{% endif %}

<!-- 2. Вградени филтри (използват символа | ) -->
<p>Цена: {{ book.price|floatformat:2 }} лв.</p>
<p>Дата: {{ book.created_at|date:"d.m.Y H:i" }}</p>
<p>Кратко описание: {{ book.summary|truncatewords:20 }}</p>
<p>Ако е празно: {{ book.subtitle|default:"Няма подзаглавие" }}</p>
```

---

## 3. НАСТРОЙКА НА СТАТИЧНИ ФАЙЛОВЕ (`STATIC_URL` И `STATICFILES_DIRS`)

Във файла **`config/settings.py`**:
```python
STATIC_URL = "static/"

# Къде Django да търси общите CSS / JS / Images папки по време на разработка:
STATICFILES_DIRS = [
    BASE_DIR / "static",
]
```

Използване в HTML файла:
```html
{% load static %}
<img src="{% static 'images/logo.png' %}" alt="Лого">
<script src="{% static 'js/app.js' %}"></script>
```

---

## 4. НАСТРОЙКА НА МЕДИЙНИ ФАЙЛОВЕ (`MEDIA_URL`, `MEDIA_ROOT` И `IMAGEFIELD`)

За работа с изображения се изисква библиотеката **Pillow** (`pip install pillow`).

### А. Във файла `config/settings.py`
```python
MEDIA_URL = "media/"
MEDIA_ROOT = BASE_DIR / "media"  # Папката, в която физически се записват качените файлове
```

### Б. Във файла `books/models.py`
```python
class Book(models.Model):
    title = models.CharField(max_length=100)
    cover = models.ImageField(upload_to="book_covers/", blank=True, null=True)
```

### В. В главния `config/urls.py` (Само по време на разработка с `DEBUG = True`!)
```python
from django.conf import settings
from django.conf.urls.static import static

urlpatterns = [
    # ... твоите маршрути ...
]

if settings.DEBUG:
    urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в DTL | Какво прави |
| :--- | :--- | :--- |
| **Извеждане на стойност** | `{{ variable }}` | Принтира променлива с XSS защита |
| **Наследяване на шаблон** | `{% extends "base.html" %}` | Наследява главния дизайн (на ред 1) |
| **Дефиниране на секция** | `{% block content %}...{% endblock %}` | Секция за презаписване в дъщерен шаблон |
| **Вмъкване на фрагмент** | `{% include "nav.html" %}` | Вгражда малък HTML компонент (Partial) |
| **Генериране на URL** | `{% url 'app:view_name' arg1 %}` | Създава динамичен линк по име на маршрут |
| **Зареждане на CSS/JS** | `{% load static %}` + `{% static 'a.css' %}` | Генерира правилния път до статичен файл |
| **Цикъл с празен случай** | `{% for x in list %}...{% empty %}...{% endfor %}` | Изпълнява `{% empty %}` ако списъкът е празен |
| **Форматиране с филтър** | `{{ val\|date:"d.m.Y" }}` / `{{ val\|length }}` | Преобразува стойността при показване |

---
[⬅️ 05-03-00. Теория & Капани](05-03-00-Templates-Static-Media.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-04-00. Forms and ModelForms](05-04-00-Forms-And-ModelForms.md)
