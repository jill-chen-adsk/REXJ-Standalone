using System.Collections.Generic;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public class SpSlabType : SpBase
    {
        private IList<Category> _ParamCategories;

        public SpSlabType(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Components.Parameters cmpParameters,
                          RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            _ParamCategories = base.CmpSettings.CategorySlab;
            base.SetDefCatName(_ParamCategories);
            base.DefSuccess = true;
        }
    }
}
