[⬅️ 01-03-00. Unit Testing & Mocking](01-03-00-Multi-Language-Unit-Testing.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-04-00. Selenium WebDriver, DOM Locators & POM](01-04-00-Selenium-WebDriver-And-POM.md)

---

# 01-03-01. NUnit, JUnit 5, Vitest & pytest: Синтаксис от Нулата & Cheat Sheet

**Раздел 02:** Multi-Language Unit Testing | **Цел:** Готови, копируеми шаблони от нулата за писане на Unit тестове, параметризирани тестове, проверка на изключения (Exceptions) и Mocking в четирите основни езика на Ниво 2 (**C#**, **Java**, **TypeScript** и **Python**), плюс обобщена 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. C# (.NET): NUnit + Moq (AAA, TestCase, Exceptions & Verify)](#1-c-net-nunit--moq-aaa-testcase-exceptions--verify)
* [2. Java (21+): JUnit 5 + Mockito (@ParameterizedTest, assertThrows & verify)](#2-java-21-junit-5--mockito-parameterizedtest-assertthrows--verify)
* [3. TypeScript: Vitest (describe, it.each, vi.fn & async/await)](#3-typescript-vitest-describe-iteach-vifn--asyncawait)
* [4. Python (3.x): pytest (Fixtures, parametrize, raises & MagicMock)](#4-python-3x-pytest-fixtures-parametrize-raises--magicmock)
* [5. Обобщение / Бърз Cheat Sheet (4 Езика на 1 Място)](#5-обобщение--бърз-cheat-sheet-4-езика-на-1-място)

---

## 1. C# (.NET): NUNIT + MOQ (AAA, TESTCASE, EXCEPTIONS & VERIFY)

```csharp
using Moq;
using NUnit.Framework;

[TestFixture]
public class PaymentServiceTests
{
    private Mock<IPaymentGateway> _gatewayMock;
    private PaymentService _sut;

    [SetUp]
    public void SetUp()
    {
        _gatewayMock = new Mock<IPaymentGateway>();
        _sut = new PaymentService(_gatewayMock.Object);
    }

    [Test]
    [TestCase(100, 10, 90)]
    [TestCase(200, 25, 150)]
    public void Charge_ValidAmountAndDiscount_ReturnsExpectedNetAmount(decimal amount, decimal discountPercent, decimal expected)
    {
        // Arrange
        _gatewayMock.Setup(g => g.Process(expected)).Returns(true);

        // Act
        var result = _sut.Charge(amount, discountPercent);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
        _gatewayMock.Verify(g => g.Process(expected), Times.Once);
    }

    [Test]
    public void Charge_NegativeAmount_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.Charge(-50m, 10m));
    }
}
```

---

## 2. JAVA (21+): JUNIT 5 + MOCKITO (@PARAMETERIZEDTEST, ASSERTTHROWS & VERIFY)

```java
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class UserServiceTest {

    @Mock
    private UserRepository userRepository;

    @InjectMocks
    private UserService userService;

    @ParameterizedTest
    @CsvSource({
        "elena@dev.bg, true",
        "invalid-email, false"
    })
    void isValidEmail_ShouldValidateFormat(String email, boolean expected) {
        boolean actual = userService.isValidEmail(email);
        assertEquals(expected, actual);
    }

    @Test
    void findUserById_WhenMissing_ShouldThrowException() {
        // Arrange
        when(userRepository.findById(99L)).thenReturn(java.util.Optional.empty());

        // Act & Assert
        assertThrows(UserNotFoundException.class, () -> userService.getUserOrThrow(99L));
        verify(userRepository, times(1)).findById(99L);
    }
}
```

---

## 3. TYPESCRIPT: VITEST (DESCRIBE, IT.EACH, VI.FN & ASYNC/AWAIT)

```typescript
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { OrderService } from './order.service';

describe('OrderService Unit Tests', () => {
    const mockNotifier = { sendEmail: vi.fn() };
    let sut: OrderService;

    beforeEach(() => {
        vi.clearAllMocks();
        sut = new OrderService(mockNotifier);
    });

    it.each([
        { price: 100, qty: 2, expected: 200 },
        { price: 50, qty: 3, expected: 150 },
    ])('calculates total $expected for price=$price and qty=$qty', ({ price, qty, expected }) => {
        expect(sut.calculateTotal(price, qty)).toBe(expected);
    });

    it('throws error and does not send email when cart is empty', async () => {
        await expect(sut.checkout([])).rejects.toThrow('Cart cannot be empty');
        expect(mockNotifier.sendEmail).not.toHaveBeenCalled();
    });
});
```

---

## 4. PYTHON (3.X): PYTEST (FIXTURES, PARAMETRIZE, RAISES & MAGICMOCK)

```python
import pytest
from unittest.mock import MagicMock
from billing_service import BillingService

@pytest.fixture
def billing_setup():
    mock_repo = MagicMock()
    service = BillingService(repo=mock_repo)
    return service, mock_repo

@pytest.mark.parametrize("subtotal, tax_rate, expected", [
    (100.0, 0.20, 120.0),
    (50.0, 0.10, 55.0),
])
def test_calculate_gross_amount(billing_setup, subtotal, tax_rate, expected):
    service, _ = billing_setup
    assert service.calculate_gross(subtotal, tax_rate) == pytest.approx(expected)

def test_save_invoice_calls_repository_once(billing_setup):
    service, mock_repo = billing_setup
    mock_repo.save.return_value = "INV-2026"

    invoice_id = service.create_invoice(amount=250.0)

    assert invoice_id == "INV-2026"
    mock_repo.save.assert_called_once_with(250.0)

def test_create_invoice_zero_amount_raises_value_error(billing_setup):
    service, _ = billing_setup
    with pytest.raises(ValueError, match="Amount must be positive"):
        service.create_invoice(amount=0)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET (4 ЕЗИКА НА 1 МЯСТО)

| Операция в теста | C# (NUnit + Moq) | Java (JUnit 5 + Mockito) | TypeScript (Vitest) & Python (pytest) |
| :--- | :--- | :--- | :--- |
| **Маркиране на тест метод** | `[Test]` | `@Test` | **TS:** `it('...', () => {})`<br>**Py:** `def test_...():` |
| **Параметризиран тест** | `[TestCase(a, b)]` | `@ParameterizedTest` + `@CsvSource` | **TS:** `it.each([...])`<br>**Py:** `@pytest.mark.parametrize(...)` |
| **Проверка за равенство** | `Assert.That(act, Is.EqualTo(exp));` | `assertEquals(exp, act);` | **TS:** `expect(act).toBe(exp)`<br>**Py:** `assert act == exp` |
| **Проверка за Exception** | `Assert.Throws<Ex>(() => act());` | `assertThrows(Ex.class, () -> act());` | **TS:** `expect(() => act()).toThrow()`<br>**Py:** `with pytest.raises(Ex):` |
| **Настройка на Mock отговор** | `mock.Setup(m => m.Get()).Returns(x);` | `when(mock.get()).thenReturn(x);` | **TS:** `vi.fn().mockReturnValue(x)`<br>**Py:** `mock.get.return_value = x` |
| **Верификация на извикване** | `mock.Verify(m => m.Send(), Times.Once);` | `verify(mock, times(1)).send();` | **TS:** `expect(fn).toHaveBeenCalledTimes(1)`<br>**Py:** `mock.send.assert_called_once()` |
| **Изпълнение от терминал** | `dotnet test` | `mvn test` или `./gradlew test` | **TS:** `npx vitest run`<br>**Py:** `pytest -v` |

---

[⬅️ 01-03-00. Unit Testing & Mocking](01-03-00-Multi-Language-Unit-Testing.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-04-00. Selenium WebDriver, DOM Locators & POM](01-04-00-Selenium-WebDriver-And-POM.md)

© 2026 Dev-Library / 01-03-01. NUnit, JUnit 5, Vitest & pytest: Синтаксис от Нулата & Cheat Sheet
