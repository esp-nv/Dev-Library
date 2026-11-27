[⬅️ 03-97. Python Category Cheat Sheet](03-97-Category-Cheat-Sheet.md) | 📂 Папка: 03-Python-Scripting | [🏠 Начало](../README.md) | [➡️ 03-99. Interview QA, Traps & Debugging](03-99-Interview-QA-Traps-And-Debugging.md)

---

# 03-98. Python Common Errors, Edge Cases & Fixes (Каталог на грешките)

**Цел:** Практически наръчник за бързо диагностициране и фиксиране на най-честите изключения в **Python 3.x** (`TypeError`, `ValueError`, `KeyError`, `IndexError`, `AttributeError`, `UnboundLocalError`), гранични случаи (**Edge Cases** при колекции, мутируеми аргументи, `0.1 + 0.2`) и йерархията на `BaseException` / `Exception`.

## 🧭 Съдържание на темата
* [1. Грешки при работа с колекции и речници (`KeyError` / `IndexError` / `AttributeError`)](#1-грешки-при-работа-с-колекции-и-речници-keyerror--indexerror--attributeerror)
* [2. Грешки при типове, стойности и обхват (`TypeError` / `ValueError` / `UnboundLocalError`)](#2-грешки-при-типове-стойности-и-обхват-typeerror--valueerror--unboundlocalerror)
* [3. Гранични случаи (Edge Cases): Мутируеми аргументи, празни списъци и точност на числата](#3-гранични-случаи-edge-cases-мутируеми-аргументи-празни-списъци-и-точност-на-числата)
* [4. Йерархия на изключенията, Custom Exceptions & Exception Chaining (`raise ... from err`)](#4-йерархия-на-изключенията-custom-exceptions--exception-chaining-raise--from-err)
* [5. Подробна справка: Кога възниква всяка грешка и в коя фаза?](#5-подробна-справка-кога-възниква-всяка-грешка-и-в-коя-фаза)

---

## 1. ГРЕШКИ ПРИ РАБОТА С КОЛЕКЦИИ И РЕЧНИЦИ (`KEYERROR` / `INDEXERROR` / `ATTRIBUTEERROR`)

### 1.1 `KeyError: 'email'`
*   **Причина:** Опит за достъп до несъществуващ ключ в речник чрез квадратни скоби `user["email"]`.
*   **Грешно срещу Правилно:**
```python
user = {"id": 1, "username": "ivan"}

# ❌ ГРЕШНО: Гърми с KeyError, ако ключът липсва в JSON отговора
# email = user["email"]

# ✅ ПРАВИЛНО: Използвайте .get() със стойност по подразбиране или проверка с 'in'
email = user.get("email", "no-email@domain.bg")
```

### 1.2 `AttributeError: 'NoneType' object has no attribute 'strip'`
*   **Причина:** Функция или ORM заявка е върнала `None` (еквивалент на `NullReferenceException` в C# / `NullPointerException` в Java), а кодът вика метод върху обекта.
*   **Грешно срещу Правилно:**
```python
def find_user_bio(user_id: int) -> str | None:
    return None

bio = find_user_bio(42)

# ❌ ГРЕШНО: AttributeError: 'NoneType' object has no attribute 'strip'
# clean_bio = bio.strip()

# ✅ ПРАВИЛНО: Проверка за None (Guard Clause) или условен израз
clean_bio = bio.strip() if bio is not None else ""
```

---

## 2. ГРЕШКИ ПРИ ТИПОВЕ, СТОЙНОСТИ И ОБХВАТ (`TYPEERROR` / `VALUEERROR` / `UNBOUNDLOCALERROR`)

### `UnboundLocalError: cannot access local variable 'count' where it is not associated with a value`
*   **Причина:** В Python, ако вътре във функция има присвояване `count += 1`, интерпретаторът смята `count` за **локална променлива** в цялата функция и отказва да чете глобалната `count` преди присвояването.
*   **Грешно срещу Правилно:**
```python
total_requests = 0

# ❌ ГРЕШНО: Гърми с UnboundLocalError на реда total_requests += 1
def track_bad() -> None:
    # total_requests += 1
    pass

# ✅ ПРАВИЛНО: Подавайте състоянието като параметър или използвайте клас
def track_good(current_total: int) -> int:
    return current_total + 1
```

---

## 3. ГРАНИЧНИ СЛУЧАИ (EDGE CASES): МУТИРУЕМИ АРГУМЕНТИ, ПРАЗНИ СПИСЪЦИ И ТОЧНОСТ НА ЧИСЛАТА

| Граничен случай (Edge Case) | Какво се чупи в Runtime | Как се фиксира правилно |
| :--- | :--- | :--- |
| **Мутируем аргумент по подразбиране (`def add(x, items=[]):`)** | Списъкът `items=[]` се създава **само 1 път** при дефиниране на функцията и се споделя между всички следващи извиквания! | Използвайте `items: list[str] \| None = None` и вътре във функцията: `if items is None: items = []`. |
| **Празен списък при `items` или `max(items)`** | `items` хвърля `IndexError`, а `max([])` хвърля `ValueError: max() iterable argument is empty`. | Проверявайте `if not items:` в началото на функцията или подавайте `max(items, default=0)`. |
| **Деление на нула (`10 / 0` или `10 % 0`)** | Хвърля `ZeroDivisionError: division by zero`. | Проверявайте `if divisor == 0:` преди делението. |
| **Финансови сметки с `float` (`0.1 + 0.2 == 0.3`)** | Връща `False` (`0.30000000000000004`) заради двоичното представяне IEEE 754! | За пари и цени винаги използвайте **`from decimal import Decimal`** (`Decimal("0.1") + Decimal("0.2") == Decimal("0.3")`). |
| **Премахване на елементи от `list` по време на итерация** | `for x in nums: nums.remove(x)` прескача елементи, защото индексите се изместват! | Създавайте нов списък с List Comprehension: `nums = [x for x in nums if keep(x)]`. |

---

---

## 4. ЙЕРАРХИЯ НА ИЗКЛЮЧЕНИЯТА, CUSTOM EXCEPTIONS & EXCEPTION CHAINING (`RAISE ... FROM ERR`)

По модела на системните среди, в Python има критична разлика между системни сигнали (`BaseException`) и стандартни програмни грешки (`Exception`):

### 1. `BaseException` срещу `Exception` (Никога не пиши гол `except:`!)
*   **`BaseException` (Системни сигнали):** Включва `KeyboardInterrupt` (когато натиснеш `Ctrl+C`), `SystemExit` (при `sys.exit()`) и `GeneratorExit`. Ако напишеш гол `except:`, ще хванеш и `Ctrl+C` и програмата няма да може да бъде спряна нормално!
*   **`Exception` (Всички стандартни грешки и бизнес изключения):** От него наследяват `ValueError`, `TypeError`, `KeyError`, `OSError` и всички твои потребителски класове.

### 2. Custom Exceptions & Запазване на оригиналната грешка (`raise ... from err`)
```python
class OrderProcessingError(Exception):
    """Потребителско бизнес изключение (наследява Exception)."""
    def __init__(self, order_id: int, message: str) -> None:
        self.order_id = order_id
        super().__init__(f"Order #{order_id}: {message}")

def process_payment(order_id: int, raw_amount: str) -> float:
    try:
        return float(raw_amount)
    except ValueError as err:
        # ✅ ПРАВИЛНО: `raise ... from err` закача оригиналната грешка в `__cause__`
        # (еквивалент на InnerException в C# / cause в Java и TS), запазвайки пълния Traceback!
        raise OrderProcessingError(order_id, "Невалидна сума за плащане") from err
```
*   **Правило за производителност:** В Python концепцията *EAFP (Easier to Ask for Forgiveness than Permission)* прави `try-except` евтин, **когато не възниква грешка**, но самото хвърляне (`raise`) и изграждане на `Traceback` обект е скъпо — затова в горещи цикли проверявайте условията предварително (*LBYL*).

---

## 5. ПОДРОБНА СПРАВКА: КОГА ВЪЗНИКВА ВСЯКА ГРЕШКА И В КОЯ ФАЗА?

| Клас / Тип грешка | Фаза на възникване | При какво възниква (Точна причина) | Пример и как се предотвратява |
| :--- | :--- | :--- | :--- |
| **`SyntaxError` / `IndentationError`** | **Компилация до байткод** (`Parse-time` преди старт) | Нарушен синтаксис, липсващо двуеточие `:` или смесени табулации и интервали. | `if x > 5` (без `:`) ➔ Използвайте автоматичен форматер (`Ruff` / `Black`). |
| **`ImportError` / `ModuleNotFoundError`** | **При изпълнение** (`Import-time` при зареждане) | Опит за импорт на неинсталиран пакет, грешно име на модул или кръгов импорт (*Circular Import*). | `import non_existent` ➔ Проверете активната виртуална среда (`.venv`) и `requirements.txt`. |
| **`TypeError`** | **При изпълнение** (`Runtime` в CPython VM) | Операция върху несъвместим тип — напр. събиране на `str + int` или подаване на грешен брой аргументи на функция. | `"Age: " + 25` ➔ Използвайте f-strings `f"Age: {25}"` и статичен анализатор (`mypy` / `Pyright`). |
| **`ValueError`** | **При изпълнение** (`Runtime` в CPython VM) | Типът е правилен, но **стойността е невалидна** — напр. `int("abc")`, `math.sqrt(-1)` или `max([])`. | `int("abc")` ➔ Валидирайте входните низове или ползвайте `Pydantic` схеми. |
| **`KeyError` / `IndexError`** | **При изпълнение** (`Runtime` в CPython VM) | Липсващ ключ в `dict` (`d["missing"]`) или индекс извън границите на `list` (`arr`). | Проверявайте границите и ползвайте `d.get("key", default)`. |
| **`AttributeError`** | **При изпълнение** (`Runtime` в CPython VM) | Достъп до несъществуващ метод/атрибут — най-често върху `None` (`NoneType`). | Проверявайте `if obj is not None:` преди достъп до атрибути. |
| **`ZeroDivisionError`** | **При изпълнение** (`Runtime` в CPython VM) | Деление или модулно делене на нула (`10 / 0`, `10 // 0`, `10 % 0`). | Проверявайте знаменателя `if b == 0:` преди изчислението. |
| **`RecursionError` / `MemoryError`** | **При изпълнение** (`Runtime` — критичен срив) | Препълване на стека при безкрайна рекурсия (лимит 1000 извиквания) или изчерпване на RAM. | Слагайте базово дъно на рекурсията и ползвайте генератори вместо огромни списъци. |
| **Custom `Exception` (`OrderProcessingError`)** | **При изпълнение** (`Runtime` — бизнес логика) | Когато кодът умишлено хвърли домейн изключение (`raise OrderProcessingError(...)`). | Хваща се в API слоя (FastAPI `@app.exception_handler` / Django Middleware) и се връща като HTTP `400`/`422`. |

---
[⬅️ 03-97. Python Category Cheat Sheet](03-97-Category-Cheat-Sheet.md) | 📂 Папка: 03-Python-Scripting | [🏠 Начало](../README.md) | [➡️ 03-99. Interview QA, Traps & Debugging](03-99-Interview-QA-Traps-And-Debugging.md)
