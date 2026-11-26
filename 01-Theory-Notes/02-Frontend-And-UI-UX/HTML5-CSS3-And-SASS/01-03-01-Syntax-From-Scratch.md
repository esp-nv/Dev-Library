[⬅️ 01-03-00. CSS Layout — Flexbox Архитектура](01-03-00-CSS-Layout-Flexbox.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: Responsive-Forms-And-Frameworks](Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-04-00. Sass и SCSS Архитектура](01-04-00-Sass-And-SCSS-Architecture.md)

---

# 01-03-01. Syntax-From-Scratch & Flexbox Cheat Sheet (Адаптивни подредби и Navbar Snippets)

**Раздел 01:** HTML5 & CSS3 Foundations | **Цел:** Практическо изграждане на Flexbox оформления от нулата, перфектно центриране, пълен Cheat Sheet на контейнерните и елементните свойства, вход/изход примери и готов за копиране компонент за адаптивна навигация.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Активиране на Flexbox и перфектно центриране](#1-стъпка-по-стъпка-активиране-на-flexbox-и-перфектно-центриране)
* [2. Синтаксис от нулата: Flex Grow, Order и Gap (Вход / Изход)](#2-синтаксис-от-нулата-flex-grow-order-и-gap-вход--изход)
* [3. Module Cheat Sheet (Пълна Flexbox синтактична справка)](#3-module-cheat-sheet-пълна-flexbox-синтактична-справка)
* [4. Готов за директно ползване Snippet: Адаптивна Навигационна Лента (Flex Navbar)](#4-готов-за-директно-ползване-snippet-адаптивна-навигационна-лента-flex-navbar)
* [5. Системни капани: Смяна на осите при flex-direction: column](#5-системни-капани-смяна-на-осите-при-flex-direction-column)
* [6. Детайлни подмодули към Глава 03](#6-детайлни-подмодули-към-глава-03)

---

## 1. СТЪПКА ПО СТЪПКА: АКТИВИРАНЕ НА FLEXBOX И ПЕРФЕКТНО ЦЕНТРИРАНЕ

За да подредиш елементи с Flexbox, винаги поставяш `display: flex;` върху **родителския контейнер**, а не върху самите елементи:

```css
/* Абсолютният закон за перфектно центриране по X и Y: */
.hero-center-box {
    display: flex;
    justify-content: center; /* Центрира по главната ос (хоризонтално) */
    align-items: center;     /* Центрира по напречната ос (вертикално) */
    height: 300px;
}
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: FLEX GROW, ORDER И GAP (ВХОД / ИЗХОД)

### 🔹 Пример А: Гъвкава колона с `flex: 1` (Sticky Footer / Main Content)
* **Вход (`index.html` + `style.css`):**
  ```html
  <div class="layout-row">
      <aside class="sidebar">Меню (250px)</aside>
      <main class="content">Основно съдържание (Заема остатъка)</main>
  </div>
  ```
  ```css
  .layout-row {
      display: flex;
      gap: 20px;
  }
  .sidebar {
      flex: 0 0 250px; /* Не расте, не се свива, точна ширина 250px */
  }
  .content {
      flex: 1;         /* Поема цялото останало свободно пространство */
  }
  ```
* **Изход в браузъра:** Страничното меню стои фиксирано на точно `250px`, а основното съдържание се разпъва автоматично до десния край на екрана независимо от резолюцията.

---

## 3. MODULE CHEAT SHEET (ПЪЛНА FLEXBOX СИНТАКТИЧНА СПРАВКА)

### 📦 Свойства за Родителя (Flex Container)

| Свойство | Възможни стойности | Какво прави? |
| :--- | :--- | :--- |
| `display` | `flex` \| `inline-flex` | Активира Flexbox контекст за директните деца. |
| `flex-direction` | `row` \| `row-reverse` \| `column` \| `column-reverse` | Определя посоката на главната ос (хоризонтално или вертикално). |
| `justify-content` | `flex-start` \| `flex-end` \| `center` \| `space-between` \| `space-around` \| `space-evenly` | Подравнява елементите по **Главната ос**. |
| `align-items` | `stretch` \| `flex-start` \| `flex-end` \| `center` \| `baseline` | Подравнява елементите по **Напречната ос**. |
| `flex-wrap` | `nowrap` \| `wrap` \| `wrap-reverse` | Позволява пренасяне на елементите на нов ред при недостиг на място. |
| `gap` | `16px` \| `10px 20px` (row-gap column-gap) | Задава чисто разстояние само *между* кутиите без външни отстъпи. |

### 🧩 Свойства за Децата (Flex Items)

| Свойство | Синтаксис / Стойности | Какво прави? |
| :--- | :--- | :--- |
| `flex` (Shorthand) | `flex: 1;` (`grow: 1, shrink: 1, basis: 0%`) | Накарва елемента да заеме цялото останало свободно пространство. |
| `flex-basis` | `250px` \| `30%` \| `auto` | Задава началния размер на елемента преди разпределяне на остатъка. |
| `align-self` | `auto` \| `flex-start` \| `center` \| `flex-end` | Презаписва `align-items` на родителя само за едно конкретно дете. |
| `order` | `-1` \| `0` (по подразбиране) \| `1` | Променя визуалната поредност на елемента без редакция на HTML. |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: АДАПТИВНА НАВИГАЦИОННА ЛЕНТА (FLEX NAVBAR)

Копирай този готов HTML и CSS фрагмент, за да имаш модерно, гъвкаво навигационно уеб меню, което автоматично изтласква логото отляво, а линковете за достъп — в десния край на екрана:

```html
<!-- Готов Flexbox компонент за навигация -->
<header class="main-header-hub">
    <div class="logo-area">SoftUni Project</div>
    <nav class="nav-links-area">
        <a href="#">Начало</a>
        <a href="#">Портфолио</a>
        <a href="#">Контакти</a>
    </nav>
</header>

<style>
    /* Нулиране и изолация на компонента */
    .main-header-hub,
    .main-header-hub *,
    .main-header-hub *::before,
    .main-header-hub *::after {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }

    .main-header-hub {
        font-family: Arial, sans-serif;
        background-color: #1e293b;
        padding: 16px 32px;
        
        /* Активираме Flexbox */
        display: flex;
        align-items: center; /* Центрира елементите вертикално */
        justify-content: space-between; /* Избутва логото вляво, линковете вдясно */
    }

    .logo-area {
        color: #f1f5f9;
        font-size: 22px;
        font-weight: bold;
    }

    .nav-links-area {
        display: flex;
        gap: 24px; /* Чисто разстояние между уеб линковете */
    }

    .nav-links-area a {
        color: #cbd5e1;
        text-decoration: none;
        font-size: 16px;
        transition: color 0.2s ease-in-out;
    }

    .nav-links-area a:hover {
        color: #38bdf8; /* Промяна на цвета при посочване */
    }
</style>
```

---

## 5. СИСТЕМНИ КАПАНИ: СМЯНА НА ОСИТЕ ПРИ FLEX-DIRECTION: COLUMN

### 🚨 Капан №1: Слагаш `flex-direction: column;`, но `justify-content: center` спира да центрира хоризонтално
* **Защо се случва:** Когато смениш `flex-direction` от `row` на `column`, **двете оси се завъртат на 90 градуса**! Главната ос (`justify-content`) вече контролира **вертикалното** подравняване, а напречната ос (`align-items`) контролира **хоризонталното** подравняване.
* **Как да го поправиш:** При `flex-direction: column;` използвай `align-items: center;` за хоризонтално центриране и `justify-content: center;` за вертикално.


---
[⬅️ 01-03-00. CSS Layout — Flexbox Архитектура](01-03-00-CSS-Layout-Flexbox.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: Responsive-Forms-And-Frameworks](Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-04-00. Sass и SCSS Архитектура](01-04-00-Sass-And-SCSS-Architecture.md)


© 2026 Dev-Library / 01-03-01. Syntax-From-Scratch & Flexbox Cheat Sheet (Адаптивни подредби и Navbar Snippets)
