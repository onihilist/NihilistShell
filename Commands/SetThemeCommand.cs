using NihilistShell.Shell;
using Spectre.Console;

namespace NihilistShell.Commands
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
                AnsiConsole.MarkupLine("[red][[*]] - Usage: settheme <theme>[/]");
                return;
            }

            string themeName = args[0];

            // Essaye de changer le thème dans le contexte
            context.SetTheme(themeName);

            // Affiche un message de confirmation
            AnsiConsole.MarkupLine($"[green]Theme set to: {themeName}[/]");
        }
    }
}