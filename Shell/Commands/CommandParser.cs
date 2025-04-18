using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace NeonShell.Shell;

public class CommandParser
{
    private readonly Dictionary<string, ICustomCommand> _commands = new();
    private readonly HashSet<string> _systemCommands = new();
    
    private static readonly HashSet<string> InteractiveCommands = new()
    {
        "vim", "nano", "less", "more", "top", "htop", "man", "ssh"
    };

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

        LoadSystemCommands();
        AnsiConsole.MarkupLine($"[bold grey]→ Total commands loaded:[/] [bold green]{_commands.Count + _systemCommands.Count}[/]");
    }

    private void LoadSystemCommands()
    {
        var paths = new[]
        {
            "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin"
        };

        foreach (var dir in paths)
        {
            if (!Directory.Exists(dir)) continue;

            var commands = Directory.GetFiles(dir)
                                    .Select(Path.GetFileName)
                                    .Where(f => !string.IsNullOrWhiteSpace(f));

            foreach (var cmd in commands)
            {
                _systemCommands.Add(cmd);
                var safeCmd = EscapeMarkup(cmd);
                AnsiConsole.MarkupLine($"\t[[[green]+[/]]] Loaded system command: [yellow]{safeCmd}[/]");
            }
        }
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
            if (command is IMetadataCommand meta && meta.RequiresRoot && !(usedSudo || IsRootUser()))
            {
                AnsiConsole.MarkupLine($"[red][[-]] - This command requires root privileges. Prefix with [bold yellow]sudo[/] or run as root.[/]");
                return true;
            }

            command.Execute(context, args);
            return true;
        }

        if (_systemCommands.Contains(cmdName))
        {
            var fullPath = ResolveSystemCommandPath(cmdName);
            if (fullPath != null)
            {
                RunSystemCommand(fullPath, args, usedSudo);
                return true;
            }
        }

        AnsiConsole.MarkupLine($"[red][[-]] - Unknown command:[/] [bold yellow]{cmdName}[/]");
        return true;
    }

    private static string? ResolveSystemCommandPath(string cmdName)
    {
        var paths = new[] { "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin" };

        foreach (var path in paths)
        {
            var fullPath = Path.Combine(path, cmdName);
            if (File.Exists(fullPath) && IsExecutable(fullPath))
                return fullPath;
        }

        return null;
    }

    private static bool IsExecutable(string path)
    {
        return (new FileInfo(path).Exists && (new FileInfo(path).Attributes & FileAttributes.Directory) == 0);
    }

    private static void RunSystemCommand(string path, string[] args, bool useSudo)
    {
        bool isInteractive = InteractiveCommands.Contains(Path.GetFileName(path));

        var startInfo = new ProcessStartInfo
        {
            FileName = useSudo ? "/usr/bin/sudo" : path,
            Arguments = useSudo ? $"{path} {string.Join(' ', args)}" : string.Join(' ', args),
            UseShellExecute = isInteractive,
            RedirectStandardOutput = !isInteractive,
            RedirectStandardError = !isInteractive,
            RedirectStandardInput = false,
            CreateNoWindow = false
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        if (!isInteractive)
        {
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Data);
                    Console.ResetColor();
                }
            };
        }

        process.Start();

        if (!isInteractive)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        process.WaitForExit();
    }

    private static bool IsRootUser()
    {
        return Environment.UserName == "root" || Environment.GetEnvironmentVariable("USER") == "root";
    }

    private static string EscapeMarkup(string input)
    {
        return input.Replace("[", "[[").Replace("]", "]]");
    }
}
