using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAppSzczegielniak.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    private DbSet<User> Users { get; set; }
    private DbSet<Project> Projects { get; set; }
    private DbSet<Assignment> Assignments { get; set; }
}