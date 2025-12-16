# Render Modes

In this lab, you'll learn about Blazor's render modes and how to use the built-in `RendererInfo` API to detect the current rendering context. You'll also explore the `[PersistentState]` attribute for maintaining state across render mode transitions.

## Understanding Render Modes

Blazor supports several render modes:

| Render Mode | Description |
|-------------|-------------|
| **Static SSR** | Component is rendered on the server as static HTML. No interactivity. |
| **Streaming SSR** | Component streams content to the browser as it becomes available. No interactivity. |
| **Interactive Server** | Component runs on the server with UI updates sent via SignalR. |
| **Interactive WebAssembly** | Component runs entirely in the browser using WebAssembly. |
| **Interactive Auto** | Starts with Server, then switches to WebAssembly once the runtime is downloaded. |

## Opening the Solution

1. Open the `Blazor10RenderModes` solution from the lab folder
2. Notice the solution has two projects:
   - **Blazor10RenderModes** - The server project containing server-rendered components
   - **Blazor10RenderModes.Client** - The client project containing WebAssembly-capable components

## Exploring RendererInfo

Blazor provides a built-in `RendererInfo` object that gives you information about the current rendering context. This is available in any component without any setup.

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `RendererInfo.Name` | `string` | The name of the current renderer ("Static", "Server", "WebAssembly") |
| `RendererInfo.IsInteractive` | `bool` | Whether the component is currently interactive |

### Exercise 1: View RendererInfo on the Home Page

1. Open `Components/Pages/Home.razor` in the server project
2. Notice the existing code that displays the renderer info:

```razor
<p class="border border-info">@RendererInfo.Name, IsInteractive: @RendererInfo.IsInteractive</p>
```

3. Run the application (`F5` or `dotnet run`)
4. Navigate to the Home page
5. **Observe**: The page shows `Static, IsInteractive: False` - this is Static Server-Side Rendering (SSR)

### Exercise 2: Explore the Counter Page with InteractiveAuto

1. Open `Pages/Counter.razor` in the **Client** project
2. Note the render mode directive at the top:

```razor
@rendermode InteractiveAuto
```

3. Notice how the button visibility is controlled by checking `RendererInfo.IsInteractive`:

```razor
@if (RendererInfo.IsInteractive)
{
    <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
}
```

4. Run the application and navigate to the Counter page
5. **Watch carefully** as the page loads:
   - First, you'll see `Static, IsInteractive: False` (no button visible)
   - Then it transitions to `Server, IsInteractive: True` (button appears)
   - On subsequent visits (after WebAssembly downloads), you may see `WebAssembly, IsInteractive: True`

6. **Why this matters**: The button is hidden during static rendering because `@onclick` handlers don't work in static mode. Showing a non-functional button would confuse users.

### Exercise 3: Explore Streaming SSR with the Weather Page

1. Open `Components/Pages/Weather.razor` in the server project
2. Notice the `[StreamRendering]` attribute:

```razor
@attribute [StreamRendering]
```

3. Run the application and navigate to the Weather page
4. **Observe**: 
   - You'll briefly see "Loading..." as the data is being fetched
   - The page streams in the data when it's ready
   - The RendererInfo shows `Static, IsInteractive: False`

5. This demonstrates **streaming SSR** - the page sends initial HTML immediately, then streams updates as async operations complete.

## Understanding PersistentState

When a component transitions from server-side prerendering to interactive mode, `OnInitializedAsync` runs twice:
1. Once during server prerendering
2. Once when the interactive runtime starts

This can cause issues like:
- Duplicate data fetches
- Flickering UI
- Inconsistent state

The `[PersistentState]` attribute solves this by automatically persisting and restoring state across these transitions.

### Exercise 4: Explore the Weather Interactive Page

1. Open `Pages/WeatherInteractive.razor` in the **Client** project
2. Examine the code structure:

```razor
@page "/weatherinteractive"
@rendermode InteractiveServer

<PageTitle>Weather</PageTitle>

<h1>Weather Interactive</h1>

<p class="border border-info">@RendererInfo.Name, IsInteractive: @RendererInfo.IsInteractive</p>
```

3. Notice the `[PersistentState]` attribute on the forecasts property:

```csharp
[PersistentState]
public WeatherForecast[]? forecasts { get; set; }
```

4. Examine how `OnInitializedAsync` uses this for optimization:

```csharp
protected override async Task OnInitializedAsync()
{
    if (RendererInfo.IsInteractive && forecasts != null)
    {
        // Data is already loaded in persistent state
        return;
    }

    // Simulate asynchronous loading
    await Task.Delay(500);
    
    // ... generate forecasts
}
```

5. Run the application and navigate to Weather Interactive
6. **Observe**: The data loads once during prerendering, and when the component becomes interactive, the state is automatically restored - no second data fetch!

### Exercise 5: See PersistentState in Action

1. Modify the `WeatherInteractive.razor` to add logging:

```csharp
protected override async Task OnInitializedAsync()
{
    Console.WriteLine($"OnInitializedAsync called. IsInteractive: {RendererInfo.IsInteractive}, HasData: {forecasts != null}");
    
    if (RendererInfo.IsInteractive && forecasts != null)
    {
        Console.WriteLine("Using persisted state - skipping data fetch");
        return;
    }

    Console.WriteLine("Fetching new data...");
    // ... rest of the method
}
```

2. Run the application with the terminal visible
3. Navigate to Weather Interactive
4. Check the console output - you should see:
   - First call with `IsInteractive: False` (prerendering)
   - Second call with `IsInteractive: True` and `HasData: True` (interactive, using persisted state)

### Exercise 6: Compare With and Without PersistentState

1. Create a new page in the Client project called `WeatherNoPersist.razor`:

```razor
@page "/weathernopersist"
@rendermode InteractiveServer

<PageTitle>Weather No Persist</PageTitle>

<h1>Weather No Persist</h1>

<p class="border border-info">@RendererInfo.Name, IsInteractive: @RendererInfo.IsInteractive</p>

<p>This page does NOT use [PersistentState], so data is fetched twice.</p>

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    // Note: NO [PersistentState] attribute here!
    private WeatherForecast[]? forecasts;

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine($"[NoPersist] OnInitializedAsync called. IsInteractive: {RendererInfo.IsInteractive}");

        // Data is always fetched, even on second render
        await Task.Delay(500);

        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToArray();
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
```

2. Add navigation in `Components/Layout/NavMenu.razor`:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="weathernopersist">
        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Weather No Persist
    </NavLink>
</div>
```

3. Run the application and compare the two pages:
   - **Weather Interactive**: Data loads once, no flicker on transition
   - **Weather No Persist**: Data loads twice, you may see different random data!

4. Check the console output to see the difference in behavior

### Exercise 7: Loading Data Only in Interactive Mode

Sometimes you may want to skip data loading entirely during SSR prerendering and only load data once the component is interactive. This is useful when:
- The data is user-specific and requires authentication that's not available during SSR
- You want to avoid the overhead of loading data that will be discarded
- The data source is only accessible from the client

1. Open `Pages/WeatherInteractiveOnly.razor` in the **Client** project
2. Examine how it checks `RendererInfo.IsInteractive` before loading data:

```csharp
protected override async Task OnInitializedAsync()
{
    // Only load data when interactive - skip during SSR prerendering
    if (!RendererInfo.IsInteractive)
    {
        return;
    }

    // Load data only in interactive mode
    await Task.Delay(500);
    // ... generate forecasts
}
```

3. Notice how the UI shows "Loading..." during both SSR and until data loads:

```razor
@if (!RendererInfo.IsInteractive || forecasts == null)
{
    <p><em>Loading...</em></p>
}
```

4. Run the application and navigate to Weather Interactive Only
5. **Observe**:
   - During SSR prerendering, you see "Loading..." with `Static, IsInteractive: False`
   - When it becomes interactive, you still see "Loading..." briefly with `Server, IsInteractive: True`
   - Once data loads, the table appears

6. Check the console output - you should see:
   - First call during SSR skips data loading
   - Second call in interactive mode actually loads the data

### Comparing the Three Approaches

| Approach | Loads During SSR | Loads in Interactive | Data Consistency | Best For |
|----------|------------------|---------------------|------------------|----------|
| **PersistentState** | Yes | No (uses persisted) | Same data | Most scenarios - best UX |
| **No Persist** | Yes | Yes (reloads) | May differ | When you need fresh data |
| **Interactive Only** | No | Yes | N/A | Auth-required or client-only data |

## Summary

In this lab, you learned:

1. **RendererInfo** - A built-in API to detect the current render mode:
   - `RendererInfo.Name` tells you the renderer type ("Static", "Server", "WebAssembly")
   - `RendererInfo.IsInteractive` tells you if the component can handle user interactions

2. **Render Modes** - Different ways components can render:
   - Static SSR for fast initial loads
   - Streaming SSR for async content
   - Interactive Server for real-time updates via SignalR
   - Interactive WebAssembly for client-side execution
   - Interactive Auto for the best of both worlds

3. **PersistentState** - An attribute that:
   - Automatically persists component state during prerendering
   - Restores state when the interactive runtime starts
   - Prevents duplicate data fetches and UI flicker

## Bonus Challenges

1. Add a timestamp to the WeatherInteractive page that shows when data was fetched. Verify it only updates once.

2. Create a Counter page variant that persists the count value across render mode transitions.

3. Experiment with `@rendermode InteractiveWebAssembly` vs `@rendermode InteractiveServer` and observe the differences in `RendererInfo.Name`.
