namespace WebApp.Shared.Dto;

public class ProjectDto
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreationDateTime { get; set; }
    public DateTime? DeadlineDateTime { get; set; }
}