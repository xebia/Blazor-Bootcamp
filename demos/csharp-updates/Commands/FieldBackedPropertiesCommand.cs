using CSharpUpdatesDemos.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CSharpUpdatesDemos.Commands;

[Description("Shows field-backed properties with the 'field' keyword (C# 13)")]
internal sealed class FieldBackedPropertiesCommand : Command<CSharpUpdatesDemos.GlobalSettings>
{
    public override int Execute(CommandContext context, CSharpUpdatesDemos.GlobalSettings settings, CancellationToken cancellationToken)
    {
        FieldBackedPropertiesDemo.Run(settings.Verbose);
        return 0;
    }
}
