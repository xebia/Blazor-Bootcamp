using AsyncAwaitDemos.App.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

var app = new CommandApp();

app.Configure(config =>
{
	config.SetApplicationName("async-await-demos");

	config.AddCommand<ListCommand>("list")
		.WithDescription("List available demos");

	config.AddCommand<AwaitBasicsCommand>("await-basics")
		.WithDescription("Demonstrate how async/await schedules continuations");

	config.AddCommand<TaskWhenAllCommand>("task-whenall")
		.WithDescription("Demonstrate Task.WhenAll, exceptions, and cancellation");

	config.AddCommand<ValueTaskCommand>("valuetask")
		.WithDescription("Demonstrate ValueTask pitfalls and allocation patterns");

	config.AddCommand<ChannelCommand>("channel")
		.WithDescription("Producer/consumer using System.Threading.Channels");

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

namespace AsyncAwaitDemos.App
{
	[Description("Top-level settings shared by demos")]
		internal class GlobalSettings : CommandSettings
	{
		[CommandOption("--iterations <N>")]
		[DefaultValue(5)]
		public int Iterations { get; init; }

		[CommandOption("--delay-ms <MS>")]
		[DefaultValue(150)]
		public int DelayMs { get; init; }

		[CommandOption("--verbose")]
		[DefaultValue(false)]
		public bool Verbose { get; init; }
	}
}
