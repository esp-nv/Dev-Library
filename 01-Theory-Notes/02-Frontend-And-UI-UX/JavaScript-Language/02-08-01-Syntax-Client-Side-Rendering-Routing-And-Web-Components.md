[⬅️ 02-08-00. Client-Side Rendering, Browser Routing & W...](02-08-00-Client-Side-Rendering-Routing-And-Web-Components.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-09-00. Software Testing Architecture: Unit, Integ...](02-09-00-Testing-And-E2E-Playwright.md)

---

# 🟨 02-08-01. `lit-html`, `page.js` & Web Components — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. `lit-html` Bindings, Условия, Списъци и Директиви](#1-lit-html-bindings-условия-списъци-и-директиви)
* [2. Нативен HTML5 History API (`pushState` & `popstate`)](#2-нативен-html5-history-api-pushstate--popstate)
* [3. SPA Routing с `page.js` (Middleware, Params & Redirect)](#3-spa-routing-с-pagejs-middleware-params--redirect)
* [4. Създаване на Native Web Component със Shadow DOM и `<slot>`](#4-създаване-на-native-web-component-със-shadow-dom-и-slot)

---

## 1. `lit-html` Bindings, Условия, Списъци и Директиви

```javascript
import { html, render } from "lit-html";
import { classMap } from "lit-html/directives/class-map.js";
import { styleMap } from "lit-html/directives/style-map.js";
import { repeat } from "lit-html/directives/repeat.js";

const catalogTemplate = (ctx) => html`
  <section class="catalog">
    <!-- 1. Текстово и Атрибутно свързване -->
    <h1>${ctx.title}</h1>

    <!-- 2. Boolean атрибут (?) и DOM Property (.) -->
    <input .value=${ctx.searchQuery} ?disabled=${ctx.isLoading} />

    <!-- 3. Декларативен Event Listener (@event) -->
    <button @click=${ctx.onRefresh}>Обнови</button>

    <!-- 4. Директиви classMap и styleMap -->
    <div class=${classMap({ banner: true, "is-active": ctx.hasFilter })}
         style=${styleMap({ color: ctx.isError ? "crimson" : "inherit" })}>
      Статус: ${ctx.statusText}
    </div>

    <!-- 5. Условно рендиране (Ternary оператор) -->
    ${ctx.items.length === 0
      ? html`<p>Няма намерени елементи.</p>`
      : html`
          <ul>
            <!-- 6. Оптимизиран списък с ключ чрез repeat() -->
            \${repeat(ctx.items, (item) => item._id, (item) => html`
              <li>
                <a href="/details/${item._id}">${item.name}</a>
              </li>
            `)}
          </ul>
        `}
  </section>
`;

// Рендиране в контейнера
const root = document.querySelector("#app");
render(catalogTemplate(state), root);
```

---

## 2. Нативен HTML5 History API (`pushState` & `popstate`)

```javascript
// Промяна на URL без презареждане на страницата
function navigateTo(path, state = {}) {
  window.history.pushState(state, "", path);
  renderCurrentRoute(path);
}

// Слушане за бутоните Back / Forward в браузъра
window.addEventListener("popstate", (event) => {
  console.log("Възстановено състояние:", event.state);
  renderCurrentRoute(window.location.pathname);
});
```

---

## 3. SPA Routing с `page.js` (Middleware, Params & Redirect)

```javascript
import page from "page";
import { render } from "lit-html";
import { getUserData } from "./api/authService.js";

const mainContainer = document.querySelector("main");

// 1. Глобален Middleware за обогатяване на контекста (ctx)
function decorateContext(ctx, next) {
  ctx.render = (templateResult) => render(templateResult, mainContainer);
  ctx.user = getUserData();
  next(); // Задължително предава контрола към следващия handler!
}

// 2. Зареждане на данни като отделен Middleware
async function loadDetails(ctx, next) {
  const id = ctx.params.id; // Извлича динамичния параметър :id
  ctx.item = await fetch(`/api/items/${id}`).then(r => r.json());
  next();
}

// 3. Регистриране на маршрути
page(decorateContext);
page("/", homeView);
page("/catalog", catalogView);
page("/details/:id", loadDetails, detailsView);
page("*", () => page.redirect("/")); // Fallback при несъществуващ път

page.start(); // Стартира рутера
```

---

## 4. Създаване на Native Web Component със Shadow DOM и `<slot>`

```javascript
const template = document.createElement("template");
template.innerHTML = `
  <style>
    /* Изолирани стилове — важат САМО вътре в компонента! */
    :host { display: block; border: 1px solid #cbd5e1; border-radius: 8px; padding: 16px; }
    h3 { margin: 0 0 8px; color: #0f172a; }
    button { background: #2563eb; color: white; border: none; padding: 6px 12px; border-radius: 4px; }
  </style>
  <article>
    <h3><slot name="title">Заглавие по подразбиране</slot></h3>
    <p id="value-box"></p>
    <button id="copy-btn">Копирай</button>
  </article>
`;

class FormulaBadge extends HTMLElement {
  static get observedAttributes() {
    return ["formula"]; // Атрибутите, които компонентът следи за промени
  }

  constructor() {
    super();
    this.attachShadow({ mode: "open" });
    this.shadowRoot.appendChild(template.content.cloneNode(true));
    this.#valueBox = this.shadowRoot.querySelector("#value-box");
    this.#copyBtn = this.shadowRoot.querySelector("#copy-btn");
  }

  #valueBox;
  #copyBtn;

  connectedCallback() {
    this.#valueBox.textContent = this.getAttribute("formula") ?? "";
    this.#copyBtn.addEventListener("click", this.#handleCopy);
  }

  disconnectedCallback() {
    this.#copyBtn.removeEventListener("click", this.#handleCopy);
  }

  attributeChangedCallback(name, oldValue, newValue) {
    if (name === "formula" && oldValue !== newValue) {
      this.#valueBox.textContent = newValue;
    }
  }

  #handleCopy = () => {
    navigator.clipboard?.writeText(this.#valueBox.textContent);
  };
}

// Регистрация на новия HTML таг
customElements.define("formula-badge", FormulaBadge);
```

```html
<!-- Използване директно в HTML -->
<formula-badge formula="E = m * c^2">
  <span slot="title">Енергия на покой</span>
</formula-badge>
```







---

[⬅️ 02-08-00. Client-Side Rendering, Browser Routing & W...](02-08-00-Client-Side-Rendering-Routing-And-Web-Components.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-09-00. Software Testing Architecture: Unit, Integ...](02-09-00-Testing-And-E2E-Playwright.md)

© 2026 Dev-Library / 02-08-01. `lit-html`, `page.js` & Web Components — Syntax & Cheat Sheet
