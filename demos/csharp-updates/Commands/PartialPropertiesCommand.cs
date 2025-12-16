using CSharpUpdatesDemos.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CSharpUpdatesDemos.Commands;

[Description("Shows partial properties for source generators (C# 13)")]
internal sealed class PartialPropertiesCommand : Command<CSharpUpdatesDemos.GlobalSettings>
{
    public override int Execute(CommandContext context, CSharpUpdatesDemos.GlobalSettings settings, CancellationToken cancellationToken)
    {
        PartialPropertiesDemo.Run(settings.Verbose);
        return 0;
    }
}
