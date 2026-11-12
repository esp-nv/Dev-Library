[⬅️ 02-09-00. Теория & Капани](02-09-00-Unit-Testing.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-10-00. Design Patterns in Java](02-10-00-Design-Patterns.md)

---

# 02-09-01. Unit Testing (JUnit 5 & Mockito): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за JUnit 5 Assertions, тестване на изключения (`assertThrows`), параметризирани тестове (`@ParameterizedTest`) и Mockito (`when`, `any()`, `verify`, `ArgumentCaptor`) в Java от нулата.

## 🧭 Съдържание на темата
* [1. Пълен шаблон на JUnit 5 тестови клас с Lifecycle анотации](#1-пълен-шаблон-на-junit-5-тестови-клас-с-lifecycle-анотации)
* [2. Тестване на изключения (`assertThrows`) и групови проверки (`assertAll`)](#2-тестване-на-изключения-assertthrows-и-групови-проверки-assertall)
* [3. Параметризирани тестове (`@ParameterizedTest` & `@CsvSource`)](#3-параметризирани-тестове-parameterizedtest--csvsource)
* [4. Mockito в действие (`when`, `any()`, `doThrow`, `verify`, `ArgumentCaptor`)](#4-mockito-в-действие-when-any-dothrow-verify-argumentcaptor)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПЪЛЕН ШАБЛОН НА JUNIT 5 ТЕСТОВИ КЛАС С LIFECYCLE АНОТАЦИИ

В Maven/Gradle проектите тестовете живеят в папка **`src/test/java/`** в същия пакет като тествания клас:

```java
import org.junit.jupiter.api.*;
import static org.junit.jupiter.api.Assertions.*;

@DisplayName("Тестове на калкулатор за отстъпки")
class DiscountCalculatorTest {
    private DiscountCalculator calculator;

    @BeforeEach
    void setUp() {
        // Изпълнява се преди всеки @Test метод
        calculator = new DiscountCalculator();
    }

    @Test
    @DisplayName("Трябва да приложи 10% отстъпка за VIP клиенти")
    void shouldApplyVipDiscount() {
        double result = calculator.calculate(100.0, true);
        assertEquals(90.0, result, 0.001); // 0.001 е делта толеранс за double!
    }
}
```

---

## 2. ТЕСТВАНЕ НА ИЗКЛЮЧЕНИЯ (`ASSERTTHROWS`) И ГРУПОВИ ПРОВЕРКИ (`ASSERTALL`)

```java
@Test
void withdraw_ShouldThrowException_WhenBalanceIsInsufficient() {
    BankAccount account = new BankAccount("Иван", 50.0);

    // 1. Проверка дали методът хвърля точното изключение и съобщение
    IllegalArgumentException ex = assertThrows(
        IllegalArgumentException.class,
        () -> account.withdraw(100.0)
    );
    assertEquals("Недостатъчна наличност!", ex.getMessage());
}

@Test
void createUser_ShouldPopulateAllFields() {
    User user = new User("Елизабет", "ADMIN");

    // 2. assertAll изпълнява ВСИЧКИ проверки, дори първата да се провали!
    assertAll("Проверка на потребител",
        () -> assertNotNull(user),
        () -> assertEquals("Елизабет", user.getName()),
        () -> assertEquals("ADMIN", user.getRole())
    );
}
```

---

## 3. ПАРАМЕТРИЗИРАНИ ТЕСТОВЕ (`@PARAMETERIZEDTEST` & `@CSVSOURCE`)

Позволяват да пуснеш един и същ тест с 10 различни набора входни данни без копиране на код:

```java
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

class ValidationTest {
    @ParameterizedTest
    @ValueSource(strings = {"", "   ", "\t"})
    void isBlank_ShouldReturnTrue_ForEmptyStrings(String input) {
        assertTrue(input.isBlank());
    }

    @ParameterizedTest
    @CsvSource({
        "10, 20, 30",
        "5,  5,  10",
        "-3, 3,  0"
    })
    void add_ShouldReturnCorrectSum(int a, int b, int expectedSum) {
        assertEquals(expectedSum, a + b);
    }
}
```

---

## 4. MOCKITO В ДЕЙСТВИЕ (`WHEN`, `ANY()`, `DOTHROW`, `VERIFY`, `ARGUMENTCAPTOR`)

```java
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.*;
import org.mockito.junit.jupiter.MockitoExtension;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class OrderServiceTest {
    @Mock private PaymentGateway paymentGateway;
    @InjectMocks private OrderService orderService;

    @Captor private ArgumentCaptor<Double> amountCaptor;

    @Test
    void processOrder_ShouldChargeExactAmountWithVat() {
        // 1. Симулиране на връщана стойност за произволен аргумент (anyDouble())
        when(paymentGateway.charge(anyDouble())).thenReturn(true);

        // 2. Симулиране на грешка при void метод:
        // doThrow(new RuntimeException("Timeout")).when(paymentGateway).logTransaction();

        orderService.placeOrder(100.0); // Добавя 20% ДДС -> 120.0

        // 3. Улавяне на точния аргумент, който OrderService е подал към paymentGateway!
        verify(paymentGateway).charge(amountCaptor.capture());
        assertEquals(120.0, amountCaptor.getValue(), 0.001);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Анотация | Библиотека | Какво прави |
| :--- | :--- | :--- |
| **`assertEquals(exp, act)`** | JUnit 5 | Проверява равенство чрез `.equals()` |
| **`assertTrue(cond)` / `assertFalse(cond)`** | JUnit 5 | Проверява булево условие |
| **`assertNull(obj)` / `assertNotNull(obj)`** | JUnit 5 | Проверява дали обектът е `null` |
| **`assertThrows(Ex.class, () -> ...)`** | JUnit 5 | Гарантира, че кодът хвърля очакваното изключение |
| **`when(mock.fn(any())).thenReturn(val)`** | Mockito | Задава какво да върне фалшивият обект |
| **`verify(mock, times(n)).fn(...)`** | Mockito | Проверява дали методът е извикан точно `n` пъти |
| **`verifyNoInteractions(mock)`** | Mockito | Гарантира, че зависимостта изобщо не е пипана |

---
[⬅️ 02-09-00. Теория & Капани](02-09-00-Unit-Testing.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-10-00. Design Patterns in Java](02-10-00-Design-Patterns.md)
