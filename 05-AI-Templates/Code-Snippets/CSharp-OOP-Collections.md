
### 📄 CSharp-OOP-Collections.md`
*Бърз навигатор за C# колекции. Когато се чудиш дали ти трябва динамичен списък (`List`) или бързо търсене по ключ-стойност (`Dictionary`), отваряш това.*


### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го в ядрото на твоите C# алгоритми (напр. в папка Core/ на Calculation Engine).
- Модифицирай: Типовете данни в колекциите и логиката в циклите.

### 🛠️ ГОТОВ КОД (C# / .NET)
```csharp
using System;
using System.Collections.Generic;

// 1. ДИНАМИЧЕН СПИСЪК (List) - Най-ползваният за подредени данни
List<string> projects = new List<string>();
projects.Add("Universal Engine");
projects.Add("Game Project");

// Бърза проверка и обхождане
if (projects.Contains("Universal Engine"))
{
    foreach (var project in projects)
    {
        Console.WriteLine($"Активен проект: {project}");
    }
}

// 2. РЕЧНИК (Dictionary) - Перфектен за бързо търсене по Ключ -> Стойност
// Пример: Пазим грешки по техния код (Код -> Описание)
Dictionary<int, string> errorLog = new Dictionary<int, string>();
errorLog.Add(404, "Not Found");
errorLog.Add(500, "Internal Server Error");

// Безопасно взимане на стойност без гърмене на кода
int searchCode = 404;
if (errorLog.TryGetValue(searchCode, out string errorMessage))
{
    Console.WriteLine($"Грешка {searchCode}: {errorMessage}");
}
```
