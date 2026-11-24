[⬅️ 02-02. ApplicationDbContext Конфигурация](02-02-ApplicationDbContext-And-Fluent-API-Configuration.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) |  [➡️ 02-04. Мулти-провайдър Превключване](02-04-Multi-Database-Provider-Switch.md)
---

# 02-03. EF Core Fluent API и Миграции (Глобален Инженерен Пищов)

**Цел:** Бърза и визуална справка за синтаксиса на Fluent API релациите, ограниченията, CLI командите в конзолата и двупосочния превод от C# към SQL Server.

## Съдържание на темата
* [1. Fluent API ограничения (Constraints) и превод към SQL](#1-fluent-api-ограничения-constraints-и-превод-към-sql)
* [2. Двупосочен превод (Съпоставка в таблица)](#2-двупосочен-превод-съпоставка-в-таблица)
* [3. Синтаксис на релациите (Връзки между таблици)](#3-синтаксис-на-релациите-връзки-между-таблици)
* [4. Желязна защита: Спиране на каскадното триене](#4-желязна-защита-спиране-на-каскадното-триене)
* [5. Кратък CLI дневник на миграциите (PMC команди)](#5-кратък-cli-дневник-на-миграциите-pmc-команди)

---

## 1. FLUENT API ОГРАНИЧЕНИЯ (CONSTRAINTS) И ПРЕВОД КЪМ SQL

```csharp
modelBuilder.Entity<Product>()                   // ➔ SQL: CREATE TABLE store_products
    .ToTable("store_products");                  

modelBuilder.Entity<Product>()                   
    .HasKey(p => p.Id);                          // ➔ SQL: PRIMARY KEY (Id) -> Прави Clustered Index

modelBuilder.Entity<Product>()                   
    .Property(p => p.Name)                       
    .HasColumnName("product_name")               // ➔ SQL: Преименува колоната на [product_name]
    .HasMaxLength(50)                            // ➔ SQL: NVARCHAR(50) -> Задължително за кирилица!
    .IsRequired();                               // ➔ SQL: NOT NULL (Полето става задължително)

modelBuilder.Entity<Product>()                   
    .Property(p => p.Price)                      
    .HasColumnType("decimal(18,2)")              // ➔ SQL: DECIMAL(18,2) -> Златен стандарт за пари!
    .HasDefaultValue(0.00);                      // ➔ SQL: DEFAULT 0.00
```

---

## 2. СИНТАКСИС НА РЕЛАЦИИТЕ (ВРЪЗКИ МЕЖДУ ТАБЛИЦИ)

При изграждане на връзки винаги мисли през трите вълшебни думи:
*   `Has[One/Many]` ➔ С какво разполага **текущата** таблица.
*   `With[One/Many]` ➔ С какво разполага **съседната** таблица от другата страна.
*   `HasForeignKey` ➔ Коя софтуерна колона държи физическия Външен ключ в SQL Server.

### А. Връзка Едно към Много (1:Many)
```csharp
modelBuilder.Entity<Student>()
    .HasOne(s => s.Course)             // Текущият Студент има ЕДИН Курс
    .WithMany(c => c.Students)         // Съседният Курс има МНОГО Студенти
    .HasForeignKey(s => s.CourseId);   // Външният ключ в SQL е CourseId
```

### Б. Връзка Едно към Едно (1:1)
```csharp
modelBuilder.Entity<StudentProfile>()
    .HasOne(sp => sp.Student)          // Профилът има ЕДИН Студент
    .WithOne(s => s.Profile)           // Студентът има ЕДИН Профил
    .HasForeignKey<StudentProfile>(sp => sp.StudentId); // Твърдо указваме къде живее физическия FK
```

### В. Връзка Много към Много (Many:Many — Базов вариант)
```csharp
modelBuilder.Entity<Student>()
    .HasMany(s => s.Lectures)          // Студентът има МНОГО Лекции
    .WithMany(l => l.Students)         // Лекцията има МНОГО Студенти
    .UsingEntity(j => j.ToTable("students_lectures")); // Генерира скрита междинна SQL таблица
```

### Г. Връзка Много към Много (Many:Many — С помощна мостова таблица)
Използва се задължително, когато в междинната таблица искаме да запишем допълнителни бизнес колони (напр. `EnrolledOn` дата). Разбива се на две отделни `1:Many` връзки през композитен ключ:

```csharp
// Помощният мостови клас (Entity)
public class StudentLecture
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;

    public DateTime EnrolledOn { get; set; } // Допълнителна колона
}

// Конфигурация във Fluent API
modelBuilder.Entity<StudentLecture>()
    .HasKey(sl => new { sl.StudentId, sl.LectureId }); // Съставен първичен ключ (Composite PK)

modelBuilder.Entity<StudentLecture>()
    .HasOne(sl => sl.Student).WithMany(s => s.StudentLectures).HasForeignKey(sl => sl.StudentId);

modelBuilder.Entity<StudentLecture>()
    .HasOne(sl => sl.Lecture).WithMany(l => l.StudentLectures).HasForeignKey(sl => sl.LectureId);
```

---

## 3. ЖЕЛЯЗНА ЗАЩИТА: СПИРАНЕ НА КАСКАДНОТО ТРИЕНЕ

За да забраним на SQL Server автоматично да изтрива финансова или бизнес история при изтриване на запис-родител, налагаме глобална Restrict политика най-отдолу в метода `OnModelCreating`:

```csharp
foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
{
    foreignKey.DeleteBehavior = DeleteBehavior.Restrict; // Превръща "Падащото домино" в "Желязна стена"
}
```

### Справка за DeleteBehavior:
*   **`DeleteBehavior.Cascade`** ➔ Изтриваш родителя ➔ базата автоматично изтрива децата. **(Опасно за продукция!)**
*   **`DeleteBehavior.Restrict`** ➔ Базата отказва да изтрие родителя, ако има поне едно свързано дете. **(Златен стандарт)**
*   **`DeleteBehavior.SetNull`** ➔ Изтриваш родителя ➔ в колоната на децата се записва `NULL`.

---

## ⏳ 4. КРАТЪК CLI ДНЕВНИК НА МИГРАЦИИТЕ (PMC КОМАНДИ)

Командите се изпълняват в **Package Manager Console** и управляват жизнения цикъл на структурата на данните:

*   **`Add-Migration InitialCreate`** ➔ Сканира C# кода и генерира файл-рецепта в проекта. Базата в SQL Server все още **НЕ** е променена.
*   **`Update-Database`** ➔ Изпълнява непратените рецепти, превежда ги на суров SQL и обновява базата на диска.
*   **`Remove-Migration`** ➔ Изтрива последния локален файл-рецепта. Работи **САМО АКО** все още не си изпълнила `Update-Database`.
*   **`Drop-Database`** ➔ Изтрива физически целия файл на базата данни (`.mdf` и `.ldf`) от твърдия диск на компютъра за пълно нулиране.
*   **`Update-Database ИмаНаМиграция`** ➔ Извършва автоматичен **Rollback (връщане назад)** на структурата на диска до състоянието на посочената миграция.

---
## 5. ПЪЛЕН CLI ДНЕВНИК НА МИГРАЦИИТЕ (ПАКЕТЕН КАТАЛОГ СЪС СЪПОСТАВКА)

В софтуерната разработка изборът на команда зависи от това дали работиш във **Visual Studio (PMC)** или извън него във **VS Code / Терминал (.NET CLI)**. Ето пълната съпоставка и синтаксис:

| Действие по структурата | Visual Studio (PMC) Команда | Терминал / VS Code (.NET CLI) | Какво прави софтуерно под капака? |
| :--- | :--- | :--- | :--- |
| **Създаване на миграция** | `Add-Migration [Име]` | `dotnet ef migrations add [Име]` | Сканира C# кода и генерира C# файл-рецепта в папка `Migrations`. |
| **Прилагане в базата** | `Update-Database` | `dotnet ef database update` | Превежда C# рецептите на SQL и ги изпълнява реално в SQL Server. |
| **Изтриване на последна** | `Remove-Migration` | `dotnet ef migrations remove` | Изтрива последния локален файл-рецепта (само ако още не е пратен в DB). |
| **Пълно изтриване на DB** | `Drop-Database` | `dotnet ef database drop` | Премахва физически целия файл на базата (`.mdf`/`.ldf`) от твърдия диск. |
| **Списък с миграции** | `Get-Migration` | `dotnet ef migrations list` | Извежда списък с всички миграции и маркира кои са приложени в SQL. |
| **Генериране на SQL скрипт**| `Script-Migration` | `dotnet ef migrations script` | Генерира суров текстов SQL скрипт, който да дадеш на системния администратор. |

### 🔄 Професионални трикове от практиката (Специфични параметри)

#### 1. Генериране на Идемпотентен (Idempotent) SQL Скрипт за Продукция
Когато искаш да качиш промените на реален сървър при клиента, е забранено да ползваш `Update-Database`. Генерира се специален защитен скрипт, който сам проверява дали дадена таблица съществува, преди да я създаде:
*   **Visual Studio (PMC):** `Script-Migration -Idempotent`
*   **Терминал / VS Code (.NET CLI):** `dotnet ef migrations script --idempotent`

#### 2. Връщане на базата към по-старо състояние (Rollback)
Ако си на миграция №5, но искаш да изтриеш промените от последните три миграции и да върнеш базата данни до състоянието на миграция №2, изпълняваш командата с името на втората миграция. EF Core сам ще разбере кои колони и таблици да изтрие:
*   **Visual Studio (PMC):** `Update-Database НазваниеНаВтораМиграция`
*   **Терминал / VS Code (.NET CLI):** `dotnet ef database update НазваниеНаВтораМиграция`

#### 3. Изрично указване на Стартиращ проект (При Multi-Project архитектура)
Ако твоят `DbContext` се намира в папка `Data.DataAccess`, а уеб проектът ти е `Web.API`, трябва насила да кажеш на конзолата откъде да прочете `appsettings.json`:
*   **Терминал / VS Code (.NET CLI):** `dotnet ef database update --project Data.DataAccess --startup-project Web.API`

---

[⬅️ 02-02. ApplicationDbContext Конфигурация](02-02-ApplicationDbContext-And-Fluent-API-Configuration.md) | [📂 Глава 02. Основи на Entity Framework Core и Code-First](02-00-EF-CORE-AND-CODE-FIRST-INTRO-AND-CRUD.md) |  [➡️ 02-04. Мулти-провайдър Превключване](02-04-Multi-Database-Provider-Switch.md)

© 2026 Dev-Library / Справочник: Fluent API & Миграции Пищов

