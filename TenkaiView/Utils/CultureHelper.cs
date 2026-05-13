using System;
using System.Globalization;
using System.Threading;
using ADSK.ViewExtension.TenkaiView.Resources;
using Microsoft.Win32;

namespace ADSK.ViewExtension.TenkaiView.Utils
{
    public static class CultureHelper
    {
        public const string LanguageJapanese = "ja";
        public const string LanguageEnglish = "en";

        private const string RegistryKeyPath = @"Software\ADSK.ViewExtension.TenkaiView";

        public static void InitializeCulture()
        {
            string saved = GetSavedLanguage();
            if (!string.IsNullOrEmpty(saved))
                SetCulture(saved);
            else
            {
                string two = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                SetCulture(two == "ja" ? LanguageJapanese : LanguageEnglish);
            }
        }

        public static void SetCulture(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                return;
            try
            {
                CultureInfo culture = new CultureInfo(languageCode);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Text.Culture = culture;
                SaveLanguage(languageCode);
            }
            catch
            {
                SetCulture(LanguageJapanese);
            }
        }

        public static string GetCurrentLanguage() => Thread.CurrentThread.CurrentUICulture.Name;

        private static string GetSavedLanguage()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
                return key?.GetValue("Language") as string ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SaveLanguage(string lang)
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);
                key?.SetValue("Language", lang);
            }
            catch { }
        }

        public static bool IsLanguageSupported(string languageCode) =>
            languageCode == LanguageJapanese || languageCode == LanguageEnglish;
    }
}
