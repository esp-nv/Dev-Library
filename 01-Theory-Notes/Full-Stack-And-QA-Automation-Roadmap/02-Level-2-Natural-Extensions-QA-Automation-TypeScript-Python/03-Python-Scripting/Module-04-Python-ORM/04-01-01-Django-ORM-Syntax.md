[⬅️ 04-01-00. Теория & Сравнение](04-01-00-Models-And-Fields.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01-02. SQLAlchemy Syntax](04-01-02-SQLAlchemy-Syntax.md)

---

# 04-01-01. Django ORM Models: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на модели, полета, ограничения и `Meta` настройки в Django ORM от нулата.

## 🧭 Съдържание на темата
* [1. Основен скелет на Django модел](#1-основен-скелет-на-django-модел)
* [2. Всички основни типове полета (Field Types)](#2-всички-основни-типове-полета-field-types)
* [3. Ограничения на колоните (Field Options)](#3-ограничения-на-колоните-field-options)
* [4. Вътрешният клас `Meta` (Настройки на таблицата)](#4-вътрешният-клас-meta-настройки-на-таблицата)
* [4.1. ПРЕОБРАЗУВАНЕ В JSON, XML И NOSQL (MONGODB) СТЪПКА ПО СТЪПКА](#41-преобразуване-в-json-xml-и-nosql-mongodb-стъпка-по-стъпка)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОСНОВЕН СКЕЛЕТ НА DJANGO МОДЕЛ

Всеки модел в Django е клас в `models.py`, който задължително наследява **`models.Model`**. Полето `id` (Primary Key) се добавя автоматично от Django.

```python
from django.db import models

class Student(models.Model):
    first_name = models.CharField(max_length=50)
    last_name = models.CharField(max_length=50)
    email = models.EmailField(unique=True)
    age = models.PositiveIntegerField()
    is_active = models.BooleanField(default=True)

    # Определя как ще се показва обектът в Admin панела и при print()
    def __str__(self):
        return f"{self.first_name} {self.last_name}"
```

---

## 2. ВСИЧКИ ОСНОВНИ ТИПОВЕ ПОЛЕТА (FIELD TYPES)

```python
class DataTypesDemo(models.Model):
    # Текстови полета
    short_text = models.CharField(max_length=100)       # VARCHAR(100) - задължителен max_length
    long_text = models.TextField()                      # TEXT - за статии, коментари (без лимит)
    slug = models.SlugField(unique=True)                # За URL адреси (напр. "python-orm-guide")
    email = models.EmailField()                         # Валидира имейл формат

    # Числови полета
    integer_num = models.IntegerField()                 # Цяло число (+ и -)
    positive_num = models.PositiveIntegerField()        # Само положителни цели числа (>= 0)
    money = models.DecimalField(max_digits=10, decimal_places=2)  # За пари (напр. 12345678.99)

    # Логически и времеви полета
    flag = models.BooleanField(default=False)           # True / False
    birth_date = models.DateField()                     # Само дата (YYYY-MM-DD)
    created_at = models.DateTimeField(auto_now_add=True)# Дата + час при създаване
    updated_at = models.DateTimeField(auto_now=True)    # Дата + час при всяко .save()

    # Уникален идентификатор (вместо числово ID)
    # import uuid
    # uuid_id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
```

---

## 3. ОГРАНИЧЕНИЯ НА КОЛОНИТЕ (FIELD OPTIONS)

```python
class Account(models.Model):
    username = models.CharField(
        max_length=30,
        unique=True,                # Не позволява дубликати в базата
        db_index=True,              # Създава SQL индекс за по-бързо търсене
        verbose_name="Потребител"   # Красиво име в Admin панела
    )
    bio = models.TextField(
        blank=True,                 # Позволява празно поле във формите
        default=""                  # Стойност по подразбиране
    )
    deleted_at = models.DateTimeField(
        null=True,                  # Позволява NULL в базата данни
        blank=True                  # Позволява празно във формите
    )
```

---

## 4. ВЪТРЕШНИЯТ КЛАС `META` (НАСТРОЙКИ НА ТАБЛИЦАТА)

Вътре в модела можеш да добавиш вложен клас `class Meta:`, за да управляваш поведението на цялата SQL таблица:

```python
class Article(models.Model):
    title = models.CharField(max_length=200)
    published_date = models.DateTimeField()

    class Meta:
        db_table = "blog_articles"              # Персонализирано име на таблицата в SQL
        ordering = ["-published_date", "title"] # Сортиране по подразбиране ('-' е низходящо)
        verbose_name = "Статия"                 # Име в единствено число в Admin
        verbose_name_plural = "Статии"          # Име в множествено число в Admin
```
## 4.1. ПРЕОБРАЗУВАНЕ НА DJANGO МОДЕЛ В JSON И XML (СТЪПКА ПО СТЪПКА)

Django разполага с вграден сериализатор (`django.core.serializers`), който превръща моделите директно в JSON или XML потоци:

```python
from django.core import serializers
from django.forms.models import model_to_dict

# Стъпка 1: Създаваме или взимаме обект от Django модел
student = Student(id=1, first_name="Елизабет", last_name="Иванова", email="eli@softuni.bg", age=25)

# Стъпка 2: Превръщане на единичен модел в Python речник (dict)
student_dict = model_to_dict(student)
print(student_dict)
# Изход: {'id': 1, 'first_name': 'Елизабет', 'last_name': 'Иванова', 'email': 'eli@softuni.bg', 'age': 25, 'is_active': True}

# Стъпка 3: Превръщане на списък от модели (QuerySet) директно в JSON низ
json_output = serializers.serialize("json", [student], ensure_ascii=False)
print(json_output)

# Стъпка 4: Превръщане на същите модели директно в XML формат
xml_output = serializers.serialize("xml", [student])
print(xml_output)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Поле / Настройка | Синтаксис в Django ORM | SQL Еквивалент |
| :--- | :--- | :--- |
| **Кратък текст** | `models.CharField(max_length=100)` | `VARCHAR(100)` |
| **Дълъг текст** | `models.TextField()` | `TEXT` |
| **Парична сума** | `models.DecimalField(max_digits=8, decimal_places=2)` | `DECIMAL(8, 2)` |
| **Булева стойност** | `models.BooleanField(default=True)` | `BOOLEAN` |
| **Дата на създаване** | `models.DateTimeField(auto_now_add=True)` | `TIMESTAMP` (при `INSERT`) |
| **Уникална стойност** | `unique=True` | `UNIQUE` constraint |
| **Позволяване на `NULL`** | `null=True, blank=True` | `NULL` (за нетекстови полета) |
| **Сортиране на таблицата** | `class Meta: ordering = ["-id"]` | `ORDER BY id DESC` |

---
[⬅️ 04-01-00. Теория & Сравнение](04-01-00-Models-And-Fields.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01-02. SQLAlchemy Syntax](04-01-02-SQLAlchemy-Syntax.md)
