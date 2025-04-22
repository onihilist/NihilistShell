using System;
using System.Collections.Generic;
using System.IO;

namespace NihilistShell.Shell.History
{
    public class HistoryManager
    {
        private readonly string _historyPath;
        private readonly List<string> _history = new();
        private int _currentIndex = -1;

        public HistoryManager(string? path = null)
        {
            _historyPath = path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nihilist_history");
            Load();
        }

        private void Load()
        {
            if (File.Exists(_historyPath))
                _history.AddRange(File.ReadAllLines(_historyPath));
        }

        public void Save()
        {
            File.WriteAllLines(_historyPath, _history);
        }

        public void Add(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                _history.Add(command);
                _currentIndex = _history.Count;
            }
        }

        public string? GetPrevious()
        {
            if (_history.Count == 0 || _currentIndex <= 0) return null;
            _currentIndex--;
            return _history[_currentIndex];
        }

        public string? GetNext()
        {
            if (_currentIndex >= _history.Count - 1) return null;
            _currentIndex++;
            return _history[_currentIndex];
        }

        public void ResetIndex()
        {
            _currentIndex = _history.Count;
        }
    }
}