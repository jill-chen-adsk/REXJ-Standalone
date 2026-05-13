using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.GridDimension.Components
{
    /// <summary>要素</summary>
    public class Elements
    {
        private readonly UIDocument _rvtUIDoc;

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }

        public UIDocument RvtUIDoc => _rvtUIDoc;

        public Document RvtDBDoc { get; }

        public ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

        /// <summary>アクティブ平面図または立面断面ビュー</summary>
        public View ActiveView
        {
            get
            {
                View view = null;
                View activeView = RvtDBDoc.ActiveView;
                if (activeView != null)
                {
                    if (activeView is ViewPlan)
                        view = activeView as ViewPlan;

                    if (activeView is ViewSection)
                        view = activeView as ViewSection;
                }
                return view;
            }
        }

        public Element GetElementDoc(int elementId)
        {
            return RvtDBDoc.GetElement(new ElementId((long)elementId));
        }

        /// <summary>選択セット - 通り芯</summary>
        public IList<Element> SelSetGrids
        {
            get
            {
                var sysTypes = new List<Type> { typeof(Grid), typeof(MultiSegmentGrid) };
                return GetElementsSelection(sysTypes, null, null, true);
            }
        }

        /// <summary>選択要素取得（型でフィルタ）</summary>
        public IList<Element> GetElementsSelection(IList<Type> sysTypes, IList<Category> categories, IList<string> names, bool groupMode)
        {
            var retElems = new List<Element>();
            ICollection<ElementId> selElemIds = _rvtUIDoc.Selection.GetElementIds();
            if (selElemIds == null)
                return retElems;

            foreach (ElementId elemId in selElemIds)
            {
                Element elem = RvtDBDoc.GetElement(elemId);
                if (elem == null)
                    continue;
                if (sysTypes == null || sysTypes.Count == 0)
                {
                    retElems.Add(elem);
                    continue;
                }
                foreach (Type t in sysTypes)
                {
                    if (t != null && t.IsInstanceOfType(elem))
                    {
                        retElems.Add(elem);
                        break;
                    }
                }
            }
            return retElems;
        }

        /// <summary>寸法作成</summary>
        public Dimension CreateDimension(View view, XYZ pos1, XYZ pos2, ReferenceArray refAry)
        {
            Line line = Line.CreateBound(pos1, pos2);
            return RvtDBDoc.Create.NewDimension(view, line, refAry);
        }

        /// <summary>通り芯を位置でソート — xyMode 1=X, 2=Y（昇順）</summary>
        public void SortGridsPoint(IList<Grid> grids, int xyMode, ref IList<int> sortedIndex, ref IList<Grid> sortedGridAry)
        {
            if (grids == null || grids.Count == 0)
                return;

            var idxList = sortedIndex as List<int> ?? new List<int>();
            var gridList = sortedGridAry as List<Grid> ?? new List<Grid>();
            idxList.Clear();
            gridList.Clear();
            sortedIndex = idxList;
            sortedGridAry = gridList;

            var sortPoints = new List<double>();
            var sortIdx = new List<int>();

            for (int i = 0; i < grids.Count; ++i)
            {
                Grid g = grids[i];
                if (g == null || g.Curve == null)
                    continue;

                double coord = 0.0;
                if (xyMode == 1)
                    coord = g.Curve.GetEndPoint(0).X;
                else if (xyMode == 2)
                    coord = g.Curve.GetEndPoint(0).Y;
                else
                    continue;

                if (sortIdx.Count == 0)
                {
                    sortIdx.Add(i);
                    sortPoints.Add(coord);
                }
                else
                {
                    int index = -1;
                    for (int j = 0; j < sortPoints.Count; ++j)
                    {
                        if (sortPoints[j] > coord)
                        {
                            index = j;
                            break;
                        }
                    }
                    if (index > -1)
                    {
                        sortIdx.Insert(index, i);
                        sortPoints.Insert(index, coord);
                    }
                    else
                    {
                        sortIdx.Add(i);
                        sortPoints.Add(coord);
                    }
                }
            }

            foreach (int ix in sortIdx)
            {
                idxList.Add(ix);
                gridList.Add(grids[ix]);
            }
        }

        /// <summary>GetAllGrid</summary>
        public IList<Element> GetAllGrids(out IList<Element> lstSegmentGrids)
        {
            IList<Element> retVal = new List<Element>();
            lstSegmentGrids = new List<Element>();
            foreach (var ele in SelSetGrids)
            {
                if (ele is Grid grid)
                {
                    if (retVal.Any(x => x.Id == grid.Id))
                        continue;
                    retVal.Add(grid);
                }
                if (ele is MultiSegmentGrid multiSegment)
                {
                    if (multiSegment == null)
                        continue;
                    lstSegmentGrids.Add(ele);

                    var listGrid = multiSegment.GetGridIds();
                    foreach (var elId in listGrid)
                    {
                        Grid g = _rvtUIDoc.Document.GetElement(elId) as Grid;
                        if (g == null)
                            continue;
                        if (retVal.Any(x => x.Id == elId))
                            continue;
                        retVal.Add(g);
                    }
                }
            }

            return retVal;
        }
    }
}
