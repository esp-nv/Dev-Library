[⬅️ 05-02. Spring Security 6+ & Authentication](05-02-00-Spring-Security.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-03-01. Syntax From Scratch](05-03-01-Syntax-From-Scratch.md)

---

# 05-03-00. Stateless Security with JWT & Refresh Tokens (Теория & Капани)

**Модул 05:** Spring Advanced | **Лекционна тема 03:** Как работи **Stateless автентикацията** с **JWT (JSON Web Tokens — RFC 7519)** в Spring Boot 3, анатомията на токена (`Header.Payload.Signature`), разликата между подписан (**JWS**) и криптиран (**JWE**) токен, архитектурата на **`JwtAuthenticationFilter` (`OncePerRequestFilter`)**, двойката **Access Token + Refresh Token** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на JWT Authentication Flow (Блок-схема)](#2-архитектура-на-jwt-authentication-flow-блок-схема)
* [3. Сравнителна карта в 4 езика (Java JJWT, C# JwtBearer, Python PyJWT, JS jose)](#3-сравнителна-карта-в-4-езика-java-jjwt-c-jwtbearer-python-pyjwt-js-jose)
* [4. Плюсове и минуси на JWT (Stateless) срещу Сървърни Сесии (Stateful)](#4-плюсове-и-минуси-на-jwt-stateless-срещу-сървърни-сесии-stateful)
* [5. Кодов одит: Правилен срещу Грешен код (Записване на пароли в JWT и забравен `SessionCreationPolicy.STATELESS`)](#5-кодов-одит-правилен-срещу-грешен-код-записване-на-пароли-в-jwt-и-забравен-sessioncreationpolicystateless)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Security 6 + JJWT) срещу C# 12+ (ASP.NET Core 8 JwtBearer)](#7-архитектурно-сравнение-java-21-spring-security-6--jjwt-срещу-c-12-aspnet-core-8-jwtbearer)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трите части на всеки JWT низ (`xxxxx.yyyyy.zzzzz`, разделени с точка `.`):**
    1.  **Header (Заглавие — Base64Url):** Казва какъв алгоритъм е използван (напр. `{"alg": "HS256", "typ": "JWT"}`).
    2.  **Payload (Съдържание / Claims — Base64Url):** Съдържа данните на потребителя (`sub`: имейл/ID, `roles`: `["ROLE_ADMIN"]`, `iat`: кога е издаден, `exp`: кога изтича). **ВАЖНО:** Payload **НЕ е криптиран**, а само кодиран с Base64Url — всеки може да го прочете на `jwt.io`!
    3.  **Signature (Цифров подпис):** Изчислява се на сървъра по формулата `HMACSHA256(base64(header) + "." + base64(payload), SECRET_KEY)`. Ако хакер промени и една буква в Payload-а (напр. смени `USER` на `ADMIN`), подписът вече няма да съвпада и сървърът отхвърля токена!
*   **Access Token срещу Refresh Token (Дневен билет срещу Абонаментна карта):**
    *   **Access Token:** Живее много кратко (**5–15 минути**), носи се в заглавието `Authorization: Bearer <token>` и дава достъп до API ендпойнтите.
    *   **Refresh Token:** Живее дълго (**7–30 дни**), пази се в сигурна **`HttpOnly` бисквитка** и служи единствено за издаване на нов Access Token, когато старият изтече.

---

## 2. АРХИТЕКТУРА НА JWT AUTHENTICATION FLOW (БЛОК-СХЕМА)

```text
[ Клиент ]                                      [ Spring Boot 3 + Spring Security 6 ]
    │ 1. POST /api/v1/auth/login (email, pass)                 │
    ├─────────────────────────────────────────────────────────►│ AuthenticationManager проверява паролата
    │ 2. Връща JWT: eyJhbGciOiJIUzI1NiJ9.eyJzdWI...            │ JwtService подписва токена със SECRET_KEY
    │◄─────────────────────────────────────────────────────────┤
    │                                                          │
    │ 3. GET /api/v1/orders                                    │
    │    Header: Authorization: Bearer eyJhbGciOi...           │
    ├─────────────────────────────────────────────────────────►│ JwtAuthenticationFilter (OncePerRequestFilter):
    │                                                          │  • Валидира подписа и срока (exp)
    │ 4. 200 OK + Данни за поръчките                           │  • Записва User в SecurityContextHolder
    │◄─────────────────────────────────────────────────────────┤  • Пуска заявката към Controller-а!
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA JJWT, C# JWTBEARER, PYTHON PYJWT, JS JOSE)

| Концепция | ☕ Java 21+ (Spring + `jjwt`) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (`PyJWT` / FastAPI) | 🟡 JS / TS (`jose` / `jsonwebtoken`) |
| :--- | :--- | :--- | :--- | :--- |
| **Популярна JWT библиотека** | `io.jsonwebtoken:jjwt-api` (0.12+) | `Microsoft.AspNetCore.Authentication.JwtBearer` | `PyJWT` (`import jwt`) | `jose` (модерен стандарт) |
| **Генериране на токен** | `Jwts.builder().subject(u).signWith(key).compact()` | `new JwtSecurityTokenHandler().WriteToken(...)` | `jwt.encode(payload, key, algorithm="HS256")` | `await new SignJWT(payload).sign(key)` |
| **Проверка и парсване** | `Jwts.parser().verifyWith(key).build()...` | Автоматично чрез `AddJwtBearer(...)` | `jwt.decode(token, key, algorithms=["HS256"])` | `await jwtVerify(token, key)` |
| **Изключване на сървърни сесии** | `SessionCreationPolicy.STATELESS` | Не се извиква `app.UseSession()` | Stateless по подразбиране в FastAPI | `{ session: false }` в Passport |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА JWT (STATELESS) СРЕЩУ СЪРВЪРНИ СЕСИИ (STATEFUL)

### 🟢 Предимства (Pros):
*   **Нулево натоварване на базата/Redis при проверка на автентикация:** Сървърът проверява валидността на токена само с математическа криптографска проверка в CPU за микросекунди, без да прави `SELECT` заявка.
*   **Идеален за микросървиси и SSO:** Един Auth сървър издава токена, а 15 други микросървиса го валидират независимо.

### 🔴 Недостатъци (Cons):
*   **Невъзможност за моментално анулиране (Revocation) без Blacklist:** Ако потребител смени паролата си или бъде блокиран, издаденият Access Token остава валиден до изтичането си (`exp`), освен ако не поддържаш кратък живот (5 мин) или Redis Blacklist.

---
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ЗАПИСВАНЕ НА ПАРОЛИ В JWT И ЗАБРАВЕН `SESSIONCREATIONPOLICY.STATELESS`)

### ❌ ГРЕШЕН КОД (Чувствителни данни в Payload и липсващ `STATELESS` режим):
```java
// 🚨 ГРЕШКА 1 (Критична уязвимост): Payload на JWT се декодира с Base64 от всеки!
// Никога не слагай пароли, ЕГН или банкови данни в Claims на JWT!
String badToken = Jwts.builder()
        .subject("ivan@mail.bg")
        .claim("password", "MySecretPass123") // ВИДИМО ЗА ВСЕКИ!
        .compact(); // 🚨 ГРЕШКА 2: Липсва .signWith(key)! Токенът е неподписан ("alg": "none")!

// 🚨 ГРЕШКА 3: В SecurityFilterChain е забравено:
// .sessionManagement(s -> s.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
// Spring Security ще създаде JSESSIONID бисквитка и ще кешира потребителя в сесията!
```
*   **Как да се избегне:** Пази в JWT само публични идентификатори (`sub` = userId/email, `roles`, `iat`, `exp`). Винаги подписвай токена със силен криптографски ключ (минимум 256 бита за `HS256`) и задължително конфигурирай **`SessionCreationPolicy.STATELESS`**.

### 🟢 ПРАВИЛЕН КОД (Чист Stateless `SecurityFilterChain` с `JwtAuthenticationFilter`):
```java
@Bean
public SecurityFilterChain securityFilterChain(HttpSecurity http,
                                               JwtAuthenticationFilter jwtFilter) throws Exception {
    return http
            .csrf(AbstractHttpConfigurer::disable)
            .sessionManagement(session ->
                    session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .authorizeHttpRequests(auth -> auth
                    .requestMatchers("/api/v1/auth/**").permitAll()
                    .anyRequest().authenticated()
            )
            // Поставяме нашия JWT филтър ПРЕДИ стандартния UsernamePasswordAuthenticationFilter!
            .addFilterBefore(jwtFilter, UsernamePasswordAuthenticationFilter.class)
            .build();
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между симетрично подписване (**`HS256`**) и асиметрично подписване (**`RS256`**) при JWT, и кога в реалната архитектура задължително избираме `RS256`?“*
*   **Уловката:** В малки проекти всички ползват `HS256` (един общ таен ключ), но в микросървисна архитектура това създава сериозен риск!
*   **Точен Senior отговор:** 
    1.  **`HS256` (HMAC + SHA-256 — Симетричен алгоритъм):** Използва **един и същ таен ключ (`SECRET_KEY`)** както за *създаване* на токена, така и за *проверката* му. Подходящ е за монолитно приложение, където един и същ сървър издава и проверява токените.
    2.  **`RS256` (RSA + SHA-256 — Асиметричен алгоритъм):** Използва **двойка ключове**:
        *   **Private Key (Частен ключ):** Пази се **само в Auth сървъра** — единствено той може да издава и подписва нови JWT токени.
        *   **Public Key (Публичен ключ):** Раздава се свободно на всички останали микросървиси (`OrderService`, `PaymentService`, API Gateway) чрез стандартния ендпойнт **JWKS (`/.well-known/jwks.json`)**. Те могат само да *проверяват* дали токенът е истински, но дори някой микросървис да бъде хакнат, хакерът **не може да фалшифицира нови токени**, защото няма Private Key!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING SECURITY 6 + JJWT) СРЕЩУ C# 12+ (ASP.NET CORE 8 JWTBEARER)

| Архитектурен детайл | ☕ Java 21+ (Spring Security 6 + JJWT) | 🟣 C# 12+ (ASP.NET Core 8 JwtBearer) |
| :--- | :--- | :--- |
| **Четене и валидация на `Bearer` токен** | Пишеш собствен клас, наследяващ **`OncePerRequestFilter`** | Вграден Middleware `builder.Services.AddAuthentication().AddJwtBearer(...)` |
| **Защита срещу изпълнение 2 пъти на заявка** | Наследяване на **`OncePerRequestFilter`** (гарантира 1 изпълнение на HTTP заявка) | ASP.NET Core Middleware се изпълнява точно 1 път в конвейера |
| **Стандартен Claim за потребителско име** | `sub` (`Claims::getSubject`) | `ClaimTypes.NameIdentifier` или `JwtRegisteredClaimNames.Sub` |
| **Грешка при изтекъл токен (`ExpiredJwtException`)** | Хваща се във филтъра и се предава към `HandlerExceptionResolver` за статус `401` | Автоматично връща `401 Unauthorized` + header `WWW-Authenticate: Bearer error="invalid_token"` |
| **OAuth2 / OpenID Connect (Keycloak / Auth0)** | `spring-boot-starter-oauth2-resource-server` (вграден JWT декодер без JJWT!) | `AddJwtBearer` с `options.Authority = "https://example.com"` |

---
[⬅️ 05-02. Spring Security 6+ & Authentication](05-02-00-Spring-Security.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-03-01. Syntax From Scratch](05-03-01-Syntax-From-Scratch.md)
