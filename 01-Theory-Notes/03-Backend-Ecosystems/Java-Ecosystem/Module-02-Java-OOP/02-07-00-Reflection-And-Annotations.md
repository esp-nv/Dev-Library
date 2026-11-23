[⬅️ 02-06. SOLID Principles](02-06-00-SOLID-Principles.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-07-01. Syntax From Scratch](02-07-01-Syntax-From-Scratch.md)

---

# 02-07-00. Reflection and Annotations (Теория, Магията на Spring Boot & Капани)

**Модул 02:** Java OOP | **Лекционна тема 07:** Java Reflection API (`Class<?>`, `Field`, `Method`, `Constructor`), как Spring Boot и Hibernate инспектират и създават обекти по време на изпълнение (Runtime), потребителски анотации (`@interface`, `@Retention`, `@Target`), прескачане на модификатори за достъп (`setAccessible(true)`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Как работи Reflection API (Рентгенов скенер в JVM Блок-схема)](#2-как-работи-reflection-api-рентгенов-скенер-в-jvm-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на Reflection API](#4-плюсове-и-минуси-на-reflection-api)
* [5. Кодов одит: Правилен срещу Грешен код (`getFields` срещу `getDeclaredFields` и липсващ `@Retention(RUNTIME)`)](#5-кодов-одит-правилен-срещу-грешен-код-getfields-срещу-getdeclaredfields-и-липсващ-retentionruntime)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Reflection = Рентгенов скенер + Огледало по време на изпълнение:** Програмата поглежда сама себе си в огледалото, докато работи, вижда какви полета, методи и конструктори има един клас (дори да са `private`!), и може да ги извика по име, подадено като обикновен `String`.
*   **Annotations (`@Service`, `@Override`, `@Id`) = Цветни лепящи се листчета (Sticky Notes):** Сама по себе си анотацията **не изпълнява никакъв код**! Тя е просто метаданни (етикет), залепен върху клас или поле. Кодът се изпълнява чак когато външен двигател (като **Spring** или **JUnit**) мине с Reflection скенера и прочете етикетите.
*   **`get...()` срещу `getDeclared...()` (Златната разлика в Java Reflection):**
    *   `clazz.getFields()` / `getMethods()` ➔ Връща **САМО `public`** членове, **ВКЛЮЧИТЕЛНО наследените** от родителските класове (и `Object`).
    *   `clazz.getDeclaredFields()` / `getDeclaredMethods()` ➔ Връща **ВСИЧКИ членове (`private`, `protected`, `public`)**, декларирани **САМО в текущия клас** (без наследените).

---

## 2. КАК РАБОТИ REFLECTION API (РЕНТГЕНОВ СКЕНЕР В JVM БЛОК-СХЕМА)

```text
1. Зареждане на класа в JVM (Metaspace):
   Class<?> clazz = User.class;  (или Class.forName("bg.softuni.User"))
          │
          ├──► clazz.getDeclaredConstructor().newInstance() ➔ Създава обект в Heap!
          ├──► Field f = clazz.getDeclaredField("password") ➔ Намира private поле!
          │       └── f.setAccessible(true)                 ➔ Отключва private защитата!
          │       └── f.set(userObj, "NewSecret123")        ➔ Променя стойността му!
          └──► clazz.isAnnotationPresent(Entity.class)      ➔ Проверява за @Entity!
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Концепция | ☕ Java 21+ (Reflection) | 🟣 C# 12+ (`System.Reflection`) | 🐍 Python 3.x (Introspection) | 🟡 JavaScript (`Reflect` / `Proxy`) |
| :--- | :--- | :--- | :--- | :--- |
| **Обект за метаданни на тип** | `Class<?> c = User.class;` | `Type t = typeof(User);` | `cls = User` (класът е обект) | `user.constructor` |
| **Зареждане на клас по име** | `Class.forName("com.app.User")` | `Type.GetType("App.User")` | `getattr(module, "User")` | Няма (ES dynamic `import()`) |
| **Всички полета на класа** | `c.getDeclaredFields()` | `t.GetFields(BindingFlags.NonPublic \| ...)` | `vars(obj)` / `obj.__dict__` | `Object.keys(obj)` / `Reflect.ownKeys()` |
| **Прескачане на `private`** | `field.setAccessible(true);` | `fieldInfo.SetValue(obj, val);` | Директен достъп (няма защита) | Невъзможно за `#privateField`! |
| **Метаданни (Анотации)** | `@MyAnnotation` (`@interface`) | `[MyAttribute]` (`: Attribute`) | `@decorator` (обвиваща функция) | `@decorator` (Stage 3 / TypeScript) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА REFLECTION API

### 🟢 Предимства (Pros):
*   **Прави възможни модерните рамки:** Без Reflection нямаше да съществуват **Spring Boot** (Dependency Injection), **Hibernate / JPA** (ORM мапинг от таблица към клас), **Jackson** (JSON сериализация) и **JUnit 5** (автоматично пускане на `@Test` методи).

### 🔴 Недостатъци (Cons):
*   **По-ниска скорост (Performance Overhead):** Reflection заобикаля компилаторните оптимизации и проверява типовете и правата динамично при всяко извикване.
*   **Чупи капсулацията и типова безопасност:** Грешките (напр. сбъркано име на метод в низ `"setNmae"`) не се хващат от компилатора, а гърмят чак по време на работа с `NoSuchMethodException`.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`GETFIELDS` СРЕЩУ `GETDECLAREDFIELDS` И ЛИПСВАЩ `@RETENTION(RUNTIME)`)

### ❌ ГРЕШЕН КОД (Собствена анотация без `@Retention(RetentionPolicy.RUNTIME)`):
```java
// 🚨 ГРЕШКА 1: По подразбиране @Retention е RetentionPolicy.CLASS!
// Компилаторът записва анотацията в .class файла, но JVM я ИЗХВЪРЛЯ при стартиране!
// Затова clazz.isAnnotationPresent(AuditLog.class) винаги ще връща FALSE!
@Target(ElementType.METHOD)
public @interface AuditLog {
    String action();
}

// 🚨 ГРЕШКА 2: Използване на clazz.getField("secret") за private поле -> NoSuchFieldException!
```
*   **Как да се избегне:** Всяка анотация, която ще четеш с Reflection по време на изпълнение, **задължително** трябва да има **`@Retention(RetentionPolicy.RUNTIME)`**. За `private` членове винаги ползвай **`getDeclaredField()`** + **`setAccessible(true)`**.

### 🟢 ПРАВИЛЕН КОД (Валидна Runtime анотация и безопасно инстанциране):
```java
import java.lang.annotation.*;

@Retention(RetentionPolicy.RUNTIME) // Задължително, за да се вижда от Reflection!
@Target(ElementType.METHOD)         // Може да се слага само върху методи
public @interface AuditLog {
    String action() default "EXECUTE";
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между остарелия начин `clazz.newInstance()` и модерния **`clazz.getDeclaredConstructor().newInstance()`** (от Java 9+), и защо `clazz.newInstance()` беше обявен за `@Deprecated`?“*
*   **Уловката:** Това е класически въпрос за зрялост в Java 9–21.
*   **Точен Senior отговор:** Старият метод `clazz.newInstance()` имаше сериозен архитектурен дефект в обработката на грешки: ако конструкторът на класа хвърлеше **Checked Exception** (например `IOException`), `clazz.newInstance()` го промъкваше директно навън, **заобикаляйки проверката на компилатора** (*Sneaky Throw*)! Модерният **`clazz.getDeclaredConstructor().newInstance()`** решава проблема, като обвива всяка грешка от конструктора в стандартното изключение **`InvocationTargetException`** и позволява да избереш и конструктор с конкретни параметри (`getDeclaredConstructor(String.class, int.class)`).

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ (JVM) | 🟣 C# 12+ (.NET 8+ CLR) |
| :--- | :--- | :--- |
| **Метаданни върху клас/метод** | **Annotations** (`@interface MyTag`, ползват се като `@MyTag`) | **Attributes** (`class MyTagAttribute : Attribute`, ползват се като `[MyTag]`) |
| **Видимост на анотации при Runtime** | Изисква изрично **`@Retention(RetentionPolicy.RUNTIME)`** | Всички атрибути са достъпни при Runtime по подразбиране |
| **Отключване на `private` полета** | `field.setAccessible(true)` (в Java 17+ е ограничено за вътрешни JDK модули — *Strong Encapsulation*) | `BindingFlags.NonPublic \| BindingFlags.Instance` (работи директно без `setAccessible`) |
| **Generics по време на изпълнение** | **Type Erasure** (`List<String>.class` не съществува; вижда се само суров `List.class`) | **Reified Generics** (`typeof(List<string>)` пази пълния тип `string` в паметта!) |
| **Модерна алтернатива на Reflection** | **MethodHandles** (`java.lang.invoke.MethodHandles.Lookup`) / AOT (GraalVM) | **Source Generators** (генерират C# код по време на компилация без Reflection) |

---
[⬅️ 02-06. SOLID Principles](02-06-00-SOLID-Principles.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-07-01. Syntax From Scratch](02-07-01-Syntax-From-Scratch.md)
