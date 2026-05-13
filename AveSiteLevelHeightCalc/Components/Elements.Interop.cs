using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    public partial class Elements
    {
        public FamilySymbol LoadFamilyByFamilyName(string fileName, string familyName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
                    return null;
                Document doc = RvtDBDoc;
                if (!doc.LoadFamily(fileName, out Family family) || family == null)
                    return null;
                foreach (ElementId id in family.GetFamilySymbolIds())
                {
                    if (doc.GetElement(id) is FamilySymbol fs)
                    {
                        string fn = fs.Family?.Name ?? fs.Name;
                        if (!string.IsNullOrEmpty(fn) && fn == familyName)
                            return fs;
                    }
                }
            }
            catch { }

            return null;
        }

        public IList<Category> GetCategoriesList(BuiltInCategory bic)
        {
            IList<Category> cats = new List<Category>();
            Category c = Category.GetCategory(RvtDBDoc, bic);
            if (c != null)
                cats.Add(c);
            return cats;
        }

        public IList<Element> GetElementsSelection(
            IList<System.Type> sysTypes,
            IList<Category> categories,
            IList<string> names,
            bool groupMode)
        {
            var sel = _rvtUIDoc.Selection.GetElementIds();
            var targetElems = new List<Element>();
            if (sel == null || sel.Count == 0)
                return targetElems;
            foreach (ElementId elemId in sel)
            {
                Element elem = RvtDBDoc.GetElement(elemId);
                if (elem != null)
                    targetElems.Add(elem);
            }

            return GetElementsMerged(targetElems, sysTypes, categories, names, null, groupMode);
        }

        public IList<Element> GetViewElements(View view, System.Type sysType, IList<string> names)
        {
            if (view == null)
                return new List<Element>();
            ICollection<ElementId> ids = new FilteredElementCollector(RvtDBDoc, view.Id)
                .WhereElementIsNotElementType()
                .ToElementIds();
            var elems = ids.Select(id => RvtDBDoc.GetElement(id)).Where(e => e != null).ToList();
            IList<System.Type> sysTypes = sysType != null ? new List<System.Type> { sysType } : null;
            return GetElementsMerged(elems, sysTypes, null, names, null, true);
        }

        public IList<Element> GetViewElements(View view, System.Type sysType)
        {
            return GetViewElements(view, sysType, null);
        }

        public IList<Element> GetElementsDoc(
            ElementFilter elemFilter,
            IList<System.Type> sysTypes,
            IList<Category> categories,
            IList<string> names,
            Level level)
        {
            List<Element> ret;
            FilteredElementCollector col = new FilteredElementCollector(RvtDBDoc);
            if (elemFilter != null)
                ret = col.WherePasses(elemFilter).ToElements().ToList();
            else if (sysTypes != null && sysTypes.Count > 0)
            {
                var filters = sysTypes.Where(t => t != null).Select(t => (ElementFilter)new ElementClassFilter(t)).Cast<ElementFilter>().ToList();
                if (filters.Count > 1)
                    ret = col.WherePasses(new LogicalOrFilter(filters)).ToElements().ToList();
                else
                    ret = col.WherePasses(filters[0]).ToElements().ToList();
            }
            else if (categories != null && categories.Count > 0)
            {
                var filters = categories.Where(c => c != null).Select(c => new ElementCategoryFilter(c.Id)).Cast<ElementFilter>().ToList();
                ret = col.WherePasses(new LogicalOrFilter(filters)).ToElements().ToList();
            }
            else
                ret = col.ToElements().ToList();

            return GetElementsMerged(ret, sysTypes == null ? null : sysTypes, categories, names, level, false);
        }

        public IList<Element> GetElementsDoc(System.Type sysType)
        {
            List<System.Type> sysTypes = null;
            if (sysType != null)
                sysTypes = new List<System.Type> { sysType };
            return GetElementsDoc(null, sysTypes, null, null, null);
        }

        private IList<Element> GetElementsMerged(
            IList<Element> elements,
            IList<System.Type> sysTypes,
            IList<Category> categories,
            IList<string> names,
            Level level,
            bool groupMode)
        {
            var retElems = new List<Element>();
            foreach (Element elem in elements)
            {
                if (!groupMode)
                    retElems.Add(elem);
                else if (elem is Group g)
                {
                    ICollection<ElementId> mids = g.GetMemberIds();
                    foreach (ElementId mid in mids)
                    {
                        Element member = RvtDBDoc.GetElement(mid);
                        if (member != null)
                            retElems.Add(member);
                    }
                }
                else
                    retElems.Add(elem);
            }

            if (sysTypes != null && sysTypes.Count > 0)
            {
                retElems = retElems
                    .Where(e => sysTypes.Any(t => t != null && t.IsInstanceOfType(e)))
                    .ToList();
            }

            if (categories != null && categories.Count > 0)
            {
                var catIds = new HashSet<ElementId>();
                foreach (Category c in categories)
                {
                    if (c?.Id != null)
                        catIds.Add(c.Id);
                }
                retElems = retElems
                    .Where(e => e?.Category?.Id != null && catIds.Contains(e.Category.Id))
                    .ToList();
            }

            if (names != null && names.Count > 0)
            {
                retElems = retElems
                    .Where(e =>
                    {
                        string nm = e.Name ?? "";
                        foreach (string n in names)
                        {
                            if (nm.IndexOf(n, StringComparison.Ordinal) >= 0)
                                return true;
                        }

                        return false;
                    }).ToList();
            }

            if (level != null)
            {
                retElems = retElems
                    .Where(e =>
                    {
                        ElementId lvlId = e.LevelId;
                        if (lvlId == null || lvlId.Equals(ElementId.InvalidElementId))
                            return false;
                        return lvlId.Equals(level.Id);
                    }).ToList();
            }

            return retElems;
        }

        private TextNote CreateTextNoteInternal(View view, XYZ pos, HorizontalTextAlignment align, string text)
        {
            ElementId typeId = RvtDBDoc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            TextNoteOptions opts = new TextNoteOptions(typeId) { HorizontalAlignment = align };
            return TextNote.Create(RvtDBDoc, view.Id, pos, text, opts);
        }

        public CurveElement CreateDetailCurve(View view, Curve curve)
        {
            return RvtDBDoc.Create.NewDetailCurve(view, curve);
        }

        public Dimension CreateDimension(View view, XYZ pos1, XYZ pos2, ReferenceArray refs)
        {
            Line line = Line.CreateBound(pos1, pos2);
            return RvtDBDoc.Create.NewDimension(view, line, refs);
        }

        public ViewDrafting CreateViewDrafting(string viewName, ViewFamilyType viewFamType, int scale)
        {
            if (viewFamType == null)
            {
                viewFamType = new FilteredElementCollector(RvtDBDoc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(v => v.ViewFamily == ViewFamily.Drafting);
            }
            if (viewFamType == null)
                return null;
            ViewDrafting v = Autodesk.Revit.DB.ViewDrafting.Create(RvtDBDoc, viewFamType.Id);
            v.Name = viewName;
            v.Scale = scale;
            return v;
        }


        private Element GetElementDocByLegacyId(int id)
        {
            try { return RvtDBDoc?.GetElement(new ElementId(id)); }
            catch { return RvtDBDoc?.GetElement(new ElementId((long)id)); }
        }

        private View GetActiveView(ViewType vt)
        {
            View active = RvtDBDoc.ActiveView;
            if (active != null && active.ViewType == vt)
                return active;
            return null;
        }
    }
}
