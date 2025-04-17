using Spectre.Console;

namespace NeonShell.Shell;

public class CommandParser
{
    private readonly Dictionary<string, Action<ShellContext, string[]>> _commands;

    public CommandParser()
    {
        _commands = new()
        {
            { "set", (ctx, args) => ctx.SetVar(args[0], args[1]) },
            { "unset", (ctx, args) => ctx.UnsetVar(args[0]) },
            { "echo", (ctx, args) => AnsiConsole.MarkupLine(string.Join(" ", args)) },
            { "exit", (_, _) => Environment.Exit(0) }
        };
    }

    public bool TryExecute(string commandLine, ShellContext context)
    {
        var expanded = context.ExpandVariables(commandLine);
        var sequences = expanded.Split("&&", StringSplitOptions.RemoveEmptyEntries);

        foreach (var sequence in sequences)
        {
            var commands = sequence.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var cmd in commands)
            {
                string clean = cmd.Trim();
                if (string.IsNullOrWhiteSpace(clean)) continue;

                var parts = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var cmdName = parts[0];
                var args = parts.Skip(1).ToArray();

                if (_commands.TryGetValue(cmdName, out var action))
                {
                    try
                    {
                        action(context, args);
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.MarkupLine($"[red]Command error: {e.Message}[/]");
                    }
                }
                else
                {
                    ExecuteWithBash(clean);
                }
            }
        }

        return true;
    }

    private void ExecuteWithBash(string line)
    {
        try
        {
            string shell;
            string args;

            if (OperatingSystem.IsWindows())
            {
                shell = "wsl.exe";
                args = line;
            }
            else
            {
                shell = "/bin/bash";
                args = $"-c \"{line}\"";
            }

            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = args,
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
