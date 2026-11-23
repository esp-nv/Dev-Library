[⬅️ 03-03-00. Теория & Капани](03-03-00-Encapsulation.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-04-00. Static and Class Methods](03-04-00-Static-And-Class-Methods.md)

---

# 03-03-01. Encapsulation: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за защитени и частни атрибути, `@property` гетъри, сетъри и Read-Only свойства в Python от нулата.

## 🧭 Съдържание на темата
* [1. Нива на видимост (`public`, `_protected`, `__private`)](#1-нива-на-видимост-public-_protected-__private)
* [2. Пълен синтаксис на `@property` (Getter и Setter)](#2-пълен-синтаксис-на-property-getter-и-setter)
* [3. Създаване на Read-Only (само за четене) свойство](#3-създаване-на-read-only-само-за-четене-свойство)
* [4. Частни методи (`def __helper_method(self):`)](#4-частни-методи-def-__helper_methodself)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. НИВА НА ВИДИМОСТ (`PUBLIC`, `_PROTECTED`, `__PRIVATE`)

```python
class Employee:
    def __init__(self, name: str, department: str, salary: float):
        self.name = name                # Public: достъпно отвсякъде
        self._department = department   # Protected: конвенция (за класа и наследниците)
        self.__salary = salary          # Private: скрито чрез Name Mangling

emp = Employee("Елизабет", "Engineering", 5000)

print(emp.name)             # Работи -> Елизабет
print(emp._department)      # Работи технически, но редакторът дава предупреждение
# print(emp.__salary)       # ГЪРМИ с AttributeError!
```

---

## 2. ПЪЛЕН СИНТАКСИС НА `@PROPERTY` (GETTER И SETTER)

```python
class Person:
    def __init__(self, username: str):
        self.username = username        # Вика сетъра автоматично

    # 1. GETTER (Четене: print(p.username))
    @property
    def username(self) -> str:
        return self.__username

    # 2. SETTER (Запис: p.username = "НовоИме")
    # Името на декоратора ТРЯБВА да съвпада с името на @property метода!
    @username.setter
    def username(self, value: str):
        if len(value.strip()) < 3:
            raise ValueError("Потребителското име трябва да е поне 3 символа!")
        self.__username = value.strip()

p = Person("  esp_nv  ")
print(p.username)           # Изход: esp_nv
```

---

## 3. СЪЗДАВАНЕ НА READ-ONLY (САМО ЗА ЧЕТЕНЕ) СВОЙСТВО

Ако искаш дадено свойство да се изчислява или чете, но **никой да не може да го презаписва** отвън, просто не му пишеш `.setter`.

```python
class Circle:
    def __init__(self, radius: float):
        self.radius = radius

    @property
    def area(self) -> float:
        return 3.14159 * (self.radius ** 2)

c = Circle(5)
print(c.area)               # Чете се без скоби () като променлива -> 78.53975
# c.area = 100              # ГЪРМИ с AttributeError: property 'area' has no setter!
```

---

## 4. ЧАСТНИ МЕТОДИ (`DEF __HELPER_METHOD(SELF):`)

```python
class PaymentProcessor:
    def process(self, card_number: str):
        if self.__validate_card(card_number):
            print("Плащането е одобрено!")

    # Частен помощен метод - вижда се само вътре в класа
    def __validate_card(self, number: str) -> bool:
        return len(number) == 16 and number.isdigit()

processor = PaymentProcessor()
processor.process("1234567812345678")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис | Какво прави |
| :--- | :--- | :--- |
| **Публичен атрибут** | `self.name` | Свободен достъп за четене и писане |
| **Защитен атрибут** | `self._name` | Сигнал към програмистите да не го пипат отвън |
| **Частен атрибут / метод** | `self.__name` / `def __calc(self):` | Активира Name Mangling (`_ClassName__name`) |
| **Getter свойство** | `@property \n def age(self):` | Позволява четене като `obj.age` без скоби |
| **Setter свойство** | `@age.setter \n def age(self, val):` | Позволява валидация при `obj.age = 20` |
| **Изтриване на свойство** | `@age.deleter \n def age(self):` | Извиква се при `del obj.age` |

---
[⬅️ 03-03-00. Теория & Капани](03-03-00-Encapsulation.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-04-00. Static and Class Methods](03-04-00-Static-And-Class-Methods.md)
