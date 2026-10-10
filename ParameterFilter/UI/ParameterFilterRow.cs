using System.Collections.Generic;
using System.ComponentModel;
using ADSK.JExtRAC.ParameterFilter.Entities;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public class ParameterFilterRow : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _value;
        private string _min;
        private string _max;
        private string _error;
        private string _countDisplay;
        private bool _isVisible = true;
        private bool _valueReadOnly;
        private bool _minReadOnly;
        private bool _maxReadOnly;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string Category { get; set; }
        public string FamilyType { get; set; }
        public string ParameterName { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Min
        {
            get => _min;
            set
            {
                if (_min == value)
                    return;
                _min = value;
                OnPropertyChanged(nameof(Min));
            }
        }

        public string Max
        {
            get => _max;
            set
            {
                if (_max == value)
                    return;
                _max = value;
                OnPropertyChanged(nameof(Max));
            }
        }

        public string Error
        {
            get => _error;
            set
            {
                if (_error == value)
                    return;
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public string CountDisplay
        {
            get => _countDisplay;
            set
            {
                if (_countDisplay == value)
                    return;
                _countDisplay = value;
                OnPropertyChanged(nameof(CountDisplay));
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public bool ValueReadOnly
        {
            get => _valueReadOnly;
            set
            {
                if (_valueReadOnly == value)
                    return;
                _valueReadOnly = value;
                OnPropertyChanged(nameof(ValueReadOnly));
            }
        }

        public bool MinReadOnly
        {
            get => _minReadOnly;
            set
            {
                if (_minReadOnly == value)
                    return;
                _minReadOnly = value;
                OnPropertyChanged(nameof(MinReadOnly));
            }
        }

        public bool MaxReadOnly
        {
            get => _maxReadOnly;
            set
            {
                if (_maxReadOnly == value)
                    return;
                _maxReadOnly = value;
                OnPropertyChanged(nameof(MaxReadOnly));
            }
        }

        public string TypeKey { get; set; }
        public List<ObjectLengthParameter> LengthParameters { get; set; }
        public double? MinLengthTag { get; set; }
        public double? MaxLengthTag { get; set; }
        public int CountTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
