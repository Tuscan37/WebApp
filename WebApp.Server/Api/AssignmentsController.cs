using WebApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [HttpGet("{projectid}")]
    public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByProject(int projectid)
    {
        var assignments = await _context.Assignments.Include(x=>x.Project).Where(a=>a.Project.Id==projectid).ToListAsync();

        return assignments;
    }
}
