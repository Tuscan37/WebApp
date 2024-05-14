using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WebApp.Client.Services;

public class AuthenticationService
{
    private readonly NavigationManager _navigation;
    private readonly IAccessTokenProvider _tokenGoogleProvider;
    private readonly IAccessTokenProvider _tokenTwitterProvider;

    public AuthenticationService(NavigationManager navigation, IAccessTokenProvider google,
        IAccessTokenProvider twitter)
    {
        _navigation = navigation;
        _tokenGoogleProvider = google;
        _tokenTwitterProvider = twitter;
    }
    
}