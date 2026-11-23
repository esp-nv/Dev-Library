[⬅️ Назад към QA Automation Хъба](README.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-01-00. SDLC, STLC, Нива и Видове Тестване](01-01-00-SDLC-STLC-And-Testing-Types.md)

---

# 01-00. Intro to QA Engineering, Testing Pyramid & Roadmap

**Сектор:** QA Automation & SDET | **Цел:** Изграждане на цялостна инженерна визия за осигуряване на качеството на софтуера (Quality Assurance), разбиране на Пирамидата на тестването (Test Automation Pyramid), принципа **Shift-Left Testing** и логическата последователност от ръчното тестване към пълната автоматизация.

## 🧭 Съдържание на темата
* [1. Какво е ролята на съвременния QA Automation / SDET инженер?](#1-какво-е-ролята-на-съвременния-qa-automation--sdet-инженер)
* [2. Пирамидата на тестването (The Test Automation Pyramid)](#2-пирамидата-на-тестването-the-test-automation-pyramid)
* [3. Принципът Shift-Left Testing (Цената на дефектите във времето)](#3-принципът-shift-left-testing-цената-на-дефектите-във-времето)
* [4. Пътна карта на обучението в QA сектора](#4-пътна-карта-на-обучението-в-qa-сектора)

---

## 1. КАКВО Е РОЛЯТА НА СЪВРЕМЕННИЯ QA AUTOMATION / SDET ИНЖЕНЕР?

Тестването на софтуер не е просто „търсене на бъгове“ в края на разработката. Съвременният **SDET (Software Development Engineer in Test)** участва от първия ден на проекта, за да **предотврати** дефектите още на ниво изисквания и архитектура, и изгражда автоматизирани тестови рамки, които проверяват всяка промяна в кода за минути.

* **Manual QA:** Анализира бизнес изискванията, проектира тестови сценарии (Test Cases) чрез математически техники (EP, BVA), извършва проучвателно тестване (Exploratory Testing) и документира дефекти в Jira.
* **Automation QA / SDET:** Пише код на **C#, Java, TypeScript или Python**, за да автоматизира Unit, Integration, API, Web UI и Performance тестове и ги интегрира в **CI/CD конвейери** (GitHub Actions, Jenkins, Azure DevOps).

---

## 2. ПИРАМИДАТА НА ТЕСТВАНЕТО (THE TEST AUTOMATION PYRAMID)

За да бъде една тестова стратегия бърза, стабилна и евтина за поддръжка, разпределението на тестовете следва класическата пирамида на Майк Кон:

```text
                        /\
                       /  \
                      / E2E\         ◄── 10% (UI / Selenium / Playwright / Appium)
                     /  UI  \            • Най-бавни, най-скъпи за поддръжка, тестват критични потоци
                    /────────\
                   /   API &  \      ◄── 20% (REST / GraphQL / Integration / Contract)
                  / Integration\         • Бързи, тестват бизнес логика и комуникация без UI
                 /──────────────\
                /      Unit      \   ◄── 70% (NUnit / JUnit 5 / Vitest / pytest)
               /      Tests       \      • Изпълняват се за милисекунди, изолират отделни методи
              /────────────────────\
```

---

## 3. ПРИНЦИПЪТ SHIFT-LEFT TESTING (ЦЕНАТА НА ДЕФЕКТИТЕ ВЪВ ВРЕМЕТО)

**Shift-Left** означава преместване на тестовите дейности колкото се може по-наляво (по-рано) в жизнения цикъл на софтуера (SDLC):

* **Грешка в изискванията (Requirements):** Поправя се за минути с един разговор (`1x` цена).
* **Грешка по време на писане на код (Development / Unit Test):** Открива се веднага от разработчика (`5x` цена).
* **Грешка при системно тестване (QA Environment):** Изисква описване в Jira, връщане, нов билд и ретест (`15x` цена).
* **Грешка в продукция (Production):** Води до загуба на клиенти, срив на репутацията и спешни хотфиксове (`100x` цена).

---

## 4. ПЪТНА КАРТА НА ОБУЧЕНИЕТО В QA СЕКТОРА

1. **Тема 01-01 (`-00` / `-01`):** SDLC, STLC, нива и видове тестване + шаблони за Test Plan, Test Case и Bug Report.
2. **Тема 01-02 (`-00` / `-01`):** Test Design техники (EP, BVA, Decision Tables, State Transition), Jira, Scrum/Kanban, JQL, CI/CD и Microsoft Project.
3. **Тема 01-03 (`-00` / `-01`):** Unit тестване и Mocking в 4 езика (C# NUnit+Moq, Java JUnit 5+Mockito, TypeScript Vitest, Python pytest).
4. **Тема 01-04 (`-00` / `-01`):** Selenium WebDriver, DOM Locators (CSS/XPath), Explicit Waits и Page Object Model (POM).
5. **Тема 01-05 (`-00` / `-01`):** Модерна автоматизация с Playwright (Auto-Waiting, Trace Viewer), AJAX/jQuery и мобилно тестване с Appium.
6. **Тема 01-06 (`-00` / `-01`):** REST API, GraphQL, Contract тестване (Postman, RestSharp, REST Assured) и Performance/Load тестване (Grafana K6, JMeter).

---

[⬅️ Назад към QA Automation Хъба](README.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-01-00. SDLC, STLC, Нива и Видове Тестване](01-01-00-SDLC-STLC-And-Testing-Types.md)

© 2026 Dev-Library / 01-00. Intro to QA Engineering, Testing Pyramid & Roadmap
