using CSharpUpdatesDemos.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CSharpUpdatesDemos.Commands;

[Description("Shows nullable reference types, null-state analysis, and common patterns")]
internal sealed class NullableRefTypesCommand : Command<CSharpUpdatesDemos.GlobalSettings>
{
    public override int Execute(CommandContext context, CSharpUpdatesDemos.GlobalSettings settings, CancellationToken cancellationToken)
    {
        NullableRefTypesDemo.Run(settings.Verbose);
        return 0;
    }
}
