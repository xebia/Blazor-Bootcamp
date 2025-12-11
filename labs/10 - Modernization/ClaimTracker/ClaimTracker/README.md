# Claims Tracker - WPF to Blazor Migration Lab

## ?? Lab Overview

This is a **pre-built WPF application** that demonstrates common legacy enterprise application patterns. Your task is to migrate this application to Blazor while preserving functionality and improving the architecture.

**Time Allotted:** 90 minutes of hands-on migration work

---

## ?? What You're Given

A fully functional WPF desktop application for managing insurance claims with:

- ? Claims list with DataGrid
- ? Add new claims with validation
- ? Edit existing claims
- ? Delete claims with confirmation
- ? SQL Server LocalDB database
- ? Traditional code-behind patterns (no MVVM)
- ? Direct ADO.NET data access

---

## ?? Getting Started

### Prerequisites
- Visual Studio 2026 or later
- .NET Framework 4.8.1 SDK
- SQL Server LocalDB (included with Visual Studio)

### Running the WPF Application
1. Open `ClaimTracker.sln` in Visual Studio
2. Press **F5** to run
3. Database will auto-create on first run with sample data
4. Explore the application:
   - Click **Claims > View All Claims** to see the list
   - Try adding, editing, and deleting claims
   - Notice the validation rules

**Spend 10 minutes exploring and understanding the WPF code.**

---

## ?? Application Structure

```
ClaimTracker/
??? App.xaml.cs              # Application entry, DB initialization
??? MainWindow.xaml          # Menu navigation, page container
??? Pages/
?   ??? ClaimsListPage.xaml  # DataGrid with claims list
??? Windows/
?   ??? AddEditClaimWindow.xaml  # Modal dialog for Add/Edit
??? Models/
?   ??? Claim.cs             # Simple POCO entity
??? Data/
?   ??? DatabaseManager.cs   # DB creation and seeding
??? Styles/
    ??? AppStyles.xaml       # SF BLI brand colors/styles
```

---

## ?? Key Legacy Patterns to Identify

Look for these "old school" patterns in the WPF code:

### 1. **Code-Behind Data Binding**
```csharp
// In ClaimsListPage.xaml.cs
ClaimsDataGrid.ItemsSource = claimsTable.DefaultView;
```
No MVVM, no ViewModels - just direct property setting.

### 2. **Direct Database Access in UI Code**
```csharp
// In ClaimsListPage.xaml.cs
using (SqlConnection connection = new SqlConnection(connectionString))
{
    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
    adapter.Fill(claimsTable);
}
```
No repository pattern, no service layer.

### 3. **MessageBox for User Feedback**
```csharp
MessageBox.Show("Claim added successfully.", "Success", ...);
```
Simple but not flexible or testable.

### 4. **Modal Dialog Pattern**
```csharp
var dialog = new AddEditClaimWindow(claimId);
bool? result = dialog.ShowDialog();
if (result == true) { /* refresh */ }
```
Synchronous, blocks UI thread.

### 5. **Manual Validation**
```csharp
if (string.IsNullOrWhiteSpace(ClaimNumberTextBox.Text))
    errors.AppendLine("Claim Number is required.");
```
No data annotations, no validation framework.

### 6. **ResourceDictionary Styling**
```xaml
<Button Style="{StaticResource PrimaryButton}"/>
```
XAML-based theming, not CSS.

---

## ?? Database Schema

```sql
CREATE TABLE Claims (
    ClaimId INT PRIMARY KEY IDENTITY(1,1),
    ClaimNumber NVARCHAR(50) NOT NULL,
    PolicyHolderName NVARCHAR(100) NOT NULL,
    DateOfIncident DATETIME NOT NULL,
    DateFiled DATETIME NOT NULL,
    ClaimAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL,  -- Pending/Processing/Approved/Denied
    Description NVARCHAR(500) NULL
);
```

---

## ? Validation Rules (Must Be Preserved)

| Field | Validation |
|-------|------------|
| ClaimNumber | Required, max 50 chars |
| PolicyHolderName | Required, max 100 chars |
| DateOfIncident | Required, must be in past |
| DateFiled | Required |
| ClaimAmount | Required, must be > 0 |
| Status | Required (Pending/Processing/Approved/Denied) |
| Description | Optional, max 500 chars |

---

## ?? Migration Goals

### Core Objectives (Must Complete)
1. ? Create a Blazor Server or WebAssembly project
2. ? Migrate the Claims list page
3. ? Implement Add claim functionality
4. ? Implement Edit claim functionality
5. ? Implement Delete claim functionality
6. ? Preserve all validation rules
7. ? Connect to the same database (or equivalent)

### Architecture Improvements (Recommended)
- Separate data access into a service layer
- Use Entity Framework Core instead of ADO.NET
- Implement async/await patterns
- Use Blazor validation attributes
- Replace MessageBox with modal components
- Apply responsive CSS styling

### Stretch Goals (If Time Permits)
- Add search/filter functionality
- Implement pagination
- Add loading indicators
- Create reusable modal component
- Add unit tests
- Implement status workflow validation

---

## ??? Migration Roadmap (Suggested)

### Phase 1: Setup (5 minutes)
- [ ] Create new Blazor project
- [ ] Add EF Core NuGet packages
- [ ] Create DbContext and Claim entity
- [ ] Test database connection

### Phase 2: Claims List (25 minutes)
- [ ] Create ClaimsList Razor component
- [ ] Implement data service to load claims
- [ ] Display claims in a table
- [ ] Add Delete functionality with confirmation
- [ ] Test data loading and deletion

### Phase 3: Add/Edit Modal (35 minutes)
- [ ] Create AddEditClaim Razor component
- [ ] Implement form with all fields
- [ ] Add validation attributes or manual validation
- [ ] Wire up Save functionality
- [ ] Test Add and Edit workflows

### Phase 4: Integration & Polish (25 minutes)
- [ ] Add navigation menu
- [ ] Style with CSS (Bootstrap or custom)
- [ ] Test full CRUD cycle
- [ ] Handle errors gracefully
- [ ] Add loading states

---

## ?? WPF ? Blazor Pattern Mapping

| WPF Concept | Blazor Equivalent |
|-------------|-------------------|
| `Window` | Razor component or page |
| `UserControl` | Razor component |
| `DataGrid` | `<table>` or component library grid |
| `MessageBox.Show()` | Modal component or JS Interop |
| `ShowDialog()` | Modal component with state |
| Code-behind | `@code` block in .razor file |
| `x:Name="MyControl"` | `@ref="myControl"` |
| `Click="Handler"` | `@onclick="Handler"` |
| `ItemsSource` | `@foreach` or component binding |
| `StaticResource` | CSS class or inline style |
| ADO.NET | EF Core or API calls |
| Synchronous I/O | `async`/`await` |
| `ConfigurationManager` | `IConfiguration` + DI |

---

## ?? Tips & Hints

### Data Access
- Consider creating an `IClaimService` interface
- Implement async methods: `GetAllAsync()`, `AddAsync()`, etc.
- Use EF Core for cleaner data access code

### Validation
- Use Data Annotations on the `Claim` model
- Use `<EditForm>` and `<ValidationSummary>` in Blazor
- Consider `FluentValidation` for complex rules

### Modal Dialogs
- Use a Blazor modal component library (e.g., Blazored.Modal)
- Or create a custom modal using CSS and state management
- Pass claim data as component parameters

### Styling
- The WPF app uses SF BLI brand colors:
  - Primary Red: `#842231`
  - Primary Blue: `#435B6B`
- Apply these in your CSS to maintain brand consistency

### State Management
- Use `StateHasChanged()` to force UI refresh after data changes
- Consider `EventCallback` for component communication
- Services registered as Scoped will maintain state per user session

---

## ?? Troubleshooting

### Database Issues
- If LocalDB doesn't work, switch to SQL Server Express
- Update connection string in Blazor `appsettings.json`
- Ensure migrations are applied (if using EF Core)

### WPF App Won't Run
- Check that SQL Server LocalDB is installed
- Verify .NET Framework 4.8.1 is installed
- See `DATABASE_TROUBLESHOOTING.md` for detailed help

---

## ?? Resources

### Blazor Learning
- [Official Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Blazor University](https://blazor-university.com)

### Component Libraries
- [MudBlazor](https://mudblazor.com) - Material Design components
- [Radzen](https://blazor.radzen.com) - Rich component library
- [Blazored Modal](https://github.com/Blazored/Modal) - Modal dialogs

### EF Core
- [EF Core Documentation](https://docs.microsoft.com/ef/core)
- [EF Core with Blazor](https://docs.microsoft.com/aspnet/core/blazor/blazor-server-ef-core)

---

## ?? Success Criteria

Your Blazor application should:
- ? Display all claims in a list/table
- ? Allow adding new claims with validation
- ? Allow editing existing claims
- ? Allow deleting claims with confirmation
- ? Enforce all validation rules from the WPF app
- ? Work without errors
- ? Have a clean, professional UI

---

## ?? Submission (If Applicable)

If this is a graded lab, submit:
1. Your Blazor solution folder (zipped)
2. Screenshots of:
   - Claims list page
   - Add/Edit form
   - Validation errors
   - Successful CRUD operations
3. Brief written comparison of WPF vs Blazor patterns (1-2 pages)

---

## ? Questions?

During the lab, ask your instructor about:
- Recommended Blazor project structure
- Best practices for data access in Blazor
- Component communication patterns
- Handling async operations in Blazor
- Blazor vs WPF architectural differences

---

## ?? Final Notes

**Remember:** The goal isn't just to make it work - it's to understand the differences between desktop (WPF) and web (Blazor) application patterns. Think about:
- How state is managed differently
- How navigation works in web vs desktop
- How validation is applied
- How async operations improve UX
- How component-based UI differs from control-based UI

**Good luck with your migration!** ??

---

**Lab Version:** 1.0  
**Last Updated:** 2024  
**Estimated Time:** 90 minutes hands-on + 10 minutes review  
**Difficulty:** Intermediate
