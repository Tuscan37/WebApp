using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WebApp.Server;


// TODO Lista pokojów
// losowy identyfikator
// zapraszanie

[Authorize]
//public class ChatHub(IDictionary<string, Room> rooms) : Hub<IChatClient>
public class ChatHub([FromServices] HubMemory hubMemory, [FromServices] ILogger<ChatHub> logger) : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        //await Clients.Client(Context.ConnectionId).ReceiveInitialMessage(
        //    $"Thank you for connecting {Context.User?.Identity?.Name}"
        //);
        var rooms = hubMemory.Rooms.Values.Select(r =>new RoomDto(r.RoomId,r.Name)).ToList();
        await Clients.All.ReceiveAllRooms(rooms);
        logger.LogInformation("User connected");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var rooms = hubMemory.Rooms.Where(r => r.Value.ConnectionIds.Contains(Context.ConnectionId))
            .ToList();
        foreach (var room in rooms)
        {
            logger.LogInformation("Room: {roomName} has {x} connections", room.Value.Name, room.Value.ConnectionIds.Count);
            if (room.Value.ConnectionIds.Count == 0)
            {
                hubMemory.Rooms.Remove(room.Key);
                await Clients.All.ReceiveRoomRemoved(room.Key);
                logger.LogInformation("Room removed");
            }
        }
        logger.LogInformation("User disconnected");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(string name)
    {
        if (hubMemory.Rooms.Any(r => r.Value.ConnectionIds.Contains(Context.ConnectionId)))
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification("You can't create a room while in a room.");
            return;
        }
        string roomId = Guid.NewGuid().ToString();
        var room = new Room(roomId, name);
        if (hubMemory.Rooms.TryAdd(roomId, room))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.All.ReceiveNewRoomCreated(roomId, name);
            await Clients.Client(Context.ConnectionId).ReceiveRoomCreatedConfirmation(new RoomDto(roomId, name));
            room.ConnectionIds.Add(Context.ConnectionId);
            logger.LogInformation("Created new Room");
            return;
        }
        logger.LogInformation("Failed to create room");
        await Clients.Client(Context.ConnectionId).ReceiveNotification("Error occured when creating a room");
    }

    public async Task JoinRoom(string roomId)
    {
        if (hubMemory.Rooms.Any(r => r.Value.ConnectionIds.Contains(Context.ConnectionId)))
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification("You are already in a room.");
        }
        if (hubMemory.Rooms.TryGetValue(roomId, out Room? room))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId);
            await Clients.Group(room.RoomId).UserConnectedToRoom(Context.User!.Identity!.Name!);
            room.ConnectionIds.Add(Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).ReceiveRoomJoinedRoomConfirmation(new RoomDto(room.RoomId,room.Name));
            logger.LogInformation("User joined a room");
            return;
        }
        await Clients.Client(Context.ConnectionId).ReceiveNotification("Room not found");
        
    }

    public async Task LeaveRoom(string roomId)
    {
        if (hubMemory.Rooms.TryGetValue(roomId, out Room? room))
        {
            room.ConnectionIds.Remove(Context.ConnectionId);
            logger.LogInformation("Room: {roomName} has {x} connections", room.Name, room.ConnectionIds.Count);
            if (room.ConnectionIds.Count == 0)
            {
                hubMemory.Rooms.Remove(roomId);
                await Clients.All.ReceiveRoomRemoved(roomId);
                logger.LogInformation("Room removed");
            }
            logger.LogInformation("User left a room");
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
    Task ReceiveAllRooms(List<RoomDto> rooms);
    Task ReceiveRoomRemoved(string roomId);
    Task ReceiveRoomCreatedConfirmation(RoomDto room);
    Task ReceiveRoomJoinedRoomConfirmation(RoomDto room);
    Task ReceiveCurrentRoomUsers(List<string> users);
}