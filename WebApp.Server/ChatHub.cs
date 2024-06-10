using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApp.Server;

public class Message
{
    public string Content;
    public string? UserName;
    public Message(string content, string? userName)
    {
        Content = content;
        UserName = userName;
    }
}

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

    public async Task JoinRoom(string room)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).UserConnectedToRoom(Context.User!.Identity!.Name!);
    }

    public async Task LeaveRoom(string room)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
    }

    public async Task SendMessage(string message, string room)
    {
        var msg = new Message(message, Context.User?.Identity?.Name);
        await Clients.Group(room).ReceiveMessage(msg.Content,msg.UserName, room);
    }
}

public interface IChatClient
{
    Task ReceiveMessage(string message, string? senderName, string room);
    Task ReceiveInitialMessage(string message);
    Task UserConnectedToRoom(string username);
}