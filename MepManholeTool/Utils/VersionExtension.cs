using Autodesk.Revit.DB ;
namespace MepManholeTool.Utils
{
  public static class VersionExtension
  {
    public static long ValueEx( this ElementId id )
    {
            return id.Value ;
    }

    public static ElementId NewElementIdEx(long value)
    {
            return new ElementId( value ) ;
    }
  }
}