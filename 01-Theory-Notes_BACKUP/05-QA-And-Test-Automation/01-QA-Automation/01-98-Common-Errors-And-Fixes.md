[⬅️ 01-97. QA Category Cheat Sheet](01-97-Category-Cheat-Sheet.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-99. QA Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)

---

# 01-98. QA Automation Common Errors, Edge Cases & Fixes (Каталог на грешките)

**Цел:** Практически наръчник за бързо диагностициране и фиксиране на най-честите изключения и нестабилни (*Flaky*) откази в UI, API и Performance автоматизацията (`StaleElementReferenceException`, `ElementClickInterceptedException`, `TimeoutError`, Strict Mode Violation в Playwright, CORS / `401` / `415` в API тестове), гранични случаи (**Edge Cases**) и таблица по фази на възникване.

## 🧭 Съдържание на темата
* [1. UI Automation грешки в Selenium & Playwright (`StaleElement` / `ClickIntercepted` / `StrictMode`)](#1-ui-automation-грешки-в-selenium--playwright-staleelement--clickintercepted--strictmode)
* [2. API & GraphQL тестови грешки (`401` / `415` / `CORS` / Фалшиво-положителни `200 OK` при GraphQL)](#2-api--graphql-тестови-грешки-401--415--cors--фалшиво-положителни-200-ok-при-graphql)
* [3. Гранични случаи (Edge Cases) при писане на E2E и API тестове](#3-гранични-случаи-edge-cases-при-писане-на-e2e-и-api-тестове)
* [4. Подробна справка: Кога възниква всяка QA грешка и в коя фаза?](#4-подробна-справка-кога-възниква-всяка-qa-грешка-и-в-коя-фаза)

---

## 1. UI AUTOMATION ГРЕШКИ В SELENIUM & PLAYWRIGHT (`STALEELEMENT` / `CLICKINTERCEPTED` / `STRICTMODE`)

### 1.1 `StaleElementReferenceException` (Selenium)
* **Причина:** Тестът е намерил елемент (`var btn = driver.FindElement(...)`), но след това AJAX / jQuery / React прерисува тази част от DOM дървото. Старата референция в паметта вече сочи към изтрит DOM възел!
* **Грешно срещу Правилно:**
```csharp
// ❌ ГРЕШНО: Кеширане на IWebElement преди AJAX презареждане на таблицата
IWebElement row = driver.FindElement(By.CssSelector("tr.order-row"));
driver.FindElement(By.Id("refresh-btn")).Click();
row.Click(); // Гърми със StaleElementReferenceException!

// ✅ ПРАВИЛНО: Локирай елемента наново непосредствено преди действие (или ползвай Playwright Locator!)
driver.FindElement(By.Id("refresh-btn")).Click();
wait.Until(d => d.FindElement(By.CssSelector("tr.order-row"))).Click();
```

### 1.2 `ElementClickInterceptedException` (Selenium) / `Element is obscured` (Playwright)
* **Причина:** Върху целевия бутон стои модален прозорец (Cookie Banner), Loading Spinner или лепкав хедър (`position: sticky`), който прихваща клика.
* **Решение:** Изчакай изчезването на спинъра (`wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".spinner")))`) или затвори банера за бисквитки преди клика.

### 1.3 `Error: strict mode violation: locator('button') resolved to 4 elements` (Playwright)
* **Причина:** За разлика от Selenium (който тихо взима първия елемент), Playwright изисква локаторът да сочи към **точно 1 уникален елемент** и спира веднага при двусмислие!
* **Решение:** Стесни локатора чрез роля и име (`page.getByRole("button", { name: "Запази" })`) или родителски контейнер (`page.locator(".modal-footer").getByRole("button")`).

---

## 2. API & GRAPHQL ТЕСТОВИ ГРЕШКИ (`401` / `415` / `CORS` / ФАЛШИВО-ПОЛОЖИТЕЛНИ `200 OK`)

| Грешка / Симптом | Точна причина | Как се фиксира в теста |
| :--- | :--- | :--- |
| **`415 Unsupported Media Type`** | Изпращаш JSON тяло без хедър `Content-Type: application/json`. | Добави `.AddHeader("Content-Type", "application/json")` в RestSharp / Postman / K6. |
| **`401 Unauthorized` в CI/CD** | Hardcoded JWT токен в теста, който е изтекъл след 15 минути. | Генерирай пресен токен динамично в `[OneTimeSetUp]` / `beforeAll` чрез заявка към `/api/auth/login`. |
| **GraphQL тест минава зелено при грешка** | Проверяваш само `expect(res.status()).toBe(200)`, а GraphQL връща `200 OK` с `"errors": [...]`. | Задължително проверявай **`expect(body.errors).toBeUndefined()`** и **`expect(body.data).not.toBeNull()`**! |

---

## 3. ГРАНИЧНИ СЛУЧАИ (EDGE CASES) ПРИ ПИСАНЕ НА E2E И API ТЕСТОВЕ

| Граничен случай (Edge Case) | Какво се чупи в Runtime / CI/CD | Как се фиксира правилно |
| :--- | :--- | :--- |
| **Липсващо `await` пред Playwright команда** | `expect(page.getByText("OK")).toBeVisible()` без `await` минава зелено за 0ms без реално да изчака проверката! | Включи ESLint правилото `@typescript-eslint/no-floating-promises`, което маркира всяко пропуснато `await` като компилационна грешка. |
| **Тестът минава локално, но пада в Headless Linux Docker (CI)** | Различна резолюция на екрана (800×600 по подразбиране в headless) скрива десктоп менюто в бургер меню или променя шрифтовете. | Фиксирай изрично резолюцията в конфига (`viewport: { width: 1920, height: 1080 }`) и ползвай официалния Playwright Docker образ. |
| **Сблъсък на тестови данни при паралелни тестове** | Два паралелни теста регистрират `"test@shop.bg"` едновременно — вторият гърми с `409 Conflict`! | Генерирай уникални данни за всеки тест с `crypto.randomUUID()` или timestamp: `user_${Date.now()}_${workerIndex}@shop.bg`. |
| **Елемент вътре в `<iframe>` или `Shadow DOM`** | Стандартният `driver.FindElement` не вижда елементи вътре в `<iframe>` (напр. Stripe форма за карта). | В Selenium превключи с `driver.SwitchTo().Frame(...)`; в Playwright ползвай `page.frameLocator("iframe[name='card']").getByLabel(...)`. |
| **Мрежов таймаут при бавна AJAX заявка или K6 Load тест** | Натоварен сървър отговаря за 6s при 5s таймаут и тестът пада без ясна причина. | Задай ясен таймаут на ниво мрежово изчакване (`page.waitForResponse(..., { timeout: 15000 })`) и добави 1 retry в CI (`retries: process.env.CI ? 2 : 0`). |

---

## 4. ПОДРОБНА СПРАВКА: КОГА ВЪЗНИКВА ВСЯКА QA ГРЕШКА И В КОЯ ФАЗА?

| Клас / Тип грешка | Фаза на възникване | При какво възниква (Точна причина) | Пример и как се предотвратява |
| :--- | :--- | :--- | :--- |
| **`SyntaxError` / `TSxxxx` / Build Error** | **Компилационна** (`Compile / Parse-time`) | Грешен синтаксис в тестовия код, липсващ импорт или несъвместим тип в TypeScript/C#/Java. | Пропуснато `await` или счупен Page Object интерфейс ➔ Хваща се преди старта на тестовете. |
| **`SessionNotCreatedException` / Browser Executable Missing** | **При стартиране** (`Driver / Browser Startup`) | Версията на ChromeDriver не съвпада с браузъра или в CI контейнера не са инсталирани браузърите. | Изпълнете `npx playwright install --with-deps` или ползвайте Selenium Manager (вграден в Selenium 4.6+). |
| **`NoSuchElementException` / `TimeoutError`** | **При изпълнение** (`Runtime — UI Step`) | Елементът не се появява в DOM-а в рамките на таймаута (грешен селектор, липсващ `iframe` или бавен AJAX). | Проверете локатора в Playwright Inspector / DevTools и чакайте API отговора с `waitForResponse`. |
| **`StaleElementReferenceException`** | **При изпълнение** (`Runtime — DOM Re-render`) | Елементът е бил намерен, но AJAX / React / jQuery е презаредил DOM дървото преди клика. | Не кеширайте `IWebElement` в променливи; ползвайте Playwright `Locator`, който се преизчислява при всяко действие. |
| **`ElementClickInterceptedException`** | **При изпълнение** (`Runtime — UI Overlay`) | Друг елемент (модал, Cookie банер, Loading spinner) закрива целевия бутон в момента на клика. | Изчакайте спинърът да стане невидим (`toBeHidden()`) преди взаимодействие. |
| **`StrictModeViolation` (Playwright)** | **При изпълнение** (`Runtime — Locator Audit`) | Локаторът съвпада с повече от 1 елемент на страницата (напр. 3 бутона `"Купи"`). | Стеснете локатора чрез родителски елемент (`.filter({ hasText: "Лаптоп" })`) или `data-testid`. |
| **`AssertionError` (`Expected 200 but was 500`)** | **При проверка** (`Runtime — Assert Phase`) | Тестът е изпълнил стъпките успешно, но реалният резултат от системата не съвпада с очаквания. | **Открит реален дефект (или остарял тест)** — прикачете Trace/Screenshot и докладвайте Bug в Jira! |

---
[⬅️ 01-97. QA Category Cheat Sheet](01-97-Category-Cheat-Sheet.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-99. QA Interview Q&A, Traps & Debugging](01-99-Interview-QA-Traps-And-Debugging.md)
