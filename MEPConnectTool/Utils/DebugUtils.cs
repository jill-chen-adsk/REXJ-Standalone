using System ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Electrical ;

namespace MEPConnectTool.Utils 
{
  public static class DebugUtils
  {
    public static string ToIntXyzString( this XYZ xyz ) => $"({(int)xyz.X},{(int)xyz.Y},{(int)xyz.Z})" ;

    public static string ToF2( this XYZ xyz ) => $"({xyz.X.ToString( "F2" )}, {xyz.Y.ToString( "F2" )}, {xyz.Z.ToString( "F2" )})" ;


    public static string FtToMmF2( this XYZ xyz ) => $"({xyz.X.FtToMm().ToString( "F2" )}, {xyz.Y.FtToMm().ToString( "F2" )}, {xyz.Z.FtToMm().ToString( "F2" )})" ;

    
    public static string I( this XYZ xyz ) => $"({(int)xyz.X},{(int)xyz.Y},{(int)xyz.Z})" ;


    public static void LogVerts( this Wire wire )
    {
      Console.WriteLine( $"== Wire {wire.Name}[{wire.Id}]" ) ;
      for ( var i = wire.NumberOfVertices - 1 ; i >= 0 ; i-- ) {
        Console.WriteLine( $"vert[{i}]: {wire.GetVertex( i )}" ) ;
      }
    }
    
    public static void LogVertsF2( this Wire wire )
    {
      Console.WriteLine( $"== Wire {wire.Name}[{wire.Id}]" ) ;
      for ( var i = wire.NumberOfVertices - 1 ; i >= 0 ; i-- ) {
        Console.WriteLine( $"vert[{i}]: {wire.GetVertex( i ).ToF2()}" ) ;
      }
    }
    
  }
}