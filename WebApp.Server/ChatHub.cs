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

public class Room(string roomId,string name)
{
    public string RoomId { get; } = roomId;
    public string Name { get; } = name;
    public int ConnectionCount = 1;
}
// TODO Lista pokojów
// losowy identyfikator
// zapraszanie

[Authorize]
public class ChatHub(IDictionary<string, Room> rooms) : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        //await Clients.Client(Context.ConnectionId).ReceiveInitialMessage(
        //    $"Thank you for connecting {Context.User?.Identity?.Name}"
        //);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //rooms.Remove()
    }

    public async Task CreateRoom(string name)
    {
        string roomId = Guid.NewGuid().ToString();
        if (rooms.TryAdd(roomId, new Room(roomId, name)))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.All.ReceiveNewRoomCreated(roomId, name);
            return;
        }

        await Clients.Client(Context.ConnectionId).ReceiveNotification("Error occured when creating a room");
    }

    public async Task JoinRoom(string roomId)
    {
        if (rooms.TryGetValue(roomId, out Room? room))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId);
            await Clients.Group(room.RoomId).UserConnectedToRoom(Context.User!.Identity!.Name!);
            room.ConnectionCount++;
            return;
        }
        await Clients.Client(Context.ConnectionId).ReceiveNotification("Room not found");

    }

    public async Task LeaveRoom(string roomId)
    {
        if (rooms.TryGetValue(roomId, out Room? room))
        {
            room.ConnectionCount--;
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
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
    Task ReceiveNotification(string notification);
    Task ReceiveNewRoomCreated(string roomId, string name);
    Task ReceiveAllRooms();
    Task ReceiveRoomRemoved(string roomId);
}