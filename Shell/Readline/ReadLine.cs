using NShell.Shell.History;

namespace NShell.Shell.Readline;

/// <summary>
/// <c>ReadLine</c> is a simple readline library
/// base on the tonerdo/readline project
/// </summary>
public class ReadLine
{
	public static KeyHandler Handler { get; }
	public static HistoryManager History { get; }

	static ReadLine()
	{
		History = new HistoryManager();
		Handler = new KeyHandler(History);
	}

	public static string GetText()
	{
		ConsoleKeyInfo key = Console.ReadKey(intercept: true);
		while (key.Key != ConsoleKey.Enter)
		{
			Handler.HandleInput(key);
			key = Console.ReadKey(intercept: true);
		}
		string input = Handler.InputBuffer;
		Handler.ResetInput();
		Console.WriteLine();
		return input;
	}
}
