

using NeonShell.Shell;
using Spectre.Console;

namespace NeonShell.Commands;

public class TestCommand
{

    public class EchoCommand : ICustomCommand
    {
        public string Name => "testabc";

        public void Execute(ShellContext context, string[] args)
        {
            AnsiConsole.MarkupLine("Salut c'est un test du loader de commands");
        }
    }

}