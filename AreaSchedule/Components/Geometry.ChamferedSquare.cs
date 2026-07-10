using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public sealed class ChamferedSquareDecomposition
    {
        public double FlatSideInternal { get; set; }
        public double ChamferLegInternal { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double BboxSideInternal => MaxX - MinX;

        public double AreaInternal =>
            BboxSideInternal * BboxSideInternal -
            4 * (0.5 * ChamferLegInternal * ChamferLegInternal);
    }

    public partial class Geometry
    {
        private const int ChamferedSquareFigureType = 50;
        private const double Sqrt2 = 1.4142135623730951;

        private static readonly BoundaryEdgeOrientation[] ChamferPattern =
        {
            BoundaryEdgeOrientation.Horizontal,
            BoundaryEdgeOrientation.Diagonal45,
            BoundaryEdgeOrientation.Vertical,
            BoundaryEdgeOrientation.Diagonal45,
            BoundaryEdgeOrientation.Horizontal,
            BoundaryEdgeOrientation.Diagonal45,
            BoundaryEdgeOrientation.Vertical,
            BoundaryEdgeOrientation.Diagonal45,
        };

        public int ChamferedSquareTypeId => ChamferedSquareFigureType;

        public bool TryDecomposeChamferedSquare(
            IList<Curve> figure,
            out ChamferedSquareDecomposition decomposition,
            out List<Line> auxiliaryLines)
        {
            decomposition = null;
            auxiliaryLines = new List<Line>();

            if (!TryAnalyzeChamferedSquare(figure, out decomposition))
                return false;

            double z = figure[0].GetEndPoint(0).Z;
            double centerX = (decomposition.MinX + decomposition.MaxX) * 0.5;
            double centerY = (decomposition.MinY + decomposition.MaxY) * 0.5;

            Line horizontal = SafeCreateBound(
                new XYZ(decomposition.MinX, centerY, z),
                new XYZ(decomposition.MaxX, centerY, z));
            Line vertical = SafeCreateBound(
                new XYZ(centerX, decomposition.MinY, z),
                new XYZ(centerX, decomposition.MaxY, z));
            if (horizontal != null)
                auxiliaryLines.Add(horizontal);
            if (vertical != null)
                auxiliaryLines.Add(vertical);

            XYZ topFlatLeft = new XYZ(decomposition.MinX + decomposition.ChamferLegInternal, decomposition.MaxY, z);
            XYZ cornerInner = new XYZ(
                decomposition.MinX + decomposition.ChamferLegInternal,
                decomposition.MaxY - decomposition.ChamferLegInternal,
                z);
            XYZ leftVertTop = new XYZ(decomposition.MinX, decomposition.MaxY - decomposition.ChamferLegInternal, z);

            Line chamferLegVertical = SafeCreateBound(topFlatLeft, cornerInner);
            Line chamferLegHorizontal = SafeCreateBound(cornerInner, leftVertTop);
            if (chamferLegVertical != null)
                auxiliaryLines.Add(chamferLegVertical);
            if (chamferLegHorizontal != null)
                auxiliaryLines.Add(chamferLegHorizontal);

            return true;
        }

        public bool IsChamferedSquarePolygon(IList<Curve> figure)
        {
            return TryAnalyzeChamferedSquare(figure, out _);
        }

        private bool TryAnalyzeChamferedSquare(
            IList<Curve> figure,
            out ChamferedSquareDecomposition decomposition)
        {
            decomposition = null;
            if (figure == null || figure.Count != 8)
                return false;

            var orientations = new List<BoundaryEdgeOrientation>(8);
            var lengths = new List<double>(8);

            foreach (Curve curve in figure)
            {
                if (curve is not Line line)
                    return false;

                BoundaryEdgeOrientation orientation = ClassifyBoundaryEdge(line);
                if (orientation == BoundaryEdgeOrientation.Other)
                    return false;

                orientations.Add(orientation);
                lengths.Add(line.Length);
            }

            if (!HasAlternatingChamferPattern(orientations))
                return false;

            var horizontalLengths = new List<double>();
            var verticalLengths = new List<double>();
            var diagonalLengths = new List<double>();

            for (int i = 0; i < orientations.Count; i++)
            {
                switch (orientations[i])
                {
                    case BoundaryEdgeOrientation.Horizontal:
                        horizontalLengths.Add(lengths[i]);
                        break;
                    case BoundaryEdgeOrientation.Vertical:
                        verticalLengths.Add(lengths[i]);
                        break;
                    case BoundaryEdgeOrientation.Diagonal45:
                        diagonalLengths.Add(lengths[i]);
                        break;
                }
            }

            if (horizontalLengths.Count != 2 ||
                verticalLengths.Count != 2 ||
                diagonalLengths.Count != 4)
            {
                return false;
            }

            if (!LengthsNearlyEqual(horizontalLengths[0], horizontalLengths[1]) ||
                !LengthsNearlyEqual(verticalLengths[0], verticalLengths[1]) ||
                !diagonalLengths.All(length => LengthsNearlyEqual(length, diagonalLengths[0])))
            {
                return false;
            }

            double flatSide = (horizontalLengths[0] + verticalLengths[0]) * 0.5;
            double diagonal = diagonalLengths[0];
            double chamferLeg = diagonal / Sqrt2;

            double minX = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double minY = double.PositiveInfinity;
            double maxY = double.NegativeInfinity;

            foreach (Curve curve in figure)
            {
                XYZ start = curve.GetEndPoint(0);
                XYZ end = curve.GetEndPoint(1);
                minX = Math.Min(minX, Math.Min(start.X, end.X));
                maxX = Math.Max(maxX, Math.Max(start.X, end.X));
                minY = Math.Min(minY, Math.Min(start.Y, end.Y));
                maxY = Math.Max(maxY, Math.Max(start.Y, end.Y));
            }

            double bboxWidth = maxX - minX;
            double bboxHeight = maxY - minY;
            if (!LengthsNearlyEqual(bboxWidth, flatSide + 2.0 * chamferLeg) ||
                !LengthsNearlyEqual(bboxHeight, flatSide + 2.0 * chamferLeg))
            {
                return false;
            }

            double expectedArea = bboxWidth * bboxWidth - 2.0 * chamferLeg * chamferLeg;
            double actualArea = Math.Abs(GetPolygonArea(figure));
            if (!LengthsNearlyEqual(expectedArea, actualArea))
                return false;

            decomposition = new ChamferedSquareDecomposition
            {
                FlatSideInternal = flatSide,
                ChamferLegInternal = chamferLeg,
                MinX = minX,
                MaxX = maxX,
                MinY = minY,
                MaxY = maxY
            };
            return true;
        }

        private enum BoundaryEdgeOrientation
        {
            Other,
            Horizontal,
            Vertical,
            Diagonal45
        }

        private BoundaryEdgeOrientation ClassifyBoundaryEdge(Line line)
        {
            XYZ start = line.GetEndPoint(0);
            XYZ end = line.GetEndPoint(1);
            double dx = Math.Abs(end.X - start.X);
            double dy = Math.Abs(end.Y - start.Y);
            if (dy < Approx0Len)
                return BoundaryEdgeOrientation.Horizontal;
            if (dx < Approx0Len)
                return BoundaryEdgeOrientation.Vertical;

            double longer = Math.Max(dx, dy);
            if (longer < Approx0Len)
                return BoundaryEdgeOrientation.Other;

            if (Math.Abs(dx - dy) / longer <= 0.02)
                return BoundaryEdgeOrientation.Diagonal45;

            return BoundaryEdgeOrientation.Other;
        }

        private bool HasAlternatingChamferPattern(IReadOnlyList<BoundaryEdgeOrientation> orientations)
        {
            if (orientations.Count != 8)
                return false;

            for (int offset = 0; offset < 8; offset++)
            {
                if (MatchesPattern(orientations, ChamferPattern, offset))
                    return true;
            }

            var reversed = orientations.Reverse().ToList();
            for (int offset = 0; offset < 8; offset++)
            {
                if (MatchesPattern(reversed, ChamferPattern, offset))
                    return true;
            }

            return false;
        }

        private static bool MatchesPattern(
            IReadOnlyList<BoundaryEdgeOrientation> orientations,
            IReadOnlyList<BoundaryEdgeOrientation> pattern,
            int offset)
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                if (orientations[i] != pattern[(i + offset) % pattern.Count])
                    return false;
            }

            return true;
        }

        private bool LengthsNearlyEqual(double a, double b, double relativeTolerance = 0.01)
        {
            double scale = Math.Max(Math.Max(Math.Abs(a), Math.Abs(b)), 1.0);
            return Math.Abs(a - b) <= Math.Max(Approx0Len, scale * relativeTolerance);
        }
    }
}
