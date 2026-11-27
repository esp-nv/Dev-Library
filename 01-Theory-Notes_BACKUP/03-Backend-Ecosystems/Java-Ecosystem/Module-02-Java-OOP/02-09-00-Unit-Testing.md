[⬅️ 02-08. Exceptions & Error Handling](02-08-00-Exceptions-Handling.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-09-01. Syntax From Scratch](02-09-01-Syntax-From-Scratch.md)

---

# 02-09-00. Unit Testing (JUnit 5 & Mockito — Теория, AAA Патерн & Капани)

**Модул 02:** Java OOP | **Лекционна тема 09:** Модулно тестване в Java с **JUnit 5 (Jupiter)**, жизнения цикъл на тестовете (`@BeforeEach`, `@AfterEach`, `@BeforeAll`, `@AfterAll`), структурата **AAA (Arrange — Act — Assert)**, изолиране на зависимости с **Mockito** (`@Mock`, `@InjectMocks`, `when().thenReturn()`, `verify()`) и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Жизнен цикъл на JUnit 5 тест и Mockito изолация (Блок-схема)](#2-жизнен-цикъл-на-junit-5-тест-и-mockito-изолация-блок-схема)
* [3. Сравнителна карта в 4 езика (Java, C#, Python, JavaScript)](#3-сравнителна-карта-в-4-езика-java-c-python-javascript)
* [4. Плюсове и минуси на Unit Testing и Mocking](#4-плюсове-и-минуси-на-unit-testing-и-mocking)
* [5. Кодов одит: Правилен срещу Грешен код (Тестване с реална база и `UnnecessaryStubbingException`)](#5-кодов-одит-правилен-срещу-грешен-код-тестване-с-реална-база-и-unnecessarystubbingexception)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)
* [7. Архитектурно сравнение: Java 21+ (JUnit 5 + Mockito) срещу C# 12+ (xUnit + Moq)](#7-архитектурно-сравнение-java-21-junit-5--mockito-срещу-c-12-xunit--moq)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Патернът AAA (Трите фази на всеки чист тест):**
    1.  **Arrange (Подготовка):** Създаваш тествания обект и настройваш фалшивите зависимости (`when(repo.findById(1)).thenReturn(...)`).
    2.  **Act (Действие):** Извикваш **само един** метод върху тествания клас (`var result = service.getUser(1);`).
    3.  **Assert (Проверка):** Проверяваш дали резултатът съвпада с очаквания (`assertEquals("Ivan", result.getName())`).
*   **`@Mock` срещу `@InjectMocks` (Актьор срещу Сцена):**
    *   **`@Mock`:** Създава празна „кукла“ (фалшив обект) на зависимостта (напр. `UserRepository`), която не прави нищо, освен ако не ѝ кажеш какво да върне с `when(...)`.
    *   **`@InjectMocks`:** Създава **истинска инстанция** на класа, който тестваш (`UserService`), и автоматично пъха всички `@Mock` обекти в неговия конструктор!

---

## 2. ЖИЗНЕН ЦИКЪЛ НА JUNIT 5 ТЕСТ И MOCKITO ИЗОЛАЦИЯ (БЛОК-СХЕМА)

JUnit 5 създава **нова, чиста инстанция на тестовия клас преди ВСЕКИ `@Test` метод**, за да не си пречат тестовете един на друг:

```text
@BeforeAll (1 път преди всички тестове - задължително static метод!)
    │
    ├──► [Нова инстанция на TestClass] ➔ @BeforeEach ➔ @Test #1 ➔ @AfterEach
    │
    ├──► [Нова инстанция на TestClass] ➔ @BeforeEach ➔ @Test #2 ➔ @AfterEach
    │
@AfterAll  (1 път след всички тестове - задължително static метод!)
```

---

## 3. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА (JAVA, C#, PYTHON, JAVASCRIPT)

| Концепция | ☕ Java 21+ (JUnit 5 + Mockito) | 🟣 C# 12+ (xUnit + Moq / NSubstitute) | 🐍 Python 3.x (`pytest` + `unittest.mock`) | 🟡 JavaScript (`Vitest` / `Jest`) |
| :--- | :--- | :--- | :--- | :--- |
| **Анотация за тест** | `@Test` | `[Fact]` (или `[Theory]`) | Функция `def test_...():` | `test("...", () => { ... })` |
| **Setup преди всеки тест** | `@BeforeEach` | Конструктор на тестовия клас | `@pytest.fixture` | `beforeEach(() => { ... })` |
| **Проверка за равенство** | `assertEquals(expected, actual)` | `Assert.Equal(expected, actual)` | `assert actual == expected` | `expect(actual).toBe(expected)` |
| **Проверка за изключение** | `assertThrows(Ex.class, () -> ...)` | `Assert.Throws<Ex>(() => ...)` | `with pytest.raises(Ex):` | `expect(() => ...).toThrow()` |
| **Параметризиран тест** | `@ParameterizedTest` + `@ValueSource` | `[Theory]` + `[InlineData(...)]` | `@pytest.mark.parametrize` | `test.each([...])` |
| **Създаване на Mock** | `@Mock` или `Mockito.mock(Repo.class)` | `new Mock<IRepo>()` | `MagicMock()` / `mocker.patch` | `vi.fn()` / `jest.fn()` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА UNIT TESTING И MOCKING

### 🟢 Предимства (Pros):
*   **Светкавична обратна връзка (милисекунди):** Хиляда изолирани Unit теста се изпълняват за 1–2 секунди без Docker, без мрежа и без база данни.
*   **Увереност при рефакторинг:** Можеш смело да пренапишеш вътрешната логика на един клас и веднага да разбереш дали не си счупил стари изисквания.

### 🔴 Недостатъци (Cons):
*   **Прекаленото мокване (Over-Mocking) скрива интеграционни бъгове:** Ако мокнеш всичко, тестовете ти може да са 100% зелени, но приложението да гръмне в продукция заради грешна SQL заявка или сбъркан JSON мапинг (затова се комбинират с **Integration Tests** в Spring).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (ТЕСТВАНЕ С РЕАЛНА БАЗА И `UNNECESSARYSTUBBINGEXCEPTION`)

### ❌ ГРЕШЕН КОД (Объркан ред на аргументите в `assertEquals` и ненужен `when()` в Mockito):
```java
@ExtendWith(MockitoExtension.class)
class BadUserServiceTest {
    @Mock private UserRepository repo;
    @InjectMocks private UserService service;

    @Test
    void testFindUser() {
        // 🚨 ГРЕШКА 1: Настройваш mock за ID = 1, но в теста викаш метод с ID = 2!
        // Mockito Strict Stubbing веднага хвърля UnnecessaryStubbingException!
        when(repo.findById(1L)).thenReturn(Optional.of(new User("Ivan")));

        String name = service.getUserNameById(2L);

        // 🚨 ГРЕШКА 2: Обърнат ред! В JUnit 5 ПЪРВИЯТ аргумент е EXPECTED, а ВТОРИЯТ е ACTUAL!
        assertEquals(name, "Ivan");
    }
}
```
*   **Как да се избегне:** В JUnit 5 **винаги** подавай `(expected, actual)` в този ред (иначе при грешка съобщението ще каже *"Expected: null, Actual: Ivan"* наобратно!). Никога не дефинирай `when(...)` правила, които не се използват в конкретния тест.

### 🟢 ПРАВИЛЕН КОД (Чист AAA тест с JUnit 5 и Mockito):
```java
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class UserServiceTest {

    @Mock
    private UserRepository userRepository;

    @InjectMocks
    private UserService userService;

    @Test
    void getUserNameById_ShouldReturnName_WhenUserExists() {
        // 1. Arrange
        when(userRepository.findById(1L)).thenReturn(Optional.of(new User(1L, "Елизабет")));

        // 2. Act
        String actualName = userService.getUserNameById(1L);

        // 3. Assert
        assertEquals("Елизабет", actualName);
        verify(userRepository, times(1)).findById(1L);
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между **`@Mock`** и **`@Spy`** в Mockito?“*
*   **Уловката:** Кандидатите често ползват `@Mock` за всичко и се затрудняват, когато трябва да тестват клас, при който 4 метода трябва да работят реално, а само 1 метод да бъде подменен.
*   **Точен Senior отговор:** 
    1.  **`@Mock`** създава **100% фалшив обект (куха обвивка)** — нито един реален метод на класа не се изпълнява; всички методи връщат стойности по подразбиране (`0`, `false`, `null`, празен списък), освен ако не ги настроиш изрично с `when(...).thenReturn(...)`.
    2.  **`@Spy` (Частичен Mock)** обвива **реална инстанция на класа** — по подразбиране се изпълняват **истинските методи** на обекта, но ти можеш да прехванеш и подмениш само конкретен метод (чрез `doReturn(val).when(spyObj).method()`), както и да провериш с `verify()` колко пъти е бил извикан.

---

## 7. АРХИТЕКТУРНО СРАВНЕНИЕ: JAVA 21+ (JUNIT 5 + MOCKITO) СРЕЩУ C# 12+ (XUNIT + MOQ)

| Архитектурен детайл | ☕ Java 21+ (JUnit 5 + Mockito) | 🟣 C# 12+ (xUnit + Moq) |
| :--- | :--- | :--- |
| **Зависимост за тестове (Build)** | `junit-jupiter` + `mockito-junit-jupiter` (в `pom.xml`) | `xunit` + `Moq` (в `.csproj`) |
| **Видимост на тестовия клас/метод** | В JUnit 5 **не е нужно да са `public`** (достатъчно е `package-private`: `void test()`) | Класът и методът **задължително са `public`** |
| **Инициализация на Mocks** | `@ExtendWith(MockitoExtension.class)` над класа | `var mockRepo = new Mock<IUserRepository>();` |
| **Подаване на Mock към Service** | Автоматично чрез `@InjectMocks` | Ръчно в конструктора: `new UserService(mockRepo.Object)` |
| **Настройка на поведение (Stub)** | `when(repo.save(any())).thenReturn(user);` | `mockRepo.Setup(r => r.Save(It.IsAny<User>())).Returns(user);` |
| **Проверка за извикване (Verify)** | `verify(repo, times(1)).deleteById(5L);` | `mockRepo.Verify(r => r.DeleteById(5), Times.Once);` |

---
[⬅️ 02-08. Exceptions & Error Handling](02-08-00-Exceptions-Handling.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-09-01. Syntax From Scratch](02-09-01-Syntax-From-Scratch.md)
