[⬅️ Назад към Модул 05 Индекс](README.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-01. Web API & Global Error Handling](05-01-00-REST-And-Error-Handling.md)

---

# 05-00. Intro: Въведение в Spring Advanced & Enterprise Архитектура

**Модул 05:** Spring Advanced | **Цел:** Преход от монолитни MVC сайтове към модерни **Stateless REST API** услуги със **Spring Security 6+**, **JWT автентикация**, стандартизирана обработка на грешки (**RFC 9457 `ProblemDetail`**), **Aspect-Oriented Programming (AOP)**, интеграционни тестове с **Testcontainers** и пакетиране в **Docker** контейнери.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурен конвейер на защитена REST API заявка (Блок-схема)](#2-архитектурен-конвейер-на-защитена-rest-api-заявка-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot 3, ASP.NET Core 8, FastAPI, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-3-aspnet-core-8-fastapi-nestjs)
* [4. Плюсове и минуси на Stateless REST + JWT архитектурата](#4-плюсове-и-минуси-на-stateless-rest--jwt-архитектурата)
* [5. Кодов одит: Правилен срещу Грешен код (Изтичане на Stack Trace към клиента и стар Spring Security 5 синтаксис)](#5-кодов-одит-правилен-срещу-грешен-код-изтичане-на-stack-trace-към-клиента-и-стар-spring-security-5-синтаксис)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Security 6) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-security-6-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Автентикация (Authentication) срещу Авторизация (Authorization):**
    *   **Authentication (`401` при неуспех) = Проверка на личната карта на входа:** *„Кой си ти?“* (Проверка на имейл + парола или подпис на JWT токен).
    *   **Authorization (`403` при неуспех) = Проверка на пропуска за VIP залата:** *„Знам кой си, но имаш ли право да влезеш тук?“* (Проверка на роли като `ROLE_ADMIN` или права `authority`).
*   **Aspect-Oriented Programming (AOP) = Очила за напречни грижи (Cross-Cutting Concerns):** Вместо да пишеш логове, измерване на време за изпълнение и проверка на права във всеки от 50-те си сървиса, слагаш една анотация (`@LogExecutionTime` или `@PreAuthorize`) и Spring автоматично обвива метода в невидимо прокси!

---

## 2. АРХИТЕКТУРЕН КОНВЕЙЕР НА ЗАЩИТЕНА REST API ЗАЯВКА (БЛОК-СХЕМА)

```text
[ Клиент (Angular / React / Mobile App) ]
       │  GET /api/v1/orders  |  Header: Authorization: Bearer <JWT>
       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 1. Spring Security Filter Chain (Преди заявката да стигне до MVC!)      │
│    • JwtAuthenticationFilter валидира подписа и зарежда SecurityContext │
│    • Проверява правата за URL-а (напр. .hasRole("USER"))                │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 2. DispatcherServlet ➔ @RestController ➔ @Service (@Transactional)      │
│    • Ако възникне грешка ➔ @RestControllerAdvice я хваща и превръща в   │
│      стандартен RFC 9457 JSON отговор (ProblemDetail)!                  │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT 3, ASP.NET CORE 8, FASTAPI, NESTJS)

| Enterprise Компонент | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (FastAPI) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Глобален Error Handler** | `@RestControllerAdvice` + `ProblemDetail` | `IExceptionHandler` + `ProblemDetails` | `@app.exception_handler(...)` | `@Catch()` Exception Filter |
| **Защита на ендпойнти** | `SecurityFilterChain` + `@PreAuthorize` | `[Authorize(Roles = "Admin")]` | `Depends(get_current_admin)` | `@UseGuards(JwtAuthGuard, RolesGuard)` |
| **Хеширане на пароли** | `BCryptPasswordEncoder` / `Argon2` | `PasswordHasher<TUser>` (PBKDF2) | `passlib` / `pwdlib` (`Argon2` / `bcrypt`) | `bcrypt` / `argon2` |
| **API Документация** | **SpringDoc OpenAPI** (`/swagger-ui.html`) | **Swashbuckle** / NSwag / Built-in OpenAPI | Вграден `/docs` (Swagger) и `/redoc` | `@nestjs/swagger` |
| **Интеграционни тестове с БД** | **Testcontainers** (`@ServiceConnection`) | **Testcontainers.PostgreSql** + `WebApplicationFactory` | `testcontainers-python` + `pytest` | `testcontainers` + `supertest` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА STATELESS REST + JWT АРХИТЕКТУРАТА

### 🟢 Предимства (Pros):
*   **Безкрайно хоризонтално мащабиране:** Тъй като сървърът не пази сесия в RAM паметта, можеш да пуснеш 20 копия на Spring Boot приложението зад Load Balancer и всяка заявка ще бъде обслужена от произволен сървър без нужда от споделена сесия.
*   **Един бекенд за всички клиенти:** Едно и също REST API обслужва едновременно уеб фронтенд (Angular/React), мобилни приложения (iOS/Android) и външни партньорски системи.

### 🔴 Недостатъци (Cons):
*   **Трудно незабавно анулиране на изтекъл/откраднат JWT:** Докато не изтече срокът му (`exp`), JWT токенът е валиден. Затова Access токените се правят кратки (5–15 минути) и се комбинират с **Refresh Token** в `HttpOnly` бисквитка.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ИЗТИЧАНЕ НА STACK TRACE КЪМ КЛИЕНТА И СТАР SPRING SECURITY 5 СИНТАКСИС)

### ❌ ГРЕШЕН КОД (Изтичане на вътрешни SQL/Java грешки към клиента и остарял `WebSecurityConfigurerAdapter`):
```java
// 🚨 ГРЕШКА 1: WebSecurityConfigurerAdapter е ИЗТРИТ в Spring Security 6 / Spring Boot 3!
// Кодът изобщо няма да се компилира!
// public class SecurityConfig extends WebSecurityConfigurerAdapter { ... }

// 🚨 ГРЕШКА 2 (Information Disclosure): Връщане на суровия e.getMessage() или Stack Trace към клиента!
// Ако базата гръмне, клиентът вижда имената на таблиците, версиите на PostgreSQL и класовете!
@ExceptionHandler(Exception.class)
public ResponseEntity<String> handleAll(Exception e) {
    return ResponseEntity.status(500).body(Arrays.toString(e.getStackTrace()));
}
```
*   **Как да се избегне:** В Spring Security 6+ дефинирай `@Bean public SecurityFilterChain filterChain(HttpSecurity http)` с **Lambda DSL** синтаксис. В глобалния Exception Handler логвай пълния Stack Trace **само на сървъра** (`log.error("...", e)`), а към клиента връщай чисто, стандартизирано **`ProblemDetail`** съобщение!

### 🟢 ПРАВИЛЕН КОД (Модерен Spring Security 6 Lambda DSL и RFC 9457 `ProblemDetail`):
```java
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
public class ModernSecurityConfig {

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        return http
                .csrf(AbstractHttpConfigurer::disable)
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/api/v1/auth/**", "/swagger-ui/**", "/v3/api-docs/**").permitAll()
                        .requestMatchers("/api/v1/admin/**").hasRole("ADMIN")
                        .anyRequest().authenticated()
                )
                .build();
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **Servlet `Filter`**, **Spring `HandlerInterceptor`** и **Spring `AOP (@Aspect)`**, и в какъв ред се изпълняват при една HTTP заявка?“*
*   **Уловката:** Това е **топ архитектурен въпрос** за Senior Spring разработчици, който проверява дали познаваш в дълбочина жизнения цикъл на заявката!
*   **Точен Senior отговор (Точният ред на изпълнение отвън навътре):** 
    1.  **Servlet `Filter` (Ниво Tomcat / Servlet Container):** Изпълнява се **най-отвън**, още преди заявката да влезе в Spring `DispatcherServlet`. Идеален за **Spring Security**, CORS, компресия и Request ID логове (има достъп до суровите `HttpServletRequest` / `HttpServletResponse`).
    2.  **Spring `HandlerInterceptor` (Ниво Spring MVC):** Изпълнява се **след `DispatcherServlet`**, точно преди и след извикването на метода в `@Controller` (`preHandle`, `postHandle`, `afterCompletion`). Знае кой точно метод на контролера ще се изпълни (`HandlerMethod`).
    3.  **Spring `AOP (@Aspect)` (Ниво Spring Beans):** Изпълнява се **най-вътре**, около извикването на всеки Spring Bean метод (обикновено в `@Service` слоя — например `@Transactional`, `@Cacheable` или одит логове), и има директен достъп до вече парснатите Java аргументи и връщаната стойност на метода!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING SECURITY 6) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3 / Security 6) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Конвейер за сигурност** | `SecurityFilterChain` (верига от Servlet филтри) | Middleware Pipeline (`app.UseAuthentication()` ➔ `app.UseAuthorization()`) |
| **Префикс на ролите** | Spring автоматично добавя **`ROLE_`** (`hasRole("ADMIN")` проверява за `"ROLE_ADMIN"`) | Няма автоматичен префикс (`[Authorize(Roles = "Admin")]` проверява точно `"Admin"`) |
| **Стандартен формат за API грешки** | Вграден клас **`ProblemDetail`** (RFC 9457, от Spring 6+) | Вграден клас **`ProblemDetails`** (RFC 9457, вграден от ASP.NET Core 2.1+) |
| **Интеграционни тестове с Docker БД** | **`@Testcontainers`** + **`@ServiceConnection`** (Spring Boot 3.1+ настройва URL/user/pass сам!) | `Testcontainers` + `WebApplicationFactory<Program>` |
| **Оптимизация на Docker Image** | Layered JAR (``spring-boot:build-image`` / Paketo Buildpacks) или GraalVM Native | ``dotnet publish /t:PublishContainer`` или Chiseled Ubuntu images |

---
[⬅️ Назад към Модул 05 Индекс](README.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-01. Web API & Global Error Handling](05-01-00-REST-And-Error-Handling.md)
