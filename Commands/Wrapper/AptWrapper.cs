using System.Diagnostics;
using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class AptWrapper : ICustomCommand, IMetadataCommand
{
    public string Name => "apt";
    public string Description => "Run apt command";
    public bool   RequiresRoot => true;

    public void Execute(ShellContext context, string[] args)
    {
        RunExternalCommand("apt", args);
    }

    private void RunExternalCommand(string command, string[] args)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = string.Join(" ", args),
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red][[-]] - Failed to run command '{command}': {ex.Message}[/]");
        }
    }
}