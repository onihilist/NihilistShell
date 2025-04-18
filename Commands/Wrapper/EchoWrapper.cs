using System.Diagnostics;
using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class EchoWrapper : ICustomCommand, IMetadataCommand
{
    public string Name => "echo";
    public string Description => "Write args in the terminal";
    public bool   RequiresRoot => false;

    public void Execute(ShellContext context, string[] args)
    {
        RunExternalCommand("echo", args);
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