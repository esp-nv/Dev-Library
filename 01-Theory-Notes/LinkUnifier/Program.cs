using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

var currentDir = Directory.GetCurrentDirectory();
var rootDir = Directory.Exists(Path.Combine(currentDir, "01-Theory-Notes"))
    ? Path.Combine(currentDir, "01-Theory-Notes")
    : Path.GetFullPath(Path.Combine(currentDir, "..", "01-Theory-Notes"));

if (!Directory.Exists(rootDir))
{
    Console.WriteLine($"Не е намерена папка 01-Theory-Notes в: {rootDir}");
    return;
}

var utf8NoBom = new UTF8Encoding(false);
var allMdFiles = Directory.GetFiles(rootDir, "*.md", SearchOption.AllDirectories);

// Индексираме всички файлове по име (без README.md) за бързо търсене при преместване
var filesByName = allMdFiles
    .Where(f => !Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase))
    .GroupBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

int updatedFilesCount = 0;
int autoFixedLinksCount = 0;
var remainingBrokenLinks = new List<string>();

// Regex за Markdown линкове [текст](цел), без изображения ![...]
var linkRegex = new Regex(@"(?<!!)\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

foreach (var file in allMdFiles)
{
    string original = File.ReadAllText(file, Encoding.UTF8);
    string content = original;
    var fileDir = Path.GetDirectoryName(file)!;
    string relCurrentFile = Path.GetRelativePath(rootDir, file).Replace('\\', '/');

    // 1. Унифициране на иконките
    content = Regex.Replace(content, @"\[[^\]]*Начало Dev-Library\]", "[🏛️ Начало Dev-Library]");
    content = Regex.Replace(content, @"\[[^\]]*Главен Портал \(Dev-Library\)\]", "[🏛️ Главен Портал (Dev-Library)]");
    content = Regex.Replace(content, @"\[[^\]]*Домейн:", "[🌐 Домейн:");
    content = Regex.Replace(content, @"\[[^\]]*Следващ домейн:", "[🌐 Следващ домейн:");
    content = Regex.Replace(content, @"\[[^\]]*Предишен модул:", "[📦 Предишен модул:");
    content = Regex.Replace(content, @"\[[^\]]*Следващ модул:", "[📦 Следващ модул:");

    // 2. Поправка на бутона [▶️ Започни от ...] в модулните README.md
    if (Path.GetFileName(file).Equals("README.md", StringComparison.OrdinalIgnoreCase))
    {
        var introFile = Directory.GetFiles(fileDir, "*-00-Intro*.md", SearchOption.AllDirectories)
            .OrderBy(p => p.Length)
            .FirstOrDefault();

        if (introFile != null)
        {
            string relPath = Path.GetRelativePath(fileDir, introFile).Replace('\\', '/');
            string title = File.ReadLines(introFile, Encoding.UTF8)
                .FirstOrDefault(l => l.StartsWith("# "))?
                .TrimStart('#', ' ') ?? "00. Intro";

            title = Regex.Replace(title, @"^[^\w\d]+", "").Trim();

            content = Regex.Replace(
                content,
                @"\[▶️ Започни от [^\]]+\]\([^)]+\)",
                $"[▶️ Започни от {title}]({relPath})"
            );
        }
    }

    // 3. Автоматична проверка и поправка на вътрешни линкове извън code blocks
    var codeBlocks = new List<string>();
    string workingContent = Regex.Replace(content, @"```[\s\(\S\)]*?```", m =>
    {
        codeBlocks.Add(m.Value);
        return $"__CODE_BLOCK_{codeBlocks.Count - 1}__";
    });

    workingContent = linkRegex.Replace(workingContent, match =>
    {
        string linkText = match.Groups[1].Value;
        string rawTarget = match.Groups[2].Value.Trim();

        if (rawTarget.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            rawTarget.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            rawTarget.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
            rawTarget.StartsWith("#"))
        {
            return match.Value;
        }

        var parts = rawTarget.Split('#', 2);
        string pathPart = parts[0];
        string anchorPart = parts.Length > 1 ? "#" + parts[1] : "";

        if (string.IsNullOrWhiteSpace(pathPart)) return match.Value;

        string targetFullPath = Path.GetFullPath(Path.Combine(fileDir, pathPart));

        // Ако пътят е валиден, не пипаме нищо
        if (File.Exists(targetFullPath) || Directory.Exists(targetFullPath))
        {
            return match.Value;
        }

        // ОПИТ ЗА АВТОМАТИЧНА ПОПРАВКА:
        string targetDir = Path.GetDirectoryName(targetFullPath)!;
        string targetFileName = Path.GetFileName(targetFullPath);

        if (Directory.Exists(targetDir))
        {
            var localFiles = Directory.GetFiles(targetDir, "*.md");

            // Случай А: Липсва -00- или -01- в името (напр. 01-01-Basic-Syntax.md -> 01-01-00-Basic-Syntax.md)
            var prefixMatch = Regex.Match(targetFileName, @"^(\d{2}-\d{2})-(.+)$");
            if (prefixMatch.Success)
            {
                string numPrefix = prefixMatch.Groups[1].Value;
                string rest = prefixMatch.Groups[2].Value;

                var candidate = localFiles.FirstOrDefault(f =>
                {
                    string name = Path.GetFileName(f);
                    return name.Equals($"{numPrefix}-00-{rest}", StringComparison.OrdinalIgnoreCase) ||
                           name.Equals($"{numPrefix}-01-{rest}", StringComparison.OrdinalIgnoreCase) ||
                           name.StartsWith($"{numPrefix}-00-", StringComparison.OrdinalIgnoreCase);
                });

                if (candidate != null)
                {
                    string fixedRel = Path.GetRelativePath(fileDir, candidate).Replace('\\', '/') + anchorPart;
                    autoFixedLinksCount++;
                    return $"[{linkText}]({fixedRel})";
                }
            }
        }

        // Случай Б: Файлът има уникално име някъде другаде в 01-Theory-Notes
        if (filesByName.TryGetValue(targetFileName, out var foundList) && foundList.Count == 1)
        {
            string fixedRel = Path.GetRelativePath(fileDir, foundList[0]).Replace('\\', '/') + anchorPart;
            autoFixedLinksCount++;
            return $"[{linkText}]({fixedRel})";
        }

        // Ако не може да се поправи автоматично -> записваме в отчета
        remainingBrokenLinks.Add($"[{relCurrentFile}] -> [{linkText}]({rawTarget})");
        return match.Value;
    });

    // Връщаме code blocks обратно
    for (int i = 0; i < codeBlocks.Count; i++)
    {
        workingContent = workingContent.Replace($"__CODE_BLOCK_{i}__", codeBlocks[i]);
    }

    if (workingContent != original)
    {
        File.WriteAllText(file, workingContent, utf8NoBom);
        updatedFilesCount++;
        Console.WriteLine($"✅ Обновен: {relCurrentFile}");
    }
}

Console.WriteLine($"\n📊 Готово! Обновени файлове: {updatedFilesCount} | Автоматично поправени линкове: {autoFixedLinksCount}");

string reportPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "broken-links-report.txt"));

var distinctBroken = remainingBrokenLinks.Distinct().ToList();
if (distinctBroken.Count > 0)
{
    File.WriteAllLines(reportPath, distinctBroken, utf8NoBom);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"⚠️ Остават {distinctBroken.Count} счупени линка за ръчен преглед.");
    Console.WriteLine($"📄 Пълният списък е записан в: {reportPath}");
    Console.ResetColor();
}
else
{
    if (File.Exists(reportPath)) File.Delete(reportPath);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("🎉 0 счупени линка! Всичко в библиотеката е 100% валидно!");
    Console.ResetColor();
}
