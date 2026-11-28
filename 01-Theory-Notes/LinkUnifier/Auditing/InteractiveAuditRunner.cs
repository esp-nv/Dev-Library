using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

public static class InteractiveAuditRunner
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void Run(string rootNotesDir, AuditStats stats, bool anchorsOnly = false)
    {
        var allIssues = LinkScanner.ScanAll(rootNotesDir, stats);
        var issues = anchorsOnly ? allIssues.Where(i => i.IsAnchorOnly).ToList() : allIssues;

        if (issues.Count == 0)
        {
            Console.WriteLine(anchorsOnly
                ? "\n✅ Няма открити невалидни вътрешни #anchor котви!"
                : "\n✅ Няма открити счупени линкове или невалидни котви!");
            return;
        }

        Console.WriteLine($"\n⚠️  Открити {issues.Count} проблема за преглед:");
        Console.WriteLine("   [1] Одобри всички   [2] Прегледай един по един   [3] Само запис в отчетите (без промяна)");
        Console.Write("👉 Избери режим: ");
        string mode = Console.ReadLine()?.Trim() ?? "3";

        if (mode == "3") return;

        bool approveAll = mode == "1";
        var byFile = issues.GroupBy(i => i.RelativeFile).ToList();

        foreach (var fileGroup in byFile)
        {
            string fullPath = Path.Combine(rootNotesDir, fileGroup.Key.Replace('/', Path.DirectorySeparatorChar));
            var lines = File.ReadAllLines(fullPath).ToList();
            bool fileModified = false;

            if (!approveAll)
            {
                Console.WriteLine("\n======================================================================");
                Console.WriteLine($"📄 ФАЙЛ: {fileGroup.Key} ({fileGroup.Count()} проблема)");
                Console.WriteLine("======================================================================");
            }

            foreach (var issue in fileGroup.OrderByDescending(x => x.LineNumber))
            {
                int idx = issue.LineNumber - 1;
                if (idx < 0 || idx >= lines.Count) continue;

                if (approveAll)
                {
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, issue.ProposedReplacement);
                    fileModified = true;
                    LogApproval(stats, issue, issue.ProposedReplacement);
                    continue;
                }

                Console.WriteLine($"\n  🔹 [#{issue.Id}] Ред {issue.LineNumber}: {issue.Reason}");
                Console.WriteLine($"     Текущо:     {issue.OriginalLink}");
                Console.WriteLine($"     Предложено: {issue.ProposedReplacement}");
                Console.WriteLine("     [1] Одобри  [2] Пропусни  [3] Маркирай *(в разработка)*  [4] Свой път  [5] Изтрий реда  [6] Одобри до края  [0] Спри");
                Console.Write("     👉 Избор: ");
                string choice = Console.ReadLine()?.Trim() ?? "2";

                if (choice == "0")
                {
                    if (fileModified) File.WriteAllLines(fullPath, lines, Utf8NoBom);
                    return;
                }
                if (choice == "6")
                {
                    approveAll = true;
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, issue.ProposedReplacement);
                    fileModified = true;
                    LogApproval(stats, issue, issue.ProposedReplacement);
                }
                else if (choice == "1")
                {
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, issue.ProposedReplacement);
                    fileModified = true;
                    LogApproval(stats, issue, issue.ProposedReplacement);
                }
                else if (choice == "3")
                {
                    string wip = $"**{issue.LinkText}** *(в разработка)*";
                    lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, wip);
                    fileModified = true;
                    stats.RejectedLog.Add($"[В ПРОЦЕС] {issue.RelativeFile}:{issue.LineNumber} | {issue.OriginalLink} -> {wip}");
                }
                else if (choice == "4")
                {
                    Console.Write("     ✏️ Въведи нов път (напр. ../Folder/File.md): ");
                    string custom = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(custom))
                    {
                        string customLink = $"[{issue.LinkText}]({custom})";
                        lines[idx] = ReplaceFirst(lines[idx], issue.OriginalLink, customLink);
                        fileModified = true;
                        LogApproval(stats, issue, customLink);
                    }
                }
                else if (choice == "5")
                {
                    stats.DeletedLinesLog.Add($"[ИЗТРИТ РЕД] {issue.RelativeFile}:{issue.LineNumber} | {lines[idx].Trim()}");
                    lines.RemoveAt(idx);
                    fileModified = true;
                }
                else
                {
                    stats.SkippedLog.Add($"[ПРОПУСНАТ] {issue.RelativeFile}:{issue.LineNumber} | {issue.OriginalLink}");
                }
            }

            if (fileModified)
                File.WriteAllLines(fullPath, lines, Utf8NoBom);
        }

        // Пресканираме, за да обновим точния брой валидни, в разработка и оставащи счупени
        LinkScanner.ScanAll(rootNotesDir, stats);
    }

    private static void LogApproval(AuditStats stats, LinkIssue issue, string replacement)
    {
        if (replacement.Contains("*(в разработка)*"))
            stats.RejectedLog.Add($"[В ПРОЦЕС] {issue.RelativeFile}:{issue.LineNumber} | {issue.OriginalLink} -> {replacement}");
        else
            stats.ApprovedLog.Add($"[ОДОБРЕН] {issue.RelativeFile}:{issue.LineNumber} | {issue.OriginalLink} -> {replacement}");
    }

    private static string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search, StringComparison.Ordinal);
        if (pos < 0) return text;
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
}
