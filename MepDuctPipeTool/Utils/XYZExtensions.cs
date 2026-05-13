using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class XYZExtensions
  {
    public const double AngleToleranceDegree = 1e-6;

    public static bool IsSameDirectionTo( this XYZ vec1, XYZ vec2 )
    {
      return vec1.AngleTo( vec2 ) < AngleToleranceDegree;
    }

    public static bool IsOppositeDirectionTo( this XYZ vec1, XYZ vec2 )
    {
      if ( ! vec1.IsParallelTo( vec2 ) ) return false;
      return vec1.DotProduct( vec2 ) <= 0;
    }

    public static bool IsParallelTo( this XYZ vec1, XYZ vec2 )
    {
      return vec1.CrossProduct( vec2 ).GetLength() < AngleToleranceDegree;
    }
  }
}