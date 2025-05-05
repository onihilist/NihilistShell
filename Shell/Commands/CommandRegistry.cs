
using Spectre.Console;
using NShell.Commands;

namespace NShell.Shell.Commands;

/// <summary>
/// <c>CommandRegistry</c> is responsible for registering all custom commands used in the shell.
/// </summary>
public static class CommandRegistry
{
    
    private static readonly Dictionary<string, ICustomCommand> _commands = new();
    
    /// <summary>
    /// Retrieves all registered custom commands implemented in the shell.
    /// </summary>
    /// <returns>An enumeration of <see cref="ICustomCommand"/> instances.</returns>
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            new CdCommand(),
            new SetThemeCommand(),
        };
    }
    
    /// <summary>
    /// Register a new plugin command into the shell.
    /// This method adds the command to the command list and prints a confirmation.
    /// </summary>
    /// <param name="command">The plugin command implementing ICustomCommand to be registered.</param>
    public static void Register(ICustomCommand command)
    {
        if (!_commands.ContainsKey(command.Name))
        {
            _commands.Add(command.Name, command);
            AnsiConsole.MarkupLine($"\t[[[green]+[/]]] - Registered plugin command: [yellow]{command.Name}[/]");
        }
    }


}