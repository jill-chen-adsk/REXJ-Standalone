using System;
using System.Collections.Generic;

namespace ADSK.Ext.Fukashi.Utils
{
    internal static class UtilValue
    {
        internal static IList<string> SplitString(string input, string separator)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(input))
                return result;
            string[] parts = input.Split(new string[] { separator }, StringSplitOptions.None);
            foreach (string part in parts)
                result.Add(part);
            return result;
        }

        internal static bool IsInteger(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return int.TryParse(value, out _);
        }

        internal static bool IsNumber(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return double.TryParse(value, out _);
        }
    }
}
