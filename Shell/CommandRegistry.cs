using System.Collections.Generic;

namespace NeonShell.Shell;

public static class CommandRegistry
{
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            new Commands.TestCommand(),
            new Commands.RealTestCommand()
        };
    }
}