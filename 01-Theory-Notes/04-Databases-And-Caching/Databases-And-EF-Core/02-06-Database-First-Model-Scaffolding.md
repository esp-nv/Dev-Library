[⬅️ 02-05. ADO.NET и Change Tracking](02-05-AdoNet-Vs-EfCore-And-Change-Tracking.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) 

---

# 02-06. Database-First подход и Автоматично генериране на код (Scaffolding)

**Цел:** Овладяване на подхода за обратно инженерство (Reverse Engineering) — автоматично сканиране на съществуваща SQL база данни и генериране на C# Entity класове и DbContext чрез CLI инструментите на EF Core.

## Съдържание на темата
* [1. Какво е Database-First модел и кога се използва](#1-какво-е-database-first-модел-и-кога-се-използва)
* [2. Задължителни NuGet пакети за Scaffolding](#2-задължителни-nuget-пакети-за-scaffolding)
* [3. Пълен CLI Команден дневник (Превръщане на DB в C# код)](#3-пълен-cli-команден-дневник-превръщане-на-db-в-c-код)
* [4. Критични капани при Database-First подход](#4-критични-капани-при-database-first-подход)

---

## 1. КАКВО Е DATABASE-FIRST МОДЕЛ И КОГА СЕ ИЗПОЛЗВА

За разлика от Code-First, при **Database-First** модела ти започваш работа със софтуерна система, в която релационната база данни вече съществува на твърдия диск (създадена е ръчно от DBA администратори или наследена от стар проект). 

```text
  [ Налична SQL База ] ──( dotnet ef dbcontext scaffold )──> [ Автоматично генерирани C# класове ]
```

Вместо да пренаписваш ръчно стотици таблици и колони в C#, ти извикваш процеса **Scaffolding (Скеле)**. Инструментите на EF Core прочитат схемата на базата данни и автоматично изплюват готови, структурирани C# POCO класове и конфигуриран `DbContext` в твоята папка.

---

## 2. ЗАДЪЛЖИТЕЛНИ NUGET ПАКЕТИ ЗА SCAFFOLDING

За да може конзолата да извърши обратното инженерство, в проекта ти трябва да бъдат инсталирани следните три компонента (през Package Manager или CLI):

1.  **Основният провайдър (напр. SQL Server):**
    `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
2.  **Инструментите за дизайн (Design-time components):**
    `dotnet add package Microsoft.EntityFrameworkCore.Design`
3.  **Глобалният инструмент (ако липсва на машината):**
    `dotnet tool install --global dotnet-ef`

---

## 3. ПЪЛЕН CLI КОМАНДЕН ДНЕВНИК (ПРЕВРЪЩАНЕ НА DB В C# КОД)

Командата се изпълнява в терминала на проекта (.NET CLI) или във VS Code и изисква подаване на Connection String, провайдъра и изходната папка:

### 🛠️ Базова команда за Scaffolding:
```bash
dotnet ef dbcontext scaffold "Server=Localhost;Database=SoftUni;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
```
*   **`-o Models` (Output Directory):** Казва на програмата да създаде папка с име `Models` и да изсипе всички генерирани C# класове там.

### 🔄 Напреднали флагове при промяна на базата (Професионална поддръжка):
Ако утре администраторът добави нова колона в SQL Server, ти трябва да обновиш C# кода си. За целта използваме следните мощни флагове:

```bash
dotnet ef dbcontext scaffold "Server=Localhost;Database=SoftUni;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -f -d
```
*   **`-f` (`--force`):** **Насилствено пренаписване.** Презаписва съществуващите C# файлове в папка `Models` на чисто, за да отрази новите промени от SQL Server, без да триеш папката ръчно.
*   **`-d` (`--data-annotations`):** Казва на EF Core да конфигурира ограниченията (като Primary Key, MaxLength, Required) чрез **Data Attributes директно над свойствата**, вместо да генерира дълъг Fluent API код в `DbContext`.

---

## 4. КРИТИЧНИ КАПАНИ ПРИ DATABASE-FIRST ПОДХОД

*   **Капан 1: Изчезване на ръчно написания C# код при повторен Scaffold:**
    *   *Какво се случва:* Пускаш Scaffolding, отваряш генерирания клас `Employee.cs` и вътре ръчно написваш твой помощен метод (напр. `public string GetFullName()`). След един месец базата се променя, ти пускаш командата отново с флага `-f` и EF Core изтрива и пренаписва файла на чисто, изтривайки твоя код завинаги.
    *   *Решение:* Всички генерирани класове от EF Core са маркирани като **`public partial class`**. Никога не пиши твой код вътре в тях! Създай отделен твой C# файл в проекта (напр. `Employee.Extension.cs`) и дефинирай същия partial клас вътре — така твоят код ще оцелее при всеки нов Scaffold!
*   **Капан 2: Забрана за използване на стандартните Code-First миграции:**
    *   *Какво се случва:* Опитваш се да пуснеш `Add-Migration` в проект, който е направен по Database-First. Миграционната система се заплита, защото базата данни вече съществува и се управлява отвън.
    *   *Решение:* При Database-First модела ти си изцяло „консуматор“ на базата. Всички промени по структурата се правят директно в SQL Server (през SSMS), след което просто опресняваш C# кода чрез флага `-f`.

---
[⬅️ 02-05. ADO.NET и Change Tracking](02-05-AdoNet-Vs-EfCore-And-Change-Tracking.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) 

© 2026 Dev-Library / Справочник: Database-First & Scaffolding
