using System.IO;
using System.Reflection;
using JExtComCompat;

namespace RSTExtension.Components
{
    public class Attribute : UtilAttrib
    {
        public Attribute() : base()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            SetAssembly(
                Assembly.GetExecutingAssembly(),
                "RSTExtension.Resources.Text",
                "RSTExtension.Resources.Image",
                dir);
        }
    }
}
