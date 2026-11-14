[⬅️ 01-05. Multi-Language Unit Testing](01-05-Multi-Language-Unit-Testing.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-07. API (REST & GraphQL) & Performance Testing](01-07-API-And-Performance-Testing.md)

---

# 01-06. Modern UI Automation: Playwright, AJAX / jQuery & Appium

**Цел:** Изграждане на стабилни, светкавични End-to-End UI тестове с **Microsoft Playwright** (наследникът на Selenium), професионално тестване на динамични **AJAX & jQuery** страници чрез Network Interception (без нито един `sleep`), дизайн шаблон **Page Object Model (POM)** и основи на мобилната автоматизация с **Appium**.

## 🧭 Съдържание на темата
* [1. Playwright срещу Selenium WebDriver (Архитектурна блок-схема)](#1-playwright-срещу-selenium-webdriver-архитектурна-блок-схема)
* [2. Тестване на AJAX & jQuery динамични приложения (Чакане на XHR/Fetch и Mocking на отговори)](#2-тестване-на-ajax--jquery-динамични-приложения-чакане-на-xhrfetch-и-mocking-на-отговори)
* [3. Page Object Model (POM) в Playwright (TypeScript и C#)](#3-page-object-model-pom-в-playwright-typescript-и-c)
* [4. Appium основи за Mobile Test Automation (Android & iOS)](#4-appium-основи-за-mobile-test-automation-android--ios)

---

## 1. PLAYWRIGHT СРЕЩУ SELENIUM WEBDRIVER (АРХИТЕКТУРНА БЛОК-СХЕМА)

```text
❌ SELENIUM WEBDRIVER (Класически HTTP JSON Wire / W3C WebDriver):
   [ Тест код ] ──(HTTP заявка за всяка команда)──► [ Browser Driver ] ──► [ Браузър ]
   • Изисква ръчно писане на WebDriverWait за всеки елемент (иначе Flaky тестове!)

✅ MICROSOFT PLAYWRIGHT (Двупосочен WebSocket / Chrome DevTools Protocol):
   [ Тест код ] ◄══════(Постоянна WebSocket връзка)══════► [ Chromium / Firefox / WebKit ]
   • Вграден Auto-Waiting: преди всеки .click() или .fill() проверява автоматично дали
     елементът е видим, стабилен, активен (не е disabled) и не е закрит от друг елемент!
   • Вграден Network Interception, Trace Viewer и паралелно изпълнение по подразбиране!
```

---

## 2. ТЕСТВАНЕ НА AJAX & JQUERY ДИНАМИЧНИ ПРИЛОЖЕНИЯ (ЧАКАНЕ НА XHR/FETCH И MOCKING НА ОТГОВОРИ)

При уеб приложения с **AJAX** (`fetch` / `XMLHttpRequest`) или **jQuery** (`$.ajax()`, `$.get()`, динамично вмъкване в DOM след заявка), най-честата грешка е тестът да провери текста **преди** AJAX заявката да е приключила.

```typescript
import { test, expect } from "@playwright/test";

test.describe("AJAX & jQuery Dynamic UI Tests", () => {

    // 1. Чакане на реална AJAX / XHR заявка да приключи преди проверка на DOM-а
    test("loads products via AJAX when category filter changes", async ({ page }) => {
        await page.goto("https://example.com");

        // Започваме да слушаме за конкретния AJAX отговор ПРЕДИ да кликнем!
        const responsePromise = page.waitForResponse(
            res => res.url().includes("/api/products") && res.status() === 200
        );

        await page.getByLabel("Категория").selectOption("laptops");
        await responsePromise; // Изчаква точно AJAX заявката, без произволен timeout!

        // Проверяваме обновения от jQuery / JS списък в DOM дървото
        await expect(page.locator(".product-card")).toHaveCount(6);
    });

    // 2. Network Mocking (Симулиране на сървърна грешка 500 при AJAX заявка)
    test("displays error banner when AJAX call fails with 500", async ({ page }) => {
        await page.route("**/api/products*", async route => {
            await route.fulfill({
                status: 500,
                contentType: "application/json",
                body: JSON.stringify({ message: "Database connection failed" })
            });
        });

        await page.goto("https://example.com");
        await expect(page.getByRole("alert")).toContainText("Възникна грешка при зареждане");
    });
});
```

---

---

## 3. PAGE OBJECT MODEL (POM) В PLAYWRIGHT (TYPESCRIPT И C#)

Шаблонът **Page Object Model (POM)** отделя локаторите и действията върху страницата от самите тестови проверки (`Assert`). Ако фронтенд екипът промени бутон в HTML-а, редактираш **само 1 ред** в Page класа вместо в 50 теста!

### 3.1 TypeScript Playwright Page Object (`LoginPage.ts`)
```typescript
import { Page, Locator, expect } from "@playwright/test";

export class LoginPage {
    readonly page: Page;
    readonly emailInput: Locator;
    readonly passwordInput: Locator;
    readonly submitButton: Locator;
    readonly errorBanner: Locator;

    constructor(page: Page) {
        this.page = page;
        // Използваме достъпни User-Facing локатори (getByRole / getByLabel) вместо крехки XPath!
        this.emailInput = page.getByLabel("Имейл адрес");
        this.passwordInput = page.getByLabel("Парола");
        this.submitButton = page.getByRole("button", { name: "Вход" });
        this.errorBanner = page.getByRole("alert");
    }

    async goto() {
        await this.page.goto("/login");
    }

    async login(email: string, password: string) {
        await this.emailInput.fill(email);
        await this.passwordInput.fill(password);
        await this.submitButton.click();
    }
}
```

### 3.2 C# (.NET 8+) Playwright + NUnit (`LoginTests.cs`)
```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Bookstore.E2ETests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : PageTest
{
    [Test]
    public async Task Login_ShouldShowError_WhenCredentialsAreInvalid()
    {
        await Page.GotoAsync("https://example.com");

        await Page.GetByLabel("Имейл адрес").FillAsync("wrong@test.bg");
        await Page.GetByLabel("Парола").FillAsync("WrongPass123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Вход" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Alert)).ToContainTextAsync("Невалиден имейл или парола");
    }
}
```

---

## 4. APPIUM ОСНОВИ ЗА MOBILE TEST AUTOMATION (ANDROID & IOS)

**Appium** разширява протокола W3C WebDriver за автоматизиране на **Native**, **Hybrid** и **Mobile Web** приложения върху Android (чрез **UiAutomator2**) и iOS (чрез **XCUITest**).

| Платформа | Драйвер в Appium 2.x | Най-стабилни локатори | Инструмент за инспекция |
| :--- | :--- | :--- | :--- |
| **Android** | `UiAutomator2` | `AccessibilityId` (`content-desc`), `Id` (`resource-id`) | **Appium Inspector** / Android Studio Layout Inspector |
| **iOS** | `XCUITest` | `AccessibilityId` (`accessibilityIdentifier`), `-ios predicate string` | **Appium Inspector** / Xcode Accessibility Inspector |

```python
# Пример с Appium Python Client за мобилно приложение:
from appium import webdriver
from appium.options.android import UiAutomator2Options
from appium.webdriver.common.appiumby import AppiumBy

options = UiAutomator2Options()
options.platform_name = "Android"
options.device_name = "Pixel_8_Emulator"
options.app = "/builds/app-release.apk"

driver = webdriver.Remote("http://127.0.0.1:4723", options=options)
login_btn = driver.find_element(AppiumBy.ACCESSIBILITY_ID, "login_submit_btn")
login_btn.click()
driver.quit()
```

---
[⬅️ 01-05. Multi-Language Unit Testing](01-05-Multi-Language-Unit-Testing.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-07. API (REST & GraphQL) & Performance Testing](01-07-API-And-Performance-Testing.md)
