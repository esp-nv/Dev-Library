[⬅️ 04-06-00. Теория & Сравнение](04-06-00-Transactions-And-Async.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-02. SQLAlchemy Syntax](04-06-02-SQLAlchemy-Syntax.md)

---

# 04-06-01. Django ORM Transactions, Raw SQL & Async: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `transaction.atomic`, `on_commit`, параметризиран Raw SQL, асинхронни заявки и атомарно импортиране на JSON/XML в Django от нулата [1.53].

## 🧭 Съдържание на темата
* [1. Трансакции с `transaction.atomic` и `transaction.on_commit`](#1-трансакции-с-transactionatomic-и-transactionon_commit)
* [2. Безопасен Raw SQL (`raw()` и `connection.cursor()`)](#2-безопасен-raw-sql-raw-и-connectioncursor)
* [3. Асинхронни заявки в Django 5+ (`aget`, `acreate`, `async for`)](#3-асинхронни-заявки-в-django-5-aget-acreate-async-for)
* [4. Атомарно импортиране на JSON и XML пакети в базата](#4-атомарно-импортиране-на-json-и-xml-пакети-в-базата)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ТРАНСАКЦИИ С `TRANSACTION.ATOMIC` И `TRANSACTION.ON_COMMIT`

```python
from django.db import transaction
from store.models import Order, AuditLog

# Вариант 1: Като контекст мениджър (with)
with transaction.atomic():
    order = Order.objects.create(total=250.00)
    AuditLog.objects.create(action=f"Създадена поръчка #{order.id}")
    
    # Изпълнява се ЧАК след като базата потвърди (COMMIT) трансакцията успешно!
    # (Идеално за пращане на имейл или Celery задача, за да не пратиш имейл при провален запис!)
    transaction.on_commit(lambda: print(f"Изпратен имейл за поръчка #{order.id}"))

# Вариант 2: Като декоратор върху цяла функция
@transaction.atomic
def create_full_invoice(data: dict):
    # Всички ORM операции вътре в тази функция са в една обща трансакция
    pass
```

---

## 2. БЕЗОПАСЕН RAW SQL (`RAW()` И `CONNECTION.CURSOR()`)

Винаги използвай **`%s`** като контейнер за параметри и подавай стойностите в списък `[...]`, за да те защити драйверът от SQL Injection:

```python
from django.db import connection
from store.models import Product

# 1. Връщане на истински Django модел обекти чрез .raw()
min_price = 100
expensive_products = Product.objects.raw(
    "SELECT * FROM store_products WHERE price >= %s ORDER BY price DESC",
    [min_price]
)

# 2. Изпълнение на произволен SQL (без модели) чрез connection.cursor()
with connection.cursor() as cursor:
    cursor.execute("SELECT COUNT(*), AVG(price) FROM store_products WHERE category = %s", ["EL"])
    total_count, avg_price = cursor.fetchone()
    print(f"Брой: {total_count} | Средна цена: {avg_price}")
```

---

## 3. АСИНХРОННИ ЗАЯВКИ В ДЖАНГО 5+ (`AGET`, `ACREATE`, `ASYNC FOR`)

В модерния Django всички стандартни методи имат асинхронен аналог с префикс **`a`** отпред:

```python
async def fetch_vip_users():
    # Асинхронно създаване и четене на 1 обект
    user = await User.objects.acreate(username="async_eli")
    found = await User.objects.aget(id=user.id)

    # Асинхронно обхождане на QuerySet с 'async for'
    async for active_user in User.objects.filter(is_active=True):
        print(active_user.username)
```

---

## 4. АТОМАРНО ИМПОРТИРАНЕ НА JSON И XML ПАКЕТИ В БАЗАТА

Когато получаваш външен JSON или XML файл със 100 записа и искаш или всички да влязат в базата, или нито един при грешка:

```python
from django.core import serializers
from django.db import transaction

def import_xml_batch_safely(xml_data: str) -> None:
    try:
        with transaction.atomic():
            # Чете записите от XML потока и ги записва в една трансакция
            for deserialized_obj in serializers.deserialize("xml", xml_data):
                deserialized_obj.save()
        print("Всички записи от XML файла са импортирани успешно!")
    except Exception as e:
        print(f"Грешка във файла! Базата е върната в начално състояние (Rollback): {e}")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция | Синтаксис в Django ORM |
| :--- | :--- |
| **Трансакционен блок** | `with transaction.atomic():` |
| **Декоратор за трансакция** | `@transaction.atomic` над функция |
| **Действие след успешен `COMMIT`** | `transaction.on_commit(callback_func)` |
| **Заключване на ред (`FOR UPDATE`)** | `Model.objects.select_for_update().filter(...)` |
| **Параметризиран Raw SQL** | `Model.objects.raw("SELECT ... WHERE id = %s", [val])` |
| **Асинхронно четене / създаване** | `await Model.objects.aget(...)` / `acreate(...)` |
| **Асинхронно обхождане** | `async for item in Model.objects.filter(...):` |

---
[⬅️ 04-06-00. Теория & Сравнение](04-06-00-Transactions-And-Async.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-02. SQLAlchemy Syntax](04-06-02-SQLAlchemy-Syntax.md)
