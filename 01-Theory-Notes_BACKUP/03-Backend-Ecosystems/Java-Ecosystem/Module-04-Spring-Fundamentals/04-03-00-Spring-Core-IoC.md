[⬅️ 04-02. Spring Boot Intro & Layered Architecture](04-02-00-Spring-Boot-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-03-01. Syntax From Scratch](04-03-01-Syntax-From-Scratch.md)

---

# 04-03-00. Spring Core (IoC Container, Beans & Dependency Injection — Теория & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 03:** Сърцето на Spring Framework — **IoC (Inversion of Control) Container** (`ApplicationContext`), какво е **Spring Bean**, трите вида Dependency Injection (Constructor, Setter, Field), шестте **Bean Scopes** (`singleton`, `prototype`, `request`, `session`, `application`, `websocket`), жизнения цикъл на Bean (`@PostConstruct` / `@PreDestroy`), разрешаване на конфликти (`@Primary` / `@Qualifier`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Жизнен цикъл на един Spring Bean (Lifecycle Блок-схема)](#2-жизнен-цикъл-на-един-spring-bean-lifecycle-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring DI, .NET DI, FastAPI Depends, NestJS DI)](#3-сравнителна-карта-в-4-езика-spring-di-net-di-fastapi-depends-nestjs-di)
* [4. Плюсове и минуси на Singleton по подразбиране в Spring](#4-плюсове-и-минуси-на-singleton-по-подразбиране-в-spring)
* [5. Кодов одит: Правилен срещу Грешен код (Състояние в Singleton Bean и Circular Dependencies)](#5-кодов-одит-правилен-срещу-грешен-код-състояние-в-singleton-bean-и-circular-dependencies)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Core) срещу C# 12+ (.NET DI)](#7-архитектурно-сравнение-java-21-spring-core-срещу-c-12-net-di)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Обикновен Java обект срещу Spring Bean:**
    *   Когато напишеш `UserService s = new UserService();`, ти си го създала и ти отговаряш за него — това е обикновен обект (**POJO**), за който Spring изобщо не знае (и не може да приложи `@Transactional` или `@Cacheable` върху него!).
    *   **Spring Bean** е обект, който е създаден, сглобен с нужните зависимости и управляван от **Spring IoC контейнера (`ApplicationContext`)**.
*   **Главните два Bean Scope-а:**
    *   **`singleton` (По подразбиране):** Контейнерът създава **точно 1 обект** при стартиране и го подава на всички класове, които го поискат. Затова `@Service` и `@Repository` класовете **НИКОГА не трябва да имат изменяеми полета (State)**!
    *   **`prototype`:** Контейнерът създава **чисто нов обект** всеки път, когато някой го поиска от контекста (и след създаването му спира да следи кога ще бъде унищожен!).

---

## 2. ЖИЗНЕН ЦИКЪЛ НА ЕДИН SPRING BEAN (LIFECYCLE БЛОК-СХЕМА)

```text
1. Instantiate       ➔ Spring извиква конструктора: new OrderService(orderRepo)
       │
       ▼
2. Populate Props    ➔ Инжектира @Value и Setter зависимостите (ако има такива)
       │
       ▼
3. @PostConstruct    ➔ Изпълнява метода за начална подготовка (напр. зареждане на кеш)
       │
       ▼
4. Ready in Context  ➔ Bean-ът е готов за работа в ApplicationContext (обвит в Proxy, ако има @Transactional)
       │
       ▼  (При спиране на сървъра)
5. @PreDestroy       ➔ Изпълнява метода за почистване (затваряне на ресурси/нишки)
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING DI, .NET DI, FASTAPI DEPENDS, NESTJS DI)

| Концепция | ☕ Java 21+ (Spring Core) | 🟣 C# 12+ (.NET DI) | 🐍 Python 3.x (FastAPI) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **IoC Контейнер** | `ApplicationContext` | `IServiceProvider` | FastAPI Dependency Graph | NestJS `Module` Container |
| **Регистрация на клас** | `@Service` / `@Component` | `services.AddSingleton<T>()` | Функция с `Depends(get_db)` | `@Injectable()` декоратор |
| **Фабричен метод за Bean** | `@Bean` вътре в `@Configuration` | `services.AddScoped(sp => ...)` | Generator функция с `yield` | `useFactory: (...) => ...` |
| **Разрешаване на конфликт** | `@Primary` или `@Qualifier("name")` | Keyed Services `[FromKeyedServices]` | Отделни функции за зависимост | `@Inject('TOKEN_NAME')` |
| **Жизнен цикъл (Init/Destroy)** | `@PostConstruct` / `@PreDestroy` | `IHostedService` / `IDisposable` | `lifespan` async context manager | `OnModuleInit` / `OnModuleDestroy` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SINGLETON ПО ПОДРАЗБИРАНЕ В SPRING

### 🟢 Предимства (Pros):
*   **Нулев натиск върху Garbage Collector-а:** При 10 000 HTTP заявки в секунда Spring не създава 10 000 копия на `UserController`, `UserService` и `UserRepository` — всички нишки споделят едни и същи вече инициализирани инстанции в паметта.

### 🔴 Недостатъци (Cons):
*   **Опасност от Race Condition при невнимателно програмиране:** Ако разработчик сложи променяемо поле (например `private List<String> tempItems = new ArrayList<>();`) в `@Service` клас, двама потребители, влезли едновременно в сайта, ще смесят данните си един с друг!

---
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (СЪСТОЯНИЕ В SINGLETON BEAN И CIRCULAR DEPENDENCIES)

### ❌ ГРЕШЕН КОД (Споделено променяемо състояние в `@Service` и `NoUniqueBeanDefinitionException`):
```java
@Service
public class BadShoppingCartService {
    // 🚨 ГРЕШКА 1 (Критичен бъг при много потребители!):
    // @Service е SINGLETON! Всички потребители на сайта споделят този единствен списък!
    // Потребител Б ще види и купи продуктите на Потребител А!
    private final List<String> currentCartItems = new ArrayList<>();

    public void addItem(String item) {
        currentCartItems.add(item);
    }
}

// 🚨 ГРЕШКА 2: Имаш два класа (CardPayment и CashPayment), имплементиращи PaymentService.
// Ако инжектираш просто 'PaymentService payment' без @Primary или @Qualifier,
// Spring Boot гърми при стартиране с NoUniqueBeanDefinitionException!
```
*   **Как да се избегне:** Държи всички `@Service` класове **100% Stateless** (само `private final` референции към други сървиси/репозиторита; данните на потребителя пътуват като аргументи на методите или в сесия/БД). При две имплементации на един интерфейс сложи **`@Primary`** на основната или **`@Qualifier("beanName")`** в конструктора.

### 🟢 ПРАВИЛЕН КОД (Stateless `@Service` и разрешаване на конфликт с `@Qualifier`):
```java
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Service;

@Service
public class CheckoutProcessor {

    private final PaymentGateway primaryGateway;
    private final PaymentGateway backupGateway;

    // Избираме точните Bean-ове по име чрез @Qualifier!
    public CheckoutProcessor(@Qualifier("stripeGateway") PaymentGateway primaryGateway,
                             @Qualifier("paypalGateway") PaymentGateway backupGateway) {
        this.primaryGateway = primaryGateway;
        this.backupGateway = backupGateway;
    }

    // Данните се подават като параметри -> напълно безопасно за 1000 едновременни нишки!
    public void processOrder(String orderId, double amount) {
        primaryGateway.charge(orderId, amount);
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Ако инжектираш Bean със Scope **`prototype`** директно в конструктора на Bean със Scope **`singleton`** (напр. `@Service`), колко пъти ще се създаде `prototype` обектът?“*
*   **Уловката:** Повечето кандидати казват *„Всеки път, когато извикам метода на `@Service`“* — и грешат!
*   **Точен Senior отговор:** `Prototype` обектът ще се създаде **САМО 1 ПЪТ** (при стартиране на приложението)! Причината е, че конструктурът на `singleton` Bean-а се извиква само веднъж при сглобяването на контейнера — той получава тази една инстанция на `prototype` обекта и я държи завинаги (**Captive Dependency Problem**).
    *   **Как се решава правилно:** В конструктора на Singleton-а инжектирай **`ObjectProvider<MyPrototypeBean>`** и вътре в метода викай **`prototypeProvider.getObject()`**, за да получаваш свежа инстанция при всяко извикване!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING CORE) СРЕЩУ C# 12+ (.NET DI)

| Архитектурен детайл | ☕ Java 21+ (Spring Core) | 🟣 C# 12+ (`Microsoft.Extensions.DI`) |
| :--- | :--- | :--- |
| **Три главни Lifetimes / Scopes** | `singleton` (default), `prototype`, `request` / `session` | `Singleton`, `Transient` (като `prototype`), `Scoped` (1 на HTTP заявка) |
| **Защита от Captive Dependency** | Изисква `ObjectProvider<T>` или `@Scope(..., proxyMode = TARGET_CLASS)` | Вградена **Scope Validation** (хвърля грешка при старт, ако инжектираш `Scoped` в `Singleton`) |
| **Кръгови зависимости (A ➔ B ➔ A)** | **Забранени по подразбиране** от Spring Boot 2.6+ (`BeanCurrentlyInCreationException`) | **Забранени** (`InvalidOperationException: A circular dependency was detected`) |
| **Външни библиотеки в контейнера** | `@Bean` методи вътре в `@Configuration` клас | `builder.Services.AddSingleton<T>(sp => new T(...))` |
| **Отложено създаване на Bean** | `@Lazy` анотация върху класа или параметъра на конструктора | `Lazy<T>` в конструктора (изисква регистрация на `Lazy<>`) |

---
[⬅️ 04-02. Spring Boot Intro & Layered Architecture](04-02-00-Spring-Boot-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-03-01. Syntax From Scratch](04-03-01-Syntax-From-Scratch.md)
