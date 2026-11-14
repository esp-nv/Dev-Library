[⬅️ 01-09-00. Теория & Капани](01-09-00-Text-Processing.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-10-00. Regular Expressions (Regex)](01-10-00-Regular-Expressions.md)

---

# 01-09-01. Text Processing (`String` & `StringBuilder`): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за манипулация на низове, търсене, заместване, `StringBuilder` методи и модерни многоредови низове (Text Blocks) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Най-важните методи на класа `String`](#1-най-важните-методи-на-класа-string)
* [2. Пълен шаблон за работа със `StringBuilder`](#2-пълен-шаблон-за-работа-със-stringbuilder)
* [3. Модерно форматиране и Text Blocks (`"""` в Java 15+)](#3-модерно-форматиране-и-text-blocks--в-java-15)
* [4. Превръщане между `String` и `char[]` (за манипулация на отделни букви)](#4-превръщане-между-string-и-char-за-манипулация-на-отделни-букви)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. НАЙ-ВАЖНИТЕ МЕТОДИ НА КЛАСА `STRING`

```java
String text = "  Java & Spring Boot  ";

// 1. Почистване и промяна на регистъра
String clean = text.strip();                    // "Java & Spring Boot"
String upper = clean.toUpperCase();             // "JAVA & SPRING BOOT"
String lower = clean.toLowerCase();             // "java & spring boot"

// 2. Търсене и проверки
boolean starts = clean.startsWith("Java");      // true
boolean ends = clean.endsWith("Boot");          // true
boolean hasSpring = clean.contains("Spring");   // true
int firstIdx = clean.indexOf("a");              // 1
int lastIdx = clean.lastIndexOf("a");           // 3

// 3. Изрязване и заместване
String sub = clean.substring(7, 13);            // "Spring" (от 7 включително до 13 изключително)
String replaced = clean.replace("Boot", "MVC"); // "Java & Spring MVC"
```

---

## 2. ПЪЛЕН ШАБЛОН ЗА РАБОТА СЪС `STRINGBUILDER`

```java
StringBuilder sb = new StringBuilder("Hello");

// 1. Добавяне в края (Chainable)
sb.append(" ").append("World").append("!");     // "Hello World!"

// 2. Вмъкване на конкретен индекс
sb.insert(5, ",");                              // "Hello, World!"

// 3. Замяна на диапазон (от индекс 7 до 12)
sb.replace(7, 12, "Java");                      // "Hello, Java!"

// 4. Изтриване на диапазон или единичен символ
sb.delete(5, 6);                                // Премахва запетаята -> "Hello Java!"
sb.deleteCharAt(sb.length() - 1);               // Премахва '!' -> "Hello Java"

// 5. Обръщане на текста наобратно на място
sb.reverse();                                   // "avaJ olleH"

// 6. Финално превръщане в обикновен String
String finalStr = sb.toString();
```

---

## 3. МОДЕРНО ФОРМАТИРАНЕ И TEXT BLOCKS (`"""` В JAVA 15+)

```java
// 1. Методът .formatted() (по-четим заместител на String.format)
String greeting = "Здравей, %s! Имаш %d нови съобщения.".formatted("Елизабет", 3);

// 2. Text Blocks (многоредови JSON, SQL или HTML шаблони без екраниране на кавички!)
String jsonPayload = """
        {
            "user": "%s",
            "role": "ADMIN",
            "active": true
        }
        """.formatted("elizabeth");
```

---

## 4. ПРЕВРЪЩАНЕ МЕЖДУ `STRING` И `CHAR[]` (ЗА МАНИПУЛАЦИЯ НА ОТДЕЛНИ БУКВИ)

```java
String word = "algorithm";
char[] letters = word.toCharArray(); // Превръща в изменяем масив от символи

letters = Character.toUpperCase(letters);
String capitalized = new String(letters); // "Algorithm"
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод | Клас | Какво прави |
| :--- | :--- | :--- |
| **`s.substring(start, end)`** | `String` | Връща подниз от `start` (вкл.) до `end` (изкл.) |
| **`s.replace(old, new)`** | `String` | Заменя всички срещания на буквалния текст `old` |
| **`s.replaceAll(regex, new)`** | `String` | Заменя всички съвпадения по **регулярен израз** |
| **`s.isEmpty()` / `s.isBlank()`** | `String` | `""` ➔ `true` / `"   "` ➔ `isBlank()` е `true` |
| **`String.join(", ", list)`** | `String` | Съединява колекция от низове с разделител |
| **`sb.append(val)`** | `StringBuilder` | Добавя всякаква стойност в края за $O(1)$ |
| **`sb.setCharAt(idx, ch)`** | `StringBuilder` | Променя символ на конкретен индекс |
| **`sb.setLength(0)`** | `StringBuilder` | Изчиства буфера за повторна употреба без нова алокация |

---
[⬅️ 01-09-00. Теория & Капани](01-09-00-Text-Processing.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-10-00. Regular Expressions (Regex)](01-10-00-Regular-Expressions.md)
