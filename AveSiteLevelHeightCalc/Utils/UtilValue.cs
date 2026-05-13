using System;
using System.Collections.Generic;
using System.Globalization;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils
{
    public static class UtilValue
    {
        private static readonly System.Text.Encoding ShiftJis =
            System.Text.CodePagesEncodingProvider.Instance?.GetEncoding("Shift_JIS")
            ?? System.Text.Encoding.UTF8;

        public static IList<string> SplitString(string value, string separator)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(value)) return list;
            foreach (string p in value.Split(new[] { separator }, StringSplitOptions.None))
                list.Add(p);
            return list;
        }

        public static bool IsNull(string value) => string.IsNullOrEmpty(value);

        public static bool IsInteger(string value) => int.TryParse(value?.Trim(), out _);

        public static bool IsNumber(string value) => double.TryParse(value?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        /// <summary>Round value; decimalType/fracttionType mirror legacy JExt UtilValue (decimals ≈ decimalType-1).</summary>
        public static string Rounding(double value, int decimalType, int fractionType)
        {
            int fracDigits = Math.Max(0, decimalType - 1);
            double scaled = FractionAdjust(value, fracDigits, fractionType);
            if (fracDigits <= 0)
                return Math.Round(scaled).ToString(CultureInfo.InvariantCulture);

            string fmt = "F" + Math.Min(15, fracDigits);
            return scaled.ToString(fmt, CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
        }

        private static double FractionAdjust(double value, int fracDigits, int fractionType)
        {
            if (fracDigits <= 0) return Math.Round(value);
            double m = Math.Pow(10.0, fracDigits);
            double v = value * m;

            switch (fractionType)
            {
                case 1: 
                    return Math.Round(v + 1e-9) / m;
                case 2:
                    return Math.Floor(value * m + 1e-9) / m;
                case 3:
                    return Math.Ceiling(value * m - 1e-9) / m;
                default:
                    return Math.Round(v, MidpointRounding.AwayFromZero) / m;
            }
        }

        public static int GetByteCountString(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            try { return ShiftJis.GetByteCount(s); }
            catch { return s.Length; }
        }

        public static string CreateBlankString(int repeat)
        {
            if (repeat <= 0) return string.Empty;
            return new string(' ', repeat);
        }
    }
}
