[⬅️ 01-05. Архитектурно и Хардуерно Сравнение на Стиловете](01-05-CSS-Methodologies-And-Frameworks-Comparison.md) | [📂 Назад към 01. HTML5 & CSS3 Foundations Хъба](README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: JavaScript-Language](../JavaScript-Language/README.md) | [➡️ 01-96. Performance & Optimizations](01-96-Performance-And-Optimizations.md)

---

# 04-00. Пълен Изпитен Наръчник, Стратегия за Judge & Готови Шаблони

**Раздел 04:** Exam Preparation & Judge Simulations | **Цел:** Концентриран, единен наръчник за успешно преминаване на практически изпити по HTML & CSS, Responsive Design и SEO/Web оптимизация — включващ 5-стъпков работен алгоритъм, автоматизиран и визуален одит, табличен **Module Cheat Sheet**, готови стартови шаблони и решение на най-честите изпитни капани.

## 🧭 Съдържание на темата
* [1. Изпитен алгоритъм в 5 стъпки (Как се решава задача за 100/100)](#1-изпитен-алгоритъм-в-5-стъпки-как-се-решава-задача-за-100100)
* [2. Архитектурна схема за разграфяване на макет (Wireframe Decomposition)](#2-архитектурна-схема-за-разграфяване-на-макет-wireframe-decomposition)
* [3. Module Cheat Sheet (Бърза изпитна справка по категории)](#3-module-cheat-sheet-бърза-изпитна-справка-по-категории)
* [4. Готови за директно ползване изпитни шаблони (HTML5 + CSS3 Reset & Layout)](#4-готови-за-директно-ползване-изпитни-шаблони-html5--css3-reset--layout)
* [5. Системни капани на изпит: 0/100 в Judge, Счупени пътища и Шрифтове](#5-системни-капани-на-изпит-0100-в-judge-счупени-пътища-и-шрифтове)
* [6. Детайлни подмодули и Следващи стъпки (Инженерни одити 96–99)](#6-детайлни-подмодули-и-следващи-стъпки-инженерни-одити-9699)

---

## 1. ИЗПИТЕН АЛГОРИТЪМ В 5 СТЪПКИ (КАК СЕ РЕШАВА ЗАДАЧА ЗА 100/100)

Когато получиш изпитното задание и референтните изображения (screenshots), не започвай да пишеш CSS на сляпо. Следвай този строг ред:

1. **Стъпка 1 — Подготовка на файловата структура (2 минути):**
   * Създай чиста папка с `index.html`, `style.css` (или `reset.css` + `typography.css` + `style.css`, ако условието го изисква) и папка `images/`.
   * Включи `Auto Save` във VS Code и стартирай `Live Server` в режим Side-by-Side с Google Chrome.
2. **Стъпка 2 — Семантичен HTML скелет без стилове (15 минути):**
   * Изгради първо **100% от HTML структурата** (`<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<footer>`) и вкарай всички текстове и снимки.
   * Провери дали имаш `<meta charset="UTF-8">` и `<meta name="viewport" content="width=device-width, initial-scale=1.0">`.
3. **Стъпка 3 — Global Reset & Типография (10 минути):**
   * Зареди външните шрифтове (Google Fonts / Font Awesome) в `<head>` **преди** твоя `style.css`.
   * Наложи глобално `box-sizing: border-box; margin: 0; padding: 0;` и задай базовия `font-family` и `line-height` върху `body`.
4. **Стъпка 4 — Layout отвън навътре (Container ➔ Flexbox / Grid):**
   * Първо подреди големите секции и колони с `display: flex` или `display: grid` и `gap`, и чак накрая стилизирай дребните детайли (цветове, сенки, заобляне на бутони).
5. **Стъпка 5 — Пред-изпитен одит и ZIP архивиране (5 минути):**
   * Провери конзолата в Chrome DevTools (`F12`) за липсващи снимки (`404`) и тествай мобилния изглед (`Ctrl + Shift + M`).
   * Архивирай само нужните файлове без излишни системни папки (`.vscode`, `node_modules`).

---

## 2. АРХИТЕКТУРНА СХЕМА ЗА РАЗГРАФЯВАНЕ НА МАКЕТ (WIREFRAME DECOMPOSITION)

Всеки уеб макет се разделя на хоризонтални ленти (`<header>`, `<section>`, `<footer>`), а вътре във всяка лента се поставя центриран `.container` с `max-width`:

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │ <header class="site-header">  (Пълен фон: 100% ширина)                  │
 │   ┌─────────────────────────────────────────────────────────────────┐   │
 │   │ .container (max-width: 1200px; margin: 0 auto; display: flex;)  │   │
 │   │ [Logo]                                      [Nav Links + CTA]   │   │
 │   └─────────────────────────────────────────────────────────────────┘   │
 ├─────────────────────────────────────────────────────────────────────────┤
 │ <main>                                                                  │
 │   <section class="hero-section">                                        │
 │     ┌───────────────────────────────────────────────────────────────┐   │
 │     │ .container (display: flex; align-items: center; gap: 40px;)   │   │
 │     │ [Лява колона: H1 + Текст + Бутон]   [Дясна колона: Hero Img]  │   │
 │     └───────────────────────────────────────────────────────────────┘   │
 │   </section>                                                            │
 │   <section class="cards-section">                                       │
 │     ┌───────────────────────────────────────────────────────────────┐   │
 │     │ .container (display: grid; grid-template-columns: repeat(3..))│   │
 │     │ [Card 1]                  [Card 2]                  [Card 3]  │   │
 │     └───────────────────────────────────────────────────────────────┘   │
 │   </section>                                                            │
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. MODULE CHEAT SHEET (БЪРЗА ИЗПИТНА СПРАВКА ПО КАТЕГОРИИ)

| Задача на изпита | Най-бързо и сигурно CSS решение | Защо работи без грешка? |
| :--- | :--- | :--- |
| **Центриране на секция по средата** | `width: 100%; max-width: 1200px; margin: 0 auto;` | Центрира на голям екран и предотвратява хоризонтален скрол на малък. |
| **Перфектно центриране (X и Y)** | `display: flex; justify-content: center; align-items: center;` | Центрира икони, текстове и модали по двете оси едновременно. |
| **Избутване на елемент вдясно (Flex)** | `margin-left: auto;` върху конкретното Flex дете | Избутва бутона или менюто до десния край без допълнителни контейнери. |
| **Равни колони без смятане на %** | `display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px;` | Дели пространството на 3 абсолютно равни части независимо от съдържанието. |
| **Адаптивна мрежа без Media Queries** | `grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));` | Автоматично пренася картите на нов ред, когато паднат под `280px`. |
| **Снимка в рамка без разтягане** | `width: 100%; height: 220px; object-fit: cover;` | Изрязва излишното, но запазва естествените пропорции на изображението. |
| **Кръгъл аватар или икона** | `width: 64px; height: 64px; border-radius: 50%; object-fit: cover;` | Изисква еднакви `width` и `height`, за да стане идеален кръг вместо елипса. |
| **Фоново изображение на Hero блок** | `background: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('bg.jpg') center/cover no-repeat;` | Покрива целия блок, центрира снимката и добавя тъмен слой за четим бял текст. |

---

## 4. ГОТОВИ ЗА ДИРЕКТНО ПОЛЗВАНЕ ИЗПИТНИ ШАБЛОНИ (HTML5 + CSS3 RESET & LAYOUT)

### 📄 Стартов `index.html` (Изпитен Boilerplate)
```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Изпитна Задача — Dev-Library</title>
    <!-- Външни шрифтове и икони винаги стоят ПРЕДИ style.css -->
    <link rel="stylesheet" href="https://cloudflare.com">
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <header class="site-header">
        <div class="container header-flex">
            <a href="#" class="brand-logo">BrandName</a>
            <nav class="main-nav">
                <a href="#features">Предимства</a>
                <a href="#pricing">Планове</a>
                <a href="#contact" class="btn-primary">Контакт</a>
            </nav>
        </div>
    </header>

    <main>
        <section class="hero-section">
            <div class="container">
                <h1>Заглавие на проекта</h1>
                <p>Кратко и ясно подзаглавие с оптимална четимост.</p>
            </div>
        </section>
    </main>

    <footer class="site-footer">
        <div class="container">
            <p>© 2026 Всички права запазени.</p>
        </div>
    </footer>
</body>
</html>
```

### 🎨 Стартов `style.css` (Reset + Променливи + Базова структура)
```css
/* 1. Глобално нулиране */
*, *::before, *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
}

/* 2. Цветова палитра и променливи */
:root {
    --color-primary: #2563eb;
    --color-dark: #0f172a;
    --color-text: #334155;
    --color-bg-light: #f8fafc;
}

/* 3. Базова типография и защита на снимки */
body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    color: var(--color-text);
    line-height: 1.6;
}

img {
    max-width: 100%;
    height: auto;
    display: block;
}

a {
    text-decoration: none;
    color: inherit;
}

/* 4. Основен центриращ контейнер */
.container {
    width: 100%;
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
}

.header-flex {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-top: 16px;
    padding-bottom: 16px;
}

.main-nav {
    display: flex;
    align-items: center;
    gap: 24px;
}

.btn-primary {
    background-color: var(--color-primary);
    color: #ffffff;
    padding: 10px 20px;
    border-radius: 6px;
    font-weight: 600;
}
```

---
## 5. СИСТЕМНИ КАПАНИ НА ИЗПИТ: 0/100 В JUDGE, СЧУПЕНИ ПЪТИЩА И ШРИФТОВЕ

### 🚨 Капан №1: Абсолютни пътища към снимки и CSS (`C:/Users/...` или `/images/pic.jpg`)
* **Какво се случва:** На твоя компютър снимката се вижда, но в Judge или на друг компютър излиза счупена икона.
* **Защо се случва:** Започващият с наклонена черта път `/images/pic.jpg` търси папката в корена на сървъра, а `C:/Users/...` сочи към локалния ти диск.
* **Решение:** Използвай винаги **относителни пътища** спрямо текущия файл: `images/pic.jpg` в HTML, а вътре в `css/style.css` — `../images/pic.jpg`!

### 🚨 Капан №2: Използване на грешни имена на файлове при архивиране за Judge
* **Защо се случва:** Автоматизираните тестове често търсят конкретно `index.html` и `style.css` (с малки букви). Ако ги кръстиш `Index.html` или `Styles.css`, Linux сървърът на Judge (който е Case-Sensitive) няма да ги намери и ще даде `0/100`.

### 🚨 Капан №3: Пренебрегване на `line-height` и `letter-spacing` при сравнение на скрийншоти
* **Решение:** Браузърите по подразбиране слагат `line-height: 1.2`, докато повечето модерни дизайни (Figma / Adobe XD) използват `line-height: 1.5` или `1.6`. Винаги проверявай височината на текстовите блокове, за да не се разминава вертикалният ритъм на страницата.

---

## 6. ДЕТАЙЛНИ ПОДМОДУЛИ И СЛЕДВАЩИ СЪПКИ (ИНЖЕНЕРНИ ОДИТИ 96–99)

* **[01-96. Performance & Optimizations](../01-96-Performance-And-Optimizations.md)** — Сравнения (❌ Бавен CSS/DOM/No-Cache срещу ✅ Оптимизиран път) с ASCII блок-схеми.
* **[01-97. Category Cheat Sheet](../01-97-Category-Cheat-Sheet.md)** — Пълен обобщен справочник по категории за цялото Ниво 1.
* **[01-98. Common Errors & Fixes](../01-98-Common-Errors-And-Fixes.md)** — Каталог на грешките (вход / грешен изход / причина / решение).
* **[01-99. Interview Q&A, Traps & Debugging](../01-99-Interview-QA-Traps-And-Debugging.md)** — Senior интервю въпроси с отговори, скрити капани и техники за дебъгване.







---

[⬅️ 01-05. Архитектурно и Хардуерно Сравнение на Стиловете](01-05-CSS-Methodologies-And-Frameworks-Comparison.md) | [📂 Назад към 01. HTML5 & CSS3 Foundations Хъба](README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: JavaScript-Language](../JavaScript-Language/README.md) | [➡️ 01-96. Performance & Optimizations](01-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 04-00. Пълен Изпитен Наръчник, Стратегия за Judge & Готови Шаблони
