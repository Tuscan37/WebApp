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
        authenticationStateProvider.MarkUserAsAuthenticated(loginDto.Email);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AuthToken.AccessToken);
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
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AuthToken.AccessToken);
        return loginResult;
    }

    public async Task Logout()
    {
        await localStorageService.RemoveItemAsync("accessToken");
        await localStorageService.RemoveItemAsync("refreshToken");
        httpClient.DefaultRequestHeaders.Authorization = null;
        authenticationStateProvider.MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
    
    public async Task<string> AuthTest()
    {
        await Helpers.RefreshTokenIfExpired(this,localStorageService);
        var response = await httpClient.GetAsync("api/temp/temp1");
        return await response.Content.ReadAsStringAsync();
    }
}