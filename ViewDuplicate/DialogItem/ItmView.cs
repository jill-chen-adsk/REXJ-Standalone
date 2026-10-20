using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmView
    {
        public ItmView(View myView)
        {
            View = myView;
            Name = myView.Name;
        }

        public View View { get; }

        // Cached so list rendering and selection never call back into the Revit API.
        public string Name { get; }

        public override string ToString() => Name;

        public ViewType ViewType => View.ViewType;
    }
}
