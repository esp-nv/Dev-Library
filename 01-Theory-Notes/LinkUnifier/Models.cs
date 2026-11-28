using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinkUnifier;

public class FolderNode
{
    public string GeneratedAt { get; set; } = "";
    public int Depth { get; set; }
    public string FolderName { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public string Title { get; set; } = "";
    public bool IsEmpty { get; set; }
    public string? PrevSiblingFolder { get; set; }
    public string? NextSiblingFolder { get; set; }

    [JsonIgnore]
    public FolderNode? Parent { get; set; }

    public List<FolderNode> SubFolders { get; set; } = new();
    public List<LessonNode> Lessons { get; set; } = new();
}

public class LessonNode
{
    public string FileName { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public string Title { get; set; } = "";
    public string? PrevLessonFile { get; set; }
    public string? NextLessonFile { get; set; }
}

public class AuditIssue
{
    public int Id { get; set; }
    public string FullFilePath { get; set; } = "";
    public string RelativeFile { get; set; } = "";
    public int LineNumber { get; set; }
    public bool IsAnchorOnly { get; set; }
    public string Reason { get; set; } = "";
    public string OriginalLink { get; set; } = "";
    public string ProposedReplacement { get; set; } = "";
}

public class AuditStats
{
    public int RenamedSyntaxFiles { get; set; }
    public int CreatedReadmes { get; set; }
    public int UpdatedNavFiles { get; set; }
    public List<string> ApprovedLog { get; } = new();
    public List<string> DeletedLinesLog { get; } = new();
    public List<string> RejectedLog { get; } = new();
    public List<string> NavSyncLog { get; } = new();
    public List<string> CrudLog { get; } = new();
    public List<AuditIssue> LastAuditIssues { get; } = new();
}
