using WebApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Shared.Dto;
namespace WebApp.Server.Api;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssignmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // pobranie jednego projektu po id
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByProject(int projectId)
    {
        var assignments = await _context.Assignments
            .Where(a => a.Project.Id == projectId)
            .Select(a => new AssignmentDto
            {
                Id = a.Id,
                Name = a.Name,
                Priority = a.Priority,
                Description = a.Description,
                DeadlineDateTime = a.DeadlineDateTime,
                Project = new ProjectDto
                {
                    Id = a.Project.Id,
                    ProjectName = a.Project.ProjectName,
                    Description = a.Project.Description,
                    CreationDateTime = a.Project.CreationDateTime,
                    DeadlineDateTime = a.Project.DeadlineDateTime
                }
            })
            .ToListAsync();

        return Ok(assignments);
    }

    // nowy task
    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(AssignmentDto assignmentDto)
    {
        var project = await _context.Projects.FindAsync(assignmentDto.Project.Id);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        var assignment = new Assignment
        {
            Project = project,
            Name = assignmentDto.Name,
            Priority = assignmentDto.Priority,
            Description = assignmentDto.Description,
            DeadlineDateTime = Convert.ToDateTime(assignmentDto.DeadlineDateTime).ToUniversalTime()
        };

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        assignmentDto.Id = assignment.Id;

        return CreatedAtAction(nameof(GetAssignmentsByProject), new { projectId = assignment.Project.Id }, assignmentDto);
    }

    // modyfikacja tasaka
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssignment(int id, AssignmentDto assignmentDto)
    {
        if (id != assignmentDto.Id)
        {
            return BadRequest("Assignment ID not found");
        }

        var assignment = await _context.Assignments.Include(x => x.Project).FirstOrDefaultAsync(a => a.Id == id);
        if (assignment == null)
        {
            return NotFound("Assignment not found");
        }

        assignment.Name = assignmentDto.Name;
        assignment.Priority = assignmentDto.Priority;
        assignment.Description = assignmentDto.Description;
        assignment.DeadlineDateTime = Convert.ToDateTime(assignmentDto.DeadlineDateTime).ToUniversalTime();

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // kasacja taska
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null)
        {
            return NotFound("Assignment not found");
        }

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // pobranie wszystkich taskow
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignments(
        [FromQuery] int? id, [FromQuery] string? name, [FromQuery] int? priority, [FromQuery] string? description)
    {
        IQueryable<Assignment> query = _context.Assignments.Include(a => a.Project);

        if (id.HasValue)
        {
            query = query.Where(a => a.Id == id);
        }
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }
        if (priority.HasValue)
        {
            query = query.Where(a => a.Priority == priority);
        }
        if (!string.IsNullOrEmpty(description))
        {
            query = query.Where(a => a.Description.Contains(description));
        }

        return await query.Select(a => new AssignmentDto
        {
            Id = a.Id,
            Name = a.Name,
            Priority = a.Priority,
            Description = a.Description,
            DeadlineDateTime = a.DeadlineDateTime,
            Project = new ProjectDto
            {
                Id = a.Project.Id,
                ProjectName = a.Project.ProjectName,
                Description = a.Project.Description,
                CreationDateTime = a.Project.CreationDateTime,
                DeadlineDateTime = a.Project.DeadlineDateTime
            }
        }).ToListAsync();
    }
}
