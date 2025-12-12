using AsyncAwaitDemos.App.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AsyncAwaitDemos.App.Commands;

[Description("Demonstrates ValueTask fast-path and the 'don't await twice' pitfall")]
internal sealed class ValueTaskCommand : AsyncCommand<ValueTaskCommand.Settings>
{
	internal sealed class Settings : AsyncAwaitDemos.App.GlobalSettings
	{
		[CommandOption("--hit-rate <PERCENT>")]
		[DefaultValue(80)]
		public int HitRatePercent { get; init; }

		[CommandOption("--seed <N>")]
		[DefaultValue(42)]
		public int Seed { get; init; }

		[CommandOption("--double-await")]
		[DefaultValue(false)]
		public bool DoubleAwait { get; init; }
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
	{
		try
		{
			await ValueTaskDemo.RunAsync(settings.Iterations, settings.DelayMs, settings.HitRatePercent, settings.Seed, settings.DoubleAwait, cancellationToken);
			return 0;
		}
		catch
		{
			return 1;
		}
	}
}
