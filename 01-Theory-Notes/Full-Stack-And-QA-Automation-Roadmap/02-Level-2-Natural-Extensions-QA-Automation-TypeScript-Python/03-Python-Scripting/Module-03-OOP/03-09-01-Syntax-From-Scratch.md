[⬅️ 03-09-00. Теория & Капани](03-09-00-Testing.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-10-00. Design Patterns](03-10-00-Design-Patterns.md)

---

# 03-09-01. Testing: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за писане на Unit тестове с `unittest`, проверка на изключения и симулиране с `Mock` в Python от нулата.

## 🧭 Съдържание на темата
* [1. Основен скелет на `unittest` файл](#1-основен-скелет-на-unittest-файл)
* [2. Най-честите `assert` методи за проверка](#2-най-честите-assert-методи-за-проверка)
* [3. Проверка за хвърлена грешка (`assertRaises`)](#3-проверка-за-хвърлена-грешка-assertraises)
* [4. Симулиране на външен обект с `Mock` и `@patch`](#4-симулиране-на-външен-обект-с-mock-и-patch)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОСНОВЕН СКЕЛЕТ НА `UNITTEST` ФАЙЛ

```python
import unittest

class TestMyFeature(unittest.TestCase):
    # Изпълнява се ПРЕДИ всеки тест (подготовка)
    def setUp(self):
        self.items = [10, 20, 30]

    # Изпълнява се СЛЕД всеки тест (почистване - по желание)
    def tearDown(self):
        self.items.clear()

    # Тестов метод (ЗАДЪЛЖИТЕЛНО започва с test_)
    def test_append_adds_element_at_end(self):
        self.items.append(40)
        self.assertEqual(self.items, [10, 20, 30, 40])

# Позволява стартиране на тестовете директно от бутона Run във VS Code
if __name__ == "__main__":
    unittest.main()
```

---

## 2. НАЙ-ЧЕСТИТЕ `ASSERT` МЕТОДИ ЗА ПРОВЕРКА

```python
self.assertEqual(a, b)          # Проверява дали a == b
self.assertNotEqual(a, b)       # Проверява дали a != b
self.assertTrue(x)              # Проверява дали bool(x) is True
self.assertFalse(x)             # Проверява дали bool(x) is False
self.assertIsNone(x)            # Проверява дали x is None
self.assertIn(item, collection) # Проверява дали item in collection
self.assertAlmostEqual(0.1 + 0.2, 0.3, places=5)  # За дробни числа (float)!
```

---

## 3. ПРОВЕРКА ЗА ХВЪРЛЕНА ГРЕШКА (`ASSERTRASES`)

```python
def parse_positive(text: str) -> int:
    num = int(text)
    if num <= 0:
        raise ValueError("Числото не е положително!")
    return num

# В тестовия метод:
with self.assertRaises(ValueError):
    parse_positive("-5")  # Тестът МИНАВА (Pass), само ако този ред хвърли ValueError!
```

---

## 4. СИМУЛИРАНЕ НА ВЪНШЕН ОБЕКТ С `MOCK` И `@PATCH`

```python
from unittest.mock import Mock

# Създаване на фалшив обект (напр. вместо реално API за времето)
weather_api = Mock()
weather_api.get_temperature.return_value = 24.5

# Кодът извиква фалшивия метод и получава 24.5 без интернет връзка
temp = weather_api.get_temperature("Vidin")
print(f"Температура: {temp}°C")

# Проверка дали методът наистина е бил извикан с правилния аргумент
weather_api.get_temperature.assert_called_once_with("Vidin")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис (`unittest`) | Бележка |
| :--- | :--- | :--- |
| **Наследяване на тестов клас** | `class TestApp(unittest.TestCase):` | Задължително за `unittest` |
| **Име на тестов метод** | `def test_something(self):` | Без `test_` тестът се пропуска |
| **Подготовка преди всеки тест** | `def setUp(self):` | Гарантира свежи данни за всеки тест |
| **Проверка за равенство** | `self.assertEqual(actual, expected)` | Първо реалният, после очакваният |
| **Проверка за дробни числа** | `self.assertAlmostEqual(a, b)` | Избягва грешки от типа `0.30000004` |
| **Проверка за грешка** | `with self.assertRaises(ValueError):` | Очакваният срив се слага вътре в блока |
| **Стартиране през терминал** | `python -m unittest discover` | Намира и пуска всички `test_*.py` файлове |

---
[⬅️ 03-09-00. Теория & Капани](03-09-00-Testing.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-10-00. Design Patterns](03-10-00-Design-Patterns.md)
