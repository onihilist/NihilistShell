namespace NeonShell.Shell;

public interface IMetadataCommand
{
    string Description { get; }

    bool IsHidden => false;
    bool IsInteractive => false;
    bool RequiresRoot => false;
}