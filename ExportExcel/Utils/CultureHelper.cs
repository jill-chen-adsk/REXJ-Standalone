using System;
using System.Globalization;
using System.Threading;
namespace ADSK.JExtRAC.ExportExcel.Utils
{
    public static class CultureHelper
    {
        public const string LanguageJapanese = "ja";
        public const string LanguageEnglish = "en";

        private const string LanguageSettingKey = "Language";
        private const string LanguageSettingSection = "Globalization";

        public static void InitializeCulture()
        {
            string savedLanguage = GetSavedLanguage();

            if (!string.IsNullOrEmpty(savedLanguage))
            {
                SetCulture(savedLanguage);
            }
            else
            {
                CultureInfo systemCulture = Thread.CurrentThread.CurrentUICulture;
                string twoLetter = systemCulture.TwoLetterISOLanguageName;

                if (twoLetter == "ja")
                    SetCulture(LanguageJapanese);
                else
                    SetCulture(LanguageEnglish);
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

                Resources.Text.Culture = culture;

                SaveLanguage(languageCode);
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                SetCulture(LanguageJapanese);
            }
        }

        public static string GetCurrentLanguage()
        {
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        private static string GetSavedLanguage()
        {
            string language = string.Empty;
            Setting.LoadSetting(LanguageSettingSection, LanguageSettingKey, ref language);
            return language;
        }

        private static void SaveLanguage(string languageCode)
        {
            Setting.SaveSetting(LanguageSettingSection, LanguageSettingKey, languageCode);
        }

        public static bool IsLanguageSupported(string languageCode)
        {
            return languageCode == LanguageJapanese || languageCode == LanguageEnglish;
        }
    }
}
