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
        string targetPathPart = rawTarget.Split('#')[0].Trim();
        string anchorPart = rawTarget.Contains('#') ? "#" + rawTarget.Split('#')[1] : "";
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

        string fileName = Path.GetFileName(targetPathPart.TrimEnd('/', '\\'));
        if (string.IsNullOrEmpty(fileName) && (targetPathPart.EndsWith("/") || targetPathPart.EndsWith("\\")))
            fileName = "README.md";

        if (fileName.Equals("README.md", StringComparison.OrdinalIgnoreCase))
        {
            string parentFolder = Path.GetFileName(targetPathPart.TrimEnd('/', '\\').Replace("/README.md", "", StringComparison.OrdinalIgnoreCase));
            var folderMatch = allValidFiles.FirstOrDefault(f =>
                Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase) &&
                Path.GetFileName(Path.GetDirectoryName(f) ?? "").Equals(parentFolder, StringComparison.OrdinalIgnoreCase));

            if (folderMatch != null)
            {
                reason = $"Намерен README.md в папка '{parentFolder}'";
                return $"[{linkText}]({MakeRelative(currentDir, folderMatch)}{anchorPart})";
            }
        }
        else if (!string.IsNullOrEmpty(fileName))
        {
            var exact = allValidFiles.FirstOrDefault(f => Path.GetFileName(f).Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                reason = "Файлът е намерен на нов път в библиотеката";
                return $"[{linkText}]({MakeRelative(currentDir, exact)}{anchorPart})";
            }

            var prefixMatch = Regex.Match(fileName, @"^(\d{2}-\d{2}(?:-\d{2})?)");
            if (prefixMatch.Success)
            {
                string prefix = prefixMatch.Groups[1].Value;
                var candidates = allValidFiles
                    .Where(f => Path.GetFileName(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (candidates.Count == 1)
                {
                    reason = $"Намерен файл с префикс '{prefix}'";
                    return $"[{linkText}]({MakeRelative(currentDir, candidates[0])}{anchorPart})";
                }
                if (candidates.Count > 1)
                {
                    var best = candidates.OrderBy(c => AnchorHelper.LevenshteinDistance(Path.GetFileName(c), fileName)).First();
                    reason = $"Намерен най-близък файл с префикс '{prefix}'";
                    return $"[{linkText}]({MakeRelative(currentDir, best)}{anchorPart})";
                }
            }
        }

        reason = "Целевият файл не съществува -> маркиране като *(в разработка)*";
        return $"**{linkText}** *(в разработка)*";
    }

    public static string MakeRelative(string fromDir, string toFile)
        => Path.GetRelativePath(fromDir, toFile).Replace('\\', '/');
}
