using System;
using System.IO;
using System.Text;
using LinkUnifier;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var currentDir = Directory.GetCurrentDirectory();
var rootDir = Directory.Exists(Path.Combine(currentDir, "01-Theory-Notes"))
    ? Path.Combine(currentDir, "01-Theory-Notes")
    : Path.GetFullPath(Path.Combine(currentDir, "..", "01-Theory-Notes"));

if (!Directory.Exists(rootDir))
{
    Console.WriteLine($"❌ Не е намерена папка 01-Theory-Notes в: {rootDir}");
    return;
}

string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
string mapJsonPath = Path.Combine(projectDir, "library-map.json");
string reportsDir = Path.Combine(projectDir, "reports");
string visualMapMdPath = Path.Combine(reportsDir, "00-library-tree-map.md");

var stats = new AuditStats();
FolderNode? currentTree = null;

FolderNode EnsureTree()
{
    if (currentTree == null)
    {
        Console.WriteLine("🌲 Изграждане на дървото с Post-Order DFS...");
        currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
    }
    return currentTree;
}

string AskStepMode(string stepTitle)
{
    Console.WriteLine();
    Console.WriteLine($"🔹 СТЪПКА: {stepTitle}");
    Console.Write("   Избери: [1] Само одит (без промени) | [2] Изпълни / Одит + Поправка | [0] Пропусни: ");
    return (Console.ReadLine() ?? "").Trim();
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("================================================================================");
    Console.WriteLine("🧭 LINKUNIFIER — ГЛАВНО ИНТЕРАКТИВНО МЕНЮ");
    Console.WriteLine("================================================================================");
    Console.WriteLine("  [1] 🌲 Пълен одит на структурата (library-map.json + визуална таблица/йерархия .md)");
    Console.WriteLine("  [2] 🛠️ CRUD управление на файлове и папки (подменю: одит, създаване, редакция, преномериране, изтриване)");
    Console.WriteLine("  [3] 🧭 Синхронизация само на Navbar и Footer в цялата библиотека + репорт");
    Console.WriteLine("  [4] 🔍 Одит и поправка на ВСИЧКИ счупени линкове и #anchor котви (1/ линкове; 2/ #anchor)");
    Console.WriteLine("  [5] ⚓ Одит и поправка САМО на вътрешните котви (#anchors)");
    Console.WriteLine("  [6] 📊 Генериране на всички отделни отчети в папка reports/");
    Console.WriteLine("  [7] 🚀 Пълен разширен цикъл (1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 9 с избор на всяка стъпка)");
    Console.WriteLine("  [8] ⚡ Пълен съкратен цикъл (САМО одит на всичко + генериране на всички отчети)");
    Console.WriteLine("  [9] 🏗️ Допълнителни README.md / # 01-00. Intro подмодули с авто-номер и котва");
    Console.WriteLine("  [0] ❌ Изход");
    Console.WriteLine("--------------------------------------------------------------------------------");
    Console.Write("👉 Избери опция [0-9]: ");

    string choice = (Console.ReadLine() ?? "").Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            stats.RenamedSyntaxFiles += TreeScanner.NormalizeSyntaxFileNames(rootDir);
            currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
            ReportGenerator.WriteAllReports(currentTree, stats, reportsDir);
            Console.WriteLine($"✅ Пълният одит на структурата приключи ({currentTree.SubFolders.Count} домейна).");
            Console.WriteLine($"   • JSON карта:      {mapJsonPath}");
            Console.WriteLine($"   • Визуален .md:    {visualMapMdPath}");
            break;

        case "2":
            CrudManager.RunCrudSubmenu(rootDir, mapJsonPath, visualMapMdPath, stats, ref currentTree);
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            break;

        case "3":
            NavigationSyncer.SyncAll(EnsureTree(), rootDir, stats);
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            Console.WriteLine($"✅ Синхронизацията на Navbar/Footer приключи! Създадени README: {stats.CreatedReadmes}, Обновени файлове: {stats.UpdatedNavFiles}");
            break;

        case "4":
            LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: false, autoPromptToFix: true);
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            break;

        case "5":
            LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: true, autoPromptToFix: true);
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            break;

        case "6":
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            Console.WriteLine($"📊 Всички отделни отчети са генерирани успешно в: {reportsDir}");
            break;

        case "7":
            Console.WriteLine("🚀 СТАРТ НА ПЪЛЕН РАЗШИРЕН ЦИКЪЛ (1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 9)");
            var m1 = AskStepMode("[1] Сканиране на структурата (library-map.json + визуален .md)");
            if (m1 == "1")
                currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
            else if (m1 == "2")
            {
                stats.RenamedSyntaxFiles += TreeScanner.NormalizeSyntaxFileNames(rootDir);
                currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
            }

            var m2 = AskStepMode("[2] CRUD управление на файлове и папки");
            if (m2 == "1")
                Console.WriteLine("ℹ️ За одит на празни папки виж reports/05-empty-folders.txt.");
            else if (m2 == "2")
                CrudManager.RunCrudSubmenu(rootDir, mapJsonPath, visualMapMdPath, stats, ref currentTree);

            var m3 = AskStepMode("[3] Синхронизация на Navbar и Footer");
            if (m3 == "2")
                NavigationSyncer.SyncAll(EnsureTree(), rootDir, stats);
            else if (m3 == "1")
                Console.WriteLine("ℹ️ Пропусната промяна по Navbar/Footer (режим само одит).");

            var m4 = AskStepMode("[4] Одит на ВСИЧКИ счупени линкове и #anchor котви");
            if (m4 == "1")
                LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: false, autoPromptToFix: false);
            else if (m4 == "2")
                LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: false, autoPromptToFix: true);

            var m5 = AskStepMode("[5] Одит САМО на вътрешните котви (#anchors)");
            if (m5 == "1")
                LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: true, autoPromptToFix: false);
            else if (m5 == "2")
                LinkAuditor.AuditAndOptionallyFix(EnsureTree(), rootDir, stats, onlyAnchors: true, autoPromptToFix: true);

            var m9 = AskStepMode("[9] Допълнителни README.md / # 01-00. Intro подмодули");
            if (m9 == "2")
                ScaffoldGenerator.RunInteractiveScaffolding(rootDir, mapJsonPath, stats, ref currentTree);

            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            Console.WriteLine($"🎉 Пълният разширен цикъл приключи! Всички отчети са записани в: {reportsDir}");
            break;

        case "8":
            Console.WriteLine("⚡ СТАРТ НА ПЪЛЕН СЪКРАТЕН ЦИКЪЛ (САМО ОДИТ БЕЗ ПРОМЕНИ ПО ФАЙЛОВЕТЕ)...");
            currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
            LinkAuditor.AuditAndOptionallyFix(currentTree, rootDir, stats, onlyAnchors: false, autoPromptToFix: false);
            ReportGenerator.WriteAllReports(currentTree, stats, reportsDir);
            Console.WriteLine($"✅ Пълният съкратен одит приключи! Всички отчети са готови в: {reportsDir}");
            break;

        case "9":
            ScaffoldGenerator.RunInteractiveScaffolding(rootDir, mapJsonPath, stats, ref currentTree);
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            break;

        case "0":
            ReportGenerator.WriteAllReports(EnsureTree(), stats, reportsDir);
            Console.WriteLine($"👋 Довиждане! Всички отчети са запазени в: {reportsDir}");
            return;

        default:
            Console.WriteLine("⚠️ Невалиден избор. Моля въведи число от 0 до 9.");
            break;
    }
}
