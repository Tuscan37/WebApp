using WebAppSzczegielniak.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects(
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

        return await projekt.ToListAsync();
    }

    // nowy projekt
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // modyfikacja projektu
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        if (id != project.Id)
        {
            return BadRequest();
        }

        _context.Entry(project).State = EntityState.Modified;

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

}
