[⬅️ 03-04. Spring Data Intro & Repositories](03-04-00-Spring-Data-Repositories.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-05-01. Syntax From Scratch](03-05-01-Syntax-From-Scratch.md)

---

# 03-05-00. Spring Data Advanced Querying (Теория, `JPQL`, `@Query`, Projections & N+1 Капани)

**Модул 03:** Spring Data | **Лекционна тема 05:** Писане на потребителски заявки с **JPQL (Jakarta Persistence Query Language)** и нативен SQL (`nativeQuery = true`), масови промени с `@Modifying`, **N+1 Query Problem** и как се решава чрез `JOIN FETCH` и `@EntityGraph`, интерфейсни и DTO проекции (`Projections`), странициране (`Pageable`, `Page<T>` срещу `Slice<T>`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на N+1 проблема и решението с `JOIN FETCH` (Блок-схема)](#2-архитектура-на-n1-проблема-и-решението-с-join-fetch-блок-схема)
* [3. Сравнителна карта в 4 езика (Java Spring Data, C# EF Core, Python Django ORM, JS Prisma)](#3-сравнителна-карта-в-4-езика-java-spring-data-c-ef-core-python-django-orm-js-prisma)
* [4. Плюсове и минуси на JPQL срещу Native SQL и Projections](#4-плюсове-и-минуси-на-jpql-срещу-native-sql-и-projections)
* [5. Кодов одит: Правилен срещу Грешен код (Забравен `clearAutomatically = true` при `@Modifying` и `JOIN FETCH` с `Pageable`)](#5-кодов-одит-правилен-срещу-грешен-код-забравен-clearautomatically--true-при-modifying-и-join-fetch-с-pageable)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Data JPA) срещу C# 12+ (EF Core)](#7-архитектурно-сравнение-java-21-spring-data-jpa-срещу-c-12-ef-core)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **JPQL гледа Java класовете и полетата, а НЕ таблиците и колоните в базата:**
    *   В нативен SQL пишеш: `SELECT first_name FROM app_users WHERE is_active = true` (имена от PostgreSQL).
    *   В **JPQL** пишеш: `SELECT u.firstName FROM User u WHERE u.isActive = true` (**името на Java класа `User` и имената на Java полетата `firstName`, `isActive`** — главните и малките букви в името на класа са задължителни!).
*   **`Page<T>` срещу `Slice<T>` (Книга срещу Безкраен скрол):**
    *   **`Page<T>`:** Изпълнява **ДВЕ** заявки към базата — една за данните (`LIMIT/OFFSET`) и втора `SELECT COUNT(*)` за общия брой страници (идеално за таблици с номера на страници `1, 2, 3 ... 50`).
    *   **`Slice<T>`:** Изпълнява **САМО ЕДНА** заявка (`LIMIT size + 1`), без скъпото `COUNT(*)`, и просто казва `hasNext()` (идеално за бутон *„Зареди още“ / Infinite Scroll* в мобилни приложения и социални мрежи).

---

## 2. АРХИТЕКТУРА НА N+1 ПРОБЛЕМА И РЕШЕНИЕТО С `JOIN FETCH` (БЛОК-СХЕМА)

```text
❌ ПРОБЛЕМЪТ N+1 (1 заявка за списъка + N отделни заявки в цикъл):
   1. List<Author> authors = authorRepo.findAll();   ➔ SELECT * FROM authors; (1 заявка, връща 100 автора)
   2. for (Author a : authors) a.getBooks().size();  ➔ 100 отделни заявки: SELECT * FROM books WHERE author_id = ?
      ОБЩО: 101 SQL заявки за един екран! 💥

✅ РЕШЕНИЕТО С JOIN FETCH (Всичко в 1 единствена SQL заявка):
   @Query("SELECT DISTINCT a FROM Author a LEFT JOIN FETCH a.books")
   ➔ Изпълнява 1 заявка: SELECT a.*, b.* FROM authors a LEFT JOIN books b ON a.id = b.author_id;
      ОБЩО: 1 SQL заявка! ⚡
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA SPRING DATA, C# EF CORE, PYTHON DJANGO ORM, JS PRISMA)

| Операция | ☕ Java 21+ (Spring Data JPA) | 🟣 C# 12+ (EF Core) | 🐍 Python 3.x (Django ORM) | 🟡 JS / TS (Prisma ORM) |
| :--- | :--- | :--- | :--- | :--- |
| **Потребителска заявка** | `@Query("SELECT u FROM User u WHERE ...")` | `db.Users.FromSqlInterpolated(...)` | `User.objects.raw("SELECT ...")` | `prisma.$queryRaw` |
| **Решаване на N+1 (1:N)** | `JOIN FETCH` или `@EntityGraph` | `.Include(a => a.Books)` | `.prefetch_related("books")` | `include: { books: true }` |
| **Проекция (само 2 колони)** | `SELECT new UserDto(u.id, u.email)` | `.Select(u => new UserDto(...))` | `.values("id", "email")` | `select: { id: true, email: true }` |
| **Странициране (Pagination)** | `Pageable` (`PageRequest.of(0, 20)`) | `.Skip(0).Take(20)` | `Paginator(qs, 20)` | `skip: 0, take: 20` |
| **Масов `UPDATE` / `DELETE`** | `@Modifying @Query("UPDATE ...")` | `.ExecuteUpdateAsync(...)` | `User.objects.filter(...).update(...)` | `prisma.user.updateMany(...)` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА JPQL СРЕЩУ NATIVE SQL И PROJECTIONS

### 🟢 Предимства на JPQL и Projections (Pros):
*   **Database Independence (Независимост от диалекта):** JPQL се превежда автоматично към PostgreSQL, MySQL, Oracle или H2 (чудесно при пускане на тестове върху in-memory H2 база).
*   **Драстично по-малко RAM чрез Projections:** Когато заредиш цяло `@Entity`, Hibernate пази копие на всичките му 30 колони в L1 кеша за Dirty Checking. При **DTO / Interface Projection** Hibernate чете **само 2-те поискани колони** и не ги слага в кеша!

### 🔴 Недостатъци (Cons):
*   **Ограничения на JPQL при сложен SQL:** JPQL не поддържа специфични конструкции като PostgreSQL `JSONB` оператори, Common Table Expressions (`WITH ...`), Window Functions (`ROW_NUMBER() OVER(...)`) или `UPSERT` (`ON CONFLICT`) — за тях се използва **`@Query(value = "...", nativeQuery = true)`**.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ЗАБРАВЕН `CLEARAUTOMATICALLY` ПРИ `@MODIFYING` И `JOIN FETCH` С `PAGEABLE`)

### ❌ ГРЕШЕН КОД (Стари данни в L1 кеша след `@Modifying` и `HHH90003004` грешка с `JOIN FETCH` + `Pageable`):
```java
public interface BadOrderRepository extends JpaRepository<Order, Long> {

    // 🚨 ГРЕШКА 1: Липсва clearAutomatically = true!
    // Ако преди това в същата транзакция си заредила Order #1 со статус PENDING,
    // тази заявка ще обнови базата на PAID, но L1 кешът в паметта ще продължи да връща PENDING!
    @Modifying
    @Query("UPDATE Order o SET o.status = 'PAID' WHERE o.id = :id")
    void markAsPaid(@Param("id") Long id);

    // 🚨 ГРЕШКА 2 (Класически Hibernate капан!):
    // Комбиниране на JOIN FETCH върху колекция (@OneToMany) с Pageable!
    // Hibernate изписва предупреждение: "HHH90003004: firstResult/maxResults specified with collection fetch; applying in memory!"
    // Изтегля ЦЯЛАТА таблица от 1 милион реда в RAM паметта и реже страницата в Java!
    @Query("SELECT o FROM Order o JOIN FETCH o.items")
    Page<Order> findAllWithItems(Pageable pageable);
}
```
*   **Как да се избегне:** При `@Modifying` **винаги** слагай **`@Modifying(clearAutomatically = true, flushAutomatically = true)`**. Никога не комбинирай `JOIN FETCH` върху колекция с `Pageable` — използвай **`@EntityGraph`** или двустъпково зареждане (първо `Page<Long>` с ID-тата, после `WHERE o.id IN :ids`).

### 🟢 ПРАВИЛЕН КОД (Безопасен `@Modifying` и `@EntityGraph` без N+1):
```java
import org.springframework.data.jpa.repository.*;
import org.springframework.data.repository.query.Param;
import java.util.List;

public interface OrderRepository extends JpaRepository<Order, Long> {

    // 1. Безопасен масов UPDATE, който изчиства остарелите обекти от Persistence Context
    @Modifying(clearAutomatically = true, flushAutomatically = true)
    @Query("UPDATE Order o SET o.status = :status WHERE o.id = :id")
    int updateOrderStatus(@Param("id") Long id, @Param("status") OrderStatus status);

    // 2. Чист начин за зареждане на релации без писане на дълъг JPQL JOIN FETCH
    @EntityGraph(attributePaths = {"customer", "items"})
    List<Order> findByStatus(OrderStatus status);
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **Interface-based Projection** (Closed vs Open Projection) и **Class-based (DTO Record) Projection** в Spring Data JPA?“*
*   **Уловката:** Това е любим въпрос на Senior интервюиращи за оптимизация на Spring Data!
*   **Точен Senior отговор:** 
    1.  **Closed Interface Projection:** Дефинираш интерфейс само с гетъри на нужните колони (напр. `interface UserSummary { String getUsername(); String getEmail(); }`). Spring Data генерира Proxy и SQL-ът избира **само тези 2 колони** (`SELECT u.username, u.email FROM users u`).
    2.  **Open Interface Projection (Капан!):** Ако в интерфейса добавиш метод с `@Value("#{target.firstName + ' ' + target.lastName}")` (SpEL израз), Spring губи възможността да оптимизира SQL-а и изтегля **всички колони на цялото Entity (`SELECT *`)** в паметта!
    3.  **Class-based (Record) Projection:** Използваш конструктор директно в JPQL: `SELECT new com.app.dto.UserSummaryDto(u.username, u.email) FROM User u` — 100% типово безопасно, оптимизира SQL-а до 2 колони и връща чист Java `record` без Proxy обвивки.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING DATA JPA) СРЕЩУ C# 12+ (EF CORE)

| Архитектурен детайл | ☕ Java 21+ (Spring Data JPA) | 🟣 C# 12+ (Entity Framework Core) |
| :--- | :--- | :--- |
| **Заявки в код с пълна типова проверка** | **QueryDSL** или JPA Criteria API (JPQL е низ в `@Query`) | **LINQ** (`db.Users.Where(...).Select(...)` — нативно в C# компилатора) |
| **Решаване на Cartesian Explosion при 2+ колекции** | Две отделни заявки (за да се избегне `MultipleBagFetchException`) | `.AsSplitQuery()` (EF Core 5+ автоматично разделя JOIN-а на отделни SELECT-и) |
| **Масов `UPDATE` / `DELETE` без зареждане в RAM** | `@Modifying(clearAutomatically = true) @Query(...)` | `await db.Users.Where(...).ExecuteUpdateAsync(...)` (EF Core 7+) |
| **Глобален филтър (напр. Soft Delete `deleted = false`)** | Hibernate `@SQLRestriction("deleted = false")` (от Hibernate 6.3+) | `modelBuilder.Entity<User>().HasQueryFilter(u => !u.Deleted)` |
| **Проекции (Partial Select)** | Interface Projections или ``SELECT new RecordDto(...)`` | Анонимни типове .``Select(x => new { x.Id, x.Name })`` или DTO класове |

---

[⬅️ 03-04. Spring Data Intro & Repositories](03-04-00-Spring-Data-Repositories.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-05-01. Syntax From Scratch](03-05-01-Syntax-From-Scratch.md)