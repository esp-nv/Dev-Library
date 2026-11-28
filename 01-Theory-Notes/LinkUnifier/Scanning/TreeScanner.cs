using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class TreeScanner
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static FolderNode BuildAndSaveTree(
        string rootNotesDir,
        string jsonOutputPath,
        string? mdOutputPath = null,
        LinkUnifierConfig? cfg = null)
    {
        cfg ??= new LinkUnifierConfig();
        var root = ScanNode(rootNotesDir, rootNotesDir, 0, null, cfg);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        string json = JsonSerializer.Serialize(root, options);
        File.WriteAllText(jsonOutputPath, json, Utf8NoBom);

        if (!string.IsNullOrWhiteSpace(mdOutputPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mdOutputPath)!);
            TreeMapExporter.ExportMarkdown(root, mdOutputPath);
        }

        return root;
    }

    private static FolderNode ScanNode(
        string dirPath,
        string rootNotesDir,
        int depth,
        FolderNode? parent,
        LinkUnifierConfig cfg)
    {
        string relPath = Path.GetRelativePath(rootNotesDir, dirPath).Replace('\\', '/');
        if (relPath == ".") relPath = "";

        string readmePath = Path.Combine(dirPath, "README.md");
        bool hasReadme = File.Exists(readmePath);
        string title = hasReadme
            ? ExtractTitle(readmePath, Path.GetFileName(dirPath))
            : Path.GetFileName(dirPath);

        var node = new FolderNode
        {
            FolderName = Path.GetFileName(dirPath),
            RelativePath = relPath,
            Depth = depth,
            HasReadme = hasReadme,
            DisplayTitle = title,
            Parent = parent
        };

        foreach (var file in Directory.GetFiles(dirPath, "*.md").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            string fileName = Path.GetFileName(file);
            if (fileName.Equals("README.md", StringComparison.OrdinalIgnoreCase))
                continue;

            node.Lessons.Add(new()
            {
                FileName = fileName,
                RelativePath = Path.GetRelativePath(rootNotesDir, file).Replace('\\', '/'),
                Title = ExtractTitle(file, Path.GetFileNameWithoutExtension(file))
            });
        }

        foreach (var subDir in Directory.GetDirectories(dirPath).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
        {
            string name = Path.GetFileName(subDir);
            if (name.StartsWith(".") || cfg.IgnoredFolders.Contains(name, StringComparer.OrdinalIgnoreCase))
                continue;

            node.SubFolders.Add(ScanNode(subDir, rootNotesDir, depth + 1, node, cfg));
        }

        return node;
    }

    public static string ExtractTitle(string mdPath, string fallback)
    {
        if (!File.Exists(mdPath)) return fallback;
        foreach (var line in File.ReadLines(mdPath).Take(30))
        {
            var m = Regex.Match(line.Trim(), @"^#{1,2}\s+(.+)$");
            if (m.Success)
                return m.Groups.Cast<Group>().ElementAt(1).Value.Trim();
        }
        return fallback;
    }

    public static void CollectEmptyNodes(FolderNode node, List<string> emptyPaths)
    {
        foreach (var sub in node.SubFolders)
            CollectEmptyNodes(sub, emptyPaths);

        if (node.Depth > 0 && node.Lessons.Count == 0 && node.SubFolders.Count == 0)
            emptyPaths.Add(node.RelativePath);
    }

    public static (int modules, int lessons) CountDomainItems(FolderNode node)
    {
        int mods = node.SubFolders.Count;
        int lessons = node.Lessons.Count;
        foreach (var sub in node.SubFolders)
        {
            var (subMods, subLessons) = CountDomainItems(sub);
            mods += subMods;
            lessons += subLessons;
        }
        return (mods, lessons);
    }
}
