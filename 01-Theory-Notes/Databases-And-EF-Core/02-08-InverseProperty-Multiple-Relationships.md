[⬅️ 02-07. TDD и InMemory Тестване](02-07-EF-Core-TDD-And-InMemory-Testing.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 03-00. Кратък справочник за защити](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md)

---

# 02-08. Атрибутът InverseProperty при множество връзки между две таблици

**Цел:** Разбиране на казуса с дублиращи се релации между едни и същи обекти — как да укажем софтуерно на EF Core коя колекция към кой Foreign Key принадлежи, използвайки Data Annotations атрибута `[InverseProperty]`.

## Съдържание на темата
* [1. Какъв проблем решава InverseProperty (Казусът с двете връзки)](#1-какъв-проблем-решава-inverseproperty-казусът-с-двете-връзки)
* [2. Анатомия на кода: Пълен двупосочен пример](#2-анатомия-на-кода-пълен-двупосочен-пример)
* [3. Алтернативно решение чрез Fluent API (OnModelCreating)](#3-алтернативно-решение-чрез-fluent-api-onmodelcreating)

---

## 1. КАКЪВ ПРОБЛЕМ РЕШАВА INVERSEPROPERTY (КАЗУСЪТ С ДВЕТЕ ВРЪЗКИ)

В стандартните софтуерни релации две таблици се свързват само веднъж (напр. `Student` има един `Course`). Има обаче бизнес сценарии, при които едно Entity е свързано с друго по **две или повече напълно независими причини**.

### Реален пример: Връзката между Град (Town) и Човек (Person)
*   Един град може да бъде **Място на раждане** за много хора (`Natives`).
*   Същият този град може да бъде и **Настоящ адрес / местоживеене** за съвсем друга група от хора (`Residents`).

Ако просто декларираш двете колекции `ICollection<Person>` в класа `Town`, EF Core ще се обърка и няма да разбере как да ги навърже. На софтуерно ниво се налага използването на **`[InverseProperty("ИмеНаСвойствотоОтДругатаСтрана")]`** — той служи за „обратен указател“, който заключва колекцията към точния Foreign Key навигационен обект.

---

## 2. АНАТОМИЯ НА КОДА: ПЪЛЕН ДВУПОСОЧЕН ПРИМЕР

За да легне логиката правилно, ето как изглеждат двата C# класа, работещи в пълен синхрон:

### Лява страна (Подчиненото Entity — Person)
В класа `Person` имаме два отделни навигационни обекта, сочещи към града:

```csharp
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Връзка №1: Къде е роден човекът
    public int? PlaceOfBirthId { get; set; }
    public Town? PlaceOfBirth { get; set; }

    // Връзка №2: Къде живее в момента човекът
    public int? CurrentResidenceId { get; set; }
    public Town? CurrentResidence { get; set; }
}
```

### Дясна страна (Главното Entity — Town с InverseProperty)
В класа `Town` използваме атрибута, за да кажем коя колекция на кое свойство от `Person` съответства:

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Town
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Обвързваме колекцията Natives със свойството PlaceOfBirth в клас Person
    [InverseProperty("PlaceOfBirth")]
    public ICollection<Person> Natives { get; set; } = new HashSet<Person>();

    // Обвързваме колекцията Residents със свойството CurrentResidence в клас Person
    [InverseProperty("CurrentResidence")]
    public ICollection<Person> Residents { get; set; } = new HashSet<Person>();
}
```

---

## 3. АЛТЕРНАТИВНО РЕШЕНИЕ ЧРЕЗ FLUENT API (ONMODELCREATING)

Ако предпочиташ инженерния стандарт Fluent API, за да запазиш моделите напълно чисти от атрибути, същата софтуерна релация се описва по следния начин в твоя `DbContext`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Конфигурираме Връзка №1 (Място на раждане)
    modelBuilder.Entity<Person>()
        .HasOne(p => p.PlaceOfBirth)          // Човекът има едно място на раждане
        .WithMany(t => t.Natives)             // Градът има много родом от него хора
        .HasForeignKey(p => p.PlaceOfBirthId);

    // Конфигурираме Връзка №2 (Настоящ адрес)
    modelBuilder.Entity<Person>()
        .HasOne(p => p.CurrentResidence)      // Човекът има един настоящ адрес
        .WithMany(t => t.Residents)           // Градът има много жители в момента
        .HasForeignKey(p => p.CurrentResidenceId);
}
```

---
[⬅️ 02-07. TDD и InMemory Тестване](02-07-EF-Core-TDD-And-InMemory-Testing.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [🏠 Начало (EF Core)](README.md) | [➡️ 03-00. Кратък справочник за защити](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md)

© 2026 Dev-Library / Справочник: Множествено свързване и InverseProperty
