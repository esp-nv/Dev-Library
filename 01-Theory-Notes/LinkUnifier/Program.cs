using System;
using System.IO;
using System.Text;

namespace LinkUnifier;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        string projectDir = FindLinkUnifierProjectDir(Directory.GetCurrentDirectory())
                            ?? FindLinkUnifierProjectDir(AppContext.BaseDirectory)
                            ?? Path.Combine(Directory.GetCurrentDirectory(), "LinkUnifier");
        string rootNotesDir = Directory.GetParent(projectDir)?.FullName ?? Directory.GetCurrentDirectory();
        string jsonMapPath = Path.Combine(projectDir, "library-map.json");
        string reportsDir = Path.Combine(projectDir, "reports");
        string mdMapPath = Path.Combine(reportsDir, "00-library-tree-map.md");

        Directory.CreateDirectory(reportsDir);
        var stats = new AuditStats();
        FolderNode? rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath);

        while (true)
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine($"🏛️  LINKUNIFIER — ОДИТОР НА БИБЛИОТЕКАТА (Root: {Path.GetFileName(rootNotesDir)})");
            Console.WriteLine("================================================================================");
            Console.WriteLine("  [1] 🌲 Пълен одит на структурата (library-map.json + визуална таблица/йерархия .md)");
            Console.WriteLine("  [2] 🛠️  CRUD управление на файлове и папки (подменю: одит, създаване, редакция, преименуване, изтриване)");
            Console.WriteLine("  [3] 🧭 Синхронизация само на Navbar и Footer в цялата библиотека + репорт");
            Console.WriteLine("  [4] 🔍 Одит и поправка на ВСИЧКИ счупени линкове и #anchor котви (1/ линкове; 2/ #anchor)");
            Console.WriteLine("  [5] ⚓ Одит и поправка САМО на вътрешните котви (#anchors)");
            Console.WriteLine("  [6] 📊 Генериране на всички отделни отчети в папка reports/");
            Console.WriteLine("  [7] 🚀 Пълен разширен цикъл (1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 9 с избор на всяка стъпка)");
            Console.WriteLine("  [8] ⚡ Пълен съкратен цикъл (САМО одит на всичко + генериране на всички отчети)");
            Console.WriteLine("  [9] 📑 Допълнителни README.md / # 01-00. Intro подмодули с авто-номер и котва");
            Console.WriteLine("  [0] ❌ Изход");
            Console.Write("\n👉 Избери опция [0-9]: ");

            string choice = Console.ReadLine()?.Trim() ?? "0";
            if (choice == "0")
            {
                SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                Console.WriteLine($"\n👋 Довиждане! Всички отчети са запазени в: {reportsDir}");
                break;
            }

            switch (choice)
            {
                case "1":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath);
                    Console.WriteLine("✅ Дървото и картата са обновени!");
                    break;

                case "2":
                    CrudManager.RunCrudSubmenu(rootNotesDir, jsonMapPath, mdMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                case "3":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath);
                    NavigationSyncer.SyncAll(rootTree, rootNotesDir, stats);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                case "4":
                    InteractiveAuditRunner.Run(rootNotesDir, stats, anchorsOnly: false);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                case "5":
                    InteractiveAuditRunner.Run(rootNotesDir, stats, anchorsOnly: true);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                case "6":
                    LinkScanner.ScanAll(rootNotesDir, stats);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    Console.WriteLine("✅ Всички отчети са генерирани в LinkUnifier/reports/!");
                    break;

                case "7":
                    rootTree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath);
                    NavigationSyncer.SyncAll(rootTree, rootNotesDir, stats);
                    InteractiveAuditRunner.Run(rootNotesDir, stats, anchorsOnly: false);
                    ScaffoldGenerator.RunInteractiveScaffolding(rootNotesDir, jsonMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                case "8":
                    LinkScanner.ScanAll(rootNotesDir, stats);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    Console.WriteLine("✅ Съкратеният одит приключи! Виж LinkUnifier/reports/01-summary-report.txt.");
                    break;

                case "9":
                    ScaffoldGenerator.RunInteractiveScaffolding(rootNotesDir, jsonMapPath, stats, ref rootTree);
                    SaveAll(rootNotesDir, jsonMapPath, mdMapPath, reportsDir, stats);
                    break;

                default:
                    Console.WriteLine("⚠️ Невалиден избор. Моля, въведи цифра от 0 до 9.");
                    break;
            }
        }
    }

    private static void SaveAll(string rootNotesDir, string jsonMapPath, string mdMapPath, string reportsDir, AuditStats stats)
    {
        Directory.CreateDirectory(reportsDir);
        var tree = TreeScanner.BuildAndSaveTree(rootNotesDir, jsonMapPath, mdMapPath);
        DetailReportsWriter.WriteAll(tree, stats, reportsDir);
    }

    private static string? FindLinkUnifierProjectDir(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "LinkUnifier.csproj")))
                return dir.FullName;

            string sub = Path.Combine(dir.FullName, "LinkUnifier");
            if (File.Exists(Path.Combine(sub, "LinkUnifier.csproj")))
                return sub;

            dir = dir.Parent;
        }
        return null;
    }
}
