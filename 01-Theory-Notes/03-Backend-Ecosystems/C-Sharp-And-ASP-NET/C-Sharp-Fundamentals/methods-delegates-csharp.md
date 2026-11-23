# 📘 Учебник: Методи, Stack Frames, Делегати и Ламбда изрази в C#

**Файл:** `methods-delegates-csharp.md` | **Ниво:** Архитектурно ядро / C# Internals  
**Технологии:** .NET CLR, Call Stack, `ref`/`out`/`in`, `Func`/`Action`, Closures

---

## 🏗️ РАЗДЕЛ 1: КАК МЕТОДИТЕ РАБОТЯТ В ПАМЕТТА (CALL STACK И STACK FRAMES)

Всеки път, когато извикаш метод, .NET заделя нов **Stack Frame** (изолирана кутия на върха на Stack паметта). В нея се записват параметрите, локалните променливи и адресът, на който програмата трябва да се върне след `return`.

```csharp
// ==================== CALL STACK, REF, OUT, IN И PARAMS ====================

using System;

public class MethodsMemoryMechanics
{
    public void ExplainParameterPassing()
    {
        // 1. СТАНДАРТНО ПОДАВАНЕ (Pass by Value)
        int score = 10;
        AddBonusByValue(score);     // Подаваме КОПИЕ на стойността 10 в нов Stack Frame
        Console.WriteLine(score);   // Изход: 10 (оригиналът в текущия Frame не е пипнат)

        // 2. ПОДАВАНЕ ПО РЕФЕРЕНЦИЯ С 'ref' (Двупосочна връзка)
        int health = 50;            // ЗАДЪЛЖИТЕЛНО трябва да има начална стойност преди извикването
        HealPlayer(ref health);     // Подаваме директния АДРЕС в Stack-а на променливата health
        Console.WriteLine(health);  // Изход: 100 (променено директно в оригиналната клетка)

        // 3. ИЗХОДЕН ПАРАМЕТЪР С 'out' (Връщане на повече от една стойност)
        // Не изисква предварителна инициализация (може да се декларира директно в скобите)
        if (TryDivide(10, 2, out double divisionResult))
        {
            Console.WriteLine($"Резултат: {divisionResult}"); // Изход: 5
        }

        // 4. ПАРАМЕТЪР САМО ЗА ЧЕТЕНЕ С 'in' (Оптимизация на паметта)
        int readOnlyId = 999;
        PrintId(in readOnlyId);     // Подава се по референция (без копиране), но компилаторът забранява промяна

        // 5. ПРОМЕНЛИВ БРОЙ АРГУМЕНТИ С 'params'
        int total1 = CalculateSum(1, 2, 3);         // Компилаторът автоматично ги опакова в int
        int total2 = CalculateSum(10, 20, 30, 40);  // Може да подадеш колкото искаш аргументи
    }

    private void AddBonusByValue(int value)
    {
        value += 50; // Променя само локалното копие, което изчезва при '}'
    }

    private void HealPlayer(ref int currentHealth)
    {
        currentHealth += 50; // Бърка директно в клетката на викащия метод
    }

    private bool TryDivide(double a, double b, out double result)
    {
        if (b == 0)
        {
            result = 0;   // Методът с 'out' е ЗАДЪЛЖЕН да присвои стойност преди всеки return!
            return false;
        }

        result = a / b;   // Записваме резултата директно в променливата отвън
        return true;
    }

    private void PrintId(in int id)
    {
        // id = 500; -> КОМПИЛАЦИОННА ГРЕШКА! 'in' параметърът е строго readonly.
        Console.WriteLine($"ID: {id}");
    }

    // 'params' винаги трябва да е ПОСЛЕДНИЯТ параметър в сигнатурата на метода
    private int CalculateSum(params int numbers)
    {
        int sum = 0;
        foreach (int num in numbers) sum += num;
        return sum;
    }
}
```

---

## 🧩 РАЗДЕЛ 2: OVERLOADING И РЕКУРСИЯ (КАПАНЪТ STACK OVERFLOW)

Претоварването (Overloading) позволява един клас да има множество методи с едно име, но различни входни данни. Рекурсията е метод, който извиква сам себе си, създавайки поредица от Stack Frames.

```csharp
// ==================== METHOD OVERLOADING И РЕКУРСИЯ В ПАМЕТТА ====================

public class OverloadingAndRecursionDemo
{
    // 1. ВАЛИДЕН OVERLOADING (Различни типове или брой параметри)
    public int Multiply(int a, int b) => a * b;
    public double Multiply(double a, double b) => a * b;
    public int Multiply(int a, int b, int c) => a * b * c;

    // НЕВАЛИДЕН OVERLOADING (Гърми при компилация!):
    // public double Multiply(int a, int b) => a * b; // Само различен return тип НЕ стига!

    // 2. РЕКУРСИЯ (Всеки рекурсивен вик добавя нов Stack Frame върху предишния)
    public int CalculateFactorial(int n)
    {
        // ДЪНО НА РЕКУРСИЯТА (Base Case) - критично важно!
        // Без него Stack паметта (около 1MB в .NET) се препълва и програмата умира със StackOverflowException!
        if (n <= 1)
        {
            return 1; // Започва разгъването (Unwinding) на Stack Frame-овете назад
        }

        // Рекурсивна стъпка: чака резултата от (n - 1), преди да умножи по n
        return n * CalculateFactorial(n - 1);
    }
}
```

---

## ⚡ РАЗДЕЛ 3: ДЕЛЕГАТИ, FUNC, ACTION И ЛАМБДА ИЗРАЗИ (ФУНКЦИИ КАТО ДАННИ)

Делегатът е **типово безопасен указател към метод** в паметта. Чрез него можем да подаваме алгоритми (поведение) като параметър на други методи – основата на LINQ и Event-Driven програмирането.

```csharp
// ==================== FUNC, ACTION, PREDICATE И CUSTOM DELEGATES ====================

using System;
using System.Collections.Generic;

public class DelegatesAndLambdaDeepDive
{
    // 1. СТАРИЯТ НАЧИН: Дефиниране на собствен тип делегат (указател към метод)
    public delegate int MathOperation(int x, int y);

    public void ExplainDelegatesAndLambdas()
    {
        // Присвояване на Ламбда израз към собствен делегат
        MathOperation add = (a, b) => a + b;
        MathOperation multiply = (a, b) => a * b;

        int sumResult = add(5, 3);        // Изпълнява се като обикновен метод -> 8
        int multResult = multiply(5, 3);  // Изпълнява се като обикновен метод -> 15

        // 2. МОДЕРНИЯТ НАЧИН: Вградени Generic делегати (Няма нужда да пишем 'delegate')

        // ACTION<T>: Приема параметри, НЕ връща нищо (void)
        Action<string, int> logStudent = (name, grade) =>
        {
            Console.WriteLine($"Студент: {name} | Оценка: {grade}");
        };
        logStudent("Иван", 6); // Извикваме Action-а

        // FUNC<T1, T2, TResult>: Последният тип ВИНАГИ е типът на връщания резултат!
        // Тук приема два int-а и връща string
        Func<int, int, string> compareScores = (s1, s2) =>
        {
            if (s1 > s2) return "Първият печели";
            if (s2 > s1) return "Вторият печели";
            return "Равенство";
        };
        string outcome = compareScores(90, 85); // "Първият печели"

        // PREDICATE<T>: Специализиран Func<T, bool> (винаги връща true или false)
        Predicate<int> isPassingGrade = grade => grade >= 3;
        bool passed = isPassingGrade(4); // true

        // 3. ПОДАВАНЕ НА ПОВЕДЕНИЕ КАТО ПАРАМЕТЪР (Higher-Order Functions)
        var numbers = new List<int> { 1, 2, 3, 4, 5, 6 };

        // Подаваме логиката за филтриране директно отвън чрез ламбда израз:
        var evenNumbers = FilterNumbers(numbers, n => n % 2 == 0);
        var bigNumbers = FilterNumbers(numbers, n => n > 4);
    }

    // Метод от по-висок ред: приема данни (List) + логика (Func<int, bool>)
    private List<int> FilterNumbers(List<int> source, Func<int, bool> filterRule)
    {
        var result = new List<int>();
        foreach (var item in source)
        {
            // Изпълняваме подадената отвън ламбда функция за всеки елемент
            if (filterRule(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
}
```

---

## ⚠️ РАЗДЕЛ 4: CLOSURES (ЗАТВАРЯНИЯ) И КАК ЛАМБДА ИЗРАЗИТЕ ПРЕМЕСТВАТ ПАМЕТТА В HEAP

Когато една ламбда функция използва локална променлива от външния метод, компилаторът извършва магия под капака: той **изважда тази променлива от Stack-а и я премества в скрит клас в Heap паметта**, за да може ламбдата да я ползва дори след като външният метод е приключил!

```csharp
// ==================== CLOSURES В ПАМЕТТА И КАПАНЪТ НА FOR ЦИКЪЛА ====================

using System;
using System.Collections.Generic;

public class ClosuresDeepDive
{
    public Func<int> CreateCounter()
    {
        // Тази променлива НЕ умира при приключване на CreateCounter()!
        // Компилаторът я опакова в скрит клас в Heap паметта (Closure),
        // защото ламбда функцията отдолу я "улавя" (captures).
        int count = 0;

        return () =>
        {
            count++;      // Всяко извикване на върнатата функция увеличава същата клетка в Heap-а
            return count;
        };
    }

    public void ExplainLoopClosureTrap()
    {
        // ❌ КАПАН: Улавяне на брояча 'i' директно в цикъл
        var brokenActions = new List<Action>();
        for (int i = 0; i < 3; i++)
        {
            // Всички 3 ламбди сочат към ЕДНА И СЪЩА референция на 'i' в Heap-а!
            brokenActions.Add(() => Console.Write(i + " "));
        }
        // Когато ги изпълним след цикъла, 'i' вече е станало 3 -> Изход: 3 3 3!

        // ✅ РЕШЕНИЕ: Локално копие за всяка итерация
        var fixedActions = new List<Action>();
        for (int i = 0; i < 3; i++)
        {
            int localCopy = i; // Създава се ЧИСТО НОВА променлива в паметта за всяко завъртане
            fixedActions.Add(() => Console.Write(localCopy + " "));
        }
        // Изход: 0 1 2 (Всяка ламбда пази своята собствена копирана стойност!)
    }
}
```

---

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Използвай `out` за безопасни Try-методи** (`bool TryGetStudent(int id, out Student student)`), когато не искаш да хвърляш изключения при липсващ запис.
2. **Помни подредбата на `Func<...>`:** Последният generic параметър **винаги** е типът на връщаната стойност; всички преди него са входните параметри.
3. **Не пиши тежка логика в ламбда изрази:** Ако ламбдата стане повече от 3-4 реда, изнеси я в отделен `private` метод с ясно име и го подай директно по име (Method Group: `.Where(IsValidStudent)`).
4. **Внимавай за Closures в цикли**: Никога не подавай брояча i директно в ламбда, която ще се изпълнява по-късно – винаги го копирай в локална променлива вътре в тялото на цикъла.

---
© 2026 Dev-Library / Пълен учебен модул по Методи, Делегати и Ламбда изрази в C#
