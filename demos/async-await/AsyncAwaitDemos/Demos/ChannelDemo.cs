using AsyncAwaitDemos.App.Infrastructure;
using Spectre.Console;
using System.Threading.Channels;

namespace AsyncAwaitDemos.App.Demos;

/// <summary>
/// Demonstrates producer/consumer pattern using System.Threading.Channels with backpressure.
/// </summary>
internal static class ChannelDemo
{
	public static async Task RunAsync(int capacity, int items, int producerDelayMs, int consumerDelayMs, CancellationToken cancellationToken = default)
	{
		ConsoleEx.Header("channel");
		ConsoleEx.Kv("Capacity", capacity.ToString());
		ConsoleEx.Kv("Items", items.ToString());
		ConsoleEx.Kv("ProducerDelayMs", producerDelayMs.ToString());
		ConsoleEx.Kv("ConsumerDelayMs", consumerDelayMs.ToString());

		var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(capacity)
		{
			SingleReader = true,
			SingleWriter = true,
			FullMode = BoundedChannelFullMode.Wait
		});

		using var _ = ConsoleEx.StartStopwatch("Channel demo");

		var producer = ProduceAsync(channel.Writer, items, producerDelayMs, cancellationToken);
		var consumer = ConsumeAsync(channel.Reader, consumerDelayMs, cancellationToken);

		await Task.WhenAll(producer, consumer);
		ConsoleEx.Success("Done.");
	}

	private static async Task ProduceAsync(ChannelWriter<int> writer, int count, int delayMs, CancellationToken ct)
	{
		try
		{
			for (var i = 1; i <= count; i++)
			{
				await Task.Delay(delayMs, ct);
				await writer.WriteAsync(i, ct);
				AnsiConsole.MarkupLine($"[grey]Produced[/] [aqua]{i}[/]");
			}
			writer.TryComplete();
		}
		catch (Exception ex)
		{
			writer.TryComplete(ex);
			throw;
		}
	}

	private static async Task ConsumeAsync(ChannelReader<int> reader, int delayMs, CancellationToken ct)
	{
		await foreach (var item in reader.ReadAllAsync(ct))
		{
			await Task.Delay(delayMs, ct);
			AnsiConsole.MarkupLine($"[grey]Consumed[/] [green]{item}[/]");
		}
	}
}
