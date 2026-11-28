[⬅️ 03-03-00. Hibernate Code First & Entity Relations](03-03-00-Entity-Relations.md) | [📂 Назад към Module 03: Spring Data Хъба](README.md) | [📦 Предишен модул: Module-02-Java-OOP](../Module-02-Java-OOP/README.md) | [📂 Родителски модул: Java & Spring Boot Ecosystem](../README.md) | [🌐 Домейн: 03-Backend-Ecosystems](../../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../../README.md) | [🏛️ Начало Dev-Library](../../../../README.md) | [📂 Текуща папка: Module 03: Spring Data](README.md) | [🌐 Следващ домейн: ️ 04. Бази Данни, ORM Системи и Кеширане](../../../04-Databases-And-Caching/README.md) | [📦 Следващ модул: Module-04-Spring-Fundamentals](../Module-04-Spring-Fundamentals/README.md) | [➡️ 03-04-00. Spring Data Intro & Repositories](03-04-00-Spring-Data-Repositories.md)

---

# 03-03-01. Hibernate Code First & Entity Relations: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за мапиране на `@OneToOne` (`@MapsId`), `@OneToMany` / `@ManyToOne`, `@ManyToMany` с `@JoinTable`, междинна таблица с допълнителни колони и стратегии за наследяване (`@Inheritance`) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Връзка Едно-към-Едно (`@OneToOne` с `@MapsId` за споделен Primary Key)](#1-връзка-едно-към-едно-onetoone-с-mapsid-за-споделен-primary-key)
* [2. Връзка Много-към-Много (`@ManyToMany` със `Set<T>` и `@JoinTable`)](#2-връзка-много-към-много-manytomany-със-sett-и-jointable)
* [3. Когато M:N връзката има допълнителни колони (Междинно Entity)](#3-когато-mn-връзката-има-допълнителни-колони-междинно-entity)
* [4. Наследяване на модели (`@Inheritance(strategy = InheritanceType.JOINED)`)](#4-наследяване-на-модели-inheritancestrategy--inheritancetypejoined)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ВРЪЗКА ЕДНО-КЪМ-ЕДНО (`@ONETOONE` С `@MAPSID` ЗА СПОДЕЛЕН PRIMARY KEY)

Най-ефективният начин за 1:1 връзка в JPA е споделянето на един и същ Primary Key чрез `@MapsId` (спестява излишна колона и индекс):

```java
import jakarta.persistence.*;

@Entity
@Table(name = "users")
public class User {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String username;
}

@Entity
@Table(name = "user_profiles")
public class UserProfile {
    @Id
    private Long id; // Няма @GeneratedValue -> взима ID-то директно от User!

    private String bio;
    private String avatarUrl;

    @OneToOne(fetch = FetchType.LAZY)
    @MapsId
    @JoinColumn(name = "user_id")
    private User user;
}
```

---

## 2. ВРЪЗКА МНОГО-КЪМ-МНОГО (`@MANYTOMANY` СЪС `SET<T>` И `@JOINTABLE`)

```java
import jakarta.persistence.*;
import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "posts")
public class Post {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String title;

    @ManyToMany(cascade = {CascadeType.PERSIST, CascadeType.MERGE})
    @JoinTable(
        name = "posts_tags",
        joinColumns = @JoinColumn(name = "post_id"),
        inverseJoinColumns = @JoinColumn(name = "tag_id")
    )
    private Set<Tag> tags = new HashSet<>();

    public void addTag(Tag tag) {
        this.tags.add(tag);
        tag.getPosts().add(this);
    }
}

@Entity
@Table(name = "tags")
public class Tag {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true, nullable = false)
    private String name;

    @ManyToMany(mappedBy = "tags")
    private Set<Post> posts = new HashSet<>();

    public Set<Post> getPosts() { return posts; }
}
```

---

## 3. КОГАТО M:N ВРЪЗКАТА ИМА ДОПЪЛНИТЕЛНИ КОЛОНИ (МЕЖДИННО ENTITY)

Ако свързващата таблица пази дата на записване, оценка или количество (напр. `StudentCourse` с колона `grade`), разделяме M:N на **две 1:N връзки**:

```java
@Entity
@Table(name = "enrollments")
public class Enrollment {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "student_id", nullable = false)
    private Student student;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "course_id", nullable = false)
    private Course course;

    private Double finalGrade;
}
```

---

## 4. НАСЛЕДЯВАНЕ НА МОДЕЛИ (`@INHERITANCE(STRATEGY = INHERITANCETYPE.JOINED)`)

```java
@Entity
@Table(name = "payments")
@Inheritance(strategy = InheritanceType.JOINED) // Всяко под-entity има собствена таблица, свързана по ID
public abstract class Payment {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private Double amount;
}

@Entity
@Table(name = "card_payments")
public class CardPayment extends Payment {
    private String maskedCardNumber;
}

@Entity
@Table(name = "bank_transfers")
public class BankTransfer extends Payment {
    private String iban;
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| JPA Анотация / Параметър | Къде се поставя | Какво прави |
| :--- | :--- | :--- |
| **`@JoinColumn(name = "...")`** | Върху **Owning Side** (`@ManyToOne` / `@OneToOne`) | Указва името на Foreign Key колоната в таблицата |
| **`mappedBy = "fieldName"`** | Върху **Inverse Side** (`@OneToMany` / `@ManyToMany`) | Сочи към полето в другата класа, което държи `@JoinColumn` |
| **`fetch = FetchType.LAZY`** | Върху всички релации | Зарежда свързаните данни само при изрично извикване |
| **`CascadeType.ALL`** | Върху родителя при 1:N композиция | Пренася `persist`, `merge`, `remove` към децата |
| **`orphanRemoval = true`** | Върху `@OneToMany` / `@OneToOne` | Изтрива записа от БД, ако го премахнеш от списъка `list.remove(child)` |
| **`@MappedSuperclass`** | Върху базов клас без собствена таблица (напр. `BaseEntity`) | Наследниците наследяват неговите колони (`id`, `createdAt`) в своите таблици |







---

[⬅️ 03-03-00. Hibernate Code First & Entity Relations](03-03-00-Entity-Relations.md) | [📂 Назад към Module 03: Spring Data Хъба](README.md) | [📦 Предишен модул: Module-02-Java-OOP](../Module-02-Java-OOP/README.md) | [📂 Родителски модул: Java & Spring Boot Ecosystem](../README.md) | [🌐 Домейн: 03-Backend-Ecosystems](../../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../../README.md) | [🏛️ Начало Dev-Library](../../../../README.md) | [📂 Текуща папка: Module 03: Spring Data](README.md) | [🌐 Следващ домейн: ️ 04. Бази Данни, ORM Системи и Кеширане](../../../04-Databases-And-Caching/README.md) | [📦 Следващ модул: Module-04-Spring-Fundamentals](../Module-04-Spring-Fundamentals/README.md) | [➡️ 03-04-00. Spring Data Intro & Repositories](03-04-00-Spring-Data-Repositories.md)

© 2026 Dev-Library / 03-03-01. Hibernate Code First & Entity Relations: Синтаксис от нулата & Cheat Sheet
