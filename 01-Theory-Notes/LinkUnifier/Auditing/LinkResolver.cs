using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class LinkResolver
{
    public static string FindBestReplacement(
        string currentFile,
        string linkText,
        string rawTarget,
        string rootNotesDir,
        List<string> allValidFiles,
        out string reason)
    {
        string[] parts = rawTarget.Split('#');
        string targetPathPart = parts.First().Trim();
        string anchorPart = parts.Length > 1 ? "#" + parts.ElementAt(1) : "";
        string currentDir = Path.GetDirectoryName(currentFile) ?? rootNotesDir;

        if (targetPathPart.Contains("README.md", StringComparison.OrdinalIgnoreCase) &&
            (linkText.Contains("Начало", StringComparison.OrdinalIgnoreCase) || rawTarget.Contains("../../../README.md")))
        {
            string rootReadme = Path.Combine(rootNotesDir, "README.md");
            if (File.Exists(rootReadme))
            {
                reason = "Коригиран път към главния README.md";
                return $"[{linkText}]({MakeRelative(currentDir, rootReadme)}{anchorPart})";
            }
        }

        bool isFolderLink = targetPathPart.EndsWith("/") || targetPathPart.EndsWith("\\");
        string normalizedPath = targetPathPart.Replace('\\', '/').TrimEnd('/');
        string lastSegment = normalizedPath.Split('/').LastOrDefault() ?? "";

        if (isFolderLink || lastSegment.Equals("README.md", StringComparison.OrdinalIgnoreCase))
        {
            string folderName = lastSegment.Equals("README.md", StringComparison.OrdinalIgnoreCase)
                ? normalizedPath.Split('/').Reverse().Skip(1).FirstOrDefault() ?? ""
                : lastSegment;

            if (!string.IsNullOrEmpty(folderName) && folderName != "." && folderName != "..")
            {
                string strippedTargetFolder = StripNumericPrefix(folderName);
                var allReadmes = allValidFiles
                    .Where(f => Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var folderMatch = allReadmes.FirstOrDefault(f =>
                    Path.GetFileName(Path.GetDirectoryName(f) ?? "").Equals(folderName, StringComparison.OrdinalIgnoreCase) ||
                    StripNumericPrefix(Path.GetFileName(Path.GetDirectoryName(f) ?? "")).Equals(strippedTargetFolder, StringComparison.OrdinalIgnoreCase));

                if (folderMatch == null)
                {
                    var modPrefix = Regex.Match(folderName, @"^(Module-\d{2}-)", RegexOptions.IgnoreCase);
                    if (modPrefix.Success)
                    {
                        string prefix = modPrefix.Groups.Cast<Group>().ElementAt(1).Value;
                        var modCandidates = allReadmes
                            .Where(f => Path.GetFileName(Path.GetDirectoryName(f) ?? "").StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(f => AnchorHelper.LevenshteinDistance(Path.GetFileName(Path.GetDirectoryName(f) ?? ""), folderName))
                            .ToList();

                        if (modCandidates.Count > 0 &&
                            AnchorHelper.LevenshteinDistance(Path.GetFileName(Path.GetDirectoryName(modCandidates.First()) ?? ""), folderName) <= 18)
                        {
                            folderMatch = modCandidates.First();
                        }
                    }
                }

                if (folderMatch != null)
                {
                    reason = $"Намерена папка и нейният README.md ('{Path.GetFileName(Path.GetDirectoryName(folderMatch))}')";
                    return $"[{linkText}]({MakeRelative(currentDir, folderMatch)}{anchorPart})";
                }
            }
        }
        else if (!string.IsNullOrEmpty(lastSegment))
        {
            var exact = allValidFiles.FirstOrDefault(f => Path.GetFileName(f).Equals(lastSegment, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                reason = "Файлът е намерен на нов път в библиотеката";
                return $"[{linkText}]({MakeRelative(currentDir, exact)}{anchorPart})";
            }

            var prefixMatch = Regex.Match(lastSegment, @"^(\d{2}-\d{2}(?:-\d{2})?)");
            if (prefixMatch.Success)
            {
                string prefix = prefixMatch.Groups.Cast<Group>().ElementAt(1).Value;
                var candidates = allValidFiles
                    .Where(f => Path.GetFileName(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (candidates.Count == 1)
                {
                    reason = $"Намерен файл с префикс '{prefix}'";
                    return $"[{linkText}]({MakeRelative(currentDir, candidates.First())}{anchorPart})";
                }
                if (candidates.Count > 1)
                {
                    var best = candidates.OrderBy(c => AnchorHelper.LevenshteinDistance(Path.GetFileName(c), lastSegment)).First();
                    reason = $"Намерен най-близък файл с префикс '{prefix}'";
                    return $"[{linkText}]({MakeRelative(currentDir, best)}{anchorPart})";
                }
            }
        }

        reason = "Целевият файл не съществува -> маркиране като *(в разработка)*";
        return $"**{linkText}** *(в разработка)*";
    }

    public static List<string> FindTopSuggestions(string currentFile, string rawTarget, string rootNotesDir, LinkUnifierConfig? cfg = null)
    {
        cfg ??= new LinkUnifierConfig();
        string currentDir = Path.GetDirectoryName(currentFile) ?? rootNotesDir;
        string targetPathPart = rawTarget.Split('#').First().Trim().Replace('\\', '/').TrimEnd('/');
        string searchKey = targetPathPart.EndsWith("/README.md", StringComparison.OrdinalIgnoreCase)
            ? targetPathPart.Substring(0, targetPathPart.Length - "/README.md".Length).Split('/').LastOrDefault() ?? "README.md"
            : targetPathPart.Split('/').LastOrDefault() ?? targetPathPart;

        var allMd = Directory.GetFiles(rootNotesDir, "*.md", SearchOption.AllDirectories)
            .Where(f =>
            {
                var relParts = Path.GetRelativePath(rootNotesDir, f)
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return !relParts.Any(p => p.StartsWith(".") || cfg.IgnoredFolders.Contains(p, StringComparer.OrdinalIgnoreCase));
            })
            .ToList();

        return allMd
            .OrderBy(f =>
            {
                string compareName = Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileName(Path.GetDirectoryName(f) ?? "")
                    : Path.GetFileName(f);
                return AnchorHelper.LevenshteinDistance(compareName.ToLowerInvariant(), searchKey.ToLowerInvariant());
            })
            .Take(5)
            .Select(f => MakeRelative(currentDir, f))
            .ToList();
    }

    private static string StripNumericPrefix(string name)
        => Regex.Replace(name, @"^\d{2}-", "").Trim();

    public static string MakeRelative(string fromDir, string toFile)
        => Path.GetRelativePath(fromDir, toFile).Replace('\\', '/');
}
