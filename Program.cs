using Spectre.Console;
using NeonShell.Shell;
using static NeonShell.Animation.GlitchOutput;

AnsiConsole.Markup("[bold cyan]⧖ Booting NeonShell...[/]\n");
AnsiConsole.Markup("[bold cyan]⧖ Loading commands...[/]\n");
ShellContext context = new();
CommandParser parser = new();
await GlitchedPrint("System Online", TimeSpan.FromMilliseconds(30));
Console.WriteLine();


while (true)
{
    AnsiConsole.Markup("[bold green][[nihilist-shell@core]][/] >> ");
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