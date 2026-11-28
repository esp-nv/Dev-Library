using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkUnifier;

public static class CrudManager
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static readonly Regex LinkRegex = new(@"(?<!!)\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

    public static void RunCrudSubmenu(string rootDir, string mapJsonPath, string visualMapMdPath, AuditStats stats, ref FolderNode? currentTree)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine("🛠️ CRUD УПРАВЛЕНИЕ НА ФАЙЛОВЕ И ПАПКИ — ПОДМЕНЮ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("  [1] 🔎 Одит на папка (празни папки, липсващ README.md и неномерирани файлове)");
            Console.WriteLine("  [2] ➕ Създаване на нов файл или папка (с готов шаблон)");
            Console.WriteLine("  [3] ✏️ Редакция на файл (отваряне директно във VS Code)");
            Console.WriteLine("  [4] 🔢 Преименуване и интелигентно преномериране (по README / азбучен / +1)");
            Console.WriteLine("  [5] 🗑️ Изтриване на файл или папка (с двойно предупреждение)");
            Console.WriteLine("  [0] 🔙 Обратно към Главното меню");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.Write("👉 Избери CRUD опция [0-5]: ");

            string choice = (Console.ReadLine() ?? "").Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    AuditFolderStructure(rootDir, stats);
                    break;
                case "2":
                    CreateFileOrFolder(rootDir, stats);
                    RefreshTree(rootDir, mapJsonPath, visualMapMdPath, ref currentTree);
                    break;
                case "3":
                    EditFileInVsCode(rootDir, stats);
                    break;
                case "4":
                    RenameAndRenumber(rootDir, stats);
                    RefreshTree(rootDir, mapJsonPath, visualMapMdPath, ref currentTree);
                    break;
                case "5":
                    DeleteFileOrFolder(rootDir, stats);
                    RefreshTree(rootDir, mapJsonPath, visualMapMdPath, ref currentTree);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("⚠️ Невалиден избор. Въведи число от 0 до 5.");
                    break;
            }
        }
    }

    private static void RefreshTree(string rootDir, string mapJsonPath, string visualMapMdPath, ref FolderNode? currentTree)
    {
        currentTree = TreeScanner.BuildAndSaveTree(rootDir, mapJsonPath, visualMapMdPath);
    }

    private static void AuditFolderStructure(string rootDir, AuditStats stats)
    {
        Console.Write("👉 Въведи относителен път до папка за одит (или Enter за цялата библиотека): ");
        string rel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        string targetDir = string.IsNullOrWhiteSpace(rel) ? rootDir : Path.GetFullPath(Path.Combine(rootDir, rel));

        if (!Directory.Exists(targetDir))
        {
            Console.WriteLine($"❌ Папката не съществува: {targetDir}");
            return;
        }

        var dirs = Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories)
            .Where(d => !Path.GetFileName(d).StartsWith(".") && !d.Contains("LinkUnifier") && !d.Contains("reports"))
            .OrderBy(d => d)
            .ToList();

        int missingReadmes = 0;
        int emptyDirs = 0;
        foreach (var d in dirs)
        {
            var entries = Directory.GetFileSystemEntries(d);
            if (entries.Length == 0)
            {
                emptyDirs++;
                Console.WriteLine($"   📂 Празна папка: {Path.GetRelativePath(rootDir, d).Replace('\\', '/')}");
            }
            else if (!File.Exists(Path.Combine(d, "README.md")))
            {
                missingReadmes++;
                Console.WriteLine($"   📄 Липсва README.md: {Path.GetRelativePath(rootDir, d).Replace('\\', '/')}");
            }
        }

        string msg = $"🔎 [CRUD ОДИТ] Папка: {(string.IsNullOrWhiteSpace(rel) ? "Всички" : rel)} -> Празни папки: {emptyDirs}, Липсващи README.md: {missingReadmes}";
        stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {msg}");
        Console.WriteLine($"✅ {msg}");
    }

    private static void CreateFileOrFolder(string rootDir, AuditStats stats)
    {
        Console.Write("👉 Въведи относителен път за създаване (ако завършва на .md създава файл, иначе папка с README.md): ");
        string rel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(rel)) return;

        string fullPath = Path.GetFullPath(Path.Combine(rootDir, rel));
        if (fullPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            if (File.Exists(fullPath))
            {
                Console.WriteLine("⚠️ Този файл вече съществува!");
                return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            string rawTitle = Path.GetFileNameWithoutExtension(fullPath).Replace('-', ' ');
            string content = $"# 📘 {rawTitle}\n\n## 1. Въведение и архитектурна роля\n\n*(Работна чернова)*\n\n## 2. Основни концепции\n\n* ...\n";
            File.WriteAllText(fullPath, content, Utf8NoBom);
            string log = $"➕ [СЪЗДАДЕН ФАЙЛ] {rel}";
            stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
            Console.WriteLine($"✅ {log}");
            OpenInVsCode(fullPath, 1);
        }
        else
        {
            Directory.CreateDirectory(fullPath);
            string readmePath = Path.Combine(fullPath, "README.md");
            if (!File.Exists(readmePath))
            {
                string folderTitle = Path.GetFileName(fullPath).Replace('-', ' ');
                File.WriteAllText(readmePath, $"# 📦 {folderTitle}\n\nИндексен хъб на модула.\n", Utf8NoBom);
            }
            string log = $"➕ [СЪЗДАДЕНА ПАПКА] {rel} (с README.md)";
            stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
            Console.WriteLine($"✅ {log}");
            OpenInVsCode(readmePath, 1);
        }
    }

    private static void EditFileInVsCode(string rootDir, AuditStats stats)
    {
        Console.Write("👉 Въведи относителен път до .md файл за редакция: ");
        string rel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(rel)) return;

        string fullPath = Path.GetFullPath(Path.Combine(rootDir, rel));
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"❌ Файлът не съществува: {fullPath}");
            return;
        }

        OpenInVsCode(fullPath, 1);
        string log = $"✏️ [ОТВОРЕН ЗА РЕДАКЦИЯ] {rel}";
        stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
        Console.WriteLine($"✅ {log}");
    }

    private static void RenameAndRenumber(string rootDir, AuditStats stats)
    {
        Console.WriteLine("Избери режим на преименуване / преномериране:");
        Console.WriteLine("  [1] Обикновено преименуване на един файл или папка (+ обновяване на линковете)");
        Console.WriteLine("  [2] Преномериране на файловете в папка по реда им в README.md");
        Console.WriteLine("  [3] Преномериране на файловете в папка по азбучен ред (01, 02, 03...)");
        Console.WriteLine("  [4] Изместване на номерацията (+1) от конкретен номер нататък (при вмъкване на нов урок)");
        Console.Write("👉 Избор [1-4]: ");
        string mode = (Console.ReadLine() ?? "").Trim();

        if (mode == "1")
        {
            Console.Write("👉 Текущ относителен път (файл или папка): ");
            string oldRel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
            Console.Write("👉 Ново име (само новото име на файла/папката, напр. 01-03-New-Name.md): ");
            string newName = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(oldRel) || string.IsNullOrWhiteSpace(newName)) return;

            string oldFull = Path.GetFullPath(Path.Combine(rootDir, oldRel));
            string parentDir = Path.GetDirectoryName(oldFull)!;
            string newFull = Path.Combine(parentDir, newName);

            if (File.Exists(oldFull))
            {
                File.Move(oldFull, newFull, overwrite: false);
                ReplaceFileLinksAcrossLibrary(rootDir, Path.GetFileName(oldFull), newName);
                string log = $"🔢 [ПРЕИМЕНУВАН ФАЙЛ] {oldRel} -> {newName}";
                stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
                Console.WriteLine($"✅ {log}");
            }
            else if (Directory.Exists(oldFull))
            {
                Directory.Move(oldFull, newFull);
                ReplaceFileLinksAcrossLibrary(rootDir, Path.GetFileName(oldFull) + "/", newName + "/");
                string log = $"🔢 [ПРЕИМЕНУВАНА ПАПКА] {oldRel} -> {newName}";
                stats.CrudLog.Add( $"[{DateTime.Now:ddd HH:mm:ss}] {log}" );
                Console.WriteLine($"✅ {log}");
            }
            else
            {
                Console.WriteLine("❌ Не е намерен такъв файл или папка.");
            }
            return;
        }

                Console.Write("👉 Въведи относителен път до папката с уроци: ");
        string folderRel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        string targetDir = Path.GetFullPath(Path.Combine(rootDir, folderRel));
        if (!Directory.Exists(targetDir))
        {
            Console.WriteLine("❌ Папката не съществува.");
            return;
        }

        Console.Write("👉 Въведи префикс на главата (напр. 01 за 01-01, 01-02...): ");
        string chapterPrefix = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(chapterPrefix)) chapterPrefix = "01";

        var lessonFiles = Directory.GetFiles(targetDir, "*.md")
            .Where(f => !Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase) &&
                        !Regex.IsMatch(Path.GetFileName(f), @"^\d{2}-(00|9[5-9])-"))
            .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (mode == "2")
        {
            string readmePath = Path.Combine(targetDir, "README.md");
            if (!File.Exists(readmePath))
            {
                Console.WriteLine("❌ В тази папка няма README.md, по който да се подредят.");
                return;
            }
            string readmeText = File.ReadAllText(readmePath, Encoding.UTF8);
            var orderedFromReadme = new List<string>();
            foreach (Match m in LinkRegex.Matches(readmeText))
            {
                string linkTarget = Path.GetFileName(m.Groups[2].Value.Split('#')[0]);
                string fullCandidate = Path.Combine(targetDir, linkTarget);
                if (File.Exists(fullCandidate) && !orderedFromReadme.Contains(fullCandidate) &&
                    !linkTarget.Equals("README.md", StringComparison.OrdinalIgnoreCase))
                {
                    orderedFromReadme.Add(fullCandidate);
                }
            }
            foreach (var f in lessonFiles)
                if (!orderedFromReadme.Contains(f)) orderedFromReadme.Add(f);

            ApplySequentialRenumbering(rootDir, orderedFromReadme, chapterPrefix, stats, "по реда в README.md");
        }
        else if (mode == "3")
        {
            ApplySequentialRenumbering(rootDir, lessonFiles, chapterPrefix, stats, "по азбучен ред");
        }
        else if (mode == "4")
        {
            Console.Write("👉 От кой пореден номер нататък да изместя с +1 (напр. 3 за 01-03 -> 01-04): ");
            if (!int.TryParse((Console.ReadLine() ?? "").Trim(), out int startNum)) return;

            var toShift = lessonFiles
                .Select(f => new { Path = f, Match = Regex.Match(Path.GetFileName(f), @"^(\d{2})-(\d{2})-(.+)$") })
                .Where(x => x.Match.Success && int.Parse(x.Match.Groups[2].Value) >= startNum)
                .OrderByDescending(x => int.Parse(x.Match.Groups[2].Value))
                .ToList();

            foreach (var item in toShift)
            {
                int oldN = int.Parse(item.Match.Groups[2].Value);
                string newFileName = $"{item.Match.Groups[1].Value}-{(oldN + 1):D2}-{item.Match.Groups[3].Value}";
                string newFull = Path.Combine(targetDir, newFileName);
                File.Move(item.Path, newFull, overwrite: false);
                ReplaceFileLinksAcrossLibrary(rootDir, Path.GetFileName(item.Path), newFileName);
                string log = $"🔢 [ИЗМЕСТВАНЕ +1] {Path.GetFileName(item.Path)} -> {newFileName}";
                stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
                Console.WriteLine($"   ✅ {log}");
            }
        }
    }

    private static void ApplySequentialRenumbering(string rootDir, List<string> orderedFiles, string chapterPrefix, AuditStats stats, string modeLabel)
    {
        int seq = 1;
        var tempMoves = new List<(string TempPath, string FinalPath, string OldName, string NewName)>();

        foreach (var file in orderedFiles)
        {
            string oldName = Path.GetFileName(file);
            string baseTopic = Regex.Replace(oldName, @"^\d{2}-\d{2}(?:-\d{2})?-", "");
            string newName = $"{chapterPrefix}-{seq:D2}-00-{baseTopic}";
            if (oldName.Contains("-Syntax-", StringComparison.OrdinalIgnoreCase))
            {
                seq--;
                newName = $"{chapterPrefix}-{seq:D2}-01-{baseTopic}";
            }
            seq++;

            if (oldName == newName) continue;
            string dir = Path.GetDirectoryName(file)!;
            string tempPath = Path.Combine(dir, $".tmp_{Guid.NewGuid():N}_{newName}");
            File.Move(file, tempPath);
            tempMoves.Add((tempPath, Path.Combine(dir, newName), oldName, newName));
        }

        foreach (var move in tempMoves)
        {
            File.Move(move.TempPath, move.FinalPath, overwrite: true);
            ReplaceFileLinksAcrossLibrary(rootDir, move.OldName, move.NewName);
            string log = $"🔢 [ПРЕНОМЕРИРАНЕ ({modeLabel})] {move.OldName} -> {move.NewName}";
            stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
            Console.WriteLine($"   ✅ {log}");
        }
    }

    private static void ReplaceFileLinksAcrossLibrary(string rootDir, string oldToken, string newToken)
    {
        foreach (var md in Directory.GetFiles(rootDir, "*.md", SearchOption.AllDirectories))
        {
            string content = File.ReadAllText(md, Encoding.UTF8);
            if (content.Contains(oldToken))
            {
                File.WriteAllText(md, content.Replace(oldToken, newToken), Utf8NoBom);
            }
        }
    }

    private static void DeleteFileOrFolder(string rootDir, AuditStats stats)
    {
        Console.Write("👉 Въведи относителен път до файла или папката за ИЗТРИВАНЕ: ");
        string rel = (Console.ReadLine() ?? "").Trim().Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(rel)) return;

        string fullPath = Path.GetFullPath(Path.Combine(rootDir, rel));
        bool isFile = File.Exists(fullPath);
        bool isDir = Directory.Exists(fullPath);

        if (!isFile && !isDir)
        {
            Console.WriteLine($"❌ Не е намерен такъв файл или папка: {fullPath}");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"⚠️ ВНИМАНИЕ! На път си да ИЗТРИЕШ БЕЗВЪЗВРАТНО: {(isDir ? "ПАПКАТА и всичко в нея" : "ФАЙЛА")}:");
        Console.WriteLine($"   🗑️ {fullPath}");
        Console.Write("👉 Потвърждение 1/2 — Сигурна ли си? [y/n]: ");
        string c1 = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (c1 is not ("y" or "yes" or "д" or "да"))
        {
            Console.WriteLine("🛡️ Изтриването е отменено.");
            return;
        }

        Console.Write("👉 Потвърждение 2/2 — Напиши точно думата DELETE за окончателно изтриване: ");
        string c2 = (Console.ReadLine() ?? "").Trim();
        if (!c2.Equals("DELETE", StringComparison.Ordinal))
        {
            Console.WriteLine("🛡️ Изтриването е отменено.");
            return;
        }

        if (isFile) File.Delete(fullPath);
        else Directory.Delete(fullPath, recursive: true);

        string log = $"🗑️ [ИЗТРИТО] {rel}";
        stats.CrudLog.Add($"[{DateTime.Now:ddd HH:mm:ss}] {log}");
        Console.WriteLine($"✅ {log}");
    }

    private static void OpenInVsCode(string file, int line)
    {
        try
        {
            var vsCodeExe = Process.GetProcessesByName("Code")
                .Select(p => { try { return p.MainModule?.FileName; } catch { return null; } })
                .FirstOrDefault(path => !string.IsNullOrEmpty(path));

            if (vsCodeExe != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = vsCodeExe,
                    Arguments = $"-r -g \"{file}:{line}\"",
                    UseShellExecute = false
                });
            }
        }
        catch { }
    }
}
