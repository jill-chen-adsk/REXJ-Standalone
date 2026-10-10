using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ADSK.JExtRAC.GridDimension.UI
{
    /// <summary>
    /// Builds the A/B explanatory diagram as vector content. The original artwork was a
    /// 271x305 bitmap whose labels were roughly six pixels tall, so it could never render
    /// sharply. Drawing it instead keeps it crisp at any size or DPI and lets it take its
    /// colours from the active Weave palette.
    /// </summary>
    internal static class GridDiagram
    {
        public const double DesignWidth = 300.0;
        public const double DesignHeight = 320.0;

        private const double CenterX = DesignWidth / 2.0;
        private const double CenterY = 155.0;

        private const double GridSpacing = 30.0;
        private const double BubbleRadius = 14.0;
        private const double BubbleInset = 25.0;

        private const double CoreHalfSpan = GridSpacing + 10.0;
        private const double TickLength = 7.0;

        // Distances from the grid centre out to the two dimension lines. The nearer line
        // carries the individual 2000 spans; the farther one carries the overall 4000.
        private const double NearDimOffset = 66.0;
        private const double FarDimOffset = 92.0;

        private const double TextGap = 8.0;

        private const double BubbleFontSize = 11.0;
        private const double DimensionFontSize = 8.5;
        private const double AccentFontSize = 9.0;

        private static readonly string[] ColumnLabels = { "X1", "X2", "X3" };
        private static readonly string[] RowLabels = { "Y3", "Y2", "Y1" };

        /// <summary>
        /// Creates the linear-grid diagram. <paramref name="neutral"/> draws the grid and
        /// dimensions, <paramref name="accent"/> draws the A/B call-outs.
        /// </summary>
        public static Canvas CreateLinear(Brush neutral, Brush accent)
        {
            var canvas = new Canvas
            {
                Width = DesignWidth,
                Height = DesignHeight,
                Background = Brushes.Transparent,
                SnapsToDevicePixels = false,
            };

            double[] columns = { CenterX - GridSpacing, CenterX, CenterX + GridSpacing };
            double[] rows = { CenterY - GridSpacing, CenterY, CenterY + GridSpacing };

            double gridTop = BubbleInset + BubbleRadius;
            double gridBottom = DesignHeight - BubbleInset - BubbleRadius;
            double gridLeft = BubbleInset + BubbleRadius;
            double gridRight = DesignWidth - BubbleInset - BubbleRadius;

            DrawGridLines(canvas, neutral, columns, rows, gridTop, gridBottom, gridLeft, gridRight);
            DrawBubbles(canvas, neutral, columns, rows);
            DrawDimensionBands(canvas, neutral, columns, rows);
            DrawAccentCallouts(canvas, accent, gridTop, gridBottom, gridLeft, gridRight);

            return canvas;
        }

        private static void DrawGridLines(
            Canvas canvas,
            Brush neutral,
            double[] columns,
            double[] rows,
            double gridTop,
            double gridBottom,
            double gridLeft,
            double gridRight)
        {
            double coreTop = CenterY - CoreHalfSpan;
            double coreBottom = CenterY + CoreHalfSpan;
            double coreLeft = CenterX - CoreHalfSpan;
            double coreRight = CenterX + CoreHalfSpan;

            foreach (double x in columns)
            {
                AddLine(canvas, neutral, x, gridTop, x, coreTop);
                AddLine(canvas, neutral, x, coreTop, x, coreBottom, dashed: true);
                AddLine(canvas, neutral, x, coreBottom, x, gridBottom);
            }

            foreach (double y in rows)
            {
                AddLine(canvas, neutral, gridLeft, y, coreLeft, y);
                AddLine(canvas, neutral, coreLeft, y, coreRight, y, dashed: true);
                AddLine(canvas, neutral, coreRight, y, gridRight, y);
            }
        }

        private static void DrawBubbles(Canvas canvas, Brush neutral, double[] columns, double[] rows)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                AddBubble(canvas, neutral, columns[i], BubbleInset, ColumnLabels[i]);
                AddBubble(canvas, neutral, columns[i], DesignHeight - BubbleInset, ColumnLabels[i]);
            }

            for (int i = 0; i < rows.Length; i++)
            {
                AddBubble(canvas, neutral, BubbleInset, rows[i], RowLabels[i]);
                AddBubble(canvas, neutral, DesignWidth - BubbleInset, rows[i], RowLabels[i]);
            }
        }

        private static void DrawDimensionBands(Canvas canvas, Brush neutral, double[] columns, double[] rows)
        {
            double left = columns[0];
            double middle = columns[1];
            double right = columns[2];
            double top = rows[0];
            double centre = rows[1];
            double bottom = rows[2];

            // Horizontal bands above and below the grid measure the X spacing.
            foreach (double sign in new[] { -1.0, 1.0 })
            {
                double nearY = CenterY + sign * NearDimOffset;
                double farY = CenterY + sign * FarDimOffset;

                AddHorizontalDimension(canvas, neutral, left, middle, nearY, "2000");
                AddHorizontalDimension(canvas, neutral, middle, right, nearY, "2000");
                AddHorizontalDimension(canvas, neutral, left, right, farY, "4000");
            }

            // Vertical bands left and right of the grid measure the Y spacing.
            foreach (double sign in new[] { -1.0, 1.0 })
            {
                double nearX = CenterX + sign * NearDimOffset;
                double farX = CenterX + sign * FarDimOffset;

                AddVerticalDimension(canvas, neutral, top, centre, nearX, "2000");
                AddVerticalDimension(canvas, neutral, centre, bottom, nearX, "2000");
                AddVerticalDimension(canvas, neutral, top, bottom, farX, "4000");
            }
        }

        private static void DrawAccentCallouts(
            Canvas canvas,
            Brush accent,
            double gridTop,
            double gridBottom,
            double gridLeft,
            double gridRight)
        {
            double verticalRailX = CenterX - GridSpacing - 18.0;
            double horizontalRailY = CenterY - GridSpacing - 18.0;

            // A spans the grid end to the overall dimension line, B carries on to the
            // individual-span line, mirroring how the original artwork annotated them.
            foreach (double sign in new[] { -1.0, 1.0 })
            {
                double anchor = sign < 0 ? gridTop : gridBottom;
                double far = CenterY + sign * FarDimOffset;
                double near = CenterY + sign * NearDimOffset;

                AddLine(canvas, accent, verticalRailX, anchor, verticalRailX, near, thickness: 1.2);
                foreach (double y in new[] { anchor, far, near })
                    AddLine(canvas, accent, verticalRailX - TickLength / 2.0, y, verticalRailX + TickLength / 2.0, y, thickness: 1.2);

                AddText(canvas, "A", verticalRailX - TextGap, (anchor + far) / 2.0, AccentFontSize, accent, bold: true, rotation: -90.0);
                AddText(canvas, "B", verticalRailX - TextGap, (far + near) / 2.0, AccentFontSize, accent, bold: true, rotation: -90.0);
            }

            foreach (double sign in new[] { -1.0, 1.0 })
            {
                double anchor = sign < 0 ? gridLeft : gridRight;
                double far = CenterX + sign * FarDimOffset;
                double near = CenterX + sign * NearDimOffset;

                AddLine(canvas, accent, anchor, horizontalRailY, near, horizontalRailY, thickness: 1.2);
                foreach (double x in new[] { anchor, far, near })
                    AddLine(canvas, accent, x, horizontalRailY - TickLength / 2.0, x, horizontalRailY + TickLength / 2.0, thickness: 1.2);

                AddText(canvas, "A", (anchor + far) / 2.0, horizontalRailY - TextGap, AccentFontSize, accent, bold: true);
                AddText(canvas, "B", (far + near) / 2.0, horizontalRailY - TextGap, AccentFontSize, accent, bold: true);
            }
        }

        private static void AddHorizontalDimension(
            Canvas canvas,
            Brush neutral,
            double startX,
            double endX,
            double y,
            string label)
        {
            AddLine(canvas, neutral, startX, y, endX, y);
            AddLine(canvas, neutral, startX, y - TickLength / 2.0, startX, y + TickLength / 2.0);
            AddLine(canvas, neutral, endX, y - TickLength / 2.0, endX, y + TickLength / 2.0);

            AddText(canvas, label, (startX + endX) / 2.0, y - TextGap, DimensionFontSize, neutral);
        }

        private static void AddVerticalDimension(
            Canvas canvas,
            Brush neutral,
            double startY,
            double endY,
            double x,
            string label)
        {
            AddLine(canvas, neutral, x, startY, x, endY);
            AddLine(canvas, neutral, x - TickLength / 2.0, startY, x + TickLength / 2.0, startY);
            AddLine(canvas, neutral, x - TickLength / 2.0, endY, x + TickLength / 2.0, endY);

            // Rotating the label -90 degrees puts "above the line" to the left of it.
            AddText(canvas, label, x - TextGap, (startY + endY) / 2.0, DimensionFontSize, neutral, rotation: -90.0);
        }

        private static void AddBubble(Canvas canvas, Brush neutral, double centreX, double centreY, string label)
        {
            var ellipse = new Ellipse
            {
                Width = BubbleRadius * 2.0,
                Height = BubbleRadius * 2.0,
                Stroke = neutral,
                StrokeThickness = 1.0,
                Fill = Brushes.Transparent,
            };

            Canvas.SetLeft(ellipse, centreX - BubbleRadius);
            Canvas.SetTop(ellipse, centreY - BubbleRadius);
            canvas.Children.Add(ellipse);

            AddText(canvas, label, centreX, centreY, BubbleFontSize, neutral);
        }

        private static void AddLine(
            Canvas canvas,
            Brush stroke,
            double x1,
            double y1,
            double x2,
            double y2,
            bool dashed = false,
            double thickness = 1.0)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = thickness,
            };

            if (dashed)
                line.StrokeDashArray = new DoubleCollection { 6.0, 3.0, 1.0, 3.0 };

            canvas.Children.Add(line);
        }

        private static void AddText(
            Canvas canvas,
            string text,
            double centreX,
            double centreY,
            double fontSize,
            Brush foreground,
            bool bold = false,
            double rotation = 0.0)
        {
            var block = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                Foreground = foreground,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                FontFamily = new FontFamily("Segoe UI"),
            };

            if (rotation != 0.0)
            {
                block.RenderTransformOrigin = new Point(0.5, 0.5);
                block.RenderTransform = new RotateTransform(rotation);
            }

            block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(block, centreX - block.DesiredSize.Width / 2.0);
            Canvas.SetTop(block, centreY - block.DesiredSize.Height / 2.0);
            canvas.Children.Add(block);
        }
    }
}
