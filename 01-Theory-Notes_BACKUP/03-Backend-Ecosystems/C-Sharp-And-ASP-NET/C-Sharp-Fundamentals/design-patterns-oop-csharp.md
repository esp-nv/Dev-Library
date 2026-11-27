# 📘 Учебник: Шаблони за проектиране (Design Patterns) в C# OOP (.NET 8)

**Файл:** `design-patterns-oop-csharp.md` | **Ниво:** Архитектурно ядро / C# Advanced OOP  
**Технологии:** .NET 8, `Lazy<T>`, Fluent Builder, Decorator Composition, Strategy & Observer

---

## 🏗️ РАЗДЕЛ 1: CREATIONAL PATTERNS (УПРАВЛЕНИЕ НА СЪЗДАВАНЕТО В ПАМЕТТА)

Създаващите шаблони изолират логиката по заделяне на памет (`new`), предотвратяват грешки при многонишковост и премахват конструкторите с десетки параметри.

```csharp
// ==================== THREAD-SAFE SINGLETON, FACTORY METHOD И BUILDER ====================

using System;
using System.Text;

// 1. THREAD-SAFE SINGLETON С LAZY<T>
// Гарантира, че в Heap паметта ще съществува точно ЕДНА инстанция на класа,
// създадена чак в момента, в който някой я поиска за първи път (Lazy Initialization).
public sealed class AppConfiguration
{
    // Lazy<T> е вграден в .NET и е 100% потокобезопасен (Thread-Safe) без ръчни lock-ове
    private static readonly Lazy<AppConfiguration> _lazyInstance =
        new Lazy<AppConfiguration>(() => new AppConfiguration());

    public string ConnectionString { get; }

    // Частен конструктор (private): забранява на външния код да пише 'new AppConfiguration()'
    private AppConfiguration()
    {
        ConnectionString = "Server=ProdDB;Database=StudentManagement;";
    }

    // Глобална точка за достъп до единствената инстанция
    public static AppConfiguration Instance => _lazyInstance.Value;
}

// 2. FACTORY METHOD (Делегиране на създаването без директен 'new' в бизнес логиката)
public interface INotification
{
    void Send(string message);
}

public class EmailNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"[EMAIL]: {message}");
}

public class SmsNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"[SMS]: {message}");
}

// Фабриката капсулира решението КОЙ конкретен клас да се създаде в Heap-а
public static class NotificationFactory
{
    public static INotification Create(string channel) => channel.ToLower() switch
    {
        "email" => new EmailNotification(),
        "sms"   => new SmsNotification(),
        _       => throw new ArgumentException($"Неподдържан канал: {channel}")
    };
}

// 3. FLUENT BUILDER (Решава проблема с конструктори с много опционални параметри)
public class StudentReport
{
    public string Title { get; init; }
    public bool IncludeGrades { get; init; }
    public bool IncludeAttendance { get; init; }
    public string FooterNote { get; init; }
}

public class StudentReportBuilder
{
    private string _title = "Стандартен отчет";
    private bool _includeGrades;
    private bool _includeAttendance;
    private string _footerNote = string.Empty;

    // Всеки метод връща 'this' (самия Builder), което позволява верижно извикване (Chaining)
    public StudentReportBuilder WithTitle(string title) { _title = title; return this; }
    public StudentReportBuilder AddGrades() { _includeGrades = true; return this; }
    public StudentReportBuilder AddAttendance() { _includeAttendance = true; return this; }
    public StudentReportBuilder WithFooter(string note) { _footerNote = note; return this; }

    // Финалната стъпка сглобява и връща готовия непроменяем обект
    public StudentReport Build() => new StudentReport
    {
        Title = _title,
        IncludeGrades = _includeGrades,
        IncludeAttendance = _includeAttendance,
        FooterNote = _footerNote
    };
}
```

---

## 🔌 РАЗДЕЛ 2: STRUCTURAL PATTERNS (КОМПОЗИЦИЯ И ОБВИВАНЕ НА ОБЕКТИ)

Структурните шаблони използват **композиция (обвиване на един обект в друг)** вместо наследяване, за да добавят нова функционалност или да свързват несъвместими класове.

```csharp
// ==================== ADAPTER И DECORATOR (КОМПОЗИЦИЯ В ПАМЕТТА) ====================

using System;

// 1. ADAPTER PATTERN (Преходник между несъвместими интерфейси)
// Нашият модерен интерфейс в системата
public interface ILogger
{
    void LogInfo(string text);
}

// Външна / стара библиотека, която не можем да променяме (има различно име на метода)
public class LegacyFileWriter
{
    public void WriteToDisk(string rawData, int severity)
        => Console.WriteLine($"[LEGACY DISK SEV-{severity}]: {rawData}");
}

// Адаптерът имплементира нашия ILogger, но вътре в себе си извиква стария клас
public class LegacyLoggerAdapter : ILogger
{
    private readonly LegacyFileWriter _legacyWriter;

    public LegacyLoggerAdapter(LegacyFileWriter legacyWriter)
    {
        _legacyWriter = legacyWriter;
    }

    public void LogInfo(string text)
    {
        // Превежда извикването от новия формат към стария
        _legacyWriter.WriteToDisk(text, severity: 1);
    }
}

// 2. DECORATOR PATTERN (Динамично наслояване на поведение в Runtime)
public interface ICourseFee
{
    decimal CalculateCost();
    string GetDescription();
}

// Базов компонент
public class BaseCourse : ICourseFee
{
    public decimal CalculateCost() => 200m;
    public string GetDescription() => "Основен C# курс";
}

// Абстрактен декоратор, който обвива произволен ICourseFee обект
public abstract class CourseDecorator : ICourseFee
{
    protected readonly ICourseFee _innerCourse;

    protected CourseDecorator(ICourseFee course)
    {
        _innerCourse = course ?? throw new ArgumentNullException(nameof(course));
    }

    public virtual decimal CalculateCost() => _innerCourse.CalculateCost();
    public virtual string GetDescription() => _innerCourse.GetDescription();
}

// Конкретен декоратор 1: Добавя менторство
public class MentorshipDecorator : CourseDecorator
{
    public MentorshipDecorator(ICourseFee course) : base(course) { }

    public override decimal CalculateCost() => base.CalculateCost() + 80m;
    public override string GetDescription() => base.GetDescription() + " + Личен ментор";
}

// Конкретен декоратор 2: Добавя изпитен сертификат
public class CertificateDecorator : CourseDecorator
{
    public CertificateDecorator(ICourseFee course) : base(course) { }

    public override decimal CalculateCost() => base.CalculateCost() + 40m;
    public override string GetDescription() => base.GetDescription() + " + Сертификат";
}

// Употреба: ICourseFee fullPackage = new CertificateDecorator(new MentorshipDecorator(new BaseCourse()));
// Резултат: 200 + 80 + 40 = 320m!
```

---

## 💡 РАЗДЕЛ 3: BEHAVIORAL PATTERNS (АЛГОРИТМИ И СЪБИТИЙНА КОМУНИКАЦИЯ)

Поведенческите шаблони премахват твърдото обвързване при изпълнение на алгоритми и уведомяване при промяна на състоянието.

```csharp
// ==================== STRATEGY, OBSERVER И COMMAND В ACTION ====================

using System;
using System.Collections.Generic;

// 1. STRATEGY PATTERN (Смяна на алгоритъм без промяна на класа, който го ползва)
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal originalPrice);
}

public class NoDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal price) => price;
}

public class StudentDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal price) => price * 0.80m; // 20% отстъпка
}

public class CheckoutService
{
    private IDiscountStrategy _strategy;

    public CheckoutService(IDiscountStrategy strategy) => _strategy = strategy;

    // Позволява смяна на стратегията директно по време на изпълнение (Runtime)
    public void SetStrategy(IDiscountStrategy newStrategy) => _strategy = newStrategy;

    public decimal CalculateFinalPrice(decimal basePrice) => _strategy.ApplyDiscount(basePrice);
}

// 2. OBSERVER PATTERN (Издател / Абонати чрез вградените C# Events)
public class CoursePublisher
{
    // В C# делегатът EventHandler<T> е стандартната, потокобезопасна реализация на Observer
    public event EventHandler<string> OnNewLectureUploaded;

    public void UploadLecture(string lectureTitle)
    {
        Console.WriteLine($"Качена е лекция: {lectureTitle}");
        // Уведомява всички записани абонати (ако има такива)
        OnNewLectureUploaded?.Invoke(this, lectureTitle);
    }
}

public class StudentSubscriber
{
    public string Name { get; }
    public StudentSubscriber(string name) => Name = name;

    public void HandleNotification(object sender, string lectureTitle)
        => Console.WriteLine($"  -> Студент {Name} получи известие за '{lectureTitle}'");
}

// 3. COMMAND PATTERN (Опаковане на операция като обект с поддръжка на UNDO)
public interface ICommand
{
    void Execute();
    void Undo();
}

public class EnrollCommand : ICommand
{
    private readonly List<string> _enrolledStudents;
    private readonly string _studentName;

    public EnrollCommand(List<string> students, string name)
    {
        _enrolledStudents = students;
        _studentName = name;
    }

    public void Execute() => _enrolledStudents.Add(_studentName);
    public void Undo() => _enrolledStudents.Remove(_studentName); // Връща състоянието назад!
}
```

---

## 🎯 РАЗДЕЛ 4: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Предпочитай композиция пред наследяване:** Шаблони като **Decorator** и **Strategy** ти позволяват да комбинираш поведения динамично без дълбоки йерархии от класове.
2. **Винаги ползвай `Lazy<T>` за Singleton:** Никога не пиши ръчни `if (instance == null)` проверки в съвременен C#.
3. **Разпознавай шаблоните в .NET библиотеките:**
   * `StreamReader` около `FileStream` е **Decorator**.
   * `StringBuilder` е **Builder**.
   * `IEnumerable<T>` / `foreach` е **Iterator**.
   * `Moq` (`new Mock<T>()`) е **Proxy**.
   * `event` / `EventHandler` е **Observer**.
4. **Не прекалявай с шаблоните:** Използвай шаблон само когато реално премахва дублиране на код или тежки `if-else` / `switch` конструкции.

---

---

## 📚 ПОЛЕЗНИ РЕСУРСИ И ПРИМЕРИ В GITHUB

Ето как да ги използваш в зависимост от това какво учиш в момента:

* **[csharp-design-patterns-for-humans](https://github.com)** *(от anupavanm)*: Най-доброто за **бързо разбиране на концепцията** с примери от реалния живот (врати, бургери, кафе, дистанционно). Идеално е за кратките справочници, когато искаш да схванеш *"защо съществува този шаблон"* за 30 секунди.
* **[uml-diagram-for-csharp-design-pattern-examples](https://github.com)** *(от takaakit)*: Най-доброто за **визуалната архитектура** — показва всички 23 класически GoF шаблона чрез интерактивни UML диаграми на класовете и връзките между тях.
* **[design-patterns-csharp](https://github.com)** *(от RefactoringGuru)*: Най-доброто за **модерен .NET 8 код и структура** — всеки шаблон е разделен в самостоятелен проект с подробни концептуални коментари и примери от реалния свят.

---
© 2026 Dev-Library / Архитектурно ядро


