[⬅️ 01-08-00. Теория & Капани](01-08-00-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-09-00. Text Processing](01-09-00-Text-Processing.md)

---

# 01-08-01. Stream API & Lambda Expressions: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за Lambda изрази, Method References (`::`), числови потоци (`IntStream`), `Collectors` (`groupingBy`, `joining`, `toMap`) и `Optional` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Видове Lambda изрази и Method References (`::`)](#1-видове-lambda-изрази-и-method-references-)
* [2. Числови потоци (`IntStream`, `range`, `sum`, `average`, `summaryStatistics`)](#2-числови-потоци-intstream-range-sum-average-summarystatistics)
* [3. Мощните колектори (`Collectors.joining`, `groupingBy`, `partitioningBy`, `toMap`)](#3-мощните-колектори-collectorsjoining-groupingby-partitioningby-tomap)
* [4. Изравняване на вложени списъци с `flatMap`](#4-изравняване-на-вложени-списъци-с-flatmap)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ВИДОВЕ LAMBDA ИЗРАЗИ И METHOD REFERENCES (`::`)

Когато ламбдата просто вика съществуващ метод без да променя аргументите, я съкращаваме с двоен двоеточие знак **`::` (Method Reference)**:

```java
import java.util.List;

List<String> names = List.of("ivan", "maria", "georgi");

// 1. Стандартен Lambda синтаксис:
List<String> upper1 = names.stream()
        .map(name -> name.toUpperCase())
        .toList();

// 2. Съкратен Method Reference (::) — по-чист и четим:
List<String> upper2 = names.stream()
        .map(String::toUpperCase)
        .toList();

upper2.forEach(System.out::println);
```

---

## 2. ЧИСЛОВИ ПОТОЦИ (`INTSTREAM`, `RANGE`, `SUM`, `AVERAGE`, `SUMMARYSTATISTICS`)

```java
import java.util.IntSummaryStatistics;
import java.util.List;
import java.util.stream.IntStream;

// 1. Генериране на числа от 1 до 10 (rangeClosed включва 10, range го изключва)
int sum1To10 = IntStream.rangeClosed(1, 10).sum(); // 55

// 2. Всички статистики наведнъж (Брой, Сума, Мин, Средно, Макс)
List<Integer> grades = List.of(4, 5, 6, 6, 5);
IntSummaryStatistics stats = grades.stream()
        .mapToInt(Integer::intValue)
        .summaryStatistics();

System.out.printf("Средно: %.2f | Макс: %d | Брой: %d%n",
        stats.getAverage(), stats.getMax(), stats.getCount());
```

---

## 3. МОЩНИТЕ КОЛЕКТОРИ (`COLLECTORS.JOINING`, `GROUPINGBY`, `PARTITIONINGBY`, `TOMAP`)

```java
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

List<String> words = List.of("Java", "Spring", "Docker", "Kafka");

// 1. Съединяване на елементи в низ с префикс и суфикс
String joined = words.stream()
        .collect(Collectors.joining(", ", "[", "]")); // "[Java, Spring, Docker, Kafka]"

// 2. Групиране по дължина на думата -> Map<Integer, List<String>>
Map<Integer, List<String>> byLength = words.stream()
        .collect(Collectors.groupingBy(String::length)); // {4=[Java], 6=[Spring, Docker], 5=[Kafka]}

// 3. Разделяне на две групи (true / false) -> Map<Boolean, List<String>>
Map<Boolean, List<String>> longVsShort = words.stream()
        .collect(Collectors.partitioningBy(w -> w.length() > 5));

// 4. Превръщане на списък в Map (с функция за разрешаване на дублирани ключове!)
Map<String, Integer> wordLengths = words.stream()
        .collect(Collectors.toMap(
            w -> w,             // Ключ
            String::length,     // Стойност
            (oldVal, newVal) -> oldVal // Защита против IllegalStateException при дубликати!
        ));
```

---

## 4. ИЗРАВНЯВАНЕ НА ВЛОЖЕНИ СПИСЪЦИ С `FLATMAP`

```java
List<List<String>> matrix = List.of(
    List.of("A", "B"),
    List.of("C", "D", "E")
);

List<String> flatList = matrix.stream()
        .flatMap(List::stream)
        .toList(); //
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция в Stream API | Вид операция | Какво прави |
| :--- | :--- | :--- |
| **`.filter(Predicate)`** | Intermediate (Lazy) | Оставя само елементите, отговарящи на условието |
| **`.map(Function)`** | Intermediate (Lazy) | Трансформира всеки елемент в нов тип/стойност |
| **`.flatMap(Function)`** | Intermediate (Lazy) | Слива вложени колекции в един плосък поток |
| **`.distinct()` / `.sorted()`** | Intermediate (Stateful) | Премахва дубликати / Сортира елементите |
| **`.skip(n)` / `.limit(n)`** | Intermediate (Short-circuit) | Пропуска първите `n` / Взима максимум `n` елемента |
| **`.peek(Consumer)`** | Intermediate (Debug) | Позволява принтиране за дебъгване по средата на веригата |
| **`.toList()`** | Terminal | Връща непроменяем `List<T>` (Java 16+) |
| **`.findFirst()` / `.findAny()`** | Terminal (Short-circuit) | Връща `Optional<T>` с първия намерен елемент |
| **`opt.orElseGet(() -> val)`** | `Optional` метод | Мързеливо връща алтернативна стойност при празен `Optional` |

---
[⬅️ 01-08-00. Теория & Капани](01-08-00-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-09-00. Text Processing](01-09-00-Text-Processing.md)
