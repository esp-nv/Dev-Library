[⬅️ 04-03-00. Теория & Капани](04-03-00-Spring-Core-IoC.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-04-00. Spring MVC with Thymeleaf](04-04-00-Spring-MVC-Thymeleaf.md)

---

# 04-03-01. Spring Core (IoC Container, Beans & DI): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на `@Configuration` и `@Bean` методи, `@Primary` и `@Qualifier`, `@PostConstruct` / `@PreDestroy`, `Bean Scopes` (`@Scope`) и `ObjectProvider<T>` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Конфигурационен клас (`@Configuration`) и ръчно създаване на `@Bean`](#1-конфигурационен-клас-configuration-и-ръчно-създаване-на-bean)
* [2. Избор между множество имплементации (`@Primary`, `@Qualifier` и `Map<String, Interface>`)](#2-избор-между-множество-имплементации-primary-qualifier-и-mapstring-interface)
* [3. Bean Scopes (`@Scope`) и `ObjectProvider<T>`](#3-bean-scopes-scope-и-objectprovidert)
* [4. Lifecycle Callbacks (`@PostConstruct` и `@PreDestroy`)](#4-lifecycle-callbacks-postconstruct-и-predestroy)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. КОНФИГУРАЦИОНЕН КЛАС (`@CONFIGURATION`) И РЪЧНО СЪЗДАВАНЕ НА `@BEAN`

Използва се, когато искаш да регистрираш в Spring контейнера клас от външна библиотека (напр. `ObjectMapper` или `PasswordEncoder`), върху който не можеш да сложиш `@Component`:

```java
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class AppBeansConfig {

    // Името на метода ("objectMapper") става името на Bean-а в ApplicationContext!
    @Bean
    public ObjectMapper objectMapper() {
        return new ObjectMapper().registerModule(new JavaTimeModule());
    }
}
```

---

## 2. ИЗБОР МЕЖДУ МНОЖЕСТВО ИМПЛЕМЕНТАЦИИ (`@PRIMARY`, `@QUALIFIER` И `MAP<STRING, INTERFACE>`)

```java
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.context.annotation.Primary;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.Map;

public interface NotificationSender {
    void send(String message);
}

@Service("emailSender")
@Primary // Ако никой не уточни @Qualifier, Spring ще избере този Bean по подразбиране!
public class EmailNotificationSender implements NotificationSender {
    @Override
    public void send(String message) { System.out.println("Email: " + message); }
}

@Service("smsSender")
public class SmsNotificationSender implements NotificationSender {
    @Override
    public void send(String message) { System.out.println("SMS: " + message); }
}

@Service
public class AlertDispatcher {
    private final NotificationSender defaultSender;
    private final NotificationSender smsSender;
    private final Map<String, NotificationSender> allSendersMap;

    public AlertDispatcher(
            NotificationSender defaultSender, // Взима @Primary (EmailNotificationSender)
            @Qualifier("smsSender") NotificationSender smsSender, // Взима конкретно SmsNotificationSender
            Map<String, NotificationSender> allSendersMap // Взима ВСИЧКИ имплементации като речник!
    ) {
        this.defaultSender = defaultSender;
        this.smsSender = smsSender;
        this.allSendersMap = allSendersMap;
    }
}
```

---

## 3. BEAN SCOPES (`@SCOPE`) И `OBJECTPROVIDER<T>`

```java
import org.springframework.beans.factory.ObjectProvider;
import org.springframework.beans.factory.config.ConfigurableBeanFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Service;
import java.util.UUID;

@Component
@Scope(ConfigurableBeanFactory.SCOPE_PROTOTYPE) // Нов обект при всяко поискване!
public class ReportGeneratorTask {
    private final String taskId = UUID.randomUUID().toString();
    public String getTaskId() { return taskId; }
}

@Service // Singleton
public class ReportService {
    private final ObjectProvider<ReportGeneratorTask> taskProvider;

    public ReportService(ObjectProvider<ReportGeneratorTask> taskProvider) {
        this.taskProvider = taskProvider;
    }

    public void runNewReport() {
        // Всеки път създава чисто нова инстанция на Prototype Bean-а!
        ReportGeneratorTask freshTask = taskProvider.getObject();
        System.out.println("Стартиране на задача ID: " + freshTask.getTaskId());
    }
}
```

---

## 4. LIFECYCLE CALLBACKS (`@POSTCONSTRUCT` И `@PREDESTROY`)

```java
import jakarta.annotation.PostConstruct;
import jakarta.annotation.PreDestroy;
import org.springframework.stereotype.Component;

@Component
public class InMemoryCacheLoader {

    @PostConstruct
    public void initCache() {
        // Изпълнява се автоматично веднага след като зависимостите са инжектирани
        System.out.println("🔄 Зареждане на референтните данни в паметта...");
    }

    @PreDestroy
    public void cleanUp() {
        // Изпълнява се при спиране на Spring Boot сървъра
        System.out.println("🧹 Освобождаване на ресурсите преди спиране...");
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Клас | Предназначение | Бележка |
| :--- | :--- | :--- |
| **`@Configuration` + `@Bean`** | Регистрира обект от метод в Spring контейнера | Задължително за външни библиотеки |
| **`@Primary`** | Задава основен Bean при множество имплементации | Избягва `NoUniqueBeanDefinitionException` |
| **`@Qualifier("beanName")`** | Избира конкретен Bean по име при инжектиране | Има по-висок приоритет от `@Primary` |
| **`@Scope("prototype")`** | Създава нова инстанция при всяко поискване | По подразбиране всички Beans са `"singleton"` |
| **`@RequestScope` / `@SessionScope`** | Уеб обхвати (1 обект на HTTP заявка / сесия) | Работят само в Web приложения |
| **`@Lazy`** | Отлага създаването на Bean-а до първото му ползване | По подразбиране Singleton Beans се правят Eager |
| **`@PostConstruct` / `@PreDestroy`** | Куки при инициализация и унищожаване на Bean | От пакета `jakarta.annotation.*` |

---
[⬅️ 04-03-00. Теория & Капани](04-03-00-Spring-Core-IoC.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-04-00. Spring MVC with Thymeleaf](04-04-00-Spring-MVC-Thymeleaf.md)
