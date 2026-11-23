[⬅️ 04-03-00. Теория & Сравнение](04-03-00-CRUD-And-Queries.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-02. SQLAlchemy Syntax](04-03-02-SQLAlchemy-Syntax.md)

---

# 04-03-01. Django ORM CRUD: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване, четене, филтриране, обновяване и изтриване на записи в Django ORM от нулата.

## 🧭 Съдържание на темата
* [1. CREATE (Създаване и масово вмъкване)](#1-create-създаване-и-масово-вмъкване)
* [2. READ (Четене, филтриране и оператори `__`)](#2-read-четене-филтриране-и-оператори-__)
* [3. Сложни условия с `Q` обекти и сортиране](#3-сложни-условия-с-q-обекти-и-сортиране)
* [4. UPDATE и DELETE (Обновяване и изтриване)](#4-update-и-delete-обновяване-и-изтриване)
* [4.1. CRUD ОПЕРАЦИИ ПРИ ПРЕНОС В JSON И XML (СТЪПКА ПО СТЪПКА)](#41-crud-операции-при-пренос-в-json-и-xml-стъпка-по-стъпка)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. CREATE (СЪЗДАВАНЕ И МАСОВО ВМЪКВАНЕ)

```python
from store.models import Product

# Вариант 1: Създаване на обект в паметта и запис с .save()
p1 = Product(name="Мишка Logitech", price=89.90)
p1.save()

# Вариант 2: Създаване и запис на 1 ред с .create() (най-използваният начин!)
p2 = Product.objects.create(name="Клавиатура Keychron", price=199.00)

# Вариант 3: Вземи ако съществува, или създай нов (изключително полезно!)
product, created = Product.objects.get_or_create(
    name="Монитор Dell",
    defaults={"price": 450.00}  # Използва се само ако записът не съществува
)

# Вариант 4: Масово създаване на 1000 записа с 1 SQL заявка (Bulk Create)
Product.objects.bulk_create([
    Product(name="Кабел HDMI", price=15.00),
    Product(name="USB Хъб", price=35.00),
])
```

---

## 2. READ (ЧЕТЕНЕ, ФИЛТРИРАНЕ И ОПЕРАТОРИ `__`)

```python
# 1. Всички записи (връща QuerySet)
all_products = Product.objects.all()

# 2. Един запис (безопасно с .first() -> връща None ако го няма)
cheap_item = Product.objects.filter(price__lt=50).first()

# 3. Най-честите оператори за филтриране (Field Lookups):
Product.objects.filter(price__exact=100)        # Точно равно (= 100)
Product.objects.filter(price__gt=100)           # По-голямо (> 100)
Product.objects.filter(price__gte=100)          # По-голямо или равно (>= 100)
Product.objects.filter(price__lt=100)           # По-малко (< 100)
Product.objects.filter(price__range=(50, 200))  # Между 50 и 200 (BETWEEN)
Product.objects.filter(name__icontains="dell")  # Съдържа "dell" без значение главни/малки букви
Product.objects.filter(name__startswith="Pro")  # Започва с "Pro"
Product.objects.filter(id__in=)        # Намира се в списъка (IN (1, 5, 10))
Product.objects.exclude(price__gt=500)          # Всички БЕЗ тези с цена > 500 (NOT)
```

---

## 3. СЛОЖНИ УСЛОВИЯ С `Q` ОБЕКТИ И СОРТИРАНЕ

По подразбиране запетаята вътре във `.filter(a=1, b=2)` означава логическо **И (`AND`)**. За логическо **ИЛИ (`OR`)** и **НЕ (`NOT`)** се импортира класът **`Q`**:

```python
from django.db.models import Q

# Логическо ИЛИ (|) и отрицание (~)
results = Product.objects.filter(
    (Q(name__icontains="Dell") | Q(name__icontains="Apple")) & ~Q(price__gt=3000)
)

# Сортиране (order_by) и ограничаване (Slicing -> LIMIT / OFFSET)
top_5_expensive = Product.objects.order_by("-price", "name")[:5]
```

---

## 4. UPDATE И DELETE (ОБНОВЯВАНЕ И ИЗТРИВАНЕ)

```python
# 1. Обновяване на единичен обект
item = Product.objects.get(id=1)
item.price = 120.00
item.save()

# 2. Масово обновяване директно в SQL (много по-бързо!)
Product.objects.filter(price__lt=20).update(price=25.00)

# 3. Изтриване на единичен обект или група записи
item.delete()
Product.objects.filter(price=0).delete()
```
## 4.1. CRUD ОПЕРАЦИИ ПРИ ПРЕНОС В JSON И XML (СТЪПКА ПО СТЪПКА)

```python
import json
import xml.etree.ElementTree as ET
from django.core import serializers

# --- ЧАСТ А: Четене (Deserialize) от JSON / XML обратно в Django обекти ---
json_payload = '[{"model": "store.product", "pk": 1, "fields": {"name": "Монитор", "price": "450.00"}}]'

for deserialized_obj in serializers.deserialize("json", json_payload):
    product_instance = deserialized_obj.object  # Възстановява истински Django модел
    # product_instance.save()  # Записва го директно в SQL базата!

# --- ЧАСТ Б: Манипулация на XML дърво (ElementTree) ---
catalog = ET.Element("Catalog")
item = ET.SubElement(catalog, "Product", id="1")
price_tag = ET.SubElement(item, "Price")
price_tag.text = "450.00"

# Търсене (Read) и Обновяване (Update) чрез XPath
found = catalog.find("./Product[@id='1']/Price")
if found is not None:
    found.text = "420.00"

# Изтриване (Delete)
catalog.remove(item)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция | Синтаксис в Django ORM |
| :--- | :--- |
| **Създаване на запис** | `Model.objects.create(field=val)` |
| **Всички записи** | `Model.objects.all()` |
| **Безопасно взимане на 1 запис** | `Model.objects.filter(id=5).first()` |
| **Филтриране (`>`, `>=`, `<`, `<=`)** | `filter(age__gt=18)`, `filter(age__lte=65)` |
| **Търсене в текст (Case-insensitive)** | `filter(title__icontains="python")` |
| **Логическо ИЛИ (`OR`)** | `filter(Q(a=1) \| Q(b=2))` |
| **Сортиране (Низходящо)** | `order_by("-created_at")` |
| **Броене / Проверка за наличие** | `qs.count()` / `qs.exists()` |
| **Масов Update / Delete** | `qs.update(x=1)` / `qs.delete()` |

---
[⬅️ 04-03-00. Теория & Сравнение](04-03-00-CRUD-And-Queries.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-02. SQLAlchemy Syntax](04-03-02-SQLAlchemy-Syntax.md)
