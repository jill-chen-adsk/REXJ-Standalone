using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmView
    {
        private View m_View;

        public ItmView(View myView)
        {
            m_View = myView;
        }

        public View View
        {
            get => m_View;
            set => m_View = value;
        }

        public override string ToString() => m_View.Name;

        public ViewType ViewType => m_View.ViewType;
    }
}
