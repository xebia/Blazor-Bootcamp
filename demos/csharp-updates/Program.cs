using CSharpUpdatesDemos.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("csharp-updates");

    config.AddCommand<ListCommand>("list")
        .WithDescription("List available demos");

    config.AddCommand<NullableRefTypesCommand>("nullable")
        .WithDescription("Demonstrate nullable reference types and null-safety patterns");

    config.AddCommand<PartialPropertiesCommand>("partial-props")
        .WithDescription("Demonstrate partial properties in C# 13");

    config.AddCommand<FieldBackedPropertiesCommand>("field-keyword")
        .WithDescription("Demonstrate field-backed properties with the 'field' keyword");

    config.PropagateExceptions();
});

try
{
    return await app.RunAsync(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);
    return -1;
}

namespace CSharpUpdatesDemos
{
    [Description("Top-level settings shared by demos")]
    internal class GlobalSettings : CommandSettings
    {
        [CommandOption("--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
    }
}
