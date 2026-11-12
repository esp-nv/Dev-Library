[⬅️ 02-06-00. Теория & Капани](02-06-00-SOLID-Principles.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-07-00. Reflection & Annotations](02-07-00-Reflection-And-Annotations.md)

---

# 02-06-01. SOLID Principles in Java: Синтаксис от нулата & Cheat Sheet

**Цел:** Практически рефакторинг примери на всеки от петте SOLID принципа (SRP, OCP, LSP, ISP, DIP) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Single Responsibility Principle (SRP) — Разделяне на отговорностите](#1-single-responsibility-principle-srp--разделяне-на-отговорностите)
* [2. Open/Closed Principle (OCP) — Разширяване без промяна](#2-openclosed-principle-ocp--разширяване-без-промяна)
* [3. Liskov Substitution Principle (LSP) — Коректна йерархия](#3-liskov-substitution-principle-lsp--коректна-йерархия)
* [4. Interface Segregation (ISP) и Dependency Inversion (DIP)](#4-interface-segregation-isp-и-dependency-inversion-dip)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. SINGLE RESPONSIBILITY PRINCIPLE (SRP) — РАЗДЕЛЯНЕ НА ОТГОВОРНОСТИТЕ

Вместо един клас `Invoice` да смята данъци, да принтира и да записва в база данни, разделяме задачите в три специализирани класа:

```java
// 1. Отговаря само за данните и сметките
public class Invoice {
    private final double amount;
    public Invoice(double amount) { this.amount = amount; }
    public double calculateTotalWithVat() { return amount * 1.20; }
}

// 2. Отговаря само за визуализацията
public class InvoicePrinter {
    public void print(Invoice invoice) {
        System.out.printf("Фактура общо: %.2f лв.%n", invoice.calculateTotalWithVat());
    }
}

// 3. Отговаря само за записа в базата данни
public class InvoiceRepository {
    public void save(Invoice invoice) {
        System.out.println("Запис в PostgreSQL...");
    }
}
```

---

## 2. OPEN/CLOSED PRINCIPLE (OCP) — РАЗШИРЯВАНЕ БЕЗ ПРОМЯНА

```java
public interface DiscountStrategy {
    double apply(double price);
}

public class RegularDiscount implements DiscountStrategy {
    @Override
    public double apply(double price) { return price * 0.95; } // 5% отстъпка
}

public class VipDiscount implements DiscountStrategy {
    @Override
    public double apply(double price) { return price * 0.80; } // 20% отстъпка
}

// Този калкулатор НИКОГА повече няма да се пипа, дори да добавим 50 нови промоции!
public class PriceCalculator {
    public double getFinalPrice(double price, DiscountStrategy strategy) {
        return strategy.apply(price);
    }
}
```

---

## 3. LISKOV SUBSTITUTION PRINCIPLE (LSP) — КОРЕКТНА ЙЕРАРХИЯ

Вместо `Square` да наследява `Rectangle` и да чупи очакванията при `setWidth()`, и двата класа имплементират общ интерфейс `Shape`:

```java
public interface Shape {
    double getArea();
}

public record Rectangle(double width, double height) implements Shape {
    @Override
    public double getArea() { return width * height; }
}

public record Square(double side) implements Shape {
    @Override
    public double getArea() { return side * side; }
}
```

---

## 4. INTERFACE SEGREGATION (ISP) И DEPENDENCY INVERSION (DIP)

```java
// ISP: Малки, фокусирани интерфейси вместо един "дебел" интерфейс
interface Printable { void print(); }
interface Scannable { void scan(); }

// Обикновеният принтер имплементира само това, което може
class SimplePrinter implements Printable {
    @Override
    public void print() { System.out.println("Печат..."); }
}

// DIP: Високоуровневият клас зависи от абстракция, подадена в конструктора
class ReportGenerator {
    private final Printable printer;

    public ReportGenerator(Printable printer) {
        this.printer = printer;
    }

    public void generate() {
        printer.print();
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Буква | Принцип | Червен флаг (Нарушение) | Решение в Java |
| :--- | :--- | :--- | :--- |
| **S** | Single Responsibility | Клас с 1000 реда (`UserManager`) | Разделяне на `Model`, `Service`, `Repository` |
| **O** | Open / Closed | Дълъг `if-else` / `switch` по тип | Интерфейс + отделни класове имплементации |
| **L** | Liskov Substitution | `@Override` метод хвърля `UnsupportedOperationException` | Преосмисляне на йерархията и общ интерфейс |
| **I** | Interface Segregation | Празни методи в клас, имплементиращ интерфейс | Разцепване на интерфейса на по-малки роли |
| **D** | Dependency Inversion | `private MySqlRepo repo = new MySqlRepo();` | Constructor Injection през интерфейс |

---
[⬅️ 02-06-00. Теория & Капани](02-06-00-SOLID-Principles.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-07-00. Reflection & Annotations](02-07-00-Reflection-And-Annotations.md)
