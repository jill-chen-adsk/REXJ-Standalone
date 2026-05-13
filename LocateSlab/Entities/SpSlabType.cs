using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public class SpSlabType : SpBase
    {
        public SpSlabType(Components.Attribute cmpAttribute, Components.Parameters cmpParameters, Components.Settings cmpSettings)
            : base(cmpAttribute, cmpParameters, cmpSettings)
        {
            var paramCategories = CmpSettings.CategorySlab;
            SetDefCatName(paramCategories);
            DefSuccess = true;
        }
    }
}
