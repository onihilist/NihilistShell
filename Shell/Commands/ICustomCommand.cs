namespace NeonShell.Shell;

public interface ICustomCommand
{
    string Name { get; }
    void Execute(ShellContext context, string[] args);
}