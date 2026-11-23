[⬅️ 00-06-01. Python Implementation](00-06-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-03. JavaScript Implementation](00-06-03-JavaScript-Implementation.md)

---

# 00-06-02. Dynamic Programming: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на Memoization с `Dictionary<int, long>`, Coin Change DP и 0/1 Knapsack в C# от нулата.

## 🧭 Съдържание на файла
* [1. Top-Down Memoization с `Dictionary<int, long>`](#1-top-down-memoization-с-dictionaryint-long)
* [2. Coin Change (Bottom-Up Tabulation в C#)](#2-coin-change-bottom-up-tabulation-в-c)
* [3. 0/1 Knapsack (Целочислена раница в C#)](#3-01-knapsack-целочислена-раница-в-c)
* [4. C# Специфики & Cheat Sheet](#4-c-специфики--cheat-sheet)

---

## 1. TOP-DOWN MEMOIZATION С `DICTIONARY<INT, LONG>`

```csharp
using System.Collections.Generic;

public class MemoizationDemo
{
    private static readonly Dictionary<int, long> Memo = new();

    public static long ClimbStairs(int n)
    {
        if (n <= 2) return n;
        if (Memo.TryGetValue(n, out long cached)) return cached;

        long result = ClimbStairs(n - 1) + ClimbStairs(n - 2);
        Memo[n] = result;
        return result;
    }
}
```

---

## 2. COIN CHANGE (BOTTOM-UP TABULATION В C#)

```csharp
using System;

public class CoinChangeDp
{
    public static int MinCoins(int[] coins, int amount)
    {
        // Внимание: В C# не ползвай int.MaxValue, защото int.MaxValue + 1 прелива в отрицателно число!
        int maxVal = amount + 1;
        int[] dp = new int[amount + 1];
        Array.Fill(dp, maxVal);
        dp = 0;

        for (int sum = 1; sum <= amount; sum++)
        {
            foreach (int coin in coins)
            {
                if (sum - coin >= 0)
                {
                    dp[sum] = Math.Min(dp[sum], dp[sum - coin] + 1);
                }
            }
        }

        return dp[amount] > amount ? -1 : dp[amount];
    }
}
```

---

## 3. 0/1 KNAPSACK (ЦЕЛОЧИСЛЕНА РАНИЦА В C#)

```csharp
using System;

public class Knapsack01
{
    public static int Solve(int capacity, int[] weights, int[] values)
    {
        int[] dp = new int[capacity + 1];

        for (int i = 0; i < weights.Length; i++)
        {
            int w = weights[i];
            int v = values[i];

            for (int cap = capacity; cap >= w; cap--)
            {
                dp[cap] = Math.Max(dp[cap], dp[cap - w] + v);
            }
        }

        return dp[capacity];
    }
}
```

---

## 4. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Запълване на DP масив** | `Array.Fill(dp, amount + 1);` | Избягва препълването на `int.MaxValue + 1` |
| **Проверка в кеша** | `if (memo.TryGetValue(key, out var val))` | Едно търсене в хеш таблицата ($O(1)$) |
| **2D таблица** | `int[,] dp = new int[rows, cols];` | По-компактна в паметта от jagged масив `int[][]` |

---
[⬅️ 00-06-01. Python Implementation](00-06-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-03. JavaScript Implementation](00-06-03-JavaScript-Implementation.md)
