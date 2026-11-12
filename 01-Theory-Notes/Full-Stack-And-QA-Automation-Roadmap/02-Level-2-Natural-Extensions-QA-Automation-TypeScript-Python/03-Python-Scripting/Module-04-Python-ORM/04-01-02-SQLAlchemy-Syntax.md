[⬅️ 04-01-01. Django ORM Syntax](04-01-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-02-00. Migrations](04-02-00-Migrations.md)

---

# 04-01-02. SQLAlchemy 2.0 Models: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на модели чрез модерния декларативен стил (`DeclarativeBase`, `Mapped`, `mapped_column`) в SQLAlchemy 2.0+ от нулата.

## 🧭 Съдържание на темата
* [1. Основен скелет на SQLAlchemy 2.0 модел](#1-основен-скелет-на-sqlalchemy-20-модел)
* [2. Типове колони и Type Annotations (`Mapped`)](#2-типове-колони-и-type-annotations-mapped)
* [3. Ограничения, индекси и стойности по подразбиране](#3-ограничения-индекси-и-стойности-по-подразбиране)
* [4. Създаване на таблиците чрез `Engine` и `Metadata`](#4-създаване-на-таблиците-чрез-engine-и-metadata)
* [4.1. ПРЕОБРАЗУВАНЕ В JSON, XML И NOSQL (MONGODB) СТЪПКА ПО СТЪПКА](#41-преобразуване-в-json-xml-и-nosql-mongodb-стъпка-по-стъпка)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОСНОВЕН СКЕЛЕТ НА SQLALCHEMY 2.0 МОДЕЛ

В SQLAlchemy 2.0 всички модели наследяват общ базов клас `Base`, който наследява **`DeclarativeBase`**. Името на таблицата (`__tablename__`) и първичният ключ (`primary_key=True`) се пишат **задължително**.

```python
from sqlalchemy import String
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column

# 1. Базов клас, който събира метаданните на всички таблици
class Base(DeclarativeBase):
    pass

# 2. Дефиниране на таблицата 'students'
class Student(Base):
    __tablename__ = "students"

    id: Mapped[int] = mapped_column(primary_key=True, autoincrement=True)
    first_name: Mapped[str] = mapped_column(String(50))
    last_name: Mapped[str] = mapped_column(String(50))
    email: Mapped[str] = mapped_column(String(120), unique=True)
    is_active: Mapped[bool] = mapped_column(default=True)

    def __repr__(self) -> str:
        return f"Student(id={self.id}, email={self.email!r})"
```

---

## 2. ТИПОВЕ КОЛОНИ И TYPE ANNOTATIONS (`MAPPED`)

В SQLAlchemy 2.0 типът на колоната се определя от комбинацията между Python типа в `Mapped[...]` и SQL типа в `mapped_column(...)`:

```python
from datetime import datetime
from decimal import Decimal
from typing import Optional
from sqlalchemy import String, Text, Numeric, DateTime, func
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column

class Base(DeclarativeBase):
    pass

class Product(Base):
    __tablename__ = "products"

    id: Mapped[int] = mapped_column(primary_key=True)
    
    # VARCHAR(150) NOT NULL
    title: Mapped[str] = mapped_column(String(150))
    
    # TEXT NULL (Optional[str] или str | None автоматично прави колоната nullable=True!)
    description: Mapped[Optional[str]] = mapped_column(Text)
    
    # DECIMAL(10, 2) NOT NULL
    price: Mapped[Decimal] = mapped_column(Numeric(10, 2))
    
    # TIMESTAMP със сървърна стойност по подразбиране NOW()
    created_at: Mapped[datetime] = mapped_column(
        DateTime(timezone=True),
        server_default=func.now()
    )
```

---

## 3. ОГРАНИЧЕНИЯ, ИНДЕКСИ И СТОЙНОСТИ ПО ПОДРАЗБИРАНЕ

```python
from sqlalchemy import String, CheckConstraint, UniqueConstraint
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column

class Base(DeclarativeBase):
    pass

class Employee(Base):
    __tablename__ = "employees"

    id: Mapped[int] = mapped_column(primary_key=True)
    
    # index=True създава SQL индекс за бързо търсене
    badge_number: Mapped[str] = mapped_column(String(20), unique=True, index=True)
    salary: Mapped[int] = mapped_column(default=1000)

    # Допълнителни таблични ограничения (на ниво база данни)
    __table_args__ = (
        CheckConstraint("salary >= 800", name="check_min_salary"),
    )
```

---

## 4. СЪЗДАВАНЕ НА ТАБЛИЦИТЕ ЧРЕЗ `ENGINE` И `METADATA`

За разлика от Django, SQLAlchemy не чете настройки от `settings.py`, а се свързва чрез обект **`Engine`**:

```python
from sqlalchemy import create_engine

# 1. Създаване на връзка към базата (SQLite файл или PostgreSQL URL)
engine = create_engine("sqlite:///company.db", echo=True)  # echo=True принтира генерирания SQL!

# 2. Генериране на всички таблици в базата, които наследяват Base
Base.metadata.create_all(engine)
```
## 4.1. ПРЕОБРАЗУВАНЕ В JSON, XML И NOSQL (MONGODB) СТЪПКА ПО СТЪПКА

В екосистемата на SQLAlchemy и FastAPI за сериализация и NoSQL бази данни се използват библиотеките **Pydantic**, **Beanie (MongoDB)** и **xmltodict**:

```python
import json
import xml.etree.ElementTree as ET
from pydantic import BaseModel

# --- ЧАСТ А: Преобразуване в JSON и XML ---
class CourseSchema(BaseModel):
    id: int
    title: str
    tags: list[str]

# Стъпка 1: Създаване на обект в паметта
course = CourseSchema(id=10, title="Python ORM", tags=["SQL", "NoSQL"])

# Стъпка 2: Директно превръщане в JSON низ (чрез Pydantic)
json_str = course.model_dump_json(indent=2)
print("JSON изход:\n", json_str)

# Стъпка 3: Превръщане в XML дърво (чрез вградения модул ElementTree)
root = ET.Element("Course", id=str(course.id))
title_el = ET.SubElement(root, "Title")
title_el.text = course.title
xml_str = ET.tostring(root, encoding="unicode")
print("XML изход:\n", xml_str)
# Изход: <Course id="10"><Title>Python ORM</Title></Course>

# --- ЧАСТ Б: Как изглежда същият модел за NoSQL база (MongoDB с Beanie ODM) ---
# Вместо SQL таблици и миграции, тук наследяваме Document:
"""
from beanie import Document

class CourseMongoDocument(Document):
    title: str
    tags: list[str]  # Списъците се пазят директно в документа без втора таблица!

    class Settings:
        name = "courses_collection"  # Име на колекцията в MongoDB
"""
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Поле / Настройка | Синтаксис в SQLAlchemy 2.0 | SQL Еквивалент |
| :--- | :--- | :--- |
| **Базов клас** | `class Base(DeclarativeBase): pass` | Съхранява схемата (`Base.metadata`) |
| **Име на таблица** | `__tablename__ = "users"` | `CREATE TABLE users` |
| **Първичен ключ** | `id: Mapped[int] = mapped_column(primary_key=True)` | `INTEGER PRIMARY KEY` |
| **Кратък текст** | `name: Mapped[str] = mapped_column(String(100))` | `VARCHAR(100) NOT NULL` |
| **Опционално поле (`NULL`)** | `bio: Mapped[str \| None] = mapped_column(Text)` | `TEXT NULL` |
| **Парична сума** | `price: Mapped[Decimal] = mapped_column(Numeric(10, 2))` | `DECIMAL(10, 2)` |
| **Дата от сървъра** | `mapped_column(server_default=func.now())` | `DEFAULT CURRENT_TIMESTAMP` |
| **Уникалност и Индекс** | `mapped_column(unique=True, index=True)` | `UNIQUE` + `CREATE INDEX` |

---
[⬅️ 04-01-01. Django ORM Syntax](04-01-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-02-00. Migrations](04-02-00-Migrations.md)
