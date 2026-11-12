[⬅️ 00-05-02. C# Implementation](00-05-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-04-Java-Implementation.md](00-05-04-Java-Implementation.md)

---

# 00-05-03. Graphs and Traversal: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на BFS с опашка без бавното `.shift()`, рекурсивен DFS и Топологично сортиране в JavaScript от нулата.

## 🧭 Съдържание на файла
* [1. BFS (Оптимизирана опашка с указател `head` вместо `.shift()`)](#1-bfs-оптимизирана-опашка-с-указател-head-вместо-shift)
* [2. DFS (Рекурсивно обхождане в дълбочина)](#2-dfs-рекурсивно-обхождане-в-дълбочина)
* [3. Топологично сортиране (Алгоритъм на Кан в JS)](#3-топологично-сортиране-алгоритъм-на-кан-в-js)
* [4. JavaScript Специфики & Cheat Sheet](#4-javascript-специфики--cheat-sheet)

---

## 1. BFS (ОПТИМИЗИРАНА ОПАШКА С УКАЗАТЕЛ `HEAD` ВМЕСТО `.SHIFT()`)

*В JavaScript вграденият масив няма истински `Dequeue` — методът `arr.shift()` премества всички останали елементи в паметта ($O(n)$). Затова в алгоритмични задачи използваме обикновен масив и индекс `head++` ($O(1)$)!*

```javascript
function bfs(graph, start) {
    const visited = new Set([start]);
    const queue = [start];
    let head = 0; // Указател към началото на опашката -> O(1) четене!
    const order = [];

    while (head < queue.length) {
        const current = queue[head++];
        order.push(current);

        for (const neighbor of graph.get(current) || []) {
            if (!visited.has(neighbor)) {
                visited.add(neighbor);
                queue.push(neighbor);
            }
        }
    }

    return order;
}

const graph = new Map([
    [1, [2, 3]],
    [2, [4, 5]],
    [3, [6]],
    [4, []], [5, []], [6, []]
]);
console.log(bfs(graph, 1)); // [1, 2, 3, 4, 5, 6]
```

---

## 2. DFS (РЕКУРСИВНО ОБХОЖДАНЕ В ДЪЛБОЧИНА)

```javascript
function dfs(graph, node, visited = new Set(), result = []) {
    visited.add(node);
    result.push(node);

    for (const neighbor of graph.get(node) || []) {
        if (!visited.has(neighbor)) {
            dfs(graph, neighbor, visited, result);
        }
    }

    return result;
}

console.log(dfs(graph, 1)); // [1, 2, 4, 5, 3, 6]
```

---

## 3. ТОПОЛОГИЧНО СОРТИРАНЕ (АЛГОРИТЪМ НА КАН В JS)

```javascript
function topologicalSort(numNodes, edges) {
    const graph = Array.from({ length: numNodes }, () => []);
    const inDegree = new Array(numNodes).fill(0);

    for (const [src, dst] of edges) {
        graph[src].push(dst);
        inDegree[dst]++;
    }

    const queue = [];
    for (let i = 0; i < numNodes; i++) {
        if (inDegree[i] === 0) queue.push(i);
    }

    let head = 0;
    const order = [];

    while (head < queue.length) {
        const curr = queue[head++];
        order.push(curr);

        for (const next of graph[curr]) {
            inDegree[next]--;
            if (inDegree[next] === 0) queue.push(next);
        }
    }

    if (order.length !== numNodes) {
        throw new Error("Графът съдържа цикъл!");
    }

    return order;
}

console.log(topologicalSort(4, [[0, 1], [0, 2], [1, 3], [2, 3]])); // [0, 1, 2, 3]
```

---

## 4. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Представяне на граф** | `const graph = new Map();` | Запазва точния тип на ключовете (числа или низове) |
| **Посетени върхове** | `const visited = new Set();` | `.has()` и `.add()` работят за $O(1)$ |
| **Бърза BFS опашка** | `const q = [start]; let head = 0; q[head++]` | Избягва бавното $O(n)$ на `q.shift()` |
| **DFS Стек** | `stack.push(x)` / `stack.pop()` | Бързи $O(1)$ операции в края на масива |

---
[⬅️ 00-05-02. C# Implementation](00-05-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-04-Java-Implementation.md](00-05-04-Java-Implementation.md)
