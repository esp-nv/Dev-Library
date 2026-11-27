[⬅️ 05-01-00. Теория & Капани](05-01-00-REST-And-Error-Handling.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-02-00. Spring Security 6+](05-02-00-Spring-Security.md)

---

# 05-01-01. Web API, REST Controllers & Global Error Handling: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за пълен CRUD `@RestController` (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`), странициране с `Pageable` в REST API и професионален `@RestControllerAdvice` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Пълен CRUD REST Controller с `ResponseEntity<T>` и `URI` Location](#1-пълен-crud-rest-controller-с-responseentityt-и-uri-location)
* [2. Странициране и филтриране в REST ендпойнт (`Pageable` & `@RequestParam`)](#2-странициране-и-филтриране-в-rest-ендпойнт-pageable--requestparam)
* [3. Собствени бизнес изключения и централизиран `@RestControllerAdvice`](#3-собствени-бизнес-изключения-и-централизиран-restcontrolleradvice)
* [4. Включване на стандартни `ProblemDetail` отговори в `application.properties`](#4-включване-на-стандартни-problemdetail-отговори-в-applicationproperties)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПЪЛЕН CRUD REST CONTROLLER С `RESPONSEENTITY<T>` И `URI` LOCATION

```java
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.support.ServletUriComponentsBuilder;
import java.net.URI;
import java.util.List;

@RestController
@RequestMapping("/api/v1/books")
public class BookRestController {

    private final BookService bookService;

    public BookRestController(BookService bookService) {
        this.bookService = bookService;
    }

    // 1. GET /api/v1/books -> 200 OK
    @GetMapping
    public ResponseEntity<List<BookResponseDto>> getAllBooks() {
        return ResponseEntity.ok(bookService.findAll());
    }

    // 2. GET /api/v1/books/{id} -> 200 OK (или 404 от GlobalExceptionHandler)
    @GetMapping("/{id}")
    public ResponseEntity<BookResponseDto> getBookById(@PathVariable Long id) {
        return ResponseEntity.ok(bookService.findById(id));
    }

    // 3. POST /api/v1/books -> 201 Created + Header "Location: /api/v1/books/15"
    @PostMapping
    public ResponseEntity<BookResponseDto> createBook(@RequestBody @Valid CreateBookDto dto) {
        BookResponseDto created = bookService.create(dto);

        URI location = ServletUriComponentsBuilder
                .fromCurrentRequest()
                .path("/{id}")
                .buildAndExpand(created.id())
                .toUri();

        return ResponseEntity.created(location).body(created);
    }

    // 4. PUT /api/v1/books/{id} -> 200 OK (Пълна актуализация)
    @PutMapping("/{id}")
    public ResponseEntity<BookResponseDto> updateBook(@PathVariable Long id,
                                                      @RequestBody @Valid UpdateBookDto dto) {
        return ResponseEntity.ok(bookService.update(id, dto));
    }

    // 5. PATCH /api/v1/books/{id}/price -> 200 OK (Частична актуализация)
    @PatchMapping("/{id}/price")
    public ResponseEntity<BookResponseDto> updatePrice(@PathVariable Long id,
                                                       @RequestBody @Valid UpdatePriceDto dto) {
        return ResponseEntity.ok(bookService.updatePrice(id, dto.newPrice()));
    }

    // 6. DELETE /api/v1/books/{id} -> 204 No Content
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteBook(@PathVariable Long id) {
        bookService.deleteById(id);
        return ResponseEntity.noContent().build();
    }
}
```

---

## 2. СТРАНИЦИРАНЕ И ФИЛТРИРАНЕ В REST ЕНДПОЙНТ (`PAGEABLE` & `@REQUESTPARAM`)

Spring MVC автоматично парсва query параметри като `GET /api/v1/books/paged?page=0&size=10&sort=price,desc` директно в обект `Pageable` (а `@PageableDefault` задава стойности по подразбиране):

```java
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.data.web.PageableDefault;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@GetMapping("/paged")
public ResponseEntity<Page<BookResponseDto>> getPagedBooks(
        @PageableDefault(size = 20, sort = "id", direction = Sort.Direction.ASC) Pageable pageable) {
    return ResponseEntity.ok(bookService.findAllPaged(pageable));
}
```

---

## 3. СОБСТВЕНИ БИЗНЕС ИЗКЛЮЧЕНИЯ И ЦЕНТРАЛИЗИРАН `@RESTCONTROLLERADVICE`

```java
// 1. Собствено изключение за несъществуващ ресурс
public class ResourceNotFoundException extends RuntimeException {
    public ResourceNotFoundException(String message) {
        super(message);
    }
}

// 2. Собствено изключение за бизнес конфликт (напр. дублиран имейл -> 409 Conflict)
public class DuplicateResourceException extends RuntimeException {
    public DuplicateResourceException(String message) {
        super(message);
    }
}
```

```java
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ProblemDetail;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import java.time.Instant;

@RestControllerAdvice
public class ApiExceptionHandler {

    private static final Logger log = LoggerFactory.getLogger(ApiExceptionHandler.class);

    @ExceptionHandler(DuplicateResourceException.class)
    public ProblemDetail handleConflict(DuplicateResourceException ex) {
        ProblemDetail problem = ProblemDetail.forStatusAndDetail(HttpStatus.CONFLICT, ex.getMessage());
        problem.setTitle("Resource Conflict");
        problem.setProperty("timestamp", Instant.now());
        return problem;
    }

    // Предпазна мрежа за всяка необработена грешка (никога не издава детайли на клиента!)
    @ExceptionHandler(Exception.class)
    public ProblemDetail handleUnexpected(Exception ex) {
        log.error("Неочаквана сървърна грешка: ", ex);
        ProblemDetail problem = ProblemDetail.forStatusAndDetail(
                HttpStatus.INTERNAL_SERVER_ERROR,
                "Възникна вътрешна грешка на сървъра. Моля, опитайте по-късно."
        );
        problem.setTitle("Internal Server Error");
        return problem;
    }
}
```

---

## 4. ВКЛЮЧВАНЕ НА СТАНДАРТНИ `PROBLEMDETAIL` ОТГОВОРИ В `APPLICATION.PROPERTIES`

С този един ред Spring Boot 3 автоматично превръща всички вградени Spring MVC грешки (като невалиден JSON или грешен HTTP метод) в RFC 9457 `ProblemDetail`:

```properties
spring.mvc.problemdetails.enabled=true
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Клас | Предназначение | HTTP Статус / Резултат |
| :--- | :--- | :--- |
| **`@RestController`** | Обединява `@Controller` + `@ResponseBody` | Всички методи връщат JSON/XML |
| **`@RequestBody @Valid`** | Десериализира входящия JSON и го валидира | При грешка хвърля `MethodArgumentNotValidException` (400) |
| **`ResponseEntity.ok(body)`** | Връща отговор с тяло | `200 OK` |
| **`ResponseEntity.created(uri).body(b)`** | Връща създадения обект + `Location` header | `201 Created` |
| **`ResponseEntity.noContent().build()`** | Връща празен отговор (при `DELETE`) | `204 No Content` |
| **`@RestControllerAdvice`** | Глобален прихващач на грешки за всички контролери | Връща структуриран `ProblemDetail` JSON |

---
[⬅️ 05-01-00. Теория & Капани](05-01-00-REST-And-Error-Handling.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-02-00. Spring Security 6+](05-02-00-Spring-Security.md)