using Microsoft.EntityFrameworkCore;
using WebAppSzczegielniak.Data;

namespace WebAppTestProject;

public class BaseUnitTest : IDisposable
{
    private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=webappszczegielniaktest;User Id=appuser;Password=1234";
    protected readonly ApplicationDbContext Context;
    
    protected BaseUnitTest()
    {
        Context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
        Context.Database.EnsureDeleted();
        Context.Database.Migrate();

    }
    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}