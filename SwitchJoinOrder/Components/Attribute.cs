using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.SwitchJoinOrder.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;
        private readonly ResourceManager _resourceManImage;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.SwitchJoinOrder.Resources.Text", assembly);
            _resourceManImage = new ResourceManager("ADSK.JExtRAC.SwitchJoinOrder.Resources.Image", assembly);
        }

        public string ResourceText(string resourceId)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            var value = _resourceManText?.GetString(resourceId, culture);
            return value ?? "";
        }

        public object ResourceImage(string resourceId)
        {
            return _resourceManImage?.GetObject(resourceId);
        }
    }
}
