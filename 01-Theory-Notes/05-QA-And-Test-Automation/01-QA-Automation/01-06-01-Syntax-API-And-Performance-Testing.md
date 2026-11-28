[⬅️ 01-06-00. REST API, GraphQL, Contract & Load Testing](01-06-00-API-And-Performance-Testing.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-96. QA Automation Оптимизации: Кое е грешно, защо...](01-96-Performance-And-Optimizations.md)

---

# 01-06-01. Postman, RestSharp, REST Assured & K6: Синтаксис от Нулата & Cheat Sheet

**Раздел 04:** API & Performance Testing | **Цел:** Готови, копируеми скриптове от нулата за писане на автоматизирани API тестове в **Postman (JavaScript pm.test)**, **C# (RestSharp)**, **Java (REST Assured)** и нагрузочни тестове с **Grafana K6 (Stages & Thresholds)**, плюс обобщена 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Postman Tests (JavaScript pm.test, Променливи & JSON Schema)](#1-postman-tests-javascript-pmtest-променливи--json-schema)
* [2. C# (.NET): RestSharp API Тест (GET, POST, JWT Auth & Deserialization)](#2-c-net-restsharp-api-тест-get-post-jwt-auth--deserialization)
* [3. Java: REST Assured BDD Синтаксис (given / when / then)](#3-java-rest-assured-bdd-синтаксис-given--when--then)
* [4. Grafana K6: Load & Stress Тест със Stages, Checks и Thresholds](#4-grafana-k6-load--stress-тест-със-stages-checks-и-thresholds)
* [5. Обобщение / Бърз Cheat Sheet (HTTP Статуси, API & Performance)](#5-обобщение--бърз-cheat-sheet-http-статуси-api--performance)

---

## 1. POSTMAN TESTS (JAVASCRIPT PM.TEST, ПРОМЕНЛИВИ & JSON SCHEMA)

В таба **Scripts ➔ Post-response** на Postman валидираме отговора и предаваме токени към следващите заявки в колекцията (изпълнявана в CI/CD чрез **Newman**):

```javascript
// 1. Проверка на HTTP статус код и време за отговор
pm.test("Status code is 200 OK", function () {
    pm.response.to.have.status(200);
});

pm.test("Response time is under 300ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(300);
});

// 2. Парсване на JSON отговора и проверка на стойности
const jsonData = pm.response.json();

pm.test("Response contains valid accessToken and user role", function () {
    pm.expect(jsonData).to.have.property("accessToken");
    pm.expect(jsonData.user.role).to.eql("SDET");
});

// 3. Запазване на токена в Environment променлива за следващите заявки
pm.environment.set("authToken", jsonData.accessToken);
```

---

## 2. C# (.NET): RESTSHARP API ТЕСТ (GET, POST, JWT AUTH & DESERIALIZATION)

```csharp
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

[TestFixture]
public class ProductsApiTests
{
    private RestClient _client;

    [SetUp]
    public void Setup()
    {
        var options = new RestClientOptions("https://devlib.local")
        {
            Authenticator = new JwtAuthenticator("eyJhbGciOiJIUzI1NiIs...")
        };
        _client = new RestClient(options);
    }

    [Test]
    public async Task GetProductById_ExistingId_ReturnsProductDetails()
    {
        var request = new RestRequest("/api/v1/products/{id}", Method.Get)
            .AddUrlSegment("id", 101);

        var response = await _client.ExecuteAsync<ProductDto>(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Data?.Name, Is.EqualTo("Mechanical Keyboard"));
    }

    [TearDown]
    public void TearDown() => _client.Dispose();
}
```

---

## 3. JAVA: REST ASSURED BDD СИНТАКСИС (GIVEN / WHEN / THEN)

```java
import io.restassured.http.ContentType;
import org.junit.jupiter.api.Test;

import static io.restassured.RestAssured.*;
import static org.hamcrest.Matchers.*;

class OrdersApiTest {

    @Test
    void createOrder_ValidRequest_Returns201AndOrderId() {
        given()
            .baseUri("https://devlib.local")
            .header("Authorization", "Bearer " + token)
            .contentType(ContentType.JSON)
            .body("""
                  { "productId": 42, "quantity": 2 }
                  """)
        .when()
            .post("/api/v1/orders")
        .then()
            .statusCode(201)
            .contentType(ContentType.JSON)
            .body("orderId", greaterThan(0))
            .body("status", equalTo("CONFIRMED"));
    }
}
```

---

## 4. GRAFANA K6: LOAD & STRESS ТЕСТ СЪС STAGES, CHECKS И THRESHOLDS

Стартира се от терминала с `k6 run load-test.js`:

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    // Плавна рампа на виртуалните потребители (Ramp-up -> Steady -> Ramp-down)
    stages: [
        { duration: '30s', target: 50 },  // Вдига до 50 потребители за 30 сек.
        { duration: '1m',  target: 50 },  // Задържа 50 потребители за 1 мин.
        { duration: '15s', target: 0 },   // Сваля до 0 потребители
    ],
    // Автоматични критерии за успех/провал в CI/CD
    thresholds: {
        http_req_duration: ['p(95)<250'], // 95% от заявките трябва да са под 250ms
        http_req_failed: ['rate<0.01'],   // По-малко от 1% грешки
    },
};

export default function () {
    const res = http.get('https://devlib.local/api/v1/products');

    check(res, {
        'status is 200': (r) => r.status === 200,
        'body is not empty': (r) => r.body.length > 0,
    });

    sleep(1); // Пауза (Think Time) между действията на потребителя
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET (HTTP СТАТУСИ, API & PERFORMANCE)

| Категория / Инструмент | Команда / Код | Значение и Приложение |
| :--- | :--- | :--- |
| **HTTP `200` / `201` / `204`** | `OK` / `Created` / `No Content` | Успешно четене (`200`), създаване (`201`), изтриване без тяло (`204`). |
| **HTTP `400` / `401` / `403` / `404`** | Клиентски грешки | Невалидни данни (`400`), нелогнат (`401`), без права (`403`), липсващ ресурс (`404`). |
| **HTTP `429` / `500` / `503`** | Rate Limit & Сървърни грешки | Твърде много заявки (`429`), срив в кода (`500`), претоварен сървър (`503`). |
| **Newman (Postman CLI)** | `newman run collection.json -e env.json` | Пуска Postman тестове в конзолата и CI/CD конвейери. |
| **K6 Стартиране** | `k6 run --vus 100 --duration 30s test.js` | Изпълнява нагрузочен тест със 100 едновременни виртуални потребители. |
| **JMeter Non-GUI (CI/CD)** | `jmeter -n -t plan.jmx -l results.jtl -e -o report/` | Винаги пускай JMeter в конзолен режим (`-n`) при реални тестове (никога в GUI!). |







---

[⬅️ 01-06-00. REST API, GraphQL, Contract & Load Testing](01-06-00-API-And-Performance-Testing.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-96. QA Automation Оптимизации: Кое е грешно, защо...](01-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 01-06-01. Postman, RestSharp, REST Assured & K6: Синтаксис от Нулата & Cheat Sheet
