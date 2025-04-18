using NeonShell.Shell;
using Spectre.Console;
using System.Diagnostics;

namespace NeonShell.Commands;

public class SudoWrapper : ICustomCommand, IMetadataCommand
{
    public string Name => "sudo";
    public string Description => "Execute a command as superuser";
    public bool IsInteractive => true;

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red][[*]] - Usage: sudo <command> [args][/]");
            return;
        }

        string command = string.Join(" ", args);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/sudo",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                CreateNoWindow = false
            }
        };

        try
        {
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red][[-]] - Failed to execute sudo: {ex.Message}[/]");
        }
    }
}