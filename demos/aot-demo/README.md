# AOT Compilation Demo

A simple console application demonstrating Native AOT (Ahead-of-Time) compilation in .NET.

## Why a Separate Project?

This demo is separate from the other C# demos because Spectre.Console doesn't support AOT compilation. This project uses only AOT-compatible APIs.

## Running the Demo

### Run with JIT (normal)

```bash
dotnet run
```

### Publish as Native AOT

```bash
# Windows
dotnet publish -c Release -r win-x64

# Linux
dotnet publish -c Release -r linux-x64

# macOS (Intel)
dotnet publish -c Release -r osx-x64

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64
```

The native executable will be in `bin/Release/net10.0/<rid>/publish/`

## What This Demo Shows

1. **Detecting AOT at Runtime** - Using `RuntimeFeature.IsDynamicCodeSupported`
2. **Startup Performance** - AOT apps start faster (no JIT warmup)
3. **Memory Footprint** - Typically lower memory usage
4. **AOT-Compatible JSON** - Using source generators with `System.Text.Json`
5. **Patterns to Avoid** - Reflection, dynamic code generation
6. **AOT-Friendly Alternatives** - Generics, source generators, annotations

## Key Project Settings

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

## Requirements

- .NET 10 SDK
- For publishing: Native build tools for your platform
  - Windows: Visual Studio Build Tools with C++ workload
  - Linux: clang, build-essential
  - macOS: Xcode Command Line Tools
