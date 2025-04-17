using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class TestCommand : ICustomCommand
{
    public string Name => "testabc";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.MarkupLine("[green]Salut c'est un test du loader de commandes ![/]");
    }
}