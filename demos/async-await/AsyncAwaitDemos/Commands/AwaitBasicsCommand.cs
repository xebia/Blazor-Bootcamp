using AsyncAwaitDemos.App.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AsyncAwaitDemos.App.Commands;

[Description("Shows how async/await yields, resumes, and how continuations hop threads")]
internal sealed class AwaitBasicsCommand : AsyncCommand<AsyncAwaitDemos.App.GlobalSettings>
{
	public override async Task<int> ExecuteAsync(CommandContext context, AsyncAwaitDemos.App.GlobalSettings settings, CancellationToken cancellationToken)
	{
		await AwaitBasicsDemo.RunAsync(settings.Iterations, settings.DelayMs, cancellationToken);
		return 0;
	}
}
