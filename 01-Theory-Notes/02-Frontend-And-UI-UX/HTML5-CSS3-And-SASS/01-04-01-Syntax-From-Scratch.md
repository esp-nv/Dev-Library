[⬅️ 01-04-00. Sass и SCSS Архитектура](01-04-00-Sass-And-SCSS-Architecture.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🏠 Начало 01-Theory-Notes](../../README.md)| [📂 Следваща папка: 02. Responsive, Forms & Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-05. Сравнение на CSS стиловете и рамките](01-05-CSS-Methodologies-And-Frameworks-Comparison.md)

---

# 01-04-01. Syntax-From-Scratch & SCSS Cheat Sheet (Променливи, Mixins и Цикли)

**Раздел 01:** HTML5 & CSS3 Foundations | **Цел:** Практическо писане на модерен SCSS код от нулата — дефиниране на променливи (`$`), влагане със селектор `&`, преизползваеми `@mixin` шаблони, генериране на класове с `@each`, вход/изход превод към стандартен CSS и табличен **Module Cheat Sheet**.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Инсталация и компилация на SCSS файл](#1-стъпка-по-стъпка-инсталация-и-компилация-на-scss-файл)
* [2. Синтаксис от нулата: Nesting, Mixins и @each цикли (Вход / Изход)](#2-синтаксис-от-нулата-nesting-mixins-и-each-цикли-вход--изход)
* [3. Module Cheat Sheet (Пълна SCSS синтактична справка)](#3-module-cheat-sheet-пълна-scss-синтактична-справка)
* [4. Готов за директно ползване Snippet: Модулен SCSS бутон с цветови варианти](#4-готов-за-директно-ползване-snippet-модулен-scss-бутон-с-цветови-варианти)
* [5. Системни капани: Разлика между SCSS променливи ($var) и CSS Custom Properties (--var)](#5-системни-капани-разлика-между-scss-променливи-var-и-css-custom-properties---var)
* [6. Детайлни подмодули към Тема 01-04](#6-детайлни-подмодули-към-тема-01-04)

---

## 1. СТЪПКА ПО СТЪПКА: ИНСТАЛАЦИЯ И КОМПИЛАЦИЯ НА SCSS ФАЙЛ

Можеш да компилираш `.scss` файлове по два начина — чрез разширението **Live Sass Compiler** във VS Code (бутон *Watch Sass* в долната лента) или чрез официалния терминален инструмент **Sass**:

```bash
# 1. Глобална инсталация на Dart Sass
npm install -g sass

# 2. Еднократна компилация с минифициране за продукция
sass scss/main.scss css/style.min.css --style=compressed

# 3. Режим на наблюдение (автоматично прекомпилира при всеки запис)
sass --watch scss/main.scss:css/style.css
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: NESTING, MIXINS И @EACH ЦИКЛИ (ВХОД / ИЗХОД)

### 🔹 Пример А: Влагане (Nesting) с родителския селектор `&`
* **Вход (`_buttons.scss`):**
  ```scss
  \$primary: #2563eb;

  .btn {
      padding: 10px 20px;
      background-color: \$primary;
      color: #fff;

      &:hover {
          background-color: darken(\$primary, 10%);
      }

      &--outline {
          background-color: transparent;
          border: 2px solid \$primary;
          color: \$primary;
      }
  }
  ```
* **Изход (Компилиран `style.css`):**
  ```css
  .btn { padding: 10px 20px; background-color: #2563eb; color: #fff; }
  .btn:hover { background-color: #1649b8; }
  .btn--outline { background-color: transparent; border: 2px solid #2563eb; color: #2563eb; }
  ```

### 🔹 Пример Б: Параметризиран `@mixin` за Flexbox центриране
* **Вход (`_mixins.scss`):**
  ```scss
  @mixin flex-center(\(direction: row,\)gap: 16px) {
      display: flex;
      flex-direction: \$direction;
      justify-content: center;
      align-items: center;
      gap: \$gap;
  }

  .hero-box {
      @include flex-center(column, 24px);
  }
  ```
* **Изход (Компилиран `style.css`):**
  ```css
  .hero-box {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      gap: 24px;
  }
  ```

---

## 3. MODULE CHEAT SHEET (ПЪЛНА SCSS СИНТАКТИЧНА СПРАВКА)

| Инструмент / Директива | Синтаксис | Предназначение и Действие |
| :--- | :--- | :--- |
| **Променлива (`$`)** | `$font-main: 'Inter', sans-serif;` | Съхранява цветове, шрифтове и размери (замества се по време на компилация). |
| **Родителски селектор (`&`)** | `&:hover`, `&::before`, `&--large` | Вмъква името на родителския селектор без излишен интервал. |
| **Зареждане на модул (`@use`)** | `@use 'variables' as v;` ➔ `v.$primary` | Включва Partial файл в изолиран namespace без глобални конфликти. |
| **Препращане (`@forward`)** | `@forward 'mixins';` | Изгражда централен `_index.scss` файл за споделяне на модули. |
| **Mixin (`@mixin` / `@include`)** | `@mixin box($p) { padding: $p; }` | Дефинира преизползваем блок от CSS правила с параметри по подразбиране. |
| **Функция (`@function`)** | `@function rem($px) { @return ($px / 16) * 1rem; }` | Изчислява и връща конкретна стойност (напр. конвертиране от `px` в `rem`). |
| **Maps (Речници)** | `$colors: ("ok": #10b981, "err": #ef4444);` | Съхранява двойки ключ-стойност, достъпни чрез `map.get($colors, "ok")`. |
| **Цикъл (`@each`)** | `@each $name, $hex in $colors { .bg-#{$name} { background: $hex; } }` | Автоматично генерира повтарящи се Utility класове от Map или списък. |
| **Условие (`@if` / `@else`)** | `@if $theme == dark { ... } @else { ... }` | Условно генериране на стилове според подаден параметър в Mixin. |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: МОДУЛЕН SCSS БУТОН С ЦВЕТОВИ ВАРИАНТИ

Копирай този готов SCSS фрагмент, който автоматично генерира 4 семантични варианта на бутони (`.btn-primary`, `.btn-success`, `.btn-warning`, `.btn-danger`) с помощта на `@each` и интерполация `#{$name}`:

```scss
@use 'sass:color';

\$theme-colors: (
    'primary': #2563eb,
    'success': #10b981,
    'warning': #f59e0b,
    'danger':  #ef4444
);

.btn {
    display: inline-block;
    padding: 10px 20px;
    border: none;
    border-radius: 6px;
    font-weight: 600;
    color: #ffffff;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

@each \$name, \(hex in\)theme-colors {
    .btn-#{\$name} {
        background-color: \$hex;

        &:hover {
            background-color: color.adjust(\(hex,\)lightness: -8%);
        }
    }
}
```

---

## 5. СИСТЕМНИ КАПАНИ: РАЗЛИКА МЕЖДУ SCSS ПРОМЕНЛИВИ ($VAR) И CSS CUSTOM PROPERTIES (--VAR)

### 🚨 Капан №1: Опитваш се да смениш SCSS променлива (`$bg-color`) в реално време при Dark Mode или през JavaScript
* **Защо не работи:** SCSS променливите (`$bg-color`) съществуват **само по време на компилация (Build Time)**. В крайния `.css` файл те вече са заменени с твърда стойност (напр. `#ffffff`) и браузърът изобщо не знае за тях.
* **Решение:** Използвай SCSS променливи за статични константи (Breakpoints, размери на шрифтове), а за динамични теми (Light / Dark Mode), които се сменят в браузъра в реално време (Runtime), използвай **CSS Custom Properties (`--bg-color: #ffffff;`)**.

---

## 6. ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ТЕМА 01-04

* **[01-05. Пълно сравнение на CSS стиловете, методологиите и рамките](01-05-CSS-Methodologies-And-Frameworks-Comparison.md)** — Детайлен архитектурен и хардуерен одит в таблица по категории: Pure CSS, SCSS, Tailwind CSS, Bootstrap 5 и CSS-in-JS.

---
[⬅️ 01-04-00. Sass и SCSS Архитектура](01-04-00-Sass-And-SCSS-Architecture.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🏠 Начало 01-Theory-Notes](../../README.md)| [📂 Следваща папка: 02. Responsive, Forms & Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-05. Сравнение на CSS стиловете и рамките](01-05-CSS-Methodologies-And-Frameworks-Comparison.md)


© 2026 Dev-Library / 01-04-01. Syntax-From-Scratch & SCSS Cheat Sheet (Променливи, Mixins и Цикли)
