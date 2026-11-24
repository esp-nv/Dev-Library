[⬅️ 01-96. Performance & Optimizations](01-96-Performance-And-Optimizations.md) | [📂 Текуща папка: 01. The Quick Wins](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. Natural Extensions & QA Automation](../02-Level-2-Natural-Extensions-QA-Automation/README.md) | [➡️ 01-98. Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md)

---

# 01-97. Category Cheat Sheet (Пълен Справочник на Ниво 1 по Категории)

**Ниво 1:** The Quick Wins | **Цел:** Концентрирана, мигновена синтактична справка по категории за целия технологичен стек от Ниво 1 — **HTML5 Семантика и Форми**, **CSS3 Box Model, Flexbox & Grid**, **Responsive Design**, **Tailwind CSS**, **Bootstrap 5**, **Technical SEO & Core Web Vitals** и **Redis CLI**.

## 🧭 Съдържание на темата
* [1. Категория: HTML5 Семантика, Мултимедия и Форми](#1-категория-html5-семантика-мултимедия-и-форми)
* [2. Категория: CSS3 Reset, Box Model, Flexbox, Grid & Responsive](#2-категория-css3-reset-box-model-flexbox-grid--responsive)
* [3. Категория: Tailwind CSS & Bootstrap 5 (Бързо сравнение на класове)](#3-категория-tailwind-css--bootstrap-5-бързо-сравнение-на-класове)
* [4. Категория: Technical SEO, Meta Тагове, HTTP Статуси & Core Web Vitals](#4-категория-technical-seo-meta-тагове-http-статуси--core-web-vitals)
* [5. Категория: Redis CLI Команди и Структури от данни](#5-категория-redis-cli-команди-и-структури-от-данни)
* [6. Детайлни подмодули и Следващи одити (98–99)](#6-детайлни-подмодули-и-следващи-одити-9899)

---

## 1. КАТЕГОРИЯ: HTML5 СЕМАНТИКА, МУЛТИМЕДИЯ И ФОРМИ

| Подкатегория | Таг / Атрибут | Синтаксис и Предназначение |
| :--- | :--- | :--- |
| **Задължителен `<head>`** | `charset` & `viewport` | `<meta charset="UTF-8">` + `<meta name="viewport" content="width=device-width, initial-scale=1.0">` |
| **Семантична структура** | `<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<aside>`, `<footer>` | Описват логическата роля на съдържанието за SEO и достъпност (заменят излишните `<div>`). |
| **Връзки (Links)** | `target="_blank"` & `rel` | `<a href="https://..." target="_blank" rel="noopener noreferrer">Външен линк</a>` |
| **Оптимизирани снимки** | `alt`, `width`, `height`, `loading` | `<img src="pic.webp" alt="Описание" width="600" height="400" loading="lazy">` |
| **Форми (Container)** | `action`, `method`, `enctype` | `<form action="/api/save" method="POST" enctype="multipart/form-data">` |
| **Свързване на поле** | `<label for="...">` + `id` + `name` | `<label for="u">Име</label><input id="u" name="username" required>` (`name` е за бекенда!). |
| **Валидация на полета** | `required`, `minlength`, `maxlength`, `pattern`, `min`, `max` | Вградена клиентска проверка в браузъра преди изпращане към сървъра. |

---

## 2. КАТЕГОРИЯ: CSS3 RESET, BOX MODEL, FLEXBOX, GRID & RESPONSIVE

| Подкатегория | Свойство / Формула | Синтаксис и Действие |
| :--- | :--- | :--- |
| **Global Reset** | `box-sizing: border-box` | `*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }` |
| **Центриране на блок** | `max-width` + `margin` | `width: 100%; max-width: 1200px; margin: 0 auto;` |
| **Flexbox (Родител)** | `display: flex` | `justify-content` (Главна ос: `center`, `space-between`); `align-items` (Напречна ос: `center`, `stretch`); `flex-wrap: wrap`; `gap: 16px`. |
| **Flexbox (Дете)** | `flex` / `order` | `flex: 1;` (заема останалото място); `margin-left: auto;` (избутва елемента вдясно). |
| **CSS Grid (Решетка)** | `display: grid` | `grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 24px;` |
| **Позициониране** | `position` | `relative` (котва за деца), `absolute` (спрямо най-близкия `relative` родител), `fixed` (спрямо екрана), `sticky` (залепва при скрол). |
| **Адаптивност** | `@media` & `clamp()` | `@media (min-width: 768px) { ... }` (Mobile-First); `font-size: clamp(1rem, 2.5vw, 2rem);` |
| **Снимки без изкривяване** | `object-fit` | `width: 100%; height: 250px; object-fit: cover;` |

---

## 3. КАТЕГОРИЯ: TAILWIND CSS & BOOTSTRAP 5 (БЪРЗО СРАВНЕНИЕ НА КЛАСОВЕ)

| Задача | Tailwind CSS (Utility-First) | Bootstrap 5 (Component-First) |
| :--- | :--- | :--- |
| **Центриран контейнер** | `max-w-6xl mx-auto px-4` | `container` |
| **Flex ред с разстояние** | `flex justify-between items-center gap-4` | `d-flex justify-content-between align-items-center gap-3` |
| **Отзивчива мрежа (1 ➔ 2 ➔ 3)** | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6` | `row g-4` + `col-12 col-md-6 col-lg-4` |
| **Вътрешен отстъп (Padding)** | `p-4` (`16px`), `px-6 py-3` | `p-3` (`16px`), `px-4 py-2` |
| **Типография и Цвят** | `text-xl font-bold text-blue-600 text-center` | `fs-4 fw-bold text-primary text-center` |
| **Основен бутон** | `px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700` | `btn btn-primary` |

---

## 4. КАТЕГОРИЯ: TECHNICAL SEO, META ТАГОВЕ, HTTP СТАТУСИ & CORE WEB VITALS

| Елемент / Метрика | Синтаксис / Целева стойност | Инженерна роля |
| :--- | :--- | :--- |
| **Title & Description** | `<title>` (`50–60` симв.) / `<meta name="description">` (`140–160` симв.) | Определят заглавието и описанието в резултатите на Google (CTR). |
| **Canonical URL** | `<link rel="canonical" href="https://site.com">` | Предотвратява дублирано съдържание при UTM параметри и филтри. |
| **Robots контрол** | `robots.txt` (`Disallow: /admin/`) vs `<meta name="robots" content="noindex, follow">` | `robots.txt` спира обхождането; `meta noindex` вади страницата от индекса. |
| **Мултиезичност** | `<link rel="alternate" hreflang="bg" href="...">` + `x-default` | Насочва търсачките към правилната езикова/регионална версия. |
| **Structured Data** | `<script type="application/ld+json">` (`Schema.org`) | Отключва Rich Snippets (цена, наличност, рейтинг) и AI Search видимост (GEO). |
| **HTTP Статуси** | `200` (OK), `301` (Постоянен редирект), `302` (Временен), `404` (Липсва), `410` (Изтрит завинаги) | `301` прехвърля SEO тежестта; `410` изчиства изтрити продукти по-бързо. |
| **Core Web Vitals** | `LCP <= 2.5s` \| `INP <= 200ms` \| `CLS <= 0.1` | Скорост на зареждане, бързина на реакция и визуална стабилност. |

---

## 5. КАТЕГОРИЯ: REDIS CLI КОМАНДИ И СТРУКТУРИ ОТ ДАННИ

| Структура / Група | Команди | Пример за употреба |
| :--- | :--- | :--- |
| **Управление на ключове** | `SET`, `GET`, `DEL`, `EXISTS`, `EXPIRE`, `TTL`, `SCAN` | `SET key "val" EX 300`; `TTL key`; `SCAN 0 MATCH user:* COUNT 100` |
| **String & Броячи** | `SETNX`, `INCR`, `DECR`, `INCRBY` | `INCR page:views:home` (атомарен брояч за Rate Limiting и статистики). |
| **Hash (Обекти)** | `HSET`, `HGET`, `HGETALL`, `HINCRBY`, `HDEL` | `HSET user:10 name "Ivan" role "QA"`; `HGETALL user:10` |
| **List (Опашки)** | `LPUSH`, `RPUSH`, `LPOP`, `RPOP`, `LRANGE` | `LPUSH jobs:email "task1"`; `LRANGE jobs:email 0 9` |
| **Set (Уникални)** | `SADD`, `SREM`, `SMEMBERS`, `SISMEMBER`, `SCARD` | `SADD online:users "u1" "u2"`; `SCARD online:users` |
| **Sorted Set (Класации)** | `ZADD`, `ZRANGE`, `ZREVRANGE`, `ZRANK`, `ZINCRBY` | `ZADD scores 98 "Elena" 92 "Petar"`; `ZREVRANGE scores 0 -1 WITHSCORES` |
| **Трансакции & Одит** | `MULTI`, `EXEC`, `DISCARD`, `INFO memory`, `PING` | `MULTI` ... команди ... `EXEC` (атомарно изпълнение без прекъсване). |

---

## 6. ДЕТАЙЛНИ ПОДМОДУЛИ И СЛЕДВАЩИ ОДИТИ (98–99)

* **[01-98. Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md)** — Каталог на грешките (вход / грешен изход / причина / решение) при CSS разминавания, Judge сривове, CORS и Redis памет.
* **[01-99. Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)** — Senior интервю въпроси с отговори, скрити капани и техники за дебъгване с Chrome DevTools и Redis Monitor.

---
[⬅️ 01-96. Performance & Optimizations](01-96-Performance-And-Optimizations.md) | [📂 Текуща папка: 01. The Quick Wins](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. Natural Extensions & QA Automation](../02-Level-2-Natural-Extensions-QA-Automation/README.md) | [➡️ 01-98. Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md)
---
© 2026 Dev-Library / 01-97. Category Cheat Sheet (Пълен Справочник на Ниво 1 по Категории)
