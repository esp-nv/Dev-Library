### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го за създаване на нови xUnit тестове в C#.
- Модифицирай: Името на класа, името на метода и данните в секциите Arrange/Act/Assert.

### 🛠️ ГОТОВ КОД (C# / xUnit)
```csharp
using Xunit;

public class CalculationEngineTests
{
    [Fact] // Използвай [Fact] за единичен тест без параметри
    public void Calculate_ShouldReturnCorrectSum_WhenInputsAreValid()
    {
        // 1. Arrange (Подготовка на данните и обектите)
        var engine = new CalculatorEngine();
        int inputA = 5;
        int inputB = 10;
        int expectedResult = 15;

        // 2. Act (Изпълнение на действието, което тестваме)
        int actualResult = engine.CalculateSum(inputA, inputB);

        // 3. Assert (Проверка дали резултатът е равен на очаквания)
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory] // Използвай [Theory] за тестване на няколко сценария наведнъж
    [InlineData(0, 0, 0)]
    [InlineData(-1, -1, -2)]
    [InlineData(100, 200, 300)]
    public void Calculate_ShouldWorkWithMultipleInputs(int a, int b, int expected)
    {
        var engine = new CalculatorEngine();

        int actual = engine.CalculateSum(a, b);

        Assert.Equal(expected, actual);
    }
}
```

