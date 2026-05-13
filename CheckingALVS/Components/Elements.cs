using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public sealed class PickFilter : ISelectionFilter
    {
        private readonly HashSet<ElementId> _catIds = new HashSet<ElementId>();
        private readonly List<Type> _types = new List<Type>();

        public PickFilter(IEnumerable<Type> sysTypes, IEnumerable<Category> categories)
        {
            if (categories != null)
            {
                foreach (var c in categories)
                {
                    if (c != null) _catIds.Add(c.Id);
                }
            }

            if (sysTypes != null)
                _types.AddRange(sysTypes);
        }

        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;

            if (_types.Count > 0)
            {
                bool typeOk = false;
                foreach (var t in _types)
                {
                    if (t != null && t.IsInstanceOfType(elem)) { typeOk = true; break; }
                }
                if (!typeOk) return false;
            }

            if (_catIds.Count > 0 && elem.Category != null && !_catIds.Contains(elem.Category.Id))
                return false;

            return true;
        }

        public bool AllowReference(Reference reference, XYZ position) => false;
    }

    public class Elements
    {
        protected readonly UIDocument _rvtUIDoc;

        public UIDocument RvtUIDoc => _rvtUIDoc;

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(RvtDBDoc, bic);
        }

        public Element GetElementDoc(int id)
        {
            return RvtDBDoc.GetElement(new ElementId((long)id));
        }

        /// <summary>
        /// Interactive multi/single selection (DtWinDoor.SelectWinDoorFromDraw).
        /// </summary>
        public virtual bool GetElementsSelection(IList<Type> sysTypes, IList<Category> categories, object unused,
            bool multiSelect, string prompt, ref IList<Element> selElems)
        {
            selElems = new List<Element>();
            try
            {
                var filter = new PickFilter(sysTypes, categories);

                if (multiSelect)
                {
                    IList<Reference> refs = _rvtUIDoc.Selection.PickObjects(ObjectType.Element, filter, prompt);
                    foreach (var r in refs)
                        selElems.Add(RvtDBDoc.GetElement(r.ElementId));
                }
                else
                {
                    Reference r = _rvtUIDoc.Selection.PickObject(ObjectType.Element, filter, prompt);
                    selElems.Add(RvtDBDoc.GetElement(r.ElementId));
                }
                return true;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
        }

        public int GetIdRoom(FamilyInstance familyInstance)
        {
            int id = -1;
            Room room = familyInstance?.Room;
            if (room != null)
                id = int.Parse(room.Id.ToString());
            return id;
        }

        public int GetIdFromRoom(FamilyInstance familyInstance)
        {
            int id = -1;
            Room room = familyInstance?.FromRoom;
            if (room != null)
                id = int.Parse(room.Id.ToString());
            return id;
        }

        public int GetIdToRoom(FamilyInstance familyInstance)
        {
            int id = -1;
            Room room = familyInstance?.ToRoom;
            if (room != null)
                id = int.Parse(room.Id.ToString());
            return id;
        }

        public Room GetRoom(int id)
        {
            Element elem = GetElementDoc(id);
            return elem as Room;
        }

        public FamilyInstance GetFamilyInstance(int id)
        {
            Element elem = GetElementDoc(id);
            return elem as FamilyInstance;
        }

        public int GetIdFamilySymbol(int id)
        {
            var familyInstance = GetFamilyInstance(id);
            FamilySymbol fs = familyInstance?.Symbol;
            if (familyInstance == null || fs == null) return 0;
            return int.Parse(fs.Id.ToString());
        }

        public IList<Room> GetElementsSelectionRoom(int minCountIgnored, int maxCountIgnored, bool optionIgnored)
        {
            var rooms = new List<Room>();
            foreach (ElementId eid in _rvtUIDoc.Selection.GetElementIds())
            {
                if (RvtDBDoc.GetElement(eid) is Room rm)
                    rooms.Add(rm);
            }
            return rooms;
        }

        public IList<FamilyInstance> GetElementsSelectionFamilyInstance(IList<Category> categories,
            bool includeSubTypesIgnored)
        {
            var set = new HashSet<ElementId>();
            if (categories != null)
                foreach (var c in categories)
                    if (c != null)
                        set.Add(c.Id);

            var ret = new List<FamilyInstance>();
            foreach (ElementId id in _rvtUIDoc.Selection.GetElementIds())
            {
                if (RvtDBDoc.GetElement(id) is FamilyInstance fi && fi.Category != null && set.Contains(fi.Category.Id))
                    ret.Add(fi);
            }
            return ret;
        }

        public IList<Element> GetElementsDoc(Type elementType)
        {
            var list = new List<Element>();
            foreach (Element e in new FilteredElementCollector(RvtDBDoc).OfClass(elementType))
                list.Add(e);
            return list;
        }

        public virtual bool GetElementLevelName(Element elem, ref string name)
        {
            name = "";
            if (elem == null) return false;
            try
            {
                if (elem is Room room && room.LevelId != ElementId.InvalidElementId)
                {
                    if (RvtDBDoc.GetElement(room.LevelId) is Level lvl)
                    {
                        name = lvl.Name;
                        return true;
                    }
                }
                if (elem.LevelId != null && elem.LevelId != ElementId.InvalidElementId &&
                    RvtDBDoc.GetElement(elem.LevelId) is Level lvl2)
                {
                    name = lvl2.Name;
                    return true;
                }
            }
            catch { }

            return false;
        }

        public virtual bool GetElementLevelElevation(FamilyInstance elem, ref double elevation)
        {
            elevation = 0;
            try
            {
                if (elem?.LevelId == null || elem.LevelId == ElementId.InvalidElementId) return false;
                if (RvtDBDoc.GetElement(elem.LevelId) is Level lvl)
                {
                    elevation = lvl.Elevation;
                    return true;
                }
            }
            catch { }

            return false;
        }

        public IList<PropertyLine> PropertyLines
        {
            get
            {
                IList<PropertyLine> ret = new List<PropertyLine>();
                foreach (Element e in GetElementsDoc(typeof(PropertyLine)))
                {
                    if (e is PropertyLine pl)
                        ret.Add(pl);
                }
                return ret;
            }
        }

        public IList<Level> Levels
        {
            get
            {
                IList<Level> ret = new List<Level>();
                foreach (Element elem in GetElementsDoc(typeof(Level)))
                {
                    if (elem is Level lv)
                        ret.Add(lv);
                }
                return ret;
            }
        }

        public IList<Room> SelSetRooms => GetElementsSelectionRoom(1, 1, true);

        public IList<FamilyInstance> SelSetWinDoor
        {
            get
            {
                var categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_Doors));
                categories.Add(GetCategory(BuiltInCategory.OST_Windows));
                return GetElementsSelectionFamilyInstance(categories, true);
            }
        }
    }
}
