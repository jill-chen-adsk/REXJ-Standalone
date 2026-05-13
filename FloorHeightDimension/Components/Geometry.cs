using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
{
    /// <summary>Geometry helpers (ported from shared Rvt.Geometry + project extensions).</summary>
    public class Geometry
    {
        private readonly UIDocument _rvtUIDoc;
        private double _toleranceInter;

        private const double DefaultApprox0Len = 1e-6;
        private const double DefaultApprox0Ang = 1e-6;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _toleranceInter = 0.0;
        }

        public double Approx0Len => DefaultApprox0Len;
        public double Approx0Ang => DefaultApprox0Ang;

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public double UnitCoe
        {
            get
            {
                var units = _rvtUIDoc.Document.GetUnits();
                var fmtOpts = units.GetFormatOptions(SpecTypeId.Length);
                var unitTypeId = fmtOpts.GetUnitTypeId();
                if (unitTypeId == UnitTypeId.Millimeters) return 304.8;
                if (unitTypeId == UnitTypeId.Centimeters) return 30.48;
                if (unitTypeId == UnitTypeId.Meters) return 0.3048;
                if (unitTypeId == UnitTypeId.Feet) return 1.0;
                if (unitTypeId == UnitTypeId.Inches) return 12.0;
                return 304.8;
            }
        }

        public double ToleranceInter
        {
            get => _toleranceInter;
            set => _toleranceInter = value;
        }

        public double Distance(XYZ p, XYZ q)
        {
            double dx = p.X - q.X;
            double dy = p.Y - q.Y;
            double dz = p.Z - q.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
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

        public bool GetCurveOnPos2D(Curve curve, XYZ pos)
        {
            var pos0 = curve.GetEndPoint(0);
            var pos1 = curve.GetEndPoint(1);

            if (Distance2D(pos0, pos) < Approx0Len) return true;
            if (Distance2D(pos1, pos) < Approx0Len) return true;

            double rad0 = Math.Abs(Angle2D(pos0, pos1, pos));
            double rad1 = Math.Abs(Angle2D(pos1, pos0, pos));
            return rad0 < Approx0Ang && rad1 < Approx0Ang;
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
                XYZ interPos = IntersecVector2D(curve1, curve2);
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
                if (flagInter && interPos != null) ret.Add(interPos);

                var interPosAryTmp = new List<XYZ>();
                IList<XYZ> tmp = interPosAryTmp;
                IntersecCurve2D(curve1, curve2, ref tmp);
                for (int i = 0; i < tmp.Count; ++i)
                {
                    var interPosTmp = new XYZ(tmp[i].X, tmp[i].Y, curve1.GetEndPoint(0).Z);
                    bool flag = true;
                    if (flagInter && interPos != null && Distance2D(interPos, interPosTmp) < ToleranceInter)
                        flag = false;
                    if (flag) ret.Add(interPosTmp);
                }
            }
            return ret;
        }

        public void IntersecCurve(Curve curve1, Curve curve2, ref IList<XYZ> interPosAry)
        {
            var pts = GetInterPosCurves2D(curve1, curve2);
            interPosAry = pts != null ? new List<XYZ>(pts) : new List<XYZ>();
        }

        public bool IsSameDirection(XYZ p, XYZ q)
        {
            double d = p.DotProduct(q);
            var isParallel = p.CrossProduct(q).IsZeroLength();
            return d > 0 && isParallel;
        }

        public XYZ GetPointDistance(XYZ p, XYZ q, double t)
        {
            double d = Distance2D(p, q);
            if (d < Approx0Len) return new XYZ(q.X, q.Y, 0);
            double delta = t / d;
            double dx = q.X - delta * (q.X - p.X);
            double dy = q.Y - delta * (q.Y - p.Y);
            return new XYZ(dx, dy, 0);
        }

        public Curve FindExtendLine(Curve curve, double t)
        {
            t = t + 0.01;

            XYZ pos1 = null;
            XYZ pos2 = null;

            XYZ p = curve.GetEndPoint(0);
            XYZ q = curve.GetEndPoint(1);
            double dlen = Distance(p, q);
            if (dlen < Approx0Len) return curve;
            double delta = t / dlen;
            double dx = q.X + delta * (q.X - p.X);
            double dy = q.Y + delta * (q.Y - p.Y);
            double dz = q.Z + delta * (q.Z - p.Z);

            pos1 = new XYZ(dx, dy, dz);

            p = curve.GetEndPoint(1);
            q = curve.GetEndPoint(0);
            dlen = Distance(p, q);
            delta = dlen > Approx0Len ? t / dlen : 0;
            dx = q.X + delta * (q.X - p.X);
            dy = q.Y + delta * (q.Y - p.Y);
            dz = q.Z + delta * (q.Z - p.Z);

            pos2 = new XYZ(dx, dy, dz);

            return Line.CreateBound(pos1, pos2);
        }
    }
}
