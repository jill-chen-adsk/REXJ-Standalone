using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MepManholeTool.Models
{
    public class MasuLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        #region プロパティ
        private string _level;

        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
        #endregion

        public MasuLevel(string level)
        {
            this.Level = level;
        }
    }
}