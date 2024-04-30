using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebAppSzczegielniak.Data;

namespace WebAppSzczegielniak.Token;

public class TokenGenerator
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    
    private readonly IConfiguration _config;
    public TokenGenerator(IConfiguration config)
    {
        _config = config;
    }
    
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var issuer = _config["JwtSettings:Issuer"]!;
        var audience = _config["JwtSettings:Audience"]!;
        var expires = DateTime.UtcNow + TokenLifetime;

        var claims = new List<Claim>
        {
            new("userId",user.Id.ToString()),
            new(ClaimTypes.Role,"User")
        };

        JwtSecurityToken token = new JwtSecurityToken(issuer,audience,claims,expires:expires,signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);


    }
}