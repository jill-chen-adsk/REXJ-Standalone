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

        // Unit conversion: internal units (feet) to millimeters. mm = ft * UnitCoe; ft = mm / UnitCoe.
        public double UnitCoe => 304.8;
    }
}
