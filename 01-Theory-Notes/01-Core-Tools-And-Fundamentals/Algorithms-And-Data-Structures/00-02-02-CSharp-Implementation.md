[⬅️ 00-02-01. Python Implementation](00-02-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-03. JavaScript Implementation](00-02-03-JavaScript-Implementation.md)

---

# 00-02-02. Searching Algorithms: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на Linear Search, Binary Search без Integer Overflow, Lower Bound и вградения `Array.BinarySearch` от нулата.

## 🧭 Съдържание на файла
* [1. Linear Search (Линейно търсене)](#1-linear-search-линейно-търсене)
* [2. Binary Search без Integer Overflow (`left + (right - left) / 2`)](#2-binary-search-без-integer-overflow-left--right---left--2)
* [3. Намиране на първо срещане (Lower Bound)](#3-намиране-на-първо-срещане-lower-bound)
* [4. Вграденият `Array.BinarySearch` и капанът с побитовото допълнение (`~`)](#4-вграденият-arraybinarysearch-и-капанът-с-побитовото-допълнение-)
* [5. C# Специфики & Cheat Sheet](#5-c-специфики--cheat-sheet)

---

## 1. LINEAR SEARCH (ЛИНЕЙНО ТЪРСЕНЕ)

```csharp
public class SearchingAlgorithms
{
    public static int LinearSearch(int[] arr, int target)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == target)
                return i;
        }
        return -1;
    }
}
```

---

## 2. BINARY SEARCH БЕЗ INTEGER OVERFLOW (`LEFT + (RIGHT - LEFT) / 2`)

```csharp
public class BinarySearchDemo
{
    public static int BinarySearch(int[] arr, int target)
    {
        int left = 0;
        int right = arr.Length - 1;

        while (left <= right)
        {
            // Задължителна формула в C#, за да не прелее int при големи индекси!
            int mid = left + (right - left) / 2;

            if (arr[mid] == target)
                return mid;
            else if (arr[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }
}
```

---

## 3. НАМИРАНЕ НА ПЪРВО СРЕЩАНЕ (LOWER BOUND)

```csharp
public static int FindFirstOccurrence(int[] arr, int target)
{
    int left = 0, right = arr.Length - 1;
    int firstIndex = -1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;
        if (arr[mid] == target)
        {
            firstIndex = mid;
            right = mid - 1; // Продължаваме да търсим по-наляво
        }
        else if (arr[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return firstIndex;
}
```

---

## 4. ВГРАДЕНИЯТ `ARRAY.BINARYSEARCH` И КАПАНЪТ С ПОБИТОВОТО ДОПЪЛНЕНИЕ (`~`)

В C# вграденият метод `Array.BinarySearch(arr, target)` **не връща `-1`**, когато елементът липсва! Вместо това връща **отрицателно число**, което представлява побитовото допълнение (`~`) на индекса, на който числото би трябвало да се вмъкне, за да остане масивът сортиран:

```csharp
using System;

int[] numbers = { 10, 20, 30, 40 };
int index = Array.BinarySearch(numbers, 25);

if (index >= 0)
{
    Console.WriteLine(\$"Намерено на индекс: {index}");
}
else
{
    // Побитовото обръщане (~index) ни дава точната позиция за вмъкване (индекс 2)!
    int insertIndex = ~index;
    Console.WriteLine(\$"Не е намерено! Трябва да се вмъкне на индекс: {insertIndex}");
}
```

---

## 5. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Линейно търсене** | `Array.IndexOf(arr, target)` | Връща `-1` ако липсва ($O(n)$) |
| **Вграден Binary Search** | `int idx = Array.BinarySearch(arr, x);` | Ако `idx < 0`, елементът липсва ($O(\log n)$) |
| **Позиция за вмъкване при `idx < 0`** | `int insertPos = ~idx;` | Побитово НЕ (`Bitwise Complement`) |
| **Binary Search върху `Span<T>`** | `arr.AsSpan().BinarySearch(x)` | Нулеви алокации в модерния .NET |

---
[⬅️ 00-02-01. Python Implementation](00-02-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-03. JavaScript Implementation](00-02-03-JavaScript-Implementation.md)
