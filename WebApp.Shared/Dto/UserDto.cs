namespace WebApp.Shared.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}