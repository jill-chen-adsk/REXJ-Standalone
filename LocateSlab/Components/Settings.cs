using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LocateSlab.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(_rvtUIDoc.Document, bic);
        }

        public IList<Category> CategorySlab
        {
            get
            {
                var ret = new List<Category>();
                ret.Add(GetCategory(BuiltInCategory.OST_Floors));
                return ret;
            }
        }

        public Category CategoryProjInfo =>
            GetCategory(BuiltInCategory.OST_ProjectInformation);
    }
}
