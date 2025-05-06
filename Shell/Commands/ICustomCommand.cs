
namespace NShell.Shell.Commands;

/// <summary>
/// The <c>ICustomCommand</c> interface defines a contract for implementing
/// custom shell commands within the shell environment.
/// 
/// Each custom command must declare its <see cref="Name"/> and implement the
/// <see cref="Execute"/> method to define its behavior.
/// 
/// To add a new command, create a class implementing this interface
/// in the <code>Commands</code> folder.
/// </summary>
public interface ICustomCommand
{
    string Name { get; }
    void Execute(ShellContext context, string[] args);
}
