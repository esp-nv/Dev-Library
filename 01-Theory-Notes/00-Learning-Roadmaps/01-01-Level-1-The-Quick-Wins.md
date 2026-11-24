[⬅️ Назад към Roadmap Хъба](README.md) | [🗺️ Пълен Атлас (00)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 02-01. Ниво 2: Естествени разширения](02-01-Level-2-Natural-Extensions.md)

---

# 01-01. Ниво 1: Бързите победи (HTML5, CSS3, Bootstrap, Tailwind, SEO, Redis)

**Цел:** Бързо обобщение на фундаменталните уеб технологии от Ниво 1, подредени спрямо .NET / Razor мисловния модел — с практични примери, типични капани и директни връзки към пълните лекции в модула.

## 🧭 Съдържание на темата
* [1. HTML5 & CSS3 (Уеб скелет, Box Model & Flexbox)](#1-html5--css3-уеб-скелет-box-model--flexbox)
* [2. Bootstrap v5.3 & Tailwind CSS (Адаптивен дизайн и UI компоненти)](#2-bootstrap-v53--tailwind-css-адаптивен-дизайн-и-ui-компоненти)
* [3. Technical SEO (Оптимизация за търсачки и метаданни)](#3-technical-seo-оптимизация-за-търсачки-и-метаданни)
* [4. Redis Caching (Разпределено RAM кеширане)](#4-redis-caching-разпределено-ram-кеширане)
* [5. Всички файлове и подмодули в Ниво 1](#5-всички-файлове-и-подмодули-в-ниво-1)

---

## 1. HTML5 & CSS3 (УЕБ СКЕЛЕТ, BOX MODEL & FLEXBOX)

*   **Архитектурна роля:** Семантичният HTML5 (`<header>`, `<main>`, `<article>`, `<nav>`) дефинира структурата на DOM дървото, а модерният CSS3 (`box-sizing: border-box`, **Flexbox** и **CSS Grid**) управлява подредбата и отзивчивостта без нужда от тежък JavaScript.
*   **Синтактичен пример (Модерен Flexbox центриращ контейнер):**
    ```css
    *, *::before, *::after {
        box-sizing: border-box;
    }

    .card-grid {
        display: flex;
        flex-wrap: wrap;
        gap: 1.5rem;
        justify-content: space-between;
    }
    ```
*   **Системен капан и решение:** Използване на `content-box` по подразбиране, при което добавянето на `padding` раздува ширината на елемента и чупи решетката. Винаги задавайте глобално `box-sizing: border-box`.

👉 **За повече информация и пълни лекции:** Вижте главния навигационен индекс на модула в [01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md](./01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md).

---

## 2. BOOTSTRAP V5.3 & TAILWIND CSS (АДАПТИВЕН ДИЗАЙН И UI КОМПОНЕНТИ)

*   **Архитектурна роля:** **Bootstrap v5.3** предлага готови компоненти (модали, навигации, форми) и 12-колонна мрежа, докато **Tailwind CSS** прилага *Utility-First* подход с атомарни класове директно в HTML шаблоните (Razor, Thymeleaf, Vue, React).
*   **Синтактичен пример (Bootstrap 5.3 форма с валидационни класове):**
    ```html
    <div class="container mt-4">
        <form class="p-4 border rounded-3 bg-light shadow-sm">
            <div class="mb-3">
                <label for="userEmail" class="form-label fw-bold">Имейл адрес</label>
                <input type="email" id="userEmail" class="form-control" placeholder="name@example.com" required>
            </div>
            <button type="submit" class="btn btn-primary w-100">Абонирай се</button>
        </form>
    </div>
    ```
*   **Системен капан и решение:** Зареждане на тежкия Bootstrap JS bundle, когато се ползват само CSS стиловете, или дублиране на десетки Tailwind класове без изнасяне в споделен компонент.

👉 **За повече информация и пълни лекции:** Вижте пълните наръчници за Flexbox, Tailwind и Bootstrap в [01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md](./01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md).

---
---

## 3. TECHNICAL SEO (ОПТИМИЗАЦИЯ ЗА ТЪРСАЧКИ И МЕТАДАННИ)

*   **Архитектурна роля:** Използване на семантичен HTML, Open Graph тагове, `JSON-LD`, `robots.txt`, `sitemap.xml` и оптимизиран Core Web Vitals / TTFB за по-добро индексиране.
*   **Синтактичен пример:** Прилагане на стандартни мета тагове и каноничен адрес в `<head>` секцията.
*   **Системен капан и решение:** Избягвайте дублирано съдържание при URL параметри и клиентски рендиран критичен път без SSR; винаги задавайте `<link rel="canonical">` и Server-Side Rendering.

👉 **За повече информация и пълни лекции:** Вижте секцията за SEO в [01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md](./01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md).

---

## 4. REDIS CACHING (РАЗПРЕДЕЛЕНО RAM КЕШИРАНЕ)

*   **Архитектурна роля:** In-Memory Key-Value кеширане с TTL и стратегии като *Cache-Aside* за облекчаване на основната база данни.
*   **Синтактичен пример:** Основни команди в CLI за запис (`SET ... EX`), четене (`GET`), проверка на време (`TTL`) и изтриване (`DEL`).
*   **Системен капан и решение:** Винаги задавайте TTL/изтичане на ключовете с лек офсет (Jitter) срещу препълване на паметта и Cache Stampede.

👉 **За повече информация и пълни лекции:** Вижте пълния наръчник за Redis в [01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md](./01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md).

---

## 5. ВСИЧКИ ФАЙЛОВЕ И ПОДМОДУЛИ В НИВО 1

Директна навигация към материалите в папка **`01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/`**:

* 📂 **[Главен индекс на Ниво 1 (README.md)](./01-Level-1-HTML-CSS-Bootstrap-SEO-Redis/README.md)**
* 📄 Включва модули за инсталация, HTML/CSS, Box Model, Flexbox, Responsive Design, уеб форми, Tailwind CSS, Bootstrap v5.3, въведение в SEO и Redis, както и изпитни задачи. *Пълният списък с подмодули е наличен в документацията на проекта.*

---
[⬅️ Назад към Roadmap Хъба](README.md) | [🗺️ Пълен Атлас (00)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 02-01. Ниво 2: Естествени разширения](02-01-Level-2-Natural-Extensions.md)
