[⬅️ 03-03. Hibernate Code First & Entity Relations](03-03-00-Entity-Relations.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-04-01. Syntax From Scratch](03-04-01-Syntax-From-Scratch.md)

---

# 03-04-00. Spring Data Intro & Repositories (Теория, `JpaRepository`, Derived Queries & Капани)

**Модул 03:** Spring Data | **Лекционна тема 04:** Как Spring Data JPA генерира автоматични имплементации на интерфейси по време на стартиране, йерархията `Repository ➔ CrudRepository ➔ ListCrudRepository / PagingAndSortingRepository ➔ JpaRepository`, Derived Query Methods (генериране на SQL от името на метода), как работи `SimpleJpaRepository.save()` под капака и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Spring Data JPA Proxy и `save()` логика (Блок-схема)](#2-архитектура-на-spring-data-jpa-proxy-и-save-логика-блок-схема)
* [3. Сравнителна карта в 4 езика (Java Spring Data, C# EF Core, Python Django ORM, JS Prisma)](#3-сравнителна-карта-в-4-езика-java-spring-data-c-ef-core-python-django-orm-js-prisma)
* [4. Плюсове и минуси на Derived Query Methods](#4-плюсове-и-минуси-на-derived-query-methods)
* [5. Кодов одит: Правилен срещу Грешен код (`deleteAll()` срещу `deleteAllInBatch()` и чудовищни имена на методи)](#5-кодов-одит-правилен-срещу-грешен-код-deleteall-срещу-deleteallinbatch-и-чудовищни-имена-на-методи)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Spring Data JPA) срещу C# 12+ (EF Core)](#7-архитектурно-сравнение-java-21-spring-data-jpa-срещу-c-12-ef-core)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Никога не пишеш клас `UserRepositoryImpl` за стандартни заявки:** В Spring Data JPA ти дефинираш само **интерфейс** (`public interface UserRepository extends JpaRepository<User, Long>`). При стартиране на приложението Spring използва **JDK Dynamic Proxy**, за да създаде скрит клас `SimpleJpaRepository`, който ти дава наготово над 25 метода (`save`, `findById`, `findAll`, `deleteById`, `count`, `existsById`).
*   **Анатомия на Derived Query Method (Магията на имената):**
    *   `findByEmailAndIsActiveTrue(String email)` се разчленява от Spring на три части:
    *   **Subject (Какво прави):** `findBy` (или `countBy`, `existsBy`, `deleteBy`).
    *   **Predicate (Условията):** `Email` (`= ?1`) + `And` + `IsActiveTrue` (`= true`).
    *   **Генериран SQL:** `SELECT * FROM users WHERE email = ? AND is_active = true`.

---

## 2. АРХИТЕКТУРА НА SPRING DATA JPA PROXY И `SAVE()` ЛОГИКА (БЛОК-СХЕМА)

Как методът `repository.save(entity)` решава дали да направи `INSERT` (нов запис) или `UPDATE` (промяна на съществуващ)?

```text
repository.save(entity)
         │
         ▼ Проверява дали entity е нов (entity.getId() == null)
   ┌─────┴─────┐
   ▼ ДА (null) ▼ НЕ (има ID, напр. 42L)
┌──────────────────────┐      ┌──────────────────────────────────────────────┐
│ entityManager        │      │ entityManager.merge(entity)                  │
│   .persist(entity)   │      │ 1. Пуска SELECT за ID=42 (ако не е в L1 кеш) │
│                      │      │ 2. Копира новите стойности                   │
│ ➔ Генерира INSERT!   │      │ ➔ Генерира UPDATE при commit()!              │
└──────────────────────┘      └──────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA SPRING DATA, C# EF CORE, PYTHON DJANGO ORM, JS PRISMA)

| Операция | ☕ Java 21+ (`JpaRepository`) | 🟣 C# 12+ (`DbContext` / `DbSet`) | 🐍 Python 3.x (Django ORM) | 🟡 JS / TS (Prisma ORM) |
| :--- | :--- | :--- | :--- | :--- |
| **Запис на нов обект** | `repo.save(user)` | `db.Users.Add(user); await db.SaveChangesAsync();` | `user.save()` / `User.objects.create(...)` | `await prisma.user.create({ data })` |
| **Търсене по ID** | `repo.findById(1L)` (`Optional<User>`) | `await db.Users.FindAsync(1)` (`User?`) | `User.objects.filter(id=1).first()` | `await prisma.user.findUnique(...)` |
| **Проверка за съществуване** | `repo.existsByEmail(email)` (`boolean`) | `await db.Users.AnyAsync(u => u.Email == e)` | `User.objects.filter(email=e).exists()` | `await prisma.user.count(...) > 0` |
| **Брой записи по условие** | `repo.countByRole(Role.ADMIN)` | `await db.Users.CountAsync(u => ...)` | `User.objects.filter(...).count()` | `await prisma.user.count({ where })` |
| **Изтриване по ID** | `repo.deleteById(1L)` | `db.Users.Remove(user); await db.SaveChangesAsync();` | `User.objects.filter(id=1).delete()` | `await prisma.user.delete({ where })` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА DERIVED QUERY METHODS

### 🟢 Предимства (Pros):
*   **Проверка още при стартиране (Fail-Fast):** Ако сбъркаш името на поле в метода (напр. `findByEmial`), Spring Boot **няма да стартира** и веднага ще ти покаже точната грешка в конзолата, вместо да чакаш потребител да натисне бутона в продукция.
*   **Нулев шаблон за прости заявки:** Търсене по 1–2 критерия (`findByUsername`, `existsByEmail`, `findTop5ByOrderByCreatedAtDesc`) се пише с един ред в интерфейса.

### 🔴 Недостатъци (Cons):
*   **Нечетими „чудовищни“ имена при сложни филтри:** Методи от вида `findDistinctByDepartmentNameAndSalaryGreaterThanAndActiveTrueOrderByLastNameAsc(...)` са кошмар за четене и поддръжка — при над 2–3 условия **винаги** преминавай към `@Query` (JPQL).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`DELETEALL()` СРЕЩУ `DELETEALLINBATCH()` И ЧУДОВИЩНИ ИМЕНА)

### ❌ ГРЕШЕН КОД (Използване на `deleteAll()` за изтриване на 10 000 реда и ненужно `@Repository`):
```java
// 🚨 ГРЕШКА 1: @Repository анотацията е ИЗЛИШНА върху интерфейси, наследяващи JpaRepository
// (Spring Data ги сканира и регистрира автоматично!).
@Repository
public interface OrderRepository extends JpaRepository<Order, Long> {
    // 🚨 ГРЕШКА 2: Твърде дълго Derived име -> нечетимо! Използвай @Query!
    List<Order> findAllByCustomerEmailAndStatusAndTotalAmountGreaterThanOrderByCreatedAtDesc(
        String email, OrderStatus status, BigDecimal amount);
}

// 🚨 ГРЕШКА 3 (Огромен капан за производителността!):
// Ако искаш да изтриеш 10 000 записа и извикаш orderRepository.deleteAll(),
// Spring Data първо прави SELECT * FROM orders (зарежда всичките 10 000 обекта в RAM),
// и после пуска 10 000 ОТДЕЛНИ DELETE заявки (1 по 1)!
orderRepository.deleteAll();
```
*   **Как да се избегне:** За масово изтриване на редове без каскадни Java събития използвай **`deleteAllInBatch()`**, който изпълнява **една единствена SQL заявка `DELETE FROM orders` за 2 милисекунди**!

### 🟢 ПРАВИЛЕН КОД (Чист, четим `JpaRepository` интерфейс):
```java
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;
import java.util.Optional;

public interface UserRepository extends JpaRepository<User, Long> {

    Optional<User> findByEmail(String email);

    boolean existsByUsername(String username);

    List<User> findTop10ByIsActiveTrueOrderByCreatedAtDesc();
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **`repository.findById(id)`** и **`repository.getReferenceById(id)`** (старото `getOne(id)`) в Spring Data JPA?“*
*   **Уловката:** И двата метода приемат `ID` и връщат запис, но поведението им към базата данни е коренно различно!
*   **Точен Senior отговор:** 
    1.  **`findById(id)`** връща `Optional<T>` и **веднага изпълнява `SELECT` заявка** към базата данни, за да зареди реалния обект (или `Optional.empty()`, ако не съществува).
    2.  **`getReferenceById(id)`** връща директно тип `T`, но **НЕ удря базата данни изобщо**! Той създава празен **Hibernate Proxy обект** само с попълнено `id`. Това е изключително полезно, когато искаш да закачиш външен ключ без излишен `SELECT` (напр. `Book book = new Book("Dune"); book.setAuthor(authorRepo.getReferenceById(5L)); bookRepo.save(book);` — спестяваш 1 `SELECT` заявка!). Ако обаче опиташ да прочетеш `author.getName()` извън транзакцията или ако `ID=5` не съществува, ще хвърли `LazyInitializationException` или `EntityNotFoundException`.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (SPRING DATA JPA) СРЕЩУ C# 12+ (EF CORE)

| Архитектурен детайл | ☕ Java 21+ (Spring Data JPA) | 🟣 C# 12+ (Entity Framework Core) |
| :--- | :--- | :--- |
| **Repository Pattern** | **Вграден в Spring Data** (`JpaRepository<T, ID>` — не пишеш имплементация) | `DbSet<T>` вече е Repository + Unit of Work (често се обвива в ръчен `IRepository<T>`) |
| **Запазване на промени** | `repo.save(entity)` (в транзакция Dirty Checking сам пуска `UPDATE`) | `await dbContext.SaveChangesAsync()` (задължително извикване) |
| **Генериране на заявки от имена** | **Да** (`findByEmailAndAgeGreaterThan(...)`) | **Не** (ползва се C# LINQ: `db.Users.Where(u => u.Email == e && u.Age > a)`) |
| **Закачане на FK без `SELECT`** | `authorRepo.getReferenceById(5L)` | `new Book { Title = "Dune", AuthorId = 5 }` (директно чрез Foreign Key свойството!) |
| **Масово изтриване (Bulk Delete)** | `repo.deleteAllInBatch()` или `@Modifying @Query("DELETE ...")` | `await db.Orders.Where(...).ExecuteDeleteAsync()` (EF Core 7+) |

---
[⬅️ 03-03. Hibernate Code First & Entity Relations](03-03-00-Entity-Relations.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-04-01. Syntax From Scratch](03-04-01-Syntax-From-Scratch.md)
