using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

public static class DetailReportsWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void WriteAll(FolderNode rootTree, AuditStats stats, string reportsDir, LinkUnifierConfig? cfg = null)
    {
        cfg ??= new LinkUnifierConfig();
        Directory.CreateDirectory(reportsDir);
        SummaryReportWriter.Write(rootTree, stats, reportsDir, cfg);

        string ts = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new CultureInfo("bg-BG"));
        var emptyDirs = new List<string>();
        TreeScanner.CollectEmptyNodes(rootTree, emptyDirs);

        var inProgressList = stats.RejectedLog.Where(r => r.Contains("В ПРОЦЕС")).ToList();

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.ApprovedReportFile),
            $"ОДОБРЕНИ И ПРИЛОЖЕНИ ПРОМЕНИ ({stats.ApprovedLog.Count}) — {ts}",
            stats.ApprovedLog,
            "Няма одобрени промени при това пускане.");

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.DeletedReportFile),
            $"ИЗТРИТИ ОСТАРЕЛИ РЕДОВЕ ({stats.DeletedLinesLog.Count}) — {ts}",
            stats.DeletedLinesLog,
            "Няма изтрити редове при това пускане.");

        int linkCount = stats.LastAuditIssues.Count(i => !i.IsAnchorOnly);
        int anchorCount = stats.LastAuditIssues.Count(i => i.IsAnchorOnly);

        var grouped = new StringBuilder();
        grouped.AppendLine("================================================================================");
        grouped.AppendLine($"ГРУПИРАН ОДИТ ПО ФАЙЛ (1/ ЛИНКОВЕ: {linkCount} | 2/ #ANCHOR: {anchorCount} | ОБЩО: {stats.LastAuditIssues.Count}) — {ts}");
        grouped.AppendLine("================================================================================");

        if (stats.LastAuditIssues.Count == 0)
        {
            grouped.AppendLine("🎉 Няма открити счупени линкове или невалидни котви!");
        }
        else
        {
            foreach (var fg in stats.LastAuditIssues.GroupBy(i => i.RelativeFile))
            {
                grouped.AppendLine();
                grouped.AppendLine($"📄 {fg.Key}:");
                var links = fg.Where(x => !x.IsAnchorOnly).ToList();
                var anchors = fg.Where(x => x.IsAnchorOnly).ToList();

                if (links.Count > 0)
                {
                    grouped.AppendLine($"   1/ Линкове ({links.Count}):");
                    foreach (var l in links)
                        grouped.AppendLine($"      • [#{l.Id}] ред {l.LineNumber}: {l.OriginalLink}  (Предложено: {l.ProposedReplacement})");
                }
                if (anchors.Count > 0)
                {
                    grouped.AppendLine($"   2/ #anchor котви ({anchors.Count}):");
                    foreach (var a in anchors)
                        grouped.AppendLine($"      • [#{a.Id}] ред {a.LineNumber}: {a.OriginalLink}  (Предложено: {a.ProposedReplacement})");
                }
            }
        }
        File.WriteAllText(Path.Combine(reportsDir, cfg.GroupedAuditReportFile), grouped.ToString(), Utf8NoBom);

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.EmptyFoldersReportFile),
            $"ПРАЗНИ ПАПКИ ОТ POST-ORDER DFS ОБХОЖДАНЕТО ({emptyDirs.Count}) — {ts}",
            emptyDirs.Select(d => $"📂 {d}"),
            "🎉 Няма празни директории в библиотеката!");

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.CrudReportFile),
            $"CRUD ОПЕРАЦИИ ПО ФАЙЛОВЕ И ПАПКИ ({stats.CrudLog.Count}) — {ts}",
            stats.CrudLog,
            "Няма извършени CRUD операции при това пускане.");

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.NavSyncReportFile),
            $"СИНХРОНИЗИРАНИ NAVBAR И FOOTER ФАЙЛОВЕ ({stats.NavSyncLog.Count}) — {ts}",
            stats.NavSyncLog,
            "Няма променени Navbar/Footer файлове (всички са актуални).");

        WriteSectionFile(
            Path.Combine(reportsDir, cfg.InProgressReportFile),
            $"МАРКИРАНИ В ПРОЦЕС / ЗА ДОБАВЯНЕ ПРИ СЕСИЯТА ({inProgressList.Count}) — {ts}",
            inProgressList,
            "Няма маркирани елементи 'в процес / за добавяне' при текущата сесия.");

        var missingFiles = stats.LastAuditIssues
            .Where(i => !i.IsAnchorOnly && i.Reason.Contains("не съществува"))
            .ToList();

        var missingSb = new StringBuilder();
        missingSb.AppendLine("================================================================================");
        missingSb.AppendLine($"ЛИНКОВЕ КЪМ НЕСЪЩЕСТВУВАЩИ ФАЙЛОВЕ (ИМА ЛИНК, НО НЯМА ФАЙЛ: {missingFiles.Count}) — {ts}");
        missingSb.AppendLine("================================================================================");

        if (missingFiles.Count == 0)
        {
            missingSb.AppendLine("🎉 Всички линкове сочат към съществуващи файлове!");
        }
        else
        {
            var uniqueTargets = missingFiles
                .GroupBy(m => m.LinkTarget)
                .OrderByDescending(g => g.Count())
                .ToList();

            missingSb.AppendLine($"📌 Уникални липсващи целеви пътища: {uniqueTargets.Count}");
            missingSb.AppendLine("--------------------------------------------------------------------------------");
            foreach (var ug in uniqueTargets)
                missingSb.AppendLine($"   • {ug.Key}  (споменат {ug.Count()} пъти)");

            missingSb.AppendLine();
            missingSb.AppendLine("--------------------------------------------------------------------------------");
            missingSb.AppendLine("📂 РАЗБИВКА ПО ИЗХОДЕН ФАЙЛ (КЪДЕ СЕ НАМИРА ЛИНКЪТ):");
            missingSb.AppendLine("--------------------------------------------------------------------------------");

            foreach (var fg in missingFiles.GroupBy(m => m.RelativeFile))
            {
                missingSb.AppendLine();
                missingSb.AppendLine($"📄 {fg.Key} ({fg.Count()}):");
                foreach (var item in fg)
                    missingSb.AppendLine($"   • [#{item.Id}] ред {item.LineNumber}: {item.OriginalLink}");
            }
        }
        File.WriteAllText(Path.Combine(reportsDir, cfg.MissingFilesReportFile), missingSb.ToString(), Utf8NoBom);
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
