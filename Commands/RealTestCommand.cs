using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class RealTestCommand : ICustomCommand
{
    public string Name => "realtest";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.MarkupLine("[green]✅ This is a real test command![/]");
    }
}