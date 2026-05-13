using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public class Geometry
    {
        protected readonly UIDocument _rvtUIDoc;

        private const double Approx0LenDefault = 1e-6;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public double Approx0Len => Approx0LenDefault;

        public double UnitCoe
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public double UnitCoeM2
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public double UnitCoeM3
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public Transform GetElemTransform(FamilyInstance familyInstance)
        {
            try { return familyInstance.GetTotalTransform(); }
            catch { return Transform.Identity; }
        }

        public double Distance2D(XYZ p1, XYZ p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public double Distance(XYZ a, XYZ b)
        {
            return a.DistanceTo(b);
        }

        public void IntersecCurve2D(Curve c1, Curve c2, ref XYZ interPos)
        {
            var p1 = c1.GetEndPoint(0);
            var p2 = c1.GetEndPoint(1);
            var p3 = c2.GetEndPoint(0);
            var p4 = c2.GetEndPoint(1);
            double x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y, x4 = p4.X, y4 = p4.Y;
            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < Approx0Len) return;
            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double ix = x1 + t * (x2 - x1);
            double iy = y1 + t * (y2 - y1);
            double z = p1.Z;
            interPos = new XYZ(ix, iy, z);
        }

        public IList<Curve> GetCurveElem(PropertyLine propLine)
        {
            var list = new List<Curve>();
            if (propLine?.Location is LocationCurve lc && lc.Curve != null)
                list.Add(lc.Curve);
            return list;
        }

        public void GetCurvesBound(IList<Curve> curves, ref BoundingBoxXYZ bbox)
        {
            if (curves == null || curves.Count == 0)
            {
                bbox = null;
                return;
            }

            double minx = double.MaxValue, miny = double.MaxValue, minz = double.MaxValue;
            double maxx = double.MinValue, maxy = double.MinValue, maxz = double.MinValue;

            foreach (var c in curves)
            {
                for (int i = 0; i <= 1; i++)
                {
                    var p = c.GetEndPoint(i);
                    minx = Math.Min(minx, p.X); miny = Math.Min(miny, p.Y); minz = Math.Min(minz, p.Z);
                    maxx = Math.Max(maxx, p.X); maxy = Math.Max(maxy, p.Y); maxz = Math.Max(maxz, p.Z);
                }
            }

            bbox = new BoundingBoxXYZ
            {
                Min = new XYZ(minx, miny, minz),
                Max = new XYZ(maxx, maxy, maxz)
            };
        }
    }
}
