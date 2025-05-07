
using System;
using System.Collections.Generic;
using System.IO;

namespace NShell.Shell.History
{
    /// <summary>
    /// <c>HistoryManager</c> class provide all methods about history.
    /// You can load the history of all previous commands,
    /// add a command into <c>_history</c>, save the history file.
    /// </summary>
    public class HistoryManager
    {
        private readonly string _historyPath;
        private readonly List<string> _history = new();
        private int _currentIndex = -1;

        public HistoryManager(string? path = null)
        {
            _historyPath = path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nshell/.nhistory");
            Load();
        }

        /// <summary>
        /// Load all commands in the history file.
        /// </summary>
        private void Load()
        {
            if (File.Exists(_historyPath))
                _history.AddRange(File.ReadAllLines(_historyPath));
        }

        /// <summary>
        /// Save all the commands into the history file.
        /// </summary>
        public void Save()
        {
            File.WriteAllLines(_historyPath, _history);
        }

        /// <summary>
        /// Add a command into the string list <c>_history</c>.
        /// <param name="command">The command to save into <c>_history</c></param>
        /// </summary>
        public void Add(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                _history.Add(command);
                _currentIndex = _history.Count;
            }
        }

        /// <summary>
        /// Get the previous command into <c>_history</c>.
        /// </summary>
        /// <returns>A <see cref="string"/> with the string value of the command.</returns>
        public string? GetPrevious()
        {
            if (_history.Count == 0 || _currentIndex <= 0) return null;
            _currentIndex--;
            return _history[_currentIndex];
        }

        /// <summary>
        /// Get the next command into <c>_history</c>.
        /// </summary>
        /// <returns>A <see cref="string"/> with the string value of the command.</returns>
        public string? GetNext()
        {
            if (_currentIndex >= _history.Count - 1) return null;
            _currentIndex++;
            return _history[_currentIndex];
        }

        /// <summary>
        /// Reset the index of the <c>_currentIndex</c>.
        /// </summary>
        public void ResetIndex()
        {
            _currentIndex = _history.Count;
        }
    }
}