using System.Collections.Generic;
using NeonShell.Commands;

namespace NeonShell.Shell;

public static class CommandRegistry
{
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            new CdCommand(),
            new SetTheme(),
        };
    }
}