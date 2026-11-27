[⬅️ 05-05-00. Теория & Капани](05-05-00-Docs-Testing-And-Docker.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ Глобален Индекс & Речник](../../GLOBAL-INDEX-AND-GLOSSARY.md)

---

# 05-05-01. API Documentation (Swagger), Integration Testing (Testcontainers) & Docker: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за конфигуриране на OpenAPI 3 / Swagger UI с JWT Bearer поддръжка, писане на пълни интеграционни тестове с `@SpringBootTest`, `MockMvc` и Testcontainers PostgreSQL (`@ServiceConnection`), както и `docker-compose.yml` за локално стартиране в Java от нулата.

## 🧭 Съдържание на темата
* [1. OpenAPI 3 / Swagger конфигурация с JWT Bearer бутон (`@SecurityScheme`)](#1-openapi-3--swagger-конфигурация-с-jwt-bearer-бутон-securityscheme)
* [2. Документиране на Controller и DTO с `@Operation` и `@Schema`](#2-документиране-на-controller-и-dto-с-operation-и-schema)
* [3. Интеграционен тест с `@SpringBootTest`, `MockMvc` и Testcontainers (`@ServiceConnection`)](#3-интеграционен-тест-с-springboottest-mockmvc-и-testcontainers-serviceconnection)
* [4. `docker-compose.yml` (Spring Boot API + PostgreSQL 16)](#4-docker-composeyml-spring-boot-api--postgresql-16)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. OPENAPI 3 / SWAGGER КОНФИГУРАЦИЯ С JWT BEARER БУТОН (`@SECURITYSCHEME`)

Изисква зависимостта `org.springdoc:springdoc-openapi-starter-webmvc-ui` (версия `2.6+` за Spring Boot 3.3+):

```java
import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.enums.SecuritySchemeType;
import io.swagger.v3.oas.annotations.info.Contact;
import io.swagger.v3.oas.annotations.info.Info;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.security.SecurityScheme;
import org.springframework.context.annotation.Configuration;

@Configuration
@OpenAPIDefinition(
    info = @Info(
        title = "Bookstore REST API",
        version = "v1.0",
        description = "Документация на REST API за управление на книги и поръчки",
        contact = @Contact(name = "Dev Team", email = "dev@bookstore.bg")
    ),
    security = @SecurityRequirement(name = "bearerAuth")
)
@SecurityScheme(
    name = "bearerAuth",
    type = SecuritySchemeType.HTTP,
    scheme = "bearer",
    bearerFormat = "JWT"
)
public class OpenApiConfig {}
```

---

## 2. ДОКУМЕНТИРАНЕ НА CONTROLLER И DTO С `@OPERATION` И `@SCHEMA`

```java
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

record CreateBookApiDto(
    @Schema(description = "Заглавие на книгата", example = "Clean Code")
    String title,

    @Schema(description = "Цена в лева", example = "49.90")
    double price
) {}

@RestController
@RequestMapping("/api/v1/books")
@Tag(name = "Books", description = "Операции с каталога от книги")
public class BookDocsController {

    @Operation(summary = "Създаване на нова книга", description = "Изисква роля ADMIN")
    @ApiResponse(responseCode = "201", description = "Книгата е създадена успешно")
    @ApiResponse(responseCode = "400", description = "Невалидни входни данни")
    @PostMapping
    public ResponseEntity<Void> create(@RequestBody CreateBookApiDto dto) {
        return ResponseEntity.status(201).build();
    }
}
```

---

## 3. ИНТЕГРАЦИОНЕН ТЕСТ С `@SPRINGBOottest`, `MOCKMVC` И TESTCONTAINERS (`@SERVICECONNECTION`)

```java
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.testcontainers.service.connection.ServiceConnection;
import org.springframework.http.MediaType;
import org.springframework.security.test.context.support.WithMockUser;
import org.springframework.test.web.servlet.MockMvc;
import org.testcontainers.containers.PostgreSQLContainer;
import org.testcontainers.junit.jupiter.Container;
import org.testcontainers.junit.jupiter.Testcontainers;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@SpringBootTest
@AutoConfigureMockMvc
@Testcontainers
class BookApiIntegrationTest {

    // Стартира реален PostgreSQL 16 Docker контейнер за времето на теста
    @Container
    @ServiceConnection // Автоматично инжектира JDBC URL, username и password в Spring!
    static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:16-alpine");

    @Autowired
    private MockMvc mockMvc;

    @Test
    @WithMockUser(username = "admin@shop.bg", roles = {"ADMIN"})
    void createBook_ShouldReturn201_WhenAuthenticatedAsAdmin() throws Exception {
        String jsonPayload = """
                {"title": "Effective Java", "price": 65.00}
                """;

        mockMvc.perform(post("/api/v1/books")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(jsonPayload))
                .andExpect(status().isCreated())
                .andExpect(header().exists("Location"));
    }

    @Test
    void createBook_ShouldReturn403_WhenAnonymous() throws Exception {
        mockMvc.perform(post("/api/v1/books")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("{\"title\": \"Hack\", \"price\": 10}"))
                .andExpect(status().isForbidden());
    }
}
```

---

## 4. `DOCKER-COMPOSE.YML` (SPRING BOOT API + POSTGRESQL 16)

```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: bookstore-db
    environment:
      POSTGRES_DB: bookstore
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: secret_password
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 5

  api:
    build: .
    container_name: bookstore-api
    depends_on:
      postgres:
        condition: service_healthy
    ports:
      - "8080:8080"
    environment:
      DB_URL: jdbc:postgresql://postgres:5432/bookstore
      DB_USERNAME: postgres
      DB_PASSWORD: secret_password
      SPRING_PROFILES_ACTIVE: prod

volumes:
  pgdata:
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Команда | Предназначение | Бележка |
| :--- | :--- | :--- |
| **`/swagger-ui/index.html`** | Интерактивен уеб интерфейс на OpenAPI | Достъпен веднага след добавяне на `springdoc-openapi` |
| **`@SpringBootTest`** | Зарежда целия Spring ApplicationContext за тест | Използва се за пълни End-to-End / Integration тестове |
| **`@AutoConfigureMockMvc`** | Позволява симулиране на HTTP заявки с `MockMvc` | Тества филтри, валидация и контролери без реална мрежа |
| **`@ServiceConnection`** | Свързва Testcontainers контейнер със Spring Boot | Елиминира нуждата от `@DynamicPropertySource` |
| **`@WithMockUser(roles = "ADMIN")`** | Симулира логнат потребител в тестове | Изисква `spring-security-test` зависимост |
| **`docker compose up --build -d`** | Билдва и стартира API + база данни във фонов режим | В корена на проекта при наличие на `docker-compose.yml` |

---
[⬅️ 05-05-00. Теория & Капани](05-05-00-Docs-Testing-And-Docker.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ Глобален Индекс & Речник](../../GLOBAL-INDEX-AND-GLOSSARY.md)
