using NeonShell.Shell;

namespace NeonShell.Commands;

public class NanoWrapper : ICustomCommand, IFallbackAfterExecute, IMetadataCommand
{
    public string Name => "nano";
    public string Description => "A tool to write/edit files";

    public bool IsInteractive => true;

    public bool ShouldFallback => true;

    public void Execute(ShellContext context, string[] args) {}
}

