using System.Reflection;

namespace MepDuctPipeTool.Utils
{
  public static class ResourceUtils
  {
    internal static string GetHelpPath()
    {
      var assemblyPath = Assembly.GetExecutingAssembly().Location;
      return $"{System.IO.Path.GetDirectoryName( assemblyPath )}" + "\\" + Resources.HELP_FILE_NAME;
    }
  }
}