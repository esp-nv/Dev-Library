[⬅️ 02-03-00. Tailwind CSS (Utility-First Архитектура)](02-03-00-Tailwind-CSS-Utility-First.md) | [⬅️ Назад към 01. HTML5 & CSS3 Foundations](../HTML5-CSS3-And-SASS/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: Responsive-Forms-And-Frameworks](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../TypeScript-Language/README.md) | [➡️ 02-04-00. Bootstrap 5 (12-Колонна Grid система и Компоненти)](02-04-00-Bootstrap-5-Grid-And-Components.md)

---

# 02-03-01. Syntax-From-Scratch & Tailwind Utility Cheat Sheet

**Раздел 02:** Responsive Design, Forms & CSS Frameworks | **Цел:** Практическо стартиране с Tailwind CSS от нулата (Play CDN и NPM CLI), овладяване на 4-пикселната математическа скала, вход/изход превод от стандартен CSS към Tailwind класове, пълен табличен **Module Cheat Sheet** и готови за копиране UI компоненти.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Бърз старт с Tailwind CSS (Play CDN и CLI)](#1-стъпка-по-стъпка-бърз-старт-с-tailwind-css-play-cdn-и-cli)
* [2. Синтаксис от нулата: Превод от Pure CSS към Tailwind (Вход / Изход)](#2-синтаксис-от-нулата-превод-от-pure-css-към-tailwind-вход--изход)
* [3. Module Cheat Sheet (Пълна Tailwind CSS синтактична справка)](#3-module-cheat-sheet-пълна-tailwind-css-синтактична-справка)
* [4. Готов за директно ползване Snippet: Адаптивна Профилна Карта с Tailwind](#4-готов-за-директно-ползване-snippet-адаптивна-профилна-карта-с-tailwind)
* [5. Системни капани: Премахване на фабричните стилове на заглавия (Preflight Reset)](#5-системни-капани-премахване-на-фабричните-стилове-на-заглавия-preflight-reset)


---

## 1. СТЪПКА ПО СТЪПКА: БЪРЗ СТАРТ С TAILWIND CSS (PLAY CDN И CLI)

### Вариант А: Моментално тестване и прототипиране (Play CDN)
Добави официалния скрипт в `<head>` секцията на `index.html` и започни да пишеш класове веднага без инсталация:

```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Tailwind Бърз Старт</title>
    <script src="https://tailwindcss.com"></script>
</head>
<body class="bg-slate-100 text-slate-800 p-8">
    <h1 class="text-3xl font-bold text-blue-600">Здравей, Tailwind CSS!</h1>
</body>
</html>
```

### Вариант Б: Продукционна инсталация през NPM CLI
```bash
npm install -D tailwindcss
npx tailwindcss init
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: ПРЕВОД ОТ PURE CSS КЪМ TAILWIND (ВХОД / ИЗХОД)

В Tailwind скалата за отстояния и размери следва формулата **`1 единица = 0.25rem = 4px`** (например `p-4` = `16px`, `m-6` = `24px`, `w-10` = `40px`).

### 🔹 Пример А: Центриран Flex хедър с отстояния
* **Вход (Стандартен CSS):**
  ```css
  .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 32px;
      background-color: #0f172a;
      color: #ffffff;
  }
  ```
* **Изход (Еквивалент в Tailwind HTML):**
  ```html
  <header class="flex justify-between items-center py-4 px-8 bg-slate-900 text-white">
      <!-- Съдържание -->
  </header>
  ```

### 🔹 Пример Б: Интерактивен бутон с анимация и фокус
* **Вход (Tailwind класове):**
  ```html
  <button class="px-5 py-2.5 bg-indigo-600 text-white font-semibold rounded-lg shadow hover:bg-indigo-700 focus:ring-2 focus:ring-indigo-400 transition">
      Запази промените
  </button>
  ```
* **Изход в браузъра:** Син бутон със заоблени ъгли (`8px`), лека сянка, плавно потъмняване при посочване с мишката и светещ контур при фокус с клавиатурата.

---

## 3. MODULE CHEAT SHEET (ПЪЛНА TAILWIND CSS СИНТАКТИЧНА СПРАВКА)

| Категория | Tailwind Клас | CSS Еквивалент | Пояснение |
| :--- | :--- | :--- | :--- |
| **Layout & Flexbox** | `flex` / `inline-flex` / `grid` | `display: flex;` / `grid;` | Активира Flex или Grid контейнер. |
| **Flex Подравняване** | `justify-between` / `items-center` | `justify-content: space-between;` | Подравняване по главна и напречна ос. |
| **Grid Колони** | `grid-cols-1 md:grid-cols-3 gap-6` | `grid-template-columns: repeat(3, 1fr); gap: 24px;` | 1 колона на телефон, 3 на таблет/десктоп. |
| **Padding & Margin** | `p-4` / `px-6` / `py-2` / `mt-8` / `mx-auto` | `padding: 16px;` / `margin: 0 auto;` | `x` = ляво+дясно, `y` = горе+долу, `t/b/l/r` = посока. |
| **Размери (Width/Height)** | `w-full` / `max-w-4xl` / `h-screen` / `min-h-dvh` | `width: 100%;` / `max-width: 896px;` | `w-1/2` = `50%`, `w-64` = `256px`. |
| **Типография** | `text-sm` / `text-xl` / `font-bold` / `text-center` | `font-size: 14px;` / `font-weight: 700;` | Размери от `text-xs` (`12px`) до `text-6xl` (`60px`). |
| **Цветове & Фон** | `bg-slate-900` / `text-emerald-500` | `background-color` / `color` | Нюанси от `50` (най-светъл) до `950` (най-тъмен). |
| **Рамки & Сенки** | `border border-gray-300 rounded-xl shadow-md` | `border: 1px solid ...; border-radius: 12px;` | Готови степени на заобляне и дълбочина на сянката. |
| **Модификатори** | `hover:` / `focus:` / `sm:` / `md:` / `lg:` / `dark:` | Псевдо-класове и `@media` заявки | Комбинират се (напр. `md:hover:scale-105`). |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: АДАПТИВНА ПРОФИЛНА КАРТА С TAILWIND

Копирай този готов, напълно адаптивен компонент (вертикален на мобилен телефон, хоризонтален на таблет и десктоп):

```html
<div class="max-w-xl mx-auto my-6 bg-white rounded-xl shadow-md overflow-hidden border border-slate-200">
    <div class="md:flex">
        <div class="md:shrink-0 bg-indigo-600 md:w-44 h-36 md:h-auto flex items-center justify-center text-white font-bold text-2xl">
            DEV
        </div>
        <div class="p-6">
            <div class="uppercase tracking-wide text-xs text-indigo-600 font-bold">Full-Stack & QA Engineer</div>
            <h3 class="mt-1 text-lg font-semibold text-slate-900">Архитектура с Tailwind CSS</h3>
            <p class="mt-2 text-sm text-slate-600 leading-relaxed">
                Компонентът използва Mobile-First префикса <code class="bg-slate-100 px-1 rounded">md:flex</code>, за да превключи автоматично от 1 колона към хоризонтален изглед.
            </p>
            <div class="mt-4 flex gap-3">
                <button class="px-4 py-2 bg-indigo-600 text-white text-sm font-medium rounded-lg hover:bg-indigo-700 transition">
                    Преглед
                </button>
                <button class="px-4 py-2 border border-slate-300 text-slate-700 text-sm font-medium rounded-lg hover:bg-slate-50 transition">
                    Документация
                </button>
            </div>
        </div>
    </div>
</div>
```

---

## 5. СИСТЕМНИ КАПАНИ: ПРЕМАХВАНЕ НА ФАБРИЧНИТЕ СТИЛОВЕ НА ЗАГЛАВИЯ (PREFLIGHT RESET)

### 🚨 Капан №1: Слагаш таг `<h1>` или `<ul>`, но те изглеждат като обикновен текст без размер и точки
* **Защо се случва:** Вграденият модул на Tailwind (**Preflight**) умишлено нулира всички фабрични стилове на браузъра (`h1` до `h6` стават с нормален шрифт `16px`, а `ul`/`ol` губят точките и номерацията си), за да не се бориш с различни настройки в Chrome и Safari.
* **Как да го поправиш:** Винаги задавай изрични класове върху заглавията (`class="text-2xl font-bold"`) и списъците (`class="list-disc pl-5"`).

---
[⬅️ 02-03-00. Tailwind CSS (Utility-First Архитектура)](02-03-00-Tailwind-CSS-Utility-First.md) | [⬅️ Назад към 01. HTML5 & CSS3 Foundations](../HTML5-CSS3-And-SASS/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: Responsive-Forms-And-Frameworks](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../TypeScript-Language/README.md) | [➡️ 02-04-00. Bootstrap 5 (12-Колонна Grid система и Компоненти)](02-04-00-Bootstrap-5-Grid-And-Components.md)
---
© 2026 Dev-Library / 02-03-01. Syntax-From-Scratch & Tailwind Utility Cheat Sheet
