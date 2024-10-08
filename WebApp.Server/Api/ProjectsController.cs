using WebApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Shared.Dto;

namespace WebApp.Server.Api;
[Route("api/projects")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // pobranie jednego projektu po id
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    // pobranie wszystkich projektów
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(
        [FromQuery] int? id, [FromQuery] string? projectname, [FromQuery] string? description
        )
    {
        IQueryable<Project> projekt = _context.Projects;

        if (id.HasValue){
            projekt = projekt.Where(p =>p.Id == id);
        }
        if (!string.IsNullOrEmpty(projectname)){
            projekt = projekt.Where(p => p.ProjectName.Contains(projectname));
        }
        if (!string.IsNullOrEmpty(description)){
            projekt = projekt.Where(p => p.Description.Contains(description));
        }

        return await projekt.Select(p => new ProjectDto
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Description = p.Description,
            CreationDateTime = p.CreationDateTime,
            DeadlineDateTime = p.DeadlineDateTime,
        }).ToListAsync();
    }

    // nowy projekt
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(ProjectDto projectDto)
    {
        var project = new Project
        {
            ProjectName = projectDto.ProjectName,
            Description = projectDto.Description,
            DeadlineDateTime = Convert.ToDateTime(projectDto.DeadlineDateTime).ToUniversalTime().AddMinutes(60),
            CreationDateTime = DateTime.UtcNow
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // modyfikacja projektu
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectDto project)
    {
        if (id != project.Id)
        {
            return BadRequest();
        }

        var existingProject = await _context.Projects.FindAsync(id);
        if (existingProject == null)
        {
            return NotFound();
        }

        // Aktualizowanie właściwości projektu
        existingProject.ProjectName = project.ProjectName;
        existingProject.Description = project.Description;
        existingProject.DeadlineDateTime = Convert.ToDateTime(project.DeadlineDateTime).ToUniversalTime().AddMinutes(60);
        //existingProject.CreationDateTime = project.CreationDateTime;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }


    // usuniecie projektu
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok("Pomyślenie usunięto projekt");
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }

    // wyszukiwanie projektów
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> SearchProjects(string term)
    {
        int termAsInt;
        bool isInt = Int32.TryParse(term, out termAsInt);
        var projects = await _context.Projects
            .Where(p => p.ProjectName.Contains(term) || p.Description.Contains(term) || (isInt && p.Id.Equals(termAsInt)))
            .ToListAsync();

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Description = p.Description,
            CreationDateTime = p.CreationDateTime,
            DeadlineDateTime = p.DeadlineDateTime,
        }).ToList();
    }

}
