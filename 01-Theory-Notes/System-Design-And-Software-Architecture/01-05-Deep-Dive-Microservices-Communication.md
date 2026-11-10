[⬅️ 01-04. Процес на проектиране: Алгоритъм за изграждане на системи от нулата](01-04-System-Design-Interview-Process.md) | [📂 Папка: System-Design-And-Software-Architecture](README.md) | [🏠 Главен Хъб на проекта](../README.md)| [➡️ 01-06. Стратегии за Шардинг](01-06-Advanced-Database-Scaling-Sharding-Strategies.md)

---

# 01-05. Дълбоко гмуркане: Комуникация между Микроуслуги (gRPC срещу RabbitMQ)

**Модул 01:** Структурни основи на системите | **Цел:** Дълбоко практическо овладяване на комуникационните протоколи в разпределени системи. Изграждане на свръхбързи бинарни gRPC канали и проектиране на неблокиращи асинхронни съобщения през RabbitMQ брокер, съчетано с C# кодове, изходи и диагностика на мрежови крашове.

## 🧭 Съдържание на темата
* [1. Архитектурна съпоставка: Синхронна срещу Асинхронна мрежа](#1-архитектурна-съпоставка-синхронна-срещу-асинхронна-мрежа)
* [2. gRPC Протокол под капака (Проектиране през Protocol Buffers)](#2-grpc-протокол-под-капака-проектиране-през-protocol-buffers)
* [3. Асинхронен Еvent-Driven модел с RabbitMQ (Publisher/Subscriber)](#3-асинхронен-еvent-driven-модел-с-rabbitmq-publishersubscriber)
* [4. Системни капани: Сривове при липсващи опашки и Версии на Прототипи](#4-системни-капани-спиране-на-работа-при-липсващи-опашки)

---

## 1. АРХИТЕКТУРНА СЪПОСТАВКА: СИНХРОННА СРЕЩУ АСИНХРОННА МРЕЖА

Когато разбием един монолит на микроуслуги, начинът по който те си говорят през мрежата, определя стабилността на цялата система:

*   **Синхронна комуникация (REST / gRPC):** Услуга А извиква директно Услуга Б по нейния IP/URL адрес и замръзва в чакане, докато отсрещната машина не върне отговор. Това създава **Силна времева обвързаност (Temporal Coupling)** — ако Услуга Б за забави или падне, Услуга А крашва веднага.
*   **Асинхронна комуникация (Message Brokers - RabbitMQ / Kafka):** Услуга А просто пуска пакет данни (Message / Event) в централен софтуерен пощенски клон, наречен **Message Broker**, и продължава работата си веднага, без да чака никого. Услуга Б чете от този пощенски клон тогава, когато е свободна. Ако Услуга Б падне, системата не спира — съобщенията просто се трупат сигурно в опашката, докато тя се рестартира.

---

## 2. gRPC ПРОТОКОЛ ПОД КАПАКА (ПРОЕКТИРАНЕ ПРЕЗ PROTOCOL BUFFERS)

**gRPC (Google Remote Procedure Call)** е свръхбърз протокол за директна синхронна връзка. Той работи изцяло над **HTTP/2** (използва Multiplexing) и заменя тежкия четим JSON текст с изключително леки, компресирани **Бинарни съобщения**, дефинирани чрез езика **Protocol Buffers (`.proto`)**.

### 📄 1. Спецификация на договора (inventory.proto):
```protobuf
syntax = "proto3";

option csharp_namespace = "StockMicroservice.Grpc";

package inventory;

// Дефинираме отдалечената софтуерна услуга
service InventoryGrpc {
  rpc CheckStock (StockRequest) returns (StockResponse);
}

// Структура на входящия бинарен пакет
message StockRequest {
  int32 productId = 1;
}

// Структура на изходящия бинарен отговор
message StockResponse {
  bool isAvailable = 1;
  int32 quantityInStock = 2;
}
```

### 📄 2. C# Клиентски код за извикване на gRPC в Контролера:
След като `.proto` файлът се компилира, .NET автоматично генерира C# базов клиент:

```csharp
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using StockMicroservice.Grpc;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/orders")]
public class OrderController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder()
    {
        // 1. Отваряме високопроизводителен бинарен HTTP/2 gRPC канал
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = new InventoryGrpc.InventoryGrpcClient(channel);

        // 2. Изпращаме строго-типизиран бинарен пакет
        var request = new StockRequest { ProductId = 105 };
        StockResponse response = await client.CheckStockAsync(request);

        if (!response.IsAvailable)
        {
            return BadRequest($"Стоката е изчерпана. Наличност: {response.QuantityInStock}");
        }

        return Ok("gRPC проверка премина успешно!");
    }
}
```

#### 🎯 Очакван изход (Expected Output в Postman):
```json
{
  "message": "gRPC проверка премина успешно!"
}
```

---

## 3. АСИНХРОНЕН EVENT-DRIVEN МОДЕЛ С RABBITMQ (PUBLISHER/SUBSCRIBER)

Когато една поръчка се създаде, вместо да викаме други системи насила, изстрелваме събитие в **RabbitMQ**.

*   **NuGet пакет за работа:** `RabbitMQ.Client`

### 📄 1. Софтуерен C# код за Публикуване на събитие (Publisher):
```csharp
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/v1/checkout")]
public class CheckoutController : ControllerBase
{
    [HttpPost("submit")]
    public IActionResult SubmitOrder()
    {
        var orderEvent = new { OrderId = 9412, UserEmail = "ivan@test.bg", Total = 250.00 };

        // 1. Изграждаме физическа софтуерна връзка към RabbitMQ брокера
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // 2. Декларираме сигурна опашка (Queue), в която ще се трупат съобщенията
        channel.QueueDeclare(queue: "orders_queue", durable: true, exclusive: false, autoDelete: false);

        // Сериализираме обекта и го превръщаме в суров байтов масив за пренос
        string messageJson = JsonSerializer.Serialize(orderEvent);
        byte[] body = Encoding.UTF8.GetBytes(messageJson);

        // 3. Изстрелваме пакета асинхронно по мрежата без блокиране на уеб нишката!
        channel.BasicPublish(exchange: "", routingKey: "orders_queue", basicProperties: null, body: body);

        return Ok("Събитието за нова поръчка е изпратено успешно в RabbitMQ опашката!");
    }
}
```

#### 🎯 Очакван изход (Expected Output в Postman):
```text
Status: 200 OK
Събитието за нова поръчка е изпратено успешно в RabbitMQ опашката!
```

---

## 4. СИСТЕМНИ КАПАНИ: СРИВАНЕ ПРИ ЛИПСВАЩИ ОПАШКИ

Асинхронната и бинарната комуникация носят скрити архитектурни капани, които умират в продукция.

### 🚨 Капан №1: Загуба на събития при грешно изпращане (The Ghost Message)
*   **Какво правиш:** Твоят Publisher изпраща съобщение към `routingKey: "order_queue"`. Твоят Consumer обаче слуша и е декларирал опашка с име `routingKey: "orders_queue"` (с буквата "s").
*   **Очакван лош изход:** В конзолата ти няма нито една софтуерна грешка. Кодът връща `200 OK`, но нито една съседна микроуслуга не реагира и не обработва поръчката. Данните изчезват.
*   **Защо се случва:** RabbitMQ работи като разпределител. Ако му изпратиш съобщение за опашка, която все още физически не съществува или е сгрешена по име, той безкритично я изхвърля на мига в дигиталната кофа, без да връща грешка към C# кода.
*   **Как да го избегнеш:** Винаги декларирайте имената на опашките като **`public const string`** константи в споделена обща библиотека (Shared Class Library), която се реферира от всички подпроекти едновременно, елиминирайки правописните грешки напълно.

### 🚨 Капан №2: gRPC Крах при липса на HTTP/2 съвместимост
*   **Какво правиш:** Стартираш gRPC тестов канал на Mac или стара Windows среда по некриптиран HTTP път (`http://localhost:5001`).
*   **Точен софтуерен краш (Exception Message) в конзолата:**
    ```text
    Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. HttpRequestException: An error occurred while sending the request. Http2ConnectionException: The HTTP/2 server sent invalid data.")
    ```
*   **Как да го поправиш:** По подразбиране .NET Core забранява некриптиран HTTP/2 трафик за gRPC. Наложи използването на защитен **HTTPS** път със SSL сертификати, или активирай некриптирания протоколен байпас в `Program.cs` на стартиращия клиент преди извикване на канала:
    ```csharp
    // Активира некриптиран HTTP/2 софтуерен байпас само за локални тестове:
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    ```

---
[⬅️ 01-04. Процес на проектиране: Алгоритъм за изграждане на системи от нулата](01-04-System-Design-Interview-Process.md) | [📂 Папка: System-Design-And-Software-Architecture](README.md) | [🏠 Главен Хъб на проекта](../README.md)| [➡️ 01-06. Стратегии за Шардинг](01-06-Advanced-Database-Scaling-Sharding-Strategies.md)

© 2026 Dev-Library / Системен Дизайн: Комуникационни Слоеве
