[⬅️ 04-05. State Management (Cookies & HttpSession)](04-05-00-State-Management.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-06-01. Syntax From Scratch](04-06-01-Syntax-From-Scratch.md)

---

# 04-06-00. Spring Essentials (Events, Scheduling `@Scheduled` & Caching — Теория & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 06:** Разкачване на бизнес процесите чрез вътрешни събития (**Spring Application Events** с `ApplicationEventPublisher`, `@EventListener` и `@TransactionalEventListener`), фонови периодични задачи с **`@Scheduled`** (`fixedRate`, `fixedDelay`, `cron`), асинхронно изпълнение с **`@Async`**, кеширане на скъпи заявки с **`@Cacheable`** / **`@CacheEvict`** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Spring Events & Транзакционни слушатели (Блок-схема)](#2-архитектура-на-spring-events--транзакционни-слушатели-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot, ASP.NET Core, Python Celery/FastAPI, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-aspnet-core-python-celeryfastapi-nestjs)
* [4. Плюсове и минуси на Spring Events, Scheduling и Caching](#4-плюсове-и-минуси-на-spring-events-scheduling-и-caching)
* [5. Кодов одит: Правилен срещу Грешен код (Self-Invocation капанът на Spring AOP Proxy)](#5-кодов-одит-правилен-срещу-грешен-код-self-invocation-капанът-на-spring-aop-proxy)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot 3) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot-3-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Spring Events (`Observer Pattern` в контейнера):** Вместо `UserService` да инжектира 5 други сървиса (`EmailService`, `LoyaltyPointsService`, `AuditLogService`) при регистрация, той просто вика по високоговорителя: **`eventPublisher.publishEvent(new UserRegisteredEvent(user))`**. Всеки заинтересован слушател (`@EventListener`) чува съобщението и си върши работата независимо!
*   **`fixedRate` срещу `fixedDelay` при `@Scheduled`:**
    *   **`fixedRate = 5000` (Часовник с аларма):** Стартира задачата на всеки 5 секунди от **началото** на предишното стартиране (дори предишната още да не е свършила — опасно при дълги задачи!).
    *   **`fixedDelay = 5000` (Почивка след работа):** Изчаква задачата да **приключи напълно**, пуска хронометъра за 5 секунди пауза и чак тогава я пуска отново (много по-безопасно за заявки към бази данни!).
*   **Трите златни анотации за кеширане:**
    *   **`@Cacheable("products")`:** Първо проверява в кеша — ако го има, връща го мигновено без да влиза в метода; ако го няма, изпълнява метода и записва резултата в кеша.
    *   **`@CachePut("products")`:** Винаги изпълнява метода и **обновява** записа в кеша.
    *   **`@CacheEvict("products")`:** **Изтрива** остарелия запис от кеша при промяна или триене на данните.

---

## 2. АРХИТЕКТУРА НА SPRING EVENTS & ТРАНЗАКЦИОННИ СЛУШАТЕЛИ (БЛОК-СХЕМА)

```text
[ UserService.register() (@Transactional) ]
       │  1. Записва потребителя в PostgreSQL
       │  2. eventPublisher.publishEvent(new UserRegisteredEvent(userId, email))
       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Spring Event Multicaster                                                │
├────────────────────────────────────┬────────────────────────────────────┤
│ @EventListener (Стандартен)        │ @TransactionalEventListener        │
│ • Изпълнява се ВЕДНАГА в същата    │   (phase = AFTER_COMMIT) ✅        │
│   нишка и транзакция!              │ • Чака базата да потвърди записа!  │
│ • Ако имейлът гръмне -> Rollback!  │ • Не праща имейл при неуспех!      │
└────────────────────────────────────┴────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT, ASP.NET CORE, PYTHON CELERY/FASTAPI, NESTJS)

| Концепция | ☕ Java 21+ (Spring Boot 3) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (FastAPI / Celery) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Вътрешни събития (Pub/Sub)** | `ApplicationEventPublisher` + `@EventListener` | **MediatR** (`INotification` / `Publish`) | Django Signals / Blinker | `@nestjs/event-emitter` (`@OnEvent`) |
| **Периодични задачи (Cron)** | `@EnableScheduling` + `@Scheduled(cron=...)` | `BackgroundService` / **Hangfire** / Quartz.NET | **Celery Beat** / `APScheduler` | `@nestjs/schedule` (`@Cron(...)`) |
| **Асинхронен метод** | `@EnableAsync` + `@Async` (`CompletableFuture`) | `async Task` / `BackgroundService` | `BackgroundTasks` (FastAPI) / Celery | `async` / BullMQ опашки |
| **Кеширане на метод** | `@EnableCaching` + `@Cacheable("key")` | `IMemoryCache` / `IDistributedCache` / HybridCache | `@lru_cache` / `fastapi-cache2` | `@nestjs/cache-manager` (`@CacheKey`) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SPRING EVENTS, SCHEDULING И CACHING

### 🟢 Предимства (Pros):
*   **Пълна декларативност без замърсяване на бизнес логиката:** Слагаш една анотация `@Cacheable` или `@Scheduled` и Spring сам управлява кеша, нишките и таймерите.
*   **Силно разкачване чрез Events:** Можеш да добавяш нови реакции (напр. изпращане на SMS, нотификация в Slack, начисляване на бонус точки) без да променяш нито един ред в `OrderService`.

### 🔴 Недостатъци (Cons):
*   **Скрито поведение при AOP Proxy (Self-Invocation):** Всички тези анотации (`@Async`, `@Cacheable`, `@Transactional`) работят чрез **Spring AOP Proxy** — ако метод A в същия клас извика метод B с `@Cacheable` или `@Async` директно (`this.methodB()`), анотацията **няма да сработи изобщо**!
*   **Клъстерен капан при `@Scheduled`:** Ако имаш 3 работещи инстанции (Pods) на приложението в Kubernetes, `@Scheduled` задачата ще се изпълни **3 пъти едновременно**, освен ако не използваш заключване в базата данни (**ShedLock**).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (SELF-INVOCATION КАПАНЪТ НА SPRING AOP PROXY)

### ❌ ГРЕШЕН КОД (Извикване на `@Async` или `@Cacheable` метод вътре в същия клас чрез `this`):
```java
@Service
public class BadReportService {

    public void generateAndEmailReport(Long userId) {
        String data = fetchHeavyData(userId);
        // 🚨 ГРЕШКА 1 (Self-Invocation капан): Викаш this.sendEmailAsync() вътре в същия клас!
        // Повикването минава директно през 'this', заобикаляйки Spring Proxy обвивката ->
        // Методът се изпълнява СИНХРОННО (блокира потребителя), а @Async се игнорира напълно!
        this.sendEmailAsync(userId, data);
    }

    @Async
    public void sendEmailAsync(Long userId, String data) {
        // Бавно изпращане на имейл...
    }
}
```
*   **Как да се избегне:** Изнасяй `@Async` и `@Cacheable` методите в **отделен `@Service` / `@Component` клас** и ги инжектирай чрез конструктора, така че извикването винаги да минава през Spring Proxy обекта.

### 🟢 ПРАВИЛЕН КОД (Разделени Beans + `@TransactionalEventListener(phase = AFTER_COMMIT)`):
```java
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Component;
import org.springframework.transaction.event.TransactionPhase;
import org.springframework.transaction.event.TransactionalEventListener;

// 1. Неизменяемо събитие като Java record
public record OrderPlacedEvent(Long orderId, String customerEmail) {}

// 2. Слушател в отделен компонент: изпраща имейл АСИНХРОННО и САМО СЛЕД успешен запис в БД!
@Component
public class OrderNotificationListener {

    @Async
    @TransactionalEventListener(phase = TransactionPhase.AFTER_COMMIT)
    public void handleOrderPlaced(OrderPlacedEvent event) {
        System.out.printf("📧 Асинхронно изпращане на потвърждение за поръчка #%d до %s%n",
                event.orderId(), event.customerEmail());
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между обикновен **`@EventListener`** и **`@TransactionalEventListener`** в Spring, и защо при регистрация на потребител трябва да използваме втория?“*
*   **Уловката:** Ако използваш обикновен `@EventListener` за изпращане на имейл за добре дошъл, потребителят може да получи имейл, дори когато регистрацията му в базата данни се е провалила!
*   **Точен Senior отговор:** 
    1.  Обикновеният **`@EventListener`** се изпълнява **синхронно в момента на извикване на `publishEvent()`** — още **преди** транзакцията на `UserService` да е направила `COMMIT` в базата данни. Ако на следващия ред в `UserService` възникне грешка и транзакцията направи `ROLLBACK`, потребителят няма да го има в базата, но имейлът вече ще е изпратен!
    2.  **`@TransactionalEventListener(phase = TransactionPhase.AFTER_COMMIT)`** изчаква текущата транзакция да завърши успешно с **`COMMIT`** в базата данни и **чак тогава** стартира слушателя! *(Допълнителен Senior детайл: Ако вътре в `AFTER_COMMIT` слушател правиш нов запис в базата данни, трябва да добавиш `@Transactional(propagation = Propagation.REQUIRES_NEW)`, защото старата транзакция вече е приключила!)*

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT 3) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Механизъм на аспектите (`@Cacheable`, `@Async`)** | **Runtime Dynamic Proxies** (CGLIB / JDK Proxy — страда от Self-Invocation) | Изрични C# езикови конструкции (`async/await`) и Decorators в DI |
| **Пул от нишки за `@Scheduled`** | По подразбиране **само 1 нишка** за всички `@Scheduled` задачи! (настройва се с `spring.task.scheduling.pool.size=5`) | Всеки `BackgroundService` има собствен асинхронен цикъл върху ThreadPool |
| **Асинхронно изпълнение** | ``@Async`` върху метод с ``CompletableFuture<T>`` или **Virtual Threads** (Java 21: ``spring.threads.virtual.enabled=true)`` | Нативен ``async Task<T>`` навсякъде в стека |
| **Кеширане с Redis** | ``spring-boot-starter-data-redis`` + ``@Cacheable`` | ``Microsoft.Extensions.Caching.StackExchangeRedis`` / ``HybridCache`` (.NET 9) |
| **Разпределено заключване на Cron задачи** | Библиотека ShedLock (``@SchedulerLock``) | **Hangfire** или Distributed Lock в Redis / PostgreSQL |

---
[⬅️ 04-05. State Management (Cookies & HttpSession)](04-05-00-State-Management.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-06-01. Syntax From Scratch](04-06-01-Syntax-From-Scratch.md)