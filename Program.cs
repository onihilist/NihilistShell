
using Spectre.Console;
using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.History;
using NShell.Shell.Keyboard;
using NShell.Shell.Plugins;
using static NShell.Animation.GlitchOutput;

public class Program
{
    public static readonly string VERSION = "v0.2.1";
    public static readonly string GITHUB = "https://github.com/onihilist/NShell";

    public static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "--version":
                case "-v":
                    Console.WriteLine($"NShell {VERSION}");
                    return;
                case "--help":
                case "-h":
                    Console.WriteLine("Usage: nshell [--version | --help]");
                    return;
            }
        }

        AnsiConsole.Clear();
        AnsiConsole.Markup($"Welcome {Environment.UserName} to NShell !\n\n");
        AnsiConsole.Markup($"\tversion : {VERSION}\n");
        AnsiConsole.Markup($"\tgithub  : {GITHUB}\n");
        AnsiConsole.Markup("\t\n");

        AnsiConsole.Markup("[bold cyan][[*]] - Booting NShell...[/]\n");
        ShellContext context = new();
        HistoryManager history = new();
        PluginLoader plugins = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading command(s)...[/]\n");
        CommandParser parser = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading plugin(s)...[/]\n");
        plugins.LoadPlugins();

        AppDomain.CurrentDomain.ProcessExit += (_, _) => {
            history.Save();
        };

        await GlitchedPrint("[+] - System Online", TimeSpan.FromMilliseconds(20));
        Console.WriteLine();

        KeyboardHandler.Handler(history, context, parser);
        
    }
}
