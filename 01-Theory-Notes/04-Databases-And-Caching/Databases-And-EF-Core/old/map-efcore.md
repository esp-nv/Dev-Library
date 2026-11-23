````text
карта: „как да мисля, когато създавам Entity и mapping в EF Core“
🧠 1. Основният начин на мислене

Когато имам задача да свържа C# клас с база:

C# class
   ↓
Entity
   ↓
Mapping
   ↓
Database table


Не започвам директно да пиша код.

Минавам стъпка по стъпка:

Какъв Entity имам?
      ↓
Коя таблица представлява?
      ↓
Имената съвпадат ли?
      ↓
Как ще направя mapping?
      ↓
Кои properties има?
      ↓
Кои columns им съответстват?
      ↓
Как ще ги конфигурирам?
      ↓
DbSet
      ↓
LINQ

🧩 2. Entity — C# класът

Entity е C# клас, който представлява обект от домейна и който EF Core може да свърже с таблица в базата.

Например:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}


Мисля:

Product
 ├── Id
 ├── Name
 └── Price


Тоест:

Product описва как изглежда един Product обект в C#.

🔗 3. Mapping — „преводачът“

Mapping означава:

казваме на EF Core кое в C# на кое в Database съответства.

Можем да го мислим като мост:

C#
 ↓
Mapping
 ↓
Database


Например:

Product
   ↓
store_products


и:

Name
   ↓
product_name


Тоест:

Entity → Table
Property → Column

🗂️ 4. Entity → Table

Това е mapping между:

C# Entity
     ↓
Database Table


Например:

Product → store_products


Ако използвам Fluent API:

modelBuilder.Entity<Product>()
    .ToTable("store_products");


Мисля:

„Entity Product се съхранява в таблицата store_products.“

📌 5. Property → Column

Това е mapping между:

C# Property
     ↓
Database Column


Например:

Product.Name → product_name


С Fluent API:

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .HasColumnName("product_name");


Мисля:

„Property-то Name съответства на колоната product_name.“

🔄 6. Трите начина за Mapping

Имам три основни варианта:

1. Convention / Auto Mapping
2. Attributes
3. Fluent API

🤖 7. Convention / Auto Mapping

Тук не казвам изрично на EF Core какво с какво да свърже.

Оставям го да използва conventions.

Например:

Product → Products

Id    → Id
Name  → Name
Price → Price


Ако имената и структурата са стандартни, EF Core може сам да разбере mapping-а.

Мисли:

„Имената съвпадат → EF Core вероятно може да се досети сам.“

Кога е най-удобен?

Когато:

Entity и Table са стандартно именувани;
Property и Column са стандартно именувани;
няма специална конфигурация.
Най-важното:
Convention / Auto
→ най-малко код
→ използвам, когато всичко е стандартно

🏷️ 8. Attributes

При Attributes казвам mapping-а директно върху class/property.

Например:

[Table("store_products")]
public class Product
{
    [Column("product_id")]
    public int Id { get; set; }

    [Column("product_name")]
    public string Name { get; set; }

    [Column("product_price")]
    public decimal Price { get; set; }
}


Тук:

[Table]
   ↓
Entity → Table


а:

[Column]
   ↓
Property → Column


Например:

[Column("product_name")]
public string Name { get; set; }


означава:

Name → product_name

Кога е удобен?

Когато:

mapping-ът е прост;
имам малко configuration;
искам да виждам mapping-а директно върху Entity-то.
Мисли:

„Искам да видя mapping-а точно там, където е class/property-то.“

🧱 9. Fluent API

При Fluent API държа Entity класа чист:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}


А configuration-а го пиша отделно:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .ToTable("store_products");

    modelBuilder.Entity<Product>()
        .Property(p => p.Id)
        .HasColumnName("product_id");

    modelBuilder.Entity<Product>()
        .Property(p => p.Name)
        .HasColumnName("product_name");

    modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnName("product_price");
}

Мисли:
Entity
 ↓
описва обекта


Fluent API
 ↓
описва как обектът се свързва с Database

Кога е най-удобен?

Особено когато:

mapping-ът е по-сложен;
имам много configuration;
имам relationships;
имам keys;
имам constraints;
искам configuration-ът да е отделен;
работя по по-голям проект.
Нашият избор

Разбираме и трите, но ще упражняваме основно Fluent API.

⚖️ 10. Трите начина един до друг
Convention / Auto
Product → Products
Name    → Name
Price   → Price


Нищо специално не пиша.

Attributes
[Table("store_products")]
public class Product
{
    [Column("product_name")]
    public string Name { get; set; }
}

Fluent API
modelBuilder.Entity<Product>()
    .ToTable("store_products");

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .HasColumnName("product_name");


И трите начина имат една цел:

C# → Database


Разликата е как казвам на EF Core това.

🧠 11. Как да избера?

Първо питам:

Имената съвпадат ли?

Да:
Convention / Auto

Не:
Трябва explicit mapping.


После:

Attributes или Fluent API?

Attributes:
прост mapping
↓
искам го върху Entity-то

Fluent API:
по-сложен mapping
↓
искам configuration отделно


За нашата практика:

Convention → разбираме
Attributes → разбираме
Fluent API → основният ни начин за писане

🏗️ 12. OnModelCreating

OnModelCreating е мястото, където конфигурираме как EF Core да разбира нашите модели.

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // configuration
}


Мисли:

OnModelCreating
      ↓
modelBuilder
      ↓
конфигурирам Entity-тата


Не създавам самия C# клас там.

Класът вече съществува.

Тук казвам:

„EF Core, ето как искам да разбираш този Entity.“

🔨 13. modelBuilder

modelBuilder е инструментът, чрез който описвам configuration-а.

Например:

modelBuilder.Entity<Product>()


означава:

„Сега конфигурирам Entity Product.“

После мога да сляза към таблицата:

.ToTable("store_products");


или към property:

.Property(p => p.Name)

🪜 14. Fluent API — от общото към частното

Това е много полезен начин за мислене:

Product Entity
      ↓
Table
      ↓
Property
      ↓
Column


Например:

modelBuilder.Entity<Product>()
    .ToTable("store_products");

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .HasColumnName("product_name");


Мисля:

Кой Entity?
    ↓
Коя Table?
    ↓
Кое Property?
    ↓
Коя Column?

📋 15. Проверка на всички properties

Ако имам:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}


си правя:

Product
 ├── Id
 ├── Name
 └── Price


После проверявам Database:

store_products
 ├── product_id
 ├── product_name
 └── product_price


И правя:

Id    → product_id
Name  → product_name
Price → product_price


Това е добър навик, защото намалява вероятността да пропусна нещо.

⚠️ 16. Не е задължително да конфигурирам всяко Property

Например:

Price → Price


Ако имената съвпадат и няма специални правила, Convention може да свърши работата.

Тоест не е нужно винаги да пиша:

.Property(p => p.Price)
.HasColumnName("Price");


Мога да оставя EF Core да използва convention.

Мисли:

„Конфигурирам изрично това, което трябва да бъде различно или специално.“

📦 17. DbSet<Product>

В DbContext:

public DbSet<Product> Products { get; set; }


Product:

един Product Entity


DbSet<Product>:

набор от Product entities


Мисли:

Products
 ├── Product
 ├── Product
 ├── Product
 └── ...

🌉 18. DbContext

DbContext е основната връзка между приложението и Database.

В него имаме:

DbContext
   │
   ├── DbSet<Product>
   │
   └── Configuration
          ↓
     OnModelCreating


Мисли:

DbContext е мястото, където EF Core управлява Entity-тата, тяхната configuration и работата с Database.

🔎 19. context.Products

Ако имаме:

public DbSet<Product> Products { get; set; }


можем да напишем:

context.Products


Мисли:

„Работи с набора от Product entities.“

Все още не сме филтрирали.

🔍 20. Where()
context.Products
    .Where(p => p.Price > 60);


Where означава:

Филтрирай.

Оставя само елементите, за които условието е true.

Products
   ↓
Where(p => p.Price > 60)
   ↓
само Products с Price > 60

🧠 21. Lambda expression
p => p.Price > 60


Мисля:

p
↓
един Product

p.Price
↓
Price на този Product

p.Price > 60
↓
условие

true / false


Например:

Keyboard
80 > 60
↓
true

Mouse
25 > 60
↓
false


Следователно:

true  → остави
false → не оставяй

🔗 22. LINQ веригата

Например:

context.Products
    .Where(p => p.Price > 60)
    .Select(p => p.Name);


Не гледам всичко наведнъж.

Минавам:

context.Products
      ↓
всички Product entities
      ↓
Where
      ↓
само Price > 60
      ↓
Select
      ↓
вземи Name


Основният въпрос е:

Какво влиза в метода и какво излиза от него?

🔄 23. EF Core — преводът към SQL

Ние пишем:

context.Products
    .Where(p => p.Price > 60);


EF Core превежда заявката към SQL.

При нашия пример идеята е:

SELECT *
FROM store_products
WHERE product_price > 60;


Мислим:

C# / LINQ
    ↓
EF Core
    ↓
SQL
    ↓
Database

🧭 24. Целият път
Product class
      ↓
Entity
      ↓
Mapping
      ↓
DbContext
      ↓
DbSet<Product>
      ↓
LINQ
      ↓
EF Core
      ↓
SQL
      ↓
Database

📝 25. Практична последователност при създаване

Когато започвам нов Entity:

1. Създавам Entity class
       ↓
2. Проверявам Database table
       ↓
3. Проверявам имената
       ↓
4. Избирам Mapping:
       ├── Convention
       ├── Attributes
       └── Fluent API
       ↓
5. Entity → Table
       ↓
6. Property → Column
       ↓
7. Проверявам всички properties
       ↓
8. Добавям configuration
       ↓
9. Добавям DbSet
       ↓
10. Проверявам целия mapping
       ↓
11. Пиша LINQ
       ↓
12. EF Core → SQL

⚠️ 26. Най-честите капани
Капан №1 — да объркам Entity с Table
Product


е C# Entity.

store_products


е Database Table.

Капан №2 — да объркам Property с Column
Name


е C# Property.

product_name


е Database Column.

Капан №3 — да мисля, че Mapping е само Property → Column

Mapping може да бъде:

Entity → Table


и:

Property → Column


Има и други mapping-и, които ще учим по-късно.

Капан №4 — да използвам explicit mapping, когато Convention е достатъчен

Ако:

Name → Name


няма нужда задължително да пиша:

.HasColumnName("Name");

Капан №5 — да мисля, че DbSet е един Product
Product
→ един Entity

DbSet<Product>
→ набор от Product entities

Капан №6 — да мисля, че Where променя Product
Where(...)


не променя
