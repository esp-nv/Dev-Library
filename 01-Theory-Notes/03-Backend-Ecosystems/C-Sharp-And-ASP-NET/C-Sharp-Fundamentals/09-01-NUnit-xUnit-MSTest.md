# 📘 Справочник: NUnit срещу xUnit срещу MSTest в .NET

**Статус:** Архитектурно сравнение | **Цел:** Избор на правилната тестова рамка и разбиране на разликите в паметта

---

## 📊 1. СРАВНИТЕЛНА МАТРИЦА НА АТРИБУТИТЕ И ЖИЗНЕНИЯ ЦИКЪЛ

| Характеристика | NUnit | xUnit | MSTest (MS Unit) |
| :--- | :--- | :--- | :--- |
| **Клас за тестове** | `[TestFixture]` *(незадължителен)* | Няма *(всеки `public` клас)* | `[TestClass]` |
| **Единичен тест** | `[Test]` | `[Fact]` | `[TestMethod]` |
| **Тест с параметри** | `[TestCase(1, 2)]` | `[Theory]` + `[InlineData(1, 2)]` | `[DataTestMethod]` + `[DataRow(1, 2)]` |
| **Подготовка (Setup)** | `[SetUp]` метод | Конструкторът на класа | `[TestInitialize]` метод |
| **Почистване (Cleanup)** | `[TearDown]` метод | `IDisposable` (`Dispose()`) | `[TestCleanup]` метод |
| **Изолация в паметта** | **Една** инстанция за всички тестове | **Нова** инстанция за всеки тест | **Нова** инстанция за всеки тест |
| **Проверка за грешка** | `Assert.Throws<T>(...)` | `Assert.Throws<T>(...)` | `Assert.ThrowsException<T>(...)` |

---

## 💻 2. ЕДИН И СЪЩ ТЕСТ НА ТРИТЕ РАМКИ (КОДОВ КОНТРАСТ)

Ето как изглежда тестването на `StudentService` при опит за дипломиране с нисък успех:

### 🟢 1. NUnit (Стандартът в SoftUni и изпитите)
```csharp
using NUnit.Framework;
using Moq;
using System;

[TestFixture] // Маркира класа като контейнера за тестове
public class StudentServiceNUnitTests
{
    private Mock<IStudentRepository> _mockRepo;
    private StudentService _service;

    [SetUp] // Изпълнява се преди ВСЕКИ тест, за да занули състоянието на единствената инстанция
    public void Setup()
    {
        _mockRepo = new Mock<IStudentRepository>();
        _service = new StudentService(_mockRepo.Object);
    }

    [Test] // Стандартен тест
    public void GraduateStudent_ShouldThrow_WhenGradeIsLow()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = 5.00 });
        Assert.Throws<InvalidOperationException>(() => _service.GraduateStudent(1));
    }

    [TestCase(5.50)] // Тест с подадени параметри
    [TestCase(6.00)]
    public void GraduateStudent_ShouldSucceed_WhenGradeIsValid(double grade)
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = grade });
        Assert.DoesNotThrow(() => _service.GraduateStudent(1));
    }
}
```

### 🔵 2. xUnit (Модерният стандарт за .NET Core / .NET 8+)
```csharp
using Xunit;
using Moq;
using System;

// Няма атрибут на класа! xUnit създава чисто нова инстанция на този клас за ВСЕКИ тест.
public class StudentServiceXUnitTests : IDisposable
{
    private readonly Mock<IStudentRepository> _mockRepo;
    private readonly StudentService _service;

    // Конструкторът играе ролята на [SetUp]
    public StudentServiceXUnitTests()
    {
        _mockRepo = new Mock<IStudentRepository>();
        _service = new StudentService(_mockRepo.Object);
    }

    [Fact] // Означава "Факт, който винаги трябва да е верен" (тест без параметри)
    public void GraduateStudent_ShouldThrow_WhenGradeIsLow()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = 5.00 });
        Assert.Throws<InvalidOperationException>(() => _service.GraduateStudent(1));
    }

    [Theory] // Означава "Теория, която проверяваме с набор от данни"
    [InlineData(5.50)]
    [InlineData(6.00)]
    public void GraduateStudent_ShouldSucceed_WhenGradeIsValid(double grade)
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = grade });
        var exception = Record.Exception(() => _service.GraduateStudent(1));
        Assert.Null(exception); // Проверяваме, че не е хвърлена грешка
    }

    // Играе ролята на [TearDown] (почистване на ресурси след теста)
    public void Dispose() { }
}
```

### 🟣 3. MSTest (Официалната рамка на Microsoft)
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

[TestClass] // Задължителен атрибут за класа в MSTest
public class StudentServiceMSTest
{
    private Mock<IStudentRepository> _mockRepo;
    private StudentService _service;

    [TestInitialize] // Еквивалент на [SetUp]
    public void Init()
    {
        _mockRepo = new Mock<IStudentRepository>();
        _service = new StudentService(_mockRepo.Object);
    }

    [TestMethod] // Задължителен атрибут за всеки тест
    public void GraduateStudent_ShouldThrow_WhenGradeIsLow()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = 5.00 });
        Assert.ThrowsException<InvalidOperationException>(() => _service.GraduateStudent(1));
    }

    [DataTestMethod] // Тест с параметри
    [DataRow(5.50)]
    [DataRow(6.00)]
    public void GraduateStudent_ShouldSucceed_WhenGradeIsValid(double grade)
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Student { AverageGrade = grade });
        _service.GraduateStudent(1); // В MSTest ако няма грешка, тестът минава автоматично
    }
}
```

---

## ⚖️ 3. ПЛЮСОВЕ, МИНУСИ И КОЙ ЗА КАКВО Е НАЙ-ПОДХОДЯЩ

### 🟢 NUnit
* **Плюсове:**
  * Изключително богат и четим синтаксис (`Assert.That(x, Is.EqualTo(y))`).
  * Най-гъвкава поддръжка на параметризирани тестове (`[TestCase]`, `[TestCaseSource]`, `[Range]`).
  * Огромна общност и хиляди готови примери.
* **Минуси:**
  * Споделя една инстанция на тестовия клас между тестовете – ако забравиш да нулираш поле в `[SetUp]`, един тест може да счупи следващия.
* **Най-подходящ за:** Изпити (SoftUni), проекти със сложна бизнес логика, изискваща много комбинации от входни данни, и утвърдени .NET системи.

### 🔵 xUnit
* **Плюсове:**
  * **Максимална изолация:** Създава нов обект за всеки тест – невъзможно е тестовете да си пречат през полетата на класа.
  * **Модерен C# дизайн:** Използва стандартни конструктори и `IDisposable` вместо изкуствени атрибути.
  * Отлична поддръжка на паралелно изпълнение на тестове (много бърз при големи проекти).
* **Минуси:**
  * По-беден набор от вградени `Assert` методи (често се комбинира с библиотеката **FluentAssertions** за по-четими проверки).
  * Няма вграден `Assert.DoesNotThrow` (ползва се `Record.Exception`).
* **Най-подходящ за:** Нови .NET Core / .NET 8+ проекти, микросървиси, облачни приложения и екипи, търсещи висока скорост и строга изолация.

### 🟣 MSTest
* **Плюсове:**
  * Вграден директно във Visual Studio без нужда от външни пакети от трети страни.
  * Официална дългосрочна поддръжка от Microsoft.
  * Лесен и предвидим за базови нужди.
* **Минуси:**
  * По-тромав и многословен синтаксис (`[TestClass]`, `[TestMethod]`, `[DataTestMethod]`).
  * По-бавно развитие на нови функции спрямо xUnit и NUnit.
* **Най-подходящ за:** Големи корпоративни (Enterprise) компании, които имат строги правила за сигурност и разрешават използването само на официални инструменти на Microsoft.

---
© 2026 Dev-Library / Архитектурен справочник за .NET Тестване
