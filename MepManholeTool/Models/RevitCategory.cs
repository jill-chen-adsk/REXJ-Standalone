using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace MepManholeTool.Model
{
    public class RevitCategory : INotifyPropertyChanged
    {
        private string _categoryName;
        private ElementId _categoryId;
        private FamilySymbol _familySymbol ;
        private Parameter _parameter ;
        private Definition _definition ;

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public ElementId CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }

        public FamilySymbol MasuFamilySymbol
        {
            get => _familySymbol ;
            set
            {
                _familySymbol = value ;
                OnPropertyChanged() ;
            }
        }

        public Parameter MasuParameter
        {
            get => _parameter ;
            set
            {
                _parameter = value ;
                OnPropertyChanged() ;
            }
        }

        public Definition ParamDefinition
        {
            get => _definition ;
            set
            {
                _definition = value ;
                OnPropertyChanged() ;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return CategoryName;
        }
    }
} 