[⬅️ 09-00. Redis Архитектура](09-00-Redis-Architecture-And-Data-Types.md) | 📂 Папка: 09-Redis-In-Memory-Database | [🏠 Начало](../../README.md) | [➡️ 09-02. Стратегии за съхранение](09-02-TTL-And-Cache-Invalidation-Strategies.md)

---

# 09-01. Свързване с .NET среди: Работа през IDistributedCache и сурови драйвери

**Модул 09:** Суров Redis | **Лекционна тема 02:** Конфигуриране на разпределен кеш в ASP.NET Core, инжектиране на зависимости (Dependency Injection) и сериализация на данни.

## 🧭 Съдържание на темата
* [1. Пакетна интеграция и регистрация на Redis в ASP.NET Core](#1-пакетна-интеграция-и-регистрация-на-redis-в-aspnet-core)
* [2. Архитектура на абстракцията IDistributedCache](#2-архитектура-на-абстракцията-idistributedcache)
* [3. Процеси на сериализация и боравене с байтови масиви в паметта](#3-процеси-на-сериализация-и-боравене-с-байтове-масиви-в-паметта)

---

## 1. ПАКЕТНА ИНТЕГРАЦИЯ И РЕГИСТРАЦИЯ НА REDIS В ASP.NET CORE

За да свържем .NET уеб приложението нативно с Redis сървър, използваме официалния разширителен пакет на Microsoft за разпределен кеш. Инсталацията се задейства през NuGet или мениджъра в терминала:

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

След успешна инсталация, софтуерната връзка се закотвя централно в конфигурационния файл **`Program.cs`** на ASP.NET Core приложението, регистрирайки Redis услугите в IoC контейнера за Dependency Injection:

```csharp
// Регистриране на разпределения Redis кеш буфер
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Стандартен хардуерен порт на Redis
    options.InstanceName = "DevLibraryCache_"; // Префикс на ключовете в паметта за изолация
});
```

---

## 2. АРХИТЕКТУРА НА АБСТРАКЦИЯТА IDISTRIBUTEDCACHE

Вместо да пишем специфични за драйвера команди, Microsoft ни предоставя интерфейса **`IDistributedCache`**. Той абстрахира NoSQL комуникацията и ни дава четири native асинхронни софтуерни метода за боравене с ключове в паметта:
*   **`GetAsync(string key)`**: Извлича стойността по ключ като суров байтов масив (`byte[]`). Връща `null`, ако ключът липсва в RAM паметта.
*   **`SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)`**: Записва твърдо байтовия масив в Redis, придружен от изрични параметри за време на живот.
*   **`RefreshAsync(string key)`**: Опреснява времето на живот на ключа в паметта, без да променя неговата стойност.
*   **`RemoveAsync(string key)`**: Изтрива незабавно ключа и байтовете му от оперативната памет на Redis.

---

## 3. ПРОЦЕСИ НА СЕРИАЛИЗАЦИЯ И БОРАВЕНЕ С БАЙТОВИ МАСИВИ В ПАМЕТТА

Тъй като `IDistributedCache` работи на ниско ниво единствено със сурови байтови потоци (`byte[]`), пренасянето на комплексни C# обекти (класове, списъци, DTO модели) изисква задължителен процес на **Сериализация**:

1.  **Запис (C# Обект ➔ Redis RAM):** Бекендът първо сериализира C# обекта в текстов JSON низ посредством `JsonSerializer.Serialize()`. След това този низ се кодира в байтов масив през UTF-8 енкодинг (`Encoding.UTF-8.GetBytes(json)`) и се изпраща към Redis.
2.  **Четене (Redis RAM ➔ C# Обект):** Процесът е огледален — суровите байтове се извличат от Redis паметта, декодират се обратно в текстов JSON стринг (`Encoding.UTF-8.GetString(bytes)`) и се десериализират до легитимен C# обект чрез `JsonSerializer.Deserialize<T>()`.

---
[⬅️ 09-00. Redis Архитектура](09-00-Redis-Architecture-And-Data-Types.md) | 📂 Папка: 09-Redis-In-Memory-Database | [🏠 Начало](../../README.md) | [➡️ 09-02. Стратегии за съхранение](09-02-TTL-And-Cache-Invalidation-Strategies.md)
