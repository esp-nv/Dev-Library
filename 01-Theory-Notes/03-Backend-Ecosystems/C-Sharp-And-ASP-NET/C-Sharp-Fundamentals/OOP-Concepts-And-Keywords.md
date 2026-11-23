# 📘 Учебник: Обектно-Ориентирано Програмиране (OOP) и Паметта в C#

**Файл:** `oop-concepts-csharp.md` | **Ниво:** Архитектурно ядро / C# Internals  
**Технологии:** .NET CLR, Heap Object Layout, vtable (Virtual Method Table), Encapsulation, Polymorphism

---

## 🏗️ РАЗДЕЛ 1: КАК ОБЕКТИТЕ ЖИВЕЯТ В HEAP ПАМЕТТА (ENCAPSULATION & CONSTRUCTORS)

Когато напишеш `new Student("Иван")`, .NET заделя блок памет в **Heap-а**, който съдържа две скрити системни полета (**Object Header** и **TypeHandle** за типа на класа), следвани от стойностите на всички полета (`fields`). **Капсулацията** защитава тези полета от невалидни данни.

```csharp
// ==================== КАПСУЛАЦИЯ, PROPERTIES, READONLY И CONSTRUCTOR CHAINING ====================

using System;
using System.Collections.Generic;

public class Student
{
    // 1. PRIVATE FIELDS (Истинските данни в паметта - скрити от външния свят)
    private string _name;
    private readonly List<double> _grades; // 'readonly' позволява присвояване САМО при декларация или в конструктор

    // 2. PROPERTIES С ВАЛИДАЦИЯ (Контролиран достъп до полетата)
    public string Name
    {
        get => _name;
        private set // 'private set' позволява промяна само вътре в класа (Immutability отвън)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Името не може да бъде празно!");
            }
            _name = value;
        }
    }

    // 3. ЗАЩИТА НА КОЛЕКЦИИ (IReadOnlyCollection<T>)
    // Външният код може да чете оценките, но НЕ може да извика student.Grades.Clear()!
    public IReadOnlyCollection<double> Grades => _grades.AsReadOnly();

    // 4. CONSTRUCTOR CHAINING (: this(...))
    // Базов конструктор, който инициализира колекцията, за да избегнем NullReferenceException
    public Student()
    {
        _grades = new List<double>();
    }

    // Извиква първо празния конструктор чрез ': this()', а после задава името през валидиращото Property
    public Student(string name) : this()
    {
        Name = name;
    }

    public void AddGrade(double grade)
    {
        if (grade < 2.00 || grade > 6.00)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Оценката трябва да е между 2 и 6.");
        }
        _grades.Add(grade);
    }
}
```

---

## 🧬 РАЗДЕЛ 2: НАСЛЕДЯВАНЕ И ПОЛИМОРФИЗЪМ ПОД КАПАКА (VIRTUAL, OVERRIDE, NEW)

Полиморфизмът позволява да държиш обекти от различни подкласове в една обща колекция (`List<Person>`) и при извикване на метод .NET автоматично да намира правилната имплементация чрез **Virtual Method Table (vtable)** в паметта.

```csharp
// ==================== НАСЛЕДЯВАНЕ, BASE, VIRTUAL, OVERRIDE СРЕЩУ NEW ====================

using System;

public class Person
{
    public string FullName { get; }

    public Person(string fullName)
    {
        FullName = fullName;
    }

    // 'virtual' записва метода във vtable и разрешава на децата да го презапишат
    public virtual string GetRoleDescription() => \$"Човек: {FullName}";

    // Обикновен (не-виртуален) метод
    public string GetBasicInfo() => "Базова информация от Person";
}

public class Teacher : Person
{
    public string Subject { get; }

    // ': base(fullName)' предава параметъра нагоре към конструктора на родителя
    public Teacher(string fullName, string subject) : base(fullName)
    {
        Subject = subject;
    }

    // 'override' замества записа във vtable -> работи полиморфно дори през референция Person!
    public override string GetRoleDescription()
        => \$"{base.GetRoleDescription()} | Преподавател по {Subject}";

    // 'new' (Method Hiding) НЕ замества метода във vtable, а просто го скрива при директен достъп!
    public new string GetBasicInfo() => "Скрита информация от Teacher";
}

public class PolymorphismMemoryDemo
{
    public void ExplainVirtualVsNew()
    {
        // Променливата е от тип Person (Compile-time), но обектът в Heap-а е Teacher (Runtime)
        Person personRef = new Teacher("Георги Димитров", "C# OOP");

        // Извиква версията на Teacher, защото методът е 'virtual' + 'override' (проверява в Heap-а)
        Console.WriteLine(personRef.GetRoleDescription());

        // Извиква версията на Person, защото методът е скрит с 'new' (гледа типа на променливата!)
        Console.WriteLine(personRef.GetBasicInfo());
    }
}
```

---

## 🧩 РАЗДЕЛ 3: АБСТРАКЦИЯ (`ABSTRACT CLASS` СРЕЩУ `INTERFACE`)

Абстракцията скрива сложната имплементация и показва само необходимия договор. Изборът между абстрактен клас и интерфейс е една от най-честите изпитни теми.

```csharp
// ==================== ABSTRACT CLASS VS INTERFACE В C# 8+ ====================

using System;

// 1. INTERFACE (Договор за поведение - "Какво може да прави обектът?")
// Един клас може да имплементира МНОГО интерфейси (напр. IPrintable, ISavable)
public interface IExportable
{
    string ExportFormat { get; } // Свойство без поле в паметта
    string Export();             // Задължителен метод за имплементация

    // Default Interface Implementation (C# 8+): метод с тяло в интерфейс
    void LogExport() => Console.WriteLine(\$"Експортиране във формат {ExportFormat}...");
}

// 2. ABSTRACT CLASS (Частично завършен шаблон - "Какъв Е обектът?")
// НЕ може да се инстанциира директно с 'new DocumentBase()'. Един клас наследява САМО 1 базов клас.
public abstract class DocumentBase : IExportable
{
    // Може да има полета, състояние и конструктор (за разлика от интерфейса!)
    protected DateTime CreatedAt { get; }

    protected DocumentBase()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public abstract string ExportFormat { get; } // Задължава децата да го дефинират

    // Абстрактен метод (няма тяло - всяко дете го пише по свой начин)
    public abstract string Export();

    // Конкретен споделен метод за всички наследници
    public string GetTimestamp() => CreatedAt.ToString("yyyy-MM-dd HH:mm");
}

public class PdfReport : DocumentBase
{
    public override string ExportFormat => "PDF";

    public override string Export() => \$"PDF Документ, генериран на {GetTimestamp()}";
}
```

---

## 🔒 РАЗДЕЛ 4: SPECIAL KEYWORDS (`SEALED`, `STATIC`, `IS`, `AS` И PATTERN MATCHING)

Тези ключови думи контролират наследяването, споделената памет и безопасното преобразуване на типове в йерархията.

```csharp
// ==================== SEALED, STATIC И БЕЗОПАСЕН TYPE CASTING ====================

using System;

// 1. SEALED (Запечатан клас)
// Забранява на други класове да наследяват FinalExam (подобрява и скоростта на JIT компилатора)
public sealed class FinalExam
{
    public int MaxPoints { get; init; } = 100;
}

// 2. STATIC CLASS (Живее в специална High-Frequency Heap зона през целия живот на програмата)
// Няма инстанции ('new MathHelper()' е забранено), съдържа само static членове
public static class GradeConverter
{
    public static string ToText(double grade) => grade >= 5.50 ? "Отличен" : "Добър";
}

// 3. БЕЗОПАСЕН CASTING ('is' с Pattern Matching срещу 'as')
public class CastingSafetyDemo
{
    public void ExplainSafeCasting(Person person)
    {
        // ❌ ОПАСЕН CAST: Ако 'person' не е Teacher, хвърля InvalidCastException!
        // Teacher risky = (Teacher)person;

        // ✅ НАЧИН 1 ('as' оператор): Ако не е Teacher, връща 'null' вместо да гърми
        Teacher teacherAs = person as Teacher;
        if (teacherAs != null)
        {
            Console.WriteLine(teacherAs.Subject);
        }

        // ✅ НАЧИН 2 (Модерен 'is' Pattern Matching - най-чистият подход в C#):
        // Проверява типа И веднага създава локалната променлива 't', ако проверката успее!
        if (person is Teacher t)
        {
            Console.WriteLine(\$"Преподавател по: {t.Subject}");
        }
    }
}
```

---

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Дръж всички полета (`fields`) `private`:** Никога не излагай сурови списъци или полета директно — ползвай Properties с валидация и `IReadOnlyCollection<T>`.
2. **Не забравяй `virtual` на базовия метод:** Без `virtual` (или `abstract`) в родителя, не можеш да използваш `override` в наследника.
3. **Избягвай `new` за скриване на методи:** Почти винаги търсиш полиморфизъм с `virtual` + `override`, а не скриване на методите.
4. **Избирай `interface` за способности (`IComparable`, `IPayable`)** и **`abstract class` за споделено състояние и конструктор**.
5. **Използвай `if (obj is SubClass sub)`** вместо директно кастване `(SubClass)obj`, за да избегнеш `InvalidCastException`.

---
© 2026 Dev-Library / Пълен учебен модул по Обектно-Ориентирано Програмиране в C#
