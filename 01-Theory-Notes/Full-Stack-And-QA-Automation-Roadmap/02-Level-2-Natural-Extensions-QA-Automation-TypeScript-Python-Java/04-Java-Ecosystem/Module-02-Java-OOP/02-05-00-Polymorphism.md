[⬅️ 02-04. Interfaces & Abstract Classes](02-04-00-Interfaces-And-Abstract-Classes.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-05-01. Syntax From Scratch](02-05-01-Syntax-From-Scratch.md)

---

# 02-05-00. Polymorphism (Теория, Dynamic Dispatch, `instanceof` Pattern Matching & `sealed`)

**Модул 02:** Java OOP | **Лекционна тема 05:** Полиморфизъм по време на компилация (Compile-time Overloading) срещу полиморфизъм по време на изпълнение (Runtime Overriding / Dynamic Method Dispatch), Upcasting и Downcasting, модерен Pattern Matching за `instanceof` (Java 16+) и `switch` (Java 21+), запечатани класове (`sealed` classes — Java 17+) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Механизъм на Dynamic Method Dispatch (vtable Блок-схема)](#2-механизъм-на-dynamic-method-dispatch-vtable-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на Полиморфизма и `sealed` класовете](#4-плюсове-и-минуси-на-полиморфизма-и-sealed-класовете)
* [5. Кодов одит: Правилен срещу Грешен код (`ClassCastException` и скриване на статични методи)](#5-кодов-одит-правилен-срещу-грешен-код-classcastexception-и-скриване-на-статични-методи)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Универсално дистанционно управление (Polymorphism = „Много форми“):** Натискаш един и същ бутон **`play()`**, но ако дистанционното сочи към телевизор, се пуска филм; ако сочи към уредба, тръгва музика.
*   **Лявата страна решава КАКВО виждаш, Дясната страна решава КАКВО се изпълнява:**
    *   `Animal pet = new Dog();`
    *   **Ляво (`Animal`):** Типът на референцията определя **кои методи можеш да извикаш** по време на компилация (`pet.eat()` работи, но `pet.bark()` не се компилира без проверка!).
    *   **Дясно (`new Dog()`):** Реалният обект в Heap паметта определя **коя версия на метода ще се изпълни** по време на работа (ако `Dog` има `@Override eat()`, изпълнява се версията на `Dog`).

---

## 2. МЕХАНИЗЪМ НА DYNAMIC METHOD DISPATCH (VTABLE БЛОК-СХЕМА)

Когато извикаш `animal.makeSound()`, JVM поглежда реалния обект в Heap паметта и използва неговата **Virtual Method Table (vtable)**:

```text
STACK ПАМЕТ                     HEAP ПАМЕТ
┌──────────────────────┐        ┌──────────────────────────────────────────┐
│ Animal a ────────────┼───────►│ Обект Cat (Реален тип: Cat)              │
└──────────────────────┘        │ • Klass Pointer ──► vtable на Cat:       │
                                │     - makeSound() ──► Cat.makeSound() ✅ │
                                │     - sleep()     ──► Animal.sleep()     │
                                └──────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Концепция | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Проверка на тип + каст** | `if (obj instanceof Dog d)` | `if (obj is Dog d)` | `if isinstance(obj, Dog):` | `if (obj instanceof Dog)` |
| **Pattern Matching `switch`** | `switch (shape) { case Circle c -> ... }` | `shape switch { Circle c => ... }` | `match shape: case Circle(): ...` | Няма вграден |
| **Ограничено наследяване** | `sealed class Shape permits Circle` | Няма директен еквивалент | Няма | Няма |
| **Динамично свързване** | Всички нестатични методи (vtable) | Само `virtual` / `override` методи | Всички методи (Duck Typing) | Прототипна верига |
| **Принудителен Downcast** | `Dog d = (Dog) animal;` | `Dog d = (Dog)animal;` или `as Dog` | Не е нужен (динамично) | Не е нужен (динамично) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ПОЛИМОРФИЗМА И `SEALED` КЛАСОВЕТЕ

### 🟢 Предимства (Pros):
*   **Разширяемост без промяна на съществуващия код (Open/Closed Principle):** Можеш да добавиш нов клас `CryptoPayment implements Payment`, без да пипаш нито един ред от платежния процесор.
*   **Изчерпателност при `sealed` класове (Java 17+):** Когато ограничиш наследниците със `sealed`, компилаторът на Java знае всички възможни подтипове и в `switch` изразите **не изисква `default` клон**, стига да си покрил всички разрешени класове!

### 🔴 Недостатъци (Cons):
*   **Риск от `ClassCastException` при сляп Downcasting:** Ако насила преобразуваш базов тип към грешен наследник без проверка с `instanceof`, програмата гърми по време на изпълнение.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`CLASSCASTEXCEPTION` И СКРИВАНЕ НА СТАТИЧНИ МЕТОДИ)

### ❌ ГРЕШЕН КОД (Сляп Downcast и опит за полиморфизъм върху `static` метод):
```java
class Parent {
    static void greet() { System.out.println("Hello from Parent"); }
}

class Child extends Parent {
    static void greet() { System.out.println("Hello from Child"); }
}

public class BadPolymorphism {
    public static void main(String[] args) {
        // 🚨 ГРЕШКА 1: Статичните методи НЕ са полиморфни (Static Binding)!
        // Въпреки че обектът е new Child(), Java гледа типа на референцията (Parent)
        // и отпечатва "Hello from Parent"!
        Parent p = new Child();
        p.greet();

        // 🚨 ГРЕШКА 2: Сляп Downcast без проверка -> хвърля ClassCastException!
        Object text = "Java";
        Integer num = (Integer) text;
    }
}
```
*   **Как да се избегне:** Никога не пренаписвай `static` методи с очакване за полиморфизъм. Винаги комбинирай проверката и преобразуването чрез **Pattern Matching за `instanceof`** (`if (obj instanceof String s)`).

### 🟢 ПРАВИЛЕН КОД (Модерен Pattern Matching за `instanceof` и `sealed` йерархия):
```java
public sealed interface Shape permits Circle, Rectangle {}
public record Circle(double radius) implements Shape {}
public record Rectangle(double width, double height) implements Shape {}

public class CleanPolymorphism {
    public static double calculateArea(Shape shape) {
        // Изчерпателен Pattern Matching Switch (Java 21+) — без нужда от default!
        return switch (shape) {
            case Circle c -> Math.PI * c.radius() * c.radius();
            case Rectangle r -> r.width() * r.height();
        };
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Ако `Animal a = new Dog();`, а в двата класа имаме поле със същото име (`String type = "Animal"` в `Animal` и `String type = "Dog"` в `Dog`), какво ще отпечата `System.out.println(a.type)`?“*
*   **Уловката:** Кандидатите знаят, че методите в Java се резолват по реалния обект (`Dog`), и автоматично смятат, че същото важи и за полетата!
*   **Точен Senior отговор:** Ще отпечата **`"Animal"`**! В Java **полиморфизмът важи САМО за нестатични методи, но НЕ и за полета (Variables are hidden, not overridden)**! Достъпът до полета се решава още по време на компилация според **типа на референцията отляво (`Animal`)**. Това е още една причина полетата винаги да са `private` и да се четат през методи (`getType()`), които са полиморфни!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ (JVM) | 🟣 C# 12+ (.NET 8+ CLR) |
| :--- | :--- | :--- |
| **Проверка за тип и каст** | `if (obj instanceof Dog d)` (Java 16+) | `if (obj is Dog d)` (C# 7+) |
| **Безопасен каст без изключение** | Няма директен оператор `as` (ползва се `instanceof`) | `Dog? d = obj as Dog;` (връща `null` при неуспех) |
| **Забрана за наследяване** | `final class MyClass` | `sealed class MyClass` |
| **Алгебрични типове (`sealed`)** | `sealed class Shape permits Circle, Square` (поддържа се нативно!) | Няма нативен `permits` (симулира се с `private` конструктор и вложени типове) |
| **Guard условия в `switch`** | `case String s when s.length() > 5 -> ...` (Java 21+) | `case string s when s.Length > 5 => ...` (C# 7+) |
| **Полиморфизъм по подразбиране** | Да (всички нестатични, не-`final` методи) | Не (изисква изрични `virtual` и `override`) |

---
[⬅️ 02-04. Interfaces & Abstract Classes](02-04-00-Interfaces-And-Abstract-Classes.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-05-01. Syntax From Scratch](02-05-01-Syntax-From-Scratch.md)
