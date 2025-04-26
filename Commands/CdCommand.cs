
using System.IO;
using NShell.Shell;
using Spectre.Console;

namespace NShell.Commands;

public class CdCommand : ICustomCommand
{
    public string Name => "cd";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: cd <directory>");
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
            AnsiConsole.MarkupLine($"[[[red]-[/]]] - No such directory: {fullPath}");
        }
    }
}
