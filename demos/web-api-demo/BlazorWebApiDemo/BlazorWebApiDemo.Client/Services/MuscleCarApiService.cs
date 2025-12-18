using BlazorWebApiDemo.Client.Models;
using System.Net.Http.Json;

namespace BlazorWebApiDemo.Client.Services;

/// <summary>
/// Typed HttpClient service for communicating with the MuscleCar Minimal API.
/// Using a typed client provides strong typing, encapsulation of API logic,
/// and allows for centralized configuration of the HttpClient (base address, headers, etc.)
/// </summary>
public class MuscleCarApiService
{
    private readonly HttpClient _httpClient;

    public MuscleCarApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MuscleCar>> GetAllAsync()
    {
        try
        {
            var cars = await _httpClient.GetFromJsonAsync<List<MuscleCar>>("minimal/musclecars");
            return cars ?? new List<MuscleCar>();
        }
        catch (Exception)
        {
            return new List<MuscleCar>();
        }
    }

    /// <summary>
    /// Gets a muscle car by ID. Returns a tuple with the car and any error message.
    /// Demonstrates consuming ProblemDetails (RFC 7807) error responses from the API.
    /// </summary>
    public async Task<(MuscleCar? Car, string? Error)> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"minimal/musclecars/{id}");

            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<MuscleCar>(), null);

            // Parse the ProblemDetails response for a user-friendly error message
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return (null, problem?.Detail ?? "An unknown error occurred.");
        }
        catch (Exception)
        {
            return (null, "Failed to connect to the API.");
        }
    }

    public async Task<MuscleCar?> CreateAsync(MuscleCar muscleCar)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("minimal/musclecars", muscleCar);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MuscleCar>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<MuscleCar?> UpdateAsync(int id, MuscleCar muscleCar)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"minimal/musclecars/{id}", muscleCar);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MuscleCar>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"minimal/musclecars/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
