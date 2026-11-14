[⬅️ Назад към 02. Responsive, Forms & Frameworks](README.md) | [📂 Предишна папка: 01. HTML5 & CSS3 Foundations](../01-HTML-And-CSS-Foundations/README.md) | [📂 Текуща папка: 02. Responsive, Forms & Frameworks](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 03. Technical SEO & Redis Caching](../03-SEO-And-Redis-Caching/README.md) | [➡️ 02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md)

---

# 02-00. Intro to Responsive Design & UI Frameworks

**Раздел 02:** Responsive Design, Forms & CSS Frameworks | **Цел:** Разбиране на принципите за изграждане на адаптивни интерфейси за всякакви екрани (Mobile, Tablet, Desktop), безопасно събиране на потребителски данни чрез HTML5 форми и избор между Custom CSS, **Tailwind CSS** и **Bootstrap 5**.

## 🧭 Съдържание на темата
* [1. Какво обхваща Раздел 02 и защо е критичен?](#1-какво-обхваща-раздел-02-и-защо-е-критичен)
* [2. Архитектурен избор: Custom CSS срещу Tailwind CSS срещу Bootstrap 5](#2-архитектурен-избор-custom-css-срещу-tailwind-css-срещу-bootstrap-5)
* [3. Пътна карта на темите в Раздел 02](#3-пътна-карта-на-темите-в-раздел-02)
* [4. Детайлни подмодули към Глава 02](#4-детайлни-подмодули-към-глава-02)

---

## 1. КАКВО ОБХВАЩА РАЗДЕЛ 02 И ЗАЩО Е КРИТИЧЕН?

Над 60% от уеб трафика днес идва от мобилни устройства, а почти всяко уеб приложение събира данни от потребителя чрез форми за вход, регистрация или поръчка. В този раздел преминаваме от фиксирани десктоп макети към **еластични, индустриални интерфейси**:

* **Адаптивност на всяко устройство:** Овладяване на `viewport` конфигурацията и `@media` заявките по подхода **Mobile-First**.
* **Входна точка към Бекенда (HTML Forms):** Правилно свързване на `<form>`, `<input>`, `name` атрибути и вградена HTML5 валидация преди данните да стигнат до сървъра.
* **Ускоряване на разработката чрез Frameworks:** Използване на готови дизайн системи (**Tailwind CSS** за пълна визуална свобода и **Bootstrap 5** за светкавични прототипи и админ панели).

---

## 2. АРХИТЕКТУРЕН ИЗБОР: CUSTOM CSS СРЕЩУ TAILWIND CSS СРЕЩУ BOOTSTRAP 5

```text
 ┌──────────────────────┬──────────────────────────┬─────────────────────────────┐
 │ Подход / Рамка       │ Философия                │ Кога се избира?             │
 ├──────────────────────┼──────────────────────────┼─────────────────────────────┤
 │ Pure CSS + Media Q.  │ Пълен ръчен контрол      │ Уникални арт сайтове,       │
 │                      │ (Zero Dependencies)      │ изпити в SoftUni Judge      │
 ├──────────────────────┼──────────────────────────┼─────────────────────────────┤
 │ Tailwind CSS         │ Utility-First (Атомарни  │ Модерни SPA (React, Vue),   │
 │                      │ класове в HTML)          │ персонализиран бранд дизайн │
 ├──────────────────────┼──────────────────────────┼─────────────────────────────┤
 │ Bootstrap 5          │ Component-First (Готови  │ Админ панели, вътрешни      │
 │                      │ бутони, модали, 12-Grid) │ системи, бързи MVP прототипи│
 └──────────────────────┴──────────────────────────┴─────────────────────────────┘
```

---

## 3. ПЪТНА КАРТА НА ТЕМИТЕ В РАЗДЕЛ 02

Разделът е структуриран в четири последователни лекционни двойки (`-00 Теория` и `-01 Практика & Module Cheat Sheet`):

1. **Тема 02-01: Адаптивен уеб дизайн и Media Queries**
   * Ролята на `<meta name="viewport">`, стандартни Breakpoints (`576px`, `768px`, `992px`, `1200px`), разликата между `min-width` (Mobile-First) и `max-width` (Desktop-First) и относителни единици (`rem`, `em`, `vw`, `vh`).
2. **Тема 02-02: Работа с HTML форми и клиентска валидация**
   * Анатомия на `<form action="..." method="POST">`, разликата между `GET` и `POST`, свързване на `<label for="...">` с `id` и `name` атрибути, HTML5 атрибути за валидация (`required`, `pattern`, `minlength`) и псевдо-класове `:valid` / `:invalid`.
3. **Тема 02-03: Tailwind CSS (Utility-First Архитектура)**
   * Защо атомарните класове (`flex`, `p-4`, `text-slate-800`, `hover:bg-blue-600`) елиминират проблема с измислянето на имена на класове и мъртвия CSS код, конфигурация и responsive префикси (`sm:`, `md:`, `lg:`).
4. **Тема 02-04: Bootstrap 5 (12-Колонна Grid система и Компоненти)**
   * Строгата йерархия `.container` ➔ `.row` ➔ `.col-md-6`, отзивчиви класове, готови UI компоненти (`Navbar`, `Card`, `Modal`, `Alerts`) и как да презаписваме стиловете без `!important` хакове.

---

## 4. ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ГЛАВА 02

* **[02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md)** — Viewport механика, Mobile-First стратегия и капани при фиксирани пиксели.
* **[02-01-01. Syntax-From-Scratch & Responsive Breakpoints Cheat Sheet](02-01-01-Syntax-From-Scratch.md)** — Media Queries от нулата, табличен Cheat Sheet и адаптивен Grid компонент.
* **[02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md)** — HTTP методи, семантика на полетата, достъпност и капани при липсващ `name` атрибут.
* **[02-02-01. Syntax-From-Scratch & Validated Form Snippets + Cheat Sheet](02-02-01-Syntax-From-Scratch.md)** — Пълна регистрационна форма от нулата, справка за Input типове и стилизиране на валидация.
* **[02-03-00. Tailwind CSS (Utility-First Архитектура)](02-03-00-Tailwind-CSS-Utility-First.md)** — Архитектура на Utility-First, JIT компилация и капани с динамични имена на класове.
* **[02-03-01. Syntax-From-Scratch & Tailwind Utility Cheat Sheet](02-03-01-Syntax-From-Scratch.md)** — Настройка, таблица с най-използваните класове и готови Tailwind компоненти.
* **[02-04-00. Bootstrap 5 (12-Колонна Grid система и Компоненти)](02-04-00-Bootstrap-5-Grid-And-Components.md)** — 12-колонна мрежа, Flexbox основа и капани при счупена Grid йерархия.
* **[02-04-01. Syntax-From-Scratch & Bootstrap 5 Grid Templates + Cheat Sheet](02-04-01-Syntax-From-Scratch.md)** — CDN интеграция, Grid шаблони и справка за класовете на Bootstrap 5.

---
[⬅️ Назад към 02. Responsive, Forms & Frameworks](README.md) | [📂 Предишна папка: 01. HTML5 & CSS3 Foundations](../01-HTML-And-CSS-Foundations/README.md) | [📂 Текуща папка: 02. Responsive, Forms & Frameworks](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 03. Technical SEO & Redis Caching](../03-SEO-And-Redis-Caching/README.md) | [➡️ 02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md)
---
© 2026 Dev-Library / 02-00. Intro to Responsive Design & UI Frameworks
