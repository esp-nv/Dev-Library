[⬅️ 03-06. Java Auto Mapping Objects](03-06-00-Auto-Mapping-DTOs.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-07-01. Syntax From Scratch](03-07-01-Syntax-From-Scratch.md)

---

# 03-07-00. JSON and XML Processing (Jackson & Gson — Теория, Безкрайна рекурсия & Капани)

**Модул 03:** Spring Data | **Лекционна тема 07:** Сериализация и десериализация на JSON и XML данни в Java чрез **Jackson** (вграденият стандарт в Spring Boot) и **Google Gson**, ключови анотации (`@JsonProperty`, `@JsonIgnore`, `@JsonFormat`, `@JsonManagedReference` / `@JsonBackReference`), работа с дати (`JavaTimeModule`), парсване на генерични колекции (`TypeReference`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Jackson `ObjectMapper` (Serialization & Deserialization Блок-схема)](#2-архитектура-на-jackson-objectmapper-serialization--deserialization-блок-схема)
* [3. Сравнителна карта в 4 езика (Java Jackson, C# System.Text.Json, Python json, JS JSON)](#3-сравнителна-карта-в-4-езика-java-jackson-c-systemtextjson-python-json-js-json)
* [4. Плюсове и минуси: Jackson срещу Google Gson](#4-плюсове-и-минуси-jackson-срещу-google-gson)
* [5. Кодов одит: Правилен срещу Грешен код (Безкрайна рекурсия при 1:N релации и Type Erasure)](#5-кодов-одит-правилен-срещу-грешен-код-безкрайна-рекурсия-при-1n-релации-и-type-erasure)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (Jackson) срещу C# 12+ (`System.Text.Json`)](#7-архитектурно-сравнение-java-21-jackson-срещу-c-12-systemtextjson)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Serialization = Опаковане на Java обект в кашон (JSON / XML низ)** за изпращане по мрежата.
*   **Deserialization = Разопаковане на JSON / XML низ обратно в жив Java обект.**
*   **Как мисли Jackson по подразбиране:**
    *   При **Serialization** (Java ➔ JSON): Jackson търси **`public` гетъри (`getUsername()`)** и записва ключ `"username"` (маха думата `get` и прави първата буква малка!).
    *   При **Deserialization** (JSON ➔ Java): Jackson търси **конструктор без параметри** (или `@JsonCreator` / конструктор на `record`) и извиква **`public` сетъри (`setUsername(...)`)**.

---

## 2. АРХИТЕКТУРЕН ПОТОК НА JACKSON `OBJECTMAPPER` (БЛОК-СХЕМА)

```text
Java Обект (UserDto)                                       JSON Текст
┌──────────────────────────────────┐                       ┌──────────────────────────────────────┐
│ id = 1L                          │  writeValueAsString() │ {                                    │
│ username = "elizabeth"           │ ────────────────────► │   "id": 1,                           │
│ password = "secret" (@JsonIgnore)│                       │   "username": "elizabeth"            │
└──────────────────────────────────┘                       │ }                                    │
                ▲                                          └──────────────────┬───────────────────┘
                └─────────────────────── readValue(json, UserDto.class) ──────┘
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA JACKSON, C# SYSTEM.TEXT.JSON, PYTHON JSON, JS JSON)

| Операция / Анотация | ☕ Java 21+ (`Jackson`) | 🟣 C# 12+ (`System.Text.Json`) | 🐍 Python 3.x (`json` / `Pydantic`) | 🟡 JavaScript (`JSON`) |
| :--- | :--- | :--- | :--- | :--- |
| **Обект ➔ JSON низ** | `mapper.writeValueAsString(obj)` | `JsonSerializer.Serialize(obj)` | `json.dumps(dict_obj)` | `JSON.stringify(obj)` |
| **JSON низ ➔ Обект** | `mapper.readValue(json, User.class)` | `JsonSerializer.Deserialize<User>(json)` | `User.model_validate_json(json)` | `JSON.parse(json)` |
| **Преименуване на JSON ключ** | `@JsonProperty("user_name")` | `[JsonPropertyName("user_name")]` | `Field(alias="user_name")` | Ръчен мапинг |
| **Скриване на поле в JSON** | `@JsonIgnore` | `[JsonIgnore]` | `Field(exclude=True)` | `toJSON()` метод |
| **Игнориране на непознати ключове** | `@JsonIgnoreProperties(ignoreUnknown = true)` | По подразбиране ги игнорира! | `model_config = ConfigDict(extra='ignore')` | По подразбиране ги игнорира |
| **Формат на дата/час** | `@JsonFormat(pattern = "yyyy-MM-dd")` | Custom `JsonConverter<DateTime>` | `datetime.isoformat()` | ISO 8601 низ |

---

## 4. ПЛЮСОВЕ И МИНУСИ: JACKSON СРЕЩУ GOOGLE GSON

### 🟢 Jackson (Стандарт в Spring Boot):
*   **Предимства:** Вграден директно в Spring Web (`spring-boot-starter-web`); поддържа едновременно **JSON, XML (`jackson-dataformat-xml`), YAML и CSV**; нативна поддръжка за Java `record` класове.
*   **Недостатъци:** По-сложна конфигурация при създаване на ръчен `ObjectMapper` извън Spring (изисква регистриране на `JavaTimeModule` за `LocalDate` / `LocalDateTime`).

### 🔵 Google Gson:
*   **Предимства:** Изключително лек, без външни зависимости; работи директно върху полетата чрез Reflection без нужда от гетъри/сетъри.
*   **Недостатъци:** Работи само с JSON (няма XML); не е нативен за Spring Boot MVC.
## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (БЕЗКРАЙНА РЕКУРСИЯ ПРИ 1:N РЕЛАЦИИ И TYPE ERASURE)

### ❌ ГРЕШЕН КОД (Двупосочна релация без `@JsonBackReference` -> `StackOverflowError` при JSON сериализация):
```java
class Author {
    public String name;
    // 🚨 ГРЕШКА 1: Author сериализира books -> всяка Book сериализира author -> Author сериализира books...
    // Безкраен цикъл -> Jackson гърми с JsonMappingException: Infinite recursion (StackOverflowError)!
    public List<Book> books;
}

class Book {
    public String title;
    public Author author;
}

// 🚨 ГРЕШКА 2 (Type Erasure капан):
// Опит за парсване на списък с List.class -> Jackson връща List<LinkedHashMap>, а НЕ List<UserDto>!
List<UserDto> users = objectMapper.readValue(jsonArray, List.class); // ClassCastException при цикъл!
```
*   **Как да се избегне:** При двупосочни релации използвай DTO модели без обратна връзка (най-доброто решение) или сложи **`@JsonManagedReference`** върху родителя (`Author.books`) и **`@JsonBackReference`** върху детето (`Book.author`). За парсване на списъци винаги ползвай **`new TypeReference<List<UserDto>>() {}`**.

### 🟢 ПРАВИЛЕН КОД (Предпазване от безкрайна рекурсия и типово безопасно парсване на колекции):
```java
import com.fasterxml.jackson.annotation.*;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.List;

class AuthorJson {
    public String name;

    @JsonManagedReference // Сериализира се нормално в JSON
    public List<BookJson> books;
}

class BookJson {
    public String title;

    @JsonBackReference // Пропуска се при сериализация в JSON, за да прекъсне цикъла!
    public AuthorJson author;
}

public class SafeJacksonParsing {
    public static List<UserDto> parseUsers(String jsonArray) throws Exception {
        ObjectMapper mapper = new ObjectMapper();
        // TypeReference запазва генеричния тип List<UserDto> въпреки Java Type Erasure!
        return mapper.readValue(jsonArray, new TypeReference<List<UserDto>>() {});
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Ако върху полето `password` в едно DTO сложим `@JsonIgnore`, потребителят няма да може нито да го изпрати при регистрация, нито да го получи в отговора. Как в Jackson разрешаваме полето `password` да се **чете от входящия JSON (Deserialization)**, но да **НЕ се изпраща обратно в изходящия JSON (Serialization)**?“*
*   **Уловката:** Начинаещите създават два отделни класа или пишат ръчни кодове за чистене на паролата, без да знаят за вградения режим на достъп в Jackson!
*   **Точен Senior отговор:** Използваме **`@JsonProperty(access = JsonProperty.Access.WRITE_ONLY)`**!
    *   `WRITE_ONLY`: Полето се записва в Java обекта при десериализация от входящ JSON (напр. при `POST /register`), но никога не се изписва при сериализация в отговора.
    *   `READ_ONLY`: Обратното — полето се изпраща към клиента в JSON отговора (напр. `id` или `createdAt`), но ако клиентът опита да го подаде във входящ JSON, Jackson го игнорира!

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (JACKSON) СРЕЩУ C# 12+ (`SYSTEM.TEXT.JSON`)

| Архитектурен детайл | ☕ Java 21+ (`Jackson`) | 🟣 C# 12+ (`System.Text.Json`) |
| :--- | :--- | :--- |
| **Именуване на JSON ключове (Naming)** | Запазва името на полето (`camelCase` в Java) | По подразбиране в ASP.NET Core превръща `PascalCase` в **`camelCase`** |
| **Непознати свойства във входящ JSON** | Хвърля `UnrecognizedPropertyException` (изисква `FAIL_ON_UNKNOWN_PROPERTIES = false`) | **Игнорира ги автоматично** по подразбиране |
| **Работа с `java.time` / `DateTime`** | Изисква `JavaTimeModule` (вграден в Spring Boot) | Вградена нативна поддръжка за `DateTime`, `DateOnly`, `TimeOnly` |
| **Защита от циклични референции** | `@JsonManagedReference` / `@JsonBackReference` или `@JsonIdentityInfo` | `ReferenceHandler.IgnoreCycles` в `JsonSerializerOptions` |
| **Полиморфна сериализация** | `@JsonTypeInfo` и `@JsonSubTypes` | `[JsonPolymorphic]` и `[JsonDerivedType(typeof(...), "type")]` (от .NET 7+) |

---
[⬅️ 03-06. Java Auto Mapping Objects](03-06-00-Auto-Mapping-DTOs.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-07-01. Syntax From Scratch](03-07-01-Syntax-From-Scratch.md)
