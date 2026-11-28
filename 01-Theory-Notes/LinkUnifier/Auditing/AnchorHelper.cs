using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class AnchorHelper
{
    public static HashSet<string> ExtractHeadingsAsAnchors(string filePath)
    {
        var anchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(filePath)) return anchors;

        var usedCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        bool inCodeBlock = false;

        foreach (var rawLine in File.ReadLines(filePath))
        {
            var trimmed = rawLine.Trim();
            if (trimmed.StartsWith("```"))
            {
                inCodeBlock = !inCodeBlock;
                continue;
            }
            if (inCodeBlock) continue;

            var m = Regex.Match(trimmed, @"^#{1,6}\s+(.+)$");
            if (!m.Success) continue;

            string rawHeading = m.Groups.Cast<Group>().ElementAt(1).Value;
            string slug = ToGitHubSlug(rawHeading);
            if (string.IsNullOrEmpty(slug)) continue;

            if (usedCount.TryGetValue(slug, out int count))
            {
                usedCount[slug] = count + 1;
                anchors.Add($"{slug}-{count}");
            }
            else
            {
                usedCount[slug] = 1;
                anchors.Add(slug);
            }
        }
        return anchors;
    }

    public static string ToGitHubSlug(string headingText)
    {
        // Премахваме Markdown линкове [текст](url) -> текст и излишни интервали
        string text = Regex.Replace(headingText, @"\[([^\]]+)\]\([^)]+\)", "$1");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        text = text.Replace("`", "").Replace("*", "").Replace("_", "").ToLowerInvariant();

        var sb = new StringBuilder();
        foreach (char c in text)
        {
            if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
                sb.Append(c);
            else if (char.IsWhiteSpace(c))
                sb.Append('-');
        }
        return sb.ToString();
    }

    /// <summary>
    /// Нормализирана форма за детайлно търсене (свива -- в -, маха излишни интервали/тирета и уеднаквява визуални дубликати).
    /// </summary>
    public static string NormalizeForComparison(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return "";
        string s = Uri.UnescapeDataString(slug).Trim().ToLowerInvariant();
        s = Regex.Replace(s, @"\s+", "-");
        s = Regex.Replace(s, @"-{2,}", "-");
        return s.Trim('-');
    }

    /// <summary>
    /// Намира най-точното съвпадение сред наличните котви (първо нормализирано, после по Levenshtein разстояние).
    /// </summary>
    public static string FindBestAnchorMatch(string brokenAnchor, HashSet<string> validAnchors)
    {
        if (validAnchors.Count == 0) return brokenAnchor.Trim();

        string normBroken = NormalizeForComparison(brokenAnchor);

        // 1. Търсим точно съвпадение след премахване на излишни интервали и двойни тирета
        var normalizedHit = validAnchors.FirstOrDefault(a =>
            NormalizeForComparison(a).Equals(normBroken, StringComparison.OrdinalIgnoreCase));

        if (normalizedHit != null)
            return normalizedHit;

        // 2. Ако няма директно съвпадение, търсим най-близкото по Levenshtein върху нормализираните стойности
        return validAnchors
            .OrderBy(a => LevenshteinDistance(NormalizeForComparison(a), normBroken))
            .First();
    }

    public static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        int[,] d = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        return d[a.Length, b.Length];
    }
}
