using Autodesk.Revit.DB;
using MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Geometry
{
  public static class PipeAccessorySizeCalculator
  {
    public static (Length inputToOriginLength, Length outputToOriginLength) CalcLengthFromOriginToEnd( AccessoryPlacementResult accessory )
    {
      var inputToOriginLength = CalcLengthFromOriginToConnector( accessory.Accessory, accessory.InputSideConnector );
      var outputToOriginLength = CalcLengthFromOriginToConnector( accessory.Accessory, accessory.OutputSideConnector );

      return ( inputToOriginLength, outputToOriginLength );
    }

    private static Length CalcLengthFromOriginToConnector( FamilyInstance familyInstance, Connector targetConnector )
    {
      var origin = familyInstance.GetLocationPoint().Point;
      return ( origin - targetConnector.Origin ).GetLength().RevitUnitsToLength();
    }
  }
}