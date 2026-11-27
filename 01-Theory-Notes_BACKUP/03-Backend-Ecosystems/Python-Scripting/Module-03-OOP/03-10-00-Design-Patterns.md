[⬅️ 03-09. Testing](03-09-00-Testing.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-10-01. Syntax From Scratch](03-10-01-Syntax-From-Scratch.md)

---

# 03-10-00. Design Patterns (Теория, Creational / Structural / Behavioral & Капани)

**Модул 03:** Python OOP | **Лекционна тема 10:** Класически шаблони за софтуерна архитектура (Gang of Four), разделени на Създаващи, Структурни и Поведенчески, и тяхната Pythonic имплементация.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурна карта на Трите категории и Блок-схема](#2-архитектурна-карта-на-трите-категории-и-блок-схема)
* [3. Архитектурно сравнение: Python срещу C# и JavaScript](#3-архитектурно-сравнение-python-срещу-c-и-javascript)
* [4. Плюсове и минуси на Design Patterns в Python](#4-плюсове-и-минуси-на-design-patterns-в-python)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Три групи шаблони (Готови архитектурни рецепти):**
    1.  **Creational (Създаващи — „Как раждаме обекти“):** `Singleton` (само 1 инстанция в целия свят, напр. връзка с база данни), `Factory` (фабрика, която решава кой клас да създаде според входните данни), `Builder` (сглобява сложен обект стъпка по стъпка).
    2.  **Structural (Структурни — „Как сглобяваме обекти“):** `Adapter` (преходник за стар контакт към нов щепсел), `Decorator` (добавя нови екстри върху обекта без да го променя), `Facade` (едно лесно копче, което крие 10 сложни подсистеми отзад).
    3.  **Behavioral (Поведенчески — „Как обектите си говорят“):** `Observer` (абонамент за YouTube канал — когато излезе ново видео, всички абонати получават известие), `Strategy` (сменяш алгоритъма в движение — напр. навигация с кола, пеша или с колело), `Command` (опакова заявка като обект, позволява Undo/Redo).

---

## 2. АРХИТЕКТУРНА КАРТА НА ТРИТЕ КАТЕГОРИИ И БЛОК-СХЕМА

Пример за **Factory Method** — клиентският код не знае и не се интересува как се създават конкретните обекти, а пита единствено фабриката:

```text
Заявка: NotificationFactory.create("sms")

┌──────────────────────────┐     1. Подава тип "sms"    ┌─────────────────────────┐
│ Клиентски код            │ ─────────────────────────► │ NotificationFactory     │
│ (Бизнес логика)          │                            │ (Взима решението)       │
└──────────────────────────┘                            └──────┬───────────┬──────┘
                                                               │           │
                                                "email"        │           │ "sms"
                                                               ▼           ▼
┌──────────────────────────┐     2. Връща готов обект   ┌───────────┐ ┌───────────┐
│ Работи с абстрактен      │ ◄───────────────────────── │ EmailObj  │ │ SmsObj    │
│ метод .send()            │   (Скрива сложността)      └───────────┘ └───────────┘
└──────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: PYTHON СРЕЩУ C# И JAVASCRIPT

| Шаблон (Pattern) | Python 3.x | C# (.NET) | JavaScript / TypeScript |
| :--- | :--- | :--- | :--- |
| **Singleton** | Модулът в Python е Singleton по природа (или `__new__`) | `builder.Services.AddSingleton<T>()` | `export const instance = new Service()` |
| **Strategy** | Подаване на функция директно (First-Class Functions) | Интерфейс `IStrategy` + отделни класове | Подаване на callback функция `(x) => ...` |
| **Decorator** | Вграден `@decorator` синтаксис в езика | Обвиващи класове с общ интерфейс | `@decorator` или Higher-Order Functions |
| **Iterator** | Вграден протокол `__iter__` / `__next__` | `IEnumerable<T>` / `yield return` | `Symbol.iterator` / `function*` |
| **Observer** | Списък от callback функции или `asyncio` събития | `event` и `delegate` (нативно в езика) | `EventEmitter` / `addEventListener` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА DESIGN PATTERNS В PYTHON

### 🟢 Предимства (Pros):
*   **Общ инженерен език:** Когато кажеш на колега от C# или Java „Тук ползвах Adapter и Factory“, той веднага разбира архитектурата без да чете целия код.
*   **Опростена имплементация в Python:** Тъй като функциите и класовете в Python са първокласни обекти (First-Class Objects), шаблони като `Strategy` и `Command` се пишат с 3 пъти по-малко код, отколкото в C# и Java.

### 🔴 Недостатъци (Cons):
*   **Риск от сляпо копиране от C#/Java:** Писането на 5 отделни класа за прост `Strategy` патерн в Python е анти-патърн, когато можеш просто да подадеш функция като аргумент.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Грешен Singleton, който създава нови обекти при `__init__`):
```python
class BrokenSingleton:
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self):
        # 🚨 ГРЕШКА: __init__ се вика ВСЕКИ ПЪТ след __new__, дори когато обектът вече съществува!
        # Така всяко ново извикване BrokenSingleton() нулира настройките на стария обект!
        self.connection_count = 0
```
*   **Как да се избегне:** В Python най-чистият Singleton е **обикновен Python модул** (защото Python зарежда и кешира всеки импортиран `.py` файл само веднъж в `sys.modules`). Ако пишеш класов Singleton, слагай флаг `if not hasattr(self, "_initialized"):` вътре в `__init__`.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт с `Factory` и `Strategy` патерни):
```python
from abc import ABC, abstractmethod
from typing import Callable

# 1. FACTORY PATTERN (Създаване на правилния парсър според формата)
class DataParser(ABC):
    @abstractmethod
    def parse(self, raw_data: str) -> dict:
        pass

class JsonParser(DataParser):
    def parse(self, raw_data: str) -> dict:
        return {"format": "JSON", "content": raw_data}

class XmlParser(DataParser):
    def parse(self, raw_data: str) -> dict:
        return {"format": "XML", "content": raw_data}

class ParserFactory:
    _parsers = {"json": JsonParser, "xml": XmlParser}

    @classmethod
    def get_parser(cls, format_type: str) -> DataParser:
        parser_class = cls._parsers.get(format_type.lower())
        if not parser_class:
            raise ValueError(f"Неподдържан формат: {format_type}")
        return parser_class()

# 2. PYTHONIC STRATEGY PATTERN (Използва функции вместо излишни класове)
def standard_shipping(weight: float) -> float:
    return weight * 2.50

def express_shipping(weight: float) -> float:
    return (weight * 5.00) + 10.0

def calculate_delivery(weight: float, strategy: Callable[[float], float]) -> float:
    return strategy(weight)

# Тестване на двата шаблона
parser = ParserFactory.get_parser("json")
print(parser.parse('{"status": "OK"}'))

cost = calculate_delivery(4.0, express_shipping)
print(f"Цена за експресна доставка: {cost:.2f} лв.")
```
*   **Изход в конзолата:**
    ```text
    {'format': 'JSON', 'content': '{"status": "OK"}'}
    Цена за експресна доставка: 30.00 лв.
    ```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Защо шаблонът Singleton често се счита за анти-патърн в модерната архитектура и какви проблеми създава при Unit Testing и многонишковост (Multithreading)?“*
*   **Уловката:** Кандидатите често наизустяват как се пише Singleton, но не знаят кога вреди на проекта.
*   **Точен Senior отговор:** Singleton на практика представлява **скрито глобално състояние (Global State)** в паметта, което нарушава принципа за единствена отговорност (SRP) и прави зависимостите невидими:
    1.  **Проблем при тестване:** Тъй като една и съща инстанция живее през целия живот на програмата, данните от `test_1` остават в Singleton-а и замърсяват `test_2`, чупейки изолацията на тестовете.
    2.  **Проблем при многонишковост (Race Condition):** Ако две нишки влязат едновременно в `if cls._instance is None:`, и двете ще създадат отделна инстанция, освен ако не използваш нишков катинар (`threading.Lock()`).

---
[⬅️ 03-09. Testing](03-09-00-Testing.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [➡️ 03-10-01. Syntax From Scratch](03-10-01-Syntax-From-Scratch.md)
