using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.ExportExcel.Components
{
    public class Parameters
    {
        private readonly UIDocument _rvtUIDoc;

        public Parameters(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Parameter FindParameter(Element element, string parameterName)
        {
            if (element == null || string.IsNullOrEmpty(parameterName))
                return null;

            foreach (Parameter p in element.Parameters)
            {
                if (p.Definition != null && p.Definition.Name == parameterName)
                    return p;
            }

            if (element is FamilyInstance familyInstance && familyInstance.Symbol != null)
            {
                foreach (Parameter p in familyInstance.Symbol.Parameters)
                {
                    if (p.Definition != null && p.Definition.Name == parameterName)
                        return p;
                }
            }

            return null;
        }
    }
}
