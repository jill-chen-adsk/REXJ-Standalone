using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SectionListSteel.JExtComCompat
{
    public static class UtilValue
    {
        public static IList<string> SplitString(string value, string separator)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(value))
            {
                return list;
            }
            foreach (string part in value.Split(new[] { separator }, StringSplitOptions.None))
            {
                list.Add(part);
            }
            return list;
        }

        public static string Rounding(double value, int totalDigits, int decimalDigits)
        {
            return Math.Round(value, decimalDigits, MidpointRounding.AwayFromZero).ToString(
                "F" + decimalDigits,
                CultureInfo.InvariantCulture);
        }

        public static bool IsNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        public static bool IsInteger(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        public static int GetByteCountString(string s)
        {
            var enc = Encoding.GetEncoding("shift_jis");
            return enc.GetByteCount(s ?? "");
        }

        public static string CreateBlankString(int length)
        {
            if (length <= 0)
            {
                return "";
            }
            return new string(' ', length);
        }
    }
}
