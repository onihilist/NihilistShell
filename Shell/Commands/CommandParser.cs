
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace NeonShell.Shell;

/// <summary>
/// <c>CommandParser</c> class handles loading and execution of shell commands.
/// It supports custom commands, system commands, and privilege management (e.g., sudo).
/// </summary>
public class CommandParser
{

    private readonly Dictionary<string, ICustomCommand> _commands = new();
    private readonly HashSet<string> _systemCommands = new();
    private static readonly HashSet<string> InteractiveCommands = new()
    {
        "vim", "nano", "less", "more", "top", "htop", "man", "ssh"
    };

    /// <summary>
    /// Constructor that loads the commands when the parser is instantiated.
    /// </summary>
    public CommandParser()
    {
        LoadCommands();
    }

    /// <summary>
    /// Loads all the custom commands and system commands from predefined directories.
    /// </summary>
    private void LoadCommands()
    {
        foreach (var command in CommandRegistry.GetAll())
        {
            _commands[command.Name] = command;
            AnsiConsole.MarkupLine($"\t[green][[+]][/] Loaded custom command: [yellow]{command.Name}[/]");
        }

        LoadSystemCommands();
        AnsiConsole.MarkupLine($"[bold grey]→ Total commands loaded:[/] [bold green]{_commands.Count + _systemCommands.Count}[/]");
    }

    /// <summary>
    /// Loads system commands from typical binary directories.
    /// </summary>
    private void LoadSystemCommands()
    {
        var paths = new[] { "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin" };

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
                //AnsiConsole.MarkupLine($"\t[[[green]+[/]]] Loaded system command: [yellow]{safeCmd}[/]");
            }
        }
    }

    /// <summary>
    /// Attempts to execute a command from the provided command line input.
    /// Handles variable expansion, root privileges, and command execution.
    /// </summary>
    /// <param name="commandLine">The command line string entered by the user.</param>
    /// <param name="context">The shell context that contains environment variables and the current directory.</param>
    /// <returns>Returns true if the command was successfully executed, false otherwise.</returns>
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
                bool success = RunSystemCommand(fullPath, args, usedSudo);
                if ((cmdName == "apt" || cmdName == "apt-get") && args.Length > 0 && args[0] == "install")
                {
                    CommandLoader.RefreshCommands();
                }
                
                return true;
            }
        }

        AnsiConsole.MarkupLine($"[red][[-]] - Unknown command:[/] [bold yellow]{cmdName}[/]");
        return true;
    }

    /// <summary>
    /// Resolves the full path of a system command by searching in common system directories.
    /// </summary>
    /// <param name="cmdName">The name of the system command.</param>
    /// <returns>The full path to the command if found, otherwise null.</returns>
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

    /// <summary>
    /// Checks if a file is executable.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>True if the file is executable, otherwise false.</returns>
    private static bool IsExecutable(string path)
    {
        return (new FileInfo(path).Exists && (new FileInfo(path).Attributes & FileAttributes.Directory) == 0);
    }

    /// <summary>
    /// Runs a system command, optionally using `sudo`, and handles interactive vs non-interactive command behavior.
    /// </summary>
    /// <param name="path">The full path to the system command.</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="useSudo">Whether to use `sudo` to run the command.</param>
    private static bool RunSystemCommand(string path, string[] args, bool useSudo)
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

        var process = new Process { StartInfo = startInfo };

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

        return process.ExitCode == 0;
    }

    /// <summary>
    /// Checks if the current user is root.
    /// </summary>
    /// <returns>True if the current user is root, otherwise false.</returns>
    private static bool IsRootUser()
    {
        return Environment.UserName == "root" || Environment.GetEnvironmentVariable("USER") == "root";
    }

    /// <summary>
    /// Escapes markup characters in a string.
    /// </summary>
    /// <param name="input">The input string to escape.</param>
    /// <returns>The escaped string.</returns>
    private static string EscapeMarkup(string input)
    {
        return input.Replace("[", "[[").Replace("]", "]]");
    }
}

