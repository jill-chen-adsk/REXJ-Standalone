using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public static class UtilValue
    {
        public static IList<string> SplitString(string value, params char[] separators)
        {
            if (string.IsNullOrEmpty(value))
                return new List<string>();
            return value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static IList<string> SplitString(string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                return new List<string>();
            if (string.IsNullOrEmpty(separator))
                return new List<string> { value };
            if (separator.Length == 1)
                return SplitString(value, separator[0]);
            return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static bool IsNull(string s) => string.IsNullOrEmpty(s);

        public static bool IsInteger(string s) => int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out _);

        public static bool IsNumber(string s) => double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out _);

        public static int GetNextIndex(int currentIndex, int count)
        {
            if (count <= 0) return 0;
            return (currentIndex + 1) % count;
        }

        /// <param name="digits">Displayed decimal digits / precision index from UI.</param>
        /// <param name="fractionType">0 = floor (truncate), 1 = ceiling, 2 = round (away from zero).</param>
        public static string Rounding(double value, int digits, int fractionType)
        {
            double scale = Math.Pow(10, Math.Max(0, digits));
            double scaled = value * scale;
            double r = fractionType switch
            {
                0 => scaled >= 0 ? Math.Floor(scaled) : Math.Ceiling(scaled),
                1 => scaled >= 0 ? Math.Ceiling(scaled) : Math.Floor(scaled),
                _ => Math.Round(scaled, MidpointRounding.AwayFromZero)
            };
            double result = r / scale;
            return result.ToString(CultureInfo.CurrentCulture);
        }
    }
}
