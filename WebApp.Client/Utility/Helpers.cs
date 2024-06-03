using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using WebApp.Client.Services;
using WebApp.Shared.Dto;

namespace WebApp.Client.Utility;

public static class Helpers
{
    public static StringContent GetStringContentFromObject(object obj)
    {
        return new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    public static async Task RefreshTokenIfExpired(UserService userService, ILocalStorageService localStorageService)
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        var jwt = new JwtSecurityToken(accessToken);
        if (jwt.ValidTo <= DateTime.UtcNow - TimeSpan.FromSeconds(30))
        {
            var response = await userService.Refresh(new AuthToken{AccessToken = accessToken!,RefreshToken = refreshToken!});
        }
        
    }
}