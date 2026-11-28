[⬅️ 01-05-01. Playwright & Appium Cheat Sheet](01-05-01-Syntax-Playwright-AJAX-And-Appium.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-06-01. Postman, RestSharp, REST Assured & K6](01-06-01-Syntax-API-And-Performance-Testing.md)

---

# 01-06-00. REST API, GraphQL, Contract & Load Testing (K6 & JMeter)

**Раздел 04:** API & Performance Testing | **Цел:** Овладяване на автоматизираното тестване на програмни интерфейси (**RESTful APIs**, **GraphQL** и **Consumer-Driven Contract Testing с Pact**), валидиране на JSON схеми и статуси, както и проектиране на тестове за производителност (**Load, Stress, Spike и Soak Testing**) с **Grafana K6** и **Apache JMeter**.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Idempotency, Contract & Performance Видове)](#1-как-да-се-запомни-по-лесно-мнемоника-idempotency-contract--performance-видове)
* [2. Архитектура на API & Performance тестването (Блок-схема)](#2-архитектура-на-api--performance-тестването-блок-схема)
* [3. Сравнителни матрици: REST срещу GraphQL & Видове Performance тестове](#3-сравнителни-матрици-rest-срещу-graphql--видове-performance-тестове)
* [4. Плюсове и минуси на API и Contract тестването спрямо UI E2E](#4-плюсове-и-минуси-на-api-и-contract-тестването-спрямо-ui-e2e)
* [5. Кодов одит: Грешен (Проверка само на HTTP 200) срещу Правилен API тест](#5-кодов-одит-грешен-проверка-само-на-http-200-срещу-правилен-api-тест)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: IDEMPOTENCY, CONTRACT & PERFORMANCE ВИДОВЕ)

* **Idempotency (Идемпотентност — Повторение без странични ефекти):**
  * **`GET`, `PUT`, `DELETE` са идемпотентни:** Ако изпратиш `DELETE /api/users/5` един път или 10 пъти подред, крайното състояние на базата данни е едно и също (потребител №5 е изтрит).
  * **`POST` и `PATCH` НЕ са идемпотентни:** Ако изпратиш `POST /api/orders` 5 пъти, ще създадеш **5 дублирани поръчки** и ще таксуваш клиента 5 пъти!
* **Contract Testing (Договорът между Клиент и Сървър — Pact):** В микросървис архитектурите E2E тестовете са бавни и сложни за вдигане. **Contract Testing** проверява дали *Consumer* (фронтендът) и *Provider* (бекенд API-то) спазват общ JSON договор (имена на полета и типове данни) в изолация, без да стартираме цялата система.
* **Четирите основни вида Performance тестове:**
  * **Load Testing:** Очакваното нормално натоварване (напр. `200` виртуални потребители за 10 минути).
  * **Stress Testing:** Постепенно увеличаване на товара до **точката на пречупване (Breaking Point)**, за да видим кога сървърът ще падне и как се възстановява.
  * **Spike Testing:** Внезапен рязък скок (напр. от `50` на `5 000` потребители за 10 секунди при Черен петък).
  * **Soak (Endurance) Testing:** Умерено натоварване за **много дълго време** (8–24 часа), за да открием **Memory Leaks** (изтичане на RAM памет) и изчерпване на SQL връзките.

---

## 2. АРХИТЕКТУРА НА API & PERFORMANCE ТЕСТВАНЕТО (БЛОК-СХЕМА)

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  API & CONTRACT TESTING (Бърза проверка на бизнес логиката без UI)      │
 │  [Postman / RestSharp / REST Assured] ──(HTTP Request + JWT)──► [API]   │
 │  • Проверява: 1) Status Code  2) Headers  3) JSON Schema  4) БД съст.  │
 ├─────────────────────────────────────────────────────────────────────────┤
 │  PERFORMANCE & LOAD TESTING (Grafana K6 / Apache JMeter)                │
 │  [Virtual Users (VUs) Engine] ══(Хиляди паралелни заявки/сек)══► [API]  │
 │  • Измерва: p95 / p99 Latency (< 200ms), Throughput (RPS) и Error Rate  │
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: REST СРЕЩУ GRAPHQL & ВИДОВЕ PERFORMANCE ТЕСТОВЕ

### 📊 А. REST API срещу GraphQL при автоматизирано тестване

| Критерий | RESTful API | GraphQL API |
| :--- | :--- | :--- |
| **Крайни точки (Endpoints)** | Множество URL ресурси (`/users`, `/orders/5`). | **Един-единствен Endpoint** (`POST /graphql`). |
| **HTTP Методи и Статуси** | Използва `GET`, `POST`, `PUT`, `DELETE` и детайлни кодове (`201`, `404`, `422`). | Почти винаги връща **`HTTP 200 OK`**, дори при бизнес или валидационна грешка! |
| **Проверка на грешки в теста** | Проверяваме `response.StatusCode == 400 / 404`. | Задължително проверяваме дали масивът **`errors` в JSON отговора** е празен/липсва! |
| **Over-fetching / Under-fetching** | Сървърът връща фиксиран JSON модел. | Клиентът заявява само нужните полета чрез `query` или променя данни чрез `mutation`. |

### 📊 Б. Сравнение на Performance инструменти: Grafana K6 срещу Apache JMeter

| Характеристика | ⚡ Grafana K6 | ☕ Apache JMeter |
| :--- | :--- | :--- |
| **Език за писане на тестове** | Модерен **JavaScript / TypeScript** (Developer-friendly). | Визуален GUI интерфейс + XML (`.jmx` файлове). |
| **Консумация на RAM и CPU** | 🚀 **Изключително ниска** (написан на **Go** с леки Goroutines). | 🔴 **Висока** (Java JVM заделя отделна нишка за всеки потребител). |
| **CI/CD Интеграция** | Вградени **Thresholds** (тестът пада с exit code `99`, ако `p95 > 200ms`). | Изисква външни плъгини за автоматично оценяване на прагове. |
| **Поддържани протоколи** | HTTP/1.1, HTTP/2, WebSockets, gRPC, Browser. | HTTP, JDBC (директно SQL), FTP, JMS, SMTP, TCP. |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА API И CONTRACT ТЕСТВАНЕТО СПРЯМО UI E2E

* **🟢 Плюсове:** Изпълняват се **50–100 пъти по-бързо** от Selenium/Playwright тестовете (`20ms` срещу `3000ms`), не се чупят от промяна на CSS класове и позволяват лесно тестване на сигурност (401/403 права, SQL Injection, Rate Limiting).
* **🔴 Минуси:** Не проверяват визуалното рендериране в браузъра, CSS стиловете или клиентската JavaScript валидация.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (ПРОВЕРКА САМО НА HTTP 200) СРЕЩУ ПРАВИЛЕН API ТЕСТ

### ❌ ГРЕШЕН КОД (Плитък тест — проверява само статус кода):
```csharp
[Test]
public async Task CreateOrder_BadTest()
{
    var response = await _client.ExecutePostAsync(request);
    // 🚨 ГРЕШКА: Ако сървърът върне 200 OK с празен JSON или грешна цена, тестът минава!
    Assert.That((int)response.StatusCode, Is.EqualTo(200));
}
```

### 🟢 ПРАВИЛЕН КОД (Дълбока проверка на статус, Content-Type, JSON тяло и странични ефекти):
```csharp
[Test]
public async Task CreateOrder_ValidPayload_Returns201AndPersistsOrder()
{
    var request = new RestRequest("/api/v1/orders", Method.Post)
        .AddHeader("Authorization", \$"Bearer {_jwtToken}")
        .AddJsonBody(new { productId = 42, quantity = 2 });

    var response = await _client.ExecuteAsync<OrderResponseDto>(request);

    // 1. Точен HTTP статус за създаден ресурс (201 Created)
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    // 2. Проверка на заглавките (Location header & JSON тип)
    Assert.That(response.ContentType, Does.Contain("application/json"));
    // 3. Проверка на десериализираните данни в тялото
    Assert.That(response.Data, Is.Not.Null);
    Assert.That(response.Data!.OrderId, Is.GreaterThan(0));
    Assert.That(response.Data.TotalPrice, Is.EqualTo(199.98m));
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Защо при Performance тестване използваме персентили като **`p95` и `p99`**, вместо средно аритметично време за отговор (**Average Response Time**)?“*
  * **Отговор:** Средната стойност крие тежките аномалии (Outliers). Ако 99 заявки се изпълнят за `50ms`, а 1 заявка забие за `10 000ms` (10 секунди), средното време е `149ms` (изглежда отлично!), но **1% от реалните клиенти чакат 10 секунди**! Метриката **`p95 = 200ms`** гарантира, че 95% от всички заявки са обслужени под `200ms`.
* **Въпрос 2:** *„Каква е разликата между **`401 Unauthorized`** и **`403 Forbidden`** при тестване на API сигурност?“*
  * **Отговор:** **`401 Unauthorized`** означава *„Не знаем кой си“* (липсващ, невалиден или изтекъл JWT токен — липсва **Authentication**). **`403 Forbidden`** означава *„Знаем кой си, но нямаш права за този ресурс“* (напр. обикновен потребител с валиден токен се опитва да извика `DELETE /api/admin/users` — отказана **Authorization**).

---

[⬅️ 01-05-01. Playwright & Appium Cheat Sheet](01-05-01-Syntax-Playwright-AJAX-And-Appium.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-06-01. Postman, RestSharp, REST Assured & K6](01-06-01-Syntax-API-And-Performance-Testing.md)

© 2026 Dev-Library / 01-06-00. REST API, GraphQL, Contract & Load Testing (K6 & JMeter)
