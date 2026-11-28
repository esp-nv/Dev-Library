[⬅️ 02-07-00. Remote Data, Authentication & Modular SPA ...](02-07-00-Remote-Data-Auth-And-SPA-Architecture.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-08-00. Client-Side Rendering, Browser Routing & W...](02-08-00-Client-Side-Rendering-Routing-And-Web-Components.md)

---

# 🟨 02-07-01. API Requester, Auth & ES Modules — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Универсален `api.js` Requester модул (CRUD + Auto Token)](#1-универсален-apijs-requester-модул-crud--auto-token)
* [2. Автентикация (`login`, `register`, `logout`) и `sessionStorage`](#2-автентикация-login-register-logout-и-sessionstorage)
* [3. Обработка на форми с `FormData` и защита от Double Submit](#3-обработка-на-форми-с-formdata-и-защита-от-double-submit)
* [4. ES6 `export` и `import` синтаксис](#4-es6-export-и-import-синтаксис)

---

## 1. Универсален `api.js` Requester модул (CRUD + Auto Token)

Този шаблон е стандарт за чист SPA Data Layer: централизира базовия URL, автоматично добавя `Content-Type: application/json` и `X-Authorization` токена, и обработва `204 No Content` отговори:

```javascript
// src/api/api.js
const HOST = "http://localhost:3030";

async function request(method, url, data) {
  const options = {
    method,
    headers: {}
  };

  if (data !== undefined) {
    options.headers["Content-Type"] = "application/json";
    options.body = JSON.stringify(data);
  }

  const user = JSON.parse(sessionStorage.getItem("userData"));
  if (user?.accessToken) {
    options.headers["X-Authorization"] = user.accessToken;
  }

  const response = await fetch(HOST + url, options);

  if (!response.ok) {
    if (response.status === 403) {
      sessionStorage.removeItem("userData"); // Изтекла или невалидна сесия
    }
    const err = await response.json().catch(() => ({ message: response.statusText }));
    throw new Error(err.message);
  }

  // Статус 204 (напр. при logout или DELETE) няма тяло за парсване
  if (response.status === 204) {
    return response;
  }

  return response.json();
}

export const get = (url) => request("GET", url);
export const post = (url, data) => request("POST", url, data);
export const put = (url, data) => request("PUT", url, data);
export const del = (url) => request("DELETE", url);
```

---

## 2. Автентикация (`login`, `register`, `logout`) и `sessionStorage`

```javascript
// src/api/authService.js
import { get, post } from "./api.js";

export async function login(email, password) {
  const user = await post("/users/login", { email, password });
  sessionStorage.setItem("userData", JSON.stringify(user));
  return user;
}

export async function register(email, password) {
  const user = await post("/users/register", { email, password });
  sessionStorage.setItem("userData", JSON.stringify(user));
  return user;
}

export async function logout() {
  await get("/users/logout");
  sessionStorage.removeItem("userData");
}

export function getUserData() {
  return JSON.parse(sessionStorage.getItem("userData"));
}
```

---

## 3. Обработка на форми с `FormData` и защита от Double Submit

```javascript
// Универсален helper за формуляри в SPA
export function createSubmitHandler(form, callback) {
  form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const submitBtn = form.querySelector("[type='submit']");
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());

    // Изчистваме водещи и крайни интервали от всички текстови полета
    for (const key in data) {
      if (typeof data[key] === "string") {
        data[key] = data[key].trim();
      }
    }

    try {
      if (submitBtn) submitBtn.disabled = true;
      await callback(data, form);
    } finally {
      if (submitBtn) submitBtn.disabled = false;
    }
  });
}
```

---

## 4. ES6 `export` и `import` синтаксис

```javascript
// 1. Named Exports (Препоръчително за по-добър Tree-Shaking и автодовършване)
export const API_VERSION = "v1";
export function formatDate(iso) { return new Date(iso).toLocaleDateString("bg-BG"); }

// 2. Default Export (Най-много 1 на файл)
export default function initApp() { /* ... */ }

// 3. Импортиране в друг модул
import initApp, { API_VERSION, formatDate as fmt } from "./utils.js";
import * as utils from "./utils.js"; // Namespace import
```







---

[⬅️ 02-07-00. Remote Data, Authentication & Modular SPA ...](02-07-00-Remote-Data-Auth-And-SPA-Architecture.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-08-00. Client-Side Rendering, Browser Routing & W...](02-08-00-Client-Side-Rendering-Routing-And-Web-Components.md)

© 2026 Dev-Library / 02-07-01. API Requester, Auth & ES Modules — Syntax & Cheat Sheet
