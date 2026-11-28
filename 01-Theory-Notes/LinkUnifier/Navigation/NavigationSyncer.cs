using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class NavigationSyncer
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void SyncAll(FolderNode rootNode, string rootDir, AuditStats stats)
    {
        stats.NavSyncLog.Clear();
        SyncRecursive(rootNode, rootNode, rootDir, stats);
    }

    private static void SyncRecursive(FolderNode node, FolderNode rootTree, string rootDir, AuditStats stats)
    {
        if (node.IsEmpty) return;

        string nodeFullDir = string.IsNullOrEmpty(node.RelativePath)
            ? rootDir
            : Path.Combine(rootDir, node.RelativePath.Replace('/', Path.DirectorySeparatorChar));

        if (node.Depth == 0)
        {
            string rootReadme = Path.Combine(nodeFullDir, "README.md");
            if (File.Exists(rootReadme))
            {
                string orig = File.ReadAllText(rootReadme, Encoding.UTF8);
                var lines = orig.Replace("\r\n", "\n").Split('\n').ToList();
                while (lines.Count > 0 && (string.IsNullOrWhiteSpace(lines.Last()) || lines.Last().Trim().StartsWith("© 2026")))
                    lines.RemoveAt(lines.Count - 1);

                lines.Add("");
                lines.Add("© 2026 Dev-Library / Инженерна Библиотека и Технологичен Архив (`01-Theory-Notes`)");
                string updated = string.Join(Environment.NewLine, lines) + Environment.NewLine;
                if (updated != orig)
                {
                    File.WriteAllText(rootReadme, updated, Utf8NoBom);
                    stats.UpdatedNavFiles++;
                    stats.NavSyncLog.Add("[{DateTime.Now:ddd HH:mm:ss}] 🔄 [ОБНОВЕН FOOTER] README.md");
                }
            }
        }

        if (node.Depth >= 2)
        {
            var domainNode = GetDomainAncestor(node);
            var parentSiblings = node.Parent?.SubFolders.Where(s => !s.IsEmpty).ToDictionary(s => s.FolderName)
                                 ?? new Dictionary<string, FolderNode>();

            var modNavParts = new List<string>();
            if (node.PrevSiblingFolder != null && parentSiblings.TryGetValue(node.PrevSiblingFolder, out var prevSib))
                modNavParts.Add($"[📦 Предишен модул: {prevSib.FolderName}](../{prevSib.FolderName}/README.md)");

            if (node.Depth > 2 && node.Parent != null)
                modNavParts.Add($"[📂 Родителски модул: {ShortLabel(node.Parent.Title)}](../README.md)");

            modNavParts.Add($"[🌐 Домейн: {ShortLabel(domainNode.Title)}]({GoUp(node.Depth - 1)}README.md)");
            modNavParts.Add($"[📚 Инженерна Библиотека 01-Theory-Notes]({GoUp(node.Depth)}README.md)");
            modNavParts.Add($"[🏛️ Начало Dev-Library]({GoUp(node.Depth + 1)}README.md)");
            modNavParts.Add($"[📂 Текуща папка: {ShortLabel(node.Title)}](README.md)");

            if (node.NextSiblingFolder != null && parentSiblings.TryGetValue(node.NextSiblingFolder, out var nextSib))
                modNavParts.Add($"[📦 Следващ модул: {nextSib.FolderName}](../{nextSib.FolderName}/README.md)");

            if (domainNode.NextSiblingFolder != null && rootTree.SubFolders.Any(d => d.FolderName == domainNode.NextSiblingFolder && !d.IsEmpty))
            {
                var nextDom = rootTree.SubFolders.First(d => d.FolderName == domainNode.NextSiblingFolder);
                modNavParts.Add($"[🌐 Следващ домейн: {ShortLabel(nextDom.Title)}]({GoUp(node.Depth)}{nextDom.FolderName}/README.md)");
            }

            if (node.Lessons.Count > 0)
            {
                var firstLesson = node.Lessons.First();
                modNavParts.Add($"[▶️ Започни от {ShortLabel(firstLesson.Title)}]({firstLesson.FileName})");
            }
            else if (node.SubFolders.Any(s => !s.IsEmpty))
            {
                var firstSub = node.SubFolders.First(s => !s.IsEmpty);
                modNavParts.Add($"[▶️ Започни от {ShortLabel(firstSub.Title)}]({firstSub.FolderName}/README.md)");
            }

            string newModNavLine = string.Join(" | ", modNavParts);
            string readmePath = Path.Combine(nodeFullDir, "README.md");
            string relReadme = $"{node.RelativePath}/README.md";

            if (!File.Exists(readmePath))
            {
                var sb = new StringBuilder();
                sb.AppendLine(newModNavLine);
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
                sb.AppendLine($"# 📦 {node.Title}");
                sb.AppendLine();
                sb.AppendLine("Индексен хъб на модула и вложените в него под-модули.");
                sb.AppendLine();
                if (node.SubFolders.Any(s => !s.IsEmpty))
                {
                    sb.AppendLine("## 📂 Под-модули");
                    foreach (var sf in node.SubFolders.Where(s => !s.IsEmpty))
                        sb.AppendLine($"* **[{sf.Title}]({sf.FolderName}/README.md)**");
                    sb.AppendLine();
                }
                if (node.Lessons.Count > 0)
                {
                    sb.AppendLine("## 📚 Уроци и Справочници");
                    foreach (var ls in node.Lessons)
                        sb.AppendLine($"* [{ls.Title}]({ls.FileName})");
                    sb.AppendLine();
                }
                sb.AppendLine("---");
                sb.AppendLine();
                sb.AppendLine(newModNavLine);
                sb.AppendLine();
                sb.AppendLine($"© 2026 Dev-Library / {node.Title}");

                File.WriteAllText(readmePath, sb.ToString(), Utf8NoBom);
                stats.CreatedReadmes++;
                stats.NavSyncLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] ➕ [СЪЗДАДЕН README + NAVBAR] {relReadme}");
            }
            else
            {
                string orig = File.ReadAllText(readmePath, Encoding.UTF8);
                string updated = RebuildFullNavigation(orig, newModNavLine, node.Title);
                if (updated != orig)
                {
                    File.WriteAllText(readmePath, updated, Utf8NoBom);
                    stats.UpdatedNavFiles++;
                    stats.NavSyncLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] 🔄 [ОБНОВЕН NAVBAR/FOOTER] {relReadme}");
                }
            }

            foreach (var lesson in node.Lessons)
            {
                string lessonPath = Path.Combine(nodeFullDir, lesson.FileName);
                string orig = File.ReadAllText(lessonPath, Encoding.UTF8);
                var navParts = new List<string>();

                if (lesson.PrevLessonFile != null)
                {
                    var prevL = node.Lessons.First(l => l.FileName == lesson.PrevLessonFile);
                    navParts.Add($"[⬅️ {ShortLabel(prevL.Title)}]({prevL.FileName})");
                }
                else if (node.PrevSiblingFolder != null && parentSiblings.TryGetValue(node.PrevSiblingFolder, out var pSib1))
                {
                    navParts.Add($"[📦 Към предишен модул: {pSib1.FolderName}](../{pSib1.FolderName}/README.md)");
                }

                navParts.Add($"[📂 Назад към {ShortLabel(node.Title)} Хъба](README.md)");

                if (node.PrevSiblingFolder != null && parentSiblings.TryGetValue(node.PrevSiblingFolder, out var pSib2))
                    navParts.Add($"[📦 Предишен модул: {pSib2.FolderName}](../{pSib2.FolderName}/README.md)");

                if (node.Depth > 2 && node.Parent != null)
                    navParts.Add($"[📂 Родителски модул: {ShortLabel(node.Parent.Title)}](../README.md)");

                navParts.Add($"[🌐 Домейн: {ShortLabel(domainNode.Title)}]({GoUp(node.Depth - 1)}README.md)");
                navParts.Add($"[📚 Инженерна Библиотека 01-Theory-Notes]({GoUp(node.Depth)}README.md)");
                navParts.Add($"[🏛️ Начало Dev-Library]({GoUp(node.Depth + 1)}README.md)");
                navParts.Add($"[📂 Текуща папка: {ShortLabel(node.Title)}](README.md)");

                if (domainNode.NextSiblingFolder != null && rootTree.SubFolders.Any(d => d.FolderName == domainNode.NextSiblingFolder && !d.IsEmpty))
                {
                    var nextDom = rootTree.SubFolders.First(d => d.FolderName == domainNode.NextSiblingFolder);
                    navParts.Add($"[🌐 Следващ домейн: {ShortLabel(nextDom.Title)}]({GoUp(node.Depth)}{nextDom.FolderName}/README.md)");
                }

                if (node.NextSiblingFolder != null && parentSiblings.TryGetValue(node.NextSiblingFolder, out var nSib1))
                    navParts.Add($"[📦 Следващ модул: {nSib1.FolderName}](../{nSib1.FolderName}/README.md)");

                if (lesson.NextLessonFile != null)
                {
                    var nextL = node.Lessons.First(l => l.FileName == lesson.NextLessonFile);
                    navParts.Add($"[➡️ {ShortLabel(nextL.Title)}]({nextL.FileName})");
                }
                else if (node.NextSiblingFolder != null && parentSiblings.TryGetValue(node.NextSiblingFolder, out var nSib2))
                {
                    navParts.Add($"[📦 Към следващ модул: {nSib2.FolderName}](../{nSib2.FolderName}/README.md)");
                }

                string updated = RebuildFullNavigation(orig, string.Join(" | ", navParts), lesson.Title);
                if (updated != orig)
                {
                    File.WriteAllText(lessonPath, updated, Utf8NoBom);
                    stats.UpdatedNavFiles++;
                    stats.NavSyncLog.Add($" [{DateTime.Now:ddd HH:mm:ss}] 🔄 [ОБНОВЕН NAVBAR/FOOTER] {lesson.RelativePath}");
                }
            }
        }

        foreach (var child in node.SubFolders)
            SyncRecursive(child, rootTree, rootDir, stats);
    }

        private static string ShortLabel(string title)
    {
        string s = Regex.Replace(title, @"\s*\([^)]*\)\s*$", "").Trim();
        return s.Length > 55 ? s.Substring(0, 52) + "..." : s;
    }

    private static string GoUp(int levels) => levels <= 0 ? "" : string.Concat(Enumerable.Repeat("../", levels));

    private static FolderNode GetDomainAncestor(FolderNode node)
    {
        var curr = node;
        while (curr.Parent != null && curr.Depth > 1) curr = curr.Parent;
        return curr;
    }

    private static bool IsNavBarLine(string line)
    {
        string t = line.Trim();
        if (!t.Contains("](")) return false;
        if (t.StartsWith("*") || t.StartsWith("-") || t.StartsWith("#") || t.StartsWith(">") || t.StartsWith("|")) return false;
        return t.Count(c => c == '|') >= 1 || t.Contains("README.md") || t.Contains("⬅️") || t.Contains("➡️") || t.Contains("🌐") || t.Contains("📦") || t.Contains("🏠");
    }

    private static string RebuildFullNavigation(string content, string newNavLine, string footerTitle)
    {
        var lines = content.Replace("\r\n", "\n").Split('\n').ToList();
        int h1Index = lines.FindIndex(l => l.StartsWith("# "));
        if (h1Index >= 0)
        {
            for (int i = h1Index - 1; i >= 0; i--)
            {
                string t = lines.ElementAt(i).Trim();
                if (string.IsNullOrEmpty(t) || t == "---" || IsNavBarLine(t)) lines.RemoveAt(i);
            }
            lines.Insert(0, "");
            lines.Insert(0, "---");
            lines.Insert(0, "");
            lines.Insert(0, newNavLine);
        }

        while (lines.Count > 0)
        {
            string last = lines.Last().Trim();
            if (string.IsNullOrEmpty(last) || last.StartsWith("© 2026") || IsNavBarLine(last))
                lines.RemoveAt(lines.Count - 1);
            else if (last == "---")
            {
                lines.RemoveAt(lines.Count - 1);
                break;
            }
            else break;
        }

        lines.Add("");
        lines.Add("---");
        lines.Add("");
        lines.Add(newNavLine);
        lines.Add("");
        lines.Add($"© 2026 Dev-Library / {footerTitle}");
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
