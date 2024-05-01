using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppSzczegielniak.Data;
using WebAppSzczegielniak.Services;

namespace WebAppSzczegielniak.Api;

[ApiController]
[Route("api/temp")]
public class TempController : ControllerBase
{
    private readonly TokenService _tokenService;
    public TempController([FromServices] TokenService tokenService)
    {
        _tokenService = tokenService;
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
        string token = _tokenService.GenerateToken(new User
        {
            Id = 420,
        });
        return token;
    }

    [HttpGet("tempReadClaims")]
    [Authorize]
    public ActionResult<string> ReadClaims()
    {
        string id = _tokenService.GetId()!;
        string role = _tokenService.GetRole()!;
        return Ok(new { id, role });
    }
    
}