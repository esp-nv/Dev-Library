[⬅️ 01-04-01. Selenium & Locators Cheat Sheet](01-04-01-Syntax-Selenium-WebDriver-And-POM.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-05-01. Playwright & Appium Cheat Sheet](01-05-01-Syntax-Playwright-AJAX-And-Appium.md)

---

# 01-05-00. Playwright (Auto-Waiting), AJAX/jQuery & Mobile Appium

**Раздел 03:** Web UI & Mobile Automation | **Цел:** Овладяване на модерната архитектура на **Microsoft Playwright** (директна WebSocket/CDP комуникация, Browser Contexts, вграден Auto-Waiting, Network Mocking и Trace Viewer), техниките за синхронизация при динамични **AJAX / jQuery** заявки и мобилната автоматизация за Android и iOS с **Appium 2.0**.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Browser Contexts, Actionability & Appium)](#1-как-да-се-запомни-по-лесно-мнемоника-browser-contexts-actionability--appium)
* [2. Архитектура на Playwright срещу Selenium & Appium 2.0 (Блок-схема)](#2-архитектура-на-playwright-срещу-selenium--appium-20-блок-схема)
* [3. Сравнителни матрици: Playwright срещу Selenium & Мобилни стратегии (Native / Hybrid / Web)](#3-сравнителни-матрици-playwright-срещу-selenium--мобилни-стратегии-native--hybrid--web)
* [4. Синхронизация на AJAX / jQuery и Плюсове/Минуси на Playwright](#4-синхронизация-на-ajax--jquery-и-плюсовеминуси-на-playwright)
* [5. Кодов одит: Грешен (Ръчни изчаквания и счупен Shadow DOM) срещу Правилен Playwright тест](#5-кодов-одит-грешен-ръчни-изчаквания-и-счупен-shadow-dom-срещу-правилен-playwright-тест)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: BROWSER CONTEXTS, ACTIONABILITY & APPIUM)

* **Browser Contexts (Изолирани „Инкогнито“ профили за милисекунди):** В Playwright не стартираш 10 тежки браузъра за 10 теста. Стартираш **1 браузърен процес** и отваряш 10 леки `BrowserContext` сесии за под `5ms` всяка — всяка със собствени Cookies, LocalStorage и кеш!
* **Actionability Checks (6-те проверки преди всеки клик):** Преди да кликне бутон, Playwright автоматично проверява дали елементът е:
  1. **Attached** (прикачен в DOM),
  2. **Visible** (видим на екрана),
  3. **Stable** (не се движи и не се анимира),
  4. **Receives Events** (не е покрит от друг модален прозорец/спинър),
  5. **Enabled** (не е `disabled`),
  6. **Editable** (готов за писане при `fill()`).
* **Appium (Selenium за телефони):** Appium използва същия **W3C WebDriver протокол**, но превежда командите към нативните двигатели на мобилните ОС — **UiAutomator2** (за Android) и **XCUITest** (за iOS).

---

## 2. АРХИТЕКТУРА НА PLAYWRIGHT СРЕЩУ SELENIUM & APPIUM 2.0 (БЛОК-СХЕМА)

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  PLAYWRIGHT АРХИТЕКТУРА (Постоянна двупосочна WebSocket връзка)         │
 │  [Playwright Test] ◄═══════ WebSocket (CDP / Native) ═══════► [Browser] │
 │  • Нула HTTP overhead на стъпка; мигновено прихващане на Network/Events │
 ├─────────────────────────────────────────────────────────────────────────┤
 │  APPIUM 2.0 АРХИТЕКТУРА (W3C WebDriver + Платформени Драйвъри)          │
 │  [Тест Код] ──(HTTP/JSON)──► [Appium Server] ──┬──► [UiAutomator2] (Android)
 │                                                └──► [XCUITest]     (iOS)│
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: PLAYWRIGHT СРЕЩУ SELENIUM & МОБИЛНИ СТРАТЕГИИ

### 📊 А. Playwright срещу Selenium WebDriver

| Критерий | 🎭 Microsoft Playwright | 🌐 Selenium WebDriver 4 |
| :--- | :--- | :--- |
| **Комуникационен протокол** | Постоянен **WebSocket** (CDP / персонализирани протоколи). | Отделна **HTTP заявка** за всяка единична команда. |
| **Изчаквания (Waiting)** | **Вграден Auto-Waiting** при всяко действие и Web-First Assertion. | Ръчен `WebDriverWait` (Explicit Wait) за всяко динамично състояние. |
| **Shadow DOM & Iframes** | Пробива **Shadow DOM автоматично** с обикновени локатори. | Изисква `GetShadowRoot()` и ръчно превключване `SwitchTo().Frame()`. |
| **Network Mocking & Interception** | Вградено (`page.route(...)`) — подменя или блокира API отговори. | Изисква външни проксита или сложни CDP команди. |
| **Дебъгване при провал** | **Trace Viewer** (Time-travel запис с DOM снимки, Network и Console). | Статичен Screenshot и текстови логове. |
| **Поддържани езици** | TypeScript/JS, Python, C# (.NET), Java. | C#, Java, Python, JS/TS, Ruby, Kotlin, PHP. |

### 📊 Б. Видове мобилни приложения в Appium 2.0

| Вид мобилно приложение | Как е изградено? | Как се инспектират елементите? |
| :--- | :--- | :--- |
| **Native App** | Kotlin/Java (Android) или Swift/Obj-C (iOS). | **Appium Inspector** (`AccessibilityId`, `Id`, `UiSelector`, `iOSClassChain`). |
| **Mobile Web App** | Уеб сайт, отворен в мобилен Chrome или Safari. | Стандартен **Chrome DevTools** / Safari Web Inspector (CSS/XPath). |
| **Hybrid App** | Нативна обвивка с вграден **WebView** (Ionic, Cordova, Capacitor). | Превключване на контекста от `NATIVE_APP` към `WEBVIEW_...`! |

---

## 4. СИНХРОНИЗАЦИЯ НА AJAX / JQUERY И ПЛЮСОВЕ/МИНУСИ НА PLAYWRIGHT

При модерни SPA приложения (React, Vue, Angular) и класически jQuery страници, натискането на бутон изпраща асинхронна **AJAX (`XMLHttpRequest` / `fetch`)** заявка без презареждане на цялата страница:
* **Как синхронизираме в Playwright:** Използваме **Web-First Assertions** като `await expect(page.locator('.toast-success')).toBeVisible()` или изчакваме конкретен мрежов отговор с `await page.waitForResponse(res => res.url().includes('/api/orders') && res.status() === 200)`.
* **Как синхронизираме при legacy jQuery приложения в Selenium:** Проверяваме чрез `IJavaScriptExecutor`, че активните AJAX заявки са `0`: `return (typeof jQuery !== 'undefined') ? jQuery.active === 0 : true;`.

### 🟢 Предимства и 🔴 Недостатъци на Playwright:
* **🟢 Плюсове:** Изключителна скорост, почти нулеви Flaky тестове благодарение на Auto-Waiting, вграден паралелизъм (Workers), API тестване (`APIRequestContext`) и визуално сравнение на скрийншоти.
* **🔴 Минуси:** Използва модифицирани компилации на Chromium, Firefox и WebKit (а не фабричния Google Chrome или мобилен Safari), и не поддържа стари браузъри като IE11.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН СРЕЩУ ПРАВИЛЕН PLAYWRIGHT ТЕСТ

### ❌ ГРЕШЕН КОД (Използване на обикновен `expect` вместо Web-First Assertion и ръчен `waitForTimeout`):
```typescript
test('Bad Playwright Test', async ({ page }) => {
    await page.goto('https://app.local');
    // 🚨 ГРЕШКА 1: Крехък CSS клас вместо потребителски Role локатор
    await page.locator('div.card:nth-child(1) > button.btn-xs').click();

    // 🚨 ГРЕШКА 2: Hardcoded пауза (еквивалент на Thread.Sleep)
    await page.waitForTimeout(3000);

    // 🚨 ГРЕШКА 3: textContent() връща моментна снимка и НЕ чака обновяване на DOM!
    const count = await page.locator('#cart-badge').textContent();
    expect(count).toBe('1');
});
```

### 🟢 ПРАВИЛЕН КОД (User-Facing Locators + Web-First Auto-Retrying Assertions):
```typescript
import { test, expect } from '@playwright/test';

test('Add product to cart updates badge immediately', async ({ page }) => {
    await page.goto('https://app.local');

    // ✅ Устойчив локатор по роля и достъпно име (Accessibility Role)
    await page.getByRole('button', { name: 'Добави в количката' }).first().click();

    // ✅ Web-First Assertion: автоматично преизпитва до 5 секунди без Thread.Sleep!
    await expect(page.getByTestId('cart-badge')).toHaveText('1');
});
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Защо в Playwright никога не трябва да използваме `waitForLoadState('networkidle')` при модерни SPA приложения?“*
  * **Отговор:** Официалната документация на Playwright маркира `networkidle` като **discouraged (непрепоръчителен)** за тестване, защото модерни приложения с WebSockets, Analytics тракери или периодичен Polling никога не остават без мрежов трафик за 500ms, което води до чести Timeouts. Вместо това чакаме конкретен UI елемент (`await expect(locator).toBeVisible()`) или конкретен API отговор (`page.waitForResponse(...)`).
* **Въпрос 2:** *„Как превключваме между нативни екрани и уеб съдържание при тестване на **Hybrid мобилно приложение** в Appium?“*
  * **Отговор:** Извличаме наличните контексти чрез `driver.Contexts` (връща например `["NATIVE_APP", "WEBVIEW_com.company.app"]`) и извикваме `driver.Context = "WEBVIEW_com.company.app"`, за да можем да ползваме стандартни CSS селектори; след приключване се връщаме обратно към `"NATIVE_APP"`.







---

[⬅️ 01-04-01. Selenium & Locators Cheat Sheet](01-04-01-Syntax-Selenium-WebDriver-And-POM.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-05-01. Playwright & Appium Cheat Sheet](01-05-01-Syntax-Playwright-AJAX-And-Appium.md)

© 2026 Dev-Library / 01-05-00. Playwright (Auto-Waiting), AJAX/jQuery & Mobile Appium
