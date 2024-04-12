using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace WebAppSzczegielniak.Data;

public class User
{
    [Key] public int Id { get; set; }
    public string Username { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;

}