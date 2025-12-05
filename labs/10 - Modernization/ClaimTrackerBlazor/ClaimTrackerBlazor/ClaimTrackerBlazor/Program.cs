using ClaimTrackerBlazor.Client.Pages;
using ClaimTrackerBlazor.Components;
using ClaimTrackerBlazor.Contracts;
using ClaimTrackerBlazor.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);

var connectionString = builder.Configuration.GetConnectionString("ClaimsDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\ClaimsTracker.mdf;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
}

connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);

builder.Services.AddDbContext<ClaimsDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IClaimsData, EfClaimsData>();
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

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

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClaimTrackerBlazor.Client._Imports).Assembly);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
    await ClaimsDbInitializer.InitializeAsync(dbContext);
}

app.Run();
