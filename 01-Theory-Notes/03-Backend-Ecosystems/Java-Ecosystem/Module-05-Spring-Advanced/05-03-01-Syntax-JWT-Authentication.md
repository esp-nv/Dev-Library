[⬅️ 05-03-00. Теория & Капани](05-03-00-JWT-Authentication.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-04-00. Spring AOP & Interceptors](05-04-00-Spring-AOP.md)

---

# 05-03-01. Stateless Security with JWT & Refresh Tokens: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за генериране и валидиране на JWT токени с модерния **JJWT 0.12.x+ API** (`JwtService`), създаване на `JwtAuthenticationFilter` (`OncePerRequestFilter`), REST ендпойнт за вход (`/api/v1/auth/login`) с `AuthenticationManager` и конфигурация в Java от нулата.

## 🧭 Съдържание на темата
* [1. `JwtService` — Генериране, парсване и валидация на JWT (JJWT 0.12.x+)](#1-jwtservice--генериране-парсване-и-валидация-на-jwt-jjwt-012x)
* [2. `JwtAuthenticationFilter` (`OncePerRequestFilter`)](#2-jwtauthenticationfilter-onceperrequestfilter)
* [3. `AuthController` (`/api/v1/auth/login`) и `AuthenticationManager` Bean](#3-authcontroller-apiv1authlogin-и-authenticationmanager-bean)
* [4. Конфигурация на тайния ключ в `application.properties`](#4-конфигурация-на-тайния-ключ-в-applicationproperties)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. `JWTSERVICE` — ГЕНЕРИРАНЕ, ПАРСВАНЕ И ВАЛИДАЦИЯ НА JWT (JJWT 0.12.X+)

Изисква зависимостите `io.jsonwebtoken:jjwt-api`, `jjwt-impl` и `jjwt-jackson` (версия `0.12.5+`):

```java
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.io.Decoders;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.util.Date;
import java.util.List;
import java.util.function.Function;

@Service
public class JwtService {

    @Value("${app.jwt.secret}")
    private String secretKeyBase64;

    @Value("${app.jwt.expiration-ms:900000}") // По подразбиране 15 минути (900 000 ms)
    private long jwtExpirationMs;

    // 1. Генериране на JWT Access Token с потребителско име и роли
    public String generateToken(UserDetails userDetails) {
        List<String> roles = userDetails.getAuthorities().stream()
                .map(GrantedAuthority::getAuthority)
                .toList();

        return Jwts.builder()
                .subject(userDetails.getUsername())
                .claim("roles", roles)
                .issuedAt(new Date(System.currentTimeMillis()))
                .expiration(new Date(System.currentTimeMillis() + jwtExpirationMs))
                .signWith(getSigningKey())
                .compact();
    }

    // 2. Извличане на username (subject) от токена
    public String extractUsername(String token) {
        return extractClaim(token, Claims::getSubject);
    }

    // 3. Проверка дали токенът принадлежи на потребителя и не е изтекъл
    public boolean isTokenValid(String token, UserDetails userDetails) {
        final String username = extractUsername(token);
        return username.equals(userDetails.getUsername()) && !isTokenExpired(token);
    }

    private boolean isTokenExpired(String token) {
        return extractClaim(token, Claims::getExpiration).before(new Date());
    }

    private <T> T extractClaim(String token, Function<Claims, T> claimsResolver) {
        Claims claims = Jwts.parser()
                .verifyWith(getSigningKey())
                .build()
                .parseSignedClaims(token)
                .getPayload();
        return claimsResolver.apply(claims);
    }

    private SecretKey getSigningKey() {
        byte[] keyBytes = Decoders.BASE64.decode(secretKeyBase64);
        return Keys.hmacShaKeyFor(keyBytes);
    }
}
```

---

## 2. `JWTAUTHENTICATIONFILTER` (`ONCEPERREQUESTFILTER`)

Този филтър прехваща всяка входяща заявка, проверява за `Authorization: Bearer <token>`, валидира токена и зарежда потребителя в `SecurityContextHolder`:

```java
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.lang.NonNull;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

@Component
public class JwtAuthenticationFilter extends OncePerRequestFilter {

    private final JwtService jwtService;
    private final UserDetailsService userDetailsService;

    public JwtAuthenticationFilter(JwtService jwtService, UserDetailsService userDetailsService) {
        this.jwtService = jwtService;
        this.userDetailsService = userDetailsService;
    }

    @Override
    protected void doFilterInternal(@NonNull HttpServletRequest request,
                                    @NonNull HttpServletResponse response,
                                    @NonNull FilterChain filterChain) throws ServletException, IOException {

        final String authHeader = request.getHeader("Authorization");

        // 1. Ако няма заглавие или не започва с "Bearer ", пускаме заявката нататък (за публични страници)
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            filterChain.doFilter(request, response);
            return;
        }

        final String jwt = authHeader.substring(7); // Изрязва "Bearer "
        final String userEmail = jwtService.extractUsername(jwt);

        // 2. Ако има имейл и потребителят още не е автентикиран в текущия контекст
        if (userEmail != null && SecurityContextHolder.getContext().getAuthentication() == null) {
            UserDetails userDetails = this.userDetailsService.loadUserByUsername(userEmail);

            if (jwtService.isTokenValid(jwt, userDetails)) {
                UsernamePasswordAuthenticationToken authToken = new UsernamePasswordAuthenticationToken(
                        userDetails,
                        null,
                        userDetails.getAuthorities()
                );
                authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                // 3. Записваме потребителя като автентикиран за текущата HTTP заявка!
                SecurityContextHolder.getContext().setAuthentication(authToken);
            }
        }

        filterChain.doFilter(request, response);
    }
}
```

---

## 3. `AUTHCONTROLLER` (`/API/V1/AUTH/LOGIN`) И `AUTHENTICATIONMANAGER` BEAN

```java
// В SecurityConfig регистрираме AuthenticationManager като Bean:
// @Bean
// public AuthenticationManager authenticationManager(AuthenticationConfiguration config) throws Exception {
//     return config.getAuthenticationManager();
// }

import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

record LoginRequestDto(String email, String password) {}
record AuthResponseDto(String accessToken, String tokenType) {}

@RestController
@RequestMapping("/api/v1/auth")
public class AuthController {

    private final AuthenticationManager authenticationManager;
    private final JwtService jwtService;

    public AuthController(AuthenticationManager authenticationManager, JwtService jwtService) {
        this.authenticationManager = authenticationManager;
        this.jwtService = jwtService;
    }

    @PostMapping("/login")
    public ResponseEntity<AuthResponseDto> login(@RequestBody @Valid LoginRequestDto request) {
        // Spring проверява имейла и BCrypt паролата (хвърля BadCredentialsException при грешка)
        Authentication auth = authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(request.email(), request.password())
        );

        UserDetails userDetails = (UserDetails) auth.getPrincipal();
        String jwtToken = jwtService.generateToken(userDetails);

        return ResponseEntity.ok(new AuthResponseDto(jwtToken, "Bearer"));
    }
}
```

---

## 4. КОНФИГУРАЦИЯ НА ТАЙНИЯ КЛЮЧ В `APPLICATION.PROPERTIES`

Ключът за `HS256` **задължително** трябва да е поне **256 бита (32 байта / 44 Base64 символа)**, иначе JJWT хвърля `WeakKeyException`:

```properties
# В продукция се подава от променлива на средата JWT_SECRET!
app.jwt.secret=\${JWT_SECRET:404E635266556A586E3272357538782F413F4428472B4B6250645367566B5970}
app.jwt.expiration-ms=900000
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Стандартно JWT Claim име | Метод в JJWT 0.12+ | Какво описва |
| :--- | :--- | :--- |
| **`sub` (Subject)** | `.subject(email)` | Идентификатор на потребителя (email / userId) |
| **`iat` (Issued At)** | `.issuedAt(new Date())` | Точен момент на издаване на токена |
| **`exp` (Expiration)** | `.expiration(expDate)` | Момент на изтичане на валидността |
| **`iss` (Issuer)** | `.issuer("bookstore-api")` | Кой сървър е издал токена |
| **Потребителски Claims** | `.claim("roles", rolesList)` | Допълнителни данни (роли, tenantId) |
| **Заглавие на заявката** | `Authorization: Bearer <jwt>` | Стандартният HTTP Header за изпращане на токена |

---
[⬅️ 05-03-00. Теория & Капани](05-03-00-JWT-Authentication.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-04-00. Spring AOP & Interceptors](05-04-00-Spring-AOP.md)