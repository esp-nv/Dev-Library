[⬅️ 00-03-03. JavaScript Implementation](00-03-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-00. Greedy Algorithms](00-04-00-Greedy-Algorithms.md)

---

# 00-03-04. Sorting Algorithms: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на Merge Sort и Quick Sort в Java, плюс вградените `Arrays.sort` (Dual-Pivot Quicksort за примитиви и Timsort за обекти) от нулата.

## 🧭 Съдържание на файла
* [1. Merge Sort (Рекурсивно сливане в Java)](#1-merge-sort-рекурсивно-сливане-в-java)
* [2. Quick Sort (In-Place в Java)](#2-quick-sort-in-place-в-java)
* [3. Вграденото сортиране в Java (`Arrays.sort` и `Comparator.comparing`)](#3-вграденото-сортиране-в-java-arrayssort-и-comparatorcomparing)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. MERGE SORT (РЕКУРСИВНО СЛИВАНЕ В JAVA)

```java
public class MergeSortDemo {
    public static void mergeSort(int[] arr, int left, int right) {
        if (left >= right) return;

        int mid = left + (right - left) / 2;
        mergeSort(arr, left, mid);
        mergeSort(arr, mid + 1, right);
        merge(arr, left, mid, right);
    }

    private static void merge(int[] arr, int left, int mid, int right) {
        int[] temp = new int[right - left + 1];
        int i = left, j = mid + 1, k = 0;

        while (i <= mid && j <= right) {
            if (arr[i] <= arr[j]) temp[k++] = arr[i++];
            else temp[k++] = arr[j++];
        }

        while (i <= mid) temp[k++] = arr[i++];
        while (j <= right) temp[k++] = arr[j++];

        System.arraycopy(temp, 0, arr, left, temp.length);
    }
}
```

---

## 2. QUICK SORT (IN-PLACE В JAVA)

```java
public class QuickSortDemo {
    public static void quickSort(int[] arr, int low, int high) {
        if (low < high) {
            int pi = partition(arr, low, high);
            quickSort(arr, low, pi - 1);
            quickSort(arr, pi + 1, high);
        }
    }

    private static int partition(int[] arr, int low, int high) {
        int pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++) {
            if (arr[j] <= pivot) {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }

        int temp = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp;
        return i + 1;
    }
}
```

---

## 3. ВГРАДЕНОТО СОРТИРАНЕ В JAVA (`ARRAYS.SORT` И `COMPARATOR.COMPARING`)

Java използва **два различни алгоритъма** според типа на данните:
*   За примитивни типове (`int[]`, `double[]`): **Dual-Pivot Quicksort** (много бърз, нестабилен, $O(n \log n)$).
*   За обекти (`List<Student>`, `String[]`): **Timsort** (стабилен, $O(n \log n)$).

```java
import java.util.*;

record Student(String name, double grade) {}

public class JavaSorting {
    public static void main(String[] args) {
        int[] nums = {5, 1, 9, 3};
        Arrays.sort(nums); // Dual-Pivot Quicksort

        List<Student> students = new ArrayList<>(List.of(
            new Student("Борис", 6.00),
            new Student("Анна", 6.00)
        ));

        // Многокритериално сортиране (по оценка низходящо, после по име възходящо)
        students.sort(
            Comparator.comparingDouble(Student::grade).reversed()
                      .thenComparing(Student::name)
        );
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Алгоритъм под капака |
| :--- | :--- | :--- |
| **Сортиране на примитиви (`int[]`)** | `Arrays.sort(arr)` | Dual-Pivot Quicksort ($O(n \log n)$) |
| **Сортиране на колекция (`List<T>`)** | `list.sort(Comparator.naturalOrder())` | Timsort ($O(n \log n)$, стабилен) |
| **Многокритериално сортиране** | `Comparator.comparing(X::a).thenComparing(X::b)` | Еквивалент на C# `OrderBy().ThenBy()` |

---
[⬅️ 00-03-03. JavaScript Implementation](00-03-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-00. Greedy Algorithms](00-04-00-Greedy-Algorithms.md)
