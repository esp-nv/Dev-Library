⬅️ [04-06. OLTP срещу OLAP и Data Warehouses](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [00-03. Архитектура, Бекъпи и Инструменти](00-03-DBMS-Architecture-Backups-And-Tools.md)

---

# 04-07. Релации и Joins в ASP.NET Core (DbContext, ModelBuilder и LINQ)

**Цел:** Как да превеждаме релационния модел от SQL директно в ASP.NET Core / Entity Framework Core — дефиниране на `DbSet` в `DbContext`, конфигуриране на връзки `1:1`, `1:Many`, `Many:Many` и `OnDelete` правила чрез **Fluent API (`ModelBuilder`)**, и писане на `INNER JOIN` и `LEFT JOIN` в LINQ.

## Съдържание на темата
* [1. Карта на превода: SQL Концепция ➔ EF Core / C# Еквивалент](#1-карта-на-превода-sql-концепция--ef-core--c-еквивалент)
* [2. DbContext и Fluent API (ModelBuilder) — Връзки 1:1, 1:Many и Many:Many](#2-dbcontext-и-fluent-api-modelbuilder--връзки-11-1many-и-manymany)
* [3. Как се пишат INNER JOIN и LEFT JOIN в EF Core / LINQ](#3-как-се-пишат-inner-join-и-left-join-в-ef-core--linq)
* [4. Чести грешки, Капани и Как да ги избегнем](#4-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. КАРТА НА ПРЕВОДА: SQL КОНЦЕПЦИЯ ➔ EF CORE / C# ЕКВИВАЛЕНТ

| SQL Концепция | EF Core / C# Еквивалент | Какво генерира EF Core под капака? |
| :--- | :--- | :--- |
| **Database** | **`DbContext`** (`AppDbContext`) | Управлява връзката, транзакциите и следенето на промени. |
| **Table** | **`DbSet<TEntity>`** (`DbSet<Order>`) | Представлява конкретна таблица в базата данни. |
| **Foreign Key + Navigation** | `int CustomerId` + `Customer Customer` | Свързва двата класа и позволява навигация с точка (`o.Customer.Name`). |
| **`ON DELETE CASCADE / RESTRICT`** | **`.OnDelete(DeleteBehavior.Cascade / Restrict)`** | Задава правилото при изтриване вътре в `OnModelCreating`. |
| **`INNER JOIN`** | **`.Select(o => new { o.Id, o.Customer.Name })`** *(при `NOT NULL` FK)* | Автоматично генерира `INNER JOIN` без да пишеш `join` ръчно! |
| **`LEFT JOIN`** | **`.Select(c => new { c.Name, Orders = c.Orders.Count() })`** или **`.Include()`** | Генерира `LEFT JOIN`, когато навигационното свойство е nullable (`int?`) или колекция. |
| **Ръчен `JOIN` без навигация** | **`.Join(...)`** или **`.GroupJoin(...).SelectMany(... DefaultIfEmpty())`** | Използва се, когато две таблици нямат дефиниран Foreign Key помежду си. |


### Сравнение на Каскадното триене: SQL срещу EF Core (`DeleteBehavior`)

| SQL Правило (`ON DELETE ...`) | EF Core (`ModelBuilder`) | Какво се случва при изтриване на родителя (напр. `User`)? | Кога се използва? |
| :--- | :--- | :--- | :--- |
| **`ON DELETE CASCADE`** | **`.OnDelete(DeleteBehavior.Cascade)`** | Изтрива родителя **И автоматично трие всички свързани записи** (`UserProfile`, `Comments`). | Когато дъщерните записи нямат смисъл без родителя *(по подразбиране при задължителен `int` FK в EF Core)*. |
| **`ON DELETE NO ACTION` / `RESTRICT`** | **`.OnDelete(DeleteBehavior.Restrict)`** *(или `NoAction`)* | **Блокира изтриването** и хвърля грешка, ако има поне 1 свързан запис (`Orders`). | **Задължително за поръчки, фактури и плащания** — не можеш да изтриеш клиент с финансова история! |
| **`ON DELETE SET NULL`** | **`.OnDelete(DeleteBehavior.SetNull)`** | Изтрива родителя, а в дъщерната таблица слага **`NULL`** на външния ключ (`UserId = null`). | Когато пазиш поръчките за статистика без потребител *(изисква `int? UserId` — Nullable FK)*. |

> **Важен капан при `DeleteBehavior` в EF Core + MS SQL Server:**  
> Когато имаш задължителен външен ключ (`public int UserId { get; set; }`), EF Core слага **`DeleteBehavior.Cascade` по подразбиране**. Ако имаш две връзки с `Cascade` към една и съща таблица, при `Update-Database` MS SQL Server ще гръмне с грешка: `Introducing FOREIGN KEY constraint ... may cause cycles or multiple cascade paths`.  
> **Решението:** За всички финансови и бизнес връзки изрично слагай **`.OnDelete(DeleteBehavior.Restrict)`** в `OnModelCreating` (или ползвай *Soft Delete* с колона `bool IsDeleted`).

---

## 2. DBCONTEXT И FLUENT API (MODELBUILDER) — ВРЪЗКИ 1:1, 1:MANY И MANY:MANY

В ASP.NET Core най-чистият и професионален начин да опишеш релациите (вместо да разчиташ само на атрибути `[ForeignKey]`) е в метода **`OnModelCreating(ModelBuilder builder)`** на твоя `DbContext`:

```csharp
// ============================================================
// 1. ENTITY КЛАСОВЕ (МОДЕЛИ НА ТАБЛИЦИТЕ)
// ============================================================
public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;

    // 1:1 навигация към профил:
    public UserProfile? Profile { get; set; }

    // 1:Many навигация към поръчки:
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class UserProfile
{
    public int UserId { get; set; } // Едновременно PK и FK (гарантира 1:1!)
    public string Bio { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Order
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Many:Many навигация към продукти (чрез Mapping таблица OrderProduct):
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}

// Mapping таблица за Many:Many (с допълнителна колона Quantity!):
public class OrderProduct
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
```

```csharp
// ============================================================
// 2. DBCONTEXT И MODELBUILDER (FLUENT API КОНФИГУРАЦИЯ)
// ============================================================
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Връзка 1:1 (User <-> UserProfile)
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.UserId); // UserId е Primary Key
            entity.HasOne(p => p.User)
                  .WithOne(u => u.Profile)
                  .HasForeignKey<UserProfile>(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // Изтрие ли се User, трие се и профилът
        });

        // 2. Връзка 1:Many (User -> Orders)
        builder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2); // DECIMAL(18,2)
            entity.HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Restrict); // Забранява триене на клиент с поръчки!
        });

        // 3. Връзка Many:Many с междинна таблица (Order <-> OrderProduct <-> Product)
        builder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(op => new { op.OrderId, op.ProductId }); // Съставен PK (Composite Key)

            entity.HasOne(op => op.Order)
                  .WithMany(o => o.OrderProducts)
                  .HasForeignKey(op => op.OrderId);

            entity.HasOne(op => op.Product)
                  .WithMany(p => p.OrderProducts)
                  .HasForeignKey(op => op.ProductId);
        });
    }
}
```

---

## 3. КАК СЕ ПИШАТ INNER JOIN И LEFT JOIN В EF CORE / LINQ

В EF Core **не пишеш ключовата дума `join` в 95% от случаите**, защото използваш навигационните свойства (`o.User.FullName`), а EF сам преценява дали да генерира `INNER JOIN` или `LEFT JOIN`:

### А. Автоматичен `INNER JOIN` чрез `.Select()` (Най-бързият начин!)
Когато `Order.UserId` е задължителен (`int`, не `int?`), достъпът до `o.User.FullName` генерира чист `INNER JOIN`:

```csharp
// Генерира: SELECT o.Id, o.TotalAmount, u.FullName FROM Orders o INNER JOIN Users u ON o.UserId = u.Id
var ordersList = await _db.Orders
    .AsNoTracking()
    .Select(o => new 
    {
        OrderId = o.Id,
        o.TotalAmount,
        CustomerName = o.User.FullName
    })
    .ToListAsync();
```

### Б. Автоматичен `LEFT JOIN` чрез `.Include()` или навигация върху колекция
Когато тръгнеш от `Users` и поискаш броя или сумата на поръчките им, EF Core генерира `LEFT JOIN`, за да запази и потребителите без поръчки:

```csharp
// Генерира LEFT JOIN и връща всички потребители (дори тези с 0 поръчки!):
var userSummaries = await _db.Users
    .AsNoTracking()
    .Select(u => new
    {
        u.FullName,
        OrdersCount = u.Orders.Count(),
        TotalSpent = u.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0m
    })
    .ToListAsync();
```

### В. Ръчен `INNER JOIN` и `LEFT JOIN` в LINQ (когато нямаш навигационни свойства)
```csharp
// 1. Ръчен INNER JOIN:
var innerResult = await _db.Orders
    .Join(_db.Users,
          order => order.UserId,
          user => user.Id,
          (order, user) => new { order.Id, user.FullName, order.TotalAmount })
    .ToListAsync();

// 2. Ръчен LEFT JOIN (GroupJoin + SelectMany + DefaultIfEmpty):
var leftResult = await (
    from u in _db.Users
    join o in _db.Orders on u.Id equals o.UserId into userOrders
    from o in userOrders.DefaultIfEmpty() // Това превръща връзката в LEFT JOIN!
    select new
    {
        u.FullName,
        OrderTotal = o != null ? o.TotalAmount : 0m
    }).ToListAsync();
```

---

## 4. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Cartesian Explosion (Експлозия на редовете) при множество `.Include()`
* **Какво се случва:** Пишеш `_db.Users.Include(u => u.Orders).Include(u => u.Reviews).ToListAsync()`. EF Core генерира един гигантски `LEFT JOIN` и по двете колекции едновременно, което умножава редовете и прехвърля огромно количество дублирани данни по мрежата.
* **Как да избегнеш:** Когато зареждаш цели графи от обекти с 2+ колекции, добави **`.AsSplitQuery()`** — EF Core ще изпълни 3 малки, светкавични `SELECT` заявки вместо 1 гигантска.

### Грешка 2: Използване на `.Include()` когато ти трябват само 2 колони
* **Какво се случва:** Пишеш `_db.Orders.Include(o => o.User).ToListAsync()`, само за да покажеш името на клиента в таблица. Така теглиш всички 20 колони и на `Orders`, и на `Users` (включително `PasswordHash`!).
* **Как да избегнеш:** За четене на списъци винаги използвай **`.Select(o => new MyDto(...))`** — при проекция с `.Select()` изобщо не ти трябва `.Include()`, защото EF Core автоматично прави правилния `JOIN` и взима само посочените колони!

### Грешка 3: Липсваща прецизност на `decimal` в `ModelBuilder`
* **Какво се случва:** Ако имаш `public decimal Price { get; set; }` и не зададеш `.HasPrecision(18, 2)` в `OnModelCreating`, EF Core хвърля предупреждение и при миграция може да отреже стотинките или да създаде грешен тип в базата данни.
* **Как да избегнеш:** Винаги конфигурирай валутните и дробните свойства в `ModelBuilder` чрез `.HasPrecision(18, 2)` или задай глобално правило за всички `decimal` свойства в `DbContext`:
  ```csharp
  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
      // Всички decimal свойства в целия проект стават DECIMAL(18, 2) автоматично!
      configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
  }
  ```

---
⬅️ [04-06. OLTP срещу OLAP и Data Warehouses](04-06-OLTP-vs-OLAP-And-Data-Warehouses.md) | 📂 [Глава 04](04-00-SQL-PERFORMANCE-AND-PROGRAMMABILITY.md) | 🏠 [Начало (Библиотека БД)](README.md) | ⏩ [00-03. Архитектура, Бекъпи и Инструменти](00-03-DBMS-Architecture-Backups-And-Tools.md)

© 2026 Dev-Library / Бази данни: EF Core Релации, ModelBuilder и Joins

