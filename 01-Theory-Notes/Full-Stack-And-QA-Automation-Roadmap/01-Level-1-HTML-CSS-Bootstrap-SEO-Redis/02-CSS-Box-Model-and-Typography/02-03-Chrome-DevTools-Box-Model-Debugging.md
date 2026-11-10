[⬅️ 02-02. Уеб Типография](02-02-Web-Typography-And-Fonts.md) | 📂 Папка: 02-Box-Model-And-Typography | [🏠 Начало](../../README.md)

---

# 02-03. Браузърен одит: Дебъгване на кутии през Computed и Styles прозорците

**Модул 02:** Кутиен модел | **Цел:** Паметен одит на уеб елементите в реално време, улавяне на каскадни конфликти през Chrome DevTools, финални Cheat Sheets и готови производствени Snippets.

## 🧭 Съдържание на темата
* [1. Дебъгване през Styles и Computed панели на DevTools](#1-дебъгване-през-styles-и-computed-панели-на-devtools)
* [2. CHAPTER CHEAT SHEET (Бърз изпитен пищов)](#2-chapter-cheat-sheet-бърз-изпитен-пищов)
* [3. ГОТОВ UI SNIPPET: "Професионална Продуктова Визитка (Product Card)"](#3-готов-ui-snippet-професионална-продуктова-визитка-product-card)

---

## 1. ДЕБЪГВАНЕ ПРЕЗ STYLES И COMPUTED ПАНЕЛИ НА DEVTOOLS

Когато даден елемент се размести, натискаш клавиша `F12` за старт на DevTools:
*   **Прозорецът Styles:** Покава суровия CSS, подаден от теб. Можеш динамично да триеш и дописваш пиксели в движение, за да намериш фикса, без да рестартираш сайта.
*   **Прозорецът Computed:** Показва голяма цветна графична схема на Box Model-а. Вътре виждаш точните финални числа на Margin, Border и Padding, сметнати от браузъра.

---

## 2. CHAPTER CHEAT SHEET (БЪРЗ ИЗПИТЕН ПИЩОВ)

*   `margin: 0 auto;` ➔ Абсолютен уеб закон за **перфектно хоризонтално центриране на голяма родителска кутия** на уеб екрана (изисква елементът да има дефиниран `width`).
*   `line-height: 1.6;` ➔ Оптималното софтуерно междуредие за висока четимост.
*   `-webkit-font-smoothing: antialiased;` ➔ Изглажда пикселите на буквите в Chrome, премахвайки размазването.

---

## 3. ГОТОВ UI SNIPPET: "ПРОФЕСИОНАЛНА ПРОДУКТОВА ВИЗИТКА (PRODUCT CARD)"

Копирай този готов, застрахован от счупвания през `border-box` уеб компонент с изчистена типография направо в твоя проект:

```html
<div class="product-card-blueprint">
    <div class="card-inner">
        <h3 class="card-title">Слушалки Studio PRO</h3>
        <p class="card-text">Високопроизводителни уеб аудио кутии с дълбока честотна изолация и меки кожени падинг уплътнения.</p>
        <span class="card-price">149.00 лв.</span>
    </div>
</div>

<style>
    /* Глобален изпитен Reset буфер */
    .product-card-blueprint *, 
    .product-card-blueprint *::before, 
    .product-card-blueprint *::after {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }
    .card-inner {
        width: 300px;
        border: 1px solid #cbd5e1;
        border-radius: 8px;
        padding: 24px;
        background-color: #ffffff;
        box-shadow: 0 4px 6px rgba(0,0,0,0.05);
        font-family: sans-serif;
    }
    .card-title {
        color: #0f172a;
        font-size: 20px;
        margin-bottom: 8px;
    }
    .card-text {
        color: #475569;
        font-size: 14px;
        line-height: 1.6;
        margin-bottom: 16px;
    }
    .card-price {
        color: #10b981;
        font-size: 22px;
        font-weight: bold;
    }
</style>
```

---
[⬅️ 02-02. Уеб Типография](02-02-Web-Typography-And-Fonts.md) | 📂 Папка: 02-Box-Model-And-Typography | [🏠 Начало](../../README.md)

© 2026 Dev-Library / Изпитен Кутиен Атлас и Снипети
