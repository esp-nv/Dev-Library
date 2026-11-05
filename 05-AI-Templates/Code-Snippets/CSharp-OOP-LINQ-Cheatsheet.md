
### 📄 CSharp-OOP-LINQ-Cheatsheet.md
*LINQ е магията на C#, която ти позволява да филтрираш, сортираш и трансформираш колекции с по 1 ред код, вместо да пишеш дълги `foreach` цикли.*


### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го навсякъде, където имаш `List` или масив и искаш да извлечеш специфични данни от него.
- Модифицирай: Ламбда изразите (`x => x...`) според твоите критерии.

### 🛠️ ГОТОВ КОД (C# / LINQ)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

List<int> numbers = new List<int> { 5, 12, 3, 8, 25, 40, 11 };

// 1. ФИЛТРИРАНЕ (Вземи само числата по-големи от 10)
List<int> bigNumbers = numbers.Where(n => n > 10).ToList(); // Резултат: 12, 25, 40, 11

// 2. СОРТИРАНЕ (Сортирай ги по големина)
List<int> sortedNumbers = numbers.OrderBy(n => n).ToList();

// 3. ТЪРСЕНЕ НА ЕДИНИЧЕН ЕЛЕМЕНТ (Вземи първото число, което се дели на 5)
// FirstOrDefault връща 0, ако нищо не отговаря на условието, вместо да хвърли грешка
int firstMatch = numbers.FirstOrDefault(n => n % 5 == 0); // Резултат: 5

// 4. ТРАНСФОРМАЦИЯ (Умножи всяко число по 2)
List<int> doubledNumbers = numbers.Select(n => n * 2).ToList();
```
