using System.Globalization;
using System.Threading;
using Microsoft.Win32;

namespace ADSK.JExtRAC.LocateSlab
{
    public static class CultureHelper
    {
        private const string RegKey = @"SOFTWARE\ADSK\JExtRAC\LocateSlab";
        private const string RegValue = "Language";

        public static void ApplySavedCulture()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegKey);
                if (key?.GetValue(RegValue) is string lang && !string.IsNullOrEmpty(lang))
                {
                    var ci = new CultureInfo(lang);
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
            }
            catch { }
        }

        public static void SaveCulture(string lang)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RegKey);
                key?.SetValue(RegValue, lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            catch { }
        }
    }
}
