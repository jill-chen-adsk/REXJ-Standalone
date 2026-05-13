using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LevelFilter.Components
{
    public class Elements
    {
        private readonly UIDocument _rvtUIDoc;

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public UIDocument RvtUIDoc => _rvtUIDoc;
        public Document RvtDBDoc => _rvtUIDoc.Document;

        public ProjectInfo ProjectInfo => _rvtUIDoc.Document.ProjectInformation;

        public Element GetElementDoc(int id)
        {
            return _rvtUIDoc.Document.GetElement(new ElementId((long)id));
        }

        public IList<Element> SelElems
        {
            get
            {
                IList<Element> elementList = new List<Element>();
                foreach (ElementId elementId in _rvtUIDoc.Selection.GetElementIds())
                {
                    if (elementId != null)
                    {
                        Element element = RvtDBDoc.GetElement(elementId);
                        elementList.Add(element);
                    }
                }
                return elementList;
            }
        }

        public IList<Material> PartsMaterials(IList<Part> parts)
        {
            IList<Material> materialList = new List<Material>();
            foreach (Part part in parts)
            {
                foreach (ElementId materialId in part.GetMaterialIds(false))
                {
                    Material element = RvtDBDoc.GetElement(materialId) as Material;
                    materialList.Add(element);
                }
            }
            return materialList;
        }

        public List<ParameterFilterElement> GetRuleFilterElements(Document doc)
        {
            List<ParameterFilterElement> retVal = new List<ParameterFilterElement>();
            IList<Element> filterList = new FilteredElementCollector(doc)
                        .OfClass(typeof(ParameterFilterElement)).ToElements();
            foreach (Element rule in filterList)
            {
                if (rule == null) continue;
                ParameterFilterElement pfe = rule as ParameterFilterElement;
                if (pfe == null) continue;
                retVal.Add(pfe);
            }
            retVal = retVal.OrderBy(x => x.Name).ToList();
            return retVal;
        }
    }
}
