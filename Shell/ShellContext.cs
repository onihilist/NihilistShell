using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using NShell.Shell.Themes;
using Spectre.Console;

namespace NShell.Shell
{
    public class ShellContext
    {
        public string CurrentDirectory { get; set; }
        public string CurrentTheme { get; set; } = "default";
        public string Prompt { get; private set; }
        public string LSColors { get; private set; }

        public ShellContext()
        {
            CurrentDirectory = Directory.GetCurrentDirectory();
            SetPromptAndColors("default");
        }

        public void SetPromptAndColors(string themeName)
        {
            string[] themeData = GetThemePrompt(themeName, CurrentDirectory);
            Prompt = themeData[0];
            LSColors = themeData[1];
        }

        public string GetPrompt()
        {
            return Prompt;
        }

        public string GetLsColors()
        {
            return LSColors;
        }

        public void ChangeDirectory(string newDir)
        {
            if (Directory.Exists(newDir))
            {
                CurrentDirectory = newDir;
                SetPromptAndColors("default");
            }
        }

        private string[] GetThemePrompt(string themeName, string currentDirectory)
        {
            if (ThemeLoader.TryGetTheme(themeName, out var defaultTheme))
            {
                return ThemeLoader.ToStringValue(defaultTheme, currentDirectory);
            }
            
            string themeDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nshell",
                "themes"
            );

            if (!Directory.Exists(themeDir))
                return new[] { "[[[red]-[/]]] - Theme directory not found." };

            foreach (var file in Directory.EnumerateFiles(themeDir, "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    JsonNode? data = JsonNode.Parse(json);

                    if (data is not null && 
                        data["name"]?.ToString().Equals(themeName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return ThemeLoader.LoadCustomTheme(Path.GetFileNameWithoutExtension(file), currentDirectory);
                    }
                }
                catch
                {
                    AnsiConsole.MarkupLine($"[[[yellow]*[/]]] - File '{file}' contains errors, impossible to parse/read.[/]");
                }
            }

            return new[] { "[[[red]-[/]]] - Theme not found." };
        }
        
        public string ExpandVariables(string input)
        {
            return input
                .Replace("{user}", Environment.UserName)
                .Replace("{host}", Environment.MachineName)
                .Replace("{cwd}", Directory.GetCurrentDirectory());
        }
        
        public bool SetTheme(string themeName)
        {
            string[] themeData;
            
            if (ThemeLoader.TryGetTheme(themeName, out var themeEnum))
            {
                CurrentTheme = themeName;
                themeData = ThemeLoader.ToStringValue(themeEnum, CurrentDirectory);
            }
            else
            {
                CurrentTheme = themeName;
                themeData = ThemeLoader.LoadCustomTheme(themeName, CurrentDirectory);
            }
            
            if (!themeData[0].Contains("invalid", StringComparison.OrdinalIgnoreCase) && !themeData[0].Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                Prompt = themeData[0];
                LSColors = themeData[1];
            }
            else
            {
                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Theme not found or invalid.");
                Prompt = $"[white]\u250c[/][bold green][[{Environment.UserName}@{Environment.MachineName}]][/]\n[white]\u2514[/][blue][[{CurrentDirectory}]][/] >> ";
                LSColors = "di=34:fi=37:ln=36:pi=33:so=35:ex=32";
                return false;
            }
            return true;
        }
    }
}
