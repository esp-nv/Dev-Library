[⬅️ 06-03-00. Теория & Капани](06-03-00-Middlewares-And-Sessions.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-04-00. Django REST Framework](06-04-00-Django-REST-Framework.md)

---

# 06-03-01. Middlewares & Sessions: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за писане на собствен Middleware (класов и функционален), регистрация в `settings.py` и управление на сесии и бисквитки в Django от нулата.

## 🧭 Съдържание на темата
* [1. Създаване на собствен Middleware (Класов и Функционален)](#1-създаване-на-собствен-middleware-класов-и-функционален)
* [2. Работа със Сесии (`request.session`)](#2-работа-със-сесии-requestsession)
* [3. Работа с Бисквитки (`request.COOKIES` и `response.set_cookie`)](#3-работа-с-бисквитки-requestcookies-и-responseset_cookie)
* [4. Конфигурация на Redis Sessions в `settings.py`](#4-конфигурация-на-redis-sessions-в-settingspy)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА СОБСТВЕН MIDDLEWARE (КЛАСОВ И ФУНКЦИОНАЛЕН)

Създай файл **`middleware.py`** в папката на приложението си:

```python
from django.http import HttpResponseForbidden

# Вариант 1: Класов Middleware (Препоръчителен стандарт)
class BlockBannedIpMiddleware:
    BANNED_IPS = {"192.168.1.99", "10.0.0.55"}

    def __init__(self, get_response):
        self.get_response = get_response

    def __call__(self, request):
        client_ip = request.META.get("REMOTE_ADDR")
        if client_ip in self.BANNED_IPS:
            # Спира заявката веднага, без изобщо да стига до views.py!
            return HttpResponseForbidden("Достъпът от вашия IP адрес е забранен.")

        response = self.get_response(request)
        return response

# Вариант 2: Функционален Middleware (по-кратък за прости задачи)
def simple_log_middleware(get_response):
    def middleware(request):
        print(f"Входяща заявка: {request.method} {request.path}")
        response = get_response(request)
        return response
    return middleware
```

### Регистрация в `config/settings.py`:
```python
MIDDLEWARE = [
    "django.middleware.security.SecurityMiddleware",
    "django.contrib.sessions.middleware.SessionMiddleware",
    "django.middleware.common.CommonMiddleware",
    "django.middleware.csrf.CsrfViewMiddleware",
    "django.contrib.auth.middleware.AuthenticationMiddleware",
    "django.contrib.messages.middleware.MessageMiddleware",
    # Добавяш своя Middleware най-отдолу:
    "core.middleware.BlockBannedIpMiddleware",
]
```

---

## 2. РАБОТА СЪС СЕСИИ (`REQUEST.SESSION`)

Обектът `request.session` се държи точно като обикновен Python речник (`dict`):

```python
def session_demo_view(request):
    # 1. Записване на стойност в сесията
    request.session["favorite_color"] = "blue"

    # 2. Четене на стойност (безопасно с .get())
    color = request.session.get("favorite_color", "default")

    # 3. Задаване на срок на валидност на сесията (в секунди: 1800 = 30 минути)
    request.session.set_expiry(1800)

    # 4. Изтриване на конкретен ключ от сесията
    if "favorite_color" in request.session:
        del request.session["favorite_color"]

    # 5. Пълно изтриване на цялата сесия (използва се при Logout)
    request.session.flush()
```

---

## 3. РАБОТА С БИСКВИТКИ (`REQUEST.COOKIES` И `RESPONSE.SET_COOKIE`)

```python
from django.http import HttpResponse

def cookie_demo_view(request):
    # 1. Четене на бисквитка, изпратена от браузъра
    current_theme = request.COOKIES.get("ui_theme", "light")

    # 2. Записване на нова бисквитка в отговора (max_age е в секунди: 7 дни)
    response = HttpResponse(f"Текуща тема: {current_theme}")
    response.set_cookie(
        key="ui_theme",
        value="dark",
        max_age=7 * 24 * 60 * 60,
        httponly=True,
        samesite="Lax"
    )
    return response
```

---

## 4. КОНФИГУРАЦИЯ НА REDIS SESSIONS В `SETTINGS.PY`

```python
CACHES = {
    "default": {
        "BACKEND": "django.core.cache.backends.redis.RedisCache",
        "LOCATION": "redis://127.0.0.1:6379/1",
    }
}

# Казва на Django да пази всички сесии в Redis RAM паметта вместо в PostgreSQL
SESSION_ENGINE = "django.contrib.sessions.backends.cache"
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис | Бележка |
| :--- | :--- | :--- |
| **Запис / Четене на сесия** | `request.session["k"] = v` / `.get("k")` | Пази се на сървъра (SQL или Redis) |
| **Принудителен запис на вложен обект** | `request.session.modified = True` | Задължително при `.append()` върху списък в сесия |
| **Изтриване на цялата сесия** | `request.session.flush()` | Изтрива данните от сървъра и бисквитката |
| **Срок на живот на сесията** | `request.session.set_expiry(3600)` | В секунди (`0` = изтича при затваряне на браузъра) |
| **Четене на Cookie** | `request.COOKIES.get("name")` | Пази се в браузъра на клиента |
| **Запис / Изтриване на Cookie** | `res.set_cookie(...)` / `res.delete_cookie("name")` | Извършва се върху `HttpResponse` обекта |
| **Почистване на стари сесии (CLI)** | `python manage.py clearsessions` | Трие изтеклите редове от `django_session` |

---
[⬅️ 06-03-00. Теория & Капани](06-03-00-Middlewares-And-Sessions.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-04-00. Django REST Framework](06-04-00-Django-REST-Framework.md)
