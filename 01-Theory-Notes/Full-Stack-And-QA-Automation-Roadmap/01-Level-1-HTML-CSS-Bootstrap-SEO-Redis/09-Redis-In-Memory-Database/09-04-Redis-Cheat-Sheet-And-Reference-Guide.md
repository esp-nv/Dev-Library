[⬅️ 09-03. Инженерен Интервю Справочник](09-03-Advanced-Redis-Traps-And-Snippets.md) | 📂 Папка: 09-Redis-In-Memory-Database | [🏠 Начало](../../README.md)

---

# 09-04. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 09:** Суров Redis | **Цел:** Глобална, бърза синтактична справка за конзолни Redis CLI команди, .NET абстракции, Distributed Cache правила и готов за производство асинхронен кеширащ C# компонент.

## 🧭 Съдържание на темата
* [1. REDIS CLI REFERENCE GUIDE: Нисконивови конзолни команди](#1-redis-cli-reference-guide-нисконивови-конзолни-команди)
* [2. .NET CACHING OPTIONS CHEAT SHEET (Времеви настройки)](#2-net-caching-options-cheat-sheet-времеви-настройки)
* [3. ГОТОВ UI & BACKEND SNIPPET: "Професионален Thread-Safe Кеш Мениджър"](#3-готов-ui--backend-snippet-професионален-thread-safe-кеш-мениджър)

---

## 1. REDIS CLI REFERENCE GUIDE: Нисконивови конзолни команди

При директен мрежов одит през терминала (Redis CLI), софтуерният инженер управлява Key-Value паметта чрез следните сурови атомарни команди:

### 🗛 А. Операции над Низове (Strings):
*   `SET user:101 "Viktor"` ➔ Записва твърдо стойността под дефинирания ключ в RAM паметта.
*   `GET user:101` ➔ Извлича низа. Връща `(nil)`, ако ключът не съществува.
*   `EXISTS user:101` ➔ Връща `1`, ако ключът е зает в паметта, и `0`, ако е свободен.
*   `DEL user:101` ➔ Незабавно изтрива ключа и освобождава байтовете му в паметта.
*   `INCR counter:views` ➔ Атомарно инкрементира (увеличава с +1) числови низове без нишкови конфликти.

### 🗛 Б. Операции над Хешове и Жизнен цикъл (Hashes & TTL):
*   `HSET student:id01 name "Maria" age "22"` ➔ Генерира вътрешни полета и стойности в ключа.
*   `HGET student:id01 name` ➔ Извлича само специфичното поле без четене на целия обект.
*   `EXPIRE user:101 60` ➔ Налага твърд TTL брояч от точно 60 секунди на ключа насила.
*   `TTL user:101` ➔ Връща оставащите секунди живот на ключа. Връща `-2`, ако ключът вече е мъртъв.
*   `FLUSHALL` ➔ **Опасна команда!** Изтрива абсолютно всички ключове и бази данни от RAM паметта на мига.

---

## 2. .NET CACHING OPTIONS CHEAT SHEET (Времеви настройки)

Конфигуриране на еластичността на паметта в C# бекенд средите през `DistributedCacheEntryOptions`:

```csharp
var cacheOptions = new DistributedCacheEntryOptions()
    // 1. Абсолютно изтичане: Ключът умира твърдо след 1 час
    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
    
    // 2. Плаващо изтичане: Времето се рестартира с още 20 минути при всяко успешно четене
    .SetSlidingExpiration(TimeSpan.FromMinutes(20));
```

---

## 3. ГОТОВ UI & BACKEND SNIPPET: "ПРОФЕСИОНАЛЕН THREAD-SAFE КЕШ МЕНИДЖЪР"

Копирай този готов, високопроизводителен C# софтуерна обвивка (Wrapper). Той капсулира `IDistributedCache`, имплементира правилно **Cache-Aside патерна** и използва `SemaphoreSlim` нишкови предпазители за постигане на абсолютна защита против **Cache Stampede / Thundering Herd** аномалии под тежък индустриален товар:

```csharp
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;

public interface ICacheHubManager
{
    Task<T?> GetOrCacheAsync<T>(string key, Func<Task<T>> databaseQuery, TimeSpan? absoluteExpiration = null) where T : class;
}

public class RedisCacheHubManager : ICacheHubManager
{
    private readonly IDistributedCache _distributedCache;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Локален нишков катинар

    public RedisCacheHubManager(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public Task<T?> GetOrCacheAsync<T>(string key, Func<Task<T>> databaseQuery, TimeSpan? absoluteExpiration = null) where T : class
        => GetOrCacheInternalAsync(key, databaseQuery, absoluteExpiration);

    private async Task<T?> GetOrCacheInternalAsync<T>(string key, Func<Task<T>> databaseQuery, TimeSpan? absoluteExpiration) where T : class
    {
        // Step 1: Опит за прочитане на сурови байтове от Redis RAM
        byte[]? cachedBytes = await _distributedCache.GetAsync(key);
        if (cachedBytes != null)
        {
            string jsonString = Encoding.UTF8.GetString(cachedBytes);
            return JsonSerializer.Deserialize<T>(jsonString); // Cache Hit 🚀
        }

        // Step 2: Защита против паралелно претоварване на SQL Server (Thundering Herd)
        await _semaphore.WaitAsync();
        try
        {
            // Двойна проверка (Double-Check Lock Pattern)
            cachedBytes = await _distributedCache.GetAsync(key);
            if (cachedBytes != null)
            {
                string jsonString = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<T>(jsonString);
            }

            // Step 3: Cache Miss -> Изпълнение на тежката SQL заявка към диска
            T databaseResult = await databaseQuery();

            if (databaseResult != null)
            {
                // Сериализация и запис в Redis с контролиран TTL
                string jsonString = JsonSerializer.Serialize(databaseResult);
                byte[] bytesToCache = Encoding.UTF8.GetBytes(jsonString);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(15)
                };

                await _distributedCache.SetAsync(key, bytesToCache, options);
            }

            return databaseResult;
        }
        finally
        {
            _semaphore.Release(); // Отключване на катинара за следващите нишки
        }
    }
}
```

---
[⬅️ 09-03. Инженерен Интервю Справочник](09-03-Advanced-Redis-Traps-And-Snippets.md) | 📂 Папка: 09-Redis-In-Memory-Database | [🏠 Начало](../../README.md)
