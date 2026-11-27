[⬅️ 03-01. DB Apps Introduction (JDBC)](03-01-00-JDBC-Introduction.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-02-01. Syntax From Scratch](03-02-01-Syntax-From-Scratch.md)

---

# 03-02-00. Introduction to Hibernate & Entity Lifecycle (Теория, Persistence Context & Капани)

**Модул 03:** Spring Data | **Лекционна тема 02:** Основи на Hibernate ORM и Jakarta Persistence API (`jakarta.persistence.*`), анотации за мапинг (`@Entity`, `@Table`, `@Id`, `@GeneratedValue`, `@Column`), четирите състояния на едно Entity (**Transient**, **Managed / Persistent**, **Detached**, **Removed**), механизмите **Dirty Checking** и First-Level Cache, и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Жизнен цикъл на едно Entity (Entity Lifecycle Блок-схема)](#2-жизнен-цикъл-на-едно-entity-entity-lifecycle-блок-схема)
* [3. Сравнителна карта в 4 езика (Java Hibernate, C# EF Core, Python Django ORM, JS Prisma)](#3-сравнителна-карта-в-4-езика-java-hibernate-c-ef-core-python-django-orm-js-prisma)
* [4. Плюсове и минуси на Persistence Context и Dirty Checking](#4-плюсове-и-минуси-на-persistence-context-и-dirty-checking)
* [5. Кодов одит: Правилен срещу Грешен код (Lombok `@Data` върху `@Entity` и липсващ празен конструктор)](#5-кодов-одит-правилен-срещу-грешен-код-lombok-data-върху-entity-и-липсващ-празен-конструктор)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Hibernate) срещу C# 12+ (EF Core)](#7-архитектурно-сравнение-java-21-hibernate-срещу-c-12-ef-core)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`Persistence Context` (`EntityManager`) = Служебният бележник на Hibernate по време на транзакция:**
    1.  **Transient (Нов обект в паметта):** Създала си `User u = new User("Ivan");` с `new`. Бележникът на Hibernate изобщо не знае за него и в базата няма такъв ред.
    2.  **Managed / Persistent (Вписан в бележника):** Извикваш `em.persist(u)` или го зареждаш от базата с `em.find(User.class, 1L)`. От този момент нататък Hibernate пази копие (Snapshot) на обекта в бележника си. Ако промениш `u.setEmail("new@mail.bg")`, **не е нужно да викаш `save()` или `update()`** — при края на транзакцията Hibernate сравнява обекта със снимката и сам пуска `UPDATE` (**Dirty Checking**)!
    3.  **Detached (Откъснат от бележника):** Транзакцията приключва и `EntityManager` се затваря. Обектът още съществува в RAM паметта на Java, но Hibernate вече не го следи. Ако го промениш сега, базата няма да разбере (освен ако не го върнеш обратно с `em.merge(u)`).
    4.  **Removed (Зачертан за изтриване):** Извикваш `em.remove(u)`. Обектът остава в паметта до края на транзакцията, но при `commit()` Hibernate изпълнява `DELETE FROM users WHERE id = ?`.

---

## 2. ЖИЗНЕН ЦИКЪЛ НА ЕДНО ENTITY (ENTITY LIFECYCLE БЛОК-СХЕМА)

```text
   [ new User() ]
         │
         ▼
┌─────────────────┐   em.persist(u)    ┌────────────────────────────────────────┐
│   TRANSIENT     │ ─────────────────► │ MANAGED / PERSISTENT                   │
│ (Само в Java    │                    │ • Живее в First-Level Cache (L1)       │
│  Heap паметта)  │ ◄──────────────────│ • Dirty Checking следи за промени!     │
└─────────────────┘   em.remove(u)     └───────┬────────────────────────▲───────┘
         │                 │                   │                        │
         │                 ▼                   │ Транзакцията затваря   │ em.merge(u)
         │        ┌─────────────────┐          │ или em.detach(u)       │
         │        │     REMOVED     │          ▼                        │
         │        │ (Ще бъде изтрит │  ┌────────────────────────────────┴───────┐
         │        │  при commit())  │  │ DETACHED (Има ID в БД, но не се следи) │
         │        └─────────────────┘  └─────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA HIBERNATE, C# EF CORE, PYTHON DJANGO ORM, JS PRISMA)

| Концепция | ☕ Java 21+ (Hibernate / JPA) | 🟣 C# 12+ (Entity Framework Core) | 🐍 Python 3.x (Django ORM) | 🟡 JS / TS (Prisma ORM) |
| :--- | :--- | :--- | :--- | :--- |
| **Дефиниция на модел** | `@Entity @Table(name = "users")` | `public class User` + `DbSet<User>` | `class User(models.Model):` | `model User { ... }` в `schema.prisma` |
| **Първичен ключ** | `@Id @GeneratedValue(...)` | `public int Id { get; set; }` | Автоматично `id = BigAutoField` | `id Int @id @default(autoincrement())` |
| **Колона с ограничения** | `@Column(name = "email", unique = true)` | `[Column("email")]` / Fluent API | `models.EmailField(unique=True)` | `email String @unique` |
| **Поле само в паметта** | `@Transient` | `[NotMapped]` | Обикновен Python атрибут | Не се описва в модела |
| **Следене на промени** | **Dirty Checking** в `Persistence Context` | **Change Tracker** в `DbContext` | **Няма** (изисква изрично `.save()`) | **Няма** (изисква `prisma.user.update()`) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА PERSISTENCE CONTEXT И DIRTY CHECKING

### 🟢 Предимства (Pros):
*   **First-Level Cache (L1 Cache) по подразбиране:** Ако в рамките на една и съща транзакция извикаш `userRepo.findById(5L)` десет пъти на различни места, Hibernate ще удари базата данни **само 1 път**, а останалите 9 пъти ще върне обекта директно от RAM паметта за 0 милисекунди!
*   **Write-Behind (Отложено писане):** Hibernate трупа промените в паметта и изпраща SQL заявките наведнъж при `flush()` или `commit()`, което спестява излишни мрежови обръщения към сървъра на базата.

### 🔴 Недостатъци (Cons):
*   **Опасност от `OutOfMemoryError` при масова обработка (Batch Jobs):** Тъй като L1 кешът държи референция към **всеки** зареден или записан обект до края на транзакцията, ако заредиш 500 000 реда в един цикъл без периодично да викаш `entityManager.flush()` и `entityManager.clear()`, паметта на JVM ще прелее.

---
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (LOMBOK `@DATA` ВЪРХУ `@ENTITY` И ЛИПСВАЩ ПРАЗЕН КОНСТРУКТОР)

### ❌ ГРЕШЕН КОД (Използване на Lombok `@Data` върху JPA `@Entity` и липса на празен конструктор):
```java
import jakarta.persistence.*;
import lombok.Data;

@Entity
@Table(name = "products")
// 🚨 ГРЕШКА 1: @Data генерира equals(), hashCode() и toString() върху ВСИЧКИ полета!
// 1) При @OneToMany/@ManyToOne това пуска скрити LAZY заявки или хвърля LazyInitializationException!
// 2) При двупосочни релации предизвиква безкрайна рекурсия (StackOverflowError)!
// 3) hashCode() се променя след запис (когато id мине от null на 1), което чупи HashSet!
@Data
public class BadProduct {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    // 🚨 ГРЕШКА 2: Ако напишеш конструктор с параметри и забравиш празния конструктор,
    // Hibernate ще гръмне с InstantiationException, защото създава обектите чрез Reflection!
    public BadProduct(String name) {
        this.name = name;
    }
}
```
*   **Как да се избегне:** Върху JPA `@Entity` класове **никога не слагай `@Data`**. Използвай **`@Getter` и `@Setter`**, остави **`protected No-Args Constructor`** (`protected Product() {}`) за Hibernate, а `equals()` и `hashCode()` базирай единствено на бизнес ключ (напр. `UUID` / `sku`) или проверка по `id != null`.

### 🟢 ПРАВИЛЕН КОД (Чист, безопасен JPA `@Entity` клас по индустриален стандарт):
```java
import jakarta.persistence.*;
import java.math.BigDecimal;
import java.util.Objects;

@Entity
@Table(name = "products")
public class Product {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name", nullable = false, length = 120)
    private String name;

    @Column(name = "price", nullable = false, precision = 10, scale = 2)
    private BigDecimal price;

    // Задължителен конструктор без параметри за Hibernate (protected пази класа от празен new отвън)
    protected Product() {}

    public Product(String name, BigDecimal price) {
        this.name = name;
        this.price = price;
    }

    public Long getId() { return id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public BigDecimal getPrice() { return price; }
    public void setPrice(BigDecimal price) { this.price = price; }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Product product)) return false;
        return id != null && Objects.equals(id, product.id);
    }

    @Override
    public int hashCode() {
        return getClass().hashCode(); // Константен хеш код -> безопасен за HashSet преди и след persist()!
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **`GenerationType.IDENTITY`** и **`GenerationType.SEQUENCE`**, и защо `IDENTITY` изключва **JDBC Batch Inserts** в Hibernate?“*
*   **Уловката:** Това е класически Senior въпрос по производителност на бази данни в Java!
*   **Точен Senior отговор:** 
    1.  **`GenerationType.IDENTITY`** разчита на колона `AUTO_INCREMENT` / `SERIAL` в базата данни. Базата генерира ID-то **чак в момента, в който изпълни `INSERT` заявката**. Тъй като Hibernate трябва веднага да знае ID-то на новия обект, за да го вкара в `Persistence Context` (L1 Cache), той е **принуден да изпрати всеки `INSERT` веднага поотделно**, което **напълно изключва JDBC Batch Inserts** при запис на 1000 реда!
    2.  **`GenerationType.SEQUENCE`** (стандарт в PostgreSQL и Oracle) използва отделен обект `SEQUENCE` в базата данни. Hibernate изтегля наведнъж блок от 50 поредни ID-та (`allocationSize = 50`) с една бърза заявка, раздава ги в RAM паметта на 50-те нови обекта и изпраща всичките 50 `INSERT` заявки наведнъж в **един единствен JDBC Batch**!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (HIBERNATE) СРЕЩУ C# 12+ (EF CORE)

| Архитектурен детайл | ☕ Java 21+ (Hibernate / JPA) | 🟣 C# 12+ (Entity Framework Core) |
| :--- | :--- | :--- |
| **Пакет за анотации** | `jakarta.persistence.*` (от Spring Boot 3+; преди беше `javax.persistence.*`) | `System.ComponentModel.DataAnnotations` |
| **Тип на ID полето** | Wrapper клас **`Long`** или `UUID` (`null` означава нов запис) | Примитив **`int` / `long`** или `Guid` (`0` или `Guid.Empty` означава нов запис) |
| **Кога се изпълнява `INSERT` при `IDENTITY`** | **Веднага при `persist()` / `save()`** (преди `commit`), за да се вземе ID | **Чак при `await context.SaveChangesAsync()`** |
| **Откачане на обект от кеша** | `entityManager.detach(entity)` или `entityManager.clear()` | `context.Entry(entity).State = EntityState.Detached` или `context.ChangeTracker.Clear()` |
| **Запис на Enum в колона** | `@Enumerated(EnumType.STRING)` (иначе по подразбиране пази число `0, 1, 2`!) | По подразбиране пази `int`, или `.HasConversion<string>()` в Fluent API |
| **Автоматични дати (`CreatedAt` / `UpdatedAt`)** | `@CreationTimestamp` / `@UpdateTimestamp` или Spring Data `@CreatedDate` | Пренаписване на `SaveChangesAsync()` или Interceptors |

---
[⬅️ 03-01. DB Apps Introduction (JDBC)](03-01-00-JDBC-Introduction.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-02-01. Syntax From Scratch](03-02-01-Syntax-From-Scratch.md)
