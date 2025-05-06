using Spectre.Console;
using System.Reflection;
using NShell.Shell.Commands;

namespace NShell.Shell.Plugins;

/// <summary>
/// Manages loading and registration of plugins implementing <c>ICustomCommand</c>.
/// </summary>
public class PluginLoader
{
    private readonly string PluginFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nshell", "plugins");
    private readonly List<Assembly> _loadedPlugins = new();

    public string[] PluginList { get; private set; } = [];
    public int NumberOfPlugins { get; private set; }

    public static PluginLoader Instance { get; private set; } = new();

    public void LoadPlugins()
    {
        if (!Directory.Exists(PluginFolderPath))
        {
            AnsiConsole.MarkupLine("\t[[[red]-[/]]] - Plugin directory doesn't exist.");
            AnsiConsole.MarkupLine("\t[[[yellow]*[/]]] - Creating plugins directory.");
            Directory.CreateDirectory(PluginFolderPath);
        }

        PluginList = Directory.GetFiles(PluginFolderPath, "*.dll", SearchOption.AllDirectories);

        if (PluginList.Length > 0)
        {
            foreach (var pluginPath in PluginList)
            {
                try
                {
                    var context = new PluginLoadContext(pluginPath);
                    var assembly = context.LoadFromAssemblyPath(pluginPath);

                    _loadedPlugins.Add(assembly);
                    AnsiConsole.MarkupLine($"\t[[[green]+[/]]] - Loading plugin: [yellow]{Path.GetFileName(pluginPath)}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"\t[[[red]-[/]]] - Failed to load plugin: {Path.GetFileName(pluginPath)}");
                    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
                }
            }

            NumberOfPlugins = _loadedPlugins.Count;
            AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold green]{NumberOfPlugins}[/]");

            LoadPluginCommands();
        }
        else
        {
            NumberOfPlugins = 0;
            AnsiConsole.MarkupLine("\t[[[yellow]*[/]]] - No plugins found.");
            AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold yellow]0[/]");
        }
    }

    /// <summary>
    /// Extracts and registers all valid types implementing <c>ICustomCommand</c>.
    /// </summary>
    private void LoadPluginCommands()
    {
        foreach (var plugin in _loadedPlugins)
        {
            try
            {
                var commandTypes = plugin.GetTypes()
                    .Where(t => typeof(ICustomCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in commandTypes)
                {
                    if (Activator.CreateInstance(type) is ICustomCommand command)
                    {
                        CommandRegistry.Register(command);
                        AnsiConsole.MarkupLine($"[[[green]+[/]]] - Loaded plugin command: [yellow]{command.Name}[/]");
                    }
                }
            }
            catch (ReflectionTypeLoadException rtle)
            {
                foreach (var loaderEx in rtle.LoaderExceptions)
                {
                    AnsiConsole.MarkupLine($"[[[red]-[/]]] - Error loading plugin type: [red]{loaderEx?.Message}[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Error processing plugin: [red]{ex.Message}[/]");
            }
        }
    }
}
