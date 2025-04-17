namespace NeonShell.Shell;

public interface IFallbackAfterExecute
{
    bool ShouldFallback { get; }
}