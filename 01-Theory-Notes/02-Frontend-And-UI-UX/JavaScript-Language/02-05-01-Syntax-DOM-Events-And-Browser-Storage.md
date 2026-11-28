[⬅️ 02-05-00. DOM, Events & Browser Storage](02-05-00-DOM-Events-And-Browser-Storage.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-06. Browser JS vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md)

---

# 🟨 02-05-01. DOM Manipulation & Event Delegation — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Селектиране и навигация в DOM дървото](#1-селектиране-и-навигация-в-dom-дървото)
* [2. Създаване, вмъкване и изтриване на елементи (`DocumentFragment`)](#2-създаване-вмъкване-и-изтриване-на-елементи-documentfragment)
* [3. Работа с класове (`classList`), атрибути и `dataset`](#3-работа-с-класове-classlist-атрибути-и-dataset)
* [4. Event Delegation и управление на форми (`preventDefault`)](#4-event-delegation-и-управление-на-форми-preventdefault)
* [5. Типово-безопасен Wrapper за `localStorage`](#5-типово-безопасен-wrapper-за-localstorage)
* [6. Синтактичен Cheat Sheet (DOM & Storage API)](#6-синтактичен-cheat-sheet-dom--storage-api)

---

## 1. Селектиране и навигация в DOM дървото

```javascript
// 1. Модерни CSS селектори (Static резултати)
const display = document.querySelector("#calc-display");
const buttons = document.querySelectorAll(".keypad__btn"); // NodeList (поддържа forEach)

// 2. Навигация спрямо текущ елемент (само Element възли)
const card = display.closest(".calculator-card"); // Търси нагоре по предците
const parent = display.parentElement;
const nextEl = display.nextElementSibling;
const children = Array.from(card.children);
```

---

## 2. Създаване, вмъкване и изтриване на елементи (`DocumentFragment`)

```javascript
// Оптимално добавяне на списък с 1 единствен Reflow чрез DocumentFragment
function renderHistoryList(container, items) {
  const fragment = document.createDocumentFragment();

  for (const item of items) {
    const li = document.createElement("li");
    li.className = "history-item";
    li.textContent = `${item.expression} = ${item.result}`; // Безопасно срещу XSS!
    li.dataset.id = item.id;
    fragment.append(li);
  }

  // Изчистваме старите деца и вмъкваме всички нови наведнъж
  container.replaceChildren(fragment);
}
```

---

## 3. Работа с класове (`classList`), атрибути и `dataset`

```javascript
const panel = document.querySelector(".settings-panel");

// 1. Управление на CSS класове
panel.classList.add("is-open");
panel.classList.remove("is-hidden");
panel.classList.toggle("dark-theme");
const isOpen = panel.classList.contains("is-open"); // true

// 2. HTML5 data-* атрибути (data-formula-id="42" -> element.dataset.formulaId)
panel.dataset.formulaId = "42";
console.log(panel.dataset.formulaId); // "42"

// 3. Стандартни атрибути
panel.setAttribute("aria-expanded", "true");
panel.removeAttribute("disabled");
```

---

## 4. Event Delegation и управление на форми (`preventDefault`)

```javascript
// 1. Event Delegation върху родителски контейнер
const keypad = document.querySelector(".keypad");

keypad.addEventListener("click", (event) => {
  const button = event.target.closest("button[data-action]");
  if (!button || !keypad.contains(button)) return;

  const { action, value } = button.dataset;
  console.log(`Действие: ${action}, Стойност: ${value}`);
});

// 2. Обработка на форма с FormData API (без презареждане на страницата)
const form = document.querySelector("#formula-form");
form.addEventListener("submit", (event) => {
  event.preventDefault(); // Спира стандартното HTTP изпращане на браузъра

  const formData = new FormData(form);
  const payload = Object.fromEntries(formData.entries());
  console.log("Данни от формата:", payload);
});
```

---

## 5. Типово-безопасен Wrapper за `localStorage`

```javascript
const storage = {
  get(key, fallback = null) {
    try {
      const raw = localStorage.getItem(key);
      return raw ? JSON.parse(raw) : fallback;
    } catch {
      return fallback; // Предпазва от срив при повредени JSON данни
    }
  },
  set(key, value) {
    localStorage.setItem(key, JSON.stringify(value));
  },
  remove(key) {
    localStorage.removeItem(key);
  }
};

storage.set("calc_prefs", { precision: 4, theme: "dark" });
const prefs = storage.get("calc_prefs", { precision: 2, theme: "light" });
```

---

## 6. Синтактичен Cheat Sheet (DOM & Storage API)

| Цел | Модерен метод (ES2025+) | Забележка |
| :--- | :--- | :--- |
| **Намиране на 1 елемент** | `document.querySelector(".btn")` | Връща първия съвпадащ `Element` или `null` |
| **Намиране на всички елементи** | `document.querySelectorAll(".btn")` | Връща статичен `NodeList` |
| **Търсене на родител по селектор** | `el.closest(".card")` | Незаменимо при Event Delegation |
| **Безопасно задаване на текст** | `el.textContent = "Hello"` | Предпазва от XSS инжекции |
| **Замяна на всички деца** | `parent.replaceChildren(node1, node2)` | По-чисто и бързо от `innerHTML = ""` |
| **Премахване на елемент** | `el.remove()` | Премахва директно елемента от DOM |
| **Запис в LocalStorage** | `localStorage.setItem("k", JSON.stringify(v))` | Приема само стрингове |

---

[⬅️ 02-05-00. DOM, Events & Browser Storage](02-05-00-DOM-Events-And-Browser-Storage.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-06. Browser JS vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md)

© 2026 Dev-Library / 02-05-01. DOM Manipulation & Event Delegation — Syntax & Cheat Sheet
