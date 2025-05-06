namespace NShell.Shell.Config;

public class ShellConfig
{
    public int HistoryExpirationTime { get; set; }
    public int HistoryMaxStorage { get; set; }

    public static void CheckHistoryConfiguration()
    {
        
    }
}