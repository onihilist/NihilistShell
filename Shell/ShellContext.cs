
using NihilistShell.Themes;
using Spectre.Console;

namespace NeonShell.Shell;

public class ShellContext
{
    private ThemesEnum CurrentTheme = ThemesEnum.Default;
    public Dictionary<string, string> EnvVars { get; set; } = new();
    public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();

    public string ExpandVariables(string input)
    {
        foreach (var kv in EnvVars)
        {
            input = input.Replace($"${kv.Key}", kv.Value);
        }
        return input;
    }

    public void SetVar(string key, string value)
    {
        EnvVars[key] = value;
    }

    public void UnsetVar(string key)
    {
        EnvVars.Remove(key);
    }

    public string? GetVar(string key)
    {
        return EnvVars.TryGetValue(key, out var val) ? val : null;
    }
    
    public void SetTheme(string theme)
    {
        if (ThemeLoader.TryGetTheme(theme, out var selectedTheme))
        {
            CurrentTheme = selectedTheme;
            AnsiConsole.MarkupLine($"[[[green]+[/]]] Thème défini sur : [yellow]{selectedTheme}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[[[red]-[/]]] Thème inconnu : '[yellow]{theme}[/]'");
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] Setting default theme...");
            CurrentTheme = ThemesEnum.Default;
            AnsiConsole.MarkupLine("[[[green]+[/]]] Default theme has been set");
        }
    }


    public string GetPrompt()
    {
        return ThemeLoader.ToStringValue(CurrentTheme, CurrentDirectory);
    }

}