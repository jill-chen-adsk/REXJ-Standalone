using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Geometry
{
  internal static class PickPositionEvaluator
  {
    internal static bool IsPickPositionBeyondPipeEnd( XYZ position, Pipe pipe, double marginRatio )
    {
      var line = pipe.GetLine();
      var startPoint = line.GetEndPoint( 0 );
      var endPoint = line.GetEndPoint( 1 );
      var margin = pipe.Diameter * marginRatio;
      return ! position.IsBetweenWithMargin( startPoint, endPoint, margin );
    }
  }
}