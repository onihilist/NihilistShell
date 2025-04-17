using Spectre.Console;
using NeonShell.Shell;

AnsiConsole.Markup("[bold cyan]⧖ Booting NeonShell...[/]\n");
await Task.Delay(1000);
AnsiConsole.Markup("[bold green]System Online.[/]\n\n");

ShellContext context = new();
CommandParser parser = new();

while (true)
{
    AnsiConsole.Markup("[bold green][[neon@core]] >> [/]");
    string? input = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(input)) continue;

    try
    {
        parser.TryExecute(input, context);
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]💥 Shell crash: {ex.Message}[/]");
    }
}