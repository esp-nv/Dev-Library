````text
EF Core — Чеклист за утре
1. Entity

Мога ли да обясня:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}


→ Това е C# клас / Entity.

2. Mapping

Мога ли да кажа какво е mapping?

Mapping = казваме на EF Core кое от C# на кое от Database съответства.

Пример:

Product → store_products
Name    → product_name

3. Трите начина за Mapping
Convention / Auto Mapping

Питам се:

Съвпадат ли имената?

Product → Products
Name    → Name
Price   → Price


Ако да → EF Core може сам да направи mapping-а.

Attributes

Знам:

[Table("store_products")]


→ Entity → Table

И:

[Column("product_name")]


→ Property → Column

Fluent API

Знам:

modelBuilder.Entity<Product>()
    .ToTable("store_products");


→ Entity → Table

И:

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .HasColumnName("product_name");


→ Property → Column

4. Кога кой използвам?

Запомням:

Имената съвпадат
      ↓
Convention / Auto

Прост mapping
      ↓
Attributes

По-сложна конфигурация
      ↓
Fluent API


За нашата практика:

Разбирам и трите → упражнявам основно Fluent API.

5. OnModelCreating

Мога ли да обясня:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
}


→ Мястото, където конфигурираме как EF Core да разбира Entity-тата и връзката им с Database.

6. DbContext

Мога ли да обясня:

DbContext е основният мост между приложението и Database и управлява Entity-тата, DbSet-овете и configuration-а.

7. DbSet

Знам:

public DbSet<Product> Products { get; set; }


→ набор/колекция от Product entities.

И:

context.Products


→ работя с този набор от Product entities.

8. LINQ — Where

Знам:

context.Products
    .Where(p => p.Price > 60);


→ филтрирам Product-ите.

9. Lambda expression

Мога да разбия:

p => p.Price > 60


на:

p
↓
един Product

p.Price
↓
Price property

p.Price > 60
↓
true / false


И:

true  → остава
false → отпада

10. EF Core → SQL

Мога да обясня:

C# / LINQ
    ↓
EF Core
    ↓
SQL
    ↓
Database


Тоест:

EF Core превежда LINQ заявката към SQL, който Database разбира.

⭐ Основна карта

Утре трябва да мога да разкажа това сама:

1. Създавам Entity
        ↓
2. Проверявам Table
        ↓
3. Проверявам имената
        ↓
4. Избирам Mapping:
   Auto / Attributes / Fluent API
        ↓
5. Entity → Table
        ↓
6. Property → Column
        ↓
7. Configuration в OnModelCreating
        ↓
8. DbSet
        ↓
9. LINQ
        ↓
10. EF Core → SQL
        ↓
11. Database

🎯 Цел за утре

Не е нужно да помня кода наизуст.

Трябва да мога да обясня логиката:

Entity → Mapping → DbContext → DbSet → LINQ → EF Core → SQL → Database

А кода ще го изградим стъпка по стъпка, както правихме днес.
