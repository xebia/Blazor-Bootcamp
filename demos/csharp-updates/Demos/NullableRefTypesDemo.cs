using CSharpUpdatesDemos.Infrastructure;
using Spectre.Console;

namespace CSharpUpdatesDemos.Demos;

/// <summary>
/// Demonstrates nullable reference types and null-safety patterns.
/// </summary>
internal static class NullableRefTypesDemo
{
    public static void Run(bool verbose)
    {
        ConsoleEx.Header("Nullable Reference Types Demo");

        // Demo 1: Basic nullable vs non-nullable
        ConsoleEx.SubHeader("1. Nullable vs Non-Nullable Reference Types");
        ConsoleEx.Code("string name = \"Alice\";       // Non-nullable - compiler warns if null");
        ConsoleEx.Code("string? nickname = null;      // Nullable - explicitly allows null");
        
        string name = "Alice";
        string? nickname = null;
        
        ConsoleEx.Kv("name", name);
        ConsoleEx.Kv("nickname", nickname ?? "(null)");
        ConsoleEx.Note("The compiler tracks null-state and warns on potential null dereference.");

        // Demo 2: Null-forgiving operator
        ConsoleEx.SubHeader("2. Null-Forgiving Operator (!)");
        ConsoleEx.Code("string definitelyNotNull = GetMaybeNull()!;");
        ConsoleEx.Warn("Use sparingly! This tells the compiler 'trust me, it's not null'.");
        
        string? maybeNull = GetMaybeNull();
        string definitelyNotNull = maybeNull!; // We know it's not null here
        ConsoleEx.Kv("definitelyNotNull", definitelyNotNull);

        // Demo 3: Null-conditional and null-coalescing
        ConsoleEx.SubHeader("3. Null-Conditional and Null-Coalescing Operators");
        ConsoleEx.Code("int? length = nickname?.Length;           // null if nickname is null");
        ConsoleEx.Code("string display = nickname ?? \"No nickname\"; // default if null");
        ConsoleEx.Code("nickname ??= \"Default\";                    // assign if null");
        
        int? length = nickname?.Length;
        string display = nickname ?? "No nickname";
        ConsoleEx.Kv("length", length?.ToString() ?? "(null)");
        ConsoleEx.Kv("display", display);

        // Demo 4: Pattern matching with null
        ConsoleEx.SubHeader("4. Pattern Matching with Null");
        ConsoleEx.Code("if (value is not null) { ... }");
        ConsoleEx.Code("if (value is string { Length: > 0 } str) { ... }");
        
        ProcessValue(null);
        ProcessValue("");
        ProcessValue("Hello");

        // Demo 5: Nullable attributes
        ConsoleEx.SubHeader("5. Nullable Attributes");
        ConsoleEx.Code("[NotNull] - Parameter won't be null after call");
        ConsoleEx.Code("[MaybeNull] - Return value might be null");
        ConsoleEx.Code("[NotNullWhen(true)] - Not null when method returns true");
        ConsoleEx.Code("[MemberNotNull(nameof(Field))] - Member won't be null after call");

        var person = new Person { Name = "Bob" };
        if (TryGetValue("key", out var value))
        {
            ConsoleEx.Kv("Found value", value); // Compiler knows value is not null here
        }

        // Demo 6: Required members (C# 11+)
        ConsoleEx.SubHeader("6. Required Members");
        ConsoleEx.Code("public required string Name { get; init; }");
        ConsoleEx.Note("'required' ensures the property must be set during initialization.");
        
        var customer = new Customer { Name = "Charlie", Email = "charlie@example.com" };
        ConsoleEx.Kv("Customer", $"{customer.Name} <{customer.Email}>");

        ConsoleEx.BlankLine();
        ConsoleEx.Success("Done.");
    }

    private static string? GetMaybeNull() => "Actually not null";

    private static void ProcessValue(string? value)
    {
        var result = value switch
        {
            null => "Value is null",
            "" => "Value is empty",
            { Length: > 10 } => "Value is long",
            _ => $"Value is: {value}"
        };
        ConsoleEx.Note($"  → {result}");
    }

    private static bool TryGetValue(string key, out string value)
    {
        value = "found-value";
        return true;
    }

    private class Person
    {
        public string Name { get; set; } = "";
    }

    private class Customer
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
    }
}
