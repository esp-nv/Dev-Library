[⬅️ 00-04-01. Python Implementation](00-04-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-03. JavaScript Implementation](00-04-03-JavaScript-Implementation.md)

---

# 00-04-02. Greedy Algorithms: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на Coin Change, Activity Selection и Fractional Knapsack с `record` модели и LINQ от нулата.

## 🧭 Съдържание на файла
* [1. Coin Change (Връщане на ресто)](#1-coin-change-връщане-на-ресто)
* [2. Activity Selection (Избор на интервали)](#2-activity-selection-избор-на-интервали)
* [3. Fractional Knapsack (Дробна раница с C# `record`)](#3-fractional-knapsack-дробна-раница-с-c-record)
* [4. C# Специфики & Cheat Sheet](#4-c-специфики--cheat-sheet)

---

## 1. COIN CHANGE (ВРЪЩАНЕ НА РЕСТО)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class GreedyCoinChange
{
    public static Dictionary<int, int> GetChange(int[] coins, int targetSum)
    {
        var sortedCoins = coins.OrderByDescending(c => c);
        var result = new Dictionary<int, int>();

        foreach (int coin in sortedCoins)
        {
            if (targetSum == 0) break;
            int count = targetSum / coin;
            if (count > 0)
            {
                result[coin] = count;
                targetSum -= count * coin;
            }
        }

        if (targetSum > 0)
            throw new InvalidOperationException("Не може да се върне точна сума.");

        return result;
    }
}
```

---

## 2. ACTIVITY SELECTION (ИЗБОР НА ИНТЕРВАЛИ)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public record Meeting(int Start, int End);

public class Scheduler
{
    public static List<Meeting> SelectMaxMeetings(List<Meeting> meetings)
    {
        var sorted = meetings.OrderBy(m => m.End).ToList();
        var selected = new List<Meeting>();
        int lastEnd = int.MinValue;

        foreach (var meeting in sorted)
        {
            if (meeting.Start >= lastEnd)
            {
                selected.Add(meeting);
                lastEnd = meeting.End;
            }
        }

        return selected;
    }
}
```

---

## 3. FRACTIONAL KNAPSACK (ДРОБНА РАНИЦА С C# `RECORD`)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public record Item(double Value, double Weight)
{
    public double Ratio => Value / Weight;
}

public class KnapsackSolver
{
    public static double GetMaxValue(double capacity, List<Item> items)
    {
        var sortedItems = items.OrderByDescending(i => i.Ratio);
        double totalValue = 0;

        foreach (var item in sortedItems)
        {
            if (capacity <= 0) break;

            if (item.Weight <= capacity)
            {
                totalValue += item.Value;
                capacity -= item.Weight;
            }
            else
            {
                totalValue += item.Value * (capacity / item.Weight);
                capacity = 0;
            }
        }

        return totalValue;
    }
}
```

---

## 4. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Сортиране низходящо** | `items.OrderByDescending(x => x.Ratio)` | LINQ създава сортирана поредица ($O(n \log n)$) |
| **Моделиране на елемент** | `public record Item(double Val, double W);` | Кратък и чист код без излишен boilerplate |
| **Динамична алчна опашка** | `var pq = new PriorityQueue<T, int>();` | Вграден Min-Heap в .NET 6+ (за Dijkstra / Huffman) |

---
[⬅️ 00-04-01. Python Implementation](00-04-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-04-03. JavaScript Implementation](00-04-03-JavaScript-Implementation.md)
