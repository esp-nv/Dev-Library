using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class TreeScanner
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static int NormalizeSyntaxFileNames(string rootDir)
    {
        int count = 0;
        foreach (var file in Directory.GetFiles(rootDir, "*-0?-*-Syntax.md", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            var m = Regex.Match(fileName, @"^(\d{2}-\d{2}-0[1-9])-(.+)-Syntax\.md$");
            if (m.Success)
            {
                string dir = Path.GetDirectoryName(file)!;
                string prefix = m.Groups.Cast<Group>().ElementAt(1).Value;
                string topic = m.Groups.Cast<Group>().ElementAt(2).Value;
                File.Move(file, Path.Combine(dir, $"{prefix}-Syntax-{topic}.md"), overwrite: true);
                count++;
            }
        }
        return count;
    }

    public static FolderNode BuildAndSaveTree(string rootDir, string mapJsonPath, string? visualMapMdPath = null)
    {
        var tree = BuildTreePostOrderDFS(rootDir, rootDir, 0, null);
        tree.GeneratedAt = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new System.Globalization.CultureInfo("bg-BG"));


        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        File.WriteAllText(mapJsonPath, JsonSerializer.Serialize(tree, jsonOptions), Utf8NoBom);

        if (!string.IsNullOrWhiteSpace(visualMapMdPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(visualMapMdPath)!);
            WriteVisualTreeMarkdown(tree, visualMapMdPath);
        }

        return tree;
    }

    public static void WriteVisualTreeMarkdown(FolderNode rootTree, string mdOutputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🌲 Визуална Карта и Йерархия на Библиотеката (`01-Theory-Notes`)");
        sb.AppendLine();
        sb.AppendLine($"> **Генерирано на:** `{rootTree.GeneratedAt}` | **Общо домейни:** `{rootTree.SubFolders.Count}`");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 📊 1. Обобщена Таблица по Домейни и Модули");
        sb.AppendLine();
        sb.AppendLine("| № | Домейн | Папка | Модули | Уроци (`.md`) | Статус |");
        sb.AppendLine("| :--- | :--- | :--- | :---: | :---: | :--- |");

        int idx = 1;
        int totalLessons = 0;
        foreach (var domain in rootTree.SubFolders)
        {
            int domainLessons = CountLessonsRecursive(domain);
            totalLessons += domainLessons;
            string status = domain.IsEmpty ? "⚠️ Празна" : "✅ Активен";
            sb.AppendLine($"| {idx++} | **{domain.Title}** | `{domain.FolderName}` | {domain.SubFolders.Count} | {domainLessons} | {status} |");
        }
        sb.AppendLine($"| — | **ОБЩО ЗА БИБЛИОТЕКАТА** | `01-Theory-Notes` | **{rootTree.SubFolders.Sum(d => d.SubFolders.Count)}** | **{totalLessons}** | — |");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 🗂️ 2. Пълна Йерархична Структура (Post-Order DFS)");
        sb.AppendLine();

        foreach (var domain in rootTree.SubFolders)
        {
            AppendNodeHierarchy(sb, domain, 0);
        }

        File.WriteAllText(mdOutputPath, sb.ToString(), Utf8NoBom);
    }

    private static void AppendNodeHierarchy(StringBuilder sb, FolderNode node, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        string icon = node.IsEmpty ? "⚠️" : (node.Depth == 1 ? "🌐" : "📦");
        sb.AppendLine($"{indent}* {icon} **{node.Title}** (`{node.FolderName}/`) — *{node.Lessons.Count} урока*");

        foreach (var lesson in node.Lessons)
        {
            sb.AppendLine($"{indent}  * 📄 [{lesson.Title}](../01-Theory-Notes/{lesson.RelativePath}) (`{lesson.FileName}`)");
        }

        foreach (var sub in node.SubFolders)
        {
            AppendNodeHierarchy(sb, sub, indentLevel + 1);
        }
    }

    private static int CountLessonsRecursive(FolderNode node)
    {
        return node.Lessons.Count + node.SubFolders.Sum(CountLessonsRecursive);
    }

    private static FolderNode BuildTreePostOrderDFS(string fullPath, string rootDir, int depth, FolderNode? parent)
    {
        string folderName = Path.GetFileName(fullPath);
        string relPath = depth == 0 ? "" : Path.GetRelativePath(rootDir, fullPath).Replace('\\', '/');

        var node = new FolderNode
        {
            Depth = depth,
            FolderName = folderName,
            RelativePath = relPath,
            Parent = parent
        };

        var excludedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LinkUnifier", "bin", "obj", ".git", ".vs", "node_modules", "reports"
        };

        var childDirs = Directory.GetDirectories(fullPath)
            .Where(d =>
            {
                string name = Path.GetFileName(d);
                if (name.StartsWith(".") || excludedFolders.Contains(name)) return false;
                if (depth == 0) return Regex.IsMatch(name, @"^\d{2}-");
                return true;
            })
            .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (int i = 0; i < childDirs.Count; i++)
        {
            var childNode = BuildTreePostOrderDFS(childDirs.ElementAt(i), rootDir, depth + 1, node);
            childNode.PrevSiblingFolder = i > 0 ? Path.GetFileName(childDirs.ElementAt(i - 1)) : null;
            childNode.NextSiblingFolder = i < childDirs.Count - 1 ? Path.GetFileName(childDirs.ElementAt(i + 1)) : null;
            node.SubFolders.Add(childNode);
        }

        string[] allFilesInDir = Directory.GetFiles(fullPath);
        var mdFiles = allFilesInDir
            .Where(f => f.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
                        !Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .ToList();

        bool hasAnyContent = allFilesInDir.Length > 0 || node.SubFolders.Any(sf => !sf.IsEmpty);
        if (!hasAnyContent && depth > 0)
        {
            node.IsEmpty = true;
            node.Title = folderName;
            return node;
        }

        node.IsEmpty = false;
        node.Title = ExtractTitle(Path.Combine(fullPath, "README.md"), folderName, depth);

        if (depth > 0)
        {
            for (int i = 0; i < mdFiles.Count; i++)
            {
                string lFile = mdFiles.ElementAt(i);
                string lName = Path.GetFileName(lFile);
                node.Lessons.Add(new LessonNode
                {
                    FileName = lName,
                    RelativePath = $"{relPath}/{lName}",
                    Title = ExtractTitle(lFile, Path.GetFileNameWithoutExtension(lName), depth),
                    PrevLessonFile = i > 0 ? Path.GetFileName(mdFiles.ElementAt(i - 1)) : null,
                    NextLessonFile = i < mdFiles.Count - 1 ? Path.GetFileName(mdFiles.ElementAt(i + 1)) : null
                });
            }
        }

        return node;
    }

    public static string ExtractTitle(string mdPath, string fallback, int depth)
    {
        if (!File.Exists(mdPath)) return fallback;
        var h1 = File.ReadLines(mdPath, Encoding.UTF8).FirstOrDefault(l => l.StartsWith("# "));
        if (h1 == null) return fallback;
        string clean = Regex.Replace(h1.TrimStart('#', ' '), @"^[^\w\d]+", "").Trim();
        if (string.IsNullOrWhiteSpace(clean)) return fallback;
        if (depth > 0 && clean.Contains("Инженерна Библиотека")) return fallback;
        return clean;
    }

    public static void CollectAllNodes(FolderNode node, List<FolderNode> list)
    {
        list.Add(node);
        foreach (var sf in node.SubFolders) CollectAllNodes(sf, list);
    }

    public static void CollectEmptyNodes(FolderNode node, List<string> emptyList)
    {
        if (node.Depth >= 1 && node.IsEmpty) emptyList.Add(node.RelativePath);
        foreach (var child in node.SubFolders) CollectEmptyNodes(child, emptyList);
    }
}
