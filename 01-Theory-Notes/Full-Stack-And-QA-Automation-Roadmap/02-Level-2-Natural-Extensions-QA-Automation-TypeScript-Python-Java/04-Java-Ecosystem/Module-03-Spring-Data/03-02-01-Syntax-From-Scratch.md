[⬅️ 03-02-00. Теория & Капани](03-02-00-Hibernate-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-03-00. Entity Relations (1:1, 1:N, M:N)](03-03-00-Entity-Relations.md)

---

# 03-02-01. Introduction to Hibernate & Entity Lifecycle: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за конфигуриране на Spring Boot + PostgreSQL (`application.properties`), дефиниране на `@Entity` класове с валидация и мапинг на типове (`@Enumerated`, `@Column`, `GenerationType.SEQUENCE`), управление на жизнения цикъл с `EntityManager` и Lifecycle Callbacks (`@PrePersist`, `@PreUpdate`) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Конфигурация на PostgreSQL и Hibernate в `application.properties`](#1-конфигурация-на-postgresql-и-hibernate-в-applicationproperties)
* [2. Пълен шаблон на `@Entity` с `@Enumerated`, `@Column` и `GenerationType.SEQUENCE`](#2-пълен-шаблон-на-entity-с-enumerated-column-и-generationtypesequence)
* [3. Работа с `EntityManager` (`persist`, `find`, `merge`, `remove`, `detach`)](#3-работа-с-entitymanager-persist-find-merge-remove-detach)
* [4. Вградени Lifecycle Callbacks (`@PrePersist`, `@PreUpdate`)](#4-вградени-lifecycle-callbacks-prepersist-preupdate)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. КОНФИГУРАЦИЯ НА POSTGRESQL И HIBERNATE В `APPLICATION.PROPERTIES`

```properties
# Връзка към PostgreSQL база данни
spring.datasource.url=jdbc:postgresql://localhost:5432/shop_db
spring.datasource.username=postgres
spring.datasource.password=secret_pass

# Стратегия за схемата:
# update   -> автоматично създава/обновява таблиците (само за разработка!)
# validate -> проверява дали таблиците съвпадат с @Entity класовете (задължително за продукция!)
spring.jpa.hibernate.ddl-auto=update

# Красиво принтиране на генерирания от Hibernate SQL в конзолата за дебъгване
spring.jpa.show-sql=true
spring.jpa.properties.hibernate.format_sql=true
```

---

## 2. ПЪЛЕН ШАБЛОН НА `@ENTITY` С `@ENUMERATED`, `@COLUMN` И `GENERATIONTYPE.SEQUENCE`

```java
import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

enum AccountStatus {
    ACTIVE, SUSPENDED, CLOSED
}

@Entity
@Table(name = "bank_accounts", indexes = {
    @Index(name = "idx_iban", columnList = "iban", unique = true)
})
public class BankAccount {

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "acc_seq")
    @SequenceGenerator(name = "acc_seq", sequenceName = "bank_accounts_seq", allocationSize = 50)
    private Long id;

    @Column(name = "iban", nullable = false, unique = true, length = 34)
    private String iban;

    @Column(name = "balance", nullable = false, precision = 15, scale = 2)
    private BigDecimal balance;

    // Винаги EnumType.STRING! Иначе при добавяне на нова стойност в средата на Enum-а старите данни се чупят!
    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false, length = 20)
    private AccountStatus status;

    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    // Не се записва в базата данни (живее само в Java паметта)
    @Transient
    private double temporaryDailyLimit;

    protected BankAccount() {}

    public BankAccount(String iban, BigDecimal balance) {
        this.iban = iban;
        this.balance = balance;
        this.status = AccountStatus.ACTIVE;
        this.createdAt = LocalDateTime.now();
    }

    public Long getId() { return id; }
    public String getIban() { return iban; }
    public BigDecimal getBalance() { return balance; }
    public void setBalance(BigDecimal balance) { this.balance = balance; }
    public AccountStatus getStatus() { return status; }
    public void setStatus(AccountStatus status) { this.status = status; }
}
```

---

## 3. РАБОТА С `ENTITYMANAGER` (`PERSIST`, `FIND`, `MERGE`, `REMOVE`, `DETACH`)

```java
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;
import java.math.BigDecimal;

@Repository
public class BankAccountManualRepo {

    @PersistenceContext
    private EntityManager em;

    @Transactional
    public void demonstrateLifecycle() {
        // 1. TRANSIENT (още не е в базата)
        BankAccount acc = new BankAccount("BG80BNBG96611020345678", new BigDecimal("1000.00"));

        // 2. MANAGED (влиза в Persistence Context -> ще се запише при commit)
        em.persist(acc);

        // Dirty Checking: не викаме нищо, Hibernate сам ще генерира UPDATE в края на метода!
        acc.setBalance(new BigDecimal("1500.00"));

        // 3. DETACHED (откачаме обекта -> следващата промяна НЯМА да се запише в базата!)
        em.detach(acc);
        acc.setBalance(new BigDecimal("999999.00"));

        // Връщаме промените обратно в Managed състояние чрез merge()
        BankAccount reattached = em.merge(acc);

        // 4. REMOVED (маркираме за DELETE при commit)
        em.remove(reattached);
    }
}
```

---

## 4. ВГРАДЕНИ LIFECYCLE CALLBACKS (`@PREPERSIST`, `@PREUPDATE`)

Можеш да дефинираш методи вътре в `@Entity` класа, които Hibernate извиква автоматично преди `INSERT` или `UPDATE`:

```java
import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "articles")
public class Article {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String title;
    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;

    protected Article() {}

    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
        this.updatedAt = LocalDateTime.now();
    }

    @PreUpdate
    protected void onUpdate() {
        this.updatedAt = LocalDateTime.now();
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| JPA Анотация / Метод | Предназначение | Важен капан / Бележка |
| :--- | :--- | :--- |
| **`@Entity` & `@Table(name = "...")`** | Свързва Java клас с таблица в базата | Класът не може да е `final` и трябва да има празен конструктор |
| **`@Id` & `@GeneratedValue`** | Дефинира Primary Key | Ползвай `Long` или `UUID`, не примитивен `long` |
| **`@Column(nullable = false, unique = true)`** | Ограничения на колоната в БД | `precision` и `scale` се ползват за `BigDecimal` |
| **`@Enumerated(EnumType.STRING)`** | Записва Enum като текст | Никога не оставяй `EnumType.ORDINAL` |
| **`@Transient`** | Изключва поле от мапинга към базата | Аналог на `[NotMapped]` в C# EF Core |
| **`entityManager.flush()`** | Изпраща натрупаните SQL заявки веднага | Не комитва транзакцията, само синхронизира с БД |
| **`entityManager.clear()`** | Изпразва First-Level Cache | Спасява от `OutOfMemoryError` при големи цикли |

---
[⬅️ 03-02-00. Теория & Капани](03-02-00-Hibernate-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-03-00. Entity Relations (1:1, 1:N, M:N)](03-03-00-Entity-Relations.md)
