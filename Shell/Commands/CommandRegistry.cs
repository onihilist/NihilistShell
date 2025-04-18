using System.Collections.Generic;
using NeonShell.Commands;

namespace NeonShell.Shell;

public static class CommandRegistry
{
    public static IEnumerable<ICustomCommand> GetAll()
    {
        return new List<ICustomCommand>
        {
            // Custom commands
            new CdCommand(),
            new SetTheme(),
            
            // Commands Wrappers
            new SudoWrapper(),
            new AptWrapper(),
            new EchoWrapper(),
            new NanoWrapper(),
            new HistoryWrapper(),
        };
    }
}