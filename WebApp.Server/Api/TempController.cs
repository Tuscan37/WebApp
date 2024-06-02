using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using WebApp.Server.Data;
using WebApp.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace WebApp.Server.Api;
[ApiController]
[Route("api/temp")]
public class TempController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly ApplicationDbContext _context;
    
    public TempController([FromServices] TokenService tokenService,[FromServices] ApplicationDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }
    
    [Authorize]
    [HttpGet("temp1")]
    public ActionResult<String> Temp1()
    {
        var email = User.Claims.Single(c => c.Type == ClaimTypes.Email)
            .Value;
        return $"{email} Very cool!";
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
        string token = _tokenService.GenerateAccessToken(new User
        {
            Id = 420,
        });
        return token;
    }
    
    [HttpPost("tempLoginAdmin")]
    public ActionResult<String> TempLoginAdmin()
    {
        string token = _tokenService.GenerateAccessToken(new User
        {
            Id = 420,
        },"Admin");
        return token;
    }
    

    [HttpPost("CreateProjects")]
    public ActionResult CreateProjects()
    {
        _context.Projects.Add(new Project
        {
            ProjectName = "Haha!",
            Description = "Yeha!",
            DeadlineDateTime = DateTime.UtcNow + TimeSpan.FromDays(7)
        });
        _context.SaveChanges();
        return Ok();
    }
    
}