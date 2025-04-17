using NeonShell.Shell;

namespace NeonShell.Commands;

public class NanoWrapper : ICustomCommand, IFallbackAfterExecute
{
    public string Name => "nano";

    public bool IsInteractive => true;

    public bool ShouldFallback => true;

    public void Execute(ShellContext context, string[] args) {}
}

