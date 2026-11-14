[⬅️ 01-04. Manual QA & Test Design Techniques](01-04-Manual-QA-And-Test-Design-Techniques.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-06. Modern UI Automation: Playwright & AJAX](01-06-Modern-UI-Automation-Playwright-And-AJAX.md)

---

# 01-05. Multi-Language Unit Testing (C# NUnit, Java JUnit 5, TS Vitest, Python pytest)

**Цел:** Писане на чисти, изолирани и бързи компонентни тестове (Unit Tests) по индустриалния стандарт **AAA (Arrange – Act – Assert)**, параметризирани тестове за гранични стойности и изолиране на зависимости чрез **Mocking** в четирите основни езика.

## 🧭 Съдържание на темата
* [1. Златният стандарт AAA (Arrange – Act – Assert) & Сравнителна карта в 4 езика](#1-златният-стандарт-aaa-arrange--act--assert--сравнителна-карта-в-4-езика)
* [2. C# (.NET 8+): NUnit 4 + Moq (Unit Tests, `[TestCase]` & Mocking)](#2-c-net-8-nunit-4--moq-unit-tests-testcase--mocking)
* [3. Java 21+: JUnit 5 + Mockito (`@Test`, `@ParameterizedTest` & `@Mock`)](#3-java-21-junit-5--mockito-test-parameterizedtest--mock)
* [4. TypeScript (Vitest) & Python (`pytest` + `unittest.mock`)](#4-typescript-vitest--python-pytest--unittestmock)

---

## 1. ЗЛАТНИЯТ СТАНДАРТ AAA (ARRANGE – ACT – ASSERT) & СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА

Всеки качествен Unit тест се разделя визуално с празен ред на три части:
1. **Arrange (Подготовка):** Създаваме обектите, входните данни и моковете.
2. **Act (Действие):** Извикваме **само един** целеви метод, който тестваме.
3. **Assert (Проверка):** Проверяваме дали резултатът или хвърленото изключение отговаря на очакванията.

| Концепция | 🔷 C# (`NUnit` + `Moq`) | ☕ Java (`JUnit 5` + `Mockito`) | 🟦 TypeScript (`Vitest`) | 🐍 Python (`pytest`) |
| :--- | :--- | :--- | :--- | :--- |
| **Атрибут / Функция за тест** | `[Test]` | `@Test` | `it("...", () => { ... })` | `def test_...():` |
| **Параметризиран тест (BVA)** | `[TestCase(18, true)]` | `@ParameterizedTest` + `@CsvSource` | `it.each([[18, true]])` | `@pytest.mark.parametrize` |
| **Базова проверка (Equality)** | `Assert.That(res, Is.EqualTo(5));` | `assertEquals(5, res);` | `expect(res).toBe(5);` | `assert res == 5` |
| **Проверка за изключение** | `Assert.Throws<ArgumentException>(...)` | `assertThrows(IllegalArgumentException.class, ...)` | `expect(() => fn()).toThrow()` | `with pytest.raises(ValueError):` |
| **Създаване на Mock** | `new Mock<IUserRepo>()` | `@Mock UserRepository repo;` | `vi.fn()` / `vi.spyOn()` | `MagicMock()` / `mocker.patch` |

---

## 2. C# (.NET 8+): NUNIT 4 + MOQ (UNIT TESTS, `[TESTCASE]` & MOCKING)

```csharp
using Moq;
using NUnit.Framework;

public interface IDiscountPolicy { decimal GetRate(string tier); }

public class PriceCalculator(IDiscountPolicy policy)
{
    public decimal CalculateFinal(decimal basePrice, string tier)
    {
        if (basePrice < 0) throw new ArgumentOutOfRangeException(nameof(basePrice));
        var rate = policy.GetRate(tier);
        return basePrice * (1 - rate);
    }
}

[TestFixture]
public class PriceCalculatorTests
{
    [TestCase(100, 0.10, 90)]
    [TestCase(200, 0.25, 150)]
    public void CalculateFinal_ShouldApplyDiscount_WhenPriceIsValid(
        decimal price, decimal rate, decimal expected)
    {
        // 1. Arrange
        var policyMock = new Mock<IDiscountPolicy>();
        policyMock.Setup(p => p.GetRate("VIP")).Returns(rate);
        var calculator = new PriceCalculator(policyMock.Object);

        // 2. Act
        var actual = calculator.CalculateFinal(price, "VIP");

        // 3. Assert
        Assert.That(actual, Is.EqualTo(expected));
        policyMock.Verify(p => p.GetRate("VIP"), Times.Once);
    }

    [Test]
    public void CalculateFinal_ShouldThrow_WhenPriceIsNegative()
    {
        var calculator = new PriceCalculator(Mock.Of<IDiscountPolicy>());

        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateFinal(-10m, "STD"));
    }
}
```

---

---

## 3. JAVA 21+: JUNIT 5 + MOCKITO (`@TEST`, `@PARAMETERIZEDTEST` & `@MOCK`)

```java
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.math.BigDecimal;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class OrderServiceTest {

    @Mock
    private UserRepository userRepository;

    @InjectMocks
    private OrderService orderService;

    @ParameterizedTest
    @CsvSource({"18, true", "17, false", "65, true", "66, false"})
    void isEligibleAge_ShouldRespectBoundaries(int age, boolean expected) {
        assertEquals(expected, orderService.isEligibleAge(age));
    }

    @Test
    void createOrder_ShouldThrow_WhenUserNotFound() {
        // 1. Arrange
        when(userRepository.findById(99L)).thenReturn(Optional.empty());

        // 2. Act & 3. Assert
        assertThrows(UserNotFoundException.class,
                () -> orderService.createOrder(99L, BigDecimal.TEN));
        verify(userRepository, times(1)).findById(99L);
    }
}
```

---

## 4. TYPESCRIPT (VITEST) & PYTHON (`PYTEST` + `UNITTEST.MOCK`)

### 4.1 TypeScript (`Vitest` — `describe`, `it.each`, `vi.fn()`)
```typescript
import { describe, it, expect, vi } from "vitest";

interface EmailSender {
    send(to: string, subject: string): Promise<boolean>;
}

async function notifyUser(email: string, sender: EmailSender): Promise<string> {
    if (!email.includes("@")) throw new Error("Invalid email");
    const sent = await sender.send(email, "Welcome");
    return sent ? "SENT" : "FAILED";
}

describe("notifyUser", () => {
    it.each(["invalid-email", "", "user.domain.bg"])(
        "throws error for invalid email: %s",
        async (badEmail) => {
            const mockSender: EmailSender = { send: vi.fn() };
            await expect(notifyUser(badEmail, mockSender)).rejects.toThrow("Invalid email");
            expect(mockSender.send).not.toHaveBeenCalled();
        }
    );

    it("returns SENT when email gateway succeeds", async () => {
        const mockSender: EmailSender = { send: vi.fn().mockResolvedValue(true) };
        const result = await notifyUser("ana@test.bg", mockSender);
        expect(result).toBe("SENT");
        expect(mockSender.send).toHaveBeenCalledWith("ana@test.bg", "Welcome");
    });
});
```

### 4.2 Python (`pytest` — `parametrize`, `pytest.raises` & `MagicMock`)
```python
import pytest
from unittest.mock import MagicMock

def calculate_shipping(weight_kg: float, is_vip: bool) -> float:
    if weight_kg <= 0:
        raise ValueError("Теглото трябва да е положително!")
    if is_vip or weight_kg <= 2.0:
        return 0.0
    return 5.0 + (weight_kg - 2.0) * 1.5

@pytest.mark.parametrize("weight, is_vip, expected", [
    (1.0, False, 0.0),   # Долна граница безплатна доставка
    (2.0, False, 0.0),   # Точна граница 2.0 кг
    (2.1, False, 5.15),  # Над границата
    (10.0, True, 0.0),   # VIP безплатно
])
def test_calculate_shipping_boundaries(weight: float, is_vip: bool, expected: float) -> None:
    assert calculate_shipping(weight, is_vip) == pytest.approx(expected)

def test_calculate_shipping_invalid_weight() -> None:
    with pytest.raises(ValueError, match="положително"):
        calculate_shipping(0.0, False)

def test_payment_gateway_mock() -> None:
    gateway = MagicMock()
    gateway.charge.return_value = True
    assert gateway.charge(100) is True
    gateway.charge.assert_called_once_with(100)
```

---
[⬅️ 01-04. Manual QA & Test Design Techniques](01-04-Manual-QA-And-Test-Design-Techniques.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-06. Modern UI Automation: Playwright & AJAX](01-06-Modern-UI-Automation-Playwright-And-AJAX.md)
