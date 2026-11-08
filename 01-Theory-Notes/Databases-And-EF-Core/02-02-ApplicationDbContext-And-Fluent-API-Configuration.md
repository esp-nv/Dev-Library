[⬅️ 02-01. Подходи за Mapping](02-01-EF-Core-Mapping-Approaches.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [⬅️ 02-03. Глобален Пищов](02-03-EF-Core-Fluent-API-And-Migrations-CheatSheet.md) 

---

# 02-02. ApplicationDbContext и Fluent API конфигурация

**Цел:** Практическо конфигуриране на връзката към SQL Server през външен `appsettings.json` файл, инжектиране на `IConfiguration` настройки в жизнения цикъл на `DbContext` и дълбоко разбиране на LINQ-към-SQL транслацията.

## Съдържание на темата
* [1. Настройка на връзката чрез appsettings.json (Connection String)](#1-настройка-на-връзката-чрез-appsettingsjson-connection-string)
* [2. Прочитане на Connection String и инжектиране в DbContext](#2-прочитане-на-connection-string-и-инжектиране-в-dbcontext)
* [3. LINQ вериги и разглобяване на Lambda изразите](#3-linq-вериги-и-разглобяване-на-ламбда-изразите)
* [4. Чести грешки, Капани и Как да ги избегнем](#4-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. НАСТРОЙКА НА ВРЪЗКАТА ЧРЕЗ APPSETTINGS.JSON (CONNECTION STRING)

В реалната софтуерна практика е абсолютно забранено адресът на базата данни (Connection String) да бъде твърдо кодиран (хардкоднат) в C# файловете. Ако сървърът или паролата се сменят в продукция, това ще изисква прекомпилиране на целия софтуер. Затова данните се изнасят в конфигурационния `appsettings.json` файл в уеб проекта.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=Localhost;Database=MySchoolDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Анатомия на параметрите в Connection String-а:
*   **`Server=Localhost;`** (или `Server=.\SQLEXPRESS;`): Указва къде физически се намира SQL Server. Точката `.` или `Localhost` означава текущата локална машина.
*   **`Database=MySchoolDb;`**: Името на базата данни, която Entity Framework Core ще създаде автоматично на диска за този конкретен проект.
*   **`Trusted_Connection=True;`** (или `Integrated Security=True;`): Използва текущия Windows/ОС акаунт на програмиста за автентификация. Няма нужда от писане на потребителски имена и пароли.
*   **`TrustServerCertificate=True;`**: Изключително важен съвременен параметър. Казва на приложението да се довери на локалния криптографски сертификат на SQL Server, предотвратявайки SSL/TLS грешки при първоначално свързване в .NET средите.

---
### 🧠 МИСЛОВНА КАРТА: ОТ FLUENT API КЪМ СУРОВ SQL (РЕД ПО РЕД)

Ето как се мапва софтуерната верига от C# до физическия SQL Server:

```csharp
modelBuilder.Entity<Product>()                   // ➔ SQL: CREATE TABLE [име]
    .ToTable("store_products");                  // ➔ SQL: ... store_products (Преименува таблицата)

modelBuilder.Entity<Product>()                   // ➔ SQL: (Конфигурираме колона вътре в таблицата)
    .Property(p => p.Price)                      // ➔ SQL: Взима C# свойството Price...
    .HasColumnName("product_price")              // ➔ SQL: ... и го преименува на колона [product_price]
    .HasColumnType("decimal(18,2)")              // ➔ SQL: ... със строго фиксиран тип DECIMAL(18,2)
    .HasDefaultValue(0.00)                       // ➔ SQL: DEFAULT 0.00 (Ако няма стойност, записва 0.00)
    .IsRequired();                               // ➔ SQL: NOT NULL (Полето става задължително!)
```

### 🔁 ДВУПОСОЧЕН ПРЕВОД (СЪПОСТАВКА В ТАБЛИЦА)

| Fluent API Метод в C# | Еквивалентна клауза в SQL Server | Какво се случва физически на диска? |
| :--- | :--- | :--- |
| `.ToTable("store_products")` | `CREATE TABLE store_products` | Преименува таблицата (по подразбиране е `Products`). |
| `.HasKey(p => p.Id)` | `PRIMARY KEY (Id)` | Създава първичен ключ и автоматично прави `Clustered Index`. |
| `.Property(p => p.Name)` | `[Name]` | Избира конкретното C# свойство за конфигурация. |
| `.HasColumnName("product_name")` | `product_name` | Променя заглавието на самата колона в SQL Server. |
| `.HasMaxLength(50)` | `NVARCHAR(50)` | Налага твърд лимит в байтове на диска (пази от препълване). |
| `.IsRequired()` | `NOT NULL` | Забранява празни (`NULL`) стойности за този запис. |
| `.IsRequired(false)` | `NULL` | Позволява колоната да бъде празна и незадължителна. |
| `.HasDefaultValue(true)` | `DEFAULT 1` | SQL автоматично записва `1`, ако C# не подаде стойност. |

--- 
## 2. ПРОЧИТАНЕ НА CONNECTION STRING И ИНЖЕКТИРАНЕ В DBCONTEXT

За да прочетем данните от JSON файла, използваме интерфейса `IConfiguration` от пакета `Microsoft.Extensions.Configuration`. Той ни осигурява гъвкавост, като автоматично извлича параметрите по време на изпълнение на софтуера (Runtime):

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class SchoolDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    // Конструктор, който инжектира конфигурацията от appsettings.json
    public SchoolDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Student> Students { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Четем директно от JSON файла по име на ключа
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
```

---

## 3. LINQ ВЕРИГИ И РАЗГЛОБЯВАНЕ НА LAMBDA ИЗРАЗИТЕ

При работа с Entity Framework Core ние не пишем суров SQL текст в нашите сервизи, а изграждаме **LINQ (Language Integrated Query)** вериги, съчетани с Ламбда изрази:

```csharp
var expensiveProducts = context.Products
    .Where(p => p.Price > 60)
    .ToList();
```

### 🧠 Анатомия на Ламбда израза `p => p.Price > 60`:
*   **`p`**: Представлява **един единствен софтуерен обект** от тип `Product`, преминаващ през веригата.
*   **`=>`** (Goes to / Предай на): Оператор-стрелка, който подава текущия обект към логическото условие вдясно.
*   **`p.Price > 60`**: Бизнес израз, който дава стойност `true` или `false`. Ако за конкретния продукт условието е вярно, той се запазва в паметта. Ако даде `false` — отпада веднага.

### 🔄 Как EF Core превежда тази LINQ логика към суров SQL зад кулисите:
```sql
SELECT [product_id], [product_name], [product_price]
FROM store_products
WHERE product_price > 60.00;
```

### 🔗 FLUENT API КОНФИГУРАЦИЯ НА РЕЛАЦИИ (1:1, 1:МНОГО, МНОГО:МНОГО)

Ето как се описват трите вида софтуерни връзки вътре в метода `OnModelCreating` чрез `modelBuilder`:

#### 1. Връзка Едно към Много (1:Many) — Най-масовата релация
*   *Бизнес логика:* Един Студент (`Student`) има точно един Курс (`Course`), но един Курс може да има много Студенти (`List<Student>`).
*   *Къде е Foreign Key:* Външният ключ `CourseId` се намира винаги в таблицата на „Многото“ (при Студента).

```csharp
modelBuilder.Entity<Student>()
    .HasOne(s => s.Course)             // Студентът има ЕДИН Курс
    .WithMany(c => c.Students)         // Курсът има МНОГО Студенти
    .HasForeignKey(s => s.CourseId)    // Външният ключ в SQL е CourseId
    .OnDelete(DeleteBehavior.Restrict);// Забранява каскадно триене за счетоводна сигурност
```

#### 2. Връзка Едно към Едно (1:1)
*   *Бизнес логика:* Един Студент (`Student`) има точно един Детайлен профил (`StudentProfile`), и един Профил принадлежи само на един единствен Студент.
*   *Къде е Foreign Key:* Външният ключ се поставя в подчинената таблица (`StudentProfile`).

```csharp
modelBuilder.Entity<StudentProfile>()
    .HasOne(sp => sp.Student)          // Профилът има ЕДИН Студент
    .WithOne(s => s.Profile)           // Студентът има ЕДИН Профил
    .HasForeignKey<StudentProfile>(sp => sp.StudentId); // Указваме изрично в коя таблица е FK
```

#### 3. Връзка Много към Много (Many:Many)
*   *Бизнес логика:* Един Студент (`Student`) може да посещава много Лекции (`List<Lecture>`), и една Лекция може да бъде посетена от много различни Студенти (`List<Student>`).
*   *Как работи в SQL:* Автоматично се ражда скрита междинна таблица (Join Table), която съдържа две колони с външни ключове (`StudentId` и `LectureId`).

```csharp
modelBuilder.Entity<Student>()
    .HasMany(s => s.Lectures)          // Студентът има МНОГО Лекции
    .WithMany(l => n.Students)         // Лекцията има МНОГО Студенти
    .UsingEntity(j => j.ToTable("students_lectures")); // Преименува междинната таблица в SQL
```
#### 4. Разширен вариант на Много към Много (Many:Many) с допълнителни колони в междинната таблица
*   *Бизнес сценарий:* Един Студент има много Лекции, но искаме да пазим и **датата на записване (`EnrolledOn`)**.
*   *Решение:* Създаваме изричен C# клас `StudentLecture`, който работи като мост, съдържа двата външни ключа и допълнителното свойство.

```csharp
// 1. Изрично създадената междинна (мостова) таблица
public class StudentLecture
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;

    // Допълнителна бизнес колона, която базовият Many:Many подход не може да задели!
    public DateTime EnrolledOn { get; set; } 
}

// 2. Конфигурация във Fluent API (OnModelCreating)
modelBuilder.Entity<StudentLecture>()
    .HasKey(sl => new { sl.StudentId, sl.LectureId }); // ➔ SQL: Съставен първичен ключ (Composite PK)

modelBuilder.Entity<StudentLecture>()
    .HasOne(sl => sl.Student)
    .WithMany(s => s.StudentLectures) // Студентът има колекция List<StudentLecture>
    .HasForeignKey(sl => sl.StudentId);

modelBuilder.Entity<StudentLecture>()
    .HasOne(sl => sl.Lecture)
    .WithMany(l => l.StudentLectures) // Лекцията има колекция List<StudentLecture>
    .HasForeignKey(sl => sl.LectureId);
```

#### 5. Глобална защита: Спиране на Каскадното триене (Cascade Delete Restriction)
*   *Бизнес сценарий:* По подразбиране в SQL, ако изтриеш `Region`, базата данни автоматично ще изтрие абсолютно всички градове, клиенти и финансови трансакции в него. Това е фатално за бизнеса.
*   *Решение:* Вместо да пишеш ръчно `.OnDelete(DeleteBehavior.Restrict)` на стотици места, налагаме едно железно глобално правило най-отдолу в метода `OnModelCreating`:

```csharp
// Претърсва абсолютно всички Foreign Keys (връзки), дефинирани в проекта
foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
{
    // Подменя стандартното поведение на EF Core към Restrict (Забрана за изтриване)
    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
}
```

---

## 🛠️ QUICK FLUENT API CHEAT-SHEET (ПИЩОВ ЗА ИНТЕРВЮ)

Ето как да асоциираш и запомниш трите най-важни концепции в EF Core за 5 секунди:

### 1. Трите вълшебни думи за Връзки (Relationships)
```text
  Has[Нещо] ───> С какво разполага ТЕКУЩАТА таблица (Едно или Много)
  With[Нещо] ──> С какво разполага СЪСЕДНАТА таблица (Едно или Много)
  HasForeignKey ➔ Коя точно C# колона държи физическия Външен ключ в SQL
```

### 2. Матрица за избор на DeleteBehavior
*   **`DeleteBehavior.Cascade`** ➔ *„Пада домино“* (Изтриваш родителя ➔ базата трие автоматично и децата). **Опасен за финансови системи!**
*   **`DeleteBehavior.Restrict`** ➔ *„Желязна стена“* (SQL Server отказва да изтрие родителя, ако има макар и едно дете). **Златен стандарт за сигурност.**
*   **`DeleteBehavior.SetNull`** ➔ *„Остават сираци“* (Изтриваш родителя ➔ в колоната на децата се записва `NULL`).

### 3. Бърз синтактичен пищов за релации
*   **1 : Много** ➔ `.HasOne().WithMany().HasForeignKey()`
*   **1 : 1** ➔ `.HasOne().WithOne().HasForeignKey<SubTable>()`
*   **Много : Много (Базов)** ➔ `.HasMany().WithMany().UsingEntity()`
*   **Много : Много (Мост)** ➔ Разбива се на две отделни **1 : Много** връзки през междинен клас с композитен ключ: `modelBuilder.Entity<Bridge>().HasKey(x => new { x.Id1, x.Id2 });`


---
## 4. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Капан 1: Тотален срив с SqlException поради забравен `Update-Database`
*   **Какво се случва:** Добавяш ново свойство `public string SecondaryEmail { get; set; }` в C# класа. Стартираш проекта, опитваш се да запишеш или прочетеш потребител и приложението забива моментално с тежък crash.
*   **Грешка на екрана:** `SqlException: Invalid column name 'SecondaryEmail'.`
*   **Защо се случва:** Ти си направила промяна в C# кода, но си забравила да изпълнеш миграция в конзолата. C# знае за свойството, но в реалния SQL Server на диска колона с такова име все още не съществува.
*   **Как да избегнеш:** Винаги след промяна на модел изпълнявай последователно двете PMC команди: `Add-Migration [Име]` и `Update-Database`.

### Капан 2: NullReferenceException при опит за четене на свързана таблица (Eager Loading)
*   **Какво се случва:** Имаш студент, който е релационно записан в даден курс. Опитваш се да прочетеш заглавието на курса директно през обекта на студента: `var title = student.Course.Title;`. Програмата се срива фатално.
*   **Грешка на екрана:** `NullReferenceException: Object reference not set to an instance of an object.`
*   **Защо се случва:** По подразбиране EF Core използва строга стратегия за пестене на памет – той изтегля **само** чистите данни за студента от неговата таблица, без автоматично да прави тежки `JOIN` операции към съседни таблици. Свойството `.Course` остава празно (`null`).
*   **Как да избегнеш:** Винаги изрично указвай на EF Core да зареди свързаната таблица насила чрез метода **`.Include()`** вътре в твоята LINQ заявка:
    ```csharp
    var studentWithCourse = context.Students
        .Include(s => s.Course)
        .FirstOrDefault(s => s.Id == 1);
    ```

---
[⬅️ 02-01. Подходи за Mapping](02-01-EF-Core-Mapping-Approaches.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) | [⬅️ 02-03. Глобален Пищов](02-03-EF-Core-Fluent-API-And-Migrations-CheatSheet.md) 

© 2026 Dev-Library / Основи на EF Core: Конфигурация и LINQ
