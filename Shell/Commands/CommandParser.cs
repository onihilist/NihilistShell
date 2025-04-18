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
            AnsiConsole.MarkupLine($"\t[green][[+]][/] Loaded command: [yellow]{command.Name}[/]");
        }

        AnsiConsole.MarkupLine($"[bold grey]→ Total commands loaded:[/] [bold green]{_commands.Count}[/]");
    }

    public bool TryExecute(string commandLine, ShellContext context)
    {
        string expanded = context.ExpandVariables(commandLine);
        var parts = expanded.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return false;

        bool usedSudo = false;

        if (parts[0] == "sudo")
        {
            usedSudo = true;
            parts = parts.Skip(1).ToArray();
        }

        if (parts.Length == 0) return false;

        var cmdName = parts[0];
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(cmdName, out var command))
        {
            if (command is IMetadataCommand meta)
            {
                if (meta.RequiresRoot && !(usedSudo || IsRootUser()))
                {
                    AnsiConsole.MarkupLine($"[red][-] - This command requires root privileges. Prefix with [bold yellow]sudo[/] or run as root.[/]");
                    return true;
                }
            }

            command.Execute(context, args);
            return true;
        }

        // Fallback to system binary in /usr/bin
        string path = $"/usr/bin/{cmdName}";
        if (File.Exists(path) && IsExecutable(path))
        {
            RunSystemCommand(path, args, usedSudo);
            return true;
        }

        AnsiConsole.MarkupLine($"[red][-] - Unknown command:[/] [bold yellow]{cmdName}[/]");
        return true;
    }

    private static bool IsExecutable(string path)
    {
        return (new FileInfo(path).Exists && (new FileInfo(path).Attributes & FileAttributes.Directory) == 0);
    }

    private static void RunSystemCommand(string path, string[] args, bool useSudo)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = useSudo ? "/usr/bin/sudo" : path,
            Arguments = useSudo ? $"{path} {string.Join(' ', args)}" : string.Join(' ', args),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = new System.Diagnostics.Process
        {
            StartInfo = startInfo
        };

        process.OutputDataReceived += (s, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (s, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Data);
                Console.ResetColor();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }

    private static bool IsRootUser()
    {
        return Environment.UserName == "root" || Environment.GetEnvironmentVariable("USER") == "root";
    }
}