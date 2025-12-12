using AsyncAwaitDemos.App.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AsyncAwaitDemos.App.Commands;

[Description("Runs a few tasks in parallel and demonstrates Task.WhenAll exception/cancellation behavior")]
internal sealed class TaskWhenAllCommand : AsyncCommand<TaskWhenAllCommand.Settings>
{
	internal sealed class Settings : AsyncAwaitDemos.App.GlobalSettings
	{
		[CommandOption("--cancel-after-ms <MS>")]
		[DefaultValue(0)]
		public int CancelAfterMs { get; init; }

		[CommandOption("--fail-on <N>")]
		[DefaultValue(0)]
		public int FailOn { get; init; }
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
	{
		try
		{
			await TaskWhenAllDemo.RunAsync(settings.Iterations, settings.DelayMs, settings.FailOn, settings.CancelAfterMs, cancellationToken);
			return 0;
		}
		catch (OperationCanceledException)
		{
			return 2;
		}
		catch
		{
			return 1;
		}
	}
}
