using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class Elements
    {
        private UIDocument _rvtUIDoc;
        private Attribute _cmpAttribute;

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _cmpAttribute = new Attribute();
        }

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public ProjectInfo ProjectInfo => _rvtUIDoc.Document.ProjectInformation;

        public Element GetElementDoc(int id)
        {
            return RvtDBDoc.GetElement(new ElementId((long)id));
        }

        public View GetActiveView(ViewType viewType)
        {
            var view = _rvtUIDoc.ActiveView;
            if (view != null && view.ViewType == viewType)
                return view;
            return null;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(RvtDBDoc, bic);
        }

        public IList<FamilyInstance> GetElementsFamilyInstanceKind(Category category)
        {
            if (category == null) return new List<FamilyInstance>();
            return new FilteredElementCollector(RvtDBDoc, _rvtUIDoc.ActiveView.Id)
                .OfCategoryId(category.Id)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();
        }

        public IList<FamilyInstance> GetElementsSelectionFamilyInstanceKind(Category category, bool includeSubTypes)
        {
            if (category == null) return new List<FamilyInstance>();
            var selIds = _rvtUIDoc.Selection.GetElementIds();
            var result = new List<FamilyInstance>();
            foreach (var eid in selIds)
            {
                var elem = RvtDBDoc.GetElement(eid);
                if (elem is FamilyInstance fi && fi.Category != null && fi.Category.Id == category.Id)
                    result.Add(fi);
            }
            return result;
        }

        public void GetElementsTable(Type sysType, ref DataTable table)
        {
            var collector = new FilteredElementCollector(RvtDBDoc).OfClass(sysType);
            foreach (var elem in collector)
            {
                table.Rows.Add(Int32.Parse(elem.Id.ToString()), elem.Name);
            }
        }

        public void GetElementsTable(Type sysType, IList<string> nameFilters, ref DataTable table)
        {
            var collector = new FilteredElementCollector(RvtDBDoc).OfClass(sysType);
            foreach (var elem in collector)
            {
                bool match = false;
                foreach (var prefix in nameFilters)
                {
                    if (elem.Name.Contains(prefix))
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                    table.Rows.Add(Int32.Parse(elem.Id.ToString()), elem.Name);
            }
        }

        public void GetElementsTable(object unused, Type sysType, Category category, ref DataTable table)
        {
            var collector = new FilteredElementCollector(RvtDBDoc)
                .OfClass(sysType);
            if (category != null)
                collector = collector.OfCategoryId(category.Id);
            foreach (var elem in collector)
            {
                table.Rows.Add(Int32.Parse(elem.Id.ToString()), elem.Name);
            }
        }

        public IList<Element> GetElementsDoc(object unused1, IList<Type> sysTypes, object unused2, IList<string> names, object unused3)
        {
            var result = new List<Element>();
            foreach (var sysType in sysTypes)
            {
                var collector = new FilteredElementCollector(RvtDBDoc).OfClass(sysType);
                foreach (var elem in collector)
                {
                    bool match = false;
                    foreach (var name in names)
                    {
                        if (elem.Name.Contains(name))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match)
                        result.Add(elem);
                }
            }
            return result;
        }

        public IList<Element> GetElementsViewPorts(string sheetName)
        {
            var result = new List<Element>();
            var sheets = new FilteredElementCollector(RvtDBDoc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Where(s => s.Name == sheetName);
            foreach (var sheet in sheets)
            {
                foreach (var vpId in sheet.GetAllViewports())
                {
                    var vp = RvtDBDoc.GetElement(vpId);
                    if (vp != null) result.Add(vp);
                }
            }
            return result;
        }

        public IList<View> GetUsedViewOfSheet()
        {
            var result = new List<View>();
            var sheets = new FilteredElementCollector(RvtDBDoc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>();
            foreach (var sheet in sheets)
            {
                foreach (var vpId in sheet.GetAllViewports())
                {
                    var vp = RvtDBDoc.GetElement(vpId) as Viewport;
                    if (vp != null)
                    {
                        var view = RvtDBDoc.GetElement(vp.ViewId) as View;
                        if (view != null) result.Add(view);
                    }
                }
            }
            return result;
        }

        public IList<Element> GetViewElements(View view)
        {
            return new FilteredElementCollector(RvtDBDoc, view.Id).ToList();
        }

        public IList<ViewFamilyType> GetViewFamilyTypes(ViewFamily viewFamily)
        {
            return new FilteredElementCollector(RvtDBDoc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .Where(v => v.ViewFamily == viewFamily)
                .ToList();
        }

        public IndependentTag CreateTag(View view, FamilyInstance familyInstance, ElementType tagSymbol)
        {
            var refElem = new Reference(familyInstance);
            var tag = IndependentTag.Create(RvtDBDoc, view.Id, refElem, false,
                TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, view.Origin);

            if (tag != null && tagSymbol != null)
            {
                tag.ChangeTypeId(tagSymbol.Id);
            }
            return tag;
        }

        // Properties for door/window types and tags
        public IList<FamilyInstance> ElemntsDoorTypes
        {
            get
            {
                var cat = GetCategory(BuiltInCategory.OST_Doors);
                return GetElementsFamilyInstanceKind(cat);
            }
        }

        public IList<FamilyInstance> ElemntsWindowTypes
        {
            get
            {
                var cat = GetCategory(BuiltInCategory.OST_Windows);
                return GetElementsFamilyInstanceKind(cat);
            }
        }

        public IList<FamilyInstance> SelSetDoorTypes
        {
            get
            {
                var cat = GetCategory(BuiltInCategory.OST_Doors);
                return GetElementsSelectionFamilyInstanceKind(cat, true);
            }
        }

        public IList<FamilyInstance> SelSetWindowTypes
        {
            get
            {
                var cat = GetCategory(BuiltInCategory.OST_Windows);
                return GetElementsSelectionFamilyInstanceKind(cat, true);
            }
        }

        public DataTable ElementsTableDoorTag
        {
            get
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("NAME", typeof(string));
                var cat = GetCategory(BuiltInCategory.OST_DoorTags);
                GetElementsTable(null, typeof(FamilySymbol), cat, ref table);
                return table;
            }
        }

        public DataTable ElementsTableWindowTag
        {
            get
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("NAME", typeof(string));
                var cat = GetCategory(BuiltInCategory.OST_WindowTags);
                GetElementsTable(null, typeof(FamilySymbol), cat, ref table);
                return table;
            }
        }

        private static readonly string[] ElevationViewPrefixes = new[]
        {
            "姿図_ドア", "姿図_窓",
            "Elevation_Door", "Elevation_Window"
        };

        public DataTable ElementsTableViewSectionParts
        {
            get
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("NAME", typeof(string));
                GetElementsTable(typeof(ViewSection), ElevationViewPrefixes, ref table);
                return table;
            }
        }

        public IList<Element> ElementsViewSectionParts
        {
            get
            {
                var types = new List<Type> { typeof(ViewSection) };
                return GetElementsDoc(null, types, null, ElevationViewPrefixes, null);
            }
        }

        public ViewSheet ActiveViewSheet
        {
            get
            {
                var view = GetActiveView(ViewType.DrawingSheet);
                return view as ViewSheet;
            }
        }

        public ElementType ViewPortSymbolNoTitle
        {
            get
            {
                var names = new List<string>
                {
                    _cmpAttribute.ResourceText("IDS_SET_VIEWPORTSYM_1"),
                    _cmpAttribute.ResourceText("IDS_SET_VIEWPORTSYM_2"),
                    _cmpAttribute.ResourceText("IDS_SET_VIEWPORTSYM_3")
                };
                var types = new List<Type> { typeof(ElementType) };
                var elems = GetElementsDoc(null, types, null, names, null);
                if (elems != null)
                {
                    foreach (var elem in elems)
                    {
                        if (elem.GetType() != typeof(ElementType)) continue;
                        var et = elem as ElementType;
                        if (et.FamilyName != LabelUtils.GetLabelFor(BuiltInCategory.OST_Viewports)) continue;
                        foreach (var name in names)
                        {
                            if (elem.Name == name) return et;
                        }
                    }
                }
                return null;
            }
        }

        public ElementType GetTagType(int id)
        {
            var elem = GetElementDoc(id);
            return elem as ElementType;
        }

        public ViewSection GetViewSection(int id)
        {
            var elem = GetElementDoc(id);
            return elem as ViewSection;
        }

        public IList<Element> GetViewPorts(string sheetName, IList<ViewSection> viewSections)
        {
            var result = new List<Element>();
            var viewNames = new List<string>();
            if (viewSections != null)
            {
                foreach (var vs in viewSections)
                    viewNames.Add(vs.Name);
            }

            var elems = GetElementsViewPorts(sheetName);
            if (elems != null)
            {
                foreach (var elem in elems)
                {
                    if (viewNames.Count > 0)
                    {
                        var param = elem.get_Parameter(BuiltInParameter.VIEW_NAME);
                        if (param != null)
                        {
                            var valStr = param.AsString();
                            if (valStr != null && viewNames.Contains(valStr))
                                result.Add(elem);
                        }
                    }
                    else
                    {
                        result.Add(elem);
                    }
                }
            }
            return result;
        }

        public void CompareViewOfSheet(ref DataTable table)
        {
            var views = GetUsedViewOfSheet();
            if (views != null && table != null && table.Rows.Count > 0)
            {
                for (int i = table.Rows.Count - 1; i >= 0; i--)
                {
                    bool keep = true;
                    var row = table.Rows[i];
                    string idStr = row[0].ToString();
                    if (int.TryParse(idStr, out int id) && id > -1)
                    {
                        foreach (var view in views)
                        {
                            if (id == Int32.Parse(view.Id.ToString()))
                            {
                                keep = false;
                                break;
                            }
                        }
                    }
                    if (!keep)
                        table.Rows.Remove(row);
                }
            }
        }
    }
}
