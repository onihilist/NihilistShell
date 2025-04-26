
using Spectre.Console;
using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.Plugins;
using NShell.Shell.Readline;
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
        PluginLoader plugins = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading command(s)...[/]\n");
        CommandParser parser = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading plugin(s)...[/]\n");
        plugins.LoadPlugins();

        AppDomain.CurrentDomain.ProcessExit += (_, _) => {
            ReadLine.History.Save();
        };

        await GlitchedPrint("[+] - System Online", TimeSpan.FromMilliseconds(20));
        string inputBuffer;

        while (true)
        {
            Environment.SetEnvironmentVariable("LS_COLORS", context.GetLsColors());
            context.SetTheme(context.CurrentTheme);
            AnsiConsole.Markup(context.GetPrompt());
            ReadLine.History.ResetIndex();
            ReadLine.Handler.UpdateInitCursorPos(Console.CursorLeft);

            inputBuffer = ReadLine.GetText();

            if (string.IsNullOrWhiteSpace(inputBuffer)) continue;
            ReadLine.History.Add(inputBuffer);

            try
            {
                parser.TryExecute(inputBuffer, context);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Shell crash: [yellow]{ex.Message}[/]");
            }
        }
    }
}
