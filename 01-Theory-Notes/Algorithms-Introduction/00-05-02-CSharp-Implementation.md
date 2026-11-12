[⬅️ 00-05-01. Python Implementation](00-05-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-03. JavaScript Implementation](00-05-03-JavaScript-Implementation.md)

---

# 00-05-02. Graphs and Traversal: C# (.NET) Имплементация & Cheat Sheet

**Език:** C# 12+ (.NET) | **Цел:** Чисти имплементации на BFS с `Queue<int>`, рекурсивен DFS с `HashSet<int>` и Топологично сортиране в C# от нулата.

## 🧭 Съдържание на файла
* [1. BFS (Обхождане в ширина с `Queue<int>`)](#1-bfs-обхождане-в-ширина-с-queueint)
* [2. DFS (Рекурсивно обхождане в дълбочина)](#2-dfs-рекурсивно-обхождане-в-дълбочина)
* [3. Топологично сортиране (Алгоритъм на Кан в C#)](#3-топологично-сортиране-алгоритъм-на-кан-в-c)
* [4. C# Специфики & Cheat Sheet](#4-c-специфики--cheat-sheet)

---

## 1. BFS (ОБХОЖДАНЕ В ШИРИНА С `QUEUE<INT>`)

```csharp
using System.Collections.Generic;

public class GraphTraversal
{
    public static List<int> Bfs(Dictionary<int, List<int>> graph, int start)
    {
        var visited = new HashSet<int> { start };
        var queue = new Queue<int>();
        queue.Enqueue(start);

        var result = new List<int>();

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            result.Add(current);

            if (!graph.ContainsKey(current)) continue;

            foreach (int neighbor in graph[current])
            {
                // visited.Add() връща true, ако елементът е нов (не е бил вътре)
                if (visited.Add(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }
}
```

---

## 2. DFS (РЕКУРСИВНО ОБХОЖДАНЕ В ДЪЛБОЧИНА)

```csharp
using System.Collections.Generic;

public class DfsTraversal
{
    public static void Dfs(Dictionary<int, List<int>> graph, int node, HashSet<int> visited, List<int> result)
    {
        visited.Add(node);
        result.Add(node);

        if (!graph.ContainsKey(node)) return;

        foreach (int neighbor in graph[node])
        {
            if (!visited.Contains(neighbor))
            {
                Dfs(graph, neighbor, visited, result);
            }
        }
    }
}
```

---

## 3. ТОПОЛОГИЧНО СОРТИРАНЕ (АЛГОРИТЪМ НА КАН В C#)

```csharp
using System;
using System.Collections.Generic;

public class TopologicalSorter
{
    public static List<int> Sort(int numNodes, List<(int From, int To)> edges)
    {
        var graph = new List<int>[numNodes];
        for (int i = 0; i < numNodes; i++) graph[i] = new List<int>();

        int[] inDegree = new int[numNodes];
        foreach (var (from, to) in edges)
        {
            graph[from].Add(to);
            inDegree[to]++;
        }

        var queue = new Queue<int>();
        for (int i = 0; i < numNodes; i++)
        {
            if (inDegree[i] == 0) queue.Enqueue(i);
        }

        var order = new List<int>();
        while (queue.Count > 0)
        {
            int curr = queue.Dequeue();
            order.Add(curr);

            foreach (int next in graph[curr])
            {
                inDegree[next]--;
                if (inDegree[next] == 0) queue.Enqueue(next);
            }
        }

        if (order.Count != numNodes)
            throw new InvalidOperationException("Открит е цикъл в графа!");

        return order;
    }
}
```

---

## 4. C# СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в C# | Бележка |
| :--- | :--- | :--- |
| **Проверка и добавяне на 1 ред** | `if (visited.Add(neighbor))` | Връща `false`, ако вече е посетен ($O(1)$) |
| **BFS Опашка** | `queue.Enqueue(x)` / `queue.Dequeue()` | Вградена кръгова опашка ($O(1)$) |
| **DFS Стек** | `stack.Push(x)` / `stack.Pop()` | За итеративен DFS без риск от `StackOverflowException` |
| **Претеглен граф (Dijkstra)** | `var pq = new PriorityQueue<int, int>();` | Сортира автоматично по най-малко разстояние |

---
[⬅️ 00-05-01. Python Implementation](00-05-01-Python-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-03. JavaScript Implementation](00-05-03-JavaScript-Implementation.md)
