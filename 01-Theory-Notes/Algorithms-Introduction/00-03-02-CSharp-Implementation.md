[⬅️ 00-03-01. Python Implementation](00-03-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-03. JavaScript Implementation](00-03-03-JavaScript-Implementation.md)

---

# 00-03-02. Sorting Algorithms: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на Insertion Sort, Merge Sort и Quick Sort в C#, плюс вградените `Array.Sort` и LINQ `OrderBy` от нулата.

## 🧭 Съдържание на файла
* [1. Insertion Sort и размяна с Tuple Deconstruction](#1-insertion-sort-и-размяна-с-tuple-deconstruction)
* [2. Merge Sort (Рекурсивно сливане в C#)](#2-merge-sort-рекурсивно-сливане-в-c)
* [3. Quick Sort (In-Place в C#)](#3-quick-sort-in-place-в-c)
* [4. Вградено сортиране в .NET (`Array.Sort` срещу LINQ `OrderBy`)](#4-вградено-сортиране-в-net-arraysort-срещу-linq-orderby)
* [5. C# Специфики & Cheat Sheet](#5-c-специфики--cheat-sheet)

---

## 1. INSERTION SORT И РАЗМЯНА С TUPLE DECONSTRUCTION

```csharp
public class BasicSorts
{
    public static void InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            int key = arr[i];
            int j = i - 1;

            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
    }
}
```

---

## 2. MERGE SORT (РЕКУРСИВНО СЛИВАНЕ В C#)

```csharp
public class MergeSortDemo
{
    public static void Sort(int[] arr)
    {
        if (arr.Length <= 1) return;
        MergeSortRecursive(arr, 0, arr.Length - 1);
    }

    private static void MergeSortRecursive(int[] arr, int left, int right)
    {
        if (left >= right) return;

        int mid = left + (right - left) / 2;
        MergeSortRecursive(arr, left, mid);
        MergeSortRecursive(arr, mid + 1, right);
        Merge(arr, left, mid, right);
    }

    private static void Merge(int[] arr, int left, int mid, int right)
    {
        int[] temp = new int[right - left + 1];
        int i = left, j = mid + 1, k = 0;

        while (i <= mid && j <= right)
        {
            if (arr[i] <= arr[j]) temp[k++] = arr[i++];
            else temp[k++] = arr[j++];
        }

        while (i <= mid) temp[k++] = arr[i++];
        while (j <= right) temp[k++] = arr[j++];

        Array.Copy(temp, 0, arr, left, temp.Length);
    }
}
```

---

## 3. QUICK SORT (IN-PLACE В C#)

```csharp
public class QuickSortDemo
{
    public static void Sort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(arr, low, high);
            Sort(arr, low, pi - 1);
            Sort(arr, pi + 1, high);
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                // Модерен C# синтаксис за размяна без temp променлива:
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
        return i + 1;
    }
}
```

---

## 4. ВГРАДЕНО СОРТИРАНЕ В .NET (`ARRAY.SORT` СРЕЩУ LINQ `ORDERBY`)

```csharp
using System;
using System.Linq;

int[] nums = { 5, 1, 9, 3 };

// 1. In-Place сортиране (IntroSort - най-бързо, нулева допълнителна памет)
Array.Sort(nums);

// 2. LINQ стабилно сортиране (връща нова колекция, не променя оригинала)
var sortedStudents = students
    .OrderByDescending(s => s.Grade)
    .ThenBy(s => s.Name)
    .ToList();
```

---

## 5. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Размяна на 2 стойности** | `(arr[i], arr[j]) = (arr[j], arr[i]);` | Въведено в C# 7.0 |
| **In-Place сортиране на масив** | `Array.Sort(arr)` | Ползва IntroSort ($O(n \log n)$) |
| **Обратно обръщане на масив** | `Array.Reverse(arr)` | Работи за $O(n)$ време на място |
| **Стабилно многокритериално** | `list.OrderBy(x => x.A).ThenByDescending(x => x.B)` | LINQ ползва стабилен Merge Sort |

---
[⬅️ 00-03-01. Python Implementation](00-03-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-03. JavaScript Implementation](00-03-03-JavaScript-Implementation.md)
