using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class TreeScanner
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static FolderNode BuildAndSaveTree(string rootNotesDir, string jsonOutputPath, string? mdOutputPath = null)
    {
        var root = ScanNode(rootNotesDir, rootNotesDir, 0, null);
        LinkSiblings(root);

        var json = JsonSerializer.Serialize(root, new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        });
        File.WriteAllText(jsonOutputPath, json, Utf8NoBom);

        if (!string.IsNullOrEmpty(mdOutputPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mdOutputPath)!);
            TreeMapExporter.ExportMarkdown(root, mdOutputPath);
        }

        return root;
    }

    public static FolderNode ScanPostOrder(string dirPath, string rootNotesDir)
    {
        var root = ScanNode(dirPath, rootNotesDir, 0, null);
        LinkSiblings(root);
        return root;
    }

    private static FolderNode ScanNode(string dirPath, string rootNotesDir, int depth, FolderNode? parent)
    {
        string relPath = Path.GetRelativePath(rootNotesDir, dirPath).Replace('\\', '/');
        var node = new FolderNode
        {
            FullPath = dirPath,
            RelativePath = relPath,
            FolderName = Path.GetFileName(dirPath),
            Depth = depth,
            Parent = parent,
            HasReadme = File.Exists(Path.Combine(dirPath, "README.md"))
        };

        foreach (var subDir in Directory.GetDirectories(dirPath).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
        {
            string name = Path.GetFileName(subDir);
            if (name.StartsWith(".") ||
                name.Equals("assets", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("LinkUnifier", StringComparison.OrdinalIgnoreCase))
                continue;

            node.SubFolders.Add(ScanNode(subDir, rootNotesDir, depth + 1, node));
        }

        foreach (var file in Directory.GetFiles(dirPath, "*.md").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            string fName = Path.GetFileName(file);
            if (fName.Equals("README.md", StringComparison.OrdinalIgnoreCase))
                continue;

            node.Lessons.Add(new LessonInfo
            {
                FullPath = file,
                RelativePath = Path.GetRelativePath(rootNotesDir, file).Replace('\\', '/'),
                FileName = fName,
                Title = ExtractH1Title(file, fName)
            });
        }

        for (int i = 0; i < node.Lessons.Count; i++)
        {
            if (i > 0) node.Lessons[i].PrevLessonFile = node.Lessons[i - 1].FileName;
            if (i < node.Lessons.Count - 1) node.Lessons[i].NextLessonFile = node.Lessons[i + 1].FileName;
        }

        node.Title = node.HasReadme
            ? ExtractH1Title(Path.Combine(dirPath, "README.md"), node.FolderName)
            : node.FolderName;

        return node;
    }

    private static void LinkSiblings(FolderNode node)
    {
        for (int i = 0; i < node.SubFolders.Count; i++)
        {
            if (i > 0) node.SubFolders[i].PrevSiblingFolder = node.SubFolders[i - 1].RelativePath;
            if (i < node.SubFolders.Count - 1) node.SubFolders[i].NextSiblingFolder = node.SubFolders[i + 1].RelativePath;
            LinkSiblings(node.SubFolders[i]);
        }
    }

    public static (int modules, int lessons) CountDomainItems(FolderNode domain)
    {
        int mods = domain.SubFolders.Count(sf => sf.SubFolders.Count == 0)
                   + domain.SubFolders.Sum(sf => sf.SubFolders.Count);
        int less = domain.Lessons.Count + domain.SubFolders.Sum(CountLessonsRec);
        return (mods, less);
    }

    public static int CountLessonsRec(FolderNode node)
        => node.Lessons.Count + node.SubFolders.Sum(CountLessonsRec);

    public static void CollectEmptyNodes(FolderNode node, List<string> emptyPaths)
    {
        foreach (var sub in node.SubFolders)
            CollectEmptyNodes(sub, emptyPaths);

        if (node.SubFolders.Count == 0 && node.Lessons.Count == 0 && node.RelativePath != ".")
            emptyPaths.Add(node.RelativePath);
    }

    public static string ExtractH1Title(string filePath, string fallback)
    {
        if (!File.Exists(filePath)) return fallback;
        foreach (var line in File.ReadLines(filePath))
        {
            var t = line.Trim();
            if (t.StartsWith("# "))
                return Regex.Replace(t.Substring(2), @"^[^\p{L}\p{N}]+", "").Trim();
        }
        return fallback;
    }
}
