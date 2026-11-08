# 📘 Глава 4: Свързване на C# с Бази данни (Entity Framework Core)
**Статус:** Актуализиран учебен материал | **Цел:** Учене без лутане, Code-First логика, Миграции и реални грешки

---

## 🔌 1. ЗАЩО НИ Е НУЖЕН EF CORE (Какъв проблем решава?)

В миналото, за да запишем данни от C# в SQL Server, трябваше да пишем сурови SQL заявки като текстови стрингове вътре в кода (`"INSERT INTO Students..."`), да отваряме ръчно връзки към сървъра и да превръщаме редовете от таблицата обратно в C# обекти. Това беше досадно, бавно и лесно се допускаха правописни грешки.

**Entity Framework Core (EF Core) решава това, като работи като ORM (Object-Relational Mapper).** Той служи за „преводач“ – превръща нашите C# класове в SQL таблици и обратно, автоматично. Ние работим само с чист C# код, а EF Core пише SQL-а зад кулисите вместо нас.

---

## 💡 2. АНАЛОГИЯ ОТ РЕАЛНИЯ ЖИВОТ

**Автоматичен двупосочен преводач между български и китайски.** 
Ти си C# програмистът и говориш само български (C# обекти). Базата данни е китайският ресторант (SQL Server), който разбира само китайски (SQL заявки). Вместо ти да учиш китайски, ти наемаш преводач (EF Core). Ти му казваш: *„Добави нов студент Иван“*, преводачът си записва това в тефтера (`DbContext`), превежда го на китайски (`INSERT INTO...`) и го праща към кухнята (Базата данни).

---

## 🏗️ 3. ПЪЛЕН СЛУЧАЙ ОТ ЖИВОТА: СЪЗДАВАНЕ НА ТАБЛИЦА (Code-First)

Ето как събираме всичко на едно място. Искаме да направим система, която пази студенти и техните курсове (точно като твоето `StudentManagement` приложение).

### 📝 Стъпка 1: Дефиниране на C# модела (Entity)
```csharp
// Това е нашият чист C# клас. EF Core ще го превърне в таблица "Students"
public class Student 
{
    // Запомни: Свойство с име "Id" автоматично става Първичен ключ (Primary Key) в SQL
    public int Id { get; set; } 
    
    // NVARCHAR в SQL (задължително за кирилица)
    public string FirstName { get; set; } 
    
    public string LastName { get; set; }
    
    // DATETIME2 в SQL
    public DateTime DateOfBirth { get; set; } 
}
```

### 🗄️ Стъпка 2: Създаване на Сърцето на базата (`DbContext`)
`DbContext` е тефтерът на преводача. Той казва на EF Core кои класове трябва да станат таблици и къде се намира SQL сървърът.
```csharp
using Microsoft.EntityFrameworkCore;

public class SchoolDbContext : DbContext
{
    // DbSet казва: "Искам таблица със Студенти"
    public DbSet<Student> Students { get; set; }

    // Конфигуриране на връзката към нашия локален SQL Server (Connection String)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=Localhost;Database=MySchoolDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
```

---

## ⏳ 4. ХРОНОЛОГИЯ НА МИГРАЦИИТЕ (Как пренасяме кода в SQL?)

Когато работим по Code-First подход, базата данни се управлява през **Конзолата (Package Manager Console)** във Visual Studio чрез две основни команди. Ето какво се случва на екрана стъпка по стъпка:

### 🛠️ Команда 1: `Add-Migration InitialCreate`
*   **Какво прави логически:** Сканира твоя `DbContext` и генерира C# скрипт, който описва как да се създаде таблицата.
*   **Изход на екрана (Успех):**
    ```text
    ✔ Done. To undo this action, use 'Remove-Migration'.
    (В проекта ти се появява папка 'Migrations' с файл xxxx_InitialCreate.cs)
    ```

### 🛠️ Команда 2: `Update-Database`
*   **Какво делает логически:** Взема генерирания C# скрипт, превежда го на суров SQL и го изпълнява в SQL Server. Чак в тази милисекунда базата данни се ражда на диска!
*   **Изход на екрана (Успех):**
    ```text
    Applying migration '20261103210000_InitialCreate'.
    ✔ Done.
    ```

---

## ❌ 5. НЕДОСТАТЪЦИ, ГРАНИЧНИ СЛУЧАИ И КРИТИЧНИ ГРЕШКИ

Макар и много мощен, EF Core има своите капани. Ето къде най-често ще забие програмата ти и как да се спасиш:

### ⚠️ Капан №1: Забравен `Update-Database`
*   **Ситуация:** Добавяш ново свойство `public string Email { get; set; }` в класа `Student`. Пускаш програмата и тя забива.
*   **Грешка на екрана (`Exception`):** `SqlException: Invalid column name 'Email'.`
*   **Защо се случва:** Ти си променила C# кода, но си забравила да направиш миграция и да обновиш SQL Server. Базата данни все още няма колона `Email`.
*   **Решение:** Спираш програмата, пишеш `Add-Migration AddEmail` и след това `Update-Database`.

### ⚠️ Капан №2: Проблемът с Празния Конструктор
*   **Ситуация:** Създаваш конструктор с параметри в твоя Entity клас (напр. `public Student(string name) { ... }`).
*   **Грешка на екрана:** `InvalidOperationException: Each entity type requires a parameterless constructor.`
*   **Защо се случва:** Когато EF Core тегли данни от SQL, той трябва да създаде празен обект и да го напълни. За целта му е жизнено необходим конструктор **без параметри** (празен конструктор).
*   **Решение:** Винаги дописвай по един празен конструктор `public Student() { }`, ако си създала друг с параметри.

### ⚠️ Капан №3: Мързеливо зареждане на релации (The `Null` Trap)
*   **Ситуация:** Имаш студент, който е записан в `Course`. Опитваш се да прочетеш името на курса: `student.Course.Title`.
*   **Грешка на екрана:** `NullReferenceException: Object reference not set to an instance of an object.`
*   **Защо се случва:** По подразбиране EF Core спестява памет и зарежда **само** данните за студента, без да тегли свързаните таблици (Мързеливо зареждане / Lazy Loading). Свойството `.Course` е `null`.
*   **Решение:** Използвай метода **`.Include()`** в твоята LINQ заявка, за да кажеш насила на EF Core да зареди и свързаната таблица:
    ```csharp
    var studentWithCourse = context.Students.Include(s => s.Course).FirstOrDefault(s => s.Id == 1);
    ```

---

## 📊 6. СРАВНИТЕЛЕН ЛИСТ (За бързо препрочитане)

| Операция в C# | Какво генерира EF Core в SQL зад кулисите? | Скорост |
| :--- | :--- | :--- |
| `context.Students.Add(newStudent);`<br>`context.SaveChanges();` | `INSERT INTO Students (FirstName...) VALUES (...)` | Бързо (Единичен запис) |
| `context.Students.Remove(student);`<br>`context.SaveChanges();` | `DELETE FROM Students WHERE Id = ...` | Бързо |
| `context.Students.ToList();` | `SELECT * FROM Students` | **Бавно**, ако таблицата има милиони редове! |
| `context.Students.Where(s => s.Id == 1);` | `SELECT * FROM Students WHERE Id = 1` | Светкавично ($O(1)$ през Индекс) |
