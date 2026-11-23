[⬅️ 01-02. Dependency Injection Дълбоко гмуркане](01-02-ASP-NET-Core-Dependency-Injection-Deep-Dive.md) | 📂 Глава 01. Еволюция и Архитектура на ASP.NET | [🏠 Начало (ASP.NET)](README.md)

---

# 01-03. Архитектурни слоеве, Сервизи и пренасяне на данни (DTOs)

**Цел:** Изучаване на трислойния модел на разработка (N-Tier Architecture), изнасяне на бизнес логиката в изолиран сервизен слой (Service Layer) и сигурен пренос на данни между базата и браузъра чрез DTO (Data Transfer Objects) обекти.

## Съдържание на темата
* [1. Трислойният софтуерен модел (Separation of Concerns)](#1-трислойният-софтуерен-модел-separation-of-concerns)
* [2. Защо е забранено да показваме Entity моделите във View слоя?](#2-защо-е-забранено-да-показваме-entity-моделите-във-view-слоя)
* [3. Практическа реализация на DTO патерна в C#](#3-практическа-реализация-на-dto-патерна-в-c)

---

## 1. ТРИСЛОЙНИЯТ СОФТУЕРЕН МОДЕЛ (SEPARATION OF CONCERNS)

В професионалния софтуер е абсолютно забранено един файл да прави всичко — например уеб контролерът да чете директно от базата, да прави математически изчисления и да форматира HTML текста. Проектът се разбива на три независими логически слоя, които комуникират помежду си чрез Dependency Injection:

```text
  [ Presentation Layer ] ──> [ Business / Service Layer ] ──> [ Data Access Layer ] ──> [ SQL DB ]
     (Контролери / Views)          (Бизнес логика / Изчисления)     (DbContext / Entities)
```

1.  **Presentation Layer (Слой на представянето):** Твоите уеб контролери (`Controllers`). Тяхната единствена задача е да приемат HTTP заявката от браузъра, да я препратят надолу към сервизите и да върнат финалния визуален отговор на клиента.
2.  **Business Logic Layer (Сервизен слой):** Чисти C# класове (напр. `StudentService`), където живее интелигентността на приложението — изчисляване на балове, данъци, валидиране на бизнес правила и проверки за сигурност.
3.  **Data Access Layer (Слой за достъп до данни):** Твоят `DbContext` и базата данни. Отговаря единствено за физическото извличане и записване на суровите байтове памет на твърдия диск.

---
## 2. ЗАЩО Е ЗАБРАНЕНО ДА ПОКАЗВАМЕ ENTITY МОДЕЛИТЕ ВЪВ VIEW СЛОЯ?

Когато използваме Entity Framework Core, класовете в нашия Data Access слой (Entities) описват 1:1 структурата на физическите таблици в SQL Server. Показването на тези модели директно на фронтенда или уеб браузъра на клиента е тежък архитектурен анти-патерн:

*   **Изтичане на конфиденциални данни (Security Over-exposure):** Ако твоето `User` Entity съдържа свойства като `PasswordHash`, `InternalSecurityStamp` или `FinancialMetadata`, изпращането на целия обект към клиента ще изложи тези тайни на показ в мрежовия пакет.
*   **Счупване на софтуера (Tight Coupling & Serialization Loops):** Ако имаш циклични релации в базата (напр. *Курс пази списък от Студенти, а всеки Студент пази линк към своя Курс*), серийният JSON конвертор в браузъра ще влезе в безкраен цикъл и приложението ще забие фатално.
*   **Прекомерен обем на трафика (Over-fetching):** За да покажеш просто списък с имена в падащо меню, ти изтегляш и пренасяш по мрежата дълги текстови описания и тежки системни дати.

**Решението: Data Transfer Objects (DTOs) / View Models.** Това са леки, чисти софтуерни кутии, които съдържат единствено свойствата, необходими за конкретния уеб екран, изолирайки изцяло архитектурата на базата данни от външния свят.

---

## 3. ПРАКТИЧЕСКА РЕАЛИЗАЦИЯ НА DTO ПАТЕРНА В C#

### А. Базовият persistens модел (Entity) — Скрит в Data слоя
```csharp
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public decimal Salary { get; set; } // 🔒 Скрита чувствителна информация!
    public string PasswordHash { get; set; } = null!; // 🔒 Критична системна тайна!
    public DateTime CreatedOn { get; set; }
}
```

### Б. Чистият преносен модел (DTO) — Публичен във View слоя
```csharp
public class EmployeeSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    // Пази само нужните за екрана данни, без заплати и пароли!
}
```

### В. Ръчно мапване в Сервизния слой (Сравнителен модел)
```csharp
public class EmployeeService : IEmployeeService
{
    private readonly SchoolDbContext _context;

    public EmployeeService(SchoolDbContext context)
    {
        _context = context;
    }

    public List<EmployeeSummaryDto> GetPublicDashboard()
    {
        return _context.Employees
            .Select(e => new EmployeeSummaryDto
            {
                Id = e.Id,
                FullName = e.FirstName + " " + e.LastName // Ръчно сглобяване
            })
            .ToList();
    }
}
```

---

## 🏎️ AUTOMAPPER: АВТОМАТИЗАЦИЯ НА ПРЕХВЪРЛЯНЕТО НА ДАННИ

Когато софтуерният проект нарасне и имаш стотици таблици, писането на ръчни равенства (`Dto.Name = entity.Name`) във всеки един сервиз се превръща в досадна, повтаряща се работа. В индустрията и в лекциите на SoftUni се използва специална библиотека, наречена **AutoMapper**.

*   **NuGet пакет за инсталация:** `AutoMapper.Extensions.Microsoft.DependencyInjection`
*   **Как работи:** Ти създаваш един конфигурационен профил, а AutoMapper сканира имената на свойствата и автоматично прехвърля паметта от Entity към DTO с един единствен чист ред код: `_mapper.Map<EmployeeSummaryDto>(employeeEntity);`.

### Конфигуриране на Мапинг Профил (Mapping Profile):
```csharp
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Казваме на AutoMapper как автоматично да превежда обектите
        CreateMap<Employee, EmployeeSummaryDto>()
            .ForMember(dest => dest.FullName, 
                       opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
    }
}
```

### Използване чрез Dependency Injection в Сервиза:
```csharp
public class EmployeeService : IEmployeeService
{
    private readonly SchoolDbContext _context;
    private readonly IMapper _mapper; // Инжектираме автоматичния преводач

    public EmployeeService(SchoolDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public List<EmployeeSummaryDto> GetPublicDashboard()
    {
        var entities = _context.Employees.ToList();
        
        // Превръща целия масив от Entities в DTOs с един лек ред код!
        return _mapper.Map<List<EmployeeSummaryDto>>(entities);
    }
}
```

---
[⬅️ 01-02. Dependency Injection Дълбоко гмуркане](01-02-ASP-NET-Core-Dependency-Injection-Deep-Dive.md) | 📂 Глава 01. Еволюция и Архитектура на ASP.NET | [🏠 Начало (ASP.NET)](README.md)

© 2026 Dev-Library / Архитектура: Слоеве, Сервизи и DTO Патерни
