using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using System.Reflection;

namespace NeonShell.Shell;

public class CommandParser
{
    private readonly Dictionary<string, ICustomCommand> _commands = new();

    public CommandParser()
    {
        LoadCommands();
    }
    
    private void LoadCommands()
    {
        foreach (var command in CommandRegistry.GetAll())
        {
            _commands[command.Name] = command;
            Spectre.Console.AnsiConsole.MarkupLine($"\t[green][[+]][/] Loaded command: [yellow]{command.Name}[/]");
        }

        Spectre.Console.AnsiConsole.MarkupLine($"[bold grey]→ Total commands loaded:[/] [bold green]{_commands.Count}[/]");
    }

    public bool TryExecute(string commandLine, ShellContext context)
    {
        string expanded = context.ExpandVariables(commandLine);
        var parts = expanded.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return false;

        var cmdName = parts[0];
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(cmdName, out var command))
        {
            command.Execute(context, args);

            if (command is IFallbackAfterExecute fallbackCommand && fallbackCommand.ShouldFallback)
            {
                ExecuteFallback(cmdName, args);
            }

            return true;
        }

        ExecuteFallback(cmdName, args);
        return true;
    }


    private void ExecuteFallback(string cmdName, string[] args)
    {
        string line = $"{cmdName} {string.Join(" ", args)}";

        string shell = OperatingSystem.IsWindows() ? "wsl.exe" : "/bin/bash";
        string shellArgs = OperatingSystem.IsWindows() ? line : $"-c \"{line}\"";
        var interactiveCommands = new HashSet<string> { "nano", "vim", "less", "top", "htop", "man" };
        bool isInteractive = interactiveCommands.Contains(cmdName);

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = shell,
                Arguments = shellArgs,
                UseShellExecute = isInteractive,
                RedirectStandardOutput = !isInteractive,
                RedirectStandardError = !isInteractive,
                RedirectStandardInput = !isInteractive,
                CreateNoWindow = false
            }
        };

        process.OutputDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data))
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Data);
                Console.ResetColor();
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}
