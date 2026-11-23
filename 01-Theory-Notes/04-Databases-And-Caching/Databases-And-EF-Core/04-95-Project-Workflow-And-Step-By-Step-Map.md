[⬅️ Назад към Databases & EF Core](README.md) | [📂 Текуща папка: Databases & EF Core](README.md) | [🏠 Начало](../../../README.md) | [➡️ 04-96. Performance & Optimizations](04-96-Performance-And-Optimizations.md)

---

# 04-95. EF Core Project Workflow: Карта Стъпка по Стъпка (Entity ➔ Mapping ➔ LINQ ➔ SQL)

**Домейн:** Databases & ORM Systems | **Цел:** Практическа мисловна карта и последователност от стъпки при изграждане на модели и достъп до база данни в реален C# / .NET проект — с ясно сравнение между трите начина за Mapping (**Convention / Auto**, **Data Annotations / Attributes** и **Fluent API**), правила кога кой подход да избереш и контролен списък преди пускане на миграции.

## 🧭 Съдържание на картата
* [1. Голямата картина: Пътят от C# класа до Базата данни (Блок-схема)](#1-голямата-картина-пътят-от-c-класа-до-базата-данни-блок-схема)
* [2. Сравнение на трите подхода за Mapping (Кой и кога е най-удобен?)](#2-сравнение-на-трите-подхода-за-mapping-кой-и-кога-е-най-удобен)
* [3. Стъпка по стъпка: Как изграждам модела при писане на проект (Етапи 1–7)](#3-стъпка-по-стъпка-как-изграждам-модела-при-писане-на-проект-етапи-17)
* [4. Как чета и пиша LINQ заявка от условие (Двупосочно мислене)](#4-как-чета-и-пиша-linq-заявка-от-условие-двупосочно-мислене)
* [5. Контролен списък (Checklist) преди добавяне на Migration](#5-контролен-списък-checklist-преди-добавяне-на-migration)

---

## 1. ГОЛЯМАТА КАРТИНА: ПЪТЯТ ОТ C# КЛАСА ДО БАЗАТА ДАННИ (БЛОК-СХЕМА)

Когато работиш с Entity Framework Core, не започваш хаотично да пишеш заявки. Винаги следваш този строго подреден инженерен поток:

```text
  [1. C# Class (Entity)] ──► [2. Mapping (Таблица & Колони)] ──► [3. DbContext & DbSet<T>]
                                                                            │
  [6. SQL Database] ◄── [5. EF Core Превод (LINQ -> SQL)] ◄── [4. LINQ Заявка (Where/Select)]
```

---

## 2. СРАВНЕНИЕ НА ТРИТЕ ПОДХОДА ЗА MAPPING (КОЙ И КОГА Е НАЙ-УДОБЕН?)

| Подход за Mapping | Как работи? | Къде се пише? | Кога е най-удобен в реален проект? |
| :--- | :--- | :--- | :--- |
| **1. Convention (Auto Mapping)** | EF Core сам свързва `Product` ➔ `Products` и `Name` ➔ `Name` по съвпадение на имената. | Никъде (вградени правила на EF Core). | Когато започваш нов проект (Code-First) и имената на класовете и колоните са стандартни. |
| **2. Attributes (Data Annotations)** | Слагаш етикети `[Table("...")]`, `[Column("...")]`, `[Required]`, `[MaxLength(100)]`. | Директно върху класа и свойствата му. | При малки/средни проекти, когато искаш да виждаш правилата веднага върху самото Entity. |
| **3. Fluent API** | Конфигурираш правилата чрез `modelBuilder.Entity<T>()` или `IEntityTypeConfiguration<T>`. | В отделен конфигурационен клас / `OnModelCreating`. | **Златен стандарт за големи проекти:** държи Entity класовете чисти и поддържа сложни връзки (`1:N`, `N:N`), съставни ключове и индекси. |

### 🧭 Дърво на решението (Кой подход да избера?):
```text
             Съвпадат ли имената на класа и колоните с базата?
                                    │
                       ┌────────────┴────────────┐
                      ДА                         НЕ
                       │                          │
                       ▼                          ▼
             [Convention / Auto]        [Explicit Mapping]
             (Не пишеш нищо)                      │
                                     ┌────────────┴────────────┐
                                Прост случай             Сложен проект /
                                     │                   Много връзки
                                     ▼                         │
                               [Attributes]                    ▼
                                                         [Fluent API]
```

---

## 3. СТЪПКА ПО СТЪПКА: КАК ИЗГРАЖДАМ МОДЕЛА ПРИ ПИСАНЕ НА ПРОЕКТ (ЕТАПИ 1–7)

### 🔹 Етап 1: Създавам чистия Entity клас (`Models/Product.cs`)
Дефинирам свойствата (Properties), които описват обекта в C#:
```csharp
namespace MyApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

### 🔹 Етап 2: Определям Mapping-а (От общото към частното: Таблица ➔ Колони)
Ако имената в базата са различни (напр. таблица `store_products` с колони `product_id`, `product_name`, `product_price`), изнасям конфигурацията в отделен клас (`Configurations/ProductConfiguration.cs`):
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Models;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // 1. Общото: Entity -> Table
        builder.ToTable("store_products");
        builder.HasKey(p => p.Id);

        // 2. Частното: Property -> Column (проверявам всяко свойство по ред)
        builder.Property(p => p.Id).HasColumnName("product_id");
        builder.Property(p => p.Name).HasColumnName("product_name").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Price).HasColumnName("product_price").HasPrecision(18, 2);
    }
}
```

### 🔹 Етап 3: Регистрирам `DbSet<T>` и конфигурациите в `DbContext` (`Data/AppDbContext.cs`)
```csharp
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Автоматично зарежда всички IEntityTypeConfiguration класове в проекта!
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

### 🔹 Етап 4: Регистрирам контекста в `Program.cs` и пускам миграция
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 4. КАК ЧЕТА И ПИША LINQ ЗАЯВКА ОТ УСЛОВИЕ (ДВУПОСОЧНО МИСЛЕНЕ)

* **Посока 1 (Условие ➔ Логика ➔ Код):**
  * *Условие:* „Искам имената на продуктите с цена над 60 лв., подредени от най-скъпия.“
  * *Стъпки:* Взимам `context.Products` ➔ филтрирам `.Where(p => p.Price > 60)` ➔ сортирам `.OrderByDescending(p => p.Price)` ➔ взимам само имената `.Select(p => p.Name)` ➔ изпълнявам към базата `.ToListAsync()`.
* **Посока 2 (Готов Код ➔ SQL превод):**
  * EF Core превежда горната верига директно в оптимизиран SQL (`SELECT product_name FROM store_products WHERE product_price > 60 ORDER BY product_price DESC`).

---

## 5. КОНТРОЛЕН СПИСЪК (CHECKLIST) ПРЕДИ ДОБАВЯНЕ НА MIGRATION

* [ ] Създадох ли Entity класа с ясни типове (`decimal` за пари, `string.Empty` или `required` за задължителни текстове)?
* [ ] Проверих ли дали таблицата и колоните съвпадат по конвенция, или съм описал изричен Fluent API mapping?
* [ ] Добавих ли `public DbSet<EntityName>` в `AppDbContext`?
* [ ] Извиках ли `ApplyConfigurationsFromAssembly(...)` в `OnModelCreating`?
* [ ] Регистрирах ли `AddDbContext` с правилния Connection String в `Program.cs`?

---

[⬅️ Назад към Databases & EF Core](README.md) | [📂 Текуща папка: Databases & EF Core](README.md) | [🏠 Начало](../../../README.md) | [➡️ 04-96. Performance & Optimizations](04-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 04-95. EF Core Project Workflow: Карта Стъпка по Стъпка
