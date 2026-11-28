[⬅️ 02-04-00. Event Loop, Microtasks, Promises & `async/...](02-04-00-Event-Loop-And-Asynchronous-JS.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-05-00. DOM Manipulation, Event Delegation & Web S...](02-05-00-DOM-Events-And-Browser-Storage.md)

---

# 🟨 02-04-01. Promises, Async/Await & Fetch API — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Създаване и консумиране на `Promise`](#1-създаване-и-консумиране-на-promise)
* [2. Модерен `async / await` с `try / catch / finally`](#2-модерен-async--await-с-try--catch--finally)
* [3. Комбинатори на Promises: `all`, `allSettled`, `race` и `any`](#3-комбинатори-на-promises-all-allsettled-race-и-any)
* [4. HTTP заявки с `fetch()` API (GET, POST, Headers, JSON)](#4-http-заявки-с-fetch-api-get-post-headers-json)
* [5. Прекъсване на заявки и Timeout с `AbortController`](#5-прекъсване-на-заявки-и-timeout-с-abortcontroller)
* [6. Синтактичен Cheat Sheet (JS Async срещу C# Task/HttpClient)](#6-синтактичен-cheat-sheet-js-async-срещу-c-taskhttpclient)

---

## 1. Създаване и консумиране на `Promise`

```javascript
// Създаване на Promise (напр. обвиване на setTimeout в Promise)
const delay = (ms) => new Promise((resolve, reject) => {
  if (ms < 0) {
    reject(new RangeError("Времето не може да е отрицателно."));
    return;
  }
  setTimeout(() => resolve(`Изминаха ${ms}ms`), ms);
});

// Консумиране чрез .then() / .catch() / .finally()
delay(500)
  .then(message => console.log(message))
  .catch(err => console.error(err.message))
  .finally(() => console.log("Операцията приключи."));
```

---

## 2. Модерен `async / await` с `try / catch / finally`

```javascript
async function loadFormulaData(formulaId) {
  try {
    const response = await fetch(`/api/formulas/${formulaId}`);

    // Задължителна проверка за HTTP статуси 4xx / 5xx!
    if (!response.ok) {
      throw new Error(`HTTP грешка! Статус: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Неуспешно зареждане на формула:", error.message);
    throw error; // Прехвърляме грешката нагоре, ако викащият код трябва да знае
  } finally {
    console.log("Заявката за формула приключи.");
  }
}
```

---

## 3. Комбинатори на Promises: `all`, `allSettled`, `race` и `any`

```javascript
const p1 = fetch("/api/categories").then(r => r.json());
const p2 = fetch("/api/constants").then(r => r.json());
const p3 = fetch("/api/user-settings").then(r => r.json());

// 1. Promise.all — чака ВСИЧКИ да успеят; проваля се веднага при ПЪРВАТА грешка (Fail-Fast)
const [categories, constants, settings] = await Promise.all([p1, p2, p3]);

// 2. Promise.allSettled — чака ВСИЧКИ да приключат, независимо дали са успели или пропаднали
const results = await Promise.allSettled([p1, p2, p3]);
results.forEach(res => {
  if (res.status === "fulfilled") console.log("Успех:", res.value);
  else console.warn("Грешка:", res.reason);
});

// 3. Promise.race — връща резултата (успех ИЛИ грешка) на НАЙ-БЪРЗИЯ Promise
const fastest = await Promise.race([p1, p2]);

// 4. Promise.any — връща ПЪРВИЯ УСПЕШЕН Promise (игнорира грешките, освен ако всички пропаднат)
const firstSuccess = await Promise.any([p1, p2, p3]);
```

---

## 4. HTTP заявки с `fetch()` API (GET, POST, Headers, JSON)

```javascript
// POST заявка с JSON тяло и Authorization Bearer токен
async function saveCalculation(payload, token) {
  const response = await fetch("https://example.com", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(`API Error (${response.status}): ${errorBody}`);
  }

  return await response.json();
}
```

---

## 5. Прекъсване на заявки и Timeout с `AbortController`

```javascript
// 1. Автоматичен Timeout след 5 секунди чрез AbortSignal.timeout() (Модерен ES2023+)
async function fetchWithTimeout(url, timeoutMs = 5000) {
  try {
    const res = await fetch(url, { signal: AbortSignal.timeout(timeoutMs) });
    return await res.json();
  } catch (err) {
    if (err.name === "TimeoutError") {
      console.error("Заявката изтече (Timeout)!");
    }
    throw err;
  }
}

// 2. Ръчно прекъсване (напр. при бързо писане в търсачка / потребителска отмяна)
const controller = new AbortController();
fetch("/api/search?q=circle", { signal: controller.signal });
controller.abort(); // Прекъсва HTTP заявката веднага (хвърля AbortError)
```

---

## 6. Синтактичен Cheat Sheet (JS Async срещу C# Task/HttpClient)

| Асинхронна операция | JavaScript (ES2025+) | C# (.NET) |
| :--- | :--- | :--- |
| **Асинхронна функция** | `async function getData() { ... }` | `public async Task<Data> GetDataAsync()` |
| **Изчакване на резултат** | `const data = await promise;` | `var data = await task;` |
| **Паралелно изчакване (All)** | `await Promise.all([p1, p2])` | `await Task.WhenAll(t1, t2);` |
| **Най-бърза задача (Any/Race)** | `await Promise.race([p1, p2])` | `await Task.WhenAny(t1, t2);` |
| **Пауза без блокиране** | `await new Promise(r => setTimeout(r, 1000))` | `await Task.Delay(1000);` |
| **Отмяна на заявка (Cancel)** | `AbortController` / `AbortSignal` | `CancellationTokenSource` / `CancellationToken` |
| **HTTP GET + JSON** | `await (await fetch(url)).json()` | `await httpClient.GetFromJsonAsync<T>(url)` |







---

[⬅️ 02-04-00. Event Loop, Microtasks, Promises & `async/...](02-04-00-Event-Loop-And-Asynchronous-JS.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-05-00. DOM Manipulation, Event Delegation & Web S...](02-05-00-DOM-Events-And-Browser-Storage.md)

© 2026 Dev-Library / 02-04-01. Promises, Async/Await & Fetch API — Syntax & Cheat Sheet
