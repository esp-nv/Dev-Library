[⬅️ 02-03-01. Classes, Inheritance & Prototypes — Syntax...](02-03-01-Syntax-Prototypes-OOP-And-Classes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-04-01. Promises, Async/Await & Fetch API — Syntax...](02-04-01-Syntax-Event-Loop-And-Asynchronous-JS.md)

---

# 🟨 02-04-00. Event Loop, Microtasks, Promises & `async/await`

## 📑 Съдържание
* [1. Защо JavaScript е еднонишков (Single-Threaded), но неблокиращ?](#1-защо-javascript-е-еднонишков-single-threaded-но-неблокиращ)
* [2. Архитектура на Event Loop: Microtask Queue vs Macrotask Queue](#2-архитектура-на-event-loop-microtask-queue-vs-macrotask-queue)
* [3. Точният алгоритъм на един цикъл (Tick) на Event Loop](#3-точният-алгоритъм-на-един-цикъл-tick-на-event-loop)
* [4. Promises: 3-те състояния и верижно изпълнение (Chaining)](#4-promises-3-те-състояния-и-верижно-изпълнение-chaining)
* [5. `async / await` под капака и паралелизъм](#5-async--await-под-капака-и-паралелизъм)
* [6. Скрити капани и Edge Cases](#6-скрити-капани-и-edge-cases)

---

## 1. Защо JavaScript е еднонишков (Single-Threaded), но неблокиращ?

JavaScript е проектиран да манипулира **DOM дървото** в браузъра. Ако две нишки едновременно променят или изтриват един и същ HTML елемент, биха възникнали тежки *Race Conditions*. Затова V8 енджинът има **само един Call Stack**.

За да не „замръзва“ интерфейсът при бавни операции (HTTP заявки, таймери, четене на файлове), браузърът делегира тези задачи на многонишковите **Web APIs** (написани на C++), а резултатите се връщат обратно в JavaScript чрез **Event Loop**.

---

## 2. Архитектура на Event Loop: Microtask Queue vs Macrotask Queue

В Browser Runtime съществуват **две отделни опашки** за асинхронни задачи, като **Microtask Queue има абсолютен приоритет** пред Macrotask Queue:

```text
┌───────────────────┐     ┌────────────────────────────────────────┐
│    CALL STACK     │     │               WEB APIs                 │
│  (Синхронен код)  │────►│  • fetch() / Network                   │
└─────────▲─────────┘     │  • setTimeout() / setInterval()        │
          │               │  • DOM Events (click, input)           │
          │               └───────────┬────────────────┬───────────┘
          │                           │                │
   ┌──────┴──────┐                    ▼                ▼
   │             │       ┌─────────────────────┐  ┌────────────────────┐
   │ EVENT LOOP  │◄──────┤  MICROTASK QUEUE    │  │  MACROTASK QUEUE   │
   │             │  1-во │ (Висок приоритет!)  │  │ (Callback Queue)   │
   └─────────────┘       │ • Promise.then/catch│  │ • setTimeout       │
                         │ • queueMicrotask()  │  │ • setInterval      │
                         │ • MutationObserver  │  │ • UI Events        │
                         └─────────────────────┘  └────────────────────┘
```

> **🧠 Мнемоника:** **Microtasks са VIP пътници** — докато на летището има поне един VIP пътник (Microtask), самолетът (Event Loop) **няма да качи нито един** редови пътник от Macrotask опашката!

---

## 3. Точният алгоритъм на един цикъл (Tick) на Event Loop

1. Изпълнява целия **синхронен код** в **Call Stack**, докато стекът стане напълно празен.
2. Проверява **Microtask Queue** и изпълнява **ВСИЧКИ** налични микрозадачи до пълното изпразване на опашката (включително нови микрозадачи, добавени по време на самото им изпълнение!).
3. Ако е необходимо (обикновено на всеки ~16.6ms при 60Hz екран), браузърът извършва **UI Render** (Style ➔ Layout/Reflow ➔ Paint).
4. Взема **само ЕДНА** задача от **Macrotask Queue**, изпълнява я в Call Stack, и след приключването ѝ отново проверява **цялата Microtask Queue** (стъпка 2).

---

## 4. Promises: 3-те състояния и верижно изпълнение (Chaining)

**`Promise`** е обект, представляващ евентуалното завършване (или провал) на асинхронна операция. Той може да бъде в едно от **3 взаимноизключващи се състояния**:

1. **`pending` (Изчакващ):** Началното състояние — операцията все още тече.
2. **`fulfilled` (Изпълнен успешно):** Операцията е приключила със стойност (`resolve(value)`).
3. **`rejected` (Отхвърлен с грешка):** Операцията се е провалила с причина (`reject(error)`).

След като един Promise премине от `pending` в `fulfilled` или `rejected`, той е **settled (установен)** и състоянието му **никога повече не може да се промени** (Immutable state).

---

## 5. `async / await` под капака и паралелизъм

* Ключовата дума **`async`** пред функция гарантира, че тя **винаги връща `Promise`** (дори ако върнете обикновено число `return 42`, то автоматично се обвива в `Promise.resolve(42)`).
* Ключовата дума **`await`** поставя на пауза изпълнението **само на текущата `async` функция** (без да блокира Call Stack или интерфейса!), регистрира остатъка от функцията като **Microtask** и отстъпва контрола на Event Loop.
* **Сравнение с C#:** `Promise` в JavaScript е директният концептуален аналог на `Task<T>` в C#, а `async/await` работи по идентичен модел със state machine (с разликата, че в JS всичко се изпълнява върху 1 нишка).

---

## 6. Скрити капани и Edge Cases

1. **Последователен `await` вместо паралелен (`Promise.all`):**
   * Писането на `const a = await fetchUser(); const b = await fetchOrders();` изпълнява заявките **една след друга** (удвоява времето за чакане). Когато заявките не зависят една от друга, винаги ги стартирайте едновременно с `await Promise.all([fetchUser(), fetchOrders()])`!
2. **`fetch()` НЕ отхвърля (reject) Promise при HTTP 404 или 500!**
   * `fetch()` се проваля (`reject`) **само при мрежова грешка** (липса на интернет, DNS грешка, CORS блокиране). При отговор `404 Not Found` или `500 Internal Server Error`, Promise-ът е **`fulfilled`** с `response.ok === false`! Винаги проверявайте `if (!response.ok) throw new Error(...)`.
3. **Грешка в `forEach` с `async/await`:**
   * `array.forEach(async (item) => { await save(item); })` **НЕ изчаква** операциите — `forEach` стартира всички callbacks наведнъж и продължава напред. За последователно изпълнение използвайте `for (const item of array) { await save(item); }`, а за паралелно — `await Promise.all(array.map(item => save(item)))`.







---

[⬅️ 02-03-01. Classes, Inheritance & Prototypes — Syntax...](02-03-01-Syntax-Prototypes-OOP-And-Classes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-04-01. Promises, Async/Await & Fetch API — Syntax...](02-04-01-Syntax-Event-Loop-And-Asynchronous-JS.md)

© 2026 Dev-Library / 02-04-00. Event Loop, Microtasks, Promises & `async/await`
