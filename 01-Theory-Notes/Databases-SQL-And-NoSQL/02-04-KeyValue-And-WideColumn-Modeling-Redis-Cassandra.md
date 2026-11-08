⬅️ [02-03. Моделиране в Document БД (MongoDB)](02-03-Document-DB-Modeling-MongoDB.md) | [📂 Глава 02-00. Проектиране и Моделиране на Бази Данни (SQL и NoSQL)](02-00-DATA-MODELING-OVERVIEW.md) | [🏠 Начало (README)](README.md) | ➡️ [02-05. Моделиране в Graph, Time-Series и Vector БД](02-05-Graph-TimeSeries-Vector-Modeling.md)

---

# 02-04. Моделиране в Key-Value (Redis) и Wide-Column (Cassandra)

**Цел:** Как да структурираме данни в паметта на Redis (конвенция за ключове и избор между `String`, `Hash`, `List`, `Set`, `Sorted Set`) и как се проектират таблици в Cassandra по метода **Query-First** (`Partition Key` и `Clustering Key`).

## Съдържание на темата
* [1. Моделиране в Redis: Именуване на ключове и Структури](#1-моделиране-в-redis-именуване-на-ключове-и-структури)
* [2. Redis в C#: Практически пример с Hash и Sorted Set](#2-redis-в-c-практически-пример-с-hash-и-sorted-set)
* [3. Моделиране в Cassandra: Query-First дизайн и Ключове](#3-моделиране-в-cassandra-query-first-дизайн-и-ключове)
* [4. Чести грешки, Капани и Как да ги избегнем](#4-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. МОДЕЛИРАНЕ В REDIS: ИМЕНУВАНЕ НА КЛЮЧОВЕ И СТРУКТУРИ

В Redis няма таблици и колони — всички ключове живеят в едно общо пространство в RAM паметта. За да не настъпи хаос, използваме **двоеточие (`:`)** като разделител на нива:

`обект:идентификатор:подобект` ➔ например `user:105:cart` или `game:season1:leaderboard`

### Избор на правилната структура от данни в Redis:

| Структура в Redis | Аналог в C# | Кога се използва и реален пример? | Основни команди |
| :--- | :--- | :--- | :--- |
| **`String`** | `string` / `int` | Кеш на готов JSON обект, потребителска сесия, броячи на преглеждания. | `SET`, `GET`, `INCR`, `EXPIRE` |
| **`Hash`** | `Dictionary<string, string>` | Количка в онлайн магазин или потребителски профил (променяш 1 поле без да теглиш целия JSON). | `HSET`, `HGET`, `HGETALL`, `HINCRBY` |
| **`List`** | `LinkedList<string>` | Опашка за задачи (Queue), последни 10 прегледани продукта, хронология на съобщения. | `LPUSH`, `RPOP`, `LRANGE` |
| **`Set`** | `HashSet<string>` | Уникални тагове, онлайн потребители в момента, общи приятели между двама души. | `SADD`, `SISMEMBER`, `SINTER` |
| **`Sorted Set` (`ZSET`)** | `SortedDictionary` | **Класации в игри (Leaderboards)**, най-продавани продукти (всеки елемент има числов `Score`). | `ZADD`, `ZREVRANGE`, `ZRANK` |

```bash
# ============================================================
# ПРИМЕР В REDIS: КОЛИЧКА (HASH) И КЛАСАЦИЯ (SORTED SET)
# ============================================================

# 1. Пазарска количка на потребител 105 (продуктId -> бройка):
HSET cart:user:105 product:12 2
HSET cart:user:105 product:45 1
HINCRBY cart:user:105 product:12 1    # Увеличава бройката на продукт 12 от 2 на 3!
HGETALL cart:user:105                 # Връща цялата количка

# 2. Класация на играчи по точки (Sorted Set):
ZADD leaderboard:2026 1500 "Elena"
ZADD leaderboard:2026 2350 "Georgi"
ZADD leaderboard:2026 1900 "Maria"
ZREVRANGE leaderboard:2026 0 2 WITHSCORES # Връща Топ 3 играчи от най-много точки надолу!
```

---

## 2. REDIS В C#: ПРАКТИЧЕСКИ ПРИМЕР С HASH И SORTED SET

Когато използваш **`Hash`** в C#, не се налага да десериализираш целия JSON обект, за да смениш само количеството на един продукт в количката:

```csharp
// ============================================================
// C# РАБОТА С REDIS HASH И SORTED SET (StackExchange.Redis)
// ============================================================
using StackExchange.Redis;

public class ShoppingAndRankingService
{
    private readonly IDatabase _redis;

    public ShoppingAndRankingService(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    // 1. Добавяне/увеличаване на продукт в количката чрез Redis Hash
    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        string cartKey = \$"cart:user:{userId}";
        await _redis.HashIncrementAsync(cartKey, \$"product:{productId}", quantity);
        await _redis.KeyExpireAsync(cartKey, TimeSpan.FromDays(7)); // Количката се пази 7 дни
    }

    // 2. Обновяване на точки в класация чрез Sorted Set
    public async Task UpdatePlayerScoreAsync(string playerName, double points)
    {
        await _redis.SortedSetAddAsync("leaderboard:2026", playerName, points);
    }

    // 3. Взимане на Топ 10 играчи
    public async Task<SortedSetEntry[]> GetTop10PlayersAsync()
    {
        return await _redis.SortedSetRangeByRankWithScoresAsync(
            "leaderboard:2026", start: 0, stop: 9, order: Order.Descending);
    }
}
```

---

## 3. МОДЕЛИРАНЕ В CASSANDRA: QUERY-FIRST ДИЗАЙН И КЛЮЧОВЕ

В SQL първо проектираш таблиците и после мислиш какви `SELECT` заявки ще пишеш.  
В **Apache Cassandra** е точно обратното (**Query-First Design**): **първо пишеш въпросите (заявките), които приложението ще задава, и правиш по една отделна таблица за всяка заявка!**

### Как работи `PRIMARY KEY` в Cassandra?
Първичният ключ има две части: `PRIMARY KEY ((PartitionKey), ClusteringKey)`
1. **Partition Key (Ключ на дяла):** Определя **на кой физически сървър** в клъстера отива записът. Всички редове с един и същ Partition Key се пазят заедно.
2. **Clustering Key (Ключ за подредба):** Определя **в какъв ред са сортирани записите на диска** вътре в този сървър (за светкавично четене без допълнително `ORDER BY`).

```sql
-- ============================================================
-- QUERY-FIRST МОДЕЛИРАНЕ В CASSANDRA (CQL)
-- ============================================================
-- Заявка на приложението: "Дай ми последните 50 поръчки на конкретен потребител,
-- подредени от най-новата към най-старата."

CREATE TABLE orders_by_user (
    user_id UUID,               -- Partition Key: събира всички поръчки на клиента на едно място
    order_date TIMESTAMP,       -- Clustering Key: държи ги предварително сортирани на диска
    order_id UUID,
    total_amount DECIMAL,
    status TEXT,
    PRIMARY KEY ((user_id), order_date, order_id)
) WITH CLUSTERING ORDER BY (order_date DESC);

-- Тази заявка се изпълнява за 1-2 милисекунди, защото чете директно от диска без сортиране:
SELECT order_id, order_date, total_amount, status
FROM orders_by_user
WHERE user_id = 550e8400-e29b-41d4-a716-446655440000
LIMIT 50;
```

---

## 4. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Използване на командата `KEYS *` в Redis в продукция
* **Какво се случва:** Redis изпълнява командите в една нишка (Single-threaded). Ако имаш 5 милиона ключа и пуснеш `KEYS user:*`, Redis замръзва за няколко секунди и блокира целия уеб сайт!
* **Как да избегнеш:** В продукция никога не ползвай `KEYS *` — използвай **`SCAN`**, който обхожда ключовете на малки порции без да спира сървъра.

### Грешка 2: Записване на ключове в Redis без срок на годност (`TTL`)
* **Какво се случва:** Тъй като Redis държи всичко в RAM паметта, забравените кеш ключове и стари сесии запълват RAM-а на 100% и сървърът започва да хвърля грешки (*OOM command not allowed when used memory > 'maxmemory'*).
* **Как да избегнеш:** Винаги подавай `TimeSpan` (TTL) при запис на кеш или сесия, освен ако данните не са критични броячи.

### Грешка 3: Лош Partition Key в Cassandra („Hot Partition“)
* **Какво се случва:** Избираш колона с малко уникални стойности за Partition Key — например `country` (`"BG"`) или `status` (`"Active"`). Тогава 90% от всички данни се изсипват върху **един-единствен сървър** в клъстера и го сриват, докато останалите сървъри стоят празни.
* **Как да избегнеш:** Избирай Partition Key с висока уникалност (`user_id`, `device_id`, `room_id`) или комбиниран ключ `((city, date))`, за да разпределиш товара равномерно по всички сървъри.

### Грешка 4: Писане на `ALLOW FILTERING` в Cassandra
* **Какво се случва:** Опитваш се да направиш `WHERE status = 'Paid'` без `status` да е Partition Key. Cassandra отказва и ти предлага да добавиш `ALLOW FILTERING`. Ако го добавиш, тя сканира всички сървъри и милиарди редове, което забива системата.
* **Как да избегнеш:** Никога не ползвай `ALLOW FILTERING` в продукция — направи нова денормализирана таблица `orders_by_status`, чийто Partition Key е `status` (или `(status, order_date)`).

---
⬅️ [02-03. Моделиране в Document БД (MongoDB)](02-03-Document-DB-Modeling-MongoDB.md) | [📂 Глава 02-00. Проектиране и Моделиране на Бази Данни (SQL и NoSQL)](02-00-DATA-MODELING-OVERVIEW.md) | [🏠 Начало (README)](README.md) | ➡️ [02-05. Моделиране в Graph, Time-Series и Vector БД](02-05-Graph-TimeSeries-Vector-Modeling.md)

© 2026 Dev-Library / Бази данни: Моделиране в Redis и Cassandra
