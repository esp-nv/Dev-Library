using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

public static class InteractiveAuditRunner
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

     public static void Run(string rootNotesDir, AuditStats stats, LinkUnifierConfig? cfg = null, bool anchorsOnly = false,
        bool missingFilesOnly = false)
    {
        var allIssues = LinkScanner.ScanAll(rootNotesDir, stats);
        var issues = allIssues
            .Where(i => (!anchorsOnly || i.IsAnchorOnly) &&
                        (!missingFilesOnly || (!i.IsAnchorOnly && i.Reason.Contains("не съществува"))))
            .ToList();

        string label = missingFilesOnly
            ? "ЛИПСВАЩИ ФАЙЛОВЕ (Има линк, но няма файл)"
            : anchorsOnly ? "#ANCHOR КОТВИ" : "ВСИЧКИ СЧУПЕНИ ЛИНКОВЕ И #ANCHOR КОТВИ";

        Console.WriteLine($"\n🔍 Одит ({label}): намерени {issues.Count} проблема.");

        if (issues.Count == 0)
        {
            Console.WriteLine("🎉 Всичко в тази категория е напълно валидно!");
            return;
        }

        Console.WriteLine("   1. Одобри всички автоматични предложения");
        Console.WriteLine("   2. Прегледай един по един (интерактивно меню за текст, път, избор от списък, изтриване)");
        Console.WriteLine("   3. Само запис в отчетите (без промяна по файловете)");
        Console.Write("👉 Избор (1-3): ");
        string mode = Console.ReadLine()?.Trim() ?? "3";

        if (mode == "3") return;

        bool approveAll = mode == "1";
        var fileLinesCache = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var modifiedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var issue in issues)
        {
            string fullPath = Path.Combine(rootNotesDir, issue.RelativeFile.Replace('/', Path.DirectorySeparatorChar));
            if (!fileLinesCache.TryGetValue(fullPath, out var lines))
            {
                lines = File.ReadAllLines(fullPath).ToList();
                fileLinesCache[fullPath] = lines;
            }

            int idx = issue.LineNumber - 1;
            if (idx < 0 || idx >= lines.Count) continue;

            if (approveAll)
            {
                lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, issue.ProposedReplacement);
                modifiedFiles.Add(fullPath);
                stats.ApprovedLog.Add($"[{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink} -> {issue.ProposedReplacement}");
                continue;
            }

            Console.WriteLine("\n--------------------------------------------------------------------------------");
            Console.WriteLine($"📄 Файл: {issue.RelativeFile} (ред {issue.LineNumber})");
            Console.WriteLine($"   Причина:     {issue.Reason}");
            Console.WriteLine($"   Текущ линк:  {issue.OriginalLink}");
            Console.WriteLine($"   Предложено:  {issue.ProposedReplacement}");
            Console.WriteLine("   1. Одобри предложението");
            Console.WriteLine("   2. Пропусни");
            Console.WriteLine("   3. Маркирай *(в разработка)* (без линк)");
            Console.WriteLine("   4. Промени САМО пътя (избор от топ 5 файла или свой път)");
            Console.WriteLine("   5. Промени И текста [текст], И пътя (път)");
            Console.WriteLine("   6. Изтрий целия ред");
            Console.WriteLine("   7. Одобри всички останали до края");
            Console.WriteLine("   0. Спри одита");
            Console.Write("👉 Избор (0-7): ");

            string act = Console.ReadLine()?.Trim() ?? "2";
            if (act == "0") break;

            if (act == "7")
            {
                approveAll = true;
                act = "1";
            }

            switch (act)
            {
                case "1":
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, issue.ProposedReplacement);
                    modifiedFiles.Add(fullPath);
                    stats.ApprovedLog.Add($"[{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink} -> {issue.ProposedReplacement}");
                    break;

                case "3":
                    string wip = $"**{issue.LinkText}** *(в разработка)*";
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, wip);
                    modifiedFiles.Add(fullPath);
                    stats.RejectedLog.Add($"[В ПРОЦЕС] [{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink} -> {wip}");
                    break;

                case "4":
                {
                    string chosenPath = PickOrEnterPath(fullPath, issue.LinkTarget, rootNotesDir);
                    if (!string.IsNullOrWhiteSpace(chosenPath))
                    {
                        string customLink = $"[{issue.LinkText}]({chosenPath})";
                        lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, customLink);
                        modifiedFiles.Add(fullPath);
                        stats.ApprovedLog.Add($"[РЪЧЕН ПЪТ] [{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink} -> {customLink}");
                    }
                    break;
                }

                case "5":
                {
                    Console.Write($"✏️ Нов текст (Enter за '{issue.LinkText}'): ");
                    string newText = Console.ReadLine()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(newText)) newText = issue.LinkText;

                    string chosenPath = PickOrEnterPath(fullPath, issue.LinkTarget, rootNotesDir);
                    if (!string.IsNullOrWhiteSpace(chosenPath))
                    {
                        string customLink = $"[{newText}]({chosenPath})";
                        lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, customLink);
                        modifiedFiles.Add(fullPath);
                        stats.ApprovedLog.Add($"[ТЕКСТ+ПЪТ] [{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink} -> {customLink}");
                    }
                    break;
                }

                case "6":
                    stats.DeletedLinesLog.Add($"[{issue.RelativeFile}:{issue.LineNumber}] {lines[idx].Trim()}");
                    lines[idx] = "__DELETE_LINE__";
                    modifiedFiles.Add(fullPath);
                    break;

                default:
                    stats.SkippedLog.Add($"[{issue.RelativeFile}:{issue.LineNumber}] {issue.OriginalLink}");
                    break;
            }
        }

        foreach (var file in modifiedFiles)
        {
            var cleaned = fileLinesCache[file].Where(l => l != "__DELETE_LINE__").ToList();
            File.WriteAllLines(file, cleaned, Utf8NoBom);
        }

        LinkScanner.ScanAll(rootNotesDir, stats);
        Console.WriteLine($"✅ Обновени файлове: {modifiedFiles.Count}");
    }

    private static string PickOrEnterPath(string currentFile, string rawTarget, string rootNotesDir)
    {
        var suggestions = LinkResolver.FindTopSuggestions(currentFile, rawTarget, rootNotesDir);
        Console.WriteLine("   💡 Топ 5 най-близки съществуващи файла:");
        for (int i = 0; i < suggestions.Count; i++)
            Console.WriteLine($"      {i + 1}. {suggestions[i]}");

        Console.Write("   👉 Избери номер (1-5) или въведи свой относителен път (празно за отказ): ");
        string input = Console.ReadLine()?.Trim() ?? "";
        if (int.TryParse(input, out int pick) && pick >= 1 && pick <= suggestions.Count)
            return suggestions[pick - 1];

        return input;
    }

    private static string ReplaceFirst(string source, string search, string replace)
    {
        int pos = source.IndexOf(search, StringComparison.Ordinal);
        return pos < 0 ? source : source.Substring(0, pos) + replace + source.Substring(pos + search.Length);
    }
}
