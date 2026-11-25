[⬅️ 01-02-00. CSS Box Model и Типография](01-02-00-CSS-Box-Model-And-Typography.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🏠 Начало 01-Theory-Notes](../../README.md)| [📂 Следваща папка: 02. Responsive, Forms & Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-03-00. CSS Layout — Flexbox Архитектура](01-03-00-CSS-Layout-Flexbox.md)

---

# 01-02-01. Syntax-From-Scratch & UI Card Snippets (CSS Box Model и Типография)

**Раздел 01:** HTML5 & CSS3 Foundations | **Цел:** Практическо прилагане на `box-sizing: border-box` от нулата, центриране на блокови елементи, типографски настройки, вход/изход изчисления и готов за копиране компонент за Продуктова карта.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Задължителният CSS Reset шаблон](#1-стъпка-по-стъпка-задължителният-css-reset-шаблон)
* [2. Синтаксис от нулата: Сравнение content-box срещу border-box (Вход / Изход)](#2-синтаксис-от-нулата-сравнение-content-box-срещу-border-box-вход--изход)
* [3. Module Cheat Sheet (Бърз Изпитен Пищов)](#3-module-cheat-sheet-бърз-изпитен-пищов)
* [4. Готов за директно ползване Snippet: Професионална Продуктова Карта (Product Card)](#4-готов-за-директно-ползване-snippet-професионална-продуктова-карта-product-card)
* [5. Системни капани: Центриране с margin: 0 auto без ширина и inline елементи](#5-системни-капани-центриране-с-margin-0-auto-без-ширина-и-inline-елементи)
* [6. Детайлни подмодули към Глава 02](#6-детайлни-подмодули-към-глава-02)

---

## 1. СТЪПКА ПО СТЪПКА: ЗАДЪЛЖИТЕЛНИЯТ CSS RESET ШАБЛОН

Всеки професионален `.css` файл започва с тези 5 реда код в самото начало (ред 1), за да премахне различните фабрични отстояния на Chrome, Firefox и Safari:

```css
*, *::before, *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
}
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: СРАВНЕНИЕ CONTENT-BOX СРЕЩУ BORDER-BOX (ВХОД / ИЗХОД)

### 🔹 Пример А: Как се променя физическата ширина в браузъра
* **Вход (`style.css`):**
  ```css
  .box-default {
      box-sizing: content-box; /* Фабрично поведение */
      width: 200px;
      padding: 20px;
      border: 5px solid #333;
  }

  .box-modern {
      box-sizing: border-box;  /* Златен стандарт */
      width: 200px;
      padding: 20px;
      border: 5px solid #10b981;
  }
  ```
* **Изход в Chrome DevTools (`Computed` таб):**
  * `.box-default` ➔ Заема реални **`250px`** ширина (`200 + 40 + 10`) и често чупи оформлението.
  * `.box-modern` ➔ Заема точно **`200px`** ширина (вътрешното съдържание се свива до `150px`, за да побере рамката и отстоянията).

### 🔹 Пример Б: Съкратен запис (Shorthand) на Margin и Padding
* **Вход:**
  ```css
  .card {
      padding: 10px 20px;        /* 10px горе/долу | 20px ляво/дясно */
      margin: 15px 10px 25px 5px; /* горе | дясно | долу | ляво (по часовниковата стрелка) */
  }
  ```
* **Изход:** Спестява 6 реда излишен код и намалява размера на CSS файла.

---

## 3. MODULE CHEAT SHEET (БЪРЗ ИЗПИТЕН ПИЩОВ)

| Свойство / Трикове | Синтаксис | Изпитно приложение |
| :--- | :--- | :--- |
| **Хоризонтално центриране на блок** | `margin: 0 auto;` | Златен закон за центриране на контейнер (изисква дефиниран `width`). |
| **Удебеляване на текст** | `font-weight: bold;` (или `700`) | Прави шрифта плътен и акцентно четим. |
| **Главни букви (Капитализация)** | `text-transform: uppercase;` | Автоматично превръща целия текст в ГЛАВНИ БУКВИ без промяна в HTML. |
| **Защита от дълги думи/линкове** | `word-wrap: break-word;` | Пренася насилствено дълги URL адреси на нов ред, за да не излизат от кутията. |
| **Оптимално междуредие** | `line-height: 1.5;` | Осигурява четимо вертикално разстояние (1.5 пъти размера на шрифта). |
| **Заобляне на ъгли** | `border-radius: 8px;` | Заобля ръбовете на кутия (`50%` прави перфектен кръг при квадратен блок). |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: ПРОФЕСИОНАЛНА ПРОДУКТОВА КАРТА (PRODUCT CARD)

Копирай този готов HTML и CSS блок директно в твоето уеб приложение, за да имаш напълно изградена, застрахована от счупвания през `border-box` продуктова визитка с изчистена типография:

```html
<!-- Готов продуктов компонент с изчистен Box Model -->
<div class="product-card-container">
    <div class="product-box">
        <div class="product-badge">Ново</div>
        <h3 class="product-title">Слушалки PRO v2</h3>
        <p class="product-desc">Професионални студийни уеб аудио слушалки с дълбока честотна изолация на паметта и меки кожени кутии.</p>
        <div class="product-price">189.00 лв.</div>
    </div>
</div>

<style>
    /* Глобална защита на размерите */
    .product-card-container *, 
    .product-card-container *::before, 
    .product-card-container *::after {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }

    .product-card-container {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        padding: 20px;
    }

    .product-box {
        width: 320px;
        border: 1px solid #e2e8f0;
        border-radius: 8px;
        padding: 24px; /* Вътрешно разстояние */
        background-color: #ffffff;
        position: relative;
        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }

    .product-badge {
        position: absolute;
        top: 12px;
        right: 12px;
        background-color: #ef4444;
        color: white;
        font-size: 11px;
        font-weight: bold;
        text-transform: uppercase;
        padding: 4px 8px;
        border-radius: 4px;
    }

    .product-title {
        color: #1e293b;
        font-size: 20px;
        font-weight: 600;
        margin-bottom: 8px;
    }

    .product-desc {
        color: #64748b;
        font-size: 14px;
        line-height: 1.5; /* Оптимално междуредие */
        margin-bottom: 16px;
        word-wrap: break-word; /* Предпазва от препълване */
    }

    .product-price {
        color: #10b981;
        font-size: 22px;
        font-weight: bold;
    }
</style>
```

---

## 5. СИСТЕМНИ КАПАНИ: ЦЕНТРИРАНЕ С MARGIN: 0 AUTO БЕЗ ШИРИНА И INLINE ЕЛЕМЕНТИ

### 🚨 Капан №1: Пишеш `margin: 0 auto;`, но кутията не мърда от левия край
* **Какво правиш:** Искаш да центрираш един `<div>` или `<section>` в средата на екрана и му слагаш `margin: 0 auto;`, но той си остава залепен вляво.
* **Защо се случва:** По подразбиране блоковите елементи (`<div>`, `<section>`, `<p>`) заемат **100% от ширината** на родителя си. Когато един блок е широк 100%, няма свободно пространство отляво и отдясно, което `auto` да раздели поравно.
* **Как да го поправиш:** Винаги задавай конкретна ширина по-малка от 100% (напр. `width: 800px;` или `max-width: 1200px;`) заедно с `margin: 0 auto;`.

### 🚨 Капан №2: Опитваш се да зададеш `width`, `height` или горен `margin` на `<span>` или `<a>`
* **Защо се случва:** Тагове като `<span>`, `<a>`, `<strong>` и `<em>` са по подразбиране **`display: inline`** и игнорират `width`, `height`, `margin-top` и `margin-bottom`.
* **Как да го поправиш:** Добави `display: inline-block;` или `display: block;` върху елемента, за да започне да спазва правилата на Box Model.


---
[⬅️ 01-02-00. CSS Box Model и Типография](01-02-00-CSS-Box-Model-And-Typography.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🏠 Начало 01-Theory-Notes](../../README.md)| [📂 Следваща папка: 02. Responsive, Forms & Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-03-00. CSS Layout — Flexbox Архитектура](01-03-00-CSS-Layout-Flexbox.md)


© 2026 Dev-Library / 01-02-01. Syntax-From-Scratch & UI Card Snippets (CSS Box Model и Типография)
