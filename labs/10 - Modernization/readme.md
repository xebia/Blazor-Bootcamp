# Lab 10: Modernization - Migrating WPF to Blazor

**Duration:** 90 minutes

## Overview

In this lab, you will migrate key components from a legacy WPF desktop application to a modern Blazor web application. This exercise simulates a real-world modernization scenario where you need to bring an existing line-of-business application to the web.

### What You'll Learn

- Converting XAML UI to Razor components
- Migrating code-behind event handlers to Blazor `@code` blocks
- Replacing ADO.NET data access with Entity Framework Core
- Understanding the differences between WPF and Blazor patterns
- Working with Blazor's InteractiveAuto render mode

### The Applications

**ClaimTracker (WPF)** - A legacy insurance claims management application:
- Built with WPF (.NET Framework style patterns)
- Uses ADO.NET with raw SQL for data access
- LocalDB database for storage
- Traditional code-behind pattern with event handlers

**ClaimTrackerBlazor (Blazor)** - The modernized web application:
- Blazor Web App with InteractiveAuto render mode
- Entity Framework Core for data access
- Clean architecture with separate projects
- Dependency injection throughout

### Project Structure

The Blazor solution has been set up with four projects:

```
ClaimTrackerBlazor/
├── ClaimTrackerBlazor/              # Server project (hosts the app)
│   ├── Controllers/                  # API controllers
│   ├── Components/                   # Server-side Razor components
│   └── Program.cs                    # App startup & DI configuration
├── ClaimTrackerBlazor.Client/        # Client project (WebAssembly)
│   ├── Pages/                        # Interactive pages (Claims, AddEditClaim)
│   └── Data/                         # HTTP client data access
├── ClaimTrackerBlazor.Contracts/     # Shared contracts
│   ├── Claim.cs                      # Domain model
│   └── IClaimsData.cs               # Data access interface
└── ClaimTrackerBlazor.DataAccess/    # EF Core data access
    ├── ClaimsDbContext.cs            # EF Core DbContext
    ├── ClaimsDbInitializer.cs        # Database seeding
    └── EfClaimsData.cs              # IClaimsData implementation
```

---

## Part 1: Explore the WPF Application (10 minutes)

Before migrating, let's understand the existing application.

### Step 1.1: Run the WPF Application

1. Open the `ClaimTracker` solution in Visual Studio
2. Build and run the application
3. Explore the functionality:
   - View the list of claims
   - Add a new claim
   - Edit an existing claim
   - Delete a claim

### Step 1.2: Examine the Code Structure

Review these key files in the WPF project:

| WPF File | Purpose | Blazor Equivalent |
|----------|---------|-------------------|
| `Windows/ClaimsListPage.xaml` | Claims list UI | `Pages/Claims.razor` |
| `Windows/ClaimsListPage.xaml.cs` | List event handlers | `@code` block in Claims.razor |
| `Windows/AddEditClaimWindow.xaml` | Add/Edit form UI | `Pages/AddEditClaim.razor` |
| `Windows/AddEditClaimWindow.xaml.cs` | Form logic & validation | `@code` block in AddEditClaim.razor |
| `Models/Claim.cs` | Data model | `Contracts/Claim.cs` |
| Inline SQL in code-behind | Data access | `DataAccess/EfClaimsData.cs` |

### Key Observations

Notice these WPF patterns that we'll modernize:

1. **Data Binding:** WPF uses `{Binding PropertyName}` - Blazor uses `@property` or `@bind-Value`
2. **Event Handlers:** WPF uses `Click="Handler_Click"` - Blazor uses `@onclick="Handler"`
3. **Navigation:** WPF opens modal windows - Blazor navigates to routes
4. **Data Access:** WPF has SQL inline in code-behind - Blazor uses DI and repositories

---

## Part 2: Implement the Data Access Layer (15 minutes)

Let's start with the foundation - the data access layer.

### Step 2.1: Open the Blazor Solution

1. Open the `ClaimTrackerBlazor` solution in a **new** Visual Studio instance
2. Review the existing structure - notice the `IClaimsData` interface is already defined

### Step 2.2: Implement EfClaimsData

Open `ClaimTrackerBlazor.DataAccess/EfClaimsData.cs`. This file contains TODO comments.

**Replace the stub implementations with the following:**

#### GetClaimsAsync

```csharp
public async Task<IEnumerable<Claim>> GetClaimsAsync()
{
    return await _context.Claims
        .AsNoTracking()
        .OrderByDescending(c => c.DateFiled)
        .ToListAsync();
}
```

Compare this to the WPF version in `ClaimsListPage.xaml.cs`:
```csharp
// WPF Version - ADO.NET
string query = @"SELECT ... FROM Claims ORDER BY DateFiled DESC";
SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
adapter.Fill(claimsTable);
```

#### GetClaimAsync

```csharp
public Task<Claim?> GetClaimAsync(int id)
{
    return _context.Claims
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.ClaimId == id);
}
```

#### AddClaimAsync

```csharp
public async Task<int> AddClaimAsync(Claim claim)
{
    _context.Claims.Add(claim);
    await _context.SaveChangesAsync();
    return claim.ClaimId;
}
```

#### UpdateClaimAsync

```csharp
public async Task UpdateClaimAsync(Claim claim)
{
    _context.Claims.Update(claim);
    await _context.SaveChangesAsync();
}
```

#### DeleteClaimAsync

```csharp
public async Task DeleteClaimAsync(int id)
{
    var entity = await _context.Claims.FindAsync(id);
    if (entity == null)
    {
        return;
    }

    _context.Claims.Remove(entity);
    await _context.SaveChangesAsync();
}
```

### Step 2.3: Implement HttpClaimsData

Open `ClaimTrackerBlazor.Client/Data/HttpClaimsData.cs`. This is used when the app runs in WebAssembly mode.

**Replace the stub implementations:**

```csharp
public async Task<IEnumerable<Claim>> GetClaimsAsync()
{
    var result = await httpClient.GetFromJsonAsync<IEnumerable<Claim>>("api/claims");
    return result ?? Array.Empty<Claim>();
}

public Task<Claim?> GetClaimAsync(int id)
{
    return httpClient.GetFromJsonAsync<Claim>($"api/claims/{id}");
}

public async Task<int> AddClaimAsync(Claim claim)
{
    var response = await httpClient.PutAsJsonAsync("api/claims", claim);
    response.EnsureSuccessStatusCode();
    return (await response.Content.ReadFromJsonAsync<int?>()) ?? 0;
}

public Task UpdateClaimAsync(Claim claim)
{
    return httpClient.PostAsJsonAsync("api/claims", claim);
}

public Task DeleteClaimAsync(int id)
{
    return httpClient.DeleteAsync($"api/claims/{id}");
}
```

> **Why two implementations?** With `InteractiveAuto` render mode, the app starts on the server and transitions to WebAssembly. On the server, `EfClaimsData` accesses the database directly. In WebAssembly, `HttpClaimsData` calls the API because browsers can't connect to databases.

---

## Part 3: Implement the Claims List Page (25 minutes)

Now let's build the main claims listing page.

### Step 3.1: Understand the WPF Version

Review `ClaimTracker/Windows/ClaimsListPage.xaml`:
- DataGrid with columns for claim properties
- "Add New Claim" button in header
- Edit and Delete buttons per row

### Step 3.2: Create the Blazor Claims Page

Open `ClaimTrackerBlazor.Client/Pages/Claims.razor` and replace the content with the following.

#### Page Header and Directives

```razor
@page "/claims"
@rendermode InteractiveAuto
@inject IClaimsData ClaimsData
@inject NavigationManager Navigation

<PageTitle>Claims Management</PageTitle>
```

**Key differences from WPF:**
- `@page "/claims"` defines the route (WPF uses code-behind navigation)
- `@inject` provides dependency injection (WPF typically uses `new` or ServiceLocator)
- `@rendermode InteractiveAuto` enables interactivity in both Server and WebAssembly modes

#### Page Header Section

```razor
<div class="page-header">
    <div class="container-fluid">
        <div class="row align-items-center">
            <div class="col">
                <h1 class="page-title">Claims Management</h1>
            </div>
            <div class="col-auto">
                <button class="btn btn-primary" @onclick="AddNewClaim">
                    <i class="bi bi-plus-circle"></i> Add New Claim
                </button>
            </div>
        </div>
    </div>
</div>
```

**Key difference:** `@onclick="AddNewClaim"` vs WPF's `Click="AddNewClaim_Click"`

#### Content Area with Conditional Rendering

```razor
<div class="content-area">
    @if (claims == null)
    {
        <div class="text-center p-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p class="mt-3">Loading claims...</p>
        </div>
    }
    else if (!claims.Any())
    {
        <div class="alert alert-info m-4">
            <i class="bi bi-info-circle"></i> No claims found. Click "Add New Claim" to create your first claim.
        </div>
    }
    else
    {
        <div class="table-responsive m-4">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>Claim Number</th>
                        <th>Policy Holder</th>
                        <th>Date Filed</th>
                        <th>Amount</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var claim in claims)
                    {
                        <tr>
                            <td>@claim.ClaimNumber</td>
                            <td>@claim.PolicyHolderName</td>
                            <td>@claim.DateFiled.ToString("MM/dd/yyyy")</td>
                            <td>@claim.ClaimAmount.ToString("C")</td>
                            <td>
                                <span class="badge @GetStatusBadgeClass(claim.Status)">
                                    @claim.Status
                                </span>
                            </td>
                            <td>
                                <button class="btn btn-sm btn-edit-custom me-2" 
                                        @onclick="() => EditClaim(claim.ClaimId)"
                                        title="Edit">
                                    <i class="bi bi-pencil"></i> Edit
                                </button>
                                <button class="btn btn-sm btn-outline-danger" 
                                        @onclick="() => ShowDeleteConfirmation(claim)"
                                        title="Delete">
                                    <i class="bi bi-trash"></i> Delete
                                </button>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    }
</div>
```

**Key differences:**
- `@if`/`else` for conditional rendering (WPF uses Visibility and converters)
- `@foreach` to iterate (WPF uses ItemsSource binding)
- `@claim.PropertyName` for data binding (WPF uses `{Binding PropertyName}`)
- Lambda expressions in `@onclick` to pass parameters

#### Delete Confirmation Dialog

```razor
@if (showDeleteDialog)
{
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Delete</h5>
                    <button type="button" class="btn-close" @onclick="CancelDelete"></button>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to delete this claim?</p>
                    @if (claimToDelete != null)
                    {
                        <div class="alert alert-warning">
                            <strong>Claim Number:</strong> @claimToDelete.ClaimNumber<br />
                            <strong>Policy Holder:</strong> @claimToDelete.PolicyHolderName
                        </div>
                    }
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="CancelDelete">Cancel</button>
                    <button type="button" class="btn btn-danger" @onclick="ConfirmDelete">Delete</button>
                </div>
            </div>
        </div>
    </div>
}
```

**Key difference:** In WPF, you'd use `MessageBox.Show()` - in Blazor, we render a modal as part of the component.

#### Code Block

```razor
@code {
    private IEnumerable<Claim>? claims;
    private bool showDeleteDialog = false;
    private Claim? claimToDelete = null;

    protected override async Task OnInitializedAsync()
    {
        // Only load data when in interactive mode to avoid double-loading
        // during server static -> server interactive -> wasm interactive transitions
        if (!RendererInfo.IsInteractive)
        {
            return;
        }

        await LoadClaims();
    }

    private async Task LoadClaims()
    {
        claims = await ClaimsData.GetClaimsAsync();
    }

    private void AddNewClaim()
    {
        Navigation.NavigateTo("/claims/add");
    }

    private void EditClaim(int claimId)
    {
        Navigation.NavigateTo($"/claims/edit/{claimId}");
    }

    private void ShowDeleteConfirmation(Claim claim)
    {
        claimToDelete = claim;
        showDeleteDialog = true;
    }

    private void CancelDelete()
    {
        claimToDelete = null;
        showDeleteDialog = false;
    }

    private async Task ConfirmDelete()
    {
        if (claimToDelete != null)
        {
            await ClaimsData.DeleteClaimAsync(claimToDelete.ClaimId);
            showDeleteDialog = false;
            claimToDelete = null;
            await LoadClaims();
        }
    }

    private string GetStatusBadgeClass(string status)
    {
        return status.ToLower() switch
        {
            "approved" => "bg-success",
            "pending" => "bg-warning text-dark",
            "denied" => "bg-danger",
            "processing" => "bg-info",
            _ => "bg-secondary"
        };
    }
}
```

**Key differences:**
- `OnInitializedAsync` replaces constructor initialization
- `NavigationManager.NavigateTo()` replaces opening new windows
- State changes automatically trigger UI updates (no need for `INotifyPropertyChanged`)

---

## Part 4: Implement the Add/Edit Claim Page (30 minutes)

Now let's build the form for adding and editing claims.

### Step 4.1: Understand the WPF Version

Review `ClaimTracker/Windows/AddEditClaimWindow.xaml`:
- Form with TextBox, DatePicker, ComboBox controls
- Manual validation in code-behind
- Separate constructor for Add vs Edit mode

### Step 4.2: Create the Blazor Add/Edit Page

Open `ClaimTrackerBlazor.Client/Pages/AddEditClaim.razor` and replace the content.

#### Page Directives with Multiple Routes

```razor
@page "/claims/add"
@page "/claims/edit/{ClaimId:int}"
@rendermode InteractiveAuto
@inject IClaimsData ClaimsData
@inject NavigationManager Navigation

<PageTitle>@pageTitle</PageTitle>
```

**Key concept:** Multiple `@page` directives allow the same component to handle both add and edit scenarios. The `{ClaimId:int}` is a route parameter with type constraint.

#### Page Header

```razor
<div class="page-header">
    <div class="container-fluid">
        <div class="row align-items-center">
            <div class="col">
                <h1 class="page-title">@pageTitle</h1>
            </div>
        </div>
    </div>
</div>
```

#### Form Content

```razor
<div class="content-area">
    @if (isLoading)
    {
        <div class="text-center p-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p class="mt-3">Loading claim...</p>
        </div>
    }
    else
    {
        <div class="content-surface mx-4">
            <EditForm Model="@claim" OnValidSubmit="@HandleValidSubmit">
                <DataAnnotationsValidator />

                <!-- Claim Number -->
                <div class="mb-3">
                    <label for="claimNumber" class="form-label fw-bold">
                        Claim Number <span class="text-danger">*</span>
                    </label>
                    <InputText id="claimNumber" 
                              class="form-control" 
                              @bind-Value="claim.ClaimNumber"
                              maxlength="50" />
                    <ValidationMessage For="@(() => claim.ClaimNumber)" />
                </div>

                <!-- Policy Holder Name -->
                <div class="mb-3">
                    <label for="policyHolderName" class="form-label fw-bold">
                        Policy Holder Name <span class="text-danger">*</span>
                    </label>
                    <InputText id="policyHolderName" 
                              class="form-control" 
                              @bind-Value="claim.PolicyHolderName"
                              maxlength="100" />
                    <ValidationMessage For="@(() => claim.PolicyHolderName)" />
                </div>

                <!-- Date of Incident -->
                <div class="mb-3">
                    <label for="dateOfIncident" class="form-label fw-bold">
                        Date of Incident <span class="text-danger">*</span>
                    </label>
                    <InputDate id="dateOfIncident" 
                              class="form-control" 
                              @bind-Value="claim.DateOfIncident" />
                    <small class="form-text text-muted">Must be a date in the past</small>
                    <ValidationMessage For="@(() => claim.DateOfIncident)" />
                    @if (showDateValidationError)
                    {
                        <div class="validation-message">Date of Incident must be in the past (not today or future).</div>
                    }
                </div>

                <!-- Date Filed -->
                <div class="mb-3">
                    <label for="dateFiled" class="form-label fw-bold">
                        Date Filed <span class="text-danger">*</span>
                    </label>
                    <InputDate id="dateFiled" 
                              class="form-control" 
                              @bind-Value="claim.DateFiled" />
                    <ValidationMessage For="@(() => claim.DateFiled)" />
                </div>

                <!-- Claim Amount -->
                <div class="mb-3">
                    <label for="claimAmount" class="form-label fw-bold">
                        Claim Amount <span class="text-danger">*</span>
                    </label>
                    <InputNumber id="claimAmount" 
                                class="form-control" 
                                @bind-Value="claim.ClaimAmount"
                                step="0.01" />
                    <small class="form-text text-muted">Must be greater than 0</small>
                    <ValidationMessage For="@(() => claim.ClaimAmount)" />
                </div>

                <!-- Status -->
                <div class="mb-3">
                    <label for="status" class="form-label fw-bold">
                        Status <span class="text-danger">*</span>
                    </label>
                    <InputSelect id="status" 
                                class="form-control" 
                                @bind-Value="claim.Status">
                        <option value="">-- Select Status --</option>
                        <option value="Pending">Pending</option>
                        <option value="Processing">Processing</option>
                        <option value="Approved">Approved</option>
                        <option value="Denied">Denied</option>
                    </InputSelect>
                    <ValidationMessage For="@(() => claim.Status)" />
                </div>

                <!-- Description -->
                <div class="mb-3">
                    <label for="description" class="form-label fw-bold">
                        Description (Optional)
                    </label>
                    <InputTextArea id="description" 
                                  class="form-control" 
                                  @bind-Value="claim.Description"
                                  rows="5"
                                  maxlength="500" />
                    <small class="form-text text-muted">Maximum 500 characters</small>
                    <ValidationMessage For="@(() => claim.Description)" />
                </div>

                <!-- Footer Buttons -->
                <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top">
                    <button type="button" class="btn btn-secondary" @onclick="Cancel">
                        Cancel
                    </button>
                    <button type="submit" class="btn btn-primary" disabled="@(!isFormValid)">
                        @(isEditMode ? "Update Claim" : "Save Claim")
                    </button>
                </div>
            </EditForm>
        </div>
    }
</div>
```

**Key differences from WPF:**
- `EditForm` with `DataAnnotationsValidator` replaces manual validation
- `InputText`, `InputDate`, `InputNumber`, `InputSelect` are Blazor's form components
- `@bind-Value` provides two-way binding (similar to WPF's `{Binding}` with Mode=TwoWay)
- `ValidationMessage` displays validation errors automatically

#### Cancel Confirmation Dialog

```razor
@if (showCancelDialog)
{
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Cancel</h5>
                    <button type="button" class="btn-close" @onclick="CancelDialogNo"></button>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to cancel? Any unsaved changes will be lost.</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="CancelDialogNo">No</button>
                    <button type="button" class="btn btn-danger" @onclick="CancelDialogYes">Yes</button>
                </div>
            </div>
        </div>
    </div>
}
```

#### Code Block

```razor
@code {
    [Parameter]
    public int? ClaimId { get; set; }

    private Claim claim = new();
    private bool isEditMode => ClaimId.HasValue;
    private bool isLoading = false;
    private bool showCancelDialog = false;
    private bool showDateValidationError = false;
    private bool isFormValid => ValidateForm();
    private string pageTitle => isEditMode ? "Edit Claim" : "Add New Claim";

    protected override async Task OnInitializedAsync()
    {
        // Only load data when in interactive mode to avoid double-loading
        // during server static -> server interactive -> wasm interactive transitions
        if (!RendererInfo.IsInteractive)
        {
            isLoading = true;
            return;
        }

        if (isEditMode)
        {
            isLoading = true;
            var existingClaim = await ClaimsData.GetClaimAsync(ClaimId!.Value);
            if (existingClaim != null)
            {
                claim = existingClaim;
            }
            else
            {
                // Claim not found, redirect to claims list
                Navigation.NavigateTo("/claims");
            }
            isLoading = false;
        }
        else
        {
            // Set default values for new claim
            claim.DateFiled = DateTime.Today;
            claim.Status = "Pending";
        }
    }

    private bool ValidateForm()
    {
        // Check required fields
        if (string.IsNullOrWhiteSpace(claim.ClaimNumber) ||
            string.IsNullOrWhiteSpace(claim.PolicyHolderName) ||
            string.IsNullOrWhiteSpace(claim.Status) ||
            claim.ClaimAmount <= 0)
        {
            return false;
        }

        // Check date validation
        if (claim.DateOfIncident >= DateTime.Today)
        {
            showDateValidationError = true;
            return false;
        }
        
        showDateValidationError = false;
        return true;
    }

    private async Task HandleValidSubmit()
    {
        // Additional validation for DateOfIncident
        if (claim.DateOfIncident >= DateTime.Today)
        {
            showDateValidationError = true;
            return;
        }

        showDateValidationError = false;

        try
        {
            if (isEditMode)
            {
                await ClaimsData.UpdateClaimAsync(claim);
            }
            else
            {
                await ClaimsData.AddClaimAsync(claim);
            }

            // Navigate back to claims list
            Navigation.NavigateTo("/claims");
        }
        catch (Exception ex)
        {
            // In a production app, you'd want to display this error to the user
            Console.Error.WriteLine($"Error saving claim: {ex.Message}");
        }
    }

    private void Cancel()
    {
        showCancelDialog = true;
    }

    private void CancelDialogNo()
    {
        showCancelDialog = false;
    }

    private void CancelDialogYes()
    {
        showCancelDialog = false;
        Navigation.NavigateTo("/claims");
    }
}
```

**Key differences from WPF:**
- `[Parameter]` attribute receives route parameters (WPF uses constructor overloads)
- `OnInitializedAsync` handles both add and edit mode initialization
- Form validation combines DataAnnotations (on the model) with custom logic

---

## Part 5: Test the Application (10 minutes)

### Step 5.1: Build and Run

1. Build the solution (Ctrl+Shift+B)
2. Set `ClaimTrackerBlazor` (the server project) as the startup project
3. Press F5 to run

### Step 5.2: Test All Features

1. **Home Page:** Click "View All Claims" to navigate to the claims list
2. **Claims List:** Verify the sample claims are displayed
3. **Add Claim:** Click "Add New Claim" and fill out the form
   - Test validation by leaving fields empty
   - Test the "Date of Incident must be in the past" validation
4. **Edit Claim:** Click "Edit" on an existing claim and modify it
5. **Delete Claim:** Click "Delete" and confirm the deletion
6. **Status Badges:** Verify different statuses show different colors

### Step 5.3: Observe Render Mode Transitions

Open browser developer tools (F12) and watch the console:
1. Initial load uses Server rendering
2. After WebAssembly loads, the app transitions to client-side rendering
3. The `RendererInfo.IsInteractive` check prevents double-loading

---

## Summary

### What You Accomplished

✅ Migrated ADO.NET data access to Entity Framework Core  
✅ Converted WPF XAML to Blazor Razor components  
✅ Replaced code-behind event handlers with `@code` blocks  
✅ Implemented modern form validation with EditForm  
✅ Built a responsive web UI with Bootstrap  
✅ Created a clean architecture with separation of concerns  

### Key Patterns Comparison

| Pattern | WPF | Blazor |
|---------|-----|--------|
| UI Markup | XAML | Razor |
| Event Handling | `Click="Handler"` | `@onclick="Handler"` |
| Data Binding | `{Binding Property}` | `@property` or `@bind-Value` |
| Conditional UI | Visibility + Converters | `@if`/`else` |
| Lists | ItemsSource | `@foreach` |
| Navigation | Window.Show() | NavigationManager |
| Dialogs | MessageBox / Modal Windows | Inline modal components |
| Data Access | ADO.NET inline | EF Core + DI |
| Validation | Code-behind validation | DataAnnotations + EditForm |

### Next Steps

To further modernize this application, consider:
- Adding authentication with ASP.NET Core Identity
- Implementing real-time updates with SignalR
- Adding comprehensive unit tests with bUnit
- Containerizing with Docker
- Deploying to Azure App Service

