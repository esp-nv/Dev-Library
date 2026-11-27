[⬅️ 06-05-00. Теория & Капани](06-05-00-Celery-And-Redis.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-06-00. Testing in Django](06-06-00-Testing-In-Django.md)

---

# 06-05-01. Celery & Redis in Django: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни стъпки за инсталиране и настройване на Celery и Redis в Django, създаване на `celery.py`, писане на задачи (`@shared_task`), планиране с Celery Beat и кеширане от нулата.

## 🧭 Съдържание на темата
* [1. Инсталация и конфигурация на `config/celery.py`](#1-инсталация-и-конфигурация-на-configcelerypy)
* [2. Настройки в `config/settings.py` и `config/__init__.py`](#2-настройки-в-configsettingspy-и-config__init__py)
* [3. Писане и викане на фонови задачи (`@shared_task`)](#3-писане-и-викане-на-фонови-задачи-shared_task)
* [4. Стартиране на Worker и Периодични задачи (Celery Beat)](#4-стартиране-на-worker-и-периодични-задачи-celery-beat)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИНСТАЛАЦИЯ И КОНФИГУРАЦИЯ НА `CONFIG/CELERY.PY`

```bash
pip install celery redis
```

Създай нов файл **`config/celery.py`** (до `settings.py`):
```python
import os
from celery import Celery

# 1. Задава стандартния файл с настройки на Django
os.environ.setdefault("DJANGO_SETTINGS_MODULE", "config.settings")

# 2. Създава инстанция на Celery приложението
app = Celery("config")

# 3. Чете всички настройки от settings.py, започващи с префикс 'CELERY_'
app.config_from_object("django.conf:settings", namespace="CELERY")

# 4. Автоматично намира всички файлове 'tasks.py' във всяко приложение (App)
app.autodiscover_tasks()
```

---

## 2. НАСТРОЙКИ В `CONFIG/SETTINGS.PY` И `CONFIG/__INIT__.PY`

### А. Във файла `config/__init__.py` (за да се зарежда Celery при старт на Django):
```python
from .celery import app as celery_app

__all__ = ("celery_app",)
```

### Б. Във файла `config/settings.py`:
```python
CELERY_BROKER_URL = "redis://127.0.0.1:6379/0"
CELERY_RESULT_BACKEND = "redis://127.0.0.1:6379/0"
CELERY_ACCEPT_CONTENT = ["json"]
CELERY_TASK_SERIALIZER = "json"
CELERY_TIMEZONE = "Europe/Sofia"
```

---

## 3. ПИСАНЕ И ВИКАНЕ НА ФОНОВИ ЗАДАЧИ (`@SHARED_TASK`)

Създай файл **`tasks.py`** вътре в приложението си (напр. `books/tasks.py`):

```python
import time
from celery import shared_task

@shared_task
def generate_monthly_report(user_id: int) -> str:
    time.sleep(5)  # Симулация на тежка 5-секундна работа
    return f"Отчетът за потребител #{user_id} е генериран успешно!"
```

### Извикване от `views.py`:
```python
from .tasks import generate_monthly_report

def report_view(request):
    # 1. Стандартно фоново пускане (веднага в опашката)
    generate_monthly_report.delay(request.user.id)

    # 2. Пускане със закъснение (например след 60 секунди)
    generate_monthly_report.apply_async(args=[request.user.id], countdown=60)
```

---

## 4. СТАРТИРАНЕ НА WORKER И ПЕРИОДИЧНИ ЗАДАЧИ (CELERY BEAT)

### А. Команди в терминала (в отделен прозорец от `runserver`):
```bash
# За Linux / macOS:
celery -A config worker --loglevel=info

# За Windows (задължително се добавя --pool=solo):
celery -A config worker --loglevel=info --pool=solo

# Стартиране на планировчика за периодични задачи (Celery Beat):
celery -A config beat --loglevel=info
```

### Б. Планиране на периодична задача в `settings.py` (на всеки 30 минути):
```python
from celery.schedules import crontab

CELERY_BEAT_SCHEDULE = {
    "cleanup-every-midnight": {
        "task": "books.tasks.generate_monthly_report",
        "schedule": crontab(hour=0, minute=0),  # Всяка нощ в 00:00 ч.
        "args": (1,),
    },
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Команда / Конструкция | Какво прави |
| :--- | :--- |
| **`@shared_task`** | Декоратор, който превръща функция в фонова Celery задача |
| **`task.delay(arg1, arg2)`** | Изпраща задачата в Redis опашката (кратък синтаксис) |
| **`task.apply_async(args=[...], countdown=10)`** | Изпраща задачата със закъснение или приоритет |
| **`celery -A config worker -l info`** | Стартира работния процес, който изпълнява задачите |
| **`celery -A config beat -l info`** | Стартира планировчика за периодични Cron задачи |
| **`cache.set("key", val, timeout=300)`** | Записва данни в Redis кеша за 5 минути |
| **`cache.get("key")`** | Чете кеширани данни от Redis RAM паметта |

---
[⬅️ 06-05-00. Теория & Капани](06-05-00-Celery-And-Redis.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-06-00. Testing in Django](06-06-00-Testing-In-Django.md)
