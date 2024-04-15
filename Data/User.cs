using System.ComponentModel.DataAnnotations;

namespace WebAppSzczegielniak.Data;

public class User
{
    [Key] public int Id { get; set; }
    [MaxLength(32)] public string Username { get; set; } = String.Empty;
    [MaxLength(32)] public string Password { get; set; } = String.Empty;
}