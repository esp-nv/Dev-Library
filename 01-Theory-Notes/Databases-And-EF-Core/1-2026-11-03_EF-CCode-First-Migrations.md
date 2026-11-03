
📘 Глава 4: Свързване на C# с Бази данни (Entity Framework Core & Настройки)

Статус: Пълен учебен материал | Цел: Учене без лутане, JSON конфигурации, разширен Fluent API и жизнен цикъл на миграциите

🔌 1. ЗАЩО НИ Е НУЖЕН EF CORE (Какъв проблем решава?)

В миналото, за да запишем данни от C# в SQL Server, бяхме длъжни да пишем сурови SQL заявки като обикновен текст вътре в C# кода ("INSERT INTO Students..."), да отваряме ръчно връзки към сървъра и да превръщаме редовете от таблицата обратно в обекти. Това водеше до досадни правописни грешки.
Entity Framework Core (EF Core) работи като ORM (Object-Relational Mapper). Той служи за „автоматичен преводач“ – превръща нашите C# класове в SQL таблици и обратно. Ние пишем только чист C# и LINQ, а EF Core генерира SQL-а зад кулисите вместо нас.

🗺️ Визуална схема на целия софтуерен път:

text
  C# Приложение (Application) ──> Прочита [ appsettings.json ] (Взема Connection String)
        │
        ▼
   C# Клас / Обект (Product Entity)
        │
        ▼
   МАПИНГ ПРЕВОДАЧ (Fluent API) ───> [ OnModelCreating: Правила, Ключове, Ограничения ]
        │
        ▼
   Контекст на базата (DbContext / DbSet)
        │
        ▼
   LINQ Заявка в кода (context.Products.Where...)
        │
        ▼
   EF Core Двигател (Генерира SQL текст зад кулисите)
        │
        ▼
   Сурова SQL Заявка (SELECT * FROM store_products...)
        │
        ▼
   БАЗА ДАННИ (SQL Server / Database)
Use code with caution.

💡 2. АНАЛОГИЯ ОТ РЕАЛНИЯ ЖИВОТ

Автоматичен двупосочен преводач между български и китайски.
Ти си C# програмистът и говориш само български (C# обекти). Базата данни е китайски ресторант (SQL Server), който разбира само китайски (SQL заявки). Вместо ти да учиш китайски, наемаш преводач (EF Core). Ти му казваш на български: „Добави нов продукт Лаптоп“, преводачът си го записва в тефтера (DbContext), превежда го на китайски (INSERT INTO store_products...) и го праща към кухнята (Базата данни).

🗂️ 3. НАСТРОЙКА НА ВРЪЗКАТА ЧРЕЗ appsettings.json (Connection String)

В реални проекти (като EnterpriseReportSystem и StudentManagement) е абсолютно забранено връзката към базата да бъде хардкодната вътре в C# кода. Ако утре се смени сървърът или паролата, трябва да прекомпилираш цялото приложение. Затова тя се изнася във външен конфигурационен JSON файл.

📄 Стъпка 1: Конфигуриране на appsettings.json

В корена на твоя уеб проект се намира този конфигурационен файл. В него дефинираме секция ConnectionStrings:
json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=Localhost;Database=MySchoolDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Use code with caution.

🔍 Анатомия на един Connection String (Анатомия на параметрите):

• Server=Localhost; (или Server=.\SQLEXPRESS;): Казва къде се намира SQL Server. Localhost или точка . означава "на моя личен компютър".
• Database=MySchoolDb;: Името на базата данни, която EF Core ще създаде за този конкретен проект.
• Trusted_Connection=True; (или Integrated Security=True;): Използва твоя Windows акаунт за автентификация. Няма нужда да пишеш потребителско име и парола за SQL Server.
• TrustServerCertificate=True;: Изключително важен параметър при по-новите .NET версии. Казва на програмата да се довери на локалния криптографски сертификат на твоя SQL Server, предотвратявайки SSL грешки при свързване.

🔌 Стъпка 2: Прочитане на Connection String в DbContext

csharp
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

    public DbSet<Student> Students { get; set; }

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
Use code with caution.

🧱 4. ДЪЛБОКО ПОТАПЯНЕ ВЪВ Fluent API (Разширени правила)

При Fluent API ние държим Entity класовете чисти, а в метода OnModelCreating разписваме точните правила, ограничения и бизнес валидации за базата данни.
csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // 1. Дефиниране на име на таблица (Entity -> Table)
    modelBuilder.Entity<Student>()
        .ToTable("school_students");

    // 2. Изрично задаване на Първичен ключ (Primary Key)
    modelBuilder.Entity<Student>()
        .HasKey(s => s.Id);

    // 3. Конфигуриране на колона (Property -> Column) + Ограничения (Constraints)
    modelBuilder.Entity<Student>()
        .Property(s => s.FirstName)
        .HasColumnName("first_name")
        .HasMaxLength(50)       // NVARCHAR(50) - Ограничава размера в SQL
        .IsRequired();          // NOT NULL - Полето става задължително в базата!

    modelBuilder.Entity<Student>()
        .Property(s => s.Email)
        .HasColumnName("email_address")
        .IsRequired(false);     // NULL - Полето е незадължително

    // 4. Задаване на стойност по подразбиране в SQL (Default Value)
    modelBuilder.Entity<Student>()
        .Property(s => s.IsActive)
        .HasDefaultValue(true); // Ако не подадем стойност, SQL автоматично записва True
}
Use code with caution.

⏳ 5. ЖИЗНЕН ЦИКЪЛ НА МИГРАЦИИТЕ (Пълен CLI Команден Дневник)

Управлението на миграциите през конзолата (Package Manager Console) изисква познаване както на командите за създаване, така и на тези за изтриване и пълно нулиране.
text
               КОМАНДЕН ЖИЗНЕН ЦИКЪЛ НА КОДА И БАЗАТА ДАННИ
               
  [ Написан C# код ] ──( Add-Migration )──> [ Създаден файл-рецепта в C# ]
                                                       │
                                              ( Remove-Migration ) -> Изтрива локалния файл
                                                       │ (ако не е пратен в DB)
                                                       ▼
  [ Реална SQL DB ]  <──( Update-Database ) <──────────┘
           │
     ( Drop-Database ) ──> Пълно изтриване на физическата база данни от диска
Use code with caution.

🛠️ Команда 1: Add-Migration InitialCreate

• Какво прави: Сканира моделите и генерира C# файл-рецепта. Базата данни все още НЕ е променена.

🛠️ Команда 2: Update-Database

• Какво прави: Изпълнява всички непратени миграции в SQL Server. Базата се обновява на диска.

🛠️ Команда 3: Remove-Migration (Изтриване на последна миграция)

• Кога се използва: Когато си написала Add-Migration, но секунда след това осъзнаваш, че си допуснала правописна грешка в някое свойство или Fluent API правило.
• Критично условие: Работи САМО АКО все още не си изпълнила Update-Database. Изтрива последния генериран файл в папка Migrations безопасно.
• Грешка, ако базата е вече обновена: Програмата ще ти забрани да я изтриеш, защото базата на диска вече зависи от нея.

🛠️ Команда 4: Drop-Database (Пълно премахване/Нулиране на Базата)

• Кога се използва: Когато проектът се е объркал тотално, миграциите са се заплели или искаш да изтриеш локалната база данни изцяло от твърдия диск, за да започнеш на чисто от нулата.
• Какво прави софтуерно: Изпраща команда към SQL Server да изтрие физическия файл на базата данни (.mdf и .ldf). След тази команда можеш да изтриеш папка Migrations и да започнеш чисто решение.

🔄 Как да върна базата към по-стара миграция (Rollback)?

Ако си на миграция №5, но искаш да се върнеш обратно към миграция №2 на диска, не триеш файлове. Просто пишеш:
text
Update-Database НазваниеНаВтора Миграция
Use code with caution.
EF Core автоматично ще разбере кои таблици и колони трябва да изтрие в SQL Server, за да върне състоянието назад.

❌ 6. ВНИМАНИЕ: КРИТИЧНИТЕ КАПАНИ В EF CORE

• Капан №1 (Забравен Update-Database): Добавяш свойство public string Email { get; set; } в C# кода, но забравиш да пуснеш миграция. Програмата ще забие с SqlException: Invalid column name 'Email'.
• Капан №2 (Липса на празен конструктор): Ако създадеш твой конструктор с параметри (напр. public Product(string name)), EF Core ще забие с InvalidOperationException: Each entity type requires a parameterless constructor. Винаги дописвай по един празен конструктор public Product() { }!
• Капан №3 (Изтриване на файлове с миграции на ръка): Никога не трий файлове в папка Migrations ръчно през Windows Explorer! Това чупи вътрешната база данни на EF Core (__EFMigrationsHistory). Винаги използвай Remove-Migration през конзолата.
