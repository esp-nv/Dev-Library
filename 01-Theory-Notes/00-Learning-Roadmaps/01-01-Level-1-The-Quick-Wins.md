[⬅️ Назад към Примерни Пътеки за Учене и Инженерни Сравнения (00-Learning-Roadmaps)](README.md) | [🗺️ 01-00. Глобален уеб атлас и Път за учене (Roadmap) за C# разработчици)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 02-01. Ниво 2: Естествени разширения](02-01-Level-2-Natural-Extensions.md)


## 01-01. Ниво 1: Бързите победи

**Цел:** Бързо обобщение на фундаменталните уеб технологии от Ниво 1, подредени спрямо .NET / Razor мисловния модел — с практически примери, типични инженерни капани и директни връзки към пълните лекции в модула.

## 🧭 Съдържание на темата

- [1. HTML5 & CSS3 (Уеб скелет, Box Model & Flexbox)](#1-html5--css3-уеб-скелет-box-model--flexbox)
- [2. Bootstrap v5.3 & Tailwind CSS (Адаптивен дизайн и UI компоненти)](#2-bootstrap-v53--tailwind-css-адаптивен-дизайн-и-ui-компоненти)
- [3. Technical SEO (Оптимизация за търсачки и метаданни)](#3-technical-seo-оптимизация-за-търсачки-и-метаданни)
- [4. Redis Caching (Разпределено RAM кеширане)](#4-redis-caching-разпределено-ram-кеширане)
- [5. Всички файлове и подмодули в Ниво 1](#5-всички-файлове-и-подмодули-в-ниво-1)

---

## 1. HTML5 & CSS3 (УЕБ СКЕЛЕТ, BOX MODEL & FLEXBOX)

- **Архитектурна роля:** Семантичният HTML5 (`<header>`, `<main>`, `<article>`, `<nav>`, `<footer>`) дефинира логическата структура на документа и DOM дървото. Модерният CSS3 (`box-sizing: border-box`, Flexbox и CSS Grid) управлява визуалното оформление, позиционирането и адаптивността (Responsive Design), без да е необходим JavaScript за стандартните задачи по оформление.

- **Синтактичен пример (Модерен Flexbox контейнер с адаптивно подреждане):**

```css
*,
*::before,
*::after {
    box-sizing: border-box;
}

.card-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
    justify-content: space-between;
}

.card {
    flex: 1 1 250px;
    min-width: 0;
}
```

- **Системен капан и решение:** Използването на `content-box` по подразбиране може да доведе до неочаквано увеличаване на размерите на елементите, тъй като `padding` и `border` се добавят извън зададената ширина и височина. Добра практика е глобалното задаване на `box-sizing: border-box`.

👉 **За повече информация:** Вижте главния навигационен индекс на модула в [02-Frontend-And-UI-UX / HTML и CSS](../02-Frontend-And-UI-UX/HTML5-CSS3-And-SASS/README.md).

---

## 2. BOOTSTRAP V5.3 & TAILWIND CSS (АДАПТИВЕН ДИЗАЙН И UI КОМПОНЕНТИ)

- **Архитектурна роля:** Bootstrap v5.3 предлага готови UI компоненти, Responsive Grid System с 12 колони и стандартизирани CSS класове. Tailwind CSS използва Utility-First подход, при който малки класове за конкретни CSS свойства се комбинират директно в HTML шаблоните.

- **Синтактичен пример (Bootstrap 5.3 форма с валидационни класове):**

```html
<div class="container mt-4">
    <form class="needs-validation p-4 border rounded-3 bg-light shadow-sm" novalidate>
        <div class="mb-3">
            <label for="userEmail" class="form-label fw-bold">Имейл адрес</label>
            <input type="email" id="userEmail" class="form-control" required>
            <div class="invalid-feedback">
                Моля, въведете валиден имейл адрес.
            </div>
        </div>

        <button type="submit" class="btn btn-primary w-100">
            Абонирай се
        </button>
    </form>
</div>
```

**JavaScript логика за активиране на Bootstrap валидацията:**

```javascript
(() => {
    'use strict';

    const forms = document.querySelectorAll('.needs-validation');

    forms.forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        });
    });
})();
```

- **Системен капан и решение:** Не зареждайте целия Bootstrap JavaScript Bundle, когато използвате единствено CSS компонентите. При Tailwind избягвайте неконтролираното дублиране на дълги комбинации от utility класове — използвайте споделени Razor компоненти или подходящи абстракции.

👉 **За повече информация и пълни лекции:** Вижте пълните наръчници за Flexbox, Tailwind и Bootstrap в [02-Frontend-And-UI-UX / 02. Responsive Design, Forms & CSS Frameworks](../02-Frontend-And-UI-UX/Responsive-Forms-And-Frameworks/README.md).

---

## 3. TECHNICAL SEO (ОПТИМИЗАЦИЯ ЗА ТЪРСАЧКИ И МЕТАДАННИ)

- **Архитектурна роля:** Technical SEO обхваща техническите условия, които позволяват на търсачките да откриват, обхождат, рендират и индексират съдържанието на даден уебсайт. Основните инструменти включват семантичен HTML, Meta Tags, Open Graph, Structured Data (`JSON-LD`), `robots.txt`, `sitemap.xml`, Canonical URLs и Core Web Vitals.

- **Синтактичен пример (Стандартни метаданни и SEO настройки в HTML `<head>`):**

```html
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Примерна страница | Dev Library</title>
    <meta name="description" content="Описание на съдържанието на страницата.">
    <meta name="robots" content="index, follow">
    <link rel="canonical" href="https://example.com/page">

    <meta property="og:title" content="Примерна страница">
    <meta property="og:description" content="Описание на страницата за социалните мрежи.">
    <meta property="og:type" content="website">
    <meta property="og:url" content="https://example.com/page">
</head>
```

**Пример за Structured Data (JSON-LD):**

```html
<script type="application/ld+json">
{
    "@context": "https://schema.org",
    "@type": "WebPage",
    "name": "Примерна страница",
    "url": "https://example.com/page",
    "description": "Описание на съдържанието."
}
</script>
```

- **Системен капан и решение:** Избягвайте дублирано съдържание, причинено от URL параметри и различни адреси към една и съща страница. Използвайте `rel="canonical"`, когато има нужда да посочите предпочитания URL за дублирани или сходни страници. SSR (Server-Side Rendering) не е абсолютно задължително условие за SEO — търсачките могат да обработват и JavaScript съдържание, но SSR или Pre-rendering могат да подобрят надеждността и достъпността му.

  Следете основните Core Web Vitals:

  - **LCP (Largest Contentful Paint):** Скорост на зареждане на най-големия видим елемент.
  - **INP (Interaction to Next Paint):** Отзивчивост при взаимодействие с потребителя.
  - **CLS (Cumulative Layout Shift):** Визуална стабилност на страницата.

  Допълнително следете TTFB (Time to First Byte), който измерва времето до получаване на първия байт от сървъра, но не е самостоятелен Core Web Vital.

👉 **За повече информация и пълни лекции:** Вижте секцията за SEO в [06-Cloud-DevOps-And-Architecture/Technical-SEO-And-Web-Vitals](../06-Cloud-DevOps-And-Architecture/Technical-SEO-And-Web-Vitals/README.md).

---

## 4. REDIS CACHING (РАЗПРЕДЕЛЕНО RAM КЕШИРАНЕ)

- **Архитектурна роля:** Redis е високопроизводителна In-Memory структура от данни, която поддържа Key-Value операции и различни типове структури. Използва се за кеширане, сесии, временни данни и други сценарии. TTL (Time To Live) и Cache-Aside са основни концепции за намаляване на натоварването върху основната база данни.

- **Синтактичен пример (Основни Redis CLI команди):**

```redis
SET user:1001 "Ivan" EX 300
GET user:1001
TTL user:1001
DEL user:1001
EXISTS user:1001
```

**Пример за Cache-Aside логика:**

```text
1. Приложението получава заявка за дадени данни.
2. Проверява дали данните съществуват в Redis.
3. Ако съществуват (Cache Hit): връща кешираната стойност.
4. Ако не съществуват (Cache Miss):
   - Извлича данните от основната база данни.
   - Записва ги в Redis с подходящ TTL.
   - Връща резултата на клиента.
```

- **Системен капан и решение:** Не съхранявайте безконтролно данни без TTL, когато те трябва да имат ограничен жизнен цикъл. При масово кеширане използвайте TTL Jitter, за да избегнете едновременно изтичане на множество ключове и Cache Stampede.

  Разграничаваме три важни механизма:

  - **TTL Jitter:** Добавя случайно отклонение към TTL, така че много ключове да не изтичат едновременно.
  - **Cache Stampede Protection:** Използвайте механизми като Locking или Single-Flight, за да предотвратите едновременното генериране на едни и същи липсващи кеширани данни от множество заявки.
  - **Memory Management:** Конфигурирайте `maxmemory` и подходяща `maxmemory-policy`, за да управлявате поведението на Redis при достигане на лимита на паметта.

👉 **За повече информация и пълни лекции:** Вижте пълния наръчник за Redis в [04-Databases-And-Caching/Redis Caching](../04-Databases-And-Caching/Redis-Caching/README.md).

---

## 5. ВСИЧКИ ФАЙЛОВЕ И ПОДМОДУЛИ В НИВО 1

**Директна навигация към материалите:**

- 📂 **Главен индекс на Ниво 1**
- 📄 Включва модули за инсталация, HTML/CSS, Box Model, Flexbox, Responsive Design, уеб форми, Tailwind CSS, Bootstrap v5.3, въведение в SEO и Redis, както и изпитни задачи.
- Пълният списък с подмодули е наличен в документацията на проекта.

**Основни учебни модули:**

*   **[00. Въведение в курса и инсталация на среди](../01-Core-Tools-And-Fundamentals/Course-Intro-And-Setup/00-00-Intro.md)**
*   **[01. Въведение в HTML и CSS - Теория и Упражнения](../02-Frontend-And-UI-UX/HTML5-CSS3-And-SASS/README.md)**
*   **[02. Responsive Design, Forms & CSS Frameworks](../02-Frontend-And-UI-UX/Responsive-Forms-And-Frameworks/README.md)**
*   **[03. Въведение в SEO: Бизнес роля, еволюция на алгоритмите и увод в On-Page и Off-Page оптимизацията](../06-Cloud-DevOps-And-Architecture/Technical-SEO-And-Web-Vitals/README.md)**
*   **[04. Въведение в Redis: Бизнес роля, Еволюция на NoSQL и Кеш нивата](../04-Databases-And-Caching/Redis-Caching/README.md)**

---

[⬅️ Назад към Примерни Пътеки за Учене и Инженерни Сравнения (00-Learning-Roadmaps)](README.md) | [🗺️ 01-00. Глобален уеб атлас и Път за учене (Roadmap) за C# разработчици)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 02-01. Ниво 2: Естествени разширения](02-01-Level-2-Natural-Extensions.md)

© 2026 Dev-Library / Ниво 1: Бързите победи (HTML5, CSS3, Bootstrap, Tailwind, SEO, Redis)
