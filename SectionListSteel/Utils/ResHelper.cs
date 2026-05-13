using System ;
using System.Drawing;
using System.Reflection;
namespace SectionListSteel.Utils
{
  public class ResHelper
  {
    public static Image GetEmbeddedImage(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      using var stream = assembly.GetManifestResourceStream(resourceName);
      if (stream == null)
      {
        throw new Exception();
      }
      return Image.FromStream(stream);
    }
  }
}