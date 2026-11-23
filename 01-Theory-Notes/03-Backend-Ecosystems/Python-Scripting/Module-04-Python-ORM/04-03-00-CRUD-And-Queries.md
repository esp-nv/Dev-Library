[⬅️ 04-02. Migrations and Schema](04-02-00-Migrations.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-01. Django ORM Syntax](04-03-01-Django-ORM-Syntax.md)

---

# 04-03-00. CRUD Operations and Queries (Теория, `QuerySet` срещу `Session` & Капани)

**Модул 04:** Python ORM | **Лекционна тема 03:** Четирите основни операции с данни (Create, Read, Update, Delete), филтриране, сортиране, сложни логически условия (`Q` обекти срещу `and_`/`or_`) и масови заявки (Bulk Operations).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на изпълнение на заявка (Lazy Evaluation) и Блок-схема](#2-етапи-на-изпълнение-на-заявка-lazy-evaluation-и-блок-схема)
* [3. Архитектурно сравнение: Django ORM срещу SQLAlchemy и C# EF Core](#3-архитектурно-сравнение-django-orm-срещу-sqlalchemy-и-c-ef-core)
* [3.1. АРХИТЕКТУРНО СРАВНЕНИЕ: CRUD В NOSQL (MONGODB / REDIS), JSON И XML](#31-архитектурно-сравнение-crud-в-nosql-mongodb--redis-json-и-xml)
* [4. Плюсове и минуси на двата подхода за заявки](#4-плюсове-и-минуси-на-двата-подхода-за-заявки)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Двете долни черти (`__`) в Django = Оператори за сравнение:** В Django ORM не можеш да напишеш `filter(age > 18)` (това е невалиден Python синтаксис за аргументи). Вместо знаци, Django слага **две долни черти** след името на колоната:
    *   `age__gt=18` (*Greater Than* ➔ `> 18`)
    *   `age__gte=18` (*Greater Than or Equal* ➔ `>= 18`)
    *   `age__lt=18` (*Less Than* ➔ `< 18`)
    *   `name__icontains="eli"` (*Case-Insensitive Contains* ➔ `ILIKE '%eli%'`)
*   **SQLAlchemy 2.0 = Пишеш истински SQL на Python:** В SQLAlchemy пишеш изрази, които приличат на самия SQL: `select(User).where(User.age > 18)`.

---

## 2. ЕТАПИ НА ИЗПЪЛНЕНИЕ НА ЗАЯВКА (LAZY EVALUATION) И БЛОК-СХЕМА

```text
DJANGO ORM ПОТОК:
[ Product.objects.filter(...) ] ──► [ .order_by(...) ] ──► (Още НЯМА SQL заявка!)
                                                                  │
                                          Изпълнява се ЧАК при:   ▼
                                   [ for p in qs: / list(qs) / qs.first() ] ──► SQL SELECT

SQLALCHEMY 2.0 ПОТОК:
[ stmt = select(Product).where(...) ] ──► (Само SQL чертеж в паметта)
                                                 │
                                                 ▼
                           [ session.execute(stmt).scalars().all() ] ──► SQL SELECT
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO ORM СРЕЩУ SQLALCHEMY И C# EF CORE

| CRUD Операция | Django ORM | SQLAlchemy (v2.0+) | C# Entity Framework Core |
| :--- | :--- | :--- | :--- |
| **Create (Създаване)** | `User.objects.create(name="A")` | `session.add(User(name="A")); session.commit()` | `ctx.Users.Add(u); ctx.SaveChanges();` |
| **Read All (Всички)** | `User.objects.all()` | `session.scalars(select(User)).all()` | `ctx.Users.ToList()` |
| **Read One (По ID)** | `User.objects.get(id=1)` | `session.get(User, 1)` | `ctx.Users.Find(1)` |
| **Filter (Филтриране)** | `User.objects.filter(age__gte=18)` | `select(User).where(User.age >= 18)` | `ctx.Users.Where(u => u.Age >= 18)` |
| **OR условие (ИЛИ)** | `filter(Q(a=1) \| Q(b=2))` | `where(or_(User.a == 1, User.b == 2))` | `Where(u => u.A == 1 \|\| u.B == 2)` |
| **Update (Масово)** | `User.objects.filter(...).update(x=1)` | `session.execute(update(User).values(x=1))` | `ctx.Users.ExecuteUpdate(...)` |
| **Delete (Масово)** | `User.objects.filter(...).delete()` | `session.execute(delete(User).where(...))` | `ctx.Users.ExecuteDelete()` |


## 3.1. АРХИТЕКТУРНО СРАВНЕНИЕ: CRUD В NOSQL (MONGODB / REDIS), JSON И XML

| CRUD Операция | MongoDB (Beanie ODM) | Redis (`redis-py` + JSON) | JSON (`json` / Pydantic) | XML (`ElementTree`) |
| :--- | :--- | :--- | :--- | :--- |
| **Create (Създаване)** | `await prod.insert()` | `r.set("prod:1", json_str)` | `json.dumps(data)` | `ET.SubElement(root, "item")` |
| **Read One (По ключ/ID)** | `await Product.get(id)` | `json.loads(r.get("prod:1"))` | `data.get("id")` | `root.find("./item[@id='1']")` |
| **Read Filter (Търсене)** | `Product.find(Product.price >= 100)` | `r.scan_iter("prod:*")` | `[x for x in data if x["price"] >= 100]` | `root.findall(".//item")` |
| **Update (Обновяване)** | `await prod.set({"price": 120})` | `r.set("prod:1", new_json)` | `data["price"] = 120` | `el.text = "120"` |
| **Delete (Изтриване)** | `await prod.delete()` | `r.delete("prod:1")` | `del data["price"]` | `root.remove(el)` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ДВАТА ПОДХОДА ЗА ЗАЯВКИ

### 🟢 Django ORM — Предимства и Недостатъци:
*   **Предимства:** Изключително кратък и четим синтаксис; автоматично кеширане на резултатите вътре в `QuerySet` при повторно обхождане; лесно преминаване през релации с `__` (напр. `Order.objects.filter(user__city="Sofia")`).
*   **Недостатъци:** Синтаксисът с двойни долни черти (`__gte`, `__isnull`) не се проверява от редактора за правописни грешки, докато не пуснеш кода.

### 🔵 SQLAlchemy 2.0 — Предимства и Недостатъци:
*   **Предимства:** Използва реални класови атрибути (`User.age >= 18`), което дава пълно автодопълнение и проверка за грешки във VS Code; поддържа Identity Map (ако заредиш обект с `id=5` два пъти в една сесия, връща същия обект от RAM без втора SQL заявка).
*   **Недостатъци:** По-многословен синтаксис (`session.execute(select(...)).scalars().all()`).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Използване на `.get()` без защита и `len(queryset)` за броене):
```python
# 🚨 ГРЕШКА 1: .get() гърми с DoesNotExist, ако записът липсва, или MultipleObjectsReturned, ако са 2!
# user = User.objects.get(email="missing@bg.com") -> CRASH!

# 🚨 ГРЕШКА 2: Използване на len() върху QuerySet вместо .count()!
# len(User.objects.all()) изтегля ВСИЧКИ 1 милион реда от базата в RAM паметта, само за да ги преброи!
total_users = len(User.objects.all())
```
*   **Как да се избегне:** Когато търсиш 1 обект и не си сигурна дали съществува, ползвай **`.filter(...).first()`** (връща обекта или `None` без да гърми). За броене **винаги** ползвай **`.count()`** (изпълнява бърз `SELECT COUNT(*)` директно в SQL сървъра).

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт за безопасни заявки):
```python
# --- DJANGO ORM ПРИМЕР ---
from django.db.models import Q, F

# 1. Безопасно търсене на един запис (връща None ако липсва)
admin_user = User.objects.filter(email="admin@softuni.bg").first()

# 2. Сложно ИЛИ (OR) условие с Q обекти + директно броене в SQL
vip_count = User.objects.filter(
    Q(is_staff=True) | Q(points__gte=1000)
).count()

# 3. Атомарно увеличаване на стойност директно в базата чрез F() (избягва Race Condition!)
User.objects.filter(id=1).update(points=F("points") + 50)
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво представляват `F()` изразите в Django ORM (и директните SQL изрази в SQLAlchemy) и как предпазват от Race Condition при едновременни заявки?“*
*   **Уловката:** Кандидатите често пишат `product.stock -= 1; product.save()`, което при 100 едновременни поръчки води до грешна наличност!
*   **Точен Senior отговор:** Ако двама потребители купят последния лаптоп в една и съща милисекунда:
    *   При `product.stock -= 1; product.save()`, и двата сървъра прочитат `stock = 1` в Python паметта, вадят `1` и записват `0` — продали сме 2 лаптопа, а сме имали само 1!
    *   Когато използваме **`F("stock") - 1`** (`Product.objects.filter(id=1).update(stock=F("stock") - 1)`), Python изобщо не зарежда числото в паметта си, а изпраща директна SQL инструкция: **`UPDATE products SET stock = stock - 1 WHERE id = 1`**. Базата данни заключва реда и изпълнява операцията атомарно, гарантирайки 100% точност.

---
[⬅️ 04-02. Migrations and Schema](04-02-00-Migrations.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-01. Django ORM Syntax](04-03-01-Django-ORM-Syntax.md)
