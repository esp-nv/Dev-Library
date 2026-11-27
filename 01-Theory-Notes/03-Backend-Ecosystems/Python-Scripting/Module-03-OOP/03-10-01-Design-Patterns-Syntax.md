[⬅️ 03-10-00. Теория & Капани](03-10-00-Design-Patterns.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)

---

# 03-10-01. Design Patterns: Синтаксис от нулата & Cheat Sheet

**Цел:** Готови, изчистени кодови шаблони за най-важните Design Patterns в Python (`Singleton`, `Factory`, `Builder`, `Observer`, `Adapter`) от нулата.

## 🧭 Съдържание на темата
* [1. Thread-Safe Singleton Pattern (Нишково-защитен)](#1-thread-safe-singleton-pattern-нишково-защитен)
* [2. Builder Pattern (Сглобяване на обект стъпка по стъпка)](#2-builder-pattern-сглобяване-на-обект-стъпка-по-стъпка)
* [3. Observer Pattern (Абонамент и известяване)](#3-observer-pattern-абонамент-и-известяване)
* [4. Adapter Pattern (Преходник между несъвместими класове)](#4-adapter-pattern-преходник-между-несъвместими-класове)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. THREAD-SAFE SINGLETON PATTERN (НИШКОВО-ЗАЩИТЕН)

```python
import threading

class DatabaseConnection:
    _instance = None
    _lock = threading.Lock()  # Нишков катинар против паралелно създаване

    def __new__(cls):
        if cls._instance is None:
            with cls._lock:
                # Двойна проверка (Double-Checked Locking)
                if cls._instance is None:
                    cls._instance = super().__new__(cls)
                    cls._instance.status = "Connected"
        return cls._instance

db1 = DatabaseConnection()
db2 = DatabaseConnection()
print(db1 is db2)           # Изход: True (абсолютно един и същ адрес в RAM!)
```

---

## 2. BUILDER PATTERN (СГЛОБЯВАНЕ НА ОБЕКТ СТЪПКА ПО СТЪПКА)

Използва се с верижно извикване на методи (`return self`), за да избегнеш конструктори с 10 параметъра.

```python
class HttpRequest:
    def __init__(self):
        self.url = ""
        self.method = "GET"
        self.headers = {}

class HttpRequestBuilder:
    def __init__(self):
        self._request = HttpRequest()

    def set_url(self, url: str):
        self._request.url = url
        return self  # Връща себе си за верижно извикване (Fluent Interface)

    def set_method(self, method: str):
        self._request.method = method
        return self

    def add_header(self, key: str, value: str):
        self._request.headers[key] = value
        return self

    def build(self) -> HttpRequest:
        return self._request

# Чисто сглобяване на заявката
req = (HttpRequestBuilder()
       .set_url("https://softuni.bg")
       .set_method("POST")
       .add_header("Authorization", "Bearer token123")
       .build())

print(f"{req.method} -> {req.url} | Headers: {req.headers}")
```

---

## 3. OBSERVER PATTERN (АБОНАМЕНТ И ИЗВЕСТЯВАНЕ)

```python
class NewsPublisher:
    def __init__(self):
        self._subscribers = []

    def subscribe(self, callback):
        self._subscribers.append(callback)

    def publish_news(self, headline: str):
        for subscriber in self._subscribers:
            subscriber(headline)  # Известява всички абонати

# Абонати (функции или методи)
def email_alert(news): print(f"[Email] Нова статия: {news}")
def sms_alert(news):   print(f"[SMS]   Спешно: {news}")

publisher = NewsPublisher()
publisher.subscribe(email_alert)
publisher.subscribe(sms_alert)

publisher.publish_news("Python курсът е напълно завършен!")
```

---

## 4. ADAPTER PATTERN (ПРЕХОДНИК МЕЖДУ НЕСЪВМЕСТИМИ КЛАСОВЕ)

```python
# Стара система, която връща данни в XML
class LegacyXmlService:
    def fetch_xml(self) -> str:
        return "<user>Elizabeth</user>"

# Адаптер, който превежда интерфейса към модерния очакван метод .get_json()
class XmlToJsonAdapter:
    def __init__(self, xml_service: LegacyXmlService):
        self.xml_service = xml_service

    def get_json(self) -> dict:
        raw_xml = self.xml_service.fetch_xml()
        clean_name = raw_xml.replace("<user>", "").replace("</user>", "")
        return {"user": clean_name}

adapter = XmlToJsonAdapter(LegacyXmlService())
print(adapter.get_json())   # Изход: {'user': 'Elizabeth'}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Шаблон | Категория | Кога да го използваш | Ключова идея в Python |
| :--- | :--- | :--- | :--- |
| **Singleton** | Creational | Когато ти трябва строго 1 инстанция (логър, кеш) | Обикновен `.py` модул или `__new__` с `Lock` |
| **Factory** | Creational | Когато създаваш различни обекти според входен тип | Речник `{тип: Клас}` + `@classmethod` |
| **Builder** | Creational | Когато обектът има много опционални настройки | Методи, които връщат `self` (Fluent API) |
| **Adapter** | Structural | Когато свързваш стар (Legacy) код с нов интерфейс | Клас-обвивка, който превежда методите |
| **Decorator** | Structural | Когато добавяш поведение без промяна на кода | Вграденият синтаксис `@decorator` |
| **Observer** | Behavioral | Когато много обекти чакат събитие от 1 източник | Списък от функции и цикъл при събитие |
| **Strategy** | Behavioral | Когато сменяш алгоритъма динамично при работа | Подаване на функция като аргумент (`Callable`) |

---
[⬅️ 03-10-00. Теория & Капани](03-10-00-Design-Patterns.md) | 📂 Папка: Module-03-OOP | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)
