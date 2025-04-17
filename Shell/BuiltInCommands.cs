using Spectre.Console;

namespace NeonShell.Shell;

public static class BuiltInCommands
{
    public static void NeonScan(ShellContext ctx)
    {
        AnsiConsole.Markup("[yellow]Initiating deep scan...[/]\n");
        Thread.Sleep(800);
        AnsiConsole.Markup("[green]✔ Ports scanned[/]\n");
        AnsiConsole.Markup("[green]✔ Firewalls bypassed[/]\n");
        AnsiConsole.Markup("[green]✔ Root access simulated[/]\n");
    }

    public static void NeonAI(ShellContext ctx)
    {
        AnsiConsole.Markup("[blue]Launching AI protocol...[/]\n");
        Thread.Sleep(700);
        AnsiConsole.Markup("[purple]ZEON:[/] 'What do you require, Operator?'\n");
    }

    public static void NeonNet(ShellContext ctx)
    {
        AnsiConsole.Markup("[cyan]Accessing darknet nodes...[/]\n");
        Thread.Sleep(600);
        AnsiConsole.Write(new Panel("🐍 Node 01: ACTIVE\n🦾 Node 02: ENCRYPTED\n👁 Node 03: MONITORED")
            .Header("NetStatus")
            .BorderColor(Color.Fuchsia));
    }
}