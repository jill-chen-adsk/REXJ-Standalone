using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.JExtRAC.ImportExcel.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.ImportExcel.Resources.Text", assembly);
        }

        public string ResourceText(string resourceId)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            var value = _resourceManText?.GetString(resourceId, culture);
            return value ?? "";
        }
    }
}
