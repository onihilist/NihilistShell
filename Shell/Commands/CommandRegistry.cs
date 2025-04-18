
using NeonShell.Commands;

namespace NeonShell.Shell;

/// <summary>
/// <c>CommandRegistry</c> is responsible for registering all custom commands used in the shell.
/// </summary>
public static class CommandRegistry
{
    /// <summary>
    /// Retrieves all registered custom commands implemented in the shell.
    /// </summary>
    /// <returns>An enumeration of <see cref="ICustomCommand"/> instances.</returns>
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            new CdCommand(),
            new SetTheme(),
        };
    }
}