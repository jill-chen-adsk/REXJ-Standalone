using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public partial class Geometry
    {
        public double CrossProduct2D(XYZ origin, XYZ p1, XYZ p2)
        {
            double ax = p1.X - origin.X, ay = p1.Y - origin.Y;
            double bx = p2.X - origin.X, by = p2.Y - origin.Y;
            return ax * by - ay * bx;
        }

        public double DotProduct2D(XYZ origin, XYZ p1, XYZ p2)
        {
            double ax = p1.X - origin.X, ay = p1.Y - origin.Y;
            double bx = p2.X - origin.X, by = p2.Y - origin.Y;
            return ax * bx + ay * by;
        }

        public void IntersecCurve2D(Curve c1, Curve c2, ref XYZ interPos)
        {
            XYZ v = IntersecVector2D(c1, c2);
            interPos = v != null ? new XYZ(v.X, v.Y, c1.GetEndPoint(0).Z) : null;
        }

        public bool CompareParallelism(Curve c1, Curve c2)
        {
            var u1 = UnitVector(c1.GetEndPoint(0), c1.GetEndPoint(1));
            var u2 = UnitVector(c2.GetEndPoint(0), c2.GetEndPoint(1));
            if (u1.DistanceTo(XYZ.Zero) < Approx0Len || u2.DistanceTo(XYZ.Zero) < Approx0Len) return false;
            var u1n = new XYZ(-u1.X, -u1.Y, 0);
            return Distance2D(u1, u2) < Approx0Len || Distance2D(u1n, u2) < Approx0Len;
        }

        public XYZ Center2Point(Curve c)
        {
            XYZ a = c.GetEndPoint(0);
            XYZ b = c.GetEndPoint(1);
            return Center2Point(a, b);
        }

        public XYZ Center2Point(XYZ a, XYZ b)
        {
            return new XYZ((a.X + b.X) * 0.5, (a.Y + b.Y) * 0.5, (a.Z + b.Z) * 0.5);
        }

        public XYZ GetLeaning(XYZ origin, XYZ target)
        {
            return UnitVector(origin, target);
        }

        public XYZ GetVerticalPos2D(Curve baseline, XYZ pt)
        {
            XYZ p0 = baseline.GetEndPoint(0);
            XYZ p1 = baseline.GetEndPoint(1);
            double lx = p1.X - p0.X;
            double ly = p1.Y - p0.Y;
            double lenSq = lx * lx + ly * ly;
            if (lenSq < Approx0Len * Approx0Len)
                return p0;
            double t = ((pt.X - p0.X) * lx + (pt.Y - p0.Y) * ly) / lenSq;
            return new XYZ(p0.X + t * lx, p0.Y + t * ly, p0.Z);
        }

        public bool IsEqualCurve2D(Curve a, Curve b)
        {
            if (a == null || b == null) return false;
            double tol = Approx0Len;
            bool sameEndpoints =
                (Distance2D(a.GetEndPoint(0), b.GetEndPoint(0)) < tol && Distance2D(a.GetEndPoint(1), b.GetEndPoint(1)) < tol) ||
                (Distance2D(a.GetEndPoint(0), b.GetEndPoint(1)) < tol && Distance2D(a.GetEndPoint(1), b.GetEndPoint(0)) < tol);
            if (!sameEndpoints) return false;
            return Math.Abs(a.Length - b.Length) < tol;
        }

        public XYZ GetElementLocPos(Element elem)
        {
            if (elem == null) return XYZ.Zero;
            if (elem.Location is LocationPoint lp)
                return lp.Point;
            if (elem is Room room && room.Location is LocationPoint rlp)
                return rlp.Point;
            if (elem is Area area && area.Location is LocationPoint alp)
                return alp.Point;
            return XYZ.Zero;
        }

        public void GetRoomCurves(Room room, SpatialElementBoundaryLocation loc, ref IList<IList<Curve>> loops)
        {
            loops = new List<IList<Curve>>();
            if (room == null) return;
            var opt = new SpatialElementBoundaryOptions { SpatialElementBoundaryLocation = loc };
            IList<IList<BoundarySegment>> segs = room.GetBoundarySegments(opt);
            if (segs == null) return;
            foreach (IList<BoundarySegment> loop in segs)
            {
                var curves = new List<Curve>();
                foreach (BoundarySegment bs in loop)
                {
                    Curve c = bs.GetCurve();
                    if (c != null) curves.Add(c);
                }
                if (curves.Count > 0)
                    loops.Add(curves);
            }
        }

        public void GetAreaCurves(Area area, ref IList<IList<Curve>> loops)
        {
            loops = new List<IList<Curve>>();
            if (area == null) return;
            var opt = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
            };
            IList<IList<BoundarySegment>> segs = area.GetBoundarySegments(opt);
            if (segs == null) return;
            foreach (IList<BoundarySegment> loop in segs)
            {
                var curves = new List<Curve>();
                foreach (BoundarySegment bs in loop)
                {
                    Curve c = bs.GetCurve();
                    if (c != null) curves.Add(c);
                }
                if (curves.Count > 0)
                    loops.Add(curves);
            }
        }

        public IList<IList<IList<Curve>>> AreasBoundaries(IList<Area> areaAry)
        {
            var ret = new List<IList<IList<Curve>>>();
            if (areaAry == null) return ret;
            foreach (Area area in areaAry)
            {
                IList<IList<Curve>> curveAryAry = new List<IList<Curve>>();
                GetAreaCurves(area, ref curveAryAry);
                ret.Add(curveAryAry);
            }
            return ret;
        }

        public IList<Curve> GetCircle(IList<Curve> crvAry)
        {
            if (crvAry == null || crvAry.Count != 2) return null;
            if (crvAry[0] is Arc a0 && crvAry[1] is Arc a1)
            {
                if (Distance2D(a0.Center, a1.Center) < Approx0Len)
                    return new List<Curve> { a0, a1 };
            }
            return null;
        }

        public IList<Curve> OptimizeLineVertexNoConvLine(IList<Curve> curves)
        {
            if (curves == null || curves.Count == 0) return curves;

            // First pass: merge collinear connected segments (sequential adjacency)
            var outList = new List<Curve>();
            foreach (Curve c in curves)
            {
                if (c == null) continue;
                if (outList.Count > 0 && c is Line && outList[outList.Count - 1] is Line prev)
                {
                    if (CompareParallelism(prev, c) &&
                        Distance2D(prev.GetEndPoint(1), c.GetEndPoint(0)) < Approx0Len)
                    {
                        try
                        {
                            Line merged = Line.CreateBound(prev.GetEndPoint(0), c.GetEndPoint(1));
                            outList[outList.Count - 1] = merged;
                            continue;
                        }
                        catch { }
                    }
                }
                outList.Add(c);
            }

            // Wrap-around: merge last and first segments if collinear and connected
            if (outList.Count > 2 && outList[outList.Count - 1] is Line lastSeg && outList[0] is Line firstSeg)
            {
                if (CompareParallelism(lastSeg, firstSeg) &&
                    Distance2D(lastSeg.GetEndPoint(1), firstSeg.GetEndPoint(0)) < Approx0Len)
                {
                    try
                    {
                        Line merged = Line.CreateBound(lastSeg.GetEndPoint(0), firstSeg.GetEndPoint(1));
                        outList[0] = merged;
                        outList.RemoveAt(outList.Count - 1);
                    }
                    catch { }
                }
            }

            // Second pass: remove very short segments that create spurious vertices
            // at corners, and extend adjacent segments to meet.
            // Threshold: segments shorter than 2% of total perimeter are considered artifacts.
            if (outList.Count > 3)
            {
                double totalLength = 0;
                foreach (var seg in outList) totalLength += seg.ApproximateLength;
                double shortSegThreshold = totalLength * 0.02;
                if (shortSegThreshold < 0.033) shortSegThreshold = 0.033;

                var filtered = new List<Curve>();
                for (int i = 0; i < outList.Count; i++)
                {
                    if (outList[i] is Line ln && ln.Length < shortSegThreshold)
                        continue;
                    filtered.Add(outList[i]);
                }
                if (filtered.Count >= 3 && filtered.Count < outList.Count)
                {
                    // Reconnect the remaining segments by adjusting endpoints
                    var reconnected = new List<Curve>();
                    for (int i = 0; i < filtered.Count; i++)
                    {
                        int next = (i + 1) % filtered.Count;
                        XYZ end = filtered[i].GetEndPoint(1);
                        XYZ start = filtered[next].GetEndPoint(0);
                        if (Distance2D(end, start) > Approx0Len && filtered[i] is Line currLine && filtered[next] is Line nextLine)
                        {
                            XYZ interPos = null;
                            IntersecCurve2D(currLine, nextLine, ref interPos);
                            if (interPos != null)
                            {
                                try
                                {
                                    var newCurr = Line.CreateBound(currLine.GetEndPoint(0), interPos);
                                    var newNext = Line.CreateBound(interPos, nextLine.GetEndPoint(1));
                                    filtered[i] = newCurr;
                                    filtered[next] = newNext;
                                }
                                catch { }
                            }
                        }
                    }
                    outList = filtered;
                }
            }

            return outList;
        }

        public IList<int> GetLeftBottomCurveIndex(IList<Curve> figure)
        {
            var ret = new List<int> { -1, -1 };
            if (figure == null || figure.Count == 0) return ret;
            double minX = double.MaxValue, minY = double.MaxValue;
            foreach (Curve c in figure)
            {
                foreach (int k in new[] { 0, 1 })
                {
                    XYZ p = c.GetEndPoint(k);
                    if (p.X < minX - Approx0Len || (Math.Abs(p.X - minX) < Approx0Len && p.Y < minY))
                    {
                        minX = p.X;
                        minY = p.Y;
                    }
                }
            }
            for (int i = 0; i < figure.Count; i++)
            {
                for (int k = 0; k < 2; k++)
                {
                    XYZ p = figure[i].GetEndPoint(k);
                    if (Distance2D(p, new XYZ(minX, minY, p.Z)) < Approx0Len)
                    {
                        if (ret[0] < 0) ret[0] = i;
                        else if (ret[1] < 0 && ret[0] != i) { ret[1] = i; break; }
                    }
                }
            }
            if (ret[1] < 0 && figure.Count > 1)
                ret[1] = (ret[0] + 1) % figure.Count;
            return ret;
        }

        public double GetArcAngle(IList<Arc> arcs)
        {
            if (arcs == null || arcs.Count == 0) return 0;
            double sum = 0;
            foreach (Arc a in arcs)
            {
                double sw = a.GetEndParameter(1) - a.GetEndParameter(0);
                sum += Math.Abs(sw);
            }
            return sum;
        }

        public void GetArcOrder(IList<Arc> arcs, ref int index1, ref int index2)
        {
            index1 = 0;
            index2 = arcs != null && arcs.Count > 1 ? 1 : 0;
        }

        public XYZ GetArcMid(XYZ pos, XYZ center, double radius, double curveLength, ref double angleOut)
        {
            angleOut = curveLength / Math.Max(radius, Approx0Len);
            XYZ dv = pos - center;
            double dl = Math.Sqrt(dv.X * dv.X + dv.Y * dv.Y);
            if (dl < Approx0Len)
                return pos;
            double ux = dv.X / dl * radius;
            double uy = dv.Y / dl * radius;
            return center + new XYZ(ux, uy, 0);
        }
    }
}
