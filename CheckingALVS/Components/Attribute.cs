using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;
        private readonly ResourceManager _resourceManImage;
        private readonly ResourceManager _enResourceManText;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.CheckingALVS.Resources.Text", assembly);
            _resourceManImage = new ResourceManager("ADSK.JExtRAC.CheckingALVS.Resources.Image", assembly);

            try
            {
                var enCulture = new CultureInfo("en");
                var satAsm = assembly.GetSatelliteAssembly(enCulture);
                _enResourceManText = new ResourceManager(
                    "ADSK.JExtRAC.CheckingALVS.Resources.Text.en",
                    satAsm);
            }
            catch
            {
                _enResourceManText = null;
            }
        }

        public string ResourceText(string resourceId)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;

            if (_enResourceManText != null &&
                culture.TwoLetterISOLanguageName == "en")
            {
                try
                {
                    string enVal = _enResourceManText.GetString(resourceId);
                    if (enVal != null) return enVal;
                }
                catch { }
            }

            return _resourceManText?.GetString(resourceId, culture) ?? "";
        }

        public object ResourceImage(string resourceId)
        {
            return _resourceManImage?.GetObject(resourceId);
        }
    }
}
