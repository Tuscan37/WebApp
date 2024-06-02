using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Server.Data;
using WebApp.Server.Services;
using WebApp.Shared.Dto;
using WebApp.Server.Utility;
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
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginDto loginDto)
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
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        var login = new Data.Login
        {
            HashedAccessToken = Hasher.sha256_hash(accessToken),
            HashedRefreshToken = Hasher.sha256_hash(refreshToken),
            ExpiresAt = DateTime.UtcNow + Data.Login.RefreshTokenExpirationTimeSpan
        };
        _context.Logins.Add(login);
        await _context.SaveChangesAsync();
        return Ok(new LoginResult
        {
            Successful = true,
            AuthToken = new AuthToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResult>> Refresh(AuthToken authToken)
    {
        string accessToken = authToken.AccessToken;
        string refreshToken = authToken.RefreshToken;

        var jwtToken = _tokenService.GetJwtSecurityTokenFromString(accessToken);
        var emailClaim = jwtToken.Claims.Single(c => c.Type == ClaimTypes.Email);
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == emailClaim.Value);
        var hashedRefreshToken = Hasher.sha256_hash(refreshToken);
        var login = await _context.Logins.SingleOrDefaultAsync(l => l.HashedRefreshToken == hashedRefreshToken);
        
        if (user is null ||
            login is null ||
            login.ExpiresAt <= DateTime.UtcNow ||
            login.HashedAccessToken != Hasher.sha256_hash(accessToken)
            )
        {
            return BadRequest(new LoginResult
            {
                Successful = false,
                Error = "Invalid token: Expired or doesn't exist"
            });
        }
        
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        login.HashedAccessToken = Hasher.sha256_hash(newAccessToken);
        login.HashedRefreshToken = Hasher.sha256_hash(newRefreshToken);
        login.ExpiresAt = DateTime.UtcNow + Data.Login.RefreshTokenExpirationTimeSpan;
            
        await _context.SaveChangesAsync();
        return Ok(new LoginResult
        {
            Successful = true,
            AuthToken = new AuthToken
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }
        });
    }
}