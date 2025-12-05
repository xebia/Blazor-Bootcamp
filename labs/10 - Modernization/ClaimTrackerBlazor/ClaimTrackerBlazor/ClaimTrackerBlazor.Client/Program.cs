using ClaimTrackerBlazor.Client.Data;
using ClaimTrackerBlazor.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IClaimsData, HttpClaimsData>();

await builder.Build().RunAsync();
