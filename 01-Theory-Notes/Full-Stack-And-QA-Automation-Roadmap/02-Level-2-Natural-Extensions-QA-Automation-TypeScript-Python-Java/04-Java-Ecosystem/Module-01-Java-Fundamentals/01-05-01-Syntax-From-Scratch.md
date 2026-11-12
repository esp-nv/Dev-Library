[⬅️ 01-05-00. Теория & Капани](01-05-00-Lists.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-06-00. Objects, Classes & Records](01-06-00-Objects-And-Classes.md)

---

# 01-05-01. Lists (`ArrayList` & `LinkedList`): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за четене на списък от конзолата, добавяне, вмъкване, изтриване, сортиране и бързо принтиране на `List<T>` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Четене на `List<String>`, `List<Integer>` и `List<Double>` от конзолата](#1-четене-на-liststring-listinteger-и-listdouble-от-конзолата)
* [2. Основни операции с `ArrayList` (CRUD върху списък)](#2-основни-операции-с-arraylist-crud-върху-списък)
* [3. Сортиране, обръщане и търсене в `List<T>`](#3-сортиране-обръщане-и-търсене-в-listt)
* [4. Бързо принтиране на `List<T>` (за Judge изпити)](#4-бързо-принтиране-на-listt-за-judge-изпити)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ЧЕТЕНЕ НА `LIST<STRING>`, `LIST<INTEGER>` И `LIST<DOUBLE>` ОТ КОНЗОЛАТА

```java
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;
import java.util.stream.Collectors;

public class ReadListsDemo {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // 1. Четене на списък от текстове (изменяем ArrayList)
        List<String> stringList = Arrays.stream(scanner.nextLine().split("\\s+"))
                .collect(Collectors.toList());

        // 2. Четене на списък от цели числа (изменяем ArrayList)
        List<Integer> integerList = Arrays.stream(scanner.nextLine().split("\\s+"))
                .map(Integer::parseInt)
                .collect(Collectors.toList());

        // 3. Четене на списък от дробни числа
        List<Double> doubleList = Arrays.stream(scanner.nextLine().split("\\s+"))
                .map(Double::parseDouble)
                .collect(Collectors.toList());
    }
}
```

---

## 2. ОСНОВНИ ОПЕРАЦИИ С `ARRAYLIST` (CRUD ВЪРХУ СПИСЪК)

```java
import java.util.ArrayList;
import java.util.List;

List<Integer> nums = new ArrayList<>();

// 1. Добавяне и вмъкване
nums.add(10);                       // Добавя 10 в края ->
nums.add(30);                       // Добавя 30 в края ->
nums.add(1, 20);                    // Вмъква 20 на индекс 1 ->
nums.addAll(List.of(40, 50));       // Добавя всички елементи от друга колекция ->

// 2. Четене и проверки
int first = nums.get(0);            // 10
int count = nums.size();            // 5
boolean empty = nums.isEmpty();     // false
boolean has30 = nums.contains(30);  // true
int idxOf30 = nums.indexOf(30);     // 2 (връща -1, ако елементът липсва)

// 3. Замяна и изтриване
nums.set(0, 99);                    // Заменя елемента на индекс 0 с 99 ->
nums.remove(1);                     // Изтрива елемента на ИНДЕКС 1 (числото 20) ->
nums.remove(Integer.valueOf(40));   // Изтрива СТОЙНОСТТА 40 ->
```

---

## 3. СОРТИРАНЕ, ОБРЪЩАНЕ И ТЪРСЕНЕ В `LIST<T>`

```java
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

List<Integer> scores = new ArrayList<>(List.of(45, 12, 89, 30));

Collections.sort(scores);                           // Възходящо:
Collections.reverse(scores);                        // Обръща реда:
Collections.sort(scores, Collections.reverseOrder()); // Директно низходящо сортиране

int maxScore = Collections.max(scores);             // 89
int minScore = Collections.min(scores);             // 12
```

---

## 4. БЪРЗО ПРИНТИРАНЕ НА `LIST<T>` (ЗА JUDGE ИЗПИТИ)

```java
List<String> words = List.of("Java", "Spring", "Boot");
List<Integer> numbers = List.of(10, 20, 30);

// 1. За List<String> ползваме директно String.join()
System.out.println(String.join(", ", words)); // Изход: Java, Spring, Boot

// 2. За List<Integer> или List<Double> премахваме скобите и запетаите с replaceAll()
System.out.println(numbers.toString().replaceAll("[\\[\\],]", "")); // Изход: 10 20 30
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод на `List<T>` | Какво прави | Сложност (`ArrayList`) |
| :--- | :--- | :--- |
| **`list.add(el)`** | Добавя елемент в края на списъка | $O(1)$ амортизирано |
| **`list.add(idx, el)`** | Вмъква елемент на конкретен индекс | $O(n)$ |
| **`list.get(idx)` / `list.set(idx, el)`** | Чете / Заменя елемент на индекс | $O(1)$ |
| **`list.remove(idx)`** | Изтрива елемент на индекс и връща стойността му | $O(n)$ |
| **`list.remove(Integer.valueOf(val))`** | Изтрива първото срещане на стойността | $O(n)$ |
| **`list.removeIf(x -> condition)`** | Безопасно изтрива всички елементи по условие | $O(n)$ |
| **`list.contains(el)` / `list.indexOf(el)`** | Проверява дали съществува / намира индекса | $O(n)$ |
| **`Collections.sort(list)`** | Сортира списъка на място | $O(n \log n)$ |
| **`list.subList(from, to)`** | Връща изглед (View) към част от списъка | $O(1)$ |

---
[⬅️ 01-05-00. Теория & Капани](01-05-00-Lists.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-06-00. Objects, Classes & Records](01-06-00-Objects-And-Classes.md)
