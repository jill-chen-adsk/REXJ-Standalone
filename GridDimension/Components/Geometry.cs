using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.GridDimension.Components
{
    /// <summary>図形</summary>
    public class Geometry
    {
        public const int DistanceStorageVersion = 1;
        public const int DistanceStorageVersionIndex = 8;

        private readonly UIDocument _rvtUIDoc;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }

        public Document RvtDBDoc { get; }

        public double Approx0Len => 1e-6;

        public bool IsImperial
        {
            get
            {
                try
                {
                    return RvtDBDoc.DisplayUnitSystem == DisplayUnit.IMPERIAL;
                }
                catch
                {
                    return false;
                }
            }
        }

        public ForgeTypeId LengthUnitTypeId =>
            RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();

        public string LengthUnitLabel => GetUnitLabel(LengthUnitTypeId);

        /// <summary>Convert a legacy millimeter value to project length display units.</summary>
        public double FromMillimeters(double millimeters) =>
            UnitUtils.ConvertFromInternalUnits(
                UnitUtils.ConvertToInternalUnits(millimeters, UnitTypeId.Millimeters),
                LengthUnitTypeId);

        public string FormatDisplayLength(double displayLength) =>
            displayLength.ToString("0.####", CultureInfo.CurrentCulture);

        public bool TryParseDisplayLength(string text, out double internalFeet)
        {
            internalFeet = 0.0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string trimmed = text.Trim();
            Units units = RvtDBDoc.GetUnits();

            if (UnitFormatUtils.TryParse(units, SpecTypeId.Length, trimmed, out internalFeet))
                return true;

            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out double displayValue)
                || double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out displayValue))
            {
                internalFeet = UnitUtils.ConvertToInternalUnits(displayValue, LengthUnitTypeId);
                return true;
            }

            return false;
        }

        public bool UsesLegacyMillimeterStorage(IList<string> data)
        {
            if (data == null || data.Count <= DistanceStorageVersionIndex)
                return true;

            return data[DistanceStorageVersionIndex] != DistanceStorageVersion.ToString(CultureInfo.InvariantCulture);
        }

        public string ConvertStoredDistanceForDisplay(string storedValue, bool useLegacyMillimeterStorage)
        {
            if (string.IsNullOrWhiteSpace(storedValue))
                return storedValue;

            if (!useLegacyMillimeterStorage || !IsImperial)
                return storedValue;

            if (!double.TryParse(storedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double millimeters)
                && !double.TryParse(storedValue, NumberStyles.Float, CultureInfo.CurrentCulture, out millimeters))
            {
                return storedValue;
            }

            return FormatDisplayLength(FromMillimeters(millimeters));
        }

        public static string GetUnitLabel(ForgeTypeId unitTypeId)
        {
            if (unitTypeId == UnitTypeId.Millimeters)
                return "mm";
            if (unitTypeId == UnitTypeId.Centimeters)
                return "cm";
            if (unitTypeId == UnitTypeId.Meters)
                return "m";
            if (unitTypeId == UnitTypeId.Feet)
                return "ft";
            if (unitTypeId == UnitTypeId.FeetFractionalInches)
                return "ft-in";
            if (unitTypeId == UnitTypeId.Inches)
                return "in";
            if (unitTypeId == UnitTypeId.FractionalInches)
                return "in";

            return unitTypeId.TypeId;
        }

        public double Distance(XYZ pos1, XYZ pos2)
        {
            double dx = pos1.X - pos2.X;
            double dy = pos1.Y - pos2.Y;
            double dz = pos1.Z - pos2.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double Distance2D(XYZ pos1, XYZ pos2)
        {
            double dx = pos1.X - pos2.X;
            double dy = pos1.Y - pos2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public XYZ UnitVector(XYZ from, XYZ to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (len < Approx0Len)
                return new XYZ(0, 0, 0);
            return new XYZ(dx / len, dy / len, dz / len);
        }

        /// <summary>曲線上の点に最も近い垂線の足（無限直線上の射影）</summary>
        public XYZ GetVerticalPos3D(Curve line, XYZ pos)
        {
            if (line == null || pos == null)
                return null;

            Line ln = line as Line;
            if (ln == null)
                return null;

            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            XYZ lineVec = p1 - p0;
            double lenSq = lineVec.DotProduct(lineVec);
            if (lenSq < Approx0Len * Approx0Len)
                return null;

            double t = (pos - p0).DotProduct(lineVec) / lenSq;
            XYZ vertPos = p0 + lineVec * t;

            if (Distance(pos, vertPos) < Approx0Len)
                return null;

            return vertPos;
        }

        public XYZ GetPointDistance(XYZ p, XYZ q, double t)
        {
            double delta = t / Distance(p, q);
            double dx = q.X - delta * (q.X - p.X);
            double dy = q.Y - delta * (q.Y - p.Y);
            double dz = q.Z - delta * (q.Z - p.Z);
            return new XYZ(dx, dy, dz);
        }

        public bool IsHasGridArc(Document doc, IList<Element> lstSelSetGrids)
        {
            foreach (var ele in lstSelSetGrids)
            {
                if (ele is Grid grid)
                {
                    if (grid == null)
                        continue;
                    if (grid.Curve is Arc)
                        return true;
                }
                if (ele is MultiSegmentGrid multiSegment)
                {
                    if (multiSegment == null)
                        continue;
                    var listGrid = multiSegment.GetGridIds();
                    foreach (var elId in listGrid)
                    {
                        Grid g = doc.GetElement(elId) as Grid;
                        if (g == null)
                            continue;
                        if (g.Curve is Arc)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool GetDirectionOfGrids(View checkedView, IList<Element> elemGrids, out int optDirec)
        {
            optDirec = 0;
            if (checkedView == null)
                return true;

            Grid grid = null;
            foreach (var gr in elemGrids)
            {
                if (gr is Grid g)
                {
                    grid = g;
                    break;
                }
            }
            if (grid != null)
            {
                Line line = grid.Curve as Line;
                if (line == null)
                    return true;
                XYZ dicrection = line.Direction.Normalize();
                XYZ rightDirection = checkedView.RightDirection;
                XYZ upDirection = checkedView.UpDirection;
                if (dicrection.CrossProduct(rightDirection).IsZeroLength())
                {
                    optDirec = 0;
                    return true;
                }
                if (dicrection.CrossProduct(upDirection).IsZeroLength())
                {
                    optDirec = 1;
                    return false;
                }
                optDirec = 2;
                return false;
            }
            return false;
        }

        public bool IsHasDirectionXAndY(IList<Element> elemGrids)
        {
            Grid grid = null;
            XYZ dicFisrt = null;
            foreach (var gr in elemGrids)
            {
                if (gr == null)
                    continue;
                if (gr is Grid g)
                {
                    grid = g;
                    break;
                }
            }
            if (grid != null && grid.Curve != null)
                dicFisrt = (grid.Curve.GetEndPoint(1) - grid.Curve.GetEndPoint(0)).Normalize();

            foreach (var gr in elemGrids)
            {
                if (gr == null)
                    continue;
                if (gr is Grid gri)
                {
                    if (gri != null && gri != grid)
                    {
                        Line line = gri.Curve as Line;
                        if (line == null)
                            continue;
                        XYZ dicrection = line.Direction.Normalize();
                        if (!dicrection.CrossProduct(dicFisrt).IsZeroLength())
                            return true;
                    }
                }
            }
            return false;
        }

        public XYZ GetPointOnVector(XYZ pointInsert, XYZ vectorDir, double dDistance)
        {
            return pointInsert + vectorDir.Normalize() * dDistance;
        }

        public void IntersecCurve2D(Curve curve1, Curve curve2, ref IList<XYZ> interPosAry)
        {
            if (interPosAry == null)
                interPosAry = new List<XYZ>();
            XYZ p1 = curve1.GetEndPoint(0);
            XYZ p2 = curve1.GetEndPoint(1);
            XYZ p3 = curve2.GetEndPoint(0);
            XYZ p4 = curve2.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y;
            double x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y;
            double x4 = p4.X, y4 = p4.Y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < 1e-12)
                return;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denom;

            if (t >= -Approx0Len && t <= 1.0 + Approx0Len && u >= -Approx0Len && u <= 1.0 + Approx0Len)
            {
                double ix = x1 + t * (x2 - x1);
                double iy = y1 + t * (y2 - y1);
                interPosAry.Add(new XYZ(ix, iy, 0));
            }
        }
    }
}
