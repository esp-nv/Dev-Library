````text
EF Core — карта стъпка по стъпка
Общата картина
C# Class
   ↓
Entity
   ↓
Mapping
   ↓
DbContext
   ↓
Table + Columns
   ↓
DbSet
   ↓
LINQ
   ↓
EF Core
   ↓
SQL
   ↓
Database

ЕТАП 1 — Създавам Entity

Първо създавам C# клас.

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


Въпросът ми е:

Какъв обект искам да представлявам?

ЕТАП 2 — Проверявам как ще се направи Mapping

Тук имам 3 начина:

1. Convention / Auto Mapping
2. Attributes
3. Fluent API

1. Convention / Auto Mapping
Какво означава?

Оставям EF Core сам да направи mapping-а, когато имената и структурата следват неговите conventions.

Например:

C#                         Database

Product      ─────────→    Products

Id           ─────────→    Id
Name         ─────────→    Name
Price        ─────────→    Price


Ако всичко съвпада, може да не пиша никаква специална конфигурация.

Кога е най-удобен?

Използвам го, когато:

имената на Entity и Table са стандартни;
имената на Properties и Columns съвпадат;
нямам специални правила;
искам по-малко код.
Мисловно правило:

„Ако всичко е стандартно и съвпада → оставям EF Core сам да го разбере.“

2. Attributes

Ако имената не съвпадат, мога да използвам Attributes.

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


Тук казвам:

Product → store_products

Id    → product_id
Name  → product_name
Price → product_price

Кога са най-удобни?

Attributes са удобни, когато:

mapping-ът е сравнително прост;
искам да виждам mapping-а директно върху Entity-то;
имам малко конфигурация;
работя по малък/прост проект.
Мисловно правило:

„Искам да видя mapping-а директно върху property-то/класа → Attributes.“

3. Fluent API

При Fluent API не слагам mapping-а върху Entity класа.

Entity-то остава чисто:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}


А mapping-ът е в OnModelCreating:

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


Получаваме:

Product → store_products

Id    → product_id
Name  → product_name
Price → product_price

Кога е най-удобен?

Fluent API е особено удобен, когато:

mapping-ът е по-сложен;
имам много configuration;
имам relationships;
имам keys и constraints;
искам configuration-ът да е отделен от Entity класа;
работя по по-голям проект;
искам всички правила за базата да са организирани на едно място.
Мисловно правило:

„Искам Entity-то да описва обекта, а configuration-ът да е отделно → Fluent API.“

Трите начина — накратко
Начин	Какво прави?	Кога е удобен?
Convention / Auto	EF Core сам разбира mapping-а	Когато имената и структурата съвпадат
Attributes	Mapping директно върху class/property	Когато mapping-ът е прост и искам да го виждам на място
Fluent API	Mapping чрез ModelBuilder	При по-сложен mapping и по-големи проекти
Моето практично правило
Имената съвпадат?
        ↓
      ДА
        ↓
Convention / Auto Mapping


Ако не:

Имената НЕ съвпадат
        ↓
Трябва explicit mapping
        ↓
   ┌───────────────┐
   │               │
Attributes     Fluent API


За нашето обучение:

Ще разбираме и трите, но ще упражняваме основно Fluent API.

ЕТАП 3 — Entity → Table

След като знам кой mapping подход използвам, проверявам коя таблица представлява Entity-то.

Например:

Product → store_products


При Fluent API:

modelBuilder.Entity<Product>()
    .ToTable("store_products");


Мисля:

Entity
  ↓
Table

Product → store_products

ЕТАП 4 — Property → Column

След това проверявам всяко property едно по едно.

Entity:

Product
 ├── Id
 ├── Name
 └── Price


Database:

store_products
 ├── product_id
 ├── product_name
 └── product_price


Mapping:

Id    → product_id
Name  → product_name
Price → product_price


С Fluent API:

modelBuilder.Entity<Product>()
    .Property(p => p.Id)
    .HasColumnName("product_id");

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .HasColumnName("product_name");

modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasColumnName("product_price");


Мисля:

За всяко property — коя е неговата колона?

ЕТАП 5 — Проверявам mapping-а

Правя си проверка:

Product
 │
 ├── Id    → product_id       ✓
 ├── Name  → product_name     ✓
 └── Price → product_price    ✓


И:

Entity → Table

Product → store_products      ✓


Това е моят checklist.

ЕТАП 6 — Слагам Fluent API configuration-а в OnModelCreating
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


Мисля:

OnModelCreating
      ↓
modelBuilder
      ↓
Entity
      ↓
Table
      ↓
Properties
      ↓
Columns

ЕТАП 7 — Добавям DbSet

В DbContext добавям:

public DbSet<Product> Products { get; set; }


Мисля:

DbSet<Product>
      ↓
набор от Product entities


И:

context.Products


означава:

Работи с набора от Product entities.

ЕТАП 8 — Проверявам DbContext

Крайната основа изглежда така:

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

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
}

ЕТАП 9 — Пиша LINQ заявката

След като Entity, mapping и DbSet са готови, мога да работя с данните:

context.Products


Ако искам само продуктите над 60:

context.Products
    .Where(p => p.Price > 60);


Мисля:

context
   ↓
Products
   ↓
Where
   ↓
условие

ЕТАП 10 — Разбирам Lambda expression
p => p.Price > 60


p представлява един Product.

p
↓
Product


p.Price:

Product.Price
↓
Price property


p.Price > 60:

80 > 60 → true
25 > 60 → false


Следователно:

true  → остава
false → отпада

ЕТАП 11 — EF Core превежда LINQ към SQL

Аз пиша:

context.Products
    .Where(p => p.Price > 60);


EF Core превежда идеята към SQL:

SELECT *
FROM store_products
WHERE product_price > 60;


Общата схема е:

C# / LINQ
    ↓
EF Core
    ↓
SQL
    ↓
Database

МОЯТ БЪРЗ CHECKLIST

Когато започвам нов Entity, минавам през това:

□ 1. Създадох C# class / Entity

□ 2. Знам коя Database table представлява

□ 3. Проверих дали Convention / Auto Mapping
     може да свърши работа

□ 4. Ако не може — избирам explicit mapping:
       Attributes или Fluent API

□ 5. Направих Entity → Table mapping

□ 6. Проверих всички properties

□ 7. Направих Property → Column mapping,
     където е необходимо

□ 8. Ако използвам Fluent API —
     сложих configuration-а в OnModelCreating

□ 9. Добавих DbSet<Entity>

□ 10. Проверих mapping-а:
        Entity → Table
        Property → Column

□ 11. Мога да използвам context.DbSet

□ 12. Пиша LINQ заявката

□ 13. EF Core превежда LINQ → SQL

НАЙ-ВАЖНОТО ЗА ЗАПОМНЯНЕ
Трите начина за Mapping
1. Convention / Auto
   ↓
   EF Core разбира сам

2. Attributes
   ↓
   [Table(...)]
   [Column(...)]

3. Fluent API
   ↓
   .ToTable(...)
   .HasColumnName(...)

Entity → Table
Product → store_products

Property → Column
Name → product_name
Price → product_price

DbContext
управлява Entity-тата,
configuration-а и работата с Database

DbSet
DbSet<Product>
→ набор от Product entities

Where
филтрира

Lambda
p => p.Price > 60
→ условие, което дава true / false

EF Core
LINQ → SQL

МОЯТА ОСНОВНА МИСЛОВНА КАРТА
                 C# APPLICATION
                       │
                       ▼
                    Product
                    Entity
                       │
                       ▼
                   MAPPING
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
     Convention    Attributes    Fluent API
       / Auto                       │
          │                         │
          └────────────┬────────────┘
                       ▼
                  DbContext
                       │
              ┌────────┴────────┐
              ▼                 ▼
          DbSet<Product>    OnModelCreating
              │                 │
              │                 ▼
              │              Mapping
              │                 │
              │          ┌──────┴──────┐
              │          ▼             ▼
              │        Table        Columns
              │          │             │
              │          ▼             ▼
              │   store_products   product_name
              │                    product_price
              │
              ▼
            LINQ
              │
              ▼
          EF Core
              │
              ▼
             SQL
              │
              ▼
           DATABASE

ПРАВИЛО ЗА РАБОТА

Не прескачам.

Винаги мисля:

1. Какъв Entity ми трябва?
        ↓
2. Коя Table представлява?
        ↓
3. Имената съвпадат ли?
        ↓
4. Ако да → Convention / Auto
        ↓
5. Ако не → Attributes или Fluent API
        ↓
6. Entity → Table
        ↓
7. Properties → Columns
        ↓
8. Проверявам дали не съм пропуснала нещо
        ↓
9. Добавям DbSet
        ↓
10. Пиша LINQ
        ↓
11. EF Core → SQL
        ↓
12. Database

МОЕТО ПРАКТИЧНО ПРАВИЛО ЗА ИЗБОР
             Имената съвпадат?
                    │
              ┌─────┴─────┐
             ДА           НЕ
              │             │
              ▼             ▼
       Convention /      Explicit
          Auto            Mapping
                            │
                     ┌──────┴──────┐
                     ▼             ▼
                 Attributes    Fluent API

Най-просто казано:

Convention / Auto → когато EF Core може сам да се досети.

Attributes → когато mapping-ът е прост и искам да го виждам директно върху Entity-то.

Fluent API → когато искам configuration-ът да е отделен и особено когато mapping-ът става по-сложен.

За нашата практика:

Разбираме и трите → използваме основно Fluent API.
