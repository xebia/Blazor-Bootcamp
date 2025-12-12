using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AsyncAwaitDemos.App.Commands;

[Description("Shows the demos you can run")]
internal sealed class ListCommand : AsyncCommand
{
	public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
	{
		var table = new Table().RoundedBorder().AddColumn("Command").AddColumn("What it shows");
		table.AddRow("list", "This list");
		table.AddRow("await-basics", "Thread IDs, synchronization context basics, continuation scheduling");
		table.AddRow("task-whenall", "Parallel tasks, aggregation of exceptions, cancellation propagation");
		table.AddRow("valuetask", "Fast-path completions, why you must not await a ValueTask twice");
		table.AddRow("channel", "Bounded channel producer/consumer, backpressure, cancellation");

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine("\nTry: [aqua]dotnet run --project src/AsyncAwaitDemos.App -- await-basics --iterations 5[/]");
		return Task.FromResult(0);
	}
}
