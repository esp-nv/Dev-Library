[⬅️ 01-07. Етапи за изграждане](01-07-ASP-NET-Core-Application-Architecture-Steps.md) | [📂01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [⏩ Глава 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)

---

# 01-08. ASP.NET Core Fundamentals (Глобален Инженерен Пищов)

**Цел:** Бърза, синтактична и визуална справка за изпити и интервюта, групирана по категории: съпоставка на уеб конвейера, Dependency Injection регистрации, форми, Model Binding и AutoMapper шаблони.

## 🧭 Съдържание на пищова
* [1. Синтаксис на Middleware конвейера (Редът в Program.cs)](#1-синтаксис-на-middleware-конвейера-редът-в-programcs)
* [2. Dependency Injection матрица на жизнените цикли](#2-dependency-injection-матрица-на-жизнените-цикли)
* [3. Model Binding и източници на уеб параметри](#3-model-binding-и-източници-на-параметри)
* [4. Шаблон за автоматичен AutoMapper превод](#4-automapper-шаблон-и-трислоен-превод)
* [5. Справочна таблица на критичните сривове (Exceptions)](#5-таблица-на-критичните-сривове-exceptions-и-тяхното-лечение)


---

## 1. СИНТАКСИС НА MIDDLEWARE КОНВЕЙЕРА (РЕДЪТ В PROGRAM.CS)

Редът на подредба в софтуерния тръбопровод (Pipeline) е закован софтуерно. В входящата посока заявката се филтрира отгоре-надолу, а в изходящата отговорът се връща отдолу-нагоре.

```csharp
var builder = WebApplication.CreateBuilder(args);
// ➔ Място за DI Регистрации (Services)
var app = builder.Build();

// 1. ИЗКЛЮЧЕНИЯ: Трябва да е най-отгоре, за да хване абсолютно всички грешки надолу
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

// 2. СИГУРНОСТ НА МРЕЖАТА: Пренасочва http към https порт
app.UseHttpsRedirection();

// 3. СТАТИЧНИ ФАЙЛОВЕ: Връща директно CSS/JS/Снимки от /wwwroot (Бърз изход)
app.UseStaticFiles();

// 4. МАРШРУТИЗАЦИЯ: Разглобява URL адреса
app.UseRouting();

// 5. АУТЕНТИФИКАЦИЯ: Проверява "Кой си ти?" (Разчита бисквитки/токени)
app.UseAuthentication();

// 6. АВТОРИЗАЦИЯ: Проверява "Имаш ли права?" (Изисква роли/политики)
app.UseAuthorization();

// 7. ЕНДПОИНТИ: Изпълнява реалния C# код на Контролера
app.MapControllers();

app.Run();
```

### ⚡ Бърз Use / Run / Map синтаксис за къстъм филтри:
```csharp
// Ръчно прихващане и логване (Изисква извикване на next)
app.Use(async (context, next) => {
    Console.WriteLine(\$"[ВХОД] Заявка към: {context.Request.Path}");
    await next(); // Извиква следващия софтуерен блок
});

// Терминална тапа (Прекъсва конвейера, няма next)
app.Run(async context => {
    await context.Response.WriteAsync("Край на тръбопровода.");
});
```

---

## 2. DEPENDENCY INJECTION МАТРИЦА НА ЖИЗНЕНИТЕ ЦИКЛИ

Контейнерът управлява паметта на обектите в Heap структурата на базата на три Add метода в `Program.cs`:

| DI Метод в C# | Честота на пресъздаване в паметта | Основно бизнес предназначение |
| :--- | :--- | :--- |
| **`builder.Services.AddTransient<I, S>();`** | **Всеки път.** Напълно нов обект при всяко единично поискване в кода. | Леки услуги без състояние (`Stateless`), бързи калкулатори и парсери. |
| **`builder.Services.AddScoped<I, S>();`** | **Веднъж на HTTP заявка.** Обектът се споделя, докато браузърът не получи отговор. | **Златен стандарт за бизнес среди.** Всички Сервизи и `DbContext` контексти. |
| **`builder.Services.AddSingleton<I, S>();`**| **Веднъж за целия живот на сървъра.** Всички потребители четат един общ обект. | Тежки RAM кешове, фонови работници (`Background Tasks`) и конфигурации. |

---

## 3. MODEL BINDING И ИЗТОЧНИЦИ НА ПАРАМЕТРИ

Контролерите използват специфични атрибути, за да извлекат точно парчетата от HTTP пакета:

```csharp
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpPost("/api/products/update/{id}")]
    public IActionResult UpdateProduct(
        [FromRoute] int id,                      // Извлича от самия URL път (/update/5)
        [FromQuery] string lang,                 // Извлича от Query String (?lang=bg)
        [FromHeader(Name = "X-Agent")] string ag,// Извлича метаданна от Headers на пакета
        [FromBody] ProductInputDto dto)         // Извлича и десериализира JSON тялото (Body)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState); // Сървърна валидация
        return Ok(dto);
    }
}
```

---

## 4. AUTOMAPPER ШАБЛОН И ТРИСЛОЕН ПРЕВОД

Използва се за автоматично прехвърляне на паметта от Entity модели (база) към чисти DTO кутии (уеб интерфейс), спестявайки ръчни равенства.

```csharp
// 1. Профилът в бизнес слоя (Services/Mapping/MappingProfile.cs)
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.FullName, 
                       opt => opt.MapFrom(src => \$"{src.FirstName} {src.LastName}"));
    }
}

// 2. Активирането на сканирането в Program.cs:
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 3. Използване чрез Dependency Injection в Сервиза:
public class StudentService
{
    private readonly IMapper _mapper;
    public StudentService(IMapper mapper) { _mapper = mapper; }

    public StudentDto GetStudentDashboard(Student studentEntity)
    {
        // Превежда паметта автоматично с един чист ред!
        return _mapper.Map<StudentDto>(studentEntity);
    }
}
```

---

## 5. ТАБЛИЦА НА КРИТИЧНИТЕ СРИВОВЕ (EXCEPTIONS) И ТЯХНОТО ЛЕЧЕНИЕ

| Какво изписва .NET конзолата? (Exception) | Защо софтуерът гърми? (Причина) | Точна стъпка за поправка (Решение) |
| :--- | :--- | :--- |
| `AmbiguousMatchException: The request matched multiple endpoints.` | Имаш два C# уеб метода, които сочат към напълно еднакъв URL адрес в квадратните скоби. | Промени текста на единия от двата маршрута (напр. от `/list` на `/list/new`), за да бъде напълно уникален. |
| `InvalidOperationException: Cannot consume scoped service from singleton.` | **Пленена зависимост (Captive Dependency).** Вечен Singleton клас иска през конструктора си Scoped DbContext. | Смени жизнения цикъл на Singleton класа към Scoped или Transient, за да се изравнят времената им в паметта. |
| `InvalidOperationException: Headers are read-only, response has already started.` | Опитваш се да добавяш или променяш Headers на HTTP отговора **СЛЕД** като вече си извикала `await next();`. | Премести кода за промяна на Headers **ПРЕДИ** извикването на `await next();` — докато пакетът все още е локален. |
| `JsonException: A possible object cycle was detected.` | Върнала си директно Entity модел с цикличен релационен `1:Many` линк към съседна таблица (Безкраен сериен JSON цикъл). | Създай плоско DTO, съдържащо само обикновени стрингове и числа, и мапвай към него през AutoMapper Profile. |
| `InvalidOperationException: Unable to resolve service for type 'IMapper'.` | Инжектирала си интерфейс в конструктора на сервиза, но си забравила да го регистрираш като услуга в `Program.cs`. | Добави нужния Add метод (напр. `builder.Services.AddAutoMapper(...)`) в `Program.cs` преди метода `builder.Build()`. |

---
[⬅️ 01-07. Етапи за изграждане](01-07-ASP-NET-Core-Application-Architecture-Steps.md) | [📂01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [⏩ Глава 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)

© 2026 Dev-Library / Справочник: Глобален Fundamentals Пищов
