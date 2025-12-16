using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("                    AOT Compilation Demo");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// Check if running as AOT
var isAot = !System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported;
PrintKv("Running as Native AOT", isAot ? "Yes ✓" : "No (JIT)");
PrintKv("Process ID", Environment.ProcessId.ToString());
Console.WriteLine();

// Demo 1: Startup time benefit
PrintHeader("1. Startup Performance");
var startupTime = Process.GetCurrentProcess().StartTime;
var now = DateTime.Now;
PrintKv("Process started at", startupTime.ToString("HH:mm:ss.fff"));
PrintKv("Current time", now.ToString("HH:mm:ss.fff"));
PrintNote("AOT apps start faster because there's no JIT compilation at startup.");
Console.WriteLine();

// Demo 2: Memory footprint
PrintHeader("2. Memory Footprint");
var process = Process.GetCurrentProcess();
PrintKv("Working Set", $"{process.WorkingSet64 / 1024 / 1024} MB");
PrintKv("Private Memory", $"{process.PrivateMemorySize64 / 1024 / 1024} MB");
PrintNote("AOT apps typically use less memory than JIT apps.");
Console.WriteLine();

// Demo 3: AOT-compatible JSON serialization
PrintHeader("3. AOT-Compatible JSON Serialization");
PrintNote("Traditional reflection-based JSON doesn't work with AOT.");
PrintNote("Use source generators instead:");
Console.WriteLine();
PrintCode("[JsonSerializable(typeof(WeatherForecast))]");
PrintCode("internal partial class AppJsonContext : JsonSerializerContext { }");
Console.WriteLine();

var forecast = new WeatherForecast
{
    Date = DateOnly.FromDateTime(DateTime.Now),
    TemperatureC = 25,
    Summary = "Warm and sunny"
};

// AOT-compatible serialization using source-generated context
var json = JsonSerializer.Serialize(forecast, AppJsonContext.Default.WeatherForecast);
PrintKv("Serialized", json);

var deserialized = JsonSerializer.Deserialize(json, AppJsonContext.Default.WeatherForecast);
PrintKv("Deserialized", $"{deserialized?.Summary} - {deserialized?.TemperatureC}°C");
Console.WriteLine();

// Demo 4: Working with collections
PrintHeader("4. Serializing Collections");
var forecasts = new List<WeatherForecast>
{
    new() { Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Cool" },
    new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" },
    new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), TemperatureC = 30, Summary = "Hot" }
};

var listJson = JsonSerializer.Serialize(forecasts, AppJsonContext.Default.ListWeatherForecast);
PrintKv("List JSON", listJson);
Console.WriteLine();

// Demo 5: What to avoid
PrintHeader("5. Patterns to AVOID in AOT");
PrintError("✗ Activator.CreateInstance(typeName)");
PrintError("✗ Assembly.LoadFrom / LoadFile");
PrintError("✗ Type.GetType(string)");
PrintError("✗ Expression.Compile()");
PrintError("✗ Reflection.Emit");
Console.WriteLine();

PrintHeader("6. AOT-Friendly Alternatives");
PrintSuccess("✓ Use generics instead of reflection");
PrintSuccess("✓ Use source generators (like System.Text.Json)");
PrintSuccess("✓ Use [DynamicallyAccessedMembers] annotations");
PrintSuccess("✓ Use compile-time code generation");
Console.WriteLine();

// Demo 6: Publishing instructions
PrintHeader("7. How to Publish as AOT");
Console.WriteLine();
PrintCode("# In your .csproj:");
PrintCode("<PublishAot>true</PublishAot>");
Console.WriteLine();
PrintCode("# Publish command:");
PrintCode("dotnet publish -c Release");
Console.WriteLine();
PrintNote("The output will be a single native executable.");
PrintNote("No .NET runtime required on the target machine!");
Console.WriteLine();

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("                         Demo Complete");
Console.WriteLine("═══════════════════════════════════════════════════════════════");

// Helper methods
void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"── {title} ──");
    Console.ResetColor();
}

void PrintKv(string key, string value)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write($"{key}: ");
    Console.ResetColor();
    Console.WriteLine(value);
}

void PrintNote(string text)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(text);
    Console.ResetColor();
}

void PrintCode(string code)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  {code}");
    Console.ResetColor();
}

void PrintSuccess(string text)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  {text}");
    Console.ResetColor();
}

void PrintError(string text)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  {text}");
    Console.ResetColor();
}

// AOT-compatible JSON serialization context
[JsonSerializable(typeof(WeatherForecast))]
[JsonSerializable(typeof(List<WeatherForecast>))]
internal partial class AppJsonContext : JsonSerializerContext
{
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
