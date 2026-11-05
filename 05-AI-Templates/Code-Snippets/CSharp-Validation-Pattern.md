### 📄 Шаблон 2: `Code-Snippets/CSharp-Validation-Pattern.md`
*Готов шаблон за ООП капсулация и валидация на данни. Всеки път, когато правиш нов клас (Потребител, Продукт, Калкулатор) и искаш да спреш лоши входящи данни.*


### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го като база за всеки нов Property в твоите C# класове.
- Модифицирай: Името на променливата, типа (int, string) и условието в `if` проверката.

### 🛠️ ГОТОВ КОД (C# OOP)
```csharp
using System;

public class UserAccount
{
    private string username;
    private int age;

    // Валидация на Текст (напр. да не е празен)
    public string Username
    {
        get { return username; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Потребителското име не може да бъде празно!");
            }
            username = value.Trim(); // Изчиства излишни интервали
        }
    }

    // Валидация на Числа (напр. за граници)
    public int Age
    {
        get { return age; }
        set
        {
            if (value < 18 || value > 100)
            {
                throw new ArgumentOutOfRangeException("Възрастта трябва да бъде между 18 и 100 години.");
            }
            age = value;
        }
    }
}
```
---

