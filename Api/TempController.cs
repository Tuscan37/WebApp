using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppSzczegielniak.Data;
using WebAppSzczegielniak.Token;

namespace WebAppSzczegielniak.Api;

[ApiController]
[Route("api/temp")]
public class TempController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    public TempController([FromServices] TokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }
    
    [Authorize]
    [HttpGet("temp1")]
    public ActionResult<String> Temp1()
    {
        return "Cool!";
    }

    [HttpPost("tempLogin")]
    public ActionResult<String> TempLogin()
    {
        string token = _tokenGenerator.GenerateToken(new User
        {
            Id = 420
        });
        return token;
    }

    [HttpGet("tempReadClaims")]
    [Authorize]
    public ActionResult<string> ReadClaims()
    {
        string id = User.FindFirstValue("userId")!;
        string role = User.FindFirstValue(ClaimTypes.Role)!;
        return Ok(new { id, role });
    }
    
}