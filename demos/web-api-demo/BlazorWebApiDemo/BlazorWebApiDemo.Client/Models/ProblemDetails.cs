namespace BlazorWebApiDemo.Client.Models;

/// <summary>
/// Represents an RFC 7807 ProblemDetails response from the API.
/// This is a client-side class that matches the JSON structure returned by 
/// ASP.NET Core's Results.Problem() method for standardized error responses.
/// </summary>
public class ProblemDetails
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
}
