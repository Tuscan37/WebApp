using System.ComponentModel.DataAnnotations;

namespace WebAppSzczegielniak.Data;

public class Project
{
    [Key] public int Id { get; set; }
    [MaxLength(50)] public string ProjectName { get; set; } = String.Empty;
    [MaxLength(1000)]
    public string Description { get; set; } = String.Empty;
    public DateTime CreationDateTime { get; set; }
    public DateTime? DeadlineDateTime { get; set; }
}