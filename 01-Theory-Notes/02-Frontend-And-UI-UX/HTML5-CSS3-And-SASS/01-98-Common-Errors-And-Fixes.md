[⬅️ 01-97. Category Cheat Sheet](01-97-Category-Cheat-Sheet.md) | [📂 Назад към 01. HTML5 & CSS3 Foundations Хъба](README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: JavaScript-Language](../JavaScript-Language/README.md) | [➡️ 01-99. Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)

---

# 01-98. Common Errors & Fixes (Каталог на Грешките, Edge Cases и Решения)

**Ниво 1:** The Quick Wins | **Цел:** Систематизиран инженерен каталог на най-честите визуални бъгове, конзолни грешки, сривове в SoftUni Judge, SEO пропуски и Redis изключения (Exceptions) — представен в табличен вид с конкретен вход, грешен изход/съобщение, причина под капака и точен код за фиксиране.

## 🧭 Съдържание на темата
* [1. Категория: HTML5, Форми & Judge Сривове](#1-категория-html5-форми--judge-сривове)
* [2. Категория: CSS3 Box Model, Flexbox, Grid & Responsive Бъгове](#2-категория-css3-box-model-flexbox-grid--responsive-бъгове)
* [3. Категория: Tailwind CSS & Bootstrap 5 Конфликти](#3-категория-tailwind-css--bootstrap-5-конфликти)
* [4. Категория: Technical SEO & Core Web Vitals Грешки](#4-категория-technical-seo--core-web-vitals-грешки)
* [5. Категория: Redis In-Memory Exceptions & Конзолни Грешки](#5-категория-redis-in-memory-exceptions--конзолни-грешки)
* [6. Детайлни подмодули и Следващ одит (99)](#6-детайлни-подмодули-и-следващ-одит-99)

---

## 1. КАТЕГОРИЯ: HTML5, ФОРМИ & JUDGE СРИВОВЕ

| Грешка / Симптом | Вход (Грешен код) | Изход / Съобщение за грешка | Причина & Edge Case | Точно решение (Fix) |
| :--- | :--- | :--- | :--- | :--- |
| **Счупена кирилица и `0/100` в Judge** | `<head><title>Изпит</title></head>` (без charset) | `Р Р·РїРёС‚` (Mojibake) и грешка при парсване на DOM в Judge | Браузърът или Judge чете файла в `Windows-1252` вместо `UTF-8`. | Добави `<meta charset="UTF-8">` като **първи ред** веднага след `<head>`. |
| **Липсващ ресурс `404 Not Found`** | `<img src="/images/logo.png">` или `C:/Users/...` | `GET http://127.0.0 404 (Not Found)` | Началната наклонена черта `/` сочи към корена на сървъра; Linux сървърите са Case-Sensitive (`Logo.png` != `logo.png`). | Използвай относителен път `images/logo.png` и спазвай точния регистър на буквите. |
| **Празна форма на сървъра (`null`)** | `<input type="text" id="email">` | Сървърът получава празно тяло `{}` при `POST` заявка | Атрибутът `id` е само за DOM/CSS; HTTP формата изпраща данни единствено по атрибута `name`. | Добави `name="email"` на всяко входно поле. |
| **Файлът не се качва при Submit** | `<form method="POST"><input type="file" name="cv"></form>` | Сървърът получава само текста `"resume.pdf"`, без байтовете на файла | Стандартният `application/x-www-form-urlencoded` не може да пренася двоични файлове. | Добави `enctype="multipart/form-data"` в тага `<form>`. |
| **Случайно изпращане на форма** | `<form><button>Покажи парола</button></form>` | Страницата се презарежда при клик върху помощния бутон | По подразбиране всеки `<button>` вътре във `<form>` е с тип `submit`. | Изрично задай `type="button"` на всички бутони, които не изпращат формата. |

---

## 2. КАТЕГОРИЯ: CSS3 BOX MODEL, FLEXBOX, GRID & RESPONSIVE БЪГОВЕ

| Грешка / Симптом | Вход (Грешен код) | Визуален изход (Бъг) | Причина & Edge Case | Точно решение (Fix) |
| :--- | :--- | :--- | :--- | :--- |
| **The Width Crash (Кутиите падат на нов ред)** | `width: 50%; padding: 20px; border: 2px solid #000;` | Две кутии по `50%` не се побират една до друга (`50% + 44px > 100%`) | Фабричният `box-sizing: content-box` добавя padding и border *извън* ширината. | Наложи глобално `*, *::before, *::after { box-sizing: border-box; }`. |
| **Хоризонтален скрол на мобилен екран** | `.container { width: 1200px; }` | На екран `375px` страницата стърчи вдясно с `825px` | Фиксираният `width` не позволява свиване под `1200px`. | Замени с `width: 100%; max-width: 1200px; margin: 0 auto;`. |
| **Снимките се разтягат във Flexbox** | `.row { display: flex; } <img src="a.jpg">` | Лицата на снимката стават издължени вертикално при дълъг текст отдясно | Стандартното `align-items: stretch` разпъва директните деца по височина. | Задай `align-items: center;` на родителя и `object-fit: cover;` на снимката. |
| **`z-index: 9999` не работи** | `.badge { z-index: 9999; }` (без `position`) | Елементът остава скрит под съседните блокове | `z-index` работи **само** върху позиционирани елементи (`relative`, `absolute`, `fixed`, `sticky`) или Flex/Grid деца. | Добави `position: relative;` (или `absolute`) върху елемента. |
| **Неработещи `width`/`height` на `<span>`** | `<span style="width: 120px; height: 40px;">Таг</span>` | Елементът остава широк колкото текста вътре | `<span>` и `<a>` са `display: inline` по подразбиране. | Добави `display: inline-block;` или `display: flex;`. |

---

## 3. КАТЕГОРИЯ: TAILWIND CSS & BOOTSTRAP 5 КОНФЛИКТИ

| Грешка / Симптом | Вход (Грешен код) | Изход / Бъг в браузъра | Причина & Edge Case | Точно решение (Fix) |
| :--- | :--- | :--- | :--- | :--- |
| **Динамичен Tailwind клас изчезва** | ``class={`bg-${statusColor}-500`}`` | Елементът няма фонов цвят в продукционния build | Tailwind JIT скенерът не изпълнява JS и не намира пълното име на класа. | Използвай обект с цели низове: `{ error: 'bg-red-500', ok: 'bg-green-500' }`. |
| **Bootstrap `.row` излиза извън екрана** | `<div class="row bg-dark">Текст</div>` (без `.col`) | Появява се хоризонтален скрол заради отрицателните margins на `.row` (`-12px`) | `.row` е проектиран да съдържа **единствено** `.col-*` деца. | Обвий съдържанието в `<div class="col-12">...</div>` вътре в `.row`. |
| **Bootstrap Navbar / Modal не се отваря** | Зареден е само `bootstrap.min.css` | Бутонът за мобилно меню не реагира при клик | Липсва JavaScript пакетът на Bootstrap 5. | Добави `<script src="...bootstrap.bundle.min.js"></script>` преди `</body>`. |

---

## 4. КАТЕГОРИЯ: TECHNICAL SEO & CORE WEB VITALS ГРЕШКИ

| Грешка / Симптом | Вход (Грешен код) | Изход в Google Search Console / Lighthouse | Причина & Edge Case | Точно решение (Fix) |
| :--- | :--- | :--- | :--- | :--- |
| **`noindex` страница остава в Google** | `robots.txt`: `Disallow: /filter` + `<meta name="robots" content="noindex">` | `Indexed, though blocked by robots.txt` | `robots.txt` спира бота преди да отвори страницата и да прочете `noindex`. | Премахни `Disallow` от `robots.txt` и остави само `meta noindex, follow`. |
| **Висок CLS (Layout Shift > 0.1)** | `<img src="banner.webp">` без размери | Текстът скача рязко надолу при зареждане на снимката | Браузърът не знае съотношението на страните преди изтегляне на файла. | Винаги добавяй `width="1200" height="600"` (или CSS `aspect-ratio`). |
| **Лош LCP (> 2.5s) на Hero снимка** | `<img src="hero.webp" loading="lazy">` | Lighthouse предупреждение: *Largest Contentful Paint image was lazily loaded* | `loading="lazy"` отлага изтеглянето на най-важната снимка над сгъвката. | Премахни `loading="lazy"` от Hero снимката и сложи `fetchpriority="high"`. |

---

## 5. КАТЕГОРИЯ: REDIS IN-MEMORY EXCEPTIONS & КОНЗОЛНИ ГРЕШКИ

| Грешка / Exception | Вход (Команда / Ситуация) | Точно съобщение за грешка | Причина & Edge Case | Точно решение (Fix) |
| :--- | :--- | :--- | :--- | :--- |
| **Изчерпана RAM памет в Redis** | `SET session:999 "{...}"` (при пълна RAM без TTL) | `(error) OOM command not allowed when used memory > 'maxmemory'.` | Ключовете се записват без срок на валидност (`EX`) и политиката е `noeviction`. | Винаги задавай `EX <секунди>` и настрой `maxmemory-policy allkeys-lru`. |
| **Грешен тип структура върху ключ** | `SET user:1 "Ivan"` ➔ `HGETALL user:1` | `(error) WRONGTYPE Operation against a key holding the wrong kind of value` | Опитваш да изпълниш Hash/List/Set команда върху ключ, записан като `String`. | Провери типа с `TYPE user:1`, изтрий стария ключ с `DEL` или ползвай отделен namespace (`user:1:profile`). |
| **Замръзване на сървъра в продукция** | `KEYS *` върху база с 5 милиона ключа | Всички клиенти получават `Connection Timeout` за 2–5 секунди | Redis е еднонишков и `KEYS *` блокира целия Event Loop до приключване. | Замени с неблокиращ курсор: `SCAN 0 MATCH prefix:* COUNT 100`. |
| **Неуспешна връзка с Redis** | Стартиране на приложението без вдигнат контейнер | `Error: connect ECONNREFUSED 127.0.0.1:6379` | Redis сървърът не работи или слуша на друг порт/мрежа в Docker. | Провери с `docker ps` и стартирай с `-p 6379:6379`. |

---

## 6. ДЕТАЙЛНИ ПОДМОДУЛИ И СЛЕДВАЩ ОДИТ (99)

* **[01-99. Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)** — Финалният инженерен файл за Ниво 1: Senior интервю въпроси с отговори, скрити капани и техники за дебъгване с Chrome DevTools, Lighthouse и Redis CLI.







---

[⬅️ 01-97. Category Cheat Sheet](01-97-Category-Cheat-Sheet.md) | [📂 Назад към 01. HTML5 & CSS3 Foundations Хъба](README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: 01. HTML5 & CSS3 Foundations](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: JavaScript-Language](../JavaScript-Language/README.md) | [➡️ 01-99. Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)

© 2026 Dev-Library / 01-98. Common Errors & Fixes (Каталог на Грешките, Edge Cases и Решения)
