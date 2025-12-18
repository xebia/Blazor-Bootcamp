using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWebApiDemo.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register MuscleCarApiService as a Typed HttpClient.
// Typed clients receive a dedicated HttpClient instance with pre-configured settings.
// The base address is set here, so the service only needs relative paths.
// Benefits: encapsulation, testability, and centralized configuration.
builder.Services.AddHttpClient<MuscleCarApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7163/");
});

await builder.Build().RunAsync();
