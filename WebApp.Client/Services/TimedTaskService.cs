namespace WebApp.Client.Services;

public class TimedTaskService : IDisposable
{
    private readonly Timer _timer;
    private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(15);
    private readonly IServiceScopeFactory _scopeFactory;
    public TimedTaskService(IServiceScopeFactory scopeFactory)
    {
        Console.WriteLine("Timed Task registered");
        _scopeFactory = scopeFactory;
        _timer = new Timer(TempName, null, TimeSpan.Zero, _timeSpan);
        
    }

    async void TempName(object? obj)
    {
        using var scope = _scopeFactory.CreateScope();
        var stateProvider = scope.ServiceProvider.GetService<ApiAuthenticationStateProvider>();
        //var state = await stateProvider!.GetAuthStateFromToken();
        await stateProvider!.GetAuthenticationStateAsync();
        //NotifyAuthenticationStateChanged(Task.FromResult(state));
    }
    
    public void Dispose()
    {
        _timer.Dispose();
    }
    
}