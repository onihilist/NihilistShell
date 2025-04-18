﻿
using Spectre.Console;
using NihilistShell.Shell;
using static NihilistShell.Animation.GlitchOutput;

if (args.Length > 0)
{
    switch (args[0])
    {
        case "--version":
        case "-v":
            Console.WriteLine("NihilistShell v0.1.0");
            return;
        case "--help":
        case "-h":
            Console.WriteLine("Usage: nihilistshell [--version | --help]");
            return;
    }
}

AnsiConsole.Markup("[bold cyan][[*]] - Booting NihilistShell...[/]\n");
AnsiConsole.Markup("[bold cyan][[*]] - Loading commands...[/]\n");
ShellContext context = new();
CommandParser parser = new();
await GlitchedPrint("[+] - System Online", TimeSpan.FromMilliseconds(20));
Console.WriteLine();


while (true)
{
    Environment.SetEnvironmentVariable("LS_COLORS", context.GetLsColors());
    AnsiConsole.Markup(context.GetPrompt());
    string? input = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(input)) continue;

    try
    {
        parser.TryExecute(input, context);
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red][[-]] - Shell crash: {ex.Message}[/]");
    }
}