using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ADSK.JExtRAC.PrintRegion.Utils
{
    internal static class ViewScaleHelper
    {
        static readonly (int Value, string Name)[] MetricPresets =
        {
            (1, "1:1"),
            (2, "1:2"),
            (5, "1:5"),
            (10, "1:10"),
            (20, "1:20"),
            (25, "1:25"),
            (50, "1:50"),
            (100, "1:100"),
            (200, "1:200"),
            (500, "1:500"),
            (1000, "1:1000"),
            (2000, "1:2000"),
            (5000, "1:5000"),
        };

        static readonly (int Value, string Name)[] ImperialPresets =
        {
            (768, "1/64\" = 1'-0\""),
            (384, "1/32\" = 1'-0\""),
            (192, "1/16\" = 1'-0\""),
            (96, "1/8\" = 1'-0\""),
            (64, "3/16\" = 1'-0\""),
            (48, "1/4\" = 1'-0\""),
            (32, "3/8\" = 1'-0\""),
            (24, "1/2\" = 1'-0\""),
            (16, "3/4\" = 1'-0\""),
            (12, "1\" = 1'-0\""),
            (8, "1 1/2\" = 1'-0\""),
            (4, "3\" = 1'-0\""),
            (2, "6\" = 1'-0\""),
            (1200, "1\" = 100'-0\""),
            (720, "1\" = 60'-0\""),
            (600, "1\" = 50'-0\""),
            (480, "1\" = 40'-0\""),
            (360, "1\" = 30'-0\""),
            (240, "1\" = 20'-0\""),
            (120, "1\" = 10'-0\""),
        };

        public static bool IsImperial(Document document)
        {
            if (document == null)
                return false;

            try
            {
                return document.DisplayUnitSystem == DisplayUnit.IMPERIAL;
            }
            catch
            {
                return false;
            }
        }

        public static DataTable CreateScaleDataTable(bool isImperial)
        {
            var scaleData = new DataTable();
            scaleData.Columns.Add("Name", typeof(string));
            scaleData.Columns.Add("Value", typeof(int));

            foreach ((int value, string name) in isImperial ? ImperialPresets : MetricPresets)
            {
                DataRow row = scaleData.NewRow();
                row["Name"] = name;
                row["Value"] = value;
                scaleData.Rows.Add(row);
            }

            return scaleData;
        }

        public static string FormatScaleDisplay(Document document, View view, int scale, bool isImperial)
        {
            if (scale <= 0)
                return isImperial ? string.Empty : "1:1";

            foreach ((int value, string name) in isImperial ? ImperialPresets : MetricPresets)
            {
                if (value == scale)
                    return name;
            }

            string formatted = TryFormatWithRevit(document, view, scale);
            if (!string.IsNullOrWhiteSpace(formatted))
                return formatted;

            return isImperial ? FormatImperialScale(scale) : $"1:{scale}";
        }

        static string TryFormatWithRevit(Document document, View view, int scale)
        {
            if (document == null || view == null || !view.IsValidObject)
                return null;

            try
            {
                using (var transaction = new Transaction(document, "Format scale"))
                {
                    transaction.Start();
                    int originalScale = view.Scale;
                    view.Scale = scale;
                    document.Regenerate();
                    string formatted = view.get_Parameter(BuiltInParameter.VIEW_SCALE)?.AsValueString();
                    view.Scale = originalScale;
                    transaction.RollBack();
                    return string.IsNullOrWhiteSpace(formatted) ? null : formatted;
                }
            }
            catch
            {
                return null;
            }
        }

        static string FormatImperialScale(int scale)
        {
            if (scale <= 0)
                return string.Empty;

            double paperInchesForOneFoot = 12.0 / scale;
            if (paperInchesForOneFoot >= 0.01 && paperInchesForOneFoot <= 12.0 &&
                TryFormatInchesAsFraction(paperInchesForOneFoot, out string inchText))
            {
                return $"{inchText} = 1'-0\"";
            }

            if (scale % 12 == 0)
            {
                int feet = scale / 12;
                return $"1\" = {feet}'-0\"";
            }

            return $"1:{scale}";
        }

        static bool TryFormatInchesAsFraction(double inches, out string text)
        {
            text = null;
            const double tolerance = 0.001;

            int[] denominators = { 1, 2, 4, 8, 16, 32, 64 };
            foreach (int denominator in denominators)
            {
                double scaled = inches * denominator;
                int rounded = (int)Math.Round(scaled);
                if (Math.Abs(scaled - rounded) > tolerance || rounded <= 0)
                    continue;

                int whole = rounded / denominator;
                int numerator = rounded % denominator;

                if (numerator == 0)
                    text = $"{whole}\"";
                else if (whole == 0)
                    text = $"{numerator}/{denominator}\"";
                else
                    text = $"{whole} {numerator}/{denominator}\"";

                return true;
            }

            return false;
        }

        public static bool TryParseScale(string text, bool isImperial, out int scale)
        {
            scale = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();

            Match ratioMatch = Regex.Match(text, @"^\s*1\s*:\s*(\d+)\s*$");
            if (ratioMatch.Success &&
                int.TryParse(ratioMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out scale) &&
                scale > 0)
            {
                return true;
            }

            if (!isImperial)
                return false;

            Match feetMatch = Regex.Match(text, @"^\s*1\s*""\s*=\s*(\d+)\s*'\s*-?\s*0?\s*""?\s*$");
            if (feetMatch.Success &&
                int.TryParse(feetMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int feet) &&
                feet > 0)
            {
                scale = feet * 12;
                return scale > 0;
            }

            Match inchMatch = Regex.Match(text, @"^\s*(?:(\d+)\s+)?(\d+)\s*/\s*(\d+)\s*""?\s*=\s*1\s*'\s*-?\s*0?\s*""?\s*$");
            if (inchMatch.Success &&
                int.TryParse(inchMatch.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numerator) &&
                int.TryParse(inchMatch.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int denominator) &&
                numerator > 0 && denominator > 0)
            {
                int whole = 0;
                if (!string.IsNullOrEmpty(inchMatch.Groups[1].Value))
                {
                    if (!int.TryParse(inchMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out whole) || whole < 0)
                        return false;
                }

                double inches = whole + (double)numerator / denominator;
                if (inches <= 0)
                    return false;

                scale = (int)Math.Round(12.0 / inches);
                return scale > 0;
            }

            return false;
        }

        public static bool AllowsScaleInputChar(char keyChar, string currentText, bool isImperial)
        {
            if (char.IsControl(keyChar))
                return true;

            if (char.IsDigit(keyChar))
                return true;

            if (!isImperial)
                return keyChar == ':';

            return keyChar == ':' ||
                   keyChar == '/' ||
                   keyChar == '"' ||
                   keyChar == '\'' ||
                   keyChar == '-' ||
                   keyChar == ' ' ||
                   keyChar == '=';
        }
    }
}
