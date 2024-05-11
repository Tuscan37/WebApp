using System.Security.Claims;
using App.Server.Data;
using App.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        return "You are a regular user!";
    }
        
    [Authorize(Roles = "Admin")]
    [HttpGet("temp2")]
    public ActionResult<String> Temp2()
    {
        return "You are an administrator!";
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
    
    [HttpPost("tempLoginAdmin")]
    public ActionResult<String> TempLoginAdmin()
    {
        string token = _tokenService.GenerateToken(new User
        {
            Id = 420,
        },"Admin");
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