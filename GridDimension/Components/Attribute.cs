using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.GridDimension.Components
{
    public class Attribute
    {
        private readonly ResourceManager _textResourceManager;
        private readonly ResourceManager _imageResourceManager;

        public Attribute()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            _textResourceManager = new ResourceManager("ADSK.JExtRAC.GridDimension.Resources.Text", asm);
            _imageResourceManager = new ResourceManager("ADSK.JExtRAC.GridDimension.Resources.Image", asm);
        }

        public string ResourceText(string key)
        {
            return _textResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }

        public object ResourceImage(string key)
        {
            return _imageResourceManager.GetObject(key, Thread.CurrentThread.CurrentUICulture);
        }
    }
}
