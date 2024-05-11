using System.ComponentModel.DataAnnotations;

namespace App.Server.Data;

public class Assignment
{
    [Key] public int Id { get; set; }
    public Project Project { get; set; }
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    public int? Priority { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public DateTime DeadlineDateTime { get; set; }
}