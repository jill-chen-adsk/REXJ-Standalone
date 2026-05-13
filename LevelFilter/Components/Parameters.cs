using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LevelFilter.Components
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

            foreach (Parameter param in element.Parameters)
            {
                if (param.Definition.Name == parameterName)
                    return param;
            }

            return null;
        }
    }
}
