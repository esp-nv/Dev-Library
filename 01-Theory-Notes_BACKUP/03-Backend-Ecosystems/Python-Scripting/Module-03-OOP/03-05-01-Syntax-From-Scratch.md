[⬅️ 03-05-00. Теория & Капани](03-05-00-Polymorphism-And-Abstraction.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-06-00. SOLID Principles](03-06-00-SOLID.md)

---

# 03-05-01. Polymorphism and Abstraction: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за абстрактни класове (`ABC`), абстрактни методи, абстрактни пропъртита, Duck Typing и операторно предефиниране в Python.

## 🧭 Съдържание на темата
* [1. Създаване на Абстрактен клас (`ABC` и `@abstractmethod`)](#1-създаване-на-абстрактен-клас-abc-и-abstractmethod)
* [2. Абстрактно свойство (`@property` + `@abstractmethod`)](#2-абстрактно-свойство-property--abstractmethod)
* [3. Полиморфизъм чрез Duck Typing (без общ родител)](#3-полиморфизъм-чрез-duck-typing-без-общ-родител)
* [4. Полиморфизъм чрез оператори (`__add__`, `__gt__`)](#4-полиморфизъм-чрез-оператори-__add__-__gt__)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА АБСТРАКТЕН КЛАС (`ABC` И `@ABSTRACTMETHOD`)

```python
from abc import ABC, abstractmethod

# 1. Класът задължително наследява ABC
class Shape(ABC):
    # 2. Всеки задължителен метод получава декоратор @abstractmethod
    @abstractmethod
    def area(self) -> float:
        pass

    # Може да има и обикновени готови методи, които наследниците получават наготово
    def describe(self):
        print(f"Лицето на фигурата е: {self.area():.2f}")

# 3. Конкретен клас, който изпълнява договора
class Rectangle(Shape):
    def __init__(self, width: float, height: float):
        self.width = width
        self.height = height

    def area(self) -> float:
        return self.width * self.height

rect = Rectangle(4, 5)
rect.describe()             # Изход: Лицето на фигурата е: 20.00
```

---

## 2. АБСТРАКТНО СВОЙСТВО (`@PROPERTY` + `@ABSTRACTMETHOD`)

Внимание: Когато комбинираш двата декоратора, **`@property` винаги стои НАЙ-ОТГОРЕ**, а `@abstractmethod` веднага под него!

```python
from abc import ABC, abstractmethod

class Vehicle(ABC):
    @property
    @abstractmethod
    def max_speed(self) -> int:
        pass

class SportsCar(Vehicle):
    @property
    def max_speed(self) -> int:
        return 320

car = SportsCar()
print(car.max_speed)        # Изход: 320
```

---

## 3. ПОЛИМОРФИЗЪМ ЧРЕЗ DUCK TYPING (БЕЗ ОБЩ РОДИТЕЛ)

```python
class FileLogger:
    def write_log(self, msg: str):
        print(f"[Файл] {msg}")

class ConsoleLogger:
    def write_log(self, msg: str):
        print(f"[Конзола] {msg}")

# Тази функция приема ВСЕКИ обект, който има метод .write_log()
def save_event(logger, text: str):
    logger.write_log(text)

save_event(FileLogger(), "Старт на сървъра")
save_event(ConsoleLogger(), "Грешка в заявката")
```

---

## 4. ПОЛИМОРФИЗЪМ ЧРЕЗ ОПЕРАТОРИ (`__ADD__`, `__GT__`)

```python
class Wallet:
    def __init__(self, amount: float):
        self.amount = amount

    # Позволява събиране на два портфейла със знака + (wallet1 + wallet2)
    def __add__(self, other):
        return Wallet(self.amount + other.amount)

    # Позволява сравнение със знака > (wallet1 > wallet2)
    def __gt__(self, other) -> bool:
        return self.amount > other.amount

w1 = Wallet(100)
w2 = Wallet(250)
w3 = w1 + w2

print(w3.amount)            # Изход: 350
print(w2 > w1)              # Изход: True
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис | Какво прави |
| :--- | :--- | :--- |
| **Импортиране за абстракция** | `from abc import ABC, abstractmethod` | Вграден модул за абстрактни класове |
| **Абстрактен клас** | `class Base(ABC):` | Забранява директно създаване `Base()` |
| **Абстрактен метод** | `@abstractmethod \n def run(self): pass` | Задължава наследниците да го напишат |
| **Абстрактно пропърти** | `@property \n @abstractmethod` | `@property` задължително е отгоре |
| **Събиране на обекти (`+`)** | `def __add__(self, other):` | Дефинира поведението на оператора `+` |
| **Сравнение (`>` / `<`)** | `def __gt__(self, other):` / `__lt__` | Дефинира поведението на `>` и `<` |

---
[⬅️ 03-05-00. Теория & Капани](03-05-00-Polymorphism-And-Abstraction.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-06-00. SOLID Principles](03-06-00-SOLID.md)
