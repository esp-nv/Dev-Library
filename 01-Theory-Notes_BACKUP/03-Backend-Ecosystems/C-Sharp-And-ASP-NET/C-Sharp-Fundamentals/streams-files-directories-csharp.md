# 📘 Учебник: Потоци (Streams / I/O), Буфери в RAM, Файлове и Директории в C#

**Файл:** `streams-files-directories-csharp.md` | **Ниво:** Архитектурно ядро / C# Internals  
**Технологии:** .NET `System.IO`, `FileStream`, `StreamReader`/`StreamWriter`, `IDisposable`, Byte Buffers

---

## 🏗️ РАЗДЕЛ 1: КАК ПОТОЦИТЕ И БУФЕРИТЕ РАБОТЯТ ПОД КАПАКА В ПАМЕТТА

Когато четеш или пишеш върху твърдия диск, директното обръщение към диска за всеки отделен байт е хиляди пъти по-бавно от работата с RAM. Затова .NET използва **буфериране**: заделя малък масив `byte[]` в паметта (обикновено 4 KB), пълни го наведнъж от диска и подава данните на програмата.

```csharp
// ==================== FILESTREAM И РЪЧЕН БАЙТОВ БУФЕР В ПАМЕТТА ====================

using System;
using System.IO;
using System.Text;

public class StreamBuffersMechanics
{
    public void CopyFileWithByteBuffer(string sourcePath, string destinationPath)
    {
        // 1. ОТВАРЯМЕ ДВА ПОТОКА С 'using', ЗА ДА ГАРАНТИРАМЕ ЗАТВАРЯНЕТО ИМ В ОС
        // FileMode.Open изисква файлът да съществува; FileAccess.Read го отваря само за четене
        using var inputStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);

        // FileMode.Create създава нов файл или презаписва съществуващ от 0 байта
        using var outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);

        // 2. ЗАДЕЛЯМЕ БУФЕР ОТ 4 KB (4096 байта) В HEAP ПАМЕТТА
        // Дори файлът да е 10 GB, програмата ще използва само тези 4 KB RAM!
        byte[] buffer = new byte[4096];
        int bytesRead;

        // 3. ЧЕТЕМ НА ПОРЦИИ, ДОКАТО ПОТОКЪТ ВЪРНЕ 0 (Край на файла - EOF)
        while ((bytesRead = inputStream.Read(buffer, offset: 0, count: buffer.Length)) > 0)
        {
            // ВАЖНО: Записваме само 'bytesRead' на брой байтове, а не целия buffer.Length!
            // При последното четене буферът може да е запълнен само наполовина.
            outputStream.Write(buffer, offset: 0, count: bytesRead);
        }
        // При излизане от метода, .Dispose() извиква .Flush() и освобождава файловете в Windows/Linux
    }
}
```

---

## 📖 РАЗДЕЛ 2: ТЕКСТОВИ ПОТОЦИ (STREAMREADER, STREAMWRITER И КОДИРАНЕ)

`StreamReader` и `StreamWriter` са обвивки (Decorators) около `FileStream`. Те автоматично превеждат суровите байтове (`byte[]`) в четими символи (`char` / `string`) според зададеното кодиране (UTF-8).

```csharp
// ==================== STREAMREADER, STREAMWRITER И UTF-8 ENCODING ====================

using System;
using System.IO;
using System.Text;

public class TextStreamsDeepDive
{
    public void ProcessLargeLogFile(string inputPath, string outputPath)
    {
        // 1. ИНИЦИАЛИЗИРАМЕ READER С ИЗРИЧЕН UTF-8 ЗА КИРИЛИЦА
        using var reader = new StreamReader(inputPath, Encoding.UTF8);

        // 2. ИНИЦИАЛИЗИРАМЕ WRITER (append: false презаписва файла; append: true добавя в края)
        using var writer = new StreamWriter(outputPath, append: false, Encoding.UTF8);

        int lineNumber = 1;
        string currentLine;

        // 3. ЧЕТЕМ РЕД ПО РЕД (В паметта стои само по 1 ред в даден момент)
        while ((currentLine = reader.ReadLine()) != null)
        {
            if (currentLine.Contains("ERROR"))
            {
                // Записваме в буфера на StreamWriter
                writer.WriteLine(\$"[Ред {lineNumber}]: {currentLine}");
            }

            lineNumber++;
        }

        // По желание: writer.Flush() принудително изпразва буфера към диска веднага,
        // но 'using' ще го направи автоматично на затварящата скоба '}' на метода.
    }
}
```

---

## 📂 РАЗДЕЛ 3: УПРАВЛЕНИЕ НА ФАЙЛОВЕ, ДИРЕКТОРИИ И ПЪТИЩА (SYSTEM.IO)

Тук сравняваме бързите статични помощници (`File`, `Directory`) с обектно-ориентираните класове (`FileInfo`, `DirectoryInfo`), както и безопасната работа с пътища.

```csharp
// ==================== FILE, DIRECTORY, FILEINFO И PATH.COMBINE ====================

using System;
using System.IO;

public class FileSystemManagementDemo
{
    public void ManageFilesAndFolders()
    {
        // 1. БЕЗОПАСНО СГЛОБЯВАНЕ НА ПЪТИЩА (Работи еднакво на Windows, Linux и macOS)
        string baseFolder = "DataStorage";
        string subFolder = "Reports_2026";
        string fileName = "students_export.csv";

        string fullDirectoryPath = Path.Combine(baseFolder, subFolder);
        string fullFilePath = Path.Combine(fullDirectoryPath, fileName);

        // 2. СЪЗДАВАНЕ НА ДИРЕКТОРИЯ (Създава всички липсващи папки по веригата без грешка)
        Directory.CreateDirectory(fullDirectoryPath);

        // 3. БЪРЗ ЗАПИС И ЧЕТЕНЕ ЗА МАЛКИ ФАЙЛОВЕ (До няколко мегабайта)
        File.WriteAllText(fullFilePath, "Id,Name,Grade\n1,Иван,6.00\n2,Мария,5.80", Encoding.UTF8);

        if (File.Exists(fullFilePath))
        {
            string[] allLines = File.ReadAllLines(fullFilePath, Encoding.UTF8);
            Console.WriteLine(\$"Общо редове: {allLines.Length}");
        }

        // 4. ИНСТАНЦИОННИ КЛАСОВЕ (FileInfo и DirectoryInfo) - За анализ на метаданни
        var fileInfo = new FileInfo(fullFilePath);
        long sizeInBytes = fileInfo.Length;                 // Размер на файла в байтове
        string extension = fileInfo.Extension;              // ".csv"
        string nameOnly = Path.GetFileNameWithoutExtension(fullFilePath); // "students_export"

        // 5. ОБХОЖДАНЕ НА ВСИЧКИ ФАЙЛОВЕ В ПАПКА И ПОДПАПКИ
        var dirInfo = new DirectoryInfo(baseFolder);
        // SearchOption.AllDirectories търси рекурсивно във всички вътрешни папки
        FileInfo[] csvFiles = dirInfo.GetFiles("*.csv", SearchOption.AllDirectories);

        foreach (FileInfo file in csvFiles)
        {
            Console.WriteLine(\$"Намерен файл: {file.FullName} ({file.Length} B)");
        }
    }
}
```

---

## 🧠 РАЗДЕЛ 4: MEMORYSTREAM И IDISPOSABLE ПОД КАПАКА

`MemoryStream` работи изцяло в RAM паметта (без да докосва твърдия диск), а конструкцията `using` се превежда от компилатора в стандартен `try-finally` блок.

```csharp
// ==================== MEMORYSTREAM И КАК КОМПИЛАТОРЪТ ПРЕВЕЖДА 'USING' ====================

using System;
using System.IO;
using System.Text;

public class MemoryStreamAndDisposableDemo
{
    public byte[] CreateInMemoryData(string content)
    {
        // MemoryStream пази данните в динамичен byte[] масив в Heap паметта
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

        writer.Write(content);
        writer.Flush(); // ЗАДЪЛЖИТЕЛНО преди ToArray(), за да прехвърлим текста от writer в memoryStream!

        // Връщаме суровите байтове (готови напр. за изпращане по мрежа или сваляне в браузър)
        return memoryStream.ToArray();
    }

    public void HowCompilerTranslatesUsing(string path)
    {
        // Този код:
        // using (var reader = new StreamReader(path)) { ... }
        // Се компилира от C# точно в следния try-finally блок:

        StreamReader reader = new StreamReader(path);
        try
        {
            string line = reader.ReadLine();
        }
        finally
        {
            // Гарантирано се изпълнява ВИНАГИ, дори при грешка в try блока!
            if (reader != null)
            {
                ((IDisposable)reader).Dispose();
            }
        }
    }
}
```

---

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Никога не пиши наклонени черти на ръка в пътища:** Използвай `Path.Combine("dir", "file.txt")` за съвместимост между операционните системи.
2. **Винаги обвивай потоците в `using`:** Без `using` или `.Dispose()`, `StreamWriter` може да не запише последните данни от буфера, а файлът ще остане заключен.
3. **Избирай правилния инструмент според размера:** За малки конфигурационни файлове ползвай `File.ReadAllLines()`, а за големи файлове/логове — `StreamReader` с `while ((line = reader.ReadLine()) != null)`.
4. **При `FileStream.Read` винаги ползвай върнатия брой прочетени байтове (`bytesRead`):** Никога не приемай, че буферът се е напълнил на 100% до края на файла.
5. **Задавай изрично `Encoding.UTF8`**, когато четеш или пишеш текстови файлове на кирилица.

---
© 2026 Dev-Library / Пълен учебен модул по Потоци, Файлове и Директории в C#
