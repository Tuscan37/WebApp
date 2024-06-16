using System.Text;
using Newtonsoft.Json;
using WebApp.Shared.Dto;

namespace WebApp.Client.Services;

public class AssignmentService(HttpClient httpClient)
{
    private const string BaseUrl = "/api/Assignments";

    public async Task<List<AssignmentDto>> GetAssignmentsAsync()
    {
        var response = await httpClient.GetAsync(BaseUrl);
        return JsonConvert.DeserializeObject<List<AssignmentDto>>(await response.Content.ReadAsStringAsync())!;
    }
    public async Task<AssignmentDto> GetAssignmentByIdAsync(int id)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/{id.ToString()}");
        return JsonConvert.DeserializeObject<AssignmentDto>(await response.Content.ReadAsStringAsync())!;
    }
    public async Task AddAssignment(AssignmentDto asgn)
    {
        using StringContent jsonContent = new(JsonConvert.SerializeObject(asgn), Encoding.UTF8, "application/json");
        var responseMessage = await httpClient.PostAsync(BaseUrl, jsonContent);
    }
    public async Task<List<AssignmentDto>> SearchAssignmentsAsync(string searchTerm)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/search?term={searchTerm}");
        return JsonConvert.DeserializeObject<List<AssignmentDto>>(await response.Content.ReadAsStringAsync())!;
    }

}