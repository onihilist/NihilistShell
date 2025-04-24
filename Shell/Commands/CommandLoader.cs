
using Spectre.Console;

namespace NShell.Shell.Commands;

/// <summary>
/// <c>CommandLoader</c> is responsible for loading custom and system commands.
/// </summary>
public static class CommandLoader
{

    /// <summary>
    /// Refresh commands list after each apt/apt-get install.
    /// Prevent from installing a binary and don't find it if you don't reboot the shell.
    /// </summary>
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
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Access denied to directory: {path}. Error: [bold yellow]{ex.Message}[/]");
            }
            catch (DirectoryNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Directory not found: {path}. Error: [bold yellow]{ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Error accessing directory {path}: [bold yellow]{ex.Message}[/]");
            }
        }

        HashSet<string> availableCommands = newCommands;
        AnsiConsole.MarkupLine($"[[[green]+[/]]] - Refreshed command list. Found {availableCommands.Count} new commands.");
    }
    
}
