[⬅️ 01-04-00. Теория & Капани](01-04-00-Methods.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-05-00. Lists](01-05-00-Lists.md)

---

# 01-04-01. Methods: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на `void` и връщащи стойност методи, `Method Overloading`, `varargs` и връщане на множество стойности в Java от нулата.

## 🧭 Съдържание на темата
* [1. Анатомия на един Java метод (Модификатори, Връщан тип, Сигнатура)](#1-анатомия-на-един-java-метод-модификатори-връщан-тип-сигнатура)
* [2. `void` методи и ранно излизане с `return;`](#2-void-методи-и-ранно-излизане-с-return)
* [3. Променлив брой аргументи (`varargs`)](#3-променлив-брой-аргументи-varargs)
* [4. Как да върнем две или повече стойности от метод в Java?](#4-как-да-върнем-две-или-повече-стойности-от-метод-в-java)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. АНАТОМИЯ НА ЕДИН JAVA МЕТОД (МОДИФИКАТОРИ, ВРЪЩАН ТИП, СИГНАТУРА)

```java
public class MathUtils {
    // [Модификатор за достъп] [static] [Връщан тип] [имеНаМетод]([Параметри])
    public static double calculateRectangleArea(double width, double height) {
        return width * height;
    }

    public static void main(String[] args) {
        double area = calculateRectangleArea(5.5, 4.0);
        System.out.printf("Лице: %.2f%n", area); // 22.00
    }
}
```

---

## 2. `VOID` МЕТОДИ И РАННО ИЗЛИЗАНЕ С `RETURN;`

При `void` методите ключовата дума `return;` (без стойност) прекратява изпълнението на метода веднага (Guard Clause):

```java
public static void printPositiveNumber(int number) {
    if (number <= 0) {
        System.out.println("Числото не е положително.");
        return; // Излиза веднага от метода
    }
    System.out.println("Валидно число: " + number);
}
```

---

## 3. ПРОМЕНЛИВ БРОЙ АРГУМЕНТИ (`VARARGS`)

Под капака Java превръща `int... numbers` в обикновен масив `int[] numbers`:

```java
public static int findMax(int first, int... rest) {
    int max = first;
    for (int num : rest) {
        if (num > max) {
            max = num;
        }
    }
    return max;
}

// Извикване с произволен брой аргументи (поне 1 заради 'int first'):
int biggest = findMax(10, 45, 3, 99, 21); // 99
```

---

## 4. КАК ДА ВЪРНЕМ ДВЕ ИЛИ ПОВЕЧЕ СТОЙНОСТИ ОТ МЕТОД В JAVA?

Тъй като в Java няма `out` параметри и вградени `(int, string)` кортежи като в C#, най-чистият модерен начин (от Java 16+) е локален **`record`**:

```java
public class MultiReturnDemo {
    // Дефинираме олекотен record за резултата
    record MinMax(int min, int max) {}

    public static MinMax getMinMax(int[] arr) {
        int min = arr;
        int max = arr;
        for (int n : arr) {
            if (n < min) min = n;
            if (n > max) max = n;
        }
        return new MinMax(min, max);
    }

    public static void main(String[] args) {
        MinMax result = getMinMax(new int[]{15, 3, 42, 8});
        System.out.printf("Мин: %d, Макс: %d%n", result.min(), result.max());
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java | Аналог в C# | Аналог в Python |
| :--- | :--- | :--- | :--- |
| **Статичен метод** | `public static void run()` | `public static void Run()` | `@staticmethod def run():` |
| **Променлив брой аргументи** | `void fn(String... args)` | `void Fn(params string[] args)` | `def fn(*args):` |
| **Симулиране на Default параметър** | Претоварен метод `fn(a)` ➔ `fn(a, 10)` | `void Fn(int a, int b = 10)` | `def fn(a, b=10):` |
| **Връщане на 2 стойности** | `record Pair(int a, int b) {}` | `(int a, int b) Fn()` (ValueTuple) | `return a, b` (`tuple`) |
| **Ранно прекратяване на `void`** | `if (invalid) return;` | `if (invalid) return;` | `if invalid: return` |

---
[⬅️ 01-04-00. Теория & Капани](01-04-00-Methods.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-05-00. Lists](01-05-00-Lists.md)
