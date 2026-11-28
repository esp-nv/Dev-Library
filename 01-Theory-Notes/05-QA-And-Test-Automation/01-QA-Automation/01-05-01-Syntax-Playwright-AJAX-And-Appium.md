[⬅️ 01-05-00. Playwright (Auto-Waiting), AJAX/jQuery & M...](01-05-00-Playwright-AJAX-And-Appium.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-06-00. REST API, GraphQL, Contract & Load Testing](01-06-00-API-And-Performance-Testing.md)

---

# 01-05-01. Playwright & Appium Cheat Sheet (Синтаксис от Нулата)

**Раздел 03:** Web UI & Mobile Automation | **Цел:** Практически шаблони от нулата за **Playwright (TypeScript & Python)** — локатори по роля, прихващане и мокване на мрежови заявки (`page.route`), запазване на автентикация (`storageState`), работа с **Appium 2.0 (Android UiAutomator2 & W3C Touch Actions)** и обобщена 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Playwright (TypeScript): Локатори, Network Mocking & Storage State](#1-playwright-typescript-локатори-network-mocking--storage-state)
* [2. Playwright (Python Sync/Async): Кратък работен шаблон](#2-playwright-python-syncasync-кратък-работен-шаблон)
* [3. Appium 2.0 (C#): Настройка на Android Driver, Locators & Swipe жестове](#3-appium-20-c-настройка-на-android-driver-locators--swipe-жестове)
* [4. Обобщение / Бърз Cheat Sheet (Playwright & Appium API)](#4-обобщение--бърз-cheat-sheet-playwright--appium-api)

---

## 1. PLAYWRIGHT (TYPESCRIPT): ЛОКАТОРИ, NETWORK MOCKING & STORAGE STATE

```typescript
import { test, expect } from '@playwright/test';

test.describe('Dashboard & API Mocking', () => {

    test('mocks API response and verifies UI rendering', async ({ page }) => {
        // 1. Прихващане и подмяна (Mocking) на мрежова заявка преди отваряне на страницата
        await page.route('**/api/v1/user/profile', async route => {
            await route.fulfill({
                status: 200,
                contentType: 'application/json',
                body: JSON.stringify({ name: 'Елена', role: 'SDET Lead', credits: 500 })
            });
        });

        await page.goto('https://app.local');

        // 2. Вградени потребителски локатори (User-Facing Locators)
        await expect(page.getByRole('heading', { name: 'Добре дошла, Елена' })).toBeVisible();
        await expect(page.getByText('SDET Lead')).toBeVisible();

        // 3. Попълване на форма и натискане на клавиш
        await page.getByLabel('Търсене на проект').fill('Dev-Library');
        await page.keyboard.press('Enter');
    });
});
```

---

## 2. PLAYWRIGHT (PYTHON SYNC/ASYNC): КРАТЪК РАБОТЕН ШАБЛОН

```python
from playwright.sync_api import Page, expect

def test_login_flow(page: Page):
    page.goto("https://app.local")

    page.get_by_placeholder("Въведете имейл").fill("elena@devlib.bg")
    page.get_by_label("Парола").fill("Pass1234!")
    page.get_by_role("button", name="Вход").click()

    expect(page).to_have_url("https://app.local")
    expect(page.get_by_test_id("welcome-banner")).to_contain_text("Елена")
```

---

## 3. APPIUM 2.0 (C#): НАСТРОЙКА НА ANDROID DRIVER, LOCATORS & SWIPE ЖЕСТОВЕ

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

var options = new AppiumOptions
{
    PlatformName = "Android",
    AutomationName = AutomationName.AndroidUIAutomator2,
    DeviceName = "Pixel_8_API_34",
    App = @"C:\Builds\app-debug.apk"
};
options.AddAdditionalAppiumOption("autoGrantPermissions", true);

using var driver = new AndroidDriver(new Uri("http://127.0.0.1:4723"), options);

// 1. Намиране на елемент по Accessibility ID (Най-бърз и крос-платформен локатор!)
var loginBtn = driver.FindElement(MobileBy.AccessibilityId("btn_login_submit"));
loginBtn.Click();

// 2. Намиране на елемент чрез Android UiAutomator2 скролиране до текст
var settingsItem = driver.FindElement(MobileBy.AndroidUIAutomator(
    "new UiScrollable(new UiSelector().scrollable(true)).scrollIntoView(new UiSelector().text(\"Изход\"))"
));
settingsItem.Click();
```

---

## 4. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET (PLAYWRIGHT & APPIUM API)

| Действие / Инструмент | Playwright Синтаксис (TS / JS) | Appium 2.0 / Конзолни команди |
| :--- | :--- | :--- |
| **Търсене по роля и текст** | `page.getByRole('button', { name: 'Запази' })` | `MobileBy.AccessibilityId("save_btn")` |
| **Търсене по Label / Placeholder** | `page.getByLabel('Имейл')`<br>`page.getByPlaceholder('Търси...')` | `MobileBy.Id("com.app:id/email_input")` |
| **Търсене по Test ID** | `page.getByTestId('submit-order')` | `MobileBy.XPath("//android.widget.Button[@text='OK']")` |
| **Писане и Кликане** | `await locator.fill('text')`<br>`await locator.click()` | `element.SendKeys("text");`<br>`element.Click();` |
| **Web-First Assertions** | `await expect(loc).toBeVisible()`<br>`await expect(loc).toHaveText('OK')` | `Assert.That(element.Displayed, Is.True);` |
| **Изчакване на API отговор** | `await page.waitForResponse('**/api/items')` | `driver.Contexts` / `driver.Context = "WEBVIEW_..."` |
| **Стартиране на тестове & Trace** | `npx playwright test --ui`<br>`npx playwright show-trace trace.zip` | `appium --use-drivers=uiautomator2,xcuitest` |







---

[⬅️ 01-05-00. Playwright (Auto-Waiting), AJAX/jQuery & M...](01-05-00-Playwright-AJAX-And-Appium.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-06-00. REST API, GraphQL, Contract & Load Testing](01-06-00-API-And-Performance-Testing.md)

© 2026 Dev-Library / 01-05-01. Playwright & Appium Cheat Sheet (Синтаксис от Нулата)
