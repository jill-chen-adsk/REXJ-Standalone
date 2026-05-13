using System.Globalization;
using System.Threading;
using Microsoft.Win32;

namespace ADSK.JExtRAC.FittingSchedule
{
    internal static class CultureHelper
    {
        private const string RegPath = @"Software\ADSK\REXJ\Standalone";
        private const string RegKey = "Language";

        internal static void ApplySavedCulture()
        {
            string lang = GetSavedLanguage();
            if (!string.IsNullOrEmpty(lang))
            {
                try
                {
                    var ci = new CultureInfo(lang);
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
                catch { }
            }
        }

        private static string GetSavedLanguage()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegPath);
                return key?.GetValue(RegKey) as string;
            }
            catch { return null; }
        }
    }
}
