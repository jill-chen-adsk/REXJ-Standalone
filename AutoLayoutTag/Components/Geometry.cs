using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutoLayoutTag.Components
{
    public class Geometry
    {
        private readonly UIDocument _rvtUIDoc;
        public Document RvtDBDoc { get; }

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }

        public double UnitCoe
        {
            get
            {
                return UnitUtils.ConvertToInternalUnits(1.0, UnitTypeId.Millimeters);
            }
        }
    }
}
