using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SectionListSteel.JExtComCompat
{
    public class RvtGeometry
    {
        public RvtGeometry(UIDocument uidoc)
        {
            UiDocument = uidoc;
            RvtDBDoc = uidoc.Document;
        }

        public UIDocument UiDocument { get; }
        public Document RvtDBDoc { get; }
        public double UnitCoe { get; } = 304.8;

        /// <summary>Curve length の「ゼロ扱い」しきい値（フィート）</summary>
        public double Approx0Len { get; } = 1e-6;

        public XYZ Center2Point(XYZ p1, XYZ p2)
        {
            return new XYZ((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0, (p1.Z + p2.Z) / 2.0);
        }

        public double Distance2D(XYZ p1, XYZ p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>2D vectors (p1-p0) · (p2-p0)，Z を無視</summary>
        public double DotProduct2D(XYZ p0, XYZ p1, XYZ p2)
        {
            double vx = p1.X - p0.X;
            double vy = p1.Y - p0.Y;
            double wx = p2.X - p0.X;
            double wy = p2.Y - p0.Y;
            return vx * wx + vy * wy;
        }

        /// <summary>2D 外積の Z 成分 (p1-p0)×(p2-p0)，Z を無視</summary>
        public double CrossProduct2D(XYZ p0, XYZ p1, XYZ p2)
        {
            double vx = p1.X - p0.X;
            double vy = p1.Y - p0.Y;
            double wx = p2.X - p0.X;
            double wy = p2.Y - p0.Y;
            return vx * wy - vy * wx;
        }

        /// <summary>XY 平面の三角形重心（Z は p0 の値）</summary>
        public XYZ TriangleGravity2D(XYZ p0, XYZ p1, XYZ p2)
        {
            return new XYZ((p0.X + p1.X + p2.X) / 3.0, (p0.Y + p1.Y + p2.Y) / 3.0, p0.Z);
        }

        public XYZ UnitVector(XYZ from, XYZ to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (len < 1e-12) return XYZ.Zero;
            return new XYZ(dx / len, dy / len, dz / len);
        }
    }
}
