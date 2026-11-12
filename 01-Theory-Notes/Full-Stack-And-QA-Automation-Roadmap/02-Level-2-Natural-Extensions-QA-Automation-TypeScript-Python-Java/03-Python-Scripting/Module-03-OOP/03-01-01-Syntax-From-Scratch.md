[⬅️ 03-01-00. Теория & Капани](03-01-00-Classes-And-Objects.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-02-00. Inheritance](03-02-00-Inheritance.md)

---

# 03-01-01. Classes and Objects: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на класове, инициализатори, методи, магически методи и инстанции в Python от нулата.

## 🧭 Съдържание на темата
* [1. Създаване на клас и инстанция (`__init__` и `self`)](#1-създаване-на-клас-и-инстанция-__init__-и-self)
* [2. Атрибути на класа срещу Атрибути на инстанцията](#2-атрибути-на-класа-срещу-атрибути-на-инстанцията)
* [3. Магически методи (`__str__`, `__repr__`, `__eq__`, `__len__`)](#3-магически-методи-__str__-__repr__-__eq__-__len__)
* [4. Вградени функции за инспекция на обекти (`getattr`, `setattr`, `hasattr`)](#4-вградени-функции-за-инспекция-на-обекти-getattr-setattr-hasattr)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА КЛАС И ИНСТАНЦИЯ (`__INIT__` И `SELF`)

Имената на класовете се пишат с Главна Буква на всяка дума (**`PascalCase`**).

```python
class BankAccount:
    # 1. Инициализатор (вика се автоматично при създаване на обект)
    def __init__(self, owner: str, balance: float = 0.0):
        self.owner = owner          # Записва името в конкретния обект
        self.balance = balance      # Записва баланса в конкретния обект

    # 2. Метод на обекта (първият параметър ВИНАГИ е self)
    def deposit(self, amount: float):
        if amount > 0:
            self.balance += amount
            print(f"Внесени {amount} лв. Нов баланс: {self.balance} лв.")

# 3. Създаване на обекти (инстанции) - БЕЗ ключовата дума 'new'!
acc1 = BankAccount("Елизабет", 100.0)
acc2 = BankAccount("Иван")          # Използва balance=0.0 по подразбиране

# Извикване на метод (Python подава 'acc1' на мястото на 'self' автоматично!)
acc1.deposit(50.0)
```

---

## 2. АТРИБУТИ НА КЛАСА СРЕЩУ АТРИБУТИ НА ИНСТАНЦИЯТА

```python
class Student:
    # Class Attribute -> Общ за всички студенти (дефиниран извън __init__)
    university = "SoftUni"

    def __init__(self, name: str):
        # Instance Attribute -> Уникален за всеки отделен студент
        self.name = name

s1 = Student("Мария")
s2 = Student("Георги")

print(s1.university)        # Изход: SoftUni
print(Student.university)   # Правилно четене директно през класа -> SoftUni
```

---

## 3. МАГИЧЕСКИ МЕТОДИ (`__STR__`, `__REPR__`, `__EQ__`, `__LEN__`)

```python
class Playlist:
    def __init__(self, name: str, songs: list):
        self.name = name
        self.songs = songs

    # Извиква се при print(my_playlist) или str(my_playlist)
    def __str__(self):
        return f"Плейлист '{self.name}' ({len(self.songs)} песни)"

    # Извиква се при len(my_playlist)
    def __len__(self):
        return len(self.songs)

    # Извиква се при сравнение: playlist1 == playlist2
    def __eq__(self, other):
        if not isinstance(other, Playlist):
            return False
        return self.name == other.name and self.songs == other.songs

pl = Playlist("Coding Focus", ["Track 1", "Track 2", "Track 3"])
print(pl)           # Изход: Плейлист 'Coding Focus' (3 песни)
print(len(pl))      # Изход: 3
```

---

## 4. ВГРАДЕНИ ФУНКЦИИ ЗА ИНСПЕКЦИЯ НА ОБЕКТИ (`GETATTR`, `SETATTR`, `HASATTR`)

```python
class Config:
    debug = True

cfg = Config()

# 1. Проверка дали обектът има такова поле
print(hasattr(cfg, "debug"))            # Изход: True

# 2. Четене на поле по име като стринг (с безопасна стойност по подразбиране)
port = getattr(cfg, "port", 8080)       # Изход: 8080

# 3. Динамично задаване на ново поле по име
setattr(cfg, "host", "localhost")
print(cfg.host)                         # Изход: localhost
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис | Какво прави |
| :--- | :--- | :--- |
| **Дефиниране на клас** | `class MyClass:` | Имената са в `PascalCase` |
| **Инициализатор** | `def __init__(self, x):` | Задава началните свойства на обекта |
| **Създаване на обект** | `obj = MyClass(10)` | Без думата `new` |
| **Форматиране при `print()`** | `def __str__(self):` | Еквивалент на `ToString()` в C# |
| **Сравнение с `==`** | `def __eq__(self, other):` | Дефинира кога два обекта са равни |
| **Поддръжка на `len()`** | `def __len__(self):` | Позволява `len(obj)` да връща число |
| **Всички полета на обекта** | `obj.__dict__` | Връща речник с полетата на инстанцията |
| **Оптимизация на паметта** | `__slots__ = ("a", "b")` | Спира създаването на `__dict__` и пести RAM |

---
[⬅️ 03-01-00. Теория & Капани](03-01-00-Classes-And-Objects.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-02-00. Inheritance](03-02-00-Inheritance.md)
