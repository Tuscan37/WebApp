using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using WebApp.Client.Utility;
using WebApp.Shared.Dto;

namespace WebApp.Client.Services;

public class ApiAuthenticationStateProvider(ILocalStorageService localStorageService ,HttpClient httpClient) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        httpClient.DefaultRequestHeaders.Authorization = null;
        AuthenticationState? state;
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            state = await GetAuthStateFromToken();
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
        var jwt = new JwtSecurityToken(accessToken);
        if (await IsTokenExpired())
        {
            var isSuccessful = await RefreshAuthToken();
            if (!isSuccessful)
            {
                state = await GetAuthStateFromToken();
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
        state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims,"apiauth")));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    public async Task<AuthenticationState> GetAuthStateFromToken()
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
            var stateAnonymous = new AuthenticationState(_anonymous);
            //NotifyAuthenticationStateChanged(Task.FromResult(stateAnonymous));
            return new AuthenticationState(_anonymous);
        }
        var jwt = new JwtSecurityToken(accessToken);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims,"apiauth")));
        //NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }
    
    public async Task<Boolean> IsTokenExpired()
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        //var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        var jwt = new JwtSecurityToken(accessToken);
        if (jwt.ValidTo <= DateTime.UtcNow - TimeSpan.FromSeconds(30))
        {
            return true;
        }

        return false;
    }
    
    private Task<bool>? _refreshTokenTask;
    public Task<bool> RefreshAuthToken()
    {
        if (_refreshTokenTask == null)
        {
            _refreshTokenTask = RefreshTokenTask();
        }

        return _refreshTokenTask;
    }

    private async Task<bool> RefreshTokenTask()
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        var response = await RequestNewAuthToken(new AuthToken{AccessToken = accessToken!,RefreshToken = refreshToken!}, httpClient);
        if (response.Successful)
        {
            await localStorageService.SetItemAsStringAsync("accessToken", response!.AuthToken!.AccessToken);
            await localStorageService.SetItemAsStringAsync("refreshToken", response!.AuthToken!.RefreshToken);
        }
        else
        {
            await localStorageService.RemoveItemAsync("accessToken");
            await localStorageService.RemoveItemAsync("refreshToken");
        }
        await GetAuthStateFromToken();
        _refreshTokenTask = null;
        return response.Successful;

    }
    private static async Task<LoginResult> RequestNewAuthToken(AuthToken authToken, HttpClient httpClient)
    {
        using StringContent json = Helpers.GetStringContentFromObject(authToken);
        var response = await httpClient.PostAsync("/api/user/refresh", json);
        var loginResult = JsonConvert.DeserializeObject<LoginResult>(await response.Content.ReadAsStringAsync());
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return loginResult!;
            }

            throw new Exception("Server Error");
        }
        return loginResult!;
    }
    
}