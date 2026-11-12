[⬅️ 00-02-02. C# Implementation](00-02-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-04-Java-Implementation.md](00-02-04-Java-Implementation.md)

---

# 00-02-03. Searching Algorithms: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на Linear Search, Binary Search (с `Math.floor` и побитово изместване `>>> 1`) и Lower/Upper Bound от нулата.

## 🧭 Съдържание на файла
* [1. Linear Search (Линейно търсене)](#1-linear-search-линейно-търсене)
* [2. Binary Search (`Math.floor` срещу `>>> 1`)](#2-binary-search-mathfloor-срещу--1)
* [3. Намиране на първо и последно срещане (Lower / Upper Bound)](#3-намиране-на-първо-и-последно-срещане-lower--upper-bound)
* [4. JavaScript Специфики & Cheat Sheet](#4-javascript-специфики--cheat-sheet)

---

## 1. LINEAR SEARCH (ЛИНЕЙНО ТЪРСЕНЕ)

```javascript
function linearSearch(arr, target) {
    for (let i = 0; i < arr.length; i++) {
        if (arr[i] === target) {
            return i;
        }
    }
    return -1;
}

console.log(linearSearch([15, 42, 7, 99], 7)); // Изход: 2
```

---

## 2. BINARY SEARCH (`MATH.FLOOR` СРЕЩУ `>>> 1`)

В JavaScript **няма целочислен тип `int`** — всички числа са 64-битови дробни (`Number`). Затова `(left + right) / 2` връща дробно число (напр. `2.5`) и `arr[2.5]` дава `undefined`! Задължително закръгляваме надолу:

```javascript
function binarySearch(arr, target) {
    let left = 0;
    let right = arr.length - 1;

    while (left <= right) {
        // Начин 1 (Четим): Math.floor(left + (right - left) / 2)
        // Начин 2 (Бърз побитов трик): (left + right) >>> 1
        const mid = Math.floor(left + (right - left) / 2);

        if (arr[mid] === target) {
            return mid;
        } else if (arr[mid] < target) {
            left = mid + 1;
        } else {
            right = mid - 1;
        }
    }

    return -1;
}

const sorted =;
console.log(binarySearch(sorted, 23)); // Изход: 5
```

---

## 3. НАМИРАНЕ НА ПЪРВО И ПОСЛЕДНО СРЕЩАНЕ (LOWER / UPPER BOUND)

```javascript
function findFirstOccurrence(arr, target) {
    let left = 0;
    let right = arr.length - 1;
    let result = -1;

    while (left <= right) {
        const mid = Math.floor(left + (right - left) / 2);

        if (arr[mid] === target) {
            result = mid;
            right = mid - 1; // Свиваме търсенето наляво за първото срещане
        } else if (arr[mid] < target) {
            left = mid + 1;
        } else {
            right = mid - 1;
        }
    }

    return result;
}

console.log(findFirstOccurrence([1, 4, 4, 4, 9], 4)); // Изход: 1
```

---

## 4. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Линейно търсене (индекс)** | `arr.indexOf(target)` | Връща `-1` ако липсва ($O(n)$) |
| **Линейно търсене (по условие)** | `arr.findIndex(x => x.id === 5)` | За масиви от обекти ($O(n)$) |
| **Закръгляне на средата (`mid`)** | `Math.floor(left + (right - left) / 2)` | Без `Math.floor` индексът става дробен! |
| **Побитово деление на 2** | `(left + right) >>> 1` | Работи за индекси до `2³¹ - 1` |
| **Вграден Binary Search** | *Няма* | Имплементира се ръчно за $O(\log n)$ |

---
[⬅️ 00-02-02. C# Implementation](00-02-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-04-Java-Implementation.md](00-02-04-Java-Implementation.md)
