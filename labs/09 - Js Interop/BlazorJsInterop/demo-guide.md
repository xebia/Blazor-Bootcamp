# Modern JavaScript Interop Demo

This project demonstrates advanced JavaScript interop techniques in Blazor WebAssembly, including source-generated interop using `[JSImport]` and `[JSExport]` attributes.

## Running the Demo

1. Open `BlazorJsInterop.sln` in Visual Studio
2. Set **BlazorJsInterop** (the server project) as the startup project
3. Press F5 to run
4. Navigate to the different pages to see various interop techniques

## Demo Pages

| Page | Route | Demonstrates |
|------|-------|--------------|
| Counter | `/counter` | Traditional `IJSRuntime` interop with module imports |
| Modern Chart | `/modernchart` | Source-generated `[JSImport]` interop |

## Understanding the Two Interop Approaches

### Traditional Approach: `IJSRuntime`

Located in `Counter.razor`, this approach uses dependency injection and async calls:

```csharp
@inject IJSRuntime JSRuntime

await JSRuntime.InvokeVoidAsync("createChart", "myChart");
```

**Characteristics:**
- Works with both Server and WebAssembly render modes
- Uses reflection at runtime to marshal data between .NET and JavaScript
- All calls are asynchronous
- Universal and flexible

### Modern Approach: `[JSImport]` Source Generation

Located in `ChartInterop.cs` and `ModernChart.razor`, this approach uses compile-time code generation:

```csharp
[JSImport("createChart", "ChartModule")]
public static partial void CreateChart(string canvasId);
```

**Characteristics:**
- **WebAssembly only** - does not work with Server render mode
- Generates optimized marshalling code at compile time
- Calls can be synchronous (no async overhead)
- Better performance for frequent JS calls
- Requires `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` in the project file

## How Source-Generated Interop Works

### 1. Define the C# Interface (`ChartInterop.cs`)

```csharp
[SupportedOSPlatform("browser")]
public static partial class ChartInterop
{
    [JSImport("createChart", "ChartModule")]
    public static partial void CreateChart(string canvasId);
}
```

- The class must be `static partial`
- `[JSImport("functionName", "moduleName")]` maps to a JavaScript export
- The `partial` method has no body - the compiler generates it

### 2. Create the JavaScript Module (`wwwroot/ChartModule.js`)

```javascript
export function createChart(canvasId) {
    // JavaScript implementation
}
```

- Must use ES module syntax (`export function`)
- Module name in `[JSImport]` matches the name used when importing

### 3. Import the Module at Runtime (`ModernChart.razor`)

```csharp
await JSHost.ImportAsync("ChartModule", "../ChartModule.js");
```

- `JSHost.ImportAsync` loads the module and registers it by name
- Must be called before any `[JSImport]` methods from that module
- The path is relative to the page location

### 4. Call the Generated Method

```csharp
ChartInterop.CreateChart("modernChart");
```

- No `await` needed - the call is synchronous
- Type marshalling is handled by generated code

## Project Structure

```
BlazorJsInterop.Client/
├── ChartInterop.cs          # [JSImport] definitions
├── Pages/
│   ├── Counter.razor        # Traditional IJSRuntime demo
│   └── ModernChart.razor    # Source-generated interop demo
└── wwwroot/
    └── ChartModule.js       # ES module for [JSImport]

BlazorJsInterop/
└── wwwroot/
    ├── AlertUser.js         # Traditional JS for Counter page
    ├── CallDotNet.js        # JS-to-.NET callback demo
    └── DisplayAlert.js      # Global function demo
```

## When to Use Each Approach

| Scenario | Recommended Approach |
|----------|---------------------|
| Server-side Blazor | `IJSRuntime` (only option) |
| Occasional JS calls | `IJSRuntime` (simpler setup) |
| Performance-critical WebAssembly | `[JSImport]` (faster) |
| Calling global JS functions | Either works |
| Complex object marshalling | `IJSRuntime` (more flexible) |

## Additional Resources

- [Blazor JavaScript Interop Documentation](https://learn.microsoft.com/aspnet/core/blazor/javascript-interoperability/)
- [Source-generated JS interop](https://learn.microsoft.com/aspnet/core/blazor/javascript-interoperability/import-export-interop)
- [Chart.js Documentation](https://www.chartjs.org/docs/latest/)
