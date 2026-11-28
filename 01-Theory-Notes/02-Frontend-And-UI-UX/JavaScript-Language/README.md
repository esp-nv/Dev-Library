[📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [▶️ Започни от 02-00. Intro to JavaScript, V8 Engine & Architecture](02-00-Intro-To-JavaScript-And-V8-Engine.md)

---

# 🟨 JavaScript Language & Architecture (ES2025+)

Дълбочинен инженерен модул, посветен на вътрешната архитектура на **JavaScript (ECMAScript 2025+)**, **V8 Engine**, асинхронния модел на изпълнение (**Event Loop & Promises**), прототипното наследяване, **DOM & Event Delegation**, **Single-Page Applications (SPA)**, **Client-Side Rendering (`lit-html`)**, **Browser Routing (`page.js`)**, **Native Web Components** и **Automated Testing (`Playwright`)**.

[▶️ Започни от 02-00. Intro to JavaScript, V8 Engine & Architecture](02-00-Intro-To-JavaScript-And-V8-Engine.md)

---

## 📚 Съдържание на модула (Лекции & Синтаксис)

### 00. Архитектурен увод
* [02-00. Intro to JavaScript & V8 Engine Architecture](02-00-Intro-To-JavaScript-And-V8-Engine.md) — История, ECMAScript стандарти, JIT компилация (Ignition & TurboFan), Execution Context и Call Stack vs Memory Heap.

### 01. Променливи, Типове данни и Type Coercion
* [02-01-00. Variables, Types & Coercion (Теория)](02-01-00-Variables-Types-And-Coercion.md) — `var` vs `let` vs `const`, Hoisting & Temporal Dead Zone (TDZ), 7 Primitives vs Reference Types, `==` срещу `===`.
* [02-01-01. Variables, Types & Coercion — Syntax](02-01-00-Variables-Types-And-Coercion.md) — Декларации, шаблони за проверка на типове, безопасни конверсии и сравнителен Cheat Sheet с C#.

### 02. Функции, Closures, Scope и `this` Binding
* [02-02-00. Functions, Closures & `this` (Теория)](02-02-00-Functions-Closures-And-This.md) — Lexical Scope, Closure вериги, Higher-Order Functions, 4-те правила за `this` и Arrow функции.
* [02-02-01. Functions, Closures & `this` — Syntax](02-02-01-Syntax-Functions-Closures-And-This.md) — Реални Closure шаблони (Memoization, Private State), `call`/`apply`/`bind`, Destructuring и Rest/Spread.

### 03. Обекти, Прототипи (OOP) и ES6+ Класове
* [02-03-00. Prototypes, OOP & Classes (Теория)](02-03-00-Prototypes-OOP-And-Classes.md) — `[[Prototype]]` верига, Property Descriptors, Constructor функции срещу `class` синтаксис и инкапсулация.
* [02-03-01. Prototypes, OOP & Classes — Syntax](02-03-01-Syntax-Prototypes-OOP-And-Classes.md) — Модерни класове (`#private` полета, `static` блокове, `extends`/`super`, `Object.assign`, `structuredClone`).

### 04. Event Loop, Promises & Асинхронен JavaScript
* [02-04-00. Event Loop & Asynchronous JS (Теория)](02-04-00-Event-Loop-And-Asynchronous-JS.md) — Call Stack, Web APIs, Microtask Queue vs Macrotask Queue, състояния на `Promise` и `async/await` под капака.
* [02-04-01. Event Loop & Asynchronous JS — Syntax](02-04-01-Syntax-Event-Loop-And-Asynchronous-JS.md) — `Promise.all` / `allSettled` / `race` / `any`, `fetch()` API, `AbortController` и обработка на грешки.

### 05. DOM Манипулации, Събития и Browser Storage
* [02-05-00. DOM, Events & Browser Storage (Теория)](02-05-00-DOM-Events-And-Browser-Storage.md) — DOM дърво, Reflow vs Repaint, Event Bubbling & Capturing, Event Delegation, `localStorage` vs `sessionStorage` vs Cookies.
* [02-05-01. DOM, Events & Browser Storage — Syntax](02-05-01-Syntax-DOM-Events-And-Browser-Storage.md) — Селектори, `DocumentFragment`, `classList`, `dataset`, делегирани слушатели и типово-безопасен Storage wrapper.

### 06. Browser JavaScript vs Node.js Runtime
* [02-06. Browser JS vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md) — Сравнителна матрица между клиентската и сървърната среда, `window` vs `globalThis`, ES Modules vs CommonJS и Sandbox граници.

### 07. Remote Data, Authentication & Modular SPA Architecture
* [02-07-00. Remote Data, Auth & SPA Architecture (Теория)](02-07-00-Remote-Data-Auth-And-SPA-Architecture.md) — MPA vs SPA, Token-based Auth (`X-Authorization`), Ownership (`_ownerId`), ES Modules и Separation of Concerns.
* [02-07-01. API Requester, Auth & ES Modules — Syntax](02-07-01-Syntax-Remote-Data-Auth-And-SPA-Architecture.md) — Универсален `api.js` requester, `login`/`register`/`logout` поток, `FormData` обработка и ES6 `import`/`export`.

### 08. Client-Side Rendering, Browser Routing & Web Components
* [02-08-00. Client-Side Rendering, Routing & Web Components (Теория)](02-08-00-Client-Side-Rendering-Routing-And-Web-Components.md) — SSR vs CSR,Tagged Templates и `lit-html`, Hash vs Push-based Routing (`History API` / `page.js`), Custom Elements и Shadow DOM.
* [02-08-01. `lit-html`, `page.js` & Web Components — Syntax](02-08-01-Syntax-Client-Side-Rendering-Routing-And-Web-Components.md) — `lit-html` bindings и директиви (`classMap`, `styleMap`, `repeat`), `page.js` middleware, Web Component с `<slot>`.

### 09. Software Testing Architecture: Unit, Integration & E2E (Playwright)
* [02-09-00. Testing & E2E Playwright (Теория)](02-09-00-Testing-And-E2E-Playwright.md) — Тестовата пирамида, White-box vs Black-box, AAA шаблон, Mocha/Chai vs Vitest и архитектура на Playwright.
* [02-09-01. Unit Testing & Playwright E2E — Syntax](02-09-01-Syntax-Testing-And-E2E-Playwright.md) — Примери с Mocha + Chai, Vitest (`vi.fn`), Playwright Locators, Auto-waiting Assertions и `page.route` Network Interception.

---

## 🛠️ Инженерни Справочници (96 – 99)

* [02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md) — V8 Hidden Classes (Shapes), избягване на Layout Thrashing, Debouncing & Throttling, Memory Leaks и `Map`/`Set` оптимизации.
* [02-97. Category Cheat Sheet](02-97-Category-Cheat-Sheet.md) — Бърз справочник по категории: мутиращи и немутиращи Array методи (`toSorted`, `with`), `Object.groupBy`, `Map`/`Set`, String/Regex и Async комбинатори.
* [02-98. Common Errors, Edge Cases & Fixes](02-98-Common-Errors-And-Fixes.md) — Каталог на най-честите `TypeError`, `ReferenceError`, IEEE 754 капани (`0.1 + 0.2`), `NaN` особености и тихи грешки с мутации.
* [02-99. Interview Q&A, Скрити Капани & Debugging Techniques](02-99-Interview-QA-Traps-And-Debugging.md) — Топ интервю въпроси с архитектурни отговори, „Познай изхода“ загадки и техники за профилиране в DevTools.







---

[📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [▶️ Започни от 02-00. Intro to JavaScript, V8 Engine & Architecture](02-00-Intro-To-JavaScript-And-V8-Engine.md)

© 2026 Dev-Library / JavaScript Language & Architecture (ES2025+)
