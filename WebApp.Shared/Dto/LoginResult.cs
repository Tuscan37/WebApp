namespace WebApp.Shared.Dto;

public class LoginResult
{
    public required bool Successful { get; set; }
    public string Error { get; set; } = "";
    public AuthToken? AuthToken { get; set; }
}