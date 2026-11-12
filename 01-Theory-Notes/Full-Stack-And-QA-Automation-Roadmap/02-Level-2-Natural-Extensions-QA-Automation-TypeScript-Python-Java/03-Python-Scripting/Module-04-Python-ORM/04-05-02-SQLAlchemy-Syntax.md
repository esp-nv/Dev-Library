[⬅️ 04-05-01. Django ORM Syntax](04-05-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-00. Transactions and Async](04-06-00-Transactions-And-Async.md)

---

# 04-05-02. SQLAlchemy 2.0 Optimization: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `joinedload`, `selectinload`, `load_only`, предотвратяване на скрит Lazy Loading (`raise_on_sql`) и оптимизация в NoSQL (MongoDB / Redis) от нулата.

## 🧭 Съдържание на темата
* [1. `joinedload` (За 1:1 и Many-to-One — SQL JOIN)](#1-joinedload-за-11-и-many-to-one--sql-join)
* [2. `selectinload` (За One-to-Many и Many-to-Many — `WHERE IN`)](#2-selectinload-за-one-to-many-и-many-to-many--where-in)
* [3. Ограничаване на колоните (`load_only`) и защита против N+1 (`raiseload`)](#3-ограничаване-на-колоните-load_only-и-защита-против-n1-raiseload)
* [4. Оптимизация на заявки в NoSQL (MongoDB Indexes, Projections и Redis Cache)](#4-оптимизация-на-заявки-в-nosql-mongodb-indexes-projections-и-redis-cache)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. `JOINEDLOAD` (ЗА 1:1 И MANY-TO-ONE — SQL JOIN)

В SQLAlchemy 2.0 стратегиите за зареждане се подават вътре в `.options(...)` на `select()` заявката:

```python
from sqlalchemy import select
from sqlalchemy.orm import Session, joinedload
from models import engine, Book

with Session(engine) as session:
    # Зарежда Book + Author с 1 SQL LEFT OUTER JOIN заявка
    stmt = select(Book).options(joinedload(Book.author))
    books = session.scalars(stmt).all()

    for b in books:
        print(f"{b.title} -> {b.author.name}")
```

---

## 2. `SELECTINLOAD` (ЗА ONE-TO-MANY И MANY-TO-MANY — `WHERE IN`)

Когато зареждаш родител с много деца (напр. `Author` с много `books`), използвай **`selectinload`**:

```python
from sqlalchemy import select
from sqlalchemy.orm import Session, selectinload
from models import engine, Author

with Session(engine) as session:
    # Заявка 1: SELECT * FROM authors
    # Заявка 2: SELECT * FROM books WHERE books.author_id IN (1, 2, 3...)
    stmt = select(Author).options(selectinload(Author.books))
    authors = session.scalars(stmt).all()

    for author in authors:
        print(f"{author.name} има {len(author.books)} книги.")
```

---

## 3. ОГРАНИЧАВАНЕ НА КОЛОНИТЕ (`LOAD_ONLY`) И ЗАЩИТА ПРОТИВ N+1 (`RAISELOAD`)

SQLAlchemy има уникален инструмент за сигурност — **`raiseload("*")`**, който хвърля грешка веднага, ако някой програмист случайно достъпи незаредена релация в цикъл!

```python
from sqlalchemy import select
from sqlalchemy.orm import load_only, raiseload, joinedload

with Session(engine) as session:
    stmt = (
        select(Book)
        .options(
            load_only(Book.id, Book.title),   # Зарежда САМО id и title в RAM
            joinedload(Book.author),          # Зарежда автора предварително
            raiseload("*")                    # ГЪРМИ с грешка, ако се опиташ да пипнеш друга релация!
        )
    )
    books = session.scalars(stmt).all()
```

---

## 4. ОПТИМИЗАЦИЯ НА ЗАЯВКИ В NOSQL (MONGODB INDEXES, PROJECTIONS И REDIS CACHE)

В NoSQL базите няма SQL `JOIN`, затова оптимизацията се постига чрез **Индекси**, **Прожекции** (връщане само на нужните полета) и **Redis кеширане** на тежки JSON отговори:

```python
import json
import redis
from pydantic import BaseModel
from beanie import Document, Indexed

# 1. MongoDB (Beanie): Индексиране и Projection (извличане само на 2 полета)
class BookProjection(BaseModel):
    title: str
    price: float

class BookDoc(Document):
    title: Indexed(str)  # Създава B-Tree индекс в MongoDB за мигновено търсене
    price: float
    heavy_content: str

async def get_fast_books():
    # Извлича САМО 'title' и 'price' без тежкия 'heavy_content' (еквивалент на .only())
    return await BookDoc.find(BookDoc.price > 20).project(BookProjection).to_list()

# 2. Cache-Aside Pattern с Redis (предотвратява удрянето на SQL/MongoDB изобщо!)
r = redis.Redis(host="localhost", port=6379, decode_responses=True)

def get_cached_catalog(fetch_from_db_func):
    cached_json = r.get("catalog:top_books")
    if cached_json:
        return json.loads(cached_json)  # Връща от RAM за 0.2 ms!
    
    fresh_data = fetch_from_db_func()
    r.setex("catalog:top_books", 300, json.dumps(fresh_data))  # Кешира за 5 минути (300 сек.)
    return fresh_data
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Инструмент | Кога се използва | Брой SQL заявки |
| :--- | :--- | :--- |
| **`.options(joinedload(Model.parent))`** | За 1:1 и Many-to-One (1 родител) | **1 заявка** (`SQL JOIN`) |
| **`.options(selectinload(Model.children))`** | За 1:N и Many-to-Many (списък от деца) | **2 заявки** (`WHERE id IN (...)`) |
| **`.options(load_only(Model.id, ...))`** | Зарежда само посочените колони в RAM | **1 заявка** (пести памет) |
| **`.options(raiseload("*"))`** | Блокира скрития Lazy Loading с грешка | Защита против N+1 бъгове |
| **`session.scalars(select(Model.id))`** | Връща само плосък списък от стойности | **1 заявка** (без обекти) |
| **`.project(ShortModel)` (Beanie)** | Извлича само избрани полета от MongoDB | 1 NoSQL заявка (пести трафик) |

---
[⬅️ 04-05-01. Django ORM Syntax](04-05-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-00. Transactions and Async](04-06-00-Transactions-And-Async.md)
