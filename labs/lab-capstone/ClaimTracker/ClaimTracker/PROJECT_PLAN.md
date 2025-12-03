# Claims Tracker - WPF to Blazor Training App
## Project Plan & Specification

---

## 1. Overview

### Purpose
This is a **pre-built WPF application** that serves as the starting point for a hands-on training lab. Developers will be given this completed WPF application and tasked with migrating it to Blazor. The application demonstrates traditional WPF data binding patterns (non-MVVM) with SQL Server LocalDB integration - patterns commonly found in legacy enterprise applications.

### Time Constraint
The lab is designed for **90 minutes** of hands-on Blazor migration work. Participants will NOT build this WPF app - they will receive it as-is and migrate it.

### Target Audience
Developers familiar with C# who need hands-on experience:
- Analyzing legacy WPF codebases
- Identifying migration patterns and challenges
- Translating WPF concepts to Blazor equivalents
- Implementing CRUD operations in Blazor
- Working with modern data access patterns

---

## 2. Application Scope

### Lab Flow Overview
```
Day of Lab:
1. Participants receive completed WPF application
2. 10 min: Review WPF app, understand structure
3. 5 min: Plan Blazor architecture
4. 75 min: Implement Blazor migration
5. Final: Compare and discuss patterns

The WPF app is PRE-BUILT and provided as-is.
```

### Core Functionality
1. **Claims List Page** - Display all insurance claims in a grid/list
2. **Add/Edit Page** - Form to create new claims or edit existing ones
3. **Delete Confirmation** - Dialog to confirm deletion of claims
4. **Navigation** - Menu-based navigation following classic WPF patterns

### Intentional Limitations
- **No MVVM** - Uses code-behind and traditional data binding (typical of legacy apps)
- **Basic validation only** - Required fields, max lengths, and date rules
- **No authentication** - Single-user desktop app
- **No reporting** - Just basic CRUD
- **Minimal business logic** - Focus on data access patterns
- **Simple UI** - Easy to understand and replicate in Blazor within 90 minutes

---

## 3. Data Model

### Claim Entity
A simple insurance claim with the following properties:

| Property | Type | Description | Validation |
|----------|------|-------------|------------|
| ClaimId | int | Primary key (identity) | Auto-generated |
| ClaimNumber | string | User-friendly claim identifier (e.g., "CLM-2024-001") | Required, Max 50 chars |
| PolicyHolderName | string | Name of the insured person | Required, Max 100 chars |
| DateOfIncident | DateTime | When the incident occurred | Required, Must be in the past |
| DateFiled | DateTime | When the claim was filed | Required |
| ClaimAmount | decimal | Amount claimed | Required, Must be > 0 |
| Status | string | Current status (e.g., "Pending", "Approved", "Denied", "Processing") | Required, Max 20 chars |
| Description | string | Brief description of the claim | Optional, Max 500 chars |

### Database
- **SQL Server LocalDB** (.mdf file in App_Data folder)
- **Single Table**: `Claims`
- **Entity Framework 6.x** or **ADO.NET** (for traditional approach)
- Connection string stored in App.config

### Database Schema

```sql
CREATE TABLE Claims (
    ClaimId INT PRIMARY KEY IDENTITY(1,1),
    ClaimNumber NVARCHAR(50) NOT NULL,
    PolicyHolderName NVARCHAR(100) NOT NULL,
    DateOfIncident DATETIME NOT NULL,
    DateFiled DATETIME NOT NULL,
    ClaimAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Description NVARCHAR(500) NULL
);
```

---

## 4. Validation Requirements

### Purpose
Basic validation ensures data integrity and demonstrates validation patterns that will need to be migrated to Blazor. Keep validation simple and in code-behind to show traditional WPF approaches.

### Validation Rules

#### Required Fields
All fields except Description are required:
- ClaimNumber
- PolicyHolderName
- DateOfIncident
- DateFiled
- ClaimAmount
- Status

#### String Length Constraints
- ClaimNumber: Maximum 50 characters
- PolicyHolderName: Maximum 100 characters
- Status: Maximum 20 characters (enforced by ComboBox)
- Description: Maximum 500 characters (optional)

#### Date Validation
- **DateOfIncident**: Must be in the past (cannot be today or future date)
- **DateFiled**: Must be a valid date (no past/future restriction)

#### Numeric Validation
- **ClaimAmount**: Must be greater than 0

### Implementation Approach
```csharp
// Example validation in code-behind
private bool ValidateClaim()
{
    StringBuilder errors = new StringBuilder();
    
    // Required field checks
    if (string.IsNullOrWhiteSpace(ClaimNumberTextBox.Text))
        errors.AppendLine("Claim Number is required.");
    
    // Length checks
    if (ClaimNumberTextBox.Text.Length > 50)
        errors.AppendLine("Claim Number cannot exceed 50 characters.");
    
    // Date validation
    if (DateOfIncidentPicker.SelectedDate >= DateTime.Today)
        errors.AppendLine("Date of Incident must be in the past.");
    
    // Numeric validation
    if (!decimal.TryParse(ClaimAmountTextBox.Text, out decimal amount) || amount <= 0)
        errors.AppendLine("Claim Amount must be greater than 0.");
    
    if (errors.Length > 0)
    {
        MessageBox.Show(errors.ToString(), "Validation Error", 
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }
    return true;
}
```

### Error Display
- Use `MessageBox.Show()` with all validation errors
- Block save operation if validation fails
- Clear and simple error messages

---

## 5. User Interface Structure

### 5.1 MainWindow (Claims List)
**Purpose**: Primary window showing all claims

**Layout**:
```
[Menu Bar]
  - File > Exit
  - Claims > Add New Claim

[Claims List - DataGrid]
  Columns:
  - Claim Number
  - Policy Holder
  - Date Filed
  - Amount
  - Status
  - Actions (Edit | Delete buttons)

[Status Bar]
  - Total Claims Count
```

**Features**:
- DataGrid bound to Claims table
- In-line Edit and Delete buttons per row
- Menu option to add new claim
- Auto-refresh after add/edit/delete

### 5.2 AddEditClaimWindow
**Purpose**: Modal dialog for creating or editing a claim

**Layout**:
```
[Form Fields]
  - Claim Number: [TextBox] *Required, Max 50 chars
  - Policy Holder Name: [TextBox] *Required, Max 100 chars
  - Date of Incident: [DatePicker] *Required, Must be past date
  - Date Filed: [DatePicker] *Required
  - Claim Amount: [TextBox] *Required, Must be > 0
  - Status: [ComboBox] *Required (Pending/Processing/Approved/Denied)
  - Description: [TextBox - Multiline] Optional, Max 500 chars

[Buttons]
  [Save] [Cancel]
```

**Validation Rules**:
- All fields marked with * are required
- Claim Number: Cannot be empty, max 50 characters
- Policy Holder Name: Cannot be empty, max 100 characters
- Date of Incident: Must be a date in the past (not today or future)
- Date Filed: Must be a valid date
- Claim Amount: Must be a positive decimal number greater than 0
- Status: Must select one of the predefined options
- Description: Max 500 characters (optional field)
- Display validation errors near the relevant field or in a summary at the top
- Disable Save button or show error message if validation fails

**Behavior**:
- Window title changes: "Add New Claim" vs "Edit Claim"
- Validate all fields when Save button is clicked
- Only save to database if all validations pass
- Show clear error messages for validation failures
- Cancel button closes without saving or validation
- Returns DialogResult.True on successful save

### 5.3 Delete Confirmation
**Purpose**: Confirm deletion before removing claim

**Implementation**: Use `MessageBox.Show()` with Yes/No buttons
- Shows claim number and policy holder name
- Returns true/false based on user selection

---

## 6. Technical Architecture

### 6.1 Project Structure
```
ClaimTracker/
??? App.xaml / App.xaml.cs          # Application entry point
??? MainWindow.xaml / .cs           # Claims list window
??? AddEditClaimWindow.xaml / .cs   # Add/Edit form
??? App.config                       # Connection strings, settings
??? Styles/
?   ??? AppStyles.xaml              # SF BLI brand colors and styling
??? App_Data/
?   ??? ClaimsTracker.mdf           # LocalDB database file
??? Properties/
    ??? Settings.settings            # Application settings
```

### 6.2 Styling and Theming

**AppStyles.xaml Resource Dictionary** contains all colors and control styles:

**SF BLI Brand Colors:**
- Primary Red: `#842231` - Main brand color for primary actions
- Primary Blue: `#435B6B` - Button and header color
- Accent Red: `#A52A3A` - Hover states
- Accent Blue: `#5A7A8F` - Lighter accents

**Available Style Resources:**
- `PrimaryButton` - Red buttons for primary actions
- `SecondaryButton` - Blue outlined buttons
- `ActionButton` - Text-style links for Edit actions
- `DeleteButton` - Red text for delete actions
- `StandardTextBox`, `StandardComboBox`, `StandardDatePicker` - Form controls
- `StandardLabel`, `RequiredLabel` - Form labels
- `StandardDataGrid` with `DataGridColumnHeaderStyle` - Table styling
- `StandardMenu`, `StandardMenuItem` - Menu bar
- `StandardStatusBar` - Footer status bar
- `PageTitle`, `SectionHeader` - Typography

**Usage in XAML:**
```xaml
<Button Style="{StaticResource PrimaryButton}" Content="Save"/>
<TextBox Style="{StaticResource StandardTextBox}"/>
<DataGrid Style="{StaticResource StandardDataGrid}" 
          ColumnHeaderStyle="{StaticResource DataGridColumnHeaderStyle}"/>
```

**Note for Lab Participants:** This styling demonstrates a typical legacy WPF theming approach using ResourceDictionaries. In Blazor, this would translate to CSS files or component libraries.

### 6.3 Data Access Pattern
**Traditional ADO.NET Approach** (or EF6 if preferred):

```csharp
// Example pattern in code-behind
public partial class MainWindow : Window
{
    private string connectionString = 
        ConfigurationManager.ConnectionStrings["ClaimsDB"].ConnectionString;
    
    private void LoadClaims()
    {
        // Direct ADO.NET or EF query
        // Bind results to DataGrid.ItemsSource
    }
    
    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        // Get selected claim
        // Open AddEditClaimWindow in edit mode
        // Refresh grid on dialog close
    }
}
```

### 6.4 Database Setup
- Create LocalDB database on first run
- Use SQL script or code-first approach
- Simple connection string in App.config

---

## 7. Implementation Steps

### Purpose of This Section
These steps are for **building the WPF starting artifact**, not for the lab participants. This WPF app will be provided as a completed reference application.

### Phase 1: Database Setup (15 min)
1. Create Claims table schema
2. Add connection string to App.config
3. Create database initialization code
4. Add sample seed data (5-10 test claims)

### Phase 2: Main Window - Claims List (20 min)
1. Design MainWindow XAML with DataGrid
2. Add menu bar with File and Claims menus
3. Implement LoadClaims() method
4. Wire up Edit button click handler
5. Wire up Delete button with confirmation
6. Add status bar with claim count

### Phase 3: Add/Edit Window (20 min)
1. Create AddEditClaimWindow.xaml
2. Design form layout with labels and controls
3. Add ComboBox with status options
4. Implement validation logic for all required fields
5. Add max length restrictions to TextBox controls
6. Add date validation (DateOfIncident must be in past)
7. Implement Save button logic with validation (INSERT/UPDATE)
8. Implement Cancel button
9. Add constructor overload for edit mode
10. Display validation error messages

### Phase 4: Integration & Polish (15 min)
1. Wire up "Add New Claim" menu item
2. Test full CRUD cycle
3. Add proper window titles
4. Verify data refresh after operations
5. Test delete confirmation flow

### Phase 5: Documentation (10 min)
1. Add code comments explaining key patterns
2. Create README with lab instructions
3. Document database schema
4. Create migration guide with hints
5. List expected migration challenges

---

## 8. Key Learning Points for Migration

This WPF app demonstrates patterns that need translation to Blazor:

| WPF Pattern | Blazor Equivalent |
|-------------|-------------------|
| Window navigation | Component routing/navigation |
| Code-behind data binding | Razor component binding |
| DataGrid | Table/Grid component |
| Modal dialogs | Modal components or pages |
| Menu bar | Navigation menu |
| ADO.NET data access | EF Core / API layer |
| App.config | appsettings.json |
| LocalDB .mdf | SQL Server or SQLite |
| Code-behind validation | Component validation / Data Annotations |
| TextBox MaxLength | Input maxlength attribute |
| DatePicker validation | Input validation attributes |
| ResourceDictionary styles | CSS files / Blazor component libraries |
| StaticResource binding | CSS classes / inline styles |
| XAML styling | CSS / Tailwind / Bootstrap |

---

## 9. Success Criteria

### For the WPF Starter App (Pre-built)
- [ ] Database creates and seeds on first run
- [ ] Claims list displays all records
- [ ] Add new claim saves to database
- [ ] Edit claim updates existing record
- [ ] Delete claim removes record after confirmation
- [ ] UI is simple and intuitive
- [ ] Code is easy to read and understand
- [ ] Code patterns clearly demonstrate legacy WPF approaches
- [ ] Application runs out-of-the-box for lab participants

### For the Training Exercise (Lab Participants)
- [ ] Participants can understand the WPF app in 10 minutes
- [ ] Migration to Blazor can start immediately
- [ ] All major WPF patterns are represented
- [ ] Participants can complete a functional Blazor migration in 90 minutes
- [ ] Lab demonstrates clear before/after comparison

---

## 10. Technical Requirements

### Dependencies
- .NET Framework 4.8.1
- SQL Server LocalDB (included with Visual Studio)
- System.Data.SqlClient (for ADO.NET)
- Optional: EntityFramework 6.x NuGet package

### Configuration
```xml
<!-- App.config example -->
<connectionStrings>
  <add name="ClaimsDB" 
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsTracker.mdf;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## 11. Out of Scope

The following are intentionally excluded to keep the lab focused:

- ? User authentication/authorization
- ? Multiple user support
- ? Claim attachments/documents
- ? Complex validation rules (only basic required/length/date validation included)
- ? Business rule validation (e.g., claim amount limits, status workflows)
- ? Reporting or analytics
- ? Email notifications
- ? Audit logging
- ? Search/filter functionality
- ? Pagination (keep dataset small)
- ? Export to Excel/PDF
- ? MVVM pattern (intentionally code-behind)
- ? Dependency injection
- ? Unit tests (scope too large)
- ? Client-side validation frameworks (manual validation only)

---

## 12. Next Steps

### Building the WPF Starter App (Current Phase)
1. ? **Review and Approve** this plan
2. **Implement Database Schema** and initialization
3. **Build MainWindow** with claims list
4. **Create Add/Edit Window** for claim management
5. **Test Full Workflow** end-to-end
6. **Add Code Comments** highlighting legacy patterns

### Preparing the Lab Materials (After WPF App is Complete)
1. **Create Lab README** with:
   - Setup instructions for participants
   - Lab objectives and time breakdown
   - Migration hints and gotchas
   - Success criteria checklist
2. **Create Migration Guide** with:
   - Pattern mapping reference (WPF ? Blazor)
   - Suggested Blazor project structure
   - Database migration approach
   - Optional stretch goals
3. **Prepare Instructor Guide** with:
   - Solution walkthrough
   - Common mistakes to watch for
   - Time checkpoints
   - Troubleshooting tips
4. **Test the Lab** with a pilot group

### Lab Day Deliverables
Participants receive:
- ? Completed WPF application (source code)
- ? Working database with seed data
- ? Lab instructions and objectives
- ? Pattern migration reference guide
- ? Blazor solution (they build this)

## Notes

- Keep it simple! The goal is to create a **realistic but simple legacy app** for migration training
- Comment the "old patterns" so learners understand what to look for
- Seed realistic but simple test data
- Window sizes should be reasonable (not too large)
- Use standard WPF controls (no custom controls)
- **This WPF app should be "finished" and polished** - participants will not modify it
- The app should work perfectly out-of-the-box on participant machines
- Include comments that highlight typical legacy patterns: "This uses code-behind data binding - a common pattern in legacy WPF apps"
- **Always use the AppStyles.xaml resource dictionary** for colors and styling - do not hard-code colors or styles in XAML
- The SF BLI brand colors (#842231 red, #435B6B blue) should be used consistently throughout
- All UI controls should reference predefined styles (e.g., `Style="{StaticResource PrimaryButton}"`)
- This demonstrates how legacy apps typically centralized styling using ResourceDictionaries

