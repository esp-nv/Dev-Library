````text
// ============================================================
// C# SOLID PRINCIPLES — АРХИТЕКТУРЕН УЧЕБНИК (ЧАСТ 1)
// ============================================================
// 🧠 ИДЕЯТА: Не пишем спагети код. Знаем как софтуерните компоненти
// се свързват в паметта и как да ги направим нечупливи.
// ============================================================

// ============================================================
// 1. ВЪВЕДЕНИЕ: ЗАЩО НИ Е ГРИЖА ЗА SOLID?
// ============================================================
// В реалния софтуер изискванията се променят всеки ден.
// Ако кодът е лошо проектиран (High Coupling - силно заварен),
// всяка малка промяна води до "ефекта на доминото" — коригираш бъг
// в имейлите, а базата данни на StudentManagement спира да работи.
// SOLID са 5 златни конвенции за чиста ООП архитектура.
// ============================================================

// ============================================================
// 2. S — SINGLE RESPONSIBILITY PRINCIPLE (Единствена Отговорност)
// ============================================================
// КОГА СЕ ПРИЛАГА?
// -> Когато усетиш, че един клас знае прекалено много неща: 
//    как да смята, как да логва, как да говори с диска или SQL.
//
// АНАЛОГИЯ:
// Професионален комплект инструменти срещу евтино швейцарско ножче.
// Ако на ножчето му се счупи тирбушонът, хвърляш цялото ножче.
// В добрата архитектура ножът и тирбушонът са напълно независими.

using System;
using System.IO;

// --- ЛОШИЯТ НАЧИН (Анти-модел: God Class с 3 отговорности) ---
public class StudentManager
{
    public void RegisterStudent(string name, double grade)
    {
        // Отговорност 1: Бизнес Валидация
        if (string.IsNullOrEmpty(name)) 
            throw new ArgumentException("Името е празно!");

        // Отговорност 2: Инфраструктурен запис (База данни)
        Console.WriteLine($"[SQL DB] Запис на студент: {name} с оценка {grade}");

        // Отговорност 3: Работа с операционната система (Файлови логове)
        File.WriteAllText("app.log", $"[LOG] Регистриран: {name} на {DateTime.Now}");
    }
}

// --- ДОБРИЯТ НАЧИН (SOLID модел: Всеки клас с 1 причина за промяна) ---
public class Student
{
    public string Name { get; private set; }
    public double Grade { get; private set; }

    public Student(string name, double grade)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Невалидно име!");
        Name = name;
        Grade = grade;
    }
}

public class StudentRepository
{
    public void SaveToDatabase(Student student)
    {
        // Изолиран код: Грижи се ЕДИНСТВЕНО за SQL/базата
        Console.WriteLine($"[SQL DB] Обектът {student.Name} е записан трайно.");
    }
}

public class DiskLogger
{
    public void LogEvent(string message)
    {
        // Изолиран код: Грижи се ЕДИНСТВЕНО за файловата система
        File.AppendAllText("clean_app.log", $"{DateTime.Now}: {message}\n");
    }
}

// Оркестраторът просто събира компонентите без да знае как работят вътрешно
public class StudentRegistrationService
{
    private readonly StudentRepository _repo = new StudentRepository();
    private readonly DiskLogger _logger = new DiskLogger();

    public void Register(string name, double grade)
    {
        Student student = new Student(name, grade); // 1. Бизнес правила
        _repo.SaveToDatabase(student);              // 2. Персистенция
        _logger.LogEvent($"Студент {name} влезе в системата."); // 3. Одит
    }
}
// ЗАПОМНИ:
// Класът трябва да има само ЕДНА причина да бъде променян.
// ============================================================

// ============================================================
// 3. O — OPEN/CLOSED PRINCIPLE (Отворен за разширение / Затворен за промяна)
// ============================================================
// КОГА СЕ ПРИЛАГА?
// -> Когато бизнесът поиска нов тип функционалност и ти се хванеш, 
//    че отваряш работещ файл, за да набиеш поредния "else if" или "case".
//
// АНАЛОГИЯ:
// USB портовете на компютъра ти. Процесорът е фабрично запечатан (Затворен за промяна).
// Но ти можеш да добавиш мишка, клавиатура или флашка (Отворен за разширение) през USB без поялник.

// --- ЛОШИЯТ НАЧИН (Анти-модел: Счупен OCP чрез чуплив switch) ---
public class GradeEvaluator
{
    public double CalculateBonus(string studentType, double currentScore)
    {
        // При всеки нов тип студент (напр. Онлайн, Чуждестранен), трябва да РЕЖЕШ този готов метод!
        switch (studentType)
        {
            case "Regular": return currentScore * 1.1;
            case "Scholarship": return currentScore * 1.3;
            default: return currentScore;
        }
    }
}

// --- ДОБРИЯТ НАЧИН (SOLID модел: Разширение без модификация на работещ код) ---
public interface IStudentTier
{
    double ApplyBonus(double score);
}

// Конкретните имплементации живеят в собствени, напълно изолирани файлове
public class RegularTier : IStudentTier
{
    public double ApplyBonus(double score) => score * 1.1;
}

public class ScholarshipTier : IStudentTier
{
    public double ApplyBonus(double score) => score * 1.3;
}

// 🚀 НОВА БИЗНЕС ЛОГИКА: Добавяме я в НАПЪЛНО НОВ КЛАС. Старият код не се докосва!
public class OnlineStudentTier : IStudentTier
{
    public double ApplyBonus(double score) => score * 1.05;
}

// Този енджин работи само с договора (интерфейса). Той е затворен за промяна завинаги.
public class PerformanceEngine
{
    public double ProcessScore(IStudentTier tier, double baseScore)
    {
        return tier.ApplyBonus(baseScore); // Полиморфизъм в действие под капака
    }
}
// ЗАПОМНИ:
// Вместо да променяш готов работещ код, добавяй ново поведение чрез нови класове.
// ============================================================
// ============================================================
// 4. L — LISKOV SUBSTITUTION PRINCIPLE (Заменяемост на Лисков)
// ============================================================
// КОГА СЕ ПРИЛАГА?
// -> Когато създаваш наследяване (клас-наследник) и се усетиш, че
//    някои методи на родителя нямат смисъл за него и ги оставяш празни.
//
// АНАЛОГИЯ:
// Електрическа играчка (гумена патица) срещу истинска патица.
// Ако базов клас е Истинска Патица с метод Swim(), а подкласът е Гумена Патица,
// която изисква батерии или хвърля грешка в езерото, полиморфизмът се чупи.
// Ако подкласът се нуждае от "специално отношение", наследяването е грешно.

// --- ЛОШИЯТ НАЧИН (Анти-модел: Нарушен Лисков, кодът крашва в Runtime) ---
public class BaseCourse
{
    public virtual string GetClassroomLocation() => "Аула 201";
}

public class OnlineCourse : BaseCourse
{
    public override string GetClassroomLocation()
    {
        // ❌ БУУМ! Хвърлянето на тази грешка саботира всеки foreach цикъл в паметта!
        throw new NotImplementedException("Онлайн курсовете нямат физическа стая!");
    }
}

// --- ДОБРИЯТ НАЧИН (SOLID модел: Безопасна заменяемост и интерфейсно разделение) ---
public abstract class Course
{
    public string Title { get; set; }
    // Базовият клас налага само 100% споделени свойства/поведения
}

public interface IPhysicalCourse
{
    string GetClassroomLocation(); // Специализиран договор само за присъствени курсове
}

public class InPersonCourse : Course, IPhysicalCourse
{
    public string GetClassroomLocation() => "Аула 201";
}

public class ZoomCourse : Course
{
    // Безопасен! Няма физическа локация, няма празни методи, няма риск от Runtime грешка.
    public string ZoomLink { get; set; }
}
// ЗАПОМНИ:
// Подкласовете трябва да могат да заменят базовите си класове без да чупят логиката.
// ============================================================

// ============================================================
// 5. I — INTERFACE SEGREGATION PRINCIPLE (Разделяне на Интерфейсите)
// ============================================================
// КОГА СЕ ПРИЛАГА?
// -> Когато дефинираш интерфейс-чудовище (Fat Interface) и класовете, 
//    които го имплементират, са принудени да пишат празен код.
//
// АНАЛОГИЯ:
// Глобално мено срещу специализирани пакети. Не искаш в кафенето да те
// задължават да поръчаш супа и основно ястие, само защото са в един общ договор.

// --- ЛОШИЯТ НАЧИН (Анти-модел: Прекалено "дебел" договор) ---
public interface ISmartActions
{
    void SubmitHomework();
    void GradeAssignment();
}

public class StudentUser : ISmartActions
{
    public void SubmitHomework() => Console.WriteLine("Домашното е качено.");
    
    public void GradeAssignment()
    {
        // ❌ Мъртъв код: Студентът е принуден да имплементира метод за преподаватели!
        throw new InvalidOperationException("Студентите не оценяват!");
    }
}

// --- ДОБРИЯТ НАЧИН (SOLID модел: Атомарни, силно специализирани договори) ---
public interface IHomeworkSubmitter { void SubmitHomework(); }
public interface IAssignmentGrader { void GradeAssignment(); }

public class DiligentStudent : IHomeworkSubmitter
{
    public void SubmitHomework() => Console.WriteLine("Домашното по C# е предадено.");
}

public class StrictTeacher : IAssignmentGrader
{
    public void GradeAssignment() => Console.WriteLine("Оценка: Отличен (6.00).");
}
// ЗАПОМНИ:
// По-добре 10 малки и специализирани интерфейса, отколкото един масивен.
// ============================================================

// ============================================================
// 6. D — DEPENDENCY INVERSION PRINCIPLE (Обръщане на Зависимостите)
// ============================================================
// КОГА СЕ ПРИЛАГА?
// -> Най-важният принцип за ASP.NET Core! Прилага се, за да се премахне 
//    твърдото "заваряване" (`new`) между отделните софтуерни слоеве.
//
// АНАЛОГИЯ:
// Електрическият контакт в стената. Твоят лаптоп не е запоен директно за
// кабелите на Енергото. И двете страни зависят от една абстракция — контакта.

// --- ЛОШИЯТ НАЧИН (Анти-модел: Твърдо обвързване чрез 'new') ---
public class SqliteDatabase
{
    public string GetData() => "Студенти от SQLite";
}

public class StudentController
{
    // ❌ КРИТИЧНА ГРЕШКА: Твърдо заваряване! Ако сменим базата, пренаписваме контролера.
    private SqliteDatabase _db = new SqliteDatabase(); 
}

// --- ДОБРИЯТ НАЧИН (SOLID модел: Конструкторно инжектиране - Dependency Injection) ---
public interface IStudentRepository
{
    string GetData();
}

// Ниско ниво имплементация (Инфраструктура)
public class PostgresRepository : IStudentRepository
{
    public string GetData() => "Данни от стабилен PostgreSQL сървър";
}

// Високо ниво логика - зависи ЕДИНСТВЕНО от контакта (интерфейса)
public class DecoupledController
{
    private readonly IStudentRepository _repo;

    // IoC контейнерът в Program.cs автоматично ще инжектира правилния клас тук!
    public DecoupledController(IStudentRepository repo)
    {
        _repo = repo; // Обръщане на контрола (Inversion of Control)
    }

    public void LoadView() => Console.WriteLine(_repo.GetData());
}
// ЗАПОМНИ:
// Модулите от високо ниво не трябва да зависят от ниско ниво класове. Зависете от абстракции!
// ============================================================

// ============================================================
// 7. СТОП-СПИРАЧКА ЗА JUNIOR КАПАНИ ПОД КАПАКА
// ============================================================
// 🛑 1. Капанът на Over-Engineering: Не разцепвай класовете до лудост. 
//      Ако направиш отделен файл за всяка проверка от 1 ред, кодът става нечетив.
// 🛑 2. Сляпото хвърляне на NotImplementedException е знак за счупен L или I принцип.
// 🛑 3. Никога не пиши 'new' за бизнес услуги и репозиторита вътре в контролерите си.
// ============================================================
// © 2026 Dev-Library / Архитектурно ядро
