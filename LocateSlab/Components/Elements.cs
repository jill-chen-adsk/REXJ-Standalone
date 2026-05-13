using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ADSK.JExtRAC.LocateSlab.Components
{
    public class Elements
    {
        private readonly UIDocument _rvtUIDoc;

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(RvtDBDoc, bic);
        }

        public Element GetElementDoc(int elemId)
        {
            return RvtDBDoc.GetElement(new ElementId((long)elemId));
        }

        public IList<Element> GetElementsDoc(object unused, IList<Type> sysTypes,
            IList<Category> categories, object unused2, object unused3)
        {
            var ret = new List<Element>();
            var collector = new FilteredElementCollector(RvtDBDoc);

            if (sysTypes != null && sysTypes.Count > 0)
            {
                var classFilter = new ElementMulticlassFilter(sysTypes);
                collector = collector.WherePasses(classFilter);
            }

            if (categories != null && categories.Count > 0)
            {
                foreach (var cat in categories)
                {
                    var catCollector = new FilteredElementCollector(RvtDBDoc);
                    if (sysTypes != null && sysTypes.Count > 0)
                        catCollector = catCollector.WherePasses(new ElementMulticlassFilter(sysTypes));
                    catCollector = catCollector.OfCategoryId(cat.Id);
                    foreach (var e in catCollector)
                        ret.Add(e);
                }
                return ret;
            }

            foreach (var e in collector)
                ret.Add(e);
            return ret;
        }

        public IList<Element> GetElementsSelection(IList<Type> sysTypes,
            IList<Category> categories, object unused, bool prompt)
        {
            var ret = new List<Element>();
            var selIds = _rvtUIDoc.Selection.GetElementIds();

            if (selIds.Count == 0 && prompt)
            {
                try
                {
                    var refs = _rvtUIDoc.Selection.PickObjects(ObjectType.Element);
                    foreach (var r in refs)
                    {
                        var elem = RvtDBDoc.GetElement(r);
                        if (elem != null && MatchFilters(elem, sysTypes, categories))
                            ret.Add(elem);
                    }
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                return ret;
            }

            foreach (var id in selIds)
            {
                var elem = RvtDBDoc.GetElement(id);
                if (elem != null && MatchFilters(elem, sysTypes, categories))
                    ret.Add(elem);
            }
            return ret;
        }

        private bool MatchFilters(Element elem, IList<Type> sysTypes, IList<Category> categories)
        {
            bool typeMatch = sysTypes == null || sysTypes.Count == 0;
            if (!typeMatch)
            {
                foreach (var t in sysTypes)
                {
                    if (t.IsInstanceOfType(elem)) { typeMatch = true; break; }
                }
            }
            if (!typeMatch) return false;

            bool catMatch = categories == null || categories.Count == 0;
            if (!catMatch && elem.Category != null)
            {
                foreach (var c in categories)
                {
                    if (elem.Category.Id == c.Id) { catMatch = true; break; }
                }
            }
            return catMatch;
        }

        public ViewPlan ActiveViewPlan
        {
            get
            {
                var activeView = RvtDBDoc.ActiveView;
                return activeView as ViewPlan;
            }
        }

        public IList<Element> FloorTypes
        {
            get
            {
                var ret = new List<Element>();
                var collector = new FilteredElementCollector(RvtDBDoc)
                    .OfClass(typeof(FloorType));
                foreach (var elem in collector)
                {
                    if (elem is FloorType)
                        ret.Add(elem);
                }
                return ret;
            }
        }

        public ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;
    }
}
