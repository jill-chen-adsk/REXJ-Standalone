using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(_rvtUIDoc.Document, bic);
        }

        public Category CategoryProjInfo => GetCategory(BuiltInCategory.OST_ProjectInformation);
    }
}
