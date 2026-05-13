using System;
using System.Globalization;
using System.Threading;
using Microsoft.Win32;

namespace ADSK.JExtRAC.ExportSchedule.Utils
{
    public static class CultureHelper
    {
        private const string RegistryKeyPath = @"Software\ADSK.JExtRAC.ExportSchedule";

        public static void InitializeCulture()
        {
            string savedLanguage = GetSavedLanguage();
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                SetCulture(savedLanguage);
            }
            else
            {
                string twoLetter = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                SetCulture(twoLetter == "ja" ? "ja" : "en");
            }
        }

        public static void SetCulture(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return;
            try
            {
                CultureInfo culture = new CultureInfo(languageCode);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Resources.Text.Culture = culture;
                SaveLanguage(languageCode);
            }
            catch { SetCulture("ja"); }
        }

        private static string GetSavedLanguage()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
                {
                    return key?.GetValue("Language") as string ?? "";
                }
            }
            catch { return ""; }
        }

        private static void SaveLanguage(string lang)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
                {
                    key?.SetValue("Language", lang);
                }
            }
            catch { }
        }
    }
}
