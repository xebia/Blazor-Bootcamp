using Spectre.Console;
using System.Diagnostics;

namespace AsyncAwaitDemos.App.Infrastructure;

internal static class ConsoleEx
{
	public static void Header(string title)
	{
		AnsiConsole.Write(new Rule($"[yellow]{title}[/]").RuleStyle("grey"));
	}

	public static void Kv(string key, string value)
	{
		AnsiConsole.MarkupLine($"[grey]{key}[/]: {value}");
	}

	public static void Note(string text)
	{
		AnsiConsole.MarkupLine($"[grey]{text}[/]");
	}

	public static void Success(string text)
	{
		AnsiConsole.MarkupLine($"[green]{text}[/]");
	}

	public static void Warn(string text)
	{
		AnsiConsole.MarkupLine($"[yellow]{text}[/]");
	}

	public static void Error(string text)
	{
		AnsiConsole.MarkupLine($"[red]{text}[/]");
	}

	public static IDisposable StartStopwatch(string label)
	{
		var sw = Stopwatch.StartNew();
		return new StopwatchScope(label, sw);
	}

	private sealed class StopwatchScope(string label, Stopwatch stopwatch) : IDisposable
	{
		public void Dispose()
		{
			stopwatch.Stop();
			AnsiConsole.MarkupLine($"[grey]{label}[/]: [aqua]{stopwatch.ElapsedMilliseconds} ms[/]");
		}
	}
}
