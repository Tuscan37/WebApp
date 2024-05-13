using System.Security.Claims;

namespace WebApp.Shared.Dto;

public class UserClaims
{
    public required string Id { get; set; }
    public required string Role { get; set; }
}