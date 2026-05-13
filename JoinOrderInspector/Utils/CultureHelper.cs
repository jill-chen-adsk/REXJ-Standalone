using System;
using System.Globalization;
using System.Threading;

namespace ADSK.JExtRAC.JoinOrderInspector.Utils
{
    public static class CultureHelper
    {
        public const string LanguageJapanese = "ja";
        public const string LanguageEnglish = "en";

        public static void InitializeCulture()
        {
            CultureInfo systemCulture = Thread.CurrentThread.CurrentUICulture;
            string twoLetter = systemCulture.TwoLetterISOLanguageName;

            if (twoLetter == "ja")
                SetCulture(LanguageJapanese);
            else
                SetCulture(LanguageEnglish);
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
            }
            catch (Exception)
            {
            }
        }
    }
}
