using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkUnifier;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        string projectDir = FindProjectDirByCsproj(Directory.GetCurrentDirectory())
                            ?? FindProjectDirByCsproj(AppContext.BaseDirectory)
                            ?? Directory.GetCurrentDirectory();
        string rootNotesDir = Directory.GetParent(projectDir)?.FullName ?? Directory.GetCurrentDirectory();
        string toolFolderName = Path.GetFileName(projectDir);

        var cfg = LinkUnifierConfig.LoadOrCreate(projectDir);
        if (!cfg.IgnoredFolders.Contains(toolFolderName, StringComparer.OrdinalIgnoreCase))
            cfg.IgnoredFolders.Add(toolFolderName);

        string jsonMapPath = Path.Combine(projectDir, cfg.JsonMapFileName);
        string reportsDir = Path.Combine(projectDir, cfg.ReportsFolderName);
        string mdMapPath = Path.Combine(reportsDir, cfg.TreeMapReportFile);

        Directory.CreateDirectory(reportsDir);
        var stats = new AuditStats();
        FolderNode? rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath, cfg);

        while (true)
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine($"🏛️  {toolFolderName.ToUpperInvariant()} — ОДИТОР НА БИБЛИОТЕКАТА (Root: {Path.GetFileName(rootNotesDir)})");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"  [1] 🌲 Пълен одит на структурата ({cfg.JsonMapFileName} + визуална таблица/йерархия .md)");
            Console.WriteLine("  [2] 🛠️  CRUD управление на файлове и папки (подменю: одит, създаване, редакция, преименуване, изтриване)");
            Console.WriteLine("  [3] 🧭 Синхронизация само на Navbar и Footer в цялата библиотека + репорт");
            Console.WriteLine("  [4] 🔍 Одит и поправка на ВСИЧКИ счупени линкове и #anchor котви (1/ линкове; 2/ #anchor)");
            Console.WriteLine("  [5] ⚓ Одит и поправка САМО на вътрешните котви (#anchors)");
            Console.WriteLine($"  [6] 📊 Генериране на всички отделни отчети в папка {cfg.ReportsFolderName}/");
            Console.WriteLine("  [7] 🚀 Пълен разширен цикъл (1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 9 с избор на всяка стъпка)");
            Console.WriteLine("  [8] ⚡ Пълен съкратен цикъл (САМО одит на всичко + генериране на всички отчети)");
            Console.WriteLine("  [9] 📑 Допълнителни README.md / # 01-00. Intro подмодули с авто-номер и котва");
            Console.WriteLine(" [10] 🧩 Интерактивен одит САМО на линкове към несъществуващи файлове (текст + път)");
            Console.WriteLine("  [0] ❌ Изход");
            Console.Write("\n👉 Избери опция [0-10]: ");

            string choice = Console.ReadLine()?.Trim() ?? "0";
            if (choice == "0")
            {
                SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                Console.WriteLine($"\n👋 Довиждане! Всички отчети са запазени в: {reportsDir}");
                break;
            }

            switch (choice)
            {
                case "1":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath, cfg);
                    Console.WriteLine("✅ Дървото и картата са обновени!");
                    break;

                case "2":
                    CrudManager.RunCrudSubmenu(rootNotesDir, jsonMapPath, mdMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "3":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath, cfg);
                    NavigationSyncer.SyncAll(rootTree, rootNotesDir, stats);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "4":
                    InteractiveAuditRunner.Run(rootNotesDir, stats, cfg, anchorsOnly: false);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "5":
                    InteractiveAuditRunner.Run(rootNotesDir, stats, cfg, anchorsOnly: true);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "6":
                    LinkScanner.ScanAll(rootNotesDir, stats, cfg);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    Console.WriteLine($"✅ Всички отчети са генерирани в {toolFolderName}/{cfg.ReportsFolderName}/!");
                    break;

                case "7":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath, cfg);
                    NavigationSyncer.SyncAll(rootTree, rootNotesDir, stats);
                    InteractiveAuditRunner.Run(rootNotesDir, stats, cfg, anchorsOnly: false);
                    ScaffoldGenerator.RunInteractiveScaffolding(rootNotesDir, jsonMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "8":
                    LinkScanner.ScanAll(rootNotesDir, stats, cfg);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    Console.WriteLine($"✅ Съкратеният одит приключи! Виж {toolFolderName}/{cfg.ReportsFolderName}/{cfg.SummaryReportFile} и {cfg.MissingFilesReportFile}.");
                    break;

                case "9":
                    ScaffoldGenerator.RunInteractiveScaffolding(rootNotesDir, jsonMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                case "10":
                    InteractiveAuditRunner.Run(rootNotesDir, stats, cfg, anchorsOnly: false, missingFilesOnly: true);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats, cfg);
                    break;

                default:
                    Console.WriteLine("⚠️ Невалиден избор. Моля, въведи цифра от 0 до 10.");
                    break;
            }
        }
    }

    private static void SaveAll(string rootNotesDir, string jsonMapPath, string mdMapPath, string reportsDir, AuditStats stats, LinkUnifierConfig cfg)
    {
        Directory.CreateDirectory(reportsDir);
        var tree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath, cfg);
        DetailReportsWriter.WriteAll(tree, stats, reportsDir, cfg);
    }

    private static string? FindProjectDirByCsproj(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Length > 0)
                return dir.FullName;

            var childWithCsproj = dir.GetDirectories()
                .FirstOrDefault(d => !d.Name.StartsWith(".") && d.GetFiles("*.csproj").Length > 0);
            if (childWithCsproj != null)
                return childWithCsproj.FullName;

            dir = dir.Parent;
        }
        return null;
    }
}
