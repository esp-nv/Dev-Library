# 📚 C# Basic, Fundamentals, Advanced & OOP – Архитектурна Библиотека

Пълна справочна база по C# и .NET, структурирана в **двоен стандарт**:
* ⚡ **Кратък справочник (`01` – `14`):** Бърз преговор, матрици, правила и Junior капани (5 мин. преди изпит).
* 📖 **Дълъг учебник (`*-csharp.md`):** Дълбока архитектура под капака (Stack/Heap памет), кодови сепаратори `// ====================` и реални примери.

---

## 🧭 Навигация и Съдържание

### 🔹 Модул 1: C# Основи, Типове, Колекции и Текстообработка

* **01. Основи на C# и синтаксис**
  * 📄 [Кратък справочник](./01-C-SHARP-FUND-SHORT.md) | 📘 [Дълъг учебник](./csharp.md)
  * *Обхваща:* Базова структура на .NET приложение, вход/изход в конзолата и основни оператори.
* **01-01. Типове данни, Условни конструкции и Цикли**
  * 📄 [Кратък справочник](./01-01-TYPES-CONDITIONALS-LOOPS.md) | 📘 [Дълъг учебник](./types-conditionals-loops-csharp.md)
  * *Обхваща:* Value vs Reference в Stack/Heap, `decimal` vs `double`, `switch` expressions и вложени цикли.
* **04. Колекции и Структури от данни**
  * 📄 [Кратък справочник](./04-COLECTIONS.md) | 📘 [Дълъг учебник](./colections-csharp.md)
  * *Обхваща:* `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Stack<T>`, `Queue<T>` и времева сложност $O(1)$ vs $O(n)$.
* **10. Методи, Делегати и Ламбда изрази**
  * 📄 [Кратък справочник](./10-METHODS-DELEGATES-LAMBDA.md) | 📘 [Дълъг учебник](./methods-delegates-csharp.md)
  * *Обхваща:* Call Stack Frames, `ref`/`out`/`in`, `Func`, `Action`, `Predicate` и Closures в паметта.
* **11. Текстообработка и StringBuilder**
  * 📄 [Кратък справочник](./11-STRING-TEXT-PROCESSING.md) | 📘 [Дълъг учебник](./string-text-processing-csharp.md)
  * *Обхваща:* String Immutability, String Intern Pool, динамичен буфер в `StringBuilder` и `ReadOnlySpan<char>`.
* **12. Регулярни изрази (Regex)**
  * 📄 [Кратък справочник](./12-REGULAR-EXPRESSIONS.md) | 📘 [Дълъг учебник](./regular-expressions-csharp.md)
  * *Обхваща:* Символни класове, квантификатори (Greedy vs Lazy), именувани групи и защита от Backtracking.
* **13. Потоци (Streams / I/O), Файлове и Директории**
  * 📄 [Кратък справочник](./13-STREAMS-FILES-DIRECTORIES.md) | 📘 [Дълъг учебник](./streams-files-directories-csharp.md)
  * *Обхваща:* `FileStream`, `StreamReader`/`StreamWriter`, байтови буфери, `Path.Combine` и `IDisposable` (`using`).

---

### 🔹 Модул 2: Обектно-Ориентирано Програмиране (OOP), LINQ и Архитектура

* **02. ООП Концепции и Ключови думи**
  * 📄 [Кратък справочник](./02-OOP-Concepts-And-Keywords.md)
  * *Обхваща:* Наследяване, капсулация, абстракция, полиморфизъм, `virtual`/`override`, `abstract` и `interface`.
* **03. LINQ и Отложено изпълнение (Deferred Execution)**
  * 📄 [Дълбок анализ](./03-LINQ-Deep-Dive-And-Deferred-Execution.md) | 📋 [Cheat Sheet](./LINQ-Operations-Cheat-Sheet.md) | 🗺️ [LINQ Карта](./map-linq.md)
  * *Обхваща:* `IEnumerable<T>` vs `IQueryable<T>`, филтриране, групиране, агрегации и материализация.
* **05. SOLID Принципи**
  * 📄 [Кратък справочник](./05-SOLID.md) | 📘 [Дълъг учебник](./solid-principles-csharp.md)
  * *Обхваща:* Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation и Dependency Inversion.
* **06. Reflection и Атрибути**
  * 📄 [Кратък справочник](./06-REFLECTION-ATTRIBUTES.md) | 📘 [Дълъг учебник](./reflection-attributes-csharp.md)
  * *Обхваща:* Метаданни в Runtime, `Type`, динамично инстанцииране (`Activator`) и потребителски атрибути.
* **14. Шаблони за проектиране (Design Patterns)**
  * 📄 [Кратък справочник](./14-DESIGN-PATTERNS-OOP.md) | 📘 [Дълъг учебник](./design-patterns-oop-csharp.md)
  * *Обхваща:* Creational (Singleton, Factory, Builder), Structural (Adapter, Decorator) и Behavioral (Strategy, Observer).

---

### 🔹 Модул 3: Дебъгване, Unit Тестване, Mocking и TDD

* **07. Техники за дебъгване (Debugging)**
  * 📄 [Кратък справочник](./07-DEBUGGING-TECHNIQUES.md) | 📘 [Дълъг учебник](./debugging-techniques-csharp.md)
  * *Обхваща:* Breakpoints, Watch прозорци, Call Stack проследяване и изолиране на логически грешки.
* **08. Unit Тестване и Mocking (Основи)**
  * 📄 [Кратък справочник](./08-UNIT-TESTING-MOCKING.md) | 📘 [Дълъг учебник](./unit-testing-mocking-csharp.md)
  * *Обхваща:* Arrange-Act-Assert (AAA), базови NUnit проверки и изолация на зависимости с Moq.
* **09. Разширено Mocking Ядро и TDD**
  * 📄 [Кратък справочник](./09-TDD-MOCKING-ADVANCED.md) | 📘 [Дълъг учебник](./tdd-mocking-advanced-csharp.md)
  * *Обхваща:* Red-Green-Refactor цикъл, Test Doubles (Dummy, Stub, Mock, Fake), `Callback`, `SetupSequence` и `Strict` vs `Loose`.
* **09-01 & 09-02. Изпитни справочници за тестване**
  * ⚖️ [NUnit vs xUnit vs MSTest](./09-01-NUnit-xUnit-MSTest.md) | 🎯 [Изпитен чеклист (ZOMBIES)](./09-02-Unit-Testing-Exam-Checklist.md)
  * *Обхваща:* Сравнение на атрибутите между трите рамки и стратегия за 100% Code Coverage в Judge.

---
© 2026 Dev-Library / Пълна архитектурна библиотека по C#
