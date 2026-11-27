[⬅️ 04-00. Intro & Roadmap](04-00-Intro.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01-01. Django ORM Syntax](04-01-01-Django-ORM-Syntax.md)

---

# 04-01-00. Models, Fields and Data Types (Теория, Сравнение & Интервю Капани)

**Модул 04:** Python ORM | **Лекционна тема 01:** Превръщане на Python класове в SQL таблици, типове колони, първични ключове, ограничения (`null`, `blank`, `unique`) и метаданни на таблиците.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на преобразуване Клас ➔ SQL Таблица (Блок-схема)](#2-етапи-на-преобразуване-клас--sql-таблица-блок-схема)
* [2.1. АЛТЕРНАТИВЕН ПОТОК: КЛАС ➔ NOSQL (MONGODB / REDIS), JSON И XML](#21-алтернативен-поток-клас--nosql-mongodb--redis-json-и-xml)
* [3. Архитектурно сравнение: Django ORM срещу SQLAlchemy и C# EF Core](#3-архитектурно-сравнение-django-orm-срещу-sqlalchemy-и-c-ef-core)
* [4. Плюсове и минуси при дефиниране на модели](#4-плюсове-и-минуси-при-дефиниране-на-модели)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Един Клас = Една Таблица, Един Атрибут = Една Колона, Един Обект = Един Ред.**
*   **Разликата между `null=True` и `blank=True` (Златно правило в Django):**
    *   **`null=True`** е за **Базата данни (SQL)** ➔ разрешава колоната да пази стойност `NULL`.
    *   **`blank=True`** е за **Формите и Валидацията (Python/UI)** ➔ разрешава потребителят да остави полето празно, когато попълва форма.

---

## 2. ЕТАПИ НА ПРЕОБРАЗУВАНЕ КЛАС ➔ SQL ТАБЛИЦА (БЛОК-СХЕМА)

```text
┌──────────────────────────┐     1. Инспекция на класа  ┌─────────────────────────┐
│ Python Клас (Model)      │ ─────────────────────────► │ ORM Метаклас / Mapper   │
│ • title: CharField(100)  │   (Чете типовете и         │ Превежда типовете в     │
│ • price: DecimalField    │    ограниченията)          │ SQL диалект (Postgres)  │
└──────────────────────────┘                            └────────────┬────────────┘
                                                                     │ 2. DDL Заявка
                                                                     ▼
┌──────────────────────────┐     3. Физическа таблица   ┌─────────────────────────┐
│ CREATE TABLE products (  │ ◄───────────────────────── │ База Данни (PostgreSQL /│
│   id SERIAL PRIMARY KEY, │   (Създава колони, ключове │ SQLite / SQL Server)    │
│   title VARCHAR(100) ... │    и индекси на диска)     │                         │
└──────────────────────────┘                            └─────────────────────────┘
```

---
## 2.1. АЛТЕРНАТИВЕН ПОТОК: КЛАС ➔ NOSQL (MONGODB / REDIS), JSON И XML

Когато целта не е релационна SQL база, вместо ORM (Object-Relational Mapping) се използва **ODM (Object-Document Mapper)** или **Сериализация**:

┌──────────────────────────┐
│ Python Клас / Обект      │
│ • name: "Laptop"         │
│ • tags: ["tech", "pc"]   │
└────────────┬─────────────┘
             │
      ┌──────┴──────────────────────────┐
      │                                 │
      ▼ (ODM: Beanie / MongoEngine)     ▼ (Сериализатор: Pydantic / json / xml)
┌──────────────────────────┐      ┌──────────────────────────┐
│ NoSQL Документ (MongoDB) │      │ Текстов поток (API/Файл) │
│ Превръща обекта в BSON   │      │ Превръща обекта в низ:   │
│ речник без строга таблица│      │ • JSON: {"name":"Laptop"}│
│ и без миграции (Schema-  │      │ • XML:  <name>Laptop</..>│
│ less)                    │      │                          │
└──────────────────────────┘      └──────────────────────────┘

### 📊 Сравнителна таблица: SQL срещу NoSQL, JSON и XML

| Целева система | Инструмент в Python | Формат на изхода | Нужни ли са миграции? |
| :--- | :--- | :--- | :--- |
| **SQL Бази** (PostgreSQL, MSSQL) | Django ORM / SQLAlchemy | Таблици, редове и колони | **Да** (задължително) |
| **NoSQL Бази** (MongoDB) | Beanie / MongoEngine (ODM) | BSON / JSON документи | **Не** (Schema-less) |
| **Кеш Бази** (Redis) | `redis-py` + `json` | Ключ ➔ JSON низ | **Не** |
| **Уеб API (JSON)** | Pydantic / `json` | `{"id": 1, "name": "A"}` | **Не** |
| **Legacy / SOAP (XML)** | `xmltodict` / `ElementTree` | `<user><id>1</id></user>` | **Не** |

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO ORM СРЕЩУ SQLALCHEMY И C# EF CORE

| Елемент | Django ORM | SQLAlchemy (v2.0+) | C# Entity Framework Core |
| :--- | :--- | :--- | :--- |
| **Име на таблицата** | Автоматично (`appname_modelname`) или `Meta.db_table` | Задължително `__tablename__ = "users"` | Име на `DbSet<User>` или `[Table("users")]` |
| **Първичен ключ (`id`)** | Създава се **автоматично** (BigAutoField) | Пише се **изрично** (`primary_key=True`) | Автоматично за `Id` или `UserId` |
| **Кратък текст (`VARCHAR`)** | `models.CharField(max_length=100)` | `mapped_column(String(100))` | `public string Name { get; set; }` (`[MaxLength]`) |
| **Дълъг текст (`TEXT`)** | `models.TextField()` | `mapped_column(Text)` | `public string Bio { get; set; }` (`nvarchar(max)`) |
| **Пари (`DECIMAL`)** | `models.DecimalField(max_digits=10, decimal_places=2)` | `mapped_column(Numeric(10, 2))` | `public decimal Price { get; set; }` |
| **Дата на създаване** | `models.DateTimeField(auto_now_add=True)` | `mapped_column(server_default=func.now())` | `DateTime.UtcNow` / `HasDefaultValueSql` |
| **Опционално поле (`NULL`)** | `null=True, blank=True` | `Mapped[Optional[str]]` (или `str \| None`) | `public string? MiddleName { get; set; }` |

---

## 4. ПЛЮСОВЕ И МИНУСИ ПРИ ДЕФИНИРАНЕ НА МОДЕЛИ

### 🟢 Django ORM — Предимства и Недостатъци:
*   **Предимства:** Автоматичен `id` ключ; богата вградена валидация (`EmailField`, `URLField`, `SlugField`); готови `choices` за падащи менюта в админ панела.
*   **Недостатъци:** `CharField` изисква задължително `max_length`; исторически не поддържаше композитни първични ключове (съставени от 2 колони) до версия 5.2.

### 🔵 SQLAlchemy 2.0 — Предимства и Недостатъци:
*   **Предимства:** Пълна поддръжка на модерни Python Type Hints (`Mapped[int]`, `Mapped[str | None]`), което дава перфектно автодопълнение във VS Code; нативна поддръжка на композитни ключове, персонализирани SQL типове и PostgreSQL масиви/JSONB.
*   **Недостатъци:** По-дълъг синтаксис при писане на модели; липсват вградени уеб валидатори (като `EmailField`) — за тях се ползва външна библиотека като Pydantic.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Използване на `FloatField` за пари и `null=True` на текстово поле в Django):
```python
from django.db import models

class BadOrder(models.Model):
    # 🚨 ГРЕШКА 1: FloatField за пари води до грешки от закръгляване (0.1 + 0.2 = 0.30000000000000004)!
    total_price = models.FloatField()

    # 🚨 ГРЕШКА 2: Слагане на null=True на CharField в Django!
    # Това създава две различни "празни" състояния в базата: NULL и празен стринг ""!
    notes = models.CharField(max_length=200, null=True, blank=True)
```
*   **Как да се избегне:** За финансови суми **винаги** използвай `DecimalField` (в Django) или `Numeric` (в SQLAlchemy). В Django за незадължителни текстови полета пиши само `blank=True, default=""` (без `null=True`).

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт за Модел):
```python
from decimal import Decimal
from django.db import models

class Product(models.Model):
    class Category(models.TextChoices):
        ELECTRONICS = "EL", "Електроника"
        BOOKS = "BK", "Книги"

    name = models.CharField(max_length=150, unique=True)
    description = models.TextField(blank=True, default="")
    price = models.DecimalField(max_digits=10, decimal_places=2)
    category = models.CharField(max_length=2, choices=Category.choices, default=Category.ELECTRONICS)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        db_table = "store_products"
        ordering = ["-created_at"]  # Сортира най-новите отгоре по подразбиране

    def __str__(self) -> str:
        return f"{self.name} ({self.price} лв.)"
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между параметрите `auto_now_add=True` и `auto_now=True` при `DateTimeField` в Django ORM и каква уловка крият при обновяване с `.update()`?“*
*   **Уловката:** Кандидатите често ги бъркат, а освен това не знаят, че те работят на ниво Python, а не на ниво SQL тригер!
*   **Точен Senior отговор:** 
    1.  **`auto_now_add=True`:** Записва текущата дата и час **само веднъж — при създаването** на записа (`INSERT`). Идеално за колона `created_at`.
    2.  **`auto_now=True`:** Презаписва датата и часа **при всяко извикване на `obj.save()`** (`UPDATE`). Идеално за колона `updated_at`.
    *   *Скритата уловка:* Тъй като `auto_now=True` се задейства вътре в Python метода `.save()`, ако обновиш записите директно на ниво база данни чрез масов **`Product.objects.filter(...).update(price=50)`**, методът `.save()` **не се извиква** и колоната `updated_at` **няма да се обнови**!

---
[⬅️ 04-00. Intro & Roadmap](04-00-Intro.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01-01. Django ORM Syntax](04-01-01-Django-ORM-Syntax.md)
