
using System.Text.Json.Nodes;

namespace NShell.Shell.Config;

public class ShellConfig
{
    public int HistoryExpirationTime { get; set; }
    public int HistoryMaxStorage { get; set; }
    public string SelectedTheme { get; set; }

    public static ShellConfig? LoadConfig()
    {
        var configFile = Directory.GetFiles($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.nshell", "nshell.conf.json");

        foreach (var filePath in configFile)
        {
            string json = File.ReadAllText(filePath);
            JsonNode? data = JsonNode.Parse(json);
            JsonNode? historyNode = data?["configuration"]?["nshell"]?["history"]?[0];
            JsonNode? themeNode = data?["configuration"]?["nshell"]?["theme"]?[0];

            if (historyNode != null && themeNode != null)
            {
                string expiration = historyNode["expiration_time"]?.ToString() ?? "0d";
                int days = ParseExpirationTime(expiration);

                return new ShellConfig
                {
                    HistoryExpirationTime = days,
                    HistoryMaxStorage = historyNode["max_storage"]?.GetValue<int>() ?? 0,
                    SelectedTheme = themeNode["selected_theme"]?.ToString() ?? "default"
                };
            }
        }

        return null;
    }

    private static int ParseExpirationTime(string expiration)
    {
        if (expiration.EndsWith("w") && int.TryParse(expiration[..^1], out int weeks))
        {
            return weeks*168;
        } if (expiration.EndsWith("d") && int.TryParse(expiration[..^1], out int days))
        {
            return days*24;
        } if (expiration.EndsWith("h") && int.TryParse(expiration[..^1], out int hours))
        {
            return hours;
        }
        return 0;
    }
}