[⬅️ 04-00. Intro to Spring Boot & Web Architecture](04-00-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-01-01. Syntax From Scratch](04-01-01-Syntax-From-Scratch.md)

---

# 04-01-00. Internet Explained & HTTP Protocol (Теория, Методи, Статус кодове & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 01:** Как работи Интернет под капака (DNS ➔ TCP 3-Way Handshake ➔ TLS/HTTPS ➔ HTTP), структура на HTTP Request и HTTP Response, семантика на HTTP методите (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`), свойствата **Safe** и **Idempotent**, групите статус кодове (`1xx`–`5xx`), CORS и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Пътят на една HTTPS заявка от браузъра до Spring Boot (Блок-схема)](#2-пътят-на-една-https-заявка-от-браузъра-до-spring-boot-блок-схема)
* [3. Сравнителна карта на HTTP клиенти в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-на-http-клиенти-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на HTTP/1.1 срещу HTTP/2 и HTTP/3](#4-плюсове-и-минуси-на-http11-срещу-http2-и-http3)
* [5. Кодов одит: Правилен срещу Грешен код (`PUT` срещу `PATCH` и грешни HTTP статус кодове)](#5-кодов-одит-правилен-срещу-грешен-код-put-срещу-patch-и-грешни-http-статус-кодове)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (`HttpClient` / `RestClient`) срещу C# 12+ (`HttpClient`)](#7-архитектурно-сравнение-java-21-httpclient--restclient-срещу-c-12-httpclient)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Петте семейства HTTP статус кодове (Първата цифра казва всичко!):**
    *   **`1xx` (Informational):** *„Изчакай, още говоря“* (`101 Switching Protocols` при WebSockets).
    *   **`2xx` (Success):** *„Всичко мина успешно!“* (`200 OK`, `201 Created` при нов запис, `204 No Content` при успешно изтриване).
    *   **`3xx` (Redirection):** *„Преместихме се на друг адрес!“* (`301 Moved Permanently`, `302 Found`).
    *   **`4xx` (Client Error — ТИ като клиент си сбъркал!):** `400 Bad Request` (невалиден JSON/данни), `401 Unauthorized` (не си влязъл в профила си), `403 Forbidden` (влязъл си, но нямаш админ права), `404 Not Found` (няма такъв URL/ресурс).
    *   **`5xx` (Server Error — НИЕ на сървъра сме сбъркали!):** `500 Internal Server Error` (нехванато изключение в Java), `502 Bad Gateway`, `503 Service Unavailable`.
*   **Идемпотентност (Idempotency) = Бутонът на асансьора:** Да натиснеш бутона за 5-ти етаж 1 път или 10 пъти подред води до **един и същ краен резултат** (асансьорът отива на 5-ти етаж).
    *   **Idempotent методи:** `GET`, `PUT`, `DELETE`, `HEAD`, `OPTIONS`.
    *   **НЕ-Idempotent метод:** **`POST`** (ако изпратиш `POST /orders` 3 пъти, ще направиш 3 поръчки и ще изтеглиш парите 3 пъти!).

---

## 2. ПЪТЯТ НА ЕДНА HTTPS ЗАЯВКА ОТ БРАУЗЪРА ДО SPRING BOOT (БЛОК-СХЕМА)

```text
[ Браузър: https://shop.bg ]
       │
       ▼ 1. DNS Lookup ➔ Превежда "api.shop.bg" в IP адрес (напр. 185.12.44.10)
       ▼ 2. TCP 3-Way Handshake ➔ SYN ➔ SYN-ACK ➔ ACK (Отваря надежден канал)
       ▼ 3. TLS Handshake ➔ Проверка на SSL сертификат и договаряне на криптиране
       ▼ 4. Изпраща криптиран HTTP Request:
┌─────────────────────────────────────────────────────────────────────────┐
│ GET /products/5 HTTP/1.1                                                │
│ Host: api.shop.bg                                                       │
│ Accept: application/json                                                │
│ Authorization: Bearer eyJhbGciOi...                                     │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Spring Boot (Tomcat Port 8080) връща HTTP Response:                     │
│ HTTP/1.1 200 OK                                                         │
│ Content-Type: application/json                                          │
│                                                                         │
│ {"id": 5, "name": "Mechanical Keyboard", "price": 180.00}               │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА НА HTTP КЛИЕНТИ В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Действие | ☕ Java 21+ (`HttpClient` / `RestClient`) | 🟣 C# 12+ (`HttpClient`) | 🐍 Python 3.x (`httpx` / `requests`) | 🟡 JavaScript (`fetch`) |
| :--- | :--- | :--- | :--- | :--- |
| **Синхронна GET заявка** | `restClient.get().uri(url).retrieve().body(String.class)` | `await http.GetStringAsync(url)` | `httpx.get(url).json()` | `await (await fetch(url)).json()` |
| **POST с JSON тяло** | `restClient.post().uri(url).body(dto).retrieve()` | `await http.PostAsJsonAsync(url, dto)` | `httpx.post(url, json=data)` | `fetch(url, { method: "POST", body: ... })` |
| **Добавяне на Bearer Token** | `.header("Authorization", "Bearer " + token)` | `http.DefaultRequestHeaders.Authorization = ...` | `headers={"Authorization": f"Bearer {t}"}` | `headers: { Authorization: ... }` |
| **Вграден нативен клиент** | `java.net.http.HttpClient` (от Java 11+) | `System.Net.Http.HttpClient` | `urllib.request` (ползва се `httpx`) | Нативен `fetch()` (Node 18+ / Browser) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА HTTP/1.1 СРЕЩУ HTTP/2 И HTTP/3

### 🟢 Предимства на HTTP/2 и HTTP/3:
*   **Multiplexing (Мултиплексиране):** В HTTP/1.1 браузърът отваря до 6 отделни TCP връзки и чака всяка заявка на опашка (*Head-of-Line Blocking*). HTTP/2 изпраща десетки паралелни заявки и отговори **едновременно по една единствена TCP връзка** в бинарен формат + компресия на заглавията (HPACK).
*   **HTTP/3 (върху QUIC / UDP):** Елиминира забавянето при загуба на пакет в мобилни мрежи, защото преминава от TCP към **QUIC (UDP)**.

### 🔴 Недостатъци на HTTP като протокол:
*   **Stateless (Без памет за състоянието):** Всяка HTTP заявка е напълно независима от предишната — сървърът не помни кой си, освен ако не изпратиш идентификатор при **всяка заявка** (`Cookie` сесия или `Authorization: Bearer <JWT>`).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`PUT` СРЕЩУ `PATCH` И ГРЕШНИ HTTP СТАТУС КОДОВЕ)

### ❌ ГРЕШЕН КОД (Връщане на `200 OK` при грешка и ползване на `GET` за изтриване):
```java
// 🚨 ГРЕШКА 1: Никога не променяй и не трий данни чрез GET заявка!
// Търсачките (Googlebot) и браузърите предварително зареждат GET линкове (Prefetch)
// и могат случайно да изтрият всички записи в базата ти!
@GetMapping("/users/delete/{id}")
public String deleteUser(@PathVariable Long id) {
    userService.delete(id);
    return "Deleted";
}

// 🚨 ГРЕШКА 2: Връщане на статус 200 OK с текст {"error": "Not found"}!
// Мониторинг системите и фронтенд клиентите мислят, че заявката е успешна!
```
*   **Как да се избегне:** Спазвай строго HTTP семантиката: **`GET`** само чете (Safe); **`POST`** създава нов ресурс (`201 Created`); **`PUT`** заменя целия ресурс; **`PATCH`** обновява само 1–2 полета (напр. само смяна на парола или статус); **`DELETE`** трие ресурс (`204 No Content`).

### 🟢 ПРАВИЛЕН КОД (Коректни HTTP методи и статус кодове в Spring):
```java
@RestController
@RequestMapping("/api/v1/products")
public class ProductApiController {

    private final ProductService productService;

    public ProductApiController(ProductService productService) {
        this.productService = productService;
    }

    @PostMapping
    public ResponseEntity<ProductDto> create(@RequestBody @Valid CreateProductDto dto) {
        ProductDto created = productService.create(dto);
        // Връща статус 201 Created + заглавие Location: /api/v1/products/42
        URI location = URI.create("/api/v1/products/" + created.id());
        return ResponseEntity.created(location).body(created);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        productService.deleteById(id);
        // Връща статус 204 No Content (успешно изтрито, без тяло)
        return ResponseEntity.noContent().build();
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е точната разлика между HTTP статус кодове **`401 Unauthorized`** и **`403 Forbidden`**?“*
*   **Уловката:** Името `401 Unauthorized` подвежда кандидатите, защото думата *Unauthorized* звучи като „нямаш права (авторизация)“!
*   **Точен Senior отговор:** 
    1.  **`401 Unauthorized` всъщност означава `Unauthenticated` (Неразпознат / Неавтентикиран):** Сървърът изобщо не знае **кой си** (липсва или е изтекъл JWT токенът / сесийната бисквитка). Ако се логнеш, заявката може да успее.
    2.  **`403 Forbidden` означава `Unauthorized` (Забранен достъп):** Сървърът **знае точно кой си** (логнат си успешно като обикновен `USER`), но **нямаш нужната роля/права** за този ресурс (напр. опитваш да отвориш `/api/admin/users`). Повторно логване няма да помогне.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (`HTTPCLIENT` / `RESTCLIENT`) СРЕЩУ C# 12+ (`HTTPCLIENT`)

| Архитектурен детайл | ☕ Java 21+ (`java.net.http` / Spring `RestClient`) | 🟣 C# 12+ (`System.Net.Http.HttpClient`) |
| :--- | :--- | :--- |
| **Нативен HTTP/2 клиент в езика** | `java.net.http.HttpClient` (от Java 11+, поддържа Sync & Async `CompletableFuture`) | `System.Net.Http.HttpClient` (изцяло асинхронен `async/await`) |
| **Модерен синхронен Spring клиент** | **`RestClient`** (въведен в Spring Boot 3.2 — замества стария `RestTemplate`) | `HttpClient` + `System.Net.Http.Json` (`GetFromJsonAsync<T>`) |
| **Реактивен / Non-blocking клиент** | **`WebClient`** (от Spring WebFlux) | `HttpClient` с `HttpCompletionOption.ResponseHeadersRead` |
| **Декларативен HTTP интерфейс** | ``**@HttpExchange**`` (Spring 6+ — пишеш само интерфейс, Spring генерира клиента!) | **Refit** библиотека (интерфейс с [``Get("/users/{id}")``]) |
| **Повторна употреба на клиента** | Създава се веднъж (Singleton Bean) и се преизползва (има собствен Connection Pool) | Създава се чрез ``IHttpClientFactory`` (за да се избегне Socket Exhaustion) |

---
[⬅️ 04-00. Intro to Spring Boot & Web Architecture](04-00-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-01-01. Syntax From Scratch](04-01-01-Syntax-From-Scratch.md)