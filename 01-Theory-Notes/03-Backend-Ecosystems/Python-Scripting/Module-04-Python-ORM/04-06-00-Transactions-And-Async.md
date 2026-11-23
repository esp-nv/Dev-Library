[⬅️ 04-05. Query Optimization](04-05-00-Optimization-And-N-Plus-1.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-01. Django ORM Syntax](04-06-01-Django-ORM-Syntax.md)

---

# 04-06-00. Transactions, Raw SQL and Async ORM (Теория, ACID, Deadlocks & Капани)

**Модул 04:** Python ORM | **Лекционна тема 06:** Атомарни трансакции (ACID), песимистично заключване на редове (`FOR UPDATE`), защита от SQL Injection при Raw SQL, асинхронен ORM (`async`/`await`) и NoSQL трансакции.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на една Атомарна Трансакция (Commit vs Rollback) и Блок-схема](#2-етапи-на-една-атомарна-трансакция-commit-vs-rollback-и-блок-схема)
* [3. Архитектурно сравнение: Django ORM, SQLAlchemy, C# EF Core и NoSQL/JSON/XML](#3-архитектурно-сравнение-django-orm-sqlalchemy-c-ef-core-и-nosqljsonxml)
* [4. Плюсове и минуси на Raw SQL и Async ORM](#4-плюсове-и-минуси-на-raw-sql-и-async-orm)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трансакцията е правилото „Всичко или Нищо“ (All or Nothing):** Представи си банков превод. Стъпка 1 вади 100 лв. от твоята сметка, Стъпка 2 ги добавя в сметката на получателя. Ако токът спре между Стъпка 1 и Стъпка 2, трансакцията прави **Rollback** (връща твоите 100 лв. обратно), за да не изчезнат парите във въздуха.
*   **Песимистично заключване (`FOR UPDATE`):** Когато четеш ред от базата с намерение да го промениш (например последният билет за концерт), слагаш табела „Заето“ върху този ред в SQL сървъра. Всички други заявки чакат на опашка, докато ти не приключиш с `COMMIT`.

---

## 2. ЕТАПИ НА ЕДНА АТОМАРНА ТРАНСАКЦИЯ (COMMIT VS ROLLBACK) И БЛОК-СХЕМА

```text
┌──────────────────────────┐
│ 1. BEGIN TRANSACTION     │
│ (with transaction.atomic │
│  или with session.begin) │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐     ГРЕШКА (Exception)   ┌─────────────────────────┐
│ 2. Изпълнява SQL заявки  │ ───────────────────────► │ ❌ ROLLBACK             │
│    (UPDATE, INSERT...)   │                          │ Отменя ВСИЧКИ промени   │
└────────────┬─────────────┘                          │ в блока автоматично!    │
             │ УСПЕХ (Няма грешки)                    └─────────────────────────┘
             ▼
┌──────────────────────────┐
│ ✅ COMMIT                │
│ Записва промените трайно │
│ на диска в SQL сървъра   │
└──────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO ORM, SQLALCHEMY, C# EF CORE И NOSQL/JSON/XML

| Операция | Django ORM | SQLAlchemy (v2.0+) | C# EF Core | NoSQL (Mongo/Redis) & JSON/XML |
| :--- | :--- | :--- | :--- | :--- |
| **Трансакционен блок** | `with transaction.atomic():` | `with session.begin():` | `await ctx.Database.BeginTransactionAsync()` | `session.start_transaction()` (Mongo) / `pipe = r.pipeline()` (Redis) |
| **Заключване на ред** | `.select_for_update()` | `.with_for_update()` | Raw SQL `WITH (UPDLOCK)` | Атомарно обновяване на 1 документ / `WATCH` (Redis) |
| **Безопасен Raw SQL** | `Model.objects.raw(sql, [p])` | `session.execute(text(sql), {"p": p})` | `ctx.Users.FromSqlInterpolated(...)` | Агрегации / Lua скриптове в Redis |
| **Асинхронно четене** | `await User.objects.aget(id=1)` | `await async_session.get(User, 1)` | `await ctx.Users.FindAsync(1)` | `await UserDoc.get(id)` (Beanie е 100% Async) |
| **Действие след Commit** | `transaction.on_commit(fn)` | Event listener `after_commit` | Код след `CommitAsync()` | Изпращане на JSON/XML webhook след запис |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА RAW SQL И ASYNC ORM

### 🟢 Предимства (Pros):
*   **Максимална скорост с Raw SQL:** При сложни финансови отчети с 10 `JOIN`-а и прозоречни функции (`WINDOW FUNCTIONS`), ръчно написаният SQL с параметри е по-бърз и по-ясен от опитите да го сглобиш през ORM.
*   **Висока пропускателна способност с Async ORM:** При уеб сървъри с хиляди едновременни потребители (FastAPI + Async SQLAlchemy), нишката не блокира докато чака отговор от базата данни.

### 🔴 Недостатъци (Cons):
*   **Риск от SQL Injection при грешен Raw SQL:** Ако слепваш променливи със стринг форматиране (`f"SELECT * FROM users WHERE name = '{user_input}'"`), отваряш критична врата за хакери.
*   **Скрит Lazy Loading срив при Async ORM:** В асинхронния SQLAlchemy достъпването на незаредена релация (`user.orders`) гърми веднага с грешка **`MissingGreenlet`**, защото Python не може да пусне скрита синхронна SQL заявка вътре в `async` функция!

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (SQL Injection уязвимост и хващане на грешка вътре в `atomic` блок):
```python
from django.db import transaction

# 🚨 ГРЕШКА 1: Фатална SQL Injection уязвимост чрез f-string!
user_input = "admin' OR '1'='1"
# bad_query = User.objects.raw(f"SELECT * FROM auth_user WHERE username = '{user_input}'")

# 🚨 ГРЕШКА 2: Улавяне на Exception ВЪТРЕ в transaction.atomic()!
with transaction.atomic():
    try:
        Order.objects.create(total=-50)  # Хвърля грешка в базата
    except Exception:
        # Уловка! Тъй като погълна грешката вътре, Django НЕ знае, че е станал срив
        # и ще се опита да направи COMMIT на счупена трансакция (TransactionManagementError)!
        print("Игнорирана грешка.")
```
*   **Как да се избегне:** При Raw SQL **винаги** подавай параметрите отделно (`params=[user_input]` или `:param`), никога с f-string. Никога не крий database грешки с `try/except` вътре в `transaction.atomic()` — сложи `try/except` **около** целия `with transaction.atomic():` блок!

### 🟢 ПРАВИЛЕН КОД (Атомарен банков превод със защита от Race Condition):
```python
from decimal import Decimal
from django.db import transaction

def transfer_funds(from_account_id: int, to_account_id: int, amount: Decimal) -> None:
    # try/except стои ИЗВЪН transaction.atomic()!
    try:
        with transaction.atomic():
            # Подреждаме ID-тата във възходящ ред, за да предотвратим Deadlock!
            first_id, second_id = sorted([from_account_id, to_account_id])
            
            # Заключваме двата реда в SQL с select_for_update() до края на трансакцията
            accounts = {
                acc.id: acc 
                for acc in Account.objects.select_for_update().filter(id__in=[first_id, second_id])
            }
            
            sender = accounts[from_account_id]
            receiver = accounts[to_account_id]

            if sender.balance < amount:
                raise ValueError("Недостатъчна наличност по сметката!")

            sender.balance -= amount
            receiver.balance += amount
            sender.save()
            receiver.save()
    except ValueError as err:
        print(f"[❌ ОТКАЗАН ПРЕВОД]: {err}")
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Как възниква Deadlock (Взаимно блокиране) при използване на `select_for_update()` в трансакции и как се предотвратява на архитектурно ниво?“*
*   **Уловката:** Кандидатите знаят, че `select_for_update()` заключва редове, но не се сещат какво става, когато двама потребители си превеждат пари един на друг едновременно.
*   **Точен Senior отговор:** 
    *   **Как възниква:** Потребител А праща пари на Потребител Б (Трансакция 1 заключва ред `ID=1` и чака да заключи ред `ID=2`). В същата милисекунда Потребител Б праща пари на Потребител А (Трансакция 2 заключва ред `ID=2` и чака да заключи ред `ID=1`). Двете трансакции чакат безкрайно една друга, докато базата данни не убие едната с грешка **`DeadlockDetected`**.
    *   **Как се решава (Lock Ordering):** Всички трансакции в системата трябва да заключват ресурсите в **строго еднакъв, сортиран ред** (напр. винаги първо по-малкото ID: `sorted([id_a, id_b])`). Така и двете трансакции първо ще поискат ключ `ID=1` — едната ще го вземе, а другата ще изчака спокойно на опашката, без никога да се стигне до взаимно блокиране!

---
[⬅️ 04-05. Query Optimization](04-05-00-Optimization-And-N-Plus-1.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-06-01. Django ORM Syntax](04-06-01-Django-ORM-Syntax.md)
