# 📘 Учебник: Разширено Мокинг Ядро и Test-Driven Development (TDD) в C#

**Файл:** `tdd-mocking-advanced-csharp.md` | **Ниво:** Архитектурно ядро / C# Advanced  
**Технологии:** .NET, NUnit, Moq (`Castle.DynamicProxy`)

---

## 🏗️ РАЗДЕЛ 1: КАК МОКИНГЪТ РАБОТИ ПОД КАПАКА В .NET ПАМЕТТА (Castle DynamicProxy)

Когато напишеш `new Mock<IStudentRepository>()`, библиотеката Moq **не връща** реален клас, написан от теб. Вместо това .NET използва технологията **Reflection.Emit** и **Castle DynamicProxy**, за да компилира в реално време (Runtime) нов скрит клас директно в Heap паметта.

```csharp
// ==================== КАКВО СЕ СЛУЧВА В ПАМЕТТА ПРИ МОКИРАНЕ ====================

// 1. Дефинираме чист договор (Interface) - единственото, от което нашият Service трябва да зависи
public interface IStudentRepository
{
    Student GetById(int id);              // Метод за четене от базата
    void Save(Student student);           // Метод за запис в базата
    bool Delete(int id);                  // Метод за изтриване от базата
}

// 2. Модел на данните в системата StudentManagement
public class Student
{
    public int Id { get; set; }           // Уникален идентификатор в паметта
    public string Name { get; set; }      // Име на студента
    public double AverageGrade { get; set; } // Среден успех за изчисления
    public bool IsGraduated { get; set; } // Статус на завършване
}

// 3. Как Moq генерира Proxy обекта в Heap паметта (Симулация на вътрешния механизъм):
public class ProxyDemo
{
    public void ExplainMemoryProxy()
    {
        // Създаваме обвивката (Контролния панел) на Mock обекта в паметта
        var mockRepo = new Mock<IStudentRepository>();

        // .Object е реалният динамично генериран клас (напр. Castle.Proxies.IStudentRepositoryProxy),
        // който наследява IStudentRepository и прехваща всяко извикване на метод!
        IStudentRepository proxyInstance = mockRepo.Object;

        // ВСИЧКИ повиквания към proxyInstance минават през вътрешен списък Invocations в mockRepo,
        // където се записват: кой метод е извикан, с какви аргументи и колко пъти.
    }
}
```

---

## 🔴🟢🔵 РАЗДЕЛ 2: ПЪЛЕН ПРАКТИЧЕСКИ TDD ЦИКЪЛ (RED ➔ GREEN ➔ REFACTOR)

Тук изграждаме бизнес логика за дипломиране на студент (`GraduateStudent`) в `StudentService`, спазвайки стриктно трите фази на TDD. Първо пишем теста, а кодът се ражда от него.

```csharp
// ==================== СТЪПКА 1: RED (ПИШЕМ ТЕСТА ПРЕДИ КОДА) ====================
using NUnit.Framework;
using Moq;
using System;

[TestFixture]
public class StudentServiceTddTests
{
    private Mock<IStudentRepository> _mockRepo; // Декларираме Mock обвивката за репозиторито
    private StudentService _service;            // Декларираме системата под тест (SUT - System Under Test)

    [SetUp]
    public void Setup()
    {
        // Инициализираме нов чист Mock преди ВСЕКИ тест, за да няма замърсяване на състоянието
        _mockRepo = new Mock<IStudentRepository>(MockBehavior.Loose);

        // Инжектираме прокси обекта (.Object) в конструктора на сървиса (Dependency Injection)
        _service = new StudentService(_mockRepo.Object);
    }

    [Test]
    public void GraduateStudent_ShouldThrowException_WhenGradeIsBelowMinimum()
    {
        // ARRANGE: Подготвяме студент с успех 5.20 (под минимума от 5.50 за дипломиране)
        var weakStudent = new Student { Id = 1, Name = "Иван", AverageGrade = 5.20 };

        // Настройваме Stub поведение: когато сървисът потърси ID 1, върни weakStudent
        _mockRepo.Setup(r => r.GetById(1)).Returns(weakStudent);

        // ACT & ASSERT: Очакваме InvalidOperationException, защото успехът е нисък
        // На този етап методът GraduateStudent ОЩЕ НЕ СЪЩЕСТВУВА -> Компилаторът/Тестът светва в ЧЕРВЕНО (RED)!
        Assert.Throws<InvalidOperationException>(() => _service.GraduateStudent(1));
    }
}

// ==================== СТЪПКА 2: GREEN (МИНИМАЛНИЯТ КОД ЗА УСПЕХ) ====================
public class StudentService
{
    private readonly IStudentRepository _repository; // Пазим референция към интерфейса, не към конкретна база

    public StudentService(IStudentRepository repository)
    {
        _repository = repository; // Записваме инжектираната зависимост
    }

    public void GraduateStudent(int studentId)
    {
        // 1. Взимаме студента от репозиторито
        var student = _repository.GetById(studentId);

        // 2. Пишем най-простата възможна проверка, за да мине червеният тест
        if (student.AverageGrade < 5.50)
        {
            throw new InvalidOperationException("Успехът е твърде нисък за дипломиране.");
        }

        // 3. Маркираме като дипломиран и записваме
        student.IsGraduated = true;
        _repository.Save(student);
    }
}

// ==================== СТЪПКА 3: REFACTOR (ИЗЧИСТВАНЕ И ЗАЩИТА НА КОДА) ====================
public class StudentServiceRefactored
{
    private const double MinGraduationGrade = 5.50; // Махаме магическото число 5.50 в константа
    private readonly IStudentRepository _repository;  // Зависимост от абстракция

    public StudentServiceRefactored(IStudentRepository repository)
    {
        // Защита от подаване на null зависимост още в конструктора (Guard Clause)
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public void GraduateStudent(int studentId)
    {
        // Извличаме студента от инжектираното хранилище
        var student = _repository.GetById(studentId);

        // Добавяме защитна проверка, ако студентът не съществува в базата
        if (student == null)
        {
            throw new ArgumentException($"Студент с ID {studentId} не е намерен.");
        }

        // Използваме константата вместо хардкоднато число за по-чист код
        if (student.AverageGrade < MinGraduationGrade)
        {
            throw new InvalidOperationException($"Необходим е успех минимум {MinGraduationGrade:F2}.");
        }

        // Обновяваме състоянието в паметта
        student.IsGraduated = true;

        // Запазваме промените през интерфейса (това ще тестваме с Mock.Verify)
        _repository.Save(student);
    }
}
```

---

## 🛠️ РАЗДЕЛ 3: РАЗШИРЕНИ MOQ КОНФИГУРАЦИИ И ЗАЛАВЯНЕ В ПАМЕТТА

Тук разглеждаме професионалните инструменти на Moq за контрол върху последователни извиквания, улавяне на обекти в реално време (`Callback`) и строга проверка на странични ефекти.

```csharp
// ==================== СЛОЖНИ MOQ СЦЕНАРИИ В ACTION ====================

[TestFixture]
public class AdvancedMockingScenariosTests
{
    [Test]
    public void SetupSequence_ShouldSimulateChangingDatabaseState()
    {
        // ARRANGE: Създаваме Mock на репозиторито
        var mockRepo = new Mock<IStudentRepository>();

        // Конфигурираме поредица от отговори при последователни извиквания на същия метод:
        mockRepo.SetupSequence(r => r.GetById(10))
                .Returns((Student)null)                          // 1-во извикване: студентът още не е в базата
                .Returns(new Student { Id = 10, Name = "Мария" }) // 2-ро извикване: вече е записан
                .Throws(new TimeoutException("Базата прекъсна!")); // 3-то извикване: симулираме срив на връзката

        // ACT & ASSERT: Проверяваме всяко извикване поред
        Assert.IsNull(mockRepo.Object.GetById(10));               // Първият път връща null
        Assert.AreEqual("Мария", mockRepo.Object.GetById(10).Name); // Вторият път връща Мария
        Assert.Throws<TimeoutException>(() => mockRepo.Object.GetById(10)); // Третият път гърми с грешка
    }

    [Test]
    public void Callback_ShouldCaptureMutatedObjectBeforeSaving()
    {
        // ARRANGE: Искаме да видим ТОЧНО какъв обект Service-ът подава на .Save() в паметта
        var mockRepo = new Mock<IStudentRepository>();
        var student = new Student { Id = 5, Name = "Георги", AverageGrade = 5.80, IsGraduated = false };
        Student capturedStudentInMemory = null; // Променлива-капан за прихващане на аргумента

        // Настройваме Stub за четене
        mockRepo.Setup(r => r.GetById(5)).Returns(student);

        // Настройваме Callback: в момента, в който Service извика .Save(s), записваме референцията 's'
        mockRepo.Setup(r => r.Save(It.IsAny<Student>()))
                .Callback<Student>(s => capturedStudentInMemory = s);

        var service = new StudentServiceRefactored(mockRepo.Object);

        // ACT: Изпълняваме бизнес логиката
        service.GraduateStudent(5);

        // ASSERT: Проверяваме прихванатия обект в паметта
        Assert.IsNotNull(capturedStudentInMemory);                // Уверяваме се, че Save е получил обект
        Assert.IsTrue(capturedStudentInMemory.IsGraduated);       // Проверяваме дали флагът е сменен на true
        Assert.AreSame(student, capturedStudentInMemory);         // Проверяваме дали сочат към един и същ адрес в Heap-а
    }

    [Test]
    public void VerifyNoOtherCalls_ShouldGuaranteeNoSideEffects()
    {
        // ARRANGE: Проверяваме дали методът НЕ прави излишни заявки (напр. не трие нищо по погрешка)
        var mockRepo = new Mock<IStudentRepository>();
        var student = new Student { Id = 2, Name = "Елена", AverageGrade = 6.00 };
        mockRepo.Setup(r => r.GetById(2)).Returns(student);

        var service = new StudentServiceRefactored(mockRepo.Object);

        // ACT: Дипломираме Елена
        service.GraduateStudent(2);

        // ASSERT: Изброяваме ВСИЧКИ очаквани и разрешени взаимодействия с базата
        mockRepo.Verify(r => r.GetById(2), Times.Once);           // Трябва да е прочетен точно 1 път
        mockRepo.Verify(r => r.Save(student), Times.Once);        // Трябва да е записан точно 1 път

        // ЖЕЛЯЗНА ЗАЩИТА: Ако сървисът е извикал Delete() или втори път GetById(), тестът ГЪРМИ!
        mockRepo.VerifyNoOtherCalls();
    }
}
```

## ⚠️ РАЗДЕЛ 4: MOCKBEHAVIOR.STRICT СРЕЩУ MOCKBEHAVIOR.LOOSE (КОНТРАСТ)

Една от най-честите архитектурни грешки е неправилният избор на поведение на проксито при създаване.
```csharp
// ==================== LOOSE VS STRICT MODE В ПАМЕТТА ====================
public class MockBehaviorComparison
{
public void CompareModes()
{
      // 1. LOOSE MODE (По подразбиране) - "Толерантно прокси"
      // Ако извикаш метод без Setup, НЕ гърми. Връща default стойност (null за обекти, 0 за int, false за bool).
    var looseMock = new Mock(MockBehavior.Loose);
    bool deleteResult = looseMock.Object.Delete(99); // Връща false без грешка, въпреки че няма Setup!
      // 2. STRICT MODE - "Безмилостно прокси"
      // Ако извикаш КАКЪВТО И ДА Е метод без изричен .Setup(), хвърля MockException!
     var strictMock = new Mock(MockBehavior.Strict);
      // Следният ред ще хвърли: Moq.MockException: IStudentRepository.Delete(99) invocation failed with mock behavior Strict.
      // strictMock.Object.Delete(99);
      // За да мине в Strict Mode, ЗАДЪЛЖИТЕЛНО трябва да опишеш поведението предварително:
    strictMock.Setup(r => r.Delete(99)).Returns(true);
    bool strictResult = strictMock.Object.Delete(99); // Вече работи и връща true
  }
}
```

🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. Никога не мокирай класове без интерфейси (или virtual методи): Castle.DynamicProxy не може да презапише обикновен не-виртуален метод на конкретен клас и ще хвърли NotSupportedException.
2. Тествай поведението, не имплементацията: Не пиши 20 реда Verify за всеки вътрешен помощен метод. Проверявай само критичните изходи (запис в база, изпращане на имейл, хвърляне на изключение).
3. Един Assert / Една логическа концепция на тест: В TDD всеки тест отговаря на точно един бизнес въпрос (напр. "Какво става при отрицателно ID?").
4. Изолация на [SetUp]: Винаги създавай нови инстанции на Mock<T> в [SetUp] метода, за да не се натрупват броячите на извикванията (Times.Once) от предишни тестове.
----
© 2026 Dev-Library / Пълен учебен модул по C# Unit Testing, Mocking & TDD
