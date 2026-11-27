[📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript-Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [▶️ Започни от 02-00 Intro](02-00-Intro-To-JavaScript-And-V8-Engine.md)

# 🟨 JavaScript Language & Browser Runtime

Този модул покрива фундаменталната и модерната архитектура на **JavaScript (ES2025+)** в браузърна среда — от вътрешната работа на **V8 енджина** и **Event Loop**, през прототипното наследяване и асинхронното програмиране, до манипулация на DOM и изграждане на тествани интерактивни приложения.

---

## 📑 Съдържание на хъба
* [1. Архитектурна карта на знанията](#1-архитектурна-карта-на-знанията)
* [2. Основни уроци (Теория и Практика)](#2-основни-уроци-теория-и-практика)
* [3. Сравнителен анализ: Browser JS vs Node.js](#3-сравнителен-анализ-browser-js-vs-nodejs)
* [4. Практически проект: Калкулатор](#4-практически-проект-калкулатор)
* [5. Справочници, Оптимизация и Интервю подготовка](#5-справочници-оптимизация-и-интервю-подготовка)
* [6. Връзка със следващия модул (TypeScript)](#6-връзка-със-следващия-модул-typescript)
* [7. Карта на Подмодулите](#7-карта-на-подмодулите)

---

## 1. Архитектурна карта на знанията

```text
JavaScript Runtime (Browser)
 ├── V8 Engine (Call Stack + Memory Heap + JIT Compiler)
 ├── Web APIs (DOM, Fetch, Timers, LocalStorage)
 ├── Event Loop (Callback Queue / Macrotasks + Microtask Queue)
 └── Language Core (Closures, Prototypes, Promises, ES Modules)
```

---

## 2. Основни уроци (Теория и Практика)

| Тема | Теория (`-00`) | Практика (`-01`) | Ключови концепции |
| :--- | :--- | :--- | :--- |
| **00. Въведение и V8** | [02-00 Intro & V8 Engine](02-00-Intro-To-JavaScript-And-V8-Engine.md) | — | JIT компилация, Call Stack, Memory Heap, ECMAScript |
| **01. Типове и Принуда** | [02-01-00 Variables & Coercion](02-01-00-Variables-Types-And-Coercion.md) | [02-01-01 Syntax](02-01-01-Syntax-From-Scratch.md) | `let`/`const`, Stack vs Heap, `===`, `?.`, `??` |
| **02. Функции и Closures** | [02-02-00 Functions & This](02-02-00-Functions-Closures-And-This.md) | [02-02-01 Syntax](02-02-01-Syntax-From-Scratch.md) | Lexical Scope, Closures, `this`, HOF |
| **03. Прототипи и ООП** | [02-03-00 Prototypes & OOP](02-03-00-Prototypes-OOP-And-Classes.md) | [02-03-01 Syntax](02-03-01-Syntax-From-Scratch.md) | Prototype Chain, `class`, `#private`, Inheritance |
| **04. Асинхронен JS** | [02-04-00 Event Loop & Async](02-04-00-Event-Loop-And-Asynchronous-JS.md) | [02-04-01 Syntax](02-04-01-Syntax-From-Scratch.md) | Event Loop, Microtasks, Promises, `async/await` |
| **05. DOM и Събития** | [02-05-00 DOM & Storage](02-05-00-DOM-Events-And-Browser-Storage.md) | [02-05-01 Syntax](02-05-01-Syntax-From-Scratch.md) | DOM Tree, Event Delegation, Web Storage API |

---

## 3. Сравнителен анализ: Browser JS vs Node.js
* [02-06. Browser JavaScript vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md) — Архитектурни разлики, глобални обекти, модулни системи (ESM vs CJS) и сигурност.

---

## 4. Практически проект: Калкулатор
* [02-95. Project Map: Interactive Calculator](02-95-Project-Map-Interactive-Calculator.md) — Пълна карта за изграждане на калкулатор с разделена бизнес логика, клавиатурна поддръжка и Unit тестове.

---

## 5. Справочници, Оптимизация и Интервю подготовка
* [02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md)
* [02-97. Category Cheat Sheet](02-97-Category-Cheat-Sheet.md)
* [02-98. Common Errors & Fixes](02-98-Common-Errors-And-Fixes.md)
* [02-99. Interview Q&A, Traps & Debugging](02-99-Interview-QA-Traps-And-Debugging.md)

---

## 6. Връзка със следващия модул (TypeScript)
След овладяването на динамичната природа на JavaScript, следващата логична стъпка е [📦 TypeScript Language](../TypeScript-Language/README.md), който добавя статична типова система върху същия runtime.

---

## 7. Карта на Подмодулите
* [02-00. Intro to JavaScript & V8 Engine](02-00-Intro-To-JavaScript-And-V8-Engine.md)
* [02-01-00. Variables, Types & Coercion](02-01-00-Variables-Types-And-Coercion.md)
* [02-01-01. Variables & Operators — Syntax From Scratch](02-01-01-Syntax-From-Scratch.md)
* [02-02-00. Functions, Closures & `this`](02-02-00-Functions-Closures-And-This.md)
* [02-02-01. Functions & Array Methods — Syntax From Scratch](02-02-01-Syntax-From-Scratch.md)
* [02-03-00. Prototypes, OOP & Classes](02-03-00-Prototypes-OOP-And-Classes.md)
* [02-03-01. Classes & Prototypes — Syntax From Scratch](02-03-01-Syntax-From-Scratch.md)
* [02-04-00. Event Loop & Asynchronous JavaScript](02-04-00-Event-Loop-And-Asynchronous-JS.md)
* [02-04-01. Promises, Async/Await & Fetch — Syntax From Scratch](02-04-01-Syntax-From-Scratch.md)
* [02-05-00. DOM, Events & Browser Storage](02-05-00-DOM-Events-And-Browser-Storage.md)
* [02-05-01. DOM Manipulation & Event Delegation — Syntax From Scratch](02-05-01-Syntax-From-Scratch.md)
* [02-06. Browser JS vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md)
* [02-95. Project Map: Interactive Calculator](02-95-Project-Map-Interactive-Calculator.md)
* [02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md)
* [02-97. Category Cheat Sheet](02-97-Category-Cheat-Sheet.md)
* [02-98. Common Errors & Fixes](02-98-Common-Errors-And-Fixes.md)
* [02-99. Interview Q&A, Traps & Debugging](02-99-Interview-QA-Traps-And-Debugging.md)

---

© 2026 Dev-Library / JavaScript Language & Architecture
