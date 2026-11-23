[⬅️ 02-08. Множествено свързване](02-08-InverseProperty-Multiple-Relationships.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 02-10. Напреднал LINQ и Специфични операции](02-10-Advanced-LINQ-And-Query-Operations.md)

---

# 02-09. Валидации с Data Annotations и Специфични Атрибути

**Цел:** Изучаване на атрибутите за клиентска и сървърна валидация на свойствата, оптимизация чрез индекси (`[Index]`) и изключване на изчислени свойства от схемата на базата чрез `[NotMapped]`.

## Съдържание на темата
* [1. Основни атрибути за валидация на данни](#1-основни-атрибути-за-валидация-на-данни)
* [2. Оптимизация на заявките чрез атрибута Index](#2-оптимизация-на-заявките-чрез-атрибута-index)
* [3. Изключване на бизнес свойства чрез NotMapped](#3-изключване-на-бизнес-свойства-чрез-notmapped)

---

## 1. ОСНОВНИ АТРИБУТИ ЗА ВАЛИДАЦИЯ НА ДАННИ

Когато използваме Data Annotations подхода, атрибутите управляват едновременно софтуерните проверки в C# (Client/Application Validation) и физическата структура в SQL Server (DB Validation):

*   **`[Required]`**: Маркира свойството като задължително (`NOT NULL` в SQL). Ако типът е Nullable (напр. `int?` или `string`), при опит за запис на `null` EF Core ще хвърли изключение. Не-nullable типовете (като чист `int`) имат вградена стойност по подразбиране (`0`) на ниво език и няма да гръмнат, а просто ще запишат `0`.
*   **`[MinLength(n)]`**: Дефинира минимална дължина на низа. Работи изцяло като софтуерна проверка на ниво C# приложение преди изпращане.
*   **`[MaxLength(n)]` / `[StringLength(n)]`**: Дефинира максимална дължина на низа. Налага **`NVARCHAR(n)`** структура в SQL Server, предпазвайки диска от фрагментация, и спира прекалено дълги текстове още в C#.
*   **`[Range(min, max)]`**: Налага долна и горна граница за числови полета (напр. `[Range(18, 100)]` за възраст). Работи като софтуерна валидация в приложението.

---

## 2. ОПТИМИЗАЦИЯ НА ЗАЯВКИТЕ ЧРЕЗ АТРИБУТА INDEX

По подразбиране SQL Server търси редове бързо само по първичния ключ (`Primary Key`), защото той автоматично притежава индекс. Ако често филтрираме потребители по колона, която не е ключ (например търсене по `Url` или `Email`), базата извършва бавно сканиране на цялата таблица (Table Scan).

Чрез атрибута **`[Index]`** изрично караме SQL Server да изгради **Non-Clustered Index** за тази колона на диска, което ускорява `SELECT` търсенията до $O(\log n)$ време:

```csharp
using Microsoft.EntityFrameworkCore;

[Index(nameof(Url), IsUnique = true)] // Генерира уникален индекс в SQL Server
public class Student 
{
    public int Id { get; set; }
    
    public string Url { get; set; } = null!;
}
```

---

## 3. ИЗКЛЮЧВАНЕ НА БИЗНЕС СВОЙСТВА ЧРЕЗ NOTMAPPED

В реалния софтуер често ни трябват помощни свойства, които сглобяват данни в паметта за нуждите на интерфейса, но няма смисъл да заемат място като колони на твърдия диск (напр. изчислено пълно име от две колони).

Атрибутът **`[NotMapped]`** казва на EF Core: *"Игнорирай напълно това свойство и не му създавай колона в SQL Server"*:

```csharp
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    [NotMapped] // Използва се само за бизнес логика в паметта - няма колона в SQL!
    public string FullName => \$"{this.FirstName} {this.LastName}";
}
```

---
[⬅️ 02-08. Множествено свързване](02-08-InverseProperty-Multiple-Relationships.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 02-10. Напреднал LINQ и Специфични операции](02-10-Advanced-LINQ-And-Query-Operations.md)

© 2026 Dev-Library / Справочник: Data Annotations Валидации
