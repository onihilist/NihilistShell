using System.Diagnostics;

namespace NShell.Shell.Readline;

/// <summary>
/// <c>KeyHandler</c> is a class that handle all about key input.
/// define different key handling methods
/// </summary>
public partial class KeyHandler
{
	public void HandleNormalChar()
	{
		_inputBuffer.Insert(_currentCursorPos4Input, _currentKey.KeyChar);
		_inputLength++;
		Console.Write((string?)_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input));
		_currentCursorPos4Input++;
		_currentCursorPos4Console++;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleBackspaceChar()
	{
		if (IsStartOfLine())
			return;
		_inputBuffer = _inputBuffer.Remove(_currentCursorPos4Input - 1, 1);
		_inputLength--;
		_currentCursorPos4Input--;
		_currentCursorPos4Console--;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		Console.Write(_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input) + " ");
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleDeleteChar()
	{
		if (IsEndOfLine())
			return;
		_inputBuffer.Remove(_currentCursorPos4Input, 1);
		_inputLength--;
		Console.Write(_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input) + " ");
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleTab()
	{
		// TODO: Implement tab completion
	}

	public void PreviousHistory()
	{
		var prev = _history.GetPrevious();
		if (prev != null)
		{
			Console.Write(new string('\b', _inputLength) + new string(' ', _inputLength) + new string('\b', _inputLength));
			_inputBuffer.Clear();
			_inputLength = prev.Length;
			_inputBuffer.Append(prev);
			Console.Write((object?)_inputBuffer);
			_currentCursorPos4Input = _inputLength;
			_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		}
	}

	public void NextHistory()
	{
		var next = _history.GetNext();
		if (next != null)
		{
			Console.Write(new string('\b', _inputLength) + new string(' ', _inputLength) + new string('\b', _inputLength));
			_inputBuffer.Clear();
			_inputLength = next.Length;
			_inputBuffer.Append(next);
			Console.Write((object?)_inputBuffer);
			_currentCursorPos4Input = _inputLength;
			_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		}
	}

	private void MoveCursorToEnd()
	{
		_currentCursorPos4Input = _inputLength;
		_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private void MoveCursorToStart()
	{
		_currentCursorPos4Input = 0;
		_currentCursorPos4Console = _initCursorPos4Console;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	private void MoveCursorWordLeft()
	{
		Debug.Write("MoveCursorWordLeft");
	}

	private void MoveCursorWordRight()
	{
		Debug.Write("MoveCursorWordRight");
	}

	private void MoveCursorLeft()
	{
		if (_currentCursorPos4Input == 0)
			return;
		_currentCursorPos4Console--;
		_currentCursorPos4Input--;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private void MoveCursorRight()
	{
		if (_currentCursorPos4Input == _inputLength)
			return;
		_currentCursorPos4Console++;
		_currentCursorPos4Input++;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private bool IsEndOfLine() => _currentCursorPos4Input ==  _inputLength;

	private bool IsStartOfLine() => _currentCursorPos4Input == 0;
}
