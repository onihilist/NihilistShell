namespace NeonShell.Shell;

public class ShellContext
{
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
}