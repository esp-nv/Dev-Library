[⬅️ 08-03. Инженерен Интервю Справочник](08-03-Advanced-SEO-Traps-And-Snippets.md) | 📂 Папка: 08-Search-Engine-Optimization | [🏠 Начало](../../README.md)

---

# 08-04. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 08:** Оптимизация за търсачки (SEO) | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за най-важните технически SEO тагове, семантични изисквания и напреднали софтуерни формати за структурирани данни.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Справочник на задължителните SEO тагове](#1-global-reference-guide-справочник-на-задължителните-seo-тагове)
* [2. CORE ON-PAGE SEO CHEAT SHEET (Бърз синтактичен пищов)](#2-core-on-page-seo-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI SNIPPET: "Глобална Schema.org JSON-LD Архитектура"](#3-готов-ui-snippet-глобална-schemaorg-json-ld-архитектура)

---

## 1. GLOBAL REFERENCE GUIDE: Справочник на задължителните SEO тагове

### 🗛 А. Метаданни и Роботизиран контрол (Вътре в `<head>`):
*   `<title>Текст</title>` ➔ Име на страницата в търсачката. Оптимална дължина: 50-60 символа.
*   `<meta name="description" content="..." />` ➔ Кратко резюме на съдържанието. Оптимална дължина: 150-160 символа.
*   `<meta name="robots" content="noindex, nofollow" />` ➔ Изрична команда за софтуерно блокиране на индексирането на конкретна уеб страница.
*   `<link rel="canonical" href="URL" />` ➔ Защита против дублирано съдържание. Насочва ботовете към оригиналния източник.

### 🗛 Б. Семантични тагове и Атрибути (Вътре в `<body>`):
*   `<h1>Заглавие</h1>` ➔ Основната и най-важна текстова тема на страницата. **Задължително се появява точно веднъж в документа.**
*   `<img src="..." alt="Описание" />` ➔ Атрибутът `alt` е критичен за SEO — ботът не вижда пикселите на снимката, а чете софтуерния текст в алта.
*   `<a href="URL" rel="nofollow">Линк</a>` ➔ Атрибутът `rel="nofollow"` казва на Google да не прехвърля Link Juice авторитет към този външен уеб адрес.

---

## 2. CORE ON-PAGE SEO CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `User-agent: * \n Disallow: /admin/` ➔ Бърз синтаксис за затваряне на административни уеб пътища във файла `robots.txt`.
*   `Sitemap: https://site.com` ➔ Поставя се на най-долния ред в `robots.txt`, за да укаже местоположението на уеб картата.
*   `og:image` ➔ Критичен Open Graph атрибут, който дефинира коя снимка ще се рендерира като визитка при споделяне на линка в социални мрежи.

---

## 3. ГОТОВ UI SNIPPET: "ГЛОБАЛНА SCHEMA.ORG JSON-LD АРХИТЕКТУРА"

Търсачките поддържат стандарта **Structured Data (Структурирани данни)** през формата **JSON-LD**. Това е скрит скриптов блок в паметта, който подава чисти данни към изкуствения интелект на Google, за да изведе сайта ти с богати визуални резултати (Rich Snippets — звезди, оценки, цени) директно в SERP.

Копирай този готов, валидиран софтуерен модел и го инжектирай чисто в `<head>` секцията на твоята продуктова или курсова уеб страница:

```html
<!-- Валидиран JSON-LD софтуерен компонент за Google Rich Snippets -->
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "Course",
  "name": "Напреднал софтуерен модул: HTML and CSS",
  "description": "Професионално декуплирано обучение по уеб компоненти, Flexbox архитектури, Bootstrap среди и техническо SEO.",
  "provider": {
    "@type": "Organization",
    "name": "SoftUni",
    "sameAs": "https://softuni.bg"
  },
  "offers": [
    {
      "@type": "Offer",
      "category": "Paid",
      "price": "0.00",
      "priceCurrency": "BGN",
      "availability": "https://schema.org"
    }
  ],
  "educationalLevel": "Advanced",
  "hasCourseInstance": {
    "@type": "CourseInstance",
    "courseMode": "Blended",
    "instructor": "Senior Architecture Team"
  }
}
</script>
```

---
[⬅️ 08-03. Инженерен Интервю Справочник](08-03-Advanced-SEO-Traps-And-Snippets.md) | 📂 Папка: 08-Search-Engine-Optimization | [🏠 Начало](../../README.md)
