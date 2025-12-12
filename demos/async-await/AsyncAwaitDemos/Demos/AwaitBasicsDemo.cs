using AsyncAwaitDemos.App.Infrastructure;

namespace AsyncAwaitDemos.App.Demos;

/// <summary>
/// Demonstrates how async/await yields control, resumes on different threads, and schedules continuations.
/// </summary>
internal static class AwaitBasicsDemo
{
	public static async Task RunAsync(int iterations, int delayMs, CancellationToken cancellationToken = default)
	{
		ConsoleEx.Header("await-basics");
		ConsoleEx.Kv("Process", Environment.ProcessId.ToString());
		ConsoleEx.Kv("Thread", Environment.CurrentManagedThreadId.ToString());
		ConsoleEx.Kv("IsThreadPoolThread", Thread.CurrentThread.IsThreadPoolThread.ToString());
		ConsoleEx.Kv("Iterations", iterations.ToString());
		ConsoleEx.Kv("DelayMs", delayMs.ToString());

		for (var i = 1; i <= iterations; i++)
		{
			ConsoleEx.Note($"\nStep {i}: before await (thread {Environment.CurrentManagedThreadId})");
			await Task.Delay(delayMs, cancellationToken);
			ConsoleEx.Note($"Step {i}: after await  (thread {Environment.CurrentManagedThreadId})");
		}

		ConsoleEx.Success("Done.");
	}
}
