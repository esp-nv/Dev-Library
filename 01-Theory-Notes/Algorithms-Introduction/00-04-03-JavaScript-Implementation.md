[⬅️ 00-04-02. C# Implementation](00-04-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-00. Graphs and Traversal](00-05-00-Graphs-And-Traversal.md)

---

# 00-04-03. Greedy Algorithms: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на Coin Change, Activity Selection и Fractional Knapsack в JavaScript от нулата.

## 🧭 Съдържание на файла
* [1. Coin Change (Връщане на ресто)](#1-coin-change-връщане-на-ресто)
* [2. Activity Selection (Избор на интервали)](#2-activity-selection-избор-на-интервали)
* [3. Fractional Knapsack (Дробна раница)](#3-fractional-knapsack-дробна-раница)
* [4. JavaScript Специфики & Cheat Sheet](#4-javascript-специфики--cheat-sheet)

---

## 1. COIN CHANGE (ВРЪЩАНЕ НА РЕСТО)

```javascript
function greedyCoinChange(coins, targetSum) {
    // Задължителен числов компаратор (b - a) за сортиране от голямо към малко!
    const sortedCoins = [...coins].sort((a, b) => b - a);
    const result = new Map();

    for (const coin of sortedCoins) {
        if (targetSum === 0) break;
        const count = Math.floor(targetSum / coin);
        if (count > 0) {
            result.set(coin, count);
            targetSum -= count * coin;
        }
    }

    if (targetSum > 0) {
        throw new Error("Не може да се състави точната сума.");
    }

    return result;
}

console.log(greedyCoinChange([1, 2, 5, 10, 20, 50], 93));
```

---

## 2. ACTIVITY SELECTION (ИЗБОР НА ИНТЕРВАЛИ)

```javascript
function activitySelection(intervals) {
    // Сортираме по краен час (b[1])
    const sorted = [...intervals].sort((a, b) => a[1] - b[1]);
    const selected = [];
    let lastEnd = -Infinity;

    for (const [start, end] of sorted) {
        if (start >= lastEnd) {
            selected.push([start, end]);
            lastEnd = end;
        }
    }

    return selected;
}

const meetings = [[1, 4], [3, 5], [0, 6], [5, 7], [8, 9]];
console.log(activitySelection(meetings)); // [[1, 4], [5, 7], [8, 9]]
```

---

## 3. FRACTIONAL KNAPSACK (ДРОБНА РАНИЦА)

```javascript
function fractionalKnapsack(capacity, items) {
    // items: [{ value: 60, weight: 10 }, ...]
    const sorted = [...items].sort((a, b) => (b.value / b.weight) - (a.value / a.weight));
    let totalValue = 0;

    for (const item of sorted) {
        if (capacity === 0) break;

        if (item.weight <= capacity) {
            totalValue += item.value;
            capacity -= item.weight;
        } else {
            totalValue += item.value * (capacity / item.weight);
            capacity = 0;
        }
    }

    return totalValue;
}

const loot = [
    { value: 60, weight: 10 },
    { value: 100, weight: 20 },
    { value: 120, weight: 30 }
];
console.log(fractionalKnapsack(50, loot)); // 240
```

---

## 4. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Сортиране на обекти по ключ** | `[...arr].sort((a, b) => a.end - b.end)` | Копираме с `[...]`, за да не мутираме входа |
| **Целочислено делене за монети** | `Math.floor(sum / coin)` | В JS няма вграден `//` оператор |
| **Начална граница за минимум** | `let lastEnd = -Infinity;` | Гарантира, че първият интервал винаги влиза |

---
[⬅️ 00-04-02. C# Implementation](00-04-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-00. Graphs and Traversal](00-05-00-Graphs-And-Traversal.md)
