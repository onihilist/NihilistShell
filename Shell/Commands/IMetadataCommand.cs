
namespace NeonShell.Shell;

/// <summary>
/// The <c>IMetadataCommand</c> interface defines optional metadata 
/// that can be implemented by a custom shell command to provide 
/// additional information such as a description, visibility, interactivity, 
/// and required privileges.
/// </summary>
public interface IMetadataCommand
{
    string Description { get; }
    bool IsHidden => false;
    bool IsInteractive => false;
    bool RequiresRoot => false;
}
