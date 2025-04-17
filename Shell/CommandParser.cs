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
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ICustomCommand).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in commandTypes)
        {
            if (Activator.CreateInstance(type) is ICustomCommand cmd)
            {
                _commands[cmd.Name] = cmd;
            }
        }
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
            return true;
        }

        ExecuteFallback(cmdName, args);
        return true;
    }

    private void ExecuteFallback(string cmdName, string[] args)
    {
        var line = $"{cmdName} {string.Join(" ", args)}";

        try
        {
            string shell, shellArgs;

            if (OperatingSystem.IsWindows())
            {
                shell = "wsl.exe";
                shellArgs = line;
            }
            else
            {
                shell = "/bin/bash";
                shellArgs = $"-c \"{line}\"";
            }

            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = shellArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(error))
                AnsiConsole.MarkupLine($"[red]{error}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Shell fallback error: {ex.Message}[/]");
        }
    }
}
