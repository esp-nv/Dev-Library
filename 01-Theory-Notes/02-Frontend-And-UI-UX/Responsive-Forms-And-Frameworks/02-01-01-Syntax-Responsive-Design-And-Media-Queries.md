[⬅️ 02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md) | [📂 Назад към 02. Responsive Design, Forms & CSS Frameworks Хъба](README.md) | [📦 Предишен модул: JavaScript-Language](../JavaScript-Language/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 02. Responsive Design, Forms & CSS Frameworks](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md)

---

# 02-01-01. Syntax-From-Scratch & Responsive Breakpoints Cheat Sheet

**Раздел 02:** Responsive Design, Forms & CSS Frameworks | **Цел:** Практическо изграждане на адаптивни оформления по подхода Mobile-First от нулата, използване на модерната CSS функция `clamp()`, табличен Cheat Sheet със стандартните брейкпойнти и готов адаптивен Grid компонент.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Mobile-First шаблон с Media Queries](#1-стъпка-по-стъпка-mobile-first-шаблон-с-media-queries)
* [2. Синтаксис от нулата: Fluid Typography с clamp() и Aspect Ratio (Вход / Изход)](#2-синтаксис-от-нулата-fluid-typography-с-clamp-и-aspect-ratio-вход--изход)
* [3. Module Cheat Sheet (Стандартизирани Breakpoints & Responsive единици)](#3-module-cheat-sheet-стандартизирани-breakpoints--responsive-единици)
* [4. Готов за директно ползване Snippet: Адаптивна мрежа от карти (Responsive Card Grid)](#4-готов-за-директно-ползване-snippet-адаптивна-мрежа-от-карти-responsive-card-grid)
* [5. Системни капани: Капанът 100vh на мобилни браузъри (iOS / Android Address Bar)](#5-системни-капани-капанът-100vh-на-мобилни-браузъри-ios--android-address-bar)

---

## 1. СТЪПКА ПО СТЪПКА: MOBILE-FIRST ШАБЛОН С MEDIA QUERIES

Започваме с базовите стилове за мобилни телефони (без `@media`), след което добавяме правила възходящо с `min-width`:

```css
/* 1. Базов стил: Мобилни телефони (0px - 767px) */
.container {
    width: 100%;
    padding: 0 16px;
    margin: 0 auto;
}

.grid-box {
    display: flex;
    flex-direction: column;
    gap: 16px;
}

/* 2. Таблети (от 768px нагоре) */
@media (min-width: 768px) {
    .container {
        max-width: 720px;
    }
    .grid-box {
        flex-direction: row;
        flex-wrap: wrap;
    }
}

/* 3. Десктоп монитори (от 1200px нагоре) */
@media (min-width: 1200px) {
    .container {
        max-width: 1140px;
    }
}
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: FLUID TYPOGRAPHY С CLAMP() И ASPECT RATIO (ВХОД / ИЗХОД)

### 🔹 Пример А: Плавна типография без десетки Media Queries (`clamp()`)
* **Вход (`style.css`):**
  ```css
  h1.hero-title {
      /* Минимум: 24px | Идеален размер: 4vw | Максимум: 48px */
      font-size: clamp(1.5rem, 4vw, 3rem);
  }
  ```
* **Изход в браузъра:** Заглавието се мащабира плавно пиксел по пиксел при промяна на ширината на прозореца, но никога не става по-малко от `24px` на телефон и никога не надвишава `48px` на голям монитор.

### 🔹 Пример Б: Запазване на пропорциите на видео/карти без изкривяване (`aspect-ratio`)
* **Вход:**
  ```css
  .video-wrapper {
      width: 100%;
      aspect-ratio: 16 / 9;
      background-color: #0f172a;
  }
  ```
* **Изход:** Контейнерът автоматично изчислява височината си спрямо текущата ширина в съотношение `16:9` на всяко устройство.

---

## 3. MODULE CHEAT SHEET (СТАНДАРТИЗИРАНИ BREAKPOINTS & RESPONSIVE ЕДИНИЦИ)

### 📱 Индустриални Breakpoints (Bootstrap / Tailwind стандарт)

| Устройство / Клас | Ширина на екрана | Media Query (Mobile-First) | Типична подредба |
| :--- | :--- | :--- | :--- |
| **Extra Small (Mobile)** | `< 576px` | Базов CSS (без `@media`) | 1 колона (100% ширина), вертикално меню. |
| **Small (`sm`)** | `>= 576px` | `@media (min-width: 576px)` | Големи телефони (Landscape), 1–2 колони. |
| **Medium (`md`)** | `>= 768px` | `@media (min-width: 768px)` | Таблети (iPad), 2 колони, хоризонтален хедър. |
| **Large (`lg`)** | `>= 992px` | `@media (min-width: 992px)` | Малки лаптопи, 3 колони, странична лента (Sidebar). |
| **Extra Large (`xl`)** | `>= 1200px` | `@media (min-width: 1200px)` | Стандартни монитори, 4 колони, `max-width: 1140px`. |
| **2XL (`2xl`)** | `>= 1400px` | `@media (min-width: 1400px)` | Широкоекранни монитори, `max-width: 1320px`. |

### 📏 Справка за адаптивни мерни единици

| Единица | Спрямо какво се изчислява? | Кога се използва? |
| :--- | :--- | :--- |
| `rem` | Шрифта на `<html>` (по подразбиране `16px`) | Шрифтове (`font-size`), вътрешни/външни отстояния (`padding`, `margin`). |
| `em` | Шрифта на текущия/родителския елемент | Мащабиране вътре в конкретен бутон или значка (Badge). |
| `vw` / `vh` | `1%` от ширината / височината на прозореца | Пълноекранни Hero секции и плавна типография (`clamp()`). |
| `dvh` | Dynamic Viewport Height (без лентата на браузъра) | Пълноекранни секции на мобилни телефони (iOS Safari / Chrome). |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: АДАПТИВНА МРЕЖА ОТ КАРТИ (RESPONSIVE CARD GRID)

Копирай този готов код за автоматична решетка, която показва **1 колона на телефон**, **2 колони на таблет** и **3 колони на десктоп** без нито един счупен пиксел:

```html
<section class="responsive-wrapper">
    <div class="responsive-grid">
        <article class="grid-card"><h3>Модул 01</h3><p>HTML5 и CSS3 основи.</p></article>
        <article class="grid-card"><h3>Модул 02</h3><p>Responsive и Media Queries.</p></article>
        <article class="grid-card"><h3>Модул 03</h3><p>SEO и Redis кеширане.</p></article>
    </div>
</section>

<style>
    .responsive-wrapper * {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }

    .responsive-wrapper {
        width: 100%;
        max-width: 1140px;
        margin: 0 auto;
        padding: 20px;
        font-family: 'Segoe UI', sans-serif;
    }

    /* Mobile-First: 1 колона по подразбиране */
    .responsive-grid {
        display: grid;
        grid-template-columns: 1fr;
        gap: 20px;
    }

    /* Tablet: 2 колони от 768px нагоре */
    @media (min-width: 768px) {
        .responsive-grid {
            grid-template-columns: repeat(2, 1fr);
        }
    }

    /* Desktop: 3 колони от 992px нагоре */
    @media (min-width: 992px) {
        .responsive-grid {
            grid-template-columns: repeat(3, 1fr);
        }
    }

    .grid-card {
        background: #f8fafc;
        border: 1px solid #cbd5e1;
        border-radius: 8px;
        padding: 24px;
    }
</style>
```

---

## 5. СИСТЕМНИ КАПАНИ: КАПАНЪТ 100VH НА МОБИЛНИ БРАУЗЪРИ (IOS / ANDROID ADDRESS BAR)

### 🚨 Капан №1: Слагаш `height: 100vh;` на началния екран, но долният бутон остава скрит на телефон
* **Защо се случва:** Мобилните браузъри (Safari на iPhone и Chrome на Android) изчисляват `100vh` спрямо целия екран *без* да вадят височината на горната адресна лента и долната навигационна лента. Така секцията става по-висока от видимата площ.
* **Как да го поправиш:** Използвай модерната CSS единица **`min-height: 100dvh;`** (Dynamic Viewport Height), която автоматично се преизчислява при скриване и показване на лентите на телефона.

---
[⬅️ 02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md) | [⬅️ Назад към 01. HTML5 & CSS3 Foundations](../HTML5-CSS3-And-SASS/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: Responsive-Forms-And-Frameworks](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../TypeScript-Language/README.md) | [➡️ 02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md)






---

[⬅️ 02-01-00. Адаптивен уеб дизайн и Media Queries](02-01-00-Responsive-Design-And-Media-Queries.md) | [📂 Назад към 02. Responsive Design, Forms & CSS Frameworks Хъба](README.md) | [📦 Предишен модул: JavaScript-Language](../JavaScript-Language/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 02. Responsive Design, Forms & CSS Frameworks](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md)

© 2026 Dev-Library / 02-01-01. Syntax-From-Scratch & Responsive Breakpoints Cheat Sheet
