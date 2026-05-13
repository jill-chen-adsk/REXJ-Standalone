using System.Reflection;
using System.Resources;

namespace ADSK.JExtRAC.ValueCopy.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;
        private readonly ResourceManager _resourceManImage;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.ValueCopy.Resources.Text", assembly);
            _resourceManImage = new ResourceManager("ADSK.JExtRAC.ValueCopy.Resources.Image", assembly);
        }

        public string ResourceText(string resourceId)
        {
            var value = _resourceManText?.GetString(resourceId);
            return value ?? "";
        }

        public object ResourceImage(string resourceId)
        {
            return _resourceManImage?.GetObject(resourceId);
        }
    }
}
