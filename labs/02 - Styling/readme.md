# Styling Blazor Apps

## Creating the Solution

1. Open Visual Studio
2. Click on Create a new project
3. Select Blazor Web App
4. Click Next
5. Enter the project name: `BlazorHolStyling`
6. Click Next
7. Use the following options:
   - Framework: .NET 10
   - Authentication Type: None
   - Configure for HTTPS: Checked
   - Interactive render mode: Auto (Server and WebAssembly)
   - Interactivity location: Per page/component
   - Include sample pages: Checked
8. Click Create

## Setting Global CSS

1. Open the `wwwroot` folder in the server project
2. Open the `app.css` file
3. Edit the `.btn-primary` class to have the following styles:

```css
.btn-primary {
    color: #929292;
    background-color: #26b050;
    border-color: #1861ac;
}

.btn-primary:hover {
    color: #808080;
    background-color: #00ff21;
    border-color: #285e8e;
}

.btn-primary:focus {
    color: #929292;
    background-color: #26b050;
    border-color: #000000;
}
```

4. Press F5 to run the app
5. Navigate to the Counter page
6. The button should now have the styles defined in the `app.css` file
7. Close the browser

> **Note:** Hot Reload in Visual Studio can be used to see CSS changes without restarting. Clear your browser cache or use a private window if styles don't update.

## Setting Per-Component CSS

1. Open the `Counter.razor` file in the `Pages` folder
2. Right-click on the `Pages` folder in Solution Explorer and select Add -> New Item
3. Add a file named `Counter.razor.css`
4. Add the following CSS to the `Counter.razor.css` file:

```css
h1 {
    color: brown;
    font-family: Tahoma, Geneva, Verdana, sans-serif;
}

button {
    background-color: brown;
    color: white;
    font-family: Tahoma, Geneva, Verdana, sans-serif;
}
```

5. Rebuild the solution (Build > Rebuild Solution)
6. Press F5 to run the app
7. Navigate to the Counter page
8. The `h1` and `button` elements should now have the styles defined in the `Counter.razor.css` file
9. Close the browser
10. Open the `App.razor` file in the server project `Components` folder
11. Notice the consolidated css link:

```html
    <link rel="stylesheet" href="BlazorHolStyling.styles.css" />
```

This is the consolidated CSS file that contains all the per-component CSS files in the project.

## Customizing Bootstrap

To customize Bootstrap you need to recompile Bootstrap to integrate your changes. You can use the `DartSassBuilder` NuGet package to compile the SCSS files in the project. DartSassBuilder requires no configuration within the solution and executes automatically on build.

> **Note:** DartSassBuilder requires .NET 8 SDK to be installed. Download and install it from the [.NET 8 Dowloads Page](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

1. Add a reference to the `DartSassBuilder` NuGet package in the server and client projects.

> **Note:** DartSassBuilder must be installed for each project that will have .scss files, whether they are independent or associated with components.

### Optional: Configure DartSassBuilder Output Style

You can configure DartSassBuilder to output expanded (readable) CSS in Debug mode and compressed (minified) CSS in Release mode.

1. Right-click on the server project name in Solution Explorer and select **Edit Project File**
2. Add the following code inside the `<PropertyGroup>` tag (before the closing `</PropertyGroup>`):

```xml
<!--DartSass output style option-->
<DartSassOutputStyle>compressed</DartSassOutputStyle>
<!--Debug = expanded, Release = minified-->
<DartSassOutputStyle Condition="'$(Configuration)' == 'Debug'">expanded</DartSassOutputStyle>
```

3. Repeat steps 1-2 for the client project
4. Save both project files

This configuration will generate expanded CSS files in Debug mode for easier reading and compressed CSS in Release mode for production.

---

2. Create a new `scss` folder in the server project.
3. Download Bootstrap's *Source Files* from the [official website](https://getbootstrap.com/docs/5.3/getting-started/download/) and extract the `scss` folder from the zip archive to the `scss` folder in your server project.
![scss folder](scss-folder.png)
4. Add a new file named `custom.scss` to the `scss` folder.
![custom file](custom-file.png)
5. Add the following code to the `custom.scss` file:

```scss
@import url('../lib/open-iconic/font/css/open-iconic-bootstrap.min.css');

/* Custom variable overrides - MUST be before Bootstrap import */
$primary: red;
$secondary: #70af6d;
$link-color: $primary;

/* Import Bootstrap */
@import "bootstrap/scss/bootstrap.scss";

/* Custom styles - AFTER Bootstrap import so Bootstrap variables like $gray-100 are available */
.btn-primary, .btn-primary:active, .btn-primary:focus, .btn-primary:checked {
    background-color: $primary;
    border-color: $gray-100;
    color: white;
}
```

These override defaults in Bootstrap. For example, the `$primary` variable is set to red, and the `.btn-primary` class is customized to have a red background color.

6. Open the `BlazorHolStyling.csproj` file in the server project
7. Add the following code to the file before the closing `</Project>` tag:

```xml
  <Target Name="CopyCssBundles" AfterTargets="AfterBuild">
    <ItemGroup>
      <MyCssBundles Include="scss\custom.css" />
    </ItemGroup>
    <Copy SourceFiles="@(MyCssBundles)" DestinationFiles="wwwroot\%(Filename)%(Extension)" OverwriteReadOnlyFiles="true" />
  </Target>
```

This copies the compiled CSS file to the `wwwroot` folder after the build process.

8. Open the `App.razor` file in the `Components` folder
9. Comment out the `app.css` link and add a link to the `custom.css` file:

```html
@*  <link rel="stylesheet" href="@Assets["app.css"]" /> *@
    <link rel="stylesheet" href="@Assets["custom.css"]" />
```

10. Build the solution
11. Notice the `custom.css` file in the `wwwroot` folder
12. Open the `custom.css` file to see the compiled CSS
13. Press F5 to run the app
14. Navigate to the Counter page
15. The button should now have the styles defined in the `custom.css` file

⚠️ **Note:** Browser caching can cause styles to not appear immediately. You may need to clear your browser cache or use a private browsing window to see the changes.

## Using SCSS for Per-Component Styling

Now that DartSassBuilder is installed, you can use SCSS features for per-component styling, including variables, nesting, and mixins.

1. In the `BlazorHolStyling.Client` project, navigate to the `Pages` folder
2. Delete the `Counter.razor.css` file (it will regenerate automatically)
3. Right-click on the `Pages` folder and select Add -> New Item
4. Add a file named `Counter.razor.scss`
5. Add the following SCSS code to the `Counter.razor.scss` file:

```scss
/* SCSS Variables */
$counter-heading-color: #4a4a4a;
$counter-button-bg: #0066cc;
$counter-button-hover-bg: #0052a3;
$counter-status-color: #2d862d;

h1 {
    color: $counter-heading-color;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

button {
    background-color: $counter-button-bg;
    color: white;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    
    &:hover {
        background-color: $counter-button-hover-bg;
    }
}

.counter-status {
    color: $counter-status-color;
    font-weight: bold;
    font-size: 1.2rem;
}
```

6. Open the `Counter.razor` file in the `Pages` folder
7. Add the `counter-status` class to the `<p>` element:

```razor
<p role="status" class="counter-status">Current count: @currentCount</p>
```

8. Rebuild the solution (Build > Rebuild Solution)
9. Notice that `Counter.razor.css` has been automatically generated in the `Pages` folder
10. Open the generated `Counter.razor.css` file to see the compiled CSS from your SCSS
11. Press F5 to run the app
12. Navigate to the Counter page
13. The counter should now display with the SCSS-defined styles, including the green bold status text
