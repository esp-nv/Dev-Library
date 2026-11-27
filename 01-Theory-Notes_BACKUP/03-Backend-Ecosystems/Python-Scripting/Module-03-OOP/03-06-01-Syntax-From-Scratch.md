[⬅️ 03-06-00. Теория & Капани](03-06-00-SOLID.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-07-00. Iterators and Generators](03-07-00-Iterators-And-Generators.md)

---

# 03-06-01. SOLID Principles: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за прилагане на всеки от 5-те SOLID принципа с чист Python код от нулата.

## 🧭 Съдържание на темата
* [1. S — Single Responsibility (Разделяне на класовете)](#1-s--single-responsibility-разделяне-на-класовете)
* [2. O — Open/Closed (Разширяване чрез наследяване)](#2-o--openclosed-разширяване-чрез-наследяване)
* [3. L — Liskov Substitution (Коректна йерархия)](#3-l--liskov-substitution-коректна-йерархия)
* [4. I & D — Interface Segregation и Dependency Injection](#4-i--d--interface-segregation-и-dependency-injection)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. S — SINGLE RESPONSIBILITY (РАЗДЕЛЯНЕ НА КЛАСОВЕТЕ)

```python
# Клас 1: Отговаря САМО за данните на книгата
class Book:
    def __init__(self, title: str, author: str):
        self.title = title
        self.author = author

# Клас 2: Отговаря САМО за запазването на книгата във файл
class BookRepository:
    def save_to_file(self, book: Book, filename: str):
        with open(filename, "w", encoding="utf-8") as f:
            f.write(f"{book.title} - {book.author}")
```

---

## 2. O — OPEN/CLOSED (РАЗШИРЯВАНЕ ЧРЕЗ НАСЛЕДЯВАНЕ)

```python
from abc import ABC, abstractmethod

class Discount(ABC):
    @abstractmethod
    def apply(self, price: float) -> float:
        pass

class RegularDiscount(Discount):
    def apply(self, price: float) -> float:
        return price * 0.95  # 5% отстъпка

class VipDiscount(Discount):
    def apply(self, price: float) -> float:
        return price * 0.80  # 20% отстъпка

# Ако утре добавим BlackFridayDiscount, просто правим нов клас без да пипаме старите!
```

---

## 3. L — LISKOV SUBSTITUTION (КОРЕКТНА ЙЕРАРХИЯ)

```python
from abc import ABC, abstractmethod

# Базов клас само с общите за ВСИЧКИ птици действия
class Bird(ABC):
    @abstractmethod
    def eat(self):
        pass

# Отделен абстрактен клас само за летящите птици
class FlyingBird(Bird):
    @abstractmethod
    def fly(self):
        pass

class Eagle(FlyingBird):
    def eat(self): print("Орелът яде.")
    def fly(self): print("Орелът лети високо.")

class Penguin(Bird):
    def eat(self): print("Пингвинът яде риба.")
    # Пингвинът НЕ наследява FlyingBird, затова не се налага да хвърля грешка в fly()!
```

---

## 4. I & D — INTERFACE SEGREGATION И DEPENDENCY INJECTION

```python
from abc import ABC, abstractmethod

# I: Малки, тясно специализирани договори (вместо един огромен)
class IPrintable(ABC):
    @abstractmethod
    def print_doc(self): pass

class IScannable(ABC):
    @abstractmethod
    def scan_doc(self): pass

# Обикновеният принтер имплементира само това, което може
class SimplePrinter(IPrintable):
    def print_doc(self):
        print("Печатане на документ...")

# D: Подаване на зависимостта отвън през конструктора (Dependency Injection)
class OfficeManager:
    def __init__(self, printer: IPrintable):
        self.printer = printer  # Зависи от абстракция, не от конкретен принтер!

    def execute_print(self):
        self.printer.print_doc()
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Буква | Принцип | Основно правило с 1 изречение | Как се постига в Python |
| :--- | :--- | :--- | :--- |
| **S** | **Single Responsibility** | 1 клас = 1 задача (1 причина за промяна) | Разбиване на големи класове на малки |
| **O** | **Open / Closed** | Добавяй нов код, не пренаписвай стария | `ABC` + нови наследници вместо `if/elif` |
| **L** | **Liskov Substitution** | Наследникът заменя родителя без сривове | Без `NotImplementedError` в дъщерни класове |
| **I** | **Interface Segregation** | Много малки интерфейси > 1 гигантски | Малки `ABC` или `Protocol` класове |
| **D** | **Dependency Inversion** | Зависи от абстракции, не от детайли | Подаване на `ABC` обекти в `__init__` |

---
[⬅️ 03-06-00. Теория & Капани](03-06-00-SOLID.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-07-00. Iterators and Generators](03-07-00-Iterators-And-Generators.md)
