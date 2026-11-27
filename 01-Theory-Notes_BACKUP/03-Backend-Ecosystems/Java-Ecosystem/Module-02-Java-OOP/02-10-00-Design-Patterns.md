[⬅️ 02-09. Unit Testing (JUnit 5 & Mockito)](02-09-00-Unit-Testing.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-10-01. Syntax From Scratch](02-10-01-Syntax-From-Scratch.md)

---

# 02-10-00. Design Patterns in Java (Теория, GoF Класификация, Spring Приложения & Капани)

**Модул 02:** Java OOP | **Лекционна тема 10:** Трите групи шаблони за проектиране на „Бандата на четиримата“ (**Creational**, **Structural**, **Behavioral**), най-важните шаблони в Java и Spring Boot (`Singleton`, `Factory Method`, `Builder`, `Adapter`, `Decorator`, `Proxy`, `Strategy`, `Observer`), многонишкова сигурност (Thread-Safe Singleton) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Трите категории на GoF Design Patterns (Блок-схема)](#2-трите-категории-на-gof-design-patterns-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на Design Patterns](#4-плюсове-и-минуси-на-design-patterns)
* [5. Кодов одит: Правилен срещу Грешен код (Thread-Unsafe Singleton в многонишкова среда)](#5-кодов-одит-правилен-срещу-грешен-код-thread-unsafe-singleton-в-многонишкова-среда)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot) срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-spring-boot-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трите въпроса, на които отговарят шаблоните (GoF — Gang of Four):**
    1.  **Creational (Създаващи — КАК се раждат обектите?):** Крият логиката на `new` (`Singleton`, `Factory`, `Builder`, `Prototype`).
    2.  **Structural (Структурни — КАК се сглобяват класовете?):** Свързват класове в по-големи структури (`Adapter` = преходник за щепсел, `Decorator` = обличане с допълнителни слоеве, `Proxy` = охранител пред обекта).
    3.  **Behavioral (Поведенчески — КАК си говорят обектите?):** Управляват комуникацията и алгоритмите (`Strategy` = смяна на алгоритъм в движение, `Observer` = абонамент за събития).

---

## 2. ТРИТЕ КАТЕГОРИИ НА GOF DESIGN PATTERNS (БЛОК-СХЕМА)

```text
                       ┌───────────────────────────────────────┐
                       │      GoF DESIGN PATTERNS (23 бр.)     │
                       └───────────────────┬───────────────────┘
            ┌──────────────────────────────┼──────────────────────────────┐
            ▼                              ▼                              ▼
┌───────────────────────┐      ┌───────────────────────┐      ┌───────────────────────┐
│ 1. CREATIONAL         │      │ 2. STRUCTURAL         │      │ 3. BEHAVIORAL         │
│ • Singleton (1 инст.) │      │ • Adapter (Преходник) │      │ • Strategy (Алгоритъм)│
│ • Factory (Фабрика)   │      │ • Decorator (Обвивка) │      │ • Observer (Събития)  │
│ • Builder (Стъпка по  │      │ • Proxy (Spring AOP!) │      │ • Command (Команда)   │
│            стъпка)    │      │ • Facade (Опростен    │      │ • Template Method     │
└───────────────────────┘      │           интерфейс)  │      └───────────────────────┘
                               └───────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Шаблон (Pattern) | ☕ Java 21+ / Spring | 🟣 C# 12+ / .NET | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Singleton (1 инстанция)** | `enum Singleton { INSTANCE; }` / Spring `@Service` | `Lazy<T>` / `.AddSingleton<T>()` | Метаклас / Модулен обект | ES6 Module export |
| **Builder (Сглобяване)** | `User.builder().name("A").build()` (Lombok) | Fluent API (`StringBuilder`, `WebApplicationBuilder`) | Method chaining `return self` | Fluent Builder / Options `{}` |
| **Decorator (Обвиване)** | `new BufferedReader(new FileReader(...))` | `new BufferedStream(fileStream)` | Нативни `@decorator` функции | Higher-Order Functions / TS `@` |
| **Observer (Събития)** | `ApplicationEventPublisher` & `@EventListener` | `event` и `delegate` / `IObservable<T>` | `signals` (Django) / Pub-Sub | `EventEmitter` / `addEventListener` |
| **Strategy (Алгоритъм)** | `@FunctionalInterface` + Lambda / `Comparator` | Делегат `Func<T, R>` / `IComparer<T>` | Подаване на функция като аргумент | Callback функция `(a, b) => ...` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА DESIGN PATTERNS

### 🟢 Предимства (Pros):
*   **Общ инженерен речник в екипа:** Когато кажеш *„Тук ще приложим Adapter и Strategy“*, всеки Senior разработчик веднага разбира архитектурата без 50 страници документация.
*   **Доказани решения на повтарящи се проблеми:** Предпазват от архитектурни задънени улици при разрастване на системата.

### 🔴 Недостатъци (Cons):
*   **Pattern Fever (Прекаляване с шаблоните):** Насилственото вкарване на `AbstractFactoryBuilderProxy` в проста CRUD задача от 100 реда прави кода нечетим и излишно сложен.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (THREAD-UNSAFE SINGLETON В МНОГОНИШКОВА СРЕДА)

### ❌ ГРЕШЕН КОД (Наивен Lazy Singleton — чупи се при 2 едновременни нишки!):
```java
public class BadDatabaseConnection {
    private static BadDatabaseConnection instance;
    private BadDatabaseConnection() {}

    // 🚨 ГРЕШКА (Race Condition): Ако Нишка А и Нишка Б влязат тук едновременно,
    // и двете ще видят (instance == null) и ще създадат ДВЕ различни връзки!
    public static BadDatabaseConnection getInstance() {
        if (instance == null) {
            instance = new BadDatabaseConnection();
        }
        return instance;
    }
}
```
*   **Как да се избегне:** Използвай **Bill Pugh Static Inner Helper Class** (най-бързият и чист Lazy Singleton в Java без `synchronized` забавяне) или **Enum Singleton** (препоръчан от Джошуа Блок в *Effective Java*).

### 🟢 ПРАВИЛЕН КОД (Thread-Safe Bill Pugh Singleton & Enum Singleton):
```java
// Начин 1: Bill Pugh Singleton (100% Thread-Safe и Lazy благодарение на JVM ClassLoader-а!)
public class DatabaseConnection {
    private DatabaseConnection() {}

    private static class Holder {
        private static final DatabaseConnection INSTANCE = new DatabaseConnection();
    }

    public static DatabaseConnection getInstance() {
        return Holder.INSTANCE;
    }
}

// Начин 2: Enum Singleton (Защитен дори от Reflection и Deserialization атаки!)
enum AppConfig {
    INSTANCE;

    public String getEnvironment() {
        return "PRODUCTION";
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Кои GoF шаблони за проектиране са вградени директно в стандартната библиотека на Java (JDK) и в **Spring Framework**?“*
*   **Уловката:** Интервюиращите обичат да проверяват дали виждаш шаблоните в реалните инструменти, които ползваш всеки ден, а не само на теория!
*   **Точен Senior отговор:** 
    1.  **Singleton:** По подразбиране всеки `@Service`, `@Repository` и `@Component` в Spring IoC контейнера е **Singleton Bean** (1 инстанция на Spring контейнер).
    2.  **Proxy:** Цялата магия на **`@Transactional`**, **`@Cacheable`** и **Spring Security** работи чрез **Dynamic Proxy Pattern** — Spring обвива твоя клас в прокси обект, който отваря транзакция преди метода и я комитва след него.
    3.  **Factory Method:** `BeanFactory` / `ApplicationContext.getBean(...)`, `LocalDate.of(...)`, `List.of(...)`.
    4.  **Builder:** `StringBuilder`, `Stream.builder()`, `HttpRequest.newBuilder()`.
    5.  **Decorator:** Всички класове в `java.io` (`new BufferedInputStream(new FileInputStream(...))`) и `Collections.unmodifiableList(...)`.
    6.  **Adapter:** `Arrays.asList(...)`, `InputStreamReader` (адаптира байтов поток към символен).

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT) СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ & Spring Boot | 🟣 C# 12+ & ASP.NET Core |
| :--- | :--- | :--- |
| **Най-добър Thread-Safe Singleton** | `enum Singleton { INSTANCE; }` или Bill Pugh Holder | `private static readonly Lazy<T> _instance = new(() => new T());` |
| **Builder Pattern в практиката** | `@Builder` анотация от **Lombok** генерира целия код | Fluent API методи + C# `init` properties (`new User { Name = "A" }`) |
| **Observer / Event система** | `ApplicationEventPublisher` + `@EventListener` в Spring | Вградени езикови `event` и `EventHandler<T>` / **MediatR** |
| **Dynamic Proxy (AOP)** | JDK Dynamic Proxy (за интерфейси) и **CGLIB** (за класове) | `DispatchProxy` или библиотеката **Castle DynamicProxy** |
| **Factory в DI контейнера** | `@Bean` методи вътре в `@Configuration` клас | `services.AddScoped<IService>(sp => new Service(...))` |

---
[⬅️ 02-09. Unit Testing (JUnit 5 & Mockito)](02-09-00-Unit-Testing.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-10-01. Syntax From Scratch](02-10-01-Syntax-From-Scratch.md)
