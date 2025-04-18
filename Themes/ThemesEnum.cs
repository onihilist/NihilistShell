
namespace NihilistShell.Themes
{
    /// <summary>
    /// Enum representing the different themes available for the shell.
    /// </summary>
    public enum ThemesEnum
    {
        Default,
        Light
    }

    /// <summary>
    /// <c>ThemeLoader</c> is responsible for loading and managing themes in the shell.
    /// </summary>
    public static class ThemeLoader
    {
        private static readonly Dictionary<string, ThemesEnum> _stringToEnum = new()
        {
            { "default", ThemesEnum.Default },
            { "light", ThemesEnum.Light }
        };

        /// <summary>
        /// Tries to get a theme from the provided name.
        /// </summary>
        /// <param name="name">The name of the theme to retrieve (case-insensitive).</param>
        /// <param name="theme">The resulting theme if found; otherwise, the default theme.</param>
        /// <returns>True if the theme was found and successfully parsed; otherwise, false.</returns>
        public static bool TryGetTheme(string name, out ThemesEnum theme)
        {
            return _stringToEnum.TryGetValue(name.ToLower(), out theme);
        }

        /// <summary>
        /// Converts a ThemesEnum value into a string representation suitable for the shell prompt.
        /// This method is responsible for generating the appropriate theme-specific style.
        /// </summary>
        /// <param name="theme">The theme to convert.</param>
        /// <param name="currentDirectory">The current directory to display in the prompt.</param>
        /// <returns>A string representing the theme-specific shell prompt with the current directory.</returns>
        public static string[] ToStringValue(ThemesEnum theme, string currentDirectory)
        {
            return theme switch
            {
                ThemesEnum.Default =>
                    new string[] {
                        "[white]\u250c[/][bold green][[nihilist-shell@core]][/]\n[white]\u2514[/][blue][[{currentDirectory}]][/] >> ",
                        "di=34:fi=37:ln=36:pi=33:so=35:ex=32"
                    },
                ThemesEnum.Light =>
                    new string[] {
                        "[white]\u250c[[nihilist-shell@core]]\n\u2514[[{currentDirectory}]][/] >> "
                    },
                _ => new string[] { "[[-]] - Unknown theme" }
            };
        }


        public static string[] GetAllThemes()
        {
            Dictionary<string, ThemesEnum> themes = _stringToEnum;
            return themes.Keys.ToArray();
        }
        
    }
}
