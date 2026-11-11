[⬅️ 03-08. Decorators](03-08-00-Decorators.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-09-01. Syntax From Scratch](03-09-01-Syntax-From-Scratch.md)

---

# 03-09-00. Testing (Теория, `unittest` / `pytest`, Mocking & Капани)

**Модул 03:** Python OOP | **Лекционна тема 09:** Модулно тестване (Unit Testing), патернът AAA (Arrange-Act-Assert), вграденият модул `unittest`, модерният `pytest` и симулиране на зависимости с `unittest.mock`.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на един Unit тест (AAA Патерн) и Блок-схема](#2-етапи-на-изпълнение-на-unit-тест-aaa-патерн-и-блок-схема)
* [3. Архитектурно сравнение: Python срещу C# и JavaScript](#3-архитектурно-сравнение-python-срещу-c-и-javascript)
* [4. Плюсове и минуси на `unittest` и `pytest`](#4-плюсове-и-минуси-на-unittest-и-pytest)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Законът за имената (`test_`):** В Python тестовите методи **задължително** трябва да започват с префикса **`test_`** (напр. `def test_deposit_positive_amount(self):`). Ако забравиш `test_` отпред, тестовият двигател просто ще прескочи метода, без да го изпълни и без да ти каже!
*   **Трите А-та (AAA Pattern):** Всеки качествен тест прилича на научен експеримент в 3 стъпки:
    1.  **Arrange (Подготовка):** Създаваш обекта и входните данни.
    2.  **Act (Действие):** Извикваш конкретния метод, който тестваш.
    3.  **Assert (Проверка):** Сравняваш получения резултат с очаквания.

---

## 2. ЕТАПИ НА ИЗПЪЛНЕНИЕ НА UNIT ТЕСТ (AAA ПАТЕРН) И БЛОК-СХЕМА

При `unittest` методът `setUp()` се изпълнява **наново преди всеки отделен `test_` метод**, за да гарантира, че тестовете са 100% изолирани и не си пречат един на друг.

```text
┌──────────────────────────┐     1. setUp()             ┌─────────────────────────┐
│ Старт на Test Suite      │ ─────────────────────────► │ Създава чисто нов обект │
│                          │   (Изпълнява се ПРЕДИ      │ в паметта за Тест #1    │
└──────────────────────────┘    всеки отделен тест!)    └────────────┬────────────┘
                                                                     │ 2. test_method_1()
                                                                     ▼
┌──────────────────────────┐     3. tearDown()          ┌─────────────────────────┐
│ Повтаря цикъла на чисто  │ ◄───────────────────────── │ Изпълнява Arrange, Act  │
│ за Тест #2...            │   (Почиства ресурсите      │ и проверява Assert      │
└──────────────────────────┘    след теста)             └─────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: PYTHON СРЕЩУ C# И JAVASCRIPT

| Концепция | Python (`unittest` / `pytest`) | C# (`NUnit` / `xUnit`) | JavaScript (`Jest` / `Vitest`) |
| :--- | :--- | :--- | :--- |
| **Вградена рамка** | `unittest` (идва с Python) | Няма (инсталира се NuGet) | Няма (`node:test` в новите версии) |
| **Индустриален стандарт** | `pytest` (външен пакет) | `xUnit` / `NUnit` | `Jest` / `Vitest` |
| **Подготовка преди тест** | `def setUp(self):` или `@pytest.fixture` | `[SetUp]` или конструктор в xUnit | `beforeEach(() => { ... })` |
| **Проверка за равенство** | `self.assertEqual(a, b)` или `assert a == b` | `Assert.That(a, Is.EqualTo(b))` | `expect(a).toBe(b)` |
| **Проверка за грешка** | `with self.assertRaises(ValueError):` | `Assert.Throws<ArgumentException>(...)` | `expect(() => fn()).toThrow()` |
| **Симулиране (Mocking)** | `from unittest.mock import Mock, patch` | `Moq` (`new Mock<IService>()`) | `jest.fn()` / `vi.spyOn()` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА `UNITTEST` И `PYTEST`

### 🟢 Предимства (Pros):
*   **`unittest` е вграден:** Не изисква инсталация на нищо външно — работи веднага на всеки компютър и в изпитни среди (като Judge).
*   **Мощен `unittest.mock`:** Вграденият модул за Mocking позволява временно да подмениш бавна SQL база данни, плащане с карта или външно REST API с фалшив обект по време на теста.

### 🔴 Недостатъци (Cons):
*   **Тромав синтаксис на `unittest`:** Изисква наследяване от `unittest.TestCase`, писане на класове и запомняне на десетки методи (`assertEqual`, `assertTrue`, `assertIn`), докато модерният `pytest` използва само обикновения оператор `assert`.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Липсващ `test_` префикс и грешно извикване на функция в `assertRaises`):
```python
import unittest

class Calculator:
    def divide(self, a, b):
        if b == 0:
            raise ZeroDivisionError("Деление на нула!")
        return a / b

class TestCalculator(unittest.TestCase):
    # 🚨 ГРЕШКА 1: Името НЕ започва с 'test_' -> никога няма да се изпълни!
    def check_addition(self):
        self.assertEqual(2 + 2, 4)

    def test_divide_by_zero(self):
        calc = Calculator()
        # 🚨 ГРЕШКА 2: Извикваш calc.divide(10, 0) ДИРЕКТНО в скобите!
        # Грешката гърми ПРЕДИ assertRaises да успее да я хване!
        # self.assertRaises(ZeroDivisionError, calc.divide(10, 0)) -> CRASH!
```
*   **Как да се избегне:** Всеки тестов метод започва с `test_`. При проверка за грешки винаги използвай контекст мениджъра **`with self.assertRaises(ErrorType):`**.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic `unittest` стандарт с `setUp` и `assertRaises`):
```python
import unittest

# Кодът, който тестваме (Production Code)
class BankAccount:
    def __init__(self, balance: float):
        self.balance = balance

    def withdraw(self, amount: float) -> None:
        if amount > self.balance:
            raise ValueError("Недостатъчна наличност!")
        self.balance -= amount

# Тестовият клас (Unit Tests)
class TestBankAccount(unittest.TestCase):
    def setUp(self):
        # Arrange: Създава се чисто нова сметка със 100 лв. преди ВСЕКИ тест
        self.account = BankAccount(100.0)

    def test_withdraw_valid_amount_decreases_balance(self):
        # Act
        self.account.withdraw(40.0)
        # Assert
        self.assertEqual(self.account.balance, 60.0)

    def test_withdraw_more_than_balance_raises_value_error(self):
        # Act & Assert: Очакваме ValueError вътре в 'with' блока
        with self.assertRaises(ValueError) as context:
            self.account.withdraw(150.0)
        
        self.assertEqual(str(context.exception), "Недостатъчна наличност!")

if __name__ == "__main__":
    unittest.main()
```
*   **Изход в конзолата:**
    ```text
    ..
    ----------------------------------------------------------------------
    Ran 2 tests in 0.001s

    OK
    ```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какъв е най-честият капан при използване на декоратора `@patch` от `unittest.mock` за подмяна на външна функция (Mocking) и какво означава правилото 'Patch where it is used, not where it is defined'?“*
*   **Уловката:** Кандидатите често пишат `@patch("database_module.get_user")` вместо `@patch("user_service.get_user")` и мокът не сработва!
*   **Точен Senior отговор:** Когато файл `user_service.py` направи `from database_module import get_user`, той създава **собствена локална референция** към функцията `get_user` в своето пространство от имена. Ако ти пачнеш оригиналния `database_module.get_user`, референцията вътре в `user_service.py` остава непроменена и продължава да вика реалната база данни! Затова златният закон в Python Mocking е: **„Пачвай там, където функцията се ИЗПОЛЗВА (`user_service.get_user`), а не там, където е дефинирана“**.

---
[⬅️ 03-08. Decorators](03-08-00-Decorators.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-09-01. Syntax From Scratch](03-09-01-Syntax-From-Scratch.md)
