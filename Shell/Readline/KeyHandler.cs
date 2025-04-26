using System.Text;
using NShell.Shell.History;
using Spectre.Console;

namespace NShell.Shell.Readline;

/// <summary>
/// <c>KeyHandler</c> deals with keybindings and inputs.
/// defining key behaviors, managing input buffers, and cursor position
/// </summary>
public partial class KeyHandler
{
	// a dictionary that stores key bindings
	private Dictionary<string, Action> _keyBindings;
	// cursor position
	private int _currentCursorPos4Console;
	private int _currentCursorPos4Input;
	private int _initCursorPos4Console;
	// length of the string entered
	private int _inputLength;
	private ConsoleKeyInfo _currentKey;
	private StringBuilder _inputBuffer;
	private readonly HistoryManager _history;
	public string InputBuffer { get => _inputBuffer.ToString(); }

	public void LoadKeyBindings()
	{
		foreach (var binding in _keyBindings)
		{
			AnsiConsole.MarkupLine($"[green]+[/] - Loaded key binding: [yellow]{binding.Key}[/] -> [blue]{binding.Value}[/]");
		}
	}

	// it can add custom key bindings to facilitate subsequent expansion
	public void AddCustomKeyBinding(string key, Action handler)
	{
		if (_keyBindings.ContainsKey(key))
		{
			_keyBindings.Remove(key);
		}
		_keyBindings.Add(key, handler);
	}

	// it can remove custom key bindings
	public void RemoveCustomKeyBinding(string key)
	{
		if (_keyBindings.ContainsKey(key))
		{
			_keyBindings.Remove(key);
		}
	}

	public KeyHandler(HistoryManager history)
	{
		_history = history;
		_inputBuffer = new();
		_keyBindings = new();
		_initCursorPos4Console = Console.CursorLeft;
		_currentCursorPos4Console = _initCursorPos4Console;
		_currentCursorPos4Input = 0;
		_inputLength = 0;

		_keyBindings["Backspace"] = HandleBackspaceChar;
		_keyBindings["Delete"] = HandleDeleteChar;
		_keyBindings["Home"] = MoveCursorToStart;
		_keyBindings["ControlA"] = MoveCursorToStart;
		_keyBindings["End"] = MoveCursorToEnd;
		_keyBindings["ControlE"] = MoveCursorToEnd;
		_keyBindings["LeftArrow"] = MoveCursorLeft;
		_keyBindings["RightArrow"] = MoveCursorRight;
		_keyBindings["UpArrow"] = PreviousHistory;
		_keyBindings["DownArrow"] = NextHistory;

		_keyBindings["Tab"] = HandleTab;
		_keyBindings["ControlLeftArrow"] = MoveCursorWordLeft;
		_keyBindings["ControlRightArrow"] = MoveCursorWordRight;
	}

	private string BuildKeyInput()
	{
		string result = "";
		if (_currentKey.Modifiers.HasFlag(ConsoleModifiers.Control)) result += "Control";
		if (_currentKey.Modifiers.HasFlag(ConsoleModifiers.Alt)) result += "Alt";
		if (_currentKey.Modifiers.HasFlag(ConsoleModifiers.Shift)) result += "Shift";
		result += _currentKey.Key.ToString();
		return result;
	}

	// TODO: need to improve when input complex key (like Control+A, Alt+Shift+A)
	public void HandleInput(ConsoleKeyInfo keyInfo)
	{
		_currentKey = keyInfo;
		_keyBindings.TryGetValue(BuildKeyInput(),out var handleAction);
		handleAction??=HandleNormalChar;
		handleAction.Invoke();
	}

	// clear the input buffer and reset the cursor position
	public void ResetInput()
	{
		_inputBuffer.Clear();
		_currentCursorPos4Console = _initCursorPos4Console;
		_currentCursorPos4Input = 0;
		_inputLength = 0;
	}

	// update the initial cursor position when the prompt is being updated
	public void UpdateInitCursorPos(int initCursorPos)
	{
		_initCursorPos4Console = initCursorPos;
		_currentCursorPos4Console = _initCursorPos4Console;
		_currentCursorPos4Input = 0;
		_inputLength = 0;
	}
}
