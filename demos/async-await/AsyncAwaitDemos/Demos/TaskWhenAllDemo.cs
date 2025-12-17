using AsyncAwaitDemos.App.Infrastructure;
using Spectre.Console;

namespace AsyncAwaitDemos.App.Demos;

/// <summary>
/// Demonstrates Task.WhenAll behavior: running tasks in parallel, aggregating exceptions, and cancellation propagation.
/// </summary>
internal static class TaskWhenAllDemo
{
	public static async Task RunAsync(int iterations, int delayMs, int failOn, int cancelAfterMs, CancellationToken cancellationToken = default)
	{
		ConsoleEx.Header("task-whenall");
		ConsoleEx.Kv("Iterations", iterations.ToString());
		ConsoleEx.Kv("DelayMs", delayMs.ToString());
		ConsoleEx.Kv("FailOn", failOn.ToString());
		ConsoleEx.Kv("CancelAfterMs", cancelAfterMs.ToString());

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		if (cancelAfterMs > 0)
		{
			cts.CancelAfter(TimeSpan.FromMilliseconds(cancelAfterMs));
			ConsoleEx.Warn("Cancellation scheduled.");
		}

		var tasks = Enumerable.Range(1, iterations)
			.Select(i => WorkAsync(i, delayMs, failOn, cts.Token))
			.ToArray();

		try
		{
			using var _ = ConsoleEx.StartStopwatch("Task.WhenAll");
			await Task.WhenAll(tasks);
			ConsoleEx.Success("All tasks completed successfully.");
		}
		catch (OperationCanceledException)
		{
			ConsoleEx.Warn("Caught OperationCanceledException from Task.WhenAll.");
			DumpTaskOutcomes(tasks);
			throw;
		}
		catch (AggregateException agg)
		{
            ConsoleEx.Error($"Caught exception: {agg.GetType().Name}: {agg.Message}");
            DumpTaskOutcomes(tasks);
            AnsiConsole.MarkupLine("\n[aqua]AggregateException.InnerExceptions[/]");
			foreach (var inner in agg.InnerExceptions)
			{
				AnsiConsole.MarkupLine($"- [red]{inner.GetType().Name}[/]: {inner.Message}");
			}
			throw;
		}
		catch (Exception ex)
		{
			ConsoleEx.Error($"Caught exception: {ex.GetType().Name}: {ex.Message}");
			DumpTaskOutcomes(tasks);
			throw;
		}
	}

	private static async Task WorkAsync(int id, int delayMs, int failOn, CancellationToken ct)
	{
		await Task.Delay(delayMs, ct);
		if (failOn == id)
			throw new InvalidOperationException($"Boom from task {id}");

		await Task.Delay(delayMs, ct);
	}

	private static void DumpTaskOutcomes(Task[] tasks)
	{
		var table = new Table().RoundedBorder().AddColumn("Task").AddColumn("Status").AddColumn("Exception");
		for (var i = 0; i < tasks.Length; i++)
		{
			var t = tasks[i];
			var ex = t.Exception?.GetBaseException();
			table.AddRow(
				(i + 1).ToString(),
				t.Status.ToString(),
				ex is null ? "" : $"{ex.GetType().Name}: {ex.Message}");
		}
		AnsiConsole.Write(table);
	}
}
