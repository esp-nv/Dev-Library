[⬅️ 02-09-00. Software Testing Architecture: Unit, Integ...](02-09-00-Testing-And-E2E-Playwright.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-96. JavaScript Performance & Optimizations](02-96-Performance-And-Optimizations.md)

---

# 🟨 02-09-01. Unit Testing & Playwright E2E — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Unit Testing с Mocha + Chai (AAA шаблон)](#1-unit-testing-с-mocha--chai-aaa-шаблон)
* [2. Модерен Unit Testing с Vitest (`describe`, `it`, `expect`, `vi.fn`)](#2-модерен-unit-testing-с-vitest-describe-it-expect-vifn)
* [3. Инсталация и структура на Playwright E2E тест](#3-инсталация-и-структура-на-playwright-e2e-тест)
* [4. Playwright Locators, Действия (Interactions) и Assertions](#4-playwright-locators-действия-interactions-и-assertions)
* [5. Network Interception (`page.route`) в Playwright](#5-network-interception-pageroute-в-playwright)

---

## 1. Unit Testing с Mocha + Chai (AAA шаблон)

```javascript
import { expect } from "chai";
import { calculateCircleArea } from "../src/geometry.js";

describe("calculateCircleArea()", () => {
  it("връща коректно лице при положителен радиус", () => {
    // Arrange
    const radius = 5;

    // Act
    const result = calculateCircleArea(radius);

    // Assert (closeTo за числа с плаваща запетая!)
    expect(result).to.be.closeTo(78.5398, 0.001);
  });

  it("хвърля RangeError при отрицателен радиус", () => {
    expect(() => calculateCircleArea(-2)).to.throw(RangeError);
  });

  it("връща NaN при невалиден тип вход", () => {
    expect(calculateCircleArea("abc")).to.be.NaN;
  });
});
```

---

## 2. Модерен Unit Testing с Vitest (`describe`, `it`, `expect`, `vi.fn`)

```javascript
import { describe, it, expect, vi, beforeEach } from "vitest";

describe("Storage Service", () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it("записва и прочита потребителски обект", () => {
    const user = { id: "u1", email: "dev@example.com" };
    sessionStorage.setItem("user", JSON.stringify(user));

    expect(JSON.parse(sessionStorage.getItem("user"))).toEqual(user);
  });

  it("извиква callback функцията веднъж при промяна", () => {
    const onChangeSpy = vi.fn();
    onChangeSpy("updated");

    expect(onChangeSpy).toHaveBeenCalledTimes(1);
    expect(onChangeSpy).toHaveBeenCalledWith("updated");
  });
});
```

---

## 3. Инсталация и структура на Playwright E2E тест

```bash
# Инициализиране на Playwright проект (създава tests/ и playwright.config.js)
npm init playwright@latest

# Изпълнение на всички тестове (Headless режим)
npx playwright test

# Изпълнение с визуален браузър (Headed) или интерактивен UI режим
npx playwright test --headed
npx playwright test --ui
```

```javascript
// tests/auth.spec.js
import { test, expect } from "@playwright/test";

test.describe("Authentication Flow", () => {
  test("успешен вход пренасочва към каталога и показва имейла", async ({ page }) => {
    // 1. Навигация
    await page.goto("http://localhost:3000/login");

    // 2. Попълване на формуляра и клик
    await page.fill("input[name='email']", "peter@abv.bg");
    await page.fill("input[name='password']", "123456");
    await page.click("button[type='submit']");

    // 3. Проверка на резултата в UI (Auto-waiting)
    await expect(page).toHaveURL("http://localhost:3000/catalog");
    await expect(page.locator("#welcome-msg")).toHaveText("Welcome, peter@abv.bg");
  });
});
```

---

## 4. Playwright Locators, Действия (Interactions) и Assertions

| Категория | Playwright API | Описание |
| :--- | :--- | :--- |
| **Навигация** | `await page.goto(url)` / `page.reload()` / `page.goBack()` | Зареждане и навигация в историята на браузъра |
| **Локатори** | `page.locator(".card")` / `page.getByRole("button", { name: "Save" })` | Модерни auto-waiting локатори (заменят старите `$` и `$$`) |
| **Въвеждане и Клик** | `await locator.fill("text")` / `await locator.click()` / `locator.press("Enter")` | Симулиране на реални потребителски действия |
| **Drag & Drop** | `await page.dragAndDrop("#source", "#target")` | Плъзгане и пускане между елементи |
| **Assertions (UI)** | `await expect(locator).toBeVisible()` / `.toBeHidden()` | Автоматично изчаква елементът да се покаже или скрие |
| **Assertions (Текст)** | `await expect(locator).toHaveText("Exact")` / `.toContainText("Part")` | Проверка на текстовото съдържание |
| **Assertions (Стойност)** | `expect(title).toBe("My App")` / `expect(count).toBeGreaterThan(0)` | Синхронни проверки на извлечени стойности |

---

## 5. Network Interception (`page.route`) в Playwright

```javascript
test("показва съобщение за празен списък при празен отговор от API", async ({ page }) => {
  // Прихващаме GET заявката към /api/items и връщаме празен JSON масив []
  await page.route("**/api/items", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify([])
    });
  });

  await page.goto("http://localhost:3000/catalog");
  await expect(page.locator(".empty-state")).toBeVisible();
});
```







---

[⬅️ 02-09-00. Software Testing Architecture: Unit, Integ...](02-09-00-Testing-And-E2E-Playwright.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-96. JavaScript Performance & Optimizations](02-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 02-09-01. Unit Testing & Playwright E2E — Syntax & Cheat Sheet
