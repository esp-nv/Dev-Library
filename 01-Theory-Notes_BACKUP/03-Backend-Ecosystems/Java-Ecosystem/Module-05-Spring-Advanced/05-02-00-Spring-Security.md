[⬅️ 05-01. Web API & Global Error Handling](05-01-00-REST-And-Error-Handling.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-02-01. Syntax From Scratch](05-02-01-Syntax-From-Scratch.md)

---

# 05-02-00. Spring Security 6+ & Authentication (Теория, Filter Chain, BCrypt & Капани)

**Модул 05:** Spring Advanced | **Лекционна тема 02:** Вътрешна архитектура на **Spring Security 6+** (`DelegatingFilterProxy` ➔ `SecurityFilterChain`), конфигурация с модерен **Lambda DSL**, процеса на автентикация (`AuthenticationManager` ➔ `DaoAuthenticationProvider` ➔ `UserDetailsService` ➔ `PasswordEncoder` / `BCrypt`), съхраняване на текущия потребител в **`SecurityContextHolder`**, защита на методи с **`@PreAuthorize`**, CORS и CSRF защита, и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Spring Security 6 Filter Chain (Блок-схема)](#2-архитектура-на-spring-security-6-filter-chain-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Security 6, ASP.NET Core Identity, Django Auth, NestJS Passport)](#3-сравнителна-карта-в-4-езика-spring-security-6-aspnet-core-identity-django-auth-nestjs-passport)
* [4. Плюсове и минуси на Spring Security 6](#4-плюсове-и-минуси-на-spring-security-6)
* [5. Кодов одит: Правилен срещу Грешен код (`NoOpPasswordEncoder` и объркване на `hasRole` срещу `hasAuthority`)](#5-кодов-одит-правилен-срещу-грешен-код-nooppasswordencoder-и-объркване-на-hasrole-срещу-hasauthority)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Security 6) срещу C# 12+ (ASP.NET Core Identity)](#7-архитектурно-сравнение-java-21-spring-security-6-срещу-c-12-aspnet-core-identity)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трите главни героя на входа (Login Flow):**
    1.  **`UserDetailsService` (Архиварят):** Има само един метод — `loadUserByUsername(String username)`. Неговата единствена работа е да намери потребителя в PostgreSQL/MySQL и да го опакова в обект `UserDetails` (съдържащ `username`, криптираната `passwordHash` и списък от роли `GrantedAuthority`).
    2.  **`PasswordEncoder` (`BCryptPasswordEncoder` — Криптографът):** Никога не декриптира паролата (защото BCrypt е еднопосочен хеш със случайна „сол“ — *Salt*!). Той взима чистата парола от формата, хешира я наново и проверява `passwordEncoder.matches(rawPassword, storedHash)`.
    3.  **`SecurityContextHolder` (Личната значка по време на заявката):** Щом входът успее, Spring закача обекта `Authentication` към текущата нишка (`ThreadLocal`), за да може всеки контролер или сървис да попита *„Кой е логнат в момента?“*.
*   **`hasRole("ADMIN")` срещу `hasAuthority("ROLE_ADMIN")`:**
    *   `hasRole("ADMIN")` **автоматично залепя префикса `"ROLE_"`** отпред и търси `"ROLE_ADMIN"`.
    *   `hasAuthority("ADMIN")` търси **точно буквалния низ `"ADMIN"`** без да добавя нищо!

---

## 2. АРХИТЕКТУРА НА SPRING SECURITY 6 FILTER CHAIN (БЛОК-СХЕМА)

```text
[ HTTP Request: POST /login (username, password) ]
       │
       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ SecurityFilterChain (Верига от Servlet филтри преди Controller-а)       │
│  1. CorsFilter ➔ 2. CsrfFilter ➔ 3. UsernamePasswordAuthenticationFilter│
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ AuthenticationManager (ProviderManager)                                 │
│  └──► DaoAuthenticationProvider                                         │
│        ├── 1. Вика UserDetailsService.loadUserByUsername(username)      │
│        └── 2. Вика PasswordEncoder.matches(rawPass, dbHash)             │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼ (При успех)
┌─────────────────────────────────────────────────────────────────────────┐
│ SecurityContextHolder.getContext().setAuthentication(authToken)         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING SECURITY 6, ASP.NET CORE IDENTITY, DJANGO AUTH, NESTJS PASSPORT)

| Концепция | ☕ Java 21+ (Spring Security 6) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (Django) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Зареждане на потребител** | `UserDetailsService` (`loadUserByUsername`) | `UserManager<TUser>` / `SignInManager` | `ModelBackend` (`authenticate()`) | `LocalStrategy` (`validate()`) |
| **Хеширане на парола** | `BCryptPasswordEncoder` (сила 10–12) | `PasswordHasher<TUser>` (PBKDF2-SHA512) | `PBKDF2` / `Argon2PasswordHasher` | `bcrypt.hash(pass, 12)` |
| **Защита на метод с роля** | `@PreAuthorize("hasRole('ADMIN')")` | `[Authorize(Roles = "Admin")]` | `@user_passes_test` / `@permission_required` | `@Roles('ADMIN')` + `RolesGuard` |
| **Достъп до логнатия потребител** | `@AuthenticationPrincipal UserDetails u` | `User.FindFirstValue(ClaimTypes.NameIdentifier)` | `request.user` | `@Req() req` (`req.user`) |
| **Защита срещу CSRF атаки** | Включена по подразбиране за сесии | `[ValidateAntiForgeryToken]` | `{% csrf_token %}` + `CsrfViewMiddleware` | `csurf` / Double Submit Cookie |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SPRING SECURITY 6

### 🟢 Предимства (Pros):
*   **Защита „по подразбиране“ (Secure by Default):** Веднага след добавяне на `spring-boot-starter-security`, всички ендпойнти са заключени, паролите са защитени с BCrypt, а HTTP заглавията за сигурност (`X-Frame-Options`, `X-Content-Type-Options`, `Strict-Transport-Security`) се добавят автоматично.
*   **Мощен SpEL език в `@PreAuthorize`:** Позволява сложни проверки на ниво метод, например `@PreAuthorize("#userId == authentication.principal.id or hasRole('ADMIN')")` (потребителят може да редактира само собствения си профил или ако е админ).

### 🔴 Недостатъци (Cons):
*   **Висока крива на учене при персонализиране:** Дебъгването на грешки във веригата от 15 вградени филтъра изисква включване на `logging.level.org.springframework.security=DEBUG`.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`NOOPPASSWORDENCODER` И ОБЪРКВАНЕ НА `HASROLE` СРЕЩУ `HASAUTHORITY`)

### ❌ ГРЕШЕН КОД (Двойно залепяне на `ROLE_` и несигурен `NoOpPasswordEncoder`):
```java
@Configuration
public class BadSecurityConfig {

    @Bean
    public PasswordEncoder passwordEncoder() {
        // 🚨 ГРЕШКА 1 (Критична уязвимост): Пази паролите в чист текст (Plain Text) в базата!
        return NoOpPasswordEncoder.getInstance();
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        return http
            .authorizeHttpRequests(auth -> auth
                // 🚨 ГРЕШКА 2: hasRole("ROLE_ADMIN") търси "ROLE_ROLE_ADMIN" и връща 403 Forbidden!
                .requestMatchers("/admin/**").hasRole("ROLE_ADMIN")
                // 🚨 ГРЕШКА 3: По-общото правило anyRequest() е сложено ПРЕДИ конкретните пътища!
                // Кодът гърми при стартиране: "Can't configure antMatchers after anyRequest"!
                .anyRequest().authenticated()
                .requestMatchers("/public/**").permitAll()
            )
            .build();
    }
}
```
*   **Как да се избегне:** **Никога** не пиши `"ROLE_"` вътре в `hasRole(...)` — пиши само `hasRole("ADMIN")`. Подреждай правилата от **най-конкретните отгоре** към **`anyRequest()` най-отдолу**. Винаги използвай **`BCryptPasswordEncoder`** (или `Argon2PasswordEncoder`).

### 🟢 ПРАВИЛЕН КОД (Чист Spring Security 6 `SecurityFilterChain` + `@EnableMethodSecurity`):
```java
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
@EnableMethodSecurity // Активира @PreAuthorize върху методите в Service / Controller!
public class SecurityConfig {

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder(12); // Strength 12 (индустриален стандарт)
    }

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        return http
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/", "/login", "/register", "/css/**", "/js/**").permitAll()
                        .requestMatchers("/admin/**").hasRole("ADMIN")
                        .anyRequest().authenticated()
                )
                .formLogin(form -> form
                        .loginPage("/login")
                        .defaultSuccessUrl("/dashboard", true)
                        .permitAll()
                )
                .logout(logout -> logout
                        .logoutUrl("/logout")
                        .logoutSuccessUrl("/login?logout")
                        .invalidateHttpSession(true)
                        .deleteCookies("JSESSIONID")
                )
                .build();
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Кога **ТРЯБВА** да оставим **CSRF защитата включена** и кога е безопасно да я изключим (`.csrf(AbstractHttpConfigurer::disable)`) в Spring Security?“*
*   **Уловката:** Начинаещите разработчици изключват CSRF сляпо навсякъде, защото виждат `403 Forbidden` при `POST` заявки в Postman, без да осъзнават, че отварят дупка в сигурността!
*   **Точен Senior отговор:** 
    1.  **CSRF (Cross-Site Request Forgery)** атакува приложения, които използват **бисквитки (`JSESSIONID`) за автоматична автентикация**, защото браузърът изпраща бисквитките автоматично при заявка от злонамерен външен сайт. Затова при **MVC приложения с форми и сесии (Thymeleaf)** CSRF защитата **ЗАДЪЛЖИТЕЛНО трябва да е ВКЛЮЧЕНА**!
    2.  При **Stateless REST API-та, които използват `Authorization: Bearer <JWT>` заглавие** (и **не** пазят токена в обикновена бисквитка), браузърът никога не изпраща заглавието `Authorization` автоматично при заявка от чужд сайт — затова в този случай CSRF атаката е невъзможна и е напълно безопасно (и препоръчително) да изключим CSRF защитата.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING SECURITY 6) СРЕЩУ C# 12+ (ASP.NET CORE IDENTITY)

| Архитектурен детайл | ☕ Java 21+ (Spring Security 6) | 🟣 C# 12+ (ASP.NET Core 8 Identity) |
| :--- | :--- | :--- |
| **Входна точка на сигурността** | `SecurityFilterChain` Bean (Servlet Filter верига) | `app.UseAuthentication()` + `app.UseAuthorization()` в `Program.cs` |
| **Интерфейс за зареждане на потребител** | `UserDetailsService` ➔ връща `UserDetails` | `IUserStore<TUser>` / `UserManager<TUser>` |
| **Защита на ниво метод / сървис** | `@PreAuthorize("hasRole('ADMIN')")` (работи и върху `@Service`!) | `[Authorize(Policy = "...")]` (работи само върху Controllers / Razor Pages) |
| **Достъп до текущия потребител** | `SecurityContextHolder.getContext().getAuthentication()` | `HttpContext.User` (`ClaimsPrincipal`) |
| **Алгоритъм за пароли по подразбиране** | `BCryptPasswordEncoder` (или `DelegatingPasswordEncoder`) | `PBKDF2` с HMAC-SHA512 (310 000 итерации в .NET 8) |

---
[⬅️ 05-01. Web API & Global Error Handling](05-01-00-REST-And-Error-Handling.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-02-01. Syntax From Scratch](05-02-01-Syntax-From-Scratch.md)
