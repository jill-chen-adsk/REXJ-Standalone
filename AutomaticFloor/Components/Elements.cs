using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.AutomaticFloor.Utils;

namespace ADSK.JExtRAC.AutomaticFloor.Components
{
    public class Elements
    {
        public UIDocument RvtUIDoc { get; }
        public Document RvtDBDoc => RvtUIDoc.Document;

        public Elements(UIDocument rvtUIDoc)
        {
            RvtUIDoc = rvtUIDoc;
        }

        public ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(RvtDBDoc, bic);
        }

        public Element GetElementDoc(int id)
        {
            var elemId = new ElementId((long)id);
            return RvtDBDoc.GetElement(elemId);
        }

        public IList<Element> GetFloorTypes(eFloorType eFloorType)
        {
            IList<Element> ret = new List<Element>();

            var collector = new FilteredElementCollector(RvtDBDoc)
                .OfClass(typeof(FloorType));

            BuiltInCategory targetCat = eFloorType == eFloorType.Slab
                ? BuiltInCategory.OST_StructuralFoundation
                : BuiltInCategory.OST_Floors;

            foreach (Element elem in collector)
            {
                if (elem is FloorType floorType)
                {
                    if (floorType.Category != null && floorType.Category.BuiltInCategory == targetCat)
                    {
                        if (eFloorType == eFloorType.Slab)
                        {
                            if (!floorType.IsFoundationSlab) continue;
                        }
                        else
                        {
                            if (floorType.IsFoundationSlab) continue;
                        }
                        ret.Add(elem);
                    }
                    else if (targetCat == BuiltInCategory.OST_Floors && !floorType.IsFoundationSlab)
                    {
                        ret.Add(elem);
                    }
                }
            }

            return ret;
        }
    }
}
