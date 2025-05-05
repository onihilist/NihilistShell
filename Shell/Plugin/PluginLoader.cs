
using Spectre.Console;
using System.Reflection;
using NShell.Shell.Commands;

namespace NShell.Shell.Plugins;

/// <summary>
/// <c>PluginLoader</c> manage all about loading, parse, execute plugins.
/// </summary>
public class PluginLoader
{
    private string PluginFolderPath { get; set; } = $"/home/{Environment.UserName}/.nshell/plugins";
    private readonly List<Assembly> _loadedPlugins = new();

    public string[] PluginList { get; private set; } = [];
    public int NumberOfPlugins { get; private set; }

    public static PluginLoader Instance { get; private set; } = new();

    /// <summary>
    /// Load all plugins into <c>~/.nshell/plugins</c> folder.
    /// </summary>
    public void LoadPlugins()
    {
        if (!Directory.Exists(PluginFolderPath))
        {
            AnsiConsole.MarkupLine($"\t[[[red]-[/]]] - Plugin directory doesn't exist.");
            AnsiConsole.MarkupLine($"\t[[[yellow]*[/]]] - Creating plugins directory.");
            Directory.CreateDirectory(PluginFolderPath);
        }

        PluginList = Directory.GetFiles(PluginFolderPath, "*.dll", SearchOption.AllDirectories);

        if (PluginList.Length > 0)
        {
            foreach (var pluginPath in PluginList)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(pluginPath);
                    _loadedPlugins.Add(assembly);
                    AnsiConsole.MarkupLine($"\t[[[green]+[/]]] - Loading plugin: [yellow]{pluginPath}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"\t[[[red]-[/]]] - Failed to load plugin: {pluginPath} - {ex.Message}");
                }
            }

            NumberOfPlugins = _loadedPlugins.Count;
            AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold green]{NumberOfPlugins}[/]");
            
            var pluginCommands = Instance.GetPluginCommands();
            foreach (var cmd in pluginCommands)
            {
                CommandRegistry.Register(cmd);
            }
        }
        else
        {
            NumberOfPlugins = 0;
            AnsiConsole.MarkupLine($"\t[[[yellow]*[/]]] - No plugins found.");
            AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold yellow]0[/]");
        }
    }
    
    /// <summary>
    /// Extracts and returns all plugin commands that implement <c>ICustomCommand</c>.
    /// </summary>
    public List<ICustomCommand> GetPluginCommands()
    {
        var commands = new List<ICustomCommand>();

        foreach (var plugin in _loadedPlugins)
        {
            var types = plugin.GetTypes()
                .Where(t => typeof(ICustomCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is ICustomCommand command)
                {
                    commands.Add(command);
                    CommandRegistry.Register(command);
                    AnsiConsole.MarkupLine($"[green][+] - Loaded plugin command: {command.Name}[/]");
                }
            }
        }

        return commands;
    }
}
