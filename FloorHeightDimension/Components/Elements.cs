using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
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

        public ViewSection ActiveViewSection
        {
            get
            {
                View activeView = RvtDBDoc.ActiveView;
                return activeView as ViewSection;
            }
        }

        public IList<Element> SelSetLevels
        {
            get
            {
                var sysTypes = new List<Type> { typeof(Level) };
                return GetElementsSelection(sysTypes, null, null, true);
            }
        }

        public IList<Element> GetElementsSelection(IList<Type> sysTypes, IList<Category> categories, object unused, bool prompt)
        {
            var ret = new List<Element>();
            var selIds = RvtUIDoc.Selection.GetElementIds();

            if (selIds.Count == 0 && prompt)
            {
                try
                {
                    var refs = RvtUIDoc.Selection.PickObjects(ObjectType.Element);
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

        private static bool MatchFilters(Element elem, IList<Type> sysTypes, IList<Category> categories)
        {
            bool typeMatch = sysTypes == null || sysTypes.Count == 0;
            if (!typeMatch)
            {
                foreach (var t in sysTypes)
                {
                    if (t != null && t.IsInstanceOfType(elem)) { typeMatch = true; break; }
                }
            }
            if (!typeMatch) return false;

            bool catMatch = categories == null || categories.Count == 0;
            if (!catMatch && elem.Category != null)
            {
                foreach (var c in categories)
                {
                    if (c != null && elem.Category.Id == c.Id) { catMatch = true; break; }
                }
            }
            return catMatch;
        }

        public SketchPlane CreateSketchPlane(Plane plane)
        {
            return SketchPlane.Create(RvtDBDoc, plane);
        }

        public void SortLevelsHeight(IList<Level> levelAry, int mode, ref IList<int> sortedIndex, ref IList<Level> sortedLevelAry)
        {
            if (levelAry == null || levelAry.Count == 0)
            {
                sortedIndex = new List<int>();
                sortedLevelAry = new List<Level>();
                return;
            }

            var pairs = Enumerable.Range(0, levelAry.Count).Select(i => (idx: i, lvl: levelAry[i])).ToList();
            var ordered = mode == 1
                ? pairs.OrderBy(x => x.lvl.Elevation).ToList()
                : pairs.OrderByDescending(x => x.lvl.Elevation).ToList();

            sortedIndex = ordered.Select(x => x.idx).ToList();
            sortedLevelAry = ordered.Select(x => x.lvl).ToList();
        }

        public Dimension CreateDimension(View view, XYZ pos1, XYZ pos2, ReferenceArray refAry)
        {
            try
            {
                Line line = Line.CreateBound(pos1, pos2);
                return RvtDBDoc.Create.NewDimension(view, line, refAry);
            }
            catch
            {
                return null;
            }
        }
    }
}
