[⬅️ 01-05. Lists](01-05-00-Lists.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-06-01. Syntax From Scratch](01-06-01-Syntax-From-Scratch.md)

---

# 01-06-00. Objects, Classes and Records (Теория, Конструктори, Памет & Капани)

**Модул 01:** Java Fundamentals | **Лекционна тема 06:** Класове и обекти в Java, референции в Stack и Heap паметта, конструктори и `this(...)` верижно извикване, липсата на Properties (`getters`/`setters`), модерни `record` класове (Java 16+) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Обект в паметта (Stack + Heap Блок-схема)](#2-архитектура-на-обект-в-паметта-stack--heap-блок-схема)
* [3. Сравнителна таблица: Java Classes & Records срещу C#, Python и JavaScript](#3-сравнителна-таблица-java-classes--records-срещу-c-python-и-javascript)
* [4. Плюсове и минуси на стандартните класове срещу `record`](#4-плюсове-и-минуси-на-стандартните-класове-срещу-record)
* [5. Кодов одит: Правилен срещу Грешен код (`equals`/`hashCode` и изчезващия Default конструктор)](#5-кодов-одит-правилен-срещу-грешен-код-equalshashcode-и-изчезващия-default-конструктор)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Класът е чертежът, Обектът е построената къща:** От един чертеж `class Car` можеш да построиш 1000 реални коли (`new Car("BMW")`, `new Car("Audi")`), всяка със собствен адрес в Heap паметта.
*   **Няма C# Properties (`{ get; set; }`) в Java:** В Java полетата са `private String name;`, а достъпът става чрез обикновени методи: `public String getName()` и `public void setName(String name)`.
*   **`record` (от Java 16+) = Неизменяем носител на данни с 1 ред:** Точно като `record` в C# и `@dataclass(frozen=True)` в Python! Когато напишеш `public record UserDto(String name, int age) {}`, Java автоматично ти генерира `private final` полета, конструктор, гетъри (`user.name()`, без `get` отпред!), `equals()`, `hashCode()` и `toString()`.

---

## 2. АРХИТЕКТУРА НА ОБЕКТ В ПАМЕТТА (STACK + HEAP БЛОК-СХЕМА)

```text
STACK ПАМЕТ (Локални референции)         HEAP ПАМЕТ (Реалният обект)
┌──────────────────────────────┐         ┌──────────────────────────────────────┐
│ Student s1 = 0x9A20 ─────────┼────────►│ Обект Student @ 0x9A20               │
├──────────────────────────────┤         │ • name: "Елизабет" (ref -> String)   │
│ Student s2 = s1 (0x9A20) ────┼────────►│ • grade: 6.00                        │
└──────────────────────────────┘         └──────────────────────────────────────┘
* Ако промениш s2.setGrade(5.50), променяш обекта @ 0x9A20 — затова и s1.getGrade() става 5.50!
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA CLASSES & RECORDS СРЕЩУ C#, PYTHON И JAVASCRIPT

| Концепция | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Дефиниция на клас** | `public class User { ... }` | `public class User { ... }` | `class User:` | `class User { ... }` |
| **Конструктор** | `public User(String name) {}` | `public User(string name) {}` | `def __init__(self, name):` | `constructor(name) {}` |
| **Създаване на инстанция** | `var u = new User("Ivan");` | `var u = new User("Ivan");` | `u = User("Ivan")` (без `new`!) | `const u = new User("Ivan");` |
| **Свойства (Getters/Setters)** | Методи `getName()` / `setName()` | `public string Name { get; set; }` | `@property` декоратор | `get name()` / `set name(v)` |
| **Неизменяем DTO модел** | `record User(String name) {}` | `record User(string Name);` | `@dataclass(frozen=True)` | `Object.freeze({ name })` |
| **Текстово представяне** | `@Override public String toString()` | `public override string ToString()` | `def __str__(self):` | `toString() { ... }` |
| **Текуща инстанция** | `this.name = name;` | `this.name = name;` | `self.name = name` | `this.name = name;` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА СТАНДАРТНИТЕ КЛАСОВЕ СРЕЩУ `RECORD`

### 🟢 Стандартен `class` — Предимства и Недостатъци:
*   **Предимства:** Позволява промяна на състоянието (`setters`); може да наследява други класове (`extends`); идеален за **JPA / Hibernate Entity** модели в бази данни.
*   **Недостатъци:** Изисква много писане на повтарящ се код (`getters`, `setters`, `equals`, `hashCode`, `toString`) или външната библиотека **Lombok** (`@Data`, `@Getter`, `@Setter`).

### 🔵 Модерен `record` — Предимства и Недостатъци:
*   **Предимства:** 100% неизменяем (`Immutable` и Thread-Safe); спестява 50 реда код; идеален за **DTO обекти в REST API** и ключове в `HashMap`.
*   **Недостатъци:** Не може да има `setters`; не може да наследява друг клас (защото скрито наследява `java.lang.Record` и е `final`); **не е подходящ за Hibernate `@Entity` класове** (защото Hibernate изисква конструктор без параметри и променяеми полета).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`EQUALS`/`HASHCODE` И ИЗЧЕЗВАЩИЯ DEFAULT КОНСТРУКТОР)

### ❌ ГРЕШЕН КОД (Забравен празен конструктор и липсващ `equals`/`hashCode` в `HashSet`/`HashMap`):
```java
public class Book {
    private String title;

    // 🚨 ГРЕШКА 1: Щом напишеш конструктор с параметри, Java ИЗТРИВА празния конструктор Book()!
    // Ако по-късно Jackson (JSON) или Hibernate опита да извика new Book(), програмата гърми!
    public Book(String title) {
        this.title = title;
    }
    // 🚨 ГРЕШКА 2: Не са пренаписани equals() и hashCode()!
    // Две книги new Book("Dune") и new Book("Dune") се смятат за РАЗЛИЧНИ в HashSet и List.contains()!
}
```
*   **Как да се избегне:** Ако добавиш конструктор с параметри в стандартен клас, **винаги** оставяй и празен конструктор `public Book() {}` (особено за Spring/Hibernate/Jackson) и генерирай `equals()` + `hashCode()` (в IntelliJ IDEA: **`Alt + Insert`** ➔ *Generate equals() and hashCode()*).

### 🟢 ПРАВИЛЕН КОД (Чист Java клас с верижни конструктори, `equals`/`hashCode` и `toString`):
```java
import java.util.Objects;

public class Book {
    private String title;
    private double price;

    public Book() {
    }

    public Book(String title) {
        this(title, 0.0); // Извиква пълния конструктор (Constructor Chaining)
    }

    public Book(String title, double price) {
        this.title = title;
        this.price = price;
    }

    public String getTitle() { return title; }
    public void setTitle(String title) { this.title = title; }
    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Book book)) return false;
        return Double.compare(price, book.price) == 0 && Objects.equals(title, book.title);
    }

    @Override
    public int hashCode() {
        return Objects.hash(title, price);
    }

    @Override
    public String toString() {
        return String.format("Book{title='%s', price=%.2f}", title, price);
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какъв е договорът (Contract) между методите **`equals()`** и **`hashCode()`** в Java и какво ще се счупи, ако пренапишеш само `equals()`, но забравиш `hashCode()`?“*
*   **Уловката:** Това е **топ 3 най-често задаван въпрос на Java интервюта**!
*   **Точен Senior отговор:** 
    1.  **Договорът гласи:** Ако два обекта са равни според `a.equals(b) == true`, те **ЗАДЪЛЖИТЕЛНО** трябва да връщат едно и също цяло число от `a.hashCode() == b.hashCode()`. (Обратното не е задължително — два различни обекта могат да имат еднакъв хеш код, което се нарича *Hash Collision*).
    2.  **Какво се чупи, ако забравиш `hashCode()`:** Колекциите `HashMap` и `HashSet` първо изчисляват `hashCode()`, за да намерят в коя „кофа“ (Bucket) да търсят обекта, и **чак след това** викат `equals()`. Ако пренапишеш само `equals()`, две еднакви книги `new Book("Dune")` ще наследят различните адреси в паметта от `Object.hashCode()`, ще попаднат в **различни кофи** и `hashSet.contains(book2)` ще върне **`false`**, въпреки че `book1.equals(book2)` е `true`!

---
[⬅️ 01-05. Lists](01-05-00-Lists.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-06-01. Syntax From Scratch](01-06-01-Syntax-From-Scratch.md)
