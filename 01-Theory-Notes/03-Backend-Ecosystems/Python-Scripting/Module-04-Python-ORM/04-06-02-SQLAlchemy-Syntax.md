[⬅️ 04-06-01. Django ORM Syntax](04-06-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)

---

# 04-06-02. SQLAlchemy 2.0 Transactions, Raw SQL & Async: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `session.begin()`, `with_for_update()`, параметризиран `text()` SQL, `AsyncSession` и трансакции в NoSQL (MongoDB / Redis) от нулата [1.53].

## 🧭 Съдържание на темата
* [1. Атомарни трансакции с `session.begin()` и `with_for_update()`](#1-атомарни-трансакции-с-sessionbegin-и-with_for_update)
* [2. Безопасен Raw SQL с `text()` и именувани параметри](#2-безопасен-raw-sql-с-text-и-именувани-параметри)
* [3. Асинхронен SQLAlchemy (`create_async_engine` и `AsyncSession`)](#3-асинхронен-sqlalchemy-create_async_engine-и-asyncsession)
* [4. Трансакции в NoSQL бази: MongoDB (`start_transaction`) и Redis (`pipeline`)](#4-трансакции-в-nosql-бази-mongodb-start_transaction-и-redis-pipeline)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. АТОМАРНИ ТРАНСАКЦИИ С `SESSION.BEGIN()` И `WITH_FOR_UPDATE()`

Когато използваш `with Session(engine) as session, session.begin():`, SQLAlchemy автоматично прави `commit()` при успешно излизане от блока и автоматичен `rollback()` при грешка:

```python
from sqlalchemy import select
from sqlalchemy.orm import Session
from models import engine, Account

with Session(engine) as session:
    with session.begin():
        # Заключва реда за промяна (SELECT ... FOR UPDATE)
        stmt = select(Account).where(Account.id == 1).with_for_update()
        acc = session.scalars(stmt).one()
        
        acc.balance += 500
        # Не е нужно да пишеш session.commit() - блокът session.begin() го прави сам накрая!
```

---

## 2. БЕЗОПАСЕН RAW SQL С `TEXT()` И ИМЕНУВАНИ ПАРАМЕТРИ

В SQLAlchemy суровият SQL се обвива във функцията **`text()`**, а параметрите се пишат с двоеточие **`:име`** и се подават като речник:

```python
from sqlalchemy import text
from sqlalchemy.orm import Session

with Session(engine) as session:
    raw_sql = text("SELECT id, title, price FROM products WHERE price > :min_p AND title LIKE :pat")
    
    result = session.execute(raw_sql, {"min_p": 50, "pat": "%Pro%"})
    
    for row in result:
        print(f"ID: {row.id} | Име: {row.title} | Цена: {row.price}")
```

---

## 3. АСИНХРОНЕН SQLALCHEMY (`CREATE_ASYNC_ENGINE` И `ASYNCSESSION`)

Изисква асинхронен драйвер (напр. `pip install asyncpg` за PostgreSQL или `aiosqlite` за SQLite):

```python
from sqlalchemy import select
from sqlalchemy.ext.asyncio import create_async_engine, async_sessionmaker
from sqlalchemy.orm import selectinload

# 1. Асинхронен двигател и фабрика за сесии
async_engine = create_async_engine("sqlite+aiosqlite:///async_app.db")
AsyncSessionLocal = async_sessionmaker(async_engine, expire_on_commit=False)

async def get_author_with_books(author_id: int):
    async with AsyncSessionLocal() as session:
        # ЗАДЪЛЖИТЕЛНО ползваме selectinload/joinedload в Async режим, за да избегнем MissingGreenlet!
        stmt = select(Author).options(selectinload(Author.books)).where(Author.id == author_id)
        result = await session.scalars(stmt)
        return result.first()
```

---

## 4. ТРАНСАКЦИИ В NOSQL БАЗИ: MONGODB (`START_TRANSACTION`) И REDIS (`PIPELINE`)

```python
import redis
from motor.motor_asyncio import AsyncIOMotorClient

# --- ЧАСТ А: MongoDB Multi-Document ACID Transaction ---
async def mongo_transfer(client: AsyncIOMotorClient):
    async with await client.start_session() as s:
        async with s.start_transaction():
            db = client["bank_db"]
            await db.accounts.update_one({"_id": 1}, {"\$inc": {"balance": -100}}, session=s)
            await db.accounts.update_one({"_id": 2}, {"\$inc": {"balance": 100}}, session=s)
            # Ако възникне грешка тук, MongoDB отменя и двете операции!

# --- ЧАСТ Б: Redis Атомарен Pipeline (изпълнява група команди наведнъж) ---
r = redis.Redis(host="localhost", port=6379, decode_responses=True)

with r.pipeline(transaction=True) as pipe:
    pipe.decrby("user:1:balance", 100)
    pipe.incrby("user:2:balance", 100)
    pipe.execute()  # Изпраща и изпълнява двете команди атомарно!
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция | Синтаксис в SQLAlchemy 2.0 |
| :--- | :--- |
| **Автоматична трансакция** | `with Session(engine) as s, s.begin():` |
| **Заключване на ред (`FOR UPDATE`)** | `select(Model).where(...).with_for_update()` |
| **Безопасен Raw SQL** | `session.execute(text("... :param"), {"param": val})` |
| **Асинхронна сесия** | `async with AsyncSessionLocal() as session:` |
| **Асинхронно изпълнение** | `await session.scalars(select(Model))` |
| **NoSQL Атомарност (Redis)** | `with r.pipeline(transaction=True) as pipe: ... pipe.execute()` |

---
[⬅️ 04-06-01. Django ORM Syntax](04-06-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)
