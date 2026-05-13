using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.ImportExcel.Components
{
    public class Parameters
    {
        private readonly UIDocument _rvtUIDoc;

        public Parameters(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Parameter GetParameter(Element element, string parameterName, object unused1, ForgeTypeId unused2)
        {
            if (element == null || string.IsNullOrEmpty(parameterName))
                return null;

            foreach (Parameter param in element.Parameters)
            {
                if (param.Definition.Name == parameterName)
                    return param;
            }

            return null;
        }
    }
}
