using CSharpUpdatesDemos.Infrastructure;
using Spectre.Console;

namespace CSharpUpdatesDemos.Demos;

/// <summary>
/// Demonstrates field-backed properties with the 'field' keyword (C# 13).
/// </summary>
internal static class FieldBackedPropertiesDemo
{
    public static void Run(bool verbose)
    {
        ConsoleEx.Header("Field-Backed Properties Demo (C# 13)");

        // Demo 1: The problem with auto-properties
        ConsoleEx.SubHeader("1. The Problem with Auto-Properties");
        ConsoleEx.Note("Auto-properties are great but have limitations:");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Can't add logic without creating a backing field:");
        ConsoleEx.Code("public string Name { get; set; }");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Before C# 13: Must create explicit backing field:");
        ConsoleEx.Code("private string _name = \"\";");
        ConsoleEx.Code("public string Name");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get => _name;");
        ConsoleEx.Code("    set => _name = value?.Trim() ?? \"\";");
        ConsoleEx.Code("}");

        // Demo 2: The 'field' keyword solution
        ConsoleEx.SubHeader("2. The 'field' Keyword Solution");
        ConsoleEx.Note("C# 13 introduces the 'field' keyword for auto-property backing fields:");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("public string Name");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get => field;");
        ConsoleEx.Code("    set => field = value?.Trim() ?? \"\";");
        ConsoleEx.Code("}");
        ConsoleEx.BlankLine();
        ConsoleEx.Success("No need to declare a separate backing field!");

        // Demo 3: Practical example - validation
        ConsoleEx.SubHeader("3. Example: Property Validation");
        
        var product = new Product();
        ConsoleEx.Kv("Setting Name", "  Widget  ");
        product.Name = "  Widget  ";
        ConsoleEx.Kv("Trimmed Name", $"\"{product.Name}\"");
        
        ConsoleEx.BlankLine();
        ConsoleEx.Kv("Setting Price", "-10");
        product.Price = -10;
        ConsoleEx.Kv("Validated Price", product.Price.ToString());
        
        ConsoleEx.BlankLine();
        ConsoleEx.Kv("Setting Quantity", "5");
        product.Quantity = 5;
        ConsoleEx.Kv("Clamped Quantity", product.Quantity.ToString());

        // Demo 4: Lazy initialization
        ConsoleEx.SubHeader("4. Example: Lazy Initialization");
        ConsoleEx.Code("public string ExpensiveData");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get => field ??= ComputeExpensiveData();");
        ConsoleEx.Code("}");
        
        var service = new DataService();
        ConsoleEx.Kv("First access", service.ExpensiveData);
        ConsoleEx.Kv("Second access (cached)", service.ExpensiveData);

        // Demo 5: Combining with init accessors
        ConsoleEx.SubHeader("5. Combining with Init Accessors");
        ConsoleEx.Code("public string Id");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get => field;");
        ConsoleEx.Code("    init => field = ValidateId(value);");
        ConsoleEx.Code("}");
        
        var entity = new Entity { Id = "abc-123" };
        ConsoleEx.Kv("Normalized Id", entity.Id);

        // Demo 6: 'field' with events/notifications
        ConsoleEx.SubHeader("6. Change Notification Pattern");
        ConsoleEx.Code("public string Title");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get => field;");
        ConsoleEx.Code("    set");
        ConsoleEx.Code("    {");
        ConsoleEx.Code("        if (field != value)");
        ConsoleEx.Code("        {");
        ConsoleEx.Code("            field = value;");
        ConsoleEx.Code("            OnPropertyChanged();");
        ConsoleEx.Code("        }");
        ConsoleEx.Code("    }");
        ConsoleEx.Code("}");

        var doc = new Document();
        doc.TitleChanged += (s, e) => ConsoleEx.Note($"  → Title changed to: {e}");
        doc.Title = "Draft";
        doc.Title = "Final Report";
        doc.Title = "Final Report"; // No event - same value

        // Demo 7: Important considerations
        ConsoleEx.SubHeader("7. Important Considerations");
        ConsoleEx.Warn("  ⚠ 'field' is a contextual keyword (only in property accessors)");
        ConsoleEx.Warn("  ⚠ Existing code with 'field' identifier may need @field");
        ConsoleEx.Success("  ✓ Works with get, set, and init accessors");
        ConsoleEx.Success("  ✓ Can mix 'field' in one accessor with auto in another");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Mixed: custom get, auto set");
        ConsoleEx.Code("public int Count");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    get { Log(); return field; }");
        ConsoleEx.Code("    set;  // auto-implemented");
        ConsoleEx.Code("}");

        // Demo 8: Default values
        ConsoleEx.SubHeader("8. Default Values with 'field'");
        ConsoleEx.Code("public string Status { get; set => field = value ?? \"Unknown\"; } = \"Pending\";");
        ConsoleEx.Note("Initializer sets the backing field directly.");

        var order = new Order();
        ConsoleEx.Kv("Default Status", order.Status);
        order.Status = null!; // Intentionally setting null to show the setter handles it
        ConsoleEx.Kv("After setting null", order.Status ?? "(null)");

        ConsoleEx.BlankLine();
        ConsoleEx.Success("Done.");
    }
}

// Demo classes using field-backed properties
public class Product
{
    // Trim whitespace
    public string Name
    {
        get => field;
        set => field = value?.Trim() ?? "";
    } = "";

    // Ensure non-negative
    public decimal Price
    {
        get => field;
        set => field = Math.Max(0, value);
    }

    // Clamp to valid range
    public int Quantity
    {
        get => field;
        set => field = Math.Clamp(value, 0, 1000);
    }
}

public class DataService
{
    private int _computeCount;
    
    // Lazy initialization
    public string ExpensiveData
    {
        get => field ??= ComputeExpensiveData();
    }

    private string ComputeExpensiveData()
    {
        _computeCount++;
        ConsoleEx.Note($"  (Computing expensive data... call #{_computeCount})");
        return $"Data computed at {DateTime.Now:HH:mm:ss}";
    }
}

public class Entity
{
    // Normalize on init
    public string Id
    {
        get => field;
        init => field = value?.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(value));
    } = "";
}

public class Document
{
    public event EventHandler<string>? TitleChanged;

    // Change notification
    public string Title
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value ?? "";
                TitleChanged?.Invoke(this, field);
            }
        }
    } = "";
}

public class Order
{
    // Default with custom setter
    public string Status
    {
        get => field;
        set => field = value ?? "Unknown";
    } = "Pending";
}
