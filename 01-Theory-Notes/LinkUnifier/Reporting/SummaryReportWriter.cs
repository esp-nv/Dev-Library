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

    public static void Write(FolderNode rootTree, AuditStats stats, string reportsDir)
    {
        string ts = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new CultureInfo("bg-BG"));

        var emptyDirs = new List<string>();
        TreeScanner.CollectEmptyNodes(rootTree, emptyDirs);

        var inProgressNewlyMarked = stats.RejectedLog.Where(r => r.Contains("В ПРОЦЕС")).ToList();

        // Структура (еднаква формула с 00-library-tree-map.md)
        int totalDomains = rootTree.SubFolders.Count;
        int totalModules = 0;
        int totalLessons = 0;

        foreach (var d in rootTree.SubFolders)
        {
            var (mods, less) = TreeScanner.CountDomainItems(d);
            totalModules += mods;
            totalLessons += less;
        }

        // Броячи за 1/ Линкове и 2/ #anchor котви
        int validLinks = stats.ValidRefs.Count(v => !v.IsAnchorOnly);
        int validAnchors = stats.ValidRefs.Count(v => v.IsAnchorOnly);

        int fixedLinks = stats.ApprovedLog.Count(l => !l.Contains("(#"));
        int fixedAnchors = stats.ApprovedLog.Count(l => l.Contains("(#"));

        int totalInProgress = stats.InProgressRefs.Count;

        int brokenLinks = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly);
        int brokenAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly);

        int allLinksTotal = validLinks + totalInProgress + brokenLinks;
        int allAnchorsTotal = validAnchors + brokenAnchors;
        int grandTotal = allLinksTotal + allAnchorsTotal;

        var sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine($"ОБЩО ОДИТНО РЕЗЮМЕ ОТ LINKUNIFIER — {ts}");
        sb.AppendLine("================================================================================");
        sb.AppendLine($"• Общо домейни / модули / уроци:        {totalDomains} домейна | {totalModules} модула | {totalLessons} урока");
        sb.AppendLine($"• Визуална карта и йерархия:           reports/00-library-tree-map.md");
        sb.AppendLine($"• Преименувани *-Syntax.md файлове:       {stats.RenamedSyntaxFiles}");
        sb.AppendLine($"• Създадени липсващи README.md:          {stats.CreatedReadmes}");
        sb.AppendLine($"• Синхронизирани Navbar/Footer файлове:   {stats.UpdatedNavFiles}  -> виж 07-navbar-footer-sync.txt");
        sb.AppendLine($"• Оправени при текущата сесия:           {stats.ApprovedLog.Count}  -> виж 02-approved-links.txt");
        sb.AppendLine($"• Маркирани 'в разработка' (нови):       {inProgressNewlyMarked.Count}  -> виж 08-in-progress-to-create.txt");
        sb.AppendLine($"• Отказани / пропуснати при одита:       {stats.SkippedLog.Count}");
        sb.AppendLine($"• Изтрити остарели редове:               {stats.DeletedLinesLog.Count}  -> виж 03-deleted-lines.txt");
        sb.AppendLine($"• Празни папки от DFS обхождането:        {emptyDirs.Count}  -> виж 05-empty-folders.txt");
        sb.AppendLine($"• CRUD операции по файлове/папки:         {stats.CrudLog.Count}  -> виж 06-crud-changes.txt");
        sb.AppendLine();
        sb.AppendLine("--------------------------------------------------------------------------------");
        sb.AppendLine("📊 ПЪЛНА СТАТИСТИКА ЗА ЛИНКОВЕ И #ANCHOR КОТВИ (ВАЛИДНИ + В РАЗРАБОТКА + СЧУПЕНИ)");
        sb.AppendLine("--------------------------------------------------------------------------------");
        sb.AppendLine($"• 1/ Линкове:        Валидни: {validLinks,-5} | В разраб.: {totalInProgress,-4} | Счупени: {brokenLinks,-4} | Общо: {allLinksTotal}");
        sb.AppendLine($"• 2/ #anchor котви:  Валидни: {validAnchors,-5} | В разраб.: 0    | Счупени: {brokenAnchors,-4} | Общо: {allAnchorsTotal}");
        sb.AppendLine($"• ОБЩО (1/ + 2/):    Валидни: {validLinks + validAnchors,-5} | В разраб.: {totalInProgress,-4} | Счупени: {brokenLinks + brokenAnchors,-4} | Общо: {grandTotal}");
        sb.AppendLine();
        sb.AppendLine("--------------------------------------------------------------------------------");
        sb.AppendLine("🌐 РАЗБИВКА ПО ДОМЕЙНИ (ВАЛИДНИ | В РАЗРАБОТКА | СЧУПЕНИ | ОБЩО | ЗДРАВЕ %)");
        sb.AppendLine("--------------------------------------------------------------------------------");
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

        // Файлове в корена на 01-Theory-Notes (извън папките-домейни)
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
                "(Корен на 01-Theory-Notes)", 0, rootTree.Lessons.Count, rValid, rWip, rBrokenCol, rTotal, rHealth));
        }

        int totalValidAll = validLinks + validAnchors;
        int totalBrokenAll = brokenLinks + brokenAnchors;
        double totalHealth = (totalValidAll + totalBrokenAll) == 0 ? 100.0 : (totalValidAll * 100.0) / (totalValidAll + totalBrokenAll);
        string totalBrokenCol = $"{totalBrokenAll} ({brokenLinks}/{brokenAnchors})";

        sb.AppendLine(new string('-', 103));
        sb.AppendLine(string.Format("{0,-34} | {1,4} | {2,5} | {3,7} | {4,8} | {5,11} | {6,6} | {7,6:F1}%",
            "ОБЩО ЗА БИБЛИОТЕКАТА", totalModules, totalLessons, totalValidAll, totalInProgress, totalBrokenCol, grandTotal, totalHealth));

        File.WriteAllText(Path.Combine(reportsDir, "01-summary-report.txt"), sb.ToString(), Utf8NoBom);
    }
}
