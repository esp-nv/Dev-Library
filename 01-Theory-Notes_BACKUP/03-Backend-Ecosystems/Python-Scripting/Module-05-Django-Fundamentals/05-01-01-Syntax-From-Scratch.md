[⬅️ 05-01-00. Теория & Капани](05-01-00-Architecture-And-HTTP.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-02-00. URLs and FBV](05-02-00-URLs-And-FBV.md)

---

# 05-01-01. Django Setup & Architecture: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни стъпки и конзолни команди за създаване на виртуална среда, инсталиране на Django и PostgreSQL драйвер, стартиране на проект и създаване на приложения от нулата.

## 🧭 Съдържание на темата
* [1. Създаване на виртуална среда и инсталация](#1-създаване-на-виртуална-среда-и-инсталация)
* [2. Създаване на Django проект и приложения (Apps)](#2-създаване-на-django-проект-и-приложения-apps)
* [3. Структура на файловете в един Django проект](#3-структура-на-файловете-в-един-django-проект)
* [4. Първоначална миграция, Админ акаунт и Стартиране](#4-първоначална-миграция-админ-акаунт-и-стартиране)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА ВИРТУАЛНА СРЕДА И ИНСТАЛАЦИЯ

Винаги изолирай пакетите на всеки Django проект в отделна виртуална среда (`venv`):

```bash
# Стъпка 1: Създаване на виртуална среда в папката на проекта
python -m venv venv

# Стъпка 2: Активиране на средата
# За Windows (PowerShell / CMD):
venv\Scripts\activate
# За Linux / macOS:
source venv/bin/activate

# Стъпка 3: Инсталиране на Django и драйвера за PostgreSQL
pip install django psycopg[binary]

# Стъпка 4: Запазване на инсталираните пакети във файл (за GitHub)
pip freeze > requirements.txt
```

---

## 2. СЪЗДАВАНЕ НА DJANGO ПРОЕКТ И ПРИЛОЖЕНИЯ (APPS)

*Съвет: Слагай точка `.` в края на `startproject`, за да не създава Django излишна вложена папка с еднакво име!*

```bash
# 1. Създаване на главния проект в текущата директория
django-admin startproject config .

# 2. Създаване на отделни модули (Apps) за функционалностите
python manage.py startapp common
python manage.py startapp books
```

След това веднага регистрирай приложенията в **`config/settings.py`**:
```python
INSTALLED_APPS = [
    # ... вградените на Django ...
    "common",
    "books",
]
```

---

## 3. СТРУКТУРА НА ФАЙЛОВЕТЕ В ЕДИН DJANGO ПРОЕКТ

```text
my_django_project/
├── manage.py               # Главен инструмент за конзолни команди
├── requirements.txt        # Списък с пакети (django, psycopg...)
├── config/                 # Папката на главния проект
│   ├── settings.py         # Всички настройки (база данни, шаблони, static, media)
│   ├── urls.py             # Главната карта с URL адреси
│   ├── wsgi.py             # Входна точка за синхронни сървъри (Gunicorn)
│   └── asgi.py             # Входна точка за асинхронни сървъри (Uvicorn)
└── books/                  # Едно приложение (App)
    ├── migrations/         # История на промените по базата данни
    ├── admin.py            # Регистрация на моделите в Admin панела
    ├── models.py           # Класовете (таблиците) в базата данни
    ├── views.py            # Бизнес логиката (обработва HTTP заявки)
    └── tests.py            # Автоматизирани тестове
```

---

## 4. ПЪРВОНАЧАЛНА МИГРАЦИЯ, АДМИН АКАУНТ И СТАРТИРАНЕ

```bash
# 1. Създаване на вградените системни таблици на Django (users, sessions, admin)
python manage.py migrate

# 2. Създаване на администраторски акаунт за достъп до /admin/
python manage.py createsuperuser

# 3. Стартиране на локалния сървър за разработка (на http://127.0.0)
python manage.py runserver
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Команда в терминала | Какво прави |
| :--- | :--- |
| `python -m venv venv` | Създава изолирана Python виртуална среда |
| `pip install django psycopg[binary]` | Инсталира Django и PostgreSQL драйвера |
| `django-admin startproject config .` | Инициализира нов Django проект в текущата папка |
| `python manage.py startapp books` | Създава ново приложение (модул) `books` |
| `python manage.py makemigrations` | Генерира миграции от промените в `models.py` |
| `python manage.py migrate` | Прилага миграциите върху базата данни |
| `python manage.py createsuperuser` | Създава админ потребител за `/admin/` панела |
| `python manage.py runserver` | Стартира локалния сървър на порт `8000` |
| `python manage.py shell` | Отваря интерактивна Python конзола със зареден Django |

---
[⬅️ 05-01-00. Теория & Капани](05-01-00-Architecture-And-HTTP.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-02-00. URLs and FBV](05-02-00-URLs-And-FBV.md)
