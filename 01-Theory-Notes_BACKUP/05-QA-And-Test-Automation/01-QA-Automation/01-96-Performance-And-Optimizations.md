[⬅️ 01-08. Индустриални QA Платформи](01-08-Jira-JQL-TestRail-And-CICD-Platforms.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-97. QA Category Cheat Sheet](01-97-Category-Cheat-Sheet.md)

---

# 01-96. QA Automation Оптимизации: Кое е грешно, защо и как е по-добре (с Блок-схеми)

**Цел:** Инженерен анализ на най-честите причини за бавни и нестабилни (*Flaky*) тестови пакети и конкретни техники за ускоряване на регресията с над 80% — чрез API Authentication (`storageState`), паралелно изпълнение, блокиране на тежки медийни ресурси и замяна на `Thread.Sleep` с Event-Driven чакане.

## 🧭 Съдържание на темата
* [1. Оптимизация 1: UI Login преди всеки тест срещу API Login + `storageState` (До 10x по-бързо)](#1-оптимизация-1-ui-login-преди-всеки-тест-срещу-api-login--storagestate-до-10x-по-бързо)
* [2. Оптимизация 2: Последователно изпълнение срещу Паралелни Workers & Sharding](#2-оптимизация-2-последователно-изпълнение-срещу-паралелни-workers--sharding)
* [3. Оптимизация 3: Фиксирани паузи (`Thread.Sleep` / `waitForTimeout`) срещу Event-Driven Waits](#3-оптимизация-3-фиксирани-паузи-threadsleep--waitfortimeout-срещу-event-driven-waits)
* [4. Оптимизация 4: Пълно зареждане на тежки ресурси срещу Network Interception (`page.route`)](#4-оптимизация-4-пълно-зареждане-на-тежки-ресурси-срещу-network-interception-pageroute)

---

## 1. ОПТИМИЗАЦИЯ 1: UI LOGIN ПРЕДИ ВСЕКИ ТЕСТ СРЕЩУ API LOGIN + `STORAGESTATE` (ДО 10X ПО-БЪРЗО)

### Блок-схема на времето за изпълнение при 100 E2E теста:
```text
❌ БАВЕН ПОДХОД (UI Login в [SetUp] / beforeEach на всеки тест):
   Тест 1: Отвори /login ──► Попълни форма ──► Кликни Login (3.5s) ──► Тествай профил (1.0s)
   Тест 2: Отвори /login ──► Попълни форма ──► Кликни Login (3.5s) ──► Тествай поръчки (1.0s)
   ...
   За 100 теста: 350 секунди (близо 6 минути!) САМО за логване в системата!

✅ БЪРЗ ПОДХОД (1 API Login в Global Setup ➔ Запис на cookies/tokens в auth.json):
   1. Global Setup: POST /api/auth/login (0.2s) ──► Записва auth.json (storageState)
   2. Тестове 1..100: Стартират ДИРЕКТНО логнати от auth.json (0.0s login!) ──► Тестват веднага!
   Печалба: Спестява ~349 секунди и елиминира риска 100 теста да паднат, ако UI формата се забави!
```

### Кодово сравнение (Playwright TypeScript):
```typescript
// ✅ 1. auth.setup.ts (Изпълнява се 1 път преди всички тестове)
import { test as setup } from "@playwright/test";

setup("authenticate via API", async ({ request }) => {
    await request.post("https://shop.bg", {
        data: { email: "qa@shop.bg", password: "SecretPassword123!" }
    });
    await request.storageState({ path: "playwright/.auth/user.json" });
});

// ✅ 2. playwright.config.ts (Всички тестове зареждат готовата сесия наготово)
// use: { storageState: "playwright/.auth/user.json" }
```

---

## 2. ОПТИМИЗАЦИЯ 2: ПОСЛЕДОВАТЕЛНО ИЗПЪЛНЕНИЕ СРЕЩУ ПАРАЛЕЛНИ WORKERS & SHARDING

### Блок-схема на CI/CD конвейера:
```text
❌ БАВЕН ПОДХОД (1 Worker — Последователно):
   [Тест 1] ──► [Тест 2] ──► [Тест 3] ──► ... ──► [Тест 80] = 16 минути общо!

✅ ОПТИМИЗИРАН ПОДХОД (4 Workers локално / 4 Shards в GitHub Actions):
   Worker 1: [Тест 1..20]  ──┐
   Worker 2: [Тест 21..40] ──┼──► Всички 80 теста приключват за 4 минути!
   Worker 3: [Тест 41..60] ──┤
   Worker 4: [Тест 61..80] ──┘
```
*   **Задължително правило за паралелизъм:** Тестовете трябва да са **напълно независими** (всеки тест създава свои уникални тестови данни с `UUID` или чисти само своите записи), без да разчитат на споделен глобален запис в базата!

---

## 3. ОПТИМИЗАЦИЯ 3: ФИКСИРАНИ ПАУЗИ (`THREAD.SLEEP` / `WAITFORTIMEOUT`) СРЕЩУ EVENT-DRIVEN WAITS

### Блок-схема на чакането:
```text
❌ ГРЕШНО (Thread.Sleep(5000) / page.waitForTimeout(5000)):
   Сървърът отговаря за 150ms ──► Тестът спи 5000ms напразно! (4850ms загубено време)
   Сървърът отговаря за 5200ms ──► Тестът гърми с NoSuchElementException! (Flaky тест!)

✅ ПРАВИЛНО (Explicit WebDriverWait / Playwright waitForResponse & Web-First Assertions):
   Сървърът отговаря за 150ms ──► Тестът продължава НА 151-вата милисекунда!
   Сървърът отговаря за 5200ms ──► Тестът изчаква до таймаута (10s) и минава успешно!
```

---

## 4. ОПТИМИЗАЦИЯ 4: ПЪЛНО ЗАРЕЖДАНЕ НА ТЕЖКИ РЕСУРСИ СРЕЩУ NETWORK INTERCEPTION (`PAGE.ROUTE`)

При E2E функционални тестове (където проверяваме бизнес логика, форми и изчисления, а не визията на банерите), браузърът губи секунди в сваляне на огромни снимки, шрифтове и външни тракери (Google Analytics, Hotjar):

```typescript
// ✅ Блокираме външни тракери и тежки медийни файлове за 2x по-бързо зареждане на страниците:
await page.route("**/*", (route) => {
    const type = route.request().resourceType();
    const url = route.request().url();
    if (type === "image" || type === "media" || url.includes("google-analytics.com")) {
        return route.abort();
    }
    return route.continue();
});
```

---
[⬅️ 01-08. Индустриални QA Платформи](01-08-Jira-JQL-TestRail-And-CICD-Platforms.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-97. QA Category Cheat Sheet](01-97-Category-Cheat-Sheet.md)
