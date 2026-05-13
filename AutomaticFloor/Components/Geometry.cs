using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutomaticFloor.Components
{
    public class Geometry
    {
        private double _ToleranceInter;
        private readonly UIDocument _rvtUIDoc;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _ToleranceInter = 0.0;
        }

        // Tolerance for near-zero length comparisons (~0.0003mm)
        public double Approx0Len => 1e-6;

        // Tolerance for near-zero angle comparisons
        public double Approx0Ang => 1e-6;

        // Unit conversion: internal units (feet) to millimeters
        public double UnitCoe => 304.8;

        public double ToleranceInter
        {
            get => _ToleranceInter;
            set => _ToleranceInter = value;
        }

        public double Distance2D(XYZ pos1, XYZ pos2)
        {
            double dx = pos1.X - pos2.X;
            double dy = pos1.Y - pos2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Signed angle from vector (basePos->fromPos) to vector (basePos->toPos), CCW positive
        public double Angle2D(XYZ basePos, XYZ fromPos, XYZ toPos)
        {
            double ax = fromPos.X - basePos.X;
            double ay = fromPos.Y - basePos.Y;
            double bx = toPos.X - basePos.X;
            double by = toPos.Y - basePos.Y;

            double cross = ax * by - ay * bx;
            double dot = ax * bx + ay * by;
            return Math.Atan2(cross, dot);
        }

        public XYZ UnitVector(XYZ from, XYZ to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < Approx0Len) return new XYZ(0, 0, 0);
            return new XYZ(dx / len, dy / len, 0);
        }

        // Find 2D intersection of two line segments
        public void IntersecCurve2D(Curve curve1, Curve curve2, ref IList<XYZ> interPosAry)
        {
            XYZ p1 = curve1.GetEndPoint(0);
            XYZ p2 = curve1.GetEndPoint(1);
            XYZ p3 = curve2.GetEndPoint(0);
            XYZ p4 = curve2.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y;
            double x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y;
            double x4 = p4.X, y4 = p4.Y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < 1e-12) return;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denom;

            if (t >= -Approx0Len && t <= 1.0 + Approx0Len && u >= -Approx0Len && u <= 1.0 + Approx0Len)
            {
                double ix = x1 + t * (x2 - x1);
                double iy = y1 + t * (y2 - y1);
                interPosAry.Add(new XYZ(ix, iy, 0));
            }
        }

        // Sort points by distance from first point along the line direction
        public void SortXYPos(IList<XYZ> posAry, int sortMode, ref IList<int> sortedIdxAry, ref IList<XYZ> sortedPosAry)
        {
            if (posAry == null || posAry.Count == 0) return;

            var indexedList = new List<(int idx, XYZ pos, double dist)>();
            XYZ origin = posAry[0];

            for (int i = 0; i < posAry.Count; i++)
            {
                double dist = Distance2D(origin, posAry[i]);
                indexedList.Add((i, posAry[i], dist));
            }

            indexedList.Sort((a, b) => a.dist.CompareTo(b.dist));

            foreach (var item in indexedList)
            {
                sortedIdxAry.Add(item.idx);
                sortedPosAry.Add(item.pos);
            }
        }

        public XYZ PolygonGravity2D(IList<Curve> curves)
        {
            if (curves == null || curves.Count == 0) return null;

            double sumX = 0, sumY = 0;
            int count = 0;
            foreach (Curve curve in curves)
            {
                XYZ p = curve.GetEndPoint(0);
                sumX += p.X;
                sumY += p.Y;
                count++;
            }
            if (count == 0) return null;
            return new XYZ(sumX / count, sumY / count, 0);
        }

        // Shoelace formula for signed polygon area
        public double GetPolygonArea(IList<Curve> curves)
        {
            if (curves == null || curves.Count == 0) return 0;

            List<XYZ> pts = new List<XYZ>();
            foreach (Curve curve in curves)
            {
                pts.Add(curve.GetEndPoint(0));
            }

            double area = 0;
            int n = pts.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += pts[i].X * pts[j].Y;
                area -= pts[j].X * pts[i].Y;
            }
            return area / 2.0;
        }

        public Curve GetElementLocCurve(Element element)
        {
            if (element?.Location is LocationCurve locCurve)
                return locCurve.Curve;
            return null;
        }

        // ---- Methods ported from original Geometry.cs (these used to call base.xxx) ----

        public bool GetCurveOnPos2D(Curve curve, XYZ pos)
        {
            bool ret = false;
            XYZ pos0 = curve.GetEndPoint(0);
            XYZ pos1 = curve.GetEndPoint(1);

            if (Distance2D(pos0, pos) < Approx0Len)
                ret = true;
            else if (Distance2D(pos1, pos) < Approx0Len)
                ret = true;

            if (!ret)
            {
                double rad0 = Math.Abs(Angle2D(pos0, pos1, pos));
                double rad1 = Math.Abs(Angle2D(pos1, pos0, pos));
                if (rad0 < Approx0Ang && rad1 < Approx0Ang)
                    ret = true;
            }

            return ret;
        }

        public IList<XYZ> GetInterPosCurves2D(Curve curve1, Curve curve2)
        {
            IList<XYZ> ret = new List<XYZ>();

            XYZ pos10 = curve1.GetEndPoint(0);
            XYZ pos11 = curve1.GetEndPoint(1);
            XYZ pos20 = curve2.GetEndPoint(0);
            XYZ pos21 = curve2.GetEndPoint(1);
            XYZ interPos = null;

            bool isPalla = false;
            XYZ vec1 = UnitVector(pos10, pos11);
            XYZ vec2 = UnitVector(pos20, pos21);
            XYZ vec1Rev = new XYZ(vec1.X * -1.0, vec1.Y * -1.0, 0.0);

            if (Distance2D(vec1, vec2) < Approx0Len)
                isPalla = true;
            if (Distance2D(vec1Rev, vec2) < Approx0Len)
                isPalla = true;

            if (isPalla)
            {
                interPos = null;
                if (Distance2D(pos10, pos20) < Approx0Len)
                    interPos = pos10;
                else if (Distance2D(pos10, pos21) < Approx0Len)
                    interPos = pos10;
                else if (Distance2D(pos11, pos20) < Approx0Len)
                    interPos = pos11;
                else if (Distance2D(pos11, pos21) < Approx0Len)
                    interPos = pos11;

                if (interPos != null)
                    ret.Add(interPos);
            }
            else
            {
                IList<XYZ> interPosAryTmp = new List<XYZ>();
                IntersecCurve2D(curve1, curve2, ref interPosAryTmp);
                for (int i = 0; i < interPosAryTmp.Count; ++i)
                {
                    XYZ interPosTmp = new XYZ(interPosAryTmp[i].X, interPosAryTmp[i].Y, curve1.GetEndPoint(0).Z);
                    bool flag = true;
                    if (interPos != null)
                    {
                        if (Distance2D(interPos, interPosTmp) < ToleranceInter)
                            flag = false;
                    }
                    if (flag)
                        ret.Add(interPosTmp);
                }
            }

            return ret;
        }

        public IList<IList<XYZ>> GetInterPosCurves(IList<Curve> curveAry)
        {
            IList<IList<XYZ>> ret = new List<IList<XYZ>>();

            for (int i = 0; i < curveAry.Count; ++i)
            {
                Curve curve1 = curveAry[i];
                IList<XYZ> interPosAryTmp1 = new List<XYZ>();
                for (int j = 0; j < curveAry.Count; ++j)
                {
                    if (i == j) continue;
                    Curve curve2 = curveAry[j];

                    IList<XYZ> interPosAryTmp = GetInterPosCurves2D(curve1, curve2);
                    for (int k = 0; k < interPosAryTmp.Count; ++k)
                        interPosAryTmp1.Add(interPosAryTmp[k]);
                }

                IList<int> sortedIdxAry = new List<int>();
                IList<XYZ> sortedPosAryTmp1 = new List<XYZ>();
                SortXYPos(interPosAryTmp1, 1, ref sortedIdxAry, ref sortedPosAryTmp1);

                IList<XYZ> interPosAryTmp2 = new List<XYZ>();
                for (int j = 0; j < sortedPosAryTmp1.Count; ++j)
                {
                    bool flag = false;
                    for (int k = 0; k < interPosAryTmp2.Count; ++k)
                    {
                        if (Distance2D(sortedPosAryTmp1[j], interPosAryTmp2[k]) < Approx0Len)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        interPosAryTmp2.Add(sortedPosAryTmp1[j]);
                }

                ret.Add(interPosAryTmp2);
            }
            return ret;
        }

        public int GetPosReClockwise(XYZ basePos, XYZ compPos, IList<XYZ> posAry, bool isComp)
        {
            int ret = -1;
            XYZ pos = null;
            double rad = 0.0;
            IList<int> idxPosAry = null;

            if (basePos == null || posAry == null || compPos == null)
                return ret;

            int idxPi = -1;
            idxPosAry = new List<int>();
            for (int i = 0; i < posAry.Count; ++i)
            {
                pos = posAry[i];
                if (Distance2D(basePos, pos) < Approx0Len) continue;
                if (Distance2D(compPos, pos) < Approx0Len) continue;

                rad = Angle2D(basePos, compPos, pos);
                if (Math.Abs(Math.PI - Math.Abs(rad)) < Approx0Ang)
                    idxPi = i;
                else
                {
                    if (Math.Abs(rad) > Approx0Ang)
                    {
                        if (rad < 0)
                            idxPosAry.Add(i);
                    }
                }
            }

            double minRad = 0.0;
            int minIdx = -1;
            for (int i = 0; i < idxPosAry.Count; ++i)
            {
                int idx = idxPosAry[i];
                rad = Math.Abs(Angle2D(basePos, compPos, posAry[idx]));
                if (i == 0)
                {
                    minRad = rad;
                    minIdx = idx;
                }
                else if (rad < minRad)
                {
                    minRad = rad;
                    minIdx = idx;
                }
            }
            ret = minIdx;

            if (ret == -1 && isComp)
            {
                int idxCmp = -1;
                for (int i = 0; i < posAry.Count; ++i)
                {
                    pos = posAry[i];
                    if (Distance2D(basePos, pos) < Approx0Len) continue;
                    if (Distance2D(compPos, pos) < Approx0Len)
                    {
                        idxCmp = i;
                        break;
                    }
                }
                if (idxCmp > -1)
                {
                    bool flag = false;
                    for (int i = 0; i < posAry.Count; ++i)
                    {
                        if (i == idxCmp) continue;
                        pos = posAry[i];
                        if (Distance2D(basePos, pos) < Approx0Len) continue;

                        rad = Angle2D(basePos, pos, compPos);
                        if (Math.Abs(Math.PI - Math.Abs(rad)) < Approx0Ang)
                        {
                            flag = true;
                            break;
                        }
                        else if (Math.Abs(rad) > Approx0Ang && rad < 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag) ret = idxCmp;
                }
            }

            if (ret == -1)
            {
                if (idxPi > -1)
                    ret = idxPi;
                else if (posAry.Count == 1)
                    ret = 0;
            }

            return ret;
        }

        public IList<XYZ> GetRelatedPos(XYZ basePos, XYZ exclPos, IList<IList<XYZ>> curveInterPosAryAry)
        {
            IList<XYZ> ret = new List<XYZ>();
            IList<XYZ> nextPosAry = new List<XYZ>();

            for (int i = 0; i < curveInterPosAryAry.Count; ++i)
            {
                for (int j = 0; j < curveInterPosAryAry[i].Count; ++j)
                {
                    if (Distance2D(basePos, curveInterPosAryAry[i][j]) < Approx0Len)
                    {
                        int k = j + 1;
                        if (k < curveInterPosAryAry[i].Count)
                            nextPosAry.Add(curveInterPosAryAry[i][k]);

                        k = j - 1;
                        if (k > -1)
                            nextPosAry.Add(curveInterPosAryAry[i][k]);
                    }
                }
            }

            if (nextPosAry.Count > 0)
            {
                for (int i = 0; i < nextPosAry.Count; ++i)
                {
                    if (exclPos == null)
                        ret.Add(nextPosAry[i]);
                    else if (Distance2D(exclPos, nextPosAry[i]) > Approx0Len)
                        ret.Add(nextPosAry[i]);
                }
            }

            return ret;
        }

        public IList<Curve> GetPlanFaceCurveInterPos(XYZ basePos, XYZ relaPos, IList<IList<XYZ>> curveInterPosAryAry, double height)
        {
            IList<Curve> ret = new List<Curve>();

            XYZ posF = basePos;
            XYZ posB = basePos;
            XYZ posA = null;
            XYZ posN = null;

            int cntNum = -1;
            int numMax = 1000;
            bool flagEnd = false;

            IList<XYZ> posAry = new List<XYZ>();
            posAry.Add(posF);

            IList<XYZ> nextPosAry = GetRelatedPos(posF, null, curveInterPosAryAry);
            int idxPos = GetPosReClockwise(posF, relaPos, nextPosAry, true);
            if (idxPos == -1)
                return ret;
            else
                posN = nextPosAry[idxPos];

            posAry.Add(posN);
            posB = posN;
            posA = posF;

            while (!flagEnd)
            {
                cntNum++;
                if (cntNum > numMax) break;

                posN = null;
                nextPosAry = GetRelatedPos(posB, posA, curveInterPosAryAry);
                idxPos = GetPosReClockwise(posB, posA, nextPosAry, false);
                if (idxPos > -1)
                    posN = nextPosAry[idxPos];

                if (posN != null)
                {
                    posAry.Add(posN);
                    posA = posB;
                    posB = posN;
                    if (Distance2D(posF, posN) < Approx0Len)
                        flagEnd = true;
                }
                else
                    break;
            }

            if (posAry.Count < 3) return ret;
            if (Distance2D(posAry[0], posAry[posAry.Count - 1]) > Approx0Len)
                return ret;

            bool flagRet = true;
            IList<Curve> retCurves = new List<Curve>();
            for (int i = 1; i < posAry.Count; ++i)
            {
                XYZ pos1 = new XYZ(posAry[i - 1].X, posAry[i - 1].Y, height);
                XYZ pos2 = new XYZ(posAry[i].X, posAry[i].Y, height);
                if (Distance2D(pos1, pos2) < Approx0Len)
                {
                    flagRet = false;
                    break;
                }

                Curve curve = null;
                try { curve = Line.CreateBound(pos1, pos2); }
                catch (Exception) { continue; }

                bool flag = true;
                for (int j = 0; j < retCurves.Count; ++j)
                {
                    IList<XYZ> interPosAry = new List<XYZ>();
                    IntersecCurve2D(curve, retCurves[j], ref interPosAry);
                    for (int k = 0; k < interPosAry.Count; ++k)
                    {
                        if (Distance2D(pos1, interPosAry[k]) > Approx0Len &&
                            Distance2D(pos2, interPosAry[k]) > Approx0Len)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag) break;
                }

                if (flag)
                    retCurves.Add(curve);
                else
                {
                    flagRet = false;
                    break;
                }
            }

            if (flagRet)
                ret = retCurves;

            return ret;
        }

        public bool GetPlanFaceCurveInterPos(
            IList<IList<XYZ>> curveInterPosAryAry,
            double height,
            out IList<IList<Curve>> ret,
            out Dictionary<int, List<int>> dic_indexs)
        {
            ret = new List<IList<Curve>>();

            List<List<Curve>> pFaceCurveAryAry = new List<List<Curve>>();
            IList<XYZ> pFaceGravityAry = new List<XYZ>();

            for (int i = 0; i < curveInterPosAryAry.Count; ++i)
            {
                IList<XYZ> curveInterPosAry = curveInterPosAryAry[i];
                for (int j = 0; j < curveInterPosAry.Count; ++j)
                {
                    XYZ curveInterPos = curveInterPosAry[j];
                    XYZ basePos2 = curveInterPos;

                    int idxN = j + 1;
                    if (idxN > curveInterPosAry.Count - 1) idxN = j - 1;
                    if (idxN < 0) continue;
                    XYZ relaPos = curveInterPosAry[idxN];

                    IList<Curve> iList_planCurves = GetPlanFaceCurveInterPos(basePos2, relaPos, curveInterPosAryAry, height);
                    var planCurves = iList_planCurves as List<Curve>;

                    if (planCurves != null && planCurves.Count > 0)
                    {
                        XYZ gravity = PolygonGravity2D(planCurves);
                        if (gravity == null) continue;

                        bool flag = true;
                        for (int k = 0; k < pFaceCurveAryAry.Count; ++k)
                        {
                            if (Distance2D(gravity, pFaceGravityAry[k]) < Approx0Len)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            pFaceCurveAryAry.Add(planCurves);
                            pFaceGravityAry.Add(gravity);
                        }
                    }
                }
            }

            pFaceCurveAryAry.Sort((a, b) =>
            {
                double area1 = Math.Abs(GetPolygonArea(a));
                double area2 = Math.Abs(GetPolygonArea(b));
                return area1.CompareTo(area2);
            });

            dic_indexs = new Dictionary<int, List<int>>();
            List<int> bounds = new List<int>();

            for (int i = 0; i < pFaceCurveAryAry.Count; ++i)
            {
                if (!dic_indexs.ContainsKey(i))
                    dic_indexs.Add(i, new List<int>());

                IList<Curve> pFaceCurveAry = pFaceCurveAryAry[i];
                List<XYZ> points = new List<XYZ>();
                foreach (Curve curve in pFaceCurveAry)
                    points.AddRange(curve.Tessellate());

                for (int j = 0; j < pFaceCurveAryAry.Count; ++j)
                {
                    if (i == j) continue;
                    IList<Curve> pFaceCurveAryTmp = pFaceCurveAryAry[j];

                    List<XYZ> point2s = new List<XYZ>();
                    foreach (Curve curve in pFaceCurveAryTmp)
                        point2s.AddRange(curve.Tessellate());

                    int num = 0;
                    foreach (XYZ p in point2s)
                    {
                        if (isPointInPolyline(points, p))
                            ++num;
                    }

                    if (num == point2s.Count)
                        dic_indexs[i].Add(j);
                }
            }

            foreach (KeyValuePair<int, List<int>> keypair in dic_indexs)
            {
                if (!bounds.Contains(keypair.Key))
                    bounds.Add(keypair.Key);
                foreach (int j in keypair.Value)
                {
                    if (j == keypair.Key) continue;
                    if (!bounds.Contains(j))
                        bounds.Add(j);
                }
            }

            foreach (int bound_index in bounds)
                ret.Add(pFaceCurveAryAry[bound_index]);

            return true;
        }

        public bool isPointInPolyline(List<XYZ> polyline, XYZ pt)
        {
            int n = polyline.Count;
            double angle = 0;

            for (int i = 0; i < n; i++)
            {
                double pt1_X = polyline[i].X - pt.X;
                double pt1_Y = polyline[i].Y - pt.Y;
                double pt2_X = polyline[((i + 1) % n)].X - pt.X;
                double pt2_Y = polyline[((i + 1) % n)].Y - pt.Y;
                angle += Angle2D_Private(pt1_X, pt1_Y, pt2_X, pt2_Y);
            }

            return Math.Abs(angle) >= Math.PI;
        }

        private double Angle2D_Private(double x1, double y1, double x2, double y2)
        {
            double theta1 = Math.Atan2(y1, x1);
            double theta2 = Math.Atan2(y2, x2);
            double dtheta = theta2 - theta1;
            while (dtheta > Math.PI) dtheta -= (Math.PI * 2);
            while (dtheta < -Math.PI) dtheta += (Math.PI * 2);
            return dtheta;
        }
    }
}
