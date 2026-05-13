using System.Collections.Generic;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public static class UtilValue
    {
        public static bool IsNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsInteger(string value)
        {
            return int.TryParse(value, out _);
        }

        public static IList<string> SplitString(string value, string separator)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(value)) return result;
            var parts = value.Split(new[] { separator }, System.StringSplitOptions.None);
            result.AddRange(parts);
            return result;
        }
    }
}
