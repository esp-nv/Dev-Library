[⬅️ 02-00. Intro to Java OOP](02-00-Intro.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-01-01. Syntax From Scratch](02-01-01-Syntax-From-Scratch.md)

---

# 02-01-00. Working with Abstraction (`Enums`, `Static` & Packages — Теория & Капани)

**Модул 02:** Java OOP | **Лекционна тема 01:** Мощните изброени типове (`enum` като пълноценни класове с полета, конструктори и методи), ключовата дума `static` (Static Blocks, Static Fields & Methods), пакети и импорти, и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на `static` в паметта (Metaspace срещу Heap Блок-схема)](#2-архитектура-на-static-в-паметта-metaspace-срещу-heap-блок-схема)
* [3. Сравнителна таблица: Java `enum` & `static` срещу C#, Python и JavaScript](#3-сравнителна-таблица-java-enum--static-срещу-c-python-и-javascript)
* [4. Плюсове и минуси на богатите `Enum` класове в Java](#4-плюсове-и-минуси-на-богатите-enum-класове-в-java)
* [5. Кодов одит: Правилен срещу Грешен код (Викане на нестатичен член от `static` и `valueOf()` срив)](#5-кодов-одит-правилен-срещу-грешен-код-викане-на-нестатичен-член-от-static-и-valueof-срив)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`enum` в Java НЕ Е просто число (`int`), както в C# и C++!** В C# `enum Status { Pending = 0, Paid = 1 }` е просто име на цяло число. В Java **всеки `enum` е пълноценен специален клас**, а всяка негова константа (`PENDING`, `PAID`) е **предварително създаден `public static final` обект** от този клас! Затова Java `enum` може да има свои полета, конструктор, методи и дори да имплементира интерфейси.
*   **`static` = Принадлежи на чертежа (Класа), а не на построената къща (Обекта):**
    *   Нестатично поле (`String ownerName`): Всяка къща си има собствен собственик.
    *   Статично поле (`static int totalHousesBuilt`): Един общ брояч, закачен на самия чертеж, който се споделя от всички къщи.

---

## 2. АРХИТЕКТУРА НА `STATIC` В ПАМЕТТА (METASPACE СРЕЩУ HEAP БЛОК-СХЕМА)

```text
METASPACE (Метаданни на класа)                HEAP ПАМЕТ (Инстанции на обекти)
┌────────────────────────────────────┐        ┌─────────────────────────────────┐
│ Клас User:                         │        │ Обект u1 ("Анна")               │
│ • static int userCount = 2         │        └─────────────────────────────────┘
│ • static void printCount()         │        ┌─────────────────────────────────┐
│ • Референция към Enum.ADMIN ───────┼───────►│ Singleton обект Role.ADMIN      │
└────────────────────────────────────┘        └─────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA `ENUM` & `STATIC` СРЕЩУ C#, PYTHON И JAVASCRIPT

| Концепция | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Дефиниция на Enum** | `enum Role { ADMIN, USER }` | `enum Role { Admin, User }` | `class Role(Enum): ADMIN = 1` | Няма вграден (`Object.freeze`) |
| **Полета и методи в Enum** | **Да, вградено** (пълноценен клас) | **Не** (ползват се Extension Methods) | **Да** (Python `Enum` е клас) | Чрез обекти |
| **Всички стойности на Enum** | `Role.values()` (връща `Role[]`) | `Enum.GetValues<Role>()` | `list(Role)` | `Object.values(Role)` |
| **Парсване от String** | `Role.valueOf("ADMIN")` | `Enum.Parse<Role>("Admin")` | `Role["ADMIN"]` | `Role[str]` |
| **Статичен блок (инициализация)** | `static { ... }` | `static MyClass() { ... }` (Static ctor) | Код на ниво тяло на класа | `static { ... }` (ES2022) |
| **Static Import (методи без клас)** | `import static java.lang.Math.*;` | `using static System.Math;` | `from math import sqrt` | `import { sqrt } from ...` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА БОГАТИТЕ `ENUM` КЛАСОВЕ В JAVA

### 🟢 Предимства (Pros):
*   **Елиминират дългите `if-else` / `switch` блокове (Strategy чрез Enum):** Вместо да проверяваш `if (planet == MARS) gravity = 3.71`, самата константа `Planet.MARS(3.303e+23, 2.4397e6)` си знае масата, радиуса и метода `surfaceGravity()`.
*   **Гарантиран Singleton от самата JVM:** Едно-елементен `enum Singleton { INSTANCE; }` е най-сигурният начин за създаване на Singleton в Java (защитен от Reflection и Serialization атаки).

### 🔴 Недостатъци (Cons):
*   **Не може да наследява друг клас:** Всеки `enum` автоматично наследява `java.lang.Enum`, а Java не позволява множествено наследяване на класове (но `enum` може да имплементира интерфейси).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ВИКАНЕ НА НЕСТАТИЧЕН ЧЛЕН ОТ `STATIC` И `VALUEOF()` СРИВ)

### ❌ ГРЕШЕН КОД (Достъп до инстанционно поле от `static` метод и опасен `Enum.valueOf()`):
```java
public class OrderService {
    private double taxRate = 0.20; // Инстанционно поле (изисква new OrderService())

    public static void printTax() {
        // 🚨 ГРЕШКА 1: Compile-Time Error!
        // "Non-static field 'taxRate' cannot be referenced from a static context"
        System.out.println(taxRate);
    }

    public static void parseStatus(String input) {
        // 🚨 ГРЕШКА 2: Ако потребителят подаде "pending" (с малки букви) или "UNKNOWN",
        // Status.valueOf(input) гърми с IllegalArgumentException!
        Status s = Status.valueOf(input);
    }
}
```
*   **Как да се избегне:** Статичните методи нямат указател **`this`**, затова могат да достъпват директно само други `static` полета и методи. При парсване на `Enum` от външен текст винаги нормализирай с `.trim().toUpperCase()` или напиши безопасен търсещ метод.

### 🟢 ПРАВИЛЕН КОД (Богат `Enum` с полета, конструктор и метод):
```java
public enum OrderStatus {
    PENDING("Изчаква плащане", false),
    PAID("Платена", true),
    SHIPPED("Изпратена", true),
    CANCELLED("Отказана", false);

    private final String bgDescription;
    private final boolean isActiveFlow;

    // Конструкторът на Enum ВИНАГИ е private (дори да не го напишеш изрично)
    OrderStatus(String bgDescription, boolean isActiveFlow) {
        this.bgDescription = bgDescription;
        this.isActiveFlow = isActiveFlow;
    }

    public String getBgDescription() { return bgDescription; }
    public boolean isActiveFlow() { return isActiveFlow; }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„В какъв точен ред се изпълняват **Static Initialization Blocks (`static { ... }`)**, **Instance Initialization Blocks (`{ ... }`)** и **Конструкторът**, когато един клас се зареди за първи път и се създадат два обекта от него?“*
*   **Уловката:** Това е класически въпрос за проверка дали кандидатът разбира жизнения цикъл на класовете в JVM.
*   **Точен Senior отговор:** 
    1.  **`static { ... }` блокът** се изпълнява **само ВЕДНЪЖ** в целия живот на програмата — в момента, в който JVM зареди класа в паметта (преди `main()` или преди създаването на първия обект).
    2.  При всяко извикване на `new MyClass()` се изпълнява първо **Instance блокът `{ ... }`** (ако има такъв), и веднага след него **тялото на Конструктора**.
    *   *Ред при създаване на 2 обекта:* `Static Block` ➔ `Instance Block (Обект 1)` ➔ `Constructor (Обект 1)` ➔ `Instance Block (Обект 2)` ➔ `Constructor (Обект 2)`.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ (`enum` & `static`) | 🟣 C# 12+ (`enum` & `static`) |
| :--- | :--- | :--- |
| **Вътрешна същност на `enum`** | **Пълноценен клас** (наследява `java.lang.Enum`), всяка константа е обект | **Именувана числова константа** (наследява `System.Enum`, пази `int`/`byte`) |
| **Полета, конструктори и методи в `enum`** | **Поддържа нативно** вътре в самия `enum` | **Не поддържа** (добавят се методи отвън чрез `static` Extension Methods) |
| **Статичен клас (`static class`)** | Само за **вложени класове** (`static class Nested`); външен клас се прави `final` + `private` конструктор | Поддържа **`public static class Utils`** директно на най-горно ниво |
| **Статичен конструктор / блок** | `static { ... }` (може да има няколко блока в един клас) | `static MyClass() { ... }` (строго един статичен конструктор без модификатори) |
| **Достъп до `static` член през инстанция** | Позволено от компилатора (`obj.staticMethod()`), но дава предупреждение | **Забранено** (`Compile-time error` — достъпва се само през името на класа `MyClass.StaticMethod()`) |


---
[⬅️ 02-00. Intro to Java OOP](02-00-Intro.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-01-01. Syntax From Scratch](02-01-01-Syntax-From-Scratch.md)
