[⬅️ 06-06. Testing in Django](06-06-00-Testing-In-Django.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-07-01. Syntax From Scratch](06-07-01-Syntax-From-Scratch.md)

---

# 06-07-00. Deployment Setup (Теория, Gunicorn, Nginx, WhiteNoise & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 07:** Подготовка на Django за продукция (`DEBUG = False`), уеб сървърна архитектура (Nginx Reverse Proxy + Gunicorn/Uvicorn), управление на статични файлове (`collectstatic` и `WhiteNoise`), Docker контейнеризация и сигурност (`check --deploy`).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Production Сървър (Nginx + Gunicorn) и Блок-схема](#2-архитектура-на-production-сървър-nginx--gunicorn-и-блок-схема)
* [3. Архитектурно сравнение: Django Deployment срещу C# ASP.NET Core и Node.js](#3-архитектурно-сравнение-django-deployment-срещу-c-aspnet-core-и-nodejs)
* [4. Плюсове и минуси на класическия VPS Deployment срещу PaaS/Docker](#4-плюсове-и-минуси-на-класическия-vps-deployment-срещу-paasdocker)
* [5. Кодов одит: Правилен срещу Грешен код (Production Checklist)](#5-кодов-одит-правилен-срещу-грешен-код-production-checklist)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Никога не пускай `runserver` в продукция!** Командата `python manage.py runserver` е еднонишкова учебна играчка, написана само за локална разработка, и ще се срине при 10 едновременни потребители.
*   **Трите слоя на живия сървър (Вратар ➔ Двигател ➔ База):**
    1.  **Nginx (Reverse Proxy — Вратарят):** Стои отпред на порт `80`/`443`. Поема HTTPS криптирането, пази от бавни атаки и раздава светкавично картинките, CSS и JS файловете, без да притеснява Python.
    2.  **Gunicorn / Uvicorn (Application Server — Двигателят):** Стартира 4 или 8 паралелни Python работника (Workers), които изпълняват твоя Django код.
    3.  **WhiteNoise (Библиотеката-помощник):** Позволява на самия Python/Gunicorn да сервира компресирани статични файлове директно, когато качваш проекта в облачни платформи като Render, Railway или Heroku без отделен Nginx.

---

## 2. АРХИТЕКТУРА НА PRODUCTION СЪРВЪР (NGINX + GUNICORN) И БЛОК-СХЕМА

```text
Потребител (HTTPS :443)
          │
          ▼
┌──────────────────────────────────────────────────────────┐
│ NGINX (Reverse Proxy & SSL Termination)                  │
└─────────┬──────────────────────────────────────┬─────────┘
          │                                      │
          ▼ (Заявка за /static/ или /media/)     ▼ (Заявка за /api/ или /books/)
┌─────────────────────────┐            ┌───────────────────────────┐
│ Диск на сървъра         │            │ GUNICORN (WSGI / ASGI)    │
│ (staticfiles/ & media/) │            │ • Worker 1 (Python)       │
│ Връща файла за 1 ms!    │            │ • Worker 2 (Python)       │
└─────────────────────────┘            │ • Worker 3 (Python)       │
                                       └─────────────┬─────────────┘
                                                     │
                                                     ▼
                                       ┌───────────────────────────┐
                                       │ PostgreSQL & Redis        │
                                       └───────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO DEPLOYMENT СРЕЩУ C# ASP.NET CORE И NODE.JS

| Слой / Инструмент | Django Production (Python) | ASP.NET Core Production (C#) | Node.js / NestJS Production |
| :--- | :--- | :--- | :--- |
| **Application Server** | **Gunicorn** (WSGI) / **Uvicorn** (ASGI) | **Kestrel** (вграден в .NET Runtime) | Node.js процес (управляван от **PM2**) |
| **Reverse Proxy** | **Nginx** / Caddy / Cloudflare | **Nginx** / IIS / YARP | **Nginx** / Caddy |
| **Събиране на статични файлове** | `python manage.py collectstatic` | `dotnet publish` (копира `wwwroot/`) | `npm run build` (генерира `dist/`) |
| **Обслужване на статични файлове** | **Nginx** или **WhiteNoise** | `app.UseStaticFiles()` в Kestrel | `express.static()` или Nginx |
| **Проверка за сигурност преди качване** | `python manage.py check --deploy` | Analyzer warnings + Security Headers | `helmet` middleware + `npm audit` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА КЛАСИЧЕСКИЯ VPS DEPLOYMENT СРЕЩУ PAAS/DOCKER

### 🟢 Docker + PaaS (Render, Railway, AWS ECS) — Предимства и Недостатъци:
*   **Предимства:** Контейнерът гарантира, че кодът работи по абсолютно същия начин на твоя лаптоп и на сървъра; автоматичен HTTPS сертификат и лесно скалиране с 1 клик.
*   **Недостатъци:** По-висока месечна цена при натоварени бази данни и Redis; нужда от външно S3 хранилище (AWS S3 / Cloudflare R2) за качените потребителски снимки (`MEDIA`), защото Docker контейнерите губят локалните си файлове при рестарт!

### 🔵 Собствен Linux VPS (Ubuntu + Nginx + Systemd) — Предимства и Недостатъци:
*   **Предимства:** Много ниска цена и пълен контрол върху операционната система и хардуера.
*   **Недостатъци:** Ти отговаряш ръчно за обновяването на Linux, подновяването на SSL сертификатите (Certbot), защитната стена (UFW) и бекъпите на базата данни.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (PRODUCTION CHECKLIST)

### ❌ ГРЕШЕН КОД (Липсващи SSL настройки и забравен `STATIC_ROOT` при `DEBUG = False`):
```python
# 🚨 ГРЕШКА 1: В продукция слагаш DEBUG = False, но забравяш да зададеш STATIC_ROOT!
# Резултат: Целият сайт и Admin панелът зареждат като грозен бял текст без никакъв CSS!
DEBUG = False
ALLOWED_HOSTS = ["*"]  # 🚨 ГРЕШКА 2: Разрешаване на всички Host хедъри (HTTP Host Header атака)!
```
*   **Как да се избегне:** Когато `DEBUG = False`, задължително дефинирай `STATIC_ROOT = BASE_DIR / "staticfiles"`, изпълни `python manage.py collectstatic`, посочи точния домейн в `ALLOWED_HOSTS` и активирай HTTPS защитните хедъри.

### 🟢 ПРАВИЛЕН КОД (Пълен Production блок в `config/settings.py`):
```python
import os
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent

SECRET_KEY = os.environ["DJANGO_SECRET_KEY"]  # Гърми веднага, ако ключът липсва в средата!
DEBUG = False
ALLOWED_HOSTS = ["myshop.bg", "www.myshop.bg"]

# 1. Статични файлове + WhiteNoise за директно сервиране с компресия
STATIC_URL = "static/"
STATIC_ROOT = BASE_DIR / "staticfiles"
STORAGES = {
    "default": {
        "BACKEND": "django.core.files.storage.FileSystemStorage",
    },
    "staticfiles": {
        "BACKEND": "whitenoise.storage.CompressedManifestStaticFilesStorage",
    },
}

# 2. Задължителни HTTPS и Cookie защити (проверяват се с 'manage.py check --deploy')
SECURE_SSL_REDIRECT = True
SESSION_COOKIE_SECURE = True
CSRF_COOKIE_SECURE = True
SECURE_HSTS_SECONDS = 31536000  # 1 година HSTS защита
SECURE_HSTS_INCLUDE_SUBDOMAINS = True
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво точно прави командата `python manage.py collectstatic`, защо тя е задължителна при преминаване към продукция и защо качените от потребителите `MEDIA` файлове **никога** не трябва да се пазят локално в Docker контейнер?“*
*   **Уловката:** Кандидатите често бъркат `STATICFILES_DIRS` със `STATIC_ROOT` и губят снимките на потребителите си при първия ъпдейт на приложението!
*   **Точен Senior отговор:** 
    1.  **`collectstatic`:** По време на разработка статичните файлове са пръснати в десетки папки (в твоите приложения и във вградения `django.contrib.admin`). Командата `collectstatic` събира **всички** тези CSS/JS/Image файлове на едно централно място — папката **`STATIC_ROOT`**, откъдето Nginx или WhiteNoise ги раздава светкавично на клиентите.
    2.  **Защо `MEDIA` не се пази в Docker контейнер:** Docker контейнерите са **ефимерни (непостоянни)** — при всеки нов `git push` и ребилд старият контейнер се унищожава и се създава нов от чист образ. Всички снимки, качени от потребителите вътре в папката `media/` на контейнера, **изчезват безвъзвратно**! Затова в продукция `MEDIA` файловете се пазят в облачно обектно хранилище (**AWS S3**, **Cloudflare R2** или **Cloudinary** чрез пакета `django-storages`).

---
[⬅️ 06-06. Testing in Django](06-06-00-Testing-In-Django.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-07-01. Syntax From Scratch](06-07-01-Syntax-From-Scratch.md)
