using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public partial class Geometry
    {
        private readonly UIDocument _rvtUIDoc;
        private double _toleranceInter;

        private const double DefaultApprox0Len = 1e-6;
        private const double DefaultApprox0Ang = 1e-6;
        private const double FeetPerMm = 1.0 / 304.8;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _toleranceInter = 0.0;
        }

        public double Approx0Len => DefaultApprox0Len;
        public double Approx0Ang => DefaultApprox0Ang;

        public double UnitCoe
        {
            get
            {
                var units = _rvtUIDoc.Document.GetUnits();
                var lengthSpec = SpecTypeId.Length;
                var fmtOpts = units.GetFormatOptions(lengthSpec);
                var unitTypeId = fmtOpts.GetUnitTypeId();
                if (unitTypeId == UnitTypeId.Millimeters) return 304.8;
                if (unitTypeId == UnitTypeId.Centimeters) return 30.48;
                if (unitTypeId == UnitTypeId.Meters) return 0.3048;
                if (unitTypeId == UnitTypeId.Feet) return 1.0;
                if (unitTypeId == UnitTypeId.Inches) return 12.0;
                return 304.8;
            }
        }

        /// <summary>Feet (internal) to meters — used with length display in meters.</summary>
        public double UnitCoeTh => 0.3048;

        /// <summary>Square feet (internal) to square meters.</summary>
        public double UnitCoeM2 => 0.09290304;

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public double ToleranceInter
        {
            get => _toleranceInter;
            set => _toleranceInter = value;
        }

        public double Distance2D(XYZ p1, XYZ p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public double Angle2D(XYZ origin, XYZ p1, XYZ p2)
        {
            double ax = p1.X - origin.X, ay = p1.Y - origin.Y;
            double bx = p2.X - origin.X, by = p2.Y - origin.Y;
            double cross = ax * by - ay * bx;
            double dot = ax * bx + ay * by;
            return Math.Atan2(cross, dot);
        }

        public XYZ UnitVector(XYZ from, XYZ to)
        {
            double dx = to.X - from.X, dy = to.Y - from.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < Approx0Len) return new XYZ(0, 0, 0);
            return new XYZ(dx / len, dy / len, 0);
        }

        public XYZ IntersecVector2D(Curve c1, Curve c2)
        {
            var p1 = c1.GetEndPoint(0);
            var p2 = c1.GetEndPoint(1);
            var p3 = c2.GetEndPoint(0);
            var p4 = c2.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y;
            double x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y;
            double x4 = p4.X, y4 = p4.Y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < Approx0Len) return null;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double ix = x1 + t * (x2 - x1);
            double iy = y1 + t * (y2 - y1);
            return new XYZ(ix, iy, 0);
        }

        public void IntersecCurve2D(Curve c1, Curve c2, ref IList<XYZ> result)
        {
            var p1 = c1.GetEndPoint(0);
            var p2 = c1.GetEndPoint(1);
            var p3 = c2.GetEndPoint(0);
            var p4 = c2.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y;
            double x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y;
            double x4 = p4.X, y4 = p4.Y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < Approx0Len) return;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denom;

            if (t >= -Approx0Len && t <= 1.0 + Approx0Len &&
                u >= -Approx0Len && u <= 1.0 + Approx0Len)
            {
                double ix = x1 + t * (x2 - x1);
                double iy = y1 + t * (y2 - y1);
                result.Add(new XYZ(ix, iy, 0));
            }
        }

        public void SortXYPos(IList<XYZ> positions, int mode,
            ref IList<int> sortedIdx, ref IList<XYZ> sortedPos)
        {
            sortedIdx = new List<int>();
            sortedPos = new List<XYZ>();

            var indices = new List<int>();
            for (int i = 0; i < positions.Count; i++) indices.Add(i);

            indices.Sort((a, b) =>
            {
                int cmp = positions[a].X.CompareTo(positions[b].X);
                if (cmp != 0) return cmp;
                return positions[a].Y.CompareTo(positions[b].Y);
            });

            foreach (int i in indices)
            {
                sortedIdx.Add(i);
                sortedPos.Add(positions[i]);
            }
        }

        public XYZ PolygonGravity2D(IList<Curve> curves)
        {
            if (curves == null || curves.Count == 0) return null;

            double cx = 0, cy = 0;
            int count = 0;
            foreach (var c in curves)
            {
                var p = c.GetEndPoint(0);
                cx += p.X; cy += p.Y;
                count++;
            }
            if (count == 0) return null;
            return new XYZ(cx / count, cy / count, 0);
        }

        public double GetPolygonArea(IList<Curve> curves)
        {
            if (curves == null || curves.Count < 3) return 0;

            var pts = new List<XYZ>();
            foreach (var c in curves)
                pts.Add(c.GetEndPoint(0));

            double area = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                int j = (i + 1) % pts.Count;
                area += pts[i].X * pts[j].Y;
                area -= pts[j].X * pts[i].Y;
            }
            return area / 2.0;
        }

        public bool IsPointInPolygon(IList<Curve> curves, XYZ unused, XYZ testPt, int mode)
        {
            if (curves == null || curves.Count < 3 || testPt == null) return false;

            var pts = new List<XYZ>();
            foreach (var c in curves)
                pts.Add(c.GetEndPoint(0));

            bool inside = false;
            int n = pts.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((pts[i].Y > testPt.Y) != (pts[j].Y > testPt.Y)) &&
                    (testPt.X < (pts[j].X - pts[i].X) * (testPt.Y - pts[i].Y) / (pts[j].Y - pts[i].Y) + pts[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        public Curve GetElementLocCurve(Element elem)
        {
            if (elem?.Location is LocationCurve locCurve)
                return locCurve.Curve;
            return null;
        }

        public bool GetCurveOnPos2D(Curve curve, XYZ pos)
        {
            var pos0 = curve.GetEndPoint(0);
            var pos1 = curve.GetEndPoint(1);

            if (Distance2D(pos0, pos) < Approx0Len) return true;
            if (Distance2D(pos1, pos) < Approx0Len) return true;

            double rad0 = Math.Abs(Angle2D(pos0, pos1, pos));
            double rad1 = Math.Abs(Angle2D(pos1, pos0, pos));
            return (rad0 < Approx0Ang) && (rad1 < Approx0Ang);
        }

        public IList<XYZ> GetInterPosCurves2D(Curve curve1, Curve curve2)
        {
            var ret = new List<XYZ>();

            var pos10 = curve1.GetEndPoint(0);
            var pos11 = curve1.GetEndPoint(1);
            var pos20 = curve2.GetEndPoint(0);
            var pos21 = curve2.GetEndPoint(1);

            var vec1 = UnitVector(pos10, pos11);
            var vec2 = UnitVector(pos20, pos21);
            var vec1Rev = new XYZ(vec1.X * -1.0, vec1.Y * -1.0, 0.0);

            bool isPalla = Distance2D(vec1, vec2) < Approx0Len ||
                           Distance2D(vec1Rev, vec2) < Approx0Len;

            if (isPalla)
            {
                XYZ interPos = null;
                if (Distance2D(pos10, pos20) < Approx0Len) interPos = pos10;
                else if (Distance2D(pos10, pos21) < Approx0Len) interPos = pos10;
                else if (Distance2D(pos11, pos20) < Approx0Len) interPos = pos11;
                else if (Distance2D(pos11, pos21) < Approx0Len) interPos = pos11;
                if (interPos != null) ret.Add(interPos);
            }
            else
            {
                bool flagInter = false;
                var interPos = IntersecVector2D(curve1, curve2);
                if (interPos != null)
                {
                    interPos = new XYZ(interPos.X, interPos.Y, curve1.GetEndPoint(0).Z);
                    if (Distance2D(interPos, pos10) < ToleranceInter) flagInter = true;
                    else if (Distance2D(interPos, pos11) < ToleranceInter) flagInter = true;
                    else if (Distance2D(interPos, pos20) < ToleranceInter)
                    {
                        if (GetCurveOnPos2D(curve1, interPos)) flagInter = true;
                    }
                    else if (Distance2D(interPos, pos21) < ToleranceInter)
                    {
                        if (GetCurveOnPos2D(curve1, interPos)) flagInter = true;
                    }
                }
                if (flagInter) ret.Add(interPos);

                var interPosAryTmp = new List<XYZ>();
                IList<XYZ> tmp = interPosAryTmp;
                IntersecCurve2D(curve1, curve2, ref tmp);
                for (int i = 0; i < tmp.Count; ++i)
                {
                    var interPosTmp = new XYZ(tmp[i].X, tmp[i].Y, curve1.GetEndPoint(0).Z);
                    bool flag = true;
                    if (flagInter && Distance2D(interPos, interPosTmp) < ToleranceInter)
                        flag = false;
                    if (flag) ret.Add(interPosTmp);
                }
            }
            return ret;
        }

        public IList<IList<XYZ>> GetInterPosCurves(IList<Curve> curveAry)
        {
            var ret = new List<IList<XYZ>>();

            for (int i = 0; i < curveAry.Count; ++i)
            {
                var curve1 = curveAry[i];
                var interPosAryTmp1 = new List<XYZ>();
                for (int j = 0; j < curveAry.Count; ++j)
                {
                    if (i == j) continue;
                    var interPosAryTmp = GetInterPosCurves2D(curve1, curveAry[j]);
                    foreach (var p in interPosAryTmp) interPosAryTmp1.Add(p);
                }

                IList<int> sortedIdxAry = null;
                IList<XYZ> sortedPosAryTmp1 = null;
                SortXYPos(interPosAryTmp1, 1, ref sortedIdxAry, ref sortedPosAryTmp1);

                var interPosAryTmp2 = new List<XYZ>();
                for (int j = 0; j < sortedPosAryTmp1.Count; ++j)
                {
                    bool flag = false;
                    for (int k = 0; k < interPosAryTmp2.Count; ++k)
                    {
                        if (Distance2D(sortedPosAryTmp1[j], interPosAryTmp2[k]) < Approx0Len)
                        { flag = true; break; }
                    }
                    if (!flag) interPosAryTmp2.Add(sortedPosAryTmp1[j]);
                }
                ret.Add(interPosAryTmp2);
            }
            return ret;
        }

        public int GetPosReClockwise(XYZ basePos, XYZ compPos, IList<XYZ> posAry, bool isComp)
        {
            int ret = -1;
            if (basePos == null || posAry == null || compPos == null) return ret;

            int idxPi = -1;
            var idxPosAry = new List<int>();
            for (int i = 0; i < posAry.Count; ++i)
            {
                var pos = posAry[i];
                if (Distance2D(basePos, pos) < Approx0Len) continue;
                if (Distance2D(compPos, pos) < Approx0Len) continue;

                double rad = Angle2D(basePos, compPos, pos);
                if (Math.Abs(Math.PI - Math.Abs(rad)) < Approx0Ang)
                    idxPi = i;
                else if (Math.Abs(rad) > Approx0Ang && rad < 0)
                    idxPosAry.Add(i);
            }

            double minRad = 0.0;
            int minIdx = -1;
            for (int i = 0; i < idxPosAry.Count; ++i)
            {
                int idx = idxPosAry[i];
                double rad = Math.Abs(Angle2D(basePos, compPos, posAry[idx]));
                if (i == 0 || rad < minRad) { minRad = rad; minIdx = idx; }
            }
            ret = minIdx;

            if (ret == -1 && isComp)
            {
                int idxCmp = -1;
                for (int i = 0; i < posAry.Count; ++i)
                {
                    if (Distance2D(basePos, posAry[i]) < Approx0Len) continue;
                    if (Distance2D(compPos, posAry[i]) < Approx0Len) { idxCmp = i; break; }
                }
                if (idxCmp > -1)
                {
                    bool flag = false;
                    for (int i = 0; i < posAry.Count; ++i)
                    {
                        if (i == idxCmp) continue;
                        if (Distance2D(basePos, posAry[i]) < Approx0Len) continue;
                        double rad = Angle2D(basePos, posAry[i], compPos);
                        if (Math.Abs(Math.PI - Math.Abs(rad)) < Approx0Ang) { flag = true; break; }
                        if (Math.Abs(rad) > Approx0Ang && rad < 0) { flag = true; break; }
                    }
                    if (flag) ret = idxCmp;
                }
            }

            if (ret == -1)
            {
                if (idxPi > -1) ret = idxPi;
                else if (posAry.Count == 1) ret = 0;
            }
            return ret;
        }

        public IList<XYZ> GetRelatedPos(XYZ basePos, XYZ exclPos,
            IList<IList<XYZ>> curveInterPosAryAry)
        {
            var ret = new List<XYZ>();
            var nextPosAry = new List<XYZ>();

            for (int i = 0; i < curveInterPosAryAry.Count; ++i)
            {
                for (int j = 0; j < curveInterPosAryAry[i].Count; ++j)
                {
                    if (Distance2D(basePos, curveInterPosAryAry[i][j]) < Approx0Len)
                    {
                        if (j + 1 < curveInterPosAryAry[i].Count)
                            nextPosAry.Add(curveInterPosAryAry[i][j + 1]);
                        if (j - 1 >= 0)
                            nextPosAry.Add(curveInterPosAryAry[i][j - 1]);
                    }
                }
            }

            foreach (var np in nextPosAry)
            {
                if (exclPos == null || Distance2D(exclPos, np) > Approx0Len)
                    ret.Add(np);
            }
            return ret;
        }

        public IList<Curve> GetPlanFaceCurveInterPos(XYZ basePos, XYZ relaPos,
            IList<IList<XYZ>> curveInterPosAryAry, double height)
        {
            var ret = new List<Curve>();
            var posAry = new List<XYZ> { basePos };

            var nextPosAry = GetRelatedPos(basePos, null, curveInterPosAryAry);
            int idxPos = GetPosReClockwise(basePos, relaPos, nextPosAry, true);
            if (idxPos == -1) return ret;

            var posN = nextPosAry[idxPos];
            posAry.Add(posN);
            var posB = posN;
            var posA = basePos;

            int numMax = 1000;
            bool flagEnd = false;
            for (int cnt = 0; cnt <= numMax && !flagEnd; cnt++)
            {
                posN = null;
                nextPosAry = GetRelatedPos(posB, posA, curveInterPosAryAry);
                idxPos = GetPosReClockwise(posB, posA, nextPosAry, false);
                if (idxPos > -1) posN = nextPosAry[idxPos];

                if (posN != null)
                {
                    posAry.Add(posN);
                    posA = posB;
                    posB = posN;
                    if (Distance2D(basePos, posN) < Approx0Len) flagEnd = true;
                }
                else break;
            }

            if (posAry.Count < 3) return ret;
            if (Distance2D(posAry[0], posAry[posAry.Count - 1]) > Approx0Len) return ret;

            bool flagRet = true;
            var retCurves = new List<Curve>();
            for (int i = 1; i < posAry.Count; ++i)
            {
                var pos1 = new XYZ(posAry[i - 1].X, posAry[i - 1].Y, height);
                var pos2 = new XYZ(posAry[i].X, posAry[i].Y, height);
                if (Distance2D(pos1, pos2) < Approx0Len) { flagRet = false; break; }

                Curve curve = Line.CreateBound(pos1, pos2);
                bool flag = true;
                for (int j = 0; j < retCurves.Count; ++j)
                {
                    var interPosAry = new List<XYZ>();
                    IList<XYZ> tmp = interPosAry;
                    IntersecCurve2D(curve, retCurves[j], ref tmp);
                    foreach (var ip in tmp)
                    {
                        if (Distance2D(pos1, ip) > Approx0Len && Distance2D(pos2, ip) > Approx0Len)
                        { flag = false; break; }
                    }
                    if (!flag) break;
                }
                if (flag) retCurves.Add(curve);
                else { flagRet = false; break; }
            }
            return flagRet ? retCurves : ret;
        }

        public IList<IList<Curve>> GetPlanFaceCurveInterPos(
            IList<IList<XYZ>> curveInterPosAryAry, double height)
        {
            var ret = new List<IList<Curve>>();
            var pFaceCurveAryAry = new List<IList<Curve>>();
            var pFaceGravityAry = new List<XYZ>();

            for (int i = 0; i < curveInterPosAryAry.Count; ++i)
            {
                var curveInterPosAry = curveInterPosAryAry[i];
                for (int j = 0; j < curveInterPosAry.Count; ++j)
                {
                    int idxN = j + 1;
                    if (idxN > curveInterPosAry.Count - 1) idxN = j - 1;
                    if (idxN < 0) continue;

                    var planCurves = GetPlanFaceCurveInterPos(
                        curveInterPosAry[j], curveInterPosAry[idxN],
                        curveInterPosAryAry, height);

                    if (planCurves.Count > 0)
                    {
                        var gravity = PolygonGravity2D(planCurves);
                        if (gravity == null) continue;

                        bool flag = true;
                        for (int k = 0; k < pFaceCurveAryAry.Count; ++k)
                        {
                            if (Distance2D(gravity, pFaceGravityAry[k]) < Approx0Len)
                            { flag = false; break; }
                        }
                        if (flag)
                        {
                            pFaceCurveAryAry.Add(planCurves);
                            pFaceGravityAry.Add(gravity);
                        }
                    }
                }
            }

            for (int i = 0; i < pFaceCurveAryAry.Count; ++i)
            {
                double area = Math.Abs(GetPolygonArea(pFaceCurveAryAry[i]));
                bool flag = true;
                for (int j = 0; j < pFaceCurveAryAry.Count; ++j)
                {
                    if (i == j) continue;
                    double areaTmp = Math.Abs(GetPolygonArea(pFaceCurveAryAry[j]));
                    bool isIn = IsPointInPolygon(pFaceCurveAryAry[j], pFaceGravityAry[j], pFaceGravityAry[i], 1);
                    if (isIn && (area - areaTmp) > 0.0)
                    { flag = false; break; }
                }
                if (flag) ret.Add(pFaceCurveAryAry[i]);
            }
            return ret;
        }
    }
}
