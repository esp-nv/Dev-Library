[⬅️ 00-03-02. C# Implementation](00-03-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-04-Java-Implementation.md](00-03-04-Java-Implementation.md)

---

# 00-03-03. Sorting Algorithms: JavaScript (ES6+) Имплементация & Cheat Sheet

**Език:** JavaScript (ES6+ / Node.js) | **Цел:** Чисти имплементации на Bubble Sort, Merge Sort и Quick Sort в JavaScript, плюс вградените `.sort()` и `.toSorted()` от нулата.

## 🧭 Съдържание на файла
* [1. Bubble Sort с Array Destructuring размяна](#1-bubble-sort-с-array-destructuring-размяна)
* [2. Merge Sort (Функционален ES6 стил)](#2-merge-sort-функционален-es6-стил)
* [3. Quick Sort (In-Place в JavaScript)](#3-quick-sort-in-place-в-javascript)
* [4. Вграденото `.sort()` и модерното `.toSorted()` (ES2023)](#4-вграденото-sort-и-модерното-tosorted-es2023)
* [5. JavaScript Специфики & Cheat Sheet](#5-javascript-специфики--cheat-sheet)

---

## 1. BUBBLE SORT С ARRAY DESTRUCTURING РАЗМЯНА

```javascript
function bubbleSort(arr) {
    const n = arr.length;
    for (let i = 0; i < n; i++) {
        let swapped = false;
        for (let j = 0; j < n - i - 1; j++) {
            if (arr[j] > arr[j + 1]) {
                // ES6 Destructuring размяна без временна променлива
                [arr[j], arr[j + 1]] = [arr[j + 1], arr[j]];
                swapped = true;
            }
        }
        if (!swapped) break;
    }
    return arr;
}
```

---

## 2. MERGE SORT (ФУНКЦИОНАЛЕН ES6 СТИЛ)

```javascript
function mergeSort(arr) {
    if (arr.length <= 1) return arr;

    const mid = Math.floor(arr.length / 2);
    const left = mergeSort(arr.slice(0, mid));
    const right = mergeSort(arr.slice(mid));

    return merge(left, right);
}

function merge(left, right) {
    const result = [];
    let i = 0, j = 0;

    while (i < left.length && j < right.length) {
        if (left[i] <= right[j]) {
            result.push(left[i++]);
        } else {
            result.push(right[j++]);
        }
    }

    return [...result, ...left.slice(i), ...right.slice(j)];
}

console.log(mergeSort([38, 27, 43, 10])); // [10, 27, 38, 43]
```

---

## 3. QUICK SORT (IN-PLACE В JAVASCRIPT)

```javascript
function quickSort(arr, low = 0, high = arr.length - 1) {
    if (low < high) {
        const pi = partition(arr, low, high);
        quickSort(arr, low, pi - 1);
        quickSort(arr, pi + 1, high);
    }
    return arr;
}

function partition(arr, low, high) {
    const pivot = arr[high];
    let i = low - 1;

    for (let j = low; j < high; j++) {
        if (arr[j] <= pivot) {
            i++;
            [arr[i], arr[j]] = [arr[j], arr[i]];
        }
    }

    [arr[i + 1], arr[high]] = [arr[high], arr[i + 1]];
    return i + 1;
}
```

---

## 4. ВГРАДЕНОТО `.SORT()` И МОДЕРНОТО `.TOSORTED()` (ES2023)

```javascript
const nums =;

// 1. ВНИМАНИЕ: .sort() променя оригиналния масив на място (Mutates)!
nums.sort((a, b) => a - b); // [2, 5, 10, 100]

// 2. Модерен ES2023 стандарт: .toSorted() връща НОВ масив, без да пипа оригинала!
const original =;
const copySorted = original.toSorted((a, b) => b - a); // [8, 3, 1]

// 3. Сортиране на стрингове на български или английски език с localeCompare:
const names = ["Яна", "Анна", "Борис"];
names.sort((a, b) => a.localeCompare(b, "bg"));
```

---

## 5. JAVASCRIPT СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в JavaScript | Бележка |
| :--- | :--- | :--- |
| **Размяна на 2 елемента** | `[arr[i], arr[j]] = [arr[j], arr[i]]` | ES6 Array Destructuring |
| **Сортиране на числа (Възходящо)** | `arr.sort((a, b) => a - b)` | Без компаратор сортира като текст! |
| **Сортиране без мутация (ES2023)** | `const copy = arr.toSorted((a, b) => a - b)` | Еквивалент на Python `sorted()` |
| **Сортиране на текст (Азбучно)** | `arr.sort((a, b) => a.localeCompare(b))` | Отчита правилно кирилица и главни букви |

---
[⬅️ 00-03-02. C# Implementation](00-03-02-CSharp-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-03-04-Java-Implementation.md](00-03-04-Java-Implementation.md)
