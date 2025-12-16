using Spectre.Console;

namespace CSharpUpdatesDemos.Infrastructure;

internal static class ConsoleEx
{
    public static void Header(string title)
    {
        AnsiConsole.Write(new Rule($"[yellow]{title}[/]").RuleStyle("grey"));
    }

    public static void SubHeader(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[aqua]{title}[/]").RuleStyle("grey").LeftJustified());
    }

    public static void Kv(string key, string value)
    {
        AnsiConsole.MarkupLine($"[grey]{key}[/]: {value}");
    }

    public static void Note(string text)
    {
        AnsiConsole.MarkupLine($"[grey]{text}[/]");
    }

    public static void Code(string code)
    {
        AnsiConsole.MarkupLine($"[blue]{Markup.Escape(code)}[/]");
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

    public static void BlankLine()
    {
        AnsiConsole.WriteLine();
    }
}
