using System.IO;
using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class EchoCommand : ICustomCommand
{
    public string Name => "echo";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red][[*]] - Usage: echo <text>[/]");
            return;
        }

        string joined = string.Join(" ", args);

        string finalOutput = joined.Replace("\\n", "\n");

        AnsiConsole.MarkupLine(finalOutput);
    }

    
}