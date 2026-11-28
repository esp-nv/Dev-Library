[⬅️ 03-01-00. Техническо SEO, Семантично индексиране и Core Web Vitals](03-01-00-Technical-SEO-And-Meta-Tags.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 03-02-00. Redis Caching Архитектура и Структури от данни](../../04-Databases-And-Caching/Redis-Caching/03-02-00-Redis-Caching-Architecture.md)

---

# 03-01-01. Syntax-From-Scratch & SEO Audit Checklist + Cheat Sheet

**Раздел 03:** Technical SEO & Redis In-Memory Caching | **Цел:** Практическо изграждане на пълна Technical SEO конфигурация от нулата — оптимизиран `<head>` с Meta, Canonical, Hreflang и Open Graph тагове, конфигурация на `robots.txt` и `sitemap.xml`, структурирани данни `Schema.org JSON-LD`, табличен **Module Cheat Sheet** и чеклист за технически одит.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Пълен Technical SEO `<head>` скелет](#1-стъпка-по-стъпка-пълен-technical-seo-head-скелет)
* [2. Синтаксис от нулата: robots.txt, XML Sitemap и Schema.org JSON-LD (Вход / Изход)](#2-синтаксис-от-нулата-robotstxt-xml-sitemap-и-schemaorg-json-ld-вход--изход)
* [3. Module Cheat Sheet (Technical SEO Тагове, HTTP Статуси & Core Web Vitals)](#3-module-cheat-sheet-technical-seo-тагове-http-статуси--core-web-vitals)
* [4. Готов за директно ползване Snippet: E-Commerce Product JSON-LD & SEO Одит Чеклист](#4-готов-за-директно-ползване-snippet-e-commerce-product-json-ld--seo-одит-чеклист)
* [5. Системни капани: Относителни адреси в Canonical / Open Graph и Липсващ H1](#5-системни-капани-относителни-адреси-в-canonical--open-graph-и-липсващ-h1)


---

## 1. СТЪПКА ПО СТЪПКА: ПЪЛЕН TECHNICAL SEO `<HEAD>` СКЕЛЕТ

Всяка публична страница в уеб приложението трябва да съдържа следния структуриран набор от тагове в `<head>`:

```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <!-- 1. Основни SEO Meta тагове -->
    <title>Лаптопи за програмиране и QA — Цени и Ревюта | TechStore</title>
    <meta name="description" content="Разгледай селекция от професионални лаптопи за разработка и автоматизирано тестване с 32GB RAM и бърза доставка.">
    <meta name="robots" content="index, follow">

    <!-- 2. Каноничен URL и Мултиезични версии (Hreflang) -->
    <link rel="canonical" href="https://example.com">
    <link rel="alternate" hreflang="bg" href="https://example.com">
    <link rel="alternate" hreflang="en" href="https://example.com">
    <link rel="alternate" hreflang="x-default" href="https://example.com">

    <!-- 3. Open Graph (За споделяне във Facebook, LinkedIn, Viber, Slack) -->
    <meta property="og:type" content="website">
    <meta property="og:title" content="Лаптопи за програмиране и QA | TechStore">
    <meta property="og:description" content="Професионални работни станции с висока производителност.">
    <meta property="og:url" content="https://example.com">
    <meta property="og:image" content="https://example.com">
</head>
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: ROBOTS.TXT, XML SITEMAP И SCHEMA.ORG JSON-LD (ВХОД / ИЗХОД)

### 🔹 Пример А: Конфигурация на `robots.txt` в корена на сайта (`https://example.com`)
* **Вход (`robots.txt`):**
  ```text
  User-agent: *
  Disallow: /admin/
  Disallow: /cart/
  Disallow: /checkout/
  Disallow: /*?sort=
  Disallow: /*?filter=
  Allow: /

  Sitemap: https://example.com
  ```
* **Изход (Поведение на Googlebot):** Ботът не губи Crawl Budget за административни панели, потребителски колички и параметри за сортиране, а директно намира картата на сайта (`sitemap.xml`).

### 🔹 Пример Б: Валиден `sitemap.xml` за приоритетно индексиране
* **Вход (`sitemap.xml`):**
  ```xml
  <?xml version="1.0" encoding="UTF-8"?>
  <urlset xmlns="http://sitemaps.org">
      <url>
          <loc>https://example.com</loc>
          <lastmod>2026-11-14</lastmod>
          <changefreq>daily</changefreq>
          <priority>0.9</priority>
      </url>
  </urlset>
  ```
* **Изход в Google Search Console:** Изпраща директен сигнал кога страницата е обновена последно (`lastmod`), ускорявайки повторното ѝ обхождане.

---

## 3. MODULE CHEAT SHEET (TECHNICAL SEO ТАГОВЕ, HTTP СТАТУСИ & CORE WEB VITALS)

| Елемент / Директива | Синтаксис / Стойност | Оптимален стандарт & Предназначение |
| :--- | :--- | :--- |
| **`<title>` таг** | `50 – 60 символа` | Най-силният On-Page SEO сигнал; уникален за всяка страница. |
| **`meta description`** | `140 – 160 символа` | Определя текста под заглавието в Google (повишава CTR). |
| **`link canonical`** | `<link rel="canonical" href="https://...">` | Обединява дублирани URL адреси (с UTM/филтри) към един главен. |
| **`meta robots`** | `index, follow` \| `noindex, follow` \| `noindex, nofollow` | Контролира дали страницата да влезе в индекса на Google. |
| **Заглавна йерархия** | Само **един `<h1>`** ➔ `<h2>` ➔ `<h3>` | Структурира темата за търсачките и AI търсенето (GEO). |
| **Оптимизация на снимки** | `<img src="hero.webp" alt="Описание" width="800" height="450" loading="lazy">` | `WebP/AVIF` формат, `alt` текст за достъпност и `width/height` против CLS. |
| **HTTP `301` vs `302`** | `301` (Постоянен) \| `302` (Временен) | `301` прехвърля SEO тежестта към новия URL завинаги. |
| **HTTP `404` vs `410`** | `404` (Не е намерен) \| `410` (Изтрит завинаги) | `410` ускорява премахването на изчерпани продукти от индекса. |
| **Core Web Vitals** | `LCP <= 2.5s` \| `INP <= 200ms` \| `CLS <= 0.1` | Задължителни прагове за скорост и визуална стабилност. |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: E-COMMERCE PRODUCT JSON-LD & SEO ОДИТ ЧЕКЛИСТ

Копирай този структуриран `JSON-LD` блок преди затварящия `</head>`, за да визуализираш цена, наличност и рейтинг директно в резултатите на Google:

```html
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "Product",
  "name": "Слушалки PRO v2 Studio",
  "image": "https://example.com",
  "description": "Студийни безжични слушалки с активна шумоизолация и 40 часа батерия.",
  "brand": {
    "@type": "Brand",
    "name": "AudioTech"
  },
  "aggregateRating": {
    "@type": "AggregateRating",
    "ratingValue": "4.9",
    "reviewCount": "148"
  },
  "offers": {
    "@type": "Offer",
    "url": "https://example.com",
    "priceCurrency": "EUR",
    "price": "96.50",
    "availability": "https://schema.orgInStock"
  }
}
</script>
```

### 🔍 Бърз 5-точков чеклист за Technical SEO одит (Google Search Console & Lighthouse):
1. **HTTPS & Единичен домейн:** Всички `http://` и `www` версии пренасочват с **`301`** към единствен каноничен `https://` домейн.
2. **Нула вериги от пренасочвания (Redirect Chains):** Избягвай `URL A -> 301 -> URL B -> 301 -> URL C`; сочете директно към крайния адрес.
3. **Защита на LCP изображението:** На главната Hero снимка най-отгоре на екрана **не слагай** `loading="lazy"` — вместо това използвай `fetchpriority="high"`.
4. **Валидация на структурираните данни:** Провери JSON-LD кода през *Google Rich Results Test*.
5. **Чисти вътрешни линкове (Internal Linking):** Всяка важна страница трябва да е достъпна на максимум 3 клика от началната страница без счупени `404` връзки.

---

## 5. СИСТЕМНИ КАПАНИ: ОТНОСИТЕЛНИ АДРЕСИ В CANONICAL / OPEN GRAPH И ЛИПСВАЩ H1

### 🚨 Капан №1: Пишеш относителен път в `canonical` или `og:image` (`href="/laptops"` вместо `https://...`)
* **Защо се чупи:** Социалните мрежи (Facebook, LinkedIn) и търсачките изискват **пълни абсолютни URL адреси** с протокол и домейн (`https://example.com`). Относителните пътища често водят до липсваща снимка при споделяне или грешно канонизиране.

### 🚨 Капан №2: Поставяне на `loading="lazy"` върху главното Hero изображение над сгъвката (Above the Fold)
* **Защо се чупи:** `loading="lazy"` казва на браузъра да изчака, докато приключи парсването на целия документ, преди да изтегли снимката. Това изкуствено забавя метриката **LCP (Largest Contentful Paint)** и влошава оценката в Google PageSpeed Insights.


---
[⬅️ 03-01-00. Техническо SEO, Семантично индексиране и Core Web Vitals](03-01-00-Technical-SEO-And-Meta-Tags.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 03-02-00. Redis Caching Архитектура и Структури от данни](../../04-Databases-And-Caching/Redis-Caching/03-02-00-Redis-Caching-Architecture.md)
---
© 2026 Dev-Library / 03-01-01. Syntax-From-Scratch & SEO Audit Checklist + Cheat Sheet
