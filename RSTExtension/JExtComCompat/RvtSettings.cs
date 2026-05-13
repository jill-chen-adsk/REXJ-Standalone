using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace JExtComCompat
{
    public class RvtSettings
    {
        public RvtSettings(UIDocument uidoc)
        {
            UiDocument = uidoc;
            Document = uidoc.Document;
        }

        public UIDocument UiDocument { get; }
        public Document Document { get; }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(Document, bic);
        }

        public double UnitCoe { get; } = 304.8;
    }
}
