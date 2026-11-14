[⬅️ 03-06-00. Теория & Капани](03-06-00-Auto-Mapping-DTOs.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-07-00. JSON and XML Processing](03-07-00-JSON-And-XML-Processing.md)

---

# 03-06-01. Java Auto Mapping Objects (`MapStruct` & `ModelMapper`): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за настройване и използване на **MapStruct** (препоръчителен), **ModelMapper** (класически SoftUni подход) и чист ръчен мапинг с Java `record` DTOs в Spring Boot от нулата.

## 🧭 Съдържание на темата
* [1. MapStruct от нулата (`@Mapper`, `@Mapping`, игнориране и форматиране)](#1-mapstruct-от-нулата-mapper-mapping-игнориране-и-форматиране)
* [2. Обновяване на съществуващо Entity (`@MappingTarget`)](#2-обновяване-на-съществуващо-entity-mappingtarget)
* [3. ModelMapper от нулата (Spring `@Bean` конфигурация и персонални правила)](#3-modelmapper-от-нулата-spring-bean-конфигурация-и-персонални-правила)
* [4. Чист ръчен мапинг без външни библиотеки (Zero-Dependency Pattern)](#4-чист-ръчен-мапинг-без-външни-библиотеки-zero-dependency-pattern)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. MAPSTRUCT ОТ НУЛАТА (`@MAPPER`, `@MAPPING`, ИГНОРИРАНЕ И ФОРМАТИРАНЕ)

```java
import org.mapstruct.*;
import java.util.List;

// 1. DTO модели (Records)
public record CreateBookDto(String title, double price, Long authorId) {}
public record BookViewDto(Long id, String title, String formattedPrice, String authorFullName) {}

// 2. MapStruct Mapper интерфейс (имплементацията се генерира автоматично при компилация!)
@Mapper(componentModel = "spring", unmappedTargetPolicy = ReportingPolicy.IGNORE)
public interface BookMapper {

    // Мапиране от Entity към изходно DTO (вкл. вложено поле и Java израз)
    @Mapping(source = "author.fullName", target = "authorFullName")
    @Mapping(target = "formattedPrice", expression = "java(String.format(\"%.2f лв.\", book.getPrice()))")
    BookViewDto toViewDto(Book book);

    // Автоматично генерира цикъл за мапиране на цял списък!
    List<BookViewDto> toViewDtoList(List<Book> books);

    // Мапиране от входно DTO към ново Entity (игнорираме ID, защото базата го генерира)
    @Mapping(target = "id", ignore = true)
    @Mapping(target = "author", ignore = true)
    Book toEntity(CreateBookDto dto);
}
```

---

## 2. ОБНОВЯВАНЕ НА СЪЩЕСТВУВАЩО ENTITY (`@MAPPINGTARGET`)

Когато обновяваш съществуващ запис от базата данни (`PUT` / `PATCH`), използвай `@MappingTarget`, за да не създаваш нов обект и да запазиш `id` и `createdAt`:

```java
@Mapper(componentModel = "spring")
public interface ProductMapper {

    @Mapping(target = "id", ignore = true)
    @Mapping(target = "createdAt", ignore = true)
    // Пропуска null полетата от DTO-то (идеално за частичен PATCH update!)
    @BeanMapping(nullValuePropertyMappingStrategy = NullValuePropertyMappingStrategy.IGNORE)
    void updateProductFromDto(UpdateProductDto dto, @MappingTarget Product existingProduct);
}
```

---

## 3. MODELMAPPER ОТ НУЛАТА (SPRING `@BEAN` КОНФИГУРАЦИЯ И ПЕРСОНАЛНИ ПРАВИЛА)

```java
import org.modelmapper.ModelMapper;
import org.modelmapper.convention.MatchingStrategies;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class MapperConfig {

    @Bean
    public ModelMapper modelMapper() {
        ModelMapper mapper = new ModelMapper();
        // STRICT стратегия предотвратява случайни грешни съвпадения по части от имена!
        mapper.getConfiguration().setMatchingStrategy(MatchingStrategies.STRICT);

        // Персонално правило за мапиране на различно именувани полета:
        mapper.typeMap(User.class, UserProfileDto.class).addMappings(m ->
            m.map(src -> src.getAddress().getCity(), UserProfileDto::setCityName)
        );

        return mapper;
    }
}

// Използване в Service слой:
// UserProfileDto dto = modelMapper.map(userEntity, UserProfileDto.class);
```

---

## 4. ЧИСТ РЪЧЕН МАПИНГ БЕЗ ВЪНШНИ БИБЛИОТЕКИ (ZERO-DEPENDENCY PATTERN)

За малки и средни проекти много Senior екипи предпочитат чисти фабрични методи директно в `record` DTO-то — нула магия, 100% скорост и лесно дебъгване:

```java
public record EmployeeDto(Long id, String fullName, String department) {
    // Статичен фабричен метод за превръщане Entity -> DTO
    public static EmployeeDto fromEntity(Employee e) {
        return new EmployeeDto(
            e.getId(),
            e.getFirstName() + " " + e.getLastName(),
            e.getDepartment() != null ? e.getDepartment().getName() : "Няма отдел"
        );
    }
}

// Използване в Stream API:
// List<EmployeeDto> dtos = employees.stream().map(EmployeeDto::fromEntity).toList();
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Инструмент / Анотация | Какво прави | Бележка |
| :--- | :--- | :--- |
| **`@Mapper(componentModel = "spring")`** | Маркира MapStruct интерфейс като Spring Bean | Инжектира се в `@Service` с конструктор |
| **`@Mapping(source = "a.b", target = "c")`** | Свързва полета с различни имена или вложени обекти | Проверява се по време на компилация! |
| **`@Mapping(target = "id", ignore = true)`** | Забранява презаписването на дадено поле | Задължително за `id` при мапинг към Entity |
| **`@MappingTarget`** | Обновява полетата на вече съществуващ обект | Използва се при `update` операции |
| **`MatchingStrategies.STRICT`** | Настройка на `ModelMapper` за точно съвпадение | Предпазва от объркване на сходни имена |
| **`Dto.fromEntity(entity)`** | Чист ръчен мапинг без външни библиотеки | Най-лесен за дебъгване и 0 overhead |

---
[⬅️ 03-06-00. Теория & Капани](03-06-00-Auto-Mapping-DTOs.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-07-00. JSON and XML Processing](03-07-00-JSON-And-XML-Processing.md)
