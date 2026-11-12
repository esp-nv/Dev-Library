[⬅️ 00-05-00. Теория & Капани](00-05-00-Graphs-And-Traversal.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-02. C# Implementation](00-05-02-CSharp-Implementation.md)

---

# 00-05-01. Graphs and Traversal: Python Имплементация & Cheat Sheet

**Език:** Python 3.x | **Цел:** Чисти имплементации на BFS с `collections.deque`, рекурсивен и итеративен DFS, откриване на цикъл в насочен граф и Топологично сортиране (Алгоритъм на Кан) от нулата.

## 🧭 Съдържание на файла
* [1. BFS (Обхождане в ширина с `collections.deque`)](#1-bfs-обхождане-в-ширина-с-collectionsdeque)
* [2. DFS (Рекурсивно и Итеративно обхождане в дълбочина)](#2-dfs-рекурсивно-и-итеративно-обхождане-в-дълбочина)
* [3. Топологично сортиране (Алгоритъм на Кан — Kahn's Algorithm)](#3-топологично-сортиране-алгоритъм-на-кан--kahns-algorithm)
* [4. Python Специфики & Cheat Sheet](#4-python-специфики--cheat-sheet)

---

## 1. BFS (ОБХОЖДАНЕ В ШИРИНА С `COLLECTIONS.DEQUE`)

*Никога не използвай обикновен `list.pop(0)` за опашка в Python, защото е O(n)! Винаги ползвай `deque.popleft()`, което е O(1).*

```python
from collections import deque

def bfs(graph: dict[int, list[int]], start: int) -> list[int]:
    visited = {start}
    queue = deque([start])
    order = []

    while queue:
        vertex = queue.popleft()
        order.append(vertex)

        for neighbor in graph.get(vertex, []):
            if neighbor not in visited:
                visited.add(neighbor)
                queue.append(neighbor)

    return order

sample_graph = {
    1:,
    2:,
    3:,
    4: [], 5: [], 6: []
}
print(bfs(sample_graph, 1))  # Изход: [1, 2, 3, 4, 5, 6]
```

---

## 2. DFS (РЕКУРСИВНО И ИТЕРАТИВНО ОБХОЖДАНЕ В ДЪЛБОЧИНА)

```python
# 1. Рекурсивен DFS (Най-четим)
def dfs_recursive(graph: dict[int, list[int]], node: int, visited: set[int] | None = None) -> list[int]:
    if visited is None:
        visited = set()

    visited.add(node)
    result = [node]

    for neighbor in graph.get(node, []):
        if neighbor not in visited:
            result.extend(dfs_recursive(graph, neighbor, visited))

    return result

# 2. Итеративен DFS със Стек (Не гърми с RecursionError при 100 000 върха!)
def dfs_iterative(graph: dict[int, list[int]], start: int) -> list[int]:
    visited = set()
    stack = [start]
    order = []

    while stack:
        node = stack.pop()
        if node not in visited:
            visited.add(node)
            order.append(node)
            # Обръщаме съседите с reversed(), за да ги обходим в същия ред като рекурсията
            for neighbor in reversed(graph.get(node, [])):
                if neighbor not in visited:
                    stack.append(neighbor)

    return order
```

---

## 3. ТОПОЛОГИЧНО СОРТИРАНЕ (АЛГОРИТЪМ НА КАН — KAHN'S ALGORITHM)

Използва входящите степени (`in_degree` — колко стрелки сочат към всеки връх). Ако накрая резултатът съдържа по-малко върхове от общия брой, значи **в графа има цикъл**!

```python
from collections import deque

def topological_sort(num_nodes: int, edges: list[tuple[int, int]]) -> list[int]:
    graph = {i: [] for i in range(num_nodes)}
    in_degree = {i: 0 for i in range(num_nodes)}

    for src, dst in edges:
        graph[src].append(dst)
        in_degree[dst] += 1

    # Вкарваме в опашката всички задачи без предварителни зависимости (in_degree == 0)
    queue = deque([node for node, deg in in_degree.items() if deg == 0])
    topo_order = []

    while queue:
        curr = queue.popleft()
        topo_order.append(curr)

        for neighbor in graph[curr]:
            in_degree[neighbor] -= 1
            if in_degree[neighbor] == 0:
                queue.append(neighbor)

    if len(topo_order) != num_nodes:
        raise ValueError("Графът съдържа цикъл! Топологичното сортиране е невъзможно.")

    return topo_order

# Пример: 0 -> 1, 0 -> 2, 1 -> 3, 2 -> 3
print(topological_sort(4, [(0, 1), (0, 2), (1, 3), (2, 3)]))  # Изход: [0, 1, 2, 3]
```

---

## 4. PYTHON СПЕЦИФИКИ & CHEAT SHEET

| Операция / Структура | Синтаксис в Python | Сложност |
| :--- | :--- | :--- |
| **Изграждане на граф от ребра** | `from collections import defaultdict; g = defaultdict(list)` | $O(V + E)$ |
| **BFS Опашка (FIFO)** | `q = deque([start]); q.popleft()` | $O(1)$ |
| **DFS Стек (LIFO)** | `stack = [start]; stack.pop()` | $O(1)$ |
| **Проверка за посетен връх** | `if node not in visited: visited.add(node)` | $O(1)$ |

---
[⬅️ 00-05-00. Теория & Капани](00-05-00-Graphs-And-Traversal.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-02. C# Implementation](00-05-02-CSharp-Implementation.md)
