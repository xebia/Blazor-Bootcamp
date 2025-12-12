using AsyncAwaitDemos.App.Infrastructure;
using Spectre.Console;
using System.Diagnostics;

namespace AsyncAwaitDemos.App.Demos;

/// <summary>
/// Demonstrates ValueTask fast-path completions and the "don't await twice" pitfall.
/// </summary>
internal static class ValueTaskDemo
{
	public static async Task RunAsync(int iterations, int delayMs, int hitRatePercent, int seed, bool doubleAwait, CancellationToken cancellationToken = default)
	{
		ConsoleEx.Header("valuetask");
		ConsoleEx.Kv("Iterations", iterations.ToString());
		ConsoleEx.Kv("DelayMs", delayMs.ToString());
		ConsoleEx.Kv("HitRatePercent", hitRatePercent.ToString());
		ConsoleEx.Kv("DoubleAwait", doubleAwait.ToString());

		var rng = new Random(seed);
		var immediateCount = 0;
		var delayedCount = 0;

		var sw = Stopwatch.StartNew();
		for (var i = 0; i < iterations; i++)
		{
			var hit = rng.Next(0, 100) < hitRatePercent;
			var vt = MaybeCachedValueTaskAsync(hit, delayMs, cancellationToken);

			if (vt.IsCompletedSuccessfully)
				immediateCount++;
			else
				delayedCount++;

			if (doubleAwait)
			{
				ConsoleEx.Warn($"\nIteration {i + 1}: Attempting to await the same ValueTask twice (WRONG!)");
				
				// First await
				var a = await vt;
				ConsoleEx.Note($"  First await returned: {a}");
				
				// Second await - this is undefined behavior!
				// It might work, throw, or return wrong results depending on the backing state
				try
				{
					var b = await vt;
					ConsoleEx.Error($"  Second await returned: {b} - This worked but is UNDEFINED BEHAVIOR!");
					ConsoleEx.Note("  You got lucky this time, but this can fail unpredictably.");
					ConsoleEx.Note("  NEVER await a ValueTask multiple times!");
					ConsoleEx.Note("  If you need multiple awaits: var task = vt.AsTask(); then await task multiple times.");
				}
				catch (Exception ex)
				{
					ConsoleEx.Error($"  Second await threw: {ex.GetType().Name}: {ex.Message}");
					ConsoleEx.Success("  This demonstrates the pitfall correctly!");
				}
				
				// Only do one iteration when demonstrating the pitfall
				break;
			}
			else
			{
				_ = await vt;
			}
		}
		sw.Stop();

		AnsiConsole.Write(new BreakdownChart()
			.Width(60)
			.AddItem("Completed synchronously", immediateCount, Color.Green)
			.AddItem("Needed awaiting", delayedCount, Color.Yellow));

		ConsoleEx.Kv("Elapsed", $"{sw.ElapsedMilliseconds} ms");
		ConsoleEx.Success("Done.");
	}

	private static ValueTask<int> MaybeCachedValueTaskAsync(bool hit, int delayMs, CancellationToken ct)
	{
		if (hit)
			return ValueTask.FromResult(123);

		return new ValueTask<int>(SlowAsync(delayMs, ct));
	}

	private static async Task<int> SlowAsync(int delayMs, CancellationToken ct)
	{
		await Task.Delay(delayMs, ct);
		return 123;
	}
}
