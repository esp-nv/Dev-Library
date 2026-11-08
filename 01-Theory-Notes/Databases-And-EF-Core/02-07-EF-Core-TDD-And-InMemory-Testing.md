[⬅️ 02-06. Database-First и Scaffolding](02-06-Database-First-Model-Scaffolding.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 03-00. Кратък справочник за защити](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md)

---

# 02-07. EF Core с TDD и Тестване в паметта (In-Memory Testing)

**Цел:** Изучаване на подхода Разработка, водена от тестове (TDD) в контекста на базите данни — как да изолираме инфраструктурния слой и да тестваме LINQ заявки и CRUD операции бързо, без реална връзка с физически SQL Server.

## Съдържание на темата
* [1. Концепцията TDD (Red-Green-Refactor) при бази данни](#1-концепцията-tdd-red-green-refactor-при-бази-данни)
* [2. NuGet пакети за InMemory тестване](#2-nuget-пакети-за-inmemory-тестване)
* [3. Пълен софтуерен пример: Първо теста, после кода](#3-пълен-софтуерен-пример-първо-теста-после-кода)
* [4. Критични капани при тестване с InMemory провайдър](#4-критични-капани-при-тестване-с-inmemory-провайдър)

---

## 1. КОНЦЕПЦИЯТА TDD (RED-GREEN-REFACTOR) ПРИ БАЗИ ДАННИ

**Test-Driven Development (TDD)** обръща стандартния процес на разработка наопаки. Жизненият цикъл на писане преминава през три фази:
1.  **🔴 RED (Напиши провалящ се тест):** Пишеш тестов метод за бизнес функционалност (напр. *„Търсене на студенти с отличен успех“*), преди изобщо да си създала колоните в базата данни или LINQ заявката. Тъй като логиката липсва, тестът се проваля.
2.  **🟢 GREEN (Напиши минималния код):** Отиваш в твоя `DbContext` или сервиз и написваш възможно най-простата LINQ заявка, колкото тестът да светне в зелено.
3.  **🔵 REFACTOR (Изчисти кода):** Оптимизираш LINQ кода, премахваш дублирането и подобряваш имената, като сигурността ти е гарантирана от работещия зелен тест.

---

## 2. NUGET ПАКЕТИ ЗА INMEMORY ТЕСТВАНЕ

За да тестваме базата данни по TDD модел, е забранено да пишем и трием реални редове в производствения SQL Server, защото тестовете ще се забавят и ще си пречат един на друг. Използваме **In-Memory доставчик**, който симулира празна база данни изцяло вътре в RAM паметта на компютъра:

*   **Основен тестов провайдър:** `Microsoft.EntityFrameworkCore.InMemory`
*   **Тестова рамка (напр. xUnit):** `xunit` и `Microsoft.NET.Test.Sdk`

---

## 3. ПЪЛЕН СОФТУЕРЕН ПРИМЕР: ПЪРВО ТЕСТА, ПОСЛЕ КОДА

### Фаза 1: 🔴 RED (Писане на теста в тестов проект)
Създаваме симулация на изолирана RAM база данни чрез `DbContextOptionsBuilder` с уникално име за всеки тест:

```csharp
[Fact]
public void ShouldReturnOnlyExcellentStudents()
{
    // Аранжиране (Настройка на InMemory базата)
    var options = new DbContextOptionsBuilder<SchoolDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникално име за изолация
        .Options;

    using (var context = new SchoolDbContext(options))
    {
        context.Students.Add(new Student { Id = 1, Name = "Ivan", Grade = 6.00 });
        context.Students.Add(new Student { Id = 2, Name = "Maria", Grade = 4.50 });
        context.SaveChanges();
    }

    // Действие (Преди да сме написали реалния метод, викаме сервиза)
    using (var context = new SchoolDbContext(options))
    {
        var service = new StudentService(context);
        var result = service.GetExcellentStudents(); // ➔ В този момент тестът ГЪРМИ (Червено), защото методът не съществува!

        // Твърдение (Асерция)
        Assert.Single(result);
        Assert.Equal("Ivan", result[0].Name);
    }
}
```

### Фаза 2: 🟢 GREEN (Писане на минималния код)
Отиваме в бизнес проекта и написваш точния метод, за да задоволиш изискването на теста:

```csharp
public class StudentService
{
    private readonly SchoolDbContext _context;

    public StudentService(SchoolDbContext context)
    {
        _context = context;
    }

    public List<Student> GetExcellentStudents()
    {
        // Минималният и точен LINQ код, който кара теста да светне в ЗЕЛЕНО
        return _context.Students
            .Where(s => s.Grade == 6.00)
            .ToList();
    }
}
```

---

## 4. КРИТИЧНИ КАПАНИ ПРИ ТЕСТВАНЕ С INMEMORY ПРОВАЙДЪР

*   **Капан 1: Измамно усещане за сигурност (InMemory не е истински SQL Server):**
    *   *Проблемът:* `UseInMemoryDatabase` е просто колекция от списъци в RAM паметта. Той **НЕ** налага ограничения за външни ключове (`Foreign Keys`), не проверява за `NOT NULL` ограничения и не изпълнява суров SQL синтаксис. Тестът ти може да е зелен в паметта, но в момента, в който пуснеш софтуера в реалния SQL Server, той да се срине.
    *   *Решение:* За интеграционни тестове, изискващи 100% точност с реалната база, използвай **SQLite In-Memory** доставчик (`options.UseSqlite("Data Source=:memory:")`), тъй като той е истински релационен двигател, или използвай Docker контейнери.

---
[⬅️ 02-06. Database-First и Scaffolding](02-06-Database-First-Model-Scaffolding.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 03-00. Кратък справочник за защити](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md)

© 2026 Dev-Library / Справочник: TDD и InMemory Тестване
