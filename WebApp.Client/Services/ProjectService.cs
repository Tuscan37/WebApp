using System.Text;
using Newtonsoft.Json;
using WebApp.Shared.Dto;

namespace WebApp.Client.Services;

public class ProjectService(HttpClient httpClient)
{
    private const string BaseUrl = "/api/projects";

    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        var response = await httpClient.GetAsync(BaseUrl);
        return JsonConvert.DeserializeObject<List<ProjectDto>>(await response.Content.ReadAsStringAsync())!;
    }
    public async Task<ProjectDto> GetProjectByIdAsync(int id)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/{id.ToString()}");
        return JsonConvert.DeserializeObject<ProjectDto>(await response.Content.ReadAsStringAsync())!;
    }
    public async Task AddProject(ProjectDto proj)
    {
        using StringContent jsonContent = new(JsonConvert.SerializeObject(proj), Encoding.UTF8, "application/json");
        var responseMessage = await httpClient.PostAsync(BaseUrl, jsonContent);
        responseMessage.EnsureSuccessStatusCode();
    }
    public async Task UpdateProjectAsync(ProjectDto proj)
    {
        using StringContent jsonContent = new(JsonConvert.SerializeObject(proj), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"{BaseUrl}/{proj.Id}", jsonContent);
        response.EnsureSuccessStatusCode();
    }
        public async Task DeleteProject(int id)
    {
        var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }
    public async Task<List<ProjectDto>> SearchProjectsAsync(string searchTerm)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/search?term={searchTerm}");
        return JsonConvert.DeserializeObject<List<ProjectDto>>(await response.Content.ReadAsStringAsync())!;
    }

}