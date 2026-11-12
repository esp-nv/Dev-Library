[⬅️ 02-08-00. Теория & Капани](02-08-00-Exceptions-Handling.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-09-00. Unit Testing (JUnit 5 & Mockito)](02-09-00-Unit-Testing.md)

---

# 02-08-01. Exceptions and Error Handling: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за `try-catch-finally`, Multi-Catch, `try-with-resources` и създаване на собствени бизнес изключения (Custom Exceptions) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Пълен шаблон за `try - catch - finally` и Multi-Catch](#1-пълен-шаблон-за-try---catch---finally-и-multi-catch)
* [2. Автоматично затваряне на ресурси (`try-with-resources`)](#2-автоматично-затваряне-на-ресурси-try-with-resources)
* [3. Създаване на собствени изключения (Custom Checked & Unchecked Exceptions)](#3-създаване-на-собствени-изключения-custom-checked--unchecked-exceptions)
* [4. Най-често използваните вградени изключения в Java](#4-най-често-използваните-вградени-изключения-в-java)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПЪЛЕН ШАБЛОН ЗА `TRY - CATCH - FINALLY` И MULTI-CATCH

*Правило за подредба на `catch` блоковете: Винаги поставяй **най-конкретните (подкласовете)** отгоре, а **най-общите (`Exception`)** най-отдолу, иначе компилаторът дава грешка за недостижим код!*

```java
import java.io.FileNotFoundException;
import java.io.IOException;

public class TryCatchOrderDemo {
    public static void parseAndProcess(String input) {
        try {
            int number = Integer.parseInt(input);
            System.out.println(100 / number);
        } catch (NumberFormatException | ArithmeticException e) {
            // Multi-catch: обработва два несвързани типа по един и същ начин
            System.err.println("Невалиден вход или делене на нула: " + e.getMessage());
        } catch (Exception e) {
            // Общ предпазен блок най-отдолу
            System.err.println("Неочаквана грешка: " + e);
        } finally {
            // Изпълнява се ВИНАГИ (независимо дали е имало грешка или не)
            System.out.println("Край на проверката.");
        }
    }
}
```

---

## 2. АВТОМАТИЧНО ЗАТВАРЯНЕ НА РЕСУРСИ (`TRY-WITH-RESOURCES`)

От Java 9+ можеш да подадеш в `try (...)` дори вече създадена `effectively final` променлива:

```java
import java.io.PrintWriter;
import java.util.Scanner;

public class TryWithResourcesDemo {
    public static void copyConsoleToFile(String fileName) {
        try (
            Scanner scanner = new Scanner(System.in);
            PrintWriter writer = new PrintWriter(fileName)
        ) {
            String line = scanner.nextLine();
            writer.println(line);
        } catch (Exception e) {
            e.printStackTrace();
        }
        // Тук и writer.close(), и scanner.close() вече са извикани автоматично!
    }
}
```

---

## 3. СЪЗДАВАНЕ НА СОБСТВЕНИ ИЗКЛЮЧЕНИЯ (CUSTOM CHECKED & UNCHECKED EXCEPTIONS)

В модерни Spring Boot приложения почти винаги наследяваме **`RuntimeException`** (Unchecked):

```java
// 1. Собствено Unchecked бизнес изключение
public class InsufficientFundsException extends RuntimeException {
    private final double missingAmount;

    public InsufficientFundsException(String message, double missingAmount) {
        super(message);
        this.missingAmount = missingAmount;
    }

    public double getMissingAmount() {
        return missingAmount;
    }
}

// 2. Използване в бизнес логиката
public class Wallet {
    private double balance = 50.0;

    public void pay(double amount) {
        if (amount > balance) {
            throw new InsufficientFundsException("Недостатъчна наличност!", amount - balance);
        }
        balance -= amount;
    }
}
```

---

## 4. НАЙ-ЧЕСТО ИЗПОЛЗВАНИТЕ ВГРАДЕНИ ИЗКЛЮЧЕНИЯ В JAVA

| Клас на изключението | Тип | Кога се хвърля |
| :--- | :--- | :--- |
| **`NullPointerException`** | Unchecked | Опит за достъп до метод/поле на `null` референция |
| **`IllegalArgumentException`** | Unchecked | Подаден невалиден аргумент на метод (напр. отрицателна възраст) |
| **`IllegalStateException`** | Unchecked | Обектът не е в правилно състояние за операцията (напр. затворен Stream) |
| **`IndexOutOfBoundsException`** | Unchecked | Невалиден индекс в масив, списък или `String` |
| **`NumberFormatException`** | Unchecked | `Integer.parseInt("abc")` (наследява `IllegalArgumentException`) |
| **`UnsupportedOperationException`** | Unchecked | Опит за `.add()` / `.remove()` върху `List.of(...)` |
| **`IOException` / `SQLException`** | **Checked** | Грешки при четене/писане на файлове, мрежа или база данни |

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Метод | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Хвърляне на изключение** | `throw new IllegalArgumentException("msg");` | Прекратява текущия метод веднага |
| **Обявяване на Checked грешка** | `public void load() throws IOException` | Задължава викащия метод да я обработи |
| **Обвиване (Exception Chaining)** | `throw new AppException("msg", cause);` | Винаги подавай оригиналния `cause`! |
| **Автоматично затваряне** | `try (var conn = getConnection()) { }` | Изисква ресурсът да е `AutoCloseable` |
| **Четене на съобщението** | `e.getMessage()` / `e.printStackTrace()` | В продукция ползвай Logger (`log.error("...", e)`) |

---
[⬅️ 02-08-00. Теория & Капани](02-08-00-Exceptions-Handling.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-09-00. Unit Testing (JUnit 5 & Mockito)](02-09-00-Unit-Testing.md)
