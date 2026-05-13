using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.ExportExcel.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;
        private readonly ResourceManager _resourceManImage;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.ExportExcel.Resources.Text", assembly);
            _resourceManImage = new ResourceManager("ADSK.JExtRAC.ExportExcel.Resources.Image", assembly);
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
