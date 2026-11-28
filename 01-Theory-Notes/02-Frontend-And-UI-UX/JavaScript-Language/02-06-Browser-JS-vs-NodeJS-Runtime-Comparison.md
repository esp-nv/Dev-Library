[⬅️ 02-05-01. DOM, Events & Browser Storage — Syntax](02-05-01-Syntax-DOM-Events-And-Browser-Storage.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-96. Performance & Optimizations](../TypeScript-Language/02-96-Performance-And-Optimizations.md)

---

# 🟨 02-06. Browser JavaScript vs Node.js Runtime Comparison

## 📑 Съдържание
* [1. Общото ядро (V8) срещу различните среди за изпълнение](#1-общото-ядро-v8-срещу-различните-среди-за-изпълнение)
* [2. Архитектурна схема на двата Runtime-а](#2-архитектурна-схема-на-двата-runtime-а)
* [3. Пълна сравнителна матрица (Browser JS vs Node.js)](#3-пълна-сравнителна-матрица-browser-js-vs-nodejs)
* [4. Модулни системи: ES Modules (`import/export`) vs CommonJS (`require`)](#4-модулни-системи-es-modules-importexport-vs-commonjs-require)
* [5. Сигурност и граници на достъп (Sandbox vs OS Access)](#5-сигурност-и-граници-на-достъп-sandbox-vs-os-access)

---

## 1. Общото ядро (V8) срещу различните среди за изпълнение

Един от най-важните концептуални пробиви за всеки Full-Stack инженер е да разграничи **самия език JavaScript (ECMAScript)** от **средата за изпълнение (Host Environment / Runtime)**:

* **Езикът JavaScript** (променливи, функции, класове, `Promise`, `Map`, `Set`, `Array.prototype.map`, `JSON`) е **100% идентичен** както в браузъра, така и в Node.js, защото и двата използват **Google V8 енджина**.
* Разликата идва от **допълнителните C++ API-та**, които средата закача около V8:
  * **Браузърът** добавя **Web APIs** за работа с потребителския интерфейс (`DOM`, `window`, `document`, `localStorage`).
  * **Node.js** добавя **Libuv** и системни C++ модули за работа със сървърната операционна система (`fs` за файлове, `http` за сървъри, `process`, `Buffer`, `crypto`).

---

## 2. Архитектурна схема на двата Runtime-а

```text
BROWSER RUNTIME                              NODE.JS RUNTIME
┌──────────────────────────────────┐         ┌──────────────────────────────────┐
│         JavaScript Code          │         │         JavaScript Code          │
├──────────────────────────────────┤         ├──────────────────────────────────┤
│      V8 Engine (Heap + Stack)    │         │      V8 Engine (Heap + Stack)    │
├──────────────────────────────────┤         ├──────────────────────────────────┤
│           Web APIs               │         │       Node.js C++ Bindings       │
│  • DOM / CSSOM / History         │         │  • fs (File System) / path       │
│  • localStorage / Cookies        │         │  • http / net / streams          │
│  • fetch() / WebSockets          │         │  • process / Buffer / crypto     │
├──────────────────────────────────┤         ├──────────────────────────────────┤
│      Browser Event Loop          │         │   Libuv (Event Loop + C++ Pool)  │
└──────────────────────────────────┘         └──────────────────────────────────┘
```

---

## 3. Пълна сравнителна матрица (Browser JS vs Node.js)

| Критерий | Browser JavaScript | Node.js (Backend Runtime) |
| :--- | :--- | :--- |
| **Глобален обект** | `window` (и стандартизираното `globalThis`) | `global` (и стандартизираното `globalThis`) |
| **DOM & UI манипулация** | ✅ Пълен достъп (`document`, `HTMLElement`) | ❌ Няма DOM (сървърна среда без екран) |
| **Файлова система (Disk I/O)** | ❌ Забранен (само през `<input type="file">`) | ✅ Пълен достъп (`node:fs/promises`) |
| **Мрежови сървъри (Sockets)** | ❌ Само клиентски заявки (`fetch`, `WebSocket`) | ✅ Може да слуша на портове (`http.createServer`) |
| **Двоични данни (Binary)** | `ArrayBuffer`, `Uint8Array`, `Blob` | `Buffer` (наследник на `Uint8Array`) + `ArrayBuffer` |
| **Модулна система** | Само **ES Modules** (`import` / `export`) | **ES Modules** (`import`) + **CommonJS** (`require`) |
| **Конфигурация / Secrets** | Публичен код (никога не пазете API ключове!) | `process.env` / `.env` файлове на сървъра |
| **CORS политика** | 🔒 Строго налагана от браузъра | 🔓 Не съществува (сървър-към-сървър комуникация) |

---

## 4. Модулни системи: ES Modules (`import/export`) vs CommonJS (`require`)

* **ES Modules (ESM — Стандартът на езика):**
  * Синтаксис: `import { calc } from "./math.js";` и `export function calc() {}`.
  * Зареждат се **асинхронно и статично** (позволява *Tree-Shaking* — премахване на неизползвания код при сборка).
  * Работят нативно във всички модерни браузъри и в Node.js (чрез `"type": "module"` в `package.json` или разширение `.mjs`).
* **CommonJS (CJS — Историческият стандарт на Node.js):**
  * Синтаксис: `const { calc } = require("./math");` и `module.exports = { calc };`.
  * Зареждат се **синхронно и динамично** по време на изпълнение. Не работят директно в браузъра без Bundler (като Vite или Webpack).

---

## 5. Сигурност и граници на достъп (Sandbox vs OS Access)

* **Browser Sandbox (Пясъчник):** Браузърът изпълнява непознат код, свален от интернет, затова го изолира в строг Sandbox — JavaScript няма достъп до операционната система, регистрите или локалните папки на потребителя, а **Same-Origin Policy (SOP)** и **CORS** предпазват от кражба на сесии между различни сайтове.
* **Node.js (Пълни права на процеса):** Кодът в Node.js се стартира от администратора на сървъра и наследява правата на потребителя в операционната система — може да чете/изтрива файлове, да стартира системни процеси (`child_process`) и да се свързва директно с бази данни.

> **🔗 Връзка със следващия домейн:** За дълбоко гмуркане в сървърния JavaScript, архитектурата на Libuv, Express и NestJS, виж модул **[📦 Node.js, Express & NestJS](../../03-Backend-Ecosystems/Node-Express-And-NestJS/README.md)** в Домейн 03.

---

[⬅️ 02-05-01. DOM, Events & Browser Storage — Syntax](02-05-01-Syntax-DOM-Events-And-Browser-Storage.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-96. Performance & Optimizations](../TypeScript-Language/02-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 02-06. Browser JavaScript vs Node.js Runtime Comparison
