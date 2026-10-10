using System.Collections.Generic;
using System.ComponentModel;
using ADSK.JExtRAC.ParameterFilter.Entities;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public class FilterRowItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public string Name { get; set; }
        public string SubName { get; set; }
        public int Count { get; set; }
        public List<ObjectElement> Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
