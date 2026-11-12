[⬅️ 06-04. Django REST Framework](06-04-00-Django-REST-Framework.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-05-01. Syntax From Scratch](06-05-01-Syntax-From-Scratch.md)

---

# 06-05-00. Asynchronous Processing with Celery and Redis (Теория, Brokers, Beat & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 05:** Разпределени опашки за задачи (Distributed Task Queues), архитектура Producer-Broker-Worker, Redis като Message Broker и Cache, планиране на периодични задачи с Celery Beat и капани при сериализация на аргументи.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Celery + Redis и Блок-схема](#2-архитектура-на-celery--redis-и-блок-схема)
* [3. Архитектурно сравнение: Celery + Redis срещу C# и Node.js](#3-архитектурно-сравнение-celery--redis-срещу-c-и-nodejs)
* [4. Плюсове и минуси на Celery и Redis](#4-плюсове-и-минуси-на-celery-и-redis)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Ресторант в час пик (Producer ➔ Broker ➔ Worker):**
    *   **Django (Producer — Сервитьорът):** Приема поръчката от клиента и я слага на щипката на бара, за да се върне веднага при другите гости.
    *   **Redis (Message Broker — Щипката за поръчки):** Държи списъка с чакащите бележки в светкавичната RAM памет.
    *   **Celery Worker (Готвачът в кухнята):** Отделен процес (или друг сървър), който взима бележките от щипката една по една и готви тежките ястия (праща имейли, генерира PDF, обработва видеа), без да бави сервитьора.
    *   **Celery Beat (Будилникът):** Часовник, който сам пуска задачи по разписание (напр. *„Всяка нощ в 02:00 ч. изтрий старите сесии и прати отчет“*).

---

## 2. АРХИТЕКТУРА НА CELERY + REDIS И БЛОК-СХЕМА

```text
┌──────────────────────────┐  1. task.delay(order_id) ┌─────────────────────────┐
│ Django View (Producer)   │ ───────────────────────► │ Redis (Message Broker)  │
│ Веднага връща 202 HTTP   │   (Праща JSON съобщение) │ Опашка в RAM паметта    │
└──────────────────────────┘                          └────────────┬────────────┘
                                                                   │ 2. Взима задачата
                                                                   ▼
┌──────────────────────────┐  3. Записва резултата    ┌─────────────────────────┐
│ Result Backend           │ ◄─────────────────────── │ Celery Worker (Процес)  │
│ (Redis / PostgreSQL)     │   (SUCCESS / FAILURE)    │ Изпълнява тежкия код    │
└──────────────────────────┘                          └─────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: CELERY + REDIS СРЕЩУ C# И NODE.JS

| Компонент | Python / Django Екосистема | C# / .NET Екосистема | Node.js / TypeScript Екосистема |
| :--- | :--- | :--- | :--- |
| **Библиотека за опашки** | **Celery** (индустриален стандарт) | **Hangfire** / MassTransit / Quartz.NET | **BullMQ** / Agenda |
| **Брокер на съобщения** | **Redis** или RabbitMQ | Redis / RabbitMQ / SQL Server / Azure Service Bus | **Redis** |
| **Периодични задачи (Cron)** | **Celery Beat** (`django-celery-beat`) | `RecurringJob.AddOrUpdate()` (Hangfire) | Repeatable Jobs в BullMQ / `node-cron` |
| **Викане на задача** | `my_task.delay(arg1)` или `.apply_async()` | `BackgroundJob.Enqueue(() => Fn())` | `await myQueue.add('jobName', data)` |
| **Кеширане (Cache)** | `django.core.cache` + Redis | `IDistributedCache` + Redis | `ioredis` / `cache-manager` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА CELERY И REDIS

### 🟢 Предимства (Pros):
*   **Автоматично повторение при срив (Retries):** Ако външното платежно API или имейл сървърът падне временно, Celery може автоматично да опита задачата отново след 60 секунди (`autoretry_for`, `max_retries=3`).
*   **Хоризонтално скалиране:** Ако опашката набъбне, просто пускаш още 5 Celery Worker-а на други машини и те веднага започват да поделят работата.

### 🔴 Недостатъци (Cons):
*   **Трудна поддръжка под Windows:** Celery официално не поддържа Windows за продукция (изисква флаг `--pool=solo` при локална разработка, докато на Linux работи нативно с `prefork`).
*   **Допълнителни процеси за наблюдение:** Освен Django сървъра, трябва да следиш дали Redis и Celery Worker процесите са живи (често се ползва уеб инструментът **Flower** за мониторинг).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Подаване на цял Django ORM обект в `.delay()` и липсващ `on_commit`!):
```python
from celery import shared_task
from django.db import transaction

@shared_task
def process_order_bad(order_object):
    # 🚨 ГРЕШКА 1: Подаваш цял Django модел обект в .delay(order)!
    # Redis приема само JSON (числа, текстове, списъци) -> гърми с EncodeError: Object of type Order is not JSON serializable!
    pass

def checkout_view_bad(request):
    with transaction.atomic():
        order = Order.objects.create(total=500)
        # 🚨 ГРЕШКА 2 (Race Condition!): Викаш .delay() ВЪТРЕ в transaction.atomic() ПРЕДИ COMMIT!
        # Бързият Celery Worker може да хване задачата 1 милисекунда ПРЕДИ базата да запише поръчката
        # и да гръмне с Order.DoesNotExist!
        process_order_bad.delay(order.id)
```
*   **Как да се избегне:** В `.delay()` подавай **само прости типове** (като `order.id: int`), а вътре в задачата зареди обекта от базата. Винаги обвивай `.delay()` в **`transaction.on_commit(lambda: my_task.delay(order.id))`**!

### 🟢 ПРАВИЛЕН КОД (Idempotent Celery задача с Retry и `on_commit`):
```python
from celery import shared_task
from django.db import transaction
from .models import Order

# 1. Във файла orders/tasks.py
@shared_task(bind=True, max_retries=3, default_retry_delay=30)
def send_order_receipt_task(self, order_id: int) -> str:
    try:
        order = Order.objects.get(id=order_id)
        # Симулация на изпращане на имейл / фактура
        print(f"Изпратена разписка за поръчка #{order.id} ({order.total} лв.)")
        return f"Done #{order.id}"
    except Exception as exc:
        # Автоматичен повторен опит след 30 секунди при мрежова грешка
        raise self.retry(exc=exc)

# 2. Във файла orders/views.py
def checkout_view_clean(request):
    with transaction.atomic():
        order = Order.objects.create(total=500)
        # Задачата влиза в Redis ЧАК СЛЕД като PostgreSQL потвърди записа!
        transaction.on_commit(lambda: send_order_receipt_task.delay(order.id))
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво означава една Celery задача да бъде **Idempotent (Идемпотентна)** и защо това е критично изискване в разпределените системи?“*
*   **Уловката:** В реални системи мрежата може да прекъсне точно след като Celery Worker е изпълнил задачата, но преди да каже на Redis „Готов съм“. Тогава Redis ще прати същата задача на втори Worker!
*   **Точен Senior отговор:** **Idempotency (Идемпотентност)** означава, че ако една и съща задача бъде изпълнена **1 път или 5 пъти подред със същите аргументи**, крайният резултат в системата ще бъде абсолютно еднакъв, без странични щети.
    *   *Пример:* Ако задачата просто прави `user.balance += 100`, при повторно изпълнение клиентът ще получи 200 лв. вместо 100 лв.!
    *   *Решение:* Всяка задача проверява статуса на трансакцията преди действие (напр. `if Payment.objects.filter(transaction_id=tx_id, status="COMPLETED").exists(): return`), гарантирайки, че парите или имейлът се обработват строго веднъж.

---
[⬅️ 06-04. Django REST Framework](06-04-00-Django-REST-Framework.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-05-01. Syntax From Scratch](06-05-01-Syntax-From-Scratch.md)
