
using Spectre.Console;
using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.History;
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

        ConsoleKeyInfo key;
        ConsoleKeyInfo promptKey;
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
                    } else if (inputBuffer.Length > 0) {
                        HashSet<string> suggestCommands = new();
                        HashSet<string> allCommands = CommandParser.CustomCommands.Keys
                            .Concat(CommandParser.SystemCommands)
                            .ToHashSet();

                        foreach (var suggestion in allCommands)
                        {
                            if (suggestion.StartsWith(inputBuffer))
                            {
                                suggestCommands.Add(suggestion);
                            }
                        }

                        if (suggestCommands.Count > 15)
                        {
                            bool showFullList = false;
                            AnsiConsole.MarkupLine($"[[[yellow]*[/]]] - Do you want to list all [yellow]{suggestCommands.Count}[/] commands ? ([yellow]y[/]/[yellow]n[/]) >> ");
                            while (true)
                            {
                                promptKey = Console.ReadKey(intercept: true);
                                if (promptKey.Key == ConsoleKey.Y || promptKey.Key == ConsoleKey.N)
                                {
                                    if (promptKey.Key == ConsoleKey.Y)
                                    {
                                        showFullList = true;
                                    }
                                    break;
                                }
                            }
                            if (showFullList)
                            {
                                var sortedCommands = suggestCommands.OrderBy(c => c.Length).ToArray();
                                int i = 0;

                                foreach (var cmd in sortedCommands)
                                {
                                    if (i == 7)
                                    {
                                        AnsiConsole.MarkupLine("");
                                        i = 0;
                                    }
                                    AnsiConsole.Markup($"{cmd}  ");
                                    i++;
                                }

                                inputBuffer = string.Empty;
                                AnsiConsole.MarkupLine("");
                                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Press enter to continue...");
                            }
                        }
                        else
                        {
                            var sortedCommands = suggestCommands.OrderBy(c => c.Length).ToArray();
                            List<string> commandsList = sortedCommands.ToList();

                            int currentIndex = 0;
                            
                            if (!string.IsNullOrEmpty(inputBuffer))
                            {
                                for (int i = 0; i < commandsList.Count; i++)
                                {
                                    if (commandsList[i].StartsWith(inputBuffer))
                                    {
                                        currentIndex = (i + 1) % commandsList.Count;
                                        inputBuffer = commandsList[currentIndex];
                                        break;
                                    }
                                }
                            }
                            
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, Console.CursorTop);
                            
                            AnsiConsole.Markup(context.GetPrompt());
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
