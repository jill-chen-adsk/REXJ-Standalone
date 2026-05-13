using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public class Elements
    {
        public UIDocument RvtUIDoc { get; }
        public Document RvtDBDoc => RvtUIDoc.Document;

        public ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

        public Elements(UIDocument rvtUIDoc)
        {
            RvtUIDoc = rvtUIDoc;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(RvtDBDoc, bic);
        }

        public Element GetElementDoc(int id)
        {
            return RvtDBDoc.GetElement(new ElementId((long)id));
        }

        public View GetActiveView(ViewType viewType)
        {
            View activeView = RvtDBDoc.ActiveView;
            if (activeView == null) return null;
            if (viewType == ViewType.AreaPlan && activeView is ViewPlan vp && vp.ViewType == ViewType.AreaPlan)
                return vp;
            return null;
        }

        public IList<Area> GetElementsArea(int _a, int _b, View view)
        {
            var col = view != null
                ? new FilteredElementCollector(RvtDBDoc, view.Id)
                : new FilteredElementCollector(RvtDBDoc);
            return col.OfCategory(BuiltInCategory.OST_Areas).WhereElementIsNotElementType().Cast<Area>().ToList();
        }

        public IList<Area> GetElementsArea(int _a, int _b)
        {
            return GetElementsArea(1, 1, null);
        }

        public IList<CurveElement> GetElementsCurveElement(View view, Category category)
        {
            if (view == null || category == null) return new List<CurveElement>();
            return new FilteredElementCollector(RvtDBDoc, view.Id)
                .OfCategoryId(category.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(CurveElement))
                .Cast<CurveElement>()
                .ToList();
        }

        public IList<CurveElement> GetElementsCurveElement(Category category)
        {
            return new FilteredElementCollector(RvtDBDoc)
                .OfCategoryId(category.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(CurveElement))
                .Cast<CurveElement>()
                .ToList();
        }

        public IList<Room> GetElementsRoom(int _a, int _b)
        {
            return new FilteredElementCollector(RvtDBDoc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .ToList();
        }

        public IList<Room> GetElementsRoom(int _a, int _b, Level level)
        {
            if (level == null)
                return GetElementsRoom(1, 1);
            return GetElementsRoom(1, 1).Where(r => r.LevelId == level.Id).ToList();
        }

        public IList<Room> GetElementsSelectionRoom(int _a, int _b, bool _) 
        {
            var list = new List<Room>();
            foreach (ElementId eid in RvtUIDoc.Selection.GetElementIds())
            {
                if (RvtDBDoc.GetElement(eid) is Room r)
                    list.Add(r);
            }
            return list;
        }

        public IList<Area> GetElementsSelectionArea(int _a, int _b, bool _)
        {
            var list = new List<Area>();
            foreach (ElementId eid in RvtUIDoc.Selection.GetElementIds())
            {
                if (RvtDBDoc.GetElement(eid) is Area a)
                    list.Add(a);
            }
            return list;
        }

        public void GetElementsTable(ElementIsElementTypeFilter filter, Type ofClassType, ref DataTable table)
        {
            foreach (Element e in new FilteredElementCollector(RvtDBDoc).WherePasses(filter).OfClass(ofClassType))
            {
                DataRow row = table.NewRow();
                row["ID"] = (int)e.Id.Value;
                row["NAME"] = e.Name;
                table.Rows.Add(row);
            }
        }

        public IList<GraphicsStyle> GetElementsGraphicsStyle(Category category)
        {
            return new FilteredElementCollector(RvtDBDoc)
                .OfClass(typeof(GraphicsStyle))
                .Cast<GraphicsStyle>()
                .Where(gs => gs.GraphicsStyleCategory?.Id == category.Id)
                .ToList();
        }

        public FamilySymbol GetAreaTagFamilySymbol(int id)
        {
            return GetElementDoc(id) as FamilySymbol;
        }

        public IList<Area> GetAreasOfView(View view)
        {
            return GetElementsArea(1, 1, view);
        }

        public IList<CurveElement> GetAreasCurveElemsOfView(View view)
        {
            Category category = GetCategory(BuiltInCategory.OST_AreaSchemeLines);
            return GetElementsCurveElement(view, category);
        }

        public ViewPlan ActiveViewAreaPlan =>
            GetActiveView(ViewType.AreaPlan) as ViewPlan;

        public IList<Area> Areas =>
            GetElementsArea(1, 1);

        public IList<CurveElement> AreasCurveElems
        {
            get
            {
                Category category = GetCategory(BuiltInCategory.OST_AreaSchemeLines);
                return GetElementsCurveElement(category);
            }
        }

        public IList<Room> Rooms =>
            GetElementsRoom(1, 1);

        public IList<Room> SelSetRooms =>
            GetElementsSelectionRoom(1, 1, true);

        public IList<Area> SelSetAreas =>
            GetElementsSelectionArea(1, 1, true);

        public DataTable TableAreaTag
        {
            get
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("NAME", typeof(string));

                var collector = new FilteredElementCollector(RvtDBDoc)
                    .OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_AreaTags);
                foreach (FamilySymbol fs in collector)
                {
                    DataRow row = table.NewRow();
                    row["ID"] = (int)fs.Id.Value;
                    row["NAME"] = fs.Name;
                    table.Rows.Add(row);
                }
                return table;
            }
        }

        public GraphicsStyle LineStyleHidden
        {
            get
            {
                Category category = GetCategory(BuiltInCategory.OST_LinesHiddenLines);
                IList<GraphicsStyle> styles = GetElementsGraphicsStyle(category);
                if (styles != null && styles.Count > 0)
                    return styles[styles.Count - 1];
                return null;
            }
        }

        public SketchPlane CreateSketchPlaneProjOrigin()
        {
            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);
            return SketchPlane.Create(RvtDBDoc, plane);
        }

        public Area CreateArea(ViewPlan areaView, XYZ roomPosition)
        {
            return RvtDBDoc.Create.NewArea(areaView, new UV(roomPosition.X, roomPosition.Y));
        }

        public AreaTag CreateAreaTag(ViewPlan viewPlan, Area area, XYZ areaPosition)
        {
            UV uv = new UV(areaPosition.X, areaPosition.Y);
            return RvtDBDoc.Create.NewAreaTag(viewPlan, area, uv);
        }

        public Room GetRoomInPoint(IList<Room> rooms, XYZ point)
        {
            if (rooms == null) return null;
            foreach (Room room in rooms)
            {
                if (room?.IsPointInRoom(point) == true)
                    return room;
            }
            return null;
        }

        public TextNote CreateTextNote(View view, XYZ position, double rotationRad,
            HorizontalTextAlignment horizontalTextAlign, string textContent)
        {
            ElementId typeId = new FilteredElementCollector(RvtDBDoc).OfClass(typeof(TextNoteType)).FirstElementId();
            if (typeId == ElementId.InvalidElementId)
                return null;
            TextNote note = TextNote.Create(RvtDBDoc, view.Id, position, textContent, typeId);
            return note;
        }

        public CurveElement CreateDetailCurve(View view, Curve curve, Element lineStyle)
        {
            CurveElement dc = RvtDBDoc.Create.NewDetailCurve(view, curve);
            if (dc != null && lineStyle != null)
            {
                try
                {
                    if (lineStyle is GraphicsStyle gs && gs.Id != null)
                        dc.LineStyle = gs;
                }
                catch { }
            }
            return dc;
        }
    }
}
