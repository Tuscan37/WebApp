using Microsoft.EntityFrameworkCore;
using WebApp.Server.Data;
namespace WebApp.Test;
public class UnitTest1 : IDisposable
{
    private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=webappszczegielniaktest;User Id=appuser;Password=1234";
    private readonly ApplicationDbContext _context;

    public UnitTest1()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
        _context.Database.EnsureDeleted();
        _context.Database.Migrate();

    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Fact]
    public async Task Test1()
    {
        _context.Users.Add(new User
        {
            Username = "Jan",
            Password = "1234"
        });
        var project = _context.Projects.Add(new Project
        {
            ProjectName = "WebApp",
            CreationDateTime = DateTime.UtcNow,
            DeadlineDateTime = DateTime.UtcNow.AddDays(60),
            Description = "cool app"
        });
        _context.Assignments.Add(new Assignment
        {
            Project = project.Entity,
            Description = "add some feature",
            DeadlineDateTime = DateTime.UtcNow.AddDays(7),
            Priority = 1000,
            Name = "Add Some Feature!!"

        });
        await _context.SaveChangesAsync();

        var assignmentInDb = await _context.Assignments.Include(a =>a.Project).FirstAsync();
        Assert.Equal(project.Entity.Id,assignmentInDb.Project.Id);

    }
}