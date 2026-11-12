[⬅️ 00-06-00. Теория & Капани](00-06-00-Dynamic-Programming.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-02. C# Implementation](00-06-02-CSharp-Implementation.md)

---

# 00-06-01. Dynamic Programming: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на Memoization (`@cache`), Coin Change (общ случай с DP) и 0/1 Knapsack от нулата.

## 🧭 Съдържание на файла
* [1. Top-Down Memoization с вградения `@functools.cache`](#1-top-down-memoization-с-вградения-functoolscache)
* [2. Coin Change (Минимален брой монети за произволни номинали)](#2-coin-change-минимален-брой-монети-за-произволни-номинали)
* [3. 0/1 Knapsack (Целочислена раница с 1D DP таблица)](#3-01-knapsack-целочислена-раница-с-1d-dp-таблица)
* [4. Python Специфики & Cheat Sheet](#4-python-специфики--cheat-sheet)

---

## 1. TOP-DOWN MEMOIZATION С ВГРАДЕНИЯ `@FUNCTOOLS.CACHE`

Python има вграден декоратор `@cache` (от версия 3.9+), който автоматично кешира резултата на функцията за всеки входен аргумент:

```python
from functools import cache

@cache
def climb_stairs(n: int) -> int:
    # Колко начина има да изкачиш n стъпала, ако стъпваш по 1 или по 2 стъпала?
    if n <= 2:
        return n
    return climb_stairs(n - 1) + climb_stairs(n - 2)

print(climb_stairs(50))  # Минава мигновено: 20365011074
```

---

## 2. COIN CHANGE (МИНИМАЛЕН БРОЙ МОНЕТИ ЗА ПРОИЗВОЛНИ НОМИНАЛИ)

Решава случая `[1, 3, 4]` за сума `6`, където Greedy грешеше!

```python
def coin_change_dp(coins: list[int], amount: int) -> int:
    # Инициализираме масив с безкрайност (float('inf')) за невъзможните суми
    dp = [float("inf")] * (amount + 1)
    dp = 0  # За сума 0 трябват 0 монети

    for current_sum in range(1, amount + 1):
        for coin in coins:
            if current_sum - coin >= 0:
                dp[current_sum] = min(dp[current_sum], dp[current_sum - coin] + 1)

    return dp[amount] if dp[amount] != float("inf") else -1

print(coin_change_dp(, 6))  # Изход: 2 (оптималното 3 + 3!)
```

---

## 3. 0/1 KNAPSACK (ЦЕЛОЧИСЛЕНА РАНИЦА С 1D DP ТАБЛИЦА)

```python
def knapsack_01(capacity: int, weights: list[int], values: list[int]) -> int:
    dp = * (capacity + 1)

    for i in range(len(weights)):
        w = weights[i]
        v = values[i]
        # Обхождаме НАЗАД от capacity до w, за да не ползваме един предмет 2 пъти!
        for cap in range(capacity, w - 1, -1):
            dp[cap] = max(dp[cap], dp[cap - w] + v)

    return dp[capacity]

print(knapsack_01(7, , ))  # Изход: 9
```

---

## 4. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Инструмент / Техника | Синтаксис в Python | Бележка |
| :--- | :--- | :--- |
| **Автоматична мемоизация** | `from functools import cache; @cache` | Работи само за hashable аргументи (`int`, `str`, `tuple`) |
| **Кеш с ограничен размер** | `@lru_cache(maxsize=1024)` | Пази последните 1024 извиквания |
| **Стойност за безкрайност** | `float("inf")` или `math.inf` | Удобна при задачи за минимум (`min()`) |
| **Чист 2D масив (Без капана `*`)** | `dp = [[0] * cols for _ in range(rows)]` | Никога `[[0] * cols] * rows` (споделя референции!) |

---
[⬅️ 00-06-00. Теория & Капани](00-06-00-Dynamic-Programming.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-02. C# Implementation](00-06-02-CSharp-Implementation.md)
