[⬅️ 00-02-00. Теория & Капани](00-02-00-Searching-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-02. C# Implementation](00-02-02-CSharp-Implementation.md)

---

# 00-02-01. Searching Algorithms: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на Linear Search, итеративен и рекурсивен Binary Search, търсене на първо/последно срещане и вградения модул `bisect` от нулата.

## 🧭 Съдържание на файла
* [1. Linear Search (Линейно търсене)](#1-linear-search-линейно-търсене)
* [2. Binary Search (Итеративен и Рекурсивен)](#2-binary-search-итеративен-и-рекурсивен)
* [3. Намиране на първо и последно срещане (Lower / Upper Bound)](#3-намиране-на-първо-и-последно-срещане-lower--upper-bound)
* [4. Вграденият модул `bisect` в Python (Професионален стандарт)](#4-вграденият-модул-bisect-в-python-професионален-стандарт)
* [5. Python Специфики & Cheat Sheet](#5-python-специфики--cheat-sheet)

---

## 1. LINEAR SEARCH (ЛИНЕЙНО ТЪРСЕНЕ)

```python
def linear_search(nums: list[int], target: int) -> int:
    for index, value in enumerate(nums):
        if value == target:
            return index
    return -1

print(linear_search([40, 10, 80, 20], 80))  # Изход: 2
```

---

## 2. BINARY SEARCH (ИТЕРАТИВЕН И РЕКУРСИВЕН)

```python
# 1. Итеративен Binary Search (Препоръчителен: O(log n) време, O(1) памет)
def binary_search_iter(nums: list[int], target: int) -> int:
    left, right = 0, len(nums) - 1

    while left <= right:
        mid = left + (right - left) // 2
        if nums[mid] == target:
            return mid
        elif nums[mid] < target:
            left = mid + 1
        else:
            right = mid - 1

    return -1

# 2. Рекурсивен Binary Search (O(log n) време, O(log n) стекова памет)
def binary_search_rec(nums: list[int], target: int, left: int, right: int) -> int:
    if left > right:
        return -1

    mid = left + (right - left) // 2
    if nums[mid] == target:
        return mid
    elif nums[mid] < target:
        return binary_search_rec(nums, target, mid + 1, right)
    else:
        return binary_search_rec(nums, target, left, mid - 1)
```

---

## 3. НАМИРАНЕ НА ПЪРВО И ПОСЛЕДНО СРЕЩАНЕ (LOWER / UPPER BOUND)

```python
def find_first_occurrence(nums: list[int], target: int) -> int:
    left, right = 0, len(nums) - 1
    first_idx = -1

    while left <= right:
        mid = left + (right - left) // 2
        if nums[mid] == target:
            first_idx = mid     # Запомняме позицията и продължаваме НАЛЯВО!
            right = mid - 1
        elif nums[mid] < target:
            left = mid + 1
        else:
            right = mid - 1

    return first_idx

data = [1, 3, 5, 5, 5, 5, 8, 10]
print(find_first_occurrence(data, 5))  # Изход: 2 (първата петица)
```

---

## 4. ВГРАДЕНИЯТ МОДУЛ `BISECT` В PYTHON (ПРОФЕСИОНАЛЕН СТАНДАРТ)

Python има вграден модул **`bisect`**, написан на C, който извършва двоично търсене за микросекунди:

```python
import bisect

sorted_nums = [10, 20, 20, 20, 30, 40]

# 1. Намира най-левия индекс, където може да се вмъкне 20 (индекс 1)
left_pos = bisect.bisect_left(sorted_nums, 20)

# 2. Намира най-десния индекс след всички 20-ки (индекс 4)
right_pos = bisect.bisect_right(sorted_nums, 20)

# Брой срещания на числото 20 в сортиран списък за O(log n) време:
count_of_20 = right_pos - left_pos  # 4 - 1 = 3 пъти!

# Проверка дали елемент съществува чрез bisect_left:
exists = left_pos < len(sorted_nums) and sorted_nums[left_pos] == 20
print(f"Съществува: {exists} | Брой срещания: {count_of_20}")
```

---

## 5. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Python | Сложност |
| :--- | :--- | :--- |
| **Линейно търсене в списък** | `nums.index(target)` (хвърля `ValueError`) | $O(n)$ |
| **Целочислено делене за среда** | `mid = (left + right) // 2` | $O(1)$ |
| **Най-ляв индекс (Lower Bound)** | `bisect.bisect_left(arr, target)` | $O(\log n)$ |
| **Най-десен индекс (Upper Bound)** | `bisect.bisect_right(arr, target)` | $O(\log n)$ |
| **Вмъкване със запазване на реда** | `bisect.insort(arr, new_val)` | $O(n)$ (заради изместването) |

---
[⬅️ 00-02-00. Теория & Капани](00-02-00-Searching-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-02. C# Implementation](00-02-02-CSharp-Implementation.md)
