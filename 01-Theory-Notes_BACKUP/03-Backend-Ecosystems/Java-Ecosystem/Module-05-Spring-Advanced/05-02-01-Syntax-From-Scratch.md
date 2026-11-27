[⬅️ 05-02-00. Теория & Капани](05-02-00-Spring-Security.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-03-00. Stateless Security with JWT](05-03-00-JWT-Authentication.md)

---

# 05-02-01. Spring Security 6+ & Authentication: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за имплементиране на персонализиран `UserDetailsService` с база данни, `UserDetails` адаптер, защита на методи с `@PreAuthorize` и извличане на текущо логнатия потребител в Java от нулата.

## 🧭 Съдържание на темата
* [1. Имплементация на `UserDetailsService` от база данни (PostgreSQL / JPA)](#1-имплементация-на-userdetailsservice-от-база-данни-postgresql--jpa)
* [2. Защита на методи в Service слоя с `@PreAuthorize` и SpEL](#2-защита-на-методи-в-service-слоя-с-preauthorize-и-spel)
* [3. Извличане на текущо логнатия потребител (`@AuthenticationPrincipal` & `SecurityContextHolder`)](#3-извличане-на-текущо-логнатия-потребител-authenticationprincipal--securitycontextholder)
* [4. Хеширане и проверка на пароли с `BCryptPasswordEncoder`](#4-хеширане-и-проверка-на-пароли-с-bcryptpasswordencoder)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИМПЛЕМЕНТАЦИЯ НА `USERDETAILSSERVICE` ОТ БАЗА ДАННИ (POSTGRESQL / JPA)

Spring Security не познава твоето `@Entity User`, затова създаваме клас-адаптер (`AppUserDetailsService`), който превежда твоето `User` Entity в стандартния интерфейс `org.springframework.security.core.userdetails.UserDetails`:

```java
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.*;
import org.springframework.stereotype.Service;
import java.util.List;

@Service
public class AppUserDetailsService implements UserDetailsService {

    private final UserRepository userRepository;

    public AppUserDetailsService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Override
    public UserDetails loadUserByUsername(String email) throws UsernameNotFoundException {
        UserEntity user = userRepository.findByEmail(email)
                .orElseThrow(() -> new UsernameNotFoundException("Потребител с имейл " + email + " не е намерен!"));

        // Превръщаме ролите от базата в GrantedAuthority с префикс "ROLE_"
        List<SimpleGrantedAuthority> authorities = user.getRoles().stream()
                .map(role -> new SimpleGrantedAuthority("ROLE_" + role.getName()))
                .toList();

        return org.springframework.security.core.userdetails.User.builder()
                .username(user.getEmail())
                .password(user.getPasswordHash())
                .authorities(authorities)
                .disabled(!user.isActive())
                .build();
    }
}
```

---

## 2. ЗАЩИТА НА МЕТОДИ В SERVICE СЛОЯ С `@PREAUTHORIZE` И SPEL

Изисква `@EnableMethodSecurity` върху конфигурационния клас:

```java
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Service;

@Service
public class ArticleManagementService {

    // 1. Само потребители с роля ADMIN или MODERATOR могат да изтриват статии
    @PreAuthorize("hasAnyRole('ADMIN', 'MODERATOR')")
    public void deleteArticle(Long articleId) {
        System.out.println("Изтриване на статия #" + articleId);
    }

    // 2. Собственикът на профила (#email == authentication.name) ИЛИ ADMIN
    @PreAuthorize("#email == authentication.name or hasRole('ADMIN')")
    public void updateProfile(String email, UpdateProfileDto dto) {
        System.out.println("Обновяване на профил за: " + email);
    }
}
```

---

## 3. ИЗВЛИЧАНЕ НА ТЕКУЩО ЛОГНАТИЯ ПОТРЕБИТЕЛ (`@AUTHENTICATIONPRINCIPAL` & `SECURITYCONTEXTHOLDER`)

```java
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class CurrentUserController {

    // Начин 1: Директно инжектиране в параметър на контролера (Най-чистият начин!)
    @GetMapping("/api/me")
    public String getMyProfile(@AuthenticationPrincipal UserDetails currentUser) {
        return "Логнат потребител: " + currentUser.getUsername() +
               " | Роли: " + currentUser.getAuthorities();
    }

    // Начин 2: От произволен Service клас чрез статичния SecurityContextHolder
    public String getCurrentUsernameInService() {
        return SecurityContextHolder.getContext().getAuthentication().getName();
    }
}
```

---

## 4. ХЕШИРАНЕ И ПРОВЕРКА НА ПАРОЛИ С `BCRYPTPASSWORDENCODER`

```java
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

@Service
public class AuthRegistrationService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;

    public AuthRegistrationService(UserRepository userRepository, PasswordEncoder passwordEncoder) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
    }

    public void register(String email, String rawPassword) {
        // Генерира 60-символен BCrypt хеш със случаен Salt (напр. "\$2a\$12\$R9h/cIPz0gi...")
        String hashedPassword = passwordEncoder.encode(rawPassword);

        UserEntity newUser = new UserEntity(email, hashedPassword);
        userRepository.save(newUser);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Клас | Предназначение | Важен детайл |
| :--- | :--- | :--- |
| **`SecurityFilterChain`** | Основен `@Bean` за конфигурация на HTTP сигурността | Заменя остарелия `WebSecurityConfigurerAdapter` |
| **`@EnableMethodSecurity`** | Включва `@PreAuthorize` и `@PostAuthorize` | Слага се върху `@Configuration` клас |
| **`@PreAuthorize("hasRole('ADMIN')")`** | Проверява права преди изпълнение на метода | При неуспех хвърля `AccessDeniedException` (403) |
| **`@AuthenticationPrincipal`** | Инжектира логнатия `UserDetails` в контролера | Връща `null`, ако ендпойнтът е `permitAll()` за гости |
| **`BCryptPasswordEncoder`** | Еднопосочно криптиране на пароли със Salt | Използвай `passwordEncoder.matches(raw, hash)` за проверка |
| **`UserDetailsService`** | Интерфейс за зареждане на потребител по username/email | Единствен метод: `loadUserByUsername(String)` |

---
[⬅️ 05-02-00. Теория & Капани](05-02-00-Spring-Security.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-03-00. Stateless Security with JWT](05-03-00-JWT-Authentication.md)