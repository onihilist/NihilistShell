
using NeonShell.Shell;

namespace NihilistShell.Themes
{
    public enum ThemesEnum
    {
        Default,
        Light
    }

    public static class ThemeLoader
    {
        private static readonly Dictionary<string, ThemesEnum> _stringToEnum = new()
        {
            { "default", ThemesEnum.Default },
            { "light", ThemesEnum.Light }
        };

        public static bool TryGetTheme(string name, out ThemesEnum theme)
        {
            return _stringToEnum.TryGetValue(name.ToLower(), out theme);
        }

        public static string ToStringValue(ThemesEnum theme, string currentDirectory)
        {
            return theme switch
            {
                ThemesEnum.Default =>
                    $"[white]\u250c[/][bold green][[nihilist-shell@core]][/]\n[white]\u2514[/][blue][[{currentDirectory}]][/] >> ",
                ThemesEnum.Light =>
                    $"[white]\u250c[[nihilist-shell@core]]\n\u2514[[{currentDirectory}]][/] >> ",
                _ => "[-] - Unknown theme"
            };
        }
    }

}
