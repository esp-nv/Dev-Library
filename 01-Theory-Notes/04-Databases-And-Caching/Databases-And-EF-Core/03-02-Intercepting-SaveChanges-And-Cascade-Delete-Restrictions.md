[⬅️ 03-01. Глобални филтри и Soft Delete](03-01-Global-Query-Filters-And-Soft-Delete.md) | [📂 Глава 03. Напреднали защити в DbContext, Глобални филтри и Soft Delete](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md) 

---

# 03-02. Прихващане на SaveChanges и ChangeTracker сканиране

**Цел:** Пренаписване (overriding) на методите `SaveChanges` и `SaveChangesAsync` с цел автоматизирано сканиране на транзитната памет (`ChangeTracker`), прихващане на състоянията на обектите и блокиране на невалидни софтуерни промени.

## Съдържание на темата
* [1. Жизнен цикъл на SaveChanges (Theme & System Protection)](#1-жизнен-цикъл-на-savechanges-theme--system-protection)
* [2. Готов софтуерен патерн: Имплементиране в ApplicationDbContext](#2-готов-софтуерен-патерн-имплементиране-в-applicationdbcontext)
* [3. Наръчник за модификация и чести капани](#3-наръчник-за-модификация-и-чести-капани)

---

## 1. ЖИЗНЕН ЦИКЪЛ НА SAVECHANGES (THEME & SYSTEM PROTECTION)

Във финансовите, административните и Enterprise системи съществуват определени записи, наречени **Системни котви (System Anchors)** — например базовата Bootstrap тема №1 на сайта, системната сметка по подразбиране или главният администраторски профил. Ако потребител или администратор промени или изтрие тези записи случайно, дизайнът или бизнес логиката на цялото приложение ще се срине фатално.

За да подсигурим софтуерна защита от най-високо ниво, ние пренаписваме фабричния метод `SaveChanges()` на Entity Framework Core. Преди каквито и да е байтове да бъдат изпратени физически по мрежата към SQL Server, транзитната памет се сканира изцяло:

*   **`ChangeTracker`**: Вътрешен инструмент на EF Core, който пази списък с абсолютно всички обекти, които в момента са били заредени, променени (`Modified`) или подготвени за триене (`Deleted`) в кода.
*   **Прихващане (Interception)**: Програмата обхожда тези променени обекти. Ако засече опит за промяна на системен запис (например обект от тип `GlobalTheme` с уникален идентификатор `Id == 1`), C# веднага спира операцията на място и хвърля `InvalidOperationException`. Базата данни на диска остава напълно защитена и непроменена.

---
## 2. ГОТОВ СОФТУЕРЕН ПАТЕРН: ИМПЛЕМЕНТИРАНЕ В APPLICATIONDBCONTEXT

Ето как се пренаписват синхронният и асинхронният метод в твоя команден център, за да прихващат автоматично всяка заявка за запис:

```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ApplicationDbContext : DbContext
{
    public DbSet<GlobalTheme> GlobalThemes { get; set; } = null!;
    public DbSet<AssetTransaction> AssetTransactions { get; set; } = null!;
    public DbSet<AssetAttributeValue> AssetAttributeValues { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // 1. Пренаписване на синхронния SaveChanges
    public override int SaveChanges()
    {
        EnforceGlobalThemeProtection();
        return base.SaveChanges();
    }

    // 2. Пренаписване на асинхронния SaveChangesAsync (Задължителен за уеб приложения)
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnforceGlobalThemeProtection();
        return base.SaveChangesAsync(cancellationToken);
    }

    // 3. Частен защитен механизъм за сканиране на паметта
    private void EnforceGlobalThemeProtection()
    {
        // Сканираме ChangeTracker за записи от тип GlobalTheme, които са променени или подготвени за триене
        var violatedThemes = ChangeTracker.Entries<GlobalTheme>()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (var theme in violatedThemes)
        {
            // ЖЕЛЯЗНО ПРАВИЛО: Ако ID-то е 1, това е фабричната котва и промяната е абсолютно забранена
            if (theme.Id == 1)
            {
                throw new InvalidOperationException("Фатална грешка: Промяната или изтриването на фабричната Системна Тема №1 е абсолютно забранена!");
            }
        }
    }
}
```

---

## 3. НАРЪЧНИК ЗА МОДИФИКАЦИЯ И ЧЕСТИ КАПАНИ

Тъй като проектът ти ще расте, ето как да управляваш и допълваш този защитен механизъм без риск да счупиш архитектурата:

### ➕ Ситуация 1: Добавяне на нов критичен запис за защита
Ако утре създадеш таблица `SystemSettings` и искаш да забраниш изтриването на конфигурационния запис с `Id == 100`, просто разшири метода `EnforceGlobalThemeProtection()` (или го преименувай на универсален `EnforceGlobalSystemProtection()`) със следното допълнително сканиране:
```csharp
var violatedSettings = ChangeTracker.Entries<SystemSettings>()
    .Where(e => e.State == EntityState.Deleted && e.Entity.Id == 100);

if (violatedSettings.Any())
{
    throw new InvalidOperationException("Грешка: Системният конфигурационен запис №100 е защитен от изтриване!");
}
```

### ❌ Учебни капани: Къде може да сгрешим?
*   **Капан №1 (Пропускане на асинхронния метод):** Пренаписваш само синхронния `SaveChanges()`, но уеб контролерите в ASP.NET Core масово извикват асинхронния `await context.SaveChangesAsync();`. Тъй като защитата не е инжектирана в асинхронния клон, потребителите ще успеят да изтрият защитените данни без софтуерът да ги спре.
    *   *Решение:* Винаги пренаписвай **и двата метода** едновременно в твоя `DbContext`!
*   **Капан №2 (Проблемът с производителността при масови обхождания):** Изпълняваш тежки LINQ заявки или външни повиквания към базата данни вътре в самия метод `EnforceGlobalThemeProtection()`. Тъй като този метод се задейства при всяко едно записване на ред, софтуерът ще забави работата си драстично.
    *   *Решение:* Четем изцяло и само транзитните записи, които вече се намират в паметта на `ChangeTracker.Entries<T>()`, без да правим нови заявки по мрежата.

---
[⬅️ 03-01. Глобални филтри и Soft Delete](03-01-Global-Query-Filters-And-Soft-Delete.md) | [📂 Глава 03. Напреднали защити в DbContext, Глобални филтри и Soft Delete](03-00-ADVANCED-DBCONTEXT-PROTECTIONS-INTRO.md) 

© 2026 Dev-Library / DbContext Защити: Прихващане на SaveChanges
