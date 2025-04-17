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
        string line = $"{cmdName} {string.Join(" ", args)}";

        string shell = OperatingSystem.IsWindows() ? "wsl.exe" : "/bin/bash";
        string shellArgs = OperatingSystem.IsWindows() ? line : $"-c \"{line}\"";

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = shell,
                Arguments = shellArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
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
