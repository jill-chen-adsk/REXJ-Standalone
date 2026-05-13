using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Common.Constant;

namespace RevitMEPAddin.Common
{
    public class WrpGeometry
    {
     // メンバ変数
        #region Memeber Variables
        private UIDocument uidoc;
        private Logger log;
        #endregion

        // コンストラクタ
        #region Constructor
        public WrpGeometry(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            this.log = log;
        }
        #endregion

        // メンバ関数
        #region Member Functions

        #region ** LocationCurve **

        /// <summary>
        /// Elementの始点取得
        /// ただし、LocationCurveをもつElementに限る。
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="e"></param>
        /// <returns>取得できたかどうか</returns>
        public bool GetLocationCurveStartPoint(ref XYZ pt, Element e)
        {
            return GetLocationCurvePoint(ref pt, e, 0);
        }

        /// <summary>
        /// Elementの終点取得
        /// ただし、LocationCurveをもつElementに限る。
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="e"></param>
        /// <returns>取得できたかどうか</returns>
        public bool GetLocationCurveEndPoint(ref XYZ pt, Element e)
        {
            return GetLocationCurvePoint(ref pt, e, 1);
        }

        /// <summary>
        /// Elementの始終点取得
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="e"></param>
        /// <param name="idx">0:開始点、1:終了点、2:中点</param>
        /// <returns></returns>
        public bool GetLocationCurvePoint(ref XYZ pt, Element e, int idx)
        {

            LocationCurve curve = e.Location as LocationCurve;
            if (curve == null)
            {
                log.Error("GetLocationCurvePoint:LocationCurve取得失敗。");
                return false;
            }
            if(idx == 2)
            {
                // 中点を取得する場合
                pt = GetMidlePointOfLine((Line)curve.Curve);
            }
            else
            {
                // 始点または終点を取得する場合
                pt = curve.Curve.GetEndPoint(idx);
            }

            return true;
        }

        /// <summary>
        /// LocationCurveの方向ベクトルを取得
        /// </summary>
        /// <param name="dV"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool GetLocationCurveDirectionVector(ref XYZ dV, Element e)
        {
            XYZ sPt = new XYZ();
            XYZ ePt = new XYZ();
            if(!GetLocationCurveStartPoint(ref sPt, e)
                || !GetLocationCurveEndPoint(ref ePt, e))
            {
                return false;
            }
            dV = ePt.Subtract(sPt).Normalize();
            return true;
        }

        /// <summary>
        /// 線分pt1pt2上に点ptが含まれるかどうか？
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsBetweenTwoPoints(XYZ pt1, XYZ pt2, XYZ pt)
        {
            IntersectionResult res = Line.CreateBound(pt1, pt2).Project(pt);
            if (res.XYZPoint != null && res.XYZPoint.IsAlmostEqualTo(pt)) return true;
            return false;
        }

        #endregion

        #region ** Line **
        /// <summary>
        /// Lineの中点を取得
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public XYZ GetMidlePointOfLine(Line line)
        {
            XYZ pt = line.GetEndPoint(0).Add((line.GetEndPoint(1).Subtract(line.GetEndPoint(0))).Multiply(0.5));

            log.Trace("始点:" + line.GetEndPoint(0));
            log.Trace("終点:" + line.GetEndPoint(1));
            log.Trace("中点:" + pt);
            return pt;
        }

        /// <summary>
        /// 平面方向ビュー内2つの
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public XYZ GetIntersectInXYPlane(Line unboundLine, Line boundLine)
        {
            XYZ pt = null;
            Line newUnboundLine = Line.CreateUnbound(new XYZ(unboundLine.GetEndPoint(0).X, unboundLine.GetEndPoint(0).Y, 0), unboundLine.Direction);
            Line newBoundLine = Line.CreateBound(new XYZ(boundLine.GetEndPoint(0).X, boundLine.GetEndPoint(0).Y, 0),
                                                  new XYZ(boundLine.GetEndPoint(1).X, boundLine.GetEndPoint(1).Y, 0));
            var intersectResult = newBoundLine.Intersect(newUnboundLine, CurveIntersectResultOption.Detailed);
            SetComparisonResult res = intersectResult.Result;
            var overlaps = intersectResult.GetOverlaps();
            foreach (var overlapPt in overlaps)
            {
                pt = overlapPt.Point;
                //TaskDialog.Show("test", "距離" + intersectionRes.Distance);
            }
            if(pt != null)
            {
                IntersectionResult iRes = boundLine.Project(pt);
                pt = iRes.XYZPoint;
            }
            
            return pt;
        }

        #endregion

        #region
        /// <summary>
        /// aとbは許容誤差範囲内で一致しているかどうか
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool NearlyEquals(double a, double b)
        {
            return Math.Abs(a - b) < CommonDefine.TOLERANCE;
        }

        /// <summary>
        /// 2つのベクトルの間の角度を求める
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public bool getAngleBetweenVectors(ref double angle, XYZ v1, XYZ v2)
        {

                //angle = Math.Acos(v1.DotProduct(v2) / (v1.GetLength() * v2.GetLength()));
                angle = v1.AngleTo(v2);

            return true;
        }

        /// <summary>
        /// 原点を通る特定の面で,２つのベクトル間の角度を算出。
        /// axisを軸として、v1を何度回転すれば、v2に重なるかを返す。
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool GetAngleBetweenVectorsOnSpecificPlane(ref double angle, XYZ v1, XYZ v2, XYZ axis)
        {
            if (!getAngleBetweenVectors(ref angle, v1, v2)) return false;
            XYZ vT = Transform.CreateRotation(axis, angle).OfVector(v1);
            double angleT = 0;
            if (!getAngleBetweenVectors(ref angleT, vT, v2)) return false;
            //TaskDialog.Show("test", angleT.ToString());
            if(Math.Abs(angle * 2 - angleT) < CommonDefine.TOLERANCE)
            {
                angle = (-1) * angle;
            }
            return true;
        }
        #endregion

        #region ** 単位 **
        public double ConvertMillimetersToFeet(double mm)
        {
            return UnitUtils.Convert(mm, UnitTypeId.Millimeters, UnitTypeId.Feet);
        }

        public double ConvertFeetToMillimeters(double ft)
        {
            return UnitUtils.Convert(ft, UnitTypeId.Feet, UnitTypeId.Millimeters);
        }

        #endregion

        #endregion

        // プロパティ
        #region Properties
        #endregion   
    }
}
