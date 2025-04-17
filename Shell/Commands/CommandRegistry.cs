using System.Collections.Generic;
using NeonShell.Commands;

namespace NeonShell.Shell;

public static class CommandRegistry
{
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            new Commands.CdCommand(),
            new Commands.NanoWrapper(),
            new Commands.HistoryWrapper(),
            new Commands.EchoCommand()
        };
    }
}