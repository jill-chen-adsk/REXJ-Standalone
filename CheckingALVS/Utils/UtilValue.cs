using System;
using System.Collections.Generic;
using System.Globalization;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Replaces DnfCom.UtilValue.</summary>
    public static class UtilValue
    {
        public static IList<string> SplitString(string value, string separator)
        {
            if (value == null) return new List<string>();
            char sep = string.IsNullOrEmpty(separator) ? ',' : separator[0];
            string[] parts = value.Split(sep);
            var list = new List<string>();
            foreach (var p in parts)
            {
                list.Add((p ?? "").Trim());
            }
            return list;
        }

        public static bool IsNull(object value)
        {
            return value == null || (value is string s && string.IsNullOrWhiteSpace(s));
        }

        public static bool IsBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return bool.TryParse(value.Trim(), out _);
        }

        public static bool IsInteger(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        public static bool IsNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>Round numeric string or double; roundingMode maps to midpoint rules (AwayFromZero / ToEven).</summary>
        public static string Rounding(object value, int decimals, int roundingMode)
        {
            if (value == null) return "";
            MidpointRounding mode = roundingMode == 2 ? MidpointRounding.ToEven : MidpointRounding.AwayFromZero;
            if (value is string s && double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
            {
                return Math.Round(d, decimals, mode).ToString(CultureInfo.InvariantCulture);
            }
            if (value is double du)
                return Math.Round(du, decimals, mode).ToString(CultureInfo.InvariantCulture);
            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
                return Math.Round(x, decimals, mode).ToString(CultureInfo.InvariantCulture);
            return value.ToString();
        }
    }
}
