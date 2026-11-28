using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LinkUnifier;

public static class TreeMapExporter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static void ExportMarkdown(FolderNode root, string outputMdPath)
    {
        string rootName = root.FolderName;
        string ts = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss", new CultureInfo("bg-BG"));
        var sb = new StringBuilder();
        sb.AppendLine($"# 🌲 Визуална Карта и Йерархия на Библиотеката (`{rootName}`)");
        sb.AppendLine();
        sb.AppendLine($"> **Генерирано на:** `{ts}` | **Общо домейни:** `{root.SubFolders.Count}`");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 📊 1. Обобщена Таблица по Домейни и Модули");
        sb.AppendLine();
        sb.AppendLine("| № | Домейн | Папка | Модули | Уроци (`.md`) | Статус |");
        sb.AppendLine("| :--- | :--- | :--- | :---: | :---: | :--- |");

        int totalModules = 0, totalLessons = 0, idx = 1;
        foreach (var domain in root.SubFolders)
        {
            var (mods, lessons) = TreeScanner.CountDomainItems(domain);
            totalModules += mods;
            totalLessons += lessons;
            string status = lessons == 0 ? "🟡 Празен / Планиран" : "✅ Активен";
            sb.AppendLine($"| {idx++} | **{domain.DisplayTitle}** | `{domain.FolderName}` | {mods} | {lessons} | {status} |");
        }
        sb.AppendLine($"| — | **ОБЩО ЗА БИБЛИОТЕКАТА** | `{rootName}` | **{totalModules}** | **{totalLessons}** | — |");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 🗂️ 2. Пълна Йерархична Структура (Post-Order DFS)");
        sb.AppendLine();

        foreach (var domain in root.SubFolders)
            AppendTreeNode(sb, domain, 0);

        File.WriteAllText(outputMdPath, sb.ToString(), Utf8NoBom);
    }

    private static void AppendTreeNode(StringBuilder sb, FolderNode node, int indent)
    {
        string pad = new string(' ', indent * 2);
        string icon = indent == 0 ? "🌐" : "📦";
        sb.AppendLine($"{pad}* {icon} **{node.DisplayTitle}** (`{node.FolderName}/`) — *{node.Lessons.Count} урока*");

        foreach (var lesson in node.Lessons)
            sb.AppendLine($"{pad}  * 📄 [{lesson.Title}](../../{lesson.RelativePath}) (`{lesson.FileName}`)");

        foreach (var sub in node.SubFolders)
            AppendTreeNode(sb, sub, indent + 1);
    }
}
