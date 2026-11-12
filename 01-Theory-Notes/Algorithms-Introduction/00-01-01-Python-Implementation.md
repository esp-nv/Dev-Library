[⬅️ 00-01-00. Теория & Капани](00-01-00-Recursion-And-Backtracking.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-02. C# Implementation](00-01-02-CSharp-Implementation.md)

---

# 00-01-01. Recursion & Backtracking: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на линейна рекурсия, сума на масив и намиране на всички пътища в лабиринт (Backtracking) от нулата.

## 🧭 Съдържание на файла
* [1. Базова рекурсия (Факториел и Сума на списък)](#1-базова-рекурсия-факториел-и-сума-на-списък)
* [2. Backtracking: Всички пътища в лабиринт (2D Матрица)](#2-backtracking-всички-пътища-в-лабиринт-2d-матрица)
* [3. Backtracking: Генериране на всички комбинации от битове (0 и 1)](#3-backtracking-генериране-на-всички-комбинации-от-битове-0-и-1)
* [4. Python Специфики & Cheat Sheet](#4-python-специфики--cheat-sheet)

---

## 1. БАЗОВА РЕКУРСИЯ (ФАКТОРИЕЛ И СУМА НА СПИСЪК)

```python
# 1. Факториел: n! = n * (n - 1)!  | Time: O(n), Space: O(n)
def factorial(n: int) -> int:
    if n <= 1:                      # Base Case (Дъно)
        return 1
    return n * factorial(n - 1)     # Recursive Step

# 2. Рекурсивна сума на списък без слайсинг (пести памет чрез индекс)
def array_sum(nums: list[int], index: int = 0) -> int:
    if index == len(nums):
        return 0
    return nums[index] + array_sum(nums, index + 1)

print(factorial(5))                 # Изход: 120
print(array_sum([10, 20, 30]))      # Изход: 60
```

---

## 2. BACKTRACKING: ВСИЧКИ ПЪТИЩА В ЛАБИРИНТ (2D МАТРИЦА)

Класическа задача от SoftUni: Намери всички пътища от `(0, 0)` до клетка `'e'` (Exit), избягвайки стени `'*'`.

```python
def find_all_paths(lab: list[list[str]]) -> list[str]:
    rows, cols = len(lab), len(lab[0])
    paths = []

    def backtrack(r: int, c: int, direction: str, path: list[str]):
        # 1. Проверка за излизане извън матрицата или стена/посетена клетка
        if r < 0 or r >= rows or c < 0 or c >= cols or lab[r][c] in ("*", "v"):
            return

        path.append(direction)

        # 2. Проверка дали сме намерили изхода 'e'
        if lab[r][c] == "e":
            paths.append("".join(path))
            path.pop()  # Чистим последната посока преди връщане!
            return

        # 3. CHOOSE: Маркираме клетката като посетена ('v')
        lab[r][c] = "v"

        # 4. EXPLORE: Пробваме и 4-те посоки (Right, Down, Left, Up)
        backtrack(r, c + 1, "R", path)
        backtrack(r + 1, c, "D", path)
        backtrack(r, c - 1, "L", path)
        backtrack(r - 1, c, "U", path)

        # 5. UNCHOOSE: Отмаркираме клетката и махаме посоката (Backtrack!)
        lab[r][c] = "-"
        path.pop()

    backtrack(0, 0, "", [])
    return paths

maze = [
    ["-", "-", "-"],
    ["-", "*", "-"],
    ["-", "-", "e"]
]
print(find_all_paths(maze))  # Изход: ['RRDD', 'DDRR']
```

---

## 3. BACKTRACKING: ГЕНЕРИРАНЕ НА ВСИЧКИ КОМБИНАЦИИ ОТ БИТОВЕ (0 И 1)

```python
def generate_vectors(n: int):
    vector = [0] * n

    def backtrack(idx: int):
        if idx == n:
            print("".join(map(str, vector)))
            return
        for bit in (0, 1):
            vector[idx] = bit
            backtrack(idx + 1)

    backtrack(0)

generate_vectors(3)  # Извежда: 000, 001, 010, 011, 100, 101, 110, 111
```

---

## 4. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Операция / Модул | Синтаксис в Python | Бележка |
| :--- | :--- | :--- |
| **Проверка на лимита на стека** | `import sys; sys.getrecursionlimit()` | По подразбиране е `1000` |
| **Увеличаване на лимита** | `sys.setrecursionlimit(10_000)` | Ползва се при дълбоки графи (DFS) |
| **Копиране на списък в дъното** | `result.append(path.copy())` или `path[:]` | Задължително против презаписване |
| **Вградени комбинаторики** | `from itertools import permutations, combinations` | Написани на C — в пъти по-бързи |

---
[⬅️ 00-01-00. Теория & Капани](00-01-00-Recursion-And-Backtracking.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-02. C# Implementation](00-01-02-CSharp-Implementation.md)
