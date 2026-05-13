using Microsoft.Win32;
using System.Globalization;
using System.Threading;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    public static class CultureHelper
    {
        private const string RegistryPath = @"Software\VB and VBA Program Settings\ADSK.JExtRAC.AutoLayoutTag\Globalization";
        private const string RegistryKey = "Language";

        public static void InitializeCulture()
        {
            string savedLang = LoadLanguagePreference();
            if (savedLang != null)
            {
                SetCulture(savedLang);
                return;
            }
            var systemCulture = CultureInfo.InstalledUICulture;
            string lang = systemCulture.TwoLetterISOLanguageName == "ja" ? "ja" : "en";
            SetCulture(lang);
            SaveLanguagePreference(lang);
        }

        public static void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = culture;
            Resources.Text.Culture = culture;
        }

        public static void SaveLanguagePreference(string language)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    key?.SetValue(RegistryKey, language);
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
