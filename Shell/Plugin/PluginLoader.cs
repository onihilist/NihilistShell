
using Spectre.Console;

namespace NihilistShell.Shell.Plugins
{

    /// <summary>
    /// <c>PluginLoader</c> manage all about loading, parse, execute plugins.
    /// </summary>
    public class PluginLoader
    {
        private string   PluginFolderPath { get; set; } = $"/home/{Environment.UserName}/.nihilist_shell/plugins";
        public  string[] PluginList { get; set; }
        public  int      NumberOfPlugins { get; set; }
        
        public static PluginLoader Instance { get; private set; } = null!;

        /// <summary>
        /// Load all plugins into <c>~/.nihilist_shell/plugins</c> folder.
        /// </summary>
        public void LoadPlugins()
        {
            if (Path.Exists(PluginFolderPath))
            {
                var PluginList = Directory.GetFiles(
                    PluginFolderPath, 
                    "*.dll",
                    SearchOption.AllDirectories);

                if (PluginList.Length > 0)
                {
                    NumberOfPlugins = PluginList.Length;
                    foreach (var plugin in PluginList)
                    {
                        AnsiConsole.MarkupLine($"\t[[[green]+[/]]] - Loading plugin : [yellow]{plugin}[/].");
                    }
                    AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold green]{NumberOfPlugins}[/]");
                }
                else
                {
                    NumberOfPlugins = 0;
                    AnsiConsole.MarkupLine($"\t[[[yellow]*[/]]] - No plugins found.");
                    AnsiConsole.MarkupLine($"[bold grey]→ Total plugins loaded:[/] [bold yellow]{NumberOfPlugins}[/]");
                }
            }
            else
            {
                PluginList = [];
                AnsiConsole.MarkupLine($"\t[[[red]-[/]]] - Plugin directory doesn't exist.");
                AnsiConsole.MarkupLine($"\t[[[yellow]*[/]]] - Creating plugins directory.");
                Directory.CreateDirectory(PluginFolderPath);
            }
        }
        
    }
    
}
