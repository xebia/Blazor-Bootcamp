using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CSharpUpdatesDemos.Commands;

[Description("Shows the demos you can run")]
internal sealed class ListCommand : Command
{
    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        var table = new Table().RoundedBorder().AddColumn("Command").AddColumn("What it shows");
        table.AddRow("list", "This list");
        table.AddRow("nullable", "Nullable reference types, null checks, and patterns");
        table.AddRow("partial-props", "Partial properties (C# 13) for source generators");
        table.AddRow("field-keyword", "Field-backed properties with the 'field' keyword (C# 13)");

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\nTry: [aqua]dotnet run -- nullable[/]");
        AnsiConsole.MarkupLine("[grey]For AOT demos, see the separate ../aot-demo project[/]");
        return 0;
    }
}
