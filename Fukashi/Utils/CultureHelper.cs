using System.Globalization;
using System.Threading;

namespace ADSK.Ext.Fukashi.Utils
{
    internal static class CultureHelper
    {
        internal static void SetToEnglish()
        {
            var culture = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
