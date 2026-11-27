[⬅️ 04-03-01. Django ORM Syntax](04-03-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-00. Table Relationships](04-04-00-Relationships.md)

---

# 04-03-02. SQLAlchemy 2.0 CRUD: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване, четене (`select`), филтриране (`where`), обновяване и изтриване чрез `Session` в SQLAlchemy 2.0+ от нулата.

## 🧭 Съдържание на темата
* [1. CREATE (Добавяне чрез `Session` и `commit`)](#1-create-добавяне-чрез-session-и-commit)
* [2. READ (`select`, `where`, `scalars` и оператори)](#2-read-select-where-scalars-и-оператори)
* [3. Сложни условия (`and_`, `or_`, `not_`) и сортиране](#3-сложни-условия-and_-or_-not_-и-сортиране)
* [4. UPDATE и DELETE (Чрез обект и масови заявки)](#4-update-и-delete-чрез-обект-и-масови-заявки)
* [4.1. CRUD ОПЕРАЦИИ ПРИ ПРЕНОС В JSON И XML (СТЪПКА ПО СТЪПКА)](#41-crud-операции-при-пренос-в-json-и-xml-стъпка-по-стъпка)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. CREATE (ДОБАВЯНЕ ЧРЕЗ `SESSION` И `COMMIT`)

В SQLAlchemy всички операции минават през контекст мениджъра **`with Session(engine) as session:`**.

```python
from sqlalchemy.orm import Session
from models import engine, Product

with Session(engine) as session:
    # 1. Създаване на единичен обект
    p1 = Product(title="Мишка Logitech", price=89.90)
    session.add(p1)

    # 2. Добавяне на няколко обекта наведнъж
    p2 = Product(title="Клавиатура Keychron", price=199.00)
    p3 = Product(title="Монитор Dell", price=450.00)
    session.add_all([p2, p3])

    # 3. Изпращане на INSERT заявките към базата данни
    session.commit()
    print(f"Създадено ID: {p1.id}")  # След commit() полето p1.id се попълва автоматично!
```

---

## 2. READ (`SELECT`, `WHERE`, `SCALARS` И ОПЕРАТОРИ)

В SQLAlchemy 2.0 се строи заявка чрез функцията **`select(Модел)`** и се изпълнява през `session.scalars(stmt)`:

```python
from sqlalchemy import select
from sqlalchemy.orm import Session
from models import engine, Product

with Session(engine) as session:
    # 1. Взимане на ВСИЧКИ записи като списък от обекти
    stmt_all = select(Product)
    products = session.scalars(stmt_all).all()

    # 2. Взимане на 1 запис по Primary Key (най-бързият начин!)
    product_by_id = session.get(Product, 1)

    # 3. Взимане на първия съвпадащ запис (или None, ако няма такъв)
    stmt_one = select(Product).where(Product.title == "Монитор Dell")
    dell = session.scalars(stmt_one).first()

    # 4. Стандартни SQL оператори за филтриране:
    stmt_filters = select(Product).where(
        Product.price >= 100,                   # По-голямо или равно
        Product.title.ilike("%keychron%"),      # Case-insensitive LIKE '%keychron%'
        Product.id.in_()               # IN (1, 2, 5)
    )
    filtered_list = session.scalars(stmt_filters).all()
```

---

## 3. СЛОЖНИ УСЛОВИЯ (`AND_`, `OR_`, `NOT_`) И СОРТИРАНЕ

```python
from sqlalchemy import select, or_, and_, not_, desc

with Session(engine) as session:
    stmt = (
        select(Product)
        .where(
            and_(
                or_(Product.title.ilike("%Dell%"), Product.title.ilike("%Apple%")),
                not_(Product.price > 3000)
            )
        )
        .order_by(desc(Product.price), Product.title)  # Сортиране низходящо по цена
        .limit(5)                                      # Топ 5 резултата (LIMIT 5)
    )
    top_products = session.scalars(stmt).all()
```

---

## 4. UPDATE И DELETE (ЧРЕЗ ОБЕКТ И МАСОВИ ЗАЯВКИ)

```python
from sqlalchemy import update, delete

with Session(engine) as session:
    # 1. Обновяване на единичен обект (SQLAlchemy сам засича промяната - Dirty Tracking!)
    item = session.get(Product, 1)
    if item:
        item.price = 120.00
        session.commit()  # Генерира UPDATE само за променената колона!

    # 2. Масово обновяване директно в SQL (Bulk Update)
    stmt_upd = update(Product).where(Product.price < 20).values(price=25.00)
    session.execute(stmt_upd)

    # 3. Изтриване на единичен обект или масово изтриване
    if item:
        session.delete(item)
    
    stmt_del = delete(Product).where(Product.price == 0)
    session.execute(stmt_del)
    
    session.commit()
```
---
## 4.1. CRUD ОПЕРАЦИИ В NOSQL БАЗИ: MONGODB (BEANIE) И REDIS

```python
import json
import redis
from beanie import Document

# --- ЧАСТ А: MongoDB (Beanie ODM - асинхронни документни операции) ---
class ProductDoc(Document):
    name: str
    price: float

async def mongo_crud_demo():
    # 1. Create
    prod = ProductDoc(name="Монитор Dell", price=450.0)
    await prod.insert()

    # 2. Read (Филтриране)
    expensive = await ProductDoc.find(ProductDoc.price >= 100).to_list()

    # 3. Update
    await prod.set({ProductDoc.price: 420.0})

    # 4. Delete
    await prod.delete()

# --- ЧАСТ Б: Redis (In-Memory Key-Value CRUD с JSON) ---
r = redis.Redis(host="localhost", port=6379, decode_responses=True)

# 1. Create / Update
r.set("product:1", json.dumps({"id": 1, "name": "Клавиатура", "price": 199.0}))

# 2. Read
raw = r.get("product:1")
if raw:
    product_dict = json.loads(raw)

# 3. Delete
r.delete("product:1")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция | Синтаксис в SQLAlchemy 2.0 |
| :--- | :--- |
| **Добавяне на 1 / много обекти** | `session.add(obj)` / `session.add_all([o1, o2])` |
| **Записване в базата** | `session.commit()` |
| **Взимане по Primary Key** | `session.get(Model, id_val)` |
| **Всички записи** | `session.scalars(select(Model)).all()` |
| **Първи запис или `None`** | `session.scalars(select(Model).where(...)).first()` |
| **Търсене в текст (`ILIKE`)** | `Model.name.ilike("%text%")` |
| **Логическо ИЛИ / НЕ** | `or_(cond1, cond2)` / `not_(cond)` |
| **Сортиране и Лимит** | `.order_by(desc(Model.id)).limit(10)` |
| **Масов Update / Delete** | `session.execute(update(Model)...)` / `delete(Model)...` |

---
[⬅️ 04-03-01. Django ORM Syntax](04-03-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-00. Table Relationships](04-04-00-Relationships.md)
