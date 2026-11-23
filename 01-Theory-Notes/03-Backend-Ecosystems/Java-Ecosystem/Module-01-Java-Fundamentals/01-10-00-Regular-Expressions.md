[⬅️ 01-09. Text Processing](01-09-00-Text-Processing.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-10-01. Syntax From Scratch](01-10-01-Syntax-From-Scratch.md)

---

# 01-10-00. Regular Expressions (Regex — Теория, `Pattern` & `Matcher` & Капани)

**Модул 01:** Java Fundamentals | **Лекционна тема 10:** Регулярни изрази в Java (`java.util.regex.Pattern` и `Matcher`), двойно екраниране на обратната наклонена черта (`\\d`, `\\w`, `\\s`), именувани групи (`(?<name>...)`), алчно срещу мързеливо търсене (Greedy vs Lazy) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Regex двигателя в Java (`Pattern` ➔ `Matcher` Блок-схема)](#2-архитектура-на-regex-двигателя-в-java-pattern--matcher-блок-схема)
* [3. Сравнителна таблица: Java Regex срещу C#, Python и JavaScript](#3-сравнителна-таблица-java-regex-срещу-c-python-и-javascript)
* [4. Плюсове и минуси на `Pattern.compile()` срещу `String.matches()`](#4-плюсове-и-минуси-на-patterncompile-срещу-stringmatches)
* [5. Кодов одит: Правилен срещу Грешен код (Забравено `matcher.find()` и компилация в цикъл)](#5-кодов-одит-правилен-срещу-грешен-код-забравено-matcherfind-и-компилация-в-цикъл)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Двойният слеш (`\\`) в Java (Защо няма Raw Strings `r"..."` или `@"..."`?):** В C# пишеш `@"\d+"`, в Python пишеш `r"\d+"`, в JavaScript пишеш `/\d+/`. В Java **няма специален префикс за Regex низове** — затова за да стигне символът `\d` до Regex двигателя, първо трябва да екранираш самата обратна наклонена черта за Java компилатора: **`"\\d+"`**.
*   **`Pattern` срещу `Matcher` (Матрица за печат срещу Валик):**
    *   **`Pattern`:** Компилираният шаблон (металната матрица). Създава се **веднъж** чрез `Pattern.compile(regex)` и е напълно Thread-Safe.
    *   **`Matcher`:** Работният инструмент за конкретен текст (`pattern.matcher(text)`). Той пази вътрешен курсор и преминава от съвпадение на съвпадение при всяко извикване на `matcher.find()`.

---

## 2. АРХИТЕКТУРА НА REGEX ДВИГАТЕЛЯ В JAVA (`PATTERN` ➔ `MATCHER` БЛОК-СХЕМА)

```text
1. String regex = "(?<city>[A-Z][a-z]+): (?<temp>\\d+)C";
                         │
                         ▼  Pattern.compile(regex) (Скъпа операция - прави се 1 път!)
2. ┌──────────────────────────────────────────────────────────────────────┐
   │ Pattern обект (Компилиран краен автомат / State Machine в RAM)       │
   └─────────────────────────────────┬────────────────────────────────────┘
                                     │  pattern.matcher("Sofia: 24C, Vidin: 27C")
                                     ▼
3. ┌──────────────────────────────────────────────────────────────────────┐
   │ Matcher обект (Държи курсора върху текста)                           │
   │ • while (matcher.find()) ➔ Мести курсора към следващото съвпадение   │
   │ • matcher.group("city")  ➔ Връща "Sofia", после "Vidin"              │
   └──────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA REGEX СРЕЩУ C#, PYTHON И JAVASCRIPT

| Концепция / Действие | ☕ Java 21+ (`java.util.regex`) | 🟣 C# 12+ (`System.Text.RegularExpressions`) | 🐍 Python 3.x (`re`) | 🟡 JavaScript (`RegExp`) |
| :--- | :--- | :--- | :--- | :--- |
| **Запис на шаблон (цифри)** | `"\\d+"` (двоен слеш!) | `@"\d+"` (Verbatim string) | `r"\d+"` (Raw string) | `/\d+/g` (Regex литерал) |
| **Компилиране на шаблон** | `Pattern.compile(regex)` | `new Regex(regex)` | `re.compile(regex)` | `new RegExp(regex, "g")` |
| **Проверка на ЦЕЛИЯ низ** | `str.matches(regex)` | `Regex.IsMatch(str, "^...$")` | `bool(re.fullmatch(r, s))` | `/^...$/.test(str)` |
| **Именувана група (Syntax)** | `(?<name>\\w+)` | `(?<name>\w+)` | `(?P<name>\w+)` | `(?<name>\w+)` |
| **Четене на именувана група** | `matcher.group("name")` | `match.Groups["name"].Value` | `match.group("name")` | `match.groups.name` |
| **Заместване по Regex** | `str.replaceAll(regex, rep)` | `Regex.Replace(str, r, rep)` | `re.sub(r, rep, str)` | `str.replaceAll(/.../g, rep)` |
| **Case-insensitive флаг** | `Pattern.CASE_INSENSITIVE` | `RegexOptions.IgnoreCase` | `re.IGNORECASE` (`re.I`) | `/pattern/i` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА `PATTERN.COMPILE()` СРЕЩУ `STRING.MATCHES()`

### 🟢 Предимства на `Pattern.compile()` (Pros):
*   **10x–50x по-висока производителност в цикли:** Когато декларираш `private static final Pattern EMAIL_PATTERN = Pattern.compile(...)` като константа на класа, крайният автомат се строи само веднъж при зареждане на програмата.
*   **Достъп до групи и позиции:** Само чрез `Matcher` можеш да извличаш отделни части от съвпадението (`matcher.group("year")`) и началния/крайния им индекс (`matcher.start()`, `matcher.end()`).

### 🔴 Недостатъци на `String.matches()` (Cons):
*   **Скрита компилация при всяко извикване:** Под капака `str.matches(regex)` вика `Pattern.matches(regex, str)`, което **компилира шаблона от нулата при всеки един ред**! В цикъл от 50 000 реда това забавя програмата драстично.
*   **Сравнява само ЦЕЛИЯ низ:** За разлика от JavaScript `regex.test(str)` (който връща `true`, ако намери съвпадение *където и да е* в текста), в Java `str.matches("\\d+")` връща `true` **само ако целият низ от първия до последния символ е цифри**!

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ЗАБРАВЕНО `MATCHER.FIND()` И КОМПИЛАЦИЯ В ЦИКЪЛ)

### ❌ ГРЕШЕН КОД (Викане на `matcher.group()` преди `matcher.find()` и Greedy капан):
```java
Pattern pattern = Pattern.compile("<tag>(.*)</tag>");
Matcher matcher = pattern.matcher("<tag>First</tag> and <tag>Second</tag>");

// 🚨 ГРЕШКА 1: Викаш matcher.group(1) БЕЗ преди това да си извикала matcher.find()!
// Хвърля IllegalStateException: No match found!
System.out.println(matcher.group(1));

// 🚨 ГРЕШКА 2 (Greedy капан): '.*' е алчен и поглъща всичко от първия <tag> до ПОСЛЕДНИЯ </tag>!
// Резултатът ще бъде "First</tag> and <tag>Second" вместо два отделни тага!
```
*   **Как да се избегне:** **Никога** не викай `matcher.group()` извън `if (matcher.find())` или `while (matcher.find())`. За да спреш алчността на квантификаторите (`*`, `+`), добави въпросителен знак отзад (**`.*?`** или **`+?`** — *Lazy / Reluctant quantifier*).

### 🟢 ПРАВИЛЕН КОД (Предкомпилиран `Pattern`, Lazy квантификатор и именувани групи):
```java
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class CleanRegexDemo {
    // Предкомпилираме шаблона веднъж като константа на класа
    private static final Pattern TAG_PATTERN = Pattern.compile("<tag>(?<content>.*?)</tag>");

    public static void main(String[] args) {
        String input = "<tag>First</tag> and <tag>Second</tag>";
        Matcher matcher = TAG_PATTERN.matcher(input);

        while (matcher.find()) {
            System.out.println("Намерено съдържание: " + matcher.group("content"));
        }
        // Изход:
        // Намерено съдържание: First
        // Намерено съдържание: Second
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво представлява атаката **ReDoS (Regular Expression Denial of Service)** и какви регуляри изрази я предизвикват в Java (и в други езици с Backtracking двигател)?“*
*   **Уловката:** Кандидатите често пишат вложени квантификатори като `(a+)+$` за валидация на потребителски вход, без да подозират, че 30 символа от потребителя могат да забият сървъра за часове!
*   **Точен Senior отговор:** Стандартният Regex двигател в Java (`java.util.regex`) използва алгоритъм с връщане назад (**NFA Backtracking**). Когато шаблонът съдържа **вложени двусмислени квантификатори** (напр. `(a+)+$` или `([a-zA-Z]+)*$`) и получи невалиден вход като `"aaaaaaaaaaaaaaaaaaaaaaaaaaaa!"` (завършващ на `!`), двигателят изпробва **$2^N$ комбинации** от разбивания на буквите `'a'`, преди да се откаже (**Catastrophic Backtracking**), което удря 100% натоварване на процесора (**ReDoS атака**).
    *   *Решение:* Избягвай вложени квантификатори `(x+)+` или използвай **Possessive Quantifiers** (`a++`), които забраняват на двигателя да се връща назад при неуспех.

---
[⬅️ 01-09. Text Processing](01-09-00-Text-Processing.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-10-01. Syntax From Scratch](01-10-01-Syntax-From-Scratch.md)
