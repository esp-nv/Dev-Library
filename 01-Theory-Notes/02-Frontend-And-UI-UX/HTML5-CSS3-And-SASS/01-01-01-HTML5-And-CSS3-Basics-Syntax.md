[⬅️ 01-01-00. Въведение в HTML5 и CSS3](01-01-00-HTML5-And-CSS3-Basics.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-02-00. CSS Box Model и Типография](01-02-00-CSS-Box-Model-And-Typography.md)

---

# 01-01-01. Syntax-From-Scratch & Semantic Cheat Sheet (HTML5 & CSS3 Основи)

**Раздел 01:** HTML5 & CSS3 Foundations | **Цел:** Практическо изграждане на семантична HTML5 страница и CSS3 стилове от нулата, бърз изпитен Cheat Sheet, вход/изход примери и готов за копиране компонент със стилизирано меню.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Свързване на HTML5 и външен CSS3 файл](#1-стъпка-по-стъпка-свързване-на-html5-и-външен-css3-файл)
* [2. Синтаксис от нулата: Селектори, Списъци и Хипервръзки (Вход / Изход)](#2-синтаксис-от-нулата-селектори-списъци-и-хипервръзки-вход--изход)
* [3. Индивидуален Cheat Sheet (Бърз Изпитен Пищов)](#3-индивидуален-cheat-sheet-бърз-изпитен-пищов)
* [4. Готов за директно ползване Snippet: Семантична HTML страница със стилизирано меню](#4-готов-за-директно-ползване-snippet-семантична-html-страница-със-стилизирано-меню)

* [5. Системни капани: Счупени относителни пътища и Inline стилове](#5-системни-капани-счупени-относителни-пътища-и-inline-стилове)


---

## 1. СТЪПКА ПО СТЪПКА: СВЪРЗВАНЕ НА HTML5 И ВЪНШЕН CSS3 ФАЙЛ

В професионалната разработка и на изпитите в SoftUni CSS кодът се отделя във външен файл (`style.css`), който се зарежда в `<head>` секцията на `index.html`:

1. Създай папка на проекта с два файла: `index.html` и `style.css`.
2. В `index.html` напиши `!` и натисни `Tab` (Emmet съкращение).
3. Добави тага `<link rel="stylesheet" href="style.css">` преди затварящия `</head>`:

```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Моят Първи Проект</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <header>
        <h1 class="main-title">Добре дошли в Dev-Library</h1>
    </header>
</body>
</html>
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: СЕЛЕКТОРИ, СПИСЪЦИ И ХИПЕРВРЪЗКИ (ВХОД / ИЗХОД)

### 🔹 Пример А: Каскадност и Тежест на Селекторите
* **Вход (`index.html` + `style.css`):**
  ```html
  <p id="special-note" class="text-muted">Важно съобщение за изпита.</p>
  ```
  ```css
  p { color: blue; }               /* Тежест: 0,0,1 */
  .text-muted { color: gray; }     /* Тежест: 0,1,0 */
  #special-note { color: green; }  /* Тежест: 1,0,0 -> ПЕЧЕЛИ */
  ```
* **Изход в браузъра:** Текстът се рендерира в **зелен цвят (`green`)**, защото ID селекторът има най-висока тежест независимо от реда на записване.

### 🔹 Пример Б: Отваряне на външен линк в нов таб без подчертаване
* **Вход:**
  ```html
  <a href="https://softuni.bg" target="_blank" rel="noopener noreferrer" class="clean-link">Официален сайт</a>
  ```
  ```css
  .clean-link {
      text-decoration: none;
      color: #2563eb;
      font-weight: bold;
  }
  ```
* **Изход в браузъра:** Син, удебелен линк без долна черта, който при клик отваря страницата в нов прозорец на браузъра.

---

## 3. ИНДИВИДУАЛЕН CHEAT SHEET (БЪРЗ ИЗПИТЕН ПИЩОВ)

| Конструкция / Свойство | Синтаксис | Предназначение |
| :--- | :--- | :--- |
| **Нов таб при линк** | `<a href="URL" target="_blank">` | Отваря адреса в самостоятелен прозорец на браузъра. |
| **Неномериран списък** | `<ul type="square"><li>...</li></ul>` | Списък с квадратни маркери (Bullet points). |
| **Номериран списък** | `<ol><li>...</li></ol>` | Подреден списък с числа (`1, 2, 3...`). |
| **Курсив (Наклонен текст)** | `font-style: italic;` | Накланя шрифта в CSS. |
| **Премахване на подчертаване** | `text-decoration: none;` | Премахва стандартната долна черта на `<a>` таговете. |
| **Елементен селектор** | `p { ... }` | Стилизира всички тагове от даден вид. |
| **Класов селектор** | `.btn-primary { ... }` | Преизползваем стил за много елементи (Златен стандарт). |
| **ID селектор** | `#header-logo { ... }` | Уникален идентификатор (само 1 на страница). |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: СЕМАНТИЧНА HTML СТРАНИЦА СЪС СТИЛИЗИРАНО МЕНЮ

Копирай този готов, валиден уеб код директно в твоя проект, за да имаш готова семантична структура със стилизирана навигация без подчертаване:

```html
<!-- Суров, готов софтуерен HTML & CSS фрагмент за изпити -->
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <title>Семантичен уеб компонент</title>
    <style>
        /* CSS Почистване и стил на навигацията */
        nav ul {
            list-style-type: none; /* Премахва точките от списъка */
            padding: 0;
            margin: 0;
            background-color: #333;
            overflow: hidden;
        }

        nav li {
            float: left;
        }

        nav li a {
            display: block;
            color: white;
            text-align: center;
            padding: 14px 16px;
            text-decoration: none; /* 🟢 Бърз фикс: Премахва подчертаването */
        }

        nav li a:hover {
            background-color: #111; /* Ефект при посочване с мишката */
        }

        main {
            padding: 20px;
            background-color: #f4f4f4;
        }
    </style>
</head>
<body>

    <header>
        <nav>
            <ul>
                <li><a href="#home">Начало</a></li>
                <li><a href="#courses">Курсове</a></li>
                <li><a href="#profile">Профил</a></li>
            </ul>
        </nav>
    </header>

    <main>
        <section>
            <h2>Добре дошли в Раздел: HTML and CSS</h2>
            <p>Този софтуерен компонент е напълно валидиран и съобразен с изпитните изисквания.</p>
        </section>
    </main>

    <footer>
        <p>© 2026 Dev-Library / Всички права запазени.</p>
    </footer>

</body>
</html>
```

---

## 5. СИСТЕМНИ КАПАНИ: СЧУПЕНИ ОТНОСИТЕЛНИ ПЪТИЩА И INLINE СТИЛОВЕ

### 🚨 Капан №1: Стилизираш `<nav>` или `<ul>`, но подчертаването на линковете остава
* **Какво правиш:** Слагаш `text-decoration: none;` върху `<ul>` или `<li>`, но синята черта под текста на линка не изчезва.
* **Защо се случва:** Свойството `text-decoration` се прилага директно от браузърния стил (User Agent Stylesheet) върху самия таг **`<a>`**, а не върху родителския списък.
* **Как да го поправиш:** Винаги насочвай селектора директно към `<a>` тага (`nav li a { text-decoration: none; }`).


---
[⬅️ 01-01-00. Въведение в HTML5 и CSS3](01-01-00-HTML5-And-CSS3-Basics.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](Responsive-Forms-And-Frameworks/README.md) | [➡️ 01-02-00. CSS Box Model и Типография](01-02-00-CSS-Box-Model-And-Typography.md)


© 2026 Dev-Library / 01-01-01. Syntax-From-Scratch & Semantic Cheat Sheet (HTML5 & CSS3 Основи)
