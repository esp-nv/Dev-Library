# 🔄 Справочник: Стар срещу Нов C# Синтаксис (C# 7/8 ➔ C# 12/14)

**Цел:** Бърз превод между класическия синтаксис от лекциите и модерния съкратен запис в .NET 8+

---

## 📊 1. СРАВНИТЕЛНА ТАБЛИЦА НА СИНТАКСИСА

| Какво прави? | Стара версия (Класически C#) | Нова версия (C# 12 / 14+) |
| :--- | :--- | :--- |
| **Създаване на масив / списък** | `int[] arr = new int[] { 1, 2, 3 };`<br>`var list = new List<int> { 1, 2, 3 };` | `int[] arr = [1, 2, 3];`<br>`List<int> list = [1, 2, 3];` |
| **Празен списък или речник** | `var list = new List<string>();`<br>`var dict = new Dictionary<string, int>();` | `List<string> list = [];`<br>`Dictionary<string, int> dict = [];` |
| **Обединяване на колекции** | `var all = a.Concat(b).ToArray();` | `int[] all = [..a, ..b];` *(Spread `..`)* |
| **Проверка за празна колекция** | `if (list != null && list.Count == 0)` | `if (list is [])` |
| **Последен елемент от масив** | `int last = arr[arr.Length - 1];` | `int last = arr[^1];` *(Hat `^`)* |
| **Отрязък от масив (Slice)** | `arr.Skip(1).Take(3).ToArray();` | `int[] sub = arr[1..4];` *(Range `..`)* |
| **Проверка на тип и кастване** | `if (obj is Student)`<br>`{ var s = (Student)obj; ... }` | `if (obj is Student s)`<br>`{ ... }` |
| **Условно присвояване при `null`** | `if (user != null) user.Name = "Ivan";` | `user?.Name = "Ivan";` *(C# 14)* |
| **Конструктор на клас** | `public class User {`<br>`  public string Name { get; }`<br>`  public User(string name) => Name = name;`<br>`}` | `public class User(string name) {`<br>`  public string Name { get; } = name;`<br>`}` *(Primary Constructor)* |
| **Свойство с валидация** | `private string _name;`<br>`public string Name { get => _name; set => _name = value.Trim(); }` | `public string Name { get; set => field = value.Trim(); }` *(C# 14 `field`)* |
| **Затваряне на поток (`using`)** | `using (var r = new StreamReader(p))`<br>`{ ... }` | `using var r = new StreamReader(p);` *(без къдрави скоби)* |
| **Switch връщащ стойност** | `switch (x) { case 1: res = "A"; break; ... }` | `string res = x switch { 1 => "A", _ => "B" };` |

---

## 💻 2. ПРИМЕР НА ЖИВО: ЕДИН И СЪЩ КОД НА ДВАТА СТИЛА

### 🕰️ Класически стил (Standard / SoftUni Judge)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class ClassicExample
{
    public void ProcessData(object input)
    {
        List<int> first = new List<int> { 10, 20, 30 };
        List<int> second = new List<int> { 40, 50 };

        // Обединяване и взимане на последния елемент
        List<int> combined = new List<int>(first);
        combined.AddRange(second);
        int lastItem = combined[combined.Count - 1];

        // Проверка на тип и кастване
        if (input is string)
        {
            string text = (string)input;
            if (text.Length > 0)
            {
                Console.WriteLine(\$"{text}: {lastItem}");
            }
        }
    }
}
```

### ⚡ Модерен стил (C# 12 – 14 / .NET 8+)
```csharp
using System;
using System.Collections.Generic;

public class ModernExample
{
    public void ProcessData(object? input)
    {
        List<int> first =;
        List<int> second =;

        // Обединяване със Spread (..) и взимане на последния елемент с (^1)
        List<int> combined = [..first, ..second];
        int lastItem = combined[^1];

        // Pattern Matching с директна проверка на дължината
        if (input is string { Length: > 0 } text)
        {
            Console.WriteLine(\$"{text}: {lastItem}");
        }
    }
}
```

---

## 💡 КОЙ СТИЛ ДА ИЗПОЛЗВАШ?

* **За изпити в SoftUni / Judge:** Придържай се към **Класическия стил** (или базовите функции на C# 10/12), защото някои по-стари компилатори в Judge не поддържат най-новите C# 14 конструкции (като `field` или `?.=`).
* **За лични проекти и четене на чужд код:** Познаването на **Модерния стил** ти позволява да четеш без проблем съвременни GitHub репозиторита и официалната документация на Microsoft.
