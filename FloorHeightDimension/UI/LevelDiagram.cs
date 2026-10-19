using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ADSK.JExtRAC.FloorHeightDimension.UI
{
    /// <summary>
    /// Builds the A/B explanatory diagram as vector content, so it stays crisp at any DPI,
    /// follows the active Weave palette and shows localized labels instead of the Japanese
    /// text baked into the original bitmap.
    /// </summary>
    internal static class LevelDiagram
    {
        public const double DesignWidth = 300.0;
        public const double DesignHeight = 196.0;

        private const double CenterX = DesignWidth / 2.0;

        // Inner dimension lines carry the per-storey spans, outer ones the overall height.
        private const double OuterLeft = 28.0;
        private const double InnerLeft = 54.0;
        private const double InnerRight = DesignWidth - InnerLeft;
        private const double OuterRight = DesignWidth - OuterLeft;

        private static readonly double[] LevelYs = { 42.0, 88.0, 134.0 };
        private const double RailY = 162.0;

        private const double TickLength = 7.0;
        private const double TextGap = 8.0;

        private const double LevelFontSize = 9.0;
        private const double DimensionFontSize = 8.5;
        private const double AccentFontSize = 9.5;
        private const double SignFontSize = 13.0;

        /// <summary>
        /// Creates the level diagram. <paramref name="neutral"/> draws levels and dimensions,
        /// <paramref name="accent"/> draws the picked point and the A/B call-outs.
        /// <paramref name="levelFormat"/> receives the level number as {0}.
        /// </summary>
        public static Canvas Create(Brush neutral, Brush accent, string levelFormat, string basePointLabel)
        {
            var canvas = new Canvas
            {
                Width = DesignWidth,
                Height = DesignHeight,
                Background = Brushes.Transparent,
                SnapsToDevicePixels = false,
            };

            DrawLevels(canvas, neutral, levelFormat);
            DrawHeightDimensions(canvas, neutral);
            DrawDirectionArrows(canvas, accent);
            DrawBasePointAndRail(canvas, neutral, accent, basePointLabel);

            return canvas;
        }

        private static void DrawLevels(Canvas canvas, Brush neutral, string levelFormat)
        {
            for (int i = 0; i < LevelYs.Length; i++)
            {
                double y = LevelYs[i];
                AddLine(canvas, neutral, OuterLeft, y, OuterRight, y);

                string name = "\u25BD" + FormatLevelName(levelFormat, LevelYs.Length - i);
                AddTextAt(canvas, name, CenterX + 5.0, y - 1.0, LevelFontSize, neutral, bold: true);
            }
        }

        // The format comes from a localizable resource; a broken translation must not abort the command.
        private static string FormatLevelName(string levelFormat, int levelNumber)
        {
            if (string.IsNullOrEmpty(levelFormat))
                return levelNumber.ToString(CultureInfo.CurrentCulture);

            try
            {
                return string.Format(CultureInfo.CurrentCulture, levelFormat, levelNumber);
            }
            catch (FormatException)
            {
                return levelFormat;
            }
        }

        private static void DrawHeightDimensions(Canvas canvas, Brush neutral)
        {
            double top = LevelYs[0];
            double middle = LevelYs[1];
            double bottom = LevelYs[2];

            foreach (double sign in new[] { -1.0, 1.0 })
            {
                double inner = sign < 0 ? InnerLeft : InnerRight;
                double outer = sign < 0 ? OuterLeft : OuterRight;
                double rotation = sign < 0 ? -90.0 : 90.0;

                AddVerticalDimension(canvas, neutral, inner, top, middle, "3000", sign, rotation);
                AddVerticalDimension(canvas, neutral, inner, middle, bottom, "3000", sign, rotation);
                AddVerticalDimension(canvas, neutral, outer, top, bottom, "6000", sign, rotation);

                // Extension lines carry both dimension lines down to the A/B rail.
                AddLine(canvas, neutral, inner, bottom, inner, RailY + TickLength / 2.0, thickness: 0.6);
                AddLine(canvas, neutral, outer, bottom, outer, RailY + TickLength / 2.0, thickness: 0.6);
            }
        }

        private static void DrawDirectionArrows(Canvas canvas, Brush accent)
        {
            double y = (LevelYs[0] + LevelYs[1]) / 2.0;

            AddArrow(canvas, accent, CenterX - 12.0, y, -1.0);
            AddArrow(canvas, accent, CenterX + 12.0, y, 1.0);

            AddText(canvas, "\u2212", CenterX - 50.0, y, SignFontSize, accent, bold: true);
            AddText(canvas, "+", CenterX + 50.0, y, SignFontSize, accent, bold: true);
        }

        private static void DrawBasePointAndRail(Canvas canvas, Brush neutral, Brush accent, string basePointLabel)
        {
            AddLine(canvas, accent, CenterX, LevelYs[0] - 14.0, CenterX, RailY, dashed: true, thickness: 1.0);

            AddLine(canvas, accent, OuterLeft, RailY, OuterRight, RailY, thickness: 1.2);
            foreach (double x in new[] { OuterLeft, InnerLeft, CenterX, InnerRight, OuterRight })
                AddLine(canvas, accent, x, RailY - TickLength / 2.0, x, RailY + TickLength / 2.0, thickness: 1.2);

            double labelY = RailY - TextGap;
            AddText(canvas, "B", (OuterLeft + InnerLeft) / 2.0, labelY, AccentFontSize, accent, bold: true);
            AddText(canvas, "A", (InnerLeft + CenterX) / 2.0, labelY, AccentFontSize, accent, bold: true);
            AddText(canvas, "A", (CenterX + InnerRight) / 2.0, labelY, AccentFontSize, accent, bold: true);
            AddText(canvas, "B", (InnerRight + OuterRight) / 2.0, labelY, AccentFontSize, accent, bold: true);

            const double dotRadius = 3.0;
            var dot = new Ellipse { Width = dotRadius * 2.0, Height = dotRadius * 2.0, Fill = accent };
            Canvas.SetLeft(dot, CenterX - dotRadius);
            Canvas.SetTop(dot, RailY - dotRadius);
            canvas.Children.Add(dot);

            AddText(canvas, basePointLabel, CenterX, RailY + 17.0, LevelFontSize, neutral, bold: true);
        }

        private static void AddVerticalDimension(
            Canvas canvas,
            Brush neutral,
            double x,
            double startY,
            double endY,
            string label,
            double side,
            double rotation)
        {
            AddLine(canvas, neutral, x, startY, x, endY);
            AddLine(canvas, neutral, x - TickLength / 2.0, startY, x + TickLength / 2.0, startY);
            AddLine(canvas, neutral, x - TickLength / 2.0, endY, x + TickLength / 2.0, endY);

            AddText(canvas, label, x + side * TextGap, (startY + endY) / 2.0, DimensionFontSize, neutral, rotation: rotation);
        }

        /// <summary>Block arrow whose tail sits at <paramref name="tailX"/>, pointing along <paramref name="direction"/>.</summary>
        private static void AddArrow(Canvas canvas, Brush fill, double tailX, double y, double direction)
        {
            const double shaftLength = 12.0;
            const double headLength = 10.0;
            const double shaftHalf = 3.5;
            const double headHalf = 8.0;

            double neck = tailX + direction * shaftLength;
            double tip = neck + direction * headLength;

            var arrow = new Polygon
            {
                Fill = fill,
                Points = new PointCollection
                {
                    new Point(tailX, y - shaftHalf),
                    new Point(neck, y - shaftHalf),
                    new Point(neck, y - headHalf),
                    new Point(tip, y),
                    new Point(neck, y + headHalf),
                    new Point(neck, y + shaftHalf),
                    new Point(tailX, y + shaftHalf),
                },
            };
            canvas.Children.Add(arrow);
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

        private static TextBlock CreateText(string text, double fontSize, Brush foreground, bool bold)
        {
            var block = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                Foreground = foreground,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                FontFamily = new FontFamily("Segoe UI"),
            };
            block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return block;
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
            TextBlock block = CreateText(text, fontSize, foreground, bold);

            if (rotation != 0.0)
            {
                block.RenderTransformOrigin = new Point(0.5, 0.5);
                block.RenderTransform = new RotateTransform(rotation);
            }

            Canvas.SetLeft(block, centreX - block.DesiredSize.Width / 2.0);
            Canvas.SetTop(block, centreY - block.DesiredSize.Height / 2.0);
            canvas.Children.Add(block);
        }

        /// <summary>Places text with its left edge at <paramref name="left"/> and its bottom at <paramref name="bottom"/>.</summary>
        private static void AddTextAt(
            Canvas canvas,
            string text,
            double left,
            double bottom,
            double fontSize,
            Brush foreground,
            bool bold = false)
        {
            TextBlock block = CreateText(text, fontSize, foreground, bold);
            Canvas.SetLeft(block, left);
            Canvas.SetTop(block, bottom - block.DesiredSize.Height);
            canvas.Children.Add(block);
        }
    }
}
