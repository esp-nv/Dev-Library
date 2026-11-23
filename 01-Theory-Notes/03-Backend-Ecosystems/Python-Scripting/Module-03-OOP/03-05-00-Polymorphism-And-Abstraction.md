[⬅️ 03-04. Static and Class Methods](03-04-00-Static-And-Class-Methods.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-05-01. Syntax From Scratch](03-05-01-Syntax-From-Scratch.md)

---

# 03-05-00. Polymorphism and Abstraction (Теория, Duck Typing, ABC & Капани)

**Модул 03:** Python OOP | **Лекционна тема 05:** Полиморфизъм (Duck Typing и Operator Overloading), Абстрактни базови класове чрез модула `abc` (`ABC`, `@abstractmethod`) и структурни протоколи (`typing.Protocol`).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на валидация на Абстрактен клас и Блок-схема](#2-етапи-на-валидация-на-абстрактен-клас-и-блок-схема)
* [3. Архитектурно сравнение: Python срещу C# и JavaScript](#3-архитектурно-сравнение-python-срещу-c-и-javascript)
* [4. Плюсове и минуси на Полиморфизма и Абстракцията в Python](#4-плюсове-и-минуси-на-полиморфизма-и-абстракцията-в-python)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Duck Typing (Патешко типизиране):** *„Ако ходи като патица и квака като патица, значи за Python то Е патица“.* За разлика от C#, където два класа трябва да имплементират общ `IPayment` интерфейс, в Python е достатъчно и двата класа да имат метод `def pay(self):`.
*   **Абстракция (`ABC` — Abstract Base Class):** Това е **незавършен чертеж със задължителен договор**. Не можеш да построиш къща директно от незавършен чертеж (`shape = Shape()` гърми), а всеки наследник, който го наследи, е **длъжен по закон** да допише всички методи, маркирани с `@abstractmethod`.

---

## 2. ЕТАПИ НА ВАЛИДАЦИЯ НА АБСТРАКТЕН КЛАС И БЛОК-СХЕМА

Когато наследник наследи клас с `@abstractmethod`, Python проверява речника му в момента на създаване на инстанция (`Child()`):

```text
Клас 'PaymentGateway(ABC)' с @abstractmethod def process_payment(self)

┌──────────────────────────┐     1. Опит за инстанция:  ┌─────────────────────────┐
│ gateway = PaymentGateway()│ ────────────────────────► │ ❌ ГРЕШКА (TypeError):  │
│ (Директно от абстрактния)│                           │ Не може да инстанцираш  │
└──────────────────────────┘                           │ абстрактен клас!        │
                                                       └─────────────────────────┘

┌──────────────────────────┐     2. Наследник 'Stripe':  ┌─────────────────────────┐
│ class Stripe(Payment...):│ ────────────────────────► │ Има ли process_payment? │
└──────────────────────────┘                           └──────┬────────────┬─────┘
                                                         ДА   │            │   НЕ
                                                              ▼            ▼
                                                       ┌─────────────┐ ┌──────────┐
                                                       │ ✅ УСПЕХ!   │ │ ❌ ГРЕШКА│
                                                       │ Създава     │ │ TypeError│
                                                       │ обект Stripe│ │ при старт│
                                                       └─────────────┘ └──────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: PYTHON СРЕЩУ C# И JAVASCRIPT

| Концепция | Python 3.x | C# (.NET) | JavaScript (ES6+) |
| :--- | :--- | :--- | :--- |
| **Абстрактен клас** | `from abc import ABC, abstractmethod` | `public abstract class Shape` | Няма нативно (хвърля се `Error` в метода) |
| **Абстрактен метод** | `@abstractmethod \n def area(self): pass` | `public abstract double Area();` | Няма нативно |
| **Интерфейси** | `ABC` (всички методи абстрактни) или `Protocol` | `public interface IShape` | Няма в JS (само в TypeScript) |
| **Duck Typing** | Нативен (поведението определя типа) | Само чрез `dynamic` (рядко) | Нативен (поведението определя типа) |
| **Method Overloading** | **Не се поддържа** (ползват се default args) | Нативно поддържано | **Не се поддържа** |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ПОЛИМОРФИЗМА И АБСТРАКЦИЯТА

### 🟢 Предимства (Pros):
*   **Ниска свързаност (Loose Coupling) чрез Duck Typing:** Пишеш функции, които работят с всякакви обекти (файлове, мрежови потоци, тестови фалшиви обекти), стига да имат нужния метод (напр. `.read()`).
*   **Строги договори в големи екипи чрез `ABC`:** Гарантира, че никой разработчик няма да забрави да имплементира критичен метод в нов модул.

### 🔴 Недостатъци (Cons):
*   **Късно откриване на грешки при Duck Typing:** Ако подадеш обект без метод `.pay()`, грешката ще излезе чак в момента, в който изпълнението стигне до реда `obj.pay()` по време на работа.
*   **Липса на класически Method Overloading:** Не можеш да напишеш три метода `calculate(int)`, `calculate(str)` и `calculate(list)` в един клас — последният презаписва предишните (решава се с `@singledispatch` или `isinstance`).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Наследяване от `ABC` без `@abstractmethod` или липсваща имплементация):
```python
from abc import ABC, abstractmethod

# 🚨 ГРЕШКА 1: Класът наследява ABC, но няма нито един @abstractmethod!
# Python ще ти позволи да направиш fake = BrokenInterface() без грешка!
class BrokenInterface(ABC):
    def connect(self):
        pass

class RealDatabase(ABC):
    @abstractmethod
    def execute_query(self, sql: str):
        pass

# 🚨 ГРЕШКА 2: Наследникът забравя да имплементира execute_query
class BrokenPostgres(RealDatabase):
    def close(self):
        print("Затворено.")

# db = BrokenPostgres() -> Хвърля TypeError: Can't instantiate abstract class BrokenPostgres!
```
*   **Как да се избегне:** За да бъде един клас наистина абстрактен и да не може да се инстанцира, той **задължително** трябва едновременно да наследява `ABC` **и** да има поне един метод с декоратор `@abstractmethod`.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт с `ABC` и Полиморфизъм):
```python
from abc import ABC, abstractmethod

# 1. Дефиниране на строг абстрактен договор
class NotificationSender(ABC):
    @abstractmethod
    def send(self, recipient: str, message: str) -> None:
        pass

# 2. Конкретни имплементации
class EmailSender(NotificationSender):
    def send(self, recipient: str, message: str) -> None:
        print(f"[📧 EMAIL до {recipient}]: {message}")

class SmsSender(NotificationSender):
    def send(self, recipient: str, message: str) -> None:
        print(f"[📱 SMS до {recipient}]: {message}")

# 3. Полиморфна функция (не се интересува какъв точно е изпращачът)
def notify_all(channels: list[NotificationSender], user: str, text: str) -> None:
    for channel in channels:
        channel.send(user, text)  # Еднакъв интерфейс, различно поведение!

active_channels = [EmailSender(), SmsSender()]
notify_all(active_channels, "elizabeth@softuni.bg", "Модул 03 напредва отлично!")
```
*   **Изход в конзолата:**
    ```text
    [📧 EMAIL до elizabeth@softuni.bg]: Модул 03 напредва отлично!
    [📱 SMS до elizabeth@softuni.bg]: Модул 03 напредва отлично!
    ```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между наследяване от `ABC` (Nominal Subtyping) и използването на `typing.Protocol` (Structural Subtyping / Static Duck Typing) в модерния Python?“*
*   **Уловката:** Кандидатите знаят за `ABC`, но не познават модерния `Protocol` (въведен в Python 3.8), който решава проблема със силното обвързване.
*   **Точен Senior отговор:** 
    1.  При **`ABC`** класът-наследник **задължително** трябва изрично да наследи базовия клас (`class Stripe(PaymentBase):`). Това е неудобно, ако работиш с класове от външна библиотека, които не можеш да променяш.
    2.  При **`typing.Protocol`** дефинираш структурни правила (като интерфейсите в TypeScript!). Всеки клас, който има описаните в протокола методи, автоматично се признава за съвместим от статичните анализатори (MyPy / Pyright / VS Code), **без да се налага изрично да наследява протокола**.

---
[⬅️ 03-04. Static and Class Methods](03-04-00-Static-And-Class-Methods.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-05-01. Syntax From Scratch](03-05-01-Syntax-From-Scratch.md)
