using Autodesk.Revit.UI;

namespace JExtComCompat
{
    public class RvtGeometry
    {
        public RvtGeometry(UIDocument uidoc)
        {
            UiDocument = uidoc;
        }

        public UIDocument UiDocument { get; }

        public double UnitCoe { get; } = 304.8;
    }
}
