using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using RevitDB = Autodesk.Revit.DB;

namespace ADSK.Ext.Fukashi.Opening.Components
{
    /// <summary>Revit-independent geometry helpers inlined from legacy JExtCom.Rvt.Geometry (Opening).</summary>
    public partial class Geometry
    {
        public UIDocument RvtUIDoc { get; private set; }

        public RevitDB.Document RvtDBDoc => RvtUIDoc?.Document;

        public double Approx0Len => 1.0e-9;

        public double Approx0Ang => 1.0e-9;

        public double Distance(RevitDB.XYZ a, RevitDB.XYZ b)
        {
            if (a == null || b == null) return double.MaxValue;
            double dx = a.X - b.X, dy = a.Y - b.Y, dz = a.Z - b.Z;
            return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double Distance2D(RevitDB.XYZ a, RevitDB.XYZ b)
        {
            if (a == null || b == null) return double.MaxValue;
            double dx = a.X - b.X, dy = a.Y - b.Y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public RevitDB.XYZ UnitVector(RevitDB.XYZ from, RevitDB.XYZ to)
        {
            if (from == null || to == null) return RevitDB.XYZ.Zero;
            double vx = to.X - from.X, vy = to.Y - from.Y, vz = to.Z - from.Z;
            double len = System.Math.Sqrt(vx * vx + vy * vy + vz * vz);
            if (len < Approx0Len) return RevitDB.XYZ.Zero;
            return new RevitDB.XYZ(vx / len, vy / len, vz / len);
        }

        public double CrossProduct2D(RevitDB.XYZ origin, RevitDB.XYZ p1, RevitDB.XYZ p2)
        {
            double ax = p1.X - origin.X, ay = p1.Y - origin.Y;
            double bx = p2.X - origin.X, by = p2.Y - origin.Y;
            return ax * by - ay * bx;
        }

        /// <summary>Angle at p1 between vectors to p0 and p2 on the XY plane (signed atan2).</summary>
        public double Angle2D(RevitDB.XYZ p0, RevitDB.XYZ p1, RevitDB.XYZ p2)
        {
            double v1x = p0.X - p1.X, v1y = p0.Y - p1.Y;
            double v2x = p2.X - p1.X, v2y = p2.Y - p1.Y;
            return System.Math.Atan2(v1x * v2y - v1y * v2x, v1x * v2x + v1y * v2y);
        }

        /// <summary>Interior angle magnitude at vertex p1 (p0-p1-p2), 3D.</summary>
        public double Angle3D(RevitDB.XYZ p0, RevitDB.XYZ p1, RevitDB.XYZ p2)
        {
            if (p0 == null || p1 == null || p2 == null) return 0;
            RevitDB.XYZ v1 = new RevitDB.XYZ(p0.X - p1.X, p0.Y - p1.Y, p0.Z - p1.Z);
            RevitDB.XYZ v2 = new RevitDB.XYZ(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            double l1 = v1.GetLength();
            double l2 = v2.GetLength();
            if (l1 < Approx0Len || l2 < Approx0Len) return 0;
            double dot = (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z) / (l1 * l2);
            dot = System.Math.Max(-1, System.Math.Min(1, dot));
            return System.Math.Acos(dot);
        }

        public RevitDB.XYZ TriangleGravity2D(RevitDB.XYZ p0, RevitDB.XYZ p1, RevitDB.XYZ p2)
        {
            double z = p0.Z;
            return new RevitDB.XYZ((p0.X + p1.X + p2.X) / 3.0, (p0.Y + p1.Y + p2.Y) / 3.0, z);
        }

        public RevitDB.XYZ PolygonGravity2D(IList<RevitDB.Curve> curves)
        {
            if (curves == null || curves.Count == 0) return RevitDB.XYZ.Zero;
            double sx = 0, sy = 0, sz = curves[0].GetEndPoint(0).Z;
            int n = 0;
            foreach (RevitDB.Curve c in curves)
            {
                RevitDB.XYZ p = c.GetEndPoint(0);
                sx += p.X;
                sy += p.Y;
                n++;
            }
            if (n == 0) return RevitDB.XYZ.Zero;
            return new RevitDB.XYZ(sx / n, sy / n, sz);
        }

        public double GetPolygonArea(IList<RevitDB.Curve> curves)
        {
            if (curves == null || curves.Count < 3) return 0;
            var pts = new List<RevitDB.XYZ>();
            foreach (RevitDB.Curve c in curves)
                pts.Add(c.GetEndPoint(0));

            double a = 0;
            int count = pts.Count;
            for (int i = 0; i < count; i++)
            {
                RevitDB.XYZ p = pts[i];
                RevitDB.XYZ q = pts[(i + 1) % count];
                a += p.X * q.Y - q.X * p.Y;
            }
            return a / 2.0;
        }

        public bool IsPointOnPoint(RevitDB.XYZ a, RevitDB.XYZ b)
        {
            return Distance(a, b) < Approx0Len;
        }

        public bool IsPointInPolygon(IList<RevitDB.Curve> curves, RevitDB.XYZ gravForMode, RevitDB.XYZ pt, int mode)
        {
            _ = gravForMode;
            _ = mode;
            return IsPointInsideClosedPolyline(curves, pt);
        }

        private bool IsPointInsideClosedPolyline(IList<RevitDB.Curve> curves, RevitDB.XYZ pt)
        {
            if (curves == null || curves.Count < 3 || pt == null) return false;

            IList<RevitDB.Line> segs = new List<RevitDB.Line>();

            foreach (RevitDB.Curve c in curves)
            {
                foreach (RevitDB.Line ln in TessellateToLines(c))
                    segs.Add(ln);
            }

            double x = pt.X, y = pt.Y;
            bool inside = false;
            for (int i = 0, j = segs.Count - 1; i < segs.Count; j = i++)
            {
                RevitDB.XYZ pi = segs[i].GetEndPoint(0);
                RevitDB.XYZ pj = segs[j].GetEndPoint(0);
                if (((pi.Y > y) != (pj.Y > y)) &&
                    (x < (pj.X - pi.X) * (y - pi.Y) / (pj.Y - pi.Y + 1e-20) + pi.X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private static IEnumerable<RevitDB.Line> TessellateToLines(RevitDB.Curve c)
        {
            if (c is RevitDB.Line ln)
            {
                yield return ln;
                yield break;
            }

            IList<RevitDB.XYZ> tes = c.Tessellate();
            for (int i = 0; i + 1 < tes.Count; i++)
                yield return RevitDB.Line.CreateBound(tes[i], tes[i + 1]);
        }

        /// <remarks>Preserves curve order used by Fukashi workflows.</remarks>
        public IList<RevitDB.Curve> OptimizeLineVertexConvLine(IList<RevitDB.Curve> curves)
        {
            if (curves == null || curves.Count == 0)
                return new List<RevitDB.Curve>();

            var flat = new List<RevitDB.Curve>();

            foreach (RevitDB.Curve c in curves)
            {
                if (c is RevitDB.Line line)
                    flat.Add(line);
                else if (c != null)
                {
                    IList<RevitDB.XYZ> tes = c.Tessellate();
                    for (int i = 0; i + 1 < tes.Count; i++)
                        flat.Add(RevitDB.Line.CreateBound(tes[i], tes[i + 1]));
                }
            }

            return OptimizeLineVertexNoConvLine(flat);
        }

        public IList<RevitDB.Curve> OptimizeLineVertexNoConvLine(IList<RevitDB.Curve> curves)
        {
            if (curves == null || curves.Count == 0) return curves;
            var outList = new List<RevitDB.Curve>();

            foreach (RevitDB.Curve c in curves)
            {
                if (c == null) continue;
                if (outList.Count > 0 && c is RevitDB.Line cur && outList[outList.Count - 1] is RevitDB.Line prev)
                {
                    if (AreColinearXY(prev, cur) &&
                        Distance2D(prev.GetEndPoint(1), cur.GetEndPoint(0)) < Approx0Len)
                    {
                        try
                        {
                            RevitDB.Line merged = RevitDB.Line.CreateBound(prev.GetEndPoint(0), cur.GetEndPoint(1));
                            outList[outList.Count - 1] = merged;
                            continue;
                        }
                        catch { }
                    }
                }
                outList.Add(c);
            }
            return outList;
        }

        private static bool AreColinearXY(RevitDB.Line a, RevitDB.Line b)
        {
            RevitDB.XYZ p0 = a.GetEndPoint(0);
            RevitDB.XYZ p1 = a.GetEndPoint(1);
            RevitDB.XYZ p2 = b.GetEndPoint(0);
            RevitDB.XYZ p3 = b.GetEndPoint(1);
            double f1 = p1.X - p0.X, g1 = p1.Y - p0.Y;
            double f2 = p3.X - p2.X, g2 = p3.Y - p2.Y;
            double det = f2 * g1 - f1 * g2;
            return System.Math.Abs(det) < 1e-7;
        }

        public RevitDB.XYZ IntersecVector2D(RevitDB.Curve curve1, RevitDB.Curve curve2)
        {
            RevitDB.XYZ pos10 = curve1.GetEndPoint(0);
            RevitDB.XYZ pos11 = curve1.GetEndPoint(1);
            RevitDB.XYZ pos20 = curve2.GetEndPoint(0);
            RevitDB.XYZ pos21 = curve2.GetEndPoint(1);

            double x1 = pos10.X, y1 = pos10.Y;
            double x2 = pos11.X, y2 = pos11.Y;
            double x3 = pos20.X, y3 = pos20.Y;
            double x4 = pos21.X, y4 = pos21.Y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (System.Math.Abs(denom) < Approx0Len) return null;

            double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom;
            double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom;
            double z = pos10.Z;

            return new RevitDB.XYZ(px, py, z);
        }

        public void IntersecCurve2D(RevitDB.Curve curve1, RevitDB.Curve curve2, ref IList<RevitDB.XYZ> interPos)
        {
            if (interPos == null)
                interPos = new List<RevitDB.XYZ>();
            else
                interPos.Clear();

            RevitDB.Line l1 = curve1 as RevitDB.Line;
            RevitDB.Line l2 = curve2 as RevitDB.Line;
            if (l1 != null && l2 != null)
            {
                RevitDB.XYZ xy = IntersecVector2D(l1, l2);
                if (xy != null && PointOnBoundedLine2d(l1, xy) && PointOnBoundedLine2d(l2, xy))
                    interPos.Add(xy);
                return;
            }

            IList<RevitDB.XYZ> t1 = curve1.Tessellate();
            IList<RevitDB.XYZ> t2 = curve2.Tessellate();
            for (int i = 0; i + 1 < t1.Count; i++)
            {
                RevitDB.Line s1 = RevitDB.Line.CreateBound(t1[i], t1[i + 1]);
                for (int j = 0; j + 1 < t2.Count; j++)
                {
                    RevitDB.Line s2 = RevitDB.Line.CreateBound(t2[j], t2[j + 1]);
                    RevitDB.XYZ xy = IntersecVector2D(s1, s2);
                    if (xy != null && PointOnBoundedLine2d(s1, xy) && PointOnBoundedLine2d(s2, xy))
                        interPos.Add(xy);
                }
            }
        }

        private bool PointOnBoundedLine2d(RevitDB.Line line, RevitDB.XYZ pt)
        {
            RevitDB.XYZ p0 = line.GetEndPoint(0);
            RevitDB.XYZ p1 = line.GetEndPoint(1);
            double xmin = System.Math.Min(p0.X, p1.X) - 1e-6;
            double xmax = System.Math.Max(p0.X, p1.X) + 1e-6;
            double ymin = System.Math.Min(p0.Y, p1.Y) - 1e-6;
            double ymax = System.Math.Max(p0.Y, p1.Y) + 1e-6;
            return pt.X >= xmin && pt.X <= xmax && pt.Y >= ymin && pt.Y <= ymax;
        }

        public void IntersecCurve(RevitDB.Curve curve1, RevitDB.Curve curve2, ref IList<RevitDB.XYZ> intersects)
        {
            if (intersects == null)
                intersects = new List<RevitDB.XYZ>();
            else
                intersects.Clear();

            IList<RevitDB.Curve> segsA = TessellateToLineSegments(curve1);
            IList<RevitDB.Curve> segsB = TessellateToLineSegments(curve2);
            foreach (RevitDB.Curve sa in segsA)
            {
                foreach (RevitDB.Curve sb in segsB)
                {
                    if (!(sa is RevitDB.Line) || !(sb is RevitDB.Line)) continue;

                    IList<RevitDB.XYZ> pts = null;
                    IntersecCurve2D(sa, sb, ref pts);
                    if (pts != null && pts.Count > 0)
                    {
                        foreach (RevitDB.XYZ p in pts)
                            intersects.Add(p);
                    }
                }
            }
        }

        private static IList<RevitDB.Curve> TessellateToLineSegments(RevitDB.Curve curve)
        {
            if (curve is RevitDB.Line ln)
                return new List<RevitDB.Curve> { ln };
            IList<RevitDB.XYZ> tes = curve.Tessellate();
            var list = new List<RevitDB.Curve>();
            for (int i = 0; i + 1 < tes.Count; i++)
                list.Add(RevitDB.Line.CreateBound(tes[i], tes[i + 1]));
            return list;
        }

        public void SortXYPos(
            IList<RevitDB.XYZ> positions,
            int mode,
            ref IList<int> sortedIdx,
            ref IList<RevitDB.XYZ> sortedPos)
        {
            _ = mode;
            if (sortedIdx == null)
                sortedIdx = new List<int>();
            else
                sortedIdx.Clear();

            if (sortedPos == null)
                sortedPos = new List<RevitDB.XYZ>();
            else
                sortedPos.Clear();

            if (positions == null || positions.Count == 0) return;

            double cx = positions.Average(p => p.X);
            double cy = positions.Average(p => p.Y);

            var infos = Enumerable.Range(0, positions.Count)
                .Select(i => (
                    Idx: i,
                    Ang: System.Math.Atan2(positions[i].Y - cy, positions[i].X - cx)))
                .OrderBy(t => t.Ang).ThenBy(t => positions[t.Idx].X).ThenBy(t => positions[t.Idx].Y)
                .ToList();

            foreach (var inf in infos)
            {
                sortedIdx.Add(inf.Idx);
                sortedPos.Add(positions[inf.Idx]);
            }
        }
    }
}
