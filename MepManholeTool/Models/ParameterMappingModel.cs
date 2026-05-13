using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace MepManholeTool.Model
{
    public class ParameterMappingModel : INotifyPropertyChanged
    {
        private string _revitCategory;
        private string _fromParameter;
        private string _familySymbol ;
        private string _toParameter;
        private bool _required;

        public string RevitCategory
        {
            get => _revitCategory;
            set
            {
                _revitCategory = value;
                OnPropertyChanged();
            }
        }

        public string FromParameter
        {
            get => _fromParameter;
            set
            {
                _fromParameter = value;
                OnPropertyChanged();
            }
        }

        public string MasuSymbol 
        {
            get => _familySymbol;
            set
            {
                _familySymbol = value;
                OnPropertyChanged();
            }
        }

        public string ToParameter
        {
            get => _toParameter;
            set
            {
                _toParameter = value ;
                OnPropertyChanged();
            }
        }

        public bool Required
        {
            get => _required;
            set
            {
                _required = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 