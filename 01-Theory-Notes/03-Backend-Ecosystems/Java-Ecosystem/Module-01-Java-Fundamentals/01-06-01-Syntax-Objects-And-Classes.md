[⬅️ 01-06-00. Теория & Капани](01-06-00-Objects-And-Classes.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-07-00. Associative Arrays (Maps)](01-07-00-Maps-And-Stream-API.md)

---

# 01-06-01. Objects, Classes and Records: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на класове, конструктори, getters/setters, `record` класове с валидация (Compact Constructor) и сортиране на обекти в Java от нулата.

## 🧭 Съдържание на темата
* [1. Пълен шаблон на Java клас (Полета, Конструктори, Getters/Setters)](#1-пълен-шаблон-на-java-клас-полета-конструктори-getterssetters)
* [2. Модерен `record` клас с валидация (Compact Constructor)](#2-модерен-record-клас-с-валидация-compact-constructor)
* [3. Работа със списък от обекти и сортиране (`Comparator.comparing`)](#3-работа-със-списък-от-обекти-и-сортиране-comparatorcomparing)
* [4. Бързи клавишни комбинации в IntelliJ IDEA (`Alt + Insert`)](#4-бързи-клавишни-комбинации-в-intellij-idea-alt--insert)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПЪЛЕН ШАБЛОН НА JAVA КЛАС (ПОЛЕТА, КОНСТРУКТОРИ, GETTERS/SETTERS)

```java
public class Student {
    // 1. Капсулирани полета (private)
    private String firstName;
    private String lastName;
    private double grade;

    // 2. Конструктор
    public Student(String firstName, String lastName, double grade) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.grade = grade;
    }

    // 3. Getters и Setters
    public String getFirstName() { return firstName; }
    public void setFirstName(String firstName) { this.firstName = firstName; }

    public String getLastName() { return lastName; }
    public void setLastName(String lastName) { this.lastName = lastName; }

    public double getGrade() { return grade; }
    public void setGrade(double grade) { this.grade = grade; }

    // 4. Форматиране при принтиране
    @Override
    public String toString() {
        return String.format("%s %s: %.2f", firstName, lastName, grade);
    }
}
```

---

## 2. МОДЕРЕН `RECORD` КЛАС С ВАЛИДАЦИЯ (COMPACT CONSTRUCTOR)

В `record` можеш да добавиш **Compact Constructor** (без скоби за параметрите), за да валидираш данните още при създаването на обекта:

```java
public record ProductDto(String name, double price) {
    // Compact Constructor за валидация
    public ProductDto {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("Името на продукта е задължително!");
        }
        if (price < 0) {
            throw new IllegalArgumentException("Цената не може да е отрицателна!");
        }
    }
}

// Използване:
ProductDto laptop = new ProductDto("ThinkPad", 2400.00);
System.out.println(laptop.name());  // Достъп без 'get' -> ThinkPad
System.out.println(laptop.price()); // 2400.0
System.out.println(laptop);         // Автоматичен toString: ProductDto[name=ThinkPad, price=2400.0]
```

---

## 3. РАБОТА СЪС СПИСЪК ОТ ОБЕКТИ И СОРТИРАНЕ (`COMPARATOR.COMPARING`)

```java
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

public class ObjectsSortingDemo {
    public static void main(String[] args) {
        List<Student> students = new ArrayList<>();
        students.add(new Student("Иван", "Петров", 5.50));
        students.add(new Student("Анна", "Георгиева", 6.00));
        students.add(new Student("Борис", "Димитров", 6.00));

        // Сортиране по оценка НИЗХОДЯЩО, а при равна оценка по име ВЪЗХОДЯЩО:
        students.sort(
            Comparator.comparingDouble(Student::getGrade).reversed()
                      .thenComparing(Student::getFirstName)
        );

        for (Student s : students) {
            System.out.println(s);
        }
    }
}
```

---

## 4. БЪРЗИ КЛАВИШНИ КОМБИНАЦИИ В INTELLIJ IDEA (`ALT + INSERT`)

В Java никой не пише конструктори, getters, setters и `toString()` на ръка:
1. Напиши само `private` полетата в класа.
2. Натисни **`Alt + Insert`** (Windows/Linux) или **`Cmd + N`** (macOS).
3. Избери последователно **Constructor**, **Getter and Setter**, **equals() and hashCode()** и **toString()**.

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Извикване на друг конструктор** | `this(arg1, arg2);` | Трябва да е **първият ред** в конструктора |
| **Неизменяем клас (`record`)** | `public record Point(int x, int y) {}` | Методите за четене са `p.x()` и `p.y()` |
| **Сортиране на обекти по поле** | `list.sort(Comparator.comparing(User::getName))` | Използва Method Reference (`::`) |
| **Сортиране низходящо** | `Comparator.comparingDouble(Item::getPrice).reversed()` | Използвай `comparingInt` / `comparingDouble` за примитиви |
| **Безопасно сравнение на обекти** | `Objects.equals(objA, objB)` | Не гърми с `NullPointerException`, ако някой е `null` |

---
[⬅️ 01-06-00. Теория & Капани](01-06-00-Objects-And-Classes.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-07-00. Associative Arrays (Maps)](01-07-00-Maps-And-Stream-API.md)
