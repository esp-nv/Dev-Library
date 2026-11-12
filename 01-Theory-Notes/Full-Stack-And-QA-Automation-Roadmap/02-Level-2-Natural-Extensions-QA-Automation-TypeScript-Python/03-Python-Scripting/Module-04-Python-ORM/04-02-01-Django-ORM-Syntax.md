[⬅️ 04-02-00. Теория & Сравнение](04-02-00-Migrations.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-02-02. SQLAlchemy Syntax](04-02-02-SQLAlchemy-Syntax.md)

---

# 04-02-01. Django ORM Migrations: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни конзолни команди и шаблони за създаване, проверка, прилагане и връщане на миграции в Django от нулата.

## 🧭 Съдържание на темата
* [1. Основният работен цикъл (Терминални команди)](#1-основният-работен-цикъл-терминални-команди)
* [2. Инспекция на миграциите и преглед на SQL кода](#2-инспекция-на-миграциите-и-преглед-на-sql-кода)
* [3. Връщане на миграция назад (Rollback) и Fake миграции](#3-връщане-на-миграция-назад-rollback-и-fake-миграции)
* [4. Създаване на Data Migration (Миграция на данни)](#4-създаване-на-data-migration-миграция-на-данни)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОСНОВНИЯТ РАБОТЕН ЦИКЪЛ (ТЕРМИНАЛНИ КОМАНДИ)

Всеки път, когато промениш нещо в `models.py` (добавиш клас, промениш поле или изтриеш колона), изпълняваш тези две команди в терминала:

```bash
# Стъпка 1: Генерира нов файл в папка migrations/ (напр. 0001_initial.py)
python manage.py makemigrations

# (По желание) Генериране на миграция с конкретно име и само за едно приложение:
python manage.py makemigrations store --name add_product_discount

# Стъпка 2: Прилага всички неприложени миграции към реалната база данни
python manage.py migrate
```

---

## 2. ИНСПЕКЦИЯ НА МИГРАЦИИТЕ И ПРЕГЛЕД НА SQL КОДА

```bash
# 1. Показва списък с всички миграции и кои са приложени [X] и кои не [ ]
python manage.py showmigrations

# 2. Показва точния SQL код, който Django ще изпълни за конкретна миграция
python manage.py sqlmigrate store 0001
```

---

## 3. ВРЪЩАНЕ НА МИГРАЦИЯ НАЗАД (ROLLBACK) И FAKE МИГРАЦИИ

```bash
# 1. Връща базата назад до състоянието на миграция 0001 (отменя 0002, 0003...)
python manage.py migrate store 0001

# 2. Изтрива ВСИЧКИ таблици на приложението 'store' от базата (връща до нула)
python manage.py migrate store zero

# 3. Маркира миграцията като приложена в историята БЕЗ да изпълнява SQL (ако таблицата вече съществува)
python manage.py migrate store 0002 --fake
```

---

## 4. СЪЗДАВАНЕ НА DATA MIGRATION (МИГРАЦИЯ НА ДАННИ)

Когато искаш да промениш или попълниш данни в базата като част от миграциите:

```bash
# Генерира празен миграционен файл, в който да напишеш Python функция
python manage.py makemigrations --empty store --name set_default_categories
```

```python
# Вътре в генерирания файл:
from django.db import migrations

def set_categories(apps, schema_editor):
    Product = apps.get_model("store", "Product")
    Product.objects.filter(category="").update(category="GENERAL")

class Migration(migrations.Migration):
    dependencies = [("store", "0001_initial")]
    operations = [migrations.RunPython(set_categories)]
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Команда в терминала | Какво прави |
| :--- | :--- |
| `python manage.py makemigrations` | Създава миграционен файл от промените в `models.py` |
| `python manage.py migrate` | Прилага миграциите върху базата данни |
| `python manage.py showmigrations` | Показва статуса (`[X]` или `[ ]`) на всички миграции |
| `python manage.py sqlmigrate app 0001` | Принтира суровия SQL код на миграция `0001` |
| `python manage.py migrate app 0001` | Връща базата назад (Rollback) до миграция `0001` |
| `python manage.py migrate app zero` | Отменя всички миграции на конкретното приложение |
| `python manage.py makemigrations --empty app` | Създава празна миграция за потребителски Python код |

---
[⬅️ 04-02-00. Теория & Сравнение](04-02-00-Migrations.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-02-02. SQLAlchemy Syntax](04-02-02-SQLAlchemy-Syntax.md)
