[⬅️ 01-01-00. Теория & Капани](01-01-00-Syntax-And-Types.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-02-00. Methods, Lists & Streams](01-02-00-Methods-Lists-Streams.md)

---

# 01-01-01. Basic Syntax, Data Types & Arrays: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за конзолен вход/изход, форматиране (`printf`), условни конструкции, модерни `switch` изрази, цикли и масиви в Java от нулата.

## 🧭 Съдържание на темата
* [1. Конзолен вход (`Scanner`) и форматиран изход (`printf`)](#1-конзолен-вход-scanner-и-форматиран-изход-printf)
* [2. Условни конструкции и Модерен `switch` израз (Java 14+)](#2-условни-конструкции-и-модерен-switch-израз-java-14)
* [3. Цикли (`for`, `while`, `do-while` и Enhanced `for-each`)](#3-цикли-for-while-do-while-и-enhanced-for-each)
* [4. Статични Масиви (`Arrays`) — Създаване, обхождане и полезни методи](#4-статични-масиви-arrays--създаване-обхождане-и-полезни-методи)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. КОНЗОЛЕН ВХОД (`SCANNER`) И ФОРМАТИРАН ИЗХОД (`PRINTF`)

```java
import java.util.Scanner;

public class ConsoleIO {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // 1. Четене на различни типове (чрез nextLine + Parse)
        String name = scanner.nextLine();
        int age = Integer.parseInt(scanner.nextLine());
        double salary = Double.parseDouble(scanner.nextLine());

        // 2. Форматиран изход (%s = String, %d = int/long, %.2f = double с 2 знака, %n = нов ред)
        System.out.printf("Служител: %s | Възраст: %d | Заплата: %.2f лв.%n", name, age, salary);
    }
}
```

---

## 2. УСЛОВНИ КОНСТРУКЦИИ И МОДЕРЕН `SWITCH` ИЗРАЗ (JAVA 14+)

Освен класическия `if / else if / else`, модерната Java поддържа **Switch Expressions** със стрелка `->` (без нужда от `break` и без риск от случайно преливане в следващия `case`):

```java
String day = "Saturday";

String dayType = switch (day) {
    case "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" -> "Работен ден";
    case "Saturday", "Sunday" -> "Уикенд";
    default -> "Невалиден ден";
};

System.out.println(dayType); // Уикенд
```

---

## 3. ЦИКЛИ (`FOR`, `WHILE`, `DO-WHILE` И ENHANCED `FOR-EACH`)

```java
// 1. Стандартен for цикъл
for (int i = 0; i < 5; i++) {
    System.out.print(i + " ");
}

// 2. Enhanced for-each цикъл (аналог на C# foreach)
int[] scores = {10, 20, 30};
for (int score : scores) {
    System.out.println(score);
}

// 3. While цикъл с условие за стоп
String command = scanner.nextLine();
while (!"End".equals(command)) {
    System.out.println("Обработка на: " + command);
    command = scanner.nextLine();
}
```

---

## 4. СТАТИЧНИ МАСИВИ (`ARRAYS`) — СЪЗДАВАНЕ, ОБХОЖДАНЕ И ПОЛЕЗНИ МЕТОДИ

```java
import java.util.Arrays;

public class ArraysDemo {
    public static void main(String[] args) {
        // 1. Декларация и инициализация
        int[] numbers = new int;            // Създава [0, 0, 0, 0]
        String[] cities = {"София", "Видин", "Пловдив"};

        // 2. Четене на масив от числа на един ред от конзолата (напр. "10 20 30 40"):
        // int[] inputArr = Arrays.stream(scanner.nextLine().split(" "))
        //                        .mapToInt(Integer::parseInt)
        //                        .toArray();

        // 3. Полезни методи от класа java.util.Arrays:
        Arrays.sort(cities);                     // Сортира на място: [Видин, Пловдив, София]
        System.out.println(Arrays.toString(cities)); // Принтира красиво целия масив!
        System.out.println(String.join(" -> ", cities)); // Съединява низове с разделител
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Операция | Синтаксис в Java | Аналог в C# |
| :--- | :--- | :--- |
| **Парсване на цяло / дробно число** | `Integer.parseInt(s)` / `Double.parseDouble(s)` | `int.Parse(s)` / `double.Parse(s)` |
| **Превръщане на число в текст** | `String.valueOf(num)` или `Integer.toString(num)` | `num.ToString()` |
| **Сравнение на два текста** | `"yes".equals(input)` | `input == "yes"` |
| **Принтиране на масив** | `Arrays.toString(arr)` | `string.Join(", ", arr)` |
| **Съединяване на текстове** | `String.join(", ", stringArr)` | `string.Join(", ", stringArr)` |
| **Четене на `int[]` от един ред** | `Arrays.stream(line.split(" ")).mapToInt(Integer::parseInt).toArray()` | `line.Split(' ').Select(int.Parse).ToArray()` |
| **Математически функции** | `Math.max(a, b)`, `Math.abs(x)`, `Math.pow(a, b)` | `Math.Max(a, b)`, `Math.Abs(x)`, `Math.Pow(a, b)` |

---
[⬅️ 01-01-00. Теория & Капани](01-01-00-Syntax-And-Types.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-02-00. Methods, Lists & Streams](01-02-00-Methods-Lists-Streams.md)
