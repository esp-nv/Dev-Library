[⬅️ 04-03. Инженерен Интервю Справочник](04-03-Advanced-Responsive-Traps-And-Snippets.md) | 📂 Папка: 04-Responsive-Web-Design | [🏠 Начало](../../README.md)

---

# 04-04. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 04:** Адаптивен дизайн | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за адаптивни оразмерители, мобилни команди и готов за употреба софтуерен UI шаблон за Mobile-First Grid.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Стандартни Контролни Точки (Breakpoints)](#1-global-reference-guide-стандартни-контролни-точки-breakpoints)
* [2. RESPONSIVE CHEAT SHEET (Бърз синтактичен пищов)](#2-responsive-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI SNIPPET: "Адаптивно Студентско Портфолио (Mobile-First Grid)"](#3-готов-ui-snippet-адаптивно-студентско-портфолио-mobile-first-grid)

---

## 1. GLOBAL REFERENCE GUIDE: Стандартни Контролни Точки (Breakpoints)

### 🎛️ А. Основни хардуерни граници в индустрията:
*   `max-width: 576px` / `min-width: 576px` ➔ Мобилни телефони в хоризонтален режим.
*   `max-width: 768px` / `min-width: 768px` ➔ Таблети (в портретен режим).
*   `max-width: 992px` / `min-width: 992px` ➔ Лаптопи и малки десктоп екрани.
*   `max-width: 1200px` / `min-width: 1200px` ➔ Големи десктоп монитори.

### 🗛 Б. Логически уеб оператори:
*   `only screen and` ➔ Ограничава изпълнението само до дигитални екрани (игнорира принтери).
*   `and` ➔ Обединява условия (напр. интервал между `min-width` и `max-width`).
*   `,` (Запетая) ➔ Действа като логическо ИЛИ (OR) — прилага се, ако поне един обхват е верен.

---

## 2. RESPONSIVE CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `<meta name="viewport" content="width=device-width, initial-scale=1.0">` ➔ **Абсолютен уеб закон №1** за отключване на мобилното оразмеряване и медийните заявки.
*   `img { max-width: 100%; height: auto; display: block; }` ➔ **Златната формула за адаптивни снимки** — свиват се гъвкаво на малки екрани, но никога не се разпъват над реалния си размер.
*   `@media (pointer: coarse)` ➔ Модерна медийна заявка, която таргетира **единствено устройства с тъчскрийн екрани** (смартфони/таблети), за бързо уголемяване на бутони за по-лесно натискане с пръст.

---

## 3. ГОТОВ UI SNIPPET: "АДАПТИВНО СТУДЕНТСКО ПОРТФОЛИО (MOBILE-FIRST GRID)"

Копирай този оригинален HTML и CSS код директно във твоя проект. Той изгражда модерно уеб табло, което на телефон подрежда елементите вертикално, а на компютър се разгръща автоматично в балансирана триколона по законите на възходящата **Mobile-First каскада**:

```html
<!-- Готов адаптивен Full-Stack компонент -->
<div class="interview-responsive-wrapper">
    <div class="adaptive-grid-system">
        <div class="ui-feature-card">
            <h4>Бекенд Архитектура</h4>
            <p>Пълен одит на Kestrel нишките през затворени софтуерни слоеве.</p>
        </div>
        <div class="ui-feature-card">
            <h4>Бази Данни</h4>
            <p>ACID съвместимост на трансакциите в шардирани SQL клъстери.</p>
        </div>
        <div class="ui-feature-card">
            <h4>Фронтенд Дизайн</h4>
            <p>Utility-first оптимизация на компонентите без излишна памет.</p>
        </div>
    </div>
</div>

<style>
    /* 1. БАЗОВ CSS (Оптимизиран директно за Мобилни телефони - Mobile-First) */
    .interview-responsive-wrapper *, 
    .interview-responsive-wrapper *::before, 
    .interview-responsive-wrapper *::after {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }
    .interview-responsive-wrapper {
        width: 100%;
        padding: 16px;
        background-color: #f1f5f9;
        font-family: sans-serif;
    }
    .adaptive-grid-system {
        display: flex;
        flex-direction: column; /* 📱 На телефон картите са в чист вертикален стек */
        gap: 16px;
    }
    .ui-feature-card {
        background-color: #ffffff;
        border: 1px solid #cbd5e1;
        border-radius: 8px;
        padding: 20px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.02);
    }
    .ui-feature-card h4 { color: #1e293b; margin-bottom: 6px; font-size: 18px; }
    .ui-feature-card p { color: #64748b; font-size: 14px; line-height: 1.5; }

    /* 2. 💻 ДЕКСКТОП КАСКАДА (Задейства се само на екрани над 768px - Възходящ ред) */
    @media screen and (min-width: 768px) {
        .adaptive-grid-system {
            flex-direction: row; /* Обръща картите в перфектна хоризонтална редица */
        }
        .ui-feature-card {
            flex: 1; /* Разпределя пространството на три равни колони автоматично */
        }
    }
</style>
```

---
[⬅️ 04-03. Инженерен Интервю Справочник](04-03-Advanced-Responsive-Traps-And-Snippets.md) | 📂 Папка: 04-Responsive-Web-Design | [🏠 Начало](../../README.md)
