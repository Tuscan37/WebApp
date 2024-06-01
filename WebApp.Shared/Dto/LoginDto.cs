namespace WebApp.Shared.Dto;

public class LoginDto
{
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public bool RememberMe { get; set; }
}