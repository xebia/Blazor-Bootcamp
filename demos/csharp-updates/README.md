# C# Updates Demos

Interactive command-line demos showcasing modern C# features using Spectre.Console.

## Running the Demos

```bash
# List all available demos
dotnet run -- list

# Run individual demos
dotnet run -- nullable        # Nullable reference types
dotnet run -- partial-props   # Partial properties (C# 13)
dotnet run -- field-keyword   # Field-backed properties (C# 13)

# With verbose output
dotnet run -- nullable --verbose
```

> **Note:** For AOT compilation demos, see the separate `../aot-demo` project (Spectre.Console doesn't support AOT).

## Demo Topics

### 1. Nullable Reference Types (`nullable`)
- Nullable vs non-nullable reference types
- Null-forgiving operator (`!`)
- Null-conditional (`?.`) and null-coalescing (`??`, `??=`) operators
- Pattern matching with null
- Nullable attributes (`[NotNull]`, `[MaybeNull]`, `[NotNullWhen]`)
- Required members (`required` keyword)

### 2. Partial Properties (`partial-props`)
- What are partial properties (C# 13)
- Use with source generators
- Observable property pattern (INotifyPropertyChanged)
- Partial property rules and attributes
- Common use cases

### 3. Field-Backed Properties (`field-keyword`)
- The `field` keyword for accessing auto-property backing fields (C# 13)
- Property validation without explicit backing fields
- Lazy initialization patterns
- Combining with init accessors
- Change notification patterns

## Requirements

- .NET 10 SDK (for C# 13 preview features)
- `LangVersion` set to `preview` in the project file
