[⬅️ 01-03-00. Теория & Капани](01-03-00-Arrays.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-04-00. Methods](01-04-00-Methods.md)

---

# 01-03-01. Arrays: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване, четене от конзолата с `Arrays.stream()`, обхождане, обръщане и принтиране на статични масиви в Java от нулата.

## 🧭 Съдържание на темата
* [1. Създаване и обхождане на масиви (`for` и `for-each`)](#1-създаване-и-обхождане-на-масиви-for-и-for-each)
* [2. Четене на масив от конзолата на един ред (`String[]`, `int[]`, `double[]`)](#2-четене-на-масив-от-конзолата-на-един-ред-string-int-double)
* [3. Принтиране на масив без цикъл (`String.join` и `replaceAll`)](#3-принтиране-на-масив-без-цикъл-stringjoin-и-replaceall)
* [4. Класически алгоритмични операции (Обръщане In-Place и 2D Матрица)](#4-класически-алгоритмични-операции-обръщане-in-place-и-2d-матрица)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ И ОБХОЖДАНЕ НА МАСИВИ (`FOR` И `FOR-EACH`)

```java
public class ArraysBasics {
    public static void main(String[] args) {
        // 1. Създаване с фиксиран размер (стойностите са 0 по подразбиране)
        int[] numbers = new int[4];
        numbers[0] = 10;
        numbers[1] = 20;

        // 2. Директна инициализация със стойности
        String[] days = {"Monday", "Tuesday", "Wednesday"};

        // 3. Обхождане с индекс (когато искаш да променяш елементите или ти трябва индексът)
        for (int i = 0; i < numbers.length; i++) {
            numbers[i] *= 2;
        }

        // 4. Обхождане с Enhanced for-each (само за четене!)
        for (String day : days) {
            System.out.println(day);
        }
    }
}
```

---

## 2. ЧЕТЕНЕ НА МАСИВ ОТ КОНЗОЛАТА НА ЕДИН РЕД (`STRING[]`, `INT[]`, `DOUBLE[]`)

Използваме регулярен израз `"\\s+"`, за да разделим думите по един или повече интервала без риск от `NumberFormatException`:

```java
import java.util.Arrays;
import java.util.Scanner;

public class ReadArraysFromConsole {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // 1. Масив от текстове (напр. "Ivan Maria Georgi")
        String[] words = scanner.nextLine().split("\\s+");

        // 2. Масив от цели числа (напр. "10 20 30 40")
        int[] intArr = Arrays.stream(scanner.nextLine().split("\\s+"))
                .mapToInt(Integer::parseInt)
                .toArray();

        // 3. Масив от дробни числа (напр. "2.5 4.75 10.0")
        double[] doubleArr = Arrays.stream(scanner.nextLine().split("\\s+"))
                .mapToDouble(Double::parseDouble)
                .toArray();
    }
}
```

---

## 3. ПРИНТИРАНЕ НА МАСИВ БЕЗ ЦИКЪЛ (`STRING.JOIN` И `REPLACEALL`)

```java
import java.util.Arrays;

String[] names = {"Анна", "Борис", "Виктор"};
int[] scores = {100, 200, 300};

// 1. String.join() работи САМО върху String[] (или List<String>)
System.out.println(String.join(", ", names)); // Изход: Анна, Борис, Виктор

// 2. За int[] или double[] ползваме Arrays.toString() + премахване на скобите [ и ]
System.out.println(Arrays.toString(scores).replaceAll("[\\[\\]]", "")); // Изход: 100, 200, 300
```

---

## 4. КЛАСИЧЕСКИ АЛГОРИТМИЧНИ ОПЕРАЦИИ (ОБРЪЩАНЕ IN-PLACE И 2D МАТРИЦА)

```java
import java.util.Arrays;

public class MatrixAndReverse {
    public static void main(String[] args) {
        // 1. Обръщане на масив на място (In-Place Reverse за O(n) време и O(1) памет)
        int[] arr = {1, 2, 3, 4, 5};
        for (int i = 0; i < arr.length / 2; i++) {
            int temp = arr[i];
            arr[i] = arr[arr.length - 1 - i];
            arr[arr.length - 1 - i] = temp;
        }
        System.out.println(Arrays.toString(arr)); // [5, 4, 3, 2, 1]

        // 2. Двумерен масив (Матрица 2x3)
        int[][] matrix = {
            {1, 2, 3},
            {4, 5, 6}
        };
        System.out.println(Arrays.deepToString(matrix)); // [[1, 2, 3], [4, 5, 6]]
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Операция | Синтаксис в Java (`java.util.Arrays`) | Какво прави |
| :--- | :--- | :--- |
| **Сортиране на масив** | `Arrays.sort(arr)` | Сортира на място ($O(n \log n)$) |
| **Двоично търсене** | `Arrays.binarySearch(arr, key)` | Връща индекс (работи само върху сортиран масив!) |
| **Запълване с една стойност** | `Arrays.fill(arr, -1)` | Запълва всички клетки с `-1` |
| **Копиране на масив / отрязък** | `Arrays.copyOf(arr, newLen)` / `copyOfRange(arr, from, to)` | Създава нов независим масив |
| **Сравнение на 1D / 2D масиви** | `Arrays.equals(a, b)` / `Arrays.deepEquals(m1, m2)` | Сравнява елементите по стойност |
| **Принтиране на 1D / 2D масиви** | `Arrays.toString(arr)` / `Arrays.deepToString(m)` | Превръща масива в четим текст |
| **Бърза сума / мин / макс** | `Arrays.stream(arr).sum()` / `.min()` / `.max()` | Използва `IntStream` на един ред |

---
[⬅️ 01-03-00. Теория & Капани](01-03-00-Arrays.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-04-00. Methods](01-04-00-Methods.md)
