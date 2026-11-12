[⬅️ 04-05-00. Теория & Сравнение](04-05-00-Optimization-And-N-Plus-1.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-05-02. SQLAlchemy Syntax](04-05-02-SQLAlchemy-Syntax.md)

---

# 04-05-01. Django ORM Optimization: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `select_related`, `prefetch_related`, `Prefetch`, `only`, `values` и бързо извличане на данни за JSON/XML от нулата.

## 🧭 Съдържание на темата
* [1. `select_related` (За 1:1 и ForeignKey — SQL JOIN)](#1-select_related-за-11-и-foreignkey--sql-join)
* [2. `prefetch_related` и `Prefetch` (За ManyToMany и обратни връзки)](#2-prefetch_related-и-prefetch-за-manytomany-и-обратни-връзки)
* [3. Ограничаване на колоните (`only`, `defer`, `values`, `values_list`)](#3-ограничаване-на-колоните-only-defer-values-values_list)
* [4. Оптимизирано извличане за JSON и XML API потоци](#4-оптимизирано-извличане-за-json-и-xml-api-потоци)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. `SELECT_RELATED` (ЗА 1:1 И FOREIGNKEY — SQL JOIN)

Използва се, когато вървиш **нагоре** по `ForeignKey` или `OneToOneField`. Можеш да навързваш и вложени връзки с двойна долна черта `__`:

```python
from store.models import Book

# Зарежда Book + Author + Publisher с 1 единствена SQL JOIN заявка!
books = Book.objects.select_related("author", "author__publisher").all()

for book in books:
    # Нито една допълнителна SQL заявка не се изпълнява тук:
    print(f"{book.title} - {book.author.name} ({book.author.publisher.country})")
```

---

## 2. `PREFETCH_RELATED` И `PREFETCH` (ЗА MANYTOMANY И ОБРАТНИ ВРЪЗКИ)

Използва се, когато вървиш **надолу** към списък от деца (`ManyToManyField` или обратен `related_name`).

```python
from django.db.models import Prefetch
from store.models import Author, Book

# 1. Стандартно зареждане на всички книги на авторите (общо 2 SQL заявки)
authors = Author.objects.prefetch_related("books").all()

# 2. Филтрирано зареждане с обект Prefetch (зарежда САМО активните книги в кеша!)
active_books_qs = Book.objects.filter(is_published=True).order_by("-year")

authors_with_active = Author.objects.prefetch_related(
    Prefetch("books", queryset=active_books_qs, to_attr="published_books")
)

for author in authors_with_active:
    # Достъпваме директно списъка 'published_books' без нова заявка към базата
    print(author.name, len(author.published_books))
```

---

## 3. ОГРАНИЧАВАНЕ НА КОЛОНИТЕ (`ONLY`, `DEFER`, `VALUES`, `VALUES_LIST`)

Когато имаш таблица с 40 колони (включително големи текстове), не ти трябва да зареждаш всичко в RAM паметта:

```python
# 1. only() -> Зарежда САМО посочените колони в обекта (другите се четат чак ако ги поискаш)
light_books = Book.objects.only("id", "title")

# 2. defer() -> Зарежда всичко БЕЗ тежката колона 'full_content'
fast_articles = Article.objects.defer("full_content")

# 3. values() -> Връща списък от речници (dict) вместо тежки Python модел-обекти (най-бързо!)
dicts_list = Book.objects.values("id", "title", "author__name")

# 4. values_list(flat=True) -> Връща плосък списък само от стойности: [1, 2, 3, 4]
all_ids = list(Book.objects.values_list("id", flat=True))
```

---

## 4. ОПТИМИЗИРАНО ИЗВЛИЧАНЕ ЗА JSON И XML API ПОТОЦИ

Когато връщаш хиляди записи като JSON или XML към фронтенда, комбинацията от `.values()` и `json.dumps()` е в пъти по-бърза от сериализирането на пълни ORM модели:

```python
import json
import xml.etree.ElementTree as ET

# Стъпка 1: Извличаме директно речници от SQL (без инстанциране на ORM класове)
rows = list(Book.objects.select_related("author").values("id", "title", "author__name"))

# Стъпка 2: Светкавично превръщане в JSON за API отговор
fast_json = json.dumps(rows, ensure_ascii=False)

# Стъпка 3: Бързо изграждане на XML от същите леки речници
root = ET.Element("Books")
for r in rows:
    b_node = ET.SubElement(root, "Book", id=str(r["id"]))
    b_node.set("author", r["author__name"])
    b_node.text = r["title"]

fast_xml = ET.tostring(root, encoding="unicode")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод | Кога се използва | Брой SQL заявки |
| :--- | :--- | :--- |
| **`.select_related("fk")`** | За `ForeignKey` и `OneToOne` (1 родител) | **1 заявка** (`SQL JOIN`) |
| **`.prefetch_related("m2m")`** | За `ManyToMany` и списък от деца | **2 заявки** (`WHERE id IN (...)`) |
| **`Prefetch("rel", queryset=...)`** | Когато искаш да филтрираш заредените деца | **2 заявки** (филтрирани) |
| **`.only("id", "name")`** | Изтегля само посочените колони в модела | **1 заявка** (пести RAM) |
| **`.defer("big_text")`** | Пропуска тежки колони при зареждане | **1 заявка** (пести RAM) |
| **`.values("id", "name")`** | Връща леки Python речници (`dict`) вместо модели | **1 заявка** (максимална скорост) |
| **`.values_list("id", flat=True)`** | Връща плосък списък `[1, 2, 3]` | **1 заявка** |

---
[⬅️ 04-05-00. Теория & Сравнение](04-05-00-Optimization-And-N-Plus-1.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-05-02. SQLAlchemy Syntax](04-05-02-SQLAlchemy-Syntax.md)
