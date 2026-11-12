[⬅️ 01-01-00. Теория & Капани](01-01-00-Basic-Syntax-And-Loops.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-02-00. Data Types and Variables](01-02-00-Data-Types-And-Variables.md)

---

# 01-01-01. Basic Syntax, Conditionals & Loops: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за структура на програма, четене от конзола, условни конструкции, всички видове цикли и форматиране на изхода в Java от нулата.

## 🧭 Съдържание на темата
* [1. Минимален скелет на всяка Java програма](#1-минимален-скелет-на-всяка-java-програма)
* [2. Четене на вход и форматиран печат (`printf` и `DecimalFormat`)](#2-четене-на-вход-и-форматиран-печат-printf-и-decimalformat)
* [3. Условни конструкции (`if-else`, Тернарен оператор, `switch`)](#3-условни-конструкции-if-else-тернарен-оператор-switch)
* [4. Всички видове цикли (`for`, `while`, `do-while`, `break`, `continue`)](#4-всички-видове-цикли-for-while-do-while-break-continue)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. МИНИМАЛЕН СКЕЛЕТ НА ВСЯКА JAVA ПРОГРАМА

*Забележка за Judge: Когато предаваш решение в SoftUni Judge, **не копирай** реда `package ...;` най-отгоре и винаги кръщавай главния клас **`Main`**.*

```java
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // Твоят код започва тук...
    }
}
```

---

## 2. ЧЕТЕНЕ НА ВХОД И ФОРМАТИРАН ПЕЧАТ (`PRINTF` И `DECIMALFORMAT`)

```java
import java.text.DecimalFormat;
import java.util.Scanner;

public class InputOutputDemo {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // 1. Безопасно четене на различни типове от конзолата
        String text = scanner.nextLine();
        int wholeNumber = Integer.parseInt(scanner.nextLine());
        double decimalNumber = Double.parseDouble(scanner.nextLine());
        char symbol = scanner.nextLine().charAt(0); // Взима първия символ от реда

        // 2. Форматиране с фиксиран брой знаци след запетаята (%.2f)
        System.out.printf("Текст: %s | Цяло: %d | Дробно: %.2f | Символ: %c%n",
                text, wholeNumber, decimalNumber, symbol);

        // 3. Форматиране без излишни нули отзад (напр. 1.500 -> 1.5, 2.000 -> 2)
        DecimalFormat df = new DecimalFormat("0.###");
        System.out.println(df.format(1.500)); // Изход: 1.5
    }
}
```

---

## 3. УСЛОВНИ КОНСТРУКЦИИ (`IF-ELSE`, ТЕРНАРЕН ОПЕРАТОР, `SWITCH`)

```java
int grade = 5;

// 1. Класически if - else if - else
if (grade >= 5) {
    System.out.println("Отличен");
} else if (grade >= 3) {
    System.out.println("Среден / Добър");
} else {
    System.out.println("Слаб");
}

// 2. Тернарен оператор (условие ? ако_е_вярно : ако_е_грешно)
String status = (grade >= 3) ? "Издържал" : "Скъсан";

// 3. Модерен Switch Expression (връща стойност директно)
int month = 4;
String season = switch (month) {
    case 12, 1, 2 -> "Зима";
    case 3, 4, 5 -> "Пролет";
    case 6, 7, 8 -> "Лято";
    case 9, 10, 11 -> "Есен";
    default -> "Невалиден месец";
};
```

---

## 4. ВСИЧКИ ВИДОВЕ ЦИКЛИ (`FOR`, `WHILE`, `DO-WHILE`, `BREAK`, `CONTINUE`)

```java
// 1. For цикъл (напред и назад)
for (int i = 1; i <= 10; i++) {
    if (i % 2 == 0) continue; // Пропуска четните числа
    if (i == 7) break;        // Прекъсва цикъла при 7
    System.out.print(i + " "); // Изход: 1 3 5
}

// 2. While цикъл (докато не получим команда "End")
String command = scanner.nextLine();
while (!"End".equals(command)) {
    int number = Integer.parseInt(command);
    System.out.println(number * 2);
    command = scanner.nextLine();
}

// 3. Do-While цикъл (изпълнява се ПОНЕ ВЕДНЪЖ, преди да провери условието)
int attempts = 0;
do {
    attempts++;
} while (attempts < 3);
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java | Аналог в C# | Аналог в Python |
| :--- | :--- | :--- | :--- |
| **Печат с нов ред / без нов ред** | `System.out.println()` / `print()` | `Console.WriteLine()` / `Write()` | `print()` / `print(..., end="")` |
| **Спецификатор за нов ред в `printf`** | `%n` (платформено независим) | `\n` или `Environment.NewLine` | `\n` |
| **Парсване на `int` / `double`** | `Integer.parseInt(s)` / `Double.parseDouble(s)` | `int.Parse(s)` / `double.Parse(s)` | `int(s)` / `float(s)` |
| **Взимане на 1-ви символ от вход** | `scanner.nextLine().charAt(0)` | `Console.ReadLine()[0]` | `input()[0]` |
| **Логическо И / ИЛИ / НЕ** | `&&` / `\|\|` / `!` | `&&` / `\|\|` / `!` | `and` / `or` / `not` |
| **Сравнение на стрингове (case-insensitive)** | `s1.equalsIgnoreCase(s2)` | `s1.Equals(s2, StringComparison.OrdinalIgnoreCase)` | `s1.lower() == s2.lower()` |

---
[⬅️ 01-01-00. Теория & Капани](01-01-00-Basic-Syntax-And-Loops.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-02-00. Data Types and Variables](01-02-00-Data-Types-And-Variables.md)
