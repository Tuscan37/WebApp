namespace WebApp.Shared.Dto;

public class AssignmentDto
{
    public int Id { get; set; }
    public ProjectDto Project { get; set; }
    public string Name { get; set; }
    public int? Priority { get; set; }
    public string Description { get; set; }
    public DateTime DeadlineDateTime { get; set; }
}

