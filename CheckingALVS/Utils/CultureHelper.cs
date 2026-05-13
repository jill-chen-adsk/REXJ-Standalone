using System;
using Microsoft.Win32;
using System.Globalization;
using System.Threading;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    public static class CultureHelper
    {
        private const string RegistryPath = @"Software\ADSK.JExtRAC.CheckingALVS";
        private const string LanguageValue = "Language";

        public static void InitializeCulture()
        {
            try
            {
                string lang = RegistryLoad();
                if (string.IsNullOrEmpty(lang))
                {
                    lang = "en";
                }
                SetCulture(lang);
            }
            catch { }
        }

        public static void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName ?? "ja");
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Resources.Text.Culture = culture;
            RegistrySave(culture.Name);
        }

        private static string RegistryLoad()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                    return key?.GetValue(LanguageValue) as string ?? "";
            }
            catch { return ""; }
        }

        private static void RegistrySave(string language)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                    key?.SetValue(LanguageValue, language);
            }
            catch { }
        }
    }
}
