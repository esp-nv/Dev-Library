[⬅️ 00-06-02. C# Implementation](00-06-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [🏠 Към Главния Хъб на Библиотеката](../README.md)| [➡️ 00-06-04-Java-Implementation.md](00-06-04-Java-Implementation.md)

---

# 00-06-03. Dynamic Programming: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на Memoization с `Map`, Coin Change DP и 0/1 Knapsack в JavaScript от нулата.

## 🧭 Съдържание на файла
* [1. Top-Down Memoization с `Map`](#1-top-down-memoization-с-map)
* [2. Coin Change (Bottom-Up Tabulation в JS)](#2-coin-change-bottom-up-tabulation-в-js)
* [3. 0/1 Knapsack (Целочислена раница в JS)](#3-01-knapsack-целочислена-раница-в-js)
* [4. JavaScript Специфики & Cheat Sheet](#4-javascript-специфики--cheat-sheet)

---

## 1. TOP-DOWN MEMOIZATION С `MAP`

```javascript
function climbStairs(n, memo = new Map()) {
    if (n <= 2) return n;
    if (memo.has(n)) return memo.get(n);

    const res = climbStairs(n - 1, memo) + climbStairs(n - 2, memo);
    memo.set(n, res);
    return res;
}

console.log(climbStairs(50)); // 20365011074
```

---

## 2. COIN CHANGE (BOTTOM-UP TABULATION В JS)

```javascript
function coinChangeDP(coins, amount) {
    const dp = new Array(amount + 1).fill(Infinity);
    dp = 0;

    for (let sum = 1; sum <= amount; sum++) {
        for (const coin of coins) {
            if (sum - coin >= 0) {
                dp[sum] = Math.min(dp[sum], dp[sum - coin] + 1);
            }
        }
    }

    return dp[amount] === Infinity ? -1 : dp[amount];
}

console.log(coinChangeDP(, 6)); // 2
```

---

## 3. 0/1 KNAPSACK (ЦЕЛОЧИСЛЕНА РАНИЦА В JS)

```javascript
function knapsack01(capacity, weights, values) {
    const dp = new Array(capacity + 1).fill(0);

    for (let i = 0; i < weights.length; i++) {
        const w = weights[i];
        const v = values[i];

        for (let cap = capacity; cap >= w; cap--) {
            dp[cap] = Math.max(dp[cap], dp[cap - w] + v);
        }
    }

    return dp[capacity];
}

console.log(knapsack01(7, , )); // 9
```

---

## 4. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Инициализация на 1D DP масив** | `new Array(n + 1).fill(Infinity)` | Чист и бърз синтаксис |
| **Инициализация на 2D DP матрица** | `Array.from({ length: R }, () => new Array(C).fill(0))` | Всеки ред е независим масив |
| **Кеш за Memoization** | `const memo = new Map();` | По-бърз от обикновен `{}` при чести записи |

---
[⬅️ 00-06-02. C# Implementation](00-06-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [🏠 Към Главния Хъб на Библиотеката](../README.md) | [➡️ 00-06-04-Java-Implementation.md](00-06-04-Java-Implementation.md)
