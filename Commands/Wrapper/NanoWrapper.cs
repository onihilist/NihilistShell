﻿
using System.Diagnostics;
using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class NanoWrapper : ICustomCommand
{
    public string Name => "nano";

    public void Execute(ShellContext context, string[] args)
    {
        RunExternalCommand("nano", args);
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

