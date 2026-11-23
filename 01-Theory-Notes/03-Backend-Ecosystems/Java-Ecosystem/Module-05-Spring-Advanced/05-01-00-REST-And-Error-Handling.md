[⬅️ 05-00. Intro to Spring Advanced](05-00-Intro.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-01-01. Syntax From Scratch](05-01-01-Syntax-From-Scratch.md)

---

# 05-01-00. Web API, REST Controllers & Global Error Handling (Теория & Капани)

**Модул 05:** Spring Advanced | **Лекционна тема 01:** Проектиране на професионални **RESTful API-та** със Spring Boot 3 (`@RestController`, `ResponseEntity<T>`, именуване на ресурси, HATEOAS нива по Richardson Maturity Model), централизирана обработка на грешки чрез **`@RestControllerAdvice`** и **`@ExceptionHandler`**, стандартизиран формат за грешки **RFC 9457 (`ProblemDetail`)** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Global Exception Handling (`@RestControllerAdvice` Блок-схема)](#2-архитектура-на-global-exception-handling-restcontrolleradvice-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot 3, ASP.NET Core 8, FastAPI, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-3-aspnet-core-8-fastapi-nestjs)
* [4. Плюсове и минуси на RFC 9457 `ProblemDetail`](#4-плюсове-и-минуси-на-rfc-9457-problemdetail)
* [5. Кодов одит: Правилен срещу Грешен код (`try-catch` във всеки контролер и глаголи в URL адресите)](#5-кодов-одит-правилен-срещу-грешен-код-try-catch-във-всеки-контролер-и-глаголи-в-url-адресите)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot 3) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot-3-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Златните правила на REST URL дизайна (Съществителни в множествено число, НИКОГА глаголи!):**
    *   URL адресът описва **РЕСУРСА (съществително)**, а HTTP методът описва **ДЕЙСТВИЕТО (глагола)**!
    *   `GET /api/v1/books` ➔ Вземи всички книги.
    *   `GET /api/v1/books/42` ➔ Вземи книга №42.
    *   `POST /api/v1/books` ➔ Създай нова книга.
    *   `PUT /api/v1/books/42` ➔ Обнови изцяло книга №42.
    *   `DELETE /api/v1/books/42` ➔ Изтрий книга №42.
*   **`@RestControllerAdvice` = Глобалният парашут на API-то:** Вместо да пишеш `try-catch` във всеки метод на контролера, пускаш грешките свободно от Service слоя нагоре. `@RestControllerAdvice` ги хваща централизирано и ги превръща в чист JSON по международния стандарт **RFC 9457 (`ProblemDetail`)**.

---

## 2. АРХИТЕКТУРЕН ПОТОК НА GLOBAL EXCEPTION HANDLING (`@RESTCONTROLLERADVICE` БЛОК-СХЕМА)

```text
[ OrderService хвърля ResourceNotFoundException("Поръчка #99 не съществува") ]
                                     │
                                     ▼ (Излиза през Controller без try-catch)
┌─────────────────────────────────────────────────────────────────────────┐
│ @RestControllerAdvice (GlobalExceptionHandler)                          │
│  └── @ExceptionHandler(ResourceNotFoundException.class)                 │
│       • Сглобява ProblemDetail (status: 404, title: "Not Found", ...)   │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ HTTP/1.1 404 Not Found | Content-Type: application/problem+json         │
│ {                                                                       │
│   "type": "https://shop.bg",                       │
│   "title": "Resource Not Found",                                        │
│   "status": 404,                                                        │
│   "detail": "Поръчка #99 не съществува",                                │
│   "instance": "/api/v1/orders/99"                                       │
│ }                                                                       │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT 3, ASP.NET CORE 8, FASTAPI, NESTJS)

| Концепция | ☕ Java 21+ (Spring Boot 3) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (FastAPI) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **REST Контролер клас** | `@RestController` + `@RequestMapping` | `[ApiController]` + `[Route("api/...")]` | `router = APIRouter(prefix="/api/...")` | `@Controller('api/...')` |
| **Четене на JSON тяло** | `@RequestBody @Valid CreateDto dto` | `[FromBody] CreateDto dto` | `dto: CreateDto` (автоматично Pydantic) | `@Body() dto: CreateDto` |
| **Глобален Error Handler** | `@RestControllerAdvice` + `@ExceptionHandler` | `IExceptionHandler` (`UseExceptionHandler`) | `@app.exception_handler(CustomEx)` | `@Catch(HttpException)` Filter |
| **Стандартен Error обект** | `org.springframework.http.ProblemDetail` | `Microsoft.AspNetCore.Mvc.ProblemDetails` | `HTTPException(status_code=404, ...)` | `NotFoundException('...')` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА RFC 9457 `PROBLEMDETAIL`

### 🟢 Предимства (Pros):
*   **Единен договор с фронтенда:** Angular, React и мобилните приложения винаги получават едни и същи JSON полета (`title`, `status`, `detail`, `instance` + допълнителни `properties` като списък с валидационни грешки по полета).
*   **Вграден в Spring Framework 6 / Spring Boot 3:** Не изисква писане на собствени `ApiErrorResponse` класове — `ProblemDetail` вече е вграден и поддържа `.setProperty("errors", fieldErrors)`.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`TRY-CATCH` ВЪВ ВСЕКИ КОНТРОЛЕР И ГЛАГОЛИ В URL АДРЕСИТЕ)

### ❌ ГРЕШЕН КОД (Глаголи в URL-ите и дублиран `try-catch` във всеки метод):
```java
@RestController
@RequestMapping("/api")
public class BadBookController {

    // 🚨 ГРЕШКА 1 (RPC анти-патерн): Глаголи в URL адресите вместо HTTP методи!
    @PostMapping("/getAllBooks")
    public List<BookDto> getAll() { ... }

    @PostMapping("/deleteBookById")
    public ResponseEntity<?> delete(@RequestParam Long id) {
        // 🚨 ГРЕШКА 2: Дублиран try-catch във всеки един контролер метод!
        try {
            bookService.delete(id);
            return ResponseEntity.ok("Deleted");
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}
```
*   **Как да се избегне:** Използвай чисти REST пътища (`/api/v1/books/{id}`) и изнеси цялата обработка на грешки в един единствен **`@RestControllerAdvice`** клас.

### 🟢 ПРАВИЛЕН КОД (Чист `@RestControllerAdvice` с `ProblemDetail` за валидация и 404 грешки):
```java
import org.springframework.http.*;
import org.springframework.validation.FieldError;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.*;
import java.net.URI;
import java.util.Map;
import java.util.stream.Collectors;

@RestControllerAdvice
public class GlobalApiExceptionHandler {

    @ExceptionHandler(ResourceNotFoundException.class)
    public ProblemDetail handleNotFound(ResourceNotFoundException ex) {
        ProblemDetail problem = ProblemDetail.forStatusAndDetail(HttpStatus.NOT_FOUND, ex.getMessage());
        problem.setTitle("Resource Not Found");
        problem.setType(URI.create("https://example.com"));
        return problem;
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ProblemDetail handleValidationErrors(MethodArgumentNotValidException ex) {
        Map<String, String> errors = ex.getBindingResult().getFieldErrors().stream()
                .collect(Collectors.toMap(
                        FieldError::getField,
                        err -> err.getDefaultMessage() != null ? err.getDefaultMessage() : "Invalid value",
                        (existing, replacement) -> existing
                ));

        ProblemDetail problem = ProblemDetail.forStatusAndDetail(HttpStatus.BAD_REQUEST, "Невалидни входни данни");
        problem.setTitle("Validation Failed");
        problem.setProperty("fieldErrors", errors);
        return problem;
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **`@ControllerAdvice`** и **`@RestControllerAdvice`**, и какво се случва, ако имаш два `@ExceptionHandler` метода — един за `RuntimeException` и един за по-конкретния му наследник `ResourceNotFoundException`?“*
*   **Уловката:** Проверка дали кандидатът разбира как Spring резолва приоритета на изключенията!
*   **Точен Senior отговор:** 
    1.  **`@RestControllerAdvice`** = `@ControllerAdvice` + `@ResponseBody` (всичко, което върнеш от `@ExceptionHandler`, се сериализира директно в JSON/XML отговор, вместо да търси HTML шаблон в Thymeleaf).
    2.  **Приоритет на съвпадение (Exception Depth Matching):** За разлика от обикновения Java `try-catch` (където редът отгоре надолу определя кой блок ще сработи), в `@RestControllerAdvice` **редът на методите в класа няма значение** — Spring автоматично избира **най-близкия (най-конкретния) клас в йерархията на наследяване** (`ResourceNotFoundException` винаги печели пред `RuntimeException` и `Exception`).

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT 3) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Автоматична валидация на `@RequestBody`** | Изисква изрично **`@Valid`** пред параметъра (иначе гърми `MethodArgumentNotValidException` само ако е сложено!) | **`[ApiController]` валидира модела автоматично** преди да влезе в метода и връща `400 ValidationProblemDetails` |
| **Разширяване на базовия Error Handler** | Наследяване на **`ResponseEntityExceptionHandler`** (покрива всички вградени Spring MVC грешки!) | Имплементиране на **`IExceptionHandler`** + `builder.Services.AddProblemDetails()` |
| **Версиониране на API-то** | В URL пътя (``/api/v1/users``) или чрез ``headers = "X-API-VERSION=1"`` | Пакет ``Asp.Versioning.Http`` (``[ApiVersion("1.0")]``) |
| **Content Negotiation (JSON / XML)** | Автоматично по ``Accept`` header (при наличие на ``jackson-dataformat-xml``) | Чрез ``.AddXmlSerializerFormatters()`` в ``Program.cs`` |

---
[⬅️ 05-00. Intro to Spring Advanced](05-00-Intro.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-01-01. Syntax From Scratch](05-01-01-Syntax-From-Scratch.md)