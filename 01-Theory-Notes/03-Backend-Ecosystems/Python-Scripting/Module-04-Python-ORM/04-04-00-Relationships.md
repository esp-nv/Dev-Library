[⬅️ 04-03. CRUD and Queries](04-03-00-CRUD-And-Queries.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-01. Django ORM Syntax](04-04-01-Django-ORM-Syntax.md)

---

# 04-04-00. Table Relationships (Теория, Foreign Keys, NoSQL Embedding & Капани)

**Модул 04:** Python ORM | **Лекционна тема 04:** Трите релационни връзки (One-to-One, One-to-Many, Many-to-Many), каскадно изтриване (`on_delete`), `related_name` срещу `back_populates`, и сравнение с NoSQL вграждане (Embedding) и JSON/XML йерархии.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурна разлика: SQL Релации срещу NoSQL Embedding (Блок-схема)](#2-архитектурна-разлика-sql-релации-срещу-nosql-embedding-блок-схема)
* [3. Архитектурно сравнение: Django ORM, SQLAlchemy, C# EF Core и NoSQL/JSON/XML](#3-архитектурно-сравнение-django-orm-sqlalchemy-c-ef-core-и-nosqljsonxml)
* [4. Плюсове и минуси на SQL Релациите срещу NoSQL Вграждането](#4-плюсове-и-минуси-на-sql-релациите-срещу-nosql-вграждането)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Къде стои Външният ключ (`ForeignKey`)? Винаги при „Многото“ (Many)!**
    *   Един автор (`Author`) пише много книги (`Books`). Къде слагаме `author_id`? В таблицата **`Books`**, защото всяка книга има само един автор.
*   **Трите типа връзки в 3 думи:**
    *   **One-to-One (1:1):** Човек ➔ ЕГН / Паспорт (строго един към един).
    *   **One-to-Many (1:N):** Държава ➔ Градове (една държава има много градове).
    *   **Many-to-Many (M:N):** Студенти ➔ Курсове (изисква трета свързваща таблица — Junction / Association Table).
*   **SQL Нормализация срещу NoSQL Вграждане:** В SQL разбиваш данните на отделни таблици и ги съединяваш с `JOIN`. В NoSQL (MongoDB) и JSON просто **слагаш списъка вътре в самия обект** (Embedding).

---

## 2. АРХИТЕКТУРНА РАЗЛИКА: SQL РЕЛАЦИИ СРЕЩУ NOSQL EMBEDDING (БЛОК-СХЕМА)

```text
1. РЕЛАЦИОНЕН МОДЕЛ (SQL - Django ORM / SQLAlchemy):
┌──────────────────┐         1 : N         ┌──────────────────────┐
│ Таблица: authors │ ◄──────────────────── │ Таблица: books       │
│ • id = 1         │   (author_id = 1)     │ • id = 10, title="A" │
│ • name = "Иван"  │                       │ • id = 11, title="B" │
└──────────────────┘                       └──────────────────────┘

2. ДОКУМЕНТЕН / ЙЕРАРХИЧЕН МОДЕЛ (MongoDB / JSON / XML):
┌─────────────────────────────────────────────────────────┐
│ Документ: Author (Няма втора таблица, няма JOIN!)        │
│ {                                                       │
│   "id": 1,                                              │
│   "name": "Иван",                                       │
│   "books": [ {"id": 10, "title": "A"}, {"id": 11, ...} ]│
│ }                                                       │
└─────────────────────────────────────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO ORM, SQLALCHEMY, C# EF CORE И NOSQL/JSON/XML

| Тип връзка / Концепция | Django ORM | SQLAlchemy (v2.0+) | C# EF Core | NoSQL (Mongo) / JSON / XML |
| :--- | :--- | :--- | :--- | :--- |
| **One-to-One (1:1)** | `models.OneToOneField(User, ...)` | `ForeignKey(..., unique=True)` + `uselist=False` | `HasOne().WithOne()` | Вложен обект `{ "profile": {...} }` |
| **One-to-Many (1:N)** | `models.ForeignKey(Author, ...)` | `ForeignKey("authors.id")` + `relationship()` | `HasMany().WithOne()` | Вложен масив `"books": [...]` |
| **Many-to-Many (M:N)** | `models.ManyToManyField(Tag)` | `Table("assoc", ...)` + `secondary=` | `HasMany().WithMany()` | Масив от ID-та `["tag1", "tag2"]` |
| **Обратна навигация** | `related_name="books"` | `back_populates="books"` | Навигационно свойство | Няма (чете се от корена надолу) |
| **При изтриване (Cascade)** | `on_delete=models.CASCADE` | `cascade="all, delete-orphan"` | `DeleteBehavior.Cascade` | Трие целия документ наведнъж |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SQL РЕЛАЦИИТЕ СРЕЩУ NOSQL ВГРАЖДАНЕТО

### 🟢 SQL Релации (Django ORM / SQLAlchemy) — Предимства и Недостатъци:
*   **Предимства:** Нулево дублиране на данни (ако авторът си смени името, променяш го на 1 място); желязна цялост на данните (базата не позволява поръчка с несъществуващ `user_id`).
*   **Недостатъци:** Четенето на сложни обекти изисква скъпи `JOIN` операции между много таблици.

### 🟣 NoSQL Embedding / JSON / XML — Предимства и Недостатъци:
*   **Предимства:** Светкавично четене с 1 заявка (целият обект с децата му пристига наведнъж без `JOIN`); естествен формат за REST API (JSON) и банкови съобщения (XML).
*   **Недостатъци:** Дублиране на данни при M:N връзки; лимит от 16 MB на документ в MongoDB (не можеш да вградиш 1 милион коментара в една статия).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Опасен `CASCADE` върху финансови записи и липсващ `related_name`):
```python
from django.db import models

class Customer(models.Model):
    name = models.CharField(max_length=100)

class Invoice(models.Model):
    # 🚨 ГРЕШКА 1: Използване на CASCADE върху фактури!
    # Ако изтриеш клиента, базата автоматично унищожава всичките му счетоводни фактури!
    customer = models.ForeignKey(Customer, on_delete=models.CASCADE)
```
*   **Как да се избегне:** За важни исторически и финансови записи (фактури, плащания, логове) **винаги** използвай **`on_delete=models.PROTECT`** (забранява изтриването на клиента, ако има фактури) или **`models.SET_NULL`** (запазва фактурата, но слага `customer = NULL`).

### 🟢 ПРАВИЛЕН КОД (Защитена релация с `PROTECT` и ясен `related_name`):
```python
from django.db import models

class Customer(models.Model):
    name = models.CharField(max_length=100)

class Invoice(models.Model):
    # PROTECT хвърля ProtectedError, ако някой се опита да изтрие клиент с активни фактури!
    customer = models.ForeignKey(
        Customer,
        on_delete=models.PROTECT,
        related_name="invoices"  # Позволява четене: customer.invoices.all()
    )
    amount = models.DecimalField(max_digits=10, decimal_places=2)
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между `on_delete=models.CASCADE` в Django ORM (който се изпълнява в Python паметта) и `ON DELETE CASCADE` на ниво SQL сървър (както в SQLAlchemy `passive_deletes=True`), и какъв проблем възниква при 1 милион свързани записа?“*
*   **Уловката:** Кандидатите мислят, че `models.CASCADE` в Django просто слага `ON DELETE CASCADE` в SQL таблицата.
*   **Точен Senior отговор:** По подразбиране Django ORM **не слага** `ON DELETE CASCADE` в SQL базата! Когато извикаш `author.delete()`, Django първо изтегля **всички свързани книги в RAM паметта на Python**, за да задейства сигналите (`pre_delete`, `post_delete`), и след това ги трие. Ако авторът има 500 000 записа, сървърът ще гръмне с `OutOfMemory`!
    *   **Правилен фикс:** В SQLAlchemy се използва `ondelete="CASCADE"` на `ForeignKey` + `passive_deletes=True`, а в Django 5+ се използва `db_on_delete=models.DB_CASCADE`, за да се прехвърли работата директно към бързия SQL сървър.

---
[⬅️ 04-03. CRUD and Queries](04-03-00-CRUD-And-Queries.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-01. Django ORM Syntax](04-04-01-Django-ORM-Syntax.md)
