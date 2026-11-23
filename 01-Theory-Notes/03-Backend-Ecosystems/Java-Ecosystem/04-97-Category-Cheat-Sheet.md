[⬅️ 04-96. Java & Spring Оптимизации](04-96-Performance-And-Optimizations.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-98. Common Errors And Fixes](04-98-Common-Errors-And-Fixes.md)

---

# 04-97. Java 21+ & Spring Boot 3 Category Cheat Sheet: Пълно обобщение & Библиотеки

**Цел:** Бърза синтактична и архитектурна справка на едно място за всички **5 модула на Java екосистемата** (Java Fundamentals, Java OOP, Spring Data / JPA, Spring Fundamentals / MVC и Spring Advanced / Security / JWT / Docker) и най-важните стандартни и външни библиотеки.

## 🧭 Съдържание по категории
* [1. Категория 1: Модерен Java 21+ Синтаксис (`record`, `sealed`, Pattern Matching, `Optional`)](#1-категория-1-модерен-java-21-синтаксис-record-sealed-pattern-matching-optional)
* [2. Категория 2: Колекции (`List`, `Set`, `Map`) и Stream API Конвейер](#2-категория-2-колекции-list-set-map-и-stream-api-конвейер)
* [3. Категория 3: Spring Data JPA & Hibernate Анотации](#3-категория-3-spring-data-jpa--hibernate-анотации)
* [4. Категория 4: Spring Core, Web REST API & Spring Security 6 Анотации](#4-категория-4-spring-core-web-rest-api--spring-security-6-анотации)
* [5. Категория 5: Важни Java & Spring Библиотеки (Maven, Импорт, Извикване и Приложение)](#5-категория-5-важни-java--spring-библиотеки-maven-импорт-извикване-и-приложение)

---

## 1. КАТЕГОРИЯ 1: МОДЕРЕН JAVA 21+ СИНТАКСИС (`RECORD`, `SEALED`, PATTERN MATCHING, `OPTIONAL`)

```java
import java.math.BigDecimal;
import java.util.Optional;

// 1. Java Record (Неизменяем DTO клас с автоматични getters, equals, hashCode, toString)
public record OrderDto(Long id, String customerEmail, BigDecimal total) {
    // Compact Constructor за валидация
    public OrderDto {
        if (total.compareTo(BigDecimal.ZERO) < 0) {
            throw new IllegalArgumentException("Сумата не може да е отрицателна!");
        }
    }
}

// 2. Sealed Interface + Switch Pattern Matching (Exhaustive проверка при компилация)
public sealed interface PaymentResult permits PaymentSuccess, PaymentFailed {}
public record PaymentSuccess(String transactionId) implements PaymentResult {}
public record PaymentFailed(String reason) implements PaymentResult {}

public class ModernJavaFeatures {
    public String formatPayment(PaymentResult result) {
        return switch (result) {
            case PaymentSuccess s -> "Успех! ID: " + s.transactionId();
            case PaymentFailed f  -> "Отказано: " + f.reason();
        };
    }

    // 3. Безопасна работа с Optional<T> (никога не викай .get() без проверка!)
    public String extractDomain(Optional<String> emailOpt) {
        return emailOpt
                .filter(e -> e.contains("@"))
                .map(e -> e.substring(e.indexOf('@') + 1))
                .orElse("unknown.bg");
    }
}
```

---

## 2. КАТЕГОРИЯ 2: КОЛЕКЦИИ (`LIST`, `SET`, `MAP`) И STREAM API КОНВЕЙЕР

| Структура / Операция | Синтаксис и Пример | Характеристика / Сложност |
| :--- | :--- | :--- |
| **Неизменяеми фабрики** | `List.of(1, 2, 3)`, `Set.of("A", "B")`, `Map.of("k", 1)` | Забраняват `null` и хвърлят `UnsupportedOperationException` при `.add()` |
| **`ArrayList` / `HashSet` / `HashMap`** | `new ArrayList<>()`, `new HashSet<>()`, `new HashMap<>()` | `ArrayList` $O(1)$ достъп по индекс; `HashSet`/`HashMap` $O(1)$ по хеш |
| **`Map` модерни методи** | `map.getOrDefault(k, 0)`, `map.computeIfAbsent(k, _ -> new ArrayList<>())` | Елиминират ръчните `if (!map.containsKey(k))` проверки |
| **Stream API конвейер** | `list.stream().filter(...).map(...).sorted(...).toList()` | Lazy изпълнение — нищо не се смята до терминалната операция (`.toList()`, `.count()`) |
| **Групиране (`Collectors`)** | `users.stream().collect(Collectors.groupingBy(User::getRole))` | Връща `Map<Role, List<User>>` на един ред |

---

## 3. КАТЕГОРИЯ 3: SPRING DATA JPA & HIBERNATE АНОТАЦИИ

| Анотация | Къде се поставя | Какво прави |
| :--- | :--- | :--- |
| **`@Entity` / `@Table(name = "...")`** | Върху класа | Маркира класа като таблица в релационната база данни |
| **`@Id` / `@GeneratedValue(...)`** | Върху PK полето | Дефинира първичен ключ (`GenerationType.IDENTITY` за PostgreSQL/MySQL) |
| **`@ManyToOne(fetch = FetchType.LAZY)`** | Върху релация N:1 | Винаги сменяй на `LAZY` (по подразбиране `@ManyToOne` е `EAGER`!) |
| **`@OneToMany(mappedBy = "...")`** | Върху релация 1:N | `mappedBy` сочи името на полето в класа-собственик на Foreign Key |
| **`@Transactional`** | Върху `@Service` метод | Управлява SQL транзакцията и Hibernate Persistence Context (`readOnly = true` за `SELECT`) |
| **`@Query("... JOIN FETCH ...")`** | Върху метод в `Repository` | Зарежда свързаните колекции в 1 SQL заявка (решава $N+1$ проблема) |

---

## 4. КАТЕГОРИЯ 4: SPRING CORE, WEB REST API & SPRING SECURITY 6 АНОТАЦИИ

| Анотация | Къде се поставя | Какво прави |
| :--- | :--- | :--- |
| **`@Service` / `@Repository` / `@Component`** | Върху класа | Регистрира класа като Spring Bean в IoC контейнера (по подразбиране `Singleton`) |
| **`@Configuration` + `@Bean`** | Върху клас и метод | Дефинира и конфигурира външни класове (напр. `PasswordEncoder`, `SecurityFilterChain`) |
| **`@RestController` + `@RequestMapping`** | Върху API контролер | Обединява `@Controller` + `@ResponseBody` (връща директно JSON / `ProblemDetail`) |
| **`@GetMapping` / `@PostMapping` / `@PutMapping` / `@DeleteMapping`** | Върху метод в контролер | Мапва конкретния HTTP глагол към метод на контролера |
| **`@RequestBody @Valid`** | Пред параметър на метод | Десериализира JSON тялото в DTO и стартира Bean Validation (`@NotBlank`, `@Email`) |
| **`@RestControllerAdvice` + `@ExceptionHandler`** | Върху глобален хендлър | Прихваща изключения от всички контролери и връща RFC 9457 `ProblemDetail` |
| **`@EnableMethodSecurity` + `@PreAuthorize`** | Конфигурация и метод | Активира и проверява права по роля/SpEL преди изпълнение на метода |
| **`@Aspect` + `@Around`** | Върху AOP клас и метод | Обвива методите в Proxy за напречни грижи (логове, измерване на време, одит) |

---

## 5. КАТЕГОРИЯ 5: ВАЖНИ JAVA & SPRING БИБЛИОТЕКИ (MAVEN, ИМПОРТ, ИЗВИКВАНЕ И ПРИЛОЖЕНИЕ)

| Библиотека / Модул | Зависимост (`pom.xml`) | Импорт и Извикване (Синтаксис) | За какво се използва |
| :--- | :--- | :--- | :--- |
| **`java.time` (Time API)** | Вградена в JDK 8+ | `import java.time.Instant;`<br>`Instant now = Instant.now();` | Модерна, потокобезопасна работа с дати, часове и часови зони (`LocalDate`, `ZonedDateTime`, `Duration`). |
| **`java.net.http.HttpClient`** | Вградена в JDK 11+ | `var client = HttpClient.newHttpClient();`<br>`var res = client.send(req, BodyHandlers.ofString());` | Вграден синхронен и асинхронен HTTP/2 клиент без външни библиотеки. |
| **`Jackson` (`ObjectMapper`)** | Вграден в `spring-boot-starter-web` | `import com.fasterxml.jackson.databind.ObjectMapper;`<br>`String json = mapper.writeValueAsString(dto);` | Сериализация и десериализация между Java обекти (POJO / `record`) и JSON. |
| **`Bean Validation` (Hibernate Validator)** | `spring-boot-starter-validation` | `import jakarta.validation.constraints.*;`<br>`@NotBlank @Email String email` | Декларативна валидация на входни DTO данни чрез анотации. |
| **`MapStruct`** | `org.mapstruct:mapstruct` | `@Mapper(componentModel = "spring")`<br>`UserDto toDto(UserEntity entity);` | Генерира по време на компилация светкавичен код за мапинг между Entity и DTO (без Reflection). |
| **`JJWT` (`io.jsonwebtoken`)** | `io.jsonwebtoken:jjwt-api` | `Jwts.builder().subject(u).signWith(key).compact()` | Създаване, подписване и валидиране на JSON Web Tokens (JWT) за Stateless API сигурност. |
| **`SpringDoc OpenAPI`** | `springdoc-openapi-starter-webmvc-ui` | `@Operation(summary = "...")` + `/swagger-ui/index.html` | Автоматично генериране на OpenAPI 3 спецификация и интерактивен Swagger UI. |
| **`Lombok`** *(по избор)* | `org.projectlombok:lombok` | `@Getter @Setter @Builder` | Намалява boilerplate кода в JPA Entity класовете *(внимание: избягвай `@Data` върху `@Entity`!)*. |
| **`JUnit 5 + Mockito + Testcontainers`** | `spring-boot-starter-test` + `testcontainers` | `@SpringBootTest @Testcontainers`<br>`@ServiceConnection static PostgreSQLContainer<?> pg` | Писане на Unit тестове с мокове и пълни интеграционни тестове срещу реална база данни в Docker. |

---
[⬅️ 04-96. Java & Spring Оптимизации](04-96-Performance-And-Optimizations.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-98. Common Errors And Fixes](04-98-Common-Errors-And-Fixes.md)
