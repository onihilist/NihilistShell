using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NihilistShell.Shell.Themes
{
    /// <summary>
    /// Enum representing the available default themes.
    /// </summary>
    public enum DefaultThemesEnum
    {
        Default,
        Light
    }

    /// <summary>
    /// Manages loading and retrieving custom or predefined themes.
    /// </summary>
    public static class ThemeLoader
    {
        private static readonly Dictionary<string, DefaultThemesEnum> _stringToEnum = new()
        {
            { "default", DefaultThemesEnum.Default },
            { "light", DefaultThemesEnum.Light }
        };

        /// <summary>
        /// Tries to get a default theme from a name.
        /// </summary>
        /// <param name="name">The theme name (case-insensitive).</param>
        /// <param name="theme">The resulting theme if found.</param>
        /// <returns>True if the theme was found, false otherwise.</returns>
        public static bool TryGetTheme(string name, out DefaultThemesEnum theme)
        {
            return _stringToEnum.TryGetValue(name.ToLower(), out theme);
        }

        /// <summary>
        /// Converts a predefined theme to a formatted string for the prompt.
        /// </summary>
        /// <param name="theme">The theme to apply.</param>
        /// <param name="currentDirectory">The current directory to display in the prompt.</param>
        /// <returns>Prompt string and associated LS_COLORS.</returns>
        public static string[] ToStringValue(DefaultThemesEnum theme, string currentDirectory)
        {
            return theme switch
            {
                DefaultThemesEnum.Default =>
                    new string[] {
                        $"[white]\u250c[/][bold green][[{Environment.UserName}@{Environment.MachineName}]][/]\n[white]\u2514[/][blue][[{currentDirectory}]][/] >> ",
                        "di=34:fi=37:ln=36:pi=33:so=35:ex=32"
                    },
                DefaultThemesEnum.Light =>
                    new string[] {
                        $"[white]\u250c[[{Environment.UserName}@{Environment.MachineName}]]\n\u2514[[{currentDirectory}]][/] >> ",
                        "di=37:fi=30:ln=36:pi=33:so=35:ex=32"
                    },
                _ => new string[] { "[[-]] - Unknown theme" }
            };
        }

        /// <summary>
        /// Loads a custom theme from the <c>~/.nihilist_shell/themes/</c> directory, 
        /// based on the name found in the theme's JSON file.
        /// If the directory doesn't exist, it will be created.
        /// </summary>
        /// <param name="themeName">The name of the theme (as defined by the "name" field in the JSON file).</param>
        /// <param name="currentDirectory">The current directory to display in the prompt.</param>
        /// <returns>Prompt string and LS_COLORS if theme found, error message otherwise.</returns>
        public static string[] LoadCustomTheme(string themeName, string currentDirectory)
        {
            string themesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nihilist_shell", "themes");
            
            if (!Directory.Exists(themesDirectory))
            {
                try
                {
                    Directory.CreateDirectory(themesDirectory);
                }
                catch (Exception ex)
                {
                    return new[] { $"[[[red]-[/]]] - Could not create themes directory: {ex.Message}" };
                }
            }
            
            var themeFiles = Directory.GetFiles(themesDirectory, "*.json");
            
            foreach (var filePath in themeFiles)
            {
                string json = File.ReadAllText(filePath);
                JsonNode? data = JsonNode.Parse(json);
                
                if (data?["name"]?.ToString().Equals(themeName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    string? format = data["format"]?.ToString();
                    string? formatTop = data["format_top"]?.ToString();
                    string? formatBottom = data["format_bottom"]?.ToString();
                    string? cornerTop = data["corner_top"]?.ToString();
                    string? cornerBottom = data["corner_bottom"]?.ToString();
                    string? colors = data["ls_colors"]?.ToString() ?? "di=34:fi=37:ln=36:pi=33:so=35:ex=32";

                    if (cornerTop != null && cornerBottom != null && formatTop != null && formatBottom != null)
                    {
                        string prompt = $"{cornerTop}{formatTop.Replace("{user}", Environment.UserName).Replace("{host}", Environment.MachineName)}\n" +
                                        $"{cornerBottom}{formatBottom.Replace("{cwd}", currentDirectory)} >> ";
                        return new[] { prompt, colors };
                    }

                    if (format != null)
                    {
                        string prompt = format
                            .Replace("{user}", Environment.UserName)
                            .Replace("{host}", Environment.MachineName)
                            .Replace("{cwd}", currentDirectory);
                        return new[] { prompt, colors };
                    }
                }
            }
            return new[] { "[[[red]-[/]]] - Theme not found." };
        }


        /// <summary>
        /// Returns all available default theme names.
        /// </summary>
        /// <returns>An array of strings representing the theme names.</returns>
        public static string[] GetAllThemes()
        {
            return _stringToEnum.Keys.ToArray();
        }

        /// <summary>
        /// Tries to load a custom JSON theme and returns true if successful.
        /// </summary>
        /// <param name="themeName">The theme file name without the .json extension.</param>
        /// <param name="result">Array containing the prompt and LS_COLORS if successful.</param>
        /// <returns>True if the theme was loaded successfully, false otherwise.</returns>
        public static bool TryLoadJsonTheme(string themeName, out string[] result)
        {
            result = LoadCustomTheme(themeName, Directory.GetCurrentDirectory());
            return !result[0].Contains("invalid", StringComparison.OrdinalIgnoreCase) &&
                   !result[0].Contains("not found", StringComparison.OrdinalIgnoreCase);
        }
    }
}
