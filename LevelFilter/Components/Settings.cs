using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LevelFilter.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Category CategoryProjectInfo
        {
            get
            {
                return Category.GetCategory(_rvtUIDoc.Document, BuiltInCategory.OST_ProjectInformation);
            }
        }
    }
}
