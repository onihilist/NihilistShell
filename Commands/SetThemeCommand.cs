
using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands
{
    public class SetThemeCommand : ICustomCommand, IMetadataCommand
    {
        public string Name => "settheme";
        public string Description => "Set the CLI theme.";
        public bool IsInteractive => false;
        public bool ShouldFallback => false;

        public void Execute(ShellContext context, string[] args)
        {
            if (args.Length == 0 || args.Length > 1)
            {
                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: settheme <theme>");
                return;
            }

            string themeName = args[0];
            bool res = context.SetTheme(themeName);

            if(res){AnsiConsole.MarkupLine($"[[[green]+[/]]] - Theme set to: {themeName}");}

        }
    }
}