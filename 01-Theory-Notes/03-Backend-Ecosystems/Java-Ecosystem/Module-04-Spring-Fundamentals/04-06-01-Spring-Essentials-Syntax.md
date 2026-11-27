
[⬅️ 04-06-00. Теория & Капани](04-06-00-Spring-Essentials.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ Module 05: Spring Advanced (Security & REST APIs)](../Module-05-Spring-Advanced/05-00-Intro.md)

---

# 04-06-01. Spring Essentials (Events, Scheduling & Caching): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за активиране и използване на Spring Events (`ApplicationEventPublisher` & `@EventListener`), планирани задачи с `@Scheduled` (Cron изрази), асинхронни методи с `@Async` и кеширане с `@Cacheable` / `@CacheEvict` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Активиране на Scheduling, Async и Caching в главния клас](#1-активиране-на-scheduling-async-и-caching-в-главния-клас)
* [2. Публикуване и слушане на събития (`ApplicationEventPublisher` & `@EventListener`)](#2-публикуване-и-слушане-на-събития-applicationeventpublisher--eventlistener)
* [3. Периодични задачи с `@Scheduled` и Cron изрази в Spring](#3-периодични-задачи-с-scheduled-и-cron-изрази-в-spring)
* [4. Кеширане на заявки с `@Cacheable`, `@CachePut` и `@CacheEvict`](#4-кеширане-на-заявки-с-cacheable-cacheput-и-cacheevict)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. АКТИВИРАНЕ НА SCHEDULING, ASYNC И CACHING В ГЛАВНИЯ КЛАС

```java
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cache.annotation.EnableCaching;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling // Активира @Scheduled задачите
@EnableAsync      // Активира @Async методите
@EnableCaching    // Активира @Cacheable / @CacheEvict кеширането
public class EssentialsApplication {
    public static void main(String[] args) {
        SpringApplication.run(EssentialsApplication.class, args);
    }
}
```

---

## 2. ПУБЛИКУВАНЕ И СЛУШАНЕ НА СЪБИТИЯ (`APPLICATIONEVENTPUBLISHER` & `@EVENTLISTENER`)

```java
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Service;

// 1. Дефиниция на събитието (Record)
public record UserRegisteredEvent(Long userId, String username, String email) {}

// 2. Издател на събитието (Publisher)
@Service
public class RegistrationService {
    private final ApplicationEventPublisher eventPublisher;

    public RegistrationService(ApplicationEventPublisher eventPublisher) {
        this.eventPublisher = eventPublisher;
    }

    public void registerUser(String username, String email) {
        // Логика за запис в базата данни...
        Long newUserId = 101L;

        // Изпраща събитието към всички слушатели в приложението
        eventPublisher.publishEvent(new UserRegisteredEvent(newUserId, username, email));
    }
}

// 3. Слушател на събитието (Listener) с SpEL филтър (condition)
@Component
public class WelcomeEmailListener {

    @EventListener(condition = "#event.email().endsWith('@softuni.bg')")
    public void sendVipWelcomeEmail(UserRegisteredEvent event) {
        System.out.println("Изпращане на специален имейл до: " + event.email());
    }
}
```

---

## 3. ПЕРИОДИЧНИ ЗАДАЧИ С `@SCHEDULED` И CRON ИЗРАЗИ В SPRING

*Внимание: В Spring Cron изразите имат **6 полета** (за разлика от 5-те полета в Linux crontab): `секунда минута час ден_от_месеца месец ден_от_седмицата`.*

```java
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;
import java.time.LocalDateTime;

@Component
public class MaintenanceScheduler {

    // 1. Изпълнява се 10 секунди след приключване на предишното изпълнение
    @Scheduled(fixedDelay = 10_000, initialDelay = 3_000)
    public void cleanExpiredTokens() {
        System.out.println("Почистване на изтекли токени: " + LocalDateTime.now());
    }

    // 2. Cron израз: Всеки ден в 03:00 сутринта (софийско време)
    @Scheduled(cron = "0 0 3 * * *", zone = "Europe/Sofia")
    public void generateDailyFinancialReport() {
        System.out.println("Генериране на дневен отчет в 03:00...");
    }

    // 3. Всеки делничен ден (Понеделник-Петък) на всеки 30 минути
    @Scheduled(cron = "0 */30 * * * MON-FRI")
    public void syncExternalRates() {
        System.out.println("Синхронизация на валутните курсове...");
    }
}
```

---

## 4. КЕШИРАНЕ НА ЗАЯВКИ С `@CACHEABLE`, `@CACHEPUT` И `@CACHEEVICT`

```java
import org.springframework.cache.annotation.*;
import org.springframework.stereotype.Service;

@Service
public class CountryCatalogService {

    // Записва резултата в кеш "countries" под ключ #code (напр. "BG")
    // unless = "#result == null" гарантира, че няма да кешираме null!
    @Cacheable(value = "countries", key = "#code", unless = "#result == null")
    public CountryDto getCountryByCode(String code) {
        // Този бавен SQL код ще се изпълни САМО 1 път за "BG"!
        return fetchFromDatabase(code);
    }

    // При промяна на държава обновява кеша под същия ключ
    @CachePut(value = "countries", key = "#dto.code()")
    public CountryDto updateCountry(CountryDto dto) {
        return saveToDatabase(dto);
    }

    // При изтриване премахва записа от кеша (allEntries = true чисти целия кеш)
    @CacheEvict(value = "countries", key = "#code")
    public void deleteCountry(String code) {
        deleteFromDatabase(code);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация | Предназначение | Важен параметър |
| :--- | :--- | :--- |
| **`@EventListener`** | Слуша за публикувани обекти чрез `ApplicationEventPublisher` | `condition = "#event.id > 0"` |
| **`@TransactionalEventListener`** | Изпълнява слушателя спрямо фазата на транзакцията | `phase = TransactionPhase.AFTER_COMMIT` |
| **`@Scheduled(fixedDelay = ms)`** | Пауза от `ms` след края на предишното изпълнение | `initialDelay = 5000` (забавяне при старт) |
| **`@Scheduled(cron = "s m h d M w")`** | Изпълнение по точен календар / час | `zone = "Europe/Sofia"` |
| **`@Async`** | Изпълнява метода в отделна фонова нишка | Трябва да е в отделен Bean (не `this.fn()`) |
| **`@Cacheable("cacheName")`** | Връща кеширан резултат, ако ключът съществува | `key = "#id"`, `unless = "#result == null"` |
| **`@CacheEvict("cacheName")`** | Изтрива остарели данни от кеша | `allEntries = true` (изчиства целия кеш) |

---
[⬅️ 04-06-00. Теория & Капани](04-06-00-Spring-Essentials.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ Module 05: Spring Advanced (Security & REST APIs)](../Module-05-Spring-Advanced/05-00-Intro.md)

