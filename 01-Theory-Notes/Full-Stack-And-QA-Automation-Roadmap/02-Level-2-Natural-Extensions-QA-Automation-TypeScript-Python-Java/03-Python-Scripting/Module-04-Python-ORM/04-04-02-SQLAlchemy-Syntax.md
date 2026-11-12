[⬅️ 04-04-01. Django ORM Syntax](04-04-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-05-00. Query Optimization](04-05-00-Optimization-And-N-Plus-1.md)

---

# 04-04-02. SQLAlchemy 2.0 Relationships: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `ForeignKey`, `relationship`, `back_populates`, Many-to-Many таблици в SQLAlchemy 2.0+ и сравнение с NoSQL MongoDB релации от нулата.

## 🧭 Съдържание на темата
* [1. One-to-Many (1:N) и One-to-One (1:1) с `relationship()`](#1-one-to-many-1n-и-one-to-one-11-с-relationship)
* [2. Many-to-Many (M:N) със свързваща таблица (`Table`)](#2-many-to-many-mn-със-свързваща-таблица-table)
* [3. Работа със свързани обекти в `Session`](#3-работа-със-свързани-обекти-в-session)
* [4. Релации в NoSQL (MongoDB Beanie): Embedding срещу Link Referencing](#4-релации-в-nosql-mongodb-beanie-embedding-срещу-link-referencing)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ONE-TO-MANY (1:N) И ONE-TO-ONE (1:1) С `RELATIONSHIP()`

В SQLAlchemy 2.0 връзката се описва двустранно: физическата колона (`ForeignKey`) стои при детето, а Python навигацията (`relationship` + `back_populates`) се пише и в двата класа.

```python
from typing import List
from sqlalchemy import ForeignKey, String
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, relationship

class Base(DeclarativeBase):
    pass

class Author(Base):
    __tablename__ = "authors"

    id: Mapped[int] = mapped_column(primary_key=True)
    name: Mapped[str] = mapped_column(String(100))

    # 1:N навигация към книгите (back_populates сочи към името на атрибута в Book)
    books: Mapped[List["Book"]] = relationship(
        back_populates="author",
        cascade="all, delete-orphan"
    )

class Book(Base):
    __tablename__ = "books"

    id: Mapped[int] = mapped_column(primary_key=True)
    title: Mapped[str] = mapped_column(String(150))
    
    # Физическият SQL Foreign Key
    author_id: Mapped[int] = mapped_column(ForeignKey("authors.id"))
    
    # Обратна навигация към автора
    author: Mapped["Author"] = relationship(back_populates="books")

# За ONE-TO-ONE (1:1): Слагаш unique=True на ForeignKey и пишеш Mapped["Profile"] (без List!)
```

---

## 2. MANY-TO-MANY (M:N) СЪС СВЪРЗВАЩА ТАБЛИЦА (`TABLE`)

```python
from sqlalchemy import Table, Column, Integer, ForeignKey, String
from sqlalchemy.orm import Mapped, mapped_column, relationship

# 1. Дефиниране на помощната свързваща таблица
student_course_table = Table(
    "student_courses",
    Base.metadata,
    Column("student_id", Integer, ForeignKey("students.id"), primary_key=True),
    Column("course_id", Integer, ForeignKey("courses.id"), primary_key=True),
)

class Student(Base):
    __tablename__ = "students"
    id: Mapped[int] = mapped_column(primary_key=True)
    name: Mapped[str] = mapped_column(String(50))
    courses: Mapped[List["Course"]] = relationship(
        secondary=student_course_table,
        back_populates="students"
    )

class Course(Base):
    __tablename__ = "courses"
    id: Mapped[int] = mapped_column(primary_key=True)
    title: Mapped[str] = mapped_column(String(100))
    students: Mapped[List["Student"]] = relationship(
        secondary=student_course_table,
        back_populates="courses"
    )
```

---

## 3. РАБОТА СЪС СВЪРЗАНИ ОБЕКТИ В `SESSION`

В SQLAlchemy свързаните колекции се държат точно като обикновени Python списъци (`.append()`):

```python
from sqlalchemy.orm import Session

with Session(engine) as session:
    author = Author(name="Иван Вазов")
    
    # Директно добавяне в списъка books (SQLAlchemy сам попълва author_id!)
    author.books.append(Book(title="Под игото"))
    author.books.append(Book(title="Немили-недраги"))

    session.add(author)
    session.commit()
```

---

## 4. РЕЛАЦИИ В NOSQL (MONGODB BEANIE): EMBEDDING СРЕЩУ LINK REFERENCING

В NoSQL базите имаш два избора при моделиране на връзки:

```python
from pydantic import BaseModel
from beanie import Document, Link

# Вариант 1: EMBEDDING (Вграждане - препоръчително за 1:Few връзки, напр. Адрес в Потребител)
class EmbeddedBook(BaseModel):
    title: str
    year: int

class AuthorEmbeddedDoc(Document):
    name: str
    books: list[EmbeddedBook] = []  # Книгите живеят вътре в документа на автора (0 JOIN-а!)

# Вариант 2: REFERENCING чрез Link (Като ForeignKey - за големи колекции)
class BookRefDoc(Document):
    title: str

class AuthorRefDoc(Document):
    name: str
    favorite_book: Link[BookRefDoc]  # Пази референция (DBRef) към друг документ
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Релация / Операция | Синтаксис в SQLAlchemy 2.0 |
| :--- | :--- |
| **Физически външен ключ** | `author_id: Mapped[int] = mapped_column(ForeignKey("authors.id"))` |
| **1:N връзка (Родител ➔ Деца)** | `books: Mapped[List["Book"]] = relationship(back_populates="author")` |
| **1:1 връзка** | `ForeignKey("users.id", unique=True)` + `profile: Mapped["Profile"]` |
| **M:N връзка** | `relationship(secondary=assoc_table, back_populates="...")` |
| **Каскадно изтриване** | `cascade="all, delete-orphan"` вътре в `relationship()` |
| **Добавяне на дете към родител** | `parent.children.append(child_obj)` |
| **NoSQL Вграждане (MongoDB)** | `items: list[SubModel] = []` вътре в `Document` |

---
[⬅️ 04-04-01. Django ORM Syntax](04-04-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-05-00. Query Optimization](04-05-00-Optimization-And-N-Plus-1.md)
