using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class Settings
    {
        private UIDocument _rvtUIDoc;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(_rvtUIDoc.Document, bic);
        }

        public Category CategoryProjInfo => Category.GetCategory(_rvtUIDoc.Document, BuiltInCategory.OST_ProjectInformation);

        public BuiltInCategory GetPartsSymbolType(FamilySymbol familySymbol)
        {
            var doorCatId = GetCategory(BuiltInCategory.OST_Doors).Id.ToString();
            var windowCatId = GetCategory(BuiltInCategory.OST_Windows).Id.ToString();
            var catId = familySymbol.Category.Id.ToString();

            if (catId == doorCatId)
                return BuiltInCategory.OST_Doors;
            else if (catId == windowCatId)
                return BuiltInCategory.OST_Windows;
            return BuiltInCategory.INVALID;
        }
    }
}
