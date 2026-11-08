⬅️ [01-03. Релационни диалекти](01-03-Relational-Dialects-MSSQL-Postgres-MySQL-SQLite.md) | [📂 Глава 01. Основи, Типове и Видове БД](01-00-SQL-INTRO-AND-CRUD.md) | [🏠 Начало (README)](README.md) | ➡️ [01-05. NoSQL: Redis Cache](01-05-NoSQL-InMemory-Cache-Redis.md)

---

# 🍃 01-04. NoSQL Бази данни: MongoDB и Apache Cassandra

**Цел:** Бърз старт с двете най-популярни NoSQL бази — превод на термините от SQL, основните CRUD команди и кратки C# примери.

## 📑 Съдържание на темата
* [1. Превод на термините: SQL ➔ MongoDB ➔ Cassandra](#-1-превод-на-термините-sql--mongodb--cassandra)
* [2. MongoDB — Основни команди и C# код](#-2-mongodb--основни-команди-и-c-код)
* [3. Apache Cassandra — Основни CQL команди](#-3-apache-cassandra--основни-cql-команди)
* [4. Кога избираме MongoDB и кога Cassandra?](#-4-кога-избираме-mongodb-и-кога-cassandra)

---

## 🔄 1. ПРЕВОД НА ТЕРМИНИТЕ: SQL ➔ MONGODB ➔ CASSANDRA

За да не се объркваш, просто преведи познатите ти SQL думи:

| SQL (MS SQL / Postgres) | MongoDB (Document NoSQL) | Apache Cassandra (Wide-Column NoSQL) | Какво представлява? |
| :--- | :--- | :--- | :--- |
| **Database** | **Database** | **Keyspace** | Главният контейнер (базата данни). |
| **Table** | **Collection** | **Table** | Група от еднотипни данни (напр. `users`, `orders`). |
| **Row (Ред)** | **Document (JSON / BSON)** | **Row** | Един конкретен запис (напр. един потребител). |
| **Column (Колона)** | **Field (Поле в JSON)** | **Column** | Отделно свойство (напр. `name`, `price`). |
| **Primary Key (`Id`)** | **`_id` (ObjectId)** | **Partition Key + Clustering Key** | Уникалният идентификатор на записа. |
| **`JOIN` (Връзка)** | **Embedding (Вграден JSON)** | **Денормализация** (дублиране на данни) | В NoSQL избягваме `JOIN` — държим данните заедно. |

---

## 🍃 2. MONGODB — ОСНОВНИ КОМАНДИ И C# КОД

В MongoDB няма нужда първо да правиш `CREATE TABLE`. Колекцията се създава автоматично в момента, в който запишеш първия JSON документ.

### 🔹 А. Основни команди в конзолата (MongoDB Shell)
```javascript
// ============================================================
// MONGODB CRUD (СРАВНЕНО С SQL МИСЛЕНЕТО)
// ============================================================

// 1. CREATE (Вместо INSERT INTO Users ...)
db.users.insertOne({
    name: "Елена Петрова",
    age: 26,
    city: "София",
    skills: ["C#", "SQL", "MongoDB"] // Може да пази директно масиви!
});

// 2. READ (Вместо SELECT * FROM Users WHERE age >= 18 ORDER BY name)
db.users.find(
    { age: { \(gte: 18 }, city: "София" }, // WHERE (\)gte = по-голямо или равно)
    { name: 1, skills: 1, _id: 0 }        // SELECT само тези полета (1 = покажи, 0 = скрий)
).sort({ name: 1 }).limit(10);            // ORDER BY ASC + TOP 10

// 3. UPDATE (Вместо UPDATE Users SET age = 27 WHERE name = 'Елена Петрова')
db.users.updateOne(
    { name: "Елена Петрова" },            // WHERE
    { \(set: { age: 27 } }                 // SET (задължително ползвай \)set!)
);

// 4. DELETE (Вместо DELETE FROM Users WHERE name = 'Елена Петрова')
db.users.deleteOne({ name: "Елена Петрова" });
```

### 🔹 Б. Най-важните оператори за търсене в MongoDB
* `$eq` (`=`), `$ne` (`!=`), `$gt` (`>`), `$gte` (`>=`), `$lt` (`<`), `$lte` (`<=`)
* `$in` (като SQL `IN (...)`), `$or` (като SQL `OR`)

### 🔹 В. Бърз C# пример (`NuGet: MongoDB.Driver`)
```csharp
// ============================================================
// C# ВРЪЗКА И CRUD С MONGODB
// ============================================================
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class UserDoc
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

// Свързване и работа с колекцията:
var client = new MongoClient("mongodb://localhost:27017");
var db = client.GetDatabase("ShopDb");
var users = db.GetCollection<UserDoc>("users");

// 1. Добавяне (Insert)
await users.InsertOneAsync(new UserDoc { Name = "Иван", Age = 25 });

// 2. Четене (Select с филтър)
var adults = await users.Find(u => u.Age >= 18).ToListAsync();

// 3. Промяна (Update)
var update = Builders<UserDoc>.Update.Set(u => u.Age, 26);
await users.UpdateOneAsync(u => u.Name == "Иван", update);

// 4. Изтриване (Delete)
await users.DeleteOneAsync(u => u.Name == "Иван");
```

---

## 👁️ 3. APACHE CASSANDRA — ОСНОВНИ CQL КОМАНДИ

Cassandra е направена за **огромни потоци от данни** (милиони записи в секунда — чатове, IoT сензори, логове на Netflix/Discord). Нейният език **CQL (Cassandra Query Language)** изглежда почти като SQL, но **няма `JOIN` и няма `GROUP BY`**.

```sql
-- ============================================================
-- CASSANDRA (CQL) — МНОГО БЛИЗЪК ДО SQL СИНТАКСИС
-- ============================================================

-- 1. Създаване на база (Keyspace)
CREATE KEYSPACE chat_app
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 3};

-- 2. Създаване на таблица (Partition Key определя на кой сървър отива редът!)
CREATE TABLE chat_app.messages (
    room_id UUID,
    sent_at TIMESTAMP,
    sender_name TEXT,
    message_text TEXT,
    PRIMARY KEY (room_id, sent_at) -- room_id = къде се пази; sent_at = как се сортира
) WITH CLUSTERING ORDER BY (sent_at DESC);

-- 3. Запис (INSERT) - в Cassandra се пише светкавично бързо
INSERT INTO chat_app.messages (room_id, sent_at, sender_name, message_text)
VALUES (uuid(), toTimestamp(now()), 'Мария', 'Здравейте в чата!');

-- 4. Четене (SELECT) - задължително филтрираш по Partition Key (room_id)!
SELECT sender_name, message_text, sent_at
FROM chat_app.messages
WHERE room_id = 123e4567-e89b-12d3-a456-426614174000
LIMIT 20;
```

---

## 🎯 4. КОГА ИЗБИРАМЕ MONGODB И КОГА CASSANDRA? (И КАПАНИ)

| Критерий | MongoDB | Apache Cassandra |
| :--- | :--- | :--- |
| **Основна сила** | Гъвкави JSON документи и богати заявки/филтри. | Невероятна скорост при **писане (`INSERT`)** върху десетки сървъри. |
| **Типични приложения** | Онлайн магазини (каталози), CMS, профили, мобилни бекенди. | Чат системи (Discord), истории на гледания (Netflix), GPS/IoT сензори. |
| **Главен капан** | В `updateOne` ако забравиш **`$set`**, MongoDB **презаписва целия документ** и изтрива останалите му полета! | Не можеш да правиш `WHERE` по произволна колона, ако тя не е част от `PRIMARY KEY`. |

---
⬅️ [01-03. Релационни диалекти](01-03-Relational-Dialects-MSSQL-Postgres-MySQL-SQLite.md) |[📂 Глава 01. Основи, Типове и Видове БД](01-00-SQL-INTRO-AND-CRUD.md) | [🏠 Начало (README)](README.md) | ➡️ [01-05. NoSQL: Redis Cache](01-05-NoSQL-InMemory-Cache-Redis.md)

© 2026 Dev-Library / Бази данни: MongoDB и Cassandra
