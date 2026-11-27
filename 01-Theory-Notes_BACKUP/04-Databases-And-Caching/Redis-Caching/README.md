[⬅️ Назад към 04. Бази Данни и Кеширане](../README.md) | [📚 Инженерна Библиотека (01-Theory-Notes)](../../README.md) | [🏠 Начало](../../../README.md) | [▶️ Започни от 03-02-00 Redis Архитектура](03-02-00-Redis-Caching-Architecture.md)

---

# ⚡ Кеширане в Паметта с Redis (`Redis-Caching`)

Добре дошли в подмодула за **Redis (Remote Dictionary Server)** в **Dev-Library**. Този сектор разглежда архитектурата на високоскоростното съхранение на данни директно в RAM паметта (`< 0.5ms` латентност), ключовите структури от данни (**Strings, Hashes, Lists, Sets, Sorted Sets**), стратегиите за кеширане (**Cache-Aside**, Write-Through), управлението на живота на ключовете (**TTL**) и интеграцията с **C# / .NET** (`StackExchange.Redis` и `IDistributedCache`).

## 🧭 Съдържание на модула

* **01. Redis In-Memory Архитектура & Стратегии**
  * 📄 [03-02-00. Теория: RAM срещу Disk, Cache-Aside модел, Eviction политики & Капани в продукция (`KEYS *`)](03-02-00-Redis-Caching-Architecture.md)
  * 🛠️ [03-02-01. Практика от Нулата: `redis-cli` команди, C# / .NET интеграция & Cheat Sheet](03-02-01-Redis-Syntax-From-Scratch.md)

---

[⬅️ Назад към 04. Бази Данни и Кеширане](../README.md) | [📚 Инженерна Библиотека (01-Theory-Notes)](../../README.md) | [🏠 Начало](../../../README.md) | [▶️ Започни от 03-02-00 Redis Архитектура](03-02-00-Redis-Caching-Architecture.md)

© 2026 Dev-Library / Кеширане в Паметта с Redis
