using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace WebAppSzczegielniak.Data;

public class User
{
    [Key] public int id { get; set; }
    public string username { get; set; } = String.Empty;
    public string password { get; set; } = String.Empty;

}