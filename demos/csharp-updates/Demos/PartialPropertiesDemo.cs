using CSharpUpdatesDemos.Infrastructure;
using Spectre.Console;
using System.ComponentModel;

namespace CSharpUpdatesDemos.Demos;

/// <summary>
/// Demonstrates partial properties introduced in C# 13.
/// </summary>
internal static class PartialPropertiesDemo
{
    public static void Run(bool verbose)
    {
        ConsoleEx.Header("Partial Properties Demo (C# 13)");

        // Demo 1: What are partial properties?
        ConsoleEx.SubHeader("1. What are Partial Properties?");
        ConsoleEx.Note("Partial properties allow you to split property declaration and implementation.");
        ConsoleEx.Note("This is especially useful for source generators.");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// In your code (declaration):");
        ConsoleEx.Code("public partial class MyViewModel");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    public partial string Name { get; set; }");
        ConsoleEx.Code("}");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Generated code (implementation):");
        ConsoleEx.Code("public partial class MyViewModel");
        ConsoleEx.Code("{");
        ConsoleEx.Code("    private string _name = \"\";");
        ConsoleEx.Code("    public partial string Name");
        ConsoleEx.Code("    {");
        ConsoleEx.Code("        get => _name;");
        ConsoleEx.Code("        set { _name = value; OnPropertyChanged(); }");
        ConsoleEx.Code("    }");
        ConsoleEx.Code("}");

        // Demo 2: Real example with INotifyPropertyChanged pattern
        ConsoleEx.SubHeader("2. Example: Observable Property Pattern");
        ConsoleEx.Note("Partial properties enable source generators to implement INotifyPropertyChanged:");

        var person = new PersonViewModel();
        person.PropertyChanged += (s, e) => 
            ConsoleEx.Note($"  → PropertyChanged fired for: {e.PropertyName}");
        
        ConsoleEx.Kv("Setting FirstName", "Alice");
        person.FirstName = "Alice";
        
        ConsoleEx.Kv("Setting LastName", "Smith");
        person.LastName = "Smith";
        
        ConsoleEx.Kv("FullName", person.FullName);

        // Demo 3: Partial property rules
        ConsoleEx.SubHeader("3. Partial Property Rules");
        ConsoleEx.Success("  ✓ Both parts must be in partial classes/structs");
        ConsoleEx.Success("  ✓ Both parts must have the same type");
        ConsoleEx.Success("  ✓ Both parts must have the same accessibility");
        ConsoleEx.Success("  ✓ Implementing part provides the body");
        ConsoleEx.BlankLine();
        ConsoleEx.Warn("  ⚠ Attributes can be on either or both declarations");
        ConsoleEx.Warn("  ⚠ The implementing declaration must have a body");

        // Demo 4: Attributes on partial properties
        ConsoleEx.SubHeader("4. Attributes on Partial Properties");
        ConsoleEx.Code("// Declaration can have attributes for consumers:");
        ConsoleEx.Code("[JsonPropertyName(\"user_name\")]");
        ConsoleEx.Code("public partial string UserName { get; set; }");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Implementation can have attributes for behavior:");
        ConsoleEx.Code("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        ConsoleEx.Code("public partial string UserName { get => _userName; set => ... }");

        // Demo 5: Use cases
        ConsoleEx.SubHeader("5. Common Use Cases");
        ConsoleEx.Note("Source generators commonly use partial properties for:");
        ConsoleEx.Success("  • MVVM frameworks (INotifyPropertyChanged)");
        ConsoleEx.Success("  • Dependency injection property injection");
        ConsoleEx.Success("  • Validation frameworks");
        ConsoleEx.Success("  • ORM lazy loading");
        ConsoleEx.Success("  • Logging/tracing interceptors");

        // Demo 6: Comparison with partial methods
        ConsoleEx.SubHeader("6. Comparison with Partial Methods");
        ConsoleEx.Note("Partial properties complement existing partial methods:");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Partial method (since C# 3)");
        ConsoleEx.Code("partial void OnNameChanging(string value);");
        ConsoleEx.BlankLine();
        ConsoleEx.Code("// Partial property (C# 13)");
        ConsoleEx.Code("public partial string Name { get; set; }");

        ConsoleEx.BlankLine();
        ConsoleEx.Success("Done.");
    }
}

// Example: Manual implementation of what a source generator would produce
public partial class PersonViewModel : INotifyPropertyChanged
{
    // Declaration part (what you write)
    public partial string FirstName { get; set; }
    public partial string LastName { get; set; }
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Implementation part (what source generator produces)
public partial class PersonViewModel
{
    private string _firstName = "";
    public partial string FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName != value)
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(FullName));
            }
        }
    }
    
    private string _lastName = "";
    public partial string LastName
    {
        get => _lastName;
        set
        {
            if (_lastName != value)
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(FullName));
            }
        }
    }
}
