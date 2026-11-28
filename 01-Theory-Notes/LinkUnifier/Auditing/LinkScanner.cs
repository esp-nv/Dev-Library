using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class LinkScanner
{
    private static readonly Regex LinkRegex = new(@"\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);
    private static readonly Regex InProgressRegex = new(@"\*\*([^*]+)\*\*\s*\*в разработка\*", RegexOptions.Compiled);

    public static List<LinkIssue> ScanAll(string rootNotesDir, AuditStats stats, LinkUnifierConfig? cfg = null)
    {
        cfg ??= new LinkUnifierConfig();
        stats.ValidRefs.Clear();
        stats.InProgressRefs.Clear();

        var allValidFiles = Directory.GetFiles(rootNotesDir, "*.*", SearchOption.AllDirectories)
            .Where(f =>
            {
                var relParts = Path.GetRelativePath(rootNotesDir, f)
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return !relParts.Any(p => p.StartsWith(".") || cfg.IgnoredFolders.Contains(p, StringComparer.OrdinalIgnoreCase));
            })
            .ToList();

        var mdFiles = allValidFiles
            .Where(f => f.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var headingCache = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        var issues = new List<LinkIssue>();
        int nextId = 1;

        foreach (var file in mdFiles)
        {
            string relFile = Path.GetRelativePath(rootNotesDir, file).Replace('\\', '/');
            var lines = File.ReadAllLines(file);
            bool inCodeBlock = false;

            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                string line = lines[lineIdx];
                if (line.TrimStart().StartsWith("```"))
                {
                    inCodeBlock = !inCodeBlock;
                    continue;
                }
                if (inCodeBlock) continue;

                foreach (Match ip in InProgressRegex.Matches(line))
                {
                    stats.InProgressRefs.Add(new ValidRefInfo { RelativeFile = relFile, IsAnchorOnly = false });
                }

                foreach (Match m in LinkRegex.Matches(line))
                {
                    string text = m.Groups.Cast<Group>().ElementAt(1).Value.Trim();
                    string target = m.Groups.Cast<Group>().ElementAt(2).Value.Trim();

                    if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        target.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                        target.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                        target.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // 1. Вътрешна #anchor котва в същия файл
                    if (target.StartsWith("#"))
                    {
                        string anchor = Uri.UnescapeDataString(target.Substring(1)).Trim();
                        if (!headingCache.TryGetValue(file, out var anchors))
                        {
                            anchors = AnchorHelper.ExtractHeadingsAsAnchors(file);
                            headingCache[file] = anchors;
                        }

                        if (!anchors.Contains(anchor))
                        {
                            string bestAnchor = AnchorHelper.FindBestAnchorMatch(anchor, anchors);
                            string detailReason = target.Contains(' ')
                                ? $"Котвата '{target}' съдържа излишни интервали"
                                : $"Невалидна вътрешна котва '{target}'";

                            issues.Add(new LinkIssue
                            {
                                Id = nextId++,
                                RelativeFile = relFile,
                                LineNumber = lineIdx + 1,
                                LinkText = text,
                                LinkTarget = target,
                                OriginalLink = m.Value,
                                ProposedReplacement = $"[{text}](#{bestAnchor})",
                                Reason = detailReason,
                                IsAnchorOnly = true
                            });
                        }
                        else
                        {
                            stats.ValidRefs.Add(new ValidRefInfo { RelativeFile = relFile, IsAnchorOnly = true });
                        }
                        continue;
                    }

                    // 2. Относителен линк към файл / папка
                    string[] parts = target.Split('#');
                    string targetPathPart = parts.First().Trim();
                    string anchorPart = parts.Length > 1 ? Uri.UnescapeDataString(parts.ElementAt(1)).Trim() : "";
                    string baseDir = Path.GetDirectoryName(file) ?? rootNotesDir;
                    string resolvedPath = Path.GetFullPath(Path.Combine(baseDir, targetPathPart));

                    bool exists = File.Exists(resolvedPath) || Directory.Exists(resolvedPath);
                    if (!exists)
                    {
                        string proposed = LinkResolver.FindBestReplacement(file, text, target, rootNotesDir, allValidFiles, out string reason);
                        issues.Add(new LinkIssue
                        {
                            Id = nextId++,
                            RelativeFile = relFile,
                            LineNumber = lineIdx + 1,
                            LinkText = text,
                            LinkTarget = target,
                            OriginalLink = m.Value,
                            ProposedReplacement = proposed,
                            Reason = reason,
                            IsAnchorOnly = false
                        });
                    }
                    else if (!string.IsNullOrEmpty(anchorPart) && File.Exists(resolvedPath) &&
                             resolvedPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!headingCache.TryGetValue(resolvedPath, out var targetAnchors))
                        {
                            targetAnchors = AnchorHelper.ExtractHeadingsAsAnchors(resolvedPath);
                            headingCache[resolvedPath] = targetAnchors;
                        }

                        if (!targetAnchors.Contains(anchorPart))
                        {
                            string bestAnchor = AnchorHelper.FindBestAnchorMatch(anchorPart, targetAnchors);
                            string relTarget = LinkResolver.MakeRelative(baseDir, resolvedPath);

                            issues.Add(new LinkIssue
                            {
                                Id = nextId++,
                                RelativeFile = relFile,
                                LineNumber = lineIdx + 1,
                                LinkText = text,
                                LinkTarget = target,
                                OriginalLink = m.Value,
                                ProposedReplacement = $"[{text}]({relTarget}#{bestAnchor})",
                                Reason = $"Файлът съществува, но котва '#{anchorPart}' липсва в него",
                                IsAnchorOnly = true
                            });
                        }
                        else
                        {
                            stats.ValidRefs.Add(new ValidRefInfo { RelativeFile = relFile, IsAnchorOnly = false });
                        }
                    }
                    else
                    {
                        stats.ValidRefs.Add(new ValidRefInfo { RelativeFile = relFile, IsAnchorOnly = false });
                    }
                }
            }
        }

        stats.LastAuditIssues = issues;
        return issues;
    }
}
