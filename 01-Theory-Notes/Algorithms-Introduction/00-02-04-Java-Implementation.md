[⬅️ 00-02-03. JavaScript Implementation](00-02-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-00. Sorting Algorithms](00-03-00-Sorting-Algorithms.md)

---

# 00-02-04. Searching Algorithms: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на Linear Search, Binary Search без Integer Overflow и вградения `Arrays.binarySearch` в Java от нулата.

## 🧭 Съдържание на файла
* [1. Linear Search (Линейно търсене)](#1-linear-search-линейно-търсене)
* [2. Binary Search без Integer Overflow (`left + (right - left) / 2`)](#2-binary-search-без-integer-overflow-left--right---left--2)
* [3. Вграденият `Arrays.binarySearch` и отрицателният индекс `-(insertionPoint) - 1`](#3-вграденият-arraysbinarysearch-и-отрицателният-индекс--insertionpoint---1)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. LINEAR SEARCH (ЛИНЕЙНО ТЪРСЕНЕ)

```java
public class SearchingAlgorithms {
    public static int linearSearch(int[] arr, int target) {
        for (int i = 0; i < arr.length; i++) {
            if (arr[i] == target) return i;
        }
        return -1;
    }
}
```

---

## 2. BINARY SEARCH БЕЗ INTEGER OVERFLOW (`LEFT + (RIGHT - LEFT) / 2`)

*Любопитен факт: Прочутият бъг `(left + right) / 2` в стандартната библиотека на Java (`java.util.Arrays`) съществуваше 9 години, преди Джошуа Блок да го открие и поправи с `left + ((right - left) >>> 1)`!*

```java
public class BinarySearchDemo {
    public static int binarySearch(int[] arr, int target) {
        int left = 0;
        int right = arr.length - 1;

        while (left <= right) {
            int mid = left + (right - left) / 2;

            if (arr[mid] == target) {
                return mid;
            } else if (arr[mid] < target) {
                left = mid + 1;
            } else {
                right = mid - 1;
            }
        }

        return -1;
    }
}
```

---

## 3. ВГРАДЕНИЯТ `ARRAYS.BINARYSEARCH` И ОТРИЦАТЕЛНИЯТ ИНДЕКС

Точно както в C#, вграденият метод `Arrays.binarySearch(arr, target)` връща отрицателно число при липса на елемента:

```java
import java.util.Arrays;

public class BuiltInSearch {
    public static void main(String[] args) {
        int[] numbers = {10, 20, 30, 40};
        int idx = Arrays.binarySearch(numbers, 25);

        if (idx >= 0) {
            System.out.println("Намерено на индекс: " + idx);
        } else {
            int insertPos = -(idx + 1); // Или побитово ~idx (индекс 2)
            System.out.println("Липсва! Позиция за вмъкване: " + insertPos);
        }
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Вграден Binary Search (Масив)** | `Arrays.binarySearch(arr, key)` | Работи за $O(\log n)$ върху сортиран масив |
| **Вграден Binary Search (Списък)** | `Collections.binarySearch(list, key)` | За `ArrayList<T>` ($O(\log n)$) |
| **Позиция за вмъкване при `idx < 0`** | `int pos = ~idx;` или `-(idx + 1)` | Идентично с C# `Array.BinarySearch` |

---
[⬅️ 00-02-03. JavaScript Implementation](00-02-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-04-Java-Implementation.md](00-0-04-Java-Implementation.md)
