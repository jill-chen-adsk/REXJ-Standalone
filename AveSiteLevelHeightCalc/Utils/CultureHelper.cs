using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils
{
    public static class CultureHelper
    {
        private const string RegistryKeyPath = @"Software\ADSK.JExtRAC.AveSiteLevelHeightCalc";

        public static void InitializeCulture()
        {
            string savedLanguage = GetSavedLanguage();
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                SetCulture(savedLanguage);
            }
            else
            {
                SetCulture("en");
            }

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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
                    return key?.GetValue("Language") as string ?? "";
            }
            catch { return ""; }
        }

        private static void SaveLanguage(string lang)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
                    key?.SetValue("Language", lang);
            }
            catch { }
        }
    }
}
