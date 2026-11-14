[⬅️ 03-96. Python Оптимизации](03-96-Performance-And-Optimizations.md) | 📂 Папка: 03-Python-Scripting | [🏠 Начало](../README.md) | [➡️ 03-98. Common Errors And Fixes](03-98-Common-Errors-And-Fixes.md)

---

# 03-97. Python Category Cheat Sheet: Пълно обобщение по категории & Библиотеки

**Цел:** Бърза синтактична и архитектурна справка на едно място за всички **6 модула на Python 3.x+** (Basics, Fundamentals, Advanced, OOP, ORM/DB, Web/Django & FastAPI) и най-важните стандартни и външни библиотеки.

## 🧭 Съдържание по категории
* [1. Категория 1: Базови типове и вградени колекции (`list`, `tuple`, `set`, `dict`)](#1-категория-1-базови-типове-и-вградени-колекции-list-tuple-set-dict)
* [2. Категория 2: Comprehensions, Генератори и Вградени функционални инструменти](#2-категория-2-comprehensions-генератори-и-вградени-функционални-инструменти)
* [3. Категория 3: Функции (`*args`, `**kwargs`), Декоратори и Context Managers (`with`)](#3-категория-3-функции-args-kwargs-декоратори-и-context-managers-with)
* [4. Категория 4: ООП, `@dataclass`, Свойства (`@property`) и Dunder методи](#4-категория-4-ооп-dataclass-свойства-property-и-dunder-методи)
* [5. Категория 5: Важни библиотеки (Стандартни & Външни — Импорт, Извикване и Приложение)](#5-категория-5-важни-библиотеки-стандартни--външни--импорт-извикване-и-приложение)
* [6. Категория 6: Python ORM & Web Шаблони (SQLAlchemy / Django / FastAPI)](#6-категория-6-python-orm--web-шаблони-sqlalchemy--django--fastapi)

---

## 1. КАТЕГОРИЯ 1: БАЗОВИ ТИПОВЕ И ВГРАДЕНИ КОЛЕКЦИИ (`LIST`, `TUPLE`, `SET`, `DICT`)

| Колекция | Синтаксис и Характеристика | Най-важни методи и операции |
| :--- | :--- | :--- |
| **`list`** | `[1, 2, 3]` — Подреден, изменяем (mutable), допуска дубликати | `.append(x)`, `.extend(it)`, `.pop()`, `.sort(key=...)`, `slicing [1:4:2]` |
| **`tuple`** | `(10, 20)` — Подреден, **неизменяем (immutable)**, по-бърз и лек от `list` | Unpacking: `x, y = point`; връщане на множество стойности от функция |
| **`set`** | `{1, 2, 3}` — Неподреден, **само уникални елементи**, \(O(1)\) търсене | `.add(x)`, `.discard(x)`, Обединение `a \| b`, Сечение `a & b`, Разлика `a - b` |
| **`dict`** | `{"id": 1, "role": "ADMIN"}` — Ключ-стойност (пази реда на вмъкване от Python 3.7+) | `.get("key", default)`, `.items()`, `.keys()`, `.values()`, сливане `d1 \| d2` |
| **`str`** | `"Hello"` — Неизменяем Unicode низ | `f"{var:.2f}"`, `.strip()`, `.split(",")`, `",".join(list)`, `.startswith()` |

---

## 2. КАТЕГОРИЯ 2: COMPREHENSIONS, ГЕНЕРАТОРИ И ВГРАДЕНИ ФУНКЦИОНАЛНИ ИНСТРУМЕНТИ

```python
nums = [1, 2, 3, 4, 5, 6]

# 1. List / Set / Dict Comprehensions
evens_squared = [x ** 2 for x in nums if x % 2 == 0]        # [4, 16, 36]
unique_lengths = {len(w) for w in ["ana", "ivan", "petar"]} # {3, 4, 5}
id_map = {n: f"User_{n}" for n in nums}                     # {1: 'User_1', ...}

# 2. Вградени итератори (enumerate & zip)
for idx, val in enumerate(["a", "b"], start=1):
    print(f"{idx}: {val}")

names = ["Ana", "Boris"]
scores = [95, 88]
paired = dict(zip(names, scores))  # {'Ana': 95, 'Boris': 88}

# 3. Агрегации и проверки
has_big = any(x > 5 for x in nums)  # True (спира на първото съвпадение!)
all_pos = all(x > 0 for x in nums)  # True
```

---

## 3. КАТЕГОРИЯ 3: ФУНКЦИИ (`*ARGS`, `**KWARGS`), ДЕКОРАТОРИ И CONTEXT MANAGERS (`WITH`)

```python
from functools import wraps
from typing import Callable, Any

# 1. Гъвкави аргументи (*args = tuple, **kwargs = dict)
def audit_log(action: str, *tags: str, **metadata: Any) -> None:
    print(action, tags, metadata)

# 2. Професионален декоратор (запазва името и docstring-а чрез @wraps)
def log_call(func: Callable) -> Callable:
    @wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        print(f"Извикване на {func.__name__}...")
        return func(*args, **kwargs)
    return wrapper

# 3. Сигурна работа с файлове и ресурси чрез Context Manager (автоматичен close())
with open("report.txt", mode="w", encoding="utf-8") as file:
    file.write("Готов отчет\n")
```

---

---

## 4. КАТЕГОРИЯ 4: ООП, `@DATACLASS`, СВОЙСТВА (`@PROPERTY`) И DUNDER МЕТОДИ

```python
from dataclasses import dataclass, field

# 1. Модерен Data DTO клас (автоматично генерира __init__, __repr__, __eq__)
@dataclass(frozen=True)  # frozen=True го прави неизменяем (като Java record / C# record)
class ProductDto:
    id: int
    name: str
    price: float
    tags: list[str] = field(default_factory=list)

# 2. Класически клас с капсулирано свойство (@property) и Dunder методи
class BankAccount:
    def __init__(self, owner: str, balance: float = 0.0) -> None:
        self.owner = owner
        self._balance = balance

    @property
    def balance(self) -> float:
        return self._balance

    @balance.setter
    def balance(self, value: float) -> None:
        if value < 0:
            raise ValueError("Балансът не може да е отрицателен!")
        self._balance = value

    def __repr__(self) -> str:
        return f"BankAccount(owner={self.owner!r}, balance={self._balance:.2f})"
```

---

## 5. КАТЕГОРИЯ 5: ВАЖНИ БИБЛИОТЕКИ (СТАНДАРТНИ & ВЪНШНИ — ИМПОРТ, ИЗВИКВАНЕ И ПРИЛОЖЕНИЕ)

| Библиотека | Инсталация | Импорт и Извикване (Синтаксис) | За какво се използва |
| :--- | :--- | :--- | :--- |
| **`pathlib`** | Вградена (StdLib) | `from pathlib import Path`<br>`data = Path("data/users.txt").read_text("utf-8")` | Модерна обектно-ориентирана работа с файлови пътища, директории и четене/писане на файлове. |
| **`datetime`** | Вградена (StdLib) | `from datetime import datetime, timezone`<br>`now = datetime.now(timezone.utc)` | Работа с дати, часови зони (UTC), парсване (`strptime`) и форматиране (`strftime`). |
| **`json`** | Вградена (StdLib) | `import json`<br>`raw = json.dumps({"id": 1}); obj = json.loads(raw)` | Сериализация и десериализация на JSON низове и файлове (`json.load` / `json.dump`). |
| **`collections`** | Вградена (StdLib) | `from collections import Counter, defaultdict`<br>`counts = Counter(["a", "b", "a"])` | Специализирани структури: честотен брояч (`Counter`), речник по подразбиране (`defaultdict`) и опашка (`deque`). |
| **`asyncio`** | Вградена (StdLib) | `import asyncio`<br>`await asyncio.gather(task1(), task2())` | Асинхронно конкурентно програмиране (`async` / `await`) за мрежови I/O операции. |
| **`httpx`** / `requests` | `pip install httpx` | `import httpx`<br>`res = httpx.get("https://example.com")` | Изпращане на синхронни и асинхронни HTTP заявки към REST API-та (наследник на `requests`). |
| **`pydantic`** | `pip install pydantic` | `from pydantic import BaseModel, EmailStr`<br>`class User(BaseModel): email: EmailStr` | Строга валидация на данни и парсване на JSON схеми по време на изпълнение (ядрото на FastAPI). |
| **`sqlalchemy`** | `pip install sqlalchemy` | `from sqlalchemy.orm import Session`<br>`users = session.scalars(select(User)).all()` | Най-мощният индустриален ORM и SQL toolkit за Python (PostgreSQL, MySQL, SQLite). |
| **`pytest`** | `pip install pytest` | `import pytest`<br>`assert add(2, 3) == 5` | Златен стандарт за писане на Unit, Integration и API тестове с fixtures и параметризация. |

---

## 6. КАТЕГОРИЯ 6: PYTHON ORM & WEB ШАБЛОНИ (SQLALCHEMY / DJANGO / FASTAPI)

```python
# --- 1. FastAPI + Pydantic (Модерен асинхронен REST API ендпойнт) ---
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

class CreateBookDto(BaseModel):
    title: str
    price: float

@app.post("/api/books", status_code=201)
async def create_book(dto: CreateBookDto):
    if dto.price <= 0:
        raise HTTPException(status_code=400, detail="Цената трябва да е положителна")
    return {"id": 1, "title": dto.title, "price": dto.price}

# --- 2. Django ORM (Оптимизирана заявка без N+1 проблем) ---
# orders = Order.objects.select_related("customer").prefetch_related("items").filter(status="PAID")
```

---
[⬅️ 03-96. Python Оптимизации](03-96-Performance-And-Optimizations.md) | 📂 Папка: 03-Python-Scripting | [🏠 Начало](../README.md) | [➡️ 03-98. Common Errors And Fixes](03-98-Common-Errors-And-Fixes.md)
