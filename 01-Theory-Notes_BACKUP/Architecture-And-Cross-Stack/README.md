🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🗄️ [Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | 💾 [Entity Framework Core](../Databases-And-EF-Core/) | ▶️ [Започни от 01-00](01-00-ARCHITECTURE-AND-MEMORY-INTRO.md)

---

# Architecture & Cross-Stack — Фундаментален Архитектурен Атлас

Тази директория съдържа пълна софтуерна и системна база знания за управление на паметта, вътрешна архитектура на рънтайм средите, превод на типове данни през целия стек и избор на софтуерни архитектурни стилове в реални проекти.

## 🧭 Навигация и Съдържание

### 🔹 Модул 1: Архитектура на езиците и Управление на паметта
* **01-00. Архитектура на езиците и Управление на паметта (Кратък справочник)**
  * ⚡ [Главен справочник на темата](01-00-ARCHITECTURE-AND-MEMORY-INTRO.md)
  * *Обхваща:* Тримата крале на паметта (Stack, Heap, GC), превод на кода до процесора и бързо рънтайм сравнение.
* **01-01. Cross-Stack Карта: SQL ➔ C# ➔ JS / TypeScript ➔ Python**
  * 🗺️ [Крос-платформена карта на типовете](01-01-Cross-Stack-Types-And-Ecosystem.md)
  * *Обхваща:* Мащабна матрица на съответствието на типовете, енд-ту-енд път на данните (SQL ➔ C# ➔ JSON ➔ TS), превод на LINQ заявки към JS и Python и full-stack капани.
* **01-02. Дълбоко гмуркане в паметта: Stack срещу Heap**
  * 🧠 [Стек, Купчина и Garbage Collection](01-02-Stack-Vs-Heap-Memory-Deep-Dive.md)
  * *Обхваща:* Структури и размери на паметта, Value и Reference типове, фази на Garbage Collector-а (Mark, Sweep, Compact) и StackOverflowException капани.
* **01-03. Сравнение на бекенд платформи: .NET срещу JVM срещу Node.js**
  * 🗄️ [Сравнение на платформи и нишки](01-03-Ecosystems-Comparison-DotNet-JVM-Node.md)
  * *Обхваща:* Архитектура на CLR, JVM и V8/libuv, сравнение на Thread-per-Request срещу Single-Thread Event Loop и Thread Starvation рискове.
* **01-04. Компилатори, Интерпретатори и Рънтайм среди (Runtime)**
  * ⚙️ [Пътят на кода до процесора](01-04-Compilers-Interpreters-And-Runtime.md)
  * *Обхваща:* Нива на абстракция, Ahead-Of-Time (AOT) компилация, чиста интерпретация и Just-In-Time (JIT) хибриден модел с неговите Compile-time и Runtime капани.

---

### 🔹 Модул 2: Софтуерни архитектури и Бази данни
* **02-00. Софтуерни архитектури и Бази данни (Кратък справочник)**
  * ⚡ [Главен справочник на темата](02-00-SOFTWARE-ARCHITECTURE-OVERVIEW.md)
  * *Обхваща:* Основни архитектурни стилове, връзката между дизайна на софтуера и избора на база и преглед на модула.
* **02-01. Архитектурни стилове: Монолит, Микроуслуги и Serverless**
  * 🏗️ [Сравнение на софтуерни системи](02-01-Architectural-Styles-Monolith-Microservices-Serverless.md)
  * *Обхваща:* Предимства и търговски компромиси при деплоймент, мащабиране и латентност, Polyglot Persistence, Shared Database капани и Saga Pattern.
* **02-02. SQL срещу NoSQL: Cross-Stack Сравнение на Концепциите**
  * ⚖️ [Релационни срещу Нерелационни среди](02-02-SQL-Vs-NoSQL-Cross-Stack-Comparison.md)
  * *Обхваща:* Златно правило за избор на база, ACID сигурност срещу BASE мащаб, симулация на една и съща условна заявка в 4 различни бази и FLOAT капани при финансови изчисления.
* **02-03. Сравнение на Бази Данни: SQL срещу NoSQL (Бърз справочник)**
  * 🛠️ [Интервю и инженерен Cheat-Sheet](02-03-SQL-Vs-NoSQL-Interview-CheatSheet.md)
  * *Обхваща:* Матрица на C# пакетите, ACID срещу BASE накратко, симулация на заявка в реално време и топ грешки в практиката.

---
🏠 [Всички бележки (01-Theory-Notes)](../README.md) | 🗄️ [Бази данни (SQL & NoSQL)](../Databases-SQL-And-NoSQL/) | 💾 [Entity Framework Core](../Databases-And-EF-Core/) | ▶️ [Започни от 01-00](01-00-ARCHITECTURE-AND-MEMORY-INTRO.md)

© 2026 Dev-Library / Пълна архитектурна библиотека по Структури и Платформи
