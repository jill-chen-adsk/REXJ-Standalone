using Microsoft.Win32;
using System.Globalization;
using System.Threading;

namespace ADSK.JExtRAC.LayoutInstanceInRegion.Utils
{
    public static class CultureHelper
    {
        private const string RegistryPath = @"Software\VB and VBA Program Settings\ADSK.JExtRAC.LayoutInstanceInRegion\Globalization";
        private const string RegistryKey = "Language";

        public static void InitializeCulture()
        {
            string lang = LoadLanguagePreference();
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

        public static void SetCulture(string cultureName)
        {
            try
            {
                var ci = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = ci;
                SaveLanguagePreference(cultureName);
            }
            catch { }
        }

        public static void SaveLanguagePreference(string cultureName)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    key?.SetValue(RegistryKey, cultureName);
                }
            }
            catch { }
        }

        public static string LoadLanguagePreference()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    return key?.GetValue(RegistryKey) as string;
                }
            }
            catch { return null; }
        }
    }
}
