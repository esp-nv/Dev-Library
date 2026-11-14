[⬅️ 02-07. Reflection & Annotations](02-07-00-Reflection-And-Annotations.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-08-01. Syntax From Scratch](02-08-01-Syntax-From-Scratch.md)

---

# 02-08-00. Exceptions and Error Handling (Теория, Checked vs Unchecked & Капани)

**Модул 02:** Java OOP | **Лекционна тема 08:** Йерархията на `Throwable` (`Error` срещу `Exception`), ключовата разлика в Java между **Checked Exceptions** (проверявани при компилация) и **Unchecked / Runtime Exceptions**, автоматично затваряне на ресурси (`try-with-resources`), създаване на собствени изключения и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Йерархия на Изключенията в Java (`Throwable` Блок-схема)](#2-йерархия-на-изключенията-в-java-throwable-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на Checked Exceptions в Java](#4-плюсове-и-минуси-на-checked-exceptions-в-java)
* [5. Кодов одит: Правилен срещу Грешен код (Поглъщане на изключения и `return` във `finally`)](#5-кодов-одит-правилен-срещу-грешен-код-поглъщане-на-изключения-и-return-във-finally)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ срещу C# 12+ (.NET)](#7-архитектурно-сравнение-java-21-срещу-c-12-net)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трите клона на `Throwable`:**
    1.  **`Error` (Пожар в сървърната зала — напр. `OutOfMemoryError`, `StackOverflowError`):** Системни катастрофи в JVM, от които програмата **не може и не трябва** да се възстановява. Никога не ги хващай с `catch (Error e)`!
    2.  **`Checked Exception` (Задължителен колан в колата — наследява `Exception`, но НЕ `RuntimeException`):** Външни предвидими проблеми (липсващ файл `IOException`, паднала база `SQLException`). Компилаторът **не ти позволява да компилираш кода**, докато или не ги хванеш с `try-catch`, или не ги обявиш в сигнатурата на метода с **`throws`**.
    3.  **`Unchecked Exception` (Програмистка грешка — наследява `RuntimeException`):** Бъгове в логиката (`NullPointerException`, `IllegalArgumentException`, `IndexOutOfBoundsException`). Компилаторът **не те кара** да ги обявяваш с `throws` — те се оправят с по-добър код и валидация!
*   **`throw` срещу `throws`:**
    *   **`throw`** (глагол в единствено число) = хвърляш конкретен обект вътре в метода (`throw new IllegalArgumentException("...");`).
    *   **`throws`** (в сигнатурата на метода) = предупреждаваш, че методът *може* да хвърли даден тип изключение (`public void readFile() throws IOException`).

---

## 2. ЙЕРАРХИЯ НА ИЗКЛЮЧЕНИЯТА В JAVA (`THROWABLE` БЛОК-СХЕМА)

```text
                           ┌─────────────────┐
                           │    Throwable    │
                           └────────┬────────┘
                 ┌──────────────────┴──────────────────┐
                 ▼                                     ▼
        ┌─────────────────┐                   ┌─────────────────┐
        │      Error      │                   │    Exception    │
        │ (JVM сривове)   │                   └────────┬────────┘
        │ • OutOfMemory   │             ┌──────────────┴──────────────┐
        │ • StackOverflow │             ▼                             ▼
        └─────────────────┘    ┌─────────────────┐           ┌──────────────────┐
                               │ CHECKED         │           │ RuntimeException │
                               │ (Compile-time!) │           │ (UNCHECKED!)     │
                               │ • IOException   │           │ • NullPointer    │
                               │ • SQLException  │           │ • IllegalArg     │
                               └─────────────────┘           └──────────────────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Концепция | ☕ Java 21+ | 🟣 C# 12+ (.NET) | 🐍 Python 3.x | 🟡 JavaScript (ES6+) |
| :--- | :--- | :--- | :--- | :--- |
| **Блок за улавяне на грешки** | `try { } catch (Ex e) { } finally { }` | `try { } catch (Ex e) { } finally { }` | `try: ... except Ex as e: ... finally:` | `try { } catch (e) { } finally { }` |
| **Хвърляне на изключение** | `throw new IllegalArgumentException()` | `throw new ArgumentException()` | `raise ValueError("...")` | `throw new Error("...")` |
| **Checked Exceptions** | **Да** (задължителни `throws`) | **Не** (всички са Unchecked) | **Не** (динамичен език) | **Не** (динамичен език) |
| **Множествен `catch` на 1 ред** | `catch (IOException \| SQLException e)` | Два отделни `catch` блока | `except (IOError, ValueError) as e:` | Един `catch (e)` с `instanceof` |
| **Автоматично затваряне на ресурс** | `try (var r = new Scanner(...)) { }` | `using var r = new StreamReader(...);` | `with open(...) as f:` | `using r = ...` (ES2024 Explicit Resource) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА CHECKED EXCEPTIONS В JAVA

### 🟢 Предимства (Pros):
*   **Самодокументиращо се API:** Още като погледнеш сигнатурата `void transferMoney() throws InsufficientFundsException`, веднага знаеш какви бизнес грешки трябва да обработиш.

### 🔴 Недостатъци (Cons):
*   **Замърсяване на сигнатурите и несъвместимост със Stream API:** Ако метод A хвърля `IOException`, всички методи B, C и D по веригата нагоре също трябва да добавят `throws IOException`. В модерния Spring Boot и Stream API повечето разработчици предпочитат **Unchecked (`RuntimeException`)** изключения.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ПОГЛЪЩАНЕ НА ИЗКЛЮЧЕНИЯ И `RETURN` ВЪВ `FINALLY`)

### ❌ ГРЕШЕН КОД (Празен `catch` блок и `return` вътре във `finally`):
```java
public int dangerousMethod() {
    try {
        Integer x = null;
        return x.intValue(); // Хвърля NullPointerException!
    } catch (Exception e) {
        // 🚨 ГРЕШКА 1 (Swallowing Exception): Празен catch блок! Грешката изчезва без следа!
    } finally {
        // 🚨 ГРЕШКА 2: Никога не слагай 'return' във finally блок!
        // Той задушава и изтрива всяко хвърлено изключение от try блока!
        return 0;
    }
}
```
*   **Как да се избегне:** Никога не оставяй празен `catch` блок — минимум логни грешката или я обвии в по-смислено бизнес изключение, запазвайки оригиналната причина (`throw new ServiceException("Грешка при четене", e)`). Никога не слагай `return` във `finally`.

### 🟢 ПРАВИЛЕН КОД (`try-with-resources` и верижно обвиване на изключение):
```java
import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

public class SafeFileReader {
    public static String readFirstLine(String path) {
        // try-with-resources: reader.close() се вика АВТОМАТИЧНО, дори при грешка!
        try (BufferedReader reader = new BufferedReader(new FileReader(path))) {
            return reader.readLine();
        } catch (IOException e) {
            // Запазваме оригиналния Stack Trace чрез втория аргумент 'e' (Exception Chaining)
            throw new IllegalStateException("Неуспешно четене на файл: " + path, e);
        }
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Как работи конструкцията **`try-with-resources`** под капака и какво е **Suppressed Exception (Потиснато изключение)**?“*
*   **Уловката:** Всеки знае, че `try-with-resources` затваря файловете автоматично, но малцина знаят какъв интерфейс се изисква и какво става, ако гръмне грешка *едновременно* в `try` блока и при затварянето на ресурса!
*   **Точен Senior отговор:** 
    1.  В `try (...)` може да се постави всеки обект, който имплементира интерфейса **`AutoCloseable`** (или неговия наследник `Closeable`), който има единствен метод `void close() throws Exception`.
    2.  **Suppressed Exception:** При стария `try-finally`, ако `try` блокът хвърли `IOException`, а в `finally` методът `close()` хвърли второ изключение, второто **изтрива** оригиналната грешка! В модерния `try-with-resources` компилаторът запазва **оригиналното изключение от `try` блока като главно**, а грешката от `close()` я закача към него чрез `primaryEx.addSuppressed(closeEx)` (достъпна с `e.getSuppressed()`), така че нито една следа не се губи!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ СРЕЩУ C# 12+ (.NET)

| Архитектурен детайл | ☕ Java 21+ (JVM) | 🟣 C# 12+ (.NET 8+ CLR) |
| :--- | :--- | :--- |
| **Checked Exceptions** | **Да** (задължително обявяване с `throws` или `try-catch`) | **Не** (C# умишлено премахна Checked Exceptions при създаването си) |
| **Автоматично освобождаване на ресурси** | `try (var res = new Resource()) { ... }` (`AutoCloseable`) | `using var res = new Resource();` (`IDisposable`) |
| **Multi-catch (няколко типа в 1 блок)** | `catch (IOException \| SQLException e)` | Отделни `catch` блокове (или `catch (Exception e) when (...)`) |
| **Exception Filters (Условен `catch`)** | Няма директен `when` в `catch` | `catch (HttpRequestException ex) when (ex.StatusCode == 404)` |
| **Прехвърляне на изключение нагоре** | `throw e;` (запазва оригиналния Stack Trace) | `throw;` (без променлива! `throw ex;` рестартира Stack Trace-а!) |

---
[⬅️ 02-07. Reflection & Annotations](02-07-00-Reflection-And-Annotations.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-08-01. Syntax From Scratch](02-08-01-Syntax-From-Scratch.md)
