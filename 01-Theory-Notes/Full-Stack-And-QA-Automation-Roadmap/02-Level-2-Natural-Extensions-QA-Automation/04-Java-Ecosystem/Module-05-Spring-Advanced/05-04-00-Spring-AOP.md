[⬅️ 05-03. Stateless Security with JWT](05-03-00-JWT-Authentication.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-04-01. Syntax From Scratch](05-04-01-Syntax-From-Scratch.md)

---

# 05-04-00. Spring AOP (Aspect-Oriented Programming) & Interceptors (Теория & Капани)

**Модул 05:** Spring Advanced | **Лекционна тема 04:** Аспектно-ориентирано програмиране (**AOP**) в Spring Boot — отделяне на напречните грижи (**Cross-Cutting Concerns**: одит логове, измерване на време, транзакции, сигурност, кеширане) от бизнес логиката чрез **`@Aspect`**, петте вида **Advice** (`@Before`, `@After`, `@AfterReturning`, `@AfterThrowing`, `@Around`), **Pointcut** изрази, **JDK Dynamic Proxy срещу CGLIB Proxy**, Spring **`HandlerInterceptor`** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Spring AOP Proxy обвивката (Блок-схема)](#2-архитектура-на-spring-aop-proxy-обвивката-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring AOP, C# Filters/Interceptors, Python Decorators, NestJS Interceptors)](#3-сравнителна-карта-в-4-езика-spring-aop-c-filtersinterceptors-python-decorators-nestjs-interceptors)
* [4. Плюсове и минуси на Aspect-Oriented Programming](#4-плюсове-и-минуси-на-aspect-oriented-programming)
* [5. Кодов одит: Правилен срещу Грешен код (Забравен `joinPoint.proceed()` в `@Around` и Self-Invocation)](#5-кодов-одит-правилен-срещу-грешен-код-забравен-joinpointproceed-в-around-и-self-invocation)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring AOP) срещу C# 12+ (ASP.NET Core Filters & EF Interceptors)](#7-архитектурно-сравнение-java-21-spring-aop-срещу-c-12-aspnet-core-filters--ef-interceptors)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Четирите основни думи в AOP речника:**
    1.  **Aspect (`@Aspect`):** Класът-наблюдател, който съдържа общата функционалност (напр. `PerformanceLoggingAspect`).
    2.  **Join Point (`JoinPoint`):** Конкретният момент от изпълнението на програмата (в Spring AOP това винаги е **извикване на метод на Spring Bean**).
    3.  **Pointcut (`@Pointcut`):** Филтърът (условието), който казва **КЪДЕ** да се приложи аспектът (напр. *„върху всички методи в пакет `service`“* или *„върху всеки метод с анотация `@TrackTime`“*).
    4.  **Advice (`@Before`, `@Around`, `@AfterReturning`...):** **КАКВО** и **КОГА** да се изпълни, когато Pointcut-ът съвпадне.

---

## 2. АРХИТЕКТУРА НА SPRING AOP PROXY ОБВИВКАТА (БЛОК-СХЕМА)

Когато сложиш `@Aspect` върху метод на `OrderService`, Spring **не променя самия клас `OrderService`**, а го обвива в невидим посредник (**Proxy**):

```text
[ OrderController ]
         │  1. Вика orderService.placeOrder(dto)
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Spring AOP Proxy (CGLIB подклас на OrderService)                        │
│  ├── 2. @Around (Преди): Засича начално време (long start = nanoTime()) │
│  ├── 3. joinPoint.proceed() ──► [ Истинският OrderService.placeOrder() ]│
│  └── 4. @Around (След): Изчислява времето и записва в лога!             │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING AOP, C# FILTERS/INTERCEPTORS, PYTHON DECORATORS, NESTJS INTERCEPTORS)

| Концепция | ☕ Java 21+ (Spring AOP & MVC) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Обвиване на произволен метод** | `@Aspect` + `@Around` (Runtime Proxy) | **Castle DynamicProxy** / Source Generators | Нативни `@decorator` функции | TypeScript `@Decorator` |
| **Прехващане преди/след Controller** | `HandlerInterceptor` (`preHandle`/`postHandle`) | `IActionFilter` / `IAsyncActionFilter` | FastAPI Middleware / Dependency | NestJS `@UseInterceptors(MyInterceptor)` |
| **Custom Анотация като Pointcut** | `@annotation(TrackTime)` | Custom `ActionFilterAttribute` | `@track_time` декоратор | `@SetMetadata(...)` + Reflector |
| **Изпълнение и при грешка** | `@After` (като `finally`) / `@AfterThrowing` | `try { await next(); } catch { ... }` | `try: ... except: ... finally:` | `tap()` / `catchError()` в RxJS |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ASPECT-ORIENTED PROGRAMMING

### 🟢 Предимства (Pros):
*   **Изключително чист бизнес код (Спазване на SRP):** Бизнес методите в `@Service` остават фокусирани на 100% върху задачата си, без да са замърсени с `log.info(...)`, `System.nanoTime()` и одит записи.
*   **Една промяна важи за 100 метода:** Ако решиш да промениш формата на одит логовете в цялото приложение, редактираш само **един** `@Aspect` клас.

### 🔴 Недостатъци (Cons):
*   **По-трудно проследяване при дебъгване:** В Stack Trace-а се появяват генерирани CGLIB прокси класове (`OrderService$$SpringCGLIB$$0`), а кодът в `@Around` не се вижда директно вътре в тялото на бизнес метода.
*   **Работи САМО върху `public` методи на Spring Beans:** Spring AOP не може да прехване `private`, `static` или `final` методи, нито методи на обекти, създадени с `new`.

---
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ЗАБРАВЕН `JOINPOINT.PROCEED()` В `@AROUND` И SELF-INVOCATION)

### ❌ ГРЕШЕН КОД (Забравено `proceed()` в `@Around` и липсващо `@Component` върху `@Aspect`):
```java
// 🚨 ГРЕШКА 1: @Aspect сама по себе си НЕ прави класа Spring Bean!
// Без @Component Spring изобщо няма да зареди аспекта!
@Aspect
public class BadLoggingAspect {

    @Around("@annotation(TrackExecutionTime)")
    public Object measureTime(ProceedingJoinPoint joinPoint) throws Throwable {
        long start = System.currentTimeMillis();
        // 🚨 ГРЕШКА 2: Забравено извикване на 'Object result = joinPoint.proceed();'!
        // Истинският бизнес метод НИКОГА няма да се изпълни и винаги ще връща null!
        long duration = System.currentTimeMillis() - start;
        System.out.println("Време: " + duration);
        return null;
    }
}
```
*   **Как да се избегне:** Винаги поставяй **едновременно `@Aspect` и `@Component`** върху класа на аспекта. В `@Around` съвет винаги извиквай **`Object result = joinPoint.proceed();`** в `try-finally` блок и връщай **`result`**.

### 🟢 ПРАВИЛЕН КОД (Чист `@Aspect` за измерване на време на изпълнение с `@Around`):
```java
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

@Aspect
@Component
public class PerformanceAspect {

    private static final Logger log = LoggerFactory.getLogger(PerformanceAspect.class);

    @Around("@annotation(TrackExecutionTime)")
    public Object measureExecutionTime(ProceedingJoinPoint joinPoint) throws Throwable {
        long start = System.nanoTime();
        try {
            return joinPoint.proceed(); // Изпълнява истинския метод и връща резултата му
        } finally {
            long elapsedMs = (System.nanoTime() - start) / 1_000_000;
            log.info("⏱️ Метод {} се изпълни за {} ms",
                    joinPoint.getSignature().toShortString(), elapsedMs);
        }
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **JDK Dynamic Proxy** и **CGLIB Proxy**, и кой от двата механизма използва Spring Boot по подразбиране?“*
*   **Уловката:** Проверка дали кандидатът познава вътрешните механизми на JVM и ограниченията на `final` класовете/методите в Spring!
*   **Точен Senior отговор:** 
    1.  **JDK Dynamic Proxy:** Вграден в стандартната Java библиотека (`java.lang.reflect.Proxy`). Работи **само ако целевият клас имплементира поне един интерфейс (`interface`)**, като създава прокси обект, имплементиращ същия интерфейс.
    2.  **CGLIB Proxy:** Генерира байткод по време на изпълнение, създавайки **наследник (подклас — `extends UserService`)** на целевия клас. Работи и за класове **без интерфейси**, но **НЕ може да проксира `final` класове или `final` методи** (защото в Java `final` не може да се наследява/пренаписва!).
    3.  **По подразбиране в Spring Boot (от версия 2.0+):** Spring Boot използва **CGLIB Proxy за всички класове** (`spring.aop.proxy-target-class=true`), за да избегне `BeanNotOfRequiredTypeException`, ако някой инжектира директно конкретния клас вместо интерфейса.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING AOP) СРЕЩУ C# 12+ (ASP.NET CORE FILTERS & EF INTERCEPTORS)

| Архитектурен детайл | ☕ Java 21+ (Spring AOP & MVC) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Общ AOP върху произволен Service** | **Вграден в Spring** (`@Aspect` + AspectJ Pointcuts) | Няма вграден в .NET DI (ползват се **Decorators** с Scrutor или **PostSharp / Metalama**) |
| **Прехващане преди/след Controller Action** | `HandlerInterceptor` (`preHandle` / `postHandle`) | `IActionFilter` / `IAsyncActionFilter` (`[ServiceFilter]`) |
| **Прехващане на SQL заявки към базата** | Hibernate `StatementInspector` / P6Spy | EF Core **`DbCommandInterceptor`** |
| **Ограничение за `final` / `sealed` класове** | CGLIB не може да обвие `final` клас или `final` метод | Decorators по интерфейс работят дори върху `sealed` класове |

---
[⬅️ 05-03. Stateless Security with JWT](05-03-00-JWT-Authentication.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-04-01. Syntax From Scratch](05-04-01-Syntax-From-Scratch.md)
