[⬅️ 01-03. Arrays](01-03-00-Arrays.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-04-01. Syntax From Scratch](01-04-01-Syntax-From-Scratch.md)

---

# 01-04-00. Methods (Теория, Stack Frames, Overloading & Капани)

**Модул 01:** Java Fundamentals | **Лекционна тема 04:** Дефиниране и извикване на методи в Java, стекови рамки (Call Stack Frames), претоварване на методи (Method Overloading), променлив брой аргументи (`varargs`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Жизнен цикъл на извикване на метод (Call Stack Блок-схема)](#2-жизнен-цикъл-на-извикване-на-метод-call-stack-блок-схема)
* [3. Сравнителна таблица: Java Methods срещу C#, Python и JavaScript](#3-сравнителна-таблица-java-methods-срещу-c-python-и-javascript)
* [4. Плюсове и минуси на Method Overloading в Java](#4-плюсове-и-минуси-на-method-overloading-в-java)
* [5. Кодов одит: Правилен срещу Грешен код (Overloading само по връщан тип и `varargs`)](#5-кодов-одит-правилен-срещу-грешен-код-overloading-само-по-връщан-тип-и-varargs)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Сигнатура на метода (Method Signature = Име + Типове параметри):** Компилаторът на Java разпознава методите **единствено по тяхното име и списъка с типове на параметрите** (напр. `print(int)` срещу `print(String)`). Връщаният тип (`void`, `int`, `String`) и имената на параметрите **не влизат в сигнатурата**!
*   **Няма стойности по подразбиране (Default Parameters) като в C# и Python:** В Java не можеш да напишеш `void greet(String name = "Guest")`. Вместо това правиш **Method Overloading** — два метода с еднакво име, където по-късият извиква по-дългия със стойността по подразбиране (`greet("Guest")`).

---

## 2. ЖИЗНЕН ЦИКЪЛ НА ИЗВИКВАНЕ НА МЕТОД (CALL STACK БЛОК-СХЕМА)

Всяко извикване на метод създава нова **Стекова рамка (Stack Frame)** в паметта на нишката (LIFO — Last-In, First-Out):

```text
main() извиква calculatePrice(100, 0.20) ➔ който извиква round(120.0)

3. ┌────────────────────────────────────────┐ ◄── Връх на стека (Активен метод)
   │ round(double val) -> връща 120.00      │     (След return рамката се унищожава!)
2. ├────────────────────────────────────────┤
   │ calculatePrice(100, 0.20)              │     (Пази локалните променливи price, tax)
1. ├────────────────────────────────────────┤
   │ main(String[] args)                    │     (Дъно на стека на нишката)
   └────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA METHODS СРЕЩУ C#, PYTHON И JAVASCRIPT

| Концепция / Функция | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Дефиниране на метод** | `static int sum(int a, int b)` | `static int Sum(int a, int b)` | `def sum(a: int, b: int) -> int:` | `function sum(a, b) {}` |
| **Именна конвенция** | **`camelCase`** (`getUserById`) | **`PascalCase`** (`GetUserById`) | **`snake_case`** (`get_user_by_id`) | **`camelCase`** (`getUserById`) |
| **Метод без връщан резултат** | `void log(String msg)` | `void Log(string msg)` | `def log(msg: str) -> None:` | `function log(msg) {}` |
| **Default стойности** | **Няма** (ползва се Overloading) | `void Log(string msg = "Hi")` | `def log(msg: str = "Hi"):` | `function log(msg = "Hi") {}` |
| **Променлив брой аргументи** | `void printAll(String... items)` | `void PrintAll(params string[] items)` | `def print_all(*items):` | `function printAll(...items) {}` |
| **Предаване по референция** | **Няма** (само Pass-by-Value) | `ref int x`, `out int y` | Няма (връща се `tuple`) | Няма (връща се обект/масив) |
| **Method Overloading** | **Да** (по брой/тип параметри) | **Да** (по брой/тип параметри) | **Не** (презаписва функцията) | **Не** (презаписва функцията) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА METHOD OVERLOADING В JAVA

### 🟢 Предимства (Pros):
*   **Единно и предвидимо API:** Програмистът запомня само едно име на метод (например `System.out.println(...)`), който работи безпроблемно за `int`, `double`, `boolean`, `char[]` и `Object`.
*   **Статично свързване (Static Binding / Early Binding):** Компилаторът избира точната версия на претоварения метод още по време на компилация, което гарантира нулево забавяне по време на изпълнение.

### 🔴 Недостатъци (Cons):
*   **Многословен код при много опционални параметри:** Липсата на Default и Named аргументи (`createUser(name="Ivan", isAdmin=true)`) принуждава разработчиците да пишат по 4–5 претоварени конструктора или да използват **Builder Pattern**.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (OVERLOADING САМО ПО ВРЪЩАН ТИП И `VARARGS`)

### ❌ ГРЕШЕН КОД (Overloading само по връщан тип и грешна позиция на `varargs`):
```java
public class BadOverloading {
    // 🚨 ГРЕШКА 1: Идентична сигнатура 'calculate(int, int)'!
    // Компилаторът не гледа връщания тип (int срещу double) -> Compile-Time Error!
    public static int calculate(int a, int b) { return a + b; }
    public static double calculate(int a, int b) { return a + b; }

    // 🚨 ГРЕШКА 2: Varargs параметърът НЕ Е последен! -> Compile-Time Error!
    public static void logMessages(String... messages, int code) { }
}
```
*   **Как да се избегне:** Претоварените методи **задължително** трябва да се различават по **брой, тип или ред на параметрите**. Параметърът с многоточие (`Type... args`) винаги трябва да е **единствен и последен** в списъка с параметри.

### 🟢 ПРАВИЛЕН КОД (Чисто претоварване за симулиране на Default стойности):
```java
public class CleanMethodsDemo {
    // Пълен метод с всички параметри
    public static void printReceipt(String item, int quantity, String currency) {
        System.out.printf("Артикул: %s | Брой: %d | Валута: %s%n", item, quantity, currency);
    }

    // Претоварен метод със стойност по подразбиране за валутата ("BGN")
    public static void printReceipt(String item, int quantity) {
        printReceipt(item, quantity, "BGN");
    }

    public static void main(String[] args) {
        printReceipt("Лаптоп", 1);          // Използва "BGN"
        printReceipt("Монитор", 2, "EUR");  // Използва "EUR"
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Ако имаме два претоварени метода — `void test(int a, long b)` и `void test(long a, int b)` — какво ще се случи при извикване на `test(10, 20)`?“*
*   **Уловката:** Кандидатите често гадаят кой от двата метода ще избере JVM.
*   **Точен Senior отговор:** Кодът **няма да се компилира** и ще хвърли грешка **`Reference to test is ambiguous`**! Причината е, че числата `10` и `20` са от тип `int`, така че и двата метода изискват точно едно автоматично разширяване (`int -> long`). Тъй като нито единият метод не е по-специфичен от другия, компилаторът отказва да гадае и изисква изрично уточняване от програмиста чрез суфикс `L`: `test(10, 20L)`.

---
[⬅️ 01-03. Arrays](01-03-00-Arrays.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-04-01. Syntax From Scratch](01-04-01-Syntax-From-Scratch.md)
