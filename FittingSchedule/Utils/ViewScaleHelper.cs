using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ADSK.JExtRAC.FittingSchedule.Utils
{
    internal static class ViewScaleHelper
    {
        public const int ScaleMin = 1;
        public const int ScaleMax = 9999;

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

        public static string FormatScale(int scale, bool isImperial)
        {
            if (scale <= 0)
            {
                return isImperial ? string.Empty : "1:1";
            }

            foreach ((int value, string name) in isImperial ? ImperialPresets : MetricPresets)
            {
                if (value == scale)
                {
                    return name;
                }
            }

            return isImperial ? FormatImperialScale(scale) : $"1:{scale}";
        }

        static string FormatImperialScale(int scale)
        {
            if (scale <= 0)
            {
                return string.Empty;
            }

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
                {
                    continue;
                }

                int whole = rounded / denominator;
                int numerator = rounded % denominator;

                if (numerator == 0)
                {
                    text = $"{whole}\"";
                }
                else if (whole == 0)
                {
                    text = $"{numerator}/{denominator}\"";
                }
                else
                {
                    text = $"{whole} {numerator}/{denominator}\"";
                }

                return true;
            }

            return false;
        }

        public static bool IsValidScale(int scale)
        {
            return scale >= ScaleMin && scale <= ScaleMax;
        }

        public static bool TryParseScale(string text, bool isImperial, out int scale)
        {
            scale = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = text.Trim();

            Match ratioMatch = Regex.Match(text, @"^\s*1\s*:\s*(\d+)\s*$");
            if (ratioMatch.Success &&
                int.TryParse(ratioMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ratioScale) &&
                TryAcceptScale(ratioScale, out scale))
            {
                return true;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int integerScale) &&
                TryAcceptScale(integerScale, out scale))
            {
                return true;
            }

            if (!isImperial)
            {
                return false;
            }

            Match feetMatch = Regex.Match(text, @"^\s*1\s*""\s*=\s*(\d+)\s*'\s*-?\s*0?\s*""?\s*$");
            if (feetMatch.Success &&
                int.TryParse(feetMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int feet) &&
                feet > 0)
            {
                if (feet > ScaleMax / 12)
                {
                    return false;
                }

                return TryAcceptScale(feet * 12, out scale);
            }

            Match inchMatch = Regex.Match(text, @"^\s*(?:(\d+)\s+)?(\d+)\s*/\s*(\d+)\s*""?\s*(?:=\s*1\s*'\s*-?\s*0?\s*""?\s*)?$");
            if (inchMatch.Success &&
                int.TryParse(inchMatch.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numerator) &&
                int.TryParse(inchMatch.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int denominator) &&
                numerator > 0 && denominator > 0)
            {
                int whole = 0;
                if (!string.IsNullOrEmpty(inchMatch.Groups[1].Value))
                {
                    if (!int.TryParse(inchMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out whole) || whole < 0)
                    {
                        return false;
                    }
                }

                double inches = whole + (double)numerator / denominator;
                if (inches <= 0)
                {
                    return false;
                }

                int computedScale = (int)Math.Round(12.0 / inches);
                return TryAcceptScale(computedScale, out scale);
            }

            Match wholeInchMatch = Regex.Match(text, @"^\s*(\d+)\s*""?\s*(?:=\s*1\s*'\s*-?\s*0?\s*""?\s*)?$");
            if (wholeInchMatch.Success &&
                int.TryParse(wholeInchMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int wholeInches) &&
                wholeInches > 0)
            {
                int computedScale = (int)Math.Round(12.0 / wholeInches);
                return TryAcceptScale(computedScale, out scale);
            }

            return false;
        }

        static bool TryAcceptScale(int candidate, out int scale)
        {
            if (!IsValidScale(candidate))
            {
                scale = 0;
                return false;
            }

            scale = candidate;
            return true;
        }
    }
}

