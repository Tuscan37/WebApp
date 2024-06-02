using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApp.Server.Data;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Server.Services;

public class TokenService(IConfiguration config)
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(10);
    
    public string GenerateAccessToken(User user, string role="User")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var issuer = config["JwtSettings:Issuer"]!;
        var audience = config["JwtSettings:Audience"]!;
        var expires = DateTime.UtcNow + TokenLifetime;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.Name,user.Username)
        };

        JwtSecurityToken token = new JwtSecurityToken(issuer,
            audience,
            claims,
            expires:expires,
            signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public JwtSecurityToken GetJwtSecurityTokenFromString(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = config["JwtSettings:Audience"],
            ValidateIssuer = true,
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
            ValidateLifetime = false,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (securityToken == null ||
            !jwtSecurityToken!.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
        {
            throw new SecurityTokenException("Invalid Token");
        }
        return jwtSecurityToken;
        
    }
}