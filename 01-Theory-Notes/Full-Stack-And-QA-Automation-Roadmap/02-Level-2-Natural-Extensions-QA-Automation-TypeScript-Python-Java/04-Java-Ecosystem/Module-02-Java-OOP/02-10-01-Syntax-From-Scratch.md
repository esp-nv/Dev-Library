[⬅️ 02-10-00. Теория & Капани](02-10-00-Design-Patterns.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ Module 03: Spring Data (Hibernate & JPA)](../Module-03-Spring-Data/03-00-Intro.md)

---

# 02-10-01. Design Patterns in Java: Синтаксис от нулата & Cheat Sheet

**Цел:** Чисти, работещи имплементации от нулата на най-важните GoF шаблони за проектиране (`Builder`, `Factory Method`, `Adapter`, `Decorator`, `Strategy`, `Observer`) в Java.

## 🧭 Съдържание на темата
* [1. Builder Pattern (Стъпка по стъпка изграждане на сложен обект)](#1-builder-pattern-стъпка-по-стъпка-изграждане-на-сложен-обект)
* [2. Factory Method Pattern (Фабрика за създаване на обекти)](#2-factory-method-pattern-фабрика-за-създаване-на-обекти)
* [3. Adapter и Decorator Patterns (Структурни шаблони)](#3-adapter-и-decorator-patterns-структурни-шаблони)
* [4. Strategy и Observer Patterns (Поведенчески шаблони)](#4-strategy-и-observer-patterns-поведенчески-шаблони)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. BUILDER PATTERN (СТЪПКА ПО СТЪПКА ИЗГРАЖДАНЕ НА СЛОЖЕН ОБЕКТ)

Решава проблема с конструктори с 8+ параметъра (*Telescoping Constructor Anti-Pattern*):

```java
public class HttpRequestConfig {
    private final String url;         // Задължителен
    private final String method;      // Опционален (default "GET")
    private final int timeoutMs;      // Опционален (default 5000)

    private HttpRequestConfig(Builder builder) {
        this.url = builder.url;
        this.method = builder.method;
        this.timeoutMs = builder.timeoutMs;
    }

    public static class Builder {
        private final String url;
        private String method = "GET";
        private int timeoutMs = 5000;

        public Builder(String url) {
            this.url = url;
        }

        public Builder method(String method) {
            this.method = method;
            return this; // Връща this за верижно извикване (Fluent API)
        }

        public Builder timeoutMs(int timeoutMs) {
            this.timeoutMs = timeoutMs;
            return this;
        }

        public HttpRequestConfig build() {
            return new HttpRequestConfig(this);
        }
    }
}

// Използване:
HttpRequestConfig config = new HttpRequestConfig.Builder("https://softuni.bg")
        .method("POST")
        .timeoutMs(3000)
        .build();
```

---

## 2. FACTORY METHOD PATTERN (ФАБРИКА ЗА СЪЗДАВАНЕ НА ОБЕКТИ)

```java
interface Transport {
    void deliver();
}

class Truck implements Transport {
    @Override
    public void deliver() { System.out.println("Доставка по суша с камион."); }
}

class Ship implements Transport {
    @Override
    public void deliver() { System.out.println("Доставка по море с кораб."); }
}

class LogisticsFactory {
    public static Transport createTransport(String type) {
        return switch (type.toUpperCase()) {
            case "ROAD" -> new Truck();
            case "SEA" -> new Ship();
            default -> throw new IllegalArgumentException("Неизвестен транспорт: " + type);
        };
    }
}
```

---

## 3. ADAPTER И DECORATOR PATTERNS (СТРУКТУРНИ ШАБЛОНИ)

### А. Adapter Pattern (Преходник между несъвместими интерфейси)
```java
// Стар клас с несъвместим интерфейс
class LegacyXmlLogger {
    public void logXml(String xmlData) {
        System.out.println("<log>" + xmlData + "</log>");
    }
}

// Нов интерфейс в нашата система
interface JsonLogger {
    void logJson(String message);
}

// Adapter: обвива стария клас и го кара да изглежда като JsonLogger
class XmlToJsonLoggerAdapter implements JsonLogger {
    private final LegacyXmlLogger legacyLogger;

    public XmlToJsonLoggerAdapter(LegacyXmlLogger legacyLogger) {
        this.legacyLogger = legacyLogger;
    }

    @Override
    public void logJson(String message) {
        legacyLogger.logXml(message);
    }
}
```

### Б. Decorator Pattern (Динамично добавяне на функционалност чрез обвиване)
```java
interface DataSource {
    void writeData(String data);
}

class FileDataSource implements DataSource {
    @Override
    public void writeData(String data) {
        System.out.println("Запис във файл: " + data);
    }
}

class EncryptionDecorator implements DataSource {
    private final DataSource wrappee;

    public EncryptionDecorator(DataSource source) {
        this.wrappee = source;
    }

    @Override
    public void writeData(String data) {
        String encrypted = "ENC(" + data + ")";
        wrappee.writeData(encrypted); // Добавя криптиране и предава нататък!
    }
}

// Използване: new EncryptionDecorator(new FileDataSource()).writeData("Secret");
```

---

## 4. STRATEGY И OBSERVER PATTERNS (ПОВЕДЕНЧЕСКИ ШАБЛОНИ)

```java
import java.util.ArrayList;
import java.util.List;

// Observer Pattern (Абонати за събития)
interface EventListener {
    void update(String eventMessage);
}

class NewsAgency {
    private final List<EventListener> subscribers = new ArrayList<>();

    public void subscribe(EventListener listener) { subscribers.add(listener); }

    public void publishNews(String news) {
        for (EventListener sub : subscribers) {
            sub.update(news);
        }
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Шаблон | Категория | Проблемът, който решава |
| :--- | :--- | :--- |
| **Singleton** | Creational | Гарантира точно **една глобална инстанция** на клас |
| **Factory Method** | Creational | Създава обекти без да излага логиката на инстанциране (`new`) |
| **Builder** | Creational | Сглобява сложни обекти с много опционални параметри стъпка по стъпка |
| **Adapter** | Structural | Кара два класа с несъвместими интерфейси да работят заедно |
| **Decorator** | Structural | Добавя нови отговорности към обект динамично чрез обвиване |
| **Proxy** | Structural | Контролира достъпа до обект (кеширане, логове, транзакции в Spring) |
| **Strategy** | Behavioral | Позволява подмяна на алгоритъм по време на изпълнение |
| **Observer** | Behavioral | Уведомява автоматично множество абонати при промяна на състояние |

---
[⬅️ 02-10-00. Теория & Капани](02-10-00-Design-Patterns.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ Module 03: Spring Data (Hibernate & JPA)](../Module-03-Spring-Data/03-00-Intro.md)
