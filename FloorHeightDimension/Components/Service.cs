using System;
using System.Globalization;
using System.Threading;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;
using System.Linq;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
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

        /// <summary>Number of run</summary>
        private int _RunTime = 0;

        /// <summary>Distan dimension to start point of level</summary>
        private double _Distan = 0.0;

        /// <summary>エラーメッセージ</summary>
        private string _ErrMsg;

        #endregion Memeber Variables

        #region Constructor

        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
        }

        #endregion Constructor

        #region Member Functions

        public Revit.DB.XYZ SelPos(Revit.DB.View activeView)
        {
            Revit.DB.XYZ ret = null;

            Revit.DB.SketchPlane skPlane = activeView.SketchPlane;
            if (skPlane == null)
            {
                Revit.DB.XYZ normal = activeView.ViewDirection;
                Revit.DB.XYZ origin = activeView.Origin;
                Revit.DB.Plane plane = Revit.DB.Plane.CreateByNormalAndOrigin(normal, origin);
                skPlane = _CmpElements.CreateSketchPlane(plane);
                activeView.SketchPlane = skPlane;
            }

            Revit.UI.Selection.ObjectSnapTypes snapTypes = Revit.UI.Selection.ObjectSnapTypes.Centers |
                                                           Revit.UI.Selection.ObjectSnapTypes.Endpoints |
                                                           Revit.UI.Selection.ObjectSnapTypes.Intersections |
                                                           Revit.UI.Selection.ObjectSnapTypes.Midpoints |
                                                           Revit.UI.Selection.ObjectSnapTypes.Nearest |
                                                           Revit.UI.Selection.ObjectSnapTypes.WorkPlaneGrid;

            try
            {
                ret = _CmpElements.RvtUIDoc.Selection.PickPoint(snapTypes);
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
            }

            return ret;
        }

        public bool CreateDimension(Revit.DB.ViewSection activeViewSec,
                             Collections.Generic.IList<Revit.DB.Element> elemLevels,
                             Revit.DB.XYZ locPos,
                             string strDistA,
                             string strDistB,
                             Revit.DB.DimensionType selectedDimensionType)
        {
            bool ret = false;
            _ErrMsg = "";

            Collections.Generic.List<Revit.DB.Element> elemLevelColl = new Revit.DB.FilteredElementCollector(activeViewSec.Document, activeViewSec.Id).
                OfCategory(Revit.DB.BuiltInCategory.OST_Levels).ToElements().Where(item => elemLevels.Where(item2 => item2.Id == item.Id).ToList().Count != 0).ToList();

            if (elemLevelColl.Count == 0)
                return ret;

            double distA = 0.0;
            double distB = 0.0;
            double distAX = 0.0;
            double distAY = 0.0;
            double distBX = 0.0;
            double distBY = 0.0;
            if (!string.IsNullOrWhiteSpace(strDistA) && double.TryParse(strDistA, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out var parsedA))
                distA = parsedA;
            distA /= _CmpGeometry.UnitCoe;

            if (!string.IsNullOrWhiteSpace(strDistB) && double.TryParse(strDistB, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out var parsedB))
                distB = parsedB;
            distB /= _CmpGeometry.UnitCoe;

            Revit.DB.XYZ viewDir = activeViewSec.ViewDirection;
            Revit.DB.XYZ distDir = new Revit.DB.XYZ(viewDir.Y * -1.0, viewDir.X, viewDir.Z);
            distAX = distA * distDir.X;
            distAY = distA * distDir.Y;
            distBX = distB * distDir.X;
            distBY = distB * distDir.Y;

            double viewScale = activeViewSec.Scale * 1.0;
            distAX *= viewScale;
            distAY *= viewScale;
            distBX *= viewScale;
            distBY *= viewScale;

            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Level>> levelAryAry =
              new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Level>>();
            for (int i = 0; i < 2; ++i)
            {
                Collections.Generic.IList<Revit.DB.Level> levelAry = new Collections.Generic.List<Revit.DB.Level>();
                levelAryAry.Add(levelAry);
            }

            for (int i = 0; i < elemLevelColl.Count; ++i)
            {
                Revit.DB.Level level = elemLevelColl[i] as Revit.DB.Level;
                if (level != null)
                {
                    Revit.DB.LevelType levelType = null;
                    Revit.DB.ElementId elemId = level.GetTypeId();
                    if (elemId != null)
                    {
                        Revit.DB.Element elem = _CmpElements.RvtDBDoc.GetElement(elemId);
                        if (elemId != null)
                        {
                            levelType = elem as Revit.DB.LevelType;
                        }
                    }

                    if (levelType != null)
                    {
                        bool end1 = false;
                        if (_CmpParameters.GetValue(levelType,
                                                    Revit.DB.BuiltInParameter.DATUM_BUBBLE_END_1,
                                                    ref end1) < -1)
                        {
                        }
                        bool end2 = false;
                        if (_CmpParameters.GetValue(levelType,
                                                    Revit.DB.BuiltInParameter.DATUM_BUBBLE_END_2,
                                                    ref end2) < -1)
                        {
                        }

                        if (end1 == true)
                        {
                            levelAryAry[0].Add(level);
                        }
                        if (end2 == true)
                        {
                            levelAryAry[1].Add(level);
                        }
                    }
                }
            }

            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Level>> sortedLevelAryAry =
            new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Level>>();
            for (int i = 0; i < levelAryAry.Count; ++i)
            {
                Collections.Generic.IList<Revit.DB.Level> sortedLevelAry = new Collections.Generic.List<Revit.DB.Level>();
                Collections.Generic.IList<Revit.DB.Level> levelAry = levelAryAry[i];
                Collections.Generic.IList<int> sortedIndex = new Collections.Generic.List<int>();
                _CmpElements.SortLevelsHeight(levelAry, 1, ref sortedIndex, ref sortedLevelAry);

                sortedLevelAryAry.Add(sortedLevelAry);
            }

            double p1x = 0.0;
            double p1y = 0.0;
            double p1z = 0.0;
            double p2x = 0.0;
            double p2y = 0.0;
            double p2z = 0.0;
            Revit.DB.XYZ pos1 = null;
            Revit.DB.XYZ pos2 = null;
            for (int i = 0; i < sortedLevelAryAry.Count; ++i)
            {
                Collections.Generic.IList<Revit.DB.Level> levelAry = sortedLevelAryAry[i];
                if (levelAry.Count < 2)
                {
                    continue;
                }

                Revit.DB.ReferenceArray refAry1 = new Revit.DB.ReferenceArray();
                Revit.DB.ReferenceArray refAry2 = new Revit.DB.ReferenceArray();

                int gridNo = levelAry.Count - 1;
                refAry1.Append(levelAry[0].GetPlaneReference());
                refAry1.Append(levelAry[gridNo].GetPlaneReference());
                for (int j = 0; j < levelAry.Count; ++j)
                {
                    refAry2.Append(levelAry[j].GetPlaneReference());
                }

                Collections.Generic.IList<Revit.DB.Curve> listCurve1 = levelAry[0].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, activeViewSec);

                Revit.DB.Curve curve1 = null;
                foreach (Revit.DB.Curve item in listCurve1)
                {
                    if (item == null)
                        continue;

                    curve1 = item;
                }

                if (curve1 == null)
                    continue;

                double locPosX = double.NaN;
                double locPosY = double.NaN;

                if (locPos != null)
                {
                    locPosX = locPos.X;
                    locPosY = locPos.Y;
                }
                else
                {
                    Collections.Generic.IList<Revit.DB.Element> eleGridInView = new Revit.DB.FilteredElementCollector(activeViewSec.Document, activeViewSec.Id).
                        OfCategory(Revit.DB.BuiltInCategory.OST_Grids).ToElements().ToList();
                    if (eleGridInView.Count == 0)
                        return false;

                    Collections.Generic.List<Revit.DB.Curve> listAllGeometryCurveX = new Collections.Generic.List<Revit.DB.Curve>();
                    Collections.Generic.List<Revit.DB.Curve> listAllGeometryCurveY = new Collections.Generic.List<Revit.DB.Curve>();
                    foreach (Revit.DB.Element ele in eleGridInView)
                    {
                        Revit.DB.Grid grid = ele as Revit.DB.Grid;
                        if (grid == null)
                            continue;

                        Collections.Generic.IList<Revit.DB.Curve> listCurve = grid.GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, activeViewSec);
                        if (listCurve.FirstOrDefault() == null)
                            continue;
                        listAllGeometryCurveX.Add(listCurve.FirstOrDefault());
                        listAllGeometryCurveY.Add(listCurve.FirstOrDefault());
                    }

                    if (listAllGeometryCurveX.Count != 0)
                    {
                        listAllGeometryCurveX.Sort(delegate (Revit.DB.Curve curveFist, Revit.DB.Curve curveScond)
                        {
                            Revit.DB.XYZ end1 = curveFist.GetEndPoint(1);
                            Revit.DB.XYZ end2 = curveScond.GetEndPoint(1);
                            return end1.X.CompareTo(end2.X);
                        });

                        listAllGeometryCurveY.Sort(delegate (Revit.DB.Curve curveFist, Revit.DB.Curve curveScond)
                        {
                            Revit.DB.XYZ end1 = curveFist.GetEndPoint(1);
                            Revit.DB.XYZ end2 = curveScond.GetEndPoint(1);
                            return end1.Y.CompareTo(end2.Y);
                        });

                        Collections.Generic.IList<Revit.DB.XYZ> listInterSec = null;
                        Revit.DB.Curve curvePos = null;

                        if (distA <= 0)
                        {
                            if (activeViewSec.ViewDirection.CrossProduct(new Revit.DB.XYZ(1, 0, 0)).IsZeroLength() ||
                                activeViewSec.ViewDirection.CrossProduct(new Revit.DB.XYZ(0, 1, 0)).IsZeroLength())
                            {
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(1, 0, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveY.FirstOrDefault();
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(-1, 0, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveY.LastOrDefault();

                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(0, 1, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveX.LastOrDefault();
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(0, -1, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveX.FirstOrDefault();
                            }
                            Collections.Generic.IList<Revit.DB.Curve> curLelver = levelAry[0].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, activeViewSec);

                            Revit.DB.Curve newCurveExtend = _CmpGeometry.FindExtendLine(curvePos, curLelver.FirstOrDefault().Distance(curvePos.GetEndPoint(1)));

                            _CmpGeometry.IntersecCurve(newCurveExtend, curLelver.FirstOrDefault(), ref listInterSec);
                            if (listInterSec == null)
                                return false;

                            locPosX = listInterSec.FirstOrDefault().X;
                            locPosY = listInterSec.FirstOrDefault().Y;
                        }
                        else
                        {
                            if (activeViewSec.ViewDirection.CrossProduct(new Revit.DB.XYZ(1, 0, 0)).IsZeroLength() ||
                                activeViewSec.ViewDirection.CrossProduct(new Revit.DB.XYZ(0, 1, 0)).IsZeroLength())
                            {
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(1, 0, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveY.LastOrDefault();
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(-1, 0, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveY.FirstOrDefault();

                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(0, 1, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveX.FirstOrDefault();
                                if (activeViewSec.ViewDirection.DotProduct(new Revit.DB.XYZ(0, -1, 0)) > 0.1f)
                                    curvePos = listAllGeometryCurveX.LastOrDefault();
                            }

                            Collections.Generic.IList<Revit.DB.Curve> curLelver = levelAry[0].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, activeViewSec);

                            Revit.DB.Curve newCurveExtend = _CmpGeometry.FindExtendLine(curvePos, curLelver.FirstOrDefault().Distance(curvePos.GetEndPoint(1)));

                            _CmpGeometry.IntersecCurve(newCurveExtend, curLelver.FirstOrDefault(), ref listInterSec);
                            if (listInterSec == null)
                                return false;

                            locPosX = listInterSec.FirstOrDefault().X;
                            locPosY = listInterSec.FirstOrDefault().Y;
                        }
                    }
                    else
                        return false;
                }

                p1x = locPosX + distAX;
                p1y = locPosY + distAY;
                p1z = levelAry[0].Elevation;

                if (_RunTime == 0)
                {
                    if (locPos != null)
                    {
                        _Distan = _CmpGeometry.Distance(new Revit.DB.XYZ(p1x, p1y, p1z), curve1.GetEndPoint(1));
                        _Distan /= viewScale;
                    }
                }
                else
                {
                    listCurve1 = levelAry[0].GetCurvesInView(Revit.DB.DatumExtentType.ViewSpecific, activeViewSec);
                    foreach (Revit.DB.Curve item in listCurve1)
                    {
                        if (item == null)
                            continue;

                        if (_Distan != 0.0)
                        {
                            Revit.DB.XYZ xyNew = _CmpGeometry.GetPointDistance(item.GetEndPoint(0), item.GetEndPoint(1), _Distan * viewScale);
                            p1x = xyNew.X;
                            p1y = xyNew.Y;
                            p1z = levelAry[0].Elevation;
                        }
                        else
                        {
                            if (locPos != null)
                            {
                                p1x = item.GetEndPoint(1).X + distAX;
                                p1y = item.GetEndPoint(1).Y + distAY;
                                p1z = levelAry[0].Elevation;
                            }
                            else
                            {
                                p1x = locPosX + distAX;
                                p1y = locPosY + distAY;
                                p1z = levelAry[0].Elevation;
                            }
                        }
                    }
                }

                p2x = p1x;
                p2y = p1y;
                p2z = levelAry[gridNo].Elevation;
                pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);
                pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);
                Revit.DB.Dimension dimen1 = _CmpElements.CreateDimension(activeViewSec, pos1, pos2, refAry2);
                if (dimen1 != null)
                    dimen1.DimensionType = selectedDimensionType;

                p1x = p1x + distBX;
                p1y = p1y + distBY;
                p1z = levelAry[0].Elevation;
                p2x = p1x;
                p2y = p1y;
                p2z = levelAry[gridNo].Elevation;
                pos1 = new Revit.DB.XYZ(p1x, p1y, p1z);
                pos2 = new Revit.DB.XYZ(p2x, p2y, p2z);
                Revit.DB.Dimension dimen2 = _CmpElements.CreateDimension(activeViewSec, pos1, pos2, refAry1);
                if (dimen2 != null)
                    dimen2.DimensionType = selectedDimensionType;
            }

            _RunTime++;
            ret = true;
            return ret;
        }

        #endregion Member Functions

        #region Properties

        public string ErrMsg => _ErrMsg;

        #endregion Properties
    }
}
