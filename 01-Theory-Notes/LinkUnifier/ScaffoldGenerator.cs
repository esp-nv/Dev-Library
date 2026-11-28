using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class ScaffoldGenerator
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static readonly Regex LinkRegex = new(@"(?<!!)\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

    public static void RunInteractiveScaffolding(string rootDir, string mapJsonPath, AuditStats stats, ref FolderNode? currentTree)
    {
        Console.WriteLine();
        Console.WriteLine("================================================================================");
        Console.WriteLine("🏗️ ИНТЕРАКТИВЕН ГЕНЕРАТОР НА СЕКЦИИ И ФАЙЛОВЕ (SCAFFOLDING)");
        Console.WriteLine("================================================================================");
        Console.Write("👉 Въведи относителен път до README.md или Intro файл (напр. 02-Frontend-And-UI-UX/HTML5-CSS3-And-SASS/01-00-Intro.md) или [m] за меню: ");

        string inputPath = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(inputPath) || inputPath.Equals("m", StringComparison.OrdinalIgnoreCase))
            return;

        string targetMdPath = Path.GetFullPath(Path.Combine(rootDir, inputPath));
        if (Directory.Exists(targetMdPath))
            targetMdPath = Path.Combine(targetMdPath, "README.md");

        if (!File.Exists(targetMdPath))
        {
            Console.WriteLine($"❌ Файлът не съществува: {targetMdPath}");
            return;
        }

        string targetDir = Path.GetDirectoryName(targetMdPath)!;
        var lines = File.ReadAllLines(targetMdPath, Encoding.UTF8).ToList();

        Console.Write("👉 Име на секцията за търсене (Enter за \"ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ГЛАВА 01\"): ");
        string sectionQuery = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(sectionQuery))
            sectionQuery = "ДЕТАЙЛНИ ПОДМОДУЛИ КЪМ ГЛАВА 01";

        string cleanQuery = Regex.Replace(sectionQuery, @"^#*\s*\d+\.\s*", "").Trim();

        int foundSectionIdx = -1;
        int maxSectionNum = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Trim();
            var m = Regex.Match(line, @"^##\s+(?:(\d+)\.\s*)?(.+)$");
            if (m.Success)
            {
                if (int.TryParse(m.Groups[1].Value, out int num) && num > maxSectionNum)
                    maxSectionNum = num;

                string titlePart = m.Groups[2].Value.Trim();
                if (titlePart.Contains(cleanQuery, StringComparison.OrdinalIgnoreCase) ||
                    cleanQuery.Contains(titlePart, StringComparison.OrdinalIgnoreCase))
                {
                    foundSectionIdx = i;
                }
            }
        }

        if (foundSectionIdx == -1)
        {
            int nextNum = maxSectionNum + 1;
            string newHeading = $"## {nextNum}. {cleanQuery}";
            string anchorSlug = ToGitHubAnchorSlug($"{nextNum}. {cleanQuery}");

            Console.WriteLine();
            Console.WriteLine($"🔎 Секция \"{cleanQuery}\" НЕ беше открита във файла.");
            Console.WriteLine($"   💡 Предложено заглавие: {newHeading}");
            Console.WriteLine($"   ⚓ Генерирана котва:    #{anchorSlug}");
            Console.Write("   👉 Нужно ли е да създам тази секция във файла? [y/n]: ");

            string ans = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (ans is "y" or "yes" or "д" or "да")
            {
                int insertIdx = FindInsertBeforeFooterIndex(lines);
                lines.Insert(insertIdx, "");
                lines.Insert(insertIdx + 1, newHeading);
                lines.Insert(insertIdx + 2, "");
                lines.Insert(insertIdx + 3, "* *(Добави подмодули или генерирай автоматично по-долу)*");
                lines.Insert(insertIdx + 4, "");

                foundSectionIdx = insertIdx + 1;
                File.WriteAllText(targetMdPath, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8NoBom);
                Console.WriteLine($"✅ Секцията \"{newHeading}\" е създадена успешно (котва: #{anchorSlug})!");
            }
            else
            {
                Console.WriteLine("⏭️ Създаването на секцията е пропуснато.");
                return;
            }
        }
        else
        {
            Console.WriteLine($"✅ Секцията е открита на ред {foundSectionIdx + 1}: {lines[foundSectionIdx]}");
        }

        int sectionEndIdx = lines.Count;
        for (int i = foundSectionIdx + 1; i < lines.Count; i++)
        {
            string t = lines[i].Trim();
            if (t.StartsWith("## ") || t == "---")
            {
                sectionEndIdx = i;
                break;
            }
        }

        var missingFilesToCreate = new List<(string LinkText, string FullPath)>();
        for (int i = foundSectionIdx + 1; i < sectionEndIdx; i++)
        {
            foreach (Match match in LinkRegex.Matches(lines[i]))
            {
                string linkText = match.Groups[1].Value;
                string target = match.Groups[2].Value.Trim().Split('#')[0];
                if (string.IsNullOrWhiteSpace(target) || target.StartsWith("http")) continue;

                string fullTarget = Path.GetFullPath(Path.Combine(targetDir, target));
                if (fullTarget.EndsWith(".md", StringComparison.OrdinalIgnoreCase) && !File.Exists(fullTarget))
                {
                    missingFilesToCreate.Add((linkText, fullTarget));
                }
            }
        }

        Console.WriteLine();
        Console.Write("👉 Искаш ли да добавиш нови файлове в тази секция сега? (въведи имена разделени със запетая, напр. 01-06-00-CSS-Grid.md, 01-06-01-Syntax-CSS-Grid.md или Enter за пропуск): ");
        string newFilesInput = (Console.ReadLine() ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(newFilesInput))
        {
            var newNames = newFilesInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int bulletInsertIdx = sectionEndIdx;
            if (foundSectionIdx + 2 < lines.Count && lines[foundSectionIdx + 2].Contains("*(Добави подмодули"))
            {
                lines.RemoveAt(foundSectionIdx + 2);
                bulletInsertIdx--;
            }

            foreach (var rawName in newNames)
            {
                string fileName = rawName.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ? rawName : rawName + ".md";
                string title = FormatTitleFromFileName(fileName);
                string fullNewFile = Path.Combine(targetDir, fileName);

                lines.Insert(bulletInsertIdx, $"* **[{title}]({fileName})**");
                bulletInsertIdx++;

                if (!File.Exists(fullNewFile))
                    missingFilesToCreate.Add((title, fullNewFile));
            }

            File.WriteAllText(targetMdPath, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8NoBom);
        }

        if (missingFilesToCreate.Count == 0)
        {
            Console.WriteLine("🎉 Всички описани файлове в тази секция вече съществуват на диска!");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"📄 Открити са {missingFilesToCreate.Count} липсващи .md файла за генериране:");
            foreach (var item in missingFilesToCreate)
                Console.WriteLine($"   • {Path.GetFileName(item.FullPath)}  ({item.LinkText})");

            Console.Write("👉 Да ги създам ли автоматично на диска с готов шаблон и Navbar/Footer? [y/n]: ");
            string createAns = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (createAns is "y" or "yes" or "д" or "да")
            {
                foreach (var item in missingFilesToCreate)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(item.FullPath)!);
                    string cleanTitle = Regex.Replace(item.LinkText, @"\*+", "").Trim();
                    string skeleton = BuildMarkdownSkeleton(cleanTitle);
                    File.WriteAllText(item.FullPath, skeleton, Utf8NoBom);
                    Console.WriteLine($"   ✅ Създаден: {Path.GetRelativePath(rootDir, item.FullPath).Replace('\\', '/')}");
                }

                currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath);
                NavigationSyncer.SyncAll(currentTree, rootDir, stats);
                Console.WriteLine("🧭 Navbar и Footer са синхронизирани автоматично за всички новосъздадени файлове!");
            }
        }

        OpenInVsCode(targetMdPath, foundSectionIdx + 1);
    }

    private static int FindInsertBeforeFooterIndex(List<string> lines)
    {
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (lines[i].Trim() == "---")
                return i;
        }
        return lines.Count;
    }

    private static string FormatTitleFromFileName(string fileName)
    {
        string noExt = Path.GetFileNameWithoutExtension(fileName);
        var m = Regex.Match(noExt, @"^(\d{2}-\d{2}(?:-\d{2})?)-(.+)$");
        if (m.Success)
            return $"{m.Groups[1].Value}. {m.Groups[2].Value.Replace('-', ' ')}";
        return noExt.Replace('-', ' ');
    }
    private static string BuildMarkdownSkeleton(string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# 📘 {title}");
        sb.AppendLine();
        sb.AppendLine("## 1. Въведение и архитектурна роля");
        sb.AppendLine();
        sb.AppendLine("*(Работна чернова — описание на концепцията, предназначение и контекст.)*");
        sb.AppendLine();
        sb.AppendLine("## 2. Основни принципи и механизми");
        sb.AppendLine();
        sb.AppendLine("* **Ключова концепция 1:** ...");
        sb.AppendLine("* **Ключова концепция 2:** ...");
        sb.AppendLine();
        sb.AppendLine("## 3. Практически примери и добри практики");
        sb.AppendLine();
        sb.AppendLine("```text");
        sb.AppendLine("// Примерен код");
        sb.AppendLine("```");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string ToGitHubAnchorSlug(string headingText)
    {
        string s = Regex.Replace(headingText, @"\[([^\]]+)\]\([^)]+\)", "$1");
        s = s.ToLowerInvariant().Trim();
        s = Regex.Replace(s, @"[^\p{L}\p{N}\s\-_]", "");
        s = s.Replace(' ', '-').Trim('-');
        return s;
    }

    private static void OpenInVsCode(string file, int line)
    {
        try
        {
            var vsCodeExe = Process.GetProcessesByName("Code")
                .Select(p => { try { return p.MainModule?.FileName; } catch { return null; } })
                .FirstOrDefault(path => !string.IsNullOrEmpty(path));

            if (vsCodeExe != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = vsCodeExe,
                    Arguments = $"-r -g \"{file}:{line}\"",
                    UseShellExecute = false
                });
            }
        }
        catch { }
    }
}

