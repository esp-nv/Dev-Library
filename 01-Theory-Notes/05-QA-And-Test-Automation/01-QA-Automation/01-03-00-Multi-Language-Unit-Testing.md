[⬅️ 01-02-01. JQL, Automation, CI/CD & Cheat Sheet](01-02-01-Syntax-Test-Design-Jira-And-MS-Project.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-03-01. NUnit, JUnit 5, Vitest & pytest](01-03-01-Syntax-Multi-Language-Unit-Testing.md)

---

# 01-03-00. Unit Testing & Mocking (Теория, AAA, Тестови Двойници & Капани)

**Раздел 02:** Multi-Language Unit Testing | **Цел:** Овладяване на принципите на изолираното модулно тестване (Unit Testing), тристъпковия модел **Arrange–Act–Assert (AAA)**, разликите между петте вида тестови двойници (**Dummy, Stub, Spy, Mock, Fake**), метриките за кодово покритие (Code Coverage) и предотвратяване на капани като тестване на имплементацията вместо поведението и зависимост от реалния системен часовник.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: AAA, FIRST & Mock vs. Stub)](#1-как-да-се-запомни-по-лесно-мнемоника-aaa-first--mock-vs-stub)
* [2. Архитектура на изолирания Unit тест и Блок-схема (AAA + Mocking)](#2-архитектура-на-изолирания-unit-тест-и-блок-схема-aaa--mocking)
* [3. Сравнителна матрица: Unit Testing екосистеми в C#, Java, TypeScript и Python](#3-сравнителна-матрица-unit-testing-екосистеми-в-c-java-typescript-и-python)
* [4. Петте вида тестови двойници (Test Doubles) и Плюсове/Минуси на Mocking](#4-петте-вида-тестови-двойници-test-doubles-и-плюсовеминуси-на-mocking)
* [5. Кодов одит: Грешен (Силно обвързан) срещу Правилен (Изолиран с Dependency Injection) тест](#5-кодов-одит-грешен-силно-обвързан-срещу-правилен-изолиран-с-dependency-injection-тест)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: AAA, FIRST & MOCK VS. STUB)

* **Шаблонът AAA (Трите параграфа на всеки тест):**
  * **Arrange (Подготовка):** Създаваш тествания обект (SUT — *System Under Test*), настройваш входните данни и конфигурираш моковете.
  * **Act (Действие):** Извикваш **един конкретен метод**, който тестваш (обикновено 1 ред код).
  * **Assert (Проверка):** Проверяваш дали върнатият резултат, хвърленото изключение или извиканият метод отговарят на очакванията.
* **Принципите F.I.R.S.T. за добри Unit тестове:**
  * **F**ast (Бързи — хиляди тестове минават за секунди).
  * **I**ndependent / Isolated (Независими един от друг — редът на пускане няма значение).
  * **R**epeatable (Повтаряеми — дават еднакъв резултат на всеки компютър и по всяко време).
  * **S**elf-Validating (Самопроверяващи се — връщат ясно Pass или Fail без ръчно четене на конзолата).
  * **T**imely / Thorough (Навременни — пишат се заедно с кода и покриват граничните случаи).
* **Stub срещу Mock (Държава срещу Поведение):**
  * **Stub** просто **връща предварително подготвени данни** (напр. `repo.GetUser(1)` винаги връща `"Ivan"`), за да може тестът да продължи.
  * **Mock** **проверява дали и колко пъти е бил извикан** даден метод (напр. `emailService.Verify(x => x.Send(...), Times.Once)`).

---

## 2. АРХИТЕКТУРА НА ИЗОЛИРАНИЯ UNIT ТЕСТ И БЛОК-СХЕМА (AAA + MOCKING)

За да не докосва Unit тестът реална база данни, мрежа или файлов диск, външните зависимости се подменят с интерфейси и тестови двойници:

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  1. ARRANGE (Подготовка на изолирана среда)                             │
 │     [Mock<IOrderRepository>] ──(връща фалшив обект)──┐                  │
 │     [Mock<IEmailSender>]     ──(записва повиквания)──┼──► [OrderService]│
 │                                                      │        (SUT)     │
 └──────────────────────────────────────────────────────┴─────────┬────────┘
                                                                  │
 ┌────────────────────────────────────────────────────────────────▼────────┐
 │  2. ACT (Изпълнение на 1 бизнес операция)                               │
 │     var result = orderService.PlaceOrder(new OrderDto(...));            │
 └────────────────────────────────────────────────────────────────┬────────┘
                                                                  │
 ┌────────────────────────────────────────────────────────────────▼────────┐
 │  3. ASSERT (Проверка на резултат + Верификация на поведение)            │
 │     • Assert.That(result.IsSuccess, Is.True);     (State Verification)  │
 │     • emailMock.Verify(e => e.Send(), Times.Once); (Behavior Verify)    │
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА МАТРИЦА: UNIT TESTING ЕКОСИСТЕМИ В C#, JAVA, TYPESCRIPT И PYTHON

| Характеристика | 🟣 C# (.NET 8+) | ☕ Java (21+) | 🟡 TypeScript (Node/Web) | 🐍 Python (3.x) |
| :--- | :--- | :--- | :--- | :--- |
| **Водещи Test Frameworks** | **xUnit**, **NUnit**, MSTest | **JUnit 5 (Jupiter)**, TestNG | **Vitest**, **Jest** | **pytest**, `unittest` (вграден) |
| **Mocking Библиотеки** | **Moq**, **NSubstitute** | **Mockito** | **Vitest (`vi.fn`)**, Jest (`jest.fn`) | **`unittest.mock` (`MagicMock`)**, `pytest-mock` |
| **Fluent Assertions** | **FluentAssertions** (`result.Should().Be(5)`) | **AssertJ** (`assertThat(res).isEqualTo(5)`) | Вградени (`expect(res).toBe(5)`) | Нативен `assert res == 5` (с детайлен diff в pytest!) |
| **Параметризирани тестове** | `[TestCase(1, 2)]` / `[Theory] [InlineData]` | `@ParameterizedTest` + `@CsvSource` | `it.each([[1, 2], [3, 4]])` | `@pytest.mark.parametrize("a,b", [(1, 2)])` |
| **Setup / Teardown** | `[SetUp]` / `[TearDown]` (NUnit) | `@BeforeEach` / `@AfterEach` | `beforeEach()` / `afterEach()` | `@pytest.fixture` (с `yield` за почистване) |

---

## 4. ПЕТТЕ ВИДА ТЕСТОВИ ДВОЙНИЦИ (TEST DOUBLES) И ПЛЮСОВЕ/МИНУСИ НА MOCKING

1. **Dummy:** Празен обект, подаван само за да запълни списъка с параметри на конструктор/метод, но никога не се използва реално.
2. **Stub:** Обект с твърдо зададени отговори (`Return("OK")`), използван за симулиране на различни сценарии (успех, грешка, празен списък).
3. **Spy:** Обвива реален обект и записва информация за извикванията му (брой повиквания, подадени аргументи), докато изпълнява истинската си логика.
4. **Mock:** Програмируем обект с предварително зададени очаквания, който сам валидира дали е бил извикан правилно.
5. **Fake:** Олекотена, но работеща имплементация (напр. `In-Memory Database` или `Dictionary` хранилище вместо реален SQL Server).

### 🟢 Предимства и 🔴 Недостатъци на интензивното Mocking:
* **🟢 Плюсове:** Изключителна скорост (`< 1ms` на тест), пълна изолация от мрежови сривове и лесно симулиране на редки грешки (напр. `TimeoutException` от платежен шлюз).
* **🔴 Минуси:** Прекаленото мокване на вътрешни класове прави тестовете чупливи при рефакторинг (тестът пада, въпреки че бизнес логиката работи) и може да скрие интеграционни бъгове в реални SQL заявки.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (СИЛНО ОБВЪРЗАН) СРЕЩУ ПРАВИЛЕН (ИЗОЛИРАН) ТЕСТ

### ❌ ГРЕШЕН КОД (Силна обвързаност с `DateTime.Now` и реална база данни):
```csharp
public class DiscountService
{
    public decimal CalculateDiscount(decimal amount)
    {
        // 🚨 ГРЕШКА 1: Директно извикване на DateTime.Now -> тестът ще работи само в петък!
        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            return amount * 0.20m;

        // 🚨 ГРЕШКА 2: Директно инстанциране на SqlRepository -> бавен и чуплив тест!
        var repo = new SqlCustomerRepository();
        return repo.IsVip(1) ? amount * 0.10m : 0m;
    }
}
```

### 🟢 ПРАВИЛЕН КОД (Dependency Injection + `TimeProvider` интерфейс):
```csharp
public class DiscountService(TimeProvider timeProvider, ICustomerRepository repo)
{
    public decimal CalculateDiscount(int customerId, decimal amount)
    {
        if (timeProvider.GetUtcNow().DayOfWeek == DayOfWeek.Friday)
            return amount * 0.20m;

        return repo.IsVip(customerId) ? amount * 0.10m : 0m;
    }
}
```
*(Сега в Unit теста подаваме `FakeTimeProvider` с фиксирана дата петък и `Mock<ICustomerRepository>`, гарантирайки 100% детерминиран резултат по всяко време на денонощието.)*

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Дали **100% Code Coverage** гарантира, че в кода няма бъгове?“*
  * **Отговор:** **Не!** Code Coverage измерва само кои редове или клонове (`if/else`) са били изпълнени по време на тестовете, но **не проверява качеството на проверките (`Assertions`)** и не хваща липсващи бизнес изисквания (например забравена проверка за отрицателно число или гранична стойност `0`). Високото покритие (80–85%) е полезен индикатор за пропуснати зони, но не е крайна цел само по себе си.
* **Въпрос 2:** *„Какво е **White-Box** структурата **Branch Coverage (Decision Coverage)** спрямо **Statement (Line) Coverage**?“*
  * **Отговор:** При `if (x > 0) { doWork(); }`, ако пуснем само 1 тест с `x = 5`, постигаме **100% Statement Coverage** (всички редове са минати), но само **50% Branch Coverage**, защото клонът `false` (когато `x <= 0`) никога не е тестван! Затова Branch Coverage е много по-строга и надеждна метрика.

---

[⬅️ 01-02-01. JQL, Automation, CI/CD & Cheat Sheet](01-02-01-Syntax-Test-Design-Jira-And-MS-Project.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-03-01. NUnit, JUnit 5, Vitest & pytest](01-03-01-Syntax-Multi-Language-Unit-Testing.md)

© 2026 Dev-Library / 01-03-00. Unit Testing & Mocking (Теория, AAA, Тестови Двойници & Капани)
