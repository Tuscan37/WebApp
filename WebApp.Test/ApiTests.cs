using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Server.Api;
using WebApp.Server.Data;
using WebApp.Shared.Dto;

namespace WebApp.Test
{
    public class ApiTests : IDisposable
    {
        private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=webappszczegielniaktest;User Id=appuser;Password=1234";
        private readonly ApplicationDbContext _context;

        public ApiTests()
        {
            _context = new ApplicationDbContext(
             new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseNpgsql(ConnectionString)
                 .Options);
            //_context.Database.EnsureDeleted();
            _context.Database.Migrate();
        }

        public void Dispose()
        {
            //_context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // ProjectsController Tests

        [Fact]
        public async Task GetProject_ReturnsProject()
        {
            var project = await _context.Projects.FirstAsync();
            var controller = new ProjectsController(_context);

            var result = await controller.GetProject(project.Id);
            var returnedProject = result.Value as Project;

            Assert.NotNull(returnedProject);
            Assert.Equal(project.Id, returnedProject.Id);
            Assert.Equal(project.ProjectName, returnedProject.ProjectName);
        }

        [Fact]
        public async Task GetProjects_ReturnsProjects()
        {
            var controller = new ProjectsController(_context);

            var result = await controller.GetProjects(null, null, null);
            var projects = result.Value as List<ProjectDto>;

            Assert.NotNull(projects);
            Assert.NotEmpty(projects);
        }

        [Fact]
        public async Task CreateProject_ReturnsCreatedProject()
        {
            var newProject = new Project
            {
                ProjectName = "New Project",
                Description = "New Description",
                DeadlineDateTime = DateTime.UtcNow.AddMonths(2)      
        };

            var controller = new ProjectsController(_context);

            var result = await controller.CreateProject(new ProjectDto
            {
                ProjectName = newProject.ProjectName,
                Description = newProject.Description,
                DeadlineDateTime = newProject.DeadlineDateTime
            });
            var createdResult = result.Result as CreatedAtActionResult;
            var createdProject = createdResult.Value as Project;

            Assert.NotNull(createdResult);
            Assert.NotNull(createdProject);
            Assert.Equal(newProject.ProjectName, createdProject.ProjectName);
            Assert.Equal(newProject.Description, createdProject.Description);
        }

        [Fact]
        public async Task UpdateProject_ReturnsNoContent()
        {
            var project = await _context.Projects.FirstAsync();
            project.ProjectName = "Updated Project";
            project.Description = "Updated Description";
            project.CreationDateTime = project.CreationDateTime.AddDays(1);

            var controller = new ProjectsController(_context);

            var result = await controller.UpdateProject(project.Id, new ProjectDto
            {
                ProjectName = project.ProjectName,
                Description = project.Description,
                CreationDateTime = project.CreationDateTime
                
            });
            var noContentResult = result as NoContentResult;

            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteProject_ReturnsOk()
        {
            var project = await _context.Projects.FirstAsync();
            var controller = new ProjectsController(_context);

            var result = await controller.DeleteProject(project.Id);
            var okResult = result as OkObjectResult;

            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        // AssignmentsController Tests

        [Fact]
        public async Task GetAssignmentsByProject_ReturnsAssignments()
        {
            var projectId = 1;
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                Assert.True(false, $"Project with ID {projectId} not found.");
            }
            var controller = new AssignmentsController(_context);

            var result = await controller.GetAssignmentsByProject(project.Id);
            var assignments = result.Value as List<Assignment>;

            Assert.NotNull(assignments);
            Assert.NotEmpty(assignments);
            Assert.All(assignments, a => Assert.Equal(project.Id, a.Project.Id));
        }

    }
}
