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
}