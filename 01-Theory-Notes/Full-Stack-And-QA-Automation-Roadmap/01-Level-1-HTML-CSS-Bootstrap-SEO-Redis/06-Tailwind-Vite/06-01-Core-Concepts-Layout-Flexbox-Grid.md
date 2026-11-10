[⬅️ 06-00. Инсталация и Конфигурация](06-00-Tailwind-Vite-Installation-Guide.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | [🏠 Начало](../../README.md) | [➡️ 06-02. Декорации и Спецификации](06-02-Spacing-Sizing-Typography-Borders-Effects.md)

---

# 06-01. Справочник Категория 1: Структура, Концепции и Подредба (Layout, Flexbox & Grid)

**Модул 06:** Напреднал Tailwind CSS v4 | **Цел:** Пълно софтуерно и практическо овладяване на основните концепции за utility класове, улавяне на интерактивни състояния, налагане на Mobile-First адаптивност и изграждане на еластични мрежи без писане на суров CSS код.

## 🧭 Съдържание на темата
* [1. Core Concepts: Стилизиране с Utility класове и Състояния (:hover, :focus)](#1-core-concepts-стилизиране-с-utility-класове-и-състояния-hover-focus)
* [2. Responsive Design: Мобилна адаптивност по модела Mobile-First](#2-responsive-design-мобилна-адаптивност-по-модела-mobile-first)
* [3. Layout Справочник: Box-Sizing, Display, Position и Object-Fit](#3-layout-справочник-box-sizing-display-position-и-object-fit)
* [4. Flexbox & Grid Справочник: Подравняване, Оси и Gap Мениджмънт](#4-flexbox--grid-справочник-подравняване-оси-и-gap-мениджмънт)
* [5. Системни капани: Изкривяване на мрежи при липсващ конвенционален клас](#5-системни-капани-изкривяване-на-мрежи-при-липсващ-конвенционален-клас)
* [6. TAILWIND LAYOUT CHEAT SHEET & ГОТОВ UI SNIPPET](#6-tailwind-layout-cheat-sheet--готов-ui-snippet)

---

## 1. CORE CONCEPTS: СТИЛИЗИРАНЕ С UTILITY КЛАСОВЕ И СЪСТАЯНИЯ (:HOVER, :FOCUS)

В Tailwind v4 нямаш готови компоненти тип "копче" или "карта". Всеки дизайн се сглобява чрез комбиниране на малки, атомарни класове. Интерактивните псевдо-класове се налагат чрез специфични софтуерни представки (`Modifiers`), разделени с двоеточие:
*   `hover:bg-blue-600` ➔ Променя цвета на фона при посочване с мишката.
*   `focus:ring-2 focus:ring-blue-500` ➔ Налага синя рамка (окел) около елемента, когато потребителят кликне вътре в него (активен фокус).
*   `active:scale-95` ➔ Свива леко бутона визуално по време на реалното му натискане, симулирайки хардуерна реакция.

---

## 2. RESPONSIVE DESIGN: МОБИЛНА АДАПТИВНОСТ ПО МОДЕЛА MOBILE-FIRST

Tailwind работи строго по модела **Mobile-First**. Всички базови класове (без представки) важат за мобилни телефони. Адаптивните модификатори добавят правила за по-големи екрани, стъпвайки на вградените контролни точки (Breakpoints):
*   `sm:` (над 640px) ➔ Таблети в портретен режим.
*   `md:` (над 768px) ➔ Големи таблети и хоризонтални екрани.
*   `lg:` (над 1024px) ➔ Десктоп компютри и лаптопи.
*   `xl:` (над 1280px) ➔ Широкоекранни монитори.

```html
<!-- 📱 Телефон: колона (flex-col) | 💻 Компютър (md): ред (flex-row) -->
<div class="flex flex-col md:flex-row gap-4"> ... </div>
```

---

## 3. LAYOUT СПРАВОЧНИК: BOX-SIZING, DISPLAY, POSITION И OBJECT-FIT

Спецификация на официалните Tailwind класове за управление на кутийния поток:
*   **Box-Sizing:** `box-border` налага `box-sizing: border-box;` (вграден по дефолт в Tailwind Preflight), а `box-content` връща стандартния модел.
*   **Display:** `block`, `inline`, `inline-block`, `flex`, `grid`, `hidden` (`display: none;`).
*   **Position:** `static`, `relative`, `absolute`, `fixed`, `sticky`.
*   **Координати:** `top-0`, `right-4`, `bottom-2`, `left-1/2` (left: 50%).
*   **Z-Index:** `z-0`, `z-10`, `z-50`, `z-auto`.
*   **Object-Fit:** `object-cover` (пропорционално изрязване на снимки), `object-contain` (вместване без разпъване).

---

## 4. FLEXBOX & GRID СПРАВОЧНИК: ПОДРАВНЯВАНЕ, ОСИ И GAP МЕНИДЖМЪНТ

Управлението на едноизмерни и двуизмерни уеб решетки през Tailwind v4 спестява стотици редове суров код:

### А. Мощни Flexbox Клетки:
*   `flex-row` / `flex-col` ➔ Определя посоката на Главната ос (Хоризонтална или Вертикална).
*   `flex-wrap` ➔ Разрешава на децата да преминават на нов ред при смаляване на екрана.
*   `justify-start`, `justify-end`, `justify-center`, `justify-between` ➔ Подравнява по Главната ос.
*   `items-start`, `items-center`, `items-end`, `items-stretch` ➔ Подравнява по Напречната ос.
*   `flex-grow` / `flex-shrink` ➔ Контролира еластичното разпъване на децата.

### Б. Професионални Grid Решетки:
*   `grid` ➔ Активира двуизмерен мрежови контекст (`display: grid;`).
*   `grid-cols-3` ➔ Създава автоматично 3 колони с напълно еднакви размери.
*   `col-span-2` ➔ Налага на конкретно дете да се разпъне насила и да заеме пространството на 2 колони едновременно.
*   `gap-4` ➔ Комбинирано пикселно разстояние (`1rem`) между всички редове и колони.

---

## 5. СИСТЕМНИ КАПАНИ: ИЗКРИВЯВАНЕ НА МРЕЖИ ПРИ ЛИПСВАЩ КОНВЕНЦИОНАЛЕН КЛАС

### 🚨 Капан №1: Атрибутът `grid-cols-X` се чупи и рендерира грешна единична колона
*   **Какво правиш:** Искаш да направиш решетка и пишеш: `<div class="grid-cols-3 gap-4">`.
*   **Визуален капан:** Всички елементи се подреждат вертикално един под друг в една грозна колона, напълно игнорирайки числото 3.
*   **Защо се случва:** Ти си пропуснала да дефинираш базовия задвижващ клас **`grid`**. Класът `grid-cols-3` дефинира единствено структурата на колоните, но той е софтуерно безпомощен, ако браузърът не знае, че контейнерът работи като `display: grid;`.
*   **Как да го поправиш:** Винаги заковавайте двата класа заедно:
    ```html
    <!-- ❌ АНТИ-ПАТЕРН: -->
    <div class="grid-cols-3"> ... </div>

    <!-- 🟢 ПРАВИЛЕН СТАНДАРТ: -->
    <div class="grid grid-cols-3 gap-4"> ... </div>
    ```

---

## 6. TAILWIND LAYOUT CHEAT SHEET & ГОТОВ UI SNIPPET

### 🛠️ Бърз Изпитен Пищов (Cheat Sheet):
*   `flex items-center justify-center` ➔ Златната формула за перфектно вертикално и хоризонтално центриране.
*   `hidden md:block` ➔ Изключително полезен изпитен трик — елементът е **напълно скрит на мобилен телефон**, но се появява автоматично като твърда кутия на таблети и лаптопи.
*   `inset-0` ➔ Съкращение за `top: 0; right: 0; bottom: 0; left: 0;` (задължително при изграждане на absolute/fixed фонови слоеве).

### 💾 Готов за директно ползване Snippet: "Адаптивно Административно Табло (Responsive Dashboard Grid)"
Копирай този оригинален HTML блок директно във твоя Vite проект. Той изгражда модерно уеб табло, което на телефон подрежда елементите вертикално, а на компютър се разгъва автоматично в балансирана Grid мрежа с разпънати карти:

```html
<!-- Готов за употреба Tailwind v4 Layout компонент -->
<main class="w-full min-h-screen bg-slate-50 p-6 font-sans">
    
    <!-- Заглавна секция -->
    <header class="mb-6 flex flex-col md:flex-row md:items-center md:justify-between gap-4 border-b border-slate-200 pb-4">
        <div>
            <h1 class="text-2xl font-bold text-slate-800">Архитектурно Табло</h1>
            <p class="text-sm text-slate-500">Системен одит на бекенд нишките в реално време</p>
        </div>
        <button class="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-4 py-2 rounded-lg shadow-sm transition-colors cursor-pointer">
            + Нова заявка
        </button>
    </header>

    <!-- Глобална Адаптивна Grid Решетка -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        
        <!-- Карта 1: Разпъната на компютър (col-span) -->
        <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm md:col-span-2 lg:col-span-2 relative overflow-hidden">
            <span class="absolute top-3 right-3 bg-blue-50 text-blue-600 text-xs font-bold px-2.5 py-1 rounded-full">Kestrel</span>
            <h3 class="text-lg font-bold text-slate-800 mb-2">Мрежов Трафик</h3>
            <p class="text-sm text-slate-600 line-height-relaxed">Входящият поток от уеб заявки преминава през Nginx балансьор и се разпределя хоризонтално без загуба на сесийна памет.</p>
        </div>

        <!-- Карта 2: Странична бърза статистика -->
        <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm flex flex-col justify-between">
            <div>
                <h3 class="text-lg font-bold text-slate-800 mb-1">База Данни</h3>
                <p class="text-xs text-slate-400 uppercase font-semibold tracking-wider mb-3">SQL Server Shards</p>
                <p class="text-sm text-slate-600">Всички трансакции са напълно оптимизирани и разпределени по алгоритъма за Модулен Хеш.</p>
            </div>
            <div class="text-xl font-bold text-emerald-600 mt-4">100% Консистентност</div>
        </div>

        <!-- Карта 3: Малка информационна кутия -->
        <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm">
            <h4 class="font-bold text-slate-800 mb-2">Redis Кеш Хит</h4>
            <div class="w-full bg-slate-100 rounded-full h-2.5 mt-2">
                <div class="bg-emerald-500 h-2.5 rounded-full" style="width: 85%"></div>
            </div>
            <span class="text-xs text-slate-500 block mt-2">85% от заявките се връщат от RAM паметта</span>
        </div>

    </div>
</main>
```

⬅️ [06-00. Инсталация и Конфигурация](06-00-Tailwind-Vite-Installation-Guide.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | 🏠 Начало | ➡️ [06-02. Декорации и Спецификации](06-02-Spacing-Sizing-Typography-Borders-Effects.md)
---
© 2026 Dev-Library / Справочник: Tailwind v4 Core Layout & Grid