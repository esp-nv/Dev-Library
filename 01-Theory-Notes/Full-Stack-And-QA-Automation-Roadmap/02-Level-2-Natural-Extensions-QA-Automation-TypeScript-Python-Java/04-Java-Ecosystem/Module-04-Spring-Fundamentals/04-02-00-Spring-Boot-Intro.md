[⬅️ 04-01. Internet Explained & HTTP Protocol](04-01-00-Internet-And-HTTP.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-02-01. Syntax From Scratch](04-02-01-Syntax-From-Scratch.md)

---

# 04-02-00. Spring Boot Introduction & Layered Architecture (Теория, Auto-Configuration & Капани)

**Модул 04:** Spring Fundamentals | **Лекционна тема 02:** Вътрешен механизъм на Spring Boot (**Auto-Configuration**, **Starters**, **Embedded Tomcat**, **Fat/Uber JAR**), стандартна структура на проекта (`src/main/java` и `src/main/resources`), четирислойна архитектура (`Web ➔ Service ➔ Repository ➔ Domain/Database`), профили (`application-dev.properties` / `prod`), `CommandLineRunner` и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Стандартна архитектура и пакетиране на Spring Boot проект (Блок-схема)](#2-стандартна-архитектура-и-пакетиране-на-spring-boot-проект-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot, ASP.NET Core, Django, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-aspnet-core-django-nestjs)
* [4. Плюсове и минуси на Spring Boot Starters & Auto-Configuration](#4-плюсове-и-минуси-на-spring-boot-starters--auto-configuration)
* [5. Кодов одит: Правилен срещу Грешен код (Хардкоднати пароли в Git и грешна структура на пакетите)](#5-кодов-одит-правилен-срещу-грешен-код-хардкоднати-пароли-в-git-и-грешна-структура-на-пакетите)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot 3) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot-3-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`Spring Boot Starter` = Готово меню в ресторант:** Вместо да поръчваш поотделно картофи, салата, сос и напитка и да проверяваш дали си отиват, поръчваш едно меню (`spring-boot-starter-web`). То автоматично ти носи Tomcat сървър + Spring MVC + Jackson (JSON) + Hibernate Validator в **100% съвместими версии**.
*   **`Fat JAR (Uber JAR)` = Самодостатъчен куфар:** Когато компилираш Spring Boot приложение с `mvn clean package`, получаваш един единствен `.jar` файл, който съдържа **и твоя код, и всички библиотеки, и целия уеб сървър Tomcat вътре в себе си**. Пускаш го на всеки сървър само с `java -jar app.jar`!
*   **Правилото за пакета на главния клас:** Винаги слагай `Application.java` в **основния пакет** (напр. `bg.softuni.bookstore`), а всички други пакети (`web`, `service`, `repository`, `model`) като **негови подпакети**, за да ги намери `@ComponentScan` автоматично!

---

## 2. СТАНДАРТНА АРХИТЕКТУРА И ПАКЕТИРАНЕ НА SPRING BOOT ПРОЕКТ (БЛОК-СХЕМА)

```text
bookstore/
├── pom.xml                                  ◄── Maven зависимости (Starters)
└── src/
    ├── main/
    │   ├── java/bg/softuni/bookstore/
    │   │   ├── BookstoreApplication.java    ◄── @SpringBootApplication (Корен!)
    │   │   ├── config/                      ◄── @Configuration класове (Security, Beans)
    │   │   ├── web/                         ◄── @Controller / @RestController
    │   │   ├── service/                     ◄── @Service (Бизнес логика & @Transactional)
    │   │   ├── repository/                  ◄── JpaRepository интерфейси
    │   │   └── model/
    │   │       ├── entity/                  ◄── @Entity класове (БД таблици)
    │   │       └── dto/                     ◄── Request/Response DTO records
    │   └── resources/
    │       ├── application.properties       ◄── Общи настройки
    │       ├── application-dev.properties   ◄── Настройки за разработка
    │       ├── static/                      ◄── CSS, JS, изображения
    │       └── templates/                   ◄── Thymeleaf HTML шаблони
    └── test/java/bg/softuni/bookstore/      ◄── JUnit 5 & Mockito тестове
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT, ASP.NET CORE, DJANGO, NESTJS)

| Концепция | ☕ Java 21+ (Spring Boot 3) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (Django) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Генератор на нов проект** | **Spring Initializr** (`start.spring.io`) | `dotnet new webapi` | `django-admin startproject` | `nest new project-name` |
| **Среди / Профили (Dev/Prod)** | `spring.profiles.active=dev` | `ASPNETCORE_ENVIRONMENT=Development` | `DJANGO_SETTINGS_MODULE` | `NODE_ENV=development` |
| **Типово-безопасна конфигурация** | `@ConfigurationProperties(prefix = "app")` | `builder.Services.Configure<AppOptions>(...)` | `pydantic-settings` (`BaseSettings`) | `@nestjs/config` (`registerAs`) |
| **Код при стартиране (Seeder)** | `CommandLineRunner` / `ApplicationRunner` | `IHostedService` / код в `Program.cs` | Custom management command / `ready()` | `OnModuleInit` интерфейс |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SPRING BOOT STARTERS & AUTO-CONFIGURATION

### 🟢 Предимства (Pros):
*   **Нулеви конфликти на версии (Bill of Materials — BOM):** `spring-boot-starter-parent` гарантира, че версиите на Hibernate, Jackson, Tomcat, Logback и JUnit 5 са тествани заедно и работят безотказно.
*   **Пълна прозрачност при нужда:** Всичко, което Auto-Configuration прави автоматично, може да бъде презаписано веднага щом дефинираш свой собствен `@Bean` или настройка в `application.properties`.

### 🔴 Недостатъци (Cons):
*   **„Магия“, която скрива детайлите от начинаещите:** Ако в `pom.xml` оставиш зависимост като `spring-boot-starter-security`, Spring Boot автоматично заключва всички URL адреси с форма за вход още преди да си написал и един ред security конфигурация.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ХАРДКОДНАТИ ПАРОЛИ В GIT И ГРЕШНА СТРУКТУРА НА ПАКЕТИТЕ)

### ❌ ГРЕШЕН КОД (Записване на реални пароли и API ключове директно в `application.properties`):
```properties
# 🚨 ГРЕШКА 1 (Security Breach): Качване на реални пароли и тайни ключове в GitHub!
spring.datasource.password=MySuperSecretProdPassword123!
app.jwt.secret=9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
```
*   **Как да се избегне:** В `application.properties` използвай **Environment Variables с резервна стойност по подразбиране** (`${DB_PASSWORD:local_dev_pass}`), така че в продукция сървърът да подава реалната парола от променливите на средата, а в Git никога да не влизат тайни!

### 🟢 ПРАВИЛЕН КОД (Безопасна конфигурация с Environment Variables и `@ConfigurationProperties`):
```properties
# application.properties (Безопасен за качване в GitHub!)
spring.application.name=bookstore-api
server.port=\${SERVER_PORT:8080}

spring.datasource.url=\${DB_URL:jdbc:postgresql://localhost:5432/bookstore}
spring.datasource.username=\${DB_USERNAME:postgres}
spring.datasource.password=\${DB_PASSWORD:postgres}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Как точно работи **Spring Boot Auto-Configuration** под капака и какво прави анотацията **`@ConditionalOnClass`**?“*
*   **Уловката:** Всеки казва *„Spring Boot сам настройва всичко“*, но Senior интервюиращият иска да чуе точния механизъм!
*   **Точен Senior отговор:** При стартиране `@EnableAutoConfiguration` прочита списък с готови конфигурационни класове от файла `META-INF/spring/org.springframework.boot.autoconfigure.AutoConfiguration.imports` вътре в библиотеката `spring-boot-autoconfigure.jar`. Всеки от тези класове е защитен с условни анотации (**Conditional Annotations**):
    *   **`@ConditionalOnClass(DataSource.class)`:** Активира се **само ако** даденият клас присъства в `pom.xml` (Classpath-а).
    *   **`@ConditionalOnMissingBean`:** Създава стандартния обект **само ако програмистът не е дефинирал свой собствен `@Bean`**.
    *   **`@ConditionalOnProperty`:** Активира се само при определена стойност в `application.properties`.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT 3) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Зареждане на настройки по приоритет** | Command Line args ➔ Env Vars ➔ `application-{profile}.properties` ➔ `application.properties` | Command Line args ➔ Env Vars ➔ User Secrets ➔ `appsettings.{Env}.json` ➔ `appsettings.json` |
| **Силно типизирани настройки** | `@ConfigurationProperties(prefix = "mail")` върху `record` или клас | `services.Configure<MailSettings>(config.GetSection("Mail"))` + `IOptions<MailSettings>` |
| **Запълване на начални данни (DB Seeding)** | Компонент, имплементиращ **`CommandLineRunner`** | `using var scope = app.Services.CreateScope();` преди `app.Run()` |
| **Автоматично рестартиране при промяна** | `spring-boot-devtools` | `dotnet watch run` (Hot Reload) |
| **Packaging за Deployment** | Самостоятелен **`Fat JAR`** (`java -jar app.jar`) или Docker image | Framework-dependent DLL или Self-contained binary (`dotnet publish`) |

---
[⬅️ 04-01. Internet Explained & HTTP Protocol](04-01-00-Internet-And-HTTP.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-02-01. Syntax From Scratch](04-02-01-Syntax-From-Scratch.md)
