using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.Server.Data;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Server.Services;

public class TokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenService(IConfiguration config,IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GenerateToken(User user, string role="User")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var issuer = _config["JwtSettings:Issuer"]!;
        var audience = _config["JwtSettings:Audience"]!;
        var expires = DateTime.UtcNow + TokenLifetime;

        var claims = new List<Claim>
        {
            new("Id",user.Id.ToString()),
            new(ClaimTypes.Role,role)
        };

        JwtSecurityToken token = new JwtSecurityToken(issuer,audience,claims,expires:expires,signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }

    public string? GetId()
    {
        return _httpContextAccessor?.HttpContext?.User.FindFirstValue("Id");
    }

    public string? GetRole()
    {
        return _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    }
}