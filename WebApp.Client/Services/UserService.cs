using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using WebApp.Client.Authentication;
using WebApp.Shared.Dto;
using WebApp.Client.Utility;

namespace WebApp.Client.Services;

public class UserService(HttpClient httpClient, ApiAuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
{
    public async Task<LoginResult> Login(LoginDto loginDto)
    {
        //using StringContent json = new(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
        using StringContent json = Helpers.GetStringContentFromObject(loginDto);
        var response = await httpClient.PostAsync("/api/user/login", json);
        var loginResult = JsonConvert.DeserializeObject<LoginResult>(await response.Content.ReadAsStringAsync());
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return loginResult!;
            }

            throw new Exception("Server Error");

        }

        await localStorageService.SetItemAsStringAsync("accessToken", loginResult!.AuthToken!.AccessToken);
        await localStorageService.SetItemAsStringAsync("refreshToken", loginResult!.AuthToken!.RefreshToken);
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return loginResult;
    }

    public async Task<LoginResult> Refresh(AuthToken authToken)
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

        await localStorageService.SetItemAsStringAsync("accessToken", loginResult!.AuthToken!.AccessToken);
        await localStorageService.SetItemAsStringAsync("refreshToken", loginResult!.AuthToken!.RefreshToken);
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return loginResult;
    }

    public async Task Logout()
    {
        var accessToken = await localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
        if (accessToken is null || refreshToken is null)
        {
            return;
        }
        var json = Helpers.GetStringContentFromObject(new AuthToken
        {
            AccessToken = accessToken!,
            RefreshToken = refreshToken!
        });
        await localStorageService.RemoveItemAsync("accessToken");
        await localStorageService.RemoveItemAsync("refreshToken");
        var response = await httpClient.PostAsync("/api/user/logout",json);
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
    }
    
    public async Task<string> AuthTest()
    {
        await Helpers.RefreshTokenIfExpired(this,localStorageService);
        var response = await httpClient.GetAsync("api/temp/temp1");
        return await response.Content.ReadAsStringAsync();
    }
}