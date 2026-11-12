[⬅️ 02-01-00. Теория & Капани](02-01-00-Abstraction-And-Enums.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-02-00. Encapsulation](02-02-00-Encapsulation.md)

---

# 02-01-01. Working with Abstraction (`Enums`, `Static` & Packages): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на прости и богати `enum` класове, вградени `Enum` методи, `static` членове и пакети в Java от нулата.

## 🧭 Съдържание на темата
* [1. Прост `enum` и вградени методи (`values`, `valueOf`, `name`, `ordinal`)](#1-прост-enum-и-вградени-методи-values-valueof-name-ordinal)
* [2. Богат `enum` с полета, конструктор и абстрактен метод](#2-богат-enum-с-полета-конструктор-и-абстрактен-метод)
* [3. Статични полета, методи, константи и `static` блок](#3-статични-полета-методи-константи-и-static-блок)
* [4. Пакети (`package`), `import` и `import static`](#4-пакети-package-import-и-import-static)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПРОСТ `ENUM` И ВГРАДЕНИ МЕТОДИ (`VALUES`, `VALUEOF`, `NAME`, `ORDINAL`)

```java
public enum DayOfWeek {
    MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY
}

public class EnumMethodsDemo {
    public static void main(String[] args) {
        DayOfWeek today = DayOfWeek.THURSDAY;

        // 1. Вградени методи на всеки Enum:
        String exactName = today.name();        // "THURSDAY"
        int position = today.ordinal();         // 3 (индексът започва от 0!)

        // 2. Парсване от String към Enum
        DayOfWeek parsed = DayOfWeek.valueOf("FRIDAY");

        // 3. Обхождане на всички константи в Enum-а
        for (DayOfWeek day : DayOfWeek.values()) {
            System.out.println(day.ordinal() + " -> " + day);
        }
    }
}
```

---

## 2. БОГАТ `ENUM` С ПОЛЕТА, КОНСТРУКТОР И АБСТРАКТЕН МЕТОД

Всеки елемент на `enum` може да имплементира собствена логика (замества `switch` конструкции):

```java
public enum Operation {
    PLUS("+") {
        @Override
        public double apply(double x, double y) { return x + y; }
    },
    MINUS("-") {
        @Override
        public double apply(double x, double y) { return x - y; }
    },
    MULTIPLY("*") {
        @Override
        public double apply(double x, double y) { return x * y; }
    };

    private final String symbol;

    Operation(String symbol) {
        this.symbol = symbol;
    }

    public String getSymbol() { return symbol; }
    public abstract double apply(double x, double y);
}

// Използване:
double res = Operation.MULTIPLY.apply(5, 4); // 20.0
```

---

## 3. СТАТИЧНИ ПОЛЕТА, МЕТОДИ, КОНСТАНТИ И `STATIC` БЛОК

```java
public class BankAccount {
    // 1. Статична константа (пише се с ГЛАВНИ_БУКВИ)
    public static final String CURRENCY = "BGN";

    // 2. Споделен статичен брояч за всички обекти от класа
    private static int accountsCreated = 0;

    // 3. Статичен инициализиращ блок (изпълнява се 1 път при зареждане на класа)
    static {
        System.out.println("Класът BankAccount е зареден в JVM!");
    }

    private final int id;

    public BankAccount() {
        this.id = ++accountsCreated;
    }

    // 4. Статичен метод (вика се директно през името на класа: BankAccount.getTotalAccounts())
    public static int getTotalAccounts() {
        return accountsCreated;
    }
}
```

---

## 4. ПАКЕТИ (`PACKAGE`), `IMPORT` И `IMPORT STATIC`

```java
package bg.softuni.shop.models; // Задължително е ПЪРВИЯТ ред във файла!

import java.util.List;
import static java.lang.Math.PI;     // Позволява да пишеш директно PI вместо Math.PI
import static java.lang.Math.pow;    // Позволява да пишеш директно pow(2, 3)

public class Circle {
    public double getArea(double radius) {
        return PI * pow(radius, 2);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Метод | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Сравнение на два `Enum`-а** | `status == OrderStatus.PAID` | За `Enum` сравнението с **`==` е напълно безопасно** (Singleton) |
| **Масив от всички стойности** | `MyEnum.values()` | Връща нов масив при всяко извикване |
| **Текст към `Enum`** | `MyEnum.valueOf(str.toUpperCase())` | Хвърля `IllegalArgumentException`, ако липсва |
| **Пореден индекс на константа** | `enumVal.ordinal()` | Избягвай да го пазиш в база данни (чупи се при пренареждане!) |
| **Статична константа** | `public static final int MAX = 10;` | Инициализира се при декларация или в `static {}` блок |
| **Static Import** | `import static pkg.Class.method;` | Внася статични методи/константи директно в класа |

---
[⬅️ 02-01-00. Теория & Капани](02-01-00-Abstraction-And-Enums.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-02-00. Encapsulation](02-02-00-Encapsulation.md)
