using System;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace ADSK.JExtRAC.LocateSlab.Components
{
    public class Attribute
    {
        private readonly ResourceManager _textRM;
        private readonly ResourceManager _imageRM;

        public Attribute()
        {
            _textRM = new ResourceManager("ADSK.JExtRAC.LocateSlab.Resources.Text", Assembly.GetExecutingAssembly());
            _imageRM = null;
        }

        public string ResourceText(string key)
        {
            try { return _textRM.GetString(key) ?? key; }
            catch { return key; }
        }

        public object ResourceImage(string key)
        {
            try { return _imageRM?.GetObject(key); }
            catch { return null; }
        }
    }
}
