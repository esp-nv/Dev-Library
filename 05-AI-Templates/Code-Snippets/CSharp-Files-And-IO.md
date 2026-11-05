### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай този код, когато трябва да работиш с външни файлове в C#.
- Модифицирай: Пътя до файла (`filePath`) и логиката вътре в цикъла.

### 🛠️ ГОТОВ КОД (C# .NET)
```csharp
using System;
using System.IO;

string filePath = "history.txt"; // За преносими приложения (Portable) стои в същата папка

// 1. ЗАПИСВАНЕ ВЪВ ФАЙЛ (Добавяне на нов ред без триене на старите)
try
{
    using (StreamWriter writer = new StreamWriter(filePath, append: true))
    {
        writer.WriteLine($"[{DateTime.Now}] Изчислението приключи успешно.");
    }
}
catch (IOException ex)
{
    Console.WriteLine($"Грешка при запис: {ex.Message}");
}

// 2. ЧЕТЕНЕ ОТ ФАЙЛ (Ред по ред - щади паметта)
if (File.Exists(filePath))
{
    try
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Твоята модификация: Какво правиш с всеки прочетен ред
                Console.WriteLine(line); 
            }
        }
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Грешка при четене: {ex.Message}");
    }
}
else
{
    Console.WriteLine("Файлът все още не съществува.");
}
```
---


