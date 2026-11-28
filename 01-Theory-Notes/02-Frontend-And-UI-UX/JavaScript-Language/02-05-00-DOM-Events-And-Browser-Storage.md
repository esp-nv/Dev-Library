[⬅️ 02-04-01. Event Loop & Asynchronous JS — Syntax](02-04-01-Syntax-Event-Loop-And-Asynchronous-JS.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-05-01. DOM, Events & Browser Storage — Syntax](02-05-01-Syntax-DOM-Events-And-Browser-Storage.md)

---

# 🟨 02-05-00. DOM Manipulation, Event Delegation & Web Storage API

## 📑 Съдържание
* [1. Какво е DOM (Document Object Model) и Critical Rendering Path?](#1-какво-е-dom-document-object-model-и-critical-rendering-path)
* [2. `Node` vs `Element` и Live vs Static колекции](#2-node-vs-element-и-live-vs-static-колекции)
* [3. Фази на разпространение на събитията (Event Propagation)](#3-фази-на-разпространение-на-събитията-event-propagation)
* [4. Event Delegation (Делегиране на събития) — Златен шаблон](#4-event-delegation-делегиране-на-събития--златен-шаблон)
* [5. Съхранение на данни в браузъра: `localStorage`, `sessionStorage` и Cookies](#5-съхранение-на-данни-в-браузъра-localstorage-sessionstorage-и-cookies)
* [6. Скрити капани и Edge Cases](#6-скрити-капани-и-edge-cases)

---

## 1. Какво е DOM (Document Object Model) и Critical Rendering Path?

**DOM** не е самият HTML файл, а **живо обектно дърво в паметта на браузъра**, което C++ енджинът изгражда при парсване на HTML документа и излага към JavaScript чрез глобалния обект `document`.

```text
HTML парсване ──► DOM Tree ───┐
                              ├──► Render Tree ──► Layout (Reflow) ──► Paint ──► Composite
CSS парсване  ──► CSSOM Tree ─┘
```

* **Reflow (Layout):** Преизчисляване на размерите и позициите на елементите (скъпа операция!). Предизвиква се при промяна на `width`, `height`, `margin`, `display` или четене на `offsetHeight`.
* **Repaint:** Прерисуване на пикселите (напр. `color`, `background-color`) без промяна в геометрията (по-бърза операция).

---

## 2. `Node` vs `Element` и Live vs Static колекции

* **`Node` (Възел):** Базовият интерфейс в DOM. Включва HTML елементи (`Element`), текстови възли (`Text` — дори празните интервали и нови редове!), коментари (`Comment`) и самия `Document`.
* **`Element` (Елемент):** Наследник на `Node`, представляващ конкретен HTML таг (`<div>`, `<button>`, `<input>`). Свойства като `children`, `firstElementChild` и `nextElementSibling` работят само с елементи (пропускат празните текстови възли).
* **Static `NodeList` (`querySelectorAll`):** Моментна снимка (Snapshot) на DOM в момента на извикване — не се променя при добавяне на нови елементи.
* **Live `HTMLCollection` (`getElementsByClassName`, `children`):** Обновява се автоматично в реално време при всяка промяна в DOM (може да доведе до безкраен цикъл при обхождане с `while`!).

---

## 3. Фази на разпространение на събитията (Event Propagation)

Когато потребител кликне върху бутон `<button>` вътре в `<div>` вътре в `<body>`, събитието преминава през **3 фази**:

```text
1. CAPTURING PHASE (Надолу)      2. TARGET PHASE        3. BUBBLING PHASE (Нагоре)
   window ──► document ──► body ──► <button> (Цел) ──► body ──► document ──► window
```

1. **Capturing Phase (Потъване):** Събитието тръгва от `window` надолу към целевия елемент. (По подразбиране `addEventListener` НЕ слуша в тази фаза, освен ако не подадете `{ capture: true }`).
2. **Target Phase (Цел):** Събитието достига самия елемент (`event.target`).
3. **Bubbling Phase (Изплуване):** Събитието се изкачва нагоре по родителските елементи (по подразбиране всички `addEventListener` се изпълняват тук).

* **`event.target`:** Елементът, върху който **реално е възникнало събитието** (напр. `<span>` иконата вътре в бутона).
* **`event.currentTarget`:** Елементът, към който е **закачен самият `addEventListener`**.

---

## 4. Event Delegation (Делегиране на събития) — Златен шаблон

Вместо да закачате 100 отделни `click` слушателя върху 100 бутона в клавиатурата на калкулатор или списък със задачи (което хаби RAM и не работи за динамично добавени елементи):

* Закачате **само ЕДИН `addEventListener` върху общия родителски контейнер** (напр. `<div class="keypad">`).
* Използвате Bubbling фазата и метода **`event.target.closest("[data-action]")`**, за да разберете кой точно бутон е натиснат, дори ако потребителят е кликнал върху вложен `<span>` или `<svg>` вътре в бутона!

---

## 5. Съхранение на данни в браузъра: `localStorage`, `sessionStorage` и Cookies

| Характеристика | `localStorage` | `sessionStorage` | Cookies (Бисквитки) |
| :--- | :--- | :--- | :--- |
| **Капацитет** | ~5 MB – 10 MB на домейн | ~5 MB на домейн | ~4 KB на бисквитка |
| **Живот (Expiration)** | Постоянен (до ръчно изтриване) | До затваряне на текущия таб/прозорец | Според `Expires` / `Max-Age` |
| **Изпращане към сървъра** | Никога (само локално в JS) | Никога (само локално в JS) | **Автоматично с всяка HTTP заявка** |
| **Типове данни** | Само `string` (`JSON.stringify`) | Само `string` (`JSON.stringify`) | Само `string` |
| **Достъпност** | Всички табове на същия Origin | Само текущия таб | Всички табове (ако не е `HttpOnly`) |

---

## 6. Скрити капани и Edge Cases

1. **XSS уязвимост с `innerHTML`:** Никога не поставяйте потребителски вход чрез `element.innerHTML = userInput`, защото зловреден `<img src=x onerror=alert(1)>` ще изпълни произволен JavaScript! Използвайте **`textContent`** за чист текст.
2. **Кликане върху вложен елемент при Event Delegation:** Ако бутонът съдържа `<button data-key="7"><span>7</span></button>`, при клик върху цифрата `event.target` е `<span>`, а не `<button>`. Винаги използвайте `const btn = event.target.closest("button")` вместо `event.target.matches("button")`.
3. **Съхраняване на чувствителни токени в `localStorage`:** `localStorage` е достъпен за всеки JavaScript скрипт на страницата (вкл. компрометирани външни библиотеки). За автентикационни JWT токени в продукция се предпочитат **`HttpOnly; Secure; SameSite=Strict` Cookies**.

---

[⬅️ 02-04-01. Event Loop & Asynchronous JS — Syntax](02-04-01-Syntax-Event-Loop-And-Asynchronous-JS.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-05-01. DOM, Events & Browser Storage — Syntax](02-05-01-Syntax-DOM-Events-And-Browser-Storage.md)

© 2026 Dev-Library / 02-05-00. DOM Manipulation, Event Delegation & Web Storage API
