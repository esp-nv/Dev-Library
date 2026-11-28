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
var renamedFilesLog = new List<string>();
var updatedFilesLog = new List<string>();
var autoFixedLinksLog = new List<string>();
var remainingBrokenLinks = new List<string>();

// СТЪПКА 0: Преименуване на XX-YY-01-Topic-Syntax.md -> XX-YY-01-Syntax-Topic.md в ЦЯЛАТА 01-Theory-Notes
foreach (var file in Directory.GetFiles(rootDir, "*-0?-*-Syntax.md", SearchOption.AllDirectories))
{
    string fileName = Path.GetFileName(file);
    var m = Regex.Match(fileName, @"^(\d{2}-\d{2}-0[1-9])-(.+)-Syntax\.md$");
    if (m.Success)
    {
        string dir = Path.GetDirectoryName(file)!;
        string newFileName = $"{m.Groups[1].Value}-Syntax-{m.Groups[2].Value}.md";
        string newFullPath = Path.Combine(dir, newFileName);
        string relOld = Path.GetRelativePath(rootDir, file).Replace('\\', '/');
        string relNew = Path.GetRelativePath(rootDir, newFullPath).Replace('\\', '/');

        File.Move(file, newFullPath, overwrite: true);
        renamedFilesLog.Add($"{relOld}  --->  {relNew}");
        Console.WriteLine($"🔄 Преименуван: {relOld} -> {newFileName}");
    }
}

var allMdFiles = Directory.GetFiles(rootDir, "*.md", SearchOption.AllDirectories);

// Индексираме всички файлове по име (без README.md) за бързо търсене при преместване
var filesByName = allMdFiles
    .Where(f => !Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase))
    .GroupBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

var linkRegex = new Regex(@"(?<!!)\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

foreach (var file in allMdFiles)
{
    string original = File.ReadAllText(file, Encoding.UTF8);
    string content = original;
    var fileDir = Path.GetDirectoryName(file)!;
    string relCurrentFile = Path.GetRelativePath(rootDir, file).Replace('\\', '/');

    // 1. Унифициране на иконките навсякъде
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
    // 2.1. Стандартизиране на долния ред: © 2026 Dev-Library / <H1 заглавие на текущия файл>
    var h1Line = File.ReadLines(file, Encoding.UTF8)
        .FirstOrDefault(l => l.StartsWith("# "));

    if (h1Line != null)
    {
        // Премахваме водещото '# ' и евентуални водещи емоджита/символи, оставяйки номера и заглавието
        string cleanTitle = Regex.Replace(h1Line.TrimStart('#', ' '), @"^[^\w\d]+", "").Trim();
        if (!string.IsNullOrWhiteSpace(cleanTitle))
        {
            content = Regex.Replace(
                content,
                @"^©\s*2026\s*Dev-Library\s*/.*$",
                $"© 2026 Dev-Library / {cleanTitle}",
                RegexOptions.Multiline
            );
        }
    }

    // 3. Проверка и автоматична поправка на вътрешни линкове извън code blocks
    var codeBlocks = new List<string>();
    string workingContent = Regex.Replace(content, @"```[\s\S]*?```", m =>
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

        if (File.Exists(targetFullPath) || Directory.Exists(targetFullPath))
        {
            return match.Value;
        }

        string targetDir = Path.GetDirectoryName(targetFullPath)!;
        string targetFileName = Path.GetFileName(targetFullPath);

        if (Directory.Exists(targetDir))
        {
            var localFiles = Directory.GetFiles(targetDir, "*.md");

            // Поправка на преименувани -01-Syntax- файлове (XX-YY-01-Topic-Syntax.md -> XX-YY-01-Syntax-Topic.md)
            var syntaxMatch = Regex.Match(targetFileName, @"^(\d{2}-\d{2}-0[1-9])-(.+)-Syntax\.md$");
            if (syntaxMatch.Success)
            {
                string swappedName = $"{syntaxMatch.Groups[1].Value}-Syntax-{syntaxMatch.Groups[2].Value}.md";
                var swappedCandidate = localFiles.FirstOrDefault(f => Path.GetFileName(f).Equals(swappedName, StringComparison.OrdinalIgnoreCase));
                if (swappedCandidate != null)
                {
                    string fixedRel = Path.GetRelativePath(fileDir, swappedCandidate).Replace('\\', '/') + anchorPart;
                    autoFixedLinksLog.Add($"[{relCurrentFile}] : ({rawTarget})  --->  ({fixedRel})");
                    return $"[{linkText}]({fixedRel})";
                }
            }

            // Поправка при липсващо -00- или -01-
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
                    autoFixedLinksLog.Add($"[{relCurrentFile}] : ({rawTarget})  --->  ({fixedRel})");
                    return $"[{linkText}]({fixedRel})";
                }
            }
        }

        if (filesByName.TryGetValue(targetFileName, out var foundList) && foundList.Count == 1)
        {
            string fixedRel = Path.GetRelativePath(fileDir, foundList[0]).Replace('\\', '/') + anchorPart;
            autoFixedLinksLog.Add($"[{relCurrentFile}] : ({rawTarget})  --->  ({fixedRel})");
            return $"[{linkText}]({fixedRel})";
        }

        remainingBrokenLinks.Add($"[{relCurrentFile}] -> [{linkText}]({rawTarget})");
        return match.Value;
    });

    for (int i = 0; i < codeBlocks.Count; i++)
    {
        workingContent = workingContent.Replace($"__CODE_BLOCK_{i}__", codeBlocks[i]);
    }

    if (workingContent != original)
    {
        File.WriteAllText(file, workingContent, utf8NoBom);
        updatedFilesLog.Add(relCurrentFile);
        Console.WriteLine($"✅ Обновен: {relCurrentFile}");
    }
}

// Записваме чист, напълно презаписан доклад в broken-links-report.txt
string reportPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "broken-links-report.txt"));
var distinctBroken = remainingBrokenLinks.Distinct().OrderBy(x => x).ToList();

var report = new StringBuilder();
report.AppendLine("================================================================================");
report.AppendLine($"ОТЧЕТ ОТ LINKUNIFIER (01-Theory-Notes) — {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
report.AppendLine("================================================================================");
report.AppendLine($"• Общо проверени .md файлове:       {allMdFiles.Length}");
report.AppendLine($"• Преименувани *-Syntax.md файлове:  {renamedFilesLog.Count}");
report.AppendLine($"• Обновени .md файлове:              {updatedFilesLog.Count}");
report.AppendLine($"• Автоматично поправени линкове:     {autoFixedLinksLog.Count}");
report.AppendLine($"• Счупени линкове за ръчен преглед:  {distinctBroken.Count}");
report.AppendLine();

report.AppendLine("--------------------------------------------------------------------------------");
report.AppendLine($"1. СЧУПЕНИ ЛИНКОВЕ ЗА РЪЧЕН ПРЕГЛЕД ({distinctBroken.Count})");
report.AppendLine("--------------------------------------------------------------------------------");
if (distinctBroken.Count == 0)
{
    report.AppendLine("🎉 Няма счупени линкове!");
}
else
{
    foreach (var item in distinctBroken)
        report.AppendLine(item);
}
report.AppendLine();

report.AppendLine("--------------------------------------------------------------------------------");
report.AppendLine($"2. ПРЕИМЕНУВАНИ ФАЙЛОВЕ ({renamedFilesLog.Count})");
report.AppendLine("--------------------------------------------------------------------------------");
foreach (var item in renamedFilesLog)
    report.AppendLine(item);
report.AppendLine();

report.AppendLine("--------------------------------------------------------------------------------");
report.AppendLine($"3. АВТОМАТИЧНО ПОПРАВЕНИ ЛИНКОВЕ ({autoFixedLinksLog.Count})");
report.AppendLine("--------------------------------------------------------------------------------");
foreach (var item in autoFixedLinksLog)
    report.AppendLine(item);
report.AppendLine();

report.AppendLine("--------------------------------------------------------------------------------");
report.AppendLine($"4. ВСИЧКИ ОБНОВЕНИ ФАЙЛОВЕ ({updatedFilesLog.Count})");
report.AppendLine("--------------------------------------------------------------------------------");
foreach (var item in updatedFilesLog)
    report.AppendLine(item);

// File.WriteAllText изтрива цялото старо съдържание и записва новото на чисто
File.WriteAllText(reportPath, report.ToString(), utf8NoBom);

Console.WriteLine($"\n📊 Готово! Преименувани: {renamedFilesLog.Count} | Обновени файлове: {updatedFilesLog.Count} | Поправени линкове: {autoFixedLinksLog.Count} | За ръчен преглед: {distinctBroken.Count}");
Console.WriteLine($"📄 Пълният отчет е презаписан в: {reportPath}");


