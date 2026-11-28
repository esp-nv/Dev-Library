[📦 Към предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-01-00. Variables, Data Types & Type Coercion](02-01-00-Variables-Types-And-Coercion.md)

---

# 🟨 02-00. Intro to JavaScript, V8 Engine & Architecture

## 📑 Съдържание
* [1. Какво е JavaScript и как се отнася към ECMAScript?](#1-какво-е-javascript-и-как-се-отнася-към-ecmascript)
* [2. Архитектура на JavaScript Runtime (V8 + Web APIs)](#2-архитектура-на-javascript-runtime-v8--web-apis)
* [3. Вътрешно устройство на V8 Engine (JIT компилация)](#3-вътрешно-устройство-на-v8-engine-jit-компилация)
* [4. Call Stack и Memory Heap](#4-call-stack-и-memory-heap)
* [5. Сравнение с C# (.NET CLR) за по-бързо усвояване](#5-сравнение-с-c-net-clr-за-по-бързо-усвояване)

---

## 1. Какво е JavaScript и как се отнася към ECMAScript?

**JavaScript** е динамично типизиран, високонивов, мултипарадигмен език за програмиране (поддържащ процедурно, обектно-ориентирано чрез прототипи и функционално програмиране).

* **ECMAScript (ES):** Официалната спецификация на езика (поддържана от комитета **TC39** на Ecma International), която дефинира синтаксиса, типовете данни и вградените обекти (напр. `ES6 / ES2015`, `ES2025`, `ES2026`).
* **JavaScript:** Реалната имплементация на ECMAScript стандарта, комбинирана със средата на изпълнение (**Runtime** — браузър или Node.js).

> **🧠 Мнемоника:** **ECMAScript** е *чертежът (рецептата)*, а **JavaScript** е *готовата сграда* с всички удобства (DOM, Fetch, LocalStorage).

---

## 2. Архитектура на JavaScript Runtime (V8 + Web APIs)

Самият JavaScript енджин (като **V8** в Chrome/Edge/Node.js, **SpiderMonkey** във Firefox или **JavaScriptCore** в Safari) разбира **само чист ECMAScript** (променливи, функции, обекти, масиви, Promises). Той **няма** вградени функции като `document.querySelector()`, `setTimeout()` или `fetch()`.

Тези възможности се предоставят от **Browser Runtime** средата чрез **Web APIs**:

```text
┌─────────────────────────────────────────────────────────────────┐
│                    BROWSER RUNTIME ENVIRONMENT                  │
│                                                                 │
│  ┌─────────────────────────────┐   ┌─────────────────────────┐  │
│  │         V8 ENGINE           │   │        WEB APIs         │  │
│  │                             │   │                         │  │
│  │  ┌───────────┐ ┌──────────┐ │   │ • DOM (document)        │  │
│  │  │Call Stack │ │Memory    │ │◄─►│ • Fetch / Network       │  │
│  │  │(1 нишка)  │ │Heap      │ │   │ • Timers (setTimeout)   │  │
│  │  └───────────┘ └──────────┘ │   │ • Web Storage           │  │
│  └──────────────▲──────────────┘   └────────────┬────────────┘  │
│                 │                               │               │
│                 │       ┌───────────────┐       │               │
│                 └───────┤  EVENT LOOP   │◄──────┘               │
│                         └───────────────┘                       │
│                    (Microtasks & Macrotasks)                    │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Вътрешно устройство на V8 Engine (JIT компилация)

Съвременният JavaScript не е просто интерпретиран ред по ред — **V8** използва **Just-In-Time (JIT) компилация**, която комбинира бързо стартиране с агресивна машинна оптимизация:

1. **Parser:** Превръща изходния код в **AST (Abstract Syntax Tree)**.
2. **Ignition (Interpreter):** Генерира и изпълнява лек **Bytecode** веднага (за да няма забавяне при зареждане на страницата).
3. **Profiler (Monitor):** Следи кои функции се изпълняват многократно (*"hot functions"*) и с какви типове данни.
4. **TurboFan / Maglev (Optimizing Compilers):** Компилират "горещия" код директно до силно оптимизиран **машинен код (Machine Code)** на база предположенията за типовете. Ако типът се промени неочаквано, настъпва **Deoptimization** (връщане към Ignition).

---

## 4. Call Stack и Memory Heap

* **Call Stack (Стек на извикванията):**
  * Структура от данни тип **LIFO** (*Last-In, First-Out*).
  * JavaScript е **еднонишков (Single-Threaded)** — има точно **един Call Stack**, което означава, че в даден момент може да изпълнява само една операция.
  * При извикване на функция се създава **Execution Context (Stack Frame)** и се поставя на върха на стека; при `return` се премахва.
  * Ако рекурсия няма дъно, стекът се препълва и хвърля `RangeError: Maximum call stack size exceeded`.

* **Memory Heap (Динамична памет):**
  * Неструктурирана област в паметта, където се заделят сложни обекти, масиви и функции.
  * Освобождаването на неизползваната памет се извършва автоматично от **Garbage Collector** (алгоритъм *Mark-and-Sweep*).

---

## 5. Сравнение с C# (.NET CLR) за по-бързо усвояване

| Характеристика | JavaScript (V8 Engine) | C# (.NET CLR) |
| :--- | :--- | :--- |
| **Спецификация** | ECMAScript (TC39) | ECMA-334 / C# Language Spec |
| **Компилация** | JIT (Ignition Bytecode ➔ TurboFan Machine Code) | Ahead-of-Time към IL (CIL) ➔ RyuJIT към Machine Code |
| **Типизация** | Динамична и слаба (типовете се проверяват в Runtime) | Статична и строга (проверяват се при Compile Time) |
| **Нишки (Threading)** | Single-Threaded (Event Loop + Web Workers) | Multi-Threaded (ThreadPool, `Task`, `async/await`) |
| **Памет** | Stack (примитиви/референции) + Heap (Mark-and-Sweep GC) | Stack (Value Types) + Managed Heap (Generational GC) |







---

[📦 Към предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-01-00. Variables, Data Types & Type Coercion](02-01-00-Variables-Types-And-Coercion.md)

© 2026 Dev-Library / 02-00. Intro to JavaScript, V8 Engine & Architecture
