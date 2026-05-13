using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace MepManholeTool.Model
{
    public class MasuParameter : INotifyPropertyChanged
    {
        private string _definitionName;

        public string DefinitionName
        {
            get => _definitionName;
            set
            {
                _definitionName = value;
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