[⬅️ 01-04. Dependency Injection Дълбоко гмуркане](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-06. Identity и Автентификация](01-06-ASP-NET-Core-Identity-Authentication.md)

---

# 01-05. Архитектурни слоеве, Сервизи и пренасяне на данни (DTOs)

**Цел:** Изучаване на трислойния модел на разработка (N-Tier Architecture), капсулиране на бизнес интелигентността в изолиран сервизен слой (Service Layer) и сигурен пренос на данни между базата и браузъра чрез DTO (Data Transfer Objects).

## Съдържание на темата
* [1. Трислойният софтуерен модел (Separation of Concerns)](#1-трислойният-софтуерен-модел-separation-of-concerns)
* [2. Защо е забранено да показваме Entity моделите във View слоя?](#2-защо-е-забранено-да-показваме-entity-моделите-във-view-слоя)
* [3. Практическа реализация на DTO патерна и AutoMapper в C#](#3-практическа-реализация-на-dto-патерна-и-automapper-в-c)
* [4. Системни капани: Циклични релации и Празни профили](#4-системни-капани-циклични-релации-и-празни-профили)

---

## 1. ТРИСЛОЙНИЯТ СОФТУЕРЕН МОДЕЛ (SEPARATION OF CONCERNS)

В професионалната уеб разработка е абсолютно забранено един файл да прави всичко — например уеб контролерът да чете директно от базата данни, да извършва математически изчисления и да форматира изходния текст. Проектът се разбива на три независими логически слоя, които комуникират помежду си единствено чрез Dependency Injection:

```text
  [ Presentation Layer ] ──> [ Business / Service Layer ] ──> [ Data Access Layer ] ──> [ SQL DB ]
     (Контролери / Views)          (Бизнес логика / Изчисления)     (DbContext / Entities)
```

1.  **Presentation Layer (Слой на представянето):** Твоите уеб контролери (`Controllers`). Тяхната единствена задача е да приемат HTTP заявката от браузъра, да я препратят надолу към сервизите и да върнат финалния отговор на клиента.
2.  **Business Logic Layer (Сервизен слой):** Чисти C# класове (напр. `EmployeeService`), където живее интелигентността на приложението — изчисляване на суми, валидиране на бизнес правила и проверки за сигурност.
3.  **Data Access Layer (Слой за достъп до данни):** Твоят `DbContext` и базата данни. Отговаря единствено за физическото извличане и записване на суровите байтове памет на твърдия диск.

---

## 2. ЗАЩО Е ЗАБРАНЕНО ДА ПОКАЗВАМЕ ENTITY МОДЕЛИТЕ ВЪВ VIEW СЛОЯ?

Коласите в нашия Data Access слой (Entities) описват 1:1 структурата на физическите таблици в SQL Server. Показването на тези модели директно на фронтенда или уеб браузъра на клиента е тежък архитектурен анти-патерн:

*   **Изтичане на конфиденциални данни (Security Over-exposure):** Ако твоето `Employee` Entity съдържа свойства като `Salary` или `PasswordHash`, изпращането на целия обект към клиента ще изложи тези тайни на показ в мрежовия пакет.
*   **Счупване на софтуера (Tight Coupling):** Всеки път, когато промениш име на колона в SQL Server, фронтендът и уеб екраните на клиента ще се счупят веднага, защото са обвързани директно с персистентния модел.

**Решението: Data Transfer Objects (DTOs) / View Models.** Това са леки, чисти софтуерни кутии, които съдържат единствено свойствата, необходими за конкретния уеб екран, изолирайки изцяло архитектурата на базата данни от външния свят.

---

## 3. ПРАКТИЧЕСКА РЕАЛИЗАЦИЯ НА DTO ПАТЕРНА И AUTOMAPPER В C#

Когато проектът нарасне, писането на ръчни равенства (`Dto.Name = entity.Name`) във всеки един сервиз става досадно. Използваме индустриалния стандарт **AutoMapper**, който автоматизира прехвърлянето на паметта.

*   **NuGet пакет за инсталация:** `AutoMapper.Extensions.Microsoft.DependencyInjection`

Искаме да прочетем данни за служител, но да скрием заплатата и паролата му от браузъра:

### 📄 1. Софтуерен код на Слоевете (Entities, DTOs, Profile, Service):

```csharp
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

// А. DATA ACCESS LAYER: Суровият персистентен модел
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public decimal Salary { get; set; } // 🔒 Трябва да остане скрита!
    public string PasswordHash { get; set; } = null!; // 🔒 Критична тайна!
}

// Б. PRESENTATION LAYER (DTO): Чиста кутия за уеб екрана
public class EmployeeSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!; // Ще сглобим двете имена в едно
}

// В. AUTOMAPPER PROFILE: Автоматичният преводач на памет
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeSummaryDto>()
            .ForMember(dest => dest.FullName, 
                       opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    }
}

// Г. BUSINESS LAYER (SERVICE): Използва DI за достъп до контекста и AutoMapper
public class EmployeeService
{
    private readonly IMapper _mapper;

    public EmployeeService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public List<EmployeeSummaryDto> GetPublicDashboard(List<Employee> rawEntities)
    {
        // Превръща целия масив от Entities в DTOs с един лек и чист ред код!
        return _mapper.Map<List<EmployeeSummaryDto>>(rawEntities);
    }
}
```

### 🎯 2. Очакван изход (Expected Output в Postman):
Ако в уеб контролера извикаш сервиза, крайният HTTP отговор съдържа само безопасни за сигурността данни:
```json
[
  {
    "id": 1,
    "fullName": "Иван Иванов"
  },
  {
    "id": 2,
    "fullName": "Мария Петрова"
  }
]
```

---

## 4. СИСТЕМНИ КАПАНИ: ЦИКЛИЧНИ РЕЛАЦИИ И ПРАЗНИ ПРОФИЛИ

При конфигурирането на преносния архитектурен слой съществуват два изключително тежки капана:

### 🚨 Капан №1: Безкраен цикъл при сериализация (JsonException Loop)
*   **Какво правиш:** Забравяш да използваш DTO патерна и връщаш директно Entity модел, който има връзка `1:Many` към друга таблица, а тя от своя страна има обратна релация нагоре.
*   **Точно съобщение за грешка на екрана (Exception Message):**
    ```text
    JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth.
    ```
*   **Защо се случва:** Когато .NET се опитва да превърне обекта в JSON текст, той влиза в безкрайна спирала: *Служителят зарежда своя Департамент ➔ Департаментът зарежда обратно списъка със Служители ➔ всеки Служител пак отваря Департамента...* RAM паметта се препълва и Kestrel забива.
*   **Как да го поправиш:** Създай изчистено `EmployeeDto`, съдържащо само плоски свойства (чисти стрингове и числа) без навигационни обекти, и мапвай към него.

### 🚨 Капан №2: Липсваща регистрация на AutoMapper (DI Срив)
*   **Какво правиш:** Разписваш перфектно твоя `MappingProfile` клас, инжектираш `IMapper _mapper` в конструктора на сервиза, но забравяш да активираш услугата в централния файл `Program.cs`.
*   **Точен софтуерен Crash (Exception Message) при стартиране на уеб сървъра:**
    ```text
    InvalidOperationException: Unable to resolve service for type 'AutoMapper.IMapper' while attempting to activate 'EmployeeService'.
    ```
*   **Защо се случва:** IoC контейнерът се опитва да създаде инстанция на твоя сервиз, но тъй като никой не е регистрирал интерфейса `IMapper` в системната колекция, DI двигателят се проваля.
*   **Как да го поправиш:** Отвори `Program.cs` и добави следния ред преди метода `builder.Build()`, който казва на .NET автоматично да сканира проекта и да зареди всички мапинг профили:
    ```csharp
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
    ```

---
[⬅️ 01-04. Dependency Injection Дълбоко гмуркане](01-04-ASP-NET-Core-Dependency-Injection-Deep-Dive.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 01-06. Identity и Автентификация](01-06-ASP-NET-Core-Identity-Authentication.md)

© 2026 Dev-Library / Архитектура: Слоеве, Сервизи и DTO Патерни
