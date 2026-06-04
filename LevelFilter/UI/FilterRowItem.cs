using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LevelFilter.UI
{
    public class FilterRowItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public string Name { get; set; }
        public string SubName { get; set; }
        public int Count { get; set; }
        public object Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
