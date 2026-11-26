[⬅️ 02-04-00. Bootstrap 5 (12-Колонна Grid система и Компоненти)](02-04-00-Bootstrap-5-Grid-And-Components.md) | [⬅️ Назад към 01. HTML5 & CSS3 Foundations](../HTML5-CSS3-And-SASS/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: Responsive-Forms-And-Frameworks](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: Responsive-Forms-And-Frameworks](../TypeScript-Language/README.md) | [➡️ 03-00. Intro to Web Visibility & Caching](../03-SEO-And-Redis-Caching/03-00-Intro.md)

---

# 02-04-01. Syntax-From-Scratch & Bootstrap 5 Grid Templates + Cheat Sheet

**Раздел 02:** Responsive Design, Forms & CSS Frameworks | **Цел:** Практическо стартиране с Bootstrap 5 от нулата (CDN интеграция), изграждане на отзивчиви 12-колонни решетки, вход/изход примери с Utility класове (`g-*`, `m-*`, `p-*`, `d-flex`), пълен табличен **Module Cheat Sheet** и готов за копиране шаблон със страница, навигация и карти.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Стартов HTML5 шаблон с Bootstrap 5 CDN](#1-стъпка-по-стъпка-стартов-html5-шаблон-с-bootstrap-5-cdn)
* [2. Синтаксис от нулата: Комбиниране на колони и Gutters (Вход / Изход)](#2-синтаксис-от-нулата-комбиниране-на-колони-и-gutters-вход--изход)
* [3. Module Cheat Sheet (Пълна Bootstrap 5 синтактична справка)](#3-module-cheat-sheet-пълна-bootstrap-5-синтактична-справка)
* [4. Готов за директно ползване Snippet: Адаптивен Dashboard & Cards Layout](#4-готов-за-директно-ползване-snippet-адаптивен-dashboard--cards-layout)
* [5. Системни капани: Сбор на колони над 12 и Неравномерна височина на картите](#5-системни-капани-сбор-на-колони-над-12-и-неравномерна-височина-на-картите)
* [6. Детайлни подмодули към Тема 02-04](#6-детайлни-подмодули-към-тема-02-04)

---

## 1. СТЪПКА ПО СТЪПКА: СТАРТОВ HTML5 ШАБЛОН С BOOTSTRAP 5 CDN

За да започнеш веднага без локална инсталация, постави CSS файла в `<head>`, а JavaScript пакета (`bootstrap.bundle.min.js`) — точно преди затварящия таг `</body>`:

```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap 5 Проект</title>
    <!-- 1. Bootstrap 5 CSS -->
    <link href="https://jsdelivr.net" rel="stylesheet">
    <!-- 2. Твоят собствен CSS винаги стои СЛЕД Bootstrap -->
    <link href="style.css" rel="stylesheet">
</head>
<body>
    <div class="container py-5">
        <h1 class="text-primary fw-bold">Здравей, Bootstrap 5!</h1>
    </div>

    <!-- 3. Bootstrap 5 JS Bundle (включва Popper.js) -->
    <script src="https://jsdelivr.net"></script>
</body>
</html>
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: КОМБИНИРАНЕ НА КОЛОНИ И GUTTERS (ВХОД / ИЗХОД)

### 🔹 Пример А: Асиметрично оформление (Основно съдържание 8 колони + Sidebar 4 колони)
* **Вход (`index.html`):**
  ```html
  <div class="container my-4">
      <div class="row g-4">
          <main class="col-12 col-lg-8">
              <div class="p-4 bg-light border rounded">Основна статия (66.6% на десктоп)</div>
          </main>
          <aside class="col-12 col-lg-4">
              <div class="p-4 bg-light border rounded">Странично меню (33.3% на десктоп)</div>
          </aside>
      </div>
  </div>
  ```
* **Изход в браузъра:**
  * На телефон и таблет (`< 992px`): Двата блока заемат по `12` колони и се подреждат един под друг с вертикално разстояние `24px` (`g-4`).
  * На лаптоп и десктоп (`>= 992px`): Статията и менюто застават едно до друго в съотношение `8 : 4` (`12` общо).

---

## 3. MODULE CHEAT SHEET (ПЪЛНА BOOTSTRAP 5 СИНТАКТИЧНА СПРАВКА)

| Категория | Bootstrap 5 Класове | Какво правят? |
| :--- | :--- | :--- |
| **Контейнери** | `.container` / `.container-fluid` / `.container-lg` | Центрира съдържанието с фиксирана макс. ширина или `100%` ширина. |
| **Grid Система** | `.row` / `.col` / `.col-6` / `.col-md-4` / `.col-lg-3` | Разделя реда на 12 колони по Mobile-First брейкпойнти. |
| **Gutters (Разстояния)** | `.g-3` / `.gx-4` / `.gy-2` (`0` до `5`) | Контролира хоризонталното и вертикалното разстояние между колоните. |
| **Spacing (Margin/Padding)** | `.m-3` / `.p-4` / `.mt-5` / `.px-3` / `.mx-auto` | Скала от `0` до `5` (`1 = 4px`, `2 = 8px`, `3 = 16px`, `4 = 24px`, `5 = 48px`). |
| **Flexbox Utilities** | `.d-flex` / `.justify-content-between` / `.align-items-center` | Бързо подравняване без писане на собствен CSS. |
| **Цветове (Семантични)** | `-primary`, `-secondary`, `-success`, `-danger`, `-warning`, `-info`, `-dark`, `-light` | Прилагат се с префикси `bg-*`, `text-*`, `btn-*`, `alert-*`, `border-*`. |
| **Типография** | `.display-4` / `.lead` / `.fw-bold` / `.text-muted` / `.text-center` | Готови размери на заглавия, удебеляване и подравняване. |
| **Компоненти** | `.btn .btn-primary` / `.card` / `.navbar` / `.badge` / `.form-control` | Готови стилове за бутони, карти, навигации и входни полета. |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: АДАПТИВЕН DASHBOARD & CARDS LAYOUT

Копирай този готов, напълно валиден Bootstrap 5 компонент с навигационна лента и 3 адаптивни карти с еднаква височина (`h-100`):

```html
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
        <a class="navbar-brand fw-bold" href="#">Dev-Library</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#mainNav">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="mainNav">
            <ul class="navbar-nav ms-auto">
                <li class="nav-item"><a class="nav-link active" href="#">Начало</a></li>
                <li class="nav-item"><a class="nav-link" href="#">Модули</a></li>
                <li class="nav-item"><a class="nav-link" href="#">Контакти</a></li>
            </ul>
        </div>
    </div>
</nav>

<main class="container my-5">
    <div class="row g-4">
        <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body d-flex flex-column">
                    <span class="badge bg-primary align-self-start mb-2">HTML5 & CSS3</span>
                    <h5 class="card-title">Основи на Уеб</h5>
                    <p class="card-text text-muted flex-grow-1">Семантична структура, Box Model и гъвкави Flexbox оформления.</p>
                    <a href="#" class="btn btn-outline-primary mt-3">Отвори лекцията</a>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body d-flex flex-column">
                    <span class="badge bg-success align-self-start mb-2">Bootstrap 5</span>
                    <h5 class="card-title">12-Колонна Grid Система</h5>
                    <p class="card-text text-muted flex-grow-1">Бързо прототипиране на адаптивни интерфейси за всички устройства.</p>
                    <a href="#" class="btn btn-outline-success mt-3">Отвори лекцията</a>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body d-flex flex-column">
                    <span class="badge bg-danger align-self-start mb-2">Redis Cache</span>
                    <h5 class="card-title">Висока Производителност</h5>
                    <p class="card-text text-muted flex-grow-1">Кеширане на данни в оперативната памет и техническо SEO.</p>
                    <a href="#" class="btn btn-outline-danger mt-3">Отвори лекцията</a>
                </div>
            </div>
        </div>
    </div>
</main>
```

---

## 5. СИСТЕМНИ КАПАНИ: СБОР НА КОЛОНИ НАД 12 И НЕРАВНОМЕРНА ВИСОЧИНА НА КАРТИТЕ

### 🚨 Капан №1: Сборът от колоните в един ред надвишава 12 (напр. `.col-8` + `.col-6`)
* **Какво се случва:** Втората колона (`.col-6`) не се побира в оставащите 4 единици и автоматично пада на нов ред отдолу, оставяйки празно място отдясно на първата колона.
* **Решение:** Винаги проверявай сумата на колоните за всеки брейкпойнт да бъде точно `12` (или използвай чист `.col` за автоматично делене).

### 🚨 Капан №2: Картите в един ред имат различна височина заради различен брой редове текст
* **Защо се случва:** По подразбиране `.card` заема само височината на собственото си съдържание, а не цялата височина на колоната `.col-*`.
* **Как да го поправиш:** Добави класа **`h-100`** (`height: 100%`) върху `<div class="card h-100">`, а вътре в `.card-body` сложи `d-flex flex-column` и `mt-auto` на бутона, за да се подравнят всички бутони в една линия най-долу!

---
[⬅️ 02-04-00. Bootstrap 5 (12-Колонна Grid система и Компоненти)](02-04-00-Bootstrap-5-Grid-And-Components.md) | [⬅️ Назад към 01. HTML5 & CSS3 Foundations](../HTML5-CSS3-And-SASS/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: Responsive-Forms-And-Frameworks](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: Responsive-Forms-And-Frameworks](../TypeScript-Language/README.md) | [➡️ 03-00. Intro to Web Visibility & Caching](../03-SEO-And-Redis-Caching/03-00-Intro.md)
---
© 2026 Dev-Library / 02-04-01. Syntax-From-Scratch & Bootstrap 5 Grid Templates + Cheat Sheet
