[⬅️ 03-02-00. Теория & Капани](03-02-00-Inheritance.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-03-00. Encapsulation](03-03-00-Encapsulation.md)

---

# 03-02-01. Inheritance: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за наследяване на класове, използване на `super()`, множествено наследяване и проверка на йерархии в Python от нулата.

## 🧭 Съдържание на темата
* [1. Единично наследяване (Single Inheritance)](#1-единично-наследяване-single-inheritance)
* [2. Пренаписване на методи и `super()`](#2-пренаписване-на-методи-и-super)
* [3. Множествено наследяване (Multiple Inheritance) и MRO](#3-множествено-наследяване-multiple-inheritance-и-mro)
* [4. Проверка на класове и инстанции (`isinstance` и `issubclass`)](#4-проверка-на-класове-и-инстанции-isinstance-и-issubclass)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ЕДИНИЧНО НАСЛЕДЯВАНЕ (SINGLE INHERITANCE)

Името на родителския клас се слага в **кръгли скоби `()`** веднага след името на новия клас.

```python
# Родителски (Базов) клас
class Animal:
    def __init__(self, name: str):
        self.name = name

    def eat(self):
        print(f"{self.name} се храни.")

# Дъщерен клас (наследява всичко от Animal)
class Dog(Animal):
    def bark(self):
        print(f"{self.name} казва: Бау!")

dog = Dog("Рекс")
dog.eat()   # Метод, наследен от Animal -> Рекс се храни.
dog.bark()  # Собствен метод на Dog -> Рекс казва: Бау!
```

---

## 2. ПРЕНАПИСВАНЕ НА МЕТОДИ И `SUPER()`

```python
class Vehicle:
    def __init__(self, brand: str, speed: int):
        self.brand = brand
        self.speed = speed

    def start(self):
        print(f"{self.brand} стартира двигателя.")

class ElectricCar(Vehicle):
    def __init__(self, brand: str, speed: int, battery_kwh: int):
        # 1. Викаме конструктора на родителя за общите полета
        super().__init__(brand, speed)
        # 2. Добавяме специфичното ново поле за детето
        self.battery_kwh = battery_kwh

    # Пренаписване (Overriding) на метод от родителя
    def start(self):
        super().start()  # По желание можем да изпълним и стария код на родителя
        print(f"Батерията ({self.battery_kwh} kWh) е активна!")

tesla = ElectricCar("Tesla", 250, 100)
tesla.start()
```

---

## 3. МНОЖЕСТВЕНО НАСЛЕДЯВАНЕ (MULTIPLE INHERITANCE) И MRO

```python
class Flyer:
    def fly(self):
        print("Лети във въздуха.")

class Swimmer:
    def swim(self):
        print("Плува във водата.")

# Класът Duck наследява и от двата класа едновременно
class Duck(Flyer, Swimmer):
    pass

donald = Duck()
donald.fly()    # Лети във въздуха.
donald.swim()   # Плува във водата.

# Проверка на реда, по който Python търси методите (MRO)
print(Duck.mro())
```

---

## 4. ПРОВЕРКА НА КЛАСОВЕ И ИНСТАНЦИИ (`ISINSTANCE` И `ISSUBCLASS`)

```python
# 1. isinstance(обект, Клас) -> Проверява дали обектът е от този клас или негов наследник
print(isinstance(donald, Duck))     # True
print(isinstance(donald, Flyer))    # True (защото Duck наследява Flyer)

# 2. issubclass(Клас1, Клас2) -> Проверява дали Клас1 наследява Клас2
print(issubclass(Duck, Swimmer))    # True
print(issubclass(Flyer, Swimmer))   # False
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис | Бележка / Пример |
| :--- | :--- | :--- |
| **Наследяване на 1 клас** | `class Child(Parent):` | Детето взима всички методи и полета |
| **Наследяване на много класове** | `class Child(P1, P2):` | Търси методите отляво надясно (`P1`, после `P2`) |
| **Викане на родителски `__init__`** | `super().__init__(arg1)` | Задължително при добавяне на нови полета |
| **Пренаписване на метод** | `def parent_method(self):` | Детето заменя поведението на родителя |
| **Проверка на ред на търсене** | `Child.mro()` или `Child.__mro__` | Показва списъка на наследяване |
| **Проверка на обект** | `isinstance(obj, Parent)` | Връща `True` и за наследниците |
| **Проверка на клас** | `issubclass(Child, Parent)` | Връща `True` ако `Child` наследява `Parent` |

---
[⬅️ 03-02-00. Теория & Капани](03-02-00-Inheritance.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-03-00. Encapsulation](03-03-00-Encapsulation.md)
