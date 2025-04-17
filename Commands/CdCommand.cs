using System.IO;
using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class CdCommand : ICustomCommand
{
    public string Name => "cd";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red][*] - Usage: cd <directory>[/]");
            return;
        }

        string target = args[0];
        string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), target));

        if (Directory.Exists(fullPath))
        {
            Directory.SetCurrentDirectory(fullPath);
            context.CurrentDirectory = fullPath;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red][-] - No such directory: {fullPath}[/]");
        }
    }
}