using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
{
    public class Attribute
    {
        private readonly ResourceManager _textResourceManager;
        private readonly ResourceManager _imageResourceManager;

        public Attribute()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            _textResourceManager = new ResourceManager("ADSK.JExtRAC.FloorHeightDimension.Resources.Text", asm);
            _imageResourceManager = new ResourceManager("ADSK.JExtRAC.FloorHeightDimension.Resources.Image", asm);
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
