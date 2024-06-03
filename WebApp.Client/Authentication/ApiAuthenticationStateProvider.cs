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
        Console.WriteLine(savedToken);
        httpClient.DefaultRequestHeaders.Authorization = null;
        if (string.IsNullOrWhiteSpace(savedToken))
        {
            var stateAnonymous = new AuthenticationState(_anonymous);
            NotifyAuthenticationStateChanged(Task.FromResult(stateAnonymous));
            return new AuthenticationState(_anonymous);
        }
        var jwt = new JwtSecurityToken(savedToken);
        //var claims = new List<Claim>();
        //claims.AddRange(jwt.Claims.Where(c => c.Type != ClaimTypes.Role));
        

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims,"apiauth")));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }
    
}