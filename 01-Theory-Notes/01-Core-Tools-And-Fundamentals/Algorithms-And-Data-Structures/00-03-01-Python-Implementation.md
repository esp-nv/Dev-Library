[⬅️ 00-03-00. Теория & Капани](00-03-00-Sorting-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-02. C# Implementation](00-03-02-CSharp-Implementation.md)

---

# 00-03-01. Sorting Algorithms: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на Bubble Sort, Selection Sort, Merge Sort и Quick Sort, плюс професионално сортиране с `key=` и `functools.cmp_to_key` от нулата.

## 🧭 Съдържание на файла
* [1. Bubble Sort и Selection Sort (Базови O(n²))](#1-bubble-sort-и-selection-sort-базови-on2)
* [2. Merge Sort (Рекурсивно сливане — \(O(n \log n)\))](#2-merge-sort-рекурсивно-сливане--on-log-n)
* [3. Quick Sort (Lomuto / Hoare Partitioning — \(O(n \log n)\))](#3-quick-sort-lomuto--hoare-partitioning--on-log-n)
* [4. Вграденият Timsort (`sort` срещу `sorted` и многокритериално сортиране)](#4-вграденият-timsort-sort-срещу-sorted-и-многокритериално-сортиране)
* [5. Python Специфики & Cheat Sheet](#5-python-специфики--cheat-sheet)

---

## 1. BUBBLE SORT И SELECTION SORT (БАЗОВИ O(N²))

```python
# 1. Bubble Sort с оптимизация за ранно спиране
def bubble_sort(arr: list[int]) -> list[int]:
    n = len(arr)
    for i in range(n):
        swapped = False
        for j in range(0, n - i - 1):
            if arr[j] > arr[j + 1]:
                arr[j], arr[j + 1] = arr[j + 1], arr[j]  # Pythonic размяна (Swap)
                swapped = True
        if not swapped:
            break  # Масивът вече е сортиран!
    return arr

# 2. Selection Sort
def selection_sort(arr: list[int]) -> list[int]:
    n = len(arr)
    for i in range(n):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        arr[i], arr[min_idx] = arr[min_idx], arr[i]
    return arr
```

---

## 2. MERGE SORT (РЕКУРСИВНО СЛИВАНЕ — \(O(N \LOG N)\))

```python
def merge_sort(arr: list[int]) -> list[int]:
    if len(arr) <= 1:
        return arr

    mid = len(arr) // 2
    left_half = merge_sort(arr[:mid])
    right_half = merge_sort(arr[mid:])

    return _merge(left_half, right_half)

def _merge(left: list[int], right: list[int]) -> list[int]:
    merged = []
    i = j = 0

    while i < len(left) and j < len(right):
        if left[i] <= right[j]:  # '<=' гарантира стабилност (Stability)!
            merged.append(left[i])
            i += 1
        else:
            merged.append(right[j])
            j += 1

    merged.extend(left[i:])
    merged.extend(right[j:])
    return merged

print(merge_sort([38, 27, 43, 3, 9, 82, 10]))
```

---

## 3. QUICK SORT (IN-PLACE PARTITIONING — \(O(N \LOG N)\))

```python
def quick_sort(arr: list[int], low: int, high: int) -> None:
    if low < high:
        pivot_idx = _partition(arr, low, high)
        quick_sort(arr, low, pivot_idx - 1)
        quick_sort(arr, pivot_idx + 1, high)

def _partition(arr: list[int], low: int, high: int) -> int:
    pivot = arr[high]  # Избираме последния елемент за опорен (Pivot)
    i = low - 1

    for j in range(low, high):
        if arr[j] <= pivot:
            i += 1
            arr[i], arr[j] = arr[j], arr[i]

    arr[i + 1], arr[high] = arr[high], arr[i + 1]
    return i + 1

nums = [10, 7, 8, 9, 1, 5]
quick_sort(nums, 0, len(nums) - 1)
print(nums)  # Изход: [1, 5, 7, 8, 9, 10]
```

---

## 4. ВГРАДЕНИЯТ TIMSORT (`SORT` СРЕЩУ `SORTED` И МНОГОКРИТЕРИАЛНО СОРТИРАНЕ)

```python
students = [
    {"name": "Борис", "grade": 6.00, "age": 22},
    {"name": "Анна",  "grade": 6.00, "age": 20},
    {"name": "Иван",  "grade": 5.50, "age": 21},
]

# Сортиране по оценка НИЗХОДЯЩО (-x["grade"]), а при равна оценка по име ВЪЗХОДЯЩО (x["name"]):
ranked = sorted(students, key=lambda x: (-x["grade"], x["name"]))
print(ranked)
```

---

## 5. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Python | Бележка |
| :--- | :--- | :--- |
| **Сортиране на място (In-place)** | `arr.sort(reverse=True)` | Променя оригиналния списък и връща `None` |
| **Ново сортирано копие** | `new_arr = sorted(iterable)` | Работи върху всякаква колекция (списък, сет, кортеж) |
| **Многокритериално сортиране** | `key=lambda x: (-x.score, x.name)` | Минусът обръща реда за числови полета |
| **Размяна на два елемента** | `arr[i], arr[j] = arr[j], arr[i]` | Без временна променлива `temp` |

---
[⬅️ 00-03-00. Теория & Капани](00-03-00-Sorting-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-02. C# Implementation](00-03-02-CSharp-Implementation.md)
