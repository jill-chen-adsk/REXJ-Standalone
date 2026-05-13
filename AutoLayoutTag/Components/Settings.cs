using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutoLayoutTag.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;
        public Document RvtDBDoc { get; }

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }
    }
}
