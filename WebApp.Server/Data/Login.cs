using System.ComponentModel.DataAnnotations;

namespace WebApp.Server.Data;

public class Login
{
    [Key] public int Id { get; set; }
    public required string HashedAccessToken { get; set; }
    public required string HashedRefreshToken { get; set; }
    public required DateTime ExpiresAt { get; set; }
    
    public static TimeSpan RefreshTokenExpirationTimeSpan = TimeSpan.FromDays(7);
}