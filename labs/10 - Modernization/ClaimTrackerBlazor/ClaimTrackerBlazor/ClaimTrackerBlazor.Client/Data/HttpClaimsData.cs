using System.Net.Http.Json;
using ClaimTrackerBlazor.Contracts;

namespace ClaimTrackerBlazor.Client.Data;

/// <summary>
/// HTTP client implementation of IClaimsData for WebAssembly.
/// 
/// TODO: Implement all methods using HttpClient to call the API.
/// 
/// This class is used when the Blazor app runs in WebAssembly mode.
/// It communicates with the server via HTTP API calls to the ClaimsController.
/// 
/// Key concepts:
/// - WebAssembly runs in the browser and cannot access the database directly
/// - It must call server-side APIs to perform data operations
/// - Uses System.Net.Http.Json for JSON serialization/deserialization
/// 
/// API Endpoints (defined in ClaimsController):
/// - GET    api/claims        - Get all claims
/// - GET    api/claims/{id}   - Get single claim
/// - PUT    api/claims        - Add new claim (returns new ID)
/// - POST   api/claims        - Update existing claim
/// - DELETE api/claims/{id}   - Delete claim
/// </summary>
public class HttpClaimsData(HttpClient httpClient) : IClaimsData
{
    /// <summary>
    /// Gets all claims from the API.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use httpClient.GetFromJsonAsync<T>() to GET and deserialize
    /// - Handle null response by returning empty array
    /// </summary>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("GetClaimsAsync not yet implemented");
    }

    /// <summary>
    /// Gets a single claim by ID from the API.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use httpClient.GetFromJsonAsync<T>() with the ID in the URL
    /// </summary>
    public Task<Claim?> GetClaimAsync(int id)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("GetClaimAsync not yet implemented");
    }

    /// <summary>
    /// Adds a new claim via the API.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use httpClient.PutAsJsonAsync() to send the claim
    /// - Read the response to get the new ID
    /// - Use response.EnsureSuccessStatusCode() to throw on errors
    /// </summary>
    public async Task<int> AddClaimAsync(Claim claim)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("AddClaimAsync not yet implemented");
    }

    /// <summary>
    /// Updates an existing claim via the API.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use httpClient.PostAsJsonAsync() to send the updated claim
    /// </summary>
    public Task UpdateClaimAsync(Claim claim)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("UpdateClaimAsync not yet implemented");
    }

    /// <summary>
    /// Deletes a claim via the API.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use httpClient.DeleteAsync() with the ID in the URL
    /// </summary>
    public Task DeleteClaimAsync(int id)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("DeleteClaimAsync not yet implemented");
    }
}
