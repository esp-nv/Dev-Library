using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class LinkAuditor
{
    private static readonly Regex LinkRegex = new(@"(?<!!)\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static string NowStamp() => DateTime.Now.ToString("ddd yyyy-MM-dd HH:mm:ss");



    public static void AuditAndOptionallyFix(FolderNode rootTree, string rootDir, AuditStats stats, bool onlyAnchors, bool autoPromptToFix)
    {
        var issues = ScanAllIssues(rootTree, rootDir, onlyAnchors);
        stats.LastAuditIssues.Clear();
        stats.LastAuditIssues.AddRange(issues);

        int brokenLinksCount = issues.Count(i => !i.IsAnchorOnly);
        int brokenAnchorsCount = issues.Count(i => i.IsAnchorOnly);
        int filesAffected = issues.Select(i => i.RelativeFile).Distinct().Count();

        Console.WriteLine();
        Console.WriteLine("================================================================================");
        Console.WriteLine(onlyAnchors
            ? "⚓ РЕЗУЛТАТ ОТ ОДИТА НА КОТВИТЕ (#anchors)"
            : "🔍 РЕЗУЛТАТ ОТ ПЪЛНИЯ ОДИТ НА ЛИНКОВЕ И КОТВИ (#anchors)");
        Console.WriteLine("================================================================================");
        Console.WriteLine($"• Засегнати файлове:         {filesAffected}");
        if (!onlyAnchors)
            Console.WriteLine($"• 1/ Счупени линкове:        {brokenLinksCount}");
        Console.WriteLine($"• 2/ Невалидни #anchor котви: {brokenAnchorsCount}");
        Console.WriteLine($"• ОБЩО намерени проблеми:    {issues.Count}");
        Console.WriteLine("--------------------------------------------------------------------------------");

        if (issues.Count == 0)
        {
            Console.WriteLine("🎉 Няма открити счупени линкове или невалидни котви!");
            return;
        }

        // Показваме първите 15 групирани по файл за бърз преглед в конзолата
        foreach (var fileGroup in issues.GroupBy(i => i.RelativeFile).Take(10))
        {
            Console.WriteLine($"📄 {fileGroup.Key}");
            var links = fileGroup.Where(x => !x.IsAnchorOnly).ToList();
            var anchors = fileGroup.Where(x => x.IsAnchorOnly).ToList();

            if (links.Count > 0)
            {
                Console.WriteLine("   1/ Линкове:");
                foreach (var l in links)
                    Console.WriteLine($"      [#{l.Id}] ред {l.LineNumber}: {l.OriginalLink}  --->  {l.ProposedReplacement}");
            }
            if (anchors.Count > 0)
            {
                Console.WriteLine("   2/ #anchor котви:");
                foreach (var a in anchors)
                    Console.WriteLine($"      [#{a.Id}] ред {a.LineNumber}: {a.OriginalLink}  --->  {a.ProposedReplacement}");
            }
        }
        if (filesAffected > 10)
            Console.WriteLine($"... и още {filesAffected - 10} файла (пълният списък по файлове е в reports/04-grouped-audit-report.txt).");

        if (!autoPromptToFix)
            return;

        Console.WriteLine();
        Console.WriteLine("👉 Искаш ли да ги оправим сега?");
        Console.WriteLine("   [y] Да — поправи автоматично ВСИЧКИ с намерено съвпадение");
        Console.WriteLine("   [s] Избор по номера (напр. 1, 5, 10-25 или anchors)");
        Console.WriteLine("   [i] Интерактивно един по един с VS Code (числови команди 1-8)");
        Console.WriteLine("   [n] Не — запази само отчета в reports/");
        Console.Write("👉 Избор [y / s / i / n]: ");

        string mode = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (mode is "y" or "yes" or "д" or "да")
        {
            ApplyBatchFix(issues, rootDir, stats, onlyMatched: true);
        }
        else if (mode is "s" or "select")
        {
            Console.Write("👉 Въведи номера (напр. 1, 3, 5-12) или 'anchors': ");
            string selInput = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (selInput == "anchors")
            {
                ApplyBatchFix(issues.Where(x => x.IsAnchorOnly).ToList(), rootDir, stats, onlyMatched: false);
            }
            else
            {
                var ids = ParseIdSelection(selInput);
                var selected = issues.Where(x => ids.Contains(x.Id)).ToList();
                ApplyBatchFix(selected, rootDir, stats, onlyMatched: false);
            }
        }
        else if (mode is "i" or "interactive")
        {
            RunInteractiveNumericAudit(issues, rootDir, stats);
        }
        else
        {
            stats.RejectedLog.Clear();
            foreach (var iss in issues)
            stats.RejectedLog.Add($"[{NowStamp()}] ⏭️ [{iss.RelativeFile} : ред {iss.LineNumber}] {iss.OriginalLink}  (Предложено: {iss.ProposedReplacement} | Причина: {iss.Reason})");
            Console.WriteLine("💾 Запазен е само пълният одитен отчет без промени по файловете.");
        }
    }

    public static List<AuditIssue> ScanAllIssues(FolderNode rootTree, string rootDir, bool onlyAnchors)
    {
        var allNodes = new List<FolderNode>();
        TreeScanner.CollectAllNodes(rootTree, allNodes);
        var allLessons = allNodes.SelectMany(n => n.Lessons).ToList();

        var allMdFiles = Directory.GetFiles(rootDir, "*.md", SearchOption.AllDirectories)
    .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}LinkUnifier{Path.DirectorySeparatorChar}") &&
                !f.Contains($"{Path.DirectorySeparatorChar}reports{Path.DirectorySeparatorChar}"))
    .OrderBy(f => f)
    .ToList();

        var anchorCache = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        HashSet<string> GetCachedAnchors(string mdPath)
        {
            string full = Path.GetFullPath(mdPath);
            if (!anchorCache.TryGetValue(full, out var set))
            {
                set = ExtractFileAnchors(full);
                anchorCache[full] = set;
            }
            return set;
        }

        var issues = new List<AuditIssue>();
        int nextId = 1;

        foreach (var file in allMdFiles)
        {
            string relFile = Path.GetRelativePath(rootDir, file).Replace('\\', '/');
            string fileDir = Path.GetDirectoryName(file)!;
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            bool inCodeBlock = false;

            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                string rawLine = lines.ElementAt(lineIdx);
                if (rawLine.TrimStart().StartsWith("```"))
                {
                    inCodeBlock = !inCodeBlock;
                    continue;
                }
                if (inCodeBlock) continue;

                foreach (Match match in LinkRegex.Matches(rawLine))
                {
                    string fullMatch = match.Value;
                    string linkText = match.Groups.Cast<Group>().ElementAt(1).Value;
                    string target = match.Groups.Cast<Group>().ElementAt(2).Value.Trim();

                    if (target.StartsWith("http") || target.StartsWith("mailto:"))
                        continue;

                    var parts = target.Split('#', 2);
                    string pathPart = parts.First();
                    string anchorPart = parts.Length > 1 ? parts.ElementAt(1) : "";

                    if (onlyAnchors && string.IsNullOrWhiteSpace(anchorPart))
                        continue;

                    string targetFileFull = string.IsNullOrWhiteSpace(pathPart)
                        ? file
                        : Path.GetFullPath(Path.Combine(fileDir, pathPart));

                    bool pathExists = File.Exists(targetFileFull) || Directory.Exists(targetFileFull);

                    if (!pathExists)
                    {
                        if (onlyAnchors) continue;
                        string proposed = ProposeReplacement(linkText, target, fileDir, rootDir, allNodes, allLessons);
                        issues.Add(new AuditIssue
                        {
                            Id = nextId++,
                            FullFilePath = file,
                            RelativeFile = relFile,
                            LineNumber = lineIdx + 1,
                            IsAnchorOnly = false,
                            Reason = "Липсващ файл или празна директория",
                            OriginalLink = fullMatch,
                            ProposedReplacement = proposed
                        });
                    }
                    else if (!string.IsNullOrWhiteSpace(anchorPart))
                    {
                        string checkMdFile = Directory.Exists(targetFileFull)
                            ? Path.Combine(targetFileFull, "README.md")
                            : targetFileFull;

                        var validAnchors = GetCachedAnchors(checkMdFile);
                        if (!validAnchors.Contains(anchorPart))
                        {
                            var anchorTokens = Tokenize(anchorPart);
                            string? bestAnchor = validAnchors
                                .Select(a => new { Anchor = a, Overlap = Tokenize(a).Intersect(anchorTokens).Count() })
                                .Where(x => x.Overlap > 0)
                                .OrderByDescending(x => x.Overlap)
                                .FirstOrDefault()?.Anchor;

                            string proposed = bestAnchor != null
                                ? $"[{linkText}]({pathPart}#{bestAnchor})"
                                : (string.IsNullOrWhiteSpace(pathPart) ? linkText : $"[{linkText}]({pathPart})");

                                                       issues.Add(new AuditIssue
                            {
                                Id = nextId++,
                                FullFilePath = file,
                                RelativeFile = relFile,
                                LineNumber = lineIdx + 1,
                                IsAnchorOnly = true,
                                Reason = $"Невалидна котва (#{anchorPart})",
                                OriginalLink = fullMatch,
                                ProposedReplacement = proposed
                            });
                        }
                    }
                }
            }
        }

        return issues;
    }

    private static void ApplyBatchFix(List<AuditIssue> targetIssues, string rootDir, AuditStats stats, bool onlyMatched)
    {
        int applied = 0;
        foreach (var group in targetIssues.GroupBy(i => i.FullFilePath))
        {
            var lines = File.ReadAllLines(group.Key, Encoding.UTF8).ToList();
            bool modified = false;

            foreach (var iss in group)
            {
                if (onlyMatched && iss.ProposedReplacement.Contains("*(в разработка)*"))
                    continue;

                int idx = iss.LineNumber - 1;
                if (idx >= 0 && idx < lines.Count && lines.ElementAt(idx).Contains(iss.OriginalLink))
                {
                    lines[idx] = lines.ElementAt(idx).Replace(iss.OriginalLink, iss.ProposedReplacement);
                    modified = true;
                    applied++;
                    stats.ApprovedLog.Add($"[{NowStamp()}] ✅ [{iss.RelativeFile} : ред {iss.LineNumber}] {iss.OriginalLink}  --->  {iss.ProposedReplacement}");

                 }
            }

            if (modified)
                File.WriteAllText(group.Key, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8NoBom);
        }
        Console.WriteLine($"✅ Приложени са {applied} корекции успешно!");
    }

    private static void RunInteractiveNumericAudit(List<AuditIssue> issues, string rootDir, AuditStats stats)
    {
        string batchMode = "ask";
        stats.RejectedLog.Clear();

        foreach (var fileGroup in issues.GroupBy(i => i.FullFilePath))
        {
            if (batchMode == "menu") break;

            var lines = File.ReadAllLines(fileGroup.Key, Encoding.UTF8).ToList();
            bool fileModified = false;
            int lineOffset = 0;

            foreach (var iss in fileGroup)
            {
                if (batchMode == "menu") break;

                int currentIdx = (iss.LineNumber - 1) + lineOffset;
                if (currentIdx < 0 || currentIdx >= lines.Count) continue;
                string currentLine = lines.ElementAt(currentIdx);
                if (!currentLine.Contains(iss.OriginalLink)) continue;

                string choice = "5";
                if (batchMode == "all") choice = "1";
                else if (batchMode == "skip_all") choice = "5";
                else
                {
                    OpenInVsCode(fileGroup.Key, currentIdx + 1);
                    Console.WriteLine();
                    Console.WriteLine($"📄 [{iss.Id}/{issues.Count}] {iss.RelativeFile} (ред {currentIdx + 1}) — {(iss.IsAnchorOnly ? "2/ #anchor котва" : "1/ Линк")}: {iss.Reason}");
                    Console.WriteLine($"   ❌ Текущо:     {iss.OriginalLink}");
                    Console.WriteLine($"   💡 Предложено: {iss.ProposedReplacement}");
                    Console.WriteLine("   Команди: Одобри | Изтрий реда | Създай файла | В процес/за добавяне | Пропусни | Одобри всички | Пропусни всички | Меню");
                    Console.Write("   👉 Избор [0-7]: ");

                    while (true)
                    {
                        string input = (Console.ReadLine() ?? "").Trim();
                        if (input is "1" or "2" or "3" or "4" or "5" or "6" or "7" or "0")
                        {
                            choice = input;
                            break;
                        }
                        Console.Write("   Моля въведи число от 0 до 7: ");
                    }

                    if (choice == "6") { batchMode = "all"; choice = "1"; }
                    else if (choice == "7") { batchMode = "skip_all"; choice = "5"; }
                    else if (choice == "0") { batchMode = "menu"; break; }
                }

                if (choice == "1")
                {
                    lines[currentIdx] = currentLine.Replace(iss.OriginalLink, iss.ProposedReplacement);
                    fileModified = true;
                    stats.ApprovedLog.Add($"[{NowStamp()}] ✅ [{iss.RelativeFile} : ред {iss.LineNumber}] {iss.OriginalLink}  --->  {iss.ProposedReplacement}");
                }
                else if (choice == "2")
                {
                    stats.DeletedLinesLog.Add($"[{NowStamp()}] 🗑️ [{iss.RelativeFile} : ред {iss.LineNumber}] ИЗТРИТ РЕД: {currentLine.Trim()}");
                    lines.RemoveAt(currentIdx);
                    lineOffset--;
                    fileModified = true;
                }
                else if (choice == "3")
                {
                    CreateMissingLinkedFile(iss, fileGroup.Key, rootDir, stats);
                }
                else if (choice == "4")
                {
                    string cleanTitle = Regex.Replace(iss.OriginalLink, @"^\[([^\]]+)\].*$", "$1");
                    string inProgressText = $"**{cleanTitle}** *(в процес / за добавяне)*";
                    lines[currentIdx] = currentLine.Replace(iss.OriginalLink, inProgressText);
                    fileModified = true;
                    stats.RejectedLog.Add($"[{NowStamp()}] 🚧 [В ПРОЦЕС / ЗА ДОБАВЯНЕ] [{iss.RelativeFile} : ред {iss.LineNumber}] {iss.OriginalLink}");
                }
                else
                {
                    stats.RejectedLog.Add($"[{NowStamp()}] ⏭️ [{iss.RelativeFile} : ред {iss.LineNumber}] {iss.OriginalLink}  (Предложено: {iss.ProposedReplacement} | Причина: {iss.Reason})");
                }
            }

            if (fileModified)
                File.WriteAllText(fileGroup.Key, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8NoBom);
        }
    }

    private static void CreateMissingLinkedFile(AuditIssue iss, string sourceFile, string rootDir, AuditStats stats)
    {
        var m = LinkRegex.Match(iss.OriginalLink);
        if (!m.Success) return;

        string linkText = m.Groups.Cast<Group>().ElementAt(1).Value;
        string targetRel = m.Groups.Cast<Group>().ElementAt(2).Value.Split('#').First().Trim();
        if (string.IsNullOrWhiteSpace(targetRel)) return;

        string fullNewPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFile)!, targetRel));
        if (!fullNewPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            fullNewPath = Path.Combine(fullNewPath, "README.md");

        Directory.CreateDirectory(Path.GetDirectoryName(fullNewPath)!);
        if (!File.Exists(fullNewPath))
        {
            string skeleton = $"# 📘 {linkText}\n\n## 1. Въведение и архитектурна роля\n\n*(Създадено автоматично от LinkAuditor — в процес на попълване.)*\n";
            File.WriteAllText(fullNewPath, skeleton, Utf8NoBom);
            string relCreated = Path.GetRelativePath(rootDir, fullNewPath).Replace('\\', '/');
            stats.ApprovedLog.Add($"[{NowStamp()}]🆕 [СЪЗДАДЕН ФАЙЛ] {relCreated} (от {iss.RelativeFile} : ред {iss.LineNumber})");
            Console.WriteLine($"   🆕 Създаден файл: {relCreated}");
        }
    }

    private static HashSet<int> ParseIdSelection(string input)
    {
        var result = new HashSet<int>();
        foreach (var part in input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.Contains('-'))
            {
                var range = part.Split('-', 2);
                if (int.TryParse(range.First(), out int start) && int.TryParse(range.Last(), out int end))
                    for (int i = start; i <= end; i++) result.Add(i);
            }
            else if (int.TryParse(part, out int single))
            {
                result.Add(single);
            }
        }
        return result;
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

    private static string ProposeReplacement(string linkText, string brokenTarget, string fileDir, string rootDir, List<FolderNode> allNodes, List<LessonNode> allLessons)
    {
        var parts = brokenTarget.Split('#', 2);
        string pathPart = parts.First();

        string fullTarget = Path.GetFullPath(Path.Combine(fileDir, pathPart));
        string theoryParent = Path.GetDirectoryName(rootDir)!;
        if (!fullTarget.StartsWith(theoryParent, StringComparison.OrdinalIgnoreCase))
        {
            string relToRoot = Path.GetRelativePath(fileDir, Path.Combine(rootDir, "README.md")).Replace('\\', '/');
            return $"[{linkText}]({relToRoot})";
        }

                string normalized = pathPart.Replace('\\', '/').TrimEnd('/');
        string lastSegment = Path.GetFileName(normalized);
        bool wantsReadme = lastSegment.Equals("README.md", StringComparison.OrdinalIgnoreCase) || !lastSegment.EndsWith(".md", StringComparison.OrdinalIgnoreCase);

        string searchName = lastSegment.Equals("README.md", StringComparison.OrdinalIgnoreCase)
            ? Path.GetFileName(Path.GetDirectoryName(normalized) ?? "")
            : Path.GetFileNameWithoutExtension(lastSegment);

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            if (wantsReadme)
            {
                var exactFolders = allNodes
                    .Where(n => n.Depth >= 1 && !n.IsEmpty && n.FolderName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (exactFolders.Count == 1)
                {
                    string targetDisk = Path.Combine(rootDir, exactFolders.First().RelativePath.Replace('/', Path.DirectorySeparatorChar), "README.md");
                    if (File.Exists(targetDisk))
                        return $"[{linkText}]({Path.GetRelativePath(fileDir, targetDisk).Replace('\\', '/')})";
                }

                var targetTokens = Tokenize(searchName);
                if (targetTokens.Count > 0)
                {
                    var bestFolder = allNodes
                        .Where(n => n.Depth >= 1 && !n.IsEmpty)
                        .Select(n => new
                        {
                            Node = n,
                            Overlap = Tokenize(n.FolderName).Intersect(targetTokens).Count(),
                            Total = Tokenize(n.FolderName).Count
                        })
                        .Where(x => x.Overlap >= Math.Max(1, targetTokens.Count / 2))
                        .OrderByDescending(x => (double)x.Overlap / Math.Max(1, x.Total))
                        .ThenByDescending(x => x.Overlap)
                        .FirstOrDefault();

                    if (bestFolder != null)
                    {
                        string targetDisk = Path.Combine(rootDir, bestFolder.Node.RelativePath.Replace('/', Path.DirectorySeparatorChar), "README.md");
                        if (File.Exists(targetDisk))
                            return $"[{linkText}]({Path.GetRelativePath(fileDir, targetDisk).Replace('\\', '/')})";
                    }
                }
            }
            else
            {
                var exactFiles = allLessons
                    .Where(l => Path.GetFileNameWithoutExtension(l.FileName).Equals(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (exactFiles.Count == 1)
                {
                    string targetDisk = Path.Combine(rootDir, exactFiles.First().RelativePath.Replace('/', Path.DirectorySeparatorChar));
                    return $"[{linkText}]({Path.GetRelativePath(fileDir, targetDisk).Replace('\\', '/')})";
                }

                var prefixMatch = Regex.Match(searchName, @"^(\d{2}-\d{2}(?:-\d{2})?)-");
                string? numPrefix = prefixMatch.Success ? prefixMatch.Groups.Cast<Group>().ElementAt(1).Value : null;
                var fileTokens = Tokenize(searchName);
                string relSourceDir = Path.GetRelativePath(rootDir, fileDir).Replace('\\', '/');

                var bestFile = allLessons
                    .Select(l =>
                    {
                        string lNameNoExt = Path.GetFileNameWithoutExtension(l.FileName);
                        var lTokens = Tokenize(lNameNoExt);
                        int overlap = lTokens.Intersect(fileTokens).Count();
                        bool samePrefix = numPrefix != null && lNameNoExt.StartsWith(numPrefix + "-");
                        bool sameFolder = l.RelativePath.StartsWith(relSourceDir + "/", StringComparison.OrdinalIgnoreCase);
                        int score = overlap * 10 + (samePrefix ? 15 : 0) + (sameFolder ? 8 : 0);
                        return new { Lesson = l, Score = score, Overlap = overlap, SamePrefix = samePrefix };
                    })
                    .Where(x => x.Score >= 20 && (x.Overlap >= 2 || (x.SamePrefix && x.Overlap >= 1)))
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();

                if (bestFile != null)
                {
                    string targetDisk = Path.Combine(rootDir, bestFile.Lesson.RelativePath.Replace('/', Path.DirectorySeparatorChar));
                    return $"[{linkText}]({Path.GetRelativePath(fileDir, targetDisk).Replace('\\', '/')})";
                }
            }
        }

        return $"**{linkText}** *(в разработка)*";
    }

    private static string ToGitHubAnchorSlug(string headingText)
    {
        string s = Regex.Replace(headingText, @"\[([^\]]+)\]\([^)]+\)", "$1");
        s = s.ToLowerInvariant().Trim();
        s = Regex.Replace(s, @"[^\p{L}\p{N}\s\-_]", "");
        s = s.Replace(' ', '-').Trim('-');
        return s;
    }

    private static HashSet<string> ExtractFileAnchors(string mdFullPath)
    {
        var anchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(mdFullPath)) return anchors;

        var slugCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        bool inCodeBlock = false;

        foreach (var rawLine in File.ReadLines(mdFullPath, Encoding.UTF8))
        {
            string line = rawLine.Trim();
            if (line.StartsWith("```")) { inCodeBlock = !inCodeBlock; continue; }
            if (inCodeBlock) continue;

            var m = Regex.Match(line, @"^#{1,6}\s+(.+)$");
            if (m.Success)
            {
                string baseSlug = ToGitHubAnchorSlug(m.Groups.Cast<Group>().ElementAt(1).Value);
                if (string.IsNullOrEmpty(baseSlug)) continue;

                if (!slugCounts.TryGetValue(baseSlug, out int count))
                {
                    slugCounts[baseSlug] = 0;
                    anchors.Add(baseSlug);
                }
                else
                {
                    count++;
                    slugCounts[baseSlug] = count;
                    anchors.Add($"{baseSlug}-{count}");
                }
            }
        }
        return anchors;
    }

    private static HashSet<string> Tokenize(string input)
    {
        return Regex.Split(input.ToLowerInvariant(), @"[^a-z0-9а-я]+")
            .Where(t => t.Length >= 2 && t != "md" && t != "readme" && t != "and" && t != "the" && t != "for" && t != "in" && t != "to")
            .ToHashSet();
    }
}

