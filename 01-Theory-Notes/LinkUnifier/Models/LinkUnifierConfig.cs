using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LinkUnifier;

public sealed class LinkUnifierConfig
{
    public string JsonMapFileName { get; set; } = "library-map.json";
    public string ReportsFolderName { get; set; } = "reports";
    public List<string> IgnoredFolders { get; set; } = new() { "assets", "bin", "obj", ".git", ".vs", ".vscode" };

    // Имена на генерираните отчети (променят се само оттук)
    public string TreeMapReportFile { get; set; } = "00-library-tree-map.md";
    public string SummaryReportFile { get; set; } = "01-summary-report.txt";
    public string ApprovedReportFile { get; set; } = "02-approved-links.txt";
    public string DeletedReportFile { get; set; } = "03-deleted-lines.txt";
    public string GroupedAuditReportFile { get; set; } = "04-grouped-audit-report.txt";
    public string EmptyFoldersReportFile { get; set; } = "05-empty-folders.txt";
    public string CrudReportFile { get; set; } = "06-crud-changes.txt";
    public string NavSyncReportFile { get; set; } = "07-navbar-footer-sync.txt";
    public string InProgressReportFile { get; set; } = "08-in-progress-to-create.txt";
    public string MissingFilesReportFile { get; set; } = "09-missing-target-files.txt";

    public static LinkUnifierConfig LoadOrCreate(string projectDir)
    {
        string configPath = Path.Combine(projectDir, "linkunifier.config.json");
        if (File.Exists(configPath))
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<LinkUnifierConfig>(File.ReadAllText(configPath));
                if (loaded != null) return loaded;
            }
            catch { /* fallback to defaults */ }
        }

        var cfg = new LinkUnifierConfig();
        File.WriteAllText(configPath, JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
        return cfg;
    }
}
