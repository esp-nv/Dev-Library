using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

public static class ReportGenerator
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void WriteAllReports(FolderNode rootTree, AuditStats stats, string reportsDir)
    {
        Directory.CreateDirectory(reportsDir);
        string ts = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new CultureInfo("bg-BG"));

        var emptyDirs = new List<string>();
        TreeScanner.CollectEmptyNodes(rootTree, emptyDirs);

        var inProgressList = stats.RejectedLog.Where(r => r.Contains("В ПРОЦЕС")).ToList();

        // Изчисляване на структурата (домейни, модули, уроци)
        int totalDomains = rootTree.SubFolders.Count;
        int totalModules = 0;
        int totalLessons = 0;

        foreach (var d in rootTree.SubFolders)
        {
            var (mods, less) = CountDomainItems(d);
            totalModules += mods;
            totalLessons += less;
        }

        // Изчисляване на оправени vs оставащи счупени линкове и #anchor котви
        int fixedLinks = stats.ApprovedLog.Count(l => !l.Contains("(#"));
        int fixedAnchors = stats.ApprovedLog.Count(l => l.Contains("(#"));

        int remainingBrokenLinks = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly);
        int remainingBrokenAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly);

        int totalDetectedLinks = fixedLinks + remainingBrokenLinks;
        int totalDetectedAnchors = fixedAnchors + remainingBrokenAnchors;
        int totalDetectedAll = totalDetectedLinks + totalDetectedAnchors;

        // 1. Общо резюме
        var summary = new StringBuilder();
        summary.AppendLine("================================================================================");
        summary.AppendLine($"ОБЩО ОДИТНО РЕЗЮМЕ ОТ LINKUNIFIER — {ts}");
        summary.AppendLine("================================================================================");
        summary.AppendLine($"• Общо домейни / модули / уроци:        {totalDomains} домейна | {totalModules} модула | {totalLessons} урока");
        summary.AppendLine($"• Визуална карта и йерархия:           reports/00-library-tree-map.md");
        summary.AppendLine($"• Преименувани *-Syntax.md файлове:       {stats.RenamedSyntaxFiles}");
        summary.AppendLine($"• Създадени липсващи README.md:          {stats.CreatedReadmes}");
        summary.AppendLine($"• Синхронизирани Navbar/Footer файлове:   {stats.UpdatedNavFiles}  -> виж 07-navbar-footer-sync.txt");
        summary.AppendLine($"• Изтрити остарели редове:               {stats.DeletedLinesLog.Count}  -> виж 03-deleted-lines.txt");
        summary.AppendLine($"• Празни папки от DFS обхождането:        {emptyDirs.Count}  -> виж 05-empty-folders.txt");
        summary.AppendLine($"• CRUD операции по файлове/папки:         {stats.CrudLog.Count}  -> виж 06-crud-changes.txt");
        summary.AppendLine($"• Маркирани 'в процес / за добавяне':    {inProgressList.Count}  -> виж 08-in-progress-to-create.txt");
        summary.AppendLine();
        summary.AppendLine("--------------------------------------------------------------------------------");
        summary.AppendLine("📊 ДЕТАЙЛНА СТАТИСТИКА ЗА ЛИНКОВЕ И #ANCHOR КОТВИ");
        summary.AppendLine("--------------------------------------------------------------------------------");
        summary.AppendLine($"• 1/ Линкове:        Оправени: {fixedLinks} | Счупени/Невалидни: {remainingBrokenLinks} | Общо: {totalDetectedLinks}  -> виж 02-approved-links.txt & 04-grouped-audit-report.txt");
        summary.AppendLine($"• 2/ #anchor котви:  Оправени: {fixedAnchors} | Счупени/Невалидни: {remainingBrokenAnchors} | Общо: {totalDetectedAnchors}  -> виж 02-approved-links.txt & 04-grouped-audit-report.txt");
        summary.AppendLine($"• ОБЩО (1/ + 2/):    Оправени: {fixedLinks + fixedAnchors} | Счупени/Невалидни: {remainingBrokenLinks + remainingBrokenAnchors} | Общо: {totalDetectedAll}");
        summary.AppendLine();
        summary.AppendLine("--------------------------------------------------------------------------------");
        summary.AppendLine("🌐 РАЗБИВКА ПО ДОМЕЙНИ (МОДУЛИ | УРОЦИ | СЧУПЕНИ ЛИНКОВЕ | #ANCHORS | ОБЩО)");
        summary.AppendLine("--------------------------------------------------------------------------------");
        summary.AppendLine(string.Format("{0,-36} | {1,6} | {2,6} | {3,8} | {4,8} | {5,6}",
            "Домейн (Папка)", "Модули", "Уроци", "Линкове", "#Anchors", "Общо"));
        summary.AppendLine(new string('-', 86));

        foreach (var d in rootTree.SubFolders)
        {
            var (mods, less) = CountDomainItems(d);
            string prefix = d.FolderName + "/";
            int dLinks = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly && i.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            int dAnchors = stats.LastAuditIssues.Count(i => i.IsAnchorOnly && i.RelativeFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            summary.AppendLine(string.Format("{0,-36} | {1,6} | {2,6} | {3,8} | {4,8} | {5,6}",
                d.FolderName, mods, less, dLinks, dAnchors, dLinks + dAnchors));
        }

        // Проверяваме дали има файлове извън номерираните домейни (напр. Architecture-And-Cross-Stack/)
        var domainPrefixes = rootTree.SubFolders.Select(d => d.FolderName + "/").ToList();
        var rootOtherIssues = stats.LastAuditIssues
            .Where(i => !domainPrefixes.Any(p => i.RelativeFile.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (rootOtherIssues.Count > 0)
        {
            int oLinks = rootOtherIssues.Count(i => !i.IsAnchorOnly);
            int oAnchors = rootOtherIssues.Count(i => i.IsAnchorOnly);
            summary.AppendLine(string.Format("{0,-36} | {1,6} | {2,6} | {3,8} | {4,8} | {5,6}",
                "(Други папки в 01-Theory-Notes)", 0, 0, oLinks, oAnchors, oLinks + oAnchors));
        }

        summary.AppendLine(new string('-', 86));
        summary.AppendLine(string.Format("{0,-36} | {1,6} | {2,6} | {3,8} | {4,8} | {5,6}",
            "ОБЩО ЗА БИБЛИОТЕКАТА", totalModules, totalLessons, remainingBrokenLinks, remainingBrokenAnchors, remainingBrokenLinks + remainingBrokenAnchors));

        File.WriteAllText(Path.Combine(reportsDir, "01-summary-report.txt"), summary.ToString(), Utf8NoBom);

        // 2. Одобрени и приложени промени
        WriteSectionFile(
            Path.Combine(reportsDir, "02-approved-links.txt"),
            $"ОДОБРЕНИ И ПРИЛОЖЕНИ ПРОМЕНИ ({stats.ApprovedLog.Count}) — {ts}",
            stats.ApprovedLog,
            "Няма одобрени промени при това пускане.");

        // 3. Изтрити остарели редове
        WriteSectionFile(
            Path.Combine(reportsDir, "03-deleted-lines.txt"),
            $"ИЗТРИТИ ОСТАРЕЛИ РЕДОВЕ ({stats.DeletedLinesLog.Count}) — {ts}",
            stats.DeletedLinesLog,
            "Няма изтрити редове при това пускане.");

        // 4. Групиран одит по файл: 1/ линкове; 2/ #anchor + номер на ред
        var groupedAudit = new StringBuilder();
        groupedAudit.AppendLine("================================================================================");
        groupedAudit.AppendLine($"ГРУПИРАН ОДИТ ПО ФАЙЛ (1/ ЛИНКОВЕ: {remainingBrokenLinks} | 2/ #ANCHOR: {remainingBrokenAnchors} | ОБЩО: {stats.LastAuditIssues.Count}) — {ts}");
        groupedAudit.AppendLine("================================================================================");
        if (stats.LastAuditIssues.Count == 0)
        {
            groupedAudit.AppendLine("🎉 Няма открити счупени линкове или невалидни котви!");
        }
        else
        {
            foreach (var fg in stats.LastAuditIssues.GroupBy(i => i.RelativeFile))
            {
                groupedAudit.AppendLine();
                groupedAudit.AppendLine($"📄 {fg.Key}:");
                var links = fg.Where(x => !x.IsAnchorOnly).ToList();
                var anchors = fg.Where(x => x.IsAnchorOnly).ToList();

                if (links.Count > 0)
                {
                    groupedAudit.AppendLine($"   1/ Линкове ({links.Count}):");
                    foreach (var l in links)
                        groupedAudit.AppendLine($"      • [#{l.Id}] ред {l.LineNumber}: {l.OriginalLink}  (Предложено: {l.ProposedReplacement})");
                }
                if (anchors.Count > 0)
                {
                    groupedAudit.AppendLine($"   2/ #anchor котви ({anchors.Count}):");
                    foreach (var a in anchors)
                        groupedAudit.AppendLine($"      • [#{a.Id}] ред {a.LineNumber}: {a.OriginalLink}  (Предложено: {a.ProposedReplacement})");
                }
            }
        }
        File.WriteAllText(Path.Combine(reportsDir, "04-grouped-audit-report.txt"), groupedAudit.ToString(), Utf8NoBom);

        // 5. Празни папки
        WriteSectionFile(
            Path.Combine(reportsDir, "05-empty-folders.txt"),
            $"ПРАЗНИ ПАПКИ ОТ POST-ORDER DFS ОБХОЖДАНЕТО ({emptyDirs.Count}) — {ts}",
            emptyDirs.Select(d => $"📂 {d}"),
            "🎉 Няма празни директории в библиотеката!");

        // 6. CRUD промени
        WriteSectionFile(
            Path.Combine(reportsDir, "06-crud-changes.txt"),
            $"CRUD ОПЕРАЦИИ ПО ФАЙЛОВЕ И ПАПКИ ({stats.CrudLog.Count}) — {ts}",
            stats.CrudLog,
            "Няма извършени CRUD операции при това пускане.");

        // 7. Navbar & Footer синхронизация
        WriteSectionFile(
            Path.Combine(reportsDir, "07-navbar-footer-sync.txt"),
            $"СИНХРОНИЗИРАНИ NAVBAR И FOOTER ФАЙЛОВЕ ({stats.NavSyncLog.Count}) — {ts}",
            stats.NavSyncLog,
            "Няма променени Navbar/Footer файлове (всички са актуални).");

               // 8. В процес / за добавяне
        WriteSectionFile(
            Path.Combine(reportsDir, "08-in-progress-to-create.txt"),
            $"МАРКИРАНИ В ПРОЦЕС / ЗА ДОБАВЯНЕ ({inProgressList.Count}) — {ts}",
            inProgressList,
            "Няма маркирани елементи 'в процес / за добавяне'.");
    }

    private static (int modules, int lessons) CountDomainItems(FolderNode domain)
    {
        int mods = domain.SubFolders.Count(sf => sf.SubFolders.Count == 0)
                   + domain.SubFolders.Sum(sf => sf.SubFolders.Count);
        int less = domain.Lessons.Count + domain.SubFolders.Sum(sf => CountLessonsRec(sf));
        return (mods, less);
    }

    private static int CountLessonsRec(FolderNode node)
    {
        return node.Lessons.Count + node.SubFolders.Sum(CountLessonsRec);
    }

    private static void WriteSectionFile(string path, string header, IEnumerable<string> lines, string emptyMsg)
    {
        var sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine(header);
        sb.AppendLine("================================================================================");
        var list = lines.ToList();
        if (list.Count == 0)
            sb.AppendLine(emptyMsg);
        else
            foreach (var l in list) sb.AppendLine(l);

        File.WriteAllText(path, sb.ToString(), Utf8NoBom);
    }
}
