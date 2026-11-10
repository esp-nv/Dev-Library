[⬅️ 06-01. Структура и Подредба](06-01-Core-Concepts-Layout-Flexbox-Grid.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | [🏠 Начало](../../README.md)

---

# 06-02. Справочник Категория 2: Декорации и Спецификации (Spacing, Sizing, Typography, Borders, Effects)

**Модул 06:** Напреднал Tailwind CSS v4 | **Цел:** Пълно софтуерно и практическо овладяване на декоративните и стилови класове на Tailwind v4. Налагане на прецизна типография, еластични кутийни размери, заоблени рамки и сенки, съчетано с индивидуален пищов и готови за употреба софтуерни компоненти.

## 🧭 Съдържание на темата
* [1. Spacing Справочник: Вътрешни и Външни отстояния (Padding & Margin)](#1-spacing-справочник-вътрешни-и-външни-отстояния-padding--margin)
* [2. Sizing Справочник: Широчина, Височина и Ограничители (Width & Height)](#2-sizing-справочник-широчина-височина-и-ограничители-width--height)
* [3. Typography Справочник: Шрифтове, Междуредия и Трансформации](#3-typography-справочник-шрифтове-междуредия-и-трансформации)
* [4. Borders & Effects Справочник: Рамки, Сенки (Box-Shadow) и Прозрачност](#4-borders--effects-справочник-рамки-сенки-box-shadow-и-прозрачност)
* [5. Системни капани: Невалидни пиксели при произволни размери](#5-системни-капани-невалидни-пиксели-при-произволни-размери)
* [6. MODULE CHEAT SHEET & ГОТОВ UI КОМПОНЕНТ ЗА ИЗПИТИ](#6-module-cheat-sheet--готов-ui-компонент-за-изпити)

---

## 1. SPACING СПРАВОЧНИК: ВЪТРЕШНИ И ВЪНШНИ ОТСТОЯНИЯ (PADDING & MARGIN)

Управлението на пространството около кутиите се осъществява чрез прецизни цифрово-съкратени класове. Всяка единица съответства точно на `0.25rem` (`4px` по подразбиране):
*   **Вътрешно отстояние (Padding):** `p-4` (всички страни, 16px), `px-6` (хоризонтално ляво/дясно), `py-2` (вертикално горе/долу), `pt-1` (само отгоре).
*   **Външно разстояние (Margin):** `m-8` (всички страни), `mx-auto` (автоматично центриране на блока), `mt-4` (отгоре), `-mb-2` (негативен марджин отдолу за застъпване на елементи).

---

## 2. SIZING СПРАВОЧНИК: ШИРОЧИНА, ВИСОЧИНА И ОГРАНИЧИТЕЛИ (WIDTH & HEIGHT)

Спецификация на официалните пропорции за заемане на екранно пространство:
*   **Широчина (Width):** `w-full` (100%), `w-1/2` (50%), `w-1/3` (33.333%), `w-12` (твърди 48px).
*   **Ограничители (Max Width):** `max-w-xs` (20rem), `max-w-md` (28rem), `max-w-xl` (36rem), `max-w-6xl` (72rem).
*   **Височина (Height):** `h-full` (100% от родителя), `h-screen` (100vh - заема целия хардуерен екран), `h-auto` (автоматично разпъване по съдържанието).

---

## 3. TYPOGRAPHY СПРАВОЧНИК: ШРИФТОВЕ, МЕЖДУРЕДИЯ И ТРАНСФОРМАЦИИ

За пълна застраховка на текстовете и четимостта пред изпитната Judge система:
*   **Размер на текста:** `text-xs` (12px), `text-sm` (14px), `text-base` (16px - дефолт), `text-lg` (18px), `text-2xl` (24px), `text-5xl` (48px).
*   **Дебелина (Weight):** `font-light` (300), `font-normal` (400), `font-semibold` (600), `font-bold` (700).
*   **Подравняване:** `text-left`, `text-center`, `text-right`, `text-justify`.
*   **Трансформации:** `uppercase` (големи букви), `lowercase` (малки букви), `capitalize` (първа голяма буква), `normal-case`.
*   **Интерлайн (Line Height):** `leading-none` (свито), `leading-relaxed` (комфортно разстояние), `leading-loose` (голямо междуредие).
*   **Оптимизация:** `font-smoothing` активира `-webkit-font-smoothing: antialiased;` за кристални пиксели.

---

## 4. BORDERS & EFFECTS СПРАВОЧНИК: РАМКИ, СЕНКИ (BOX-SHADOW) И ПРОЗРАЧНОСТ

Декоративни софтуерни инструменти за налагане на микро-интерактивност:
*   **Border Radius (Заобляне):** `rounded-none` (остри ръбове), `rounded-sm` (2px), `rounded-md` (6px), `rounded-xl` (12px), `rounded-full` (9999px - прави перфектен кръг).
*   **Border Width & Color:** `border` (1px рамка), `border-2` (2px), `border-slate-200`, `border-dashed` (пунктирана линия).
*   **Сенки (Box-Shadow):** `shadow-sm` (лека козметична сянка), `shadow` (базова), `shadow-md` (средна), `shadow-xl` (дълбока облачна сянка).
*   **Прозрачност (Opacity):** `opacity-100` (плътен цвят), `opacity-50` (50% прозрачност), `opacity-0` (напълно невидим за очите).

---

## 5. СТЪПКИ ЗА КОРИГИРАНЕ: НЕВАЛИДНИ ПИКСЕЛИ ПРИ ПРОИЗВОЛНИ РАЗМЕРИ

### 🚨 Капан №1: Срив на дизайна при изписване на ръчни, произволни пиксели извън скалата
*   **Какво правиш:** В условието на задачата се изисква специфичен марджин от `17px` или широчина `387px`. Ти се опитваш да го налучкаш през скалата и пишеш: `mt-17` или `w-387`.
*   **Визуален капан:** Tailwind напълно игнорира тези класове, не генерира никакъв CSS код за тях и елементът ти остава без никакво отстояние, проваляйки пикселния Judge одит.
*   **Защо се случва:** Скалата на Tailwind е фиксирана (напр. има `mt-4` за 16px и `mt-5` за 20px, но числото 17 не съществува в стандартната софтуерна таблица).
*   **Как да го поправиш (Изпитен спасителен закон):** Когато ти трябва изричен, твърд хардкоднат размер извън дефолтната скала, използвай **Квадратни скоби (Arbitrary Values)**. Те казват на компилатора в секундата на сканиране да генерира къстъм CSS свойство на мига:
    ```html
    <!-- ❌ АНТИ-ПАТЕРН (Няма да генерира нищо): -->
    <div class="mt-17 w-387"> ... </div>

    <!-- 🟢 ПРАВИЛЕН СТАНДАРТ (Arbitrary Syntax): -->
    <div class="mt-[17px] w-[387px]"> ... </div>
    ```

---

## 6. MODULE CHEAT SHEET & ГОТОВ UI КОМПОНЕНТ ЗА ИЗПИТИ

### 🛠️ Бърз Изпитен Пищов (Cheat Sheet):
*   `w-12 h-12 rounded-full` ➔ Индустриалният стандарт за **перфектна кръгла аватарна икона**.
*   `truncate` ➔ Изключително важен трик — ако текстът е твърде дълъг и застрашава да счупи кутията, този клас автоматично го отрязва накрая и инжектира безопасни три точки (`...`).
*   `border-collapse` ➔ Налага се над `<table>` елементи, за да премахне двойните грозни рамки.

### 💾 Готов за директно ползване Snippet: "Професионална Регистрационна Форма (Tailwind UI Form)"
Копирай този оригинален, напълно валидиран HTML код директно във твоето уеб приложение. Той изгражда модерна, защитена от изтичания форма за вход с изчистена типография, интерактивни ховър състояния и сигурни `name` атрибути за .NET средите:

```html
<!-- Изчистен, готов Tailwind v4 компонент за форми -->
<div class="w-full max-w-md bg-white border border-slate-200 rounded-xl p-8 shadow-sm font-sans mx-auto mt-10">
    
    <!-- Заглавен блок -->
    <div class="text-center mb-6">
        <h3 class="text-2xl font-bold text-slate-800">Вход в Профила</h3>
        <p class="text-sm text-slate-500 mt-1">Въведете вашите данни за софтуерен достъп</p>
    </div>

    <!-- HTML Форма, обвързана с .NET Model Binding среди -->
    <form action="/api/v1/auth/login" method="POST" class="space-y-5">
        
        <!-- Поле за Имейл -->
        <div class="flex flex-col space-y-1.5">
            <label for="InputEmail" class="text-sm font-bold text-slate-700 tracking-wide">Имейл адрес:</label>
            <input type="email" id="InputEmail" name="Email" required placeholder="student@softuni.bg" 
                   class="w-full px-4 py-2.5 border border-slate-300 rounded-lg text-sm text-slate-800 focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition-all font-smoothing" />
        </div>

        <!-- Поле за Парола -->
        <div class="flex flex-col space-y-1.5">
            <label for="InputPassword" class="text-sm font-bold text-slate-700 tracking-wide">Парола:</label>
            <input type="password" id="InputPassword" name="Password" required placeholder="••••••••" 
                   class="w-full px-4 py-2.5 border border-slate-300 rounded-lg text-sm text-slate-800 focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition-all" />
        </div>

        <!-- Опционален Чекбокс -->
        <div class="flex items-center space-x-2 pt-1">
            <input type="checkbox" id="RememberMe" name="RememberMe" value="true" 
                   class="w-4 h-4 text-blue-600 border-slate-300 rounded focus:ring-blue-500 cursor-pointer" />
            <label for="RememberMe" class="text-sm text-slate-600 select-none cursor-pointer">Запомни ме на това устройство</label>
        </div>

        <!-- Вграден Submit бутон с интерактивни ховър и активни състояния -->
        <button type="submit" 
                class="w-full bg-blue-600 hover:bg-blue-700 active:scale-[0.99] text-white font-bold text-sm py-3 px-4 rounded-lg shadow-sm transition-all duration-150 cursor-pointer text-center">
            Влез в системата
        </button>

    </form>
</div>
```
[⬅️ 06-01. Структура и Подредба](06-01-Core-Concepts-Layout-Flexbox-Grid.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | [🏠 Начало](../../README.md)

© 2026 Dev-Library / Справочник: Tailwind v4 Декорации и Интензитет