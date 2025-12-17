# Calling Web APIs

## Creating the Solution

1. Open Visual Studio
2. Click on Create a new project
3. Select Blazor Web App
4. Click Next
5. Enter the project name: `BlazorHolCallingApis`
6. Click Next
7. Use the following options:
   - Framework: .NET 10
   - Authentication Type: None
   - Configure for HTTPS: Checked
   - Interactive render mode: Auto (Server and WebAssembly)
   - Interactivity location: Per page/component
   - Include sample pages: Checked
8. Click Create
9. Run the AppServer project from Visual Studio
10. Make note of the port number: `https://localhost:????`

## Adding an AppServer project

1. Right-click on the solution
2. Click on Add > New Project
3. Select ASP.NET Core Web API
4. Click Next
5. Enter the project name: `AppServer`
6. Click Next
7. Use the following options:
   - Target Framework: .NET 10
   - Authentication Type: None
   - Configure for HTTPS: Checked
   - Use controllers: Checked
8. Click Create

## Examine the WeatherForecast class

1. Open the `WeatherForecast.cs` file in the `AppServer` project
2. Notice the class has the same properties as the `WeatherForecast` class in the Blazor project
3. This class will be used to return data from the Web API

## Examine the WeatherForecastController

1. Open the `WeatherForecastController.cs` file in the `AppServer` project
2. Notice the `Get` method returns an array of `WeatherForecast` objects
3. This method will be used to return data from the Web API

## Configure Startup Projects

1. Right-click on the solution
2. Click on Startup Projects
3. Select Multiple startup projects
4. Set the Action for the `BlazorHolCallingApis` project to Start
5. Set the Action for the `AppServer` project to Start
6. Click OK

![Setting startup projects](startup-projects.png)

## Using the API in the Blazor project

1. Open the `Weather.razor` file in the `BlazorHolCallingApis` project
2. Change the `Weather.razor` page to use the Web API

```csharp
    protected override async Task OnInitializedAsync()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "MyBearerTokenValue");
        forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/weatherforecast");
    }
```

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

3. Run the application
4. Navigate to the `Weather` page
5. You will see the weather forecast data loaded from the Web API

## Using the API from a WebAssembly Page

1. Add a new `ClientWeather.razor` file to the `Pages` folder in the _client_ project
2. Copy the contents of the `Weather.razor` file to the new file
3. Change the `ClientWeather.razor` page to use interactive WebAssembly rendering

```html
@rendermode InteractiveWebAssembly
```

4. Change the title of the page to `Client Weather`

```html
<PageTitle>Client Weather</PageTitle>

<h3>Client Weather</h3>
```

5. Add the new page to the navigation menu by editing the `NavMenu.razor` file in the server project

```html
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="clientweather">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Client Weather
            </NavLink>
        </div>
```

6. Run the application
7. Navigate to the `Client Weather` page
8. Notice that the data is not loaded!

The data can't be loaded from a client app because of cross-origin resource sharing (CORS) restrictions. The Web API must be configured to allow requests from the client app.

## Configuring CORS

1. Open the `Program.cs` file in the `AppServer` project
2. Add the following code to enable CORS

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
```

3. Add the following code to use the CORS policy

```csharp
app.UseCors("AllowAllOrigins");
```

4. Run the application
5. Navigate to the `Client Weather` page
6. You will see the weather forecast data loaded from the Web API

If you watch closely, you'll see that the data is loaded twice! This is because the page is rendered twice - once on the server (server static rendering) and once on the client (WebAssembly interactive rendering).

## Fixing the Double Load of Data

There are two approaches to solve this problem. Here's the call count breakdown:

| Approach | Prerender | Interactive | Total |
|----------|-----------|-------------|-------|
| No fix (original problem) | 1 call | 1 call | **2 calls** |
| **RendererInfo.IsInteractive** | 0 calls | 1 call | **1 call** |
| **PersistentComponentState** | 1 call | 0 calls (reads from HTML) | **1 call** |

Both fixes result in 1 call - it's just a question of **when** that call happens.

| Approach | How it works | User Experience |
|----------|--------------|-----------------|
| **RendererInfo.IsInteractive** | Skips fetch during prerender, only fetches when interactive | User sees "Loading..." briefly, then data appears |
| **PersistentComponentState** | Fetches during prerender, serializes data to HTML, reuses when interactive | User sees **real data** immediately |

### 💡 When to Use Each Approach

For a **WebAssembly** page, `RendererInfo.IsInteractive` is the **better** approach because:

1. **Simpler code** - no subscription, no IDisposable, no Persist callback
2. **The spinner is good UX** - WebAssembly takes time to download/boot anyway, so users expect a brief load
3. **More realistic pattern** - most real apps use loading states
4. **Data freshness** - data is fetched when the user can actually interact with it

`PersistentComponentState` is better for:
- **SEO scenarios** where crawlers need data in the initial HTML
- **InteractiveServer** mode where there's no WebAssembly download delay
- Complex multi-component state coordination

### Approach 1: RendererInfo.IsInteractive (Recommended)

This approach is the simplest and most common pattern for WebAssembly pages.

1. Open the `ClientWeather.razor` file in the client project
2. Update the `OnInitializedAsync` method to check if the component is interactive:

```csharp
protected override async Task OnInitializedAsync()
{
    // Only fetch data when running interactively to avoid duplicate calls
    // during the static prerender phase. The prerender will show "Loading..."
    // and data loads once WebAssembly is ready.
    if (RendererInfo.IsInteractive)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "MyBearerTokenValue");
        forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/weatherforecast");
    }
}
```

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

3. Run the application
4. Navigate to the `Client Weather` page
5. You will see "Loading..." briefly while WebAssembly boots, then the data appears (fetched only once!)

### Approach 2: PersistentComponentState (Advanced Alternative)

Use this approach when you need data visible in the initial HTML (for SEO or instant display).

1. Open the `ClientWeather.razor` file in the client project
2. Add the `IDisposable` interface and inject `PersistentComponentState`:

```html
@page "/clientweather"
@using System.Text.Json
@using System.Net.Http.Headers
@rendermode InteractiveWebAssembly
@implements IDisposable

@inject PersistentComponentState ApplicationState
```

3. Add a field for the subscription in the `@code` block:

```csharp
private PersistingComponentStateSubscription _subscription;
```

4. Update the `OnInitializedAsync` method to register for persisting and check for existing state:

```csharp
protected override async Task OnInitializedAsync()
{
    // Register a callback to persist state when the prerender completes.
    // This serializes the data into the HTML response.
    _subscription = ApplicationState.RegisterOnPersisting(Persist);

    // Try to retrieve data that was persisted during prerender.
    // If found, we reuse it instead of making another HTTP call.
    var foundInState = ApplicationState
        .TryTakeFromJson<WeatherForecast[]>("forecasts", out forecasts);

    if (!foundInState)
    {
        // No persisted data found - fetch from the API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "MyBearerTokenValue");
        forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/weatherforecast");
    }
}
```

5. Add the `Persist` method to serialize state during prerender:

```csharp
private Task Persist()
{
    ApplicationState.PersistAsJson("forecasts", forecasts);
    return Task.CompletedTask;
}
```

6. Add the `Dispose` method to clean up the subscription:

```csharp
public void Dispose()
{
    _subscription.Dispose();
}
```

7. Run the application
8. Navigate to the `Client Weather` page
9. You will see the weather forecast data loaded only once, and the data appears immediately!

> **Note:** Hot Reload in Visual Studio can be used to see changes without restarting. Only restart if cached or stale code persists.

## Securing the API

1. Add a `BearerAuthnHandler` class to the `AppServer` project

```csharp
using Microsoft.AspNetCore.Authorization;

namespace AppServer;

public class BearerAuthnHandler(IHttpContextAccessor HttpContextAccessor) : AuthorizationHandler<BearerAuthnRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BearerAuthnRequirement requirement)
    {
        if (HttpContextAccessor is null)
        {
            throw new ArgumentNullException(nameof(HttpContextAccessor));
        }
        var token = HttpContextAccessor.HttpContext?.Request.Headers.Authorization;
        if (string.IsNullOrWhiteSpace(token) || token.Value != "Bearer MyBearerTokenValue")
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
```

This is just an example, and uses a hardcoded token. In a real-world scenario, you would use a more secure method to validate the token.

2. Add a `BearerAuthnRequirement` class to the `AppServer` project

```csharp
public class BearerAuthnRequirement : IAuthorizationRequirement
{
}
```

3. Register the handler in the `Program.cs` file in the `AppServer` project

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthorizationHandler, BearerAuthnHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BearerAuthn", policy =>
    {
        policy.Requirements.Add(new BearerAuthnRequirement());
    });
});
```

4. Use the policy for all controllers in the `Program.cs` file in the `AppServer` project

```csharp
app.MapControllers().RequireAuthorization("BearerAuthn");
```

5. Supply the token in the `ClientWeather.razor` file in the client project

```csharp
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "MyBearerTokenValue");
    forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/weatherforecast");
```

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

6. Supply the token in the `Weather.razor` file in the server project

```csharp
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "MyBearerTokenValue");
    forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/weatherforecast");
```

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

7. Run the application
8. Notice that the browser that should show the weather forecast data is not showing the data because it is not authorized
9. Navigate to the `Weather` page
10. You will see the weather forecast data loaded from the Web API
11. Navigate to the `Client Weather` page
12. You will see the weather forecast data loaded from the Web API

## References

https://khalidabuhakmeh.com/customize-the-authorization-pipeline-in-aspnet-core

## Adding a Minimal API Endpoint

Now that you've built a traditional controller-based API, let's add a Minimal API endpoint to see the modern approach side-by-side. Minimal APIs are lightweight, expressive, and perfect for simple endpoints.

### Creating the WeatherEndpoints Class

Rather than putting all endpoint definitions directly in `Program.cs`, we'll use a separate file with extension methods for better organization.

1. Right-click on the `AppServer` project
2. Click on Add > Class
3. Name the file: `WeatherEndpoints.cs`
4. Click Add
5. Replace the contents with the following code:

```csharp
namespace AppServer;

public static class WeatherEndpoints
{
    private static readonly string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static void MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        // Create a route group for all weather-related endpoints
        var weatherGroup = app.MapGroup("/api/weather")
            .RequireAuthorization("BearerAuthn")
            .WithTags("Weather");

        // GET /api/weather
        weatherGroup.MapGet("/", GetWeatherForecast)
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        // GET /api/weather/summary
        weatherGroup.MapGet("/summary", GetSummaries)
            .WithName("GetWeatherSummaries")
            .WithOpenApi();
    }

    private static WeatherForecast[] GetWeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        })
        .ToArray();
    }

    private static IResult GetSummaries()
    {
        return Results.Ok(summaries);
    }
}
```

This demonstrates several Minimal API best practices:
- **Extension method pattern** - keeps `Program.cs` clean
- **Route grouping** - organizes related endpoints with shared configuration
- **Authorization** - applies the same `BearerAuthn` policy we used for controllers
- **OpenAPI integration** - endpoints appear in Swagger documentation
- **Separate handler methods** - keeps the mapping code readable

### Registering the Endpoints

1. Open the `Program.cs` file in the `AppServer` project
2. Find the line `app.MapControllers().RequireAuthorization("BearerAuthn");`
3. Add the following line immediately after it:

```csharp
app.MapWeatherEndpoints();
```

Your endpoint registration should now look like this:

```csharp
app.MapControllers().RequireAuthorization("BearerAuthn");
app.MapWeatherEndpoints();
```

### Testing the Minimal API Endpoint

1. Run the application
2. Navigate to the Swagger UI for the `AppServer` project at `https://localhost:7285/swagger`

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

3. You should see both:
   - The original `WeatherForecast` controller endpoint at `/weatherforecast`
   - The new Minimal API endpoints at `/api/weather` and `/api/weather/summary`

### Calling the Minimal API from Blazor

Let's update the `ClientWeather.razor` page to call the new Minimal API endpoint instead of the controller.

1. Open the `ClientWeather.razor` file in the client project
2. Find the line with `GetFromJsonAsync<WeatherForecast[]>`
3. Change the URL from `/weatherforecast` to `/api/weather`:

```csharp
forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7285/api/weather");
```

> ⚠️ Change the port from `7285` to the port of _your_ AppServer project

4. Run the application
5. Navigate to the `Client Weather` page
6. You will see the weather forecast data loaded from the Minimal API endpoint

### Comparing Controller vs Minimal API

Now you have both approaches running side-by-side:

| Approach | Route | File Location | Best For |
|----------|-------|---------------|----------|
| **Controller** | `/weatherforecast` | `WeatherForecastController.cs` | Complex CRUD operations, traditional RESTful APIs |
| **Minimal API** | `/api/weather` | `WeatherEndpoints.cs` | Simple endpoints, microservices, modern projects |

**Key Differences:**

- **Controllers**: More structure, built-in model binding, attribute-based routing
- **Minimal APIs**: Less ceremony, functional style, easier to test, better performance

**When to use Minimal APIs:**
- New projects targeting .NET 6+
- Microservices or serverless functions
- Simple CRUD operations
- When you want less boilerplate code

**When to use Controllers:**
- Large existing codebases
- Complex validation scenarios
- When your team is more familiar with MVC patterns
- When you need controller-level filters or conventions

### Understanding API Security

Let's verify that the security is working correctly:

1. Make sure both projects are still running
2. Try to navigate directly to `https://localhost:7285/api/weather/summary` in your browser (using your AppServer port)
3. You should see a **401 Unauthorized** error

This is **expected behavior**! The endpoint is protected by the `BearerAuthn` authorization policy we applied to the entire route group. Without the bearer token, the request is rejected.

**Why this matters:**
- Your API is properly secured - unauthorized users cannot access the data
- The same security policy applies to both the controller endpoints and minimal API endpoints
- In Swagger, you can test with the bearer token by clicking "Authorize" and entering `Bearer MyBearerTokenValue`
- In a real application, you would use tools like Postman or REST Client extensions where you can properly configure authorization headers with valid JWT tokens
