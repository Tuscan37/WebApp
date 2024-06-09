using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApp.Server;
[Authorize]
public class ChatHub : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId).ReceiveInitialMessage(
            $"Thank you for connecting {Context.User?.Identity?.Name}"
        );
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.ReceiveMessage(message, Context.User?.Identity?.Name );
    }
}

public interface IChatClient
{
    Task ReceiveMessage(string message, string? senderName);
    Task SendMessage(string message);
    Task ReceiveInitialMessage(string message);
}