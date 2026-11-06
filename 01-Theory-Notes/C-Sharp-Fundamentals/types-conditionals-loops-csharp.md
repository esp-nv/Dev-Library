# 📘 Учебник: Типове данни, Управление на паметта, Условни конструкции и Цикли в C#

**Файл:** `types-conditionals-loops-csharp.md` | **Ниво:** Архитектурен фундамент / C# Internals  
**Технологии:** .NET CLR, Stack & Heap Memory, Control Flow

---

## 🏗️ РАЗДЕЛ 1: КАК ТИПОВЕТЕ ДАННИ ЖИВЕЯТ В STACK И HEAP ПАМЕТТА

В .NET паметта се дели на две основни зони: **Stack** (бърза, подредена памет за малки и краткотрайни данни) и **Heap** (голяма динамична памет за обекти и текстове). Разбирането на разликата между Value и Reference типове предотвратява скрити бъгове при копиране на данни.

```csharp
// ==================== VALUE VS REFERENCE ТИПОВЕ В ПАМЕТТА ====================

public class MemoryMechanicsDemo
{
    public void ExplainStackAndHeap()
    {
        // 1. VALUE ТИПОВЕ (Живеят директно в STACK паметта)
        int a = 10;                 // Заделя 4 байта в Stack-а със стойност 10
        int b = a;                  // Копира САМАТА СТОЙНОСТ 10 в нова клетка в Stack-а
        b = 99;                     // Променяме само 'b'. Променливата 'a' остава 10!

        // 2. ПРЕЦИЗНОСТ ПРИ ДРОБНИ ЧИСЛА (double срещу decimal)
        double d1 = 0.1 + 0.2;      // В двоична система в паметта се записва като 0.30000000000000004
        decimal m1 = 0.1m + 0.2m;   // В десетична система (16 байта) се записва като ТОЧНО 0.3m

        // 3. REFERENCE ТИПОВЕ (Указателят е в Stack, реалните данни са в HEAP)
        int[] arr1 = { 1, 2, 3 };   // Масивът {1, 2, 3} се създава в Heap; arr1 пази адреса му
        int[] arr2 = arr1;          // Копира се САМО АДРЕСЪТ (референцията), НЕ целият масив!
        arr2[0] = 500;              // Променяме данните в Heap -> arr1[0] СЪЩО става 500!

        // 4. СПЕЦИАЛНИЯТ СЛУЧАЙ: STRING (Reference тип, но IMMUTABLE - непроменяем)
        string text1 = "Hello";     // Създава обект "Hello" в Heap паметта
        string text2 = text1;       // Двете променливи сочат към един и същ адрес в Heap
        text2 = text2 + " World";   // НЕ променя "Hello"! Създава ЧИСТО НОВ обект "Hello World" в Heap
                                    // text1 остава да сочи към стария обект "Hello".
    }
}
```

---

## 🔄 РАЗДЕЛ 2: ПРЕОБРАЗУВАНЕ НА ТИПОВЕ И ЗАЩИТА ОТ ГРЕШКИ (CASTING & PARSING)

Прехвърлянето на данни между различни типове може да стане автоматично (когато няма риск от загуба на данни) или ръчно (когато стесняваме типа).

```csharp
// ==================== CASTING, OVERFLOW И БЕЗОПАСЕН PARSE ====================

public class TypeConversionDemo
{
    public void ExplainConversions()
    {
        // 1. IMPLICIT CASTING (Неявно/Автоматично - от малък към голям тип)
        int smallNumber = 42;           // 4 байта
        long bigNumber = smallNumber;   // 8 байта -> Няма загуба на данни, C# го прави сам

        // 2. EXPLICIT CASTING (Явно/Ръчно - от голям към малък тип)
        double exactPrice = 19.99;
        int roundedPrice = (int)exactPrice; // Орязва дробната част -> става 19 (НЕ закръгля до 20!)

        // 3. КАПАНЪТ НА ЦЕЛОЧИСЛЕНОТО ДЕЛЕНИЕ (Integer Division)
        int totalPoints = 5;
        int count = 2;
        double wrongAverage = totalPoints / count;          // Връща 2.0 (int / int = int)
        double rightAverage = (double)totalPoints / count;  // Връща 2.5 (double / int = double)

        // 4. OVERFLOW (Препълване на паметта)
        byte maxByte = 255;             // Максималната стойност за 1 байт (0 до 255)
        // checked блокът кара програмата да гръмне с OverflowException вместо да превърти до 0
        checked
        {
            // maxByte++; // Ако се разкоментира, ще хвърли OverflowException
        }

        // 5. ПАРСВАНЕ НА ТЕКСТ (Parse срещу TryParse)
        string userInput = "123a";
        // int risky = int.Parse(userInput); // ГЪРМИ с FormatException!

        // Безопасният професионален начин (не хвърля изключение, връща true/false):
        if (int.TryParse(userInput, out int parsedValue))
        {
            Console.WriteLine($"Успешно число: {parsedValue}");
        }
        else
        {
            Console.WriteLine($"Невалиден вход! parsedValue получава default стойност: {parsedValue}"); // 0
        }
    }
}
```

---

## 🧠 РАЗДЕЛ 3: УСЛОВНИ КОНСТРУКЦИИ И SHORT-CIRCUIT ОПТИМИЗАЦИЯ

Условните конструкции управляват кой блок код ще влезе в процесора. Познаването на Short-Circuit правилата и модерния Pattern Matching прави кода по-бърз и защитен от `NullReferenceException`.

```csharp
// ==================== IF-ELSE, GUARD CLAUSES И SWITCH PATTERN MATCHING ====================

public class ConditionalsDeepDive
{
    public void ExplainShortCircuitAndGuards(string studentName, double grade)
    {
        // 1. SHORT-CIRCUIT EVALUATION (Защита от NullReferenceException)
        // При оператор &&, ако първото условие (studentName != null) е false,
        // C# ИЗОБЩО НЕ ИЗПЪЛНЯВА втората част (.Length > 2) и програмата НЕ гърми!
        if (studentName != null && studentName.Length > 2)
        {
            Console.WriteLine($"Валидно име: {studentName}");
        }

        // 2. GUARD CLAUSES (Избягване на дълбоко влагане / Arrow Anti-Pattern)
        // Вместо да влагаме 4 нива на if { if { if { ... } } }, проверяваме грешките първи и излизаме:
        if (string.IsNullOrWhiteSpace(studentName)) return; // Ранен изход при невалидно име
        if (grade < 2.00 || grade > 6.00) return;           // Ранен изход при невалидна оценка

        // Тук кодът вече е на 1-во ниво (плосък и четим) и знаем, че данните са 100% валидни!
    }

    public string ClassifyCharacterAndGrade(char category, double grade)
    {
        // 3. КЛАСИЧЕСКИ SWITCH-CASE (Когато изпълняваме команди / действия)
        switch (category)
        {
            case 'A':
            case 'B': // Fall-through (Логическо ИЛИ)
                Console.WriteLine("Приоритетна категория");
                break;

            case 'C' when grade >= 5.50: // Комбинация с 'when' (Логическо И)
                Console.WriteLine("Категория C с отличен успех");
                break;

            default:
                Console.WriteLine("Стандартна категория");
                break;
        }

        // 4. МОДЕРЕН SWITCH EXPRESSION (Когато връщаме директно стойност / резултат)
        // Използва думите 'and', 'or', 'not' и релационни оператори (>=, <)
        return grade switch
        {
            >= 5.50 and <= 6.00 => "Отличен (6)",
            >= 4.50 and < 5.50  => "Много добър (5)",
            >= 3.50 and < 4.50  => "Добър (4)",
            >= 3.00 and < 3.50  => "Среден (3)",
            >= 2.00 and < 3.00  => "Слаб (2)",
            _                   => "Невалидна оценка" // Underscore (_) е default
        };
    }
}
```

---

## 🔄 РАЗДЕЛ 4: ЦИКЛИ, ВЛОЖЕНИ ЦИКЛИ И УПРАВЛЕНИЕ НА ПОТОКА

Циклите повтарят операции в паметта. Най-честите грешки тук са безкрайните цикли (забравена стъпка), грешки с 1 единица в индексацията (*Off-by-one error*) и модифициране на колекция по време на `foreach`.

```csharp
// ==================== ВСИЧКИ ВИДОВЕ ЦИКЛИ И NESTED LOOPS В ACTION ====================

using System;
using System.Collections.Generic;

public class LoopsDeepDive
{
    public void ExplainLoopsAndFlowControl()
    {
        // 1. FOR ЦИКЪЛ (Обратно броене и прескачане на стъпки)
        // Внимание за Off-by-one: започваме от Length - 1 и въртим до >= 0
        int[] numbers = { 10, 20, 30, 40, 50 };
        for (int i = numbers.Length - 1; i >= 0; i--)
        {
            if (numbers[i] == 30)
            {
                continue; // Прескача 30, но продължава към 20 и 10
            }
            Console.Write(numbers[i] + " "); // Изход: 50 40 20 10
        }

        // 2. WHILE ЦИКЪЛ (Четене на поток от команди до стоп-дума)
        string input = "Start";
        int step = 0;
        while (input != "End")
        {
            step++;
            if (step == 3)
            {
                input = "End"; // Променяме условието, за да избегнем безкраен цикъл
            }
        }

        // 3. DO-WHILE ЦИКЪЛ (Гарантирано поне 1 изпълнение)
        int attempts = 0;
        do
        {
            attempts++; // Изпълнява се веднъж, въпреки че условието долу е false
        } while (attempts < 0);

        // 4. FOREACH ЦИКЪЛ (Използва IEnumerator под капака)
        var students = new List<string> { "Иван", "Мария" };
        foreach (string student in students)
        {
            Console.WriteLine(student);
            // students.Add("Петър"); // ГРЕШКА В RUNTIME: InvalidOperationException: Collection was modified!
        }
    }

    public void ExplainNestedLoopsAndExitStrategies()
    {
        // 5. ВЛОЖЕНИ ЦИКЛИ (Nested Loops) - Търсене в 2D матрица
        int[,] matrix =
        {
            { 1, 2, 3 },
            { 4, 9, 6 },
            { 7, 8, 5 }
        };

        int target = 9;
        bool isFound = false; // Флаг за излизане от ВСИЧКИ вложени цикли едновременно

        // Външният цикъл обхожда редовете (GetLength(0) връща броя редове)
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            // Вътрешният цикъл обхожда колоните на текущия ред (GetLength(1) връща броя колони)
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (matrix[row, col] == target)
                {
                    Console.WriteLine($"Намерено на позиция [{row}, {col}]");
                    isFound = true;
                    break; // Прекъсва САМО вътрешния цикъл (по колоните)!
                }
            }

            // Проверяваме флага, за да прекратим и външния цикъл (по редовете)
            if (isFound)
            {
                break; // Сега вече излизаме напълно от конструкцията
            }
        }
    }
}
```

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Винаги използвай decimal за пари и финансови сметки**, а double за физични/научни изчисления и среден успех.
2. **Внимавай с int / int**: Винаги каствай поне единия операнд към (double) или (decimal), ако очакваш дробен резултат.
3. **Защитавай входа с TryParse**: Никога не се доверявай на суров потребителски текст в конзолата без проверка.
4. **Дръж условията плоски**: Използвай Guard Clauses (if (...) return;) вместо дълбоко вложени if-else дървета.
5. **Помни обхвата на break**: При вложени цикли break излиза само от най-вътрешния цикъл; за пълен изход ползвай return (в метод) или bool флаг.

   ---
© 2026 Dev-Library / Пълен учебен модул по C# Основи и Управление на потока
---

