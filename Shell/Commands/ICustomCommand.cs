namespace NeonShell.Shell;

public interface ICustomCommand
{
    string Name { get; }
    bool IsInteractive => false;
    void Execute(ShellContext context, string[] args);
}