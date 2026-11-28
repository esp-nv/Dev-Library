[⬅️ 02-08-01. `lit-html`, `page.js` & Web Components — S...](02-08-01-Syntax-Client-Side-Rendering-Routing-And-Web-Components.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-09-01. Unit Testing & Playwright E2E — Syntax & C...](02-09-01-Syntax-Testing-And-E2E-Playwright.md)

---

# 🟨 02-09-00. Software Testing Architecture: Unit, Integration & E2E (Playwright)

## 📑 Съдържание
* [1. Тестовата пирамида: Unit, Integration и End-to-End (E2E) тестове](#1-тестовата-пирамида-unit-integration-и-end-to-end-e2e-тестове)
* [2. White-Box vs Black-Box Testing и шаблонът AAA](#2-white-box-vs-black-box-testing-и-шаблонът-aaa)
* [3. Unit Testing в JavaScript (Mocha + Chai vs Vitest / Jest)](#3-unit-testing-в-javascript-mocha--chai-vs-vitest--jest)
* [4. Архитектура на Playwright за Web UI & E2E автоматизация](#4-архитектура-на-playwright-за-web-ui--e2e-автоматизация)
* [5. Скрити капани и Best Practices при тестване](#5-скрити-капани-и-best-practices-при-тестване)

---

## 1. Тестовата пирамида: Unit, Integration и End-to-End (E2E) тестове

```text
          /\
         /  \       3. End-to-End (E2E / UI) Tests
        / E2E\         • Реален браузър + Сървър + База данни (Playwright, Cypress)
       /──────\        • Бавни, но тестват целия потребителски поток
      /        \    2. Integration Tests
     /Integrat. \      • Комуникация между модули, API заявки с Mock сървър (Sinon, MSW)
    /────────────\
   /              \ 1. Unit Tests
  /   Unit Tests   \   • Изолирани чисти функции и класове без външни зависимости
 /──────────────────\  • Мигновени (милисекунди), основа на стабилния код (Mocha/Chai, Vitest)
```

| Ниво на тестване | Обхват | Скорост и Цена | Инструменти в JS екосистемата |
| :--- | :--- | :--- | :--- |
| **Unit Testing** | Една функция, модул или клас (без мрежа, DOM или БД) | ⚡ Милисекунди (най-много тестове) | **Vitest**, **Jest**, **Mocha + Chai** |
| **Integration Testing** | Взаимодействие между няколко модула (напр. UI форма + API слой) | 🟡 Средна скорост | **Vitest + Testing Library**, **Sinon** |
| **End-to-End (E2E)** | Цялото приложение в реален браузър от гледна точка на потребителя | 🐢 Секунди (критични потоци) | **Playwright**, **Cypress** |

---

## 2. White-Box vs Black-Box Testing и шаблонът AAA

* **White-Box Testing (Тестване на бяла кутия):** Пише се от разработчика, който познава вътрешната структура на кода и тества всички гранични случаи (`if/else` разклонения, грешни входове, `null`/`undefined`).
* **Black-Box Testing (Тестване на черна кутия):** Тества системата само през публичния ѝ интерфейс (UI или REST API) без значение как е написан вътрешният код.

### Златният шаблон AAA (Arrange — Act — Assert)
Всеки добре структуриран тест се дели на **3 ясни фази**:
1. **Arrange (Подготовка):** Подготвяме входните данни, началния state или mock отговорите.
2. **Act (Действие):** Извикваме тестваната функция или симулираме потребителско действие (клик, попълване на форма).
3. **Assert (Проверка):** Проверяваме дали полученият резултат съвпада с очаквания.

---

## 3. Unit Testing в JavaScript (Mocha + Chai vs Vitest / Jest)

* **Mocha + Chai (Класическият стек):**
  * **Mocha** е *Test Runner-ът* (осигурява `describe()`, `it()`, `beforeEach()`, `afterEach()`).
  * **Chai** е *Assertion библиотеката* (осигурява четливи проверки като `expect(res).to.equal(5)` и `expect(fn).to.throw()`).
* **Vitest / Jest (Модерният All-in-One стек):**
  * Обединява Test Runner, Assertions (`expect().toBe()`), Mocking (`vi.fn()`) и Code Coverage в един бърз инструмент с нативна поддръжка на ES Modules и TypeScript.

---

## 4. Архитектура на Playwright за Web UI & E2E автоматизация

**Playwright** е модерна библиотека с отворен код (разработена от Microsoft) за автоматизирано тестване на уеб приложения. Ключовите ѝ архитектурни предимства са:

1. **Multi-Browser & Cross-Platform:** Управлява **Chromium** (Chrome/Edge), **Firefox** и **WebKit** (Safari) през един и същ единен API на Windows, Linux и macOS.
2. **Auto-Waiting (Интелигентно изчакване):** Преди да кликне върху бутон или да прочете текст, Playwright автоматично изчаква елементът да се появи в DOM, да стане видим, стабилен и интерактивен — елиминирайки нуждата от изкуствени `sleep(2000)` паузи.
3. **Browser Contexts (Пълна изолация):** Всеки тест се стартира в изолиран *Browser Context* (като чист Incognito профил за милисекунди), така че бисквитките и `sessionStorage` от един тест никога не замърсяват следващия.
4. **Network Interception (`page.route`):** Позволява прихващане и подмяна на HTTP/REST заявки директно в браузъра, за да тествате UI поведението при сървърни грешки (`500 Internal Server Error`) или празни списъци.

---

## 5. Скрити капани и Best Practices при тестване

1. **Избягвайте `page.waitForTimeout(3000)` в Playwright:** Твърдите паузи правят тестовете бавни и нестабилни (*Flaky Tests*). Разчитайте на вградения Auto-Wait на `page.locator(...)` и уеб-асерциите `await expect(locator).toBeVisible()`.
2. **Използвайте устойчиви селектори (User-Facing Locators):** Вместо крехки CSS селектори като `div > ul > li:nth-child(3) > button`, използвайте семантични локатори като `page.getByRole("button", { name: "Login" })`, `page.getByLabel("Email")` или `data-testid`.
3. **Не тествайте чужди библиотеки:** В Unit тестовете не тествайте дали `Array.prototype.sort` или `fetch` работят — тествайте **вашата бизнес логика** и граничните ѝ случаи!







---

[⬅️ 02-08-01. `lit-html`, `page.js` & Web Components — S...](02-08-01-Syntax-Client-Side-Rendering-Routing-And-Web-Components.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-09-01. Unit Testing & Playwright E2E — Syntax & C...](02-09-01-Syntax-Testing-And-E2E-Playwright.md)

© 2026 Dev-Library / 02-09-00. Software Testing Architecture: Unit, Integration & E2E (Playwright)
