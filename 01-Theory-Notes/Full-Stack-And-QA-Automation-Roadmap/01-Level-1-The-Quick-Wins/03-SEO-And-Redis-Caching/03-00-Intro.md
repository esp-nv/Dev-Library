[⬅️ Назад към 03. Technical SEO & Redis Caching](README.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 03-01-00. Техническо SEO и Семантично индексиране](03-01-00-Technical-SEO-And-Meta-Tags.md)

---

# 03-00. Intro to Web Visibility & In-Memory Caching

**Раздел 03:** Technical SEO & Redis In-Memory Caching | **Цел:** Разбиране на пряката връзка между архитектурата на уеб страницата, откриваемостта ѝ от търсачките (Googlebot) и скоростта на отговор на сървъра чрез кеширане в оперативната памет (RAM) с Redis.

## 🧭 Съдържание на темата
* [1. Какво обхваща Раздел 03 и защо SEO и Redis вървят ръка за ръка?](#1-какво-обхваща-раздел-03-и-защо-seo-и-redis-вървят-ръка-за-ръка)
* [2. Архитектурна схема: Пътят на заявката (SEO Crawler & Redis Cache)](#2-архитектурна-схема-пътят-на-заявката-seo-crawler--redis-cache)
* [3. Пътна карта на темите в Раздел 03](#3-пътна-карта-на-темите-в-раздел-03)
* [4. Детайлни подмодули към Глава 03](#4-детайлни-подмодули-към-глава-03)

---

## 1. КАКВО ОБХВАЩА РАЗДЕЛ 03 И ЗАЩО SEO И REDIS ВЪРВЯТ РЪКА ЗА РЪКА?

Един уеб сайт може да има перфектен дизайн, но ако търсачките не могат да го прочетат или сървърът отговаря за 3 секунди, потребителите никога няма да стигнат до него. В този раздел решаваме два критични инженерни проблема:

* **Видимост и разбираемост (Technical SEO):** Помагаме на търсещите машини и социалните мрежи да разберат структурата на страницата чрез мета тагове, канонични линкове (`canonical`), `robots.txt`, `sitemap.xml` и структурирани данни (`JSON-LD`).
* **Екстремна скорост на отговор (Redis Caching):** Тъй като Google наказва бавните сайтове (чрез метриките **Core Web Vitals** — TTFB и LCP), използваме **Redis**, за да съхраняваме често четените данни директно в **RAM паметта** (отговор под `1ms`), вместо да натоварваме бавната релационна база данни при всяко презареждане.

---

## 2. АРХИТЕКТУРНА СХЕМА: ПЪТЯТ НА ЗАЯВКАТА (SEO CRAWLER & REDIS CACHE)

```text
 ┌────────────────────────┐       1. HTTP GET /products        ┌──────────────────────────┐
 │  Googlebot / Потребител│ ─────────────────────────────────► │   Web Server / Backend   │
 └────────────────────────┘                                    └────────────┬─────────────┘
            ▲                                                               │
            │  4. Връща HTML за < 5ms                                       │ 2. Проверка в RAM
            │     (Висок Core Web Vitals рейтинг!)                          ▼
 ┌──────────┴─────────────┐       3. Cache HIT (0.5ms)         ┌──────────────────────────┐
 │  SEO Ready HTML5 +     │ ◄───────────────────────────────── │   REDIS (In-Memory RAM)  │
 │  Meta / Open Graph /   │                                    └────────────┬─────────────┘
 │  JSON-LD Schema        │                                                 │ Cache MISS
 └────────────────────────┘                                                 ▼
                                                               ┌──────────────────────────┐
                                                               │  SQL Database (Disk I/O) │
                                                               └──────────────────────────┘
```

---

## 3. ПЪТНА КАРТА НА ТЕМИТЕ В РАЗДЕЛ 03

Разделът е структуриран в две последователни лекционни двойки (`-00 Теория` и `-01 Практика & Module Cheat Sheet`):

1. **Тема 03-01: Техническо SEO и Семантично индексиране**
   * Как работят ботовете на търсачките (**Crawling ➔ Rendering ➔ Indexing**), задължителни SEO мета тагове (`title`, `description`, `robots`, `canonical`), **Open Graph** за социални мрежи, структурирани данни **Schema.org (JSON-LD)** и метриките **Core Web Vitals** (`LCP`, `INP`, `CLS`).
2. **Тема 03-02: Redis Caching Архитектура и Структури от данни**
   * Защо Redis е толкова бърз (Single-threaded Event Loop + In-Memory RAM), основни типове данни (`String`, `Hash`, `List`, `Set`, `Sorted Set`), стратегията **Cache-Aside (Lazy Loading)**, управление на живота на ключовете (`TTL` / `EXPIRE`) и механизмите за изчистване на паметта (`Eviction Policies`).

---

## 4. ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ГЛАВА 03

* **[03-01-00. Техническо SEO и Семантично индексиране](03-01-00-Technical-SEO-And-Meta-Tags.md)** — Жизнен цикъл на индексирането, Core Web Vitals и SEO капани.
* **[03-01-01. Syntax-From-Scratch & SEO Audit Checklist + Cheat Sheet](03-01-01-Syntax-From-Scratch.md)** — Готов SEO `<head>` шаблон, `robots.txt`, `sitemap.xml`, `JSON-LD` пример и табличен **Module Cheat Sheet**.
* **[03-02-00. Redis Caching Архитектура и Структури от данни](03-02-00-Redis-Caching-Architecture.md)** — RAM срещу Disk I/O, Cache-Aside поток, TTL и капани (`Cache Stampede` и липсващ TTL).
* **[03-02-01. Syntax-From-Scratch & Redis CLI Cheat Sheet + Docker Setup](03-02-01-Syntax-From-Scratch.md)** — Стартиране на Redis с Docker от нулата, вход/изход команди в `redis-cli` и пълен **Module Cheat Sheet**.

---
[⬅️ Назад към 03. Technical SEO & Redis Caching](README.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 03-01-00. Техническо SEO и Семантично индексиране](03-01-00-Technical-SEO-And-Meta-Tags.md)
---
© 2026 Dev-Library / 03-00. Intro to Web Visibility & In-Memory Caching
