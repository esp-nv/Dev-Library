[⬅️ 02-04. Напреднал Пищов](02-04-ASP-NET-Core-Advanced-CheatSheet.md) | 📂 Глава 02. ASP.NET Core Advanced | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-06. Глобална Софтуерна Сигурност](02-06-Global-Web-Security-XSS-SQL-Injection-And-CSRF.md)

---

# 02-05. Архитектурният патерн Repository и Generic Складове

**Цел:** Изучаване на шаблона Repository за декуплиране на достъпа до данни от бизнес логиката, изграждане на мащабируем Generic Repository (`EfRepository<T>`) над Entity Framework Core и оптимизиране на паметта през изключване на проследяването.

## 🧭 Съдържание на темата
* [1. Защо ни е нужен Repository Патерн? (Separation of Concerns)](#1-защо-ни-е-нужен-repository-патерн-separation-of-concerns)
* [2. Практическа реализация на Generic Repository в C#](#2-практическа-реализация-на-generic-repository-в-c)
* [3. Плюсове, минуси и архитектурни компромиси (Trade-offs)](#3-плюсове-минуси-и-архитектурни-компромиси-trade-offs)
* [4. Системни капани: Изтичане на DbContext и Сривове при проследяване](#4-системни-капани-изтичане-на-dbcontext-и-сривове-при-проследяване)

---

## 1. ЗАЩО НИ Е НУЖЕН REPOSITORY ПАТЕРН? (SEPARATION OF CONCERNS)

Въпреки че Entity Framework Core ни предоставя вградените класове `DbSet<T>`, които сами по себе си работят като складове, директното писане на тежки LINQ заявки (`_context.Products.Where(...).Include(...)`) вътре в бизнес Сервизите води до архитектурни проблеми:
*   **Силна обвързаност с провайдъра (Tight Coupling):** Бизнес слоевете ти стават зависими директно от ORM рамката (EF Core). Ако утре фирмата реши да премина към Dapper или NoSQL за определени таблици, ще трябва да пренапишеш стотици файлове с бизнес логика.
*   **Дублиране на LINQ заявки:** Едни и същи филтри за активни потребители или наличности на продукти започват да се копират и повтарят в различните компоненти на софтуера.

**Патернът Repository декуплира инфраструктурата.** Той поставя софтуерна стена (интерфейс) между бизнес логиката и физическата база данни. Сервизите комуникират само с методи като `.All()` или `.Add()`, без изобщо да ги интересува дали отдолу работи SQL Server, PostgreSQL или InMemory памет.

---

## 2. ПРАКТИЧЕСКА РЕАЛИЗАЦИЯ НА GENERIC REPOSITORY В C#

Вместо да пишем отделен склад за всяка една от петдесетте ни таблици, изграждаме един общ **Генеричен Склад (Generic Repository)**, който обслужва абсолютно всички Entity модели автоматично:

### 📄 1. Софтуерен код на Интерфейса и Имплементацията:
```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// А. ИНТЕРФЕЙСЪТ: Намира се в централния Application / СLayer слой
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> All(); // Връща нематириализирана IQueryable заявка за филтриране
    IQueryable<TEntity> AllAsNoTracking(); // Оптимизиран метод за бързо четене без паметен товар
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<int> SaveChangesAsync(); // Запечатва трансакцията в SQL
}

// Б. РЕАЛИЗАЦИЯТА: Намира се твърдо в Infrastructure / Data слоя
public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public EfRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>(); // Динамично улавяме таблицата за съответния тип
    }

    public IQueryable<TEntity> All() => _dbSet;

    // Спираме Change Tracker-а на EF Core за светкавично бързо четене на данни (ReadOnly)
    public IQueryable<TEntity> AllAsNoTracking() => _dbSet.AsNoTracking();

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

### ⚙️ Регистрация в Program.cs:
За да може IoC контейнерът да разпознава автоматично всяко повикване, регистрираме отворената генерична двойка като **`Scoped`** услуга:
```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>)); // source: CheatSheet
```

### 🧱 Използване чрез Dependency Injection в Бизнес Сервиза:
```csharp
public class ProductDto { public int Id { get; set; } public string Name { get; set; } = null!; }

public class ProductService
{
    private readonly IRepository<Product> _productRepo; // Чиста изолация, няма DbContext!

    public ProductService(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<ProductDto>> GetActiveProductsAsync()
    {
        // Използваме AllAsNoTracking за максимална скорост и филтрираме на ниво база
        return await _productRepo.AllAsNoTracking()
            .Where(p => p.IsActive == true)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
            .ToListAsync(); // Материализираме паметта в самия край по SoftUni стандарт
    }
}
```

#### 🎯 Очакван изход (Expected Output в Postman):
```json
[
  { "id": 101, "name": "Геймърска лаптоп раница" }
]
```

---

## 3. ПЛЮСОВЕ, МИНУСИ И АРХИТЕКТУРНИ КОМПРОМИСИ (TRADE-OFFS)

*   **Плюсове:** Пълна тестова изолация (Тестването с TDD/Mocking става елементарно, тъй като в тестовия проект инжектираш фалшив `MockRepository : IRepository<T>`, без изобщо да вдигаш реална база данни). Логовете и LINQ филтрите са капсулирани чисто на едно място.
*   **Търговски компромис (Trade-off):** Изграждането на допълнителен слой над EF Core (който самият по себе си е имплементация на Repository) се нарича в софтуерния инженеринг *Абстракция над Абстракцията*. Това изисква малко повече писане в началото на проекта, но се отплаща стократно при корпоративна поддръжка.

---

## 4. СИСТЕМНИ КАПАНИ: ИЗТИЧАНЕ НА DBCONTEXT И СРИВОВЕ ПРИ ПРОСЛЕДЯВАНЕ

### 🚨 Капан №1: Опит за материализация на заявката вътре в самия Repository клас
*   **Какво правиш:** За да улесниш писането, написваш метода `.All()` така, че да връща директно материализиран масив `IEnumerable` от базата данни:
    ```csharp
    // ❌ КРИТИЧЕН АРХИТЕКТУРЕН АНТИ-ПАТЕРН (Убива производителността директно!):
    public IEnumerable<TEntity> All() => _dbSet.ToList(); // Методът .ToList() материализира паметта твърде рано!
    ```
*   **Защо се случва (Капанът IEnumerable):** Както научихме в Глава 02 на EF Core модула, извикването на `.ToList()` изпраща незабавна заявка `SELECT *` към SQL Server. Ако след това твоят бизнес сервиз напише филтър `.Where(p => p.Id == 5)`, това филтриране няма да се случи на твърдия диск на базата, а C# процесорът ще бъде принуден да изтегли **абсолютно всички милиони редове по мрежата в RAM паметта** и да ги обхожда бавно ръчно, сривайки уеб сървъра Kestrel.
*   **Как да го избегнеш:** Методът `.All()` задължително трябва да връща **`IQueryable<TEntity>`**, за да може бизнес логиката да продължи да дописва SQL кода на диска и матеріализацията да настъпи чак в самия уеб контролер.

---
[⬅️ 02-04. Напреднал Пищов](02-04-ASP-NET-Core-Advanced-CheatSheet.md) | [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md) | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-06. Глобална Софтуерна Сигурност](02-06-Global-Web-Security-XSS-SQL-Injection-And-CSRF.md)

© 2026 Dev-Library / Архитектура: Патернът Repository
