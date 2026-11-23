[⬅️ 04-02-00. Теория & Капани](04-02-00-Spring-Boot-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-03-00. Spring Core (IoC & Beans)](04-03-00-Spring-Core-IoC.md)

---

# 04-02-01. Spring Boot Introduction & Layered Architecture: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за главен клас на Spring Boot приложение, `CommandLineRunner` (DB Seeder), силно типизирана конфигурация с `@ConfigurationProperties` и работа със Spring Profiles в Java от нулата.

## 🧭 Съдържание на темата
* [1. Главният клас на Spring Boot и `CommandLineRunner` (Initial Data Seeder)](#1-главният-клас-на-spring-boot-и-commandlinerunner-initial-data-seeder)
* [2. Силно типизирана конфигурация с `@ConfigurationProperties` и `record`](#2-силно-типизирана-конфигурация-с-configurationproperties-и-record)
* [3. Работа със Spring Profiles (`@Profile("dev")` / `@Profile("prod")`)](#3-работа-със-spring-profiles-profiledev--profileprod)
* [4. Пълен пример за трислойна архитектура (Entity ➔ Repository ➔ Service ➔ Controller)](#4-пълен-пример-за-трислойна-архитектура-entity--repository--service--controller)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ГЛАВНИЯТ КЛАС НА SPRING BOOT И `COMMANDLINERUNNER` (INITIAL DATA SEEDER)

```java
package bg.softuni.bookstore;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;

@SpringBootApplication
public class BookstoreApplication {

    public static void main(String[] args) {
        SpringApplication.run(BookstoreApplication.class, args);
    }

    // Изпълнява се автоматично 1 път веднага след като Spring контекстът се зареди!
    @Bean
    public CommandLineRunner seedDatabase(RoleRepository roleRepository) {
        return args -> {
            if (roleRepository.count() == 0) {
                roleRepository.save(new Role("ADMIN"));
                roleRepository.save(new Role("USER"));
                System.out.println("✅ Началните роли са заредени в базата данни!");
            }
        };
    }
}
```

---

## 2. СИЛНО ТИПИЗИРАНА КОНФИГУРАЦИЯ С `@CONFIGURATIONPROPERTIES` И `RECORD`

Вместо да разпръскваш `@Value("${app.mail.host}")` из 15 класа, групирай настройките в чист `record`:

```properties
# В application.properties:
app.storage.upload-dir=/var/www/uploads
app.storage.max-file-size-mb=10
```

```java
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.boot.context.properties.ConfigurationPropertiesScan;

// Добави @ConfigurationPropertiesScan върху главния клас BookstoreApplication
@ConfigurationProperties(prefix = "app.storage")
public record StorageProperties(String uploadDir, int maxFileSizeMb) {}

// Вече можеш да инжектираш StorageProperties директно в конструктора на всеки @Service!
```

---

## 3. РАБОТА СЪС SPRING PROFILES (`@PROFILE("DEV")` / `@PROFILE("PROD")`)

```java
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

public interface EmailSender {
    void send(String to, String subject, String body);
}

@Service
@Profile("dev") // Активира се САМО когато spring.profiles.active=dev
public class ConsoleMockEmailSender implements EmailSender {
    @Override
    public void send(String to, String subject, String body) {
        System.out.printf("[DEV MOCK EMAIL] До: %s | Тема: %s%n", to, subject);
    }
}

@Service
@Profile("prod") // Активира се САМО в продукционна среда!
public class SmtpRealEmailSender implements EmailSender {
    @Override
    public void send(String to, String subject, String body) {
        // Реално изпращане през SMTP сървър...
    }
}
```

---

## 4. ПЪЛЕН ПРИМЕР ЗА ТРИСЛОЙНА АРХИТЕКТУРА (ENTITY ➔ REPOSITORY ➔ SERVICE ➔ CONTROLLER)

```java
// 1. Repository слой
public interface TaskRepository extends JpaRepository<Task, Long> {}

// 2. Service слой (Бизнес логика)
@Service
public class TaskService {
    private final TaskRepository taskRepository;

    public TaskService(TaskRepository taskRepository) {
        this.taskRepository = taskRepository;
    }

    public List<Task> getAllTasks() {
        return taskRepository.findAll();
    }
}

// 3. Web / Controller слой (Тънък контролер)
@RestController
@RequestMapping("/api/tasks")
public class TaskController {
    private final TaskService taskService;

    public TaskController(TaskService taskService) {
        this.taskService = taskService;
    }

    @GetMapping
    public List<Task> listTasks() {
        return taskService.getAllTasks();
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Команда | Предназначение | Къде се използва |
| :--- | :--- | :--- |
| **`@SpringBootApplication`** | Стартира Auto-Configuration и ComponentScan | Върху главния `Application` клас |
| **`@Service`** | Маркира клас с бизнес логика като Spring Bean | В Service слоя |
| **`@Repository`** | Маркира DAO клас (и превежда SQL изключения) | При ръчни репозиторита (излишно за `JpaRepository`) |
| **`@Component`** | Обща стереотипна анотация за произволен Spring Bean | Помощни класове, парсъри, валидатори |
| **`@Value("${key:default}")`** | Инжектира единична стойност от `application.properties` | В полета или параметри на конструктор |
| **`@Profile("dev")`** | Регистрира Bean само при активен конкретен профил | Разделяне на Dev, Test и Prod конфигурации |
| **`mvn spring-boot:run`** | Компилира и стартира приложението от терминала | В корена на Maven проекта |

---
[⬅️ 04-02-00. Теория & Капани](04-02-00-Spring-Boot-Intro.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-03-00. Spring Core (IoC & Beans)](04-03-00-Spring-Core-IoC.md)
