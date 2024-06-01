using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Server.Data;
using WebApp.Server.Services;
using WebApp.Shared.Dto;

namespace WebApp.Server.Api;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private ApplicationDbContext _context;
    private TokenService _tokenService;
    private ILogger _logger;
    public UserController([FromServices] ApplicationDbContext context,[FromServices] TokenService tokenService,ILogger<UserController> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<String>> Login([FromBody] LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(loginDto.Email));
        if (user is null)
        {
            _logger.LogInformation("Log in Login method: bad email");
            return BadRequest(new LoginResult
            {
                Successful = false,
                Error = "Invalid email or password"
            });
        }

        if (!user.Password.Equals(loginDto.Password))
        {
            _logger.LogInformation("Log in Login method: bad password");
            return BadRequest(new LoginResult
            {
                Successful = false,
                Error = "Invalid email or password"
            });
        }
        _logger.LogInformation("Log in Login method: correct email and password");
        string token = _tokenService.GenerateToken(user);
        return Ok(new LoginResult
        {
            Successful = true,
            Token = token
        });
    }
}