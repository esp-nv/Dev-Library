# 📘 Учебник: Текстообработка, String Immutability, StringBuilder и Span<char> в C#

**Файл:** `string-text-processing-csharp.md` | **Ниво:** Архитектурно ядро / C# Internals  
**Технологии:** .NET CLR, Heap Allocations, String Intern Pool, `StringBuilder`, `ReadOnlySpan<char>`

---

## 🏗️ РАЗДЕЛ 1: КАК STRING ЖИВЕЕ В HEAP ПАМЕТТА (IMMUTABILITY И STRING POOL)

В .NET `string` е Reference тип, но се държи като Value тип при промяна, защото е **Immutable (непроменяем)**. Веднъж записани в паметта, символите на един стринг не могат да бъдат редактирани. Всяка манипулация генерира нов обект в Heap-а.

```csharp
// ==================== IMMUTABILITY И STRING INTERN POOL В ПАМЕТТА ====================

using System;

public class StringMemoryMechanics
{
    public void ExplainImmutabilityAndInterning()
    {
        // 1. STRING INTERN POOL (Оптимизация на литерали)
        // Когато компилаторът види еднакви низове в кавички, той ги записва САМО ВЕДНЪЖ в специална таблица
        string a = "SoftUni";
        string b = "SoftUni";

        // Object.ReferenceEquals проверява дали двете променливи сочат към ЕДИН И СЪЩ адрес в RAM!
        bool areSameAddress = Object.ReferenceEquals(a, b); // Връща TRUE!

        // 2. ДИНАМИЧНО СЪЗДАДЕНИ СТРИНГОВЕ (Извън Pool-а по подразбиране)
        string dynamicStr = new string(new[] { 'S', 'o', 'f', 't', 'U', 'n', 'i' });
        bool areDynamicSame = Object.ReferenceEquals(a, dynamicStr); // Връща FALSE (различни адреси в Heap-а)

        // Но операторът '==' сравнява СТОЙНОСТТА (символ по символ), а не адреса:
        bool areValuesEqual = (a == dynamicStr); // Връща TRUE!

        // 3. КАКВО СТАВА ПРИ ПРОМЯНА (IMMUTABILITY)
        string original = "Hello";
        string copy = original; // И двете сочат към "Hello"

        // Следващият ред НЕ променя "Hello"! Той заделя НОВА памет за "Hello!" и мести указателя 'original':
        original += "!";

        Console.WriteLine(original); // "Hello!" (новият обект)
        Console.WriteLine(copy);     // "Hello"  (старият обект остава непокътнат)
    }
}
```

---

## 🚀 РАЗДЕЛ 2: STRINGBUILDER ПОД КАПАКА (КАПАЦИТЕТ И РАЗШИРЯВАНЕ)

`StringBuilder` е динамичен буфер от символи (подобен на `List<char>`). Той заделя блок памет предварително (по подразбиране **16 символа** капацитет) и пише директно в него. Когато капацитетът се запълни, той го удвоява автоматично.

```csharp
// ==================== STRINGBUILDER КАПАЦИТЕТ И ОПЕРАЦИИ НА МЯСТО ====================

using System;
using System.Text;

public class StringBuilderDeepDive
{
    public void ExplainBufferAndPerformance()
    {
        // 1. ПРЕДВАРИТЕЛНО ЗАДЕЛЯНЕ НА КАПАЦИТЕТ (Най-добра практика!)
        // Ако знаем, че ще сглобяваме около 500 символа, задаваме капацитета веднага,
        // за да избегнем междинните преоразмерявания (16 -> 32 -> 64 -> 128 -> 256 -> 512)
        var sb = new StringBuilder(capacity: 500);

        // 2. ДОБАВЯНЕ И ВМЪКВАНЕ (Без създаване на нови string обекти)
        sb.Append("Студент: ");
        sb.Append("Иван Иванов");
        sb.AppendLine(" | Статус: Активен"); // Добавя текста + символ за нов ред (\(\r\\)n)

        // Вмъкване в самото начало (измества съществуващите символи надясно в буфера)
        sb.Insert(0, "[2026] ");

        // 3. РЕДАКТИРАНЕ И ИЗТРИВАНЕ НА МЯСТО
        sb.Replace("Иван Иванов", "Мария Петрова"); // Заменя директно в масива
        sb.Remove(0, 7);                            // Изтрива "[2026] " (от индекс 0, 7 символа)

        // 4. ДОСТЪП ПО ИНДЕКС (Като при масив)
        sb[0] = 'с'; // Променя първата буква директно в паметта (при string това е невъзможно!)

        // 5. ФИНАЛИЗИРАНЕ (Единствената алокация на string за целия процес)
        string finalOutput = sb.ToString();

        // 6. ИЗЧИСТВАНЕ ЗА ПРЕИЗПОЛЗВАНЕ (Запазва заделения капацитет в паметта)
        sb.Clear();
    }
}
```

---

## 🛠️ РАЗДЕЛ 3: МЕТОДИ ЗА ПАРСВАНЕ, ТЪРСЕНЕ И ФОРМАТИРАНЕ

Тук са събрани най-често използваните операции при обработка на входни данни, логове и текстови команди в изпитните задачи.

```csharp
// ==================== ПАРСВАНЕ, SPLIT, JOIN, SUBSTRING И СРАВНЕНИЕ ====================

using System;

public class StringOperationsDeepDive
{
    public void ExplainParsingAndSearching()
    {
        string rawInput = "   C#; Java;   Python; JavaScript   ";

        // 1. ПОЧИСТВАНЕ И РАЗДЕЛЯНЕ (Trim + Split с премахване на празни елементи)
        // StringSplitOptions.TrimEntries (C# 10+) автоматично чисти интервалите около всеки елемент!
        string[] languages = rawInput.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        // Резултат: ["C#", "Java", "Python", "JavaScript"]

        // 2. СЪЕДИНЯВАНЕ НА МАСИВ В ЕДИН СТРИНГ (Оптимизирано под капака)
        string formattedList = string.Join(" -> ", languages);
        // Резултат: "C# -> Java -> Python -> JavaScript"

        // 3. ТЪРСЕНЕ И ИЗРЯЗВАНЕ (IndexOf + Substring)
        string log = "ERROR [Code: 404]: Page not found";
        int startIndex = log.IndexOf('[') + 1; // Намира индекса след '['
        int endIndex = log.IndexOf(']');       // Намира индекса на ']'
        int length = endIndex - startIndex;    // Изчислява точната дължина на парчето

        if (startIndex > 0 && length > 0)
        {
            string extractedCode = log.Substring(startIndex, length); // "Code: 404"
        }

        // 4. СРАВНЕНИЕ БЕЗ ЗНАЧЕНИЕ НА ГЛАВНИ/МАЛКИ БУКВИ (Case-Insensitive)
        // Избягвай input.ToLower() == "admin", защото ToLower() създава нов string в Heap-а!
        // Използвай StringComparison.OrdinalIgnoreCase (нула алокации на памет):
        string role = "AdMiN";
        bool isAdmin = role.Equals("admin", StringComparison.OrdinalIgnoreCase); // TRUE!
    }
}
```

---

## ⚡ РАЗДЕЛ 4: МОДЕРНА ОПТИМИЗАЦИЯ С `ReadOnlySpan<char>` (ZERO-ALLOCATION)

Когато използваш `.Substring()`, .NET заделя нова памет в Heap-а за изрязания текст. Модерният C# предлага **`ReadOnlySpan<char>`** — структура в Stack паметта, която просто "сочи" към определена част от оригиналния стринг, без да копира нито един байт!

```csharp
// ==================== SUBSTRING VS READONLYSPAN<CHAR> В ПАМЕТТА ====================

using System;

public class SpanOptimizationDemo
{
    public void ExplainZeroAllocationSlicing()
    {
        string data = "ID:10542|Name:Georgi|Score:98.5";

        // ❌ КЛАСИЧЕСКИ НАЧИН (Създава нов string в Heap-а за всяко изрязване)
        string idSubstring = data.Substring(3, 5); // Алокира "10542" в Heap-а

        // ✅ МОДЕРЕН НАЧИН С SPAN (Работи директно в Stack-а с НУЛА алокации в Heap-а)
        ReadOnlySpan<char> dataSpan = data.AsSpan();
        ReadOnlySpan<char> idSlice = dataSpan.Slice(3, 5); // Просто прозорец върху символите "10542"

        // Много типове в .NET (int, double, DateTime) приемат директно Span в .TryParse():
        if (int.TryParse(idSlice, out int parsedId))
        {
            Console.WriteLine(\$"Парснато без нова памет: {parsedId}"); // 10542
        }
    }
}
```

---

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Никога не използвай `+=` за натрупване на текст в цикъл** — винаги посягай към `StringBuilder`.
2. **Винаги присвоявай резултата от методите на `string`** (`text = text.Trim();`), защото оригиналът никога не се променя.
3. **За сравнение без разлика в регистъра (малки/главни букви)** предпочитай `string.Equals(a, b, StringComparison.OrdinalIgnoreCase)` пред `.ToLower() == .ToLower()`.
4. **Използвай `string.IsNullOrWhiteSpace(str)`** вместо `str == null || str == ""`, за да хванеш и низове, съдържащи само интервали `"   "`.
5. **При четене на символи по индекс** (`text[i]`) не можеш да пишеш `text[i] = 'a'`. За редакция по индекс превърни в `char[]` (`text.ToCharArray()`) или използвай `StringBuilder`.

---
© 2026 Dev-Library / Пълен учебен модул по Текстообработка и Управление на паметта в C#
