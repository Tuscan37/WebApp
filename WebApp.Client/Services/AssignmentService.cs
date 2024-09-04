using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using WebApp.Shared.Dto;

namespace WebApp.Client.Services;

public class AssignmentService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/assignments";

    public AssignmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AssignmentDto>> GetAssignmentsAsync()
    {
        var response = await _httpClient.GetAsync(BaseUrl);
        return JsonConvert.DeserializeObject<List<AssignmentDto>>(await response.Content.ReadAsStringAsync())!;
    }

    public async Task<AssignmentDto> GetAssignmentByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
        return JsonConvert.DeserializeObject<AssignmentDto>(await response.Content.ReadAsStringAsync())!;
    }

    public async Task AddAssignmentAsync(AssignmentDto assignment)
    {
        using StringContent jsonContent = new(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(BaseUrl, jsonContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAssignmentAsync(AssignmentDto assignment)
    {
        using StringContent jsonContent = new(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BaseUrl}/{assignment.Id}", jsonContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAssignmentAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<AssignmentDto>> SearchAssignmentsAsync(string searchTerm)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/search?term={searchTerm}");
        return JsonConvert.DeserializeObject<List<AssignmentDto>>(await response.Content.ReadAsStringAsync())!;
    }

    public async Task<List<AssignmentDto>> GetAssignmentsByProjectIdAsync(int projectId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/project/{projectId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AssignmentDto>>(content) ?? new List<AssignmentDto>();
        }
        else
        {
            return new List<AssignmentDto>();
        }
    }

}