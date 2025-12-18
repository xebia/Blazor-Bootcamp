using BlazorWebApiDemo.Client.Services;
using BlazorWebApiDemo.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register MuscleCarApiService as a Typed HttpClient for server-side rendering.
// This is needed when using InteractiveAuto - the component may run on the server first,
// so the server also needs access to the API service with the same configuration.
builder.Services.AddHttpClient<MuscleCarApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7163/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebApiDemo.Client._Imports).Assembly);

app.Run();
