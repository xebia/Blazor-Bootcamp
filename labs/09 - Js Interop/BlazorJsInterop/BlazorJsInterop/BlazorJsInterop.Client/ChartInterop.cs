// Modern JavaScript interop using source generation
// This approach provides better performance for WebAssembly by generating
// optimized marshalling code at compile time instead of runtime reflection

using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BlazorJsInterop.Client;

/// <summary>
/// Source-generated JavaScript interop for chart operations.
/// The [JSImport] attribute generates efficient interop code at compile time,
/// eliminating the overhead of reflection-based IJSRuntime calls.
/// </summary>
[SupportedOSPlatform("browser")]
public static partial class ChartInterop
{
    /// <summary>
    /// Creates a chart on the specified canvas element.
    /// The method signature matches the JavaScript function exported from the module.
    /// </summary>
    /// <param name="canvasId">The HTML canvas element ID</param>
    [JSImport("createChart", "ChartModule")]
    public static partial void CreateChart(string canvasId);

    /// <summary>
    /// Shows a browser alert - demonstrates simple interop with global functions
    /// </summary>
    /// <param name="message">Message to display</param>
    [JSImport("globalThis.alert")]
    public static partial void ShowAlert(string message);
}
