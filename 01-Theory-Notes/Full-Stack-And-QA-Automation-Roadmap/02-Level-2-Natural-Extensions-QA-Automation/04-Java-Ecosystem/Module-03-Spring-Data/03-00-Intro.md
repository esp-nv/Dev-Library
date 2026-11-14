[⬅️ Назад към Модул 03 Индекс](README.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-01. DB Apps Introduction (JDBC)](03-01-00-JDBC-Introduction.md)

---

# 03-00. Intro: Въведение в Spring Data, Hibernate & Архитектурен Roadmap

**Модул 03:** Spring Data (Hibernate & JPA) | **Цел:** Преход към работа с релационни бази данни в Java, разбиране на слоестата архитектура (`JDBC ➔ JPA ➔ Hibernate ➔ Spring Data JPA`) и директно сравнение с C# (Entity Framework Core) и Python (Django ORM / SQLAlchemy).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурна карта на Модул 03 (Блок-схема)](#2-архитектурна- карта-на-модул-03-блок-схема)
* [3. Архитектурно сравнение: Java (Spring Data JPA) срещу C# (EF Core) и Python (Django ORM)](#3-архитектурно-сравнение-java-spring-data-jpa-срещу-c-ef-core-и-python-django-orm)
* [4. Плюсове и минуси на ORM (Hibernate & Spring Data JPA)](#4-плюсове-и-минуси-на-orm-hibernate--spring-data-jpa)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Hibernate / JPA) срещу C# 12+ (EF Core)](#7-архитектурно-сравнение-java-21-hibernate--jpa-срещу-c-12-ef-core)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Четирите етажа на сградата (От суров SQL към Spring Data):**
    1.  **JDBC:** Чист SQL и ниско ниво (`SELECT * FROM users`).
    2.  **JPA:** Спецификация и набор от анотации (`@Entity`, `@Id`) като интерфейс без изпълним код.
    3.  **Hibernate:** Имплементация, която чете JPA, генерира SQL и кешира обекти (еквивалент на **Entity Framework Core** в C#).
    4.  **Spring Data JPA:** Абстракция върху Hibernate, която автоматично генерира заявки от интерфейсни методи (напр. `findByEmail`).

---

## 2. АРХИТЕКТУРНА КАРТА НА МОДУЛ 03 (БЛОК-СХЕМА)

Преходът на данните от Java обект в паметта до базата данни (PostgreSQL / MySQL) преминава през три основни нива: JDBC драйвери, ORM ядро с JPA анотации и Spring Data слой с репозиторита и JPQL.

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA (SPRING DATA JPA) СРЕЩУ C# (EF CORE) И PYTHON (DJANGO ORM)

| ORM Концепция | ☕ Java 21+ (Hibernate / Spring Data) | 🟣 C# 12+ (Entity Framework Core) | 🐍 Python 3.x (Django ORM) |
| :--- | :--- | :--- | :--- |
| **Дефиниция на таблица/модел** | `@Entity @Table(name = "users")` | `public class User` + `DbSet<User>` | `class User(models.Model):` |
| **Unit of Work / Контекст** | `EntityManager` (Persistence Context) | `DbContext` (Change Tracker) | Няма в Django (атомарни заявки) |
| **Репозиторий слой** | `interface UserRepo extends JpaRepository` | Вграден в `DbSet<T>` | `User.objects` (Model Manager) |
| **Обектен език за заявки** | **JPQL** (`SELECT u FROM User u`) | **LINQ to Entities** | `QuerySet` API / `Q` обекти |
| **Миграции на схемата** | **Flyway** или **Liquibase** (SQL) | `dotnet ef migrations add` (C#) | `python manage.py makemigrations` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ORM (HIBERNATE & SPRING DATA JPA)

### 🟢 Предимства (Pros):
*   **Огромна продуктивност при стандартни CRUD операции:** За 80% от заявките не пишеш нито един ред SQL — Hibernate генерира правилния диалект за PostgreSQL, MySQL или Oracle автоматично.
*   **Автоматичен Dirty Checking и First-Level Cache:** В рамките на една транзакция Hibernate следи промените по заредените обекти и сам генерира оптимизиран `UPDATE` при приключване на метода.

### 🔴 Недостатъци (Cons):
*   **Висока крива на учене и скрити капани:** Ако не разбираш какво се случва под капака (`LAZY` зареждане, `Persistence Context`, `N+1` заявки), ORM може да генерира стотици излишни SQL заявки и да срине базата данни.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ВРЪЩАНЕ НА ENTITY СРЕЩУ DTO)

### ❌ ГРЕШЕН КОД (Излагане на JPA `@Entity` директно в REST контролер):
```java
@RestController
@RequestMapping("/api/users")
public class BadUserController {
    private final UserRepository userRepository;

    public BadUserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    // 🚨 ГРЕШКА 1 (Security Breach): Връща директно @Entity User!
    // Всички полета на класа (вкл. passwordHash и internalRole) изтичат в JSON отговора!
    // 🚨 ГРЕШКА 2: Ако User има @OneToMany List<Order> orders с FetchType.LAZY,
    // Jackson ще гръмне с LazyInitializationException, защото транзакцията вече е затворена!
    @GetMapping("/{id}")
    public User getUser(@PathVariable Long id) {
        return userRepository.findById(id).orElseThrow();
    }
}
```
*   **Как да се избегне:** **Никога** не връщай `@Entity` класове извън Service слоя. Мапвай ги към олекотени, неизменяеми **`record` DTO** обекти, съдържащи само полетата, нужни на клиента.

### 🟢 ПРАВИЛЕН КОД (Чист Service слой и неизменяемо `record` DTO):
```java
// 1. Олекотен DTO модел (пази само безопасните данни)
public record UserProfileDto(Long id, String username, String email) {}

// 2. Service слой, който мапва Entity -> DTO вътре в транзакцията
@Service
public class UserService {
    private final UserRepository userRepository;

    public UserService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Transactional(readOnly = true)
    public UserProfileDto getUserProfile(Long id) {
        return userRepository.findById(id)
                .map(u -> new UserProfileDto(u.getId(), u.getUsername(), u.getEmail()))
                .orElseThrow(() -> new IllegalArgumentException("Потребителят не е намерен!"));
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **JPA** и **Hibernate**? Можем ли да използваме Spring Data JPA без Hibernate?“*
*   **Уловката:** Много кандидати ги смятат за едно и също нещо, защото в Spring Boot те вървят в пакет по подразбиране.
*   **Точен Senior отговор:** 
    1.  **JPA (Jakarta Persistence API)** е **само спецификация (набор от интерфейси и анотации)** в пакета `jakarta.persistence.*` — тя няма никакъв изпълним код и не може да работи самостоятелно.
    2.  **Hibernate** е **конкретна ORM имплементация** на JPA (най-популярната в света), която съдържа реалния код за генериране на SQL, кеширане и комуникация с базата.
    3.  **Да, можем без Hibernate:** Spring Data JPA е абстракция върху JPA спецификацията — можем да заменим Hibernate с друга JPA имплементация (например **EclipseLink**), без да променяме нито един ред от нашите `Repository` интерфейси!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (HIBERNATE / JPA) СРЕЩУ C# 12+ (EF CORE)

| Архитектурен детайл | ☕ Java 21+ (Hibernate / Spring Data JPA) | 🟣 C# 12+ (Entity Framework Core) |
| :--- | :--- | :--- |
| **Зареждане на релации по подразбиране** | `@OneToOne` и `@ManyToOne` са **`EAGER`**; `@OneToMany` и `@ManyToMany` са **`LAZY`** | Всички навигационни свойства са **`LAZY`** (или `null` без `.Include()`) |
| **Заявки само за четене (без проследяване)** | `@Transactional(readOnly = true)` | `.AsNoTracking()` върху `IQueryable<T>` |
| **Миграции на базата данни** | Ръчни версионирани SQL скриптове с **Flyway** или **Liquibase** (`ddl-auto=validate`) | Автоматично генериран C# код с `dotnet ef migrations add` |
| **Конфигурация на таблици и колони** | JPA анотации (`@Column`, `@JoinColumn`) или XML | Data Annotations или **Fluent API** (`OnModelCreating`) |
| **Динамично филтриране** | `JpaSpecificationExecutor<T>` (Criteria API) или **QueryDSL** | Нативен `IQueryable<T>` с верижни `.Where(...)` методи |
| **Second-Level Cache (Споделен кеш)** | Вградена поддръжка за L2 Cache (**Ehcache**, **Caffeine**, **Redis**) | Няма вграден L2 Cache в EF Core (ползва се ръчно `IMemoryCache` / Redis) |

---
[⬅️ Назад към Модул 03 Индекс](README.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-01. DB Apps Introduction (JDBC)](03-01-00-JDBC-Introduction.md)
