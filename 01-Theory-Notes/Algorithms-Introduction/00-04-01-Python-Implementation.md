[⬅️ 00-04-00. Теория & Капани](00-04-00-Greedy-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-02. C# Implementation](00-04-02-CSharp-Implementation.md)

---

# 00-04-01. Greedy Algorithms: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на Coin Change (канонични монети), Activity Selection и Fractional Knapsack (Дробна раница) от нулата.

## 🧭 Съдържание на файла
* [1. Coin Change (Връщане на ресто с минимален брой монети)](#1-coin-change-връщане-на-ресто-с-минимален-брой-монети)
* [2. Activity Selection (Максимален брой несиметрични събития)](#2-activity-selection-максимален-брой-несиметрични-събития)
* [3. Fractional Knapsack (Дробна раница — Сортиране по стойност/тегло)](#3-fractional-knapsack-дробна-раница--сортиране-по-стойносттегло)
* [4. Python Специфики & Cheat Sheet](#4-python-специфики--cheat-sheet)

---

## 1. COIN CHANGE (ВРЪЩАНЕ НА РЕСТО С МИНИМАЛЕН БРОЙ МОНЕТИ)

```python
def greedy_coin_change(coins: list[int], target_sum: int) -> dict[int, int]:
    # Сортираме монетите от най-голямата към най-малката
    sorted_coins = sorted(coins, reverse=True)
    result = {}

    for coin in sorted_coins:
        if target_sum == 0:
            break
        count = target_sum // coin  # Колко цели монети от този номинал се побират
        if count > 0:
            result[coin] = count
            target_sum -= count * coin

    if target_sum > 0:
        raise ValueError("Сумата не може да бъде съставена точно с наличните монети!")

    return result

print(greedy_coin_change([1, 2, 5, 10, 20, 50], 93))
# Изход: {50: 1, 20: 2, 2: 1, 1: 1}
```

---

## 2. ACTIVITY SELECTION (МАКСИМАЛЕН БРОЙ НЕСИМЕТРИЧНИ СЪБИТИЯ)

```python
def activity_selection(intervals: list[tuple[int, int]]) -> list[tuple[int, int]]:
    # Сортираме по краен час (втория елемент на кортежа)
    intervals.sort(key=lambda x: x[1])
    chosen = []
    current_end = float("-inf")

    for start, end in intervals:
        if start >= current_end:
            chosen.append((start, end))
            current_end = end

    return chosen

meetings = [(1, 4), (3, 5), (0, 6), (5, 7), (3, 9), (5, 9), (6, 10), (8, 11)]
print(activity_selection(meetings))  # Изход: [(1, 4), (5, 7), (8, 11)]
```

---

## 3. FRACTIONAL KNAPSACK (ДРОБНА РАНИЦА — СОРТИРАНЕ ПО СТОЙНОСТ/ТЕГЛО)

При **Fractional Knapsack** можем да отрежем част от предмет (например златен пясък или кафе), затова алчно взимаме предметите с **най-висока цена на килограм**:

```python
def fractional_knapsack(capacity: float, items: list[tuple[float, float]]) -> float:
    # items съдържа кортежи (стойност, тегло)
    # Сортираме по отношението стойност / тегло низходящо
    items.sort(key=lambda x: x / x[1], reverse=True)

    total_value = 0.0
    for value, weight in items:
        if capacity == 0:
            break
        if weight <= capacity:
            # Взимаме целия предмет
            total_value += value
            capacity -= weight
        else:
            # Взимаме само оставащата част (дроб), която се побира в раницата
            fraction = capacity / weight
            total_value += value * fraction
            capacity = 0

    return total_value

# Раница с капацитет 50 кг | Предмети: (стойност, тегло)
loot = [(60, 10), (100, 20), (120, 30)]
print(f"Максимална стойност: {fractional_knapsack(50, loot):.2f}")  # Изход: 240.00
```

---

## 4. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Задача / Операция | Ключов критерий за сортиране | Сложност |
| :--- | :--- | :--- |
| **Coin Change (Каноничен)** | `sorted(coins, reverse=True)` | $O(n \log n)$ |
| **Activity Selection** | `intervals.sort(key=lambda x: x[1])` (краен час) | $O(n \log n)$ |
| **Fractional Knapsack** | `items.sort(key=lambda x: x / x[1], reverse=True)` | $O(n \log n)$ |
| **Dynamic Greedy (Непрекъснат)** | Използвай `import heapq` (Min-Heap / Max-Heap) | $O(n \log n)$ |

---
[⬅️ 00-04-00. Теория & Капани](00-04-00-Greedy-Algorithms.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-02. C# Implementation](00-04-02-CSharp-Implementation.md)
