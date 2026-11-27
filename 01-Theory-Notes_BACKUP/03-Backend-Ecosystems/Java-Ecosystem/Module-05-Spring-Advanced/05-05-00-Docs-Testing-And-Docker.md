[⬅️ 05-04. Spring AOP & Interceptors](05-04-00-Spring-AOP.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-05-01. Syntax From Scratch](05-05-01-Syntax-From-Scratch.md)

---

# 05-05-00. API Documentation (Swagger), Integration Testing (Testcontainers) & Docker (Теория & Капани)

**Модул 05:** Spring Advanced | **Лекционна тема 05:** Автоматична интерактивна API документация със **SpringDoc OpenAPI 3 / Swagger UI**, пълноценно интеграционно тестване с **`@SpringBootTest`**, **`MockMvc`** и реални Docker бази данни чрез **Testcontainers (`@ServiceConnection`)**, оптимизиран **Multi-stage `Dockerfile`** за Spring Boot 3 и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Integration Testing с Testcontainers & Multi-Stage Docker (Блок-схема)](#2-архитектура-на-integration-testing-с-testcontainers--multi-stage-docker-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot 3, ASP.NET Core 8, FastAPI, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-3-aspnet-core-8-fastapi-nestjs)
* [4. Плюсове и минуси на Testcontainers срещу In-Memory H2 база данни](#4-плюсове-и-минуси-на-testcontainers-срещу-in-memory-h2-база-данни)
* [5. Кодов одит: Правилен срещу Грешен код (Тежък `Dockerfile` като `root` и тестване на PostgreSQL с H2)](#5-кодов-одит-правилен-срещу-грешен-код-тежък-dockerfile-като-root-и-тестване-на-postgresql-с-h2)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot 3) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot-3-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **OpenAPI 3 срещу Swagger UI (Спецификация срещу Витрина):**
    *   **OpenAPI 3** е стандартният JSON/YAML договор (`/v3/api-docs`), описващ всички ендпойнти, DTO схеми, статус кодове и JWT сигурност.
    *   **Swagger UI** (`/swagger-ui/index.html`) е интерактивният уеб интерфейс, който чете този JSON договор и позволява на фронтенд разработчиците и QA инженерите да тестват заявките директно в браузъра!
*   **Multi-stage `Dockerfile` (Строителна площадка срещу Готов апартамент):**
    *   **Етап 1 (`Builder`):** Тежък имидж с Maven и пълното JDK (~600 MB), който компилира кода и сглобява `.jar` файла.
    *   **Етап 2 (`Runtime`):** Олекотен имидж само с JRE (`eclipse-temurin:21-jre-alpine`, ~180 MB), в който копираме **единствено готовия `.jar` файл** и го пускаме от името на обикновен (не-`root`) потребител!

---

## 2. АРХИТЕКТУРА НА INTEGRATION TESTING С TESTCONTAINERS & MULTI-STAGE DOCKER (БЛОК-СХЕМА)

```text
[ JUnit 5 (@SpringBootTest) ]
       │  1. Стартира автоматично истински PostgreSQL 16 Docker контейнер!
       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ @ServiceConnection (Spring Boot 3.1+)                                   │
│ • Взима динамичния порт на Docker контейнера и настройва DataSource сам! │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ MockMvc.perform(post("/api/v1/books")...)                               │
│ • Минава през целия Spring Security Filter Chain + Controller + Service │
│ • Изпълнява реални PostgreSQL заявки + Flyway миграции                  │
│ • След теста унищожава контейнера без следа!                            │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT 3, ASP.NET CORE 8, FASTAPI, NESTJS)

| Инженерен инструмент | ☕ Java 21+ (Spring Boot 3) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (FastAPI) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **OpenAPI / Swagger UI** | `springdoc-openapi-starter-webmvc-ui` | `Swashbuckle.AspNetCore` / `Microsoft.AspNetCore.OpenApi` | Вграден по подразбиране (`/docs`) | `@nestjs/swagger` |
| **Интеграционен HTTP тест** | `@SpringBootTest` + `MockMvc` / `TestRestTemplate` | `WebApplicationFactory<Program>` + `HttpClient` | `TestClient` / `httpx.AsyncClient` | `Test.createTestingModule` + `supertest` |
| **Реална БД в тестовете** | `org.testcontainers:postgresql` + `@ServiceConnection` | `Testcontainers.PostgreSql` | `testcontainers[postgres]` | `testcontainers` (`PostgreSqlContainer`) |
| **Базов Runtime Docker Image** | `eclipse-temurin:21-jre-alpine` | `://microsoft.com` | `python:3.12-slim` | `node:20-alpine` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА TESTCONTAINERS СРЕЩУ IN-MEMORY H2 БАЗА ДАННИ

### 🟢 Предимства на Testcontainers (Pros):
*   **100% вярност към продукционната среда:** H2 често пропуска специфични PostgreSQL грешки (напр. `JSONB` колони, `ILIKE`, специфични `SEQUENCE` или Flyway SQL миграции). С Testcontainers тестваш срещу същата версия на PostgreSQL, която работи и в продукция!

### 🔴 Недостатъци (Cons):
*   **Изисква работещ Docker Engine и 3–5 секунди старт при първия тест:** В CI/CD тръбопроводите (GitHub Actions / GitLab CI) трябва да има наличен Docker daemon (което днес е стандарт в GitHub-hosted runners).

---
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ТЕЖЪК `DOCKERFILE` КАТО `ROOT` И ТЕСТВАНЕ НА POSTGRESQL С H2)

### ❌ ГРЕШЕН КОД (Single-stage Dockerfile с пълно JDK, изпълняван като `root`, без `.dockerignore`):
```dockerfile
# 🚨 ГРЕШКА 1: Използва пълно JDK (~600MB) в продукция вместо лек JRE (~180MB)!
FROM eclipse-temurin:21-jdk
WORKDIR /app
# 🚨 ГРЕШКА 2: Копира цялата папка (вкл. .git и target/classes), чупейки Docker кеша при всяка промяна!
COPY . .
RUN ./mvnw package -DskipTests
# 🚨 ГРЕШКА 3 (Security Risk): Контейнерът работи като root потребител!
ENTRYPOINT ["java", "-jar", "target/app.jar"]
```
*   **Как да се избегне:** Използвай **Multi-stage `Dockerfile`**, копирай първо само `pom.xml` и свали зависимостите (за да се кешират в Docker слой), прехвърли само `.jar` файла в олекотен **`eclipse-temurin:21-jre-alpine`** имидж и създай потребител с ограничени права (`USER appuser`).

### 🟢 ПРАВИЛЕН КОД (Production-Ready Multi-stage `Dockerfile` за Spring Boot 3):
```dockerfile
# --- ЕТАП 1: Build (Компилация) ---
FROM eclipse-temurin:21-jdk-alpine AS builder
WORKDIR /build

COPY mvnw pom.xml ./
COPY .mvn .mvn
# Сваля зависимостите в отделен слой -> кешират се, докато не промениш pom.xml!
RUN ./mvnw dependency:go-offline -B

COPY src src
RUN ./mvnw clean package -DskipTests

# --- ЕТАП 2: Runtime (Олекотен продукционен контейнер) ---
FROM eclipse-temurin:21-jre-alpine
WORKDIR /app

# Създаваме не-root потребител за максимална сигурност
RUN addgroup -S spring && adduser -S spring -G spring
USER spring:spring

COPY --from=builder /build/target/*.jar app.jar

EXPOSE 8080
ENTRYPOINT ["java", "-XX:+UseContainerSupport", "-XX:MaxRAMPercentage=75.0", "-jar", "app.jar"]
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Защо при стартиране на Java приложение в Docker контейнер **никога** не трябва да хардкодваш `-Xmx512m`, а вместо това да използваш **`-XX:MaxRAMPercentage=75.0`** (и как работи **Layered JAR** в Spring Boot)?“*
*   **Уловката:** Проверка дали кандидатът има реален опит с пускане на JVM приложения в Docker / Kubernetes!
*   **Точен Senior отговор:** 
    1.  **`-XX:MaxRAMPercentage=75.0`:** JVM не заема памет само за Heap-а (`-Xmx`), а и за Metaspace, стекове на нишки, Code Cache и Direct ByteBuffers (~25% отгоре). Ако контейнерът в Kubernetes има лимит 1 GB RAM и сложиш `-Xmx1024m`, Linux OOMKiller ще убие контейнера с `Exit Code 137 (OOMKilled)`! С `-XX:MaxRAMPercentage=75.0` JVM автоматично заделя 75% от лимита на контейнера за Heap и оставя 25% резерв за системната памет на JVM.
    2.  **Layered JAR (`jarmode=layertools`):** Обикновеният Fat JAR слага 60 MB библиотеки и 200 KB твой код в един файл — при всяка промяна на 1 ред код Docker качва 60 MB наново. С Layered JAR на Spring Boot зависимостите (`dependencies`) и твоят код (`application`) се разделят в отделни Docker слоеве, така че при деплой се обновяват само последните 200 KB!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT 3) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **OpenAPI / Swagger UI конфигурация** | Автоматично с зависимост `springdoc-openapi-starter-webmvc-ui` | Вграден `Microsoft.AspNetCore.OpenApi` (.NET 9+) или `Swashbuckle` |
| **Настройка на JWT бутон в Swagger** | `@SecurityScheme(type = SecuritySchemeType.HTTP, scheme = "bearer")` | `options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme ...)` |
| **Тестване на HTTP слой без мрежа** | **`MockMvc`** (`mockMvc.perform(get("/api/...")).andExpect(...)`) | `WebApplicationFactory<Program>` + `factory.CreateClient()` |
| **Интеграция на Testcontainers с DI** | **`@ServiceConnection`** (1 анотация свързва контейнера с Spring!) | Ръчно подаване на `container.GetConnectionString()` в `ConfigureAppConfiguration` |
| **Вграден Build на Docker Image без Dockerfile** | `mvn spring-boot:build-image` (използва **Cloud Native Buildpacks**) | `dotnet publish --os linux --arch x64 /t:PublishContainer` |

---
[⬅️ 05-04. Spring AOP & Interceptors](05-04-00-Spring-AOP.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-05-01. Syntax From Scratch](05-05-01-Syntax-From-Scratch.md)
