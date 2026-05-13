using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmView
    {
        private View _view;

        public ItmView(View myView)
        {
            _view = myView;
        }

        public View View
        {
            get => _view;
            set => _view = value;
        }

        public override string ToString() => _view.Name;

        public ViewType ViewType => _view.ViewType;
    }
}
