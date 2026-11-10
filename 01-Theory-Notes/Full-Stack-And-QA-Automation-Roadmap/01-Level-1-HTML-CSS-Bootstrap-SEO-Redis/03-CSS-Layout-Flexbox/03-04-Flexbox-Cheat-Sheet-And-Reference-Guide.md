[⬅️ 03-03. Инженерен Интервю Справочник](03-03-Advanced-Flexbox-Traps-And-Snippets.md) | 📂 Папка: 03-CSS-Layout-Flexbox | [🏠 Начало](../../README.md)

---

# 03-04. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 03:** Flexbox среди | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за свойствата на контейнера и децата във Flexbox среди, придружена от готови копируеми UI софтуерни компоненти.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Свойства на Родителя и Децата](#1-global-reference-guide-свойства-на-родителя-и-децата)
* [2. FLEXBOX CHEAT SHEET (Бърз синтактичен пищов)](#2-flexbox-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI SNIPPET: "Адаптивна Навигационна Лента (Flex Navbar)"](#3-готов-ui-snippet-адаптивна-навигационна-лента-flex-navbar)

---

## 1. GLOBAL REFERENCE GUIDE: Свойства на Родителя и Децата

### 🗛 А. Свойства за контрол над Родителя (Flex Container):
*   `display: flex;` ➔ Активира гъвкавия контекст на форматиране.
*   `flex-direction: row | column | row-reverse | column-reverse;` ➔ Определя ориентацията и посоката на Главната ос.
*   `flex-wrap: nowrap | wrap | wrap-reverse;` ➔ Разрешава или забранява автоматичното преминаване на нов ред.
*   `justify-content: flex-start | flex-end | center | space-between | space-around | space-evenly;` ➔ Подравнява елементите по Главната ос (Main Axis).
*   `align-items: stretch | center | flex-start | flex-end | baseline;` ➔ Подравнява елементите на единичен ред по Напречната ос (Cross Axis).
*   `align-content: stretch | center | space-between | space-around;` ➔ Управлява разстоянието между редовете (само при многоредов флекс поток).

### 🗚 Б. Свойства за контрол над Децата (Flex Items):
*   `flex-basis` ➔ Начален уеб размер на кутията преди скейлинг и разпределение на свободна памет.
*   `flex-grow` ➔ Коефициент на растеж при излишно празно пространство (дефолт: 0).
*   `flex-shrink` ➔ Коефициент на свиване при недостиг на място (дефолт: 1).
*   `order` ➔ Сменя софтуерния ред на изчертаване на елемента на екрана, без да променя HTML кода.

---

## 2. FLEXBOX CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `display: flex; justify-content: center; align-items: center;` ➔ **Абсолютният софтуерен закон за перфектно хоризонтално и вертикално центриране** на елемент в кутия.
*   `margin-left: auto;` ➔ Наложено над последното дете във Flex редица, браузърът автоматично го изтласка в най-десния край на екрана, отделяйки го от останалите (идеално за бутони "Изход").
*   `gap: 20px;` ➔ Налага чисто разстояние между редовете и колоните едновременно, елиминирайки нуждата от ръчни margins.

---

## 3. ГОТОВ UI SNIPPET: "АДАПТИВНА НАВИГАЦИОННА ЛЕНТА (FLEX NAVBAR)"

Копирай този готов HTML и CSS фрагмент директно за твоите проекти:

```html
<!-- Изчистен, готов Flexbox компонент за навигация -->
<header class="main-header-hub">
    <div class="logo-area">SoftUni Project</div>
    <nav class="nav-links-area">
        <a href="#">Начало</a>
        <a href="#">Портфолио</a>
        <a href="#">Контакти</a>
    </nav>
</header>

<style>
    .main-header-hub *, .main-header-hub *::before, .main-header-hub *::after {
        box-sizing: border-box;
        margin: 0; padding: 0;
    }
    .main-header-hub {
        font-family: Arial, sans-serif;
        background-color: #1e293b;
        padding: 16px 32px;
        display: flex;
        align-items: center;
        justify-content: space-between; /* Избутва логото вляво, линковете вдясно */
    }
    .logo-area { color: #f1f5f9; font-size: 22px; font-weight: bold; }
    .nav-links-area { display: flex; gap: 24px; }
    .nav-links-area a { color: #cbd5e1; text-decoration: none; font-size: 16px; transition: color 0.2s; }
    .nav-links-area a:hover { color: #38bdf8; }
</style>
```

---
[⬅️ 03-03. Инженерун Интервю Справочник](03-03-Advanced-Flexbox-Traps-And-Snippets.md) | 📂 Папка: 03-CSS-Layout-Flexbox | [🏠 Начало](../../README.md)
