using Microsoft.EntityFrameworkCore;

namespace WebAppTestProject;
using WebAppSzczegielniak.Data;
[Collection("Sequential")]
public class UnitTest1 : BaseUnitTest
{
    [Fact]
    public async Task Test1()
    {
        Context.Users.Add(new User
        {
            Username = "Jan",
            Password = "1234"
        });
        var project = Context.Projects.Add(new Project
        {
            ProjectName = "WebApp",
            CreationDateTime = DateTime.UtcNow,
            DeadlineDateTime = DateTime.UtcNow.AddDays(60),
            Description = "cool app"
        });
        Context.Assignments.Add(new Assignment
        {
            Project = project.Entity,
            Description = "add some feature",
            DeadlineDateTime = DateTime.UtcNow.AddDays(7),
            Priority = 1000,
            Name = "Add Some Feature!!"

        });
        await Context.SaveChangesAsync();

        var assignmentInDb = await Context.Assignments.Include(a =>a.Project).FirstAsync();
        Assert.Equal(project.Entity.Id,assignmentInDb.Project.Id);

    }
}