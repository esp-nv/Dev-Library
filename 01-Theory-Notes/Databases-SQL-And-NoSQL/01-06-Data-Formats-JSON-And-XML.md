⬅️ [01-05. Redis и Cloud NoSQL](01-05-NoSQL-InMemory-Cache-Redis.md) | 🏠 [01-00. Начало (Основи и CRUD)](01-00-SQL-INTRO-AND-CRUD.md) | ➡️ [01-07. Съседи на БД (RabbitMQ, Kafka, GraphQL, AD, SAP)](01-07-Architecture-Neighbors-RabbitMQ-Kafka-GraphQL-Enterprise.md)

---

# 📦 01-06. Формати за данни: JSON и XML (в SQL и C#)

**Цел:** Как изглеждат JSON и XML, кога се използва всеки от тях, как се пазят и търсят директно в SQL Server и PostgreSQL, и как ги четем/пишем в C#.

## 📑 Съдържание на темата
* [1. JSON срещу XML — Сравнение и Синтаксис](#-1-json-срещу-xml--сравнение-и-синтаксис)
* [2. JSON в SQL Server (OPENJSON / JSON_VALUE) и PostgreSQL (JSONB)](#-2-json-в-sql-server-openjson--json_value-и-postgresql-jsonb)
* [3. XML в SQL Server (XQuery основи)](#-3-xml-в-sql-server-xquery-основи)
* [4. Работа с JSON и XML в C# (System.Text.Json)](#-4-работа-с-json-и-xml-в-c-systemtextjson)

---

## ⚖️ 1. JSON СРЕЩУ XML — СРАВНЕНИЕ И СИНТАКСИС

**JSON** и **XML** не са бази данни — те са текстови формати за пренос и съхранение на структурирани данни.

| Критерий | JSON (JavaScript Object Notation) | XML (eXtensible Markup Language) |
| :--- | :--- | :--- |
| **Синтаксис** | Двойки `"ключ": стойност` в `{ }` и `[ ]`. | Отварящи и затварящи тагове `<Tag>стойност</Tag>`. |
| **Размер и скорост** | Много компактен, по-малко трафик, парсва се светкавично. | По-обемен заради повтарящите се тагове. |
| **Типове данни** | Поддържа числа, низове, булеви, `null`, масиви и обекти. | Всичко е текст (изисква XSD схема за валидация на типове). |
| **Къде се използва?** | **95% от модерните REST API-та**, уеб приложения, MongoDB, Redis. | **Банки, държавни институции, фактури (UBL), SAP, SOAP услуги**. |

### 🔹 Един и същ обект в двата формата:

```json
// ==================== JSON ФОРМАТ ====================
{
  "orderId": 105,
  "customer": "Елена Петрова",
  "isPaid": true,
  "items": ["Клавиатура", "Мишка"]
}
```

```xml
<!-- ==================== XML ФОРМАТ ==================== -->
<Order id="105">
  <Customer>Елена Петрова</Customer>
  <IsPaid>true</IsPaid>
  <Items>
    <Item>Клавиатура</Item>
    <Item>Мишка</Item>
  </Items>
</Order>
```

---

## 🗄️ 2. JSON В SQL SERVER И POSTGRESQL

И двете бази позволяват да пазиш JSON в колона и да търсиш вътре в полетата му с SQL.

### 🔹 А. MS SQL Server (`JSON_VALUE` и `OPENJSON`)
В SQL Server JSON се пази в стандартна колона `NVARCHAR(MAX)` с проверка `ISJSON()`.

```sql
-- ============================================================
-- JSON В MS SQL SERVER
-- ============================================================
CREATE TABLE ProductLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Payload NVARCHAR(MAX) NOT NULL CHECK (ISJSON(Payload) = 1)
);

INSERT INTO ProductLogs (Payload)
VALUES (N'{"brand": "Dell", "ramGb": 16, "inStock": true}');

-- Четене на конкретно поле от JSON текста:
SELECT 
    Id,
    JSON_VALUE(Payload, '\$.brand') AS Brand,
    CAST(JSON_VALUE(Payload, '\$.ramGb') AS INT) AS RamGb
FROM ProductLogs
WHERE JSON_VALUE(Payload, '\$.brand') = N'Dell';
```

### 🔹 Б. PostgreSQL (`JSONB` — Златният стандарт за JSON в SQL)
PostgreSQL има специален двоичен тип **`JSONB`**, който поддържа индекси и е изключително бърз.

```sql
-- ============================================================
-- JSONB В POSTGRESQL
-- ============================================================
CREATE TABLE product_logs (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    payload JSONB NOT NULL
);

INSERT INTO product_logs (payload)
VALUES ('{"brand": "Dell", "ramGb": 16, "inStock": true}');

-- Оператори: ->> връща текст; @> проверява дали съдържа JSON обект
SELECT id, payload->>'brand' AS brand
FROM product_logs
WHERE payload @> '{"brand": "Dell"}';
```

---

## 📄 3. XML В SQL SERVER (ОСНОВИ)

В MS SQL Server има вграден тип данни **`XML`**, който се използва при импорт на фактури, държавни отчети или стари системи.

```sql
-- ============================================================
-- XML В MS SQL SERVER
-- ============================================================
DECLARE @XmlData XML = N'
<Order id="105">
  <Customer>Елена Петрова</Customer>
  <Total>249.90</Total>
</Order>';

-- Изваждане на стойност чрез .value()
SELECT 
    @XmlData.value('(/Order/@id)[1]', 'INT') AS OrderId,
    @XmlData.value('(/Order/Customer)[1]', 'NVARCHAR(100)') AS Customer,
    @XmlData.value('(/Order/Total)[1]', 'DECIMAL(18,2)') AS Total;
```

---

## 💻 4. РАБОТА С JSON И XML В C# (`System.Text.Json`)

В модерен C# (.NET 8+) стандартната библиотека за JSON е **`System.Text.Json`** (вградена, много бърза и не изисква външни пакети).

```csharp
// ============================================================
// C# СЕРИАЛИЗАЦИЯ И ДЕСЕРИАЛИЗАЦИЯ (JSON)
// ============================================================
using System.Text.Json;

public record OrderDto(int OrderId, string Customer, bool IsPaid);

var order = new OrderDto(105, "Елена Петрова", true);

// 1. Обект ➔ JSON низ (Serialization - за изпращане по API или запис в Redis/SQL)
string json = JsonSerializer.Serialize(order);
// Резултат: {"OrderId":105,"Customer":"Елена Петрова","IsPaid":true}

// 2. JSON низ ➔ C# Обект (Deserialization - при получаване от API или база)
OrderDto? parsedOrder = JsonSerializer.Deserialize<OrderDto>(json);
```

---

## 🚨 5. КРИТИЧНИ КАПАНИ

1. **Пазене на всичко в JSON колона в SQL:** Ако 90% от заявките ти търсят и сортират по дадено поле (напр. `Email` или `Price`), направи го нормална SQL колона с индекс, а в `JSON` / `JSONB` слагай само редки и променливи характеристики.
2. **Case-Sensitivity при `JsonSerializer.Deserialize` в C#:** По подразбиране `System.Text.Json` прави разлика между `"orderId"` и `"OrderId"`. Когато четеш JSON от външни API-та (които ползват `camelCase`), винаги подавай `new JsonSerializerOptions { PropertyNameCaseInsensitive = true }`.

---
⬅️ [01-05. Redis и Cloud NoSQL](01-05-NoSQL-InMemory-Cache-Redis.md) | 🏠 [01-00. Начало (Основи и CRUD)](01-00-SQL-INTRO-AND-CRUD.md) | ➡️ [01-07. Съседи на БД (RabbitMQ, Kafka, GraphQL, AD, SAP)](01-07-Architecture-Neighbors-RabbitMQ-Kafka-GraphQL-Enterprise.md)

© 2026 Dev-Library / Бази данни: Формати JSON и XML
