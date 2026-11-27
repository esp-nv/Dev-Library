[⬅️ 03-04-00. Теория & Капани](03-04-00-Static-And-Class-Methods.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-05-00. Polymorphism and Abstraction](03-05-00-Polymorphism-And-Abstraction.md)

---

# 03-04-01. Static and Class Methods: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за писане и извикване на `@classmethod` и `@staticmethod` в Python от нулата.

## 🧭 Съдържание на темата
* [1. Синтаксис на `@staticmethod`](#1-синтаксис-на-staticmethod)
* [2. Синтаксис на `@classmethod` (Достъп до данни на класа)](#2-синтаксис-на-classmethod-достъп-до-данни-на-класа)
* [3. Синтаксис на Factory Method (Алтернативен конструктор)](#3-синтаксис-на-factory-method-алтернативен-конструктор)
* [4. Комбиниран пример в един клас](#4-комбиниран-пример-в-един-клас)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СИНТАКСИС НА `@STATICMETHOD`

Слага се декораторът `@staticmethod` над метода, а в скобите **няма** нито `self`, нито `cls`.

```python
class MathHelper:
    @staticmethod
    def add(a: int, b: int) -> int:
        return a + b

    @staticmethod
    def is_even(number: int) -> bool:
        return number % 2 == 0

# Извиква се директно през името на класа (без да създаваме обект с MathHelper())
print(MathHelper.add(10, 25))       # Изход: 35
print(MathHelper.is_even(14))       # Изход: True
```

---

## 2. СИНТАКСИС НА `@CLASSMETHOD` (ДОСТЪП ДО ДАННИ НА КЛАСА)

Слага се декораторът `@classmethod` над метода, а първият параметър в скобите **задължително е `cls`**.

```python
class ServerCluster:
    max_nodes = 10  # Атрибут на ниво клас

    @classmethod
    def update_capacity(cls, new_limit: int):
        # 'cls' сочи към самия клас ServerCluster
        cls.max_nodes = new_limit

ServerCluster.update_capacity(50)
print(ServerCluster.max_nodes)      # Изход: 50
```

---

## 3. СИНТАКСИС НА FACTORY METHOD (АЛТЕРНАТИВЕН КОНСТРУКТОР)

В Python можеш да имаш само един `__init__`. За да създаваш обекти от различни формати (например от стринг или речник), правиш `@classmethod`.

```python
class Date:
    def __init__(self, day: int, month: int, year: int):
        self.day = day
        self.month = month
        self.year = year

    # Алтернативен конструктор, който приема текст "11-11-2026"
    @classmethod
    def from_string(cls, date_text: str):
        day, month, year = [int(x) for x in date_text.split("-")]
        return cls(day, month, year)  # Създава и връща нов обект Date(11, 11, 2026)

# 1. Стандартно създаване чрез __init__
d1 = Date(11, 11, 2026)

# 2. Създаване чрез фабричния класов метод
d2 = Date.from_string("11-11-2026")
print(f"Година: {d2.year}")           # Изход: Година: 2026
```

---

## 4. КОМБИНИРАН ПРИМЕР В ЕДИН КЛАС

```python
class Order:
    vat_rate = 0.20

    def __init__(self, amount: float):
        self.amount = amount

    # Instance method -> работи с конкретната поръчка (self)
    def total_price(self) -> float:
        return self.amount * (1 + self.vat_rate)

    # Class method -> променя общото ДДС за всички поръчки (cls)
    @classmethod
    def set_vat(cls, new_vat: float):
        cls.vat_rate = new_vat

    # Static method -> чиста помощна проверка
    @staticmethod
    def is_valid_amount(value: float) -> bool:
        return value > 0
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Вид метод | Декоратор | Първи параметър | Какво може да достъпва |
| :--- | :--- | :--- | :--- |
| **Instance Method** | *Няма* | `self` | Полетата на обекта (`self.x`) и класа (`cls.y`) |
| **Class Method** | `@classmethod` | `cls` | Полетата на класа (`cls.y`), **НЕ** и на обекта |
| **Static Method** | `@staticmethod` | *Няма* | **Нищо** от класа или обекта (работи само с аргументите си) |
| **Алтернативен конструктор** | `@classmethod` | `cls` | Връща `cls(...)` за поддръжка на наследяване |

---
[⬅️ 03-04-00. Теория & Капани](03-04-00-Static-And-Class-Methods.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-05-00. Polymorphism and Abstraction](03-05-00-Polymorphism-And-Abstraction.md)
