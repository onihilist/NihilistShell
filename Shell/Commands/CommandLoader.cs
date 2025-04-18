using Spectre.Console;

namespace NeonShell.Shell;

public static class CommandLoader
{
    public static HashSet<string> AvailableCommands = new();

    public static void RefreshCommands()
    {
        var paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? Array.Empty<string>();

        var newCommands = new HashSet<string>();

        foreach (var path in paths)
        {
            if (!Directory.Exists(path)) continue;

            try
            {
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    newCommands.Add(fileName);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                AnsiConsole.MarkupLine($"[red][[-]] - Access denied to directory: {path}. Error: [/][bold yellow]{ex.Message}[/]");
            }
            catch (DirectoryNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red][[-]] - Directory not found: {path}. Error: [/][bold yellow]{ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red][[-]] - Error accessing directory {path}:[/] [bold yellow]{ex.Message}[/]");
            }
        }

        //AvailableCommands = newCommands;
        //AnsiConsole.MarkupLine($"[[[green]+[/]]] - Refreshed command list. Found {AvailableCommands.Count} commands.");
    }
    
}
