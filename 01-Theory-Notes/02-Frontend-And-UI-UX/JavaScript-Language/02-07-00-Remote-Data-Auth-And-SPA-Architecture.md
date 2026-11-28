[⬅️ 02-06. Browser JavaScript vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-07-01. API Requester, Auth & ES Modules — Syntax ...](02-07-01-Syntax-Remote-Data-Auth-And-SPA-Architecture.md)

---

# 🟨 02-07-00. Remote Data, Authentication & Modular SPA Architecture

## 📑 Съдържание
* [1. Multi-Page Applications (MPA) vs Single-Page Applications (SPA)](#1-multi-page-applications-mpa-vs-single-page-applications-spa)
* [2. Автентикация, Сесии и Оторизация (Token-Based Auth & Ownership)](#2-автентикация-сесии-и-оторизация-token-based-auth--ownership)
* [3. ES6 Modules (`import` / `export`) и Модулна капсулация](#3-es6-modules-import--export-и-модулна-капсулация)
* [4. Separation of Concerns (Разделяне на отговорностите) в SPA](#4-separation-of-concerns-разделяне-на-отговорностите-в-spa)
* [5. Скрити капани и Best Practices](#5-скрити-капани-и-best-practices)

---

## 1. Multi-Page Applications (MPA) vs Single-Page Applications (SPA)

| Критерий | Multi-Page Application (MPA) | Single-Page Application (SPA) |
| :--- | :--- | :--- |
| **Навигация** | Всяка страница зарежда нов `.html` файл от сървъра (пълен Reload) | Зарежда се **1 единствен `index.html`**; изгледите се сменят динамично чрез JS |
| **Комуникация** | Стандартни `<form>` заявки и сървърно рендиране | Асинхронни `fetch()` / AJAX заявки с **JSON** данни |
| **Потребителско изживяване (UX)** | Видимо премигване при всяка смяна на страница | Мигновени преходи, споделено състояние (State) и плавна интерактивност |
| **SEO и Първо зареждане** | Отлично SEO по подразбиране; по-лек начален JS | По-бавен първоначален старт; изисква SSR/SSG за оптимално SEO |

---

## 2. Автентикация, Сесии и Оторизация (Token-Based Auth & Ownership)

* **Authentication (Автентикация — *„Кой си ти?“*):** Процесът на потвърждаване на самоличността чрез потребителски данни (Email + Password). При успех сървърът връща подписан **Access Token**.
* **Authorization (Оторизация — *„Какво имаш право да правиш?“*):** Проверката дали текущият потребител има права да прочете, редактира или изтрие даден ресурс.
* **Жизнен цикъл на Token-Based Auth в SPA:**
  1. Клиентът изпраща `POST /users/login` (или `/register`) с JSON тяло.
  2. Сървърът връща `{ _id, email, accessToken }`.
  3. Клиентът записва сесийните данни в `sessionStorage`.
  4. При всяка следваща защитена заявка (`POST`, `PUT`, `DELETE`) клиентът прикача хедър **`X-Authorization: <accessToken>`** (или `Authorization: Bearer <token>`).
* **Data Ownership (`_ownerId`):** Всеки запис в базата пази идентификатора на своя създател (`_ownerId`). Скриването на бутоните `[Edit]` и `[Delete]` в UI служи **само за по-добър UX** — истинската сигурност се налага единствено на сървъра!

---

## 3. ES6 Modules (`import` / `export`) и Модулна капсулация

В големите JS приложения кодът се разделя на независими файлове (**ES Modules**), които:
* Имат **собствен модулен обхват** (не замърсяват глобалния `window`);
* Изпълняват се автоматично в **`strict mode`**;
* Изискват локален HTTP сървър (напр. `vite`, `live-server`) — браузърите блокират ES модули през `file://` протокол от съображения за сигурност (CORS).

```html
<!-- Атрибутът type="module" автоматично включва defer поведение -->
<script src="/src/app.js" type="module"></script>
```

---

## 4. Separation of Concerns (Разделяне на отговорностите) в SPA

Смесването на `fetch()` заявки, бизнес валидации и DOM манипулации в една функция води до **High Coupling (силна обвързаност)** и невъзможна поддръжка. Професионалната SPA структура разделя приложението на изолирани слоеве:

```text
┌─────────────────────────────────────────────────────────────────┐
│                    app.js (Entry Point / Router)                │
│         Инициализира модулите и свързва навигацията             │
└───────────────┬─────────────────────────────────┬───────────────┘
                │                                 │
                ▼                                 ▼
┌───────────────────────────────┐ ┌───────────────────────────────┐
│      views/ (Presentation)    │ │   api/ (Data & Auth Layer)    │
│ • homeView.js / catalogView.js│ │ • api.js (Generic requester)  │
│ • loginView.js / createView.js│ │ • authService.js / data.js    │
│ • Само UI рендиране и форми   │ │ • Само HTTP заявки и токени   │
└───────────────────────────────┘ └───────────────────────────────┘
```

---

## 5. Скрити капани и Best Practices

1. **Никога не пазете State в DOM дървото:** Не четете обратно цени или ID-та от `.textContent` на HTML елементи, за да изпратите заявка към сървъра — дръжте данните в JavaScript обекти/променливи.
2. **Не изпращайте директно целия `Object.fromEntries(formData)` без филтриране:** Злонамерен потребител може да добави скрито поле `<input name="isAdmin" value="true">` през DevTools. Винаги извличайте изрично само очакваните полета (`const { title, description, price } = data;`).
3. **Защита от двойно изпращане (Double Submit):** Деактивирайте бутона (`submitBtn.disabled = true`) по време на `await fetch(...)` и го възстановете във `finally` блока.







---

[⬅️ 02-06. Browser JavaScript vs Node.js Runtime Comparison](02-06-Browser-JS-vs-NodeJS-Runtime-Comparison.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-07-01. API Requester, Auth & ES Modules — Syntax ...](02-07-01-Syntax-Remote-Data-Auth-And-SPA-Architecture.md)

© 2026 Dev-Library / 02-07-00. Remote Data, Authentication & Modular SPA Architecture
