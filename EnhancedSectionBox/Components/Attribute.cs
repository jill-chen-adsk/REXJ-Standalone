using System.Reflection;
using System.Resources;

namespace ADSK.JExtRAC.EnhancedSectionBox.Components
{
    public class Attribute
    {
        private readonly ResourceManager _resourceManText;

        public Attribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _resourceManText = new ResourceManager("ADSK.JExtRAC.EnhancedSectionBox.Resources.Text", assembly);
        }

        public string ResourceText(string resourceId)
        {
            var value = _resourceManText?.GetString(resourceId);
            return value ?? "";
        }
    }
}
