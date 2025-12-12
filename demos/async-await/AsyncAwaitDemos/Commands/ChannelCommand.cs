using AsyncAwaitDemos.App.Demos;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AsyncAwaitDemos.App.Commands;

[Description("Producer/consumer using Channel<T> with backpressure")]
internal sealed class ChannelCommand : AsyncCommand<ChannelCommand.Settings>
{
	internal sealed class Settings : AsyncAwaitDemos.App.GlobalSettings
	{
		[CommandOption("--capacity <N>")]
		[DefaultValue(5)]
		public int Capacity { get; init; }

		[CommandOption("--producer-delay-ms <MS>")]
		[DefaultValue(50)]
		public int ProducerDelayMs { get; init; }

		[CommandOption("--consumer-delay-ms <MS>")]
		[DefaultValue(150)]
		public int ConsumerDelayMs { get; init; }

		[CommandOption("--items <N>")]
		[DefaultValue(20)]
		public int Items { get; init; }
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
	{
		await ChannelDemo.RunAsync(settings.Capacity, settings.Items, settings.ProducerDelayMs, settings.ConsumerDelayMs, cancellationToken);
		return 0;
	}
}
