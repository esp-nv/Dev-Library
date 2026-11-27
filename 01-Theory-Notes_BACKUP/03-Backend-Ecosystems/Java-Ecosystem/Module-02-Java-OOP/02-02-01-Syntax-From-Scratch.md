[⬅️ 02-02-00. Теория & Капани](02-02-00-Encapsulation.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-03-00. Inheritance](02-03-00-Inheritance.md)

---

# 02-02-01. Encapsulation: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за капсулирани класове, централизирана валидация в сетъри, защитно копиране на колекции (`Defensive Copying`) и вградени проверки с `Objects` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Пълен шаблон на капсулиран клас с валидация](#1-пълен-шаблон-на-капсулиран-клас-с-валидация)
* [2. Защитно копиране на колекции (Defensive Copying)](#2-защитно-копиране-на-колекции-defensive-copying)
* [3. Модификатори за достъп (`private`, `package-private`, `protected`, `public`)](#3-модификатори-за-достъп-private-package-private-protected-public)
* [4. Вградени помощни методи за валидация (`Objects.requireNonNull`)](#4-вградени-помощни-методи-за-валидация-objectsrequirenonnull)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПЪЛЕН ШАБЛОН НА КАПСУЛИРАН КЛАС С ВАЛИДАЦИЯ

Класовете следват принципа за `private` полета и валидация през сетъри, извикани още от конструктора (напр. проверка за празни имена, възрастови граници или минимални стойности за заплата с хвърляне на `IllegalArgumentException`), чиито пълни кодови реализации могат да бъдат разгледани в оригиналния документ.

Конструкторът **винаги** извиква сетърите, за да не се повтаря логиката за валидация на две места:

```java
public class Person {
    private String firstName;
    private int age;
    private double salary;

    public Person(String firstName, int age, double salary) {
        this.setFirstName(firstName);
        this.setAge(age);
        this.setSalary(salary);
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        if (firstName == null || firstName.trim().length() < 3) {
            throw new IllegalArgumentException("Името трябва да е поне 3 символа!");
        }
        this.firstName = firstName;
    }

    public int getAge() {
        return age;
    }

    public void setAge(int age) {
        if (age <= 0 || age > 130) {
            throw new IllegalArgumentException("Възрастта трябва да е между 1 и 130 години!");
        }
        this.age = age;
    }

    public double getSalary() {
        return salary;
    }

    public void setSalary(double salary) {
        if (salary < 933.0) {
            throw new IllegalArgumentException("Заплатата не може да е под минималната!");
        }
        this.salary = salary;
    }

    public void increaseSalary(double percentage) {
        if (this.age < 30) {
            percentage /= 2;
        }
        this.salary += this.salary * (percentage / 100.0);
    }
}
```

---

## 2. ЗАЩИТНО КОПИРАНЕ НА КОЛЕКЦИИ (DEFENSIVE COPYING)

При инициализация на обекти с колекции се прави ново копие (`new ArrayList<>(initialItems)`), за да се предотврати външна модификация, а гетърите връщат `Collections.unmodifiableList`.

Когато класът пази списък или карта, **никога не връщай директно полето**, за да не може външен код да извика `.clear()` или `.add()` зад гърба на класа:

```java
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class FootballTeam {
    private final String name;
    private final List<String> players;

    public FootballTeam(String name, List<String> initialPlayers) {
        this.name = name;
        // 1. Защитно копие при ВХОД (промени във външния списък няма да засегнат отбора)
        this.players = new ArrayList<>(initialPlayers);
    }

    public void addPlayer(String playerName) {
        if (playerName == null || playerName.isBlank()) {
            throw new IllegalArgumentException("Невалидно име на играч!");
        }
        this.players.add(playerName);
    }

    // 2. Защитно обвиване при ИЗХОД (външният код получава Read-Only изглед)
    public List<String> getPlayers() {
        return Collections.unmodifiableList(this.players);
    }
}
```

---

## 3. МОДИФИКАТОРИ ЗА ДОСТЪП (`PRIVATE`, `PACKAGE-PRIVATE`, `PROTECTED`, `PUBLIC`)

```java
package bg.softuni.bank;

public class AccountSecurity {
    private String pinCode;            // Видимо САМО вътре в AccountSecurity
    String internalBranchCode;         // package-private: видимо само в пакет bg.softuni.bank
    protected double overdraftLimit;   // Видимо в пакета + във всички наследници (дори в други пакети)
    public String iban;                // Видимо отвсякъде в проекта
}
```

---

## 4. ВГРАДЕНИ ПОМОЩНИ МЕТОДИ ЗА ВАЛИДАЦИЯ (`OBJECTS.REQUIRENONNULL`)

Използване на `Objects.requireNonNull(param, "msg")` за компактна проверка на задължителни параметри срещу `null`.

Класът `java.util.Objects` (от Java 7+) и `Objects.checkIndex` (от Java 9+) съкращават стандартните проверки на 1 ред:

```java
import java.util.Objects;

public class Order {
    private final String customerId;
    private final String product;

    public Order(String customerId, String product) {
        // Хвърля NullPointerException с точното съобщение, ако аргументът е null
        this.customerId = Objects.requireNonNull(customerId, "Customer ID е задължително!");
        this.product = Objects.requireNonNullElse(product, "Стандартен продукт");
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Техника за капсулация | Синтаксис в Java | Защита, която осигурява |
| :--- | :--- | :--- |
| **Скриване на състоянието** | `private double balance;` | Забранява директен достъп отвън |
| **Непроменяема референция** | `private final List<String> items;` | Полето не може да се пренасочи към нов списък |
| **Жив Read-Only изглед ($O(1)$)** | `Collections.unmodifiableList(list)` | Хвърля `UnsupportedOperationException` при `.add()` |
| **Независимо Read-Only копие ($O(n)$)** | `List.copyOf(list)` | Независимо от оригинала; не допуска `null` |
| **Защита от `null` на 1 ред** | `Objects.requireNonNull(arg, "msg")` | Бърз Guard Clause в конструктори и методи |
| **Централизирана валидация** | Викане на `this.setField(val)` в конструктора | Едно единствено място за бизнес правилата |


* **Валидация в сетър:** хвърляне на `IllegalArgumentException` при невалидни данни.

---
[⬅️ 02-02-00. Теория & Капани](02-02-00-Encapsulation.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-03-00. Inheritance](02-03-00-Inheritance.md)
