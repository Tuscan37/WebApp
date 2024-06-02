using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace WebApp.Client.Authentication;

public class ApiAuthenticationStateProvider(ILocalStorageService localStorageService, HttpClient httpClient) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorageService.GetItemAsync<string>("accessToken");
        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(_anonymous);
        }
        
        var jwt = new JwtSecurityToken(savedToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
        return new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims))
        );
        
        //return new AuthenticationState(
        //    new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken)))
        //);
    }
    

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(ParseBase64(payload));
        claims.AddRange(keyValuePairs!.Select(kvp => new Claim(kvp.Key,kvp.Value.ToString()!)));
        return claims;
    }

    private string ParseBase64(string base64)
    {
        Console.WriteLine(base64);
        var bytes = Convert.FromBase64String(WithPadding(base64));
        return Encoding.UTF8.GetString(bytes);
    }

    private string WithPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                return base64 + "==";
            case 3:
                return base64 + "=";
            
        }
        return base64;
    }
    public void MarkUserAsAuthenticated(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }, "apiauth"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public void MarkUserAsLoggedOut()
    {
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
    }
}