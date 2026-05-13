using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public class Attribute
    {
        private readonly ResourceManager _textResourceManager;
        private readonly ResourceManager _imageResourceManager;
        private readonly ResourceManager _enTextResourceManager;

        public Attribute()
        {
            var asm = Assembly.GetExecutingAssembly();
            _textResourceManager = new ResourceManager("ADSK.JExtRAC.AreaSchedule.Resources.Text", asm);
            _imageResourceManager = new ResourceManager("ADSK.JExtRAC.AreaSchedule.Resources.Image", asm);

            try
            {
                var enCulture = new CultureInfo("en");
                var satAsm = asm.GetSatelliteAssembly(enCulture);
                _enTextResourceManager = new ResourceManager(
                    "ADSK.JExtRAC.AreaSchedule.Resources.Text.en",
                    satAsm);
            }
            catch
            {
                _enTextResourceManager = null;
            }
        }

        public string ResourceText(string key)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;

            if (_enTextResourceManager != null &&
                culture.TwoLetterISOLanguageName == "en")
            {
                try
                {
                    string enVal = _enTextResourceManager.GetString(key);
                    if (enVal != null) return enVal;
                }
                catch { }
            }

            return _textResourceManager.GetString(key, culture) ?? key;
        }

        public object ResourceImage(string key)
        {
            return _imageResourceManager.GetObject(key, Thread.CurrentThread.CurrentUICulture);
        }
    }
}
