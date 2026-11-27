[⬅️ 01-96. QA Performance & Optimizations](01-96-Performance-And-Optimizations.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-98. QA Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md)

---

# 01-97. QA Automation Category Cheat Sheet: Пълно обобщение по категории & Инструменти

**Цел:** Бърза синтактична и практическа справка на едно място за всички категории в QA инженерството — йерархия на локаторите, HTTP статус кодове при API тестване, Test Design техники, CLI команди и таблица с най-важните тестови библиотеки в 4 езика.

## 🧭 Съдържание по категории
* [1. Категория 1: Йерархия на UI локаторите (От най-устойчиви към най-крехки)](#1-категория-1-йерархия-на-ui-локаторите-от-най-устойчиви-към-най-крехки)
* [2. Категория 2: HTTP Статус кодове и API / GraphQL валидационни правила](#2-категория-2-http-статус-кодове-и-api--graphql-валидационни-правила)
* [3. Категория 3: Test Design техники (Бърза формула за избор)](#3-категория-3-test-design-техники-бърза-формула-за-избор)
* [4. Категория 4: Важни QA библиотеки и фреймуърци (Инсталация, Извикване и Приложение)](#4-категория-4-важни-qa-библиотеки-и-фреймуърци-инсталация-извикване-и-приложение)
* [5. Категория 5: Най-важните CLI команди за QA инженери (Playwright, .NET, Maven, pytest, K6, JMeter)](#5-категория-5-най-важните-cli-команди-за-qa-инженери-playwright-net-maven-pytest-k6-jmeter)

---

## 1. КАТЕГОРИЯ 1: ЙЕРАРХИЯ НА UI ЛОКАТОРИТЕ (ОТ НАЙ-УСТОЙЧИВИ КЪМ НАЙ-КРЕХКИ)

| Приоритет | Вид локатор | Пример в Playwright / CSS / XPath | Защо се предпочита или избягва |
| :---: | :--- | :--- | :--- |
| **1 (Златен)** | **`data-testid` атрибут** | `page.getByTestId("submit-order")` / `[data-testid='submit-order']` | Създаден специално за тестове — не се чупи при смяна на CSS класове или превод на текст. |
| **2 (Отличен)** | **ARIA Role & Label** | `page.getByRole("button", { name: "Плати" })` / `page.getByLabel("Имейл")` | Тества приложението точно както го вижда реалният потребител и гарантира достъпност (Accessibility). |
| **3 (Добър)** | **Уникално ID / Name** | `#login-email` / `input[name='username']` | Бърз и директен, стига ID-то да не е динамично генерирано (напр. `#btn-94821`). |
| **4 (Среден)** | **CSS Селектор** | `form.checkout > button.btn-primary` | По-бърз и четим от XPath, но може да се счупи при редизайн на класовете. |
| **5 (Избягвай!)** | **Абсолютен XPath** | `/html/body/div[2]/div/form/div[3]/button` | **Анти-шаблон!** Чупи се при добавяне на един единствен `<div>` в страницата. |

---

## 2. КАТЕГОРИЯ 2: HTTP СТАТУС КОДОВЕ И API / GRAPHQL ВАЛИДАЦИОННИ ПРАВИЛА

| Код | Значение | Какво проверява QA инженерът |
| :---: | :--- | :--- |
| **`200 OK`** | Успешен `GET` / `PUT` / `GraphQL` | Провери JSON схемата, типовете на полетата и **при GraphQL провери, че `body.errors` липсва!** |
| **`201 Created`** | Успешно създаден ресурс (`POST`) | Провери, че отговорът връща новото `id` и хедър `Location: /api/resource/{id}`. |
| **`204 No Content`** | Успешен `DELETE` / `PUT` без тяло | Провери, че тялото на отговора е напълно празно (`content.length === 0`). |
| **`400 Bad Request`** | Невалидни входни данни | Провери при празни задължителни полета, отрицателни числа и невалиден JSON формат. |
| **`401 Unauthorized`** | Липсващ или изтекъл Token | Провери заявка без `Authorization` хедър или с изтекъл/подправен JWT. |
| **`403 Forbidden`** | Няма права за този ресурс | Провери достъп на обикновен `USER` до ендпойнт на `ADMIN` или чужд профил (**IDOR** тест). |
| **`404 Not Found`** | Ресурсът не съществува | Провери с несъществуващо ID (напр. `/api/users/999999`) или след успешен `DELETE`. |
| **`409 Conflict`** | Конфликт на състояние / Дубликат | Провери при регистрация с вече съществуващ имейл или нарушен уникален ключ. |
| **`429 Too Many Requests`** | Превишен Rate Limit | Провери дали API-то защитава `/login` при 100 бързи последователни заявки. |
| **`500 Internal Server Error`** | Необработена сървърна грешка | **Винаги е бъг!** Никакъв потребителски вход не трябва да води до `500` (и никога да не връща Stack Trace!). |

---

## 3. КАТЕГОРИЯ 3: TEST DESIGN ТЕХНИКИ (БЪРЗА ФОРМУЛА ЗА ИЗБОР)

*   **Входни полета с числови/текстови диапазони (напр. 18–65 г.):** Използвай **Equivalence Partitioning (EP)** + **Boundary Value Analysis (BVA)** (`17`, `18`, `19` и `64`, `65`, `66`).
*   **Сложни бизнес правила с 3+ комбинирани условия (напр. VIP + Промо код + Безплатна доставка):** Използвай **Decision Table Testing** ($2^N$ правила).
*   **Процеси с фиксирани етапи (напр. Поръчка, Банкова сметка, Билет):** Използвай **State Transition Testing** (валидни + забранени преходи).
*   **Няма детайлна документация / Спешен релийз:** Използвай **Exploratory Testing** (сесии от 45–60 мин. с конкретна харта / *Test Charter*) + **Error Guessing** (празни полета, специални символи `' " <script>`, 0, отрицателни числа).

---

## 4. КАТЕГОРИЯ 4: ВАЖНИ QA БИБЛИОТЕКИ И ФРЕЙМУЪРЦИ (ИНСТАЛАЦИЯ, ИЗВИКВАНЕ И ПРИЛОЖЕНИЕ)

| Инструмент / Библиотека | Език | Инсталация | Импорт и Извикване (Синтаксис) | За какво се използва |
| :--- | :--- | :--- | :--- | :--- |
| **`Playwright`** | TS / JS / C# / Python | `npm init playwright@latest` | `import { test, expect } from "@playwright/test";`<br>`await page.getByRole("button").click();` | Модерен E2E UI & API фреймуърк с Auto-Waiting, Network Mocking и Trace Viewer. |
| **`Selenium WebDriver`** | C# / Java / Python / JS | `dotnet add package Selenium.WebDriver` | `IWebDriver driver = new ChromeDriver();`<br>`driver.FindElement(By.CssSelector("#id"));` | Класически крос-браузър WebDriver стандарт за легаси и корпоративни проекти. |
| **`Appium`** | Python / Java / C# / JS | `npm i -g appium` + `pip install Appium-Python-Client` | `webdriver.Remote("http://127.0.0.1:4723", options)` | Автоматизация на Native, Hybrid и Mobile Web приложения за Android и iOS. |
| **`RestSharp`** | C# (.NET) | `dotnet add package RestSharp` | `var client = new RestClient(url);`<br>`var res = await client.ExecuteAsync(req);` | Тестване на REST и GraphQL API-та в комбинация с NUnit / xUnit. |
| **`REST Assured`** | Java | `io.rest-assured:rest-assured` (Maven) | `given().when().get("/api/users").then().statusCode(200);` | BDD (Given-When-Then) библиотека за тестване и JSON валидация на REST/GraphQL API-та в Java. |
| **`NUnit` + `Moq` + `FluentAssertions`** | C# (.NET) | `dotnet add package NUnit Moq FluentAssertions` | `result.Should().Be(10);` | Компонентно и интеграционно тестване с четими Fluent проверки и мокове. |
| **`JUnit 5` + `Mockito` + `Testcontainers`** | Java | `spring-boot-starter-test` + `testcontainers` | `@Test @Mock`<br>`assertEquals(expected, actual);` | Unit тестове и реални интеграционни тестове с временни Docker бази данни. |
| **`pytest` + `httpx`** | Python | `pip install pytest httpx` | `def test_api(): assert httpx.get(url).status_code == 200` | Най-гъвкавият фреймуърк за Python Unit, API и UI (с `pytest-playwright`) тестове. |
| **`Grafana K6`** | JavaScript / TypeScript | `winget install k6` / `brew install k6` | `import http from "k6/http";`<br>`export default function () { http.get(url); }` | Модерен, лек и програмируем инструмент за Load, Stress и Spike Performance тестове. |

---

## 5. КАТЕГОРИЯ 5: НАЙ-ВАЖНИТЕ CLI КОМАНДИ ЗА QA ИНЖЕНЕРИ (PLAYWRIGHT, .NET, MAVEN, PYTEST, K6, JMETER)

| Платформа | Команда в терминала | Какво прави |
| :--- | :--- | :--- |
| **Playwright (TS)** | `npx playwright test --ui` | Отваря интерактивния UI режим с Time-Travel дебъгване на всяка стъпка |
| **Playwright (TS)** | `npx playwright codegen https://example.com` | Записва кликовете в браузъра и генерира автоматично Page Object / Test код |
| **Playwright (TS)** | `npx playwright show-trace trace.zip` | Отваря пълния запис от CI/CD с мрежови заявки, конзола и DOM снимки |
| **C# (.NET / NUnit)** | `dotnet test --filter "Category=Smoke" --logger "trx"` | Изпълнява само тестовете от категория `Smoke` и записва XML/TRX отчет |
| **Java (Maven / JUnit)** | `mvn test -Dtest=OrderServiceTest` | Изпълнява конкретен тестов клас в Maven проекта |
| **Python (`pytest`)** | `pytest -v -n auto --html=report.html` | Пуска тестовете паралелно на всички CPU ядра (`pytest-xdist`) и генерира HTML отчет |
| **Grafana K6** | `k6 run --vus 50 --duration 30s load-test.js` | Симулира 50 едновременни виртуални потребители за 30 секунди |
| **Apache JMeter** | `jmeter -n -t plan.jmx -l out.jtl -e -o ./report` | Изпълнява натоварващ тест в конзолен режим (без GUI) и генерира HTML графики |

---
[⬅️ 01-96. QA Performance & Optimizations](01-96-Performance-And-Optimizations.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-98. QA Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md)
