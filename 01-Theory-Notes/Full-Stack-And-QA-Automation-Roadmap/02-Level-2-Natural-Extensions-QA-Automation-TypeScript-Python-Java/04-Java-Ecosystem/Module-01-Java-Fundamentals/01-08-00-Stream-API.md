[⬅️ 01-07. Associative Arrays (Maps)](01-07-00-Maps-And-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-08-01. Syntax From Scratch](01-08-01-Syntax-From-Scratch.md)

---

# 01-08-00. Stream API & Lambda Expressions (Теория, Lazy Evaluation, Optional & Капани)

**Модул 01:** Java Fundamentals | **Лекционна тема 08:** Функционално програмиране в Java, Lambda изрази и функционални интерфейси (`Predicate`, `Function`, `Consumer`, `Supplier`), архитектура на Stream конвейера (Source ➔ Intermediate ➔ Terminal), `flatMap` срещу `map`, класът `Optional<T>` и сравнение с C# LINQ.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Stream API & Разлика между `map` и `flatMap` (Блок-схема)](#2-архитектура-на-stream-api--разлика-между-map-и-flatmap-блок-схема)
* [3. Сравнителна таблица: Java Stream API срещу C# LINQ, Python и JavaScript](#3-сравнителна-таблица-java-stream-api-срещу-c-linq-python-и-javascript)
* [4. Плюсове и минуси на Java Stream API](#4-плюсове-и-минуси-на-java-stream-api)
* [5. Кодов одит: Правилен срещу Грешен код (Повторно ползване на Stream и `Optional.get()`)](#5-кодов-одит-правилен-срещу-грешен-код-повторно-ползване-на-stream-и-optionalget)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Четирите главни функционални интерфейса (Вградени в `java.util.function`):**
    1.  **`Predicate<T>` (Съдията):** Приема обект и връща **`boolean`** (използва се в `.filter(x -> x > 10)`).
    2.  **`Function<T, R>` (Преводачът):** Приема тип `T` и го превръща в тип `R` (използва се в `.map(User::getName)`).
    3.  **`Consumer<T>` (Консуматорът):** Приема обект и **не връща нищо (`void`)** (използва се в `.forEach(System.out::println)`).
    4.  **`Supplier<T>` (Доставчикът):** Не приема нищо `()`, а **доставя нов обект** (използва се в `.orElseGet(() -> new User())`).
*   **`map` срещу `flatMap` (Кутии с бонбони):**
    *   **`.map()`** взима 3 кутии с бонбони и ги боядисва в червено — пак имаш **3 кутии** (`Stream<List<String>>`).
    *   **`.flatMap()`** отваря всичките 3 кутии и изсипва бонбоните на една обща маса — получаваш **един плосък поток от всички бонбони** (`Stream<String>`).

---

## 2. АРХИТЕКТУРА НА STREAM API & РАЗЛИКА МЕЖДУ `MAP` И `FLATMAP` (БЛОК-СХЕМА)

```text
Вход: List<List<Integer>> = [, [3, 4], [5] ]
                                  │
           ┌──────────────────────┴──────────────────────┐
           ▼ .map(List::size)                            ▼ .flatMap(List::stream)
┌─────────────────────────┐                   ┌───────────────────────────────────┐
│ Запазва структурата 1:1 │                   │ Изравнява вложените потоци в 1:   │
│ Резултат: [ 2, 2, 1 ]   │                   │ Резултат: [ 1, 2, 3, 4, 5 ]       │
└─────────────────────────┘                   └───────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA STREAM API СРЕЩУ C# LINQ, PYTHON И JAVASCRIPT

| Операция | ☕ Java 21+ (`Stream`) | 🟣 C# 12+ (`LINQ`) | 🐍 Python 3.x | 🟡 JavaScript (`Array`) |
| :--- | :--- | :--- | :--- | :--- |
| **Филтриране** | `.filter(x -> x > 5)` | `.Where(x => x > 5)` | `[x for x in a if x > 5]` | `.filter(x => x > 5)` |
| **Трансформация** | `.map(x -> x * 2)` | `.Select(x => x * 2)` | `[x * 2 for x in a]` | `.map(x => x * 2)` |
| **Изравняване (Flatten)** | `.flatMap(List::stream)` | `.SelectMany(x => x)` | `[y for x in a for y in x]` | `.flatMap(x => x)` |
| **Сортиране** | `.sorted(Comparator.comparing(...))` | `.OrderBy(x => x.Prop)` | `sorted(a, key=...)` | `.toSorted((a, b) => ...)` |
| **Уникални стойности** | `.distinct()` | `.Distinct()` | `set(a)` / `dict.fromkeys(a)` | `[...new Set(a)]` |
| **Свиване до 1 стойност** | `.reduce(0, Integer::sum)` | `.Aggregate(0, (acc, x) => ...)` | `functools.reduce(...)` | `.reduce((acc, x) => acc + x, 0)` |
| **Групиране по ключ** | `.collect(Collectors.groupingBy(...))` | `.GroupBy(x => x.Role)` | `itertools.groupby` / `defaultdict` | `Object.groupBy(a, x => x.role)` |
| **Проверка за поне 1** | `.anyMatch(x -> x < 0)` | `.Any(x => x < 0)` | `any(x < 0 for x in a)` | `.some(x => x < 0)` |
| **Проверка за всички** | `.allMatch(x -> x > 0)` | `.All(x => x > 0)` | `all(x > 0 for x in a)` | `.every(x => x > 0)` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА JAVA STREAM API

### 🟢 Предимства (Pros):
*   **Short-Circuiting оптимизация:** Операции като `.findFirst()`, `.anyMatch()` и `.limit(3)` спират обработката на потока веднага щом намерят нужния резултат, без да обхождат останалите 1 000 000 елемента.
*   **Лесно паралелизиране:** Само чрез смяна на `.stream()` с `.parallelStream()` Java разпределя задачата върху всички ядра на процесора чрез `ForkJoinPool` (полезно за тежки математически сметки върху огромни масиви).

### 🔴 Недостатъци (Cons):
*   **Лоша поддръжка на Checked Exceptions:** Ако метод вътре в `.map(...)` хвърля Checked Exception (напр. `IOException`), компилаторът не позволява директно извикване без `try-catch` блок вътре в ламбдата.
*   **По-трудно дебъгване с Breakpoints:** Поставянето на breakpoint в дълга верига от 6 Stream операции е по-неудобно от класически `for` цикъл (макар IntelliJ IDEA да има вграден *Trace Current Stream Chain* бутон!).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ПОВТОРНО ПОЛЗВАНЕ НА STREAM И `OPTIONAL.GET()`)

### ❌ ГРЕШЕН КОД (Повторно ползване на затворен Stream и сляпо извикване на `Optional.get()`):
```java
Stream<String> namesStream = List.of("Anna", "Boris", "Ivan").stream()
        .filter(name -> name.length() > 4);

long count = namesStream.count(); // Първа терминална операция -> затваря потока!

// 🚨 ГРЕШКА 1: Опит за втора операция върху същия Stream -> IllegalStateException!
List<String> list = namesStream.toList(); 

// 🚨 ГРЕШКА 2: Извикване на .get() върху празен Optional -> NoSuchElementException!
Optional<String> firstZ = List.of("Anna", "Boris").stream()
        .filter(n -> n.startsWith("Z"))
        .findFirst();
String result = firstZ.get(); // ГЪРМИ, защото няма име започващо със 'Z'!
```
*   **Как да се избегне:** Никога не запазвай `Stream` в променлива за повторна употреба — създавай нов `.stream()` от колекцията всеки път. Върху `Optional<T>` **никога не викай `.get()` директно** — ползвай **`.orElse("Default")`**, **`.orElseGet(...)`** или **`.orElseThrow(...)`**.

### 🟢 ПРАВИЛЕН КОД (Безопасен `Optional` и чисто групиране с `Collectors`):
```java
import java.util.*;
import java.util.stream.Collectors;

public class CleanStreamDemo {
    record Employee(String name, String department, double salary) {}

    public static void main(String[] args) {
        List<Employee> staff = List.of(
            new Employee("Елизабет", "IT", 4500),
            new Employee("Иван", "IT", 3800),
            new Employee("Мария", "HR", 2900)
        );

        // 1. Безопасно търсене с Optional.orElse()
        String topHr = staff.stream()
                .filter(e -> "HR".equals(e.department()))
                .map(Employee::name)
                .findFirst()
                .orElse("Няма намерен служител");

        // 2. Групиране по отдел (Map<String, List<Employee>>) на един ред!
        Map<String, List<Employee>> byDepartment = staff.stream()
                .collect(Collectors.groupingBy(Employee::department));

        System.out.println("HR: " + topHr);
        System.out.println("Отдели: " + byDepartment.keySet());
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **`optional.orElse(createDefault())`** и **`optional.orElseGet(() -> createDefault())`** в Java, и защо `orElse()` може да предизвика сериозен проблем с производителността или скрит бъг?“*
*   **Уловката:** На пръв поглед и двата метода връщат стойност по подразбиране, когато `Optional` е празен.
*   **Точен Senior отговор:** 
    1.  **`orElse(T other)`** е **Eager (нетърпелив)** — аргументът вътре в скобите се изчислява **ВИНАГИ**, дори когато `Optional` **НЕ е празен**! Ако вътре си написала `userRepo.findById(id).orElse(createNewUserInDb())`, Java ще запише нов потребител в базата данни при **всяка заявка**, дори когато потребителят е бил намерен!
    2.  **`orElseGet(Supplier<? extends T> supplier)`** е **Lazy (мързелив)** — подадената ламбда функция се изпълнява **само и единствено ако `Optional` е празен**. Затова при извикване на методи или заявки към базата **винаги** използвай `orElseGet(...)`.

---
[⬅️ 01-07. Associative Arrays (Maps)](01-07-00-Maps-And-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-08-01. Syntax From Scratch](01-08-01-Syntax-From-Scratch.md)
