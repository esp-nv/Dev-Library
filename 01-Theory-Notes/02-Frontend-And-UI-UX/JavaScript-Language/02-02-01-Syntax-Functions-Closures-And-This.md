[⬅️ 02-02-00. Functions, Closures & `this`](02-02-00-Functions-Closures-And-This.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-03-00. Prototypes, OOP & Classes](02-03-00-Prototypes-OOP-And-Classes.md)

---

# 🟨 02-02-01. Functions, Closures & Array Methods — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Видове функции, Default Parameters и Rest (`...args`)](#1-видове-функции-default-parameters-и-rest-args)
* [2. Практически Closures (Private State & Memoization)](#2-практически-closures-private-state--memoization)
* [3. Експлицитно управление на `this`: `call`, `apply` и `bind`](#3-експлицитно-управление-на-this-call-apply-и-bind)
* [4. Higher-Order Array Methods (Функционални трансформации)](#4-higher-order-array-methods-функционални-трансформации)
* [5. Pure Functions, Currying и Function Composition](#5-pure-functions-currying-и-function-composition)
* [6. Синтактичен Cheat Sheet (JS Array Methods срещу C# LINQ)](#6-синтактичен-cheat-sheet-js-array-methods-срещу-c-linq)

---

## 1. Видове функции, Default Parameters и Rest (`...args`)

```javascript
// 1. Function Declaration (Hoisted)
function add(a, b = 0) {
  return a + b;
}

// 2. Arrow Function с неявен return (implicit return)
const multiply = (a, b) => a * b;

// Връщане на обектен литерал от Arrow функция изисква кръгли скоби ({ ... })
const createPoint = (x, y) => ({ x, y });

// 3. Rest Parameters (...args) — събира останалите аргументи в истински масив
const sumAll = (multiplier, ...numbers) =>
  numbers.reduce((acc, n) => acc + n, 0) * multiplier;

sumAll(2, 10, 20, 30); // 120
```

---

## 2. Практически Closures (Private State & Memoization)

```javascript
// 1. Инкапсулация на състояние чрез Closure (Factory Pattern)
function createCounter(initialValue = 0) {
  let count = initialValue; // Недостъпно директно отвън!

  return {
    increment: () => ++count,
    decrement: () => --count,
    getValue: () => count
  };
}

const counter = createCounter(10);
counter.increment(); // 11
counter.getValue();  // 11

// 2. Memoization (Кеширане на резултати от чиста функция)
function memoize(fn) {
  const cache = new Map();
  return (arg) => {
    if (cache.has(arg)) return cache.get(arg);
    const result = fn(arg);
    cache.set(arg, result);
    return result;
  };
}
```

---

## 3. Експлицитно управление на `this`: `call`, `apply` и `bind`

```javascript
const engine = { name: "RPN Core v1" };

function formatResult(prefix, unit) {
  return `[${this.name}] ${prefix}: 42 ${unit}`;
}

// 1. call — извиква функцията веднага с подаден this и аргументи поотделно
formatResult.call(engine, "Площ", "м²");

// 2. apply — извиква функцията веднага с подаден this и масив от аргументи
formatResult.apply(engine, ["Площ", "м²"]);

// 3. bind — НЕ извиква функцията, а връща НОВА функция с трайно заключен this
const boundFormat = formatResult.bind(engine, "Обем");
boundFormat("м³"); // "[RPN Core v1] Обем: 42 м³"
```

---

## 4. Higher-Order Array Methods (Функционални трансформации)

```javascript
const formulas = [
  { id: 1, name: "Кръг", category: "Геометрия", complexity: 2 },
  { id: 2, name: "ДДС", category: "Финанси", complexity: 1 },
  { id: 3, name: "Цилиндър", category: "Геометрия", complexity: 3 }
];

// 1. filter -> map -> reduce верига (не мутира оригиналния масив)
const geometryTotalComplexity = formulas
  .filter(f => f.category === "Геометрия")
  .map(f => f.complexity)
  .reduce((sum, val) => sum + val, 0); // 5

// 2. find & findIndex (връща първия съвпадащ елемент или undefined / -1)
const vatFormula = formulas.find(f => f.name === "ДДС");

// 3. some & every (логически проверки)
const hasComplex = formulas.some(f => f.complexity >= 3); // true
const allValid = formulas.every(f => f.id > 0);          // true

// 4. Немутиращи методи (ES2023+): toSorted, toReversed, with
const sortedByComplexity = formulas.toSorted((a, b) => a.complexity - b.complexity);

// 5. Групиране по ключ с Object.groupBy (ES2024+)
const grouped = Object.groupBy(formulas, f => f.category);
```

---

## 5. Pure Functions, Currying и Function Composition

```javascript
// Pure Function (Чиста функция): при еднакъв вход винаги връща еднакъв изход и няма side effects
const calculateTax = (amount, rate) => amount * rate;

// Currying: трансформиране на f(a, b) в f(a)(b)
const withTaxRate = rate => amount => amount * (1 + rate);
const addBulgarianVat = withTaxRate(0.20);
addBulgarianVat(100); // 120

// Function Composition (pipe отляво надясно)
const pipe = (...fns) => initialVal => fns.reduce((val, fn) => fn(val), initialVal);
const trim = str => str.trim();
const toUpper = str => str.toUpperCase();

const normalizeFuncName = pipe(trim, toUpper);
normalizeFuncName("  площ.кръг  "); // "ПЛОЩ.КРЪГ"
```

---

## 6. Синтактичен Cheat Sheet (JS Array Methods срещу C# LINQ)

| Операция | JavaScript Array Method | C# LINQ Еквивалент |
| :--- | :--- | :--- |
| **Трансформация (Projection)** | `arr.map(x => x * 2)` | `list.Select(x => x * 2)` |
| **Филтриране** | `arr.filter(x => x > 5)` | `list.Where(x => x > 5)` |
| **Намиране на първи елемент** | `arr.find(x => x.id === 1)` | `list.FirstOrDefault(x => x.Id == 1)` |
| **Агрегация (Натрупване)** | `arr.reduce((acc, x) => acc + x, 0)` | `list.Aggregate(0, (acc, x) => acc + x)` |
| **Поне един съвпада** | `arr.some(x => x.active)` | `list.Any(x => x.Active)` |
| **Всички съвпадат** | `arr.every(x => x.valid)` | `list.All(x => x.Valid)` |
| **Сортиране без мутация** | `arr.toSorted((a, b) => a - b)` | `list.OrderBy(x => x)` |
| **Разгъване на вложени масиви** | `arr.flatMap(x => x.items)` | `list.SelectMany(x => x.Items)` |

---

[⬅️ 02-02-00. Functions, Closures & `this`](02-02-00-Functions-Closures-And-This.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-03-00. Prototypes, OOP & Classes](02-03-00-Prototypes-OOP-And-Classes.md)

© 2026 Dev-Library / 02-02-01. Functions, Closures & Array Methods — Syntax & Cheat Sheet
