using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;

namespace RSTExtension.Components
{
    public class CFP_Settings : RvtSettings
    {
        public CFP_Settings(UIDocument uidoc) : base(uidoc)
        {
        }

        public Category CategoryLevel => GetCategory(BuiltInCategory.OST_Levels);
    }
}
