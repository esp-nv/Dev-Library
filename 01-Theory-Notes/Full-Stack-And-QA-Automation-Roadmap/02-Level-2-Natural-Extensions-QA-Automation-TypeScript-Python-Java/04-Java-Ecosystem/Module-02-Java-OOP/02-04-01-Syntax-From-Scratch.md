[⬅️ 02-04-00. Теория & Капани](02-04-00-Interfaces-And-Abstract-Classes.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-05-00. Polymorphism](02-05-00-Polymorphism.md)

---

# 02-04-01. Interfaces and Abstraction: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на интерфейси (`default`, `static`, `private` методи), имплементиране на множество интерфейси, абстрактни класове и `@FunctionalInterface` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Създаване и имплементиране на Интерфейс (`default` и `static` методи)](#1-създаване-и-имплементиране-на-интерфейс-default-и-static-методи)
* [2. Имплементиране на множество интерфейси и разрешаване на конфликт](#2-имплементиране-на-множество-интерфейси-и-разрешаване-на-конфликт)
* [3. Абстрактен клас с конструктор, готови и абстрактни методи](#3-абстрактен-клас-с-конструктор-готови-и-абстрактни-методи)
* [4. Функционален интерфейс (`@FunctionalInterface`) и Lambda израз](#4-функционален-интерфейс-functionalinterface-и-lambda-израз)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ И ИМПЛЕМЕНТИРАНЕ НА ИНТЕРФЕЙС (`DEFAULT` И `STATIC` МЕТОДИ)

```java
public interface PaymentProcessor {
    // 1. Константа (имплицитно public static final)
    String DEFAULT_CURRENCY = "BGN";

    // 2. Абстрактен метод (имплицитно public abstract)
    void processPayment(double amount);

    // 3. Default метод с готова имплементация (Java 8+)
    default void printReceipt(double amount) {
        logTransaction(amount);
        System.out.printf("Успешно плащане: %.2f %s%n", amount, DEFAULT_CURRENCY);
    }

    // 4. Static помощен метод в интерфейса (вика се чрез PaymentProcessor.isValidAmount(10))
    static boolean isValidAmount(double amount) {
        return amount > 0;
    }

    // 5. Private помощен метод за споделена вътрешна логика (Java 9+)
    private void logTransaction(double amount) {
        System.out.println("[LOG] Обработка на сума: " + amount);
    }
}

public class CardPayment implements PaymentProcessor {
    @Override
    public void processPayment(double amount) {
        if (PaymentProcessor.isValidAmount(amount)) {
            System.out.println("Теглене от карта...");
            printReceipt(amount);
        }
    }
}
```

---

## 2. ИМПЛЕМЕНТИРАНЕ НА МНОЖЕСТВО ИНТЕРФЕЙСИ И РАЗРЕШАВАНЕ НА КОНФЛИКТ

```java
interface Flyable {
    default void move() { System.out.println("Лети във въздуха"); }
}

interface Swimmable {
    default void move() { System.out.println("Плува във водата"); }
}

public class Duck implements Flyable, Swimmable {
    @Override
    public void move() {
        // Изричен избор на едната или двете имплементации:
        Flyable.super.move();
        Swimmable.super.move();
    }
}
```

---

## 3. АБСТРАКТЕН КЛАС С КОНСТРУКТОР, ГОТОВИ И АБСТРАКТНИ МЕТОДИ

```java
public abstract class Shape {
    private final String color;

    // Конструктор на абстрактния клас (вика се чрез super(color) от наследниците)
    protected Shape(String color) {
        this.color = color;
    }

    public String getColor() {
        return color;
    }

    // Абстрактни методи без тяло (задължителни за пренаписване в конкретните наследници)
    public abstract double calculateArea();
    public abstract double calculatePerimeter();
}

public class Circle extends Shape {
    private final double radius;

    public Circle(String color, double radius) {
        super(color);
        this.radius = radius;
    }

    @Override
    public double calculateArea() {
        return Math.PI * radius * radius;
    }

    @Override
    public double calculatePerimeter() {
        return 2 * Math.PI * radius;
    }
}
```

---

## 4. ФУНКЦИОНАЛЕН ИНТЕРФЕЙС (`@FUNCTIONALINTERFACE`) И LAMBDA ИЗРАЗ

```java
@FunctionalInterface
public interface TaxCalculator {
    double calculate(double income);
}

public class TaxDemo {
    public static void main(String[] args) {
        // Инстанциране директно чрез Lambda израз!
        TaxCalculator flatTax = income -> income * 0.10;
        System.out.println(flatTax.calculate(2000)); // 200.0
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java | Правило / Особеност |
| :--- | :--- | :--- |
| **Дефиниция на интерфейс** | `public interface Movable` | Не може да се инстанцира с `new` |
| **Имплементация на интерфейси** | `class Car implements Movable, Insurable` | Позволява произволен брой интерфейси |
| **Наследяване на интерфейс от интерфейс** | `interface FastCar extends Movable` | Интерфейсите използват `extends`, а не `implements` |
| **Default метод в интерфейс** | `default void stop() { ... }` | Може да бъде пренаписан в класа, но не е задължително |
| **Абстрактен клас** | `public abstract class Vehicle` | Може да има полета, конструктори и конкретни методи |
| **Абстрактен метод** | `public abstract void start();` | Няма тяло `{}`; класовете-наследници трябва да го напишат |

---
[⬅️ 02-04-00. Теория & Капани](02-04-00-Interfaces-And-Abstract-Classes.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-05-00. Polymorphism](02-05-00-Polymorphism.md)
