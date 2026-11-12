[⬅️ 00-05-03. JavaScript Implementation](00-05-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-00. Dynamic Programming](00-06-00-Dynamic-Programming.md)

---

# 00-05-04. Graphs and Traversal: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на BFS с `ArrayDeque`, рекурсивен DFS с `HashSet` и Топологично сортиране (Алгоритъм на Кан) в Java от нулата.

## 🧭 Съдържание на файла
* [1. BFS (Обхождане в ширина с `ArrayDeque`)](#1-bfs-обхождане-в-ширина-с-arraydeque)
* [2. DFS (Рекурсивно обхождане в дълбочина)](#2-dfs-рекурсивно-обхождане-в-дълбочина)
* [3. Топологично сортиране (Алгоритъм на Кан в Java)](#3-топологично-сортиране-алгоритъм-на-кан-в-java)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. BFS (ОБХОЖДАНЕ В ШИРИНА С `ARRAYDEQUE`)

*В Java предпочитаме `ArrayDeque` пред `LinkedList` за опашки и стекове, защото е по-бърз в кеша на процесора и заема по-малко RAM памет.*

```java
import java.util.*;

public class GraphTraversal {
    public static List<Integer> bfs(Map<Integer, List<Integer>> graph, int start) {
        Set<Integer> visited = new HashSet<>();
        Queue<Integer> queue = new ArrayDeque<>();
        List<Integer> order = new ArrayList<>();

        visited.add(start);
        queue.offer(start);

        while (!queue.isEmpty()) {
            int current = queue.poll();
            order.add(current);

            for (int neighbor : graph.getOrDefault(current, Collections.emptyList())) {
                // visited.add() връща true, ако елементът все още не е в множеството
                if (visited.add(neighbor)) {
                    queue.offer(neighbor);
                }
            }
        }

        return order;
    }
}
```

---

## 2. DFS (РЕКУРСИВНО ОБХОЖДАНЕ В ДЪЛБОЧИНА)

```java
import java.util.*;

public class DfsTraversal {
    public static void dfs(Map<Integer, List<Integer>> graph, int node, Set<Integer> visited, List<Integer> result) {
        visited.add(node);
        result.add(node);

        for (int neighbor : graph.getOrDefault(node, Collections.emptyList())) {
            if (!visited.contains(neighbor)) {
                dfs(graph, neighbor, visited, result);
            }
        }
    }
}
```

---

## 3. ТОПОЛОГИЧНО СОРТИРАНЕ (АЛГОРИТЪМ НА КАН В JAVA)

```java
import java.util.*;

public class TopologicalSorter {
    public static List<Integer> sort(int numNodes, int[][] edges) {
        List<List<Integer>> graph = new ArrayList<>();
        for (int i = 0; i < numNodes; i++) graph.add(new ArrayList<>());

        int[] inDegree = new int[numNodes];
        for (int[] edge : edges) {
            int src = edge, dst = edge;
            graph.get(src).add(dst);
            inDegree[dst]++;
        }

        Queue<Integer> queue = new ArrayDeque<>();
        for (int i = 0; i < numNodes; i++) {
            if (inDegree[i] == 0) queue.offer(i);
        }

        List<Integer> order = new ArrayList<>();
        while (!queue.isEmpty()) {
            int curr = queue.poll();
            order.add(curr);

            for (int next : graph.get(curr)) {
                inDegree[next]--;
                if (inDegree[next] == 0) queue.offer(next);
            }
        }

        if (order.size() != numNodes) {
            throw new IllegalStateException("Открит е цикъл в графа!");
        }

        return order;
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Най-бързата опашка (FIFO)** | `Queue<Integer> q = new ArrayDeque<>();` | `q.offer(x)` и `q.poll()` работят за O(1) |
| **Най-бързият стек (LIFO)** | `Deque<Integer> s = new ArrayDeque<>();` | `s.push(x)` и `s.pop()` (избягвай стария `Stack`) |
| **Безопасно четене на съседи** | `graph.getOrDefault(u, List.of())` | Избягва `NullPointerException` при върхове без изход |
| **Проверка + добавяне в Set** | `if (visited.add(neighbor))` | Идентично с C# `HashSet.Add()` |

---
[⬅️ 00-05-03. JavaScript Implementation](00-05-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-06-00. Dynamic Programming](00-06-00-Dynamic-Programming.md)
