[⬅️ 03-05. Advanced Querying (JPQL & N+1)](03-05-00-Advanced-Querying.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-06-01. Syntax From Scratch](03-06-01-Syntax-From-Scratch.md)

---

# 03-06-00. Java Auto Mapping Objects (`ModelMapper` / `MapStruct` & DTOs — Теория & Капани)

**Модул 03:** Spring Data | **Лекционна тема 06:** Защо разделяме моделите на базата данни (`@Entity`) от трансферните обекти (**DTO — Data Transfer Objects**), сравнение между Runtime Reflection мапинг (**ModelMapper**) и Compile-time генериране на код (**MapStruct**), мапиране към модерни Java `record` класове и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Поток на данните през слоевете (DTO ➔ Entity ➔ DTO Блок-схема)](#2-поток-на-данните-през-слоевете-dto--entity--dto-блок-схема)
* [3. Сравнителна карта в 4 езика (Java MapStruct, C# AutoMapper/Mapperly, Python Pydantic, JS Zod)](#3-сравнителна-карта-в-4-езика-java-mapstruct-c-automappermapperly-python-pydantic-js-zod)
* [4. Плюсове и минуси: `MapStruct` (Compile-time) срещу `ModelMapper` (Runtime)](#4-плюсове-и-минуси-mapstruct-compile-time-срещу-modelmapper-runtime)
* [5. Кодов одит: Правилен срещу Грешен код (Mass Assignment уязвимост и мапиране на `LAZY` релации)](#5-кодов-одит-правилен-срещу-грешен-код-mass-assignment-уязвимост-и-мапиране-на-lazy-релации)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (MapStruct / ModelMapper) срещу C# 12+ (AutoMapper / Mapperly)](#7-архитектурно-сравнение-java-21-mapstruct--modelmapper-срещу-c-12-automapper--mapperly)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`Entity` е вътрешният паспорт, `DTO` е визитната картичка:** На визитната картичка (DTO) слагаш само името, длъжността и служебния имейл, които искаш да покажеш на света. Никога не даваш на непознат целия си паспорт (`@Entity`) с ЕГН-то, паролата (`passwordHash`) и банковите сметки!
*   **`ModelMapper` срещу `MapStruct` (Преводач на живо срещу Предварително отпечатан речник):**
    *   **`ModelMapper`:** Работи с **Reflection по време на изпълнение (Runtime)**. Гъвкав и лесен за старт, но по-бавен и ако сбъркаш име на поле, разбираш чак когато потребител извика метода.
    *   **`MapStruct` (Индустриалният стандарт в модерна Java):** Работи като **Annotation Processor по време на компилация (Compile-time)**. Преди програмата изобщо да тръгне, MapStruct генерира чист, обикновен Java клас с директни извиквания на `get...()` и `set...()` — **100 пъти по-бърз** и с проверка на типовете от компилатора!

---

## 2. ПОТОК НА ДАННИТЕ ПРЕЗ СЛОЕВЕТЕ (DTO ➔ ENTITY ➔ DTO БЛОК-СХЕМА)

```text
[ Клиент / Браузър (JSON) ]
           │  1. CreateUserRequestDto (username, email, rawPassword)
           ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Controller слой (@RestController) ➔ Валидира DTO (@Valid)               │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ Service слой (@Service)                                                 │
│ • Мапва RequestDto ➔ @Entity User (хешира паролата!)                    │
│ • Записва в базата чрез UserRepository                                  │
│ • Мапва @Entity User ➔ UserResponseDto (БЕЗ passwordHash!)              │
└────────────────────────────────────┬────────────────────────────────────┘
                                     ▼  2. UserResponseDto (id, username, email)
                        [ Клиент / Браузър (JSON) ]
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA MAPSTRUCT, C# AUTOMAPPER/MAPPERLY, PYTHON PYDANTIC, JS ZOD)

| Концепция | ☕ Java 21+ (`MapStruct` / `ModelMapper`) | 🟣 C# 12+ (`AutoMapper` / `Mapperly`) | 🐍 Python 3.x (`Pydantic` / `DRF`) | 🟡 JS / TS (`Zod` / `class-transformer`) |
| :--- | :--- | :--- | :--- | :--- |
| **DTO модел** | `public record UserDto(Long id, String name) {}` | `public record UserDto(int Id, string Name);` | `class UserSchema(BaseModel):` | `type UserDto = z.infer<typeof UserSchema>` |
| **Дефиниция на Mapper** | `@Mapper(componentModel = "spring") interface` | `class MappingProfile : Profile` (AutoMapper) | `UserSchema.model_validate(orm_obj)` | `plainToInstance(UserDto, rawObj)` |
| **Кога се извършва мапингът** | **Compile-time** (`MapStruct`) / Runtime (`ModelMapper`) | Runtime (`AutoMapper`) / **Compile-time** (`Mapperly`) | Runtime валидация и сериализация | Runtime парсване |
| **Ръчен мапинг без библ.** | `new UserDto(u.getId(), u.getName())` | `new UserDto(u.Id, u.Name)` | `UserSchema(id=u.id, name=u.name)` | `{ id: u.id, name: u.name }` |

---

## 4. ПЛЮСОВЕ И МИНУСИ: `MAPSTRUCT` (COMPILE-TIME) СРЕЩУ `MODELMAPPER` (RUNTIME)

### 🟢 `MapStruct` — Предимства и Недостатъци:
*   **Предимства:** Нулев Reflection по време на изпълнение (светкавична скорост); компилаторът веднага сигнализира, ако си забравила да мапнеш задължително поле (`unmappedTargetPolicy = ReportingPolicy.ERROR`); поддържа перфектно Java `record` класове.
*   **Недостатъци:** Изисква настройка на `annotationProcessorPaths` в Maven/Gradle (заедно с Lombok, ако го ползваш).

### 🔵 `ModelMapper` — Предимства и Недостатъци:
*   **Предимства:** Настройва се с 2 реда код (`new ModelMapper()`) и автоматично мапва вложени обекти по съвпадащи имена (`order.getCustomer().getName()` ➔ `orderDto.getCustomerName()`).
*   **Недостатъци:** Използва тежък Reflection; лошо работи с неизменяеми `record` класове (защото търси празен конструктор и `set...` методи); тихо оставя полета `null`, ако има малка разлика в имената.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (MASS ASSIGNMENT УЯЗВИМОСТ И МАПИРАНЕ НА `LAZY` РЕЛАЦИИ)

### ❌ ГРЕШЕН КОД (Използване на едно и също Entity за вход от клиента — Mass Assignment Hack!):
```java
@PostMapping("/users/register")
public ResponseEntity<User> register(@RequestBody User incomingUser) {
    // 🚨 ГРЕШКА 1 (Mass Assignment / Over-Posting уязвимост!):
    // Хакер може да изпрати JSON: {"username": "hacker", "role": "ADMIN", "balance": 1000000}!
    // Тъй като приемаш директно @Entity User, всички тези полета се записват в базата!
    return ResponseEntity.ok(userRepository.save(incomingUser));
}
```
*   **Как да се избегне:** Създавай отделни специализирани DTO класове за всяка операция: **`RegisterUserRequestDto`** (съдържа само `username`, `email`, `password` — **без `role` и `id`!**) и **`UserResponseDto`** (съдържа само безопасните изходни данни).

### 🟢 ПРАВИЛЕН КОД (Типово безопасен `MapStruct` интерфейс с `record` DTO):
```java
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;

public record UserResponseDto(Long id, String username, String departmentName) {}

@Mapper(componentModel = "spring") // Регистрира се като Spring Bean -> инжектира се в Service!
public interface UserMapper {

    // Мапва вложено поле user.getDepartment().getName() -> dto.departmentName()
    @Mapping(source = "department.name", target = "departmentName")
    UserResponseDto toResponseDto(User user);
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какъв е скритият проблем, когато пуснеш автоматичен мапер (`ModelMapper` или `MapStruct`) върху JPA `@Entity`, което има няколко **`FetchType.LAZY`** колекции?“*
*   **Уловката:** Автоматичните мапери са „слепи“ за Hibernate Proxy обектите!
*   **Точен Senior отговор:** Когато маперът обхожда всички гетъри на `@Entity` обекта (`user.getOrders()`, `user.getRoles()`, `user.getAddresses()`), за да ги копира в DTO-то, той **насилствено задейства зареждането на всяка една `LAZY` колекция** от базата данни (предизвиквайки лавина от **N+1 SQL заявки**), а ако мапингът се извика извън `@Transactional` метод, хвърля **`LazyInitializationException`**! Затова никога не мапвай сложни Entity графи с автоматичен мапер без предварително зареждане (`JOIN FETCH`) или използвай директна **JPQL DTO Projection**.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (MAPSTRUCT / MODELMAPPER) СРЕЩУ C# 12+ (AUTOMAPPER / MAPPERLY)

| Архитектурен детайл | ☕ Java 21+ (`MapStruct` & `ModelMapper`) | 🟣 C# 12+ (`Mapperly` & `AutoMapper`) |
| :--- | :--- | :--- |
| **Compile-time Source Generator** | **MapStruct** (генерира `.java` код чрез Annotation Processor) | **Riok.Mapperly** (генерира C# код чрез Roslyn Source Generators) |
| **Runtime Reflection мапер** | **ModelMapper** (конфигурира се с `TypeMap` и `PropertyMap`) | **AutoMapper** (конфигурира се с `CreateMap<Src, Dest>().ForMember(...)`) |
| **Интеграция с DI контейнера** | `@Mapper(componentModel = "spring")` (инжектира се с конструктор) | `services.AddAutoMapper(...)` или `builder.Services.AddSingleton<UserMapper>()` |
| **Мапиране към неизменяеми Records** | MapStruct поддържа нативно Java `record` (чрез конструктора му) | Mapperly и AutoMapper поддържат нативно C# `record` (чрез Primary Constructor) |
| **Индустриална тенденция** | Масово преминаване от `ModelMapper` към **`MapStruct`** или ръчни `.toDto()` методи | Масово преминаване от `AutoMapper` към **`Mapperly`** или ръчни Extension методи `.ToDto()` |

---
[⬅️ 03-05. Advanced Querying (JPQL & N+1)](03-05-00-Advanced-Querying.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-06-01. Syntax From Scratch](03-06-01-Syntax-From-Scratch.md)
