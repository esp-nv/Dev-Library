[⬅️ 01-06. Modern UI Automation: Playwright & AJAX](01-06-Modern-UI-Automation-Playwright-And-AJAX.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-08. Jira, JQL, TestRail & CI/CD Platforms](01-08-Jira-JQL-TestRail-And-CICD-Platforms.md)

---

# 01-07. Back-End API Testing (RESTful & GraphQL) & Performance Testing (JMeter & K6)

**Цел:** Автоматизирано тестване на сървърни API интерфейси — как се проверяват **RESTful API** контракти и **GraphQL** заявки (`Queries` & `Mutations`, уловката с HTTP `200 OK` при GraphQL грешки) чрез **Postman**, **RestSharp** (C#), **REST Assured** (Java) и **Mocha + Chai** (JS/TS), както и измерване на натоварване с **Grafana K6** и **Apache JMeter**.

## 🧭 Съдържание на темата
* [1. RESTful API срещу GraphQL при тестване (Ключови разлики и уловки)](#1-restful-api-срещу-graphql-при-тестване-ключови-разлики-и-уловки)
* [2. Postman тестове (REST & GraphQL Assertions в `pm.test`)](#2-postman-тестове-rest--graphql-assertions-в-pmtest)
* [3. Кодово API тестване: RestSharp (C#), REST Assured (Java) & Mocha/Chai (JS)](#3-кодово-api-тестване-restsharp-c-rest-assured-java--mochachai-js)
* [4. Performance & Load Testing: Grafana K6 & Apache JMeter](#4-performance--load-testing-grafana-k6--apache-jmeter)

---

## 1. RESTFUL API СРЕЩУ GRAPHQL ПРИ ТЕСТВАНЕ (КЛЮЧОВИ РАЗЛИКИ И УЛОВКИ)

| Критерий | 🌐 RESTful API | ◈ GraphQL API |
| :--- | :--- | :--- |
| **Ендпойнти** | Множество URL адреси (`/api/users`, `/api/orders/5`) | **Един единствен ендпойнт** (`POST /graphql`) |
| **HTTP Глаголи** | `GET`, `POST`, `PUT`, `PATCH`, `DELETE` | Почти винаги **`POST`** с JSON тяло `{"query": "...", "variables": {...}}` |
| **HTTP Статус кодове при грешка** | `400 Bad Request`, `401`, `403`, `404 Not Found`, `500` | **УЛОВКА:** Често връща **`200 OK`**, а грешката е вътре в масива `"errors": [...]` в JSON тялото! |
| **Какво проверява QA инженерът** | Status Code, Headers (`Content-Type`), JSON Schema, Idempotency на `PUT`/`DELETE` | Status `200`, липса на `errors` поле (`expect(body.errors).toBeUndefined()`), точни полета в `data` |

---

## 2. POSTMAN ТЕСТОВЕ (REST & GRAPHQL ASSERTIONS В `PM.TEST`)

В таба **Scripts ➔ Post-response** на Postman пишем JavaScript проверки върху отговора:

```javascript
// --- 1. Проверка на RESTful POST /api/v1/books (Очакваме 201 Created) ---
pm.test("Status code is 201 Created", function () {
    pm.response.to.have.status(201);
});

pm.test("Response time is under 300ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(300);
});

const jsonData = pm.response.json();
pm.test("Created book has valid ID and title", function () {
    pm.expect(jsonData.id).to.be.a("number");
    pm.expect(jsonData.title).to.eql("Clean Code");
    // Запазваме ID-то в Environment променлива за следващия тест (GET / DELETE)!
    pm.environment.set("createdBookId", jsonData.id);
});

// --- 2. Задължителна проверка при GraphQL отговор ---
pm.test("GraphQL response has NO errors array", function () {
    pm.response.to.have.status(200);
    const gqlResponse = pm.response.json();
    pm.expect(gqlResponse.errors).to.be.undefined;
    pm.expect(gqlResponse.data).to.be.an("object");
});
```

---

---

## 3. КОДОВО API ТЕСТВАНЕ: RESTSHARP (C#), REST ASSURED (JAVA) & MOCHA/CHAI (JS)

### 3.1 C# (`RestSharp` + `NUnit` — REST & GraphQL тест)
```csharp
using System.Net;
using NUnit.Framework;
using RestSharp;

[TestFixture]
public class BooksApiTests
{
    private RestClient _client = null!;

    [SetUp]
    public void Setup() => _client = new RestClient("https://bookstore.bg");

    [Test]
    public async Task CreateBook_ShouldReturn201_AndLocationHeader()
    {
        var request = new RestRequest("/api/v1/books", Method.Post)
            .AddHeader("Authorization", "Bearer YOUR_JWT_TOKEN")
            .AddJsonBody(new { title = "Clean Architecture", price = 54.00 });

        var response = await _client.ExecuteAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Content, Does.Contain("Clean Architecture"));
    }

    [TearDown]
    public void TearDown() => _client.Dispose();
}
```

### 3.2 Java (`REST Assured` — Given / When / Then BDD стил)
```java
import io.restassured.http.ContentType;
import org.junit.jupiter.api.Test;

import static io.restassured.RestAssured.*;
import static org.hamcrest.Matchers.*;

class BookRestAssuredTest {

    @Test
    void getBookById_ShouldReturn200_AndValidJson() {
        given()
            .baseUri("https://bookstore.bg")
            .header("Accept", ContentType.JSON)
        .when()
            .get("/api/v1/books/{id}", 1)
        .then()
            .statusCode(200)
            .time(lessThan(500L))
            .body("id", equalTo(1))
            .body("title", not(emptyOrNullString()));
    }
}
```

### 3.3 JavaScript / Node.js (`Mocha` + `Chai` за GraphQL заявка)
```javascript
import { expect } from "chai";

describe("GraphQL Books API", () => {
    it("fetches book title without GraphQL errors", async () => {
        const query = `
            query GetBook($id: ID!) {
                book(id: $id) { id title price }
            }
        `;

        const res = await fetch("https://bookstore.bg/graphql", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ query, variables: { id: "1" } })
        });

        expect(res.status).to.equal(200);
        const body = await res.json();
        expect(body.errors).to.be.undefined;
        expect(body.data.book.title).to.be.a("string");
    });
});
```

---

## 4. PERFORMANCE & LOAD TESTING: GRAFANA K6 & APACHE JMETER

| Вид тест | Цел на теста | Какво следим |
| :--- | :--- | :--- |
| **Load Testing** | Проверка при нормално очаквано натоварване (напр. 500 потребители едновременно) | Средно време за отговор (`p95 < 200ms`), 0% грешки |
| **Stress Testing** | Натоварване над предела до срив, за да видим **къде** и **как** системата се чупи и възстановява | Точка на пречупване (*Breaking Point*), CPU/RAM лимити |
| **Spike Testing** | Внезапен пик от 50 до 5000 потребители за 10 секунди (напр. Black Friday) | Автоскалиране, Connection Pool изчерпване |
| **Soak (Endurance) Testing** | Умерено натоварване в продължение на 8–24 часа | **Memory Leaks** (постепенно запълване на RAM / GC) |

### Пример: Модерен Performance тест с Grafana K6 (`load-test.js`)
```javascript
import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    stages: [
        { duration: "30s", target: 50 },  // Плавно качване до 50 виртуални потребители (VUs)
        { duration: "1m",  target: 50 },  // Задържане на натоварването 1 минута
        { duration: "15s", target: 0 },   // Плавно сваляне
    ],
    thresholds: {
        http_req_duration: ["p(95)<250"], // 95% от заявките ТРЯБВА да са под 250ms!
        http_req_failed: ["rate<0.01"],   // Под 1% неуспешни заявки
    },
};

export default function () {
    const res = http.get("https://bookstore.bg/api/v1/books");
    check(res, { "status is 200": (r) => r.status === 200 });
    sleep(1);
}
```
*   **Apache JMeter (GUI / CLI):** Използва се в корпоративни среди за сложни протоколи (HTTP, JDBC, JMS, FTP). В продукционен режим винаги се стартира от конзолата (Non-GUI mode), за да не хаби RAM: `jmeter -n -t test_plan.jmx -l results.jtl -e -o ./html-report`.

---
[⬅️ 01-06. Modern UI Automation: Playwright & AJAX](01-06-Modern-UI-Automation-Playwright-And-AJAX.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-08. Jira, JQL, TestRail & CI/CD Platforms](01-08-Jira-JQL-TestRail-And-CICD-Platforms.md)
