using NeonShell.Shell;

namespace NeonShell.Commands;

public class HistoryWrapper : ICustomCommand, IFallbackAfterExecute, IMetadataCommand
{
    public string Name => "history";
    public string Description => "See all the commands in the history";

    public bool IsInteractive => true;

    public bool ShouldFallback => true;

    public void Execute(ShellContext context, string[] args) {}
}