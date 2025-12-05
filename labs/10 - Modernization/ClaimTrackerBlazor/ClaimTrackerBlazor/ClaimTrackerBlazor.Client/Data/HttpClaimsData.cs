using System.Net.Http.Json;
using ClaimTrackerBlazor.Contracts;

namespace ClaimTrackerBlazor.Client.Data;

public class HttpClaimsData(HttpClient httpClient) : IClaimsData
{
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        var result = await httpClient.GetFromJsonAsync<IEnumerable<Claim>>("api/claims");
        return result ?? Array.Empty<Claim>();
    }

    public Task<Claim?> GetClaimAsync(int id)
    {
        return httpClient.GetFromJsonAsync<Claim>($"api/claims/{id}");
    }

    public async Task<int> AddClaimAsync(Claim claim)
    {
        var response = await httpClient.PutAsJsonAsync("api/claims", claim);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<int?>()) ?? 0;
    }

    public Task UpdateClaimAsync(Claim claim)
    {
        return httpClient.PostAsJsonAsync("api/claims", claim);
    }

    public Task DeleteClaimAsync(int id)
    {
        return httpClient.DeleteAsync($"api/claims/{id}");
    }
}
