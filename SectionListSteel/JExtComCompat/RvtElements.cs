using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SectionListSteel.JExtComCompat
{
    public class RvtElements
    {
        public RvtElements(UIDocument uidoc)
        {
            UiDocument = uidoc;
            Document = uidoc.Document;
        }

        public UIDocument UiDocument { get; }
        public Document Document { get; }
        public Document RvtDBDoc => Document;

        public ProjectInfo ProjectInfo => Document.ProjectInformation;

        public Element? GetElementDoc(int id)
        {
            return Document.GetElement(new ElementId((long)id));
        }

        public IList<Element> GetElementsDoc(
            ElementFilter? filter,
            IList<Type>? types,
            IList<Category>? categories,
            IList<string>? names,
            object? unusedParameters)
        {
            var collector = new FilteredElementCollector(Document);
            IList<Element> raw;

            if (filter != null)
            {
                raw = collector.WherePasses(filter).ToElements();
            }
            else if (types != null && types.Count > 0)
            {
                var classFilters = new List<ElementFilter>();
                foreach (var t in types)
                {
                    classFilters.Add(new ElementClassFilter(t));
                }
                var orf = new LogicalOrFilter(classFilters);
                raw = collector.WherePasses(orf).ToElements();
            }
            else if (categories != null && categories.Count > 0)
            {
                var catFilters = new List<ElementFilter>();
                foreach (var c in categories)
                {
                    catFilters.Add(new ElementCategoryFilter(c.Id));
                }
                var orf = new LogicalOrFilter(catFilters);
                raw = collector.WherePasses(orf).ToElements();
            }
            else
            {
                raw = new List<Element>();
            }

            return FilterElements(raw, types, categories, names, null, false);
        }

        private static IList<Element> FilterElements(
            IList<Element> elements,
            IList<Type>? types,
            IList<Category>? categories,
            IList<string>? names,
            Level? level,
            bool groupMode)
        {
            var ret = new List<Element>();
            foreach (var elem in elements)
            {
                if (groupMode)
                {
                    ret.Add(elem);
                }
                else
                {
                    ret.Add(elem);
                }
            }

            if (types != null && types.Count > 0)
            {
                ret = ret.Where(e => types.Any(t => t.IsInstanceOfType(e))).ToList();
            }
            if (categories != null && categories.Count > 0)
            {
                var ids = new HashSet<long>(categories.Select(c => c.Id.Value));
                ret = ret.Where(e => e.Category != null && ids.Contains(e.Category.Id.Value)).ToList();
            }
            if (names != null && names.Count > 0)
            {
                var set = new HashSet<string>(names);
                ret = ret.Where(e => e.Name != null && set.Contains(e.Name)).ToList();
            }

            return ret;
        }

        public IList<T> CastElements<T>(IList<Element> elements) where T : Element
        {
            var list = new List<T>();
            foreach (var e in elements)
            {
                if (e is T t)
                {
                    list.Add(t);
                }
            }
            return list;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(Document, bic);
        }

        public Level? GetElementLevel(Element elem)
        {
            if (elem is Wall w)
            {
                var p = w.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                if (p != null)
                {
                    return Document.GetElement(p.AsElementId()) as Level;
                }
            }
            if (elem is Floor f)
            {
                var p = f.get_Parameter(BuiltInParameter.LEVEL_PARAM);
                if (p != null)
                {
                    return Document.GetElement(p.AsElementId()) as Level;
                }
            }
            if (elem is FamilyInstance fi)
            {
                var p = fi.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                if (p != null)
                {
                    return Document.GetElement(p.AsElementId()) as Level;
                }
                p = fi.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
                if (p != null)
                {
                    return Document.GetElement(p.AsElementId()) as Level;
                }
            }
            return null;
        }

        public Element? GetElementType(Element elem)
        {
            ElementId tid = elem.GetTypeId();
            if (tid == ElementId.InvalidElementId)
            {
                return null;
            }
            return Document.GetElement(tid);
        }

        public IList<Element> GetViewElements(View view, Type elementType)
        {
            var col = new FilteredElementCollector(Document, view.Id).WhereElementIsNotElementType();
            var elems = col.ToElements();
            return FilterElements(elems, new List<Type> { elementType }, null, null, null, true);
        }

        public TextNote? CreateTextNote(View view, XYZ position, HorizontalTextAlignment align, string text)
        {
            ElementId noteTypeId = GetDefaultTextNoteTypeId();
            if (noteTypeId == ElementId.InvalidElementId)
            {
                return null;
            }
            return TextNote.Create(Document, view.Id, position, text, noteTypeId);
        }

        /// <summary>詳細作成（角度・タイプオプション）</summary>
        public TextNote? CreateTextNote(
            View view,
            XYZ origin,
            double rotation,
            double lineWidthIgnored,
            HorizontalTextAlignment horizontalAlignment,
            ElementId typeId,
            string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            ElementId resolvedTypeId =
                typeId != ElementId.InvalidElementId ? typeId : GetDefaultTextNoteTypeId();
            if (resolvedTypeId == ElementId.InvalidElementId)
            {
                return null;
            }

            var opt = new TextNoteOptions
            {
                HorizontalAlignment = horizontalAlignment,
                KeepRotatedTextReadable = false,
                Rotation = rotation,
                TypeId = resolvedTypeId,
            };
            return TextNote.Create(Document, view.Id, origin, text, opt);
        }

        private ElementId GetDefaultTextNoteTypeId()
        {
            return Document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
        }

        public DetailCurve? CreateDetailCurve(View view, Curve curve)
        {
            return Document.Create.NewDetailCurve(view, curve);
        }

        public ViewDrafting? CreateViewDrafting(string name)
        {
            var vft = FindDraftingViewFamilyType();
            if (vft == null)
            {
                return null;
            }
            var vd = ViewDrafting.Create(Document, vft.Id);
            vd.Name = name;
            return vd;
        }

        public ViewDrafting? CreateViewDrafting(string name, ViewFamilyType? viewFamilyType, int scale)
        {
            var vft = viewFamilyType ?? FindDraftingViewFamilyType();
            if (vft == null)
            {
                return null;
            }
            var vd = ViewDrafting.Create(Document, vft.Id);
            vd.Name = name;
            vd.Scale = scale;
            return vd;
        }

        private ViewFamilyType? FindDraftingViewFamilyType()
        {
            return new FilteredElementCollector(Document)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(v => v.ViewFamily == ViewFamily.Drafting);
        }
    }
}
