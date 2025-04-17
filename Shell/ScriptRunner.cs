using Spectre.Console;

namespace NeonShell.Shell;

public static class ScriptRunner
{
    public static void RunScript(string path, ShellContext ctx)
    {
        if (!File.Exists(path))
        {
            AnsiConsole.Markup($"[red][[-]] - Script not found: {path}[/]\n");
            return;
        }

        var lines = File.ReadAllLines(path);
        CommandParser parser = new();

        foreach (var line in lines)
        {
            string command = line.Trim();
            if (string.IsNullOrEmpty(command)) continue;

            AnsiConsole.Markup($"[grey]> {command}[/]\n");
            if (!parser.TryExecute(command, ctx))
            {
                AnsiConsole.Markup($"[red][[-]] - Unknown command in script: {command}[/]\n");
            }

            Thread.Sleep(500);
        }
    }
}