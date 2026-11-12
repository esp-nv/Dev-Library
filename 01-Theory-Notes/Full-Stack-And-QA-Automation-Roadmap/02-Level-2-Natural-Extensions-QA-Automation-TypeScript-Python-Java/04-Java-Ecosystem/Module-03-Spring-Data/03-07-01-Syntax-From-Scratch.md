[⬅️ 03-07-00. Теория & Капани](03-07-00-JSON-And-XML-Processing.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ Module 04: Spring Fundamentals](../Module-04-Spring-Fundamentals/04-00-Intro.md)

---

# 03-07-01. JSON and XML Processing (Jackson & Gson): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за конфигуриране на Jackson `ObjectMapper`, работа с анотации и дати (`LocalDate` / `LocalDateTime`), четене и писане на списъци с `TypeReference`, обработка на XML с `XmlMapper` и използване на Google Gson в Java от нулата.

## 🧭 Съдържание на темата
* [1. Конфигуриране на Jackson `ObjectMapper` и ключови анотации](#1-конфигуриране-на-jackson-objectmapper-и-ключови-анотации)
* [2. Сериализация и десериализация на единични обекти и списъци (`TypeReference`)](#2-сериализация-и-десериализация-на-единични-обекти-и-списъци-typereference)
* [3. Обработка на XML данни с Jackson (`XmlMapper`)](#3-обработка-на-xml-данни-с-jackson-xmlmapper)
* [4. Работа с Google Gson (`GsonBuilder` и `TypeToken`)](#4-работа-с-google-gson-gsonbuilder-и-typetoken)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. КОНФИГУРИРАНЕ НА JACKSON `OBJECTMAPPER` И КЛЮЧОВИ АНОТАЦИИ

```java
import com.fasterxml.jackson.annotation.*;
import com.fasterxml.jackson.databind.*;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import java.time.LocalDate;

@JsonIgnoreProperties(ignoreUnknown = true) // Игнорира непознати полета във входящия JSON
@JsonInclude(JsonInclude.Include.NON_NULL)  // Пропуска полета със стойност null при писане
public class UserJsonDto {

    @JsonProperty("user_id")
    private Long id;

    private String username;

    @JsonProperty(access = JsonProperty.Access.WRITE_ONLY)
    private String password; // Чете се от JSON, но НИКОГА не се изпраща обратно!

    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "yyyy-MM-dd")
    private LocalDate birthDate;

    public UserJsonDto() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getUsername() { return username; }
    public void setUsername(String username) { this.username = username; }
    public String getPassword() { return password; }
    public void setPassword(String password) { this.password = password; }
    public LocalDate getBirthDate() { return birthDate; }
    public void setBirthDate(LocalDate birthDate) { this.birthDate = birthDate; }
}
```

---

## 2. СЕРИАЛИЗАЦИЯ И ДЕСЕРИАЛИЗАЦИЯ НА ЕДИНИЧНИ ОБЕКТИ И СПИСЪЦИ (`TYPEREFERENCE`)

```java
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import java.io.File;
import java.util.List;

public class JacksonDemo {
    public static void main(String[] args) throws Exception {
        // 1. Създаване и настройка на ObjectMapper (в Spring Boot се инжектира готов Bean!)
        ObjectMapper mapper = new ObjectMapper()
                .registerModule(new JavaTimeModule())
                .disable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS)
                .enable(SerializationFeature.INDENT_OUTPUT); // Pretty Print форматиране

        // 2. Java обект -> JSON низ
        UserJsonDto user = new UserJsonDto();
        user.setId(10L);
        user.setUsername("elizabeth");
        user.setPassword("TopSecret123");
        String json = mapper.writeValueAsString(user);

        // 3. JSON низ -> Единичен Java обект
        UserJsonDto parsedUser = mapper.readValue(json, UserJsonDto.class);

        // 4. JSON масив от файл -> List<UserJsonDto> чрез TypeReference
        String jsonArray = """
                [
                  {"user_id": 1, "username": "ivan"},
                  {"user_id": 2, "username": "maria"}
                ]
                """;
        List<UserJsonDto> usersList = mapper.readValue(jsonArray, new TypeReference<List<UserJsonDto>>() {});

        // 5. Директен запис във файл
        mapper.writeValue(new File("users.json"), usersList);
    }
}
```

---

## 3. ОБРАБОТКА НА XML ДАННИ С JACKSON (`XMLMAPPER`)

Изисква зависимостта `com.fasterxml.jackson.dataformat:jackson-dataformat-xml`:

```java
import com.fasterxml.jackson.dataformat.xml.XmlMapper;
import com.fasterxml.jackson.dataformat.xml.annotation.*;
import java.util.List;

@JacksonXmlRootElement(localName = "catalog")
class ProductCatalogXml {

    @JacksonXmlElementWrapper(useWrapping = false)
    @JacksonXmlProperty(localName = "product")
    private List<ProductItemXml> products;

    public List<ProductItemXml> getProducts() { return products; }
    public void setProducts(List<ProductItemXml> products) { this.products = products; }
}

class ProductItemXml {
    @JacksonXmlProperty(isAttribute = true)
    private Long id;

    @JacksonXmlProperty(localName = "title")
    private String name;

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
}

// Използване:
// XmlMapper xmlMapper = new XmlMapper();
// String xml = xmlMapper.writerWithDefaultPrettyPrinter().writeValueAsString(catalog);
// ProductCatalogXml parsed = xmlMapper.readValue(xml, ProductCatalogXml.class);
```

---

## 4. РАБОТА С GOOGLE GSON (`GSONBUILDER` И `TYPETOKEN`)

```java
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;
import com.google.gson.reflect.TypeToken;
import java.lang.reflect.Type;
import java.util.List;

class CarGsonDto {
    @Expose
    @SerializedName("car_brand")
    private String brand;

    @Expose
    private int year;

    private String internalSecret; // Няма @Expose -> ще бъде игнорирано!
}

public class GsonDemo {
    public static void main(String[] args) {
        Gson gson = new GsonBuilder()
                .excludeFieldsWithoutExposeAnnotation() // Сериализира само полета с @Expose
                .setPrettyPrinting()
                .create();

        String json = "[{\"car_brand\": \"BMW\", \"year\": 2024}]";

        // Парсване на генеричен List<CarGsonDto> с TypeToken:
        Type listType = new TypeToken<List<CarGsonDto>>() {}.getType();
        List<CarGsonDto> cars = gson.fromJson(json, listType);

        String outputJson = gson.toJson(cars);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Метод | Библиотека | Какво прави |
| :--- | :--- | :--- |
| **`@JsonProperty("key_name")`** | Jackson | Задава точното име на ключа в JSON |
| **`@JsonIgnore`** | Jackson | Напълно скрива полето и при четене, и при писане |
| **`@JsonProperty(access = Access.WRITE_ONLY)`** | Jackson | Приема полето от входящ JSON, но го крие в отговора |
| **`@JsonFormat(pattern = "yyyy-MM-dd")`** | Jackson | Форматира `LocalDate` / `LocalDateTime` |
| **`new TypeReference<List<T>>() {}`** | Jackson | Запазва генеричния тип на колекция при `readValue` |
| **`@JacksonXmlProperty(isAttribute = true)`** | Jackson XML | Записва полето като XML атрибут (`<item id="1">`) |
| **`@SerializedName("key_name")`** | Google Gson | Еквивалент на `@JsonProperty` в Gson |
| **`new TypeToken<List<T>>() {}.getType()`** | Google Gson | Еквивалент на `TypeReference` в Gson |

---
[⬅️ 03-07-00. Теория & Капани](03-07-00-JSON-And-XML-Processing.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ Module 04: Spring Fundamentals](../Module-04-Spring-Fundamentals/04-00-Intro.md)
