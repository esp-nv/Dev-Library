using System.Collections.Generic;

namespace LinkUnifier;

public class LinkIssue
{
    public int Id { get; set; }
    public string RelativeFile { get; set; } = "";
    public int LineNumber { get; set; }
    public string LinkText { get; set; } = "";
    public string LinkTarget { get; set; } = "";
    public string OriginalLink { get; set; } = "";
    public string ProposedReplacement { get; set; } = "";
    public string Reason { get; set; } = "";
    public bool IsAnchorOnly { get; set; }
}

public class AuditStats
{
    public int RenamedSyntaxFiles { get; set; }
    public int CreatedReadmes { get; set; }
    public int UpdatedNavFiles { get; set; }

    public List<ValidRefInfo> ValidRefs { get; set; } = new();
    public List<ValidRefInfo> InProgressRefs { get; set; } = new();
    public List<LinkIssue> LastAuditIssues { get; set; } = new();

    public List<string> ApprovedLog { get; set; } = new();
    public List<string> RejectedLog { get; set; } = new();
    public List<string> SkippedLog { get; set; } = new();
    public List<string> DeletedLinesLog { get; set; } = new();
    public List<string> NavSyncLog { get; set; } = new();
    public List<string> CrudLog { get; set; } = new();
}
