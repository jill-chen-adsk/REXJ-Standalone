using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Geometry
{
  internal static class XYZExtensions
  {
    internal static bool IsBetween( this XYZ point, XYZ startPoint, XYZ endPoint )
    {
      var startToEnd = endPoint - startPoint;
      var startToPoint = point - startPoint;

      return IsBetween( startToEnd, startToPoint );
    }

    internal static bool IsBetweenWithMargin( this XYZ point, XYZ startPoint, XYZ endPoint, double margin )
    {
      var startToEnd = endPoint - startPoint;

      var correctedStart = startPoint + startToEnd.Normalize() * margin;
      var correctedEnd = endPoint - startToEnd.Normalize() * margin;
      var correctedStartToEnd = correctedEnd - correctedStart;
      var correctedStartToPoint = point - correctedStart;

      return IsBetween( correctedStartToEnd, correctedStartToPoint );
    }

    private static bool IsBetween( XYZ startToEndVec, XYZ startToPointVec )
    {
      var dotProduct = startToPointVec.DotProduct( startToEndVec );
      var lengthSquared = startToEndVec.DotProduct( startToEndVec );

      // 0 < 内積  < startToEndの長さの二乗 であれば間にあると判定
      // 内積 == 0 または 内積 == lengthSquaredの場合、つまりstartPoint, endPoint上にある場合はFalse
      return 0.0 < dotProduct && dotProduct < lengthSquared;
    }
  }
}