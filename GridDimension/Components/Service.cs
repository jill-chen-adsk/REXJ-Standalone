using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;
using System.Linq;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.JExtRAC.GridDimension.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class Service
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>エラーメッセージ</summary>
        private string _ErrMsg;

        /// <summary>Number of run</summary>
        private int _RunTime = 0;

        /// <summary>Distan dimension to start point</summary>
        private double _Distan = 0.0;

        /// <summary>Document</summary>
        private Revit.DB.Document _Doc;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2011/11/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Service(Revit.DB.Document doc,
                       RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings)
        {
            // 初期化
            _Doc = doc;
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;

            _ErrMsg = "";
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        private static int CurveOverlapCount(Curve a, Curve b)
        {
            if (a == null || b == null)
                return 0;
            try
            {
                CurveIntersectResult r = a.Intersect(b, CurveIntersectResultOption.Detailed);
                if (r == null || r.Result == SetComparisonResult.Disjoint)
                    return 0;
                IList<CurveOverlapPoint> ov = r.GetOverlaps();
                return ov?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private static XYZ CurveFirstOverlapPoint(Curve a, Curve b)
        {
            if (a == null || b == null)
                return null;
            try
            {
                CurveIntersectResult r = a.Intersect(b, CurveIntersectResultOption.Detailed);
                if (r == null || r.Result == SetComparisonResult.Disjoint)
                    return null;
                IList<CurveOverlapPoint> ov = r.GetOverlaps();
                if (ov == null || ov.Count == 0)
                    return null;
                return ov[0].Point;
            }
            catch
            {
                return null;
            }
        }

        /// ================================================================================
        /// <summary>寸法作成</summary>
        ///
        /// <param name="checkedView">Checked view</param>
        /// <param name="elemGrids"     >通り芯</param>
        /// <param name="strDistA"      >距離A</param>
        /// <param name="strDistB"      >距離B</param>
        /// /// <param name="selectedDimensionType">Dimension type</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/11/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/04/03 Modified GSA,Inc. Ryo Kuroda</p>
        ///          <p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        public bool CreateDimension(Revit.DB.View checkedView, Revit.DB.Plane plane,
                           Collections.Generic.IList<Revit.DB.Element> elemGrids,
                           string strDistA,
                           string strDistB,
                           Revit.DB.DimensionType selectedDimensionType, bool isDirectionX,
                           bool left, bool right, bool top, bool bottom)
        {
            // 戻り値
            bool ret = false;
            _ErrMsg = "";

            Collections.Generic.List<Revit.DB.Element> elemGridColl = new Revit.DB.FilteredElementCollector(checkedView.Document, checkedView.Id).
                OfCategory(Revit.DB.BuiltInCategory.OST_Grids).ToElements().Where(item => elemGrids.Where(item2 => item2.Id == item.Id).ToList().Count != 0).ToList();

            if (elemGridColl.Count == 0)
                return ret;
            // 距離
            double distA = 0.0;
            if (double.TryParse(strDistA, out double distAParsed))
                distA = distAParsed;

            distA /= _CmpGeometry.UnitCoe;

            double distB = 0.0;
            if (double.TryParse(strDistB, out double distBParsed))
                distB = distBParsed;

            distB /= _CmpGeometry.UnitCoe;

            // ビュー縮尺
            double viewScale = checkedView.Scale * 1.0;
            distA *= viewScale;
            distB *= viewScale;

            // 通り芯
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Grid>> gridAryAry =
              new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Grid>>();
            for (int i = 0; i < 2; ++i)
            {
                Collections.Generic.IList<Revit.DB.Grid> gridAry = new Collections.Generic.List<Revit.DB.Grid>();
                gridAryAry.Add(gridAry);
            }

            for (int i = 0; i < elemGridColl.Count; ++i)
            {
                Revit.DB.Grid grid = elemGridColl[i] as Revit.DB.Grid;

                if (grid != null)
                {
                    // タイプ
                    Revit.DB.ElementId typeId = grid.GetTypeId();
                    Revit.DB.GridType gridType = _CmpElements.GetElementDoc(Int32.Parse(typeId.ToString())) as Revit.DB.GridType;
                    if (gridType != null)
                    {
                        bool end1 = false;
                        if (_CmpParameters.GetValue(gridType,
                                                    Revit.DB.BuiltInParameter.GRID_BUBBLE_END_1,
                                                    ref end1) < -1)
                        {
                        }
                        bool end2 = false;
                        if (_CmpParameters.GetValue(gridType,
                                                    Revit.DB.BuiltInParameter.GRID_BUBBLE_END_2,
                                                    ref end2) < -1)
                        {
                        }

                        if (end1 == true)
                        {
                            gridAryAry[0].Add(grid);
                        }
                        if (end2 == true)
                        {
                            gridAryAry[1].Add(grid);
                        }
                    }
                }
            }

            // 方向
            Collections.Generic.IList<int> sortModeAry = new Collections.Generic.List<int>();
            for (int i = 0; i < gridAryAry.Count; ++i)
            {
                int sortMode = 0;
                if (gridAryAry[i].Count > 1)
                {
                    Revit.DB.Curve curve = gridAryAry[i][0].Curve;
                    Revit.DB.XYZ unitVec = _CmpGeometry.UnitVector(curve.GetEndPoint(0), curve.GetEndPoint(1));
                    Revit.DB.XYZ unitVecAbs = new Revit.DB.XYZ(System.Math.Abs(unitVec.X), System.Math.Abs(unitVec.Y), System.Math.Abs(unitVec.Z));
                    sortMode = 1;
                    if (_CmpGeometry.Distance2D(unitVecAbs, new Revit.DB.XYZ(1.0, 0.0, 0.0)) < _CmpGeometry.Approx0Len)
                    {
                        sortMode = 2;
                    }
                }
                sortModeAry.Add(sortMode);
            }

            // 通り芯並び替え
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Grid>> sortedGridAryAry =
                new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Grid>>();
            for (int i = 0; i < gridAryAry.Count; ++i)
            {
                Collections.Generic.IList<Revit.DB.Grid> sortedGridAry = new Collections.Generic.List<Revit.DB.Grid>();
                if (sortModeAry[i] > 0)
                {
                    Collections.Generic.IList<Revit.DB.Grid> gridAry = gridAryAry[i];
                    Collections.Generic.IList<int> sortedIndex = new Collections.Generic.List<int>();
                    _CmpElements.SortGridsPoint(gridAry, sortModeAry[i], ref sortedIndex, ref sortedGridAry);
                }
                sortedGridAryAry.Add(sortedGridAry);
            }

            // 寸法作成
            for (int i = 0; i < sortedGridAryAry.Count; ++i)
            {
                Collections.Generic.IList<Revit.DB.Grid> gridAry = sortedGridAryAry[i];
                if (gridAry.Count < 2)
                    continue;

                Revit.DB.ReferenceArray refAry1 = new Revit.DB.ReferenceArray();
                Revit.DB.ReferenceArray refAry2 = new Revit.DB.ReferenceArray();

                int gridNo = gridAry.Count - 1;
                refAry1.Append(new Revit.DB.Reference(gridAry[0]));
                refAry1.Append(new Revit.DB.Reference(gridAry[gridNo]));

                Collections.Generic.List<Revit.DB.Grid> lstGridsArr = null;
                Collections.Generic.Dictionary<string, Collections.Generic.List<Revit.DB.Grid>> dicGrids = GetAllGrid(checkedView, gridAry.ToList(), isDirectionX);
                foreach (var pair in dicGrids)
                {
                    if (pair.Value == null || pair.Value.Count == 1)
                    {
                        lstGridsArr = new Collections.Generic.List<Revit.DB.Grid>();
                        lstGridsArr = pair.Value;
                        refAry2.Append(new Revit.DB.Reference(lstGridsArr[0]));
                    }
                }
                Revit.DB.Curve curve1 = null;

                Collections.Generic.IList<Revit.DB.Curve> listCurve1 = gridAry[0].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, checkedView);

                foreach (Revit.DB.Curve item in listCurve1)
                    curve1 = item;

                if (curve1 == null)
                    continue;

                Revit.DB.XYZ uvGridPosi = _CmpGeometry.UnitVector(curve1.GetEndPoint(1), curve1.GetEndPoint(0));
                Revit.DB.XYZ uvGridOppo = new Revit.DB.XYZ(uvGridPosi.X * -1.0, uvGridPosi.Y * -1.0, uvGridPosi.Z * -1.0);

                Revit.DB.Curve curve2 = null;

                Collections.Generic.IList<Revit.DB.Curve> listCurve2 = gridAry[gridNo].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, checkedView);

                foreach (var item in listCurve2)
                    curve2 = item;

                if (curve2 == null)
                    continue;

                Revit.DB.XYZ posVert = _CmpGeometry.GetVerticalPos3D(curve2, curve1.GetEndPoint(0));
                Revit.DB.XYZ uvDimPosi = _CmpGeometry.UnitVector(curve1.GetEndPoint(0), posVert);

                double dist = _CmpGeometry.Distance(curve1.GetEndPoint(0), posVert);

                double p1x = 0, p1y = 0, p1z = 0;
                double p2x = 0, p2y = 0, p2z = 0;

                // 通り芯記号半径取得
                double radius = 0.0;
                double lenA = radius + distA;
                double lenB = radius + distB;

                //
                Collections.Generic.List<Revit.DB.XYZ> lstPoint = new Collections.Generic.List<Revit.DB.XYZ>();
                SortStarPointEndPoint(checkedView, plane, gridAry, isDirectionX, ref lstPoint);
                Collections.Generic.List<Revit.DB.XYZ> lstSortPoint = null;
                int countPoint = 0;
                Revit.DB.XYZ pMin1 = null;
                Revit.DB.XYZ pMax1 = null;
                bool start = false;
                bool end = false;
                if (isDirectionX)
                {
                    // sort point by X
                    start = left;
                    end = right;
                    lstSortPoint = lstPoint.OrderBy(x => x.X).ToList();
                    countPoint = lstSortPoint.Count;
                    pMin1 = lstSortPoint[0];
                    pMax1 = lstSortPoint[countPoint - 1];
                }
                else
                {
                    // sort point by Y
                    start = top;
                    end = bottom;
                    lstSortPoint = lstPoint.OrderBy(y => y.Y).ToList();
                    countPoint = lstSortPoint.Count;
                    pMin1 = lstSortPoint[0];
                    pMax1 = lstSortPoint[countPoint - 1];
                }

                //Create dimensions at start poin
                if (start)
                {
                    Revit.DB.XYZ pos0 = curve1.GetEndPoint(1);
                    Revit.DB.XYZ uvGrid = uvGridPosi;
                    bool checkDirection = CheckDirection(gridAry[0], checkedView, isDirectionX);
                    bool paralel = CheckParallel(gridAry[0], checkedView, isDirectionX);
                    if (isDirectionX)
                    {
                        pos0 = new Revit.DB.XYZ(pMin1.X, curve1.GetEndPoint(1).Y, curve1.GetEndPoint(1).Z);
                        if (!checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                            uvGrid = uvGridOppo;
                    }
                    else
                    {
                        if (paralel)
                        {
                            pos0 = new Revit.DB.XYZ(curve1.GetEndPoint(1).X, pMax1.Y, curve1.GetEndPoint(1).Z);
                            if (checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                                uvGrid = uvGridOppo;
                        }
                        else
                        {
                            if (CheckDirection(gridAry[0]))
                            {
                                pos0 = curve1.GetEndPoint(0);
                                uvGrid = uvGridOppo;
                            }
                        }
                    }

                    if (i == 0)
                    {
                        pos0 = curve1.GetEndPoint(0);

                        if (isDirectionX)
                        {
                            uvGrid = uvGridPosi;

                            pos0 = new Revit.DB.XYZ(pMin1.X, curve1.GetEndPoint(0).Y, curve1.GetEndPoint(0).Z);
                            if (!checkDirection)
                                uvGrid = uvGridOppo;
                        }
                        else
                        {
                            if (paralel)
                            {
                                uvGrid = uvGridOppo;

                                pos0 = new Revit.DB.XYZ(curve1.GetEndPoint(0).X, pMax1.Y, curve1.GetEndPoint(0).Z);
                                if (!checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                                    uvGrid = uvGridPosi;
                            }
                            else
                            {
                                uvGrid = uvGridPosi;
                                pos0 = curve1.GetEndPoint(1);
                                if (CheckDirection(gridAry[0]))
                                {
                                    uvGrid = uvGridOppo;
                                    pos0 = curve1.GetEndPoint(0);
                                }
                            }
                        }
                    }

                    p1x = pos0.X + (uvGrid.X * lenA);
                    p1y = pos0.Y + (uvGrid.Y * lenA);
                    p1z = pos0.Z + (uvGrid.Z * lenA);

                    if (_RunTime == 0)
                    {
                        Revit.DB.XYZ xyzOnLine = _CmpGeometry.GetVerticalPos3D(curve1, new Revit.DB.XYZ(p1x, p1y, p1z));

                        if (xyzOnLine != null)
                        {
                            _Distan = _CmpGeometry.Distance(xyzOnLine, curve1.GetEndPoint(1));
                            _Distan /= viewScale;
                        }
                        else
                            _Distan = 0;
                    }
                    else
                    {
                        if (_Distan != 0.0)
                        {
                            Revit.DB.XYZ xyNew = _CmpGeometry.GetPointDistance(curve1.GetEndPoint(0), curve1.GetEndPoint(1), _Distan * viewScale);
                            p1x = xyNew.X;
                            p1y = xyNew.Y;
                            p1z = xyNew.Z;
                        }
                    }

                    p2x = p1x + (uvDimPosi.X * dist);
                    p2y = p1y + (uvDimPosi.Y * dist);
                    p2z = p1z + (uvDimPosi.Z * dist);

                    // coordinates of point 1
                    Revit.DB.XYZ pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);

                    // coordinates of point 2
                    Revit.DB.XYZ pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);

                    Revit.DB.Dimension dimen1 = _CmpElements.CreateDimension(checkedView, pos1, pos2, refAry1);
                    if (dimen1 != null)
                        dimen1.DimensionType = selectedDimensionType;

                    p1x = p1x + uvGrid.X * lenB;
                    p1y = p1y + uvGrid.Y * lenB;
                    p1z = p1z + uvGrid.Z * lenB;

                    p2x = p1x + (uvDimPosi.X * dist);
                    p2y = p1y + (uvDimPosi.Y * dist);
                    p2z = p1z + (uvDimPosi.Z * dist);

                    // coordinates of point 1
                    pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);

                    // coordinates of point 2
                    pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);

                    Revit.DB.Dimension dimen2 = _CmpElements.CreateDimension(checkedView, pos1, pos2, refAry2);
                    if (dimen2 != null)
                        dimen2.DimensionType = selectedDimensionType;
                }
                // Create dimensions at end point
                if (end)
                {
                    Revit.DB.XYZ pos0 = curve1.GetEndPoint(0);
                    Revit.DB.XYZ uvGrid = uvGridPosi;
                    bool checkDirection = CheckDirection(gridAry[0], checkedView, isDirectionX);
                    bool paralel = CheckParallel(gridAry[0], checkedView, isDirectionX);
                    if (isDirectionX)
                    {
                        uvGrid = uvGridPosi;
                        pos0 = new Revit.DB.XYZ(pMax1.X, curve1.GetEndPoint(0).Y, curve1.GetEndPoint(0).Z);
                        if (!checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                            uvGrid = uvGridOppo;
                    }
                    else
                    {
                        if (paralel)
                        {
                            uvGrid = uvGridPosi;

                            pos0 = new Revit.DB.XYZ(curve1.GetEndPoint(0).X, pMin1.Y, curve1.GetEndPoint(0).Z);
                            if (checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                                uvGrid = uvGridOppo;
                        }
                        else
                        {
                            if (CheckDirection(gridAry[0]))
                            {
                                pos0 = curve1.GetEndPoint(1);
                                uvGrid = uvGridOppo;
                            }
                        }
                    }

                    if (i == 0)
                    {
                        pos0 = curve1.GetEndPoint(1);
                        if (isDirectionX)
                        {
                            uvGrid = uvGridPosi;
                            pos0 = new Revit.DB.XYZ(pMax1.X, curve1.GetEndPoint(1).Y, curve1.GetEndPoint(1).Z);
                            if (!checkDirection)
                                uvGrid = uvGridOppo;
                        }
                        else
                        {
                            if (paralel)
                            {
                                uvGrid = uvGridOppo;
                                pos0 = new Revit.DB.XYZ(curve1.GetEndPoint(1).X, pMin1.Y, curve1.GetEndPoint(1).Z);
                                if (!checkDirection && checkedView.ViewType != Revit.DB.ViewType.Elevation)
                                    uvGrid = uvGridPosi;
                            }
                            else
                            {
                                uvGrid = uvGridPosi;
                                pos0 = curve1.GetEndPoint(0);
                                if (CheckDirection(gridAry[0]))
                                {
                                    uvGrid = uvGridOppo;
                                    pos0 = curve1.GetEndPoint(1);
                                }
                            }
                        }
                    }

                    p1x = pos0.X - (uvGrid.X * lenA);
                    p1y = pos0.Y - (uvGrid.Y * lenA);
                    p1z = pos0.Z - (uvGrid.Z * lenA);

                    if (_RunTime == 0)
                    {
                        Revit.DB.XYZ xyzOnLine = _CmpGeometry.GetVerticalPos3D(curve1, new Revit.DB.XYZ(p1x, p1y, p1z));

                        if (xyzOnLine != null)
                        {
                            _Distan = _CmpGeometry.Distance(xyzOnLine, curve1.GetEndPoint(1));
                            _Distan /= viewScale;
                        }
                        else
                            _Distan = 0;
                    }
                    else
                    {
                        if (_Distan != 0.0)
                        {
                            Revit.DB.XYZ xyNew = _CmpGeometry.GetPointDistance(curve1.GetEndPoint(0), curve1.GetEndPoint(1), _Distan * viewScale);
                            p1x = xyNew.X;
                            p1y = xyNew.Y;
                            p1z = xyNew.Z;
                        }
                    }

                    p2x = p1x - (uvDimPosi.X * dist);
                    p2y = p1y - (uvDimPosi.Y * dist);
                    p2z = p1z - (uvDimPosi.Z * dist);

                    // coordinates of point 1
                    Revit.DB.XYZ pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);

                    // coordinates of point 2
                    Revit.DB.XYZ pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);

                    Revit.DB.Dimension dimen1 = _CmpElements.CreateDimension(checkedView, pos1, pos2, refAry1);
                    if (dimen1 != null)
                        dimen1.DimensionType = selectedDimensionType;

                    p1x = p1x - uvGrid.X * lenB;
                    p1y = p1y - uvGrid.Y * lenB;
                    p1z = p1z - uvGrid.Z * lenB;

                    p2x = p1x - (uvDimPosi.X * dist);
                    p2y = p1y - (uvDimPosi.Y * dist);
                    p2z = p1z - (uvDimPosi.Z * dist);

                    // coordinates of point 1
                    pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);

                    // coordinates of point 2
                    pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);

                    Revit.DB.Dimension dimen2 = _CmpElements.CreateDimension(checkedView, pos1, pos2, refAry2);
                    if (dimen2 != null)
                        dimen2.DimensionType = selectedDimensionType;
                }
            }
            _RunTime++;
            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>寸法作成</summary>
        ///
        /// <param name="checkedView">Checked view</param>
        /// <param name="elemGrids"     >通り芯</param>
        /// <param name="strDistB"      >距離B</param>
        /// /// <param name="selectedDimensionType">Dimension type</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/11/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/04/03 Modified GSA,Inc. Ryo Kuroda</p></history>
        ///          <p>2018/11/12 Modified Applied Technology</p></history>
        /// ================================================================================
        public bool CreateDimensionCurve(Revit.DB.View checkedView,
                        System.Collections.Generic.IList<Revit.DB.Element> elemGrids,
                        string strDistB,
                        Revit.DB.DimensionType selectedDimensionType, bool left, bool right, bool top, bool bottom)
        {
            _ErrMsg = "";

            System.Collections.Generic.List<Revit.DB.Element> elemGridColl = new Revit.DB.FilteredElementCollector(checkedView.Document, checkedView.Id).
                OfCategory(Revit.DB.BuiltInCategory.OST_Grids).ToElements().Where(item => elemGrids.Any(item2 => item2.Id == item.Id)).ToList();

            if (elemGridColl.Count == 0)
                return false;

            // 距離

            double distB = 1;
            if (double.TryParse(strDistB, out double distBCurve))
            {
                distB = distBCurve;
                if (distB < 1)
                    distB = 1;
            }

            distB /= _CmpGeometry.UnitCoe;

            // ビュー縮尺
            double viewScale = checkedView.Scale;

            distB *= viewScale;

            // 通り芯
            System.Collections.Generic.IList<System.Collections.Generic.IList<Revit.DB.Grid>> gridAryAry =
              new System.Collections.Generic.List<System.Collections.Generic.IList<Revit.DB.Grid>>();
            for (int i = 0; i < 2; ++i)
            {
                System.Collections.Generic.IList<Revit.DB.Grid> gridAry = new System.Collections.Generic.List<Revit.DB.Grid>();
                gridAryAry.Add(gridAry);
            }

            // Filter grid curve and line
            FilterGridArcAndLine(elemGrids, out System.Collections.Generic.List<Revit.DB.Grid> lstGridLine, out System.Collections.Generic.List<Revit.DB.Grid> lstGridArc);
            if (lstGridArc.Count == 0 || lstGridLine.Count == 0)
                return false;
            // Check number point
            if (!CheckNumberPointIntersection(checkedView, lstGridLine, lstGridArc, left, right, top, bottom))
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_HAS_ONE_INTERSECTION"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"));
                return false;
            }

            // Create dim on line
            CreateDimOnLine(checkedView.Document, checkedView, lstGridLine, lstGridArc, selectedDimensionType, distB, left, right);

            // Create dim arc
            CreateDimOnAcr(checkedView.Document, checkedView, lstGridLine, lstGridArc, selectedDimensionType, distB, top, bottom);

            return true;
        }

        /// ================================================================================
        /// <summary>GetSegmentGrids</summary>
        ///
        /// <param name="checkedView"></param>
        /// <param name="lstGridLine"></param>
        /// <returns></returns>
        ///
        /// <history><p>2021/11/12 Modified Applied Technology</p></history>
        ///  ================================================================================
        private Dictionary<string, List<Revit.DB.Grid>> GetSegmentGrids(Revit.DB.View checkedView, List<Revit.DB.Grid> lstGridLine)
        {
            Dictionary<string, List<Revit.DB.Grid>> retVal = new Dictionary<string, List<Revit.DB.Grid>>();
            Revit.DB.FilteredElementCollector collector = new Revit.DB.FilteredElementCollector(checkedView.Document, checkedView.Id);
            List<Revit.DB.Grid> segmentGrids = null;
            foreach (var grid in lstGridLine)
            {
                segmentGrids = new List<Grid>();
                if (grid == null)
                    continue;

                if (retVal.ContainsKey(grid.Name))
                    continue;

                Revit.DB.MultiSegmentGrid multiSegment = collector
                                .OfCategory(BuiltInCategory.OST_GridChains)
                                .Cast<Revit.DB.MultiSegmentGrid>()
                                .FirstOrDefault(x => x.Name == grid.Name);
                if (multiSegment != null)
                {
                    var lst = multiSegment.GetGridIds();
                    foreach (var item in lst)
                    {
                        if (item == Revit.DB.ElementId.InvalidElementId)
                            continue;
                        Revit.DB.Grid gr = checkedView.Document.GetElement(item) as Revit.DB.Grid;
                        segmentGrids.Add(gr);
                    }
                }
                else
                {
                    segmentGrids.Add(grid);
                }

                retVal.Add(grid.Name, segmentGrids);
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Get length between two point on curve</summary>
        ///
        /// <param name="curve">curve</param>
        /// <param name="pPick">point intersection</param>
        /// <param name="length">length</param>
        /// <param name="isClockWise">isClockWise</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================

        private bool GetCurveLength(Curve curve, XYZ pPick, out double length, bool isClockWise)
        {
            length = 0;

            try
            {
                if (curve == null)
                    return false;

                Arc arcTemp = curve as Arc;
                if (arcTemp == null)
                    return false;

                double evaluateVal = 1;
                List<XYZ> lstP = new List<XYZ>();

                XYZ startP = curve.GetEndPoint(0);
                XYZ pCenter = XYZ.Zero;

                // Point same start point
                if (pPick.IsAlmostEqualTo(startP))
                    return true;

                // Point is end of curve
                if (pPick.IsAlmostEqualTo(curve.GetEndPoint(1)))
                {
                    length = arcTemp.Length;
                    return true;
                }

                while (true)
                {
                    evaluateVal = evaluateVal / 2;
                    pCenter = arcTemp.Evaluate(evaluateVal, true);
                    if (pPick.IsAlmostEqualTo(pCenter))
                    {
                        evaluateVal = evaluateVal / 2;
                        pCenter = arcTemp.Evaluate(evaluateVal, true);
                    }

                    lstP.Clear();
                    lstP.Add(startP);
                    lstP.Add(pCenter);
                    lstP.Add(pPick);

                    if (isClockWise)
                    {
                        if (IsClockwise(lstP))
                            break;
                    }
                    else
                    {
                        if (!IsClockwise(lstP))
                            break;
                    }

                    if (startP.DistanceTo(pCenter) < 0.000001)
                        return false;
                }

                Arc arc = Arc.Create(startP, pPick, pCenter);
                if (arc == null)
                    return false;

                length = arc.Length;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return false;
            }

            return true;
        }

        /// ================================================================================
        /// <summary>Check the points are clockwise</summary>
        ///
        /// <param name="vertices">list point</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/28 Created Applied Technology</p></history>
        /// ================================================================================
        public static bool IsClockwise(List<XYZ> vertices)
        {
            double sum = 0.0;
            for (int i = 0; i < vertices.Count; i++)
            {
                XYZ v1 = vertices[i];
                XYZ v2 = vertices[(i + 1) % vertices.Count]; // % is the modulo operator
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum > 0.0;
        }

        /// ================================================================================
        /// <summary>Create dim on grid is line</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="checkedView">view draw</param>
        /// <param name="lstGridLine">List grid is line</param>
        /// <param name="lstGridArc">List grid is arc </param>
        /// <param name="selectedDimensionType">type dimension</param>
        /// <param name="offset">distance offset</param>
        /// <param name="top">draw top</param>
        /// <param name="bottom">draw bottom</param>
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private void CreateDimOnAcr(Revit.DB.Document doc, Revit.DB.View checkedView, System.Collections.Generic.List<Revit.DB.Grid> lstGridLine, System.Collections.Generic.List<Revit.DB.Grid> lstGridArc, Revit.DB.DimensionType selectedDimensionType, double offset, bool top, bool bottom)
        {
            if (!bottom && !top)
                return;

            List<XYZ> listPoint = new List<XYZ>();

            // Get list grid
            List<Grid> listSegmentGrid = new List<Grid>();
            var retVal = GetSegmentGrids(checkedView, lstGridLine);
            foreach (var pair in retVal)
            {
                foreach (var grid in pair.Value)
                {
                    listSegmentGrid.Add(grid);
                }
            }
            foreach (var gridArc in lstGridArc)
            {
                var lstIntersection = GetIntersection(gridArc, listSegmentGrid, false);
                if (lstIntersection.Count < 2)
                    continue;

                for (int i = 0; i < lstIntersection.Count - 1; i++)
                {
                    // Current intersection
                    Revit.DB.XYZ currentIntersection = lstIntersection[i];

                    // Next intersection
                    Revit.DB.XYZ nextIntersection = lstIntersection[i + 1];

                    if (currentIntersection.IsAlmostEqualTo(nextIntersection))
                        continue;

                    // Create detail line
                    var detailLine = CreateDetailLine(doc, checkedView, currentIntersection, nextIntersection);
                    if (detailLine == null)
                        continue;

                    if (detailLine.GeometryCurve == null)
                        continue;

                    // Get direction of line
                    Revit.DB.XYZ dirLine = detailLine.GeometryCurve.GetEndPoint(1) - detailLine.GeometryCurve.GetEndPoint(0);

                    // Create dim the top of the curve
                    Revit.DB.XYZ pos1 = detailLine.GeometryCurve.GetEndPoint(0);
                    Revit.DB.XYZ pos2 = detailLine.GeometryCurve.GetEndPoint(1);
                    if (top)
                    {
                        Revit.DB.XYZ dirCross = dirLine.CrossProduct(checkedView.ViewDirection).Negate();

                        Revit.DB.XYZ pos1Offset = _CmpGeometry.GetPointOnVector(pos1, dirCross, offset);
                        Revit.DB.DetailLine line1 = CreateDetailLine(doc, checkedView, pos1, pos1Offset);

                        Revit.DB.XYZ pos2Offset = _CmpGeometry.GetPointOnVector(pos2, dirCross, offset);
                        Revit.DB.DetailLine line2 = CreateDetailLine(doc, checkedView, pos2, pos2Offset);

                        Revit.DB.ReferenceArray lstRef = new Revit.DB.ReferenceArray();
                        lstRef.Append(line1.GeometryCurve.Reference);
                        lstRef.Append(line2.GeometryCurve.Reference);
                        Revit.DB.Dimension dimen = _CmpElements.CreateDimension(checkedView, pos2Offset, pos1Offset, lstRef);
                        if (dimen != null)
                            dimen.DimensionType = selectedDimensionType;
                    }

                    // Create dim the bottom of the curve
                    if (bottom)
                    {
                        Revit.DB.XYZ dirCross = dirLine.CrossProduct(checkedView.ViewDirection);

                        Revit.DB.XYZ pos1Offset = _CmpGeometry.GetPointOnVector(pos1, dirCross, offset);
                        Revit.DB.DetailLine line1 = CreateDetailLine(doc, checkedView, pos1, pos1Offset);

                        Revit.DB.XYZ pos2Offset = _CmpGeometry.GetPointOnVector(pos2, dirCross, offset);
                        Revit.DB.DetailLine line2 = CreateDetailLine(doc, checkedView, pos2, pos2Offset);

                        Revit.DB.ReferenceArray lstRef = new Revit.DB.ReferenceArray();
                        lstRef.Append(line1.GeometryCurve.Reference);
                        lstRef.Append(line2.GeometryCurve.Reference);
                        Revit.DB.Dimension dimen = _CmpElements.CreateDimension(checkedView, pos1Offset, pos2Offset, lstRef);
                        if (dimen != null)
                            dimen.DimensionType = selectedDimensionType;
                    }

                    // Delete unnecessary detail line
                    doc.Delete(detailLine.Id);

                }
            }
        }

        /// ================================================================================
        /// <summary>Create dim on grid is arc</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="checkedView">view draw</param>
        /// <param name="lstGridLine">List grid is line</param>
        /// <param name="lstGridArc">List grid is arc </param>
        /// <param name="selectedDimensionType">type dimension</param>
        /// <param name="offset">distance offset</param>
        /// <param name="left">draw left</param>
        /// <param name="right">draw right</param>
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private void CreateDimOnLine(Revit.DB.Document doc, Revit.DB.View checkedView, System.Collections.Generic.List<Revit.DB.Grid> lstGridLine, System.Collections.Generic.List<Revit.DB.Grid> lstGridArc, Revit.DB.DimensionType selectedDimensionType, double offset, bool left, bool right)
        {
            if (!left && !right)
                return;

            // get all grids
            var retVal = GetSegmentGrids(checkedView, lstGridLine);
            foreach (var pair in retVal)
            {
                Revit.DB.Grid gridLine = pair.Value.FirstOrDefault();
                if (gridLine == null)
                    continue;

                var lstIntersection = GetIntersection(retVal, gridLine, lstGridArc, true);
                if (lstIntersection.Count < 2)
                    continue;

                Revit.DB.ReferenceArray lstRef = null;

                // Get direction of line
                Revit.DB.XYZ dirLine = lstIntersection[lstIntersection.Count - 1] - lstIntersection[0];
                Revit.DB.XYZ dirCross = null;
                // Create dim on the left side of the line
                if (left)
                {
                    lstRef = new Revit.DB.ReferenceArray();
                    dirCross = dirLine.CrossProduct(checkedView.ViewDirection);

                    for (int i = 0; i < lstIntersection.Count; i++)
                    {
                        // Current intersection
                        Revit.DB.XYZ currentIntersection = lstIntersection[i];

                        // Get line vertical
                        var pEnd = _CmpGeometry.GetPointOnVector(currentIntersection, dirCross, offset * 2);

                        // Create detail line
                        var detailLine = CreateDetailLine(doc, checkedView, currentIntersection, pEnd);
                        if (detailLine == null)
                            continue;

                        if (detailLine.GeometryCurve == null || detailLine.GeometryCurve.Reference == null)
                            continue;

                        lstRef.Append(detailLine.GeometryCurve.Reference);
                    }

                    Revit.DB.XYZ pos1 = gridLine.Curve.GetEndPoint(0);
                    pos1 = _CmpGeometry.GetPointOnVector(pos1, dirCross, offset);

                    Revit.DB.XYZ pos2 = gridLine.Curve.GetEndPoint(1);
                    pos2 = _CmpGeometry.GetPointOnVector(pos2, dirCross, offset);

                    //Create dimension
                    Revit.DB.Dimension dimen = _CmpElements.CreateDimension(checkedView, pos1, pos2, lstRef);
                    if (dimen != null)
                        dimen.DimensionType = selectedDimensionType;
                }
                //Create dim on the right side of the line
                if (right)
                {
                    lstRef = new Revit.DB.ReferenceArray();
                    dirCross = dirLine.CrossProduct(checkedView.ViewDirection).Negate();

                    for (int i = 0; i < lstIntersection.Count; i++)
                    {
                        // Current intersection
                        Revit.DB.XYZ currentIntersection = lstIntersection[i];

                        // Get line vertical
                        var pEnd = _CmpGeometry.GetPointOnVector(currentIntersection, dirCross, offset * 2);

                        // Create detail line
                        var detailLine = CreateDetailLine(doc, checkedView, currentIntersection, pEnd);
                        if (detailLine == null)
                            continue;

                        if (detailLine.GeometryCurve == null || detailLine.GeometryCurve.Reference == null)
                            continue;

                        lstRef.Append(detailLine.GeometryCurve.Reference);
                    }

                    Revit.DB.XYZ pos1 = gridLine.Curve.GetEndPoint(0);
                    pos1 = _CmpGeometry.GetPointOnVector(pos1, dirCross, offset);

                    Revit.DB.XYZ pos2 = gridLine.Curve.GetEndPoint(1);
                    pos2 = _CmpGeometry.GetPointOnVector(pos2, dirCross, offset);

                    //Create dimension
                    Revit.DB.Dimension dimen = _CmpElements.CreateDimension(checkedView, pos1, pos2, lstRef);
                    if (dimen != null)
                        dimen.DimensionType = selectedDimensionType;
                }
            }
        }

        /// ================================================================================
        /// <summary>Create details line</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="checkedView">view</param>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <returns>Details Line</returns>
        ///
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private Revit.DB.DetailLine CreateDetailLine(Revit.DB.Document doc, Revit.DB.View checkedView, Revit.DB.XYZ p1, Revit.DB.XYZ p2)
        {
            try
            {
                Revit.DB.Line lineCreated = Revit.DB.Line.CreateBound(p1, p2);
                return doc.Create.NewDetailCurve(checkedView, lineCreated) as Revit.DB.DetailLine;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return null;
            }
        }

        /// ================================================================================
        /// <summary>Check if the number of intersections of a line with a curve is greater than line</summary>
        ///
        /// <param name="elemGrids">List element grid</param>
        /// <returns><p>Result</p>
        ///             <p>True  = More than 2 intersections</p>
        ///             <p>False = Less than 2 intersections</p></returns>
        ///
        /// <history><p>2021/1/12 Created Applied Technology</p></history>
        /// ================================================================================
        public bool IsMultiIntersection(Document doc, System.Collections.Generic.IList<Revit.DB.Element> elemGrids)
        {
            // Get list grid line and list grid arc
            FilterGridArcAndLine(elemGrids, out Collections.Generic.List<Revit.DB.Grid> lstGridLine,
                                                    out Collections.Generic.List<Revit.DB.Grid> lstGridArc);
            foreach (var gLine in lstGridLine)
            {
                if (gLine == null || gLine.Curve == null)
                    continue;
                foreach (var gArc in lstGridArc)
                {
                    if (gArc == null || gArc.Curve == null)
                        continue;

                    if (CurveOverlapCount(gLine.Curve, gArc.Curve) > 1)
                        return true;
                }
            }

            // Check number point of multi grid intersection
            int numberIntersec = 0;
            foreach (var gLine in lstGridLine)
            {
                if (gLine == null || gLine.Curve == null)
                    continue;

                // get  multi segment grid
                Revit.DB.FilteredElementCollector collector = new Revit.DB.FilteredElementCollector(doc, doc.ActiveView.Id);
                Revit.DB.MultiSegmentGrid multiSegment = collector
                               .OfCategory(BuiltInCategory.OST_GridChains)
                               .Cast<Revit.DB.MultiSegmentGrid>()
                               .FirstOrDefault(x => x.Name == gLine.Name);

                numberIntersec = 0;
                if (multiSegment != null)
                {
                    var lst = multiSegment.GetGridIds();

                    foreach (var gArc in lstGridArc)
                    {
                        numberIntersec = 0;
                        if (gArc == null || gArc.Curve == null)
                            continue;

                        foreach (var item in lst)
                        {
                            if (item == Revit.DB.ElementId.InvalidElementId)
                                continue;
                            Revit.DB.Grid grid = doc.GetElement(item) as Revit.DB.Grid;
                            if (grid == null)
                                continue;
                            int cnt = CurveOverlapCount(grid.Curve, gArc.Curve);
                            if (cnt != 0)
                                numberIntersec += 1;

                            if (numberIntersec > 1 || cnt > 1)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// ================================================================================
        /// <summary>Filter grid </summary>
        ///
        /// <param name="lstSelSetGrids">List grid selection</param>
        /// <param name="lstGridLine">List grid is line</param>
        /// <param name="lstGridArc">List grid is arc </param>
        ///
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private void FilterGridArcAndLine(System.Collections.Generic.IList<Revit.DB.Element> lstSelSetGrids, out System.Collections.Generic.List<Revit.DB.Grid> lstGridLine, out System.Collections.Generic.List<Revit.DB.Grid> lstGridArc)
        {
            lstGridArc = new System.Collections.Generic.List<Revit.DB.Grid>();
            lstGridLine = new System.Collections.Generic.List<Revit.DB.Grid>();

            foreach (Revit.DB.Grid grid in lstSelSetGrids)
            {
                if (grid == null)
                    continue;

                if (grid.Curve is Revit.DB.Arc)
                    lstGridArc.Add(grid);
                else
                    lstGridLine.Add(grid);
            }
        }

        /// ================================================================================
        /// <summary>Get point intersection </summary>
        ///
        /// <param name="grid">Grid is line</param>
        /// <param name="listgrid">List grid is arc</param>
        /// <param name="isLine">bool isLine</param>
        /// <returns>List point </returns>
        ///
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private System.Collections.Generic.List<Revit.DB.XYZ> GetIntersection(Revit.DB.Grid grid, System.Collections.Generic.List<Revit.DB.Grid> listgrid, bool isLine)
        {
            System.Collections.Generic.List<Revit.DB.XYZ> retVal = new System.Collections.Generic.List<Revit.DB.XYZ>();

            if (grid == null || grid.Curve == null)
                return retVal;

            System.Collections.Generic.List<RvtExtApp.Entities.GridDistance> lstGridDistance = new System.Collections.Generic.List<RvtExtApp.Entities.GridDistance>();

            List<XYZ> listPoint = new List<XYZ>();

            bool isCloclwise = true;

            // Get intersection grid Line
            foreach (var arc in listgrid)
            {
                if (arc.Curve == null)
                    continue;

                XYZ interPt = CurveFirstOverlapPoint(grid.Curve, arc.Curve);
                if (interPt == null)
                    continue;

                RvtExtApp.Entities.GridDistance gridDis = new RvtExtApp.Entities.GridDistance();
                gridDis.PointOnGrid = interPt;

                listPoint = new List<XYZ>();
                listPoint.Add(grid.Curve.GetEndPoint(0));
                listPoint.Add(gridDis.PointOnGrid);
                listPoint.Add(grid.Curve.GetEndPoint(1));

                isCloclwise = IsClockwise(listPoint);

                GetCurveLength(grid.Curve, gridDis.PointOnGrid, out double length, isCloclwise);

                gridDis.DistanceGrid = length;

                lstGridDistance.Add(gridDis);
            }

            if (isLine)
            {
                lstGridDistance = lstGridDistance.OrderByDescending(x => x.PointOnGrid.Y).ToList();
                retVal = lstGridDistance.Select(x => x.PointOnGrid).ToList();
            }
            else
            {
                if (isCloclwise)
                    lstGridDistance = lstGridDistance.OrderBy(x => x.DistanceGrid).ToList();
                else
                    lstGridDistance = lstGridDistance.OrderByDescending(x => x.DistanceGrid).ToList();

                retVal = lstGridDistance.Select(x => x.PointOnGrid).ToList();
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Get point intersection </summary>
        ///
        /// <param name="grid">Grid is line</param>
        /// <param name="listgrid">List grid is arc</param>
        /// <param name="isLine">bool isLine</param>
        /// <returns>List point </returns>
        ///
        /// <history><p>2021/11/10 Created Applied Technology</p></history>
        /// ================================================================================
        private System.Collections.Generic.List<Revit.DB.XYZ> GetIntersection(Dictionary<string, List<Revit.DB.Grid>> dicGrid, Revit.DB.Grid grid, System.Collections.Generic.List<Revit.DB.Grid> listgrid, bool isLine)
        {
            System.Collections.Generic.List<Revit.DB.XYZ> retVal = new System.Collections.Generic.List<Revit.DB.XYZ>();

            if (grid == null || grid.Curve == null)
                return retVal;
            List<Grid> lstGrid = new List<Grid>();
            foreach (var pair in dicGrid)
            {
                if (pair.Key == grid.Name)
                {
                    foreach (var gr in pair.Value)
                        lstGrid.Add(gr);
                }
            }

            System.Collections.Generic.List<RvtExtApp.Entities.GridDistance> lstGridDistance = new System.Collections.Generic.List<RvtExtApp.Entities.GridDistance>();

            List<XYZ> listPoint = new List<XYZ>();

            bool isCloclwise = true;

            // Get intersection grid Line
            foreach (var arc in listgrid)
            {
                if (arc.Curve == null)
                    continue;

                Revit.DB.XYZ interPt = null;
                foreach (var grd in lstGrid)
                {
                    interPt = CurveFirstOverlapPoint(grd.Curve, arc.Curve);
                    if (interPt != null)
                        break;
                }
                if (interPt == null)
                    continue;

                RvtExtApp.Entities.GridDistance gridDis = new RvtExtApp.Entities.GridDistance();
                gridDis.PointOnGrid = interPt;

                listPoint = new List<XYZ>();
                listPoint.Add(grid.Curve.GetEndPoint(0));
                listPoint.Add(gridDis.PointOnGrid);
                listPoint.Add(grid.Curve.GetEndPoint(1));

                isCloclwise = IsClockwise(listPoint);

                GetCurveLength(grid.Curve, gridDis.PointOnGrid, out double length, isCloclwise);

                gridDis.DistanceGrid = length;

                lstGridDistance.Add(gridDis);
            }

            if (isLine)
            {
                lstGridDistance = lstGridDistance.OrderByDescending(x => x.PointOnGrid.Y).ToList();
                retVal = lstGridDistance.Select(x => x.PointOnGrid).ToList();
            }
            else
            {
                if (isCloclwise)
                    lstGridDistance = lstGridDistance.OrderBy(x => x.DistanceGrid).ToList();
                else
                    lstGridDistance = lstGridDistance.OrderByDescending(x => x.DistanceGrid).ToList();

                retVal = lstGridDistance.Select(x => x.PointOnGrid).ToList();
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Sort start point and end point by coordinate</summary>
        ///
        /// <param name="plane"> plane</param>
        /// <param name="gridAry">list grids </param>
        /// <param name="lstXYZ"> return list point sorted</param>
        ///
        /// <history><p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        private void SortStarPointEndPoint(Revit.DB.View checkView, Revit.DB.Plane plane, Collections.Generic.IList<Revit.DB.Grid> gridAry, bool isDirX, ref Collections.Generic.List<Revit.DB.XYZ> lstXYZ)
        {
            Collections.Generic.List<Revit.DB.XYZ> ListSEPoint = new Collections.Generic.List<Revit.DB.XYZ>();

            foreach (var gri in gridAry)
            {
                if (gri == null)
                    continue;
                Revit.DB.XYZ sPoint1 = null;
                Revit.DB.XYZ ePoint1 = null;
                Revit.DB.XYZ sPoint2 = null;
                Revit.DB.XYZ ePoint2 = null;
                GetStartEndPointMultiGrid(checkView, gri, isDirX, out sPoint1, out ePoint1, out sPoint2, out ePoint2);
                if (sPoint1 != null)
                    ListSEPoint.Add(sPoint1);
                if (ePoint1 != null)
                    ListSEPoint.Add(ePoint1);

                if (sPoint2 != null)
                    ListSEPoint.Add(sPoint2);
                if (ePoint2 != null)
                    ListSEPoint.Add(ePoint2);

                Revit.DB.Curve curve = gri.Curve;
                if (curve == null)
                    continue;
                ListSEPoint.Add(curve.GetEndPoint(0));
                ListSEPoint.Add(curve.GetEndPoint(1));
            }
            Revit.DB.UV uV = null;
            double distance = 0; ;
            foreach (var point in ListSEPoint)
            {
                if (point == null)
                    continue;
                plane.Project(point, out uV, out distance);

                Revit.DB.XYZ newPonit = new Revit.DB.XYZ(uV.U, uV.V + 1, 0);
                lstXYZ.Add(newPonit);
            }
        }

        /// ================================================================================
        /// <summary>Check direction grid</summary>
        ///
        /// <param name="grid">grid</param>
        /// <param name="checkedView">view </param>
        /// <param name="isDirectionX">is direction x</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        private bool CheckDirection(Revit.DB.Grid grid, Revit.DB.View checkedView, bool isDirectionX)
        {
            if (isDirectionX)
            {
                Revit.DB.Line line = grid.Curve as Revit.DB.Line;
                if (line == null)
                    return false;
                Revit.DB.XYZ dicrection = line.Direction.Normalize();
                Revit.DB.XYZ rightDirection = checkedView.RightDirection;
                if (dicrection.IsAlmostEqualTo(rightDirection))
                    return true;
            }
            else
            {
                Revit.DB.Line line = grid.Curve as Revit.DB.Line;
                if (line == null)
                    return false;
                Revit.DB.XYZ dicrection = line.Direction.Normalize();
                Revit.DB.XYZ upDirection = checkedView.UpDirection;
                if (dicrection.IsAlmostEqualTo(upDirection))
                    return true;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>Check parallel grid</summary>
        ///
        /// <param name="grid">grid</param>
        /// <param name="checkedView">view </param>
        /// <param name="isDirectionX">is direction x</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        private bool CheckParallel(Revit.DB.Grid grid, Revit.DB.View checkedView, bool isDirectionX)
        {
            if (isDirectionX)
            {
                Revit.DB.Line line = grid.Curve as Revit.DB.Line;
                if (line == null)
                    return false;
                Revit.DB.XYZ dicrection = line.Direction.Normalize();
                Revit.DB.XYZ rightDirection = checkedView.RightDirection;
                if (dicrection.CrossProduct(rightDirection).IsZeroLength())
                    return true;
            }
            else
            {
                Revit.DB.Line line = grid.Curve as Revit.DB.Line;
                if (line == null)
                    return false;
                Revit.DB.XYZ dicrection = line.Direction.Normalize();
                Revit.DB.XYZ upDirection = checkedView.UpDirection;
                if (dicrection.CrossProduct(upDirection).IsZeroLength())
                    return true;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>Check parallel grid</summary>
        ///
        /// <param name="grid">grid</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        private bool CheckDirection(Revit.DB.Grid grid)
        {
            Revit.DB.Line line = grid.Curve as Revit.DB.Line;
            if (line == null)
                return false;
            Revit.DB.XYZ dicrection = line.Direction.Normalize();
            if (dicrection.Y > 0)
                return true;
            return false;
        }

        /// ================================================================================
        /// <summary>Check if the grid is parallel together</summary>
        ///
        /// <param name="checkedView">Revit.DB.View</param>
        /// <param name="lstSegmentGrid">List grid</param>
        /// <param name="isDirectionX">Is direction X</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Created Applied Technology<p></history>
        /// ================================================================================
        public bool IsGridParallel(Revit.DB.Document doc, System.Collections.Generic.IList<Revit.DB.Element> lstSegmentGrid)
        {
            bool ret = false;
            Revit.DB.Grid gridfirst = null;
            Revit.DB.XYZ dicFisrt = null;

            foreach (var ele in lstSegmentGrid)
            {
                if (ele is Revit.DB.MultiSegmentGrid == false)
                    continue;

                Revit.DB.MultiSegmentGrid segmentGrid = ele as Revit.DB.MultiSegmentGrid;
                var lstGrids = segmentGrid.GetGridIds();
                if (lstGrids == null || lstGrids.Count == 0)
                    continue;

                Revit.DB.ElementId eleFirst = lstGrids.FirstOrDefault();
                gridfirst = doc.GetElement(eleFirst) as Revit.DB.Grid;
                if (gridfirst == null || gridfirst.Curve == null)
                    continue;
                dicFisrt = (gridfirst.Curve.GetEndPoint(1) - gridfirst.Curve.GetEndPoint(0)).Normalize();

                foreach (var item in lstGrids)
                {
                    Revit.DB.Grid grid = doc.GetElement(item) as Revit.DB.Grid;
                    if (grid == null || grid.Curve == null)
                        continue;
                    if (grid != null && grid != gridfirst)
                    {
                        Revit.DB.Line line = grid.Curve as Revit.DB.Line;
                        if (line == null)
                            continue;
                        Revit.DB.XYZ dicrection = line.Direction.Normalize();
                        if (!dicrection.CrossProduct(dicFisrt).IsZeroLength())
                            return true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Get all grid of multi segment grid</summary>
        ///
        /// <param name="checkedView">Check view </param>
        /// <param name="gridArr">List grid</param>
        /// <param name="isDirectionX">bool is direction x</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Created Applied Technology<p></history>
        /// ================================================================================
        private System.Collections.Generic.Dictionary<string, Collections.Generic.List<Revit.DB.Grid>> GetAllGrid(Revit.DB.View checkedView, Collections.Generic.List<Revit.DB.Grid> gridArr, bool isDirectionX)
        {
            string name = string.Empty;
            System.Collections.Generic.List<Revit.DB.Grid> grids = null;
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Revit.DB.Grid>> dicGrids = new Collections.Generic.Dictionary<string, System.Collections.Generic.List<Revit.DB.Grid>>();

            foreach (var grid1 in gridArr)
            {
                if (grid1 == null)
                    continue;

                name = grid1.Name;
                grids = new Collections.Generic.List<Revit.DB.Grid>();
                grids.Add(grid1);
                if (!dicGrids.ContainsKey(name))
                    dicGrids.Add(name, grids);
            }
            return dicGrids;
        }

        /// ================================================================================
        /// <summary>Get start point - end point of segment fist and segment last multi segment grid</summary>
        ///
        /// <param name="checkedView">check view</param>
        /// <param name="grid">grid</param>
        /// <param name="isDirX">is direction x or not</param>
        /// <param name="sPoint1">start point of segment fist</param>
        /// <param name="ePoint1">end point of segment fist</param>
        /// <param name="sPoint2">start point of segment last</param>
        /// <param name="ePoint2">end point of segment last</param>
        ///
        /// <history><p>2021/21/10 Created Applied Technology<p></history>
        /// ================================================================================
        private void GetStartEndPointMultiGrid(Revit.DB.View checkedView, Revit.DB.Grid grid, bool isDirX, out Revit.DB.XYZ sPoint1, out Revit.DB.XYZ ePoint1, out Revit.DB.XYZ sPoint2, out Revit.DB.XYZ ePoint2)
        {
            Revit.DB.FilteredElementCollector col = new Revit.DB.FilteredElementCollector(checkedView.Document, checkedView.Id);
            Revit.DB.MultiSegmentGrid multiGrids = col.OfCategory(Revit.DB.BuiltInCategory.OST_GridChains)
                .Cast<Revit.DB.MultiSegmentGrid>()
                .FirstOrDefault(x => x.Name.Equals(grid.Name));
            Revit.DB.Grid grid1 = null;
            Revit.DB.Grid grid2 = null;
            sPoint1 = null;
            ePoint1 = null;
            sPoint2 = null;
            ePoint2 = null;
            if (multiGrids != null)
            {
                Collections.Generic.List<Revit.DB.ElementId> elementIds = multiGrids.GetGridIds().ToList();
                int count = elementIds.Count;
                if (count > 2)
                {
                    grid1 = _Doc.GetElement(elementIds[0]) as Revit.DB.Grid;
                    grid2 = _Doc.GetElement(elementIds[count - 1]) as Revit.DB.Grid;
                    if (grid1 == null || grid2 == null)
                        return;
                    if (CheckParallel(grid1, checkedView, isDirX))
                    {
                        sPoint1 = grid1.Curve.GetEndPoint(0);
                        ePoint1 = grid1.Curve.GetEndPoint(1);
                    }
                    if (CheckParallel(grid2, checkedView, isDirX))
                    {
                        sPoint2 = grid2.Curve.GetEndPoint(0);
                        ePoint2 = grid2.Curve.GetEndPoint(1);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Check number point intersection > 1 or not</summary>
        ///
        /// <param name="lstGridLine">List grid line</param>
        /// <param name="lstGridArc">List grid arc</param>
        /// <param name="left">bool draw left</param>
        /// <param name="right">bool draw right</param>
        /// <param name="top">bool draw top</param>
        /// <param name="bottom">bool draw bottom</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/21/10 Created Applied Technology<p></history>
        /// ================================================================================
        private bool CheckNumberPointIntersection(Revit.DB.View checkedView, Collections.Generic.List<Revit.DB.Grid> lstGridLine, Collections.Generic.List<Revit.DB.Grid> lstGridArc, bool left, bool right, bool top, bool bottom)
        {
            // Get list grid
            List<Grid> listSegmentGrid = new List<Grid>();
            var retVal = GetSegmentGrids(checkedView, lstGridLine);
            foreach (var pair in retVal)
            {
                foreach (var grid in pair.Value)
                    listSegmentGrid.Add(grid);
            }

            // Check number point intersection of grid line
            int countLine = 0;
            foreach (var pair in retVal)
            {
                Revit.DB.Grid gridLine = pair.Value.FirstOrDefault();
                if (gridLine == null)
                    continue;

                var lstIntersection = GetIntersection(retVal, gridLine, lstGridArc, true);
                if (lstIntersection.Count < 2)
                    continue;
                else
                {
                    countLine++;
                    break;
                }
            }

            // Check number point intersection of grid arc
            int countArc = 0;
            foreach (var gridArc in lstGridArc)
            {
                var lstIntersection = GetIntersection(gridArc, listSegmentGrid, false);

                if (lstIntersection.Count < 2)
                    continue;
                else
                {
                    countArc++;
                    break;
                }
            }
            if (countLine == 0 && countArc == 0)
                return false;

            if (countLine == 0 && left || countLine == 0 && right)
            {
                if (!top && !bottom)
                    return false;
            }
            if (countArc == 0 && top || countArc == 0 && bottom)
            {
                if (!left && !right)
                    return false;
            }
            return true;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/11/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
        }

        #endregion Properties
    }
}