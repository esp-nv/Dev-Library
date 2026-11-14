[⬅️ 02-05. Polymorphism](02-05-00-Polymorphism.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-06-01. Syntax From Scratch](02-06-01-Syntax-From-Scratch.md)

---

# 02-06-00. SOLID Principles in Java (Теория, Архитектура, Анти-патърни & Капани)

**Модул 02:** Java OOP | **Лекционна тема 06:** Петте принципа на обектно-ориентираното проектиране (SRP, OCP, LSP, ISP, DIP), връзката им със Spring Boot IoC контейнера, класически нарушения и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурен поток на Dependency Inversion (DIP Блок-схема)](#2-архитектурен-поток-на-dependency-inversion-dip-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на стриктното прилагане на SOLID](#4-плюсове-и-минуси-на-стриктното-прилагане-на-solid)
* [5. Кодов одит: Правилен срещу Грешен код (Нарушение на OCP и DIP)](#5-кодов-одит-правилен-срещу-грешен-код-нарушение-на-ocp-и-dip)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring) срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-spring-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **S — Single Responsibility Principle (Принцип на единствената отговорност):** *„Един клас трябва да има само една причина да се променя.“* Не слагай SQL заявки, изпращане на имейли и генериране на PDF в един „Бог клас“ (`GodObject`).
*   **O — Open/Closed Principle (Принцип за отвореност/затвореност):** *„Отворен за разширяване, затворен за промяна.“* Добавяш нова функционалност като напишеш **нов клас**, вместо да редактираш съществуващ работещ `if-else` блок.
*   **L — Liskov Substitution Principle (Принцип на заместването на Лисков):** *„Наследникът трябва да може да замести родителя си навсякъде, без да чупи очакваното поведение.“* Ако `Square extends Rectangle` счупи формулата `width * height`, значи наследяването е грешно.
*   **I — Interface Segregation Principle (Принцип за разделяне на интерфейсите):** *„По-добре 5 малки, специализирани интерфейса, отколкото 1 огромен интерфейс с 20 метода.“* Не карай робот да имплементира интерфейс `Worker` с метод `eatLunch()`.
*   **D — Dependency Inversion Principle (Принцип на обръщане на зависимостите):** *„Високоуровневите модули не трябва да зависят от нискоуровневите. И двата трябва да зависят от абстракции (`interface`).“*

---

## 2. АРХИТЕКТУРЕН ПОТОК НА DEPENDENCY INVERSION (DIP БЛОК-СХЕМА)

```text
❌ ЛОША АРХИТЕКТУРА (Силна свързаност / Tight Coupling):
   [ OrderService ] ──────► [ MySqlRepository ]  (Ако минеш на PostgreSQL, преписваш OrderService!)

✅ ДОБРА АРХИТЕКТУРА (Dependency Inversion + Constructor Injection):
   [ OrderService ] ──────► [ <<interface>> OrderRepository ]
                                          ▲
                    ┌─────────────────────┴─────────────────────┐
          [ MySqlRepository ]                         [ PostgresRepository ]
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| SOLID Принцип | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript / TypeScript |
| :--- | :--- | :--- | :--- | :--- |
| **ISP (Малки интерфейси)** | `interface Readable`, `Writable` | `IReadable`, `IWritable` | `typing.Protocol` (Structural) | `interface Readable` (TS) |
| **LSP (Защита от счупване)** | `UnsupportedOperationException` е червен флаг | `NotSupportedException` е червен флаг | `NotImplementedError` е червен флаг | Хвърляне на `Error` в подклас |
| **DIP (Инжекция на зависимости)** | Constructor Injection (`final` полета) | Constructor Injection (`readonly` полета) | Подаване на аргументи в `__init__` | Constructor Injection (Angular/NestJS) |
| **Вграден DI Контейнер** | Spring Framework (`@Service`, `@Autowired`) | `Microsoft.Extensions.DependencyInjection` | `dependency-injector` / FastAPI `Depends` | NestJS / InversifyJS |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА СТРИКТНОТО ПРИЛАГАНЕ НА SOLID

### 🟢 Предимства (Pros):
*   **Висока тестируемост:** Когато класовете получават зависимостите си през интерфейси в конструктора, можеш да тестваш бизнес логиката за милисекунди с **Mockito**, без да вдигаш реална база данни.
*   **Безопасни промени в екип:** Различни разработчици могат да добавят нови имплементации (напр. нов платежен метод), без да си пречат в едни и същи файлове.

### 🔴 Недостатъци (Cons):
*   **Риск от Over-engineering (Свръх-проектиране):** Създаването на интерфейс `UserService` за всеки един клас, който никога няма да има втора имплементация, увеличава броя на файловете излишно в малки проекти.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (НАРУШЕНИЕ НА OCP И DIP)

### ❌ ГРЕШЕН КОД (Нарушение на OCP и DIP — директно `new` и безкраен `if-else`):
```java
public class CheckoutService {
    public void checkout(String paymentType, double amount) {
        // 🚨 ГРЕШКА 1 (Нарушение на OCP): За всеки нов метод на плащане (напр. Revolut)
        // трябва да отваряш и редактираш този клас!
        // 🚨 ГРЕШКА 2 (Нарушение на DIP): Директна зависимост от конкретни класове (new)!
        if ("CARD".equals(paymentType)) {
            new CardPayment().pay(amount);
        } else if ("PAYPAL".equals(paymentType)) {
            new PayPalPayment().pay(amount);
        }
    }
}
```
*   **Как да се избегне:** Извади плащането зад интерфейс **`PaymentMethod`** и подай списък или карта от имплементации през конструктора (**Strategy Pattern + DIP**).

### 🟢 ПРАВИЛЕН КОД (Спазване на SRP, OCP и DIP чрез Constructor Injection):
```java
import java.util.Map;

interface PaymentMethod {
    void pay(double amount);
}

public class CheckoutService {
    private final Map<String, PaymentMethod> paymentMethods;

    // Dependency Injection през конструктора (Spring прави това автоматично!)
    public CheckoutService(Map<String, PaymentMethod> paymentMethods) {
        this.paymentMethods = paymentMethods;
    }

    public void checkout(String paymentType, double amount) {
        PaymentMethod method = paymentMethods.get(paymentType);
        if (method == null) {
            throw new IllegalArgumentException("Неподдържан метод на плащане: " + paymentType);
        }
        method.pay(amount);
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **Dependency Inversion (принципът D от SOLID)**, **Dependency Injection (DI)** и **Inversion of Control (IoC)**?“*
*   **Уловката:** Кандидатите често ги използват като синоними, но в софтуерната архитектура това са три различни нива!
*   **Точен Senior отговор:** 
    1.  **Dependency Inversion Principle (DIP)** е **архитектурен принцип** — казва *КАКВО* да правим (класовете да зависят от абстракции/интерфейси, а не от конкретни класове).
    2.  **Dependency Injection (DI)** е **конкретен дизайн патерн**, който реализира DIP — казва *КАК* да доставим зависимостите отвън (най-често чрез параметри на конструктора), вместо класът сам да вика `new`.
    3.  **Inversion of Control (IoC)** е **по-широката концепция (Framework контрол)** — вместо твоят код да управлява създаването и жизнения цикъл на обектите, предаваш контрола на външен контейнер (**Spring ApplicationContext** в Java или **IServiceProvider** в .NET), който сглобява и инжектира всички зависимости автоматично.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING) СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ & Spring Boot | 🟣 C# 12+ & ASP.NET Core |
| :--- | :--- | :--- |
| **Предпочитан начин за DI** | **Constructor Injection** с `private final` полета | **Constructor Injection** с `private readonly` полета (или Primary Constructors) |
| **Задължителна анотация при 1 ctor** | `@Autowired` **не е нужна** при един конструктор (от Spring 4.3+) | Не се изисква атрибут при един конструктор |
| **Инжектиране на всички имплементации** | `public Service(List<PaymentMethod> all)` или `Map<String, PaymentMethod>` | `public Service(IEnumerable<IPaymentMethod> all)` |
| **Нарушение на LSP в стандартното API** | `List.of(1, 2).add(3)` хвърля `UnsupportedOperationException` | `Array` имплементира `IList<T>`, но `arr.Add(3)` хвърля `NotSupportedException` |
| **Вградени функционални интерфейси (ISP)** | `Predicate<T>`, `Function<T,R>`, `Consumer<T>`, `Supplier<T>` | `Predicate<T>`, `Func<T,R>`, `Action<T>` |

---
[⬅️ 02-05. Polymorphism](02-05-00-Polymorphism.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-06-01. Syntax From Scratch](02-06-01-Syntax-From-Scratch.md)
