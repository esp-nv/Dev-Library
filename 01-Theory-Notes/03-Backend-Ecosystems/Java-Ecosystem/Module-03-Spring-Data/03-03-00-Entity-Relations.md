[⬅️ 03-02. Introduction to Hibernate](03-02-00-Hibernate-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-03-01. Syntax From Scratch](03-03-01-Syntax-From-Scratch.md)

---

# 03-03-00. Hibernate Code First & Entity Relations (Теория, `LAZY` vs `EAGER` & Капани)

**Модул 03:** Spring Data | **Лекционна тема 03:** Връзки между таблици в JPA/Hibernate (`@OneToOne`, `@OneToMany`, `@ManyToOne`, `@ManyToMany`), собственик на връзката (`Owning Side` с `@JoinColumn` срещу `Inverse Side` с `mappedBy`), стратегии на зареждане (`FetchType.LAZY` срещу `EAGER`), каскадни операции (`CascadeType`, `orphanRemoval = true`), стратегии за наследяване (`SINGLE_TABLE`, `JOINED`, `TABLE_PER_CLASS`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Owning Side (`@JoinColumn`) срещу Inverse Side (`mappedBy`)](#2-архитектура-на-owning-side-joincolumn-срещу-inverse-side-mappedby)
* [3. Сравнителна карта в 4 езика (Java Hibernate, C# EF Core, Python Django ORM, JS Prisma)](#3-сравнителна-карта-в-4-езика-java-hibernate-c-ef-core-python-django-orm-js-prisma)
* [4. Плюсове и минуси на `FetchType.LAZY` срещу `FetchType.EAGER`](#4-плюсове-и-минуси-на-fetchtypelazy-срещу-fetchtypeeager)
* [5. Кодов одит: Правилен срещу Грешен код (Забравен `mappedBy` и `LazyInitializationException`)](#5-кодов-одит-правилен-срещу-грешен-код-забравен-mappedby-и-lazyinitializationexception)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Hibernate) срещу C# 12+ (EF Core)](#7-архитектурно-сравнение-java-21-hibernate-срещу-c-12-ef-core)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Кой държи външния ключ (Foreign Key) в базата данни? (`Owning Side`):**
    *   В една връзка **Едно-към-Много (1:N)** — например `Author` (1) и `Book` (N) — колоната `author_id` в релационната база **винаги се намира в таблицата от страната „Много“ (`books`)**!
    *   Затова страната **`@ManyToOne`** е **Owning Side (Собственикът)** и носи анотацията **`@JoinColumn(name = "author_id")`**.
    *   Страната **`@OneToMany`** е само огледало (**Inverse Side**) и **задължително** носи параметъра **`mappedBy = "author"`**, за да каже на Hibernate: *„Не създавай трета свързваща таблица! Връзката вече е описана от полето `author` в класа `Book`!“*
*   **Златно правило за зареждане (FetchType):** Направи **АБСОЛЮТНО ВСИЧКИ релации `FetchType.LAZY`** (включително `@ManyToOne(fetch = FetchType.LAZY)` и `@OneToOne(fetch = FetchType.LAZY)`, които по подразбиране са `EAGER`!).

---

## 2. АРХИТЕКТУРА НА OWNING SIDE (`@JOINCOLUMN`) СРЕЩУ INVERSE SIDE (`MAPPEDBY`)

```text
JAVA ОБЕКТИ (Двупосочна връзка)                РЕЛАЦИОННА БАЗА ДАННИ (PostgreSQL)
┌──────────────────────────────────┐           ┌──────────────────────────────────┐
│ class Author (Inverse Side)      │           │ Таблица: authors                 │
│ • @OneToMany(mappedBy = "author")│           │ • id (PK) | name                 │
│   List<Book> books;              │           └─────────────────▲────────────────┘
└────────────────┬─────────────────┘                             │ Foreign Key
                 │                                               │
┌────────────────▼─────────────────┐           ┌─────────────────┴────────────────┐
│ class Book (OWNING SIDE ✅)      │           │ Таблица: books                   │
│ • @ManyToOne(fetch = LAZY)       │ ────────► │ • id (PK) | title | author_id(FK)│
│ • @JoinColumn(name = "author_id")│           └──────────────────────────────────┘
│   Author author;                 │
└──────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA HIBERNATE, C# EF CORE, PYTHON DJANGO ORM, JS PRISMA)

| Релация / Концепция | ☕ Java 21+ (Hibernate / JPA) | 🟣 C# 12+ (EF Core) | 🐍 Python 3.x (Django ORM) | 🟡 JS / TS (Prisma ORM) |
| :--- | :--- | :--- | :--- | :--- |
| **Много-към-Едно (N:1)** | `@ManyToOne(fetch = LAZY)` + `@JoinColumn` | `public Author Author { get; set; }` + `AuthorId` | `author = models.ForeignKey(Author, ...)` | `author Author @relation(fields: [...])` |
| **Едно-към-Много (1:N)** | `@OneToMany(mappedBy = "author")` | `public ICollection<Book> Books { get; set; }` | `author.book_set` (или `related_name`) | `books Book[]` |
| **Едно-към-Едно (1:1)** | `@OneToOne` + `@JoinColumn(unique = true)` | `HasOne(...).WithOne(...)` | `models.OneToOneField(User, ...)` | `profile Profile?` |
| **Много-към-Много (M:N)** | `@ManyToMany` + `@JoinTable(...)` | `ICollection<Tag>` от двете страни | `models.ManyToManyField(Tag)` | `tags Tag[]` (implicit join table) |
| **Изтриване на сираци** | `orphanRemoval = true` | `DeleteBehavior.Cascade` | `on_delete=models.CASCADE` | `onDelete: Cascade` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА `FETCHTYPE.LAZY` СРЕЩУ `FETCHTYPE.EAGER`

### 🟢 `FetchType.LAZY` (Мързеливо зареждане — Препоръчително навсякъде):
*   **Предимства:** Когато заредиш `Book`, Hibernate **не дърпа** целия `Author` (и неговите 50 книги) от базата, докато не извикаш изрично `book.getAuthor().getName()`. Спестява огромно количество RAM и мрежов трафик.
*   **Недостатъци:** Ако опиташ да достъпиш мързеливата колекция **след като транзакцията е приключила** (напр. в контролера или в Thymeleaf шаблона), Hibernate хвърля **`LazyInitializationException: could not initialize proxy - no Session`**.

### 🔴 `FetchType.EAGER` (Нетърпеливо зареждане — Опасно по подразбиране за `@ManyToOne` / `@OneToOne`):
*   **Недостатъци:** Веднага щом заредиш 100 поръчки, Hibernate автоматично зарежда 100 потребителя, техните профили, техните адреси и т.н. (**Cartesian Product / Лавина от заявки**), дори да ти трябват само сумите на поръчките!

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ЗАБРАВЕН `MAPPEDBY` И `LAZYINITIALIZATIONEXCEPTION`)

### ❌ ГРЕШЕН КОД (Забравен `mappedBy` при `@OneToMany` и липсващ Helper метод за синхронизация):
```java
@Entity
public class Author {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    // 🚨 ГРЕШКА 1: Липсва mappedBy = "author"!
    // Hibernate решава, че това е отделна връзка и създава ТРЕТА излишна таблица `authors_books`!
    @OneToMany
    private List<Book> books = new ArrayList<>();
}

@Entity
public class Book {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    // 🚨 ГРЕШКА 2: По подразбиране @ManyToOne е EAGER! Винаги слагай fetch = FetchType.LAZY!
    @ManyToOne
    private Author author;
}
```
*   **Как да се избегне:** Винаги слагай **`mappedBy`** на `@OneToMany`, винаги задавай **`fetch = FetchType.LAZY`** на `@ManyToOne` и `@OneToOne`, и пиши помощни методи **`addBook(Book b)`**, които поддържат двете страни на връзката в паметта синхронизирани.

### 🟢 ПРАВИЛЕН КОД (Чиста двупосочна 1:N връзка с `LAZY`, `mappedBy` и синхронизиращи методи):
```java
@Entity
@Table(name = "authors")
public class Author {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    @OneToMany(mappedBy = "author", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<Book> books = new ArrayList<>();

    protected Author() {}
    public Author(String name) { this.name = name; }

    // Помощен метод, който синхронизира И ДВЕТЕ страни на връзката в паметта!
    public void addBook(Book book) {
        books.add(book);
        book.setAuthor(this);
    }

    public void removeBook(Book book) {
        books.remove(book);
        book.setAuthor(null);
    }
}

@Entity
@Table(name = "books")
public class Book {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String title;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "author_id", nullable = false)
    private Author author;

    protected Book() {}
    public Book(String title) { this.title = title; }
    public void setAuthor(Author author) { this.author = author; }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Защо при `@ManyToMany` релация в Hibernate **никога** не трябва да използваш `List<Tag>` и `CascadeType.REMOVE`, а вместо това трябва да използваш **`Set<Tag>`** и само `{CascadeType.PERSIST, CascadeType.MERGE}`?“*
*   **Уловката:** Това е класически Senior Hibernate въпрос, който улавя катастрофални грешки в продукция!
*   **Точен Senior отговор:** 
    1.  **Защо `Set<Tag>`, а не `List<Tag>`:** Когато изтриеш 1 елемент от `@ManyToMany` колекция, моделирана като `List`, Hibernate генерира `DELETE FROM post_tags WHERE post_id = ?` (изтрива **всички** редове за този пост!) и после прави `INSERT` на останалите $N-1$ реда наново! При **`Set<Tag>`** Hibernate изтрива **само конкретния ред** `DELETE FROM post_tags WHERE post_id = ? AND tag_id = ?`.
    2.  **Защо НЕ `CascadeType.REMOVE` (или `ALL`) при `@ManyToMany`:** Ако изтриеш една статия (`Post`), `CascadeType.REMOVE` ще изтрие от базата и самия споделен таг `"Java"`, което от своя страна ще счупи или изтрие всички останали 500 статии с таг `"Java"`!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (HIBERNATE) СРЕЩУ C# 12+ (EF CORE)

| Архитектурен детайл | ☕ Java 21+ (Hibernate / JPA) | 🟣 C# 12+ (Entity Framework Core) |
| :--- | :--- | :--- |
| **Default зареждане на релации** | `@ManyToOne` / `@OneToOne` са **`EAGER`**; `@OneToMany` са **`LAZY`** | Всички релации са **`LAZY`** по подразбиране (Lazy Proxies изискват `virtual` и пакет) |
| **Зареждане на релация в заявка** | `JOIN FETCH` в JPQL или `@EntityGraph` | `.Include(a => a.Books).ThenInclude(b => b.Reviews)` |
| **Изтриване на „сираци“ (Orphans)** | `orphanRemoval = true` на `@OneToMany` / `@OneToOne`| `OnDelete(DeleteBehavior.Cascade)` за задължителни връзки |
| **Наследяване на таблици в БД** | `@Inheritance(strategy = InheritanceType.JOINED)` (поддържа `SINGLE_TABLE`, `JOINED`, `TABLE_PER_CLASS`) | **TPH** (Table-Per-Hierarchy по подразбиране), **TPT** (`UseTptMappingStrategy`), **TPC** |
| **Синхронизация на двупосочна връзка** | Ръчни методи `addBook()` / `removeBook()` (задължително!) | EF Core **Fix-up** автоматично синхронизира навигационните свойства в паметта |

---
[⬅️ 03-02. Introduction to Hibernate](03-02-00-Hibernate-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-03-01. Syntax From Scratch](03-03-01-Syntax-From-Scratch.md)
