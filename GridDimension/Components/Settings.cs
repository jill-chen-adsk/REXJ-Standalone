using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.GridDimension.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }

        public Document RvtDBDoc { get; }

        public Category CategoryProjInfo => Category.GetCategory(RvtDBDoc, BuiltInCategory.OST_ProjectInformation);
    }
}
