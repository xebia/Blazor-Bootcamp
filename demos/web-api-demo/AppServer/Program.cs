using AppServer.Endpoints;
using AppServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MuscleCarService>();

builder.Services.AddControllers();

// Add CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7263", "http://localhost:5201")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AppServer API");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthorization();

app.MapControllers();

// Map minimal API endpoints
app.MapMuscleCarEndpoints();

app.Run();
