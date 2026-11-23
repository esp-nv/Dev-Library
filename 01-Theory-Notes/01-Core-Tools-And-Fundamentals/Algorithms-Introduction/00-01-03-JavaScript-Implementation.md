[⬅️ 00-01-02. C# Implementation](00-01-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-04-Java-Implementation.md](00-01-04-Java-Implementation.md)

---

# 00-01-03. Recursion & Backtracking: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на рекурсия, генериране на подмножества и Backtracking на лабиринт от нулата.

## 🧭 Съдържание на файла
* [1. Базова рекурсия (Факториел и Сума на масив)](#1-базова-рекурсия-факториел-и-сума-на-масив)
* [2. Backtracking: Всички пътища в лабиринт (2D Масив)](#2-backtracking-всички-пътища-в-лабиринт-2d-масив)
* [3. Backtracking: Генериране на битови вектори (0 и 1)](#3-backtracking-генериране-на-битови-вектори-0-и-1)
* [4. JavaScript Специфики & Cheat Sheet](#4-javascript-специфики--cheat-sheet)

---

## 1. БАЗОВА РЕКУРСИЯ (ФАКТОРИЕЛ И СУМА НА МАСИВ)

```javascript
// 1. Факториел
function factorial(n) {
    if (n <= 1) return 1;
    return n * factorial(n - 1);
}

// 2. Рекурсивна сума на масив
function arraySum(arr, index = 0) {
    if (index === arr.length) return 0;
    return arr[index] + arraySum(arr, index + 1);
}

console.log(factorial(5));            // 120
console.log(arraySum([10, 20, 30]));  // 60
```

---

## 2. BACKTRACKING: ВСИЧКИ ПЪТИЩА В ЛАБИРИНТ (2D МАСИВ)

```javascript
function findAllPaths(lab) {
    const rows = lab.length;
    const cols = lab[0].length;
    const paths = [];

    function backtrack(r, c, direction, path) {
        // 1. Проверка за граници и стени
        if (r < 0 || r >= rows || c < 0 || c >= cols || lab[r][c] === "*" || lab[r][c] === "v") {
            return;
        }

        if (direction) path.push(direction);

        // 2. Проверка за изход 'e'
        if (lab[r][c] === "e") {
            paths.push(path.join(""));
            if (direction) path.pop();
            return;
        }

        // 3. CHOOSE
        lab[r][c] = "v";

        // 4. EXPLORE
        backtrack(r, c + 1, "R", path);
        backtrack(r + 1, c, "D", path);
        backtrack(r, c - 1, "L", path);
        backtrack(r - 1, c, "U", path);

        // 5. UNCHOOSE
        lab[r][c] = "-";
        if (direction) path.pop();
    }

    backtrack(0, 0, "", []);
    return paths;
}

const maze = [
    ["-", "-", "-"],
    ["-", "*", "-"],
    ["-", "-", "e"]
];
console.log(findAllPaths(maze)); // ['RRDD', 'DDRR']
```

---

## 3. BACKTRACKING: ГЕНЕРИРАНЕ НА БИТОВИ ВЕКТОРИ (0 И 1)

```javascript
function generateVectors(n) {
    const vector = new Array(n).fill(0);

    function backtrack(idx) {
        if (idx === n) {
            console.log(vector.join(""));
            return;
        }
        for (const bit of) {
            vector[idx] = bit;
            backtrack(idx + 1);
        }
    }

    backtrack(0);
}

generateVectors(3);
```

---

## 4. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Добавяне / Махане в края** | `path.push(x)` / `path.pop()` | Работи за $O(1)$ константно време |
| **Плитко копие в дъното** | `result.push([...current])` | Spread операторът `[...]` пази моментна снимка |
| **Събиране на символи в низ** | `path.join("")` | Много по-бързо от конкатенация в цикъл |
| **Обработка на препълване** | `try { ... } catch (e)` | Хваща `RangeError: Maximum call stack size exceeded` |

---
[⬅️ 00-01-02. C# Implementation](00-01-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-01-04-Java-Implementation.md](00-01-04-Java-Implementation.md)
