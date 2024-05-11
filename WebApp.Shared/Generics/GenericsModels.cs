using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApp.Shared.Dto;

namespace WebApp.Shared.Generics;

public static class Generics
{
    public static ClaimsPrincipal SetClaimPrincipal(UserClaims userClaims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new("Id",userClaims.Id.ToString()),
            new(ClaimTypes.Role,userClaims.Role)
        },"JwtAuth"));
    }

    public static UserClaims GetClaimsFromToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        var claims = token.Claims;
        return new UserClaims
        {
            Id = claims.First(c => c.Type == "Id").Value!,
            Role = claims.First(c => c.Type == ClaimTypes.Role).Value!,
        };
    }
}