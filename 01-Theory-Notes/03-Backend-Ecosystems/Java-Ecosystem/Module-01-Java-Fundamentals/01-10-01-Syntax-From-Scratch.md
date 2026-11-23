[⬅️ 01-10-00. Теория & Капани](01-10-00-Regular-Expressions.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ Module 02: Java OOP](../Module-02-Java-OOP/02-00-Intro.md)

---

# 01-10-01. Regular Expressions (Regex): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за извличане на данни с `Pattern` и `Matcher`, именувани групи, заместване с `replaceAll` и разделяне на текст с `split` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Извличане на съвпадения и именувани групи (`Pattern` & `Matcher`)](#1-извличане-на-съвпадения-и-именувани-групи-pattern--matcher)
* [2. Валидация на низ (`matches()` срещу `find()`)](#2-валидация-на-низ-matches-срещу-find)
* [3. Заместване и разделяне с Regex (`replaceAll`, `replaceFirst`, `split`)](#3-заместване-и-разделяне-с-regex-replaceall-replacefirst-split)
* [4. Специални символи и квантификатори в Java (Двойно екраниране)](#4-специални-символи-и-квантификатори-в-java-двойно-екраниране)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИЗВЛИЧАНЕ НА СЪВПАДЕНИЯ И ИМЕНУВАНИ ГРУПИ (`PATTERN` & `MATCHER`)

```java
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class RegexExtractionDemo {
    public static void main(String[] args) {
        String logLine = "User: elizabeth | Date: 2026-11-12 | Points: 100";
        String regex = "User: (?<user>\\w+) \\| Date: (?<date>\\d{4}-\\d{2}-\\d{2}) \\| Points: (?<pts>\\d+)";

        Pattern pattern = Pattern.compile(regex);
        Matcher matcher = pattern.matcher(logLine);

        if (matcher.find()) {
            String user = matcher.group("user");            // "elizabeth"
            String date = matcher.group("date");            // "2026-11-12"
            int points = Integer.parseInt(matcher.group("pts")); // 100

            System.out.printf("%s записа %d точки на %s%n", user, points, date);
        }
    }
}
```

---

## 2. ВАЛИДАЦИЯ НА НИЗ (`MATCHES()` СРЕЩУ `FIND()`)

```java
String email = "student@softuni.bg";
String emailRegex = "^[\\w.-]+@[a-zA-Z-]+\\.[a-zA-Z]{2,}\$";

// 1. Бърза еднократна проверка на целия низ
boolean isValidEmail = email.matches(emailRegex); // true

// 2. Проверка без значение от главни и малки букви (Case-Insensitive флаг)
Pattern caseInsensitive = Pattern.compile("java", Pattern.CASE_INSENSITIVE);
boolean hasJava = caseInsensitive.matcher("I love JAVA 21").find(); // true
```

---

## 3. ЗАМЕСТВАНЕ И РАЗДЕЛЯНЕ С REGEX (`REPLACEALL`, `REPLACEFIRST`, `SPLIT`)

```java
// 1. Премахване на всички излишни интервали (2 или повече интервала -> 1 интервал)
String messy = "Java    is     awesome";
String clean = messy.replaceAll("\\s+", " "); // "Java is awesome"

// 2. Разделяне по множество разделители (запетая, точка и запетая или интервал)
String raw = "apple, banana; cherry   date";
String[] fruits = raw.split("[,;\\s]+"); // ["apple", "banana", "cherry", "date"]

// 3. Обратна референция при заместване (\$1, \$2 реферират към групите в скоби)
String date = "2026-11-12";
String bgDate = date.replaceAll("(\\d{4})-(\\d{2})-(\\d{2})", "\$3.\$2.\$1"); // "12.11.2026"
```

---

## 4. СПЕЦИАЛНИ СИМВОЛИ И КВАНТИФИКАТОРИ В JAVA (ДВОЙНО ЕКРАНИРАНЕ)

| Символ в Regex | Запис в Java низ | Какво съвпада |
| :--- | :--- | :--- |
| `\d` / `\D` | `"\\d"` / `"\\D"` | Всяка цифра `[0-9]` / Всичко, което **не** е цифра |
| `\w` / `\W` | `"\\w"` / `"\\W"` | Буква, цифра или `_` `[a-zA-Z0-9_]` / Не-дума |
| `\s` / `\S` | `"\\s"` / `"\\S"` | Интервал, табулация или нов ред / Не-интервал |
| `\b` | `"\\b"` | Граница на дума (Word Boundary) |
| `\.`, `\$`, `\+`, `\(` | `"\\."`, `"\\$"`, `"\\+"`, `"\\("` | Буквалните символи точка, долар, плюс и скоба |
| `^` / `$` | `"^"` / `"$"` | Начало / Край на текста (или реда при `MULTILINE`) |

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Конструкция | Клас | Какво прави |
| :--- | :--- | :--- |
| **`Pattern.compile(regex)`** | `Pattern` | Компилира регулярния израз за многократна употреба |
| **`pattern.matcher(text)`** | `Pattern` | Създава `Matcher` върху подадения текст |
| **`matcher.find()`** | `Matcher` | Търси следващото съвпадение в текста (връща `boolean`) |
| **`matcher.group(0)`** | `Matcher` | Връща **цялото** намерено съвпадение |
| **`matcher.group("name")`** | `Matcher` | Връща стойността на именувана група `(?<name>...)` |
| **`str.matches(regex)`** | `String` | Проверява дали **целият** низ отговаря на шаблона |
| **`str.replaceAll(regex, rep)`** | `String` | Заменя всички съвпадения на регулярния израз |
| **`str.split(regex)`** | `String` | Разделя низа на `String[]` по регулярен израз |

---
[⬅️ 01-10-00. Теория & Капани](01-10-00-Regular-Expressions.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ Module 02: Java OOP](../Module-02-Java-OOP/02-00-Intro.md)
