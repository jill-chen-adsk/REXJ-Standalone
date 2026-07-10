using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public sealed class OrthogonalRectanglePiece
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public double WidthInternal => MaxX - MinX;
        public double HeightInternal => MaxY - MinY;
        public double AreaInternal => WidthInternal * HeightInternal;
    }

    public partial class Geometry
    {
        private const int OrthogonalPolygonFigureType = 40;

        public int OrthogonalPolygonTypeId => OrthogonalPolygonFigureType;

        public bool TryDecomposeOrthogonalPolygon(
            IList<Curve> figure,
            out List<OrthogonalRectanglePiece> pieces,
            out List<Line> auxiliaryLines)
        {
            pieces = new List<OrthogonalRectanglePiece>();
            auxiliaryLines = new List<Line>();

            if (!IsOrthogonalPolygon(figure))
                return false;

            if (!TryBuildGridPieces(figure, out pieces))
                return false;

            pieces = MergeGridPieces(pieces);
            auxiliaryLines = BuildAuxiliaryLines(pieces, figure);
            return pieces.Count > 0;
        }

        public bool IsOrthogonalPolygon(IList<Curve> figure)
        {
            if (figure == null || figure.Count < 6 || figure.Count % 2 != 0)
                return false;

            foreach (Curve curve in figure)
            {
                if (curve is not Line line)
                    return false;

                XYZ start = line.GetEndPoint(0);
                XYZ end = line.GetEndPoint(1);
                double dx = Math.Abs(end.X - start.X);
                double dy = Math.Abs(end.Y - start.Y);
                if (dx < Approx0Len && dy < Approx0Len)
                    return false;
                if (dx >= Approx0Len && dy >= Approx0Len)
                    return false;
            }

            return true;
        }

        private bool TryBuildGridPieces(IList<Curve> figure, out List<OrthogonalRectanglePiece> pieces)
        {
            pieces = new List<OrthogonalRectanglePiece>();
            List<double> xs = CollectAxisCoordinates(figure, true);
            List<double> ys = CollectAxisCoordinates(figure, false);
            if (xs.Count < 2 || ys.Count < 2)
                return false;

            for (int i = 0; i < xs.Count - 1; i++)
            {
                for (int j = 0; j < ys.Count - 1; j++)
                {
                    double centerX = (xs[i] + xs[i + 1]) * 0.5;
                    double centerY = (ys[j] + ys[j + 1]) * 0.5;
                    double z = figure[0].GetEndPoint(0).Z;
                    if (!IsPointInsideOrthogonalPolygon(new XYZ(centerX, centerY, z), figure))
                        continue;

                    pieces.Add(new OrthogonalRectanglePiece
                    {
                        MinX = xs[i],
                        MinY = ys[j],
                        MaxX = xs[i + 1],
                        MaxY = ys[j + 1]
                    });
                }
            }

            return pieces.Count > 0;
        }

        private List<double> CollectAxisCoordinates(IList<Curve> figure, bool collectX)
        {
            var values = new List<double>();
            foreach (Curve curve in figure)
            {
                values.Add(collectX ? curve.GetEndPoint(0).X : curve.GetEndPoint(0).Y);
                values.Add(collectX ? curve.GetEndPoint(1).X : curve.GetEndPoint(1).Y);
            }

            values.Sort();
            var merged = new List<double>();
            foreach (double value in values)
            {
                if (merged.Count == 0 || Math.Abs(merged[merged.Count - 1] - value) > Approx0Len)
                    merged.Add(value);
            }

            return merged;
        }

        private bool IsPointInsideOrthogonalPolygon(XYZ point, IList<Curve> figure)
        {
            bool inside = false;
            for (int i = 0; i < figure.Count; i++)
            {
                XYZ a = figure[i].GetEndPoint(0);
                XYZ b = figure[i].GetEndPoint(1);
                bool intersects = ((a.Y > point.Y) != (b.Y > point.Y)) &&
                    (point.X < (b.X - a.X) * (point.Y - a.Y) / ((b.Y - a.Y) + 1e-12) + a.X);
                if (intersects)
                    inside = !inside;
            }

            return inside;
        }

        private static List<OrthogonalRectanglePiece> MergeGridPieces(List<OrthogonalRectanglePiece> pieces)
        {
            List<OrthogonalRectanglePiece> merged = pieces
                .Select(p => new OrthogonalRectanglePiece
                {
                    MinX = p.MinX,
                    MinY = p.MinY,
                    MaxX = p.MaxX,
                    MaxY = p.MaxY
                })
                .ToList();

            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < merged.Count; i++)
                {
                    for (int j = i + 1; j < merged.Count; j++)
                    {
                        if (TryMergePieces(merged[i], merged[j], out OrthogonalRectanglePiece combined))
                        {
                            merged[i] = combined;
                            merged.RemoveAt(j);
                            changed = true;
                            break;
                        }
                    }

                    if (changed)
                        break;
                }
            }
            while (changed);

            return merged;
        }

        private static bool TryMergePieces(
            OrthogonalRectanglePiece left,
            OrthogonalRectanglePiece right,
            out OrthogonalRectanglePiece merged)
        {
            merged = null;
            if (Math.Abs(left.MinY - right.MinY) < 1e-6 &&
                Math.Abs(left.MaxY - right.MaxY) < 1e-6 &&
                (Math.Abs(left.MaxX - right.MinX) < 1e-6 || Math.Abs(right.MaxX - left.MinX) < 1e-6))
            {
                merged = new OrthogonalRectanglePiece
                {
                    MinX = Math.Min(left.MinX, right.MinX),
                    MinY = left.MinY,
                    MaxX = Math.Max(left.MaxX, right.MaxX),
                    MaxY = left.MaxY
                };
                return true;
            }

            if (Math.Abs(left.MinX - right.MinX) < 1e-6 &&
                Math.Abs(left.MaxX - right.MaxX) < 1e-6 &&
                (Math.Abs(left.MaxY - right.MinY) < 1e-6 || Math.Abs(right.MaxY - left.MinY) < 1e-6))
            {
                merged = new OrthogonalRectanglePiece
                {
                    MinX = left.MinX,
                    MinY = Math.Min(left.MinY, right.MinY),
                    MaxX = left.MaxX,
                    MaxY = Math.Max(left.MaxY, right.MaxY)
                };
                return true;
            }

            return false;
        }

        private List<Line> BuildAuxiliaryLines(List<OrthogonalRectanglePiece> pieces, IList<Curve> figure)
        {
            var lines = new List<Line>();
            if (pieces.Count <= 1)
                return lines;

            double z = figure[0].GetEndPoint(0).Z;
            var xSplits = pieces.SelectMany(p => new[] { p.MinX, p.MaxX }).Distinct().OrderBy(v => v).ToList();
            var ySplits = pieces.SelectMany(p => new[] { p.MinY, p.MaxY }).Distinct().OrderBy(v => v).ToList();

            double minX = xSplits.First();
            double maxX = xSplits.Last();
            double minY = ySplits.First();
            double maxY = ySplits.Last();

            for (int i = 1; i < xSplits.Count - 1; i++)
            {
                double x = xSplits[i];
                Line candidate = SafeCreateBound(new XYZ(x, minY, z), new XYZ(x, maxY, z));
                if (candidate != null && !IsSegmentOnOuterBoundary(candidate, figure))
                    lines.Add(candidate);
            }

            for (int j = 1; j < ySplits.Count - 1; j++)
            {
                double y = ySplits[j];
                Line candidate = SafeCreateBound(new XYZ(minX, y, z), new XYZ(maxX, y, z));
                if (candidate != null && !IsSegmentOnOuterBoundary(candidate, figure))
                    lines.Add(candidate);
            }

            return lines;
        }

        private bool IsSegmentOnOuterBoundary(Line segment, IList<Curve> figure)
        {
            foreach (Curve curve in figure)
            {
                if (curve is not Line outer)
                    continue;

                if (AreCollinearOverlapping(segment, outer))
                    return true;
            }

            return false;
        }

        public bool AreCollinearOverlapping(Line a, Line b)
        {
            if (!CompareParallelism(a, b))
                return false;

            XYZ a0 = a.GetEndPoint(0);
            XYZ a1 = a.GetEndPoint(1);
            XYZ b0 = b.GetEndPoint(0);
            XYZ b1 = b.GetEndPoint(1);

            bool vertical = Math.Abs(a0.X - a1.X) < Approx0Len;
            if (vertical)
            {
                if (Math.Abs(a0.X - b0.X) > Approx0Len)
                    return false;

                double aMin = Math.Min(a0.Y, a1.Y);
                double aMax = Math.Max(a0.Y, a1.Y);
                double bMin = Math.Min(b0.Y, b1.Y);
                double bMax = Math.Max(b0.Y, b1.Y);
                return aMax > bMin + Approx0Len && bMax > aMin + Approx0Len;
            }

            if (Math.Abs(a0.Y - b0.Y) > Approx0Len)
                return false;

            double axMin = Math.Min(a0.X, a1.X);
            double axMax = Math.Max(a0.X, a1.X);
            double bxMin = Math.Min(b0.X, b1.X);
            double bxMax = Math.Max(b0.X, b1.X);
            return axMax > bxMin + Approx0Len && bxMax > axMin + Approx0Len;
        }

        private static Line SafeCreateBound(XYZ p0, XYZ p1)
        {
            if (p0 == null || p1 == null)
                return null;
            if (p0.DistanceTo(p1) < 1.0e-6)
                return null;

            try
            {
                return Line.CreateBound(p0, p1);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException)
            {
                return null;
            }
        }
    }
}
