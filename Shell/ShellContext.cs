
using NihilistShell.Themes;
using Spectre.Console;

namespace NihilistShell.Shell;

/// <summary>
/// The <c>ShellContext</c> class manages the shell environment, including environment variables,
/// current directory, and the selected theme. It provides methods to expand variables, set,
/// unset, and retrieve environment variables, as well as change the theme and get the
/// corresponding shell prompt.
/// </summary>
public class ShellContext
{
    private ThemesEnum CurrentTheme = ThemesEnum.Default;
    public Dictionary<string, string> EnvVars { get; set; } = new();
    public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();

    /// <summary>
    /// Expands environment variables in the provided input string by replacing
    /// variables of the form $VAR with their values from the EnvVars dictionary.
    /// </summary>
    /// <param name="input">The string containing variables to expand.</param>
    /// <returns>The input string with environment variables expanded.</returns>
    public string ExpandVariables(string input)
    {
        foreach (var kv in EnvVars)
        {
            input = input.Replace($"${kv.Key}", kv.Value);
        }
        return input;
    }

    /// <summary>
    /// Sets the value of an environment variable.
    /// </summary>
    /// <param name="key">The name of the variable.</param>
    /// <param name="value">The value to set for the variable.</param>
    public void SetVar(string key, string value)
    {
        EnvVars[key] = value;
    }

    /// <summary>
    /// Unsets (removes) an environment variable.
    /// </summary>
    /// <param name="key">The name of the variable to remove.</param>
    public void UnsetVar(string key)
    {
        EnvVars.Remove(key);
    }

    /// <summary>
    /// Retrieves the value of an environment variable.
    /// </summary>
    /// <param name="key">The name of the variable to retrieve.</param>
    /// <returns>The value of the variable, or null if the variable is not set.</returns>
    public string? GetVar(string key)
    {
        return EnvVars.TryGetValue(key, out var val) ? val : null;
    }

    /// <summary>
    /// Sets the theme for the shell based on the provided theme name.
    /// If the theme is valid, it is applied. Otherwise, list all the available themes.
    /// </summary>
    /// <param name="theme">The name of the theme to set.</param>
    public void SetTheme(string theme)
    {
        string[] allThemes = ThemeLoader.GetAllThemes();
        if (ThemeLoader.TryGetTheme(theme, out var selectedTheme))
        {
            CurrentTheme = selectedTheme;
            AnsiConsole.MarkupLine($"[[[green]+[/]]] Thème défini sur : [yellow]{selectedTheme}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[[[red]-[/]]] Thème inconnu : '[yellow]{theme}[/]'");
            AnsiConsole.MarkupLine("[[[blue]*[/]]] Available themes :");
            for (int i = 0; allThemes.Length < i; i++)
            {
                AnsiConsole.MarkupLine($"\t- {allThemes[i]}");
            }
        }
    }

    /// <summary>
    /// Gets the current prompt for the shell based on the selected theme and current directory.
    /// </summary>
    /// <returns>The formatted prompt string for the shell.</returns>
    public string GetPrompt()
    {
        return ThemeLoader.ToStringValue(CurrentTheme, CurrentDirectory);
    }
}
