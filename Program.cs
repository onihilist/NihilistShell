
using Spectre.Console;
using NihilistShell.Shell;
using NihilistShell.Shell.History;
using NihilistShell.Shell.Plugins;
using static NihilistShell.Animation.GlitchOutput;

public class Program
{
    public static readonly string VERSION = "v0.2.1";
    public static readonly string GITHUB = "https://github.com/onihilist/NihilistShell";

    public static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "--version":
                case "-v":
                    Console.WriteLine($"NihilistShell {VERSION}");
                    return;
                case "--help":
                case "-h":
                    Console.WriteLine("Usage: nihilistshell [--version | --help]");
                    return;
            }
        }

        AnsiConsole.Clear();
        AnsiConsole.Markup($"Welcome {Environment.UserName} to NihilistShell !\n\n");
        AnsiConsole.Markup($"\tversion : {VERSION}\n");
        AnsiConsole.Markup($"\tgithub  : {GITHUB}\n");
        AnsiConsole.Markup("\t\n");

        AnsiConsole.Markup("[bold cyan][[*]] - Booting NihilistShell...[/]\n");
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

        ConsoleKeyInfo key;
        string inputBuffer = "";
        history.ResetIndex();

        while (true)
        {
            Environment.SetEnvironmentVariable("LS_COLORS", context.GetLsColors());
            AnsiConsole.Markup(context.GetPrompt());
            inputBuffer = "";
            history.ResetIndex();

            while (true)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace && inputBuffer.Length > 0)
                {
                    inputBuffer = inputBuffer[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    var prev = history.GetPrevious();
                    if (prev != null)
                    {
                        Console.Write(new string('\b', inputBuffer.Length) + new string(' ', inputBuffer.Length) + new string('\b', inputBuffer.Length));
                        inputBuffer = prev;
                        Console.Write(inputBuffer);
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    var next = history.GetNext();
                    Console.Write(new string('\b', inputBuffer.Length) + new string(' ', inputBuffer.Length) + new string('\b', inputBuffer.Length));
                    inputBuffer = next ?? "";
                    Console.Write(inputBuffer);
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    if (inputBuffer.StartsWith("cd "))
                    {
                        string path = inputBuffer.Substring(3);
                        string currentDir = Directory.GetCurrentDirectory();
                        string fullPath = Path.Combine(currentDir, path);

                        if (Directory.Exists(fullPath))
                        {
                            inputBuffer += Path.DirectorySeparatorChar;

                            var directories = Directory.GetDirectories(fullPath);

                            if (directories.Length > 0)
                            {
                                string directoryToSuggest = directories[0];
                                Console.Write(new string('\b', inputBuffer.Length) + new string(' ', inputBuffer.Length) + new string('\b', inputBuffer.Length));
                                inputBuffer = "cd " + directoryToSuggest;
                                Console.Write(inputBuffer);
                            }
                            else
                            {
                                Console.Write(new string('\b', inputBuffer.Length) + new string(' ', inputBuffer.Length) + new string('\b', inputBuffer.Length));
                                Console.Write(inputBuffer);
                            }
                        }
                        else
                        {
                            Console.Write(new string('\b', inputBuffer.Length) + new string(' ', inputBuffer.Length) + new string('\b', inputBuffer.Length));
                            Console.Write(inputBuffer);
                        }
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    inputBuffer += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            if (string.IsNullOrWhiteSpace(inputBuffer)) continue;

            history.Add(inputBuffer);

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
