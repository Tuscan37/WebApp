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
    public List<string> ConnectionIds { get; } = new();
}

public class RoomDto(string roomId, string name)
{
    public string RoomId { get; } = roomId;
    public string Name { get; } = name;
}
public class HubMemory
{
    public Dictionary<string,Room> Rooms = new();
}