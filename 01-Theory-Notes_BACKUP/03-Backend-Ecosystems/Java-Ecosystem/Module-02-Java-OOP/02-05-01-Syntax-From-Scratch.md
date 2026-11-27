[⬅️ 02-05-00. Теория & Капани](02-05-00-Polymorphism.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-06-00. SOLID Principles](02-06-00-SOLID-Principles.md)

---

# 02-05-01. Polymorphism: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за Upcasting/Downcasting, модерен `instanceof` Pattern Matching, `switch` Pattern Matching с `when` условия и `sealed` йерархии в Java от нулата.

## 🧭 Съдържание на темата
* [1. Полиморфни колекции и Runtime Overriding](#1-полиморфни-колекции-и-runtime-overriding)
* [2. Модерен `instanceof` Pattern Matching (Java 16+)](#2-модерен-instanceof-pattern-matching-java-16)
* [3. Модерен `switch` Pattern Matching с `when` условия (Java 21+)](#3-модерен-switch-pattern-matching-с-when-условия-java-21)
* [4. Запечатани класове и интерфейси (`sealed` / `non-sealed` / `final`)](#4-запечатани-класове-и-интерфейси-sealed--non-sealed--final)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПОЛИМОРФНИ КОЛЕКЦИИ И RUNTIME OVERRIDING

```java
import java.util.List;

interface Notification {
    void send(String message);
}

class EmailNotification implements Notification {
    @Override
    public void send(String message) {
        System.out.println("Изпращане по Email: " + message);
    }
}

class SmsNotification implements Notification {
    @Override
    public void send(String message) {
        System.out.println("Изпращане по SMS: " + message);
    }
}

public class PolymorphismDemo {
    public static void main(String[] args) {
        // Upcasting: държим различни конкретни класове под общия интерфейс Notification
        List<Notification> channels = List.of(
            new EmailNotification(),
            new SmsNotification()
        );

        for (Notification channel : channels) {
            channel.send("Вашата поръчка е потвърдена!");
        }
    }
}
```

---

## 2. МОДЕРЕН `INSTANCEOF` PATTERN MATCHING (JAVA 16+)

Вместо стария тромав код с ръчно кастване, Java 16+ позволява едновременна проверка и декларация на променлива:

```java
public static void inspectObject(Object obj) {
    // Ако obj е String, автоматично се каства в променливата 's'
    if (obj instanceof String s && !s.isBlank()) {
        System.out.println("Текст с дължина: " + s.length());
    } else if (obj instanceof Integer num) {
        System.out.println("Цяло число, удвоено: " + (num * 2));
    }
}
```

---

## 3. МОДЕРЕН `SWITCH` PATTERN MATCHING С `WHEN` УСЛОВИЯ (JAVA 21+)

```java
public static String formatResponse(Object response) {
    return switch (response) {
        case null -> "Празна заявка (null)";
        case Integer code when code >= 200 && code < 300 -> "Успех (" + code + ")";
        case Integer code when code >= 400 -> "Грешка (" + code + ")";
        case String msg -> "Съобщение: " + msg;
        default -> "Неизвестен формат: " + response.getClass().getSimpleName();
    };
}
```

---

## 4. ЗАПЕЧАТАНИ КЛАСОВЕ И ИНТЕРФЕЙСИ (`SEALED` / `NON-SEALED` / `FINAL`)

Всеки клас, изброен след `permits`, **задължително** трябва да има един от трите модификатора:
1.  **`final`:** Спира наследяването напълно.
2.  **`sealed`:** Ограничава наследяването само до нов списък с `permits`.
3.  **`non-sealed`:** Отваря клона отново за свободно наследяване от всеки клас.

```java
public sealed class Vehicle permits Car, Truck {}

public final class Car extends Vehicle {}

public non-sealed class Truck extends Vehicle {} // Всеки клас вече може да направи extends Truck
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java 21+ | Бележка |
| :--- | :--- | :--- |
| **Upcasting (Неявен)** | `Animal a = new Dog();` | Винаги безопасен |
| **Pattern Matching `instanceof`** | `if (a instanceof Dog d) { d.bark(); }` | Премахва нуждата от `(Dog) a` |
| **Switch върху типове** | `case String s -> s.toLowerCase();` | Поддържа и `case null -> ...` |
| **Допълнително условие в `switch`** | `case Integer i when i > 0 -> ...` | Нарича се *Guarded Pattern* |
| **Запечатан клас (`sealed`)** | `sealed class A permits B, C` | Наследниците трябва да са `final`, `sealed` или `non-sealed` |

---
[⬅️ 02-05-00. Теория & Капани](02-05-00-Polymorphism.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-06-00. SOLID Principles](02-06-00-SOLID-Principles.md)
