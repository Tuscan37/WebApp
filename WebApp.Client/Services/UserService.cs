using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using WebApp.Client.Authentication;
using WebApp.Shared.Dto;


namespace WebApp.Client.Services;

public class UserService(HttpClient httpClient, ApiAuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
{
    public async Task<LoginResult> Login(LoginDto loginDto)
    {
        using StringContent json = new(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/api/user/login", json);
        var loginResult = JsonConvert.DeserializeObject<LoginResult>(await response.Content.ReadAsStringAsync());
        if (!response.IsSuccessStatusCode)
        {
            return loginResult!;
        }

        await localStorageService.SetItemAsStringAsync("authToken", loginResult!.Token);
        authenticationStateProvider.MarkUserAsAuthenticated(loginDto.Email);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
        return loginResult;
    }

    public async Task Logout()
    {
        await localStorageService.RemoveItemAsync("authToken");
        authenticationStateProvider.MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}