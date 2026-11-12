[⬅️ 01-02-00. Теория & Капани](01-02-00-Methods-Lists-Streams.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-03-00. Objects, Classes & Maps](01-03-00-Objects-Classes-Maps.md)

---

# 01-02-01. Methods, Lists & Stream API: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на методи, `varargs`, работа с `ArrayList` и най-важните операции в Java Stream API от нулата.

## 🧭 Съдържание на темата
* [1. Методи, Method Overloading и `varargs` (`...`)](#1-методи-method-overloading-и-varargs-)
* [2. Работа с `ArrayList` (Създаване, добавяне, търсене и сортиране)](#2-работа-с-arraylist-създаване-добавяне-търсене-и-сортиране)
* [3. Четене на `List<Integer>` от конзолата на един ред](#3-четене-на-listinteger-от-конзолата-на-един-ред)
* [4. Stream API в действие (Числови потоци `IntStream`, сортиране и статистика)](#4-stream-api-в-действие-числови-потоци-intstream-сортиране-и-статистика)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. МЕТОДИ, METHOD OVERLOADING И `VARARGS` (`...`)

В Java всички методи живеят вътре в клас. Ако ги викаш от `main()`, трябва да бъдат **`static`**:

```java
public class MethodsDemo {
    // 1. Стандартен метод с връщан тип
    public static int add(int a, int b) {
        return a + b;
    }

    // 2. Method Overloading (същото име, различни параметри)
    public static double add(double a, double b) {
        return a + b;
    }

    // 3. Varargs (аналог на C# params int[] и Python *args) -> може да е само ПОСЛЕДЕН параметър!
    public static int sumAll(int... numbers) {
        int total = 0;
        for (int n : numbers) {
            total += n;
        }
        return total;
    }

    public static void main(String[] args) {
        System.out.println(sumAll(10, 20, 30, 40)); // 100
    }
}
```

---

## 2. РАБОТА С `ARRAYLIST` (СЪЗДАВАНЕ, ДОБАВЯНЕ, ТЪРСЕНЕ И СОРТИРАНЕ)

Винаги декларирай променливата по интерфейса **`List<T>`**, а инстанцирай класа **`ArrayList<>()`** (Dependency Inversion):

```java
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class ListOperations {
    public static void main(String[] args) {
        List<String> names = new ArrayList<>();

        // Добавяне и вмъкване
        names.add("Елизабет");          // Добавя в края
        names.add(0, "Иван");           // Вмъква на индекс 0

        // Четене и промяна
        String first = names.get(0);    // "Иван"
        names.set(1, "Мария");          // Заменя елемента на индекс 1

        // Проверки и изтриване
        boolean hasMaria = names.contains("Мария"); // true
        names.remove("Иван");           // Изтрива по стойност
        names.remove(0);                // Изтрива по индекс

        // Сортиране
        Collections.sort(names);                        // Възходящо
        names.sort(Collections.reverseOrder());         // Низходящо
    }
}
```

---

## 3. ЧЕТЕНЕ НА `LIST<INTEGER>` ОТ КОНЗОЛАТА НА ЕДИН РЕД

```java
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;
import java.util.stream.Collectors;

Scanner scanner = new Scanner(System.in);

// Вход: "5 12 8 20 3" -> Изменяем List<Integer>
List<Integer> nums = Arrays.stream(scanner.nextLine().split("\\s+"))
        .map(Integer::parseInt)
        .collect(Collectors.toList());
```

---

## 4. STREAM API В ДЕЙСТВИЕ (ЧИСЛОВИ ПОТОЦИ `INTSTREAM`, СОРТИРАНЕ И СТАТИСТИКА)

```java
import java.util.Comparator;
import java.util.List;

List<Integer> values = List.of(15, 4, 22, 8, 4, 30);

// 1. Уникални четни числа, сортирани низходящо
List<Integer> processed = values.stream()
        .distinct()                                 // Премахва дубликати
        .filter(n -> n % 2 == 0)                    // Само четни
        .sorted(Comparator.reverseOrder())          // Низходящ ред: [30, 22, 8, 4]
        .limit(3)                                   // Взима първите 3 (аналог на C# .Take(3))
        .toList();

// 2. Сума, Средно аритметично и Максимум чрез mapToInt()
int sum = values.stream().mapToInt(Integer::intValue).sum();
double avg = values.stream().mapToInt(Integer::intValue).average().orElse(0.0);
int max = values.stream().mapToInt(Integer::intValue).max().orElse(0);
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис в Java | Аналог в C# (LINQ) |
| :--- | :--- | :--- |
| **Създаване на празен списък** | `List<String> l = new ArrayList<>();` | `var l = new List<string>();` |
| **Бърз непроменяем списък** | `List.of(1, 2, 3)` | `new[] { 1, 2, 3 }.AsReadOnly()` |
| **Премахване по условие** | `list.removeIf(x -> x < 0)` | `list.RemoveAll(x => x < 0)` |
| **Филтриране (`Where`)** | `.filter(x -> x > 10)` | `.Where(x => x > 10)` |
| **Проекция (`Select`)** | `.map(String::toUpperCase)` | `.Select(s => s.ToUpper())` |
| **Сортиране (`OrderBy`)** | `.sorted(Comparator.comparing(...))` | `.OrderBy(x => x.Prop)` |
| **Пропускане / Взимане (`Skip`/`Take`)** | `.skip(5).limit(10)` | `.Skip(5).Take(10)` |
| **Проверка за поне 1 / всички** | `.anyMatch(...)` / `.allMatch(...)` | `.Any(...)` / `.All(...)` |

---
[⬅️ 01-02-00. Теория & Капани](01-02-00-Methods-Lists-Streams.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-03-00. Objects, Classes & Maps](01-03-00-Objects-Classes-Maps.md)
