using Microsoft.EntityFrameworkCore;
using WebApp.Server.Data;

namespace WebApp.Server.Services;

public class DatabaseCleanupService(ILogger<DatabaseCleanupService> logger, IServiceScopeFactory scopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer = null;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(CleanDb, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        return Task.CompletedTask;
    }

    private async void CleanDb(object? state)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        await context!.Logins.Where(l => l.ExpiresAt <= DateTime.UtcNow).ExecuteDeleteAsync();
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}