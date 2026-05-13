using System;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class Attribute
    {
        private ResourceManager _textRM;
        private ResourceManager _imageRM;
        private string _assemblyFolder;

        public Attribute()
        {
            var asm = Assembly.GetExecutingAssembly();
            _textRM = new ResourceManager("ADSK.JExtRAC.FittingSchedule.Resources.Text", asm);
            _imageRM = new ResourceManager("ADSK.JExtRAC.FittingSchedule.Resources.Image", asm);
            _assemblyFolder = System.IO.Path.GetDirectoryName(asm.Location);
        }

        public string ResourceText(string key)
        {
            try { return _textRM.GetString(key) ?? key; }
            catch { return key; }
        }

        public object ResourceImage(string key)
        {
            try { return _imageRM.GetObject(key); }
            catch { return null; }
        }

        public string AssemblyFolder => _assemblyFolder;
    }
}
