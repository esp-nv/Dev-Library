[⬅️ 00-01-01. Python Implementation](00-01-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-03. JavaScript Implementation](00-01-03-JavaScript-Implementation.md)

---

# 00-01-02. Recursion & Backtracking: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на рекурсия, комбинации и Backtracking на лабиринт със строга типизация и оптимална памет от нулата.

## 🧭 Съдържание на файла
* [1. Базова рекурсия (Факториел и Сума на масив)](#1-базова-рекурсия-факториел-и-сума-на-масив)
* [2. Backtracking: Всички пътища в лабиринт (2D Матрица)](#2-backtracking-всички-пътища-в-лабиринт-2d-матрица)
* [3. Backtracking: Генериране на битови вектори (0 и 1)](#3-backtracking-генериране-на-битови-вектори-0-и-1)
* [4. C# Специфики & Cheat Sheet](#4-c-специфики--cheat-sheet)

---

## 1. БАЗОВА РЕКУРСИЯ (ФАКТОРИЕЛ И СУМА НА МАСИВ)

```csharp
using System;

public class RecursionBasics
{
    // 1. Факториел (ползваме long, защото 20! надхвърля int)
    public static long Factorial(int n)
    {
        if (n <= 1) return 1;
        return n * Factorial(n - 1);
    }

    // 2. Рекурсивна сума на масив
    public static int ArraySum(int[] arr, int index = 0)
    {
        if (index == arr.Length) return 0;
        return arr[index] + ArraySum(arr, index + 1);
    }
}
```

---

## 2. BACKTRACKING: ВСИЧКИ ПЪТИЩА В ЛАБИРИНТ (2D МАТРИЦА)

```csharp
using System;
using System.Collections.Generic;

public class LabyrinthSolver
{
    private static readonly List<char> Path = new();
    private static readonly List<string> AllPaths = new();

    public static List<string> FindPaths(char[,] lab)
    {
        AllPaths.Clear();
        Backtrack(lab, 0, 0, '\0');
        return AllPaths;
    }

    private static void Backtrack(char[,] lab, int r, int c, char direction)
    {
        int rows = lab.GetLength(0);
        int cols = lab.GetLength(1);

        // 1. Проверка за граници, стена '*' или посетена клетка 'v'
        if (r < 0 || r >= rows || c < 0 || c >= cols || lab[r, c] == '*' || lab[r, c] == 'v')
            return;

        if (direction != '\0') Path.Add(direction);

        // 2. Проверка за изход 'e'
        if (lab[r, c] == 'e')
        {
            AllPaths.Add(new string(Path.ToArray()));
            if (Path.Count > 0) Path.RemoveAt(Path.Count - 1);
            return;
        }

        // 3. CHOOSE
        lab[r, c] = 'v';

        // 4. EXPLORE
        Backtrack(lab, r, c + 1, 'R');
        Backtrack(lab, r + 1, c, 'D');
        Backtrack(lab, r, c - 1, 'L');
        Backtrack(lab, r - 1, c, 'U');

        // 5. UNCHOOSE
        lab[r, c] = '-';
        if (Path.Count > 0) Path.RemoveAt(Path.Count - 1);
    }
}
```

---

## 3. BACKTRACKING: ГЕНЕРИРАНЕ НА БИТОВИ ВЕКТОРИ (0 И 1)

```csharp
using System;

public class BitVectors
{
    public static void Generate(int index, int[] vector)
    {
        if (index == vector.Length)
        {
            Console.WriteLine(string.Join("", vector));
            return;
        }

        for (int bit = 0; bit <= 1; bit++)
        {
            vector[index] = bit;
            Generate(index + 1, vector);
        }
    }
}
```

---

## 4. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Премахване на последния елемент** | `path.RemoveAt(path.Count - 1)` | Еквивалент на Python `path.pop()` |
| **Копиране на `List<T>` в дъното** | `result.Add(new List<int>(current))` | Създава нов списък в Heap паметта |
| **Събиране на `List<char>` в `string`** | `new string(path.ToArray())` | Най-бързият начин без `StringBuilder` |
| **Защита при дълбока рекурсия** | Пренаписване с `Stack<T>` | В C# `StackOverflowException` не се лови с `try/catch`! |

---
[⬅️ 00-01-01. Python Implementation](00-01-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-03. JavaScript Implementation](00-01-03-JavaScript-Implementation.md)
