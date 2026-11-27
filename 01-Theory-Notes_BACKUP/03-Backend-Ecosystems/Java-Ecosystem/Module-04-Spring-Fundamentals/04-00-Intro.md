[⬅️ Назад към Модул 04 Индекс](README.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-01. Internet Explained & HTTP Protocol](04-01-00-Internet-And-HTTP.md)

---

# 04-00. Intro: Въведение в Spring Boot, MVC & Web Архитектура

**Модул 04:** Spring Fundamentals | **Цел:** Преход от конзолни програми и ORM към пълноценни уеб сървъри със **Spring Boot 3.x+**, разбиране на трислойната архитектура (`Controller ➔ Service ➔ Repository`) и директно сравнение с **C# (ASP.NET Core)**, **Python (Django / FastAPI)** и **JavaScript (Express / NestJS)**.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурна карта на Spring Boot Web приложение (Блок-схема)](#2-архитектурна-карта-на-spring-boot-web-приложение-блок-схема)
* [3. Сравнителна карта в 4 езика (Spring Boot, ASP.NET Core, Django, NestJS)](#3-сравнителна-карта-в-4-езика-spring-boot-aspnet-core-django-nestjs)
* [4. Плюсове и минуси на Spring Boot](#4-плюсове-и-минуси-на-spring-boot)
* [5. Кодов одит: Правилен срещу Грешен код (Бизнес логика в Controller и Field Injection `@Autowired`)](#5-кодов-одит-правилен-срещу-грешен-код-бизнес-логика-в-controller-и-field-injection-autowired)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Boot 3) срещу C# 12+ (ASP.NET Core 8)](#7-архитектурно-сравнение-java-21-spring-boot-3-срещу-c-12-aspnet-core-8)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Spring Framework срещу Spring Boot (Сурови съставки срещу Готова торта):**
    *   **Spring Framework** е огромната кутия със съставки — мощен е, но в миналото изискваше стотици редове XML конфигурация, ръчно настройване на сървър Tomcat и ръчно напасване на версиите на 30 библиотеки.
    *   **Spring Boot** е автоматизираната пекарна върху Spring Framework — дава ти готови пакетни зависимости (**Starters**), вграден уеб сървър (**Embedded Tomcat**), автоматична настройка (**Auto-Configuration**) и стартиране с един обикновен `main()` метод.
*   **Трите етажа на чистото Spring приложение (Layered Architecture):**
    1.  **Web / Controller слой (`@Controller` / `@RestController`):** Портиерът на входа — приема HTTP заявката, валидира входа (`@Valid`) и връща HTML или JSON. Не смята нищо!
    2.  **Service слой (`@Service`):** Мозъкът на приложението — съдържа бизнес логиката, проверките, транзакциите (`@Transactional`) и мапинга към DTO.
    3.  **Repository слой (`@Repository` / `JpaRepository`):** Складът — комуникира единствено с базата данни.

---

## 2. АРХИТЕКТУРНА КАРТА НА SPRING BOOT WEB ПРИЛОЖЕНИЕ (БЛОК-СХЕМА)

```text
[ Браузър / HTTP Клиент ]
          │  HTTP Request (GET / POST)
          ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Embedded Tomcat (Port 8080) ➔ DispatcherServlet (Front Controller)      │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 1. Controller (@Controller / @RestController) ➔ Валидация (@Valid)      │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 2. Service (@Service) ➔ Бизнес логика, @Transactional, DTO Mapping      │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 3. Repository (JpaRepository) ➔ Hibernate ORM ➔ PostgreSQL / MySQL      │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (SPRING BOOT, ASP.NET CORE, DJANGO, NESTJS)

| Концепция | ☕ Java 21+ (Spring Boot 3) | 🟣 C# 12+ (ASP.NET Core 8) | 🐍 Python 3.x (Django / FastAPI) | 🟡 JS / TS (NestJS) |
| :--- | :--- | :--- | :--- | :--- |
| **Входна точка на сървъра** | `@SpringBootApplication` + `main` | `WebApplication.CreateBuilder()` | `manage.py` / `uvicorn` | `NestFactory.create(AppModule)` |
| **Вграден уеб сървър** | **Tomcat** (по подразбиране) / Netty | **Kestrel** | Gunicorn / Uvicorn (ASGI) | Node.js HTTP / Fastify |
| **Конфигурационен файл** | `application.properties` / `.yml` | `appsettings.json` | `settings.py` / `.env` | `.env` / `ConfigModule` |
| **Управление на зависимости** | **Maven** (`pom.xml`) / **Gradle** | **NuGet** (`.csproj`) | `pip` (`requirements.txt` / Poetry) | `npm` / `pnpm` (`package.json`) |
| **HTML шаблонна система** | **Thymeleaf** | **Razor** (`.cshtml`) | **Django Templates** / Jinja2 | Handlebars / EJS |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SPRING BOOT

### 🟢 Предимства (Pros):
*   **Индустриален стандарт #1 за корпоративен Java бекенд:** Огромна екосистема, безупречна документация и готови интеграции за сигурност, бази данни, Kafka, Redis и облачни микросървиси.
*   **Production-Ready от кутията (Spring Boot Actuator):** С една зависимост получаваш готови `/actuator/health` и `/actuator/metrics` ендпойнти за мониторинг с Prometheus и Kubernetes.

### 🔴 Недостатъци (Cons):
*   **По-висока консумация на RAM и по-бавен студен старт (Cold Start):** Класическото JVM приложение заема ~250–500 MB RAM и стартира за 2–5 секунди (решава се чрез **GraalVM Native Image** в Spring Boot 3+).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (БИЗНЕС ЛОГИКА В CONTROLLER И FIELD INJECTION `@AUTOWIRED`)

### ❌ ГРЕШЕН КОД (Field Injection с `@Autowired` и бизнес логика директно в контролера):
```java
@Controller
public class BadOrderController {
    // 🚨 ГРЕШКА 1 (Field Injection): Не може да бъде final, не може да се инстанцира
    // лесно с 'new' в чист Unit тест без Reflection и крие цикли на зависимости!
    @Autowired
    private OrderRepository orderRepository;

    @PostMapping("/orders")
    public String createOrder(OrderDto dto) {
        // 🚨 ГРЕШКА 2 (Fat Controller): Сметки, бизнес проверки и директен достъп до Repository
        // вътре в контролера! Тази логика не може да се преизползва от REST API или Scheduler!
        if (dto.amount() <= 0) throw new IllegalArgumentException("Невалидна сума");
        orderRepository.save(new Order(dto.amount() * 1.20));
        return "redirect:/orders";
    }
}
```
*   **Как да се избегне:** **Никога** не ползвай Field Injection (`@Autowired` върху поле). Винаги използвай **Constructor Injection** с `private final` полета. Дръж контролера тънък (Thin Controller) — той само делегира работата на `@Service`.

### 🟢 ПРАВИЛЕН КОД (Thin Controller с Constructor Injection):
```java
@Controller
@RequestMapping("/orders")
public class OrderController {

    private final OrderService orderService;

    // Constructor Injection (не изисква @Autowired при един конструктор!)
    public OrderController(OrderService orderService) {
        this.orderService = orderService;
    }

    @PostMapping
    public String createOrder(@Valid CreateOrderDto dto) {
        orderService.placeOrder(dto);
        return "redirect:/orders";
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво всъщност прави анотацията **`@SpringBootApplication`** върху главния клас на приложението?“*
*   **Уловката:** Кандидатите отговарят *„Стартира Spring Boot“*, но интервюиращият очаква да чуе **трите конкретни анотации**, които тя обединява!
*   **Точен Senior отговор:** `@SpringBootApplication` е съставна (Meta) анотация, която комбинира точно три анотации:
    1.  **`@SpringBootConfiguration`:** Маркира класа като източник на конфигурационни `@Bean` дефиниции (специализирана `@Configuration`).
    2.  **`@EnableAutoConfiguration`:** Активира магията на Spring Boot — сканира Classpath-а (например вижда `postgresql` драйвер и `spring-data-jpa`) и автоматично конфигурира `DataSource`, `EntityManagerFactory` и `DispatcherServlet`.
    3.  **`@ComponentScan`:** Сканира текущия пакет и **всички негови подпакети** за класове с `@Component`, `@Service`, `@Repository` и `@Controller`, за да ги регистрира като Beans в IoC контейнера. *(Капан: Клас, поставен извън или над пакета на главния клас, няма да бъде сканиран!)*

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING BOOT 3) СРЕЩУ C# 12+ (ASP.NET CORE 8)

| Архитектурен детайл | ☕ Java 21+ (Spring Boot 3.x) | 🟣 C# 12+ (ASP.NET Core 8) |
| :--- | :--- | :--- |
| **Регистрация на сървиси в DI** | Автоматично чрез анотации (`@Service`, `@Repository`, `@Component`) | Ръчно в `Program.cs` (`builder.Services.AddScoped<IService, Service>()`) |
| **Default обхват (Scope) на Bean** | **`Singleton`** (1 инстанция за цялото приложение!) | Обикновено **`Scoped`** (1 инстанция на HTTP заявка) |
| **Конфигурационен механизъм** | `@Value("${app.name}")` или `@ConfigurationProperties` | `IConfiguration` или `IOptions<AppSettings>` |
| **Middleware / Interceptors** | Servlet `Filter` и Spring `HandlerInterceptor` | ASP.NET Core Middleware Pipeline (`app.Use(...)`) |
| **Фронтенд шаблон за сървърно рендиране** | **Thymeleaf** (чисти HTML5 атрибути `th:text`, `th:each`) | **Razor Pages / MVC Views** (`@Model.Name`, `@foreach`) |

---
[⬅️ Назад към Модул 04 Индекс](README.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-01. Internet Explained & HTTP Protocol](04-01-00-Internet-And-HTTP.md)
