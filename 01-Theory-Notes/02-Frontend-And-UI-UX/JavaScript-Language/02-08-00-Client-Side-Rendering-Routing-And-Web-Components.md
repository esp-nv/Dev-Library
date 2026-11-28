[⬅️ 02-07-01. API Requester, Auth & ES Modules — Syntax ...](02-07-01-Syntax-Remote-Data-Auth-And-SPA-Architecture.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-08-01. `lit-html`, `page.js` & Web Components — S...](02-08-01-Syntax-Client-Side-Rendering-Routing-And-Web-Components.md)

---

# 🟨 02-08-00. Client-Side Rendering, Browser Routing & Web Components

## 📑 Съдържание
* [1. Server-Side Rendering (SSR) vs Client-Side Rendering (CSR)](#1-server-side-rendering-ssr-vs-client-side-rendering-csr)
* [2. Client-Side Templating и как работи `lit-html` под капака](#2-client-side-templating-и-как-работи-lit-html-под-капака)
* [3. Browser Routing: Hash-Based Routing vs HTML5 History API (`pushState`)](#3-browser-routing-hash-based-routing-vs-html5-history-api-pushstate)
* [4. Архитектура на `page.js` (Middleware Pipeline & Context)](#4-архитектура-на-pagejs-middleware-pipeline--context)
* [5. Native Web Components: Custom Elements, Shadow DOM & Lifecycle](#5-native-web-components-custom-elements-shadow-dom--lifecycle)
* [6. Скрити капани и Best Practices](#6-скрити-капани-и-best-practices)

---

## 1. Server-Side Rendering (SSR) vs Client-Side Rendering (CSR)

| Критерий | Server-Side Rendering (SSR) | Client-Side Rendering (CSR) |
| :--- | :--- | :--- |
| **Къде се генерира HTML?** | На сървъра (Node.js, ASP.NET, Django) при всяка заявка | В браузъра чрез JavaScript след изтегляне на JSON данни |
| **Трафик по мрежата** | Пълен HTML документ при всяка навигация | Само сурови JSON данни (по-малък bandwidth) |
| **SEO и Първо рисуване (FCP)** | Мигновено визуализиране и пълно индексиране от търсачки | По-бавен първи старт; търсачките трябва да изпълнят JS |
| **Интерактивност след зареждане** | Всяка смяна на страница изисква нов HTTP round-trip | Мигновена реакция без презареждане на страницата |

---

## 2. Client-Side Templating и как работи `lit-html` под капака

Ръчното създаване на DOM елементи с `document.createElement` е бавно за писане и трудно за поддръжка, а директното презаписване на `container.innerHTML = ...` **унищожава целия DOM**, губи фокуса на `<input>` полетата и изтрива закачените Event Listeners!

Библиотеки като **`lit-html`** решават този проблем чрез **Tagged Template Literals** (``html`...` ``):
1. При първото рендиране `lit-html` разделя шаблона на **статична HTML част** (която браузърът парсва само веднъж като `<template>`) и **динамични маркери (`${...}`)**.
2. При последващи обновления `lit-html` **НЕ презаписва целия DOM** — обновява хирургически **само конкретните текстови възли или атрибути**, чиито стойности са се променили!
3. Всички вмъкнати стойности се **escape-ват автоматично**, което елиминира XSS уязвимостите.

---

## 3. Browser Routing: Hash-Based Routing vs HTML5 History API (`pushState`)

В едно SPA приложение **Router-ът** синхронизира URL адреса в браузъра с текущия изглед (View), без да презарежда страницата:

| Подход | Пример за URL | Механизъм | Предимства и Недостатъци |
| :--- | :--- | :--- | :--- |
| **Hash-Based Routing** | `https://app.com` | Променя `window.location.hash` и слуша за събитието `hashchange` | Работи без конфигурация на сървъра, но не е чисто за SEO и пречи на anchor линковете (`#section`) |
| **Push-Based (History API)** | `https://app.com` | Използва `history.pushState()`, `history.replaceState()` и събитието `popstate` | Чисти, SEO-съвместими URL адреси; изисква сървърът да връща `index.html` за всички маршрути (SPA Fallback) |

### Анатомия на `window.location`
```text
https://example.com
└─┬─┘   └───────┬──────┘└─┬─┘└──────┬─────┘└───────┬────────┘└─┬─┘
protocol    hostname    port    pathname        search        hash
```

---

## 4. Архитектура на `page.js` (Middleware Pipeline & Context)

**`page.js`** е лек (~1.2 KB) клиентски рутер с Express-подобен синтаксис. Той прихваща автоматично кликовете върху `<a href="/...">` таговете, спира презареждането и прекарва заявката през верига от **Route Handlers (Middleware)**:

```text
Клик върху <a href="/details/15">
       │
       ▼
[ Middleware 1: decorateContext ] ──► закача ctx.render() и ctx.user
       │ next()
       ▼
[ Middleware 2: loadItemData ]    ──► fetch('/api/items/15') ➔ ctx.item = data
       │ next()
       ▼
[ Handler 3: detailsView(ctx) ]   ──► рендира шаблона с ctx.item на екрана
```

---

## 5. Native Web Components: Custom Elements, Shadow DOM & Lifecycle

**Web Components** са вграден стандарт в браузърите за създаване на капсулирани, преизползваеми HTML тагове (напр. `<formula-card></formula-card>`) без зависимост от React, Angular или Vue. Те стъпват върху **3 основни спецификации**:

1. **Custom Elements (`customElements.define`):** Регистрира нов HTML таг (задължително с тире `-` в името, за да се различава от вградените тагове), наследяващ `HTMLElement`.
2. **Shadow DOM (`this.attachShadow({ mode: 'open' | 'closed' })`):** Създава изолирано под-дърво в DOM. CSS стиловете вътре в Shadow DOM **не изтичат навън**, а глобалните CSS правила на страницата **не могат да счупят** вътрешния дизайн на компонента!
3. **HTML `<template>` и `<slot>`:** `<template>` държи инертна HTML структура (не се рендира до клониране), а `<slot name="title">` позволява вмъкване на външно съдържание на точно определени места.

### Lifecycle Hooks (Жизнен цикъл на Web Component)
* **`constructor()`:** Извиква се при създаване на инстанцията (инициализира `attachShadow` и начален state).
* **`connectedCallback()`:** Извиква се при **добавяне на елемента в DOM** (идеално за `fetch()` заявки и закачане на слушатели).
* **`disconnectedCallback()`:** Извиква се при **премахване от DOM** (почистване на `setInterval` и глобални събития срещу Memory Leaks).
* **`attributeChangedCallback(name, oldVal, newVal)`:** Извиква се при промяна на атрибут, описан в `static get observedAttributes()`.

---

## 6. Скрити капани и Best Practices

1. **Имената на Custom Elements задължително съдържат тире (`-`):** `customElements.define("mywidget", MyWidget)` хвърля `SyntaxError` — валидно е `"my-widget"`.
2. **Чисти функции за шаблоните (Pure Templates):** Шаблоните в `lit-html` не трябва да правят `fetch()` заявки или да мутират глобално състояние — те приемат `data` и връщат `html` структура.
3. **SPA Fallback при History API:** Ако използвате `page.js` с чисти пътища (`/catalog/42`) и потребителят натисне **F5 (Refresh)**, браузърът ще поиска `/catalog/42` директно от сървъра. Конфигурирайте сървъра (Vite, Nginx, ASP.NET `MapFallbackToFile`) да връща `index.html`.







---

[⬅️ 02-07-01. API Requester, Auth & ES Modules — Syntax ...](02-07-01-Syntax-Remote-Data-Auth-And-SPA-Architecture.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-08-01. `lit-html`, `page.js` & Web Components — S...](02-08-01-Syntax-Client-Side-Rendering-Routing-And-Web-Components.md)

© 2026 Dev-Library / 02-08-00. Client-Side Rendering, Browser Routing & Web Components
