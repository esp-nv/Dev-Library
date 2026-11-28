using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

public static class SummaryReportWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void Write(FolderNode rootTree, AuditStats stats, string reportsDir, LinkUnifierConfig? cfg = null)
    {
        cfg ??= new LinkUnifierConfig();
        string ts = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new CultureInfo("bg-BG"));
        string toolName = Path.GetFileName(Directory.GetParent(reportsDir)?.FullName ?? "LINKUNIFIER").ToUpperInvariant();

        var emptyDirs = new List<string>();
        TreeScanner.CollectEmptyNodes(rootTree, emptyDirs);

        var inProgressNewlyMarked = stats.RejectedLog.Where(r => r.Contains("В ПРОЦЕС")).ToList();

        int totalDomains = rootTree.SubFolders.Count;
        int totalModules = 0;
        int totalLessons = 0;

        foreach (var d in rootTree.SubFolders)
        {
            var (mods, less) = TreeScanner.CountDomainItems(d);
            totalModules += mods;
            totalLessons += less;
        }

        int validLinks = stats.ValidRefs.Count(v => !v.IsAnchorOnly);
        int validAnchors = stats.ValidRefs.Count(v => v.IsAnchorOnly);

        int totalInProgress = stats.InProgressRefs.Count;

        var missingFileIssues = stats.LastAuditIssues.Where(i => !i.IsAnchorOnly && i.Reason.Contains("не съществува")).ToList();
        var wrongPathIssues = stats.LastAuditIssues.Where(i => !i.IsAnchorOnly && !i.Reason.Contains("не съществува")).ToList();

        int brokenLinks = missingFileIssues.Count + wrongPathIssues.Count;
        int brokenAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly);

        int allLinksTotal = validLinks + totalInProgress + brokenLinks;
        int allAnchorsTotal = validAnchors + brokenAnchors;
        int grandTotal = allLinksTotal + allAnchorsTotal;

        double linksHealth = (validLinks + brokenLinks) == 0 ? 100.0 : (validLinks * 100.0) / (validLinks + brokenLinks);
        double anchorsHealth = (validAnchors + brokenAnchors) == 0 ? 100.0 : (validAnchors * 100.0) / (validAnchors + brokenAnchors);
        double totalHealthAll = (validLinks + validAnchors + brokenLinks + brokenAnchors) == 0
            ? 100.0
            : ((validLinks + validAnchors) * 100.0) / (validLinks + validAnchors + brokenLinks + brokenAnchors);

        var sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine($"ОБЩО ОДИТНО РЕЗЮМЕ ОТ {toolName} — {ts}");
        sb.AppendLine("================================================================================");
        sb.AppendLine($"• Общо домейни / модули / уроци:        {totalDomains} домейна | {totalModules} модула | {totalLessons} урока");
        sb.AppendLine($"• Визуална карта и йерархия:           {cfg.ReportsFolderName}/{cfg.TreeMapReportFile}");
        sb.AppendLine($"• Преименувани *-Syntax.md файлове:       {stats.RenamedSyntaxFiles}");
        sb.AppendLine($"• Създадени липсващи README.md:          {stats.CreatedReadmes}");
        sb.AppendLine($"• Синхронизирани Navbar/Footer файлове:   {stats.UpdatedNavFiles}  -> виж {cfg.NavSyncReportFile}");
        sb.AppendLine($"• Оправени при текущата сесия:           {stats.ApprovedLog.Count}  -> виж {cfg.ApprovedReportFile}");
        sb.AppendLine($"• Маркирани 'в разработка' (нови):       {inProgressNewlyMarked.Count}  -> виж {cfg.InProgressReportFile}");
        sb.AppendLine($"• Линкове към несъществуващи файлове:    {missingFileIssues.Count}  -> виж {cfg.MissingFilesReportFile}");
        sb.AppendLine($"• Линкове с грешен относителен път:      {wrongPathIssues.Count}  -> виж {cfg.GroupedAuditReportFile}");
        sb.AppendLine($"• Отказани / пропуснати при одита:       {stats.SkippedLog.Count}");
        sb.AppendLine($"• Изтрити остарели редове:               {stats.DeletedLinesLog.Count}  -> виж {cfg.DeletedReportFile}");
        sb.AppendLine($"• Празни папки от DFS обхождането:        {emptyDirs.Count}  -> виж {cfg.EmptyFoldersReportFile}");
        sb.AppendLine($"• CRUD операции по файлове/папки:         {stats.CrudLog.Count}  -> виж {cfg.CrudReportFile}");
        sb.AppendLine();
        sb.AppendLine("-----------------------------------------------------------------------------------------------------------");
        sb.AppendLine("📊 ПЪЛНА СТАТИСТИКА ЗА ЛИНКОВЕ И #ANCHOR КОТВИ ПО КАТЕГОРИИ");
        sb.AppendLine("-----------------------------------------------------------------------------------------------------------");
        sb.AppendLine(string.Format("{0,-20} | {1,8} | {2,8} | {3,11} | {4,11} | {5,11} | {6,8} | {7,7}",
            "Категория", "Валидни", "В разр.", "Липсв. файл", "Грешен път", "Счуп.(Общо)", "Общо", "Здраве"));
        sb.AppendLine(new string('-', 107));
        sb.AppendLine(string.Format("{0,-20} | {1,8} | {2,8} | {3,11} | {4,11} | {5,11} | {6,8} | {7,6:F1}%",
            "1/ Линкове", validLinks, totalInProgress, missingFileIssues.Count, wrongPathIssues.Count, brokenLinks, allLinksTotal, linksHealth));
        sb.AppendLine(string.Format("{0,-20} | {1,8} | {2,8} | {3,11} | {4,11} | {5,11} | {6,8} | {7,6:F1}%",
            "2/ #anchor котви", validAnchors, 0, "-", brokenAnchors, brokenAnchors, allAnchorsTotal, anchorsHealth));
        sb.AppendLine(new string('-', 107));
        sb.AppendLine(string.Format("{0,-20} | {1,8} | {2,8} | {3,11} | {4,11} | {5,11} | {6,8} | {7,6:F1}%",
            "ОБЩО (1/ + 2/)", validLinks + validAnchors, totalInProgress, missingFileIssues.Count, wrongPathIssues.Count + brokenAnchors, brokenLinks + brokenAnchors, grandTotal, totalHealthAll));
        sb.AppendLine();
        sb.AppendLine("-----------------------------------------------------------------------------------------------------------");
        sb.AppendLine("🌐 РАЗБИВКА ПО ДОМЕЙНИ (ВАЛИДНИ | В РАЗРАБОТКА | СЧУПЕНИ | ОБЩО | ЗДРАВЕ %)");
        sb.AppendLine("-----------------------------------------------------------------------------------------------------------");
        sb.AppendLine(string.Format("{0,-34} | {1,4} | {2,5} | {3,7} | {4,8} | {5,11} | {6,6} | {7,7}",
            "Домейн (Папка)", "Мод.", "Уроци", "Валидни", "В разр.", "Счуп.(Л/#)", "Общо", "Здраве"));
        sb.AppendLine(new string('-', 103));

        foreach (var d in rootTree.SubFolders)
        {
            var (mods, less) = TreeScanner.CountDomainItems(d);
            string prefix = d.FolderName + "/";

            int dValid = stats.ValidRefs.Count(v => v.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            int dWip = stats.InProgressRefs.Count(w => w.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            int dLinks = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly && i.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            int dAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly && i.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            int dBroken = dLinks + dAnchors;
            int dTotal = dValid + dWip + dBroken;

            double health = (dValid + dBroken) == 0 ? 100.0 : (dValid * 100.0) / (dValid + dBroken);
            string brokenCol = $"{dBroken} ({dLinks}/{dAnchors})";

            sb.AppendLine(string.Format("{0,-34} | {1,4} | {2,5} | {3,7} | {4,8} | {5,11} | {6,6} | {7,6:F1}%",
                d.FolderName, mods, less, dValid, dWip, brokenCol, dTotal, health));
        }

        var domainPrefixes = rootTree.SubFolders.Select(d => d.FolderName + "/").ToList();
        int rValid = stats.ValidRefs.Count(v => !domainPrefixes.Any(p => v.RelativeFile.StartsWith(p, StringComparison.OrdinalIgnoreCase)));
        int rWip = stats.InProgressRefs.Count(w => !domainPrefixes.Any(p => w.RelativeFile.StartsWith(p, StringComparison.OrdinalIgnoreCase)));
        int rLinks = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly && !domainPrefixes.Any(p => i.RelativeFile.StartsWith(p, StringComparison.OrdinalIgnoreCase)));
        int rAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly && !domainPrefixes.Any(p => i.RelativeFile.StartsWith(p, StringComparison.OrdinalIgnoreCase)));
        int rBroken = rLinks + rAnchors;
        int rTotal = rValid + rWip + rBroken;

        if (rTotal > 0)
        {
            double rHealth = (rValid + rBroken) == 0 ? 100.0 : (rValid * 100.0) / (rValid + rBroken);
            string rBrokenCol = $"{rBroken} ({rLinks}/{rAnchors})";
            sb.AppendLine(string.Format("{0,-34} | {1,4} | {2,5} | {3,7} | {4,8} | {5,11} | {6,6} | {7,6:F1}%",
                $"(Корен на {rootTree.FolderName})", 0, rootTree.Lessons.Count, rValid, rWip, rBrokenCol, rTotal, rHealth));
        }

        int totalValidAll = validLinks + validAnchors;
        int totalBrokenAll = brokenLinks + brokenAnchors;
        string totalBrokenCol = $"{totalBrokenAll} ({brokenLinks}/{brokenAnchors})";

        sb.AppendLine(new string('-', 103));
        sb.AppendLine(string.Format("{0,-34} | {1,4} | {2,5} | {3,7} | {4,8} | {5,11} | {6,6} | {7,6:F1}%",
            "ОБЩО ЗА БИБЛИОТЕКАТА", totalModules, totalLessons, totalValidAll, totalInProgress, totalBrokenCol, grandTotal, totalHealthAll));

        File.WriteAllText(Path.Combine(reportsDir, cfg.SummaryReportFile), sb.ToString(), Utf8NoBom);
    }
}
