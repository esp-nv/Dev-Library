using System.Collections.Generic;

namespace LinkUnifier;

public class FolderNode
{
    public string FullPath { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public string FolderName { get; set; } = "";
    public string Title { get; set; } = "";
    public string DisplayTitle { get => Title; set => Title = value; }
    public int Depth { get; set; }
    public bool HasReadme { get; set; }
    public FolderNode? Parent { get; set; }
    public string? PrevSiblingFolder { get; set; }
    public string? NextSiblingFolder { get; set; }
    public List<FolderNode> SubFolders { get; set; } = new();
    public List<LessonInfo> Lessons { get; set; } = new();
    public bool IsEmpty => SubFolders.Count == 0 && Lessons.Count == 0;
}

public class LessonInfo
{
    public string FullPath { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public string Title { get; set; } = "";
    public string? PrevLessonFile { get; set; }
    public string? NextLessonFile { get; set; }
}

public class ValidRefInfo
{
    public string RelativeFile { get; set; } = "";
    public bool IsAnchorOnly { get; set; }
}
