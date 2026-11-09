[⬅️ 01-08. Глобален Пищов](01-08-ASP-NET-Core-Fundamentals-CheatSheet.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [01-10. Reflection и Attributes (Дълбок Архитектурен Справочник)](01-10-Reflection-And-Attributes.md)

---

# 01-09. Наръчник по конфигурация: AutoMapper и Identity в папка Fundamentals

**Цел:** Пълна синтактична и практическа справка за свързване, инжектиране и управление на AutoMapper Профили и ASP.NET Core Identity среди в `Program.cs` и Контролерите.

## 🧭 Съдържание на пищова
* [1. AutoMapper Конфигурация: От-До](#1-automapper-конфигурация-от-до)
* [2. ASP.NET Core Identity Конфигурация: От-До](#2-aspnet-core-identity-конфигурация-от-до)
* [3. Справочник за често срещани сривове (Exceptions) и Решения](#3-справочник-за-често-срещани-сривове-exceptions-и-решения)

---

## 1. AUTOMAPPER КОНФИГУРАЦИЯ: ОТ-ДО

### ⚙️ Стъпка 1: Регистрация в Program.cs
Преди метода `builder.Build()`, добави следния ред, за да накараш IoC контейнера да сканира твоите софтуерни профили:
```csharp
// Сканира автоматично цялата сглобка (Assembly) за наследени класове от тип 'Profile'
builder.Services.AddAutoMapper(typeof(Program).Assembly);
```

### 📄 Стъпка 2: Дефиниране на Mapping Profile
Създай нов файл `MappingProfile.cs` в твоята папка за мапинг:
```csharp
using AutoMapper;

public class UserEntity { public int Id { get; set; } public string Email { get; set; } = null!; public string PasswordHash { get; set; } = null!; }
public class UserDisplayDto { public int Id { get; set; } public string DisplayEmail { get; set; } = null!; }

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Конфигурираме превода от сурово Entity към леко DTO за уеб екрана
        CreateMap<UserEntity, UserDisplayDto>()
            .ForMember(dest => dest.DisplayEmail, 
                       opt => opt.MapFrom(src => src.Email.ToLower().Trim()));
    }
}
```

### 🧱 Стъпка 3: Инжектиране и използване в Контролера
```csharp
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public class UsersController : Controller
{
    private readonly IMapper _mapper;

    // Вграденият IoC контейнер инжектира интерфейса IMapper автоматично!
    public UsersController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("/api/users/profile/{id}")]
    public IActionResult GetUserProfile()
    {
        var rawEntity = new UserEntity { Id = 1, Email = "  TestUser@SoftUni.BG ", PasswordHash = "secret_hash" };

        // Мапваме обекта с един чист и лек ред код!
        var userDto = _mapper.Map<UserDisplayDto>(rawEntity);

        return Ok(userDto);
    }
}
```

#### 🎯 Очакван изход (Expected Output в Postman):
```json
{
  "id": 1,
  "displayEmail": "testuser@softuni.bg"
}
```

---

## 2. ASP.NET CORE IDENTITY КОНФИГУРАЦИЯ: ОТ-ДО

### ⚙️ Стъпка 1: Свързване в Program.cs и DbContext
Твоят `ApplicationDbContext` задължително трябва да наследи **`IdentityDbContext`**:
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext // ➔ SQL: Генерира таблиците AspNetUsers, AspNetRoles
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
```

Регистрираш Identity услугите в `Program.cs` преди `builder.Build()` и активираш конвейера:
```csharp
// 1. Регистрация на услугите в IoC Контейнера
builder.Services.AddIdentity(options =>
{
    // Конфигуриране на паролни изисквания по SoftUni стандарт
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>(); // Свързва Identity със твоя SQL Server Context

var app = builder.Build();

app.UseRouting();
// 2. ЗАДЪЛЖИТЕЛЕН конвейерен ред: Първо Аутентификация, чак след това Авторизация!
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
```

### 🧱 Стъпка 2: Практически Мениджмънт в Контролера (Регистрация и Вход)
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class AccountRegisterDto { public string Email { get; set; } = null!; public string Password { get; set; } = null!; }

public class AuthController : Controller
{
    private readonly UserManager _userManager;
    private readonly SignInManager _signInManager;

    // Инжектираме вградените софтуерни мениджъри на Microsoft
    public AuthController(UserManager userManager, SignInManager signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("/api/auth/register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterDto dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        
        // Методът автоматично хешира паролата с PBKDF2 + Salt и записва в SQL Server
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("Потребителят е създаден и паролата е шифрирана сигурно!");
    }
}
```

#### 🎯 Очакван изход (Expected Output в Postman):
```text
Status: 200 OK
Потребителят е създаден и паролата е шифрирана сигурно!
```

---

## 3. СПРАВОЧНИК ЗА ЧЕСТО СРЕЩАНИ СРИВОВЕ (EXCEPTIONS) И РЕШЕНИЯ

### 🚨 Срив 1: Ненамерен софтуерен превод в AutoMapper
*   **Грешка на екрана при изпълнение:**
    ```text
    AutoMapperMappingException: Missing type map configuration or unsupported mapping.
    Mapping types: UserEntity -> UserDisplayDto
    ```
*   **Защо се случва:** Извикала си `_mapper.Map<UserDisplayDto>(entity)`, но си забравила да разпишеш реда `CreateMap<UserEntity, UserDisplayDto>();` вътре във твоя `MappingProfile` конструктор. AutoMapper няма рецепта за превод между тези два класа.
*   **Как да поправиш:** Отвори твоя `MappingProfile.cs` клас и добави изрично `CreateMap<Източник, Дестинация>();` ред.

### 🚨 Срив 2: Лош ред на Middleware конвейера (Identity Cookie Срив)
*   **Грешка на екрана:** Потребителят влиза успешно в профила си, но при следваща заявка към защитен с `[Authorize]` метод, Kestrel сървърът го изхвърля с черна грешка или статус **`401 Unauthorized`**, игнорирайки валидната му бисквитка.
*   **Защо се случва:** Разменила си местата на двата системни метода в `Program.cs`. Написала си `app.UseAuthorization();` **ПРЕДИ** `app.UseAuthentication();`. Системата проверява бизнес правата на потребителя, *преди* изобщо да е прочела уеб бисквитката от HTTP Headers пакета.
*   **Как да поправиш:** Подсигури, че в `Program.cs` методът `app.UseAuthentication();` стои твърдо над `app.UseAuthorization();`.

---
[⬅️ 01-08. Глобален Пищов](01-08-ASP-NET-Core-Fundamentals-CheatSheet.md) | [📂 01-00. ASP.NET Core Fundamentals (Кратък справочник)](01-00-ASP-NET-CORE-FUNDAMENTALS-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [01-10. Reflection и Attributes (Дълбок Архитектурен Справочник)](01-10-Reflection-And-Attributes.md)
---
© 2026 Dev-Library / Справочник: AutoMapper и Identity Пищов